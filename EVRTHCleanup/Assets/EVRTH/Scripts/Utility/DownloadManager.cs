using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using EVRTH.Scripts.GlobeNS;
using EVRTH.Scripts.Visualization;
using UnityEngine;
using UnityEngine.Networking;

namespace EVRTH.Scripts.Utility
{
    public class DownloadManager : MonoBehaviour
    {
        public int requestsPerFrame = 2;
        public int tileTextureLoadsPerFrame = 2;

        public readonly int numQueues = Globe.MaxLayers + 1; // One queue for each layer plus a default layer
        private int nextQueue;
        private readonly List<Queue<DownloadTask>> downloadQueues = new List<Queue<DownloadTask>>();

        private readonly Queue<DownloadTask> globeTileTexturesToLoadQueue = new Queue<DownloadTask>();

        private readonly Dictionary<string, DownloadStatus> downloadRequests = new Dictionary<string, DownloadStatus>();
        private readonly Dictionary<string, DownloadTask> globeTileQueueLookup = new Dictionary<string, DownloadTask>();

        private Globe globe;
        private TileTextureCache textureCache;

        public int tileQueueSize;
        public int globeTileQueueSize;
        public int ongoingTextureLoadCount;
        public int ongoingDownloadCount;
        public int maxOngoingDownloads;

        public bool useDownloadHandlerGetContent;
        public float globalMipMapBias;
        public int globalAnisoLevel;

        public bool initialized;
        public bool linearTextures = true;
        public bool cacheTextures;

        private void Awake()
        {
            if (!initialized)
            {
                Init();
            }
        }

        private void Init()
        {
            for (int i = 0; i < numQueues; i++)
            {
                downloadQueues.Add(new Queue<DownloadTask>());
            }

            ongoingTextureLoadCount = 0;
            ongoingDownloadCount = 0;
            globe = GetComponent<Globe>();
            textureCache = globe != null ? globe.tileTextureCache ?? GetComponent<TileTextureCache>() : GetComponent<TileTextureCache>();
		
            initialized = true;
        }

        private void Update()
        {
            int totalQueueSize = 0;
            for (int i = 0; i < downloadQueues.Count; i++)
            {
                totalQueueSize += downloadQueues[i].Count;
            }
            globeTileQueueSize = totalQueueSize;

            ProcessDownloadQueues();
            ProcessTextureQueue();
        }

        /// <summary>
        /// Request a texture download. The download will be processed asynchronously. If there is
        /// already a request in progress for this URL a second request will *not* be queued.
        /// </summary>
        /// <param name="url">URL to download.</param>
        /// <param name="layerNameToSet">Layer identifier for this request.</param>
        /// <param name="dateTimeToSet">Date of this request.</param>
        /// <param name="handler">Callback that will be invoked when the texture is ready.</param>
        /// <param name="prepareForRendering">True if the texture will be used for rendering. If
        /// true mipmaps will be generated and an aniso bias set. If false the texture will be
        /// loaded and returned with no further processing.</param>
        /// <param name="queueNum">The download queue to use for this request. Queues are identified
        /// by index, and all downloads in a queue can be cancelled without affecting other
        /// queues.</param>
        public void RequestTexture(string url, string layerNameToSet, DateTime dateTimeToSet,
            TextureDownloadHandler handler, bool prepareForRendering = true, int queueNum = 0)
        {
            //Debug.Log("Request to download " + url);

            if (queueNum < 0 || queueNum >= downloadQueues.Count)
            {
                throw new ArgumentException("No such download queue " + queueNum);
            }

            DownloadStatus status;
            bool haveEntry = downloadRequests.TryGetValue(url, out status);

            bool requestInProgress = haveEntry && status == DownloadStatus.InProgress;
            if (!requestInProgress)
            {
                DownloadTask globeTileLayerSet = new DownloadTask
                {
                    url = url,
                    layerName = layerNameToSet,
                    date = dateTimeToSet,
                    handler = handler,
                    prepareTextureForRendering = prepareForRendering
                };

                bool texCached = IsTextureInCache (globeTileLayerSet);
                if (texCached)
                {
                    //Debug.Log("CACHED FILE EXISTS in StreamingAssets: " + globeTileLayerSet.localFilePath);
                    if (globeTileLayerSet.usingDxtFormat)
                    {
                        // Import DXT file
                        Texture2D myTexture = DDSTextureLoader.LoadTextureDxt (globeTileLayerSet.localFilePath, TextureFormat.DXT1);
                        globeTileLayerSet.texture = myTexture;
                    }
                    else
                    {
                        Texture2D myTexture = new Texture2D (2, 2);
                        myTexture.LoadImage (File.ReadAllBytes (globeTileLayerSet.localFilePath));
                        globeTileLayerSet.texture = myTexture;
                    }

                    InvokeHandler(globeTileLayerSet);
                }
                else
                {
                    downloadRequests [url] = DownloadStatus.InProgress;
                    downloadQueues [queueNum].Enqueue (globeTileLayerSet);

                    if (!globeTileQueueLookup.ContainsKey (url))
                    {
                        globeTileQueueLookup.Add (url, globeTileLayerSet);
                    }
                }
            }
            else
            {
                DownloadTask globeTileLayerSet = globeTileQueueLookup[url];
                globeTileLayerSet.handler += handler;
                globeTileLayerSet.prepareTextureForRendering = (globeTileLayerSet.prepareTextureForRendering || prepareForRendering);
            }
        }

