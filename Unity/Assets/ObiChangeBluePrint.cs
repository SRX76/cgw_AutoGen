using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Obi;
using UnityEditor;
using System.IO;
using System.Linq;
using System;

public class ObiChangeBluePrint : MonoBehaviour
{
    public List<string> bpFiles;
    int index = 0;
    public List<ObiSoftbodySurfaceBlueprint> bps;

    public Vector3 pos;
    public GameObject goPfb;
    public GameObject curItem;

    public float duration = 0.5f;
    public float time = 0;

//    private void Awake()
//    {
//        goPfb?.SetActive(false);
//        pos = transform.position;
//        InitBPs();
//        StartCoroutine(LoadSoftBody(index));
//        time = duration;
//    }
//    private void Update()
//    {
//        time -= Time.deltaTime;
//        if (time <= 0)
//        {
//            if (bps.Count > 0)
//            {
//                index = (index + 1) % bps.Count;
//            }
//            if (index >= 0)
//            {
//                //LoadSoftBody(index);
//                StartCoroutine(LoadSoftBody(index));
//                time = duration;
//            }
//        }
//        if (time < 0)
//        {
//            Debug.LogError($"录制完成******，主动关闭项目");
//            ReadyQuit();
//        }
//    }

//    void InitBPs()
//    {
//        string file = "Config/bps.txt";
//        bpFiles = File.ReadAllLines(file).ToList();
//        bps = new List<ObiSoftbodySurfaceBlueprint>(bpFiles.Count);
//        foreach (var bpFile in bpFiles)
//        {
//#if UNITY_EDITOR
//            string path = $"Assets/Res/BP/{bpFile}.asset";
//            var bp = AssetDatabase.LoadAssetAtPath<ObiSoftbodySurfaceBlueprint>(path);
//            if (bp != null)
//            {
//                bps.Add(bp);
//            }
//#endif
//        }
//    }

//    IEnumerator LoadSoftBody(int index)
//    {
//        index = index % bps.Count;

//        var go = GameObject.Instantiate(goPfb, transform);
//        go.GetComponent<ObiSoftbody>().softbodyBlueprint = bps[index];
//        var skinner = go.GetComponent<ObiSoftbodySkinner>();
//        skinner.enabled = false;
//        go.name = bps[index].name;
//        go.gameObject.SetActive(true);

//        var needDeleteItem = curItem;

//        skinner.enabled = true;
//        if (needDeleteItem != null)
//        {
//            needDeleteItem.SetActive(false);
//            GameObject.DestroyImmediate(needDeleteItem);
//            needDeleteItem = null;
//        }

//        curItem = go;

//#if UNITY_EDITOR
//        //if (RecorderDemo.RecorderSettings != null)
//        //{
//        //    RecorderDemo.RecorderSettings.Take++;
//        //}
//#endif
//        yield return null;
      
//    }

//    void ReadyQuit()
//    {
//#if UNITY_EDITOR
//        //EditorApplication.isPlaying = false;
//        Debug.LogError($"循环结束，主动关闭unity工程");
//        //EditorApplication.Exit(0);
//#else
//        Application.Quit();
//#endif
//    }

}
