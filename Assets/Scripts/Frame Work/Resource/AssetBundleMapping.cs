using System.Collections.Generic;
using UnityEngine;

namespace FrameWork.Resource
{
    public class AssetBundleMapping : ScriptableObject
    {
        [System.Serializable]
        public class AssetBundleInfo
        {
            /// assetbundle name 
            public string name;

            /// file names included in assetbundle
            public string[] files;
        }

        private AssetBundleInfo[] m_AssetBundleInfos;
        public AssetBundleInfo [] assetBundleInfos { get { return m_AssetBundleInfos; } }

        private Dictionary<string, string> m_DicFileNameMapAssetBundleName;

        public void Init()
        {
            m_DicFileNameMapAssetBundleName = new Dictionary<string, string>();

            int count = m_AssetBundleInfos.Length;
            for(int i = 0; i < count; ++ i)
            {
                AssetBundleInfo info = m_AssetBundleInfos[i];
                int fileCount = info.files.Length;
                for(int j = 0; j < fileCount; ++ j)
                {
                    string fileName = info.files[j];
                    if(!m_DicFileNameMapAssetBundleName.ContainsKey(fileName))
                        m_DicFileNameMapAssetBundleName.Add(fileName, info.name);
                    else
                        UnityEngine.Debug.LogError(string.Format("[AssetBundleMapping.Init] Repet FileName : {}   in AssetBundle : {1}",
                            fileName, info.name));
                }
            }
        }

        public string GetAssetBundleNameFromAssetPath(string path)
        {
            string assetBundleName = string.Empty;
            path = path.ToLower();
            if (path.Contains("/"))
                path = path.Substring(path.IndexOf('/') + 1);

            if (!m_DicFileNameMapAssetBundleName.TryGetValue(path, out assetBundleName))
                UnityEngine.Debug.Log(string.Format("[AssetBundleMapping.GetAssetBundleNameFromAssetPath] Can not found Asset : {0}",
                    path));
            else
                assetBundleName = string.Format("{0}{1}", assetBundleName, ConstantData.abExtend);

            return assetBundleName;
        }

    }
}
