using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FsmState_Record_Ready : FsmState_Record
{
    public override void Enter()
    {
        //查找场景的点位
        Boot.Instance.moduleScene.FindScenePoint();
        Boot.Instance.obiSolver.gameObject.SetActive(true);
        //加载资源
    }

}
