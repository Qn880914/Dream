using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FrameWork.Helper
{
    public sealed class LZMAHelper
    {
#if (UNITY_IOS || UNITY_WEBGL || UNITY_IPHONE) && !UNITY_EDITOR

        [DllImport("__Internal")]
        private static extern void _releaseBuffer(IntPtr buffer);

        [DllImport("__Internal")]
        private static extern IntPtr Lzma_Compress(IntPtr buffer, int bufferLength, bool makeHeader, ref int v, IntPtr Props);

        [DllImport("__Internal")]
        private static extern int Lzma_Uncompress(IntPtr buffer, int bufferLength, int uncompressedSize, IntPtr outbuffer, bool useHeader);

#else
#if (UNITY_ANDROID || UNITY_STANDALONE_LINUX)
#elif
#endif
#endif
    }
}
