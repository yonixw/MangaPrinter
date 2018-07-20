using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.ComponentModel;
using System.Windows.Threading;

namespace MangaPrinter.WpfGUI
{
    /// <summary>
    /// Interaction logic for winWorking.xaml
    /// </summary>
    public partial class winWorking : Window
    {
        private BackgroundWorker myWorker = new BackgroundWorker();
        private DispatcherTimer myUpdateTimer = new DispatcherTimer();

        Func<Action<string, int>, object> myWorkFunction = null;
        bool myProgressKnown = false;

        public winWorking(Func<Action<string, int>, object> WorkFunction, bool isProgressKnown)
        {
            myWorkFunction = WorkFunction;
            myProgressKnown = isProgressKnown;
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            myWorker.DoWork += MyWorker_DoWork;
            myWorker.RunWorkerCompleted += MyWorker_RunWorkerCompleted;

            myUpdateTimer.Tick += MyUpdateTimer_Tick;
            myUpdateTimer.Interval = TimeSpan.FromSeconds(1);
            myUpdateTimer.Start();

            if (!myProgressKnown) pbTask.IsIndeterminate = true;

            myWorker.RunWorkerAsync();
        }

        string updateStateDesc = string.Empty;
        int nextPercent = 0;
        private void MyUpdateTimer_Tick(object sender, EventArgs e)
        {
            lblTask.Content = updateStateDesc;
            if (myProgressKnown)
                pbTask.Value = Math.Min(100, Math.Max(0, nextPercent));
        }

        public object Result;
        private void MyWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            DialogResult = true; // auto close;
        }

        private void MyWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            Result = myWorkFunction((desc, percent) =>
            {
                updateStateDesc = desc;
                nextPercent = percent;
            });
        }

        public static T waitForTask<T>(Func<Action<string, int>, T> Task, bool isProgressKnwon) {
            winWorking taskWindow = new winWorking((update)=> { return Task(update); }, isProgressKnwon);
            taskWindow.ShowDialog();
            return (T)taskWindow.Result;
        }
    }
}
