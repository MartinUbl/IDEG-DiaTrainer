using Microsoft.Maui.Controls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IDEG_DiaTrainer.Controllers
{
    public class SimulationController
    {
        scgms.Execution Exec;

        scgms.DrawingFilterInspection DrawingFilter = null;

        static readonly ulong SimSegmentId = 1;

        long SimulationTimeCounter = 0;

        public SimulationController()
        {
            //
        }

        private void ExecuteCallback(scgms.ScgmsEvent evt)
        {
            if (evt.eventCode == scgms.EventCode.Level)
            {
                MessagingCenter.Send(new Messages.ValueAvailableMessage {
                    SignalId = evt.signalId,
                    DeviceTime = scgms.Utils.RatTimeToDateTime(evt.deviceTime),
                    Value = evt.level
                }, Messages.ValueAvailableMessage.Name);

                if (DrawingFilter != null && DrawingFilter.NewDataAvailable())
                    MessagingCenter.Send(new Messages.DrawingAvailableMessage { }, Messages.DrawingAvailableMessage.Name);
            }
        }

        private bool TimerCallback()
        {
            scgms.ScgmsEvent evt = new scgms.ScgmsEvent();
            evt.eventCode = scgms.EventCode.Level;
            evt.segmentId = SimSegmentId;
            evt.signalId = scgms.SignalGuids.Synchronization;
            evt.level = 0;
            evt.deviceTime = scgms.Utils.UnixTimeToRatTime((long)(DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1))).TotalSeconds + 5 * 60 * SimulationTimeCounter + 1);
            Exec.InjectEvent(evt);

            SimulationTimeCounter++;

            return true;
        }

        public void SetDrawingSize(scgms.DrawingFilterInspection.DrawingType type, int width, int height)
        {
            // NOTE: this should not be necessary once new drawing interface gets adapted

            scgms.ScgmsEvent evt = new scgms.ScgmsEvent();
            evt.eventCode = scgms.EventCode.Information;
            evt.segmentId = SimSegmentId;
            evt.infoString = "DrawingResize=" + (int)type + ",Width=" + width + ",Height=" + height;
            evt.deviceTime = scgms.Utils.UnixTimeToRatTime((long)(DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1))).TotalSeconds + 5 * 60 * SimulationTimeCounter + 1);
            Exec.InjectEvent(evt);
        }

        public string GetDrawing(scgms.DrawingFilterInspection.DrawingType type)
        {
            return DrawingFilter.Draw(type, scgms.DrawingFilterInspection.Diagnosis.NotSpecified, null, null);
        }

        public bool Start(int patient_id = -1)
        {
            Exec = new scgms.Execution();

            Exec.RegisterCallback(ExecuteCallback);
            // TODO: differentiate between patients, use different configs with different models (and different parameters)
            if (!Exec.Start("config-s2013"))
                return false;

            var draw = Exec.GetSingleFilterWithInterface(scgms.EntityGuids.IID_DrawingFilterInspection);
            if (!draw.HasValue)
                return false;

            DrawingFilter = new scgms.DrawingFilterInspection(draw.Value);

            Device.StartTimer(TimeSpan.FromSeconds(1), TimerCallback);

            return true;
        }
    }
}
