using System;

namespace Memory.API
{
    public class APIObject : IDisposable
    {
        private bool _isDisposed;
        private readonly int _id;
        
        public APIObject(int id)
        {
            MagicAPI.Allocate(id);
            _id = id;
        }

        ~APIObject()
        {
            Dispose(true);
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool isFromDisposeMethod)
        {
            if (_isDisposed) return;
            if (isFromDisposeMethod)
                MagicAPI.Free(_id);
            _isDisposed = true;
        }
    }
}
