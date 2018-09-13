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
using PhilUtil;

namespace Rottweiler.Windows
{
    /// <summary>
    /// Interaction logic for ExportFinishedWindow.xaml
    /// </summary>
    public partial class ExportFinishedWindow : Window
    {
        public ExportFinishedWindow()
        {
            InitializeComponent();
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
            ProgressWindow.SetWindowLong(hwnd, ProgressWindow.GWL_STYLE, ProgressWindow.GetWindowLongPtr(hwnd, ProgressWindow.GWL_STYLE) & ~ProgressWindow.WS_SYSMENU);
        }

        private void CloseWindow_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void OpenExportFolder_Click(object sender, RoutedEventArgs e)
        {
            PathUtil.OpenFolder("exported_audio");
            Close();
        }
    }
}
