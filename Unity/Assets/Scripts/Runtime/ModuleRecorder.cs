using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

public class ModuleRecorder
{
    private static bool SupportAsyncGpuReadback;
    private static Texture2D texture;
    public bool NeedSaveDate;

    public void Init()
    {
        SupportAsyncGpuReadback = SystemInfo.supportsAsyncGPUReadback;
        //EventCenter.OnFrameEnd += OnFrameEnd;
        EventCenter.OnAppQuit += Quit;
    }

    public void Quit()
    {
        if (texture != null)
        {
            Object.DestroyImmediate(texture);
            texture = null;
        }

    }

    public void OnFrameEnd()
    {
        if (!NeedSaveDate)
        {
            return;
        }
    }

    public static void SetSaveData(RenderTexture rt, string path)
    {
        var item = new SaveInfo(rt, path);
    }
    private static void Save(SaveInfo info)
    {
        Save(info.rt, info.path);
    }

    public static void Save(RenderTexture rt, string path)
    {
        CheckDirectory(path);

        int width = rt.width;
        int height = rt.height;
        if (texture != null && (texture.width != width || texture.height != height))
        {
            Object.DestroyImmediate(texture);
            texture = null;
        }
        if (texture == null)
        {
            //PlayerSettings.colorSpace
            texture = new Texture2D(width, height, TextureFormat.RGB24, false, false);
        }
        var prevRT = RenderTexture.active;
        RenderTexture.active = rt;

        texture.ReadPixels(new Rect(0, 0, width, height), 0, 0);
        texture.Apply();
        var bytes = texture.EncodeToPNG();
        File.WriteAllBytes(path, bytes);
        RenderTexture.active = prevRT;
    }

    static void CheckDirectory(string filePath)
    {
        string dir = Path.GetDirectoryName(filePath);
        if (!Directory.Exists(dir))
        {
            Directory.CreateDirectory(dir);
        }
    }


    class SaveInfo
    {
        public string path;
        public RenderTexture rt;
        public SaveInfo(RenderTexture rt, string path)
        {
            this.rt = rt;
            this.path = path;
        }
    }

}
