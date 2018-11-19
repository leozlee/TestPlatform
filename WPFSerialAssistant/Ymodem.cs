using System;
using System.IO;
using System.Diagnostics;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

/*
Ymodem协议： 
     
     SENDER:                             RECEIVER:
     
 
     
*/
namespace WPFSerialAssistant
{

    public enum InitialCrcValue { Zeros, NonZero1 = 0xffff, NonZero2 = 0x1D0F }


    public class Ymodem
    {

        public string mUpgradeFilePath;
        public string mUpgradeFile;
        private bool mFirstPackSend = false;
        private bool mFirstPackSendAck = false;
        private bool mLastPackSend = false;
        private bool mLastPackSendAck = false;

        //协议结束包标志位
        private bool mEndPackSend = false;
        private bool mEndPackSendAck = false;


        private bool mEotSend = false;
        private bool mEotAck = false;


        private FileStream mFileStream = null;
        private long mFileLength;
        private List<byte> mFileDataBuffer = new List<byte>();
     
        
        private int mPackNum = 0;//计算分包报数
        private int mModNum = 0;
        private byte mLastPackType = 0;

        private int mPackCnt = 0;


        public void ClearAll()
        {
            mFirstPackSend = false;
            mFirstPackSendAck = false;
            mLastPackSend = false;
            mLastPackSendAck = false;

            mEndPackSend = false;
            mEndPackSendAck = false;

            mEotSend = false;
            mEotAck = false;
            packetNumber = 0x00;
            invertedPacketNumber = 0xFF;

            mPackCnt = 0;
    }



        /* control signals */

        public const byte MODEM_SOH = 0X01;       //数据块起始字符&&128字节数据包
        public const byte MODEM_STX = 0X02;       //1024字节数据包
        public const byte MODEM_EOT = 0X04;       //文件传输结束
        public const byte MODEM_ACK = 0X06;       //确认应答
        public const byte MODEM_NAK = 0X15;       //出现错误
        public const byte MODEM_CAN = 0X18;       //取消传输
        public const byte MODEM_C = 0X43;       //接收方处于接收状态



        /* sizes */
        const int dataSize = 1024;
        const int crcSize = 2;

        /* THE PACKET: 1029 bytes */
        /* header: 3 bytes */
        // STX
        public byte packetNumber = 0x00;
        public byte invertedPacketNumber = 0xFF;
        /* data: 1024 bytes */
        byte[] data = new byte[dataSize];
        /* footer: 2 bytes */
        byte[] CRC = new byte[crcSize];
        /* get the file */



        public int GetPackNum()
        {
            return mPackNum;
        }


        //发送第一包数据标志
        public void SetFirstPackSend()
        {
            mFirstPackSend = true;
        }

        public bool CheckFirstPackSend()
        {
            return mFirstPackSend;
        }

        //第一包数据发送收到的ACK
        public void SetFirstPackSendAck()
        {
            mFirstPackSendAck = true;
        }


        public bool CheckFirstPackSendAck()
        {
            return mFirstPackSendAck;
        }



        //发送最后一包数据
        public void SetLastPackSend()
        {
            mLastPackSend = true;
        }


        public bool CheckLastPackSend()
        {
            return mLastPackSend;
        }

        //收到最后一包数据应答
        public void SetLastPackSendAck()
        {
            mLastPackSendAck = true;
        }



        public void SetEotSend()
        {
            mEotSend = true;
        }


        public bool CheckEotSend()
        {
            return mEotSend;
        }


        public bool CheckEotAck()
        {
            return mEotAck;
        }

        public void SetEotAck()
        {
            mEotAck = true;
        }


        //
        public List<byte> YmodemSendEOT()
        {
            SetEotSend();
            List<byte> List = new List<byte>();
            List.Add(MODEM_EOT);
            return List;
        }



        public bool CheckLastPackSendAck()
        {
            return mLastPackSendAck;
        }


        //该函数用于获取文件基本信息
        public void ReadFileData(string path)
        {
            mUpgradeFilePath = path;
            mUpgradeFile = mUpgradeFilePath.Substring(mUpgradeFilePath.LastIndexOf("\\") + 1);
            mFileStream = new FileStream(mUpgradeFilePath, FileMode.Open, FileAccess.Read);
            mFileLength = mFileStream.Length;
            mPackNum = (int)(mFileLength >> 10);
            mModNum = (int)(mFileLength % 1024);
            byte[] Buff = new byte[mFileLength];

            if (mModNum > 0)
            {
                mPackNum++;
                if (mModNum > 128)
                {
                    mLastPackType = MODEM_STX;
                }
                else
                {
                    mLastPackType = MODEM_SOH;
                }
            }
                
            mFileStream.Read(Buff, 0, Buff.Length);
            mFileDataBuffer = Buff.ToList();
        }



        //发送第一包数据
        //第一包数据有3+128+2个字节
        public List<byte> YmodemSendFirstPacket()
        {

            //存放数据
            List<byte> FirstPack = new List<byte>();
            FirstPack.Add(MODEM_SOH);
            FirstPack.Add(0x00);
            FirstPack.Add(0XFF);

            if (mUpgradeFilePath != "升级文件在这里显示")
            {
                //填充文件名
                foreach (byte c in mUpgradeFile)
                {
                    FirstPack.Add(c);
                }

                //填充一个空格，表示文件名结束，这个数据必须添加，否则会出现错误
                FirstPack.Add(0x00);

                //填充文件大小
                foreach (byte c in mFileLength.ToString())
                {
                    FirstPack.Add(c);
                }

                //128个数据区剩下的填充0x00
                int LastByteCnt = 128 - (mUpgradeFile.Length + mFileLength.ToString().Length + 1);

                for (int i = 0; i < LastByteCnt; i++)
                {
                    FirstPack.Add(0x00);
                }

                /* calculate CRC */
                Crc16Ccitt crc16Ccitt = new Crc16Ccitt(InitialCrcValue.Zeros);
                CRC = crc16Ccitt.ComputeChecksumBytes(FirstPack.Skip(3).Take(128).ToList());

                FirstPack.Add(CRC[1]);
                FirstPack.Add(CRC[0]);

                return FirstPack;
            }
            else
            {
                
                return null;

            }

        }

