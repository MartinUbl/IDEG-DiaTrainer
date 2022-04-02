using IDEG_DiaTrainer.Controllers;
using IDEG_DiaTrainer.Messages;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Controls.Shapes;
using Microsoft.Maui.Graphics;
using System;

namespace IDEG_DiaTrainer.Components.Pumps
{
	public class ProgressIndicatorDrawable : IDrawable
	{
		private static readonly float BorderOffset = 2.0f;
		private static readonly float IndOffset = 4.1f;

		private static readonly float HeightOffset = 2.0f;

		private PumpController pump;

		private bool IsBatteryIndicator;

		public ProgressIndicatorDrawable(PumpController controller, bool IsBattery)
		{
			pump = controller;
			IsBatteryIndicator = IsBattery;
		}

		public void Draw(ICanvas canvas, RectF rect)
		{
			canvas.FillColor = Colors.Black;
			canvas.FillRectangle(rect);

			float srcFactor = IsBatteryIndicator ? (float)pump.BatteryCharge : (float)(pump.RemainingInsulin / PumpController.InsulinReservoirSize);

			if (IsBatteryIndicator)
			{
				if (srcFactor > 0.5f)
				{
					canvas.FillColor = Colors.LimeGreen;
					canvas.StrokeColor = Colors.LimeGreen;
				}
				else if (srcFactor > 0.25f)
				{
					canvas.FillColor = Colors.Orange;
					canvas.StrokeColor = Colors.Orange;
				}
				else
				{
					canvas.FillColor = Colors.Red;
					canvas.StrokeColor = Colors.Red;
				}
			}
			else
			{
				if (srcFactor > 0.5f)
				{
					canvas.FillColor = Colors.Blue;
					canvas.StrokeColor = Colors.Blue;
				}
				else if (srcFactor > 0.25f)
				{
					canvas.FillColor = Colors.Orange;
					canvas.StrokeColor = Colors.Orange;
				}
				else
				{
					canvas.FillColor = Colors.Red;
					canvas.StrokeColor = Colors.Red;
				}
			}
			canvas.StrokeSize = 3.0f;

			canvas.DrawRoundedRectangle(rect.X + BorderOffset, rect.Y + BorderOffset + HeightOffset, rect.Width - 2.0f*BorderOffset, rect.Height - 2.0f*(BorderOffset + HeightOffset), 4.0f);
			canvas.FillRoundedRectangle(rect.X + IndOffset, rect.Y + IndOffset + HeightOffset, (rect.Width - 2.0f*IndOffset) * srcFactor, rect.Height - 2.0f * (IndOffset + HeightOffset), 2.0f);
		}
	}

	public class SimpleGraphDrawable : IDrawable
	{
		private PumpController pump;

		public double HoursDisplayed { get; set; } = 3.0;

		public SimpleGraphDrawable(PumpController controller)
		{
			pump = controller;
		}

		public void Draw(ICanvas canvas, RectF rect)
		{
			double minVal = 3.0; // always draw at least 3.0 mmol/l
			double maxVal = 11.0; // always draw at least 11 mmol/l
			DateTime newest = pump.CurrentDateTime.AddHours(-HoursDisplayed);

			foreach (var x in pump.StoredValues)
			{
				if ((pump.CurrentDateTime - x.When).TotalHours > HoursDisplayed)
					continue;

				if (x.Value > maxVal)
					maxVal = x.Value;
				if (x.Value < minVal)
					minVal = x.Value;

				if (x.When > newest)
					newest = x.When;
			}

			// adjust slightly
			minVal *= 0.95;
			maxVal *= 1.1;

			DateTime startTime = pump.CurrentDateTime.AddHours(-HoursDisplayed);

			canvas.FillColor = Color.FromRgb(30, 30, 30);

			// 3.8 - 7

			float rightMargin = 32.0f;

			float dypos = rect.Y + rect.Height - rect.Height * (float)((3.8 - minVal) / (maxVal - minVal));
			float uypos = rect.Y + rect.Height - rect.Height * (float)((7.0 - minVal) / (maxVal - minVal));

			canvas.FillRectangle(0, uypos, rect.Width - rightMargin, dypos - uypos);

			canvas.StrokeColor = Colors.White;
			canvas.FillColor = Colors.White;

			foreach (var x in pump.StoredValues)
			{
				if ((pump.CurrentDateTime - x.When).TotalHours > HoursDisplayed)
					continue;

				float xpos = rect.X + (rect.Width - rightMargin) * (float)((x.When - startTime).TotalHours / HoursDisplayed);
				float ypos = rect.Y + rect.Height - rect.Height * (float)((x.Value - minVal) / (maxVal - minVal));

				if (x.When == newest)
					canvas.DrawCircle(xpos, ypos, 4.0f);
				else
					canvas.FillCircle(xpos, ypos, 2.0f);
			}

			canvas.FontColor = Colors.White;

			canvas.DrawString("3.8", rect.Width - rightMargin + 4, dypos + 3, HorizontalAlignment.Left);
			canvas.DrawString("7.0", rect.Width - rightMargin + 4, uypos + 3, HorizontalAlignment.Left);
		}
	}

