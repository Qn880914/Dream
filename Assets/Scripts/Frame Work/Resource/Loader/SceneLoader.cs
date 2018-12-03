using UnityEngine;

namespace FrameWork.Resource
{
    /// <summary>
    /// 场景 加载器
    /// </summary>
    public class SceneLoader : Loader
    {
        private AsyncOperation m_Request = null;

        public SceneLoader() : base(LoaderType.Scene)
        { }

        public override void Start()
        {
            base.Start();

            if (m_Async)
                m_Request = UnityEngine.SceneManagement.SceneManager.LoadSceneAsync(m_Path);
            else
            {
                UnityEngine.SceneManagement.SceneManager.LoadScene(m_Path);
                LoadComplete(true);
            }
        }

        public override void Update()
        {
            if (LoaderState.Loading != m_State)
                return;

            if (null == m_Request)
                LoadComplete(false);
            else if (m_Request.isDone)
                LoadComplete(true);
            else
                OnProgress(m_Request.progress);
        }

        private void LoadComplete(bool success)
        {
            OnCompleted(success);
            m_Request = null;
            //FrameWork.Manager.ResourceManager.instance.clear
        }
    }
}
