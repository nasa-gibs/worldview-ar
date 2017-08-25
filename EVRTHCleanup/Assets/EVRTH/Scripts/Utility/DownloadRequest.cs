using System;
using UnityEngine;

namespace SwarmDownload.Scripts
{
    public struct DownloadRequest
    {
        public string url;
        public Action<Texture2D> callbackAction;
    }

}
