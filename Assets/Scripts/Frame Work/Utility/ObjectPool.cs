using System.Collections.Generic;
using UnityEngine.Events;

namespace FrameWork.Utility
{
    internal class ObjectPool<T> where T : new ()
    {
        private readonly Stack<T> m_Stack = new Stack<T>();

        private readonly UnityAction<T> m_ActionGet;

        private readonly UnityAction<T> m_ActionRelease;

        public int countAll { get; private set; }

        public int countActive { get { return countAll - countInActive; } }

        public int countInActive { get { return m_Stack.Count; } }

        public ObjectPool(UnityAction<T> actionOnGet, UnityAction<T> actionOnRelease)
        {
            m_ActionGet = actionOnGet;
            m_ActionRelease = actionOnRelease;
        }

        public T Get()
        {
            T element;

            if(0 == m_Stack.Count)
            {
                element = new T();
            }
            else
            {
                element = m_Stack.Pop();
            }

            if(null != m_ActionGet)
            {
                m_ActionGet(element);
            }

            return element;
        }

        public void Release(T element)
        {
            if (m_Stack.Count > 0 && ReferenceEquals(m_Stack.Peek(), element))
                UnityEngine.Debug.Log("Internal error, Trying to destroy object that is already released to pool.");

            if (null != m_ActionRelease)
                m_ActionRelease(element);

            m_Stack.Push(element);
        }
    }
}

