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
using System.Windows.Shapes;

namespace WPFSerialAssistant
{
    /// <summary>
    /// downloadvoice.xaml 的交互逻辑
    /// </summary>
    public partial class downloadvoice : Window
    {
        MainWindow parent;
        public downloadvoice(MainWindow parent)
        {
            InitializeComponent();
            this.parent = parent;
        }

        private void button1_Click(object sender, RoutedEventArgs e)
        {

        }


        private void button_13_Click(object sender, RoutedEventArgs e)
        {

            Microsoft.Win32.OpenFileDialog fileDialog = new Microsoft.Win32.OpenFileDialog();
            fileDialog.Multiselect = true;
            fileDialog.Title = "请选择文件";
            fileDialog.Filter = "语音文件|*.wav";      //设置要选择的文件的类型
            if (fileDialog.ShowDialog() == true)
            {

                FileBox2.Text = System.IO.Directory.GetFiles(System.IO.Path.GetDirectoryName(fileDialog.FileName)).ElementAt(1);
                FileBox3.Text = System.IO.Directory.GetFiles(System.IO.Path.GetDirectoryName(fileDialog.FileName)).ElementAt(2);
                FileBox4.Text = System.IO.Directory.GetFiles(System.IO.Path.GetDirectoryName(fileDialog.FileName)).ElementAt(3);

            }


        }


        private void button_14_Click(object sender, RoutedEventArgs e)
        {

            if ((string)(this.parent.openClosePortButton.Content) == "关闭")
            {



            }
            else
            {
                MessageBox.Show("请先打开串口，再尝试升级");
            }

        }


        private void button_15_Click(object sender, RoutedEventArgs e)
        {

        }

    }









}
