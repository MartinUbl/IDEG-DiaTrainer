using Microsoft.Maui.Controls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IDEG_DiaTrainer.Helpers
{
    public enum WindowTypes
    {
        Meal,
        Insulin,
    }

    public class WindowManager
    {
        private static WindowManager _Instance = null;
        public static WindowManager Instance {
            get {
                if (_Instance == null)
                    _Instance = new WindowManager();
                return _Instance;
            }
        }

        private Dictionary<WindowTypes, Window> ActiveWindows = new Dictionary<WindowTypes, Window>();

        public Window OpenWindow(WindowTypes type, Page page, bool navigation = false)
        {
            if (ActiveWindows.ContainsKey(type))
                Application.Current.CloseWindow(ActiveWindows[type]);

            var newWindow = new Window
            {
                Page = navigation ? new NavigationPage(page) : page,
            };

            ActiveWindows.Add(type, newWindow);

            Application.Current.OpenWindow(newWindow);

            return newWindow;
        }

        public void CloseWindow(WindowTypes type)
        {
            if (ActiveWindows.ContainsKey(type))
            {
                Application.Current.CloseWindow(ActiveWindows[type]);
                ActiveWindows.Remove(type);
            }
        }
    }
}