        /// <summary>
        /// Clear all active downloads.
        /// </summary>
        public void Clear()
        {
            if (!initialized)
            {
                Init();
            }

            Debug.Log("Clearing download content");
            StopAllCoroutines();

            for (int i = 0; i < downloadQueues.Count; i++)
            {
                Clear(i);
            }

            globeTileTexturesToLoadQueue.Clear();
            globeTileQueueLookup.Clear();
            ongoingTextureLoadCount = 0;
            ongoingDownloadCount = 0;
        }

        /// <summary>
        /// Clear all downloads in the specified queue.
        /// </summary>
        /// <param name="queueNum">Index of the queue to clear.</param>
        public void Clear(int queueNum)
        {
            if (!initialized)
            {
                Init();
            }

            if (queueNum < 0 || queueNum >= downloadQueues.Count)
            {
                throw new ArgumentException("No such download queue " + queueNum);
            }

            Queue<DownloadTask> queue = downloadQueues[queueNum];
            while (queue.Count > 0)
            {
                DownloadTask task = queue.Dequeue();
                downloadRequests.Remove(task.url);
            }
            queue.Clear();
        }

        private void ProcessDownloadQueues()
        {
            int requestSubmitted = 0;
            int numEmptyQueues = 0;

            while (requestSubmitted < requestsPerFrame &&
                   numEmptyQueues < downloadQueues.Count &&
                   ongoingDownloadCount < maxOngoingDownloads)
            {
                Queue<DownloadTask> queue = downloadQueues[nextQueue];

                if (queue.Count > 0)
                {
                    StartCoroutine(DownloadTexture(queue.Dequeue()));
                    requestSubmitted += 1;
                }
                else
                {
                    numEmptyQueues += 1;
                }

                nextQueue = (nextQueue + 1) % downloadQueues.Count;
            }
        }

        private void ProcessTextureQueue()
        {
            //Debug.Log("Processing tile texture loads!");
            for (int i = 0; i < tileTextureLoadsPerFrame; i++)
            {
                //Debug.Log("Processing load #" + i);
                if (globeTileTexturesToLoadQueue.Count > 0)
                {
                    DownloadTask task = globeTileTexturesToLoadQueue.Dequeue();

                    // Invoke download handler
                    InvokeHandler( task );

                    downloadRequests.Remove(task.url);
                    globeTileQueueLookup.Remove(task.url);

                    ongoingTextureLoadCount--;
                }
                else
                {
                    break;
                }
            }
        }

        private void InvokeHandler(DownloadTask task)
        {
            // Add this texture to the cache
            textureCache.AddTexture(task.url, task.texture);

            if (task.handler != null)
            {
                task.handler(task.layerName, task.date, task.texture);
            }
        }

        private IEnumerator DownloadTexture(DownloadTask downloadTask)
        {
            ongoingTextureLoadCount++;
            ongoingDownloadCount++;

//        Debug.Log("NEW file: " + downloadTask.URL + "  " + downloadTask.localFilePath);
            yield return RequestWebTexture (downloadTask);

            ongoingDownloadCount--;
        }

