using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class FsmState_LoadScene : FsmStateBase
{
    public string SceneName { get; private set; }
    public int SceneIndex { get; private set; }
    //public FsmState_LoadScene(string sceneName)
    public FsmState_LoadScene(int index)
    {
        SceneIndex = index;
    }
    public override void Enter()
    {
        EventCenter.OnLoadSceneFinished += OnLoadSceneFinished;
        EventCenter.OnAllSceneFinished += OnAllSceneFinished;

        ModuleSoftBody.OnMeshLoopFinished += OnMeshLoopFinished;
        Boot.Instance.fsmMachine_Recorder.SwitchState(new FsmState_Record());
        Boot.Instance.moduleScene.LoadScene(SceneIndex);
    }

    public override void Exit()
    {
        EventCenter.OnLoadSceneFinished -= OnLoadSceneFinished;
        EventCenter.OnAllSceneFinished -= OnAllSceneFinished;
        ModuleSoftBody.OnMeshLoopFinished -= OnMeshLoopFinished;
    }

    private void OnLoadSceneFinished()
    {
        SceneName = SceneManager.GetActiveScene().name;
        var moduleScene = Boot.Instance.moduleScene;
        moduleScene.FindScenePoint();
        Boot.Instance.obiSolver.gameObject.SetActive(true);
        var vcam = moduleScene.VCam;
        Boot.Instance.camRoot.transform.position = vcam.position;
        Boot.Instance.camRoot.transform.rotation = vcam.rotation;
        //开启录制状态
        Boot.Instance.fsmMachine_Recorder.SwitchState(new FsmState_Record_Process(SceneName));
    }

    private void OnAllSceneFinished()
    {
        Debug.LogError($"所有组合录制完成，准备退出流程");
#if UNITY_EDITOR
        Debug.LogError($"循环结束，主动关闭unity工程");
        UnityEditor.EditorApplication.isPlaying = false;
        //EditorApplication.Exit(0);
#else
        Application.Quit(0);
#endif

    }

    void OnMeshLoopFinished()
    {
        //模型轮换完成，准备切换到下一个场景中
        int nextSceneIndex = SceneIndex + 1;
        //Debug.LogError($"********nextSceneIndex{nextSceneIndex}");
        Boot.Instance.fsmMachine_Recorder.SwitchState(new FsmState_Record());
        Boot.Instance.fsmMachine.SwitchState(new FsmState_LoadScene(nextSceneIndex));
    }
}

