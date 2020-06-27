#define PROGRAM_RUNNING
//#define SERIAL_TYPE

using System;
using System.Text;
using System.Windows.Forms;
using PIO821_Ns;
using PISODIO_Ns;
using System.Collections;
using System.IO.Ports;
using System.Linq;
using AxXtremeSuiteControls;

namespace 워크인_1열
{
    public class IOControl//:Form
    {
        MyInterface mControl = null;

        const uint dwDataNum = 2;
        float[] fBuf = new float[dwDataNum];

        ushort wTotalBoard;
        int BoaredMaximum = 0;

        SerialPort SmartIO = new SerialPort();

        bool Open = false;
        Timer timer1 = new Timer();

        public IOControl()
        {
            timer1.Interval = 10;
            timer1.Tick += new EventHandler(Timer1_Tick);
            timer1.Enabled = true;
        }

        /// <summary>
        /// IO 제허 함수를 생성한다.
        /// </summary>
        /// <param name="mControl"></param>
        public IOControl(MyInterface mControl)
        {
            this.mControl = mControl;
            timer1.Interval = 10;
            timer1.Tick += new EventHandler(Timer1_Tick);
            timer1.Enabled = true;
            return;
        }
        private ushort SmartIO_out_old = 0xffff;
        private int smart_io_out_count = 0;

        public short Ch = 0;
        public void Timer1_Tick(object sender, EventArgs e1)
        {
            try
            {
                timer1.Enabled = false;
                if (mControl.GetConfig.UseSmartIO == true)
                {
                    if (Open == true)
                    {
                        ushort SmartIO_out = (ushort)((OutData[0] << 0) | (OutData[1] << 8));
                        if (SmartIO_out != SmartIO_out_old)
                        {
                            if (2 <= smart_io_rx_time)
                            {
                                outportb();
                                smart_io_out_count++;
                                if (2 <= smart_io_out_count) SmartIO_out_old = SmartIO_out;
                                smart_io_rx_time = 0;
                            }
                            else
                            {
                                smart_io_rx_time++;
                            }
                            smart_io_rx_flg = true;
                        }
                        else
                        {
                            if (smart_io_rx_flg == true)
                            {
                                smart_io_rx_time++;
                                if (2 <= smart_io_rx_time) IOInReadMsgSending();
                            }
                            else
                            {
                                smart_io_rx_time++;
                                if (50 <= smart_io_rx_time) smart_io_rx_flg = true;
                            }
                        }
                    }
                }
                else
                {
                    if (Ch < 0) Ch = 0;
                    if (1 < Ch) Ch = 0;
                    InData[Ch] = inportbdata(Ch);
                    Ch++;
                }
            }
            catch
            {
                timer1.Enabled = !mControl.isExit;
            }
            finally
            {
                timer1.Enabled = !mControl.isExit;
            }
        }

        /// <summary>
        /// I/O 제어 보드를 오픈한다.
        /// </summary>
        /// <returns></returns>

        public bool OpenIO()
        {
#if PROGRAM_RUNNING
            try
            {
                try
                {
                    wTotalBoard = PIO821.PIO821_TotalBoard();
                    if (wTotalBoard == 0)
                    {
                        MessageBox.Show("NO PIO-821 CARD !!!");
                        Open = false;
                        return false;
                    }
                    //active board
                    PIO821.PIO821_ActiveBoard(0);
                    BoaredMaximum = wTotalBoard - 1;
                    Open = true;
                    IOInit();
                }
                catch (Exception Msg)
                {
                    MessageBox.Show(Msg.Message + "\n" + Msg.StackTrace);
                }
            }
            finally
            {
            }
#endif
            return true;
        }

