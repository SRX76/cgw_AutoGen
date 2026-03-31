using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;
using Obi;
using System.Linq;
using System.Text;
using UnityEditor.SceneManagement;
using Codice.Client.BaseCommands.WkStatus.Printers;

public class Entry
{
    static readonly string RootPath_Mesh = "Assets/Res/model";
    static readonly string RootPath_Prefab = "Assets/Res/Prefabs";
    static readonly string RootPath_BP = "Assets/Res/BP";
    static readonly string Config_BPFile = "Config/bps.txt";

    [MenuItem("Tools/Entry")]
    [MenuItem("Tools/检测模型文件并创建对应的蓝图数据")]
    static void AutoEntry()
    {
        //这个是fbx作为输入的流程入口
        EditorSceneManager.OpenScene("Assets/Scenes/Boot.unity", OpenSceneMode.Single);
        //CheackFolderPath();
        //var meshFiles = FindsMeshFiles(RootPath_Mesh);
        //CheckAssetImporter(meshFiles);
        //StringBuilder sb = new StringBuilder();
        //foreach (var file in meshFiles)
        //{
        //    if (CreateBluePrintAsset(file, false))
        //    {
        //        sb.AppendLine(GetBPFile(file));
        //    }
        //}
        //File.WriteAllText(Config_BPFile, sb.ToString());

        EditorApplication.isPlaying = true;

        //RecorderDemo.AutoRecorder();
    }
    [MenuItem("Tools/创建配置文件")]
    static void GenConfig()
    {
        var meshFiles = FindsMeshFiles(RootPath_Mesh);
        StringBuilder sb = new StringBuilder();
        foreach (var file in meshFiles)
        {
            if (CreateBluePrintAsset(file, false))
            {
                sb.AppendLine(GetBPFile(file));
            }
        }
        File.WriteAllText(Config_BPFile, sb.ToString());
    }
    static void CheackFolderPath()
    {
        var list = new List<string>() { RootPath_Mesh, RootPath_BP };
        foreach (var folder in list)
        {
            if (!Directory.Exists(folder))
            {
                Directory.CreateDirectory(folder);
            }
        }
        AssetDatabase.Refresh();
    }

    [MenuItem("Tools/预制体流程入口")]
    static void EntryByPrefab()
    {
        //EditorSceneManager.OpenScene("Assets/Scenes/SampleScene.unity", OpenSceneMode.Single);
        CheackFolderPath();
        var prefabs = FindModelPfb(RootPath_Prefab);
        CleanErrorPrefabs(ref prefabs);
        StringBuilder sb = new StringBuilder();
        foreach (var pfb in prefabs)
        {
            if (CreateBluePrintAssetByPrefab(pfb))
            {
                var bpFile = Path.GetFileNameWithoutExtension(pfb);
                sb.AppendLine(bpFile);
            }
        }
        File.WriteAllText(Config_BPFile, sb.ToString());
        RecorderDemo.AutoRecorder();
    }

    #region 模型处理

    //根据预制体，收集数据
    static List<string> FindModelPfb(params string[] rootPath)
    {
        var guids = AssetDatabase.FindAssets("t:GameObject", rootPath);
        var prefabs = new List<string>(guids.Length);
        foreach (var guid in guids)
        {
            var path = AssetDatabase.GUIDToAssetPath(guid);
            prefabs.Add(path);
        }
        return prefabs;
    }
    //检查预制体是否是包含网格
    static void CleanErrorPrefabs(ref List<string> prefabs)
    {
        List<string> needRemoveFiles = new List<string>();
        for (int i = 0; i < prefabs.Count; i++)
        {
            var go = AssetDatabase.LoadAssetAtPath<GameObject>(prefabs[i]);
            if (go == null)
            {
                prefabs[i] = null;
                continue;
            }
            var meshFilter = go.GetComponentInChildren<MeshFilter>();
            if (meshFilter == null || meshFilter.sharedMesh == null)
            {
                prefabs[i] = null;
                continue;
            }
            var mesh = meshFilter.sharedMesh;
            var meshFile = AssetDatabase.GetAssetPath(mesh);
            if (string.IsNullOrEmpty(meshFile))
            {
                prefabs[i] = null;
                continue;
            }
            SetMeshRead(meshFile);
        }

        prefabs.RemoveAll(_ => string.IsNullOrEmpty(_));
    }


    //扫描模型数据
    static IList<string> FindsMeshFiles(params string[] rootPath)
    {
        var guids = AssetDatabase.FindAssets("t:mesh", rootPath);
        List<string> meshFiles = new List<string>(guids.Length);
        foreach (var guid in guids)
        {
            var path = AssetDatabase.GUIDToAssetPath(guid);
            meshFiles.Add(path);
        }
        return meshFiles;
    }

    //检查模型导入配置
    static void CheckAssetImporter(IList<string> files)
    {
        foreach (var file in files)
        {
            SetMeshRead(file);
        }
    }

    static void SetMeshRead(string file)
    {
        var importer = AssetImporter.GetAtPath(file);
        if (importer is ModelImporter modelImporter)
        {
            if (!modelImporter.isReadable)
            {
                modelImporter.isReadable = true;
                modelImporter.SaveAndReimport();
            }
        }
    }
    #endregion

    #region 软体模拟蓝图处理

    //根据meshFilter创建蓝图资产
    static bool CreateBluePrintAssetByPrefab(string pfbFile)
    {
        var go = AssetDatabase.LoadAssetAtPath<GameObject>(pfbFile);
        var meshFilter = go.GetComponentInChildren<MeshFilter>();
        if (meshFilter == null || meshFilter.sharedMesh == null)
        {
            return false;
        }
        var meshFile = AssetDatabase.GetAssetPath(meshFilter.sharedMesh);
        if (string.IsNullOrEmpty(meshFile))
        {
            return false;
        }

        var bpFile = $"{RootPath_BP}/{Path.GetFileNameWithoutExtension(pfbFile)}.asset";
        var bp = AssetDatabase.LoadAssetAtPath<ObiSoftbodySurfaceBlueprint>(bpFile);
        if (bp == null)
        {
            bp = ScriptableObject.CreateInstance<ObiSoftbodySurfaceBlueprint>();
            AssetDatabase.CreateAsset(bp, bpFile);
        }
        bp.inputMesh = meshFilter.sharedMesh;
        bp.scale = meshFilter.transform.localScale;
        bp.surfaceSamplingMode = ObiSoftbodySurfaceBlueprint.SurfaceSamplingMode.Vertices;
        bp.surfaceResolution = 9;
        bp.volumeSamplingMode = ObiSoftbodySurfaceBlueprint.VolumeSamplingMode.None;
        bp.volumeResolution = 9;
        bp.shapeResolution = 16;
        bp.maxAnisotropy = 3;
        bp.smoothing = 0.25f;
        bp.GenerateImmediate();
        return true;
    }

    //检测并生成蓝图资产
    static bool CreateBluePrintAsset(string file, bool forcesGenAsset = true)
    {
        var mesh = AssetDatabase.LoadAssetAtPath<Mesh>(file);
        if (mesh == null)
        {
            return false;
        }
        var bpFile = $"{RootPath_BP}/{GetBPFile(file)}.asset";
        var bp = AssetDatabase.LoadAssetAtPath<ObiSoftbodySurfaceBlueprint>(bpFile);
        if (bp == null)
        {
            bp = ScriptableObject.CreateInstance<ObiSoftbodySurfaceBlueprint>();
            AssetDatabase.CreateAsset(bp, bpFile);
        }
        if (bp.inputMesh == mesh && !forcesGenAsset)
        {
            return true;
        }
        bp.inputMesh = mesh;
        bp.GenerateImmediate();
        return true;
    }

    //生成bp资产的名字
    static string GetBPFile(string meshFile)
    {
        string file = Path.GetFileNameWithoutExtension(meshFile).Replace(" ", "");
        return file;
    }

    #endregion

}
