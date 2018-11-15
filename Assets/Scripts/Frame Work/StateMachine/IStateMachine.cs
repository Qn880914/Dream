using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FrameWork.StateMachine
{
    /// <summary>
    ///     <para></para>
    /// </summary>
    /// <typeparam name="KT"></typeparam>
    /// <typeparam name="OT"></typeparam>
    public interface IStateMachine<KT, OT> : IUpdate
    {
        OT owner { get; set; }

        KT GetCurrentState();

        void AddState(KT keyType, IState<KT, OT> state);

        void RemoveState(KT keyType);

        void SetState(KT keyType);

        void ModifyState(KT keyType);
    }
}
