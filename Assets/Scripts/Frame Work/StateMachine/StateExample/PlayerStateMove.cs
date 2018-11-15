using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FrameWork.StateMachine
{
    public class PlayerMove : PlayerState
    {
        public override void OnEnter()
        {
            UnityEngine.Debug.Log("   PlayerMove OnEnter ");
        }

        public override void OnExit()
        {
            UnityEngine.Debug.Log("   PlayerMove OnExit ");
        }

        public override void OnUpdate(float deltaTime)
        {
            UnityEngine.Debug.Log("   PlayerMove OnUpdate ");
        }
    }
}

