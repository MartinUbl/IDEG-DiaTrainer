using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Security;
using System.Text;

namespace IDEG_DiaTrainer.scgms
{
    public class DrawingFilterInspection : ReferencedObject
    {
        public enum DrawingType : ushort
        {
            Graph = 0,
            Day = 1,
            Parkes = 2,
            Clark = 3,
            AGP = 4,
            ECDF = 5,
            Profile_Glucose = 6,
            Profile_Insulin = 7,
            Profile_Carbs = 8,
        }

        public enum Diagnosis : ushort
        {
            Type1 = 0,
            Type2 = 1,
            Gestational = 2,

            NotSpecified = Type1,
        }

        [DllImport("interop-inspector", EntryPoint = "scgms_drawing__new_data_available")]
        private static extern int internal_NewDataAvailable(IntPtr obj);

        [DllImport("interop-inspector", EntryPoint = "scgms_drawing__draw")]
        private static extern int internal_Draw(IntPtr obj, ushort type, ushort diagnosis, IntPtr svg, IntPtr segmentIds, IntPtr signalIds);

        public DrawingFilterInspection(IntPtr nativeAddr) : base(nativeAddr)
        {
            //
        }

        public bool NewDataAvailable()
        {
            return internal_NewDataAvailable(NativeHandle.Value) == HResult.S_OK;
        }

        public string Draw(DrawingType type, Diagnosis diagnosis, List<ulong> segmentIds = null, List<Guid> signalIds = null)
        {
            IntPtr svgTarget = ComInterop.CreateStringContainer();

            var res = internal_Draw(NativeHandle.Value, (ushort)type, (ushort)diagnosis, svgTarget, (IntPtr)0, (IntPtr)0);

            if (res != HResult.S_OK)
                return "";

            string str = ComInterop.ExtractStringContainer(svgTarget);

            return str;
        }
    }
}
