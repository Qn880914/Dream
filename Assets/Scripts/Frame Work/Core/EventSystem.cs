using System;
using System.Collections.Generic;

namespace FrameWork.Event
{
    public class EventSystem : IEventSystem
    {
        protected static readonly object helper_lock = new object();

        protected static volatile IEventSystem m_Instance;
        public static IEventSystem instance
        {
            get
            {
                if(null == m_Instance)
                {
                    lock(helper_lock)
                    {
                        if (null == m_Instance)
                            m_Instance = new EventSystem();
                    }
                }

                return m_Instance;
            }
        }

        protected IDictionary<string, Type> m_EventMap;
        protected IDictionary<IView, List<string>> m_ViewEventMap;

        protected EventSystem()
        {
            Initialize();
        }

        protected virtual void Initialize()
        {
            m_EventMap = new Dictionary<string, Type>();
            m_ViewEventMap = new Dictionary<IView, List<string>>();
        }

        public void DispatchListener(IEvent evt)
        {
            Type type = null;
            lock (helper_lock)
            {
                if(!m_EventMap.TryGetValue(evt.name, out type))
                {
                    foreach(var viewEvent in m_ViewEventMap)
                    {
                        if (viewEvent.Value.Contains(evt.name))
                            viewEvent.Key.OnMessage(evt);
                    }
                }
                else
                {
                    object triggerInstance = Activator.CreateInstance(type);
                    IEventTrigger trigger = triggerInstance as IEventTrigger;
                    if (null != trigger)
                        trigger.Execute(evt);
                }
            }
        }

        public bool HasListener(string eventName)
        {
            return m_EventMap.ContainsKey(eventName);

        }

        public void RegisterListener(string eventName, Type eventType)
        {
            lock(helper_lock)
            {
                m_EventMap[eventName] = eventType;
            }
        }

        public void RegisterViewListener(IView view, string[] eventNames)
        {
            lock (helper_lock)
            {
                List<string> names;
                if (!m_ViewEventMap.TryGetValue(view, out names))
                {
                    names = new List<string>();
                    m_ViewEventMap[view] = names;
                }

                for (int i = 0; i < eventNames.Length; ++i)
                {
                    if (!names.Contains(eventNames[i]))
                        names.Add(eventNames[i]);
                }
            }
        }

        public void UnRegisterListener(string eventName)
        {
            lock(helper_lock)
            {
                m_EventMap.Remove(eventName);
            }
        }

        public void UnRegisterViewListener(IView view, string[] eventNames)
        {
            lock(helper_lock)
            {
                List<string> names;
                if(!m_ViewEventMap.TryGetValue(view, out names))
                {
                    return;
                }

                for(int i = 0; i < eventNames.Length; ++ i)
                {
                    names.Remove(eventNames[i]);
                }
            }
        }
    }
}
