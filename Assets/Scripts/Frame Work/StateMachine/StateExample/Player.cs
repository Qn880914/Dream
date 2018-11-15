using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FrameWork.StateMachine
{
    public partial class Player : IUpdate
    {
        public void Init()
        {
            InitState();
        }

        public void OnUpdate(float deltaTime)
        {
            m_StateMachine.OnUpdate(deltaTime);
        }

        public void Dispose()
        { }
    }
}
