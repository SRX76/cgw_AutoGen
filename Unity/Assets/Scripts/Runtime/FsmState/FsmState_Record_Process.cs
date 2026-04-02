using UnityEngine;

public class FsmState_Record_Process : FsmState_Record
{
    private ModuleSoftBody module;
    int curIndex;
    int maxCount;
    float time = 0;
    float duration = 4f;
    int frameCount = 30;
    int frameIndex;
    private string modelName;

    private readonly string SceneName;

    private string rootPath;

    private bool NeedSave;
    private Camera vCam_Solid;
    private Camera vCam_Softbody;
    private RenderTexture rt_solid;
    private RenderTexture rt_Softbody;
    private bool IsSoftBodyMode;

    public FsmState_Record_Process(string sceneName)
    {
        SceneName = sceneName;
        rootPath = "Recordings";
    }

    public override void Enter()
    {
        module = Boot.Instance.moduleSoftBody;
        module.ResetIndex();
        frameIndex = -1;
        time = 0;
        NeedSave = false;
        EventCenter.OnFrameEnd += OnFrameEnd;
        var camRoot = Boot.Instance.camRoot.transform;

        vCam_Softbody = camRoot.Find("Camera/Camera_Main").GetComponent<Camera>();
        vCam_Solid = camRoot.Find("Camera/Camera_Solid").GetComponent<Camera>();
        vCam_Solid.gameObject.SetActive(true);
        vCam_Softbody.gameObject.SetActive(true);
        rt_solid = vCam_Solid.targetTexture;
        rt_Softbody = vCam_Softbody.targetTexture;
        IsSoftBodyMode = true;
    }
    public override void Exit()
    {
        vCam_Solid?.gameObject.SetActive(false);
        vCam_Softbody?.gameObject.SetActive(false);
        EventCenter.OnFrameEnd -= OnFrameEnd;
    }

    public override void Update(float dtTime)
    {
        //if (time <= 0)
        if (frameIndex >= frameCount && time <= 0)
        {
            if (IsSoftBodyMode)
            {
                //需要切换模型
                if (!module.MoveNextGO())
                {
                    Debug.Log($"加载模型失败,可能是模型加载循环完成了");
                    return;
                }
                IsSoftBodyMode = false;
                module.CurGoSoftBody.SetActive(false);
                module.CurGoMesh.SetActive(true);
                modelName = module.GetModelName();
            }
            else
            {
                module.CurGoSoftBody.SetActive(true);
                module.CurGoMesh.SetActive(false);
                IsSoftBodyMode = true;
            }

            //加载模型，录制时间
            frameIndex = -1;
            time = duration;
            return;
        }

        NeedSave = true;
        time -= dtTime;
        frameIndex++;

        vCam_Solid?.gameObject.SetActive(!IsSoftBodyMode);
        vCam_Softbody?.gameObject.SetActive(IsSoftBodyMode);
    }

    string GetImageName_Solid()
    {
        return $"{rootPath}/Solid/{modelName}_{SceneName}/img_{frameIndex}.png";
    }

    string GetImageName_SoftBody()
    {
        return $"{rootPath}/Softbody/{modelName}_{SceneName}/img_{frameIndex}.png";
    }

    void OnFrameEnd()
    {
        if (!NeedSave || Boot.Instance.SkipRecorder)
        {
            return;
        }
        NeedSave = false;
        //Debug.LogError($"开启录制:{solidName}\n{softbodyName}\n");
        if (IsSoftBodyMode)
        {
            string softbodyName = GetImageName_SoftBody();
            ModuleRecorder.Save(rt_Softbody, softbodyName);
        }
        else
        {
            string solidName = GetImageName_Solid();
            ModuleRecorder.Save(rt_solid, solidName);
        }
    }


}
