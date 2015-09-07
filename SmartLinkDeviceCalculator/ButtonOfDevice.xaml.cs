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
using System.Xml.Linq;

namespace SmartLinkDeviceCalculator
{
    /// <summary>
    /// Interaction logic for ButtonOfDevice.xaml
    /// </summary>
    public partial class ButtonOfDevice : Button
    {
        public ButtonOfDevice()
        {
            InitializeComponent();
        }


        public ButtonOfDevice(XElement dev)
        {
            InitializeComponent();

            if (dev.Parent.Name == "Adapter")
            {
                //this.usercontrol.Width = 120;
                this.Width = 120;
                this.grid.Width = 110;
                this.Code.Width = 110;
                this.Tag1.Width = 110;
                this.Tag2.Width = 110;
                this.Tag3.Width = 110;
                this.Tag4.Width = 110;
                this.Tag5.Width = 110;
                this.Power.Width = 110;
                this.line2.X2 += 50;
                this.line3.X1 += 50;
                this.line3.X2 += 50;
                this.line1.Stroke = Brushes.Red;
                this.line2.Stroke = Brushes.Red;
                this.line3.Stroke = Brushes.Red;

                this.line1.StrokeThickness = 4;
                this.line3.StrokeThickness = 0;
            }

            if (dev.Element("Name").Value == "电源模块")
            {
                this.line1.Stroke = Brushes.Red;
                this.line2.Stroke = Brushes.Red;
                this.line3.Stroke = Brushes.Red;

                this.line1.StrokeThickness = 4;
                this.line3.StrokeThickness = 0;
            }

            ColorConverter cc = new ColorConverter();
            Color c = (Color)cc.ConvertFrom(dev.Parent.Attribute("color").Value);
            this.Code.Background = new SolidColorBrush(c);
            this.Margin = new Thickness(5, 0, 0, 0);

            this.Code.Content = dev.Element("Code").Value.ToString();
            this.Tag1.Content = dev.Element("Tags").Attribute("tag1").Value;
            this.Tag2.Content = dev.Element("Tags").Attribute("tag2").Value;
            this.Tag3.Content = dev.Element("Tags").Attribute("tag3").Value;
            this.Tag4.Content = dev.Element("Tags").Attribute("tag4").Value;
            this.Tag5.Content = dev.Element("Tags").Attribute("tag5").Value;
            this.Power.Content = Convert.ToInt32(dev.Element("OutCurrent").Value) - Convert.ToInt32(dev.Element("InCurrent").Value);

            if (Convert.ToInt32(this.Power.Content.ToString()) == 0)
            {
                this.line1.StrokeThickness = 0;
                this.line2.Stroke = Brushes.Green;
                this.line3.StrokeThickness = 0;
            }
        }

        public ButtonOfDevice Clone()
        {
            ButtonOfDevice nbtn = new ButtonOfDevice();
            //nbtn.usercontrol.Width = this.usercontrol.Width;   
            nbtn.Width = this.Width;
            nbtn.grid.Width = this.grid.Width;
            nbtn.Code.Width = this.Code.Width;
            nbtn.Tag1.Width = this.Tag1.Width;
            nbtn.Tag2.Width = this.Tag2.Width;
            nbtn.Tag3.Width = this.Tag3.Width;
            nbtn.Tag4.Width = this.Tag4.Width;
            nbtn.Tag5.Width = this.Tag5.Width;
            nbtn.Power.Width = this.Power.Width;

            nbtn.line2.X2 = this.line2.X2;
            nbtn.line3.X1 = this.line3.X1;
            nbtn.line3.X2 = this.line3.X2;

            nbtn.line1.Stroke = this.line1.Stroke;
            nbtn.line2.Stroke = this.line2.Stroke;
            nbtn.line3.Stroke = this.line3.Stroke;

            nbtn.line1.StrokeThickness = this.line1.StrokeThickness;
            nbtn.line2.StrokeThickness = this.line2.StrokeThickness;
            nbtn.line3.StrokeThickness = this.line3.StrokeThickness;

            nbtn.Code.Background = this.Code.Background;

            nbtn.Code.Content = this.Code.Content;
            nbtn.Tag1.Content = this.Tag1.Content;
            nbtn.Tag2.Content = this.Tag2.Content;
            nbtn.Tag3.Content = this.Tag3.Content;
            nbtn.Tag4.Content = this.Tag4.Content;
            nbtn.Tag5.Content = this.Tag5.Content;
            nbtn.Power.Content = this.Power.Content;

            if (Convert.ToInt32(nbtn.Power.Content.ToString()) > 0)
            {

            }

            return nbtn;
        }

        public bool IsFunctionalModule()
        {
            if (this.Tag1.Content.ToString() == "适配器" || this.Tag1.Content.ToString() == "终端模块" || this.Tag2.Content.ToString() == "电源模块")
            {
                return false;
            }
            return true;
        }
    }
}
