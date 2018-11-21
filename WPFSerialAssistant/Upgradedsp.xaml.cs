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
    /// upgradedsp.xaml 的交互逻辑
    /// </summary>
    public partial class upgradedsp : Window
    {
        MainWindow parent;
        public upgradedsp(MainWindow parent)
        {
            InitializeComponent();
            this.parent = parent;
        }

        public bool m_upgradeflag = false;


        private void OpenUpgradeFileButton_Click(object sender, RoutedEventArgs e)
        {
            if ((string)(this.parent.openClosePortButton.Content) == "关闭")
            {

                if (DspButton.IsChecked == true)
                {
                    Microsoft.Win32.OpenFileDialog fileDialog = new Microsoft.Win32.OpenFileDialog();
                    fileDialog.Multiselect = true;
                    fileDialog.Title = "请选择文件";
                    fileDialog.Filter = "LDR文件|*.ldr";      //设置要选择的文件的类型
                    if (fileDialog.ShowDialog() == true)
                    {
                        this.Dispatcher.Invoke(new Action(delegate
                        {
                            UpgradeFileBox.Text = fileDialog.FileName;
                            this.parent.statusInfoTextBlock.Text = "获取升级文件成功";
                        }));


                        //将升级文件传入ymodem打开
                        this.parent.MyYmodem.ReadFileData(fileDialog.FileName);
                        UpgradeProgressBar.Maximum = this.parent.MyYmodem.GetPackNum() + 5;

                        UpgradeProgressBar.Value = 0;

                    }
                }
                else if (McuButton.IsChecked == true)
                {

                    Microsoft.Win32.OpenFileDialog fileDialog = new Microsoft.Win32.OpenFileDialog();
                    fileDialog.Multiselect = true;
                    fileDialog.Title = "请选择文件";
                    fileDialog.Filter = "BIN文件|*.bin";      //设置要选择的文件的类型
                    if (fileDialog.ShowDialog() == true)
                    {
                        this.Dispatcher.Invoke(new Action(delegate
                        {
                            UpgradeFileBox.Text = fileDialog.FileName;
                            this.parent.statusInfoTextBlock.Text = "获取升级文件成功";
                        }));


                        //将升级文件传入ymodem打开
                        this.parent.MyYmodem.ReadFileData(fileDialog.FileName);
                        UpgradeProgressBar.Maximum = this.parent.MyYmodem.GetPackNum() + 3;
                        UpgradeProgressBar.Value = 0;

                    }
                }
                else
                {
                    MessageBox.Show("请选择升级类型");
                }
            }
            else
            {
                MessageBox.Show("请先打开串口，再尝试升级");
            }
        }












        private void UpgradeButton_Click(object sender, RoutedEventArgs e)
        {
            if ((UpgradeFileBox.Text) != "升级文件在这里显示")
            {
                //发送数据锁存数据
                m_upgradeflag = true;
                if (McuButton.IsChecked == true)
                {
                    string hex = this.parent.MyYmodem.mUpgradeFile.Split('.')[1];
                    if (hex != "bin")
                    {
                        MessageBox.Show("升级文件不对应");
                        return;
                    }
                }
                if (DspButton.IsChecked == true)
                {
                    string hex = this.parent.MyYmodem.mUpgradeFile.Split('.')[1];
                    if (hex != "ldr")
                    {
                        MessageBox.Show("升级文件不对应");
                        return;
                    }
                }
                this.parent.SendDataLock();
                this.parent.InteractionInfoShow("发送数据锁存");
                this.parent.ShowBar(0, this);
                //value = 0;//清空进度条数值
                System.Threading.Thread.Sleep(3000);

                //根据选择框来判断升级对象
                this.Dispatcher.Invoke(new Action(delegate
                {

                    if (DspButton.IsChecked == true)
                    {
                        this.parent.SendData("01 10 00 1B 00 01 02 00 01 64 7B");
                        System.Threading.Thread.Sleep(3000);
                        this.parent.ChangBaudRate(230400);
                        this.parent.InteractionInfoShow("准备升级主机，修改波特率为230400");
                    }
                    else if (McuButton.IsChecked == true)
                    {
                        this.parent.SendData("01 10 00 1A 00 01 02 00 01 65 AA");
                        System.Threading.Thread.Sleep(2000);
                        this.parent.ChangBaudRate(230400);
                        this.parent.InteractionInfoShow("准备升级灯板，修改波特率为230400");
                    }

                }));

                this.parent.MyYmodem.ClearAll();
            }
            else
            {
                MessageBox.Show("升级设备前请获取文件");
            }
        }

    }


}
