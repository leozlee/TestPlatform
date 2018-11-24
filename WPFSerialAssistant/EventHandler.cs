using System;
using System.Collections.Generic;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;

namespace WPFSerialAssistant
{
    public enum ReceiveMode
    {
        Character,  //字符显示
        Hex,        //十六进制
        Decimal,    //十进制
        Octal,      //八进制
        Binary      //二进制
    }

    public enum VersionTpye
    {
        None = 0x00,
        DSP = 0X01,
        MCU = 0x02

    }


    public enum SendMode
    {
        Character,  //字符
        Hex         //十六进制
    }
    public partial class MainWindow : Window
    {
        #region Global
        // 接收并显示的方式
        private ReceiveMode receiveMode = ReceiveMode.Character;

        // 发送的方式
        private SendMode sendMode = SendMode.Hex;

        #endregion

        public int m_showvalue   = 0;         //进度条变量
        public int m_upgradeflag = 0;      //升级程序的flag
        public int m_downloadvoiceflag = 0;      //升级程序的flag
        public int m_voicievalue = 0;
        public bool m_configflag = false;


        public bool m_resetfactshowflag = false;



        public Ymodem MyYmodem = new Ymodem();
        WPFSerialAssistant.upgradedsp m_upgradedsp = null;
        WPFSerialAssistant.downloadvoice m_downloadvoice = null;
        WPFSerialAssistant.deviceconfig m_deviceconfig = null;



        #region Event handler for menu items
        private void saveSerialDataMenuItem_Click(object sender, RoutedEventArgs e)
        {

        }

        private void saveConfigMenuItem_Click(object sender, RoutedEventArgs e)
        {
            SaveConfig();
            // 状态栏显示保存成功
            Information("配置信息保存成功。");
        }

        private void loadConfigMenuItem_Click(object sender, RoutedEventArgs e)
        {
            LoadConfig();
            // 状态栏显示加载成功
            Information("配置信息加载成功。");
        }

        private void exitMenuItem_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void serialSettingViewMenuItem_Click(object sender, RoutedEventArgs e)
        {
            bool state = serialSettingViewMenuItem.IsChecked;

            if (state == false)
            {
                serialPortConfigPanel.Visibility = Visibility.Visible;
            }
            else
            {
                serialPortConfigPanel.Visibility = Visibility.Collapsed;
                if (IsCompactViewMode())
                {
                    serialPortConfigPanel.Visibility = Visibility.Visible;
                    EnterCompactViewMode();
                }
            }

            serialSettingViewMenuItem.IsChecked = !state;
        }

        private void autoSendDataSettingViewMenuItem_Click(object sender, RoutedEventArgs e)
        {
            bool state = autoSendDataSettingViewMenuItem.IsChecked;

            if (state == false)
            {
                autoSendConfigPanel.Visibility = Visibility.Visible;
            }
            else
            {
                autoSendConfigPanel.Visibility = Visibility.Collapsed;
                if (IsCompactViewMode())
                {
                    autoSendConfigPanel.Visibility = Visibility.Visible;
                    EnterCompactViewMode();
                }
            }

            autoSendDataSettingViewMenuItem.IsChecked = !state;
        }

        private void serialCommunicationSettingViewMenuItem_Click(object sender, RoutedEventArgs e)
        {
            bool state = serialCommunicationSettingViewMenuItem.IsChecked;

            if (state == false)
            {
                serialCommunicationConfigPanel.Visibility = Visibility.Visible;
            }
            else
            {
                serialCommunicationConfigPanel.Visibility = Visibility.Collapsed;

                if (IsCompactViewMode())
                {
                    serialCommunicationConfigPanel.Visibility = Visibility.Visible;
                    EnterCompactViewMode();
                }
            }

            serialCommunicationSettingViewMenuItem.IsChecked = !state;
        }

