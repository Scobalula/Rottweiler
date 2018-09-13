using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Runtime.InteropServices;

namespace Rottweiler.Windows
{
    /// <summary>
    /// Interaction logic for ProgressWindow.xaml
    /// </summary>
    public partial class ProgressWindow : Window
    {
        // https://msdn.microsoft.com/en-us/library/windows/desktop/ms644898(v=vs.85).aspx
        // https://www.codeproject.com/Tips/1155345/How-to-Remove-the-Close-Button-from-a-WPF-ToolWind
        public const int GWL_STYLE = -16;
        public const int WS_SYSMENU = 0x80000;
        [DllImport("user32.dll", SetLastError = true)]
        public static extern int GetWindowLongPtr(IntPtr hWnd, int nIndex);
        [DllImport("user32.dll")]
        public static extern int SetWindowLong(IntPtr hWnd, int nIndex, int dwNewLong);

        /// <summary>
        /// Has the user canceled.
        /// </summary>
        public bool UserCancel = false;

        /// <summary>
        /// Is the application still working.
        /// </summary>
        public bool isWorking = true;

        public ProgressWindow()
        {
            InitializeComponent();

            // Set attribute.
            Loaded += ToolWindow_Loaded;
        }

        /// <summary>
        /// Handles changing window attribute to remove close button.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void ToolWindow_Loaded(object sender, RoutedEventArgs e)
        {
            var hwnd = new System.Windows.Interop.WindowInteropHelper(this).Handle;
            SetWindowLong(hwnd, GWL_STYLE, GetWindowLongPtr(hwnd, GWL_STYLE) & ~WS_SYSMENU);
        }

        /// <summary>
        /// Handles checking for window closing if we're still working.
        /// </summary>
        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (isWorking)
                e.Cancel = true;
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            UserCancel = true;
        }

        /// <summary>
        /// Updates the Progress Bar
        /// </summary>
        /// <param name="progress">Progress from 0-100</param>
        /// <param name="progressWindow">Progress Window</param>
        public bool UpdateProgress(float progress)
        {
            Dispatcher.Invoke(
                () =>
                {
                    Progress.Value = progress;

                    if (progress == 100 && (string)label.Content == "Decompressing Fast File....")
                    {
                        SwitchProgressMode("Searching for sounds....");
                    }
                }
                );

            return !UserCancel;

        }

        public bool CheckUserCancel()
        {
            return !UserCancel;
        }

        public void SwitchProgressMode(string message)
        {
            Dispatcher.Invoke(
                () =>
                {
                    Progress.IsIndeterminate = true;
                    label.Content = message;
                }
                );
        }
    }
}
