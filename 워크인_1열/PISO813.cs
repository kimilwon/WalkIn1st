using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;



 

namespace PISO_Ns

{
   
    
    public class PISO813

    {
         public const uint ActiveHigh =1;
         public const uint  ActiveLow=0;
        //Define PISO813 relative Address'

        public const uint AD_LO = 0xD0;
        public const uint AD_HI = 0xD4;
        public const uint SET_CH = 0xE0;
        public const uint SET_GAIN = 0xE4;
        public const uint SOFT_TRIG = 0xF0;

        //Define the gain mode'

        public const uint BI_1 = 0x0;
        public const uint BI_2 = 0x1;
        public const uint BI_4 = 0x2;
        public const uint BI_8 = 0x3;
        public const uint BI_16 = 0x4;

        //Return Code'

        public const uint NoError = 0;
        public const uint DriverOpenError = 1;
        public const uint DriverNoOpen = 2;
        public const uint GetDriverVesionError = 3;
        public const uint CallDriverError = 4;
        public const uint FindBoardError = 5;
        public const uint ExceedBoardNumber = 6;
        public const uint TimeOutError = 0xFFFF;
        public const double AdError2 = -100.0;
    
         //ID

        public const uint PISO_813 = 0x800A00;


       //**************
       // PISO Driver
       //**************
       [DllImport("Piso813.dll",EntryPoint="PISO813_DriverInit")]
        public static extern ushort DriverInit();
        
        [DllImport("Piso813.dll",EntryPoint="PISO813_DriverClose")]
        public static extern void DriverClose();
        [DllImport("Piso813.dll",EntryPoint="PISO813_SearchCard")]
        public static extern ushort SearchCard(out ushort wBoards, uint dwPIOCardID);
        [DllImport("Piso813.dll",EntryPoint="PISO813_GetConfigAddressSpace")]
        public static extern ushort GetConfigAddressSpace(
            ushort wBoardNo, out uint wAddrBase, out ushort wIrqNo,
            out ushort wSubVendor, out ushort wSubDevice, out ushort wSubAux,
            out ushort wSlotBus, out ushort wSlotDevice);
        [DllImport("Piso813.dll", EntryPoint = "PISO813_GetDriverVersion")]
        public static extern ushort GetDriverVersion(out short wDriverVersion);

        // ******************************************
        [DllImport("Piso813.dll",EntryPoint="PISO813_OutputByte")]
        public static extern void OutputByte(ushort wBaseAddr, ushort bOutputValue);
        [DllImport("Piso813.dll",EntryPoint="PISO813_InputByte")]
        public static extern ushort InputByte(ushort wBaseAddr);

        //********************
        //PISO Interrupt
        //********************
        
        [DllImport("Piso813.dll", EntryPoint = "PISO813_IntInstall")]
        public static extern ushort IntInstall(ushort wboards, out uint hEvent, ushort wInterruptSource, ushort wActiveMode);
        [DllImport("Piso813.dll", EntryPoint = "PISO813_IntRemove")]
        public static extern ushort IntRemove();


        [DllImport("Piso813.dll", EntryPoint = "PISO813_IntGetCount")]
        public static extern ushort IntGetCount(out uint intIntCount);

        [DllImport("Piso813.dll", EntryPoint = "PISO813_IntResetCount")]
        public static extern ushort IntResetCount();
        
        //********************
        //PISO Test
        //********************
        [DllImport("Piso813.dll",EntryPoint = "PISO813_ShortSub")] 
        public static extern ushort ShortSub(ushort a, ushort b); 

        [DllImport("Piso813.dll",EntryPoint ="PISO813_FloatSub")] 
        public static extern float FloatSub(float a, float b); 
 

        [DllImport("PISO813.dll")]
        public static extern ushort getDllVersion(); 
 


        //**********************
        //AD Function
        //**********************
        [DllImport("Piso813.dll", EntryPoint = "PISO813_SetChGain")]
        public static extern ushort SetChGain(uint wAddrBase, ushort wChannel, ushort wGainCode);
        
        [DllImport("Piso813.dll", EntryPoint = "PISO813_AD_Hex")]
        public static extern ushort AD_Hex(uint wAddrBase);
        
        [DllImport("Piso813.dll", EntryPoint = "PISO813_ADs_Hex")]
        public static extern uint ADs_Hex (uint wAddrBase, out ushort wBuffer, uint dwDataNo);
        
        [DllImport("Piso813.dll", EntryPoint = "PISO813_AD_Float")]
        public static extern float AD_Float(uint wAddrBase, ushort wJump10v, ushort wBipolar);
        [DllImport("Piso813.dll", EntryPoint = "PISO813_ADs_Float")]
        public static extern float ADs_Float(uint wAddrBase, ushort wJump10v, ushort wBipolar, out float fBuffer, uint dwDataNo);
        [DllImport("Piso813.dll", EntryPoint = "PISO813_AD2F")]
        public static extern float AD2F(ushort whex, ushort wGainCode, ushort wJump20v, ushort uwBipolar);


        // ******************************************
        private int DriverOpened = 0;

        // ******************************************
        //public void OutputByte(ushort wBaseAddr, ushort bValue)
        //{
         //   OutputByte(wBaseAddr, bValue);
        //}
        //public ushort InputByte(ushort wBaseAddr)
        //{
         //   return InputByte(wBaseAddr);
        //}


        public PISO813()//constroctor
        {
            DriverOpened = 0;
        }
        ~PISO813()
        {
            if (DriverOpened != 0)
            {
                DriverOpened = 0;
                DriverClose();
            }
        }
    }
}
