using FrameWork;
using FrameWork.Event;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace LuaFramework.Framework
{
    public class Facade
    {
        private static GameObject s_AppManager;
        public GameObject appManager
        {
            get
            {
                if(null == s_AppManager)
                {
                    s_AppManager = GameObject.Find("AppManager");
                }

                return s_AppManager;
            }
        }

        protected IEventSystem m_EventSystem;

        private static Dictionary<string, object> s_Managers = new Dictionary<string, object>();

        protected Facade()
        {
            InitFramework();
        }

        protected virtual void InitFramework()
        {
            if (null == m_EventSystem)
                m_EventSystem = EventSystem.instance;
        }

        #region event listener
        public virtual void RegisterListener(string eventName, Type type)
        {
            m_EventSystem.RegisterListener(eventName, type);
        }

        public virtual void UnRegisterListener(string eventName)
        {
            m_EventSystem.UnRegisterListener(eventName);
        }

        public virtual bool HasListener(string eventName)
        {
            return m_EventSystem.HasListener(eventName);
        }

        public virtual void RegisterMultiListener(Type type, params string[] eventNames)
        {
            int count = eventNames.Length;
            for(int i = 0; i < count; ++ i)
                RegisterListener(eventNames[i], type);
        }

        public virtual void UnRegisterMultiListener(params string[] eventNames)
        {
            int count = eventNames.Length;
            for (int i = 0; i < count; ++i)
                UnRegisterListener(eventNames[i]);
        }

        public void DispatchListener(string eventName, object data = null)
        {
            m_EventSystem.DispatchListener(new FrameWork.Event.Event(eventName, data));
        }
        #endregion // event listener



        public void AddManager(string typeName, IManage manage)
        {
            if(!s_Managers.ContainsKey(typeName))
            {
                s_Managers.Add(typeName, manage);
            }
        }

        public T AddManager<T>(string typeName) where T : Component
        {
            object result = null;
            s_Managers.TryGetValue(typeName, out result);
            if (result != null)
            {
                return (T)result;
            }
            Component c = s_AppManager.AddComponent<T>();
            s_Managers.Add(typeName, c);
            return default(T);
        }

        public T GetManager<T>(string typeName) where T : class
        {
            if (!s_Managers.ContainsKey(typeName))
            {
                return default(T);
            }
            object manager = null;
            s_Managers.TryGetValue(typeName, out manager);
            return (T)manager;
        }

        public void RemoveManager(string typeName)
        {
            if (!s_Managers.ContainsKey(typeName))
            {
                return;
            }
            object manager = null;
            s_Managers.TryGetValue(typeName, out manager);
            Type type = manager.GetType();
            if (type.IsSubclassOf(typeof(MonoBehaviour)))
            {
                GameObject.Destroy((Component)manager);
            }
            s_Managers.Remove(typeName);
        }
    }
}
