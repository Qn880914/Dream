using FrameWork.Utility;

namespace FrameWork.Helper
{
    public sealed class LZMACompressRequest : Disposable
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

        public static LZMACompressRequest CreateCompress(byte[] data)
        {
            LZMACompressRequest request = new LZMACompressRequest();
            request.Compress(data);

            return request;
        }

        public static LZMACompressRequest CreateDecompress(byte[] data)
        {
            LZMACompressRequest request = new LZMACompressRequest();
            request.Decompress(data);

            return request;
        }

        public void Compress(byte[] data)
        {
            Loom.instance.RunAsync(()=> 
            {
                try
                {
                    m_Bytes = new byte[1];
                    int size = LZMAHelper.Compress(data, ref m_Bytes);
                    if (0 == size)
                        m_Error = "Compress Failed";
                }
                catch (System.Exception e)
                {
                    m_Error = e.Message;
                }
                finally
                {
                    Loom.instance.QueueOnMainThread(OnComplete);
                }
            });
        }

        public void Decompress(byte [] data)
        {
            Loom.instance.RunAsync(()=>
            {
                try
                {
                    m_Bytes = new byte[1];
                    int size = LZMAHelper.Decompress(data, ref m_Bytes);
                    if (0 == size)
                        m_Error = "Decompress Failed";
                }
                catch (System.Exception e)
                {
                    m_Error = e.Message;
                }
                finally
                {
                    Loom.instance.QueueOnMainThread(OnComplete);
                }
            });
        }

        private void OnComplete()
        {
            m_IsDone = true;
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
        }
    }
}

