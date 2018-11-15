using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FrameWork.StateMachine
{
    public partial class Player
    {
        private StateMachine<PlayerStateType, Player> m_StateMachine = new StateMachine<PlayerStateType, Player>();

        protected void InitState()
        {
            RegisterState();
        }

        protected void RegisterState()
        {
            m_StateMachine.AddState(PlayerStateType.Idle, new PlayerIdle());
            m_StateMachine.AddState(PlayerStateType.Move, new PlayerMove());
        }

        protected void UnRegisterState()
        {
            m_StateMachine.RemoveState(PlayerStateType.Idle);
            m_StateMachine.RemoveState(PlayerStateType.Move);
        }

        public void OnMove()
        {
            m_StateMachine.SetState(PlayerStateType.Move);
        }

        public void OnIdle()
        {
            m_StateMachine.SetState(PlayerStateType.Idle);
        }
    }

    public abstract class PlayerState : State<PlayerStateType, Player>
    {
    }    
}