        private void compactViewMenuItem_Click(object sender, RoutedEventArgs e)
        {
            if (IsCompactViewMode())
            {
                RestoreViewMode();
            }
            else
            {
                EnterCompactViewMode();
            }
        }

        private void aboutMenuItem_Click(object sender, RoutedEventArgs e)
        {
            WPFSerialAssistant.About about = new About();
            about.ShowDialog();            
        }


        private void upgradedspMenuItem_Click(object sender, RoutedEventArgs e)
        {
            m_upgradeflag = 1;
            m_upgradedsp = new upgradedsp(this);
            m_upgradedsp.ShowDialog();
            m_upgradeflag = 0;
        }



        public void ShowBar(double value, WPFSerialAssistant.upgradedsp m_upgradedsp)
        {
            this.Dispatcher.Invoke(new Action(delegate {
                m_upgradedsp.UpgradeProgressBar.Value = value;
            }));
        }


        public void ShowBar(double value, WPFSerialAssistant.downloadvoice m_downloadvoice)
        {
            this.Dispatcher.Invoke(new Action(delegate {
                m_downloadvoice.voicebar.Value = value;
            }));
        }







        private void upgrademcuMenuItem_Click(object sender, RoutedEventArgs e)
        {

        }

        private void upgradevoiceMenuItem_Click(object sender, RoutedEventArgs e)
        {
            m_downloadvoiceflag = 1;
            m_downloadvoice = new downloadvoice(this);
            m_downloadvoice.ShowDialog();
            m_downloadvoiceflag = 0;
        }


        private void calcconfigMenuItem_Click(object sender, RoutedEventArgs e)
        {
            m_configflag = true;
            m_deviceconfig = new deviceconfig(this);
            m_deviceconfig.ShowDialog();
            m_configflag = false;


        }

        //CRC校验函数
        public static byte[] CRC16(byte[] data, int length)
        {
            int len = length;
            if (len > 0)
            {
                ushort crc = 0xFFFF;

                for (int i = 0; i < len; i++)
                {
                    crc = (ushort)(crc ^ (data[i]));
                    for (int j = 0; j < 8; j++)
                    {
                        crc = (crc & 1) != 0 ? (ushort)((crc >> 1) ^ 0xA001) : (ushort)(crc >> 1);
                    }
                }
                byte hi = (byte)((crc & 0xFF00) >> 8);
                byte lo = (byte)(crc & 0x00FF);

                return new byte[] { lo, hi };//不知道为什么顺序是反的？？？？
            }
            return new byte[] { 0, 0 };
        }


        public string GetString(byte[] rec, int cnt)
        {
            byte[] buf = new byte[2];
            buf[0] = rec[cnt + 1];
            buf[1] = rec[cnt];
            UInt16 i = BitConverter.ToUInt16(buf, 0);
            return i.ToString();
        }





        private void modifyconfigMenuItem_Click(object sender, RoutedEventArgs e)
        {

        }

        private void orderinfoMenuItem_Click(object sender, RoutedEventArgs e)
        {
 
        }

        private void helpMenuItem_Click(object sender, RoutedEventArgs e)
        {

        }
        #endregion

        #region Event handler for buttons and so on.
        private void openClosePortButton_Click(object sender, RoutedEventArgs e)
        {
            if (serialPort.IsOpen)
            {
                if (ClosePort())
                {
                    openClosePortButton.Content = "打开";
                }
            }
            else
            {
                if (OpenPort())
                {
                    openClosePortButton.Content = "关闭";
                }
            }
        }

        private void findPortButton_Click(object sender, RoutedEventArgs e)
        {
            FindPorts();
        }

        private void autoSendEnableCheckBox_Click(object sender, RoutedEventArgs e)
        {
            if (autoSendEnableCheckBox.IsChecked == true)
            {
                Information(string.Format("使能串口自动发送功能，发送间隔：{0} {1}。", autoSendIntervalTextBox.Text, timeUnitComboBox.Text.Trim()));
            }
            else
            {
                Information("禁用串口自动发送功能。");
                StopAutoSendDataTimer();
                progressBar.Visibility = Visibility.Collapsed;
            }
        }