        public bool OpenIO(string Port, short Speed)
        {
#if PROGRAM_RUNNING
            try
            {
                try
                {              
                    if ((Port != null) && (Port != "") && (Port != string.Empty))
                    {
                        string[] sName = System.IO.Ports.SerialPort.GetPortNames();

                        if (sName.Contains(Port) == true)
                        {
                            SmartIO.PortName = Port;                            
                            switch (Speed)
                            {
                                case 0: SmartIO.BaudRate = 2400; break;
                                case 1: SmartIO.BaudRate = 4800; break;
                                case 2: SmartIO.BaudRate = 9600; break;
                                case 3: SmartIO.BaudRate = 11400; break;
                                case 4: SmartIO.BaudRate = 19200; break;
                                case 5: SmartIO.BaudRate = 38400; break;
                                case 6: SmartIO.BaudRate = 57600; break;
                                default: SmartIO.BaudRate = 115200; break;
                            }

                            if (SmartIO.IsOpen == true) SmartIO.Close();
                            SmartIO.Parity = Parity.None;
                            SmartIO.DataBits = 8;
                            SmartIO.StopBits = StopBits.One;
                            SmartIO.ReadTimeout = -1;
                            SmartIO.WriteTimeout = -1;
                            SmartIO.ReceivedBytesThreshold = 1;
                            SmartIO.ReadBufferSize = 4096;
                            SmartIO.WriteBufferSize = 2048;

                            SmartIO.DataReceived += new SerialDataReceivedEventHandler(IODataReceive);
                            SmartIO.Open();

                            Open = SmartIO.IsOpen;
                        }
                    }

                    IOInit();
                }
                catch (Exception Msg)
                {
                    MessageBox.Show(Msg.Message + "\n" + Msg.StackTrace);
                }
            }
            finally
            {
            }
#endif
            return true;
        }

        private void IODataReceive(object sender, SerialDataReceivedEventArgs e)
        {
            //this.Invoke(new EventHandler(ReadSmartIOData));

            ReadSmartIOData();

            return;
        }

        /// <summary>
        /// I/O 제어 보드를 오픈 했는지 여부를 같는다.
        /// </summary>
        public bool isOpen
        {
            get { return Open; }
        }

        /// <summary>
        /// I/O 제어 보드를 닫는다.
        /// </summary>
        public void CloseIO()
        {
#if PROGRAM_RUNNING
            try
            {
                try
                {
                    if (mControl.GetConfig.UseSmartIO == true)
                    {
                        Open = false;

                        SmartIO.Close();
                    }
                    else
                    {
                        PIO821.PIO821_CloseBoard(0);
                    }
                }
                catch (Exception Msg)
                {
                    MessageBox.Show(Msg.Message + "\n" + Msg.StackTrace);
                }
            }
            finally
            {
            }
#endif
            return;
        }

        public byte[] InData = { 0x00, 0x00 };

        /// <summary>
        /// I/O 입력 데이타 값을 읽어 온다.
        /// </summary>
        public byte[] GetInData
        {
            get { return InData; }
        }
       

        private byte[] MODBUS_RTU_In = { 0x01, 0x04, 0x00, 0x00, 0x00, 0x01, 0, 0 };	//GSL-DT4C input
        public void IOInReadMsgSending()
        {
            ushort crc16;

            
            crc16 = CRC16(MODBUS_RTU_In, MODBUS_RTU_In.Length - 2);
            MODBUS_RTU_In[MODBUS_RTU_In.Length - 2] = (byte)((crc16 >> 0) & 0x00ff);
            MODBUS_RTU_In[MODBUS_RTU_In.Length - 1] = (byte)((crc16 >> 8) & 0x00ff);
            SmartIO.Write(MODBUS_RTU_In, 0, MODBUS_RTU_In.Length); // CW-DIO32 in
            //SmartIO.DiscardInBuffer();
            //SmartIO.ReceivedBytesThreshold = 6;
            smart_io_rx_time = 0;
            smart_io_rx_flg = false;            
            return;
        }


        /// <summary>
        /// 지정 포트가 동작 되었는지 읽어 온다.
        /// </summary>
        /// <param name="Pos"></param>
        /// <returns></returns>

