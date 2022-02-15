using System;
using System.Collections.Generic;
using System.Text;

namespace IDEG_DiaTrainer.scgms
{
    public class SignalGuids
    {
        public static readonly Guid BloodGlucose = new Guid("F666F6C2-D7C0-43E8-8EE1-C8CAA8F860E5");
        public static readonly Guid InterstitiaryGlucose = new Guid("3034568D-F498-455B-AC6A-BCF301F69C9E");
        public static readonly Guid RequestedInsulinBolus = new Guid("09B16B4A-54C2-4C6A-948A-3DEF8533059B");
        public static readonly Guid RequestedInsulinBasalRate = new Guid("B5897BBD-1E32-408A-A0D5-C5BFECF447D9");
        public static readonly Guid Virtual_99 = new Guid("79EDF100-B0A2-4EE7-AB17-1637418DB15A"); // temp for IG prediction

        public static readonly Guid Virtual_90 = new Guid("1FA03911-A62D-4ECF-AFE8-60B8379151B8"); // 5min IG prediction signal
        public static readonly Guid Virtual_91 = new Guid("56C37AF2-FA68-43EF-88B6-8EA28E957824"); // 10min IG prediction signal
        public static readonly Guid Virtual_92 = new Guid("E959B878-74E4-479F-AECE-7AF2F1454498"); // 15min IG prediction signal
        public static readonly Guid Virtual_93 = new Guid("B6C0CAA3-01A3-402D-823D-57FF8F2D9C45"); // 20min IG prediction signal
        public static readonly Guid Virtual_94 = new Guid("173FFBB1-51BC-4E9B-9119-5B95EED29042"); // 25min IG prediction signal
        public static readonly Guid Virtual_95 = new Guid("BBE13567-C1F8-489D-8416-AB3565DE4F67"); // 30min IG prediction signal

        public static readonly Guid DeliveredInsulinBolus = new Guid("22D87566-AF1B-4CC7-8D11-C5E04E1E9C8A");
        public static readonly Guid DeliveredInsulinBasalRate = new Guid("BF88A8CB-1290-4477-A2CF-BDD06DF628AB");
        public static readonly Guid CarbIntake = new Guid("37AA6AC1-6984-4A06-92CC-A660110D0DC7");
        public static readonly Guid CarbRescue = new Guid("F24920F7-3F7B-4000-B2D0-374F940E4898");
        public static readonly Guid PhysicalActivity = new Guid("F4438E9A-DD52-45BD-83CE-5E93615E62BD");
        public static readonly Guid Calibration = new Guid("ED4CD0F5-F728-44FE-9552-97338BD7E8D5");

        public static readonly Guid IOB = new Guid("313A1C11-6BAC-46E2-8938-7353409F2FCD");
        public static readonly Guid COB = new Guid("B74AA581-538C-4B30-B764-5BD0D97B0727");

        public static readonly Guid Synchronization = new Guid("97F2D189-2319-4A4A-8CBE-5DDDD6005F81");

        public static readonly Guid BatteryCharge = new Guid("09756178-FB99-4FEB-A9AC-DF37462CC753");

    }
}
