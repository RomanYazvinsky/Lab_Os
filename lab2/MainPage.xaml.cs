using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Core;
using Windows.UI.Text;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Documents;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace lab2
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        private LinkedList<string> logList = new LinkedList<string>();
        private volatile bool thread1IsWorking = true;
        private volatile bool thread2IsWorking = false;
        private volatile bool textEnterred = false;
        private bool thread1Execution = true;
        private bool thread2Execution = true;
        private Thread thread1;
        private Thread thread2;
        private int i = 0;

        public MainPage()
        {
            this.InitializeComponent();
            thread1 = new Thread(this.threadMethod1);
            thread1.Start();
            thread2 = new Thread(this.threadMethod2);
            thread2.Start();
        }

        private void threadMethod1()
        {
            log("Thread #1 started");
            while (thread1Execution)
            {
                while (thread2IsWorking) ;
                thread1IsWorking = true;
                criticalRegion1();
                thread1IsWorking = false;
                log("Thread1 critical region is over");
            }
        }

        private void criticalRegion1()
        {
            log("Thread1 is waiting for input");
            while (!textEnterred) ;
            string input = GetText().Result;
            StringBuilder result = new StringBuilder();
            foreach (var symb in input)
            {
                result.Append((int) symb);
            }
            SetText(result.ToString());
            textEnterred = false;
        }


        private void threadMethod2()
        {
            log("Thread #2 started");
            while (thread2Execution)
            {
                while (thread1IsWorking) ;
                thread2IsWorking = true;
                criticalRegion2();
                thread2IsWorking = false;
                log("Thread2 critical region is over");
            }
        }


        private void criticalRegion2()
        {
            log("Thread2 is waiting for input");
            while (!textEnterred) ;
            string input = GetText().Result;
            StringBuilder result = new StringBuilder();
            foreach (var symb in input)
            {
                if (Char.IsDigit(symb))
                {
                    result.Append("X");
                }
                else
                {
                    result.Append(symb);
                }
            }
            SetText(result.ToString());
            textEnterred = false;
        }

        private async Task<string> GetText()
        {
            string s = null;
            await this.Dispatcher.RunAsync(CoreDispatcherPriority.Low, () => { s = Proc1.Text; });
            return s;
        }

        private async Task SetText(string value)
        {
            await this.Dispatcher.RunAsync(CoreDispatcherPriority.Low, () => { Proc1.Text = value; });
        }

        private async void log(string text)
        {
            await this.Dispatcher.RunAsync(CoreDispatcherPriority.Low, () =>
            {
                logList.AddFirst(LastLog.Text);
                LastLog.Text = ++i + ") " + text;
                if (logList.Count > 12)
                {
                    logList.RemoveLast();
                }
                StringBuilder logText = new StringBuilder();
                foreach (var logNode in logList)
                {
                    logText.Append(logNode).Append(Environment.NewLine);
                }
                Log.Text = logText.ToString();
            });
        }

        private void Enter_Click(object sender, RoutedEventArgs e)
        {
            textEnterred = true;
        }
    }
}