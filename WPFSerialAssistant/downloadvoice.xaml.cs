using System;
using System.IO;
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

        Byte[] databuffer = new Byte[50000];

        private FileStream mFileStream = null;
        private Byte high = 0;
        private Byte low = 0;
        public bool m_dlflag = false;
        public int cnt = 0;



        Byte Crc8(Byte[] puchMsg, UInt16 usDataLen)    //crcData = crc16(sendbuf,3);
        {
            UInt16 uIndex = 0;
            Byte revalue = 0;
            for (UInt16 i = 0; i < usDataLen; ++i)
                revalue += puchMsg[uIndex++];
            return revalue;
        }




        public Byte[ ] makepackage(Byte file_num, Byte package)
        {
            Byte[] buffer = new Byte[1007];
            buffer[0] = 0XAA;
            buffer[1] = 0X85;
            if (package == 1)
            {

                this.Dispatcher.Invoke(new Action(delegate
                {
                    mFileStream = new FileStream(((TextBox)FindName("FileBox" + (file_num - 1).ToString())).Text, FileMode.Open, FileAccess.Read);
                    this.parent.statusInfoTextBlock.Text = "获取升级文件成功";
                }));
                 
                high = (Byte)((mFileStream.Length - 44)/256);
                low  = (Byte)((mFileStream.Length - 44) % 256);

                mFileStream.Seek(44, SeekOrigin.Begin);
                mFileStream.Read(databuffer, 0, (int)mFileStream.Length);
                databuffer[0] = high;
                databuffer[1] = low;
            }

            buffer[2] = file_num;
            buffer[3] = high;
            buffer[4] = low;
            buffer[5] = package;

            Array.Copy(databuffer, (package - 1) * 1000, buffer, 6,1000);
            buffer[1006] = Crc8(buffer, 1006);

            return buffer;
        }

        //public List<byte> m_voicebuffer = new List<byte>();
        
        
        
        bool CheckUartIsOpened()
        {
            if ((string)(this.parent.openClosePortButton.Content) == "关闭")
            { 
                return true;
            }
            else
            {
                return false;

            }
        }

        #region 试听语音

        private void button1_Click(object sender, RoutedEventArgs e)
        {
            if (!CheckUartIsOpened())
            {
                MessageBox.Show("请先打开串口，再试听语音");
                return;
            }
            this.parent.SendData("AA 86 00");
            this.parent.statusInfoTextBlock.Text = "试听开机语音";
        }


        private void button2_Click(object sender, RoutedEventArgs e)
        {
            if (!CheckUartIsOpened())
            {
                MessageBox.Show("请先打开串口，再试听语音");
                return;
            }
            this.parent.SendData("AA 86 01");
            this.parent.statusInfoTextBlock.Text = "试听报警前语音";
        }

        private void button3_Click(object sender, RoutedEventArgs e)
        {
            if (!CheckUartIsOpened())
            {
                MessageBox.Show("请先打开串口，再试听语音");
                return;
            }
            this.parent.SendData("AA 86 02");
            this.parent.statusInfoTextBlock.Text = "试听疲劳报警语音";
        }

        private void button4_Click(object sender, RoutedEventArgs e)
        {
            if (!CheckUartIsOpened())
            {
                MessageBox.Show("请先打开串口，再试听语音");
                return;
            }
            this.parent.SendData("AA 86 03");
            this.parent.statusInfoTextBlock.Text = "试听分心报警语音";
        }

        private void button5_Click(object sender, RoutedEventArgs e)
        {
            if (!CheckUartIsOpened())
            {
                MessageBox.Show("请先打开串口，再试听语音");
                return;
            }
            this.parent.SendData("AA 86 04");
            this.parent.statusInfoTextBlock.Text = "试听报警后语音";
        }

        private void button6_Click(object sender, RoutedEventArgs e)
        {
            if (!CheckUartIsOpened())
            {
                MessageBox.Show("请先打开串口，再试听语音");
                return;
            }
            this.parent.SendData("AA 86 05");
            this.parent.statusInfoTextBlock.Text = "试听无人相报警语音";
        }

        private void button7_Click(object sender, RoutedEventArgs e)
        {
            if (!CheckUartIsOpened())
            {
                MessageBox.Show("请先打开串口，再试听语音");
                return;
            }
            this.parent.SendData("AA 86 06");
            this.parent.statusInfoTextBlock.Text = "试听GPS连接语音";
        }

        private void button8_Click(object sender, RoutedEventArgs e)
        {
            if (!CheckUartIsOpened())
            {
                MessageBox.Show("请先打开串口，再试听语音");
                return;
            }
            this.parent.SendData("AA 86 07");
            this.parent.statusInfoTextBlock.Text = "试听GSP断开语音";
        }

        private void button9_Click(object sender, RoutedEventArgs e)
        {
            if (!CheckUartIsOpened())
            {
                MessageBox.Show("请先打开串口，再试听语音");
                return;
            }
            this.parent.SendData("AA 86 08");
            this.parent.statusInfoTextBlock.Text = "试听超速报警语音";
        }

        private void button10_Click(object sender, RoutedEventArgs e)
        {
            if (!CheckUartIsOpened())
            {
                MessageBox.Show("请先打开串口，再试听语音");
                return;
            }
            this.parent.SendData("AA 86 09");
            this.parent.statusInfoTextBlock.Text = "试听打电话报警语音";
        }

        private void button11_Click(object sender, RoutedEventArgs e)
        {
            if (!CheckUartIsOpened())
            {
                MessageBox.Show("请先打开串口，再试听语音");
                return;
            }
            this.parent.SendData("AA 86 0a");
            this.parent.statusInfoTextBlock.Text = "试听抽烟报警语音";
        }

        private void button12_Click(object sender, RoutedEventArgs e)
        {
            if (!CheckUartIsOpened())
            {
                MessageBox.Show("请先打开串口，再试听语音");
                return;
            }
            this.parent.SendData("AA 86 0b");
            this.parent.statusInfoTextBlock.Text = "试听打哈欠语音";
        }

        #endregion

        private void button_13_Click(object sender, RoutedEventArgs e)
        {

            Microsoft.Win32.OpenFileDialog fileDialog = new Microsoft.Win32.OpenFileDialog();
            fileDialog.Multiselect = true;
            fileDialog.Title = "请选择文件";
            fileDialog.Filter = "语音文件|*.wav";      //设置要选择的文件的类型
            if (fileDialog.ShowDialog() == true)
            {
                cnt = System.IO.Directory.GetFiles(System.IO.Path.GetDirectoryName(fileDialog.FileName)).Length;
                for (int i = 0; i < cnt; ++i)
                {
                    ((TextBox)FindName("FileBox" +　i.ToString())).Text = System.IO.Directory.GetFiles(System.IO.Path.GetDirectoryName(fileDialog.FileName)).ElementAt(i);
                }

                voicebar.Maximum = 48 * cnt;        //由于历史原因，每一个语音一共有48包组成，每一包字节数为1007，空白处补0
                voicebar.Value = 0;

                if (cnt == 9)
                {
                    ((TextBox)FindName("FileBox9")).Text = "";
                    ((TextBox)FindName("FileBox10")).Text = "";
                    ((TextBox)FindName("FileBox11")).Text = "";
                } 
                
            }
        }


        private void button_14_Click(object sender, RoutedEventArgs e)
        {

            if ((string)(this.parent.openClosePortButton.Content) == "关闭")
            {
                if (!((TextBox)FindName("FileBox0")).Text.EndsWith(".wav"))
                {
                    MessageBox.Show("烧录前请先加载语音文件");
                    return;
                }

                m_dlflag = true;

                string unlockdata = "01 10 00 23 00 01 02 00 01 60 C3";
                this.parent.SendData(unlockdata);
                System.Threading.Thread.Sleep(2000);

                string wipedata = "AA 85 00 00 00 00 2F";
                this.parent.SendData(wipedata);

            }
            else
            {
                MessageBox.Show("请先打开串口，再尝试升级");
            }

        }


        private void button_15_Click(object sender, RoutedEventArgs e)
        {
            if (!CheckUartIsOpened())
            {
                MessageBox.Show("请先打开串口，再试听语音");
                return;
            }
    
            if(!((TextBox)FindName("FileBox1")).Text.EndsWith(".wav"))
            {
                MessageBox.Show("请先获取加载语音文件再试听");
                return;
            }

          


            string path = ((TextBox)FindName("FileBox1")).Text;
            int cnt = System.IO.Directory.GetFiles(System.IO.Path.GetDirectoryName(path)).Length;
            for (int i = 0; i < cnt; ++i)
            {
                this.parent.SendData("AA 86 " + i.ToString("X2"));
                System.Threading.Thread.Sleep(2500);
            }
        }

    }









}
