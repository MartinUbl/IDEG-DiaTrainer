using IDEG_DiaTrainer.Messages;
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

        private bool Paused = false;

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

        private void InjectLevelEvent(Guid signalId, double level, DateTime? when)
        {
            scgms.ScgmsEvent evt = new scgms.ScgmsEvent();
            evt.eventCode = scgms.EventCode.Level;
            evt.segmentId = SimSegmentId;
            evt.signalId = signalId;
            evt.level = level;
            evt.deviceTime = when.HasValue ?
                scgms.Utils.UnixTimeToRatTime((long)(when.Value.Subtract(new DateTime(1970, 1, 1))).TotalSeconds)
                :
                scgms.Utils.UnixTimeToRatTime((long)(DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1))).TotalSeconds + 5 * 60 * SimulationTimeCounter + 1);
            Exec.InjectEvent(evt);
        }

        private bool TimerCallback()
        {
            if (Paused)
                return true;

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

            Paused = false;

            DrawingFilter = new scgms.DrawingFilterInspection(draw.Value);

            Device.StartTimer(TimeSpan.FromSeconds(1), TimerCallback);

            MessagingCenter.Subscribe<InjectCarbsMessage>(this, InjectCarbsMessage.Name, ProcessInjectCarbsMessage);

            return true;
        }

        public void Pause()
        {
            if (Exec == null || Paused)
                return;
        }

        public void Resume()
        {
            if (Exec == null || !Paused)
                return;
        }

        public bool IsPaused()
        {
            return Paused;
        }

        public void ProcessInjectCarbsMessage(InjectCarbsMessage msg)
        {
            if (msg.CarbAmount <= 0)
                return;

            InjectLevelEvent(
                msg.IsRescue ? scgms.SignalGuids.CarbRescue : scgms.SignalGuids.CarbIntake,
                msg.CarbAmount,
                msg.When);
        }
    }
}
