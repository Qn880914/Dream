using System;
using System.IO;
using System.Runtime.InteropServices;

namespace FrameWork.Helper
{
    /// <summary>
    /// use for 
    ///     Compress : byte[] \ filePath
    ///     UnCompress : byte[] \ filePath
    /// </summary>
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
#if (UNITY_ANDROID || UNITY_STANDALONE_LINUX) && (!UNITY_EDITOR || UNITY_EDITOR_LINUX)
        private const string LIB_NAME = "lzma";
#elif UNITY_EDITOR || UNITY_STANDALONE_WIN || UNITY_STANDALONE_OSX
        private const string LIB_NAME = "liblzma";
#endif

        [DllImport(LIB_NAME, EntryPoint = "_releaseBuffer")]
        private static extern void _releaseBuffer(IntPtr buffer);

        [DllImport(LIB_NAME, EntryPoint = "Lzma_Compress")]
        private static extern IntPtr Lzma_Compress(IntPtr buffer, int bufferLength, bool makeHeader, ref int compressSize, IntPtr Props);

        [DllImport(LIB_NAME, EntryPoint = "Lzma_Uncompress")]
        private static extern int Lzma_Uncompress(IntPtr buffer, int bufferLength, int uncompressedSize, IntPtr outbuffer, bool useHeader);

#endif

        private static byte[] s_OutBuffer = new byte[1024];
        private static int[] s_Props = new int[7];
        private static bool s_DefaultSet = false;

        private static void SetProps(int level = 5, int dictSize = 16777216, int lc = 3, int lp = 0, int pb = 2, int fb = 32, int numThreads = 4)
        {
            s_Props[0] = level;
            s_Props[1] = dictSize;
            s_Props[2] = lc;
            s_Props[3] = lp;
            s_Props[4] = pb;
            s_Props[5] = fb;
            s_Props[6] = numThreads;

            s_DefaultSet = true;
        }

        /// <summary>
        /// Compress -> byte[]
        /// </summary>
        /// <param name="inBuffer"> Compress Origin -> byte[] </param>
        /// <param name="outBuffer"> Compress result -> byte[] </param>
        /// <returns></returns>
        public static int Compress(byte[] inBuffer, ref byte[] outBuffer)
        {
            if (!s_DefaultSet)
                SetProps();

            GCHandle props = GCHandle.Alloc(s_Props, GCHandleType.Pinned);
            GCHandle cBuf = GCHandle.Alloc(inBuffer, GCHandleType.Pinned);

            int compressSize = 0;
            IntPtr ptr = Lzma_Compress(cBuf.AddrOfPinnedObject(), inBuffer.Length, true, ref compressSize, props.AddrOfPinnedObject());

            props.Free();
            cBuf.Free();

            if(0 == compressSize || IntPtr.Zero == ptr)
            {
                _releaseBuffer(ptr);
                return 0;
            }

            if (outBuffer.Length < compressSize)
                Array.Resize(ref outBuffer, compressSize);

            Marshal.Copy(ptr, outBuffer, 0, compressSize);
            _releaseBuffer(ptr);

            return compressSize;
        }

        /// <summary>
        /// UnCompress -> byte[]
        /// </summary>
        /// <param name="inBuffer"> UnCompress Origin -> byte[] </param>
        /// <param name="outBuffer"> UnCompress Result -> byte[] </param>
        /// <returns></returns>
        public static int Decompress(byte[] inBuffer, ref byte[] outBuffer)
        {
            int unCompressedSize = (int)BitConverter.ToUInt64(inBuffer, 5);

            if (outBuffer.Length < unCompressedSize)
                Array.Resize(ref outBuffer, unCompressedSize);

            GCHandle cbuf = GCHandle.Alloc(inBuffer, GCHandleType.Pinned);
            GCHandle obuf = GCHandle.Alloc(outBuffer, GCHandleType.Pinned);

            int ret = Lzma_Uncompress(cbuf.AddrOfPinnedObject(), inBuffer.Length, unCompressedSize, obuf.AddrOfPinnedObject(), true);

            cbuf.Free();
            obuf.Free();

            if (0 != ret)
                return 0;

            return unCompressedSize;
        }

        /// <summary>
        /// Compress -> filepath
        /// </summary>
        /// <param name="inPath"> Compress Oringin -> filePath </param>
        /// <param name="outPath"> Compress Result -> filePath </param>
        /// <returns></returns>
        public static bool Compress(string inPath, string outPath)
        {
            byte[] inBuffer = File.ReadAllBytes(inPath);
            byte[] outBuffer = s_OutBuffer;

            int size = Compress(inBuffer, ref outBuffer);
            if (size == 0)
            {
                return false;
            }

            Stream stream = File.OpenWrite(outPath);
            stream.Write(outBuffer, 0, size);
            stream.Close();

            return true;
        }

        /// <summary>
        /// UnCompress -> filepath
        /// </summary>
        /// <param name="inPath"> UnCompress origin -> filepath </param>
        /// <param name="outPath"> UnCompress result -> filepath </param>
        /// <returns></returns>
        public static bool Decompress(string inPath, string outPath)
        {
            byte[] inBuffer = File.ReadAllBytes(inPath);
            byte[] outBuffer = s_OutBuffer;

            int size = Decompress(inBuffer, ref outBuffer);
            if (size == 0)
            {
                return false;
            }

            Stream outStream = File.OpenWrite(outPath);
            outStream.Write(outBuffer, 0, size);
            outStream.Close();

            return true;
        }
    }
}
