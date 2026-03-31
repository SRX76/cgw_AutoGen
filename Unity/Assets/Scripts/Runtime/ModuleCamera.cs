using System;
using UnityEngine;

public class ModuleCamera
{
    [Flags]
    public enum CameraMode
    {
        None = 0,
        Mesh = 1,
        SoftBody = 1 << 1
    }
    public CameraMode Mode { get; private set; }

    //public Camera cameraMain;
    //public Camera SaveCameraSoftBody { get; private set; }
    //public Camera SaveCameraSolid { get; private set; }
    //public int FrameIndex { get; private set; }
    //public bool NeedSaveCamera { get; set; }
    public static ModuleCamera Create(GameObject root)
    {
        var module = new ModuleCamera();
        //module.Init(root);
        return module;
    }

    //public void Init(GameObject root)
    //{
    //    cameraMain = root.transform.Find("Camera").GetComponent<Camera>(); ;
    //    SaveCameraSoftBody = root.transform.Find("Camera/Camera_Main").GetComponent<Camera>();
    //    SaveCameraSolid = root.transform.Find("Camera/Camera_Solid").GetComponent<Camera>();
    //    EventCenter.OnFrameEnd -= OnFrameEnd;
    //    EventCenter.OnFrameEnd += OnFrameEnd;

    //    EventCenter.OnLoadSceneFinished += OnLoadSceneFinished;
    //}

    //public void SwitchMode_SoftBody()
    //{
    //    SetMode(CameraMode.SoftBody);
    //}

    //public void SwitchMode_Mesh()
    //{
    //    SetMode(CameraMode.Mesh);
    //}

    //public void SetMode(CameraMode mode)
    //{
    //    Mode = mode;
    //    SaveCameraSoftBody.gameObject.SetActive(mode.HasFlag(CameraMode.SoftBody));
    //    SaveCameraSolid.gameObject.SetActive(mode.HasFlag(CameraMode.Mesh));
    //}

    //void OnFrameEnd()
    //{
    //    if (!NeedSaveCamera)
    //    {
    //        return;
    //    }
    //    FrameIndex++;
    //    RenderTexture rt = null;
    //    string file = "";
    //    if (Mode.HasFlag(CameraMode.Mesh))
    //    {
    //        file = $"Recordings/Img_Solid_{0}.png";
    //        rt = SaveCameraSolid.targetTexture;
    //    }
    //    else if (Mode.HasFlag(CameraMode.SoftBody))
    //    {
    //        file = $"Recordings/Img_SoftBody_{0}.png";
    //        rt = SaveCameraSoftBody.targetTexture;
    //    }
    //    ModuleRecorder.Save(rt, file);

    //}

    //void OnLoadSceneFinished()
    //{
    //    var trans = Boot.Instance.moduleScene.VCam;
    //    if (trans == null)
    //    {
    //        Debug.LogError($"场景点位查找失败");
    //        return;
    //    }
    //    cameraMain.transform.position = trans.position;
    //    cameraMain.transform.rotation = trans.rotation;
    //}

}
