using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Management;
using System.Runtime.InteropServices;
using System.Security.Principal;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Shapes;
using Timer = System.Timers.Timer;

namespace LAB_OS
{
    public partial class MainWindow
    {

        public MainWindow()
        {
            InitializeComponent();
            Timer timer = new Timer(5000);
            timer.Elapsed += Timer_Elapsed;
            UpdateProcesses(null);
            timer.Start();
        }

        private void Timer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            UpdateProcesses(null);
        }

        public void UpdateProcesses(object obj)
        {
            Dispatcher.Invoke(() =>
            {
                var processes = Process.GetProcesses();

                ProcessList.Items.Clear();

                foreach (var process in processes)
                {
                    DataSet item = new DataSet();
                    item.Id = process.Id.ToString();
                    item.ProcessName = process.ProcessName;
                    item.Memory = (process.PrivateMemorySize64/1024).ToString();
                    item.Priority = process.BasePriority.ToString();
                    item.User = GetProcessUser(process);
                    item.Process = process;
                    item.Threads = process.Threads.Count.ToString();
                    ProcessList.Items.Add(item);
                }
            });
        }

        private class DataSet
        {
            public String Id { get; set; }
            public String ProcessName { get; set; }
            public String User { get; set; }
            public String Memory { get; set; }
            public String Priority { get; set; }
            public String Threads { get; set; }
            public Process Process { get; set; }
        }

        private static string GetProcessUser(Process process)
        {
            IntPtr processHandle = IntPtr.Zero;
            try
            {
                OpenProcessToken(process.Handle, 8, out processHandle);
                WindowsIdentity wi = new WindowsIdentity(processHandle);
                string user = wi.Name;
                return user.Contains(@"\") ? user.Substring(user.IndexOf(@"\", StringComparison.Ordinal) + 1) : user;
            }
            catch
            {
                return null;
            }
            finally
            {
                if (processHandle != IntPtr.Zero)
                {
                    CloseHandle(processHandle);
                }
            }
        }


        [DllImport("advapi32.dll", SetLastError = true)]
        private static extern bool OpenProcessToken(IntPtr processHandle, uint desiredAccess, out IntPtr tokenHandle);

        [DllImport("kernel32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool CloseHandle(IntPtr hObject);

        private void ProcessList_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (ProcessList.SelectedIndex >= 0)
            {
                new Threads((ProcessList.SelectedItem as DataSet)?.Process).Show();
            }
        }
    }
}