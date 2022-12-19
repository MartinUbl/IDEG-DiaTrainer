using IDEG_DiaTrainer.Messages;
using Microsoft.Maui.Controls;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace IDEG_DiaTrainer.Controllers
{
    public class PumpController : INotifyPropertyChanged
    {
        // percentage of battery discharge per day (values from 0 to 1)
        private static readonly double BatteryDischargePerDay = 1.5; // TODO: adjust this to reflect the reality

        // size of insulin reservoir (in insulin units, U)
        public static readonly double InsulinReservoirSize = 150.0;

        private DateTime _CurrentDateTime = DateTime.MinValue;
        public DateTime CurrentDateTime { get { return _CurrentDateTime; } set { _CurrentDateTime = value; OnPropertyChanged(); } }

        private double _BatteryCharge { get; set; } = 1.0;
        public double BatteryCharge { get { return _BatteryCharge; } set { _BatteryCharge = Math.Max(value, 0.0); OnPropertyChanged(); } }

        private double _RemainingInsulin { get; set; } = InsulinReservoirSize;
        public double RemainingInsulin { get { return _RemainingInsulin; } set { _RemainingInsulin = Math.Max(value, 0.0); OnPropertyChanged(); } }

        public class StoredValue
        {
            public DateTime When;
            public double Value;
        }

        private List<StoredValue> _StoredValues = new List<StoredValue>();
        public List<StoredValue> StoredValues { get { return _StoredValues; } set { _StoredValues = value; OnPropertyChanged(); } }

        private double _CurrentIG = 0.0;
        public double CurrentIG { get { return _CurrentIG; } set { _CurrentIG = Math.Max(value, 0.0); OnPropertyChanged(); } }

        private double _CurrentIOB = 0.0;
        public double CurrentIOB { get { return _CurrentIOB; } set { _CurrentIOB = Math.Max(value, 0.0); OnPropertyChanged(); } }

        public event PropertyChangedEventHandler PropertyChanged;

        public void OnPropertyChanged([CallerMemberName] string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }

        public PumpController()
        {
            MessagingCenter.Subscribe<Messages.ValueAvailableMessage>(this, Messages.ValueAvailableMessage.Name, OnValueAvailable);
        }

        private bool IsInValueHandler = false;

        private void OnValueAvailable(Messages.ValueAvailableMessage msg)
        {
            if (IsInValueHandler)
                return;

            IsInValueHandler = true;

            // process all present values

            if (msg.SignalId == scgms.SignalGuids.InterstitiaryGlucose)
            {
                StoredValues.Add(new StoredValue { When = msg.DeviceTime, Value = msg.Value });

                // remove values older than 1 day
                while (StoredValues.Count > 0 && CurrentDateTime != DateTime.MinValue && (CurrentDateTime - StoredValues[0].When).TotalHours > 24.0)
                    StoredValues.RemoveAt(0);

                CurrentIG = msg.Value;
            }

            if (msg.SignalId == scgms.SignalGuids.DeliveredInsulinBolus)
            {
                RemainingInsulin -= msg.Value;

                // stop basal delivery when the insulin reservoir is depleted
                if (RemainingInsulin == 0)
                    SendBasalRate(0);
            }

            if (msg.SignalId == scgms.SignalGuids.IOB)
                CurrentIOB = msg.Value;
            // discard past values
            if (msg.DeviceTime < CurrentDateTime)
            {
                IsInValueHandler = false;
                return;
            }
            // do not update pump controller variables when the time did not change
            if (msg.DeviceTime == CurrentDateTime)
            {
                IsInValueHandler = false;
                return;
            }

            if (CurrentDateTime != DateTime.MinValue)
            {
                double daysPrev = scgms.Utils.DateTimeToRatTime(CurrentDateTime);
                double daysNow = scgms.Utils.DateTimeToRatTime(msg.DeviceTime);

                BatteryCharge -= BatteryDischargePerDay * (daysNow - daysPrev);

                // stop basal delivery when the battery is depleted
                if (BatteryCharge == 0.0)
                    SendBasalRate(0);
            }

            CurrentDateTime = msg.DeviceTime;
            IsInValueHandler = false;
        }

        public bool SendBolus(double amount)
        {
            // not enough insulin in reservoir
            if (RemainingInsulin < amount)
                return false;

            MessagingCenter.Send<InjectBolusMessage>(new InjectBolusMessage
            {
                BolusAmount = amount,
                When = null,
            }, InjectBolusMessage.Name);

            return true;
        }

        public bool SendBasalRate(double rate)
        {
            // reservoir should have insulin for at least 10 minutes (TODO: confirm this with real pumps)
            if (rate > 0 && RemainingInsulin < rate / 6.0)
                return false;

            MessagingCenter.Send<InjectBasalMessage>(new InjectBasalMessage
            {
                BasalRate = rate,
                When = null,
            }, InjectBasalMessage.Name);

            return true;
        }
    }
}
