using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FrameWork.Manager;

namespace FrameWork.Scene
{
    public class MainScene : SceneBase
    {
        public override void OnEnter()
        {
            ResourceManager.instance.LoadScene("Login", null, null, false);
        }

        public override void OnExit()
        {
            throw new System.NotImplementedException();
        }

        public override void OnUpdate(float deltaTime)
        {
            throw new System.NotImplementedException();
        }
    }
}

