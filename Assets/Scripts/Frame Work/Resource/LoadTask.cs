using FrameWork.Utility;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace FrameWork.Resource
{
    public class LoadTask
    {
        private enum LoadState
        {
            Waiting,
            Loading,
        }

        private class AsyncCallbackInfo
        {
            private LoadAction<object> m_CompleteCallback;
            private object m_Data;

            public AsyncCallbackInfo(LoadAction<object> callback, object data)
            {
                m_CompleteCallback = callback;
                m_Data = data;
            }

            public void OnComplete()
            {
                if(null != m_CompleteCallback)
                    m_CompleteCallback(m_Data);
            }
        }

        private LoadAction<float> m_ProgressCallback;

        /// <summary>
        /// 当前正在加载的 loader
        ///     AddLoadTask  : 同步则加入这个队列
        /// </summary>
        private List<Loader> m_Loaders = new List<Loader>();

        /// <summary>
        /// 异步加载等待队列
        /// </summary>
        private Queue<Loader> m_LoaderQueueAsync = new Queue<Loader>();

        /// <summary>
        /// // 异步加载等待 loader
        ///     stirng : resource Path
        /// </summary>
        private Dictionary<string, Loader> m_DicLoaders = new Dictionary<string, Loader>();

        /// <summary>
        /// 加载状态
        ///     LoadState.Waiting
        ///     LoadState.Loading
        /// </summary>
        private LoadState m_State = LoadState.Loading;

        /// 异步加载 完成数量
        private int m_CountAsync;

        /// 异步加载 数量
        private int m_TotalCountAsync;

        private List<AsyncCallbackInfo> m_AsyncCallbackInfos = new List<AsyncCallbackInfo>();

        public void OnUpdate()
        {
            int count = m_Loaders.Count;
            for (int i = count - 1; i > -1; -- i)
            {
                Loader loader = m_Loaders[i];
                loader.Update();

                if (loader.isDone)
                {
                    if (m_DicLoaders.ContainsKey(loader.path))
                        m_DicLoaders.Remove(loader.path);

                    m_Loaders.RemoveAt(i);
                    if(!m_LoaderQueueAsync.Contains(loader))
                        LoaderPool.Release(loader);

                    if(loader.async)
                    {
                        ++m_CountAsync;
                        m_DicLoaders.Remove(loader.path);
                        RefreshLoadProgress(0f);
                    }

                    CheckNextTask();
                }
            }

            UpdateAsyncCallback();
        }

        private void CheckNextTask(bool start = false)
        {
            if (m_State != LoadState.Loading)
                return;

            if (m_Loaders.Count > 0)
                return;

            if(m_LoaderQueueAsync.Count > 0)
            {
                Loader loader = m_LoaderQueueAsync.Dequeue();
                m_Loaders.Add(loader);

                if(!loader.isDone)
                    loader.Start();

                ++m_CountAsync;
                RefreshLoadProgress(0f);
            }
            else
            {
                m_CountAsync = 0;
                m_TotalCountAsync = 0;
            }
        }

        private void UpdateAsyncCallback()
        {
            for (int i = 0; i < m_AsyncCallbackInfos.Count; ++i)
                m_AsyncCallbackInfos[i].OnComplete();

            m_AsyncCallbackInfos.Clear();
        }

        private void RefreshLoadProgress(float addRate)
        {
            float rate = addRate;
            int count = m_TotalCountAsync;
            int index = Mathf.Max(0, m_CountAsync - 1);

            if (count == 0)
            {
                index = 0;
                count = 1;
            }
            else
                rate = (index + addRate) / m_TotalCountAsync;

            if (null != m_ProgressCallback)
            {
                m_ProgressCallback(rate);

                if (rate >= 1f)
                    m_ProgressCallback = null;
            }
        }

        public bool CheckImmediateLoad(string path, LoadAction<object> callback)
        {
            Loader loader;
            if (!m_DicLoaders.TryGetValue(path, out loader))
                return false;

            if (loader.state == Loader.LoaderState.None)
            {
                loader.ChangeToSync();
                m_Loaders.Add(loader);
                loader.Start();
            }

            return true;
        }

        public void AddAsyncCallback(LoadAction<object> callback, object data)
        {
            if (null == callback)
                return;

            m_AsyncCallbackInfos.Add(new AsyncCallbackInfo(callback, data));
        }

        public int GetWaitLoadingCount(string path)
        {
            Loader loader;
            if (!m_DicLoaders.TryGetValue(path, out loader))
                return 0;

            return loader.completeCallbck.GetInvocationList().Length;
        }

        private void ClearLoader()
        {
            for (int i = 0; i < m_Loaders.Count; ++i)
                m_Loaders[i].Stop();

            for (int i = 0; i < m_Loaders.Count; ++i)
                LoaderPool.Release(m_Loaders[i]);

            for (int i = 0; i < m_LoaderQueueAsync.Count; ++i)
                LoaderPool.Release(m_LoaderQueueAsync.Dequeue());

            m_Loaders.Clear();
            m_LoaderQueueAsync.Clear();
            m_DicLoaders.Clear();
        }

        public Loader AddLoadTask(LoaderType type, string path, object param, UnityAction<object> callback, bool async)
        {
            Loader loader;
            if (type != LoaderType.Scene && type != LoaderType.BundleAsset)
            {
                if (m_DicLoaders.TryGetValue(path, out loader))
                {
                    loader.completeCallbck += callback;
                    return loader;
                }
            }

            loader = LoaderPool.Get(type);
            loader.Init(path, param, OnLoadProgress, callback, async);

            if (!async)
            {
                m_Loaders.Add(loader);
                loader.Start();
            }
            else
            {
                if (type != LoaderType.Scene && type != LoaderType.BundleAsset)
                    m_DicLoaders.Add(path, loader);

                m_LoaderQueueAsync.Enqueue(loader);
                if (m_TotalCountAsync != 0)
                    ++m_TotalCountAsync;
            }

            if (0 == m_Loaders.Count)
                CheckNextTask();

            return loader;
        }

        public void BeginFrontLoad()
        {
            m_State = LoadState.Waiting;
        }

        public void StartFrontLoad(LoadAction<float> action)
        {
            m_ProgressCallback = action;
            if (m_State != LoadState.Waiting)
                return;

            m_State = LoadState.Loading;

            m_CountAsync = 0;
            m_TotalCountAsync = m_LoaderQueueAsync.Count;

            CheckNextTask(true);
        }        

        private void OnLoadProgress(float rate)
        {
            RefreshLoadProgress(rate);
        }

        public void Clear()
        {
            ClearLoader();
        }
    }
}
