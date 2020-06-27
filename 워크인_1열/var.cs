

using System;
using System.Drawing;
using System.Runtime.InteropServices;
using MES;

namespace 워크인_1열
{
    [StructLayout(LayoutKind.Explicit)]
    public struct union_r
    {
        [FieldOffset(0)]
        public int i;
        [FieldOffset(0)]
        public byte c1;
        [FieldOffset(1)]
        public byte c2;
        [FieldOffset(2)]
        public byte c3;
        [FieldOffset(3)]
        public byte c4;
    };

    //public struct __Calibrat__
    //{
    //    public float A;
    //    public float B;
    //    public float LowSource;
    //    public float HighSource;
    //    public float LowUserData;
    //    public float HighUserData;
    //}
    //public struct PinMapItem
    //{
    //    public int Batt;
    //    public int Gnd;
    //}

    //public struct PinMapStruct
    //{
    //    public PinMapItem IMSSet;
    //    public PinMapItem IMSM1;
    //    public PinMapItem IMSM2;
    //    public PinMapItem SlideFWD;
    //    public PinMapItem SlideBWD;
    //    public PinMapItem HeightUp;
    //    public PinMapItem HeightDn;
    //    public PinMapItem TiltUp;
    //    public PinMapItem TiltDn;
    //}

  

    public enum MENU
    {
        NONE,
        AGING_TESTING,
        PERFORMANCE_TESTING,
        AGING_SETTING,
        PERFORMANCE_SETTING,
        OPTION,
        LIN1,
        LIN2,
        CAN,
        PASSWORD,
        AGING_DATAVIEW,
        PERFORMANCE_DATAVIEW,
        SELF
    }


    public class RESULT
    {
        public const short READY = 0;
        public const short PASS = 1;
        public const short NG = 2;
        public const short REJECT = 2;
        public const short END = 3;
        public const short STOP = 4;
        public const short CLEAR = 5;
        public const short TEST = 6;
        public const short NOT_TEST = 7;
        public const short TEST_STANDBY = 8;
    };


    public class IO_IN
    {
        public const short PASS_SW = 1;
        public const short RESET_SW = 2;
        public const short LH_SELECT = 3;
        public const short PRODUCT_IN = 5;
        public const short JIG_UP = 4;
        public const short AUTO_SW = 10;
        public const short OPTION_WALKIN = 11;
        public const short USB = 15;
    }

    public class IO_OUT
    {
        public const short BUZZER = 0;
        public const short TEST_OK = 1;
        //public const short IGN1 = 2;
        public const short PRODUCT_IN = 4;
        public const short TEST_ING = 2;
        public const short YELLOW_LAMP = 5;
        public const short RED_LAMP = 7;
        public const short GREEN_LAMP = 6;
        public const short RH_ECU = 9;
        public const short BATT = 13;
    }

    public class IO_IN2
    {
        public const short PASS_SW = 0;
        public const short RESET_SW = 1;
        public const short AUTO_SW = 2;
        public const short LH_SELECT = 3;
        public const short JIG_UP = 4;
        public const short PRODUCT_IN = 5;
        //public const short OPTION_WALKIN = 11;
        //public const short USB = 15;
    }

    public class IO_OUT2
    {
        public const short BATT = 0;
        public const short PRODUCT_IN = 1;
        public const short TEST_OK = 2;
        public const short TEST_ING = 3;
        //Spare 4
        public const short BUZZER = 5;
        public const short YELLOW_LAMP = 6;
        public const short GREEN_LAMP = 7;
        public const short RED_LAMP = 8;
        public const short RH_ECU = 9;
        
    }


    public struct __CheckItem__
    {
        public bool Slide;
        public bool Recline;
        public bool Usb;
        public bool LHRH;
        public bool LHDRHD;
    }



    [Serializable, StructLayout(LayoutKind.Sequential, Pack = 1, CharSet = CharSet.Unicode)]//CharSet = CharSet.Unicode를 선언해 주지 않으면 한글 처리할 때 파일에 저장하거나 할 경우 에러가 발생한다.
    public struct __MinMax__
    {
        public double Min;
        public double Max;
    }

    public struct __MinMaxToInt__
    {
        public int Min;
        public int Max;
    }

    public struct __Port__
    {
        public string Port;
        public int Speed;
    }


    public struct __LinDevice__
    {
        public short Device;
        public int Speed;
    }
    public struct __CanDevice__
    {
        public short Device;
        public short Channel;
        public short ID;
        public int Speed;
    }

    public struct __TcpIP__
    {
        public string IP;
        public int Port;
    }

    [Serializable, StructLayout(LayoutKind.Sequential, Pack = 1, CharSet = CharSet.Unicode)]//CharSet = CharSet.Unicode를 선언해 주지 않으면 한글 처리할 때 파일에 저장하거나 할 경우 에러가 발생한다.
    public struct __Config__
    {
        public __Port__ PanelMeterToCurr;        
        public __TCPIP__ Client;
        public __TCPIP__ Server;
    
        public int Batt_ID;        
        public int PSEAT_ID;
        public __Port__ IOCom;
        public bool UseSmartIO;
    }

    public struct __Time__
    {
        public int Hour;
        public short Min;
        public short Sec;
        public short mSec;
    }
    
    
    public struct __IOData__
    {
        public short Card;
        public short Pos;
        public byte Data;
    }


    public struct __TestDataItem__
    {
        public short Result;
        public bool Test;
        public float Data;
    }

    [Serializable, StructLayout(LayoutKind.Sequential, Pack = 1, CharSet = CharSet.Unicode)]//CharSet = CharSet.Unicode를 선언해 주지 않으면 한글 처리할 때 파일에 저장하거나 할 경우 에러가 발생한다.
    public struct __Data__
    {
        public short Result;
        public string Time;

        public __TestDataItem__ Slide;
        public __TestDataItem__ Recline;
        public __TestDataItem__ Usb;
    }

    public struct __Infor__
    {
        public int OkCount;
        public int NgCount;
        public int TotalCount;
        public int Day;
        public string Date;
        public string DataName;
    }
    

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct __Spec__
    {
        public string CarName;
        public float SlideSpec; //Min
        public float ReclineSpec; //Min
        public float OffCurrent;
        public float UsbCurrent;
        public float DelayTime;
        public float TestTime;
    }
}