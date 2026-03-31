using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

public class ModuleScene
{
    public static ModuleScene Instance { get; private set; }
    public int SceneCount { get; private set; }
    public int SceneIndex { get; private set; }
    public Transform VCam { get; private set; }
    public List<Transform> Points { get; private set; }

    public void Init()
    {
        Instance = this;
        SceneIndex = 0;
        SceneManager.sceneLoaded -= OnSceneLoaded;
        SceneManager.sceneLoaded += OnSceneLoaded;
        SceneCount = SceneManager.sceneCountInBuildSettings;
    }

    public void LoadScene()
    {
        var curIndex = SceneIndex;
        curIndex++;
        if (curIndex > SceneCount)
        {
            EventCenter.OnAllSceneFinished?.Invoke();
            return;
        }
        EventCenter.OnPrevUnloadScene?.Invoke();
        SceneManager.LoadScene(curIndex, new LoadSceneParameters(LoadSceneMode.Single));
    }

    public void LoadScene(int sceneIndex)
    {
        //if (sceneIndex > SceneManager.sceneCount)
        if (sceneIndex >= SceneCount)
        {
            EventCenter.OnAllSceneFinished?.Invoke();
            return;
        }
        SceneIndex = sceneIndex;
        EventCenter.OnPrevUnloadScene?.Invoke();
        SceneManager.LoadScene(SceneIndex, new LoadSceneParameters(LoadSceneMode.Single));
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        SceneIndex = scene.buildIndex;
        if (SceneIndex > 0)
        {
            //FindScenePoint();
            //Boot.Instance.obiSolver.gameObject.SetActive(true);
            EventCenter.OnLoadSceneFinished?.Invoke();
        }
    }

    //在场景中查找设置的点位
    public void FindScenePoint()
    {
        VCam = null;
        Points?.Clear();
        var root = GameObject.Find("KeyPoint")?.transform;
        if (root == null)
        {
            Debug.LogError($"场景点位查找失败");
            return;
        }
        VCam = root.Find("VCam");
        var ps = root.Find("Points");
        Points = new List<Transform>(ps.childCount);
        for (int i = 0; i < ps.childCount; i++)
        {
            Points.Add(ps.GetChild(i));
        }
    }


}
