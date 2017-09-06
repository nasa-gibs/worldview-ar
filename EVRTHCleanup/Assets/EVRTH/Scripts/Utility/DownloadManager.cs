using System;
using System.Collections;
using System.Collections.Generic;
using EVRTH.Scripts.GlobeNS;
using EVRTH.Scripts.Visualization;
using SwarmDownload.Scripts;
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

                downloadRequests[url] = DownloadStatus.InProgress;
                //downloadQueues[queueNum].Enqueue(globeTileLayerSet);
                SwarmManager.Instance.EnqueueRequest(new DownloadRequest
                {
                    url = url,
                    callbackAction = t =>
                    {
                        Texture2D myTexture;
                        if (useDownloadHandlerGetContent)
                        {
                            myTexture = DownloadHandlerTexture.GetContent(t);

                            if (globeTileLayerSet.prepareTextureForRendering)
                            {
                                myTexture.wrapMode = TextureWrapMode.Clamp;
                            }
                            print(myTexture.width + " " + myTexture.height);
                        }
                        else
                        {
                            myTexture = new Texture2D(2, 2, TextureFormat.RGB24, true, linearTextures);
                            myTexture.LoadImage(t.downloadHandler.data, false);
                            if (globeTileLayerSet.prepareTextureForRendering)
                            {
                                myTexture.wrapMode = TextureWrapMode.Clamp;
                                myTexture.mipMapBias = globalMipMapBias;
                                myTexture.anisoLevel = globalAnisoLevel;
                                myTexture.Apply(true, true);
                            }
                            print(myTexture.width + " " + myTexture.height);
                        }

                        globeTileLayerSet.texture = myTexture;
                        globeTileTexturesToLoadQueue.Enqueue(globeTileLayerSet);
                    }
                });

                if (!globeTileQueueLookup.ContainsKey(url))
                {
                    globeTileQueueLookup.Add(url, globeTileLayerSet);
                }
            }
            else
            {
                DownloadTask globeTileLayerSet = globeTileQueueLookup[url];
                globeTileLayerSet.handler += handler;
                globeTileLayerSet.prepareTextureForRendering =
                    globeTileLayerSet.prepareTextureForRendering || prepareForRendering;
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

            yield return RequestWebTexture (downloadTask);

            ongoingDownloadCount--;
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
                    print(myTexture.width + " " + myTexture.height);
                }
                else
                {
                    myTexture = new Texture2D(2, 2, TextureFormat.RGB24, true, linearTextures);
                    myTexture.LoadImage(www.downloadHandler.data, false);

                    if (downloadTask.prepareTextureForRendering)
                    {
                        myTexture.wrapMode = TextureWrapMode.Clamp;
                        myTexture.mipMapBias = globalMipMapBias;
                        myTexture.anisoLevel = globalAnisoLevel;
                        myTexture.Apply(true, true);
                    }
                    print(myTexture.width + " " + myTexture.height);
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
