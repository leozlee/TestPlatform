﻿using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace WPFSerialAssistant
{
    public partial class MainWindow : Window
    {
        /// <summary>
        /// SerialPort对象
        /// </summary>
        private SerialPort serialPort = new SerialPort();

        private void InitSerialPort()
        {
            serialPort.DataReceived += SerialPort_DataReceived;
        }

        /// <summary>
        /// 查找端口
        /// </summary>
        private void FindPorts()
        {
            portsComboBox.ItemsSource = SerialPort.GetPortNames();
            if (portsComboBox.Items.Count > 0)
            {
                portsComboBox.SelectedIndex = 0;
                portsComboBox.IsEnabled = true;
                Information(string.Format("查找到可以使用的端口{0}个。", portsComboBox.Items.Count.ToString()));
            }
            else
            {
                portsComboBox.IsEnabled = false;
                Alert("抱歉，没有查找到端口。");
            }
        }

        private bool OpenPort()
        {
            bool flag = false;
            ConfigurePort();

            try
            {
                serialPort.Open();
                Information(string.Format("成功打开端口{0}, 波特率{1}。", serialPort.PortName, serialPort.BaudRate.ToString()));
                flag = true;
            }
            catch (Exception ex)
            {
                Alert(ex.Message);
            }

            return flag;
        }

        private bool ClosePort()
        {
            bool flag = false;

            try
            {
                serialPort.Close();
                Information(string.Format("成功关闭端口{0}。", serialPort.PortName));
                flag = true;
            }
            catch (Exception ex)
            {
                Alert(ex.Message);
            }

            return flag;
        }

        private void ConfigurePort()
        {
            serialPort.PortName = GetSelectedPortName();
            serialPort.BaudRate = GetSelectedBaudRate();
            serialPort.Parity = GetSelectedParity();
            serialPort.DataBits = GetSelectedDataBits();
            serialPort.StopBits = GetSelectedStopBits();
            serialPort.Encoding = GetSelectedEncoding();
        }

        private string GetSelectedPortName()
        {
            return portsComboBox.Text;
        }

        private int GetSelectedBaudRate()
        {
            int baudRate = 9600;
            //string conv = baudRateComboBox.Text;
            int.TryParse(baudRateComboBox.Text, out baudRate);
            return baudRate;
        }

        private Parity GetSelectedParity()
        {
            string select = parityComboBox.Text;

            Parity p = Parity.None;       
            if (select.Contains("Odd"))
            {
                p = Parity.Odd;
            }
            else if (select.Contains("Even"))
            {
                p = Parity.Even;
            }
            else if (select.Contains("Space"))
            {
                p = Parity.Space;
            }
            else if (select.Contains("Mark"))
            {
                p = Parity.Mark;
            }
           
            return p;
        }

        private int GetSelectedDataBits()
        {
            int dataBits = 8;
            int.TryParse(dataBitsComboBox.Text, out dataBits);

            return dataBits;
        }

        private StopBits GetSelectedStopBits()
        {
            StopBits stopBits = StopBits.None;
            string select = stopBitsComboBox.Text.Trim();

            if (select.Equals("1"))
            {
                stopBits = StopBits.One;
            }
            else if (select.Equals("1.5"))
            {
                stopBits = StopBits.OnePointFive;
            }
            else if (select.Equals("2"))
            {
                stopBits = StopBits.Two;
            }

            return stopBits;
        }

        private Encoding GetSelectedEncoding()
        {
            string select = encodingComboBox.Text;
            Encoding enc = Encoding.Default;

            if (select.Contains("UTF-8"))
            {
                enc = Encoding.UTF8;
            }
            else if (select.Contains("ASCII"))
            {
                enc = Encoding.ASCII;
            }
            else if (select.Contains("Unicode"))
            {
                enc = Encoding.Unicode;
            }

            return enc;
        }

        private bool SerialPortWrite(string textData)
        {
            return SerialPortWrite(textData, true);
        }

        private bool SerialPortWrite(string textData, bool reportEnable)
        {
            if (serialPort == null)
            {
                return false;
            }

            if (serialPort.IsOpen == false)
            {
                Alert("串口未打开，无法发送数据。");
                return false;
            }

            try
            {
                serialPort.Write(textData);
                if (reportEnable)
                {
                    // 报告发送成功的消息，提示用户。
                    Information(string.Format("成功发送：{0}。", textData));
                }
            }
            catch (Exception ex)
            {
                Alert(ex.Message);
                return false;
            }

            return true;
        }
    }
}