        public bool inportb(short Pos)
        {
            __IOData__ Value = IOCheck(Pos);

#if PROGRAM_RUNNING
            bool Data = false;


            ushort DIVal = 0;

            ushort wRetVal;
            //receive the digital single on the board you choose
            wRetVal = PIO821.PIO821_DigitalIn((byte)0, out DIVal);
            if (wRetVal > 0)
            {
                MessageBox.Show("*** PIO821_DigitalIn()  error ! ***");
            }
            else
            {
                Data = false;
                if (((byte)~DIVal & Value.Data) == Value.Data) Data = true;
            }



            return Data;
#else
            return false;
#endif
        }



        /// <summary>
        /// 인수로 넘어온 데이타 중 지정 포트가 동작 되었는지 알아 낸다.
        /// </summary>
        /// <param name="Pos"></param>
        /// <param name="DIVal"></param>
        /// <returns></returns>
        public bool inportb(short Pos, byte[] DIVal)
        {
            __IOData__ Value = IOCheck(Pos);

#if PROGRAM_RUNNING
            bool Data;
            //uint DIVal = 0;

            //wInitialCode = UniDAQ.Ixud_ReadDI(P32ToIn[Value.Card], (ushort)Value.Pos, ref DIVal);

            Data = false;
            if (((byte)DIVal[Value.Pos] & Value.Data) == Value.Data) Data = true;
            return Data;
#else
            return false;
#endif

        }
        /// <summary>
        /// 채널별 I/O 입력을 읽어온다.
        /// </summary>
        /// <param name="Ch"></param>
        /// <returns></returns>
        public byte inportbdata(short Ch)
        {
#if PROGRAM_RUNNING
            ushort DIVal = 0;
            byte Data = 0x00;

            ushort wRetVal;
            //receive the digital single on the board you choose
            wRetVal = PIO821.PIO821_DigitalIn((byte)0, out DIVal);
            if (wRetVal > 0)
            {
                MessageBox.Show("*** PIO821_DigitalIn()  error ! ***");
            }
            else
            {
                if (Ch == 0)
                    Data = (byte)(DIVal & 0x00ff);
                else Data = (byte)((DIVal & 0xff00) >> 8);
            }
            return Data;
#else
            return (byte)0x00;
#endif
        }

        private byte[] OutData = { 0x00, 0x00 };


