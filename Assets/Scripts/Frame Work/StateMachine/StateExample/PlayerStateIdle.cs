using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FrameWork.StateMachine
{
    public class PlayerIdle : PlayerState
    {
        public override void OnEnter()
        {
            UnityEngine.Debug.Log("   PlayerIdle OnEnter ");
        }

        public override void OnExit()
        {
            UnityEngine.Debug.Log("   PlayerIdle OnExit ");
        }

        public override void OnUpdate(float deltaTime)
        {
            UnityEngine.Debug.Log("   PlayerIdle OnUpdate ");
        }
    }
}
