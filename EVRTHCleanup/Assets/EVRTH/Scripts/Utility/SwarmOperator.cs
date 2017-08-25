using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

namespace SwarmDownload.Scripts
{
    [Serializable]
    internal class SwarmOperator
    {
        [SerializeField]
        internal int jobs;
        private readonly Queue<DownloadRequest> requests;

        public SwarmOperator()
        {
            requests = new Queue<DownloadRequest>();
        }

        public void AddJob(DownloadRequest request)
        {
            requests.Enqueue(request);
            jobs++;
            if (jobs == 1)
            {
                SwarmManager.Instance.StartCoroutine(ProcessRequestsCoroutine());
            }
        }

        public void KillThread()
        {
            SwarmManager.Instance.StopAllCoroutines();
            requests.Clear();
        }

        private IEnumerator ProcessRequestsCoroutine()
        {
            while (requests.Count > 0)
            {
                DownloadRequest currRequest = requests.Dequeue();
                UnityWebRequest www = UnityWebRequestTexture.GetTexture(currRequest.url);
                yield return www.Send();

                if (www.isNetworkError)
                {
                    Debug.Log(www.error);
                }
                else
                {
                    Texture2D newTex = DownloadHandlerTexture.GetContent(www);
                    currRequest.callbackAction.Invoke(newTex);
                }
                jobs--;
            }
        }
    }
}