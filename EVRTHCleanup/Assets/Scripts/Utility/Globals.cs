using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;



#region Enums
public enum DownloadStatus
{
    InProgress,
    Error
}


internal enum LoadStatus
{
    NotStarted,
    InProgress,
    Complete
}


public enum LayerStatus
{
    Loading,       // Waiting for root tiles to load.
    Transitioning, // Fading between different sets of tiles.
    Complete       // Root tiles are loaded. Child tiles may still be streaming in.
}

public enum SimulationMode
{
    Undefined,
    Cupola,
    Workspace
}

#endregion

#region Structs

internal struct GlobeTileLayerInfo
{
    public string name;
    public DateTime date;
    public LoadStatus status;
}

internal struct Pair<T>
{
    public T item1;
    public T item2;

    public Pair(T item1, T item2)
    {
        this.item1 = item1;
        this.item2 = item2;
    }
}

[Serializable]
public struct Wmts
{
    public int row;
    public int col;
    public int zoom;

    public override string ToString()
    {
        return "Row: " + row + "  Col: " + col + "  Zoom: " + zoom;
    }
}

public struct DataRange
{

    public float min;
    public float max;
    public bool transparent;

}

public struct VertexAndUvContainer
{
    public List<int> vertexIndices;
    public List<Vector2> uvs;
}

#endregion


#region Delegates

public delegate void TextureDownloadHandler(string layer, DateTime date, Texture texture); 

#endregion


//public static class Extensions
//{
//    public static float GetMinimumIntensity(this Texture2D t)
//    {
//        Color[] pixels = t.GetPixels();
//        Color min = pixels.OrderBy(c => (c.r + c.g + c.b) * 0.3f).FirstOrDefault();
//        return (min.r + min.g + min.b) / 3;
//    }

//    public static float GetMaximumIntensity(this Texture2D t)
//    {
//        Color[] pixels = t.GetPixels();
//        Color min = pixels.OrderByDescending(c => (c.r + c.g + c.b) * 0.3f).FirstOrDefault();
//        return (min.r + min.g + min.b) / 3;
//    }
//}