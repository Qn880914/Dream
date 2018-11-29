using UnityEngine;

namespace FrameWork.Resource
{
    public class AssetLoader : Loader
    {
        private Object m_Data;

        public AssetLoader() : base(LoaderType.Asset)
        { }

        public override void Start()
        {
            base.Start();

#if UNITY_EDITOR
            System.Type type = m_Param as System.Type;
            if(null == type)
                type = typeof(Object);

            if(m_Async)
                m_Data = UnityEditor.AssetDatabase.LoadAssetAtPath(m_Path, type);
            else
            {
                Object data = UnityEditor.AssetDatabase.LoadAssetAtPath(m_Path, type);
                OnCompleted(data);
            }
#else
            if(!m_Async)
            {
                OnCompleted(null);
            }
#endif
        }

        public override void Update()
        {
            if(m_State == LoaderState.Loading)
            {
                OnCompleted(m_Data);
                m_Data = null;
            }
        }
    }
}
