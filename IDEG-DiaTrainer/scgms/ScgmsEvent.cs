using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

namespace IDEG_DiaTrainer.scgms
{
    public class ScgmsEvent
    {
        public EventCode eventCode;
        public Guid deviceId;
        public Guid signalId;
        public double deviceTime;
        public long logicalTime;
        public ulong segmentId;
        public double level;
        public double[] parameters = null;
        public string infoString = null;

        public IntPtr? additionalMemoryToBeFreed;

        public ScgmsEvent()
        {
            //
        }

        ~ScgmsEvent()
        {
            if (additionalMemoryToBeFreed.HasValue)
                Marshal.FreeHGlobal(additionalMemoryToBeFreed.Value);
        }

        private static double Reinterpret_As_Double(long input)
        {
            unsafe
            {
                long* drefptr = &input;
                return *(double*)drefptr;
            }
        }

        private static long Reinterpret_As_Long(double input)
        {
            unsafe
            {
                double* drefptr = &input;
                return *(long*)drefptr;
            }
        }

        public static unsafe ScgmsEvent From_Memory(IntPtr src)
        {
            // size of pointer and size_t (should be the same size, as both most probably represent a word/register size)
            int sizeOfPtr = IntPtr.Size;

            ScgmsEvent evt = new ScgmsEvent();

            byte[] guidBytes = new byte[16];

            evt.eventCode = (EventCode)Marshal.ReadByte(src + 0);

            Marshal.Copy(src + 1, guidBytes, 0, 16);
            evt.deviceId = new Guid(guidBytes);

            Marshal.Copy(src + 17, guidBytes, 0, 16);
            evt.signalId = new Guid(guidBytes);

            evt.deviceTime = Reinterpret_As_Double(Marshal.ReadInt64(src + 33));
            evt.logicalTime = Marshal.ReadInt64(src + 41);
            evt.segmentId = (ulong)Marshal.ReadInt64(src + 49);
            evt.level = Reinterpret_As_Double(Marshal.ReadInt64(src + 57));

            var paramPtr = Marshal.ReadIntPtr(src + 65);
            var paramCnt = (long)Marshal.ReadIntPtr(src + 65 + sizeOfPtr);
            if (paramCnt > 0)
            {
                evt.parameters = new double[paramCnt];

                for (int i = 0; i < paramCnt; i++)
                    evt.parameters[i] = Reinterpret_As_Double(Marshal.ReadInt64(paramPtr + i * 8));
            }

            var strPtr = Marshal.ReadIntPtr(src + 65 + 2 * sizeOfPtr);
            if (strPtr.ToPointer() != null)
                evt.infoString = Marshal.PtrToStringUni(strPtr);

            return evt;
        }

        public static unsafe IntPtr To_Memory(ScgmsEvent src)
        {
            // size of pointer and size_t (should be the same size, as both most probably represent a word/register size)
            int sizeOfPtr = IntPtr.Size;

            IntPtr dst = Marshal.AllocHGlobal(65 + 3 * sizeof(IntPtr));

            Marshal.WriteByte(dst, (byte)src.eventCode);
            Marshal.Copy(src.deviceId.ToByteArray(), 0, dst + 1, 16);
            Marshal.Copy(src.signalId.ToByteArray(), 0, dst + 17, 16);
            Marshal.WriteInt64(dst + 33, Reinterpret_As_Long(src.deviceTime));
            Marshal.WriteInt64(dst + 41, src.logicalTime);
            Marshal.WriteInt64(dst + 49, (long)src.segmentId);
            Marshal.WriteInt64(dst + 57, Reinterpret_As_Long(src.level));

            if (src.parameters != null)
            {
                IntPtr paramDst = Marshal.AllocHGlobal(8 * src.parameters.Length);
                src.additionalMemoryToBeFreed = paramDst;

                Marshal.WriteIntPtr(dst + 65, paramDst);
                Marshal.WriteIntPtr(dst + 65 + sizeOfPtr, (IntPtr)src.parameters.Length);

                for (int i = 0; i < src.parameters.Length; i++)
                    Marshal.WriteInt64(paramDst + 8 * i, Reinterpret_As_Long(src.parameters[i]));
            }
            else
            {
                Marshal.WriteIntPtr(dst + 65, (IntPtr)0);
                Marshal.WriteIntPtr(dst + 65 + sizeOfPtr, (IntPtr)0);
            }

            if (src.infoString != null)
            {
                IntPtr wstr = ComInterop.StringToWString(src.infoString);
                src.additionalMemoryToBeFreed = wstr;

                Marshal.WriteIntPtr(dst + 65 + 2 * sizeOfPtr, wstr);
            }
            else
                Marshal.WriteIntPtr(dst + 65 + 2 * sizeOfPtr, (IntPtr)0);

            return dst;
        }
    }
}
