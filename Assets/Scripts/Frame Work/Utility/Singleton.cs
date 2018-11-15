﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FrameWork.Utility
{
    public class Singleton<T> : Disposable where T : Singleton<T>
    {
        private static T s_Instance;

        private static object helper_lock = new object();

        public static T instance
        {
            get
            {
                if(null == s_Instance)
                {
                    lock(helper_lock)
                    {
                        if(null == s_Instance)
                        {
                            s_Instance = System.Activator.CreateInstance<T>();
                        }
                    }
                }

                return s_Instance;
            }
        }
    }
}
