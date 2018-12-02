using FrameWork.Helper;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FrameWork.Resource
{
    public class AssetBundleLoader : Loader
    {
        private AssetBundleCreateRequest m_AssetBundleCreateRequest;

        private LZMACompressRequest m_DecompressRequest = null;

        private bool m_NeedDepack = false;

        private int m_CurrentStage = 0;

        private int m_CountStage = 1;

        public AssetBundleLoader():base(LoaderType.Bundle)
        { }

        public override void Start()
        {
            base.Start();

            //string path = 
        }

        public override void Reset()
        {
            base.Reset();

            m_AssetBundleCreateRequest = null;
            m_DecompressRequest = null;

            m_CurrentStage = 0;
            m_CountStage = 1;
        }
    }
}
