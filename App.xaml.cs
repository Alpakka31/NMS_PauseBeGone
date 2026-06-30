using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Reflection;
using System.Threading;

namespace NMS_PauseBeGone
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private const string MutexName = "Local\\NMS_PauseBeGone_{06678B8B-3BCC-4CB6-BE40-FC2332A77444}";
        private Mutex? _mutex;
        private readonly System.Windows.Forms.NotifyIcon _notifyIcon = new();

        protected override void OnStartup(StartupEventArgs e)
        {
            _mutex = new Mutex(true, MutexName, out bool createdNew);
            if (!createdNew)
            {
                MessageBox.Show("Another instance of NMS_PauseBeGone is already running.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                App.Current.Shutdown(1);
                return;
            }

            var appAssembly = Assembly.GetExecutingAssembly();
            using (var appIcon = appAssembly.GetManifestResourceStream("NMS_PauseBeGone.nms.ico"))
            {
                _notifyIcon.Icon = new System.Drawing.Icon(appIcon);
            }
            _notifyIcon.Text = "NMS_PauseBeGone";
            _notifyIcon.MouseClick += (sender, e) =>
            {
                if (e.Button == System.Windows.Forms.MouseButtons.Left)
                {
                    OpenWindow();
                }
            };

            _notifyIcon.ContextMenuStrip = new System.Windows.Forms.ContextMenuStrip();
            _notifyIcon.ContextMenuStrip.Items.Add("Open", null, OnOpenClicked);
            _notifyIcon.ContextMenuStrip.Items.Add("Minimize", null, OnMinimizeClicked);
            _notifyIcon.ContextMenuStrip.Items.Add(new System.Windows.Forms.ToolStripSeparator());
            _notifyIcon.ContextMenuStrip.Items.Add("Exit", null, OnExitClicked);
            _notifyIcon.Visible = true;

            base.OnStartup(e);

            var mainWindow = new MainWindow();
            mainWindow.Show();
        }

        private void OpenWindow()
        {
            if (MainWindow.Visibility == Visibility.Hidden)
            {
                MainWindow.Show();
            }

            MainWindow.WindowState = WindowState.Normal;
            MainWindow.Activate();
        }

        private void OnOpenClicked(object sender, EventArgs e)
        {
            OpenWindow();
        }

        private void OnMinimizeClicked(object sender, EventArgs e)
        {
            if (MainWindow.Visibility != Visibility.Hidden)
            {
                MainWindow.WindowState = WindowState.Minimized;
            }
        }

        private void OnExitClicked(object sender, EventArgs e)
        {
            App.Current.Shutdown();
        }

        protected override void OnExit(ExitEventArgs e)
        {
            _notifyIcon.Dispose();

            _mutex?.ReleaseMutex();
            _mutex?.Dispose();
            base.OnExit(e);
        }
    }
}
