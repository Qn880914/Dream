using FrameWork.Resource;
using FrameWork.Utility;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FrameWork.Manager
{
    public class LoaderManager : Singleton<LoaderManager>
    {
        private LoadTask m_LoadTask;

        private AssetBundleManifest m_AssetBundleManifest;

        private AssetBundleMapping m_AssetBundleMapping;

        /// 安装包附带的资源
        private Dictionary<string, string> m_DicAssetBundlePathMapMD5Origin = new Dictionary<string, string>();

        private Dictionary<string, string> m_DicAssetBundlePathMapMD5Download = new Dictionary<string, string>();

        private float m_LastClearCacheTime;
    }
}

