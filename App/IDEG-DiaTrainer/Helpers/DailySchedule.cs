using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IDEG_DiaTrainer.Helpers
{
    public class DailySchedule
    {
        public enum BlockActivity
        {
            None, // marker value

            Normal,
            Sleep,
            Rest,
            Work,
            PhysicalActivity,
        }

        public static string BlockActivityToString(DailySchedule.BlockActivity act)
        {
            switch (act)
            {
                case DailySchedule.BlockActivity.Normal: return "Normal";
                case DailySchedule.BlockActivity.Work: return "Working";
                case DailySchedule.BlockActivity.Rest: return "Resting";
                case DailySchedule.BlockActivity.Sleep: return "Sleeping";
                case DailySchedule.BlockActivity.PhysicalActivity: return "Exercising";
            }

            return "None";
        }

        public enum EventActivity
        {
            None, // marker value

            Meal,
        }

        public static string EventActivityToString(DailySchedule.EventActivity act)
        {
            switch (act)
            {
                case DailySchedule.EventActivity.Meal: return "Meal";
            }

            return "None";
        }

        public class ScheduleBlock
        {
            public DateTime Start { get; set; }

            public DateTime End { get; set; }

            public BlockActivity Activity { get; set; }
        }

        public class ScheduleEvent
        {
            public DateTime When { get; set; }

            public EventActivity Activity { get; set; }
        }

        private List<ScheduleBlock> _Blocks = new List<ScheduleBlock>();
        public List<ScheduleBlock> Blocks { get { return _Blocks; } }

        private List<ScheduleEvent> _Events = new List<ScheduleEvent>();
        public List<ScheduleEvent> Events { get { return _Events; } }

        DailySchedule()
        {
            //
        }

        private DailySchedule AddBlock(int startHour, int startMinute, int endHour, int endMinute, BlockActivity activity)
        {
            Blocks.Add(new ScheduleBlock {
                Start = new DateTime(1970, 1, 1, startHour, startMinute, 0),
                End = new DateTime(1970, 1, 1, endHour, endMinute, 0),
                Activity = activity
            });

            return this;
        }

        private DailySchedule AddEvent(int whenHour, int whenMinute, EventActivity activity)
        {
            Events.Add(new ScheduleEvent {
                When = new DateTime(1970, 1, 1, whenHour, whenMinute, 0),
                Activity = activity
            });

            return this;
        }

        public static DailySchedule BuildDefault()
        {
            DailySchedule schedule = new DailySchedule();

            schedule.AddBlock(0, 0, 6, 30, BlockActivity.Sleep)
                    .AddBlock(6, 30, 7, 30, BlockActivity.Normal)
                    .AddBlock(7, 30, 12, 00, BlockActivity.Work)
                    .AddBlock(12, 00, 12, 30, BlockActivity.Normal)
                    .AddBlock(12, 30, 17, 00, BlockActivity.Work)
                    .AddBlock(17, 00, 17, 30, BlockActivity.PhysicalActivity)
                    .AddBlock(17, 30, 19, 00, BlockActivity.Normal)
                    .AddBlock(19, 00, 22, 30, BlockActivity.Rest)
                    .AddBlock(22, 30, 23, 59, BlockActivity.Sleep);

            schedule.AddEvent(7, 15, EventActivity.Meal)
                    .AddEvent(12, 05, EventActivity.Meal)
                    .AddEvent(15, 30, EventActivity.Meal)
                    .AddEvent(19, 00, EventActivity.Meal);

            return schedule;
        }
    }
}
