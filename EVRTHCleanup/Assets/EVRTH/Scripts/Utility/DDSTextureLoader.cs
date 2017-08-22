using System;
using System.IO;
using UnityEngine;

namespace Utility
{
    public static class DDSTextureLoader
    {
        public static Texture2D LoadTextureDXT(string ddsFilename, TextureFormat textureFormat)
        {
            //Debug.Log("Attempting load of " + ddsFilename);

            byte[] ddsBytes;

            try
            {
                ddsBytes = File.ReadAllBytes(ddsFilename);
            }
            catch (Exception e)
            {
                Debug.Log("Error occurred during file load!");
                Debug.Log(e.ToString());

                return (new Texture2D(2,2));
            }
        

            if (textureFormat != TextureFormat.DXT1 && textureFormat != TextureFormat.DXT5)
            { 
                throw new Exception("Invalid TextureFormat. Only DXT1 and DXT5 formats are supported by this method.");
            }

            byte ddsSizeCheck = ddsBytes[4];
            if (ddsSizeCheck != 124)
            { 
                throw new Exception("Invalid DDS DXTn texture. Unable to read");  //this header byte should be 124 for DDS image files
            }

            int height = ddsBytes[13] * 256 + ddsBytes[12];
            int width = ddsBytes[17] * 256 + ddsBytes[16];

            //Debug.Log("Header as hex:");
            //byte[] headerBytes = new byte[128];
            //Buffer.BlockCopy(ddsBytes, 0, headerBytes, 0, 128);
            //string hex = BitConverter.ToString(headerBytes);
            //hex = hex.Replace("-", " ");
            //Debug.Log(hex);

            int DDS_HEADER_SIZE = 128;
            byte[] dxtBytes = new byte[ddsBytes.Length - DDS_HEADER_SIZE];
            Buffer.BlockCopy(ddsBytes, DDS_HEADER_SIZE, dxtBytes, 0, ddsBytes.Length - DDS_HEADER_SIZE);

            Texture2D texture = new Texture2D(width, height, textureFormat, true);
        
            //Debug.Log("Attempting load of " + dxtBytes.Length + " bytes, for " + width + " by " + height + " image of format " + textureFormat + " for a mipmap count of " + texture.mipmapCount);

            texture.LoadRawTextureData(dxtBytes);
            texture.Apply();

            return (texture);
        }
    }
}
