using IDEG_DiaTrainer.Config;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Text;

namespace IDEG_DiaTrainer.scgms
{
    public class Execution : IDisposable
    {
        public delegate void TExecution_Callback(scgms.ScgmsEvent scgmsEvent);

        private UIntPtr ExecutionPtr;

        private scgms.Native.TInterop_Callback InteropCallback;
        //private GCHandle InteropListenerDelegateHandle;
        private IntPtr InteropListenerFunctionPointer;

        private scgms.Native.TFilter_Created_Callback FilterCreatedCallback;
        //private GCHandle FilterCreatedDelegateHandle;
        private IntPtr FilterCreatedFunctionPointer;

        private TExecution_Callback ExportedCallback;

        private List<IntPtr> Filters = new List<IntPtr>();

        public bool IsRunning { get; private set; }

        public Execution()
        {
            //
        }

        public void Dispose()
        {
            if (InteropCallback != null)
            {
                // free listener handle
                lock (this)
                {
                    //InteropListenerDelegateHandle.Free();
                    InteropCallback = null;
                }
            }
        }

        public void RegisterCallback(TExecution_Callback callback)
        {
            ExportedCallback = callback;
        }

        private void SCGMS_Execution_Callback(IntPtr scgmsEvent)
        {
            var evt = scgms.ScgmsEvent.From_Memory(scgmsEvent);

            if (evt.eventCode == EventCode.Shut_Down)
                IsRunning = false;

            ExportedCallback?.Invoke(evt);
        }

        private unsafe void SCGMS_Filter_Created_Callback(IntPtr filter, IntPtr data)
        {
            Filters.Add(filter);
        }

        public List<IntPtr> GetFiltersWithInterface(Guid iid)
        {
            List<IntPtr> result = new List<IntPtr>();

            int i = 1;
            foreach (var filter in Filters)
            {
                if (i != 1) // HACK!!!
                {
                    IntPtr resPtr;
                    if (ComInterop.QueryInterface(filter, iid, out resPtr))
                        result.Add(resPtr);
                }

                i++;
            }

            return result;
        }

        public IntPtr? GetSingleFilterWithInterface(Guid iid)
        {
            var list = GetFiltersWithInterface(iid);
            if (list.Count == 0)
                return null;

            return list[0];
        }

        public unsafe bool Start(string configName)
        {
            if (IsRunning)
                throw new InvalidOperationException("This SCGMS Execution instance has already been started");

            Filters.Clear();

            var config = ConfigMgr.ReadConfig(configName);
            StringBuilder sb = new StringBuilder(config);

            InteropCallback = SCGMS_Execution_Callback;
            //InteropListenerDelegateHandle = GCHandle.Alloc(InteropCallback, GCHandleType.Pinned);
            InteropListenerFunctionPointer = Marshal.GetFunctionPointerForDelegate(InteropCallback);

            FilterCreatedCallback = SCGMS_Filter_Created_Callback;
            //FilterCreatedDelegateHandle = GCHandle.Alloc(FilterCreatedCallback, GCHandleType.Pinned);
            FilterCreatedFunctionPointer = Marshal.GetFunctionPointerForDelegate(FilterCreatedCallback);

            ExecutionPtr = scgms.Native.Execute_SCGMS_Configuration(sb, InteropListenerFunctionPointer, FilterCreatedFunctionPointer);

            IsRunning = ExecutionPtr.ToPointer() != null;

            return IsRunning;
        }

        public bool InjectEvent(scgms.ScgmsEvent evt)
        {
            if (!IsRunning)
                return false;

            int result = scgms.Native.Inject_SCGMS_Event(ExecutionPtr, scgms.ScgmsEvent.To_Memory(evt));

            return result == 1;
        }

        public void ShutDown(bool waitForShutDown)
        {
            if (!IsRunning)
                return;

            scgms.Native.Shutdown_SCGMS(ExecutionPtr, waitForShutDown ? 1 : 0);
            IsRunning = false;
        }
    }
}
