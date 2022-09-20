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
        Exercise,
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

        private class WindowEntry
        {
            public Window window { get; set; }
            public Action onClose { get; set; }
        }

        private Dictionary<WindowTypes, WindowEntry> ActiveWindows = new Dictionary<WindowTypes, WindowEntry>();

        public Window OpenWindow(WindowTypes type, Page page, bool navigation = false, Action onCloseAction = null)
        {
            if (ActiveWindows.ContainsKey(type))
            {
                Application.Current.CloseWindow(ActiveWindows[type].window);
                ActiveWindows.Remove(type);
            }

            var newWindow = new Window
            {
                Page = navigation ? new NavigationPage(page) : page,
            };

            if (newWindow != null)
            {
                newWindow.Destroying += NewWindow_Destroying;

                ActiveWindows.Add(type, new WindowEntry { window = newWindow, onClose = onCloseAction });
                Application.Current.OpenWindow(newWindow);
            }

            return newWindow;
        }

        private void NewWindow_Destroying(object sender, EventArgs e)
        {
            foreach (var win in ActiveWindows)
            {
                if (win.Value.window == sender)
                {
                    if (win.Value.onClose != null)
                        win.Value.onClose();

                    ActiveWindows.Remove(win.Key);
                    return;
                }
            }
        }

        public void CloseWindow(WindowTypes type)
        {
            if (ActiveWindows.ContainsKey(type))
            {
                Application.Current.CloseWindow(ActiveWindows[type].window);
                ActiveWindows.Remove(type);
            }
        }
    }
}
