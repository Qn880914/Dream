using System;

namespace FrameWork.Utility
{
    /// <summary>
    /// Disposable.
    /// </summary>
    public class Disposable : IDisposable
    {
        // Flag: Has Dispose already been called?
        private bool m_Disposed = false;

        // Public implementation of Dispose pattern callable by consumers.
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if(m_Disposed)
            {
                return;
            }

            if(disposing)
            {
                // Free any other managed objects here.
                //
            }

            // Free any unmanaged objects here.
            //
            m_Disposed = true;
        }
    }
}
