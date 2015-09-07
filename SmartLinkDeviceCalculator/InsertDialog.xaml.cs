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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace SmartLinkDeviceCalculator
{
    /// <summary>
    /// Interaction logic for InsertDialog.xaml
    /// </summary>
    public partial class InsertDialog : Window
    {
        public int pos { get; set; }
        public int cunt { get; set; }

        public InsertDialog()
        {
            InitializeComponent();
        }

        //普通模块的对话框 count 装配区总设备数(含终端模块) Mcount 装配区总模块数
        public InsertDialog(int count,int Mcount)
        {
            InitializeComponent();

            if (count != 0)
            {
                for (int i = 0; i < count; i++)
                {
                    ComboBoxItem pos = new ComboBoxItem();
                    pos.Content = i + 1;
                    comboOfPosition.Items.Add(pos);
                }
            }
            else
            {
                ComboBoxItem pos = new ComboBoxItem();
                pos.Content = 1;
                comboOfPosition.Items.Add(pos);
            }

            for (int i = 0; i < 32 - Mcount; i++)
            {
                ComboBoxItem cout = new ComboBoxItem();
                cout.Content = i + 1;
                comboOfCount.Items.Add(cout);
            }
        }

        private void add_Click(object sender, RoutedEventArgs e)
        {
            this.pos = this.comboOfPosition.SelectedIndex + 1;
            this.cunt = this.comboOfCount.SelectedIndex + 1;
            this.Close();
        }

        private void cancel_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
