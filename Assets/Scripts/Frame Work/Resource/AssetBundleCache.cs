using UnityEngine;

namespace FrameWork.Resource
{
    public sealed class AssetBundleCache 
    {
        /// AssetBundle name
        private string m_Name;
        public string name { get { return m_Name; } }

        /// 引用计数
        private int m_ReferenceCount;
        public int refCount
        {
            get { return m_ReferenceCount; }
            set
            {
                m_ReferenceCount = value;

                if (canRemove)
                {
                    if (0 == ConstantData.assetBundleCacheTime)
                        UnLoad();
                    else
                        m_UnloadTime = Time.realtimeSinceStartup;
                }
                else
                    m_UnloadTime = 0;
            }
        }

        public bool canRemove { get { return (!m_Persistent && m_ReferenceCount <= 0); } }

        /// 释放时间
        private float m_UnloadTime;

        private AssetBundle m_AssetBundle;
        public AssetBundle assetBundle { get { return m_AssetBundle; } }

        private bool m_Persistent;
        public bool persistent { get { return m_Persistent; } set { m_Persistent = value; } }

        public bool isTimeOut { get { return Time.realtimeSinceStartup - m_UnloadTime >= ConstantData.assetBundleCacheTime; } }

        public AssetBundleCache(string name, AssetBundle assetBundle, bool persistent, int refCount = 1)
        {
            m_Name = name;
            m_AssetBundle = assetBundle;
            m_Persistent = persistent;
            m_ReferenceCount = refCount;
        }

        public Object LoadAsset(string name)
        {
            return LoadAsset(name, typeof(Object));
        }

        public Object LoadAsset(string name, System.Type type)
        {
            if (null == m_AssetBundle)
                return null;

            return m_AssetBundle.LoadAsset(name, type);
        }

        public T LoadAsset<T>(string name) where T : Object
        {
            return (T)(LoadAsset(name, typeof(T)));
        }

        public void UnLoad(bool unloadUnusedAsset = false)
        {
            if (null == m_AssetBundle)
                return;

            bool unloadAll = false;

            // 图集总是卸载不掉，所以图集强制卸载
            if (m_Name.Contains("altas"))
                unloadAll = true;

            m_AssetBundle.Unload(unloadAll);
            m_AssetBundle = null;

            if (unloadUnusedAsset)
                Resources.UnloadUnusedAssets();
        }
    }
}
