using FrameWork.Event;
using FrameWork.Scene;
using FrameWork.StateMachine;
using FrameWork.Utility;
using UnityEngine;

namespace FrameWork.Manager
{
    public class SceneManager : Singleton<SceneManager>, IManage
    {
        private StateMachine<SceneType, SceneManager> m_StateMachine = new StateMachine<SceneType, SceneManager>();

        public SceneType currentScene { get { return m_StateMachine.GetCurrentState(); } }

        public void Init()
        {
            RegisterScene();
            EventManager.instance.AddListener<LoadSceneEvent>(LoadScene);
            m_StateMachine.SetState(SceneType.Login);
        }

        public void OnUpdate()
        {
            m_StateMachine.OnUpdate(Time.deltaTime);
        }

        private void LoadScene(LoadSceneEvent param)
        {
            //m_StateMachine.SetState(param.type);
            switch (param.type)
            {
                case SceneType.Login:
                    break;
                case SceneType.Main:
                    break;
                case SceneType.Battle:
                    break;
            }
        }

        public void Clear()
        {
            UnRegisterScene();
            EventManager.instance.RemoveListener<LoadSceneEvent>(LoadScene);
        }

        private void RegisterScene()
        {
            m_StateMachine.AddState(SceneType.Login, new LoginScene());
            m_StateMachine.AddState(SceneType.Main, new MainScene());
        }

        private void UnRegisterScene()
        {
            m_StateMachine.RemoveState(SceneType.Login);
            m_StateMachine.RemoveState(SceneType.Main);
        }
    }
}