        private void sendDataButton_Click(object sender, RoutedEventArgs e)
        {
            if (autoSendEnableCheckBox.IsChecked == true)
            {
                AutoSendData();
            }
            else
            {
                SendData();
            }
        }


        public bool SendData(string data)
        {
            return SerialPortWrite(data);
        }


        //发送数据锁存
        public void SendDataLock()
        {
            string DataLock = "01 10 00 03 00 01 02 00 01 67 A3";
            SendData(DataLock);

        }


        public void ResetMcu()
        {
            string DataLock = "01 10 00 03 00 01 02 00 01 67 A3";
            SendData(DataLock);
            Thread.Sleep(2000);
            string resetmct = "01 10 00 1F 00 01 02 00 01 65 FF";
            SendData(resetmct);
        }






        public void ChangBaudRate(int Baud)
        {
            serialPort.BaudRate = Baud;
        }


        public void InteractionInfoShow(string info)
        {
            this.Dispatcher.Invoke(new Action(delegate {
                statusInfoTextBlock.Text = info;
            }));
        }




        private void saveRecvDataButton_Click(object sender, RoutedEventArgs e)
        {
            SaveData(GetSaveDataPath());
        }

        private void clearRecvDataBoxButton_Click(object sender, RoutedEventArgs e)
        {
            recvDataRichTextBox.Document.Blocks.Clear();
        }

        private void recvModeButton_Checked(object sender, RoutedEventArgs e)
        {
            RadioButton rb = sender as RadioButton;

            if (recvDataRichTextBox == null)
            {
                return;
            }

            if (rb != null)
            {
                //
                // TO-DO:
                // 可以将已经存在在文本框中的内容全部转换成指定形式显示，而不是简单地清空
                //
                recvDataRichTextBox.Document.Blocks.Clear();

                switch (rb.Tag.ToString())
                {
                    case "char":
                        receiveMode = ReceiveMode.Character;
                        Information("提示：字符显示模式。");
                        break;
                    case "hex":
                        receiveMode = ReceiveMode.Hex;
                        Information("提示：十六进制显示模式。");
                        break;
                    case "dec":
                        receiveMode = ReceiveMode.Decimal;
                        Information("提示：十进制显示模式。");
                        break;
                    case "oct":
                        receiveMode = ReceiveMode.Octal;
                        Information("提示：八进制显示模式。");
                        break;
                    case "bin":
                        receiveMode = ReceiveMode.Binary;
                        Information("提示：二进制显示模式。");
                        break;
                    default:
                        break;
                }
            }
        }

        private bool showReceiveData = true;
        private void showRecvDataCheckBox_Click(object sender, RoutedEventArgs e)
        {
            showReceiveData = (bool)showRecvDataCheckBox.IsChecked;
        }

        private void sendDataModeRadioButton_Click(object sender, RoutedEventArgs e)
        {
            RadioButton rb = sender as RadioButton;

            if (rb != null)
            {
                switch (rb.Tag.ToString())
                {
                    case "char":
                        sendMode = SendMode.Character;
                        Information("提示：发送字符文本。");
                        // 将文本框中内容转换成char
                        sendDataTextBox.Text = Utilities.ToSpecifiedText(sendDataTextBox.Text, SendMode.Character, serialPort.Encoding);
                        break;
                    case "hex":
                        // 将文本框中的内容转换成hex
                        sendMode = SendMode.Hex;
                        Information("提示：发送十六进制。输入十六进制数据之间用空格隔开，如：1D 2A 38。");
                        sendDataTextBox.Text = Utilities.ToSpecifiedText(sendDataTextBox.Text, SendMode.Hex, serialPort.Encoding);
                        break;
                    default:
                        break;
                }
            }
        }

        private void manualInputRadioButton_Click(object sender, RoutedEventArgs e)
        {

        }

