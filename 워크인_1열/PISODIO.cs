using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;

namespace PISODIO_Ns
{
    public class PISODIO
    { 
        // return code
        public const uint NoError                    = 0;
        public const uint DriverOpenError            = 1;
        public const uint DriverNoOpen               = 2;
        public const uint GetDriverVersionError      = 3;
        public const uint InstallIrqError            = 4;
        public const uint ClearIntCountError         = 5;
        public const uint GetIntCountError           = 6;
        public const uint RegisterApcError           = 7;
        public const uint RemoveIrqError             = 8;
        public const uint FindBoardError             = 9;
        public const uint ExceedBoardNumber          = 10;
        public const uint ResetError                 = 11;

        // to trigger a interrupt when high -> low
        public const ushort ActiveLow                  =0;
        // to trigger a interrupt when low -> high
        public const ushort ActiveHigh                 =1;

        // ID
        public const uint PISO_P16R16U                  =0x18000000;    // for PISO-P16R16U
        public const uint PISO_C64                      =  0x800800;    // for PISO-C64
        public const uint PISO_P64                      =  0x800810;    // for PISO-P64
        public const uint PISO_A64                      =  0x800850;    // for PISO-A64
        public const uint PISO_P32C32                   =  0x800820;    // for PISO-P32C32 
        public const uint PISO_P32A32                   =  0x800870;    // for PISO-P32A32 
        public const uint PISO_P8R8                     =  0x800830;    // for PISO-P8R8
        public const uint PISO_P8SSR8AC                 =  0x800830;    // for PISO-P8SSR8AC
        public const uint PISO_P8SSR8DC                 =  0x800830;    // for PISO-P8SSR8DC
        public const uint PISO_730                      =  0x800840;    // for PISO-730
        public const uint PISO_730A                     =  0x800880;    // for PISO-730A

        // Test functions
        [DllImport("PISODIO.dll", EntryPoint = "PISODIO_FloatSub")]
        public static extern float FloatSub(float fA, float fB);
        [DllImport("PISODIO.dll", EntryPoint = "PISODIO_ShortSub")]
        public static extern short ShortSub(short nA, short nB);
        [DllImport("PISODIO.dll", EntryPoint = "PISODIO_GetDllVersion")]
        public static extern ushort GetDllVersion();

        // Driver functions
        [DllImport("PISODIO.dll", EntryPoint = "PISODIO_DriverInit")]
        public static extern ushort DriverInit();
        [DllImport("PISODIO.dll", EntryPoint = "PISODIO_DriverClose")]
        public static extern void DriverClose();
        [DllImport("PISODIO.dll",EntryPoint="PISODIO_SearchCard")]
        public static extern ushort  SearchCard(out ushort wBoards, uint dwPIOCardID);
        [DllImport("PISODIO.dll", EntryPoint = "PISODIO_GetDriverVersion")]
        public static extern ushort GetDriverVersion(out ushort wDriverVersion);
        [DllImport("PISODIO.dll", EntryPoint = "PISODIO_GetConfigAddressSpace")]
        public static extern ushort GetConfigAddressSpace(ushort wBoardNo, out uint wAddrBase, out ushort wIrqNo,
                                                                                             out ushort wSubVendor, out ushort wSubDevice, out ushort wSubAux,
                                                                                             out ushort wSlotBus, out ushort wSlotDevice);
        [DllImport("PISODIO.dll", EntryPoint = "PISODIO_ActiveBoard")]
        public static extern ushort ActiveBoard(ushort wBoardNo);
        [DllImport("PISODIO.dll", EntryPoint = "PISODIO_WhichBoardActive")]
        public static extern ushort WhichBoardActive();

        // DIO functions
        [DllImport("PISODIO.dll", EntryPoint = "PISODIO_OutputWord")]
        public static extern void OutputWord(uint wPortAddress, uint wOutData);
        [DllImport("PISODIO.dll", EntryPoint = "PISODIO_OutputByte")]
        public static extern void OutputByte(uint wPortAddr, ushort bOutputValue);
        [DllImport("PISODIO.dll", EntryPoint = "PISODIO_InputWord")]
        public static extern uint InputWord(uint wPortAddress);
        [DllImport("PISODIO.dll", EntryPoint = "PISODIO_InputByte")]
        public static extern ushort InputByte(uint wPortAddr);  

        // Interrupt functions
        [DllImport("PISODIO.dll", EntryPoint = "PISODIO_IntInstall")]
        public static extern ushort IntInstall(ushort wBoardNo, out uint hEvent, ushort wInterruptSource, ushort wActiveMode);
        [DllImport("PISODIO.dll", EntryPoint = "PISODIO_IntRemove")]
        public static extern ushort IntRemove();
        [DllImport("PISODIO.dll", EntryPoint = "PISODIO_IntResetCount")]
        public static extern ushort IntResetCount();
        [DllImport("PISODIO.dll", EntryPoint = "PISODIO_IntGetCount")]
        public static extern ushort IntGetCount(out uint dwIntCount);


    }
}
