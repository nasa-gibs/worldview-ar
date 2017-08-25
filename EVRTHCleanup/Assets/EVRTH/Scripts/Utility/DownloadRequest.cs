using System;
using UnityEngine.Networking;

namespace SwarmDownload.Scripts
{
    public struct DownloadRequest
    {
        public string url;
        public Action<UnityWebRequest> callbackAction;
    }

}
