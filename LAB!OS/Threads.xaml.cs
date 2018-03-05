using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace LAB_OS
{
    
    public partial class Threads : Window
    {
        public Threads(Process process)
        {
            InitializeComponent();
           
            foreach (var thread in process.Threads)
            {
                ThreadList.Items.Add(thread);
               
            }
        }
    }
}
