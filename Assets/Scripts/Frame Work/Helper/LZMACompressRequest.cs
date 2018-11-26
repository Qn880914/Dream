using FrameWork.Utility;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FrameWork.Helper
{
    public class LZMACompressRequest : Disposable
    {
        private byte[] m_Bytes;
        public byte[] bytes { get { return m_Bytes; } }

        private float m_Progress;
        public float progress { get { return m_Progress; } }

        private bool m_IsDone;
        public bool isDone { get { return m_IsDone; } }

        private string m_Error;
        public string error { get { return m_Error; } }

        public LZMACompressRequest() { }

        private void OnComplete()
        { }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
        }
    }
}

