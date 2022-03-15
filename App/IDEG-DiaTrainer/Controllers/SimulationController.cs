using IDEG_DiaTrainer.Messages;
using Microsoft.Maui.Controls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IDEG_DiaTrainer.Controllers
{
    /// <summary>
    /// Controller of SmartCGMS simulation
    /// </summary>
    public class SimulationController
    {
        // simulation segment ID - it does not matter for frontends like this
        static readonly ulong SimSegmentId = 1;

        // starting time of simulation (every event should be sent with date relative to this one)
        private DateTime StartTime;

        // execution environment
        private scgms.Execution Exec;

        // inspected drawing filter
        private scgms.DrawingFilterInspection DrawingFilter = null;

        // simulation time counter to properly align all events
        private long SimulationTimeCounter = 0;

        // is the simulation paused?
        private bool Paused = false;

        public SimulationController()
        {
            //
        }

        /// <summary>
        /// Callback for scgms event
        /// </summary>
        /// <param name="evt">reconstructed managed event</param>
        private void ExecuteCallback(scgms.ScgmsEvent evt)
        {
            // Level events get translated into message; it is then broadcast, so every subscriber may receive levels
            if (evt.eventCode == scgms.EventCode.Level)
            {
                MessagingCenter.Send(new Messages.ValueAvailableMessage {
                    SignalId = evt.signalId,
                    DeviceTime = scgms.Utils.RatTimeToDateTime(evt.deviceTime),
                    Value = evt.level
                }, Messages.ValueAvailableMessage.Name);

                // after every level event, there may be an update available of drawings - check it and broadcast a message if there is an update available
                if (DrawingFilter != null && DrawingFilter.NewDataAvailable())
                    MessagingCenter.Send(new Messages.DrawingAvailableMessage { }, Messages.DrawingAvailableMessage.Name);
            }
        }

        /// <summary>
        /// Injects a level event into scgms execution
        /// </summary>
        /// <param name="signalId">GUID of signal - may be one of known SignalGuids</param>
        /// <param name="level">signal level to be injected</param>
        /// <param name="when">datetime (optional)</param>
        private void InjectLevelEvent(Guid signalId, double level, DateTime? when = null)
        {
            scgms.ScgmsEvent evt = new scgms.ScgmsEvent();
            evt.eventCode = scgms.EventCode.Level;
            evt.segmentId = SimSegmentId;
            evt.signalId = signalId;
            evt.level = level;
            evt.deviceTime = when.HasValue ?
                scgms.Utils.UnixTimeToRatTime((long)(when.Value.Subtract(new DateTime(1970, 1, 1))).TotalSeconds)
                :
                // no date set - use offset from base time
                scgms.Utils.UnixTimeToRatTime((long)(StartTime.Subtract(new DateTime(1970, 1, 1))).TotalSeconds + 5 * 60 * SimulationTimeCounter + 1);
            Exec.InjectEvent(evt);
        }

        /// <summary>
        /// Timer callback - this is the tick source for simulation
        /// </summary>
        /// <returns>repeat the timer?</returns>
        private bool TimerCallback()
        {
            if (Paused)
                return true;

            // insert "dummy" signal to be synchronized onto
            InjectLevelEvent(scgms.SignalGuids.Synchronization, 0.0);

            // increase stored simulation time
            SimulationTimeCounter++;

            return true;
        }

        /// <summary>
        /// Sets drawing size - sends resize events to chain
        /// </summary>
        /// <param name="type">drawing type to be resized</param>
        /// <param name="width">requested width</param>
        /// <param name="height">requested height</param>
        public void SetDrawingSize(scgms.DrawingFilterInspection.DrawingType type, int width, int height)
        {
            // NOTE: this should not be necessary once new drawing interface gets adapted

            scgms.ScgmsEvent evt = new scgms.ScgmsEvent();
            evt.eventCode = scgms.EventCode.Information;
            evt.segmentId = SimSegmentId;
            evt.infoString = "DrawingResize=" + (int)type + ",Width=" + width + ",Height=" + height;
            evt.deviceTime = scgms.Utils.UnixTimeToRatTime((long)(StartTime.Subtract(new DateTime(1970, 1, 1))).TotalSeconds + 5 * 60 * SimulationTimeCounter + 1);
            Exec.InjectEvent(evt);
        }

        /// <summary>
        /// Retrieves drawing from drawing filter inspection interface
        /// </summary>
        /// <param name="type">type of drawing to be rendered</param>
        /// <returns>SVG drawing</returns>
        public string GetDrawing(scgms.DrawingFilterInspection.DrawingType type)
        {
            if (DrawingFilter == null)
                return "";

            return DrawingFilter.Draw(type, scgms.DrawingFilterInspection.Diagnosis.NotSpecified, null, null);
        }

        /// <summary>
        /// Creates scgms execution environment and starts the simulation
        /// </summary>
        /// <param name="patient_id">Id of patient to be simulated</param>
        /// <returns>success?</returns>
        public bool Start(int patient_id = -1)
        {
            // is the simulation already in progress?
            if (Exec != null)
                return false;

            Exec = new scgms.Execution();

            StartTime = DateTime.UtcNow;
            StartTime = StartTime.AddHours(-StartTime.Hour).AddMinutes(-StartTime.Minute).AddSeconds(-StartTime.Second);

            StartTime = StartTime.AddHours(6);

            Paused = false;

            Exec.RegisterCallback(ExecuteCallback);
            // TODO: differentiate between patients, use different configs with different models (and different parameters)
            if (!Exec.Start("config-s2013"))
                return false;

            // inspect drawing filter
            var draw = Exec.GetSingleFilterWithInterface(scgms.EntityGuids.IID_DrawingFilterInspection);
            if (draw.HasValue)
            {
                // retrieve drawing filter inspection
                DrawingFilter = new scgms.DrawingFilterInspection(draw.Value);
            }

            // start the tick timer
            Device.StartTimer(TimeSpan.FromMilliseconds(1000), TimerCallback);

            // subscribe for controlling messages
            MessagingCenter.Subscribe<InjectCarbsMessage>(this, InjectCarbsMessage.Name, ProcessInjectCarbsMessage);
            MessagingCenter.Subscribe<InjectBolusMessage>(this, InjectBolusMessage.Name, ProcessInjectBolusMessage);

            return true;
        }

        /// <summary>
        /// Pause the simulation
        /// </summary>
        public void Pause()
        {
            if (Exec == null || Paused)
                return;

            Paused = true;
        }

        /// <summary>
        /// Resumes the simulation
        /// </summary>
        public void Resume()
        {
            if (Exec == null || !Paused)
                return;

            Paused = false;
        }

        /// <summary>
        /// Has the simulation been paused?
        /// </summary>
        /// <returns>simulation pause state</returns>
        public bool IsPaused()
        {
            return Paused;
        }

        /// <summary>
        /// Inject carb message callback
        /// </summary>
        /// <param name="msg"></param>
        public void ProcessInjectCarbsMessage(InjectCarbsMessage msg)
        {
            if (msg.CarbAmount <= 0)
                return;

            InjectLevelEvent(
                msg.IsRescue ? scgms.SignalGuids.CarbRescue : scgms.SignalGuids.CarbIntake,
                msg.CarbAmount,
                msg.When);
        }

        /// <summary>
        /// Inject bolus message callback
        /// </summary>
        /// <param name="msg"></param>
        public void ProcessInjectBolusMessage(InjectBolusMessage msg)
        {
            if (msg.BolusAmount <= 0)
                return;

            InjectLevelEvent(
                scgms.SignalGuids.RequestedInsulinBolus,
                msg.BolusAmount,
                msg.When);
        }
    }
}