        /// <summary>
        /// P32C32 지정된 포트를 On/Off 한다.
        /// </summary>
        /// <param name="Pos"></param>
        /// <param name="OnOff"></param>
        public void outportb(short Pos, bool OnOff)
        {
#if PROGRAM_RUNNING
            __IOData__ Value = IOCheck(Pos);

            if (OnOff == true)
                OutData[Value.Pos] |= Value.Data;
            else OutData[Value.Pos] &= (byte)~Value.Data;
            //outportb();
#endif
            return;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="Pos"></param>
        /// <param name="OnOff"></param>
        public void outportb(short Pos, byte Data)
        {
#if PROGRAM_RUNNING

            OutData[Pos] = Data;
            //outportb();
#endif
            return;
        }


        /// <summary>
        /// P32C32 지정된 포트가 동작하고 있는지 읽어 온다.
        /// </summary>
        /// <param name="Pos"></param>
        /// <returns></returns>
        public bool OutputCheck(short Pos)
        {
            __IOData__ Value = IOCheck(Pos);

            if ((OutData[Value.Pos] & Value.Data) == Value.Data)
                return true;
            else return false;
        }

        //byte[] MODBUS_RTU_Out = { 국번, 기능코드 , 시작 주소 High, 시작 주소 Low, Data High, DataLow, Data Length , 제어 상태, Crc, Crc };	//GSL-DT4C coils write
        byte[] MODBUS_RTU_Out = { 0x01, 0x10, 0x00, 0x00, 0x00, 0x01, 0x02, 0, 0, 0, 0 };	//GSL-DT4C coils write
        //byte[] MODBUS_RTU_Out = { 0x01, 0x0f, 0x00, 0x00, 0x00, 0x0f, 0x02, 0x00, 0x00, 0x00, 0x00 };


        /// <summary>
        /// P32C32 지정된 카드에 지정된 포트로 출력을 On/Off 한다.
        /// </summary>
        /// <param name="Card"></param>
        /// <param name="Port"></param>
        /// <param name="Value"></param>
        public void outportb()
        {
#if PROGRAM_RUNNING

            try
            {
                try
                {
                    if (mControl.GetConfig.UseSmartIO == true)
                    {
                        ushort crc16;

                        MODBUS_RTU_Out[8] = OutData[0];
                        MODBUS_RTU_Out[7] = OutData[1];

                        crc16 = CRC16(MODBUS_RTU_Out, MODBUS_RTU_Out.Length - 2);
                        MODBUS_RTU_Out[MODBUS_RTU_Out.Length - 2] = (byte)((crc16 >> 0) & 0x00ff);
                        MODBUS_RTU_Out[MODBUS_RTU_Out.Length - 1] = (byte)((crc16 >> 8) & 0x00ff);

                        if (SmartIO.IsOpen == true)
                        {
                            //SmartIO.DiscardInBuffer();
                            //SmartIO.ReceivedBytesThreshold = 8;
                            SmartIO.Write(MODBUS_RTU_Out, 0, MODBUS_RTU_Out.Length);
                        }
                        //SmartIO_out_old = SmartIO_out;
                    }
                    else
                    {
                        ushort Out = (ushort)(OutData[0] | (OutData[1] << 8));
                        PIO821.PIO821_DigitalOut((byte)0, Out);
                    }
                }
                catch (Exception Msg)
                {
                    MessageBox.Show(Msg.Message + "\n" + Msg.StackTrace);
                }
            }
            finally
            {
            }
#endif
            return;
        }

        /// <summary>
        /// P32C32 지정된 포트의 I/O 위치를 알아낸다.
        /// </summary>
        /// <param name="Pos"></param>
        /// <returns></returns>
        public __IOData__ IOCheck(short Pos)
        {
            __IOData__ value = new __IOData__();

            if (Pos == -1) return value;

            int OPos = (int)Pos / 8;
            byte Data = (byte)(0x01 << ((int)Pos % 8));

            value.Card = (short)(OPos / 2);
            value.Pos = (short)(OPos % 2);
            value.Data = Data;

            return value;
        }

        /// <summary>
        /// C64 지정된 포트의 I/O 위치를 알아낸다.
        /// </summary>
        /// <param name="Pos"></param>
        /// <returns></returns>
        //public __IOData__ C64IOCheck(short Pos)
        //{
        //    __IOData__ value = new __IOData__();

        //    if (Pos == -1) return value;
        //    int OPos = (int)Pos / 8;
        //    byte Data = (byte)(0x01 << ((int)Pos % 8));

        //    value.Card = (short)(OPos / 8);
        //    value.Pos = (short)(OPos % 8);
        //    value.Data = Data;

        //    return value;
        //}

        /// <summary>
        /// P32C32 출력 초기화
        /// </summary>
        public void IOInit()
        {
            //outportb(IO_OUT.특성_UIP_SERVO_SELECT, false);

            OutData[0] = 0x00;
            OutData[1] = 0x00;
            
            outportb(0, OutData[0]);
            outportb(1, OutData[1]);
            
            return;
        }

        /// <summary>
        /// P32C32 출력 초기화
        /// </summary>
        //public void C64IOInit()
        //{
        //    //outportb(IO_OUT.특성_UIP_SERVO_SELECT, false);

        //    C64.OutData[0] = 0x00;
        //    C64.OutData[1] = 0x00;
        //    C64.OutData[2] = 0x00;
        //    C64.OutData[3] = 0x00;
        //    C64.OutData[4] = 0x00;
        //    C64.OutData[5] = 0x00;
        //    C64.OutData[6] = 0x00;
        //    C64.OutData[7] = 0x00;

        //    C64outportb(0, 0, 0x00);
        //    C64outportb(0, 1, 0x00);
        //    C64outportb(0, 2, 0x00);
        //    C64outportb(0, 3, 0x00);
        //    C64outportb(0, 4, 0x00);
        //    C64outportb(0, 5, 0x00);
        //    C64outportb(0, 6, 0x00);
        //    C64outportb(0, 7, 0x00);
        //    return;
        //}

        /// <summary>
        /// Start SW 입력 상태를 읽어 온다.
        /// </summary>
        public bool GetStartSW
        {
            get
            {
                __IOData__ Value;

                if (mControl.GetConfig.UseSmartIO == true)
                    Value = IOCheck(IO_IN2.PASS_SW);
                else Value = IOCheck(IO_IN.PASS_SW);

#if PROGRAM_RUNNING
                bool Data;
                //uint DIVal = 0;

                //wInitialCode = UniDAQ.Ixud_ReadDI(P32ToIn[Value.Card], (ushort)Value.Pos, ref DIVal);

                Data = false;
                if (((byte)InData[Value.Pos] & Value.Data) == Value.Data) Data = true;
                return Data;
#else
            return false;
#endif
            }
        }

        /// <summary>
        /// Reset SW 입력 상태를 읽어 온다.
        /// </summary>
        public bool GetResetSW
        {
            get
            {
                __IOData__ Value;

                if (mControl.GetConfig.UseSmartIO == true)
                    Value = IOCheck(IO_IN2.RESET_SW);
                else Value = IOCheck(IO_IN.RESET_SW);

#if PROGRAM_RUNNING
                bool Data;
                //uint DIVal = 0;

                //wInitialCode = UniDAQ.Ixud_ReadDI(P32ToIn[Value.Card], (ushort)Value.Pos, ref DIVal);

                Data = false;
                if (((byte)InData[Value.Pos] & Value.Data) == Value.Data) Data = true;
                return Data;
#else
            return false;
#endif
            }
        }

        /// <summary>
        /// LH / RH 선택
        /// </summary>
        public bool GetLHSellect
        {
            get
            {
                __IOData__ Value;
                
                if (mControl.GetConfig.UseSmartIO == true)
                    Value = IOCheck(IO_IN2.LH_SELECT);
                else Value = IOCheck(IO_IN.LH_SELECT);

#if PROGRAM_RUNNING
                bool Data;
                //uint DIVal = 0;

                //wInitialCode = UniDAQ.Ixud_ReadDI(P32ToIn[Value.Card], (ushort)Value.Pos, ref DIVal);

                Data = false;
                if (((byte)InData[Value.Pos] & Value.Data) == Value.Data) Data = true;
                return Data;
#else
            return false;
#endif
            }
        }

        /// <summary>
        /// AUTO SW 입력 상태를 읽어 온다.
        /// </summary>
        public bool GetAuto
        {
            get
            {
                __IOData__ Value;

                if (mControl.GetConfig.UseSmartIO == true)
                    Value = IOCheck(IO_IN2.AUTO_SW);
                else Value = IOCheck(IO_IN.AUTO_SW);

#if PROGRAM_RUNNING
                bool Data;
                //uint DIVal = 0;

                //wInitialCode = UniDAQ.Ixud_ReadDI(P32ToIn[Value.Card], (ushort)Value.Pos, ref DIVal);

                Data = false;
                if (mControl.GetConfig.UseSmartIO == true)
                {
                    if (((byte)InData[Value.Pos] & Value.Data) != Value.Data) Data = true;
                }
                else
                {
                    if (((byte)InData[Value.Pos] & Value.Data) == Value.Data) Data = true;
                }
                return Data;
#else
            return false;
#endif
            }
        }
        /// <summary>
        /// 지그 상승 입력 상태를 읽어 온다.
        /// </summary>
        public bool GetJigUp
        {
            get
            {
                __IOData__ Value;
                if (mControl.GetConfig.UseSmartIO == true)
                    Value = IOCheck(IO_IN2.JIG_UP);
                else Value = IOCheck(IO_IN.JIG_UP);

#if PROGRAM_RUNNING
                bool Data;
                //uint DIVal = 0;

                //wInitialCode = UniDAQ.Ixud_ReadDI(P32ToIn[Value.Card], (ushort)Value.Pos, ref DIVal);

                Data = false;
                if (((byte)InData[Value.Pos] & Value.Data) == Value.Data) Data = true;
                return Data;
#else
            return false;
#endif
            }
        }
        /// <summary>
        /// 제품감지 입력 상태를 읽어 온다.
        /// </summary>
        public bool GetProductIn
        {
            get
            {
                __IOData__ Value;

                if (mControl.GetConfig.UseSmartIO == true)
                    Value = IOCheck(IO_IN2.PRODUCT_IN);
                else Value = IOCheck(IO_IN.PRODUCT_IN);
#if PROGRAM_RUNNING
                bool Data;
                //uint DIVal = 0;

                //wInitialCode = UniDAQ.Ixud_ReadDI(P32ToIn[Value.Card], (ushort)Value.Pos, ref DIVal);

                Data = false;
                if (((byte)InData[Value.Pos] & Value.Data) == Value.Data) Data = true;
                return Data;
#else
            return false;
#endif
            }
        }

        /// <summary>
        /// 검사 유무 선택을 가지고 온다.
        /// </summary>
        public bool GetWalkIn
        {
            get
            {

                if (mControl.GetConfig.UseSmartIO == false)
                {
                    __IOData__ Value = IOCheck(IO_IN.OPTION_WALKIN);
#if PROGRAM_RUNNING
                    bool Data;
                    //uint DIVal = 0;

                    //wInitialCode = UniDAQ.Ixud_ReadDI(P32ToIn[Value.Card], (ushort)Value.Pos, ref DIVal);

                    Data = false;
                    if (((byte)InData[Value.Pos] & Value.Data) == Value.Data) Data = true;
                    return Data;
#else
            return false;
#endif
                }
                else
                {
                    return true;
                }
            }
        }

        /// <summary>
        /// USB 입력 상태를 가지고 온다.
        /// </summary>
        public bool GetUsb
        {
            get
            {
                if (mControl.GetConfig.UseSmartIO == false)
                {
                    __IOData__ Value = IOCheck(IO_IN.USB);
#if PROGRAM_RUNNING
                    bool Data;
                    //uint DIVal = 0;

                    //wInitialCode = UniDAQ.Ixud_ReadDI(P32ToIn[Value.Card], (ushort)Value.Pos, ref DIVal);

                    Data = false;
                    if (((byte)InData[Value.Pos] & Value.Data) == Value.Data) Data = true;
                    return Data;
#else
                    return true;
#endif
                }
                else
                {
                    return true;
                }
            }
        }

        /// <summary>
        /// 제품 감지 신호를 PLC 로 출력한다.
        /// </summary>
        public bool SetProductIn
        {
            get
            {
                if (mControl.GetConfig.UseSmartIO == true)
                    return OutputCheck(IO_OUT2.PRODUCT_IN);
                else return OutputCheck(IO_OUT.PRODUCT_IN);

            }
            set
            {
                if (mControl.GetConfig.UseSmartIO == true)
                    outportb(IO_OUT2.PRODUCT_IN, value);
                else outportb(IO_OUT.PRODUCT_IN, value);
            }
        }
        
        /// <summary>
        /// 파워 시트 ign1 출력을 On/Off 하거나 출력 상태를 읽어 온다.
        /// </summary>
        public bool SetING
        {
            get
            {
                if (mControl.GetConfig.UseSmartIO == true)
                    return OutputCheck(IO_OUT2.TEST_ING);
                else return OutputCheck(IO_OUT.TEST_ING);
            }
            set
            {
                if (mControl.GetConfig.UseSmartIO == true)
                    outportb(IO_OUT2.TEST_ING, value);
                else outportb(IO_OUT.TEST_ING, value);
            }
        }

        public bool SetRHECU
        {
            get
            {
                if (mControl.GetConfig.UseSmartIO == true)
                    return OutputCheck(IO_OUT2.RH_ECU);
                else return OutputCheck(IO_OUT.RH_ECU);
            }
            set
            {
                if (mControl.GetConfig.UseSmartIO == true)
                    outportb(IO_OUT2.RH_ECU, value);
                else outportb(IO_OUT.RH_ECU, value);
            }
        }


        /// <summary>
        /// Yellow Lamp 출력을 On/Off 하거나 출력 상태를 읽어 온다.
        /// </summary>
        public bool SetYellowLamp
        {
            get
            {
                if (mControl.GetConfig.UseSmartIO == true)
                    return OutputCheck(IO_OUT2.YELLOW_LAMP);
                else return OutputCheck(IO_OUT.YELLOW_LAMP);
            }
            set
            {
                if (mControl.GetConfig.UseSmartIO == true)
                    outportb(IO_OUT2.YELLOW_LAMP, value);
                else outportb(IO_OUT.YELLOW_LAMP, value);
            }
        }

        /// <summary>
        /// Green Lamp 출력을 On/Off 하거나 출력 상태를 읽어 온다.
        /// </summary>
        public bool SetGreenLamp
        {
            get
            {
                if (mControl.GetConfig.UseSmartIO == true)
                    return OutputCheck(IO_OUT2.GREEN_LAMP);
                else return OutputCheck(IO_OUT.GREEN_LAMP);
            }
            set
            {
                if (mControl.GetConfig.UseSmartIO == true)
                    outportb(IO_OUT2.GREEN_LAMP, value);
                else outportb(IO_OUT.GREEN_LAMP, value);
            }
        }
        /// <summary>
        /// Red Lamp 출력을 On/Off 하거나 출력 상태를 읽어 온다.
        /// </summary>
        public bool SetRedLamp
        {
            get
            {
                if (mControl.GetConfig.UseSmartIO == true)
                    return OutputCheck(IO_OUT2.RED_LAMP);
                else return OutputCheck(IO_OUT.RED_LAMP);
            }
            set
            {
                if (mControl.GetConfig.UseSmartIO == true) 
                    outportb(IO_OUT2.RED_LAMP, value);
                else outportb(IO_OUT.RED_LAMP, value);
            }
        }

        /// <summary>
        /// Buzzer 출력을 On/Off 하거나 출력 상태를 읽어 온다.
        /// </summary>
        public bool SetBuzzer
        {
            get
            {
                if (mControl.GetConfig.UseSmartIO == true)
                    return OutputCheck(IO_OUT2.BUZZER);
                else return OutputCheck(IO_OUT.BUZZER);
            }
            set
            {
                if (mControl.GetConfig.UseSmartIO == true) 
                    outportb(IO_OUT2.BUZZER, value);
                else outportb(IO_OUT.BUZZER, value);
            }
        }


        /// <summary>
        /// TEST OK 신호를 On / OFF 한다.
        /// </summary>
        public bool SetTestOk
        {
            get
            {
                if (mControl.GetConfig.UseSmartIO == true)
                    return OutputCheck(IO_OUT2.TEST_OK);
                else return OutputCheck(IO_OUT.TEST_OK);
            }
            set
            {
                if (mControl.GetConfig.UseSmartIO == true)
                    outportb(IO_OUT2.TEST_OK, value);
                else outportb(IO_OUT.TEST_OK, value);
            }
        }

        /// <summary>
        /// 베터리 신호를 On / OFF 한다.
        /// </summary>
        public bool SetBatt
        {
            get
            {
                if (mControl.GetConfig.UseSmartIO == true)
                    return OutputCheck(IO_OUT.BATT);
                else return OutputCheck(IO_OUT2.BATT);
            }
            set
            {
                if (mControl.GetConfig.UseSmartIO == true) 
                    outportb(IO_OUT2.BATT, value);
                else outportb(IO_OUT.BATT, value);
            }
        }



        private ushort Smart_DIO32_in = 0x00;
        private ushort Smart_DIO32_out_rx = 0x00;
        private bool smart_io_rx_flg = false;
        private long smart_io_rx_time = 0;

        //private void ReadSmartIOData(object sender, EventArgs e)
        private void ReadSmartIOData()
        {
            //double temp_data;

            ushort crc16;

            byte crc16_high, crc16_low;

            try
            {
                mControl.공용함수.timedelay(20);

                //string serial_buff =  serialPort4. .ReadExisting();

                int iRecSize = SmartIO.BytesToRead; // 수신된 데이터 갯수

                if (iRecSize == 0)
                {
                    return;
                }

                smart_io_rx_time = 0;

                //string strRxData;

                byte[] buff1 = new byte[iRecSize];
                byte[] buff2 = new byte[iRecSize];
                SmartIO.Read(buff1, 0, iRecSize);
                //this.Text = "CW-DIO32 RX: " + BitConverter.ToString(buff1);

                if (buff1[0] == 0x7f)
                {
                    if ((buff1[11] == 0x01) && (buff1[12] == 0x0f))
                    {
                        Array.Copy(buff1, 11, buff2, 0, iRecSize - 11);
                        iRecSize -= 11;
                    }
                    else if ((buff1[10] == 0x01) && (buff1[11] == 0x0f))
                    {
                        Array.Copy(buff1, 10, buff2, 0, iRecSize - 10);
                        iRecSize -= 10;
                    }
                    else
                    {
                        Array.Copy(buff1, 8, buff2, 0, iRecSize - 8);
                        iRecSize -= 8;
                    }
                }
                else
                {
                    Array.Copy(buff1, 0, buff2, 0, iRecSize);
                }

                crc16 = CRC16_R(buff2, iRecSize - 2);

                crc16_high = (byte)((crc16 >> 0) & 0x00ff);
                crc16_low = (byte)((crc16 >> 8) & 0x00ff);

                if (crc16_high == buff2[iRecSize - 2] && crc16_low == buff2[iRecSize - 1])
                {
                    switch (buff2[1])
                    {
                        case 1:
                            Smart_DIO32_out_rx = buff2[4];
                            Smart_DIO32_out_rx |= (ushort)(buff2[3] << 8);
                            break;
                        case 4:
                            Smart_DIO32_in = buff2[4];
                            Smart_DIO32_in |= (ushort)(buff2[3] << 8);
                            InData[0] = buff2[4];
                            InData[1] = buff2[3];
                            //Smart_DIO32_in = (ushort)~Smart_DIO32_in;
                            smart_io_rx_flg = true;
                            break;
                    }

                }
            }
            catch
            {

            }
            finally
            {
                //smart_io_rx_flg = true;
            }
            return;
        }

        public const UInt16 POLYNORMIAL = 0xA001;
        public ushort CRC16(byte[] bytes, int usDataLen)
        {
            ushort crc = 0xffff, flag, ct = 0;

            try
            {
                while (usDataLen != 0)
                {
                    crc ^= bytes[ct];
                    for (int i = 0; i < 8; i++)
                    {
                        flag = 0;
                        if ((crc & 1) == 1) flag = 1;
                        crc >>= 1;
                        if (flag == 1) crc ^= POLYNORMIAL;
                    }
                    ct++;
                    usDataLen--;
                }
            }
            catch
            {
                crc = 0x00;
            }
            finally
            {

            }
            return crc;
        }

        public ushort CRC16_R(byte[] bytes, int usDataLen)
        {
            ushort crc = 0xffff, flag, ct = 0;

            try
            {
                while (usDataLen != 0)
                {
                    crc ^= bytes[ct];
                    for (int i = 0; i < 8; i++)
                    {
                        flag = 0;
                        if ((crc & 1) == 1) flag = 1;
                        crc >>= 1;
                        if (flag == 1) crc ^= POLYNORMIAL;
                    }
                    ct++;
                    usDataLen--;
                }
            }
            catch
            {
                crc = 0x00;
            }
            finally
            {

            }
            return crc;
        }

        ~IOControl()
        {
        }
    }
}