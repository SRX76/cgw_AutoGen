using Obi;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SingleModule<T>
{
    public static T Instance { get; protected set; }
}

public class ModuleSoftBody : SingleModule<ModuleSoftBody>
{
    public static event Action OnLoadGoMesh; //模型切换完成
    public static event Action OnMeshLoopFinished;//模型切换循环完成

    public static ModuleSoftBody Create()
    {
        if (Instance == null)
        {
            Instance = new ModuleSoftBody();
        }
        Instance?.Init();
        return Instance;
    }

    private List<string> goMesh = new();
    private List<Material> materials = new();
    public int Index { get; private set; }
    public string ObiPfbPath = "Assets/Res/Prefabs/pfb_ObiSoftBody.prefab";
    public string pfbRoot = "Assets/Res/Prefabs";
    public string obiBpRoot = "Assets/Res/BP";
    public string goMeshRoot = "Assets/Res/Prefabs/Mesh";

    public GameObject GoPfb { get; private set; }

    public GameObject GoObiPfb { get; private set; }

    public GameObject CurGoMesh { get; private set; }
    public GameObject CurGoSoftBody { get; private set; }
    public ObiSoftbodySurfaceBlueprint CurBP { get; private set; }
    public void Init()
    {
        goMesh = new List<string>() { "goPfb__1", "goPfb__2", "goPfb__3", "goPfb__4" };
        //EventCenter.OnLoadSceneFinished -= OnLoadSceneFinished;
        //EventCenter.OnLoadSceneFinished += OnLoadSceneFinished;
        GoObiPfb = Boot.Instance.goPfbObi;
        //GoObiPfb = LoadAsset<GameObject>(ObiPfbPath);
        GoObiPfb.SetActive(false);
    }

    public void OnLoadSceneFinished()
    {
        Index = -1;
        MoveNextGO();
    }

    public void ResetIndex()
    {
        Index = -1;
        CleanGameObject();
    }

    public string GetModelName()
    {
        return goMesh[Index];
    }
    public bool MoveNextGO()
    {
        CleanGameObject();
        if (Index + 1 >= goMesh.Count)
        {
            Index = -1;
            OnMeshLoopFinished?.Invoke();
            return false; ;
        }
        Index++;
        var points = ModuleScene.Instance?.Points;
        if (points.Count > 0)
        {
            var root = Boot.Instance.obiSolver.transform;
            CreateMeshGo(root, points[0].position);
        }

        OnLoadGoMesh?.Invoke();
        return true;
    }

    public void LoadMesh_GO(string pfbName)
    {
        string file = $"{goMeshRoot}/{pfbName}.prefab";
        string bpFile = $"{obiBpRoot}/{pfbName}.asset";
        GoPfb = LoadAsset<GameObject>(file);
        CurBP = LoadAsset<ObiSoftbodySurfaceBlueprint>(bpFile);
        materials.Clear();
        if (GoPfb != null)
        {
            materials.AddRange(GoPfb.GetComponent<MeshRenderer>().sharedMaterials);
        }
    }

    public void CreateMeshGo(Transform root, Vector3 pos)
    {
        CleanGameObject();
        if (CurGoMesh != null)
        {
            materials.Clear();
            GameObject.DestroyImmediate(CurGoMesh);
        }
        string fileName = goMesh[Index];
        LoadMesh_GO(fileName);
        if (GoPfb != null)
        {
            var go = GameObject.Instantiate(GoPfb, root);
            go.transform.position = pos;
            go.SetActive(true);
            CurGoMesh = go;
        }
    }

    public void SwitchSoftBody()
    {
        if (CurGoMesh != null)
        {
            var pos = CurGoMesh.transform.position;
            var root = Boot.Instance.obiSolver.transform;
            CreaetMeshSoftBody(root, pos);
        }
    }

    public void CreaetMeshSoftBody(Transform root, Vector3 pos)
    {
        CleanGameObject();
        if (GoObiPfb == null)
        {
            Debug.LogError($"软体模拟预制体初始化为完成");
            return;
        }

        CurGoSoftBody = GameObject.Instantiate(GoObiPfb, root);
        CurGoSoftBody.transform.position = pos;
        CurGoSoftBody.GetComponent<SkinnedMeshRenderer>().sharedMaterials = materials.ToArray();
        var softbody = CurGoSoftBody.GetComponent<ObiSoftbody>();
        softbody.softbodyBlueprint = CurBP;
        CurGoSoftBody.SetActive(true);
    }

    public void CleanGameObject()
    {
        if (CurGoSoftBody != null)
        {
            GameObject.DestroyImmediate(CurGoSoftBody);
            CurGoSoftBody = null;
        }
        if (CurGoMesh != null)
        {
            GameObject.DestroyImmediate(CurGoMesh);
        }
    }

    public static T LoadAsset<T>(string path) where T : UnityEngine.Object
    {
#if UNITY_EDITOR
        var go = UnityEditor.AssetDatabase.LoadAssetAtPath<T>(path);
        return go;
#else
        return null;
#endif
    }
}
