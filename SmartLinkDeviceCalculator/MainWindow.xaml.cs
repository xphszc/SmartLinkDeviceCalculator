using System;
using System.Collections.Generic;
using System.ComponentModel;
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
using System.Xml.Linq;

namespace SmartLinkDeviceCalculator
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window,INotifyPropertyChanged
    {
        //XML文件名
        static string fileName = "SmartLinkDevicesForCal.xml";


        //依赖属性事件句柄
        public event PropertyChangedEventHandler PropertyChanged;

        //装配区功能模块数
        private int moduleCount;
        public int ModuleCount
        {
            get
            {
                return moduleCount;
            }
            set
            {
                moduleCount = value;
                if (this.PropertyChanged != null)
                {
                    this.PropertyChanged.Invoke(this, new PropertyChangedEventArgs("ModuleCount"));
                }
            }
        }

        //装配区剩余功率
        private int surplusPower;
        public int SurplusPower
        {
            get
            {
                return surplusPower;
            }
            set
            {
                surplusPower = value;
                if (this.PropertyChanged != null)
                {
                    this.PropertyChanged.Invoke(this, new PropertyChangedEventArgs("SurplusPower"));
                }
            }
        }


        public MainWindow()
        {
            InitializeComponent();

            //功能模块数标签内容绑定SurplusPower依赖属性
            this.modulecount.SetBinding(Label.ContentProperty, new Binding("ModuleCount") { Source = this, Path = new PropertyPath("ModuleCount") });

            //剩余功率标签内容绑定SurplusPower依赖属性
            //this.label.SetBinding(Label.ContentProperty, new Binding("SurplusPower") { Source = this, Path = new PropertyPath("SurplusPower") });

            XElement root = XElement.Load(fileName);
            
            //创建设备查询
            var deviceQuery = 
                from device in root.Elements().Elements("Device")
                select device;

            //实例一个标签
            TabItemOfDevices TD = new TabItemOfDevices();

            //循环引用查询
            foreach (var dev in deviceQuery)
            {
                ButtonOfDevice btn = new ButtonOfDevice(dev);
                btn.Click += new RoutedEventHandler(add_dev_Click);
                if (btn.Tag1.Content.ToString() != "适配器" && btn.Tag1.Content.ToString() != "终端模块")
                {
                    btn.MouseRightButtonUp += new MouseButtonEventHandler(right_mod_Click);
                }
                TD.SP.Children.Add(btn);
                
                if (dev.NextNode == null)
                {
                    TD.Header = dev.Parent.Attribute("name").Value;
                    this.TC.Items.Add(TD);
                    TD = new TabItemOfDevices();
                }
            }
        }

        //添加模块
        private void add_dev_Click(object sender, RoutedEventArgs e)
        {
            ButtonOfDevice devb = (sender as ButtonOfDevice).Clone();

            switch (devb.Tag1.Content.ToString())
            {

                case "适配器":
                    {
                        if (IsAdapterOnFirst())
                        {
                            MessageBox.Show("只能有一个适配器");
                            return;
                        }
                        devicesPanel.Children.Add(devb);
                        NumberBoxOfAdapter adpNum = new NumberBoxOfAdapter();
                        numPanel.Children.Add(adpNum);
                        SurplusPower += Convert.ToInt32(devb.Power.Content);
                        PowerCheck();
                        SLCheck();
                    }
                    break;


                case "终端模块":
                    {
                        if (IsAdapterOnFirst())
                        {
                            if (IsEndOnLast())
                            {
                                MessageBox.Show("只能有一个终端模块");
                                return;
                            }
                            devb.MouseRightButtonUp += new MouseButtonEventHandler(remove_single_Click);
                            devicesPanel.Children.Add(devb);
                            NumberBoxOfModule modNum = new NumberBoxOfModule();
                            modNum.Content = string.Format("[ {0} ]", devicesPanel.Children.Count - 1);
                            numPanel.Children.Add(modNum);
                            SLCheck();
                            return;
                        }
                        MessageBox.Show("请先装配适配器");
                    }
                    break;


                default:
                    {
                        if (IsAdapterOnFirst())
                        {
                            if (!IsModuleOver32() && devb.Tag2.Content.ToString() != "电源模块")
                            {
                                MessageBox.Show("模块不能超过32个");
                                return;
                            }
                            if (IsEndOnLast())
                            {
                                MessageBox.Show("不能在终端模块后添加模块");
                                return;
                            }

                            devb.MouseRightButtonUp += new MouseButtonEventHandler(remove_single_Click);
                            devicesPanel.Children.Add(devb);

                            NumberBoxOfModule modNum = new NumberBoxOfModule();
                            modNum.Content = string.Format("[ {0} ]", devicesPanel.Children.Count - 1);
                            numPanel.Children.Add(modNum);

                            if (devb.IsFunctionalModule())
                            {
                                ModuleCount++;
                            }

                            SurplusPower += Convert.ToInt32(devb.Power.Content);
                            PowerCheck();
                            SLCheck();
                            return;
                        }
                        MessageBox.Show("请先装配适配器");
                    }
                    break;
            }
        }


        private void right_mod_Click(object sender, MouseButtonEventArgs e)
        {
            if (IsAdapterOnFirst())
            {
                if (!IsModuleOver32() && (sender as ButtonOfDevice).Tag2.Content.ToString() != "电源模块")
                {
                    MessageBox.Show("模块不能超过32个");
                    return;
                }
                //if (IsEndOnLast())
                {
                    //int count = ModulesInPanel();
                    InsertDialog ID = new InsertDialog(devicesPanel.Children.Count - 1, ModulesInPanel());
                    if ((sender as ButtonOfDevice).Tag2.Content.ToString() == "电源模块")
                    {
                        ID = new InsertDialog(devicesPanel.Children.Count - 1, 27);
                    }
                    ID.Title = string.Format("自定义添加 {0}", (sender as ButtonOfDevice).Code.Content.ToString());
                    ID.ShowDialog();
                    if (ID.pos != 0 && ID.cunt != 0)
                    {
                        for (int i = 0; i < ID.cunt; i++)
                        {
                            ButtonOfDevice devb = (sender as ButtonOfDevice).Clone();
                            devb.MouseRightButtonUp += new MouseButtonEventHandler(remove_single_Click);
                            devicesPanel.Children.Insert(ID.pos + i, devb);

                            if (devb.IsFunctionalModule())
                            {
                                ModuleCount++;
                            }

                            SurplusPower += Convert.ToInt32(devb.Power.Content);
                            PowerCheck();
                            SLCheck();
                        }
                        NumRefresh(devicesPanel.Children.Count);
                    }
                    return;
                }
                return;
            }
            MessageBox.Show("请先装配适配器");
        }

        //从后移除模块
        private void remove_Click(object sender, RoutedEventArgs e)
        {
            if (devicesPanel.Children.Count > 0)
            {
                SurplusPower -= Convert.ToInt32((devicesPanel.Children[devicesPanel.Children.Count - 1] as ButtonOfDevice).Power.Content);
                devicesPanel.Children.RemoveAt(devicesPanel.Children.Count - 1);
                numPanel.Children.RemoveAt(devicesPanel.Children.Count);
                if ((devicesPanel.Children[devicesPanel.Children.Count - 1] as ButtonOfDevice).IsFunctionalModule())
                {
                    ModuleCount--;
                }
                PowerCheck();
                SLCheck();
            }
        }


        //清除所有除适配器以外的模块
        private void clearM_Click(object sender, RoutedEventArgs e)
        {
            while (devicesPanel.Children.Count > 1)
            {
                SurplusPower -= Convert.ToInt32((devicesPanel.Children[devicesPanel.Children.Count - 1] as ButtonOfDevice).Power.Content);
                devicesPanel.Children.RemoveAt(devicesPanel.Children.Count - 1);
                numPanel.Children.RemoveRange(1, numPanel.Children.Count);
                ModuleCount = 0;
                PowerCheck();
                SLCheck();
            }
        }


        //清除所有模块
        private void clear_Click(object sender, RoutedEventArgs e)
        {
            devicesPanel.Children.Clear();
            numPanel.Children.Clear();
            ModuleCount = 0;
            SurplusPower = 0;
            PowerCheck();
            SLCheck();
        }


        //装配区模块右键清除自身
        private void remove_single_Click(object sender, MouseButtonEventArgs e)
        {
            //清除自身前的装配区设备数
            int count = devicesPanel.Children.Count;

            //要清除的设备index
            int index = devicesPanel.Children.IndexOf(sender as UIElement);

            //要清除的设备功耗
            int power = Convert.ToInt32((devicesPanel.Children[index] as ButtonOfDevice).Power.Content);

            devicesPanel.Children.RemoveAt(index);

            /*
            numPanel.Children.RemoveRange(index,count - index);
            for (int i = index; i < count - 1; i++)
            {
                NumberBoxOfModule num = new NumberBoxOfModule();
                num.Content = string.Format("[ {0} ]", i);
                numPanel.Children.Add(num);
            }
            */
            NumRefresh(devicesPanel.Children.Count);
            if ((sender as ButtonOfDevice).IsFunctionalModule())
            {
                ModuleCount--;
            }
            SurplusPower -= power;
            PowerCheck();
            SLCheck();
        }


        //装配区设备编号刷新
        public void NumRefresh(int count)
        {
            numPanel.Children.Clear();

            NumberBoxOfAdapter adpNum = new NumberBoxOfAdapter();
            numPanel.Children.Add(adpNum);

            for (int i = 1; i < count ; i++)
            {
                NumberBoxOfModule num = new NumberBoxOfModule();
                num.Content = string.Format("[ {0} ]", i);
                numPanel.Children.Add(num);
            }
        }


        //判断适配器是否在第一位
        public bool IsAdapterOnFirst()
        {
            if (devicesPanel.Children.Count != 0)
            {
                if ((devicesPanel.Children[0] as ButtonOfDevice).Tag1.Content.ToString() == "适配器")
                {
                    return true;
                }
            }
            return false;
        }


        //判断终端模块是否在最后一位
        public bool IsEndOnLast()
        {
            if ((devicesPanel.Children[devicesPanel.Children.Count - 1] as ButtonOfDevice).Tag1.Content.ToString() == "终端模块")
            {
                Console.WriteLine("EndOnLast");
                return true;
            }
            return false;
        }


        //计算装配区模块数(除终端,电源,适配器)
        public int ModulesInPanel()
        {
            int count = 0;
            for (int i = 0; i < devicesPanel.Children.Count; i++)
            {
                /*
                if ((devicesPanel.Children[i] as ButtonOfDevice).Tag1.Content.ToString() != "适配器" && (devicesPanel.Children[i] as ButtonOfDevice).Tag1.Content.ToString() != "终端模块" && (devicesPanel.Children[i] as ButtonOfDevice).Tag1.Content.ToString() != "电源模块")
                {
                    count++;
                }
                */
                if ((devicesPanel.Children[i] as ButtonOfDevice).IsFunctionalModule())
                {
                    count++;
                }
            }
            return count;
        }


        //判断模块数是否大于32个
        public bool IsModuleOver32()
        {
            if(ModuleCount < 32)
            {
                return true;
            }
            return false;
        }


        //剩余功率颜色判断
        public void PowerCheck()
        {
            /*
            if (SurplusPower > 0)
            {
                label.Foreground = Brushes.Green;
            }
            else
            {
                if (SurplusPower == 0)
                {
                    label.Foreground = Brushes.Black;
                }
                else
                {
                    label.Foreground = Brushes.Red;
                }
            }
            */
        }

        public void SLCheck()
        {
            powerPanel.Children.Clear();
            int length = 0;
            int power = 0;

            for (int i = 0; i < devicesPanel.Children.Count; i++)
            {
                int btnPower = Convert.ToInt32((devicesPanel.Children[i] as ButtonOfDevice).Power.Content);
                //int nextbtnPower = 0;
                //if (devicesPanel.Children[i+1] != null)
                {
                //    nextbtnPower = Convert.ToInt32((devicesPanel.Children[i + 1] as ButtonOfDevice).Power.Content);
                }
                int btnLength = Convert.ToInt32((devicesPanel.Children[i] as ButtonOfDevice).Width);

                /*if (i>0 || btnPower >= 0)
                {
                    PowerLabel PL = new PowerLabel();
                    PL.Width = length;
                    PL.power.Content = power;
                    powerPanel.Children.Add(PL);
                    length = 0;
                    power = 0;
                }
                */
                if (btnPower > 0)
                {
                    length += btnLength;
                    power += btnPower;
                }

                if (btnPower <= 0)
                {
                    length += btnLength;
                    power += btnPower;
                }

                //若为最后一个或者后面有电源,画出功耗线
                if (i+1 == devicesPanel.Children.Count || Convert.ToInt32((devicesPanel.Children[i + 1] as ButtonOfDevice).Power.Content) > 0)
                {
                    PowerLabel PL = new PowerLabel();
                    PL.Width = length;
                    PL.power.Content = power;
                    if (power<0)
                    {
                        PL.power.Content = string.Format("{0}  (该供电区域功耗已超标 ! )",power);
                    }
                    powerPanel.Children.Add(PL);
                    length = 0;
                    power = 0;
                }
            }
        }
    }
}