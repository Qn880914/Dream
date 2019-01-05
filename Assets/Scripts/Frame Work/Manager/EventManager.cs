using FrameWork.Event;
using FrameWork.Utility;
using System;
using System.Collections.Generic;

namespace FrameWork.Manager
{
    public partial class EventManager : Singleton<EventManager>, IManage
    {
        private Dictionary<Type, System.Delegate> delegates = new Dictionary<Type, System.Delegate>();

        private Dictionary<IView, List<string>> viewMap = new Dictionary<IView, List<string>>();

        protected readonly object m_syncRoot = new object();

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
            ClearViewListener();
        }



        public void AddViewListener(IView view, string evtName)
        {
            List<string> names;
            if(!viewMap.TryGetValue(view, out names))
            {
                names = new List<string>();
                viewMap[view] = names;
            }

            if(!names.Contains(evtName))
            {
                names.Add(evtName);
            }
            else
            {
                UnityEngine.Debug.LogWarning(string.Format("[EventManager.AddViewListener] Error, View Type : {0}  name : {1} alread listenee", view.GetType(), evtName));
            }
        }

        public void AddViewListener(IView view, List<string> evtNames)
        {
            List<string> names;
            if (!viewMap.TryGetValue(view, out names))
            {
                names = new List<string>();
                viewMap[view] = names;
            }

            for(int i = 0; i < evtNames.Count; ++ i)
            {
                if (!names.Contains(evtNames[i]))
                {
                    names.Add(evtNames[i]);
                }
                else
                {
                    UnityEngine.Debug.LogWarning(string.Format("[EventManager.AddViewListener] Error, View Type : {0}  name : {1} alread listenee", view.GetType(), evtNames.ToString()));
                }
            }
        }

        public void RemoveViewListener(IView view, string evtName)
        {
            List<string> names;
            if (!viewMap.TryGetValue(view, out names))
            {
                return;
            }

            names.Remove(evtName);
        }

        public void RemoveViewListener(IView view, List<string> evtNames)
        {
            List<string> names;
            if (!viewMap.TryGetValue(view, out names))
            {
                return;
            }

            for(int i = 0; i < evtNames.Count; ++ i)
            {
                names.Remove(evtNames[i]);
            }
        }

        public void RemoveViewListener(IView view)
        {
            viewMap.Remove(view);
        }

        public void ClearViewListener()
        {
            viewMap.Clear();
        }

        public void DispatchViewEvent(IEvent evt)
        {
            List<IView> views = new List<IView>();
            lock(m_syncRoot)
            {
                foreach(var val in viewMap)
                {
                    if(val.Value .Contains(evt.name))
                    {
                        views.Add(val.Key);
                    }
                }
            }

            for(int i = 0; i < views.Count; ++ i)
            {
                views[i].OnMessage(evt);
            }
        }
    }
}
