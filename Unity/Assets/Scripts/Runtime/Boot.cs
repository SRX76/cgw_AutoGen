using System;
using System.Collections;
using UnityEngine;
using Obi;
using UnityEngine.SceneManagement;

public class Boot : MonoBehaviour
{
    public static Boot Instance { get; private set; }
    public ModuleScene moduleScene { get; private set; }
    public ModuleCamera moduleCamera { get; private set; }
    public ModuleSoftBody moduleSoftBody { get; private set; }

    public FsmMachine fsmMachine { get; private set; }
    public FsmMachine fsmMachine_Recorder { get; private set; }


    public ObiSolver obiSolver;
    public GameObject camRoot;

    private void Awake()
    {
        Instance = this;
        DontDestroyOnLoad(gameObject);
        moduleScene = new ModuleScene();
        moduleScene.Init();
        //moduleCamera = ModuleCamera.Create(camRoot);
        moduleSoftBody = ModuleSoftBody.Create();
        fsmMachine = new FsmMachine();
        fsmMachine_Recorder = new FsmMachine();
        Application.targetFrameRate = 30;
    }

    IEnumerator Start()
    {
        fsmMachine.SwitchState(new FsmState_LoadScene(1));

        var waitForEndOfFrame = new WaitForEndOfFrame();
        while (true)
        {
            yield return waitForEndOfFrame;
            EventCenter.OnFrameEnd?.Invoke();
        }
    }
    private void Update()
    {
        EventCenter.OnUpdate?.Invoke(Time.deltaTime);
    }
    private void LateUpdate()
    {
        EventCenter.OnLateUpdate?.Invoke();
    }
    private void FixedUpdate()
    {
        EventCenter.OnFixedUpdate?.Invoke();
    }

    private void OnApplicationQuit()
    {
        EventCenter.OnAppQuit?.Invoke();
    }
}