        //发送协议结束包
        //SOH 0x00 0xFF 0X00[1...127] CRCH CRCL 
        public List<byte> YmodemSendEndPacket()
        {
            List<byte> LastPack = new List<byte>();

            LastPack.Add(MODEM_SOH);
            LastPack.Add(0X00);
            LastPack.Add(0XFF);
            //最后一包不用校验了，CRC就是00 00
            for(int i=0;i<130;i++)
            {
                LastPack.Add(0x00);
            }
            return LastPack;
        }


        //数据发送函数
        //3+1024+2 = 1029
        public List<byte> SendYmodemPacket()
        {
            mPackCnt++;
            packetNumber++;
            invertedPacketNumber--;
            List<byte> Package = new List<byte>();
            Package.Add(MODEM_STX);
            Package.Add(packetNumber);
            Package.Add(invertedPacketNumber);


            if (mPackCnt == mPackNum)//最后一包需要特殊处理
            {
                //首先判断最后一包数据数据包类型
                if (mLastPackType == MODEM_SOH)//数据低于128字节
                {

                    //将数据头改回来
                    Package[0] = MODEM_SOH;
                    //看看还剩下多少个字节的数据
                    for (int i = 0; i < mModNum; i++)
                    {
                        Package.Add(mFileDataBuffer[1024 * (mPackCnt - 1) + i]);
                    }
                    //不足128字节的数据使用 0X1A 填充

                    for (int i = 0; i < 128 - mModNum; i++)
                    {
                        Package.Add(0X1A);
                    }

                    /* calculate CRC */
                    Crc16Ccitt crc16Ccitt = new Crc16Ccitt(InitialCrcValue.Zeros);
                    CRC = crc16Ccitt.ComputeChecksumBytes(Package.Skip(3).Take(128).ToList());

                    Package.Add(CRC[1]);//不知为啥高低位反了
                    Package.Add(CRC[0]);

                }
                else//数据大于128字节
                {
                    //只复制剩余数据
                    for (int i = 0; i < mModNum; i++)
                    {
                        Package.Add(mFileDataBuffer[1024 * (mPackCnt - 1) + i]);
                    }

                    for (int i = 0; i < 1024 - mModNum; i++)
                    {
                        Package.Add(0X1A);
                    }

                    /* calculate CRC */
                    Crc16Ccitt crc16Ccitt = new Crc16Ccitt(InitialCrcValue.Zeros);
                    CRC = crc16Ccitt.ComputeChecksumBytes(Package.Skip(3).Take(1024).ToList());

                    Package.Add(CRC[1]);//不知为啥高低位反了
                    Package.Add(CRC[0]);
                }

                SetLastPackSend();

            }
            else
            {
                //根据包号来偏移数据
                for (int i = 0; i < 1024; i++)
                {
                    Package.Add(mFileDataBuffer[1024 * (mPackCnt - 1) + i]);
                }
                /* calculate CRC */
                Crc16Ccitt crc16Ccitt = new Crc16Ccitt(InitialCrcValue.Zeros);
                CRC = crc16Ccitt.ComputeChecksumBytes(Package.Skip(3).Take(1024).ToList());

                Package.Add(CRC[1]);//不知为啥高低位反了
                Package.Add(CRC[0]);
            }

           
            return Package;
        }


        public class Crc16Ccitt
        {
            const ushort poly = 4129;
            ushort[] table = new ushort[256];
            ushort initialValue = 0;

            public ushort ComputeChecksum(byte[] bytes)
            {
                ushort crc = this.initialValue;
                for (int i = 0; i < bytes.Length; ++i)
                {
                    crc = (ushort)((crc << 8) ^ table[((crc >> 8) ^ (0xff & bytes[i]))]);
                }
                return crc;
            }

            public byte[] ComputeChecksumBytes(byte[] bytes)
            {
                ushort crc = ComputeChecksum(bytes);
                return BitConverter.GetBytes(crc);
            }



            public ushort ComputeChecksum(List<byte> bytes)
            {
                ushort crc = this.initialValue;
                for (int i = 0; i < bytes.Count; ++i)
                {
                    crc = (ushort)((crc << 8) ^ table[((crc >> 8) ^ (0xff & bytes[i]))]);
                }
                return crc;
            }

            public byte[] ComputeChecksumBytes(List<byte> bytes)
            {
                ushort crc = ComputeChecksum(bytes);
                return BitConverter.GetBytes(crc);
            }






            public Crc16Ccitt(InitialCrcValue initialValue)
            {
                this.initialValue = (ushort)initialValue;
                ushort temp, a;
                for (int i = 0; i < table.Length; ++i)
                {
                    temp = 0;
                    a = (ushort)(i << 8);
                    for (int j = 0; j < 8; ++j)
                    {
                        if (((temp ^ a) & 0x8000) != 0)
                        {
                            temp = (ushort)((temp << 1) ^ poly);
                        }
                        else
                        {
                            temp <<= 1;
                        }
                        a <<= 1;
                    }
                    table[i] = temp;
                }
            }
        }


    }
}
