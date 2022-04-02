using Microsoft.Maui.Controls;
using Microsoft.Maui.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IDEG_DiaTrainer.Helpers
{
    public class TimelineGenerator : BindableObject, IDrawable
    {
        // height of a timeline strip
        private static readonly float StripHeight = 24.0f;
        // strip padding, mainly to properly fit arrow and text
        private static readonly float StripPadding = 10.0f;

        // width of the arrow
        private static readonly float ArrowWidth = 6.0f;
        // height of the arrow
        private static readonly float ArrowHeight = 8.0f;

        private Font TimelineFont = new Font("OpenSansRegular");

        // current datetime (to calculate arrow position)
        public DateTime CurrentDateTime { get; set; }

        // schedule for this timeline
        public DailySchedule Schedule { get; set; } = DailySchedule.BuildDefault();

        public void Draw(ICanvas canvas, RectF rect)
        {
            canvas.Font = TimelineFont;
            canvas.FontColor = (Color)App.Current.Resources["PrimaryColor"];

            float curMin = (float)CurrentDateTime.Hour * 60.0f + (float)CurrentDateTime.Minute;
            DailySchedule.BlockActivity currentActivity = DailySchedule.BlockActivity.Regular;
            DailySchedule.BlockActivity upcomingBlockActivity = DailySchedule.BlockActivity.None;
            DailySchedule.EventActivity upcomingEventActivity = DailySchedule.EventActivity.None;

            // clear canvas
            canvas.FillColor = Colors.AntiqueWhite;
            canvas.StrokeColor = Colors.AntiqueWhite;
            canvas.DrawRectangle(rect);

            // basic timeline rectangle - the whole strip fits into this rectangle, excluding arrow and texts
            RectF tlRect = new RectF(StripPadding, /*rect.Height / 3.0f - StripHeight / 4.0f*/14.0f, rect.Width - 2.0f * StripPadding, StripHeight);

            float minuteWidth = tlRect.Width / (24.0f * 60.0f); // how wide is one minute?

            // render block activities
            foreach (var block in Schedule.Blocks)
            {
                float startMin = (float)block.Start.Hour * 60.0f + (float)block.Start.Minute;
                float endMin = (float)block.End.Hour * 60.0f + (float)block.End.Minute;

                Color col = Colors.Blue;

                switch (block.Activity)
                {
                    case DailySchedule.BlockActivity.Regular:           col = Colors.LightBlue; break;
                    case DailySchedule.BlockActivity.Work:              col = Colors.Blue; break;
                    case DailySchedule.BlockActivity.Rest:              col = Colors.AliceBlue; break;
                    case DailySchedule.BlockActivity.Sleep:             col = Colors.DarkBlue; break;
                    case DailySchedule.BlockActivity.PhysicalActivity:  col = Colors.DeepSkyBlue; break;
                }

                canvas.FillColor = col;
                canvas.StrokeColor = col;

                canvas.FillRectangle(tlRect.X + startMin * minuteWidth, tlRect.Y, (endMin - startMin) * minuteWidth, tlRect.Height);

                if (curMin > startMin && curMin < endMin)
                    currentActivity = block.Activity;

                if (upcomingBlockActivity == DailySchedule.BlockActivity.None && curMin < startMin && curMin > startMin - 15.1f)
                    upcomingBlockActivity = block.Activity;
            }

            canvas.StrokeSize = 1.0f;

            // render event-based activities
            foreach (var evt in Schedule.Events)
            {
                float whenMin = (float)evt.When.Hour * 60.0f + (float)evt.When.Minute;

                Color fillCol = Colors.Blue;
                Color strokeCol = Colors.Blue;

                switch (evt.Activity)
                {
                    case DailySchedule.EventActivity.Meal: fillCol = Colors.YellowGreen; strokeCol = Colors.DarkGreen; break;
                }

                canvas.FillColor = fillCol;
                canvas.StrokeColor = strokeCol;

                canvas.FillCircle(tlRect.X + whenMin * minuteWidth, tlRect.Y + tlRect.Height / 2.0f, 4.0f);
                canvas.DrawCircle(tlRect.X + whenMin * minuteWidth, tlRect.Y + tlRect.Height / 2.0f, 4.0f);

                if (upcomingEventActivity == DailySchedule.EventActivity.None && curMin < whenMin && curMin > whenMin - 15.1f)
                    upcomingEventActivity = evt.Activity;
            }

            canvas.StrokeSize = 0.0f;

            // render the arrow
            canvas.FillColor = Colors.Navy;

            PathF tr = new PathF(tlRect.X + curMin * minuteWidth, tlRect.Y + tlRect.Height);
            tr.LineTo(tlRect.X + curMin * minuteWidth - ArrowWidth, tlRect.Y + tlRect.Height + ArrowHeight);
            tr.LineTo(tlRect.X + curMin * minuteWidth + ArrowWidth, tlRect.Y + tlRect.Height + ArrowHeight);
            tr.Close();
            canvas.FillPath(tr);

            float baseX;
            SizeF sz;

            // draw current time under the arrow
            var curTimeStr = String.Format("{0:0}:{1:00}", CurrentDateTime.Hour, CurrentDateTime.Minute);

            sz = canvas.GetStringSize(curTimeStr, TimelineFont, 12.0f);

            baseX = tlRect.X + curMin * minuteWidth;
            if (baseX - sz.Width / 2.0f < 0.0f)
                baseX -= (baseX - sz.Width / 2.0f);
            if (baseX + sz.Width / 2.0f > rect.Width)
                baseX = rect.Width - sz.Width / 2.0f;

            canvas.DrawString(curTimeStr, baseX, tlRect.Y + tlRect.Height + ArrowHeight + 12.0f, HorizontalAlignment.Center);

            string activityStr = DailySchedule.BlockActivityToString(currentActivity);
            sz = canvas.GetStringSize(activityStr, TimelineFont, 12.0f);

            baseX = tlRect.X + curMin * minuteWidth;
            if (baseX - sz.Width / 2.0f < 0.0f)
                baseX -= (baseX - sz.Width / 2.0f);
            if (baseX + sz.Width / 2.0f > rect.Width)
                baseX = rect.Width - sz.Width / 2.0f;

            canvas.DrawString(activityStr, baseX, tlRect.Y + tlRect.Height + ArrowHeight + 24.0f, HorizontalAlignment.Center);

            // draw timeline description above the strip
            for (int hour = 0; hour <= 24; hour += 3)
            {
                curMin = (float)hour * 60.0f;

                canvas.DrawString(String.Format("{0:0}:00", hour < 24 ? hour : 0),
                    tlRect.X + curMin * minuteWidth, tlRect.Y - 5.0f,
                    ((hour == 0) ? HorizontalAlignment.Left : (hour == 24 ? HorizontalAlignment.Right : HorizontalAlignment.Center)) );
            }


            float baseY = tlRect.Y + tlRect.Height + ArrowHeight + 40.0f;

            if (upcomingBlockActivity != DailySchedule.BlockActivity.None)
            {
                canvas.DrawString(String.Format("Upcoming state: {0}", DailySchedule.BlockActivityToString(upcomingBlockActivity)), 0, baseY, HorizontalAlignment.Left);
                //baseY += 16.0f;
            }

            if (upcomingEventActivity != DailySchedule.EventActivity.None)
            {
                canvas.DrawString(String.Format("Upcoming event: {0}", DailySchedule.EventActivityToString(upcomingEventActivity)), rect.Width, baseY, HorizontalAlignment.Right);
                //canvas.DrawString(String.Format("Upcoming event: {0}", DailySchedule.EventActivityToString(upcomingEventActivity)), 0, baseY, HorizontalAlignment.Left);
                //baseY += 16.0f;
            }
        }
    }
}
