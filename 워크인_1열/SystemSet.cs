using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace 워크인_1열
{
    public partial class SystemSet : Form
    {
        MyInterface mControl = null;
        public SystemSet()
        {
            InitializeComponent();
        }
        public SystemSet(MyInterface mControl)
        {
            InitializeComponent();
            this.mControl = mControl;            
        }


        private void SystemSet_Load(object sender, EventArgs e)
        {
            axComboBox1.Clear();
            axComboBox2.Clear();
            axComboBox3.Clear();
            axComboBox4.Clear();

            for (int i = 0; i < 20; i++)
            {
                axComboBox5.AddItem((i + 1).ToString());
                axComboBox13.AddItem((i + 1).ToString());
            }

           

            axComboBox1.AddItem("2400");
            axComboBox1.AddItem("4800");
            axComboBox1.AddItem("9600");
            axComboBox1.AddItem("11400");
            axComboBox1.AddItem("19200");
            axComboBox1.AddItem("38400");
            axComboBox1.AddItem("57600");
            axComboBox1.AddItem("115200");

            axComboBox3.AddItem("2400");
            axComboBox3.AddItem("4800");
            axComboBox3.AddItem("9600");
            axComboBox3.AddItem("11400");
            axComboBox3.AddItem("19200");
            axComboBox3.AddItem("38400");
            axComboBox3.AddItem("57600");
            axComboBox3.AddItem("115200");


            string[] PortName = mControl.공용함수.GetComName(SerialPort.GetPortNames());
            
            foreach (string pName in PortName)
            {
                //axComboBox12.AddItem(pName);
                axComboBox2.AddItem(pName);
                axComboBox4.AddItem(pName);
            }

           
            if (mControl.GetConfig.PanelMeterToCurr.Port != "")
            {
                if (PortName.Contains(mControl.GetConfig.PanelMeterToCurr.Port) == true)
                {
                    int Pos = axComboBox2.FindItem(0, mControl.GetConfig.PanelMeterToCurr.Port, true);
                    axComboBox2.ListIndex = Pos;
                }
                else
                {
                    axComboBox2.ListIndex = -1;
                }
            }
            else
            {
                axComboBox2.ListIndex = -1;
            }

            if ((0 <= mControl.GetConfig.PanelMeterToCurr.Speed) && (0 < axComboBox1.ListCount) && (mControl.GetConfig.PanelMeterToCurr.Speed < axComboBox1.ListCount)) axComboBox1.ListIndex = mControl.GetConfig.PanelMeterToCurr.Speed;

            if (mControl.GetConfig.IOCom.Port != "")
            {
                if (PortName.Contains(mControl.GetConfig.IOCom.Port) == true)
                {
                    int Pos = axComboBox4.FindItem(0, mControl.GetConfig.IOCom.Port, true);
                    axComboBox4.ListIndex = Pos;
                }
                else
                {
                    axComboBox4.ListIndex = -1;
                }
            }
            else
            {
                axComboBox4.ListIndex = -1;
            }
            if ((0 <= mControl.GetConfig.IOCom.Speed) && (0 < axComboBox1.ListCount) && (mControl.GetConfig.IOCom.Speed < axComboBox1.ListCount)) axComboBox3.ListIndex = mControl.GetConfig.IOCom.Speed;


            if (0 < mControl.GetConfig.Batt_ID)
                axComboBox5.ListIndex = mControl.GetConfig.Batt_ID - 1;
            else axComboBox5.ListIndex = -1;
                        
            if (0 < mControl.GetConfig.PSEAT_ID)
                axComboBox13.ListIndex = mControl.GetConfig.PSEAT_ID - 1;
            else axComboBox13.ListIndex = -1;

            textBox1.Text = mControl.GetConfig.Client.IP;
            textBox2.Text = mControl.GetConfig.Client.Port.ToString();

            textBox3.Text = mControl.GetConfig.Server.IP;
            textBox7.Text = mControl.GetConfig.Server.Port.ToString();
            checkBox1.Checked = mControl.GetConfig.UseSmartIO;

            return;
        }

        private void MoveSpec()
        {
            __Config__ Config = mControl.GetConfig;

            if (0 <= axComboBox2.ListIndex) Config.PanelMeterToCurr.Port = axComboBox2.Text;
            if (0 <= axComboBox1.ListIndex) Config.PanelMeterToCurr.Speed = axComboBox1.ListIndex;

            if (0 <= axComboBox4.ListIndex) Config.IOCom.Port = axComboBox4.Text;
            if (0 <= axComboBox3.ListIndex) Config.IOCom.Speed = axComboBox3.ListIndex;


            if (0 <= axComboBox5.ListIndex) Config.Batt_ID = axComboBox5.ListIndex + 1;
            if (0 <= axComboBox13.ListIndex) Config.PSEAT_ID = axComboBox13.ListIndex + 1;

            Config.Client.IP = textBox1.Text;
            if (int.TryParse(textBox2.Text, out Config.Client.Port) == false) Config.Client.Port = 0;

            Config.Server.IP = textBox3.Text;
            if (int.TryParse(textBox7.Text, out Config.Server.Port) == false) Config.Server.Port = 0;

            Config.UseSmartIO = checkBox1.Checked;
            mControl.GetConfig = Config;
            return;
        }


        private void ImageButton1_Click(object sender, EventArgs e)
        {
            MoveSpec();
            ConfigSetting GetConfig = new ConfigSetting();
            GetConfig.ReadWriteConfig = mControl.GetConfig;
            return;
        }

        private void ImageButton2_Click(object sender, EventArgs e)
        {
            this.Close();
            return;
        }
    }
}
