﻿using System;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace SmartLinkDeviceCalculator
{
    /// <summary>
    /// Interaction logic for PowerLabel.xaml
    /// </summary>
    public partial class PowerLabel : Grid
    {
        public PowerLabel()
        {
            InitializeComponent();
            this.power.Background = new LinearGradientBrush(Colors.Red, Colors.Green,2);
        }
    }
}
