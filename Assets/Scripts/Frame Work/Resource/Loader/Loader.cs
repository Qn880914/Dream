﻿using UnityEngine.Events;

namespace FrameWork.Resource
{
    public class Loader
    {
        public enum LoaderState
        {
            None, 

            Loading,    // 正在加载中
                
            Finish,     // 加载完成
        }

        private UnityAction<float> m_ActionProgress;

        private UnityAction<object> m_CompleteCallback;
        public UnityAction<object> completeCallbck { get { return m_CompleteCallback; } set { m_CompleteCallback = value; } }

        /// 加载器类型
        protected LoaderType m_Type;
        public LoaderType type { get { return m_Type; } }

        /// 加载状态
        protected LoaderState m_State;
        public LoaderState state { get { return m_State; } }

        /// 路径
        protected string m_Path;
        public string path { get { return m_Path; } }

        /// 附加参数
        protected object m_Param;

        /// 是否异步
        protected bool m_Async;
        public bool async { get { return m_Async; } }

        public bool isDone { get { return m_State == LoaderState.Finish; } }

        protected Loader(LoaderType type)
        {
            m_Type = type;
        }

        public virtual void Init(string path, object param, UnityAction<float> actionProgress,
            UnityAction<object> completeCallback, bool async = true)
        {
            m_State = LoaderState.None;
            m_Path = path;
            m_Param = param;
            m_Async = async;
            m_ActionProgress = actionProgress;
            m_CompleteCallback = completeCallback;
        }

        public virtual void Start()
        {
            m_State = LoaderState.Loading;
            OnProgress(0f);
        }

        public virtual void Update()
        { }

        public virtual void Stop()
        {
            Reset();
        }

        public virtual void Reset()
        {
            m_Path = string.Empty;
            m_CompleteCallback = null;
            m_Async = true;
        }

        public void ChangeToSync()
        {
            m_Async = false;
        }

        protected void OnProgress(float progress)
        {
            if (!m_Async)
                return;

            if(null != m_ActionProgress)
                m_ActionProgress(progress);
        }

        protected void OnCompleted(object data)
        {
            m_State = LoaderState.Finish;

            if(null != m_CompleteCallback)
                m_CompleteCallback(data);
        }
    }
}
