using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime .InteropServices ;

namespace PIO821_Ns
{
    public class PIO821
    {
        public const uint PIO821_NoError = 0x00;
        public const uint PIO821_ActiveBoardError = 0x01;
        public const uint PIO821_ExceedFindBoards = 0x02;
        public const uint PIO821_DriverNoOpen = 0x03;
        public const uint PIO821_BoardNoActive = 0x04;
        public const uint PIO821_WriteEEPROMError = 0x05;
        public const uint PIO821_ModeDAError = 0x06;
        public const uint PIO821_DAError = 0x07;
        public const uint PIO821_ConfigError = 0x08;
        public const uint PIO821_TimeoutError = 0x09;
        public const uint PIO821_AdChannelError = 0x0A;
        public const uint PIO821_AdPollingTimeOut = 0x0B;
        public const uint PIO821_AdPacerTimeOut = 0x0C;
        public const uint PIO821_CounterModeError = 0x0D;
        public const uint PIO821_InterruptError = 0x0E;

        [DllImport("pio821.dll")]public static extern ushort  PIO821_GetDllVersion();
        [DllImport("pio821.dll")]public static extern ushort  PIO821_ActiveBoard(byte BoardNo);
        [DllImport("pio821.dll")]public static extern ushort  PIO821_CloseBoard(byte BoardNo);
        [DllImport("pio821.dll")]public static extern ushort  PIO821_TotalBoard();
        [DllImport("pio821.dll")]public static extern ushort  PIO821_GetCardInf(byte BoardNo, out uint dwVID,out uint dwDID,out uint dwSDID,out uint dwSubV,out uint dwSubAux,out uint dwIRQ);
        [DllImport("pio821.dll")]public static extern byte  PIO821_IsBoardActive(byte BoardNo);
        [DllImport("pio821.dll")]public static extern ushort  PIO821_DA_Hex(byte BoardNo,ushort wValue);
        [DllImport("pio821.dll")]public static extern ushort  PIO821_DA(byte BoardNo,byte Mode,float fValue);
        [DllImport("pio821.dll")]public static extern ushort  PIO821_ReadEEP(byte BoardNo,out ushort wValue);
        [DllImport("pio821.dll")]public static extern ushort  PIO821_WriteEEP(byte BoardNo,out ushort wValue);
        [DllImport("pio821.dll")]public static extern void PIO821_OutputByte(byte BoardNo,uint dwOffset,byte bValue);
        [DllImport("pio821.dll")]public static extern byte  PIO821_InputByte(byte BoardNo,uint dwOffset);
        [DllImport("pio821.dll")]public static extern void PIO821_OutputWord(byte BoardNo,uint dwOffset,ushort wValue);
        [DllImport("pio821.dll")]public static extern ushort  PIO821_InputWord(byte BoardNo,uint dwOffset);
        [DllImport("pio821.dll")]public static extern ushort  PIO821_DigitalIn(byte BoardNo,out ushort wValue);
        [DllImport("pio821.dll")]public static extern ushort  PIO821_DigitalOut(byte BoardNo, ushort wValue);
        [DllImport("pio821.dll")]public static extern ushort  PIO821_SetChannelConfig(byte BoardNo, ushort wAdChannel, ushort wConfig);
        [DllImport("pio821.dll")]public static extern ushort  PIO821_Delay(byte BoardNo,ushort wDownCount);
        [DllImport("pio821.dll")]public static extern ushort  PIO821_ADPollingHex(byte BoardNo,out ushort wAdVal);
        [DllImport("pio821.dll")]public static extern ushort  PIO821_ADPolling(byte BoardNo,out float fAdVal);
        [DllImport("pio821.dll")]public static extern ushort  PIO821_ADsPolling(byte BoardNo,out float fAdVal, uint dwNum);
        [DllImport("pio821.dll")]public static extern ushort  PIO821_ADsPacer(byte BoardNo,out float fAdVal, uint dwNum, ushort wSamplingDiv);
        [DllImport("pio821.dll")]public static extern ushort  PIO821_InstallIrq(byte BoardNo);
        [DllImport("pio821.dll")]public static extern void PIO821_RemoveIrq(byte BoardNo);
        [DllImport("pio821.dll")]public static extern ushort  PIO821_SetCounter( byte BoardNo, ushort wCounterNo,ushort bCounterMode, uint wCounterValue);
        [DllImport("pio821.dll")]public static extern uint  PIO821_ReadCounter(byte BoardNo, ushort wCounterNo, ushort bCounterMode);
        [DllImport("pio821.dll")]public static extern ushort  PIO821_IntADStart(byte BoardNo,ushort wNum, ushort wSamplingDiv);
        [DllImport("pio821.dll")]public static extern ushort  PIO821_GetADsfloat(out float fAdVal);
        [DllImport("pio821.dll")]public static extern ushort  PIO821_GetADsHex(out ushort HAdVal);
    }
}