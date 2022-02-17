using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

namespace IDEG_DiaTrainer.scgms
{
    public class Native
    {
        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        public delegate void TInterop_Callback(IntPtr scgmsEvent);

        public delegate void TFilter_Created_Callback(IntPtr filterPtr, IntPtr dataPtr);

        /*
         * Executes given SmartCGMS configuration;
         * callback could be null or a valid interop function pointer to a method with TInterop_Callback signature
         * 
         * returns null on error, or a valid pointer to scgms_execution_t
         */
        [DllImport("Platforms/Windows/lib/x64/scgms", EntryPoint = "Execute_SCGMS_Configuration", CharSet = CharSet.Ansi)]
        public static extern UIntPtr Execute_SCGMS_Configuration(StringBuilder configStr, IntPtr callback, IntPtr filterCreatedCallback);

        /*
         * Injects an event to a running filter chain identified by a given scgmsExecutionPtr;
         * scgmsEvent must point to a valid scgms event structure
         * 
         * returns 1 on success, 0 on failure
         */
        [DllImport("scgms", EntryPoint = "Inject_SCGMS_Event")]
        public static extern int Inject_SCGMS_Event(UIntPtr scgmsExecutionPtr, IntPtr scgmsEvent);

        /*
         * Shuts down a running filter chain identified by a given scgmsExecutionPtr;
         * waitForShutdown should be set to 1 if the method should block until the chain is completely shut down,
         * otherwise use 0 to indicate non-blocking request
         */
        [DllImport("scgms", EntryPoint = "Shutdown_SCGMS")]
        public static extern void Shutdown_SCGMS(UIntPtr scgmsExecutionPtr, int waitForShutdown);
    }
}