        private void loadFileRadioButton_Click(object sender, RoutedEventArgs e)
        {

        }

        private void clearSendDataTextBox_Click(object sender, RoutedEventArgs e)
        {
            sendDataTextBox.Clear();
        }

        private void appendRadioButton_Click(object sender, RoutedEventArgs e)
        {
            RadioButton rb = sender as RadioButton;
            if (rb != null)
            {
                switch (rb.Tag.ToString())
                {
                    case "none":
                        appendContent = "";
                        break;
                    case "return":
                        appendContent = "\r";
                        break;
                    case "newline":
                        appendContent = "\n";
                        break;
                    case "retnewline":
                        appendContent = "\r\n";
                        break;
                    default:
                        break;
                }
                Information("发送追加：" + rb.Content.ToString());
            }
        }
        #endregion

        #region Event handler for timers
        private void ClockTimer_Tick(object sender, EventArgs e)
        {
            UpdateTimeDate();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void AutoSendDataTimer_Tick(object sender, EventArgs e)
        {
            bool ret = false;
            ret = SendData();

            if (ret == false)
            {
                StopAutoSendDataTimer();
            }
        }

        /// <summary>
        /// 窗口关闭前拦截
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            // 释放没有关闭的端口资源
            if (serialPort.IsOpen)
            {
                ClosePort();
            }

            // 提示是否需要保存配置到文件中
            if (MessageBox.Show("是否在退出前保存软件配置？", "小贴士", MessageBoxButton.YesNo, MessageBoxImage.Information) == MessageBoxResult.Yes)
            {
                SaveConfig();
            }
        }

        /// <summary>
        /// 捕获窗口按键。
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            // Ctrl+S保存数据
            if (e.Key == Key.S && e.KeyboardDevice.IsKeyDown(Key.LeftCtrl))
            {
                SaveData(GetSaveDataPath());
            }

            // Ctrl+Enter 进入/退出简洁视图模式
            if (e.Key == Key.Enter && e.KeyboardDevice.IsKeyDown(Key.LeftCtrl))
            {
                if (IsCompactViewMode())
                {
                    RestoreViewMode();
                }
                else
                {
                    EnterCompactViewMode();
                }
            }

            // Enter发送数据
            if (e.Key == Key.Enter)
            {
                SendData();
            }
        }

        #endregion

        #region EventHandler for serialPort
        
        // 数据接收缓冲区
        private List<byte> receiveBuffer = new List<byte>();

        // 一个阈值，当接收的字节数大于这么多字节数之后，就将当前的buffer内容交由数据处理的线程
        // 分析。这里存在一个问题，假如最后一次传输之后，缓冲区并没有达到阈值字节数，那么可能就
        // 没法启动数据处理的线程将最后一次传输的数据处理了。这里应当设定某种策略来保证数据能够
        // 在尽可能短的时间内得到处理。
        private const int THRESH_VALUE = 128;

        private bool shouldClear = true;

        /// <summary>
        /// 更新：采用一个缓冲区，当有数据到达时，把字节读取出来暂存到缓冲区中，缓冲区到达定值
        /// 时，在显示区显示数据即可。
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SerialPort_DataReceived(object sender, System.IO.Ports.SerialDataReceivedEventArgs e)
        {
            System.IO.Ports.SerialPort sp = sender as System.IO.Ports.SerialPort;

            if (sp != null)
            {
                // 临时缓冲区将保存串口缓冲区的所有数据
                int bytesToRead = sp.BytesToRead;
                byte[] tempBuffer = new byte[bytesToRead];

                // 将缓冲区所有字节读取出来
                sp.Read(tempBuffer, 0, bytesToRead);

                // 检查是否需要清空全局缓冲区先
                if (shouldClear)
                {
                    receiveBuffer.Clear();
                    shouldClear = false;
                }

                // 暂存缓冲区字节到全局缓冲区中等待处理
                receiveBuffer.AddRange(tempBuffer);

                if (receiveBuffer.Count >= THRESH_VALUE)
                {
                    //Dispatcher.Invoke(new Action(() =>
                    //{
                    //    recvDataRichTextBox.AppendText("Process data.\n");
                    //}));
                    // 进行数据处理，采用新的线程进行处理。
                    Thread dataHandler = new Thread(new ParameterizedThreadStart(ReceivedDataHandler));
                    dataHandler.Start(receiveBuffer);
                }

                // 启动定时器，防止因为一直没有到达缓冲区字节阈值，而导致接收到的数据一直留存在缓冲区中无法处理。
                StartCheckTimer();

                this.Dispatcher.Invoke(new Action(() =>
                {   
                    if (autoSendEnableCheckBox.IsChecked == false)
                    {
                        Information("");
                    }                                 
                    dataRecvStatusBarItem.Visibility = Visibility.Visible;
                }));
            }
        }

