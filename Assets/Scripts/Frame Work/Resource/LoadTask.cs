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

        private class LoaderWaitList
        {
            public Loader loader;

            public List<LoadAction<object>> callbacks;
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
                {
                    m_CompleteCallback(m_Data);
                }
            }
        }

        private LoadAction<float> m_ActionProgress;

        private List<Loader> m_Loaders = new List<Loader>();

        private Queue<Loader> m_LoaderQueue = new Queue<Loader>();

        private LoadState m_State = LoadState.Loading;

        private int m_Count;

        private int m_TotalCount;

        private Dictionary<string, LoaderWaitList> m_Waits = new Dictionary<string, LoaderWaitList>();

        private List<AsyncCallbackInfo> m_AsyncCallbackInfos = new List<AsyncCallbackInfo>();

        public LoadTask() { }

        public void Clear() { }

        public void OnUpdate()
        {
            int count = m_Loaders.Count;
            for(int i = count - 1; i > -1; -- i)
            {
                Loader loader = m_Loaders[i];
                if(loader.isDone)
                {
                    m_Loaders.RemoveAt(i);
                    if(!m_LoaderQueue.Contains(loader))
                    {
                        LoaderPool.Release(loader);
                    }

                    if(loader.async)
                    {
                        ++m_Count;
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

            if(m_LoaderQueue.Count > 0)
            {
                Loader loader = m_LoaderQueue.Dequeue();
                m_Loaders.Add(loader);

                if(!loader.isDone)
                {
                    loader.Start();
                }

                ++m_Count;
                RefreshLoadProgress(0f);
            }
            else
            {
                m_Count = 0;
                m_TotalCount = 0;
            }
        }

        private void UpdateAsyncCallback()
        {
            for (int i = 0; i < m_AsyncCallbackInfos.Count; ++i)
            {
                m_AsyncCallbackInfos[i].OnComplete();
            }

            m_AsyncCallbackInfos.Clear();
        }

        private void RefreshLoadProgress(float addRate)
        {
            float rate = addRate;
            int count = m_TotalCount;
            int index = Mathf.Max(0, m_Count - 1);

            if (count == 0)
            {
                index = 0;
                count = 1;
            }
            else
            {
                rate = (index + addRate) / m_TotalCount;
            }

            if (m_ActionProgress != null)
            {
                m_ActionProgress(rate);

                if (rate >= 1f)
                {
                    m_ActionProgress = null;
                }
            }
        }

        public bool CheckImmediateLoad(string path, LoadAction<object> callback)
        {
            Loader loader = CheckWaitLoading(path, callback);
            if (null == loader)
            {
                return false;
            }

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
            {
                return;
            }

            m_AsyncCallbackInfos.Add(new AsyncCallbackInfo(callback, data));
        }

        private Loader CheckWaitLoading(string path, LoadAction<object> callback)
        {
            LoaderWaitList waitList;
            if (!m_Waits.TryGetValue(path, out waitList))
            {
                m_Waits.Add(path, new LoaderWaitList());
                return null;
            }

            List<LoadAction<object>> callbacks = waitList.callbacks;
            if (null == callbacks)
            {
                callbacks = new List<LoadAction<object>>();
                waitList.callbacks = callbacks;
            }

            callbacks.Add(callback);
            return waitList.loader;
        }

        private void SetWaitLoader(string path, Loader loader)
        {
            LoaderWaitList waitList;
            if (m_Waits.TryGetValue(path, out waitList))
            {
                waitList.loader = loader;
            }
            else
            {
                UnityEngine.Debug.Log(string.Format("SetWaitLoader:{0}, have no wait list ....", path));
            }
        }

        public void WaitLoadingFinish(string path, object data)
        {
            LoaderWaitList waitList;
            if (!m_Waits.TryGetValue(path, out waitList))
            {
                return;
            }

            List<LoadAction<object>> callbacks = waitList.callbacks;
            if (null != callbacks)
            {
                for (int i = 0; i < callbacks.Count; ++i)
                {
                    LoadAction<object> callback = callbacks[i];
                    if (null != callback)
                    {
                        callback(data);
                    }
                }

                callbacks.Clear();
            }

            m_Waits.Remove(path);
        }

        public int GetWaitLoadingCount(string path)
        {
            LoaderWaitList waitList;
            if (!m_Waits.TryGetValue(path, out waitList))
            {
                return 0;
            }

            return waitList.callbacks.Count;
        }

        private void ClearLoader()
        {
            for (int i = 0; i < m_Loaders.Count; ++i)
            {
                m_Loaders[i].Stop();
            }

            for (int i = 0; i < m_Loaders.Count; ++i)
            {
                LoaderPool.Release(m_Loaders[i]);
            }

            for (int i = 0; i < m_LoaderQueue.Count; ++i)
            {
                LoaderPool.Release(m_LoaderQueue.Dequeue());
            }

            m_Loaders.Clear();
            m_LoaderQueue.Clear();
        }

        private Loader AddLoadTask(LoaderType type, string path, object param, UnityAction<Loader, object> callback, bool async)
        {
            Loader loader = LoaderPool.Get(type);
            loader.Init(path, param, OnLoadProgress, callback, async);

            if (!async)
            {
                m_Loaders.Add(loader);
                loader.Start();
            }
            else
            {
                m_LoaderQueue.Enqueue(loader);
                if (m_TotalCount != 0)
                {
                    ++m_TotalCount;
                }
            }

            if (0 == m_Loaders.Count)
            {
                CheckNextTask();
            }

            return loader;
        }

        public void AddLoadTask(LoaderType type, string path, object param, LoadAction<object> callback, bool async)
        {
            if (type != LoaderType.Scene && type != LoaderType.Bundle)
            {
                if (CheckWaitLoading(path, callback) != null)
                {
                    return;
                }
            }

            Loader ldr = AddLoadTask(type, path, param, (loader, data) =>
            {
                if (callback != null)
                {
                    callback(data);
                }

                if (type != LoaderType.Scene && type != LoaderType.Bundle)
                {
                    WaitLoadingFinish(path, data);
                }
            }, async);

            //只有异步加载才有所谓等待列表
            if (async && type != LoaderType.Scene)
            {
                SetWaitLoader(path, ldr);
            }
        }

        public void BeginFrontLoad()
        {
            m_State = LoadState.Waiting;
        }

        public void StartFrontLoad(LoadAction<float> progress)
        {
            m_ActionProgress = progress;
            if (m_State != LoadState.Waiting)
            {
                return;
            }

            m_State = LoadState.Loading;

            m_Count = 0;
            m_TotalCount = m_LoaderQueue.Count;

            CheckNextTask(true);
        }

        

        private void OnLoadProgress(Loader loader, float rate)
        {
            if (!loader.async)
            {
                return;
            }

            RefreshLoadProgress(rate);
        }

    }
}
