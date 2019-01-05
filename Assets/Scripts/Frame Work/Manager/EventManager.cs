using FrameWork.Event;
using FrameWork.Utility;
using System;
using System.Collections.Generic;
using FrameWork;

namespace FrameWork.Manager
{
    public class EventManager : Singleton<EventManager>, IManage
    {
        private Dictionary<Type, System.Delegate> delegates = new Dictionary<Type, System.Delegate>();

        public void AddListener<T>(EventDelegate<T> del) where T : GameEvent
        {
            System.Delegate tempDel;
            if (delegates.TryGetValue(typeof(T), out tempDel))
                tempDel = System.Delegate.Combine(tempDel, del);
            else
                tempDel = del;

            delegates[typeof(T)] = tempDel;
        }

        public void RemoveListener<T>(EventDelegate<T> del) where T : GameEvent
        {
            System.Delegate currentDel;
            if(delegates.TryGetValue(typeof(T), out currentDel))
            {
                currentDel = System.Delegate.Remove(currentDel, del);
                if (null == currentDel)
                    delegates.Remove(typeof(T));
                else
                    delegates[typeof(T)] = currentDel;
            }
        }

        public void RemoveAllListener<T>()where T : GameEvent
        {
            delegates.Remove(typeof(T));
        }

        public void DispatchEvent<T>(T evt) where T : GameEvent
        {
            System.Delegate del;
            if(delegates.TryGetValue(typeof(T), out del))
            {
                evt.LogEvent();
                del.DynamicInvoke(evt);
            }
        }

        public void ClearListener()
        {
            delegates.Clear();
        }
    }
}
