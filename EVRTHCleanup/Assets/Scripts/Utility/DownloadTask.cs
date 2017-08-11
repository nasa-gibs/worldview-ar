using System;
using UnityEngine;
public class DownloadTask
{
    public string url;
    public string layerName;
    public DateTime date;
    public TextureDownloadHandler handler;
    public bool prepareTextureForRendering;
    public Texture texture;
    public string localFilePath;
    public bool usingDxtFormat;
}
