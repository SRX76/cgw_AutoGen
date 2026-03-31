using System;

public static class EventCenter
{
    #region Unity生命周期

    public static Action<float> OnUpdate;
    public static Action OnLateUpdate;
    public static Action OnFixedUpdate;
    public static Action OnFrameEnd;
    public static Action OnAppQuit;
    #endregion

    #region 游戏事件
    //场景加载
    public static Action OnPrevUnloadScene;
    public static Action OnLoadSceneFinished;
    public static Action OnAllSceneFinished;

    #endregion



}
