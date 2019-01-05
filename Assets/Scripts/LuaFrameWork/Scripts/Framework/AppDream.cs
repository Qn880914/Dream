using FrameWork.Event;
using FrameWork.Manager;
using FrameWork.Utility;
using UnityEngine;

namespace LuaFramework.Framework
{
    public class AppDream : Singleton<AppDream>
    {
        private GameObject m_AppManager;
        public GameObject appManager
        {
            get
            {
                if(null == m_AppManager)
                {
                    m_AppManager = GameObject.Find("AppManager");
                }

                return m_AppManager;
            }
        }

        public void StartGame()
        { }

        protected void InitFramework()
        {
        }

        private void RegisterListener()
        {
            EventManager.instance.AddListener<StartGameEvent>(StartUp);
        }

        private void UnRegisterListener()
        {
            EventManager.instance.RemoveListener<StartGameEvent>(StartUp);
        }

        private void StartUp(StartGameEvent evt)
        {
            GameObject gameMgr = GameObject.Find("GlobalGenerator");
            if(null != gameMgr)
            {

            }
        }
    }
}
