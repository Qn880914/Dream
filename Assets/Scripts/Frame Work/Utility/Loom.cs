using System.Collections.Generic;
using System.Linq;
using System.Threading;
using UnityEngine;
using UnityEngine.Events;

namespace FrameWork.Utility
{
    public class Loom : MonoSingleton<Loom>
    {
        public struct DelayQueueItem
        {
            public float time;

            public UnityAction callback;
        }

        public readonly int maxThreadCount = 10;

        private int m_CurrentThreadCount;

        private List<UnityAction> m_Actions = new List<UnityAction>();

        private List<UnityAction> m_CurrentActions = new List<UnityAction>();

        private List<DelayQueueItem> m_DelayQueueItems = new List<DelayQueueItem>();

        private List<DelayQueueItem> m_CurrentDelayQueueItems = new List<DelayQueueItem>();
        
        public void QueueOnMainThread(UnityAction action)
        {
            QueueOnMainThread(action, 0f);
        }

        public void QueueOnMainThread(UnityAction action, float time)
        {
            if(0 != time)
            {
                lock(m_DelayQueueItems)
                {
                    m_DelayQueueItems.Add(new DelayQueueItem { time = Time.time + time, callback = action });
                }
            }
            else
            {
                lock(m_Actions)
                {
                    m_Actions.Add(action);
                }
            }
        }

        public Thread RunAsync(UnityAction action)
        {
            while(m_CurrentThreadCount > maxThreadCount)
            {
                Thread.Sleep(1);
            }

            Interlocked.Increment(ref m_CurrentThreadCount);
            ThreadPool.QueueUserWorkItem(RunAction, action);

            return null;
        }

        private void RunAction(object param)
        {
            try
            {
                UnityAction action = param as UnityAction;
                action.Invoke();
            }
            catch
            { }
            finally
            {
                Interlocked.Decrement(ref m_CurrentThreadCount);
            }
        }

        private void Update()
        {
            lock(m_Actions)
            {
                m_CurrentActions.Clear();
                m_CurrentActions.AddRange(m_Actions);
                m_Actions.Clear();
            }

            foreach(var action in m_Actions)
            {
                action.Invoke();
            }

            lock(m_DelayQueueItems)
            {
                m_CurrentDelayQueueItems.Clear();
                m_CurrentDelayQueueItems.AddRange(m_DelayQueueItems.Where(d => d.time <= Time.time));

                foreach (var item in m_CurrentDelayQueueItems)
                    m_DelayQueueItems.Remove(item);
            }

            foreach (var delayItem in m_CurrentDelayQueueItems)
                delayItem.callback.Invoke();
        }
    }
}

