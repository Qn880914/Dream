using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FrameWork
{
    internal static class ConstantData
    {
        public readonly static string abExtend = ".ab";

        public readonly static bool enableMD5Name = false;

        /// 是否开启检测更新
        public readonly static bool enableCheckUpdate = false;

        public readonly static bool enableAssetBundle = false;

        public readonly static string assetBundleManifestName = "data";


        /// <summary>
        /// 是否启动解压资源
        ///     打包 assetbundle 时使用lzma压缩，则设置为false
        ///     打包 assetbundle 时未压缩，打包完后进行压缩，则下载完资源后需要进行 解压缩
        /// </summary>
        public readonly static bool enableDecompressAsset = false;

        /// assetbundle 相对路径
        public readonly static string assetBundlePath = "ab";

        private static string m_AssetBundleAbsolutePath;
        public static string assetBundleAbsolutePath
        {
            get
            {
                if (string.IsNullOrEmpty(m_AssetBundleAbsolutePath))
                    m_AssetBundleAbsolutePath = string.Format("{0}/{1}", Application.streamingAssetsPath, assetBundlePath);

                return m_AssetBundleAbsolutePath;
            }
        }

        /// 解压资源绝对路径
        private static string m_DecompressPath;
        public static string decompressPath
        {
            get
            {
                if(string.IsNullOrEmpty(m_DecompressPath))
                    m_DecompressPath = string.Format("{0}/{1}", Application.persistentDataPath, abPath);

                return m_DecompressPath;
            }
        }

        /// 更新资源绝对路径
        private static string m_DownloadResourceSavePath;
        public static string downloadResourceSavePath
        {
            get
            {
                if (string.IsNullOrEmpty(m_DownloadResourceSavePath))
                    m_DownloadResourceSavePath = string.Format("{0}/download", Application.persistentDataPath);
                return m_DownloadResourceSavePath;
            }
        }

        /// assetbundle 缓存时间
        public static float assetBundleCacheTime = 10;
    }
}
