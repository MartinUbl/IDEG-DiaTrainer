using Microsoft.Maui.Controls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IDEG_DiaTrainer.Helpers
{
    public class TutorialExt
    {
        public static readonly BindableProperty TutorialPositionProperty = BindableProperty.CreateAttached("TutorialPosition", typeof(int), typeof(TutorialExt), defaultValue: -1);
        public static readonly BindableProperty TutorialBindNameProperty = BindableProperty.CreateAttached("TutorialName", typeof(string), typeof(TutorialExt), defaultValue: null);

        public static int GetTutorialPosition(BindableObject view)
        {
            return (int)view.GetValue(TutorialPositionProperty);
        }

        public static void SetTutorialPosition(BindableObject view, int value)
        {
            view.SetValue(TutorialPositionProperty, value);
        }

        public static string GetTutorialBindName(BindableObject view)
        {
            return (string)view.GetValue(TutorialBindNameProperty);
        }

        public static void SetTutorialBindName(BindableObject view, string value)
        {
            view.SetValue(TutorialBindNameProperty, value);
        }
    }
}
