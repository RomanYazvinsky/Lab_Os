using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using System.Security.Principal;
using System.Windows.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Page = Windows.UI.Xaml.Controls.Page;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace TaskManagerUWP
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        public MainPage()
        {
            this.InitializeComponent();
        }

        public void UpdateProcesses(object obj)
        {
            var processes = Process.GetProcesses();

            ProcessList.Items.Clear();

            foreach (var process in processes)
            {
                DataSet item = new DataSet();
                item.Items.Add("" + process.BasePriority + "\t " +
                               process.WorkingSet64 +
                               "\t " +
                               process.Threads.Count + "\t ");
                item.Id = process.Id.ToString();
                item.ProcessName = process.ProcessName;

                ProcessList.Items.Add(item);
            }
        }
        private class DataSet : TreeViewItem
        {
            public string Id { get; set; }
            public string ProcessName { get; set; }
        }
    }

   
}

