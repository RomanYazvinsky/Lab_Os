using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;

namespace Lab2Wpf
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow
    {
        private int _algorithm = 0;
        private int _true = 1;
        private int _false = 0;
        private int _n = 2;
        private int _turn = 0;
        private int[] _interested = new int[2];
        private LinkedList<string> _logList = new LinkedList<string>();
        private volatile bool _thread1IsWorking = true;
        private volatile bool _thread2IsWorking = false;
        private volatile bool _textEnterred = false;
        private bool _thread1Execution = true;
        private bool _thread2Execution = true;
        private Thread _thread1;
        private Thread _thread2;
        private Thread _thread1Backup;
        private Thread _thread2Backup;
        private int i = 0;

        public MainWindow()
        {
            InitializeComponent();
            _thread1 = new Thread(this.ThreadPeterson1);
            _thread1.Priority = ThreadPriority.Lowest;
            _thread1.Start();
            _thread2 = new Thread(this.ThreadPeterson2);
            _thread2.Priority = ThreadPriority.Lowest;
            _thread2.Start();
            _thread1Backup = new Thread(this.ThreadAlternation1);
            _thread1Backup.Priority = ThreadPriority.Lowest;
            _thread1Backup.Start();
            _thread2Backup = new Thread(this.ThreadAlternation2);
            _thread2Backup.Priority = ThreadPriority.Lowest;
            _thread2Backup.Start();
        }

        private void ThreadPeterson1()
        {
            AddLog("Thread #1 is ready. Algorithm: Peterson's");
            while (_thread1Execution)
            {
                while (_algorithm == 1) ;
                EnterRegion(0);
                CriticalRegion1();
                LeaveRegion(0);
                //     AddLog("Thread1 critical region is over");
            }
        }

        private void ThreadAlternation1()
        {
            AddLog("Thread #1 is ready. Algorithm: alternation");
            while (_thread1Execution)
            {
                while (_algorithm == 0) ;
                while (_thread2IsWorking) ;
                _thread1IsWorking = true;
                CriticalRegion1();
                _thread1IsWorking = false;
                //     AddLog("Thread1 critical region is over");
            }
        }

        private void CriticalRegion1()
        {
            AddLog("Thread1 is waiting for input... Alg:" + _algorithm);

            int k = _algorithm;
            while (!_textEnterred) ;
            if (k != _algorithm)
            {
                return;
            }

            string input = GetText().Result;

            StringBuilder result = new StringBuilder();
            foreach (var symb in input)
            {
                result.Append((int) symb);
            }

            SetText(result.ToString());
            _textEnterred = false;
        }

        private void ThreadPeterson2()
        {
            AddLog("Thread #2 is ready. Algorithm: Peterson's");
            while (_thread2Execution)
            {
                while (_algorithm == 1) ;
                EnterRegion(1);
                CriticalRegion2();
                LeaveRegion(1);
                // AddLog("Thread2 critical region is over");
            }
        }


        private void ThreadAlternation2()
        {
            AddLog("Thread #2 is ready. Algorithm: alternation");
            while (_thread2Execution)
            {
                while (_algorithm == 0) ;
                while (_thread1IsWorking) ;
                _thread2IsWorking = true;
                CriticalRegion2();
                _thread2IsWorking = false;
                //  AddLog("Thread2 critical region is over");
            }
        }


        private void CriticalRegion2()
        {
            AddLog("Thread2 is waiting for input... Alg:" + _algorithm);

            int k = _algorithm;
            while (!_textEnterred) ;
            if (k != _algorithm)
            {
                return;
            }

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
            _textEnterred = false;
        }


        private void EnterRegion(int process) // номер процесса - 0 или 1
        {
            int other;
            other = 1 - process; // точка 1
            _interested[process] = _true; // точка 2 (заинтересованный процесс)
            _turn = process; // точка 3
            while (_turn == process && _interested[other] == _true) ; // активное ожидание
        }

        private void LeaveRegion(int process) // номер процесса - 0 или 1
        {
            _interested[process] = 0; // признак выхода из критической секции
        }

        private async Task<string> GetText()
        {
            string result = null;

            void Get()
            {
                result = InputField.Text;
            }

            await this.Dispatcher.BeginInvoke(DispatcherPriority.Background, (Action) Get);
            return result;
        }

        private async Task SetText(string value)
        {
            void Set()
            {
                InputField.Text = value;
            }
            await this.Dispatcher.BeginInvoke(DispatcherPriority.Background, (Action) Set);
        }

        private void AddLog(string text)
        {
            void Log()
            {
                _logList.AddFirst(LastLog.Text);
                LastLog.Text = ++i + ") " + text;
                if (_logList.Count > 12)
                {
                    _logList.RemoveLast();
                }

                StringBuilder logText = new StringBuilder();
                foreach (var logNode in _logList)
                {
                    logText.Append(logNode).Append(Environment.NewLine);
                }

                this.Log.Text = logText.ToString();
            }

            this.Dispatcher.BeginInvoke(DispatcherPriority.Background, (Action) Log);
        }

        private void Enter_Click(object sender, RoutedEventArgs e)
        {
            _textEnterred = true;
        }

        private void Alternate_Click(object sender, RoutedEventArgs e)
        {
            if (_algorithm == 0)
            {
                _algorithm = 1;
            }
            else
            {
                _algorithm = 0;
            }

            Thread t;
            t = _thread1Backup;
            _thread1Backup = _thread1;
            _thread1 = t;
            t = _thread2Backup;
            _thread2Backup = _thread2;
            _thread2 = t;
        }
    }
}