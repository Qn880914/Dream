using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngine.Events;

namespace FrameWork.Utility
{
    public class Loom : MonoBehaviour
    {
        public struct DelayQueueItem
        {
            public float time;

            public UnityAction callback;
        }

        public static readonly int maxTreadCount = 10;

        private static int s_CurrentTreadCount;

        private static bool s_Initialized;

        private static object lock_helper = new object();

        private static Loom s_Current;
        public static Loom current
        {
            get
            {
                if(!s_Initialized)
                {
                    lock(lock_helper)
                    {
                        if(!s_Initialized)
                        {
                            Initialize();
                            s_Initialized = true;
                        }
                    }
                }

                return s_CurrentLoom;
            }
        }

        private List<UnityAction> m_Actions = new List<UnityAction>();

        private List<UnityAction> m_CurrentActions = new List<UnityAction>();

        private List<DelayQueueItem> m_DelayQueueItems = new List<DelayQueueItem>();

        private List<DelayQueueItem> m_CurrentDelayQueueItems = new List<DelayQueueItem>();

        private void Awake()
        {
            s_Current = this;
            s_Initialized = true;
        }

        private static void Initialize()
        {
            if (!Application.isPlaying)
                return;

            GameObject obj = new GameObject("Loom");
            DontDestroyOnLoad(obj);
            s_Current = obj.AddComponent<Loom>();
        }

        public static void QueueOnMainThread(UnityAction action)
        {
            QueueOnMainThread(action, 0f);
        }

        public static void QueueOnMainThread(UnityAction action, float time)
        {
            if(0 != time)
            {
                lock(s_Current.m_DelayQueueItems)
                {
                    s_Current.m_DelayQueueItems.Add(new DelayQueueItem { time = Time.time + time, callback = action });
                }
            }
            else
            {
                lock(current.m_Actions)
                {
                    s_Current.m_Actions.Add(action);
                }
            }
        }

        public static Thread RunAsync(UnityAction action)
        {
        }
    }
}

