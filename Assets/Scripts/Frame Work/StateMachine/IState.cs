using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FrameWork.StateMachine
{
    /// <summary>
    ///     <para> Implement this interface to make sure it is a Updatable object. </para>
    /// </summary>
    public interface IUpdate
    {
        void OnUpdate(float deltaTime);
    }

    /// <summary>
    ///     <para> Extend IUpdate </para>
    ///     <para> Implement this interface to make sure it is a Updatable object. </para>
    /// </summary>
    public interface IStatable : IUpdate
    {
        void OnEnter();

        void OnExit();
    }

    public interface IState<KT, OT> : IStatable
    {
        IStateMachine<KT, OT> GetCurrentStateMachine();

        void SetCurrentStateMachine(IStateMachine<KT, OT> stateMachine);
    }

    public interface MonoState<KT, MonoBehaviour> { }
}
