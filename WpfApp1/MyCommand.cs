using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace WpfApp1
{
    public static class MyCommand
    {
        public static RoutedCommand CommandDataFromControls = new RoutedCommand("CommandDataFromControls", typeof(WpfApp1.MyCommand));


    }
}