        private static bool IsTextureInCache(DownloadTask downloadTask)
        {
            bool fileExists = false;
            downloadTask.usingDxtFormat = false;

            // Convert URL into a filepath by cutting off the initial domain name data
            Uri fileUrl = new Uri(downloadTask.url);

            // Sanitize path to be used as a local filename
            string rawQueryString = Uri.EscapeDataString(fileUrl.Query);

            // This string gets too long to convert into a single file name! We'll do a replace
            // to turn it into a bit of a tree structure...
            StringBuilder modifiedQueryStringBuilder = new StringBuilder();
            modifiedQueryStringBuilder.Append(rawQueryString);
            modifiedQueryStringBuilder.Replace("%2F", "/");

            // Remove some extra characters to get us a shorter path (uniqueness should still be preserved)
            modifiedQueryStringBuilder = modifiedQueryStringBuilder.Replace("%26", "");
            modifiedQueryStringBuilder = modifiedQueryStringBuilder.Replace("%3F", "");
            modifiedQueryStringBuilder = modifiedQueryStringBuilder.Replace("%3A", "");

            // There are lots of ='s in path names. I'm turning these into slashes in order to balance
            // out the file hierarchy a bit more.
            modifiedQueryStringBuilder.Replace("%3D", "/");

            StringBuilder ddsFilenameStringBuilder;
            StringBuilder pngFilenameStringBuilder;
            StringBuilder jpgFilenameStringBuilder;
            if (modifiedQueryStringBuilder.Length > 0)
            {
                string path = Application.dataPath + "/StreamingAssets" + fileUrl.AbsolutePath + modifiedQueryStringBuilder;
                ddsFilenameStringBuilder = new StringBuilder (path);
                pngFilenameStringBuilder = new StringBuilder (path);
                jpgFilenameStringBuilder = new StringBuilder (path);
            }
            else
            {
                string path = Application.dataPath + "/StreamingAssets" + fileUrl.AbsolutePath.Remove (fileUrl.AbsolutePath.Length - 4, 4);
                ddsFilenameStringBuilder = new StringBuilder(path);
                pngFilenameStringBuilder = new StringBuilder(path);
                jpgFilenameStringBuilder = new StringBuilder(path);
            }

            ddsFilenameStringBuilder.Append(".dds");
            pngFilenameStringBuilder.Append(".png");
            jpgFilenameStringBuilder.Append(".jpg");

            string ddsPath = ddsFilenameStringBuilder.ToString ();
            string pngPath = pngFilenameStringBuilder.ToString ();
            string jpgPath = jpgFilenameStringBuilder.ToString ();

            // First, let's check for a compressed (DDS) version of the file
            if (File.Exists (ddsPath))
            {
                downloadTask.localFilePath = ddsPath;
                downloadTask.usingDxtFormat = true;
                fileExists = true;
            }
            // No DDS version... PNG version?
            else if (File.Exists (pngPath))
            {
                // Necessary due to how the browser processes % symbols
                downloadTask.localFilePath = pngPath;
                downloadTask.usingDxtFormat = false;
                fileExists = true;
            }
            // No PNG version... JPG version?
            else if (File.Exists (jpgPath))
            {
                // Necessary due to how the browser processes % symbols
                downloadTask.localFilePath = jpgPath;
                downloadTask.usingDxtFormat = false;
                fileExists = true;
            }
            else
            {
                if (downloadTask.url.Contains(".jpg"))
                {
                    downloadTask.localFilePath = jpgPath;
                }
                else if (downloadTask.url.Contains("image/png"))
                {
                    downloadTask.localFilePath = pngPath;
                }
                else // Default to .png path
                {
                    downloadTask.localFilePath = pngPath;
                }
                
            }
            return fileExists;
        }

        private IEnumerator RequestWebTexture( DownloadTask downloadTask)
        {
            UnityWebRequest www = UnityWebRequestTexture.GetTexture (downloadTask.url);
            yield return www.Send();

            if (!www.isNetworkError)
            {
                Texture2D myTexture;
                if (useDownloadHandlerGetContent)
                {
                    myTexture = DownloadHandlerTexture.GetContent(www);

                    if (downloadTask.prepareTextureForRendering)
                    {
                        myTexture.wrapMode = TextureWrapMode.Clamp;
                    }
                }
                else
                {
                    myTexture = new Texture2D(2, 2, TextureFormat.RGB24, true, linearTextures);
                    myTexture.LoadImage(www.downloadHandler.data, false);


//#if UNITY_EDITOR
//                    if( cacheTextures )
//                    {
//                        // Copy into StreamingAssets
//                        string fileDirectory = Path.GetDirectoryName(downloadTask.localFilePath);
//                        System.Diagnostics.Debug.Assert(fileDirectory != null, "fileDirectory != null");
//                        Directory.CreateDirectory(fileDirectory);
//                        File.WriteAllBytes(downloadTask.localFilePath, www.downloadHandler.data);
//                    }
//#endif

                    if (downloadTask.prepareTextureForRendering)
                    {
                        myTexture.wrapMode = TextureWrapMode.Clamp;
                        myTexture.mipMapBias = globalMipMapBias;
                        myTexture.anisoLevel = globalAnisoLevel;
                        myTexture.Apply(true, true);
                    }
                }

                downloadTask.texture = myTexture;
                globeTileTexturesToLoadQueue.Enqueue(downloadTask);
            }
            else
            {
                // TODO actually handle error and retry
                Debug.Log("FAILURE!");
                Debug.Log("Error occurred when trying to download file " + downloadTask.url);
                Debug.Log("to location " + downloadTask.localFilePath);
                Debug.Log("Error: " + www.error);
                Debug.Log("Response code: " + www.responseCode);
                Debug.Log("URL: " + www.url);
                downloadRequests[downloadTask.url] = DownloadStatus.Error;
            }

            www.Dispose ();
        }
    }
}
