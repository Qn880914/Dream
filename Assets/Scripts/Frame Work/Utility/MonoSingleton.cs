using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FrameWork.Utility
{
    public abstract class MonoSingleton<T> : MonoBehaviour where T : MonoSingleton<T>
    {
        private static T s_Instance = null;

        public static T instance
        {
            get
            {
                //Scene内にあったら取得
                s_Instance = s_Instance ?? (FindObjectOfType(typeof(T)) as T);
                //TをアタッチしたGameObject生成してT取得
                s_Instance = s_Instance ?? new GameObject(typeof(T).ToString(), typeof(T)).GetComponent<T>();
                return s_Instance;
            }
        }

        private void OnApplicationQuit()
        {
            s_Instance = null;
        }
    }
}
