using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FrameWork.Utility
{
    public abstract class MonoSingleton<T> : MonoBehaviour where T : MonoSingleton<T>
    {
        private static T s_Instance = null;

        private static object lock_helper = new object();

        public static T instance
        {
            get
            {
                if(null == s_Instance)
                {
                    lock(lock_helper)
                    {
                        s_Instance = FindObjectOfType(typeof(T)) as T; 
                        if(null == s_Instance)
                        {
                            GameObject obj = new GameObject(typeof(T).ToString());
                            s_Instance = obj.AddComponent<T>();
                            DontDestroyOnLoad(obj);
                        }
                    }
                }

                return s_Instance;
            }
        }

        private void OnApplicationQuit()
        {
            s_Instance = null;
        }
    }
}