        #endregion

        #region 数据处理

        private void CheckTimer_Tick(object sender, EventArgs e)
        {
            // 触发了就把定时器关掉，防止重复触发。
            StopCheckTimer();

            // 只有没有到达阈值的情况下才会强制其启动新的线程处理缓冲区数据。
            if (receiveBuffer.Count < THRESH_VALUE)
            {
                //recvDataRichTextBox.AppendText("Timeout!\n");
                // 进行数据处理，采用新的线程进行处理。
                Thread dataHandler = new Thread(new ParameterizedThreadStart(ReceivedDataHandler));
                dataHandler.Start(receiveBuffer);
            }          
        }

        private void ReceivedDataHandler(object obj)
        {
            List<byte> recvBuffer = new List<byte>();
            recvBuffer.AddRange((List<byte>)obj);

            if (recvBuffer.Count == 0)
            {
                return;
            }

            // 必须应当保证全局缓冲区的数据能够被完整地备份出来，这样才能进行进一步的处理。
            shouldClear = true;

            this.Dispatcher.Invoke(new Action(() =>
            {
                if (showReceiveData)
                {
                    // 根据显示模式显示接收到的字节.
                    recvDataRichTextBox.AppendText(Utilities.BytesToText(recvBuffer, receiveMode, serialPort.Encoding));
                    recvDataRichTextBox.ScrollToEnd();
                }

                dataRecvStatusBarItem.Visibility = Visibility.Collapsed;
            }));

            // TO-DO：
            // 处理数据，比如解析指令等等

            #region ymodem升级
            if (m_upgradedsp != null && m_upgradedsp.m_upgradeflag && m_upgradeflag == 1)
            {
                m_showvalue++;
                switch (recvBuffer.Count)
                {
                    //case 8:         //数据锁存返回
                    //    if (recvBuffer[0] == 0x01 && recvBuffer[1] == 0x10 && recvBuffer[3] == 0x03)
                    //    {
                    //        根据选择框来判断升级对象
                    //        this.Dispatcher.Invoke(new Action(delegate
                    //        {

                    //            if (m_upgradedsp.DspButton.IsChecked == true)
                    //            {
                    //                SendData("01 10 00 1B 00 01 02 00 01 64 7B");
                    //                Thread.Sleep(2000);
                    //                ChangBaudRate(230400);
                    //                InteractionInfoShow("准备升级主机，修改波特率为230400");
                    //            }
                    //            else if (m_upgradedsp.McuButton.IsChecked == true)
                    //            {
                    //                SendData("01 10 00 1A 00 01 02 00 01 65 AA");
                    //                Thread.Sleep(2000);
                    //                ChangBaudRate(230400);
                    //                InteractionInfoShow("准备升级灯板，修改波特率为230400");
                    //            }

                    //        }));
                    //    }
                    //    break;
                    case 1:
                        switch (recvBuffer[0])
                        {
                            case Ymodem.MODEM_C:
                                this.Dispatcher.Invoke(new Action(delegate {

                                    if (m_upgradedsp.McuButton.IsChecked == true || m_upgradedsp.DspButton.IsChecked == true)
                                    {
                                        if (!MyYmodem.CheckFirstPackSend())
                                        {
                                            InteractionInfoShow("目标已就绪，发送初始包数据");
                                            //发送第一包数据
                                            SendData(MyYmodem.YmodemSendFirstPacket());
                                            //将相关标志位置位
                                            MyYmodem.SetFirstPackSend();
                                        }
                                        else if (MyYmodem.CheckFirstPackSendAck() && !MyYmodem.CheckEotAck())
                                        {
                                            SendData(MyYmodem.SendYmodemPacket());//正文传输
                                            InteractionInfoShow("收到设备的起始包的C");
                                            //ShowBar(2);
                                        }
                                        else if (MyYmodem.CheckEotAck())//发送结束包
                                        {
                                            InteractionInfoShow("结束包");
                                            SendData(MyYmodem.YmodemSendEndPacket());
                                            Thread.Sleep(1000);
                                            ChangBaudRate(115200);//将波特率修改回来
                                                                  //ShowBar(Max);
                                            m_showvalue = 0;
                                        }
                                    }
                                    else
                                    {
                                        InteractionInfoShow("请选择升级文件");

                                    }
                                }));
                                break;
                            case Ymodem.MODEM_ACK:
                                if (!MyYmodem.CheckFirstPackSendAck())
                                {
                                    MyYmodem.SetFirstPackSendAck();
                                    InteractionInfoShow("收到设备的起始包的ACK");
                                    ShowBar(1, m_upgradedsp);
                                }
                                //发送了初始包但没有发送最后一包数据包
                                else if (MyYmodem.CheckFirstPackSendAck() && !MyYmodem.CheckLastPackSend())//
                                {
                                    SendData(MyYmodem.SendYmodemPacket());//正文传输
                                    InteractionInfoShow("发送数据包......");
                                    ShowBar(m_showvalue, m_upgradedsp);
                                }
                                //发送了最后一包数据包但没有发EOT
                                else if (MyYmodem.CheckLastPackSend() && !MyYmodem.CheckEotSend())
                                {
                                    MyYmodem.SetLastPackSendAck();          //收到最后一个数据包的回应
                                    SendData(MyYmodem.YmodemSendEOT());     //发送EOT信号
                                    InteractionInfoShow("EOT信号");
                                    MyYmodem.SetEotSend();

                                    this.Dispatcher.Invoke(new Action(delegate {

                                        if (m_upgradedsp.DspButton.IsChecked == true)
                                        {
                                            MyYmodem.ClearAll();
                                            //MessageBox.Show("恭喜，升级成功");
                                            InteractionInfoShow("设备升级成功");
                                            Thread.Sleep(1000);
                                            ChangBaudRate(115200);//将波特率修改回来
                                                                  //ShowBar(Max);
                                        }
                                    }));
                                }
                                else if (MyYmodem.CheckEotSend() && !MyYmodem.CheckEotAck())
                                {
                                    MyYmodem.SetEotAck();
                                    //InteractionInfoShow("收到EOT信号ACK信号");
                                    SendData(MyYmodem.YmodemSendEndPacket());
                                    MyYmodem.ClearAll();
                                    InteractionInfoShow("设备升级成功");
                                    Thread.Sleep(1000);
                                    ChangBaudRate(115200);//将波特率修改回来
                                                          //ShowBar(Max);
                                    m_upgradedsp.m_upgradeflag = false;
                                }
                                break;
                            case Ymodem.MODEM_NAK:
                                //续传，暂不实现
                                break;
                            default:
                                InteractionInfoShow("开机信号");
                                break;
                        }
                        break;
                    case 2:
                        {
                            if (recvBuffer[0] == Ymodem.MODEM_ACK && recvBuffer[1] == Ymodem.MODEM_C)
                            {

                                if (!MyYmodem.CheckFirstPackSendAck())
                                {
                                    MyYmodem.SetFirstPackSendAck();
                                    SendData(MyYmodem.SendYmodemPacket());//正文传输
                                    InteractionInfoShow("收到设备的起始包的C,开始传输");
                                }
                            }
                        }
                        break;
                    default:
                        //MessageBox.Show("升级成功:)");
                        break;
                }
            }
            #endregion



            #region 烧录语音

            if (m_downloadvoice != null && m_downloadvoice.m_dlflag 
                && m_downloadvoiceflag == 1 && recvBuffer.Count == 4) 
            {
                ++m_voicievalue;
                if (recvBuffer[2] == 0x00 && recvBuffer[3] == 0x00)
                {
                    SendData(m_downloadvoice.makepackage(0x01, 0x01));
                    ShowBar(m_voicievalue, m_downloadvoice);  
                }
                else if (recvBuffer[3] == 0x30)
                {
                    if (recvBuffer[2] >= m_downloadvoice.cnt)
                    {
                        Thread.Sleep(2000);
                        InteractionInfoShow("烧录语音完成,重启设备");
                        m_downloadvoiceflag = 0;
                        m_downloadvoice.m_dlflag = false;
                        ResetMcu();
                        return;
                    }
                       
                    SendData(m_downloadvoice.makepackage((Byte)(recvBuffer[2] + 1), 0x01));
                    ShowBar(m_voicievalue, m_downloadvoice);
                }
                else
                {
                    SendData(m_downloadvoice.makepackage(recvBuffer[2], (Byte)(recvBuffer[3] + 1)));
                    ShowBar(m_voicievalue, m_downloadvoice);
                }
            }

            #endregion

            #region 校验参数

            if (m_deviceconfig != null && m_configflag)
            {
                switch (recvBuffer.Count)
                {
                    case 6:
                        if (recvBuffer[0] == 0x01 && recvBuffer[1] == 0x90)
                        {
                            MessageBox.Show("设置参数不成功");
                        }
                        break;
                    case 8:
                        if (m_deviceconfig.m_resetfact)
                        {
                            m_deviceconfig.m_resetfact = false;
                            SendData("01 10 00 35 00 01 02 00 01 62 35");
                            m_resetfactshowflag = true;
                        }
                        if (m_resetfactshowflag)
                        {
                            m_resetfactshowflag = false;
                            m_deviceconfig.Clear();
                            this.Dispatcher.Invoke(new Action(delegate
                            {
                                statusInfoTextBlock.Text = "恢复原厂参数成功!";
                            }));
                        }

                        break;
                    case 125:
                        if(recvBuffer[0] == 0x01 && recvBuffer[1] == 0x03)
                        {
                            byte[] Data = recvBuffer.ToArray();
                            //计算CRC
                            byte[] Crc16 = CRC16(Data, Data.Length - 2);

                            //校验
                            if (Crc16[0] == Data[Data.Length - 2] && Crc16[1] == Data[Data.Length - 1])
                            {
                                //pass
                                this.Dispatcher.Invoke(new Action(delegate
                                {
                                    statusInfoTextBlock.Text = "获取参数成功";
                                }));
                                //将参数保存在一个数组中
                                for (int i = 0; i < ((Data.Length - 5) >> 1); i++)
                                {
                                    m_deviceconfig.Para[i] = GetString(Data, i * 2 + 3);//跳过head的三个字节
                                }
                                //将参数显示在对应的位置
                                Array.Copy(Data, 3, m_deviceconfig.ReadPara, 0, WPFSerialAssistant.deviceconfig.ParaCnt * 2);       //将读取的参数保存，用于对比
                                m_deviceconfig.FillBox(m_deviceconfig.Para);
                               

                            }
                            else
                            {
                                MessageBox.Show("CRC校验不通过，请再次获取参数");
                            }
                        }
                        break;
                    default:
                        break;
                }
                

            }


            #endregion



        }
        #endregion
    }
}
