using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

namespace IDEG_DiaTrainer.scgms
{
    public class ComInterop
    {
        /*
         * IUnknown
         */

        [DllImport("Platforms/Windows/lib/x64/interop-inspector", EntryPoint = "scgms_query_interface")]
        private static extern int internal_QueryInterface(IntPtr obj, IntPtr guid, out IntPtr iobj);

        [DllImport("Platforms/Windows/lib/x64/interop-inspector", EntryPoint = "scgms_add_ref")]
        private static extern int internal_AddRef(IntPtr obj);

        [DllImport("Platforms/Windows/lib/x64/interop-inspector", EntryPoint = "scgms_release")]
        private static extern int internal_Release(IntPtr obj);

        public static unsafe bool QueryInterface(IntPtr obj, Guid guid, out IntPtr resPtr)
        {
            IntPtr guidPtr;

            fixed (byte* p = guid.ToByteArray())
            {
                guidPtr = (IntPtr)p;
            }

            var res = internal_QueryInterface(obj, guidPtr, out resPtr);

            return res == HResult.S_OK;
        }

        public static unsafe bool AddRef(IntPtr obj)
        {
            if (obj.ToPointer() == null)
                return false;

            return internal_AddRef(obj) == HResult.S_OK;
        }

        public static unsafe bool Release(IntPtr obj)
        {
            if (obj.ToPointer() == null)
                return false;

            return internal_Release(obj) == HResult.S_OK;
        }

        /*
         * Containers
         */

        [DllImport("interop-inspector", EntryPoint = "scgms_create_str_container")]
        private static extern int internal_CreateStringContainer(out IntPtr obj);

        [DllImport("interop-inspector", EntryPoint = "scgms_extract_str_container")]
        private static extern int internal_ExtractStringContainer(IntPtr obj, out IntPtr target);

        [DllImport("interop-inspector", EntryPoint = "scgms_convert_str_to_wstr")]
        private static extern int internal_ConvertStringToWString(IntPtr str, out IntPtr outStr);

        public static unsafe IntPtr CreateStringContainer()
        {
            IntPtr objTarget;
            IntPtr result = (IntPtr)0;

            var res = internal_CreateStringContainer(out objTarget);
            if (res == HResult.S_OK)
                result = objTarget;

            return result;
        }

        public static unsafe string ExtractStringContainer(IntPtr strContainer)
        {
            IntPtr objTarget;

            var res = internal_ExtractStringContainer(strContainer, out objTarget);
            if (res != HResult.S_OK)
                return "";

            byte b, b2, b3, b4;
            b = Marshal.ReadByte(objTarget);
            b2 = Marshal.ReadByte(objTarget + 1);
            b3 = Marshal.ReadByte(objTarget + 2);
            b4 = Marshal.ReadByte(objTarget + 3);

            string str = Marshal.PtrToStringAnsi(objTarget);

            Marshal.FreeHGlobal(objTarget);

            return str;
        }

        public static unsafe IntPtr StringToWString(string str)
        {
            IntPtr strPtr = Marshal.StringToHGlobalAnsi(str);
            IntPtr wstrPtr;

            internal_ConvertStringToWString(strPtr, out wstrPtr);

            Marshal.FreeHGlobal(strPtr);

            return wstrPtr;
        }
    }
}