	public partial class DefaultPump : ContentView
	{
		private PumpController pump;

		public DefaultPump(PumpController controller)
		{
			pump = controller;
			BindingContext = pump;

			InitializeComponent();

			BatteryPctLabel.PropertyChanged += (o, a) => { BatteryPctIndicator.Invalidate(); };
			InsulinPctLabel.PropertyChanged += (o, a) => { InsulinPctIndicator.Invalidate(); };
			TimeLabel.PropertyChanged += (o, a) => { IGGraph.Invalidate(); };

			BatteryPctIndicator.Drawable = new ProgressIndicatorDrawable(pump, true);
			InsulinPctIndicator.Drawable = new ProgressIndicatorDrawable(pump, false);
			IGGraph.Drawable = new SimpleGraphDrawable(pump);
		}

		private enum ViewType
        {
			Default,
			Bolus,
			BolusConfirm,
			Basal,
			BasalConfirm,
        }

		private void SetViewType(ViewType type)
        {
			DefaultLayout.IsVisible = false;
			BolusLayout.IsVisible = false;
			BolusConfirmLayout.IsVisible = false;
			BasalLayout.IsVisible = false;
			BasalConfirmLayout.IsVisible = false;

			switch (type)
            {
				case ViewType.Default:
					DefaultLayout.IsVisible = true;
					break;
				case ViewType.Bolus:
					BolusLayout.IsVisible = true;
					break;
				case ViewType.BolusConfirm:
					BolusConfirmLayout.IsVisible = true;
					break;
				case ViewType.Basal:
					BasalLayout.IsVisible = true;
					break;
				case ViewType.BasalConfirm:
					BasalConfirmLayout.IsVisible = true;
					break;
            }
        }

		private void BasalButton_Clicked(object sender, EventArgs e)
		{
			SetViewType(ViewType.Basal);
		}

		private void BolusButton_Clicked(object sender, EventArgs e)
		{
			SetViewType(ViewType.Bolus);
		}

        private void BolusCancelButton_Clicked(object sender, EventArgs e)
        {
			SetViewType(ViewType.Default);
		}

        private void BolusConfirmButton_Clicked(object sender, EventArgs e)
        {
			SetViewType(ViewType.BolusConfirm);
		}

        private void BolusCancel2Button_Clicked(object sender, EventArgs e)
        {
			SetViewType(ViewType.Bolus);
		}

        private void BolusConfirm2Button_Clicked(object sender, EventArgs e)
        {
			pump.SendBolus(BolusKeyboard.GetEnteredValue());

			SetViewType(ViewType.Default);
		}

        private void BasalCancelButton_Clicked(object sender, EventArgs e)
        {
			SetViewType(ViewType.Default);
		}

        private void BasalConfirmButton_Clicked(object sender, EventArgs e)
        {
			SetViewType(ViewType.BasalConfirm);
		}

        private void BasalCancel2Button_Clicked(object sender, EventArgs e)
        {
			SetViewType(ViewType.Basal);
		}

        private void BasalConfirm2Button_Clicked(object sender, EventArgs e)
        {
			pump.SendBasalRate(BasalKeyboard.GetEnteredValue());

			SetViewType(ViewType.Default);
		}
    }
}
