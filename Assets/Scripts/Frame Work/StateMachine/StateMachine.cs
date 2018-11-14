using FrameWork.Utility;
using System.Collections.Generic;
using UnityEngine;

namespace FrameWork.StateMachine
{
    public class StateMachine<KT, OT> : IStateMachine<KT, OT>
    {
        private KT m_NowKey;

        public OT owner { get; set; }

        private IState<KT, OT> m_NowState;

        private List<Pair<KT, IState<KT, OT>>> m_PairList = new List<Pair<KT, IState<KT, OT>>>();

        private void Assert(bool condition)
        {
            if(!condition)
            {
                throw new System.Exception();
            }
        }

        public KT GetCurrentState()
        {
            return m_NowKey;
        }

        public void AddState(KT keyType, IState<KT, OT> state)
        {
            bool isResearched = false;
            foreach(var pair in m_PairList)
            {
                if(pair.first.Equals(keyType))
                {
                    pair.second = state;
                    isResearched = true;
                    break;
                }
            }

            if(!isResearched)
            {
                Pair<KT, IState<KT, OT>> pair = new Pair<KT, IState<KT, OT>>(keyType, state);
                m_PairList.Add(pair);
            }

            state.SetCurrentStateMachine(this);
        }

        public void RemoveState(KT keyType)
        {
            Pair<KT, IState<KT, OT>> remove = null;
            foreach(var pair in m_PairList)
            {
                if(pair.first.Equals(keyType))
                {
                    remove = pair;
                    break;
                }
            }

            Assert(null != remove);
            m_PairList.Remove(remove);
        }

        public void SetState(KT keyType)
        {
            bool isResearched = false;
            foreach(var pair in m_PairList)
            {
                if(pair.first.Equals(keyType))
                {
                    if(null != m_NowState)
                    {
                        m_NowState.OnExit();
                    }

                    m_NowKey = keyType;
                    m_NowState = pair.second;
                    isResearched = true;
                    break;
                }
            }

            Assert(isResearched);
            if(null != m_NowState)
            {
                m_NowState.OnEnter();
            }
        }

        public void ModifyState(KT keyType)
        {
            bool isResearched = false;
            foreach(var pair in m_PairList)
            {
                if(pair.first.Equals(keyType))
                {
                    m_NowKey = keyType;
                    m_NowState = pair.second;
                    isResearched = true;
                    break;
                }
            }

            Assert(isResearched);
        }
    }

    public class StateMachineMono<KT> : StateMachine<KT, MonoBehaviour> { }
}
