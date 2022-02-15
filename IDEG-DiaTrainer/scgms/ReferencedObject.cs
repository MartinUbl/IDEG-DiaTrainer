using System;
using System.Collections.Generic;
using System.Text;

namespace IDEG_DiaTrainer.scgms
{
    public class ReferencedObject
    {
        public IntPtr? NativeHandle { get; private set; }

        public ReferencedObject(IntPtr objHandle, bool addRef = true)
        {
            NativeHandle = objHandle;

            if (addRef)
                ComInterop.AddRef(objHandle);
        }

        ~ReferencedObject()
        {
            Release();
        }

        public void Release()
        {
            if (NativeHandle.HasValue)
            {
                ComInterop.Release(NativeHandle.Value);
                NativeHandle = null;
            }
        }
    }
}
