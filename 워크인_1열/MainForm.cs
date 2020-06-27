#define PROGRAM_RUNNING

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using MES;
using NationalInstruments.Restricted;

namespace 워크인_1열
{
    public interface MyInterface
    {
        bool isRunning { get; set; }
        bool isExit { get; }

        /// <summary>
        /// 공용 함수를 호출한다.
        /// </summary>
        COMMON_FUCTION 공용함수 { get; }
        /// <summary>
        /// 환경 변수 설정을 진해안다.
        /// </summary>
        __Config__ GetConfig { get; set; }
        
        PanelMeter GetPanelMeter { get; }
        
        __Spec__ GemSpec { get; set; }
    }
    public partial class MainForm : Form, MyInterface
    {
        private bool ExitFlag { get; set; }
        private bool RunningFlag { get; set; }

        private COMMON_FUCTION ComF = new COMMON_FUCTION();
        private __Config__ Config;
        private __Spec__ mSpec = new __Spec__();
        private PanelMeter pMeter = null;
        private __CheckItem__ CheckItem = new __CheckItem__();
        private __Data__ TData;
        private __Infor__ Infor = new __Infor__();
        
        private MES_Control MESCtrl = null;//= new MES_Control();
        private IOControl IOPort = null;
        private bool ResetButtonOn { get; set; }
        private bool StartButtonOn { get; set; }
        private bool OldPopDataCheckFlag { get; set; }
        private bool PopDataCheckFlag { get; set; }

        private Color SelectOnColor;
        private Color SelectOffColor;
        public MainForm()
        {
            InitializeComponent();
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            try
            {
                ConfigSetting GetConfig = new ConfigSetting();
                Config = GetConfig.ReadWriteConfig;
                CheckForIllegalCrossThreadCalls = false;

                led2.Indicator.Text = "";
                label2.Text = DateTime.Now.ToLongDateString() + " " + DateTime.Now.ToLongTimeString();
                테스트ToolStripMenuItem_Click(toolStripButton1, new EventArgs());

                ComF.ReadFileListNotExt(Program.SPEC_PATH.ToString(), "*.Spc", COMMON_FUCTION.FileSortMode.FILENAME_ODERBY);
                List<string> FList = ComF.GetFileList;
                ModelChangeFlag = true;
                comboBox1.Items.Clear();

                if (0 < FList.Count)
                {
                    foreach (string s in FList)
                    {
                        comboBox1.Items.Add(s);
                    }
                }

                ModelChangeFlag = false;
                if (0 < comboBox1.Items.Count)
                {
                    comboBox1.SelectedIndex = 0;
                }

                TCPOpen();
                SerialOpen();

                OpenInfor();

                sevenSegmentInteger1.Value.AsInteger = Infor.TotalCount;
                sevenSegmentInteger2.Value.AsInteger = Infor.OkCount;
                sevenSegmentInteger3.Value.AsInteger = Infor.NgCount;

                IOPort = new IOControl(this);

                if(Config.UseSmartIO == true)
                    IOPort.OpenIO(Config.IOCom.Port, (short)Config.IOCom.Speed);
                else IOPort.OpenIO();

                if (IOPort.isOpen == false)
                {
                    // MessageBox.Show(this,"I/O "
                }

                SelectOffColor = fpSpread1.ActiveSheet.Cells[0, 0].BackColor;
                SelectOnColor = Color.FromArgb(172, 227, 175);
            }
            catch { }
            finally { timer1.Enabled = true; timer2.Enabled = true; }
            return;
        }
        private void SerialOpen()
        {
            pMeter = new PanelMeter(this);
            pMeter.Open(Config.PanelMeterToCurr);
            if (pMeter.isOpen == false) MessageBox.Show("판넬메터 통신 포트 오픈 실패", "에러", MessageBoxButtons.OK, MessageBoxIcon.Error);
            return;
        }

        private void TCPOpen()
        {
            MESCtrl = new MES_Control(ClientIp: Config.Client, ServerIp: Config.Server, mControl: this);
            MESCtrl.Open();

            if (MESCtrl.isClientConnection == false)
            {
                // MessageBox.Show("서버와 접속되지 않습니다.", "에러", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            return;
        }

        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (RunningFlag == true) return;
            //DialogFlag = true;

            //panel1.Visible = false;
            ExitFlag = true;


            CloseForm CForm = new CloseForm()
            {
                WindowState = FormWindowState.Normal,
                StartPosition = FormStartPosition.CenterParent
            };

            CForm.FormClosing += delegate (object Sender1, FormClosingEventArgs e1)
            {
#if PROGRAM_RUNNING
                if (IOPort != null) IOPort.CloseIO();
#endif
                e.Cancel = false;

                //이게 없으면 실제 종료가 되지 않는다.
                //반드시 this.Dispose(); 함수를 호출 해야만 한다.
                this.Dispose();
            };
            CForm.StartPosition = FormStartPosition.CenterParent;
            CForm.Show();
            e.Cancel = true;
            //}
            //else
            //{
            //    e.Cancel = true;
            //    DialogFlag = false;
            //}
            return;
        }
        public PanelMeter GetPanelMeter
        {
            get { return pMeter; }
        }



        public COMMON_FUCTION 공용함수
        {
            get { return ComF; }
        }

        public __Config__ GetConfig
        {
            get { return Config; }
            set
            {
                Config = value;
                ConfigSetting ReadConfig = new ConfigSetting();
                ReadConfig.ReadWriteConfig = Config;
            }
        }
        
        public __Spec__ GemSpec
        {
            get { return mSpec; }
            set { mSpec = value; }
        }
        public bool isRunning
        {
            get { return RunningFlag; }
            set { RunningFlag = value; }
        }

        public bool isExit
        {
            get { return ExitFlag; }
        }


        private void 테스트ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //---------------------[Test] 
            toolStripButton1.Visible = true;
            //---------------------[설정] 
            toolStripButton2.Visible = false;
            toolStripButton3.Visible = false;
            //---------------------[로그인] 
            toolStripButton5.Visible = false;
            toolStripButton6.Visible = false;
            //---------------------[보기] 
            toolStripButton7.Visible = false;
            //---------------------[종료] 
            toolStripButton8.Visible = false;
            toolStripButton9.Visible = false;
            return;
        }

        private void 설정ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //---------------------[Test] 
            toolStripButton1.Visible = false;
            //---------------------[설정] 
            toolStripButton2.Visible = true;
            toolStripButton3.Visible = true;
            //---------------------[로그인] 
            toolStripButton5.Visible = false;
            toolStripButton6.Visible = false;
            //---------------------[보기] 
            toolStripButton7.Visible = false;
            //---------------------[종료] 
            toolStripButton8.Visible = false;
            toolStripButton9.Visible = false;
            return;
        }

        private void 로그인ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //---------------------[Test] 
            toolStripButton1.Visible = false;
            //---------------------[설정] 
            toolStripButton2.Visible = false;
            toolStripButton3.Visible = false;
            //---------------------[로그인] 
            toolStripButton5.Visible = true;
            toolStripButton6.Visible = true;
            //---------------------[보기] 
            toolStripButton7.Visible = false;
            //---------------------[종료] 
            toolStripButton8.Visible = false;
            toolStripButton9.Visible = false;
            return;
        }

        private void 보기ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //---------------------[Test] 
            toolStripButton1.Visible = false;
            //---------------------[설정] 
            toolStripButton2.Visible = false;
            toolStripButton3.Visible = false;
            //---------------------[로그인] 
            toolStripButton5.Visible = false;
            toolStripButton6.Visible = false;
            //---------------------[보기] 
            toolStripButton7.Visible = true;
            //---------------------[종료] 
            toolStripButton8.Visible = false;
            toolStripButton9.Visible = false;
            return;
        }

        private void 종료ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //---------------------[Test] 
            toolStripButton1.Visible = false;
            //---------------------[설정] 
            toolStripButton2.Visible = false;
            toolStripButton3.Visible = false;
            //---------------------[로그인] 
            toolStripButton5.Visible = false;
            toolStripButton6.Visible = false;
            //---------------------[보기] 
            toolStripButton7.Visible = false;
            //---------------------[종료] 
            toolStripButton8.Visible = true;
            toolStripButton9.Visible = true;
            return;
        }

        private void toolStripButton5_Click(object sender, EventArgs e)
        {
            //로그인
            if (toolStripButton5.Text == "로그인")
            {
                PasswordCheckForm pass = new PasswordCheckForm();
                //아래와 같이 해 주면 폼을 닫을때 Dialog로 오픈을 하지 않아도 {} 안을 실행하게 된다. 동시에 해당 폼의 FormClosing이 동시에 실행되므로 Dialog로 오픈한것 같은 효과를 얻는다.
                pass.FormClosing += delegate (object sender1, FormClosingEventArgs e1)
                {
                    if (pass.result == true)
                    {
                        toolStripButton5.Text = "로그아웃";
                        로그인ToolStripMenuItem.Text = "로그아웃";
                        toolStripButton5.Image = Properties.Resources.Pad_Unlock_36Pixel1;

                        toolStripButton2.Enabled = true;
                        toolStripButton3.Enabled = true;
                        toolStripButton6.Enabled = true;
                    }
                    pass.Dispose();
                    pass = null;
                };
                pass.Show();
            }
            else
            {
                toolStripButton5.Text = "로그인";
                로그인ToolStripMenuItem.Text = "로그인";
                toolStripButton5.Image = Properties.Resources.Pad_Lock;
                toolStripButton2.Enabled = false;
                toolStripButton3.Enabled = false;
                toolStripButton6.Enabled = false;
            }
            return;
        }

        private void toolStripButton6_Click(object sender, EventArgs e)
        {
            // 암호 설정
            if (toolStripButton5.Text == "로그인")
            {
                //MessageBox.Show("로그인을 먼저 진행하십시오.", "경고");
                //uMessageBox.Show(promptText: "로그인을 먼저 진행하십시오.", title: "경고");
                MessageBox.Show(this, "로그인을 먼저 진행하십시오.", "경고", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            PasswordSetForm set = new PasswordSetForm();
            set.MinimizeBox = false;
            set.MaximizeBox = false;
            set.FormBorderStyle = FormBorderStyle.FixedSingle;
            set.Show();
            return;
        }

        private void toolStripButton9_Click(object sender, EventArgs e)
        {
            this.Close();
            return;
        }

        private SystemSet SysSetForm = null;
        private SpecSetting SpecSetForm = null;

        private void OpenFormClose()
        {
            //if (SysSetForm != null) SysSetForm.Close();
            if (SpecSetForm != null) SpecSetForm.Close();

            return;
        }

        private void toolStripButton3_Click(object sender, EventArgs e)
        {
            if (toolStripButton5.Text == "로그인")
            {
                //MessageBox.Show("로그인을 먼저 진행하십시오.", "경고");
                //uMessageBox.Show(promptText: "로그인을 먼저 진행하십시오.", title: "경고");
                MessageBox.Show(this, "로그인을 먼저 진행하십시오.", "경고", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            //if (IOPort.GetAuto == false)
            {
                if (SysSetForm == null)
                {
                    //OpenFormClose();
                    //panel1.SendToBack();
                    //panel1.Visible = false;

                    //panel2.Visible = true;
                    //if (panel2.Parent != this) panel2.Parent = this;
                    //panel2.BringToFront();
                    SysSetForm = new SystemSet(this)
                    {
                        MaximizeBox = false,
                        MinimizeBox = false,
                        ControlBox = false,
                        ShowIcon = false,
                        StartPosition = FormStartPosition.CenterParent,
                        WindowState = FormWindowState.Normal,
                        TopMost = false,
                        TopLevel = true,
                        Owner = this,
                        FormBorderStyle = FormBorderStyle.FixedSingle,
                        //Location = new Point(0, 0),
                        //Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Bottom                        
                        Anchor = AnchorStyles.Top | AnchorStyles.Left
                    };

                    SysSetForm.FormClosing += delegate (object sender1, FormClosingEventArgs e1)
                    {
                        e1.Cancel = false;
                        SysSetForm.Parent = null;
                        SysSetForm.Dispose();
                        SysSetForm = null;
                    };

                    //SysSetForm.Parent = panel2;
                    SysSetForm.Show();
                }
            }
            return;
        }

       
        private bool ModelChangeFlag = false;
        private string Model = string.Empty;
        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (ModelChangeFlag == true) return;
            Name = Program.SPEC_PATH.ToString() + "\\" + comboBox1.SelectedItem.ToString() + ".spc";
            if (File.Exists(Name) == true)
            {
                mSpec = ComF.OpenSpec(TSpec: ref mSpec, Name: Name);
                DisplaySpec();
                Model = comboBox1.SelectedItem.ToString();
            }
            return;
        }

        private void DisplaySpec()
        {
            if (CheckItem.Slide == true) 
            {
                fpSpread1.ActiveSheet.Cells[2, 1].ForeColor = Color.Black;
                fpSpread1.ActiveSheet.Cells[2, 1].BackColor = SelectOnColor;
            }
            else
            {
                fpSpread1.ActiveSheet.Cells[2, 1].ForeColor = Color.Silver;
                fpSpread1.ActiveSheet.Cells[2, 1].BackColor = SelectOffColor;
            }
            if (CheckItem.Recline == true)
            {
                fpSpread1.ActiveSheet.Cells[3, 1].ForeColor = Color.Black;
                fpSpread1.ActiveSheet.Cells[3, 1].BackColor = SelectOnColor;
            }
            else
            {
                fpSpread1.ActiveSheet.Cells[3, 1].ForeColor = Color.Silver;
                fpSpread1.ActiveSheet.Cells[3, 1].BackColor = SelectOffColor;
            }

            if (CheckItem.Usb == true)
            {
                fpSpread1.ActiveSheet.Cells[4, 1].ForeColor = Color.Black;
                fpSpread1.ActiveSheet.Cells[4, 1].BackColor = SelectOnColor;
            }
            else
            {
                fpSpread1.ActiveSheet.Cells[4, 1].ForeColor = Color.Silver;
                fpSpread1.ActiveSheet.Cells[4, 1].BackColor = SelectOffColor;
            }
            return;
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            label2.Text = DateTime.Now.ToLongDateString() + " " + DateTime.Now.ToLongTimeString();

            if (MESCtrl != null)
            {
                if (led1.Value.AsBoolean != MESCtrl.isClientConnection) led1.Value.AsBoolean = MESCtrl.isClientConnection;
            }
            else
            {
                if (led1.Value.AsBoolean != false) led1.Value.AsBoolean = false;
            }
                        
            if (Infor.Day != DateTime.Now.Day)
            {
                string Path = Program.DATA_PATH.ToString() + "\\" + DateTime.Now.ToString("yyyyMM") + ".xls";
                Infor.Date = DateTime.Now.ToString("yyyyMMdd");
                Infor.DataName = Path;
                Infor.TotalCount = 0;
                Infor.OkCount = 0;
                Infor.NgCount = 0;
                Infor.Day = DateTime.Now.Day;
                SaveInfor();

                sevenSegmentInteger1.Value.AsInteger = Infor.TotalCount;
                sevenSegmentInteger2.Value.AsInteger = Infor.OkCount;
                sevenSegmentInteger3.Value.AsInteger = Infor.NgCount;                
            }

            return;
        }

        private void toolStripButton2_Click(object sender, EventArgs e)
        {
            if (toolStripButton5.Text == "로그인")
            {
                MessageBox.Show(this, "로그인을 먼저 진행하십시오.", "경고", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (IOPort.GetAuto == false)
            {
                if (SysSetForm == null)
                {
                    OpenFormClose();
                    panel1.SendToBack();
                    panel1.Visible = false;

                    panel2.Visible = true;
                    if (panel2.Parent != this) panel2.Parent = this;
                    panel2.BringToFront();
                    SpecSetForm = new SpecSetting(this)
                    {
                        MaximizeBox = false,
                        MinimizeBox = false,
                        ControlBox = false,
                        ShowIcon = false,
                        StartPosition = FormStartPosition.CenterParent,
                        WindowState = FormWindowState.Maximized,
                        TopMost = false,
                        TopLevel = false,
                        FormBorderStyle = FormBorderStyle.None,
                        Location = new Point(0, 0),
                        Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Bottom                        
                    };

                    SpecSetForm.FormClosing += delegate (object sender1, FormClosingEventArgs e1)
                    {
                        e1.Cancel = false;
                        SpecSetForm.Parent = null;
                        SpecSetForm.Dispose();
                        SpecSetForm = null;

                        string xName = Program.SPEC_PATH.ToString() + "\\" + comboBox1.SelectedItem.ToString() + ".spc";
                        if (File.Exists(xName) == true)
                        {
                            mSpec = ComF.OpenSpec(TSpec: ref mSpec, Name: xName);
                            DisplaySpec();
                            Model = comboBox1.SelectedItem.ToString();
                        }
                    };

                    SpecSetForm.Parent = panel2;
                    SpecSetForm.Show();
                }
            }
        }

        private BackgroundWorker backgroundWorker1 = null;
        private void ThreadSetting()
        {
            backgroundWorker1 = new BackgroundWorker();

            //ReportProgress메소드를 호출하기 위해서 반드시 true로 설정, false일 경우 ReportProgress메소드를 호출하면 exception 발생
            backgroundWorker1.WorkerReportsProgress = true;
            //스레드에서 취소 지원 여부
            backgroundWorker1.WorkerSupportsCancellation = true;
            //스레드가 run시에 호출되는 핸들러 등록
            backgroundWorker1.DoWork += new DoWorkEventHandler(BackgroundWorker1_DoWork);
            // ReportProgress메소드 호출시 호출되는 핸들러 등록
            backgroundWorker1.ProgressChanged += new ProgressChangedEventHandler(BackgroundWorker1_ProgressChanged);
            // 스레드 완료(종료)시 호출되는 핸들러 동록
            backgroundWorker1.RunWorkerCompleted += new RunWorkerCompletedEventHandler(BackgroundWorker1_RunWorkerCompleted);


            // 스레드가 Busy(즉, run)가 아니라면
            if (backgroundWorker1.IsBusy != true)
            {
                // 스레드 작동!! 아래 함수 호출 시 위에서 bw.DoWork += new DoWorkEventHandler(bw_DoWork); 에 등록한 핸들러가
                // 호출 됩니다.

                backgroundWorker1.RunWorkerAsync();
            }
            return;
        }

        private void BackgroundWorker1_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            //바로 위에서 worker.ReportProgress((i * 10));호출 시 
            // bw.ProgressChanged += new ProgressChangedEventHandler(bw_ProgressChanged); 등록한 핸들러가 호출 된다고
            // 하였는데요.. 이 부분에서는 기존 Thread에서 처럼 Dispatcher를 이용하지 않아도 됩니다. 
            // 즉 아래처럼!!사용이 가능합니다.
            //this.tbProgress.Text = (e.ProgressPercentage.ToString() + "%");

            // 기존의 Thread클래스에서 아래와 같이 UI 엘리먼트를 갱신하려면
            // Dispatcher.BeginInvoke(delegate() 
            // {
            //        this.tbProgress.Text = (e.ProgressPercentage.ToString() + "%");
            // )};
            //처럼 처리해야 할 것입니다. 그러나 바로 UI 엘리먼트를 업데이트 하고 있죠??
        }


        //스레드의 run함수가 종료될 경우 해당 핸들러가 호출됩니다.
        private void BackgroundWorker1_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {

            //스레드가 종료한 이유(사용자 취소, 완료, 에러)에 맞쳐 처리하면 됩니다.
            if ((e.Cancelled == true))
            {
            }
            else if (!(e.Error == null))
            {

            }
            else
            {

            }
        }


        private void BackgroundWorker1_DoWork(object sender, DoWorkEventArgs e)
        {
            BackgroundWorker worker = sender as BackgroundWorker;

            do
            {
                //CancellationPending 속성이 true로 set되었다면(위에서 CancelAsync 메소드 호출 시 true로 set된다고 하였죠?
                if ((worker.CancellationPending == true))
                {
                    //루프를 break한다.(즉 스레드 run 핸들러를 벗어나겠죠)
                    e.Cancel = true;
                    break;
                }
                else
                {
                    // 이곳에는 스레드에서 처리할 연산을 넣으시면 됩니다.
                    this.Invoke(new EventHandler(Processing));
                    Thread.Sleep(5);
                    // 스레드 진행상태 보고 - 이 메소드를 호출 시 위에서 
                    // bw.ProgressChanged += new ProgressChangedEventHandler(bw_ProgressChanged); 등록한 핸들러가 호출 됩니다.
                    worker.ReportProgress(10);
                }
                if ((ExitFlag == true) || (RunningFlag == false))
                {
                    worker.CancelAsync();
                }
            } while (true);
            //while (ExitFlag == false);
        }

        private void StartSetting()
        {
            Infor.TotalCount++;
            OldPopDataCheckFlag = PopDataCheckFlag;
            Step = 0;
            SpecOutputFlag = false;
            InitScreen();
            DisplayStatus(RESULT.TEST);

            IOPort.SetBatt = false;
            IOPort.SetING = true;
            ComF.timedelay(200);

            if (comboBox3.SelectedItem != null)
            {
                if (comboBox3.SelectedItem.ToString() == "LH")
                    IOPort.SetRHECU = false;
                else IOPort.SetRHECU = true;                
            }
            else
            {
                IOPort.SetRHECU = false;
            }

            ComF.timedelay(200);

            IOPort.SetYellowLamp = true;
            IOPort.SetGreenLamp = false;
            IOPort.SetRedLamp = false;
            RunningFlag = true;
            TotalTimeToFirst = ComF.timeGetTimems();
            TotalTimeToLast = ComF.timeGetTimems();
            ThreadSetting();
            return;
        }

        private void StopSetting()
        {
            label16.Text = "";
            label5.Text = "";
            RunningFlag = false;
            Infor.TotalCount--;
            IOPort.SetBatt = false;
            IOPort.SetING = false;
            IOPort.SetRHECU = false;
            IOPort.SetYellowLamp = false;
            IOPort.SetRedLamp = false;
            IOPort.SetGreenLamp = false;
            if (panel4.Visible == true) panel4.Visible = false;

            if (led5.Visible == true) led5.Visible = false;
            if (led10.Visible == true) led10.Visible = false;
            if (led5.Visible == true) led5.Visible = false;
            if (led10.Visible == true) led10.Visible = false;
            return;
        }

        private void InitScreen()
        {
            TData.Recline.Test = false;
            TData.Recline.Data = 0;
            TData.Recline.Result = RESULT.CLEAR;

            TData.Slide.Test = false;
            TData.Slide.Data = 0;
            TData.Slide.Result = RESULT.CLEAR;

            TData.Usb.Test = false;
            TData.Usb.Data = 0;
            TData.Usb.Result = RESULT.CLEAR;

            TData.Time = "";
            TData.Result = RESULT.CLEAR;

            plot1.Channels[0].Clear();

            for(int i = 2;i <fpSpread1.ActiveSheet.RowCount;i++)
            {
                fpSpread1.ActiveSheet.Cells[i, 3].Text = "";
                fpSpread1.ActiveSheet.Cells[i, 4].Text = "";
                fpSpread1.ActiveSheet.Cells[i, 4].ForeColor = Color.White;
            }
            label5.Text = "";
            label16.Text = "";
            return;
        }


        private long StepTimeToFirst = 0L;
        private long StepTimeToLast = 0L;
        private long TotalTimeToFirst = 0L;
        private long TotalTimeToLast = 0L;
        private bool SpecOutputFlag = false;
        private short Step = 0;
        private long ViewTimeToFirst = 0;
        private long ViewTimeToLast = 0;
        /// <summary>
        /// 가상 리미트 검사 진행
        /// </summary>
        private void Processing(object sender, EventArgs e)
        {
            TotalTimeToLast = ComF.timeGetTimems();
            StepTimeToLast = ComF.timeGetTimems();

            if ((panel4.Visible == true) || (led5.Visible == true) || (led10.Visible == true))
            {
                ViewTimeToLast = ComF.timeGetTimems();

                if ((panel4.BackColor == Color.DeepSkyBlue) || (led5.Value.AsBoolean == true) || (led10.Value.AsBoolean == true))
                {
                    if (600 <= (ViewTimeToLast - ViewTimeToFirst))
                    {
                        ViewTimeToFirst = ComF.timeGetTimems();
                        ViewTimeToLast = ComF.timeGetTimems();
                        if(panel4.Visible == true) panel4.BackColor = Color.RoyalBlue;
                        if (led5.Value.AsBoolean == true) led5.Value.AsBoolean = false;
                        if (led10.Value.AsBoolean == true) led10.Value.AsBoolean = false;
                    }
                }
                else
                {
                    if (500 <= (ViewTimeToLast - ViewTimeToFirst))
                    {
                        ViewTimeToFirst = ComF.timeGetTimems();
                        ViewTimeToLast = ComF.timeGetTimems();
                        if(panel4.Visible == true) panel4.BackColor = Color.DeepSkyBlue;
                        if (led5.Value.AsBoolean == false) led5.Value.AsBoolean = true;
                        if (led10.Value.AsBoolean == false) led10.Value.AsBoolean = true;
                    }
                }
            }

            int TestTime = (int)((TotalTimeToLast - TotalTimeToFirst) / 1000);

            if (sevenSegmentInteger4.Value.AsInteger != TestTime) sevenSegmentInteger4.Value.AsInteger = TestTime;

            switch (Step)
            {
                case 0:
                    IOPort.SetBatt = true;
                    ComF.timedelay(200);
                    //IOPort.SetIGN1 = true;
                    label7.Text = "";
                    //panel4.Visible = true;
                    //panel4.Parent = this;
                    //panel1.Location = new Point(119, 454);
                    //panel4.BringToFront();
                    //panel4.BackColor = Color.DeepSkyBlue;

                    //if (led5.Value.AsBoolean == false) led5.Value.AsBoolean = true;
                    //if (led10.Value.AsBoolean == true) led10.Value.AsBoolean = false;
                    //led5.Value.AsBoolean = false;
                    //ViewTimeToFirst = ComF.timeGetTimems();
                    //ViewTimeToLast = ComF.timeGetTimems();
                    plot1.Channels[0].Clear();
                    SpecOutputFlag = false;
                    Step++;
                    break;
                case 1:
                    if (SpecOutputFlag == false)
                    {
                        StepTimeToFirst = ComF.timeGetTimems();
                        StepTimeToLast = ComF.timeGetTimems();
                        SpecOutputFlag = true;
                        label5.Text = "USB 를 검사 합니다.";
                        label7.Text = "USB 를 검사 합니다.";
                        OffFlag = false;
                    }
                    else
                    {
                        float Curr = pMeter.GetPSeat;

                        if (Curr <= 0.1) OffFlag = true;

                        StepTimeToLast = ComF.timeGetTimems();
                        if (1000 <= (StepTimeToLast - StepTimeToFirst))
                        {
                            fpSpread1.ActiveSheet.Cells[4, 4].Text = "NG";
                            fpSpread1.ActiveSheet.Cells[4, 4].ForeColor = Color.Red;
                            TData.Usb.Result = RESULT.REJECT;
                            TData.Usb.Test = true;
                            TData.Usb.Data = Curr;
                            SpecOutputFlag = false;
                            Step++;
                        }
                        else if (mSpec.UsbCurrent <= pMeter.GetPSeat) //if (IOPort.GetUsb ==  true)
                        {
                            fpSpread1.ActiveSheet.Cells[4, 4].Text = "OK";
                            fpSpread1.ActiveSheet.Cells[4, 4].ForeColor = Color.Lime;
                            TData.Usb.Result = RESULT.PASS;
                            TData.Usb.Test = true;
                            TData.Usb.Data = Curr;
                            SpecOutputFlag = false;
                            Step++;
                        }
                    }
                    break;
                case 2:
                    //Walk In To Slide 
                    if (CheckItem.Slide == true)
                    {
                        WalkInToSlideCheck();
                    }
                    else
                    {
                        Step++;
                        SpecOutputFlag = false;
                    }
                    break;
                case 3:
                    //OFF 를 위한 대기 시간
                    if (SpecOutputFlag == false)
                    {
                        StepTimeToFirst = ComF.timeGetTimems();
                        SpecOutputFlag = true;
                    }
                    else
                    {                        
                        if((long)(mSpec.DelayTime * 1000F) <= (StepTimeToLast - StepTimeToFirst))
                        {
                            Step++;
                            SpecOutputFlag = false;
                        }
                    }
                    break;                               
                case 4:
                    //Walk In To Slide Fwd
                    if (CheckItem.Recline == true)
                    {
                        WalkInToReclineCheck();
                    }
                    else
                    {
                        Step++;
                        SpecOutputFlag = false;
                    }
                    break;                                
                default:
                    if (led5.Visible == true) led5.Visible = false;
                    if (led10.Visible == true) led10.Visible = false;
                    led5.Value.AsBoolean = false;
                    led10.Value.AsBoolean = false;
                    panel4.Visible = false;
                    CheckResult();
                    SaveData();
                    IOPort.SetING = false;
                    break;
            }
            return;
        }


        public void CheckResult()
        {
            TData.Result = RESULT.PASS;
            if (TData.Slide.Test == true)
            {
                if (TData.Slide.Result != RESULT.PASS) TData.Result = RESULT.REJECT;
            }
            if (TData.Recline.Test == true)
            {
                if (TData.Recline.Result != RESULT.PASS) TData.Result = RESULT.REJECT;
            }
            if (TData.Usb.Test == true)
            {
                if (TData.Usb.Result != RESULT.PASS) TData.Result = RESULT.REJECT;
            }

            Infor.TotalCount++;
            
            DisplayStatus(TData.Result);
            RunningFlag = false;

            IOPort.SetYellowLamp = true;
            if(TData.Result == RESULT.PASS) 
                IOPort.SetGreenLamp = true;
            else IOPort.SetRedLamp = true;

            if (TData.Result == RESULT.PASS)
                Infor.OkCount++;
            else Infor.NgCount++;
            
            SendData();

            if (TData.Result == RESULT.PASS)
            {
                label5.Text = "양품 제품입니다.";
                IOPort.SetBuzzer = true;
                ComF.timedelay(700);
                IOPort.SetBuzzer = false;
                IOPort.SetTestOk = true;
                ComF.timedelay(1500);
                IOPort.SetTestOk = false;                
            }
            else
            {
                label5.Text = "불량 제품입니다.\n재 검사를 진행하려면 RESET 후 PASS 버튼을 눌러주세요.";
                IOPort.SetBuzzer = true;
                ComF.timedelay(600);
                IOPort.SetBuzzer = false;
                ComF.timedelay(400);
                IOPort.SetBuzzer = true;
                ComF.timedelay(600);
                IOPort.SetBuzzer = false;
            }

            sevenSegmentInteger1.Value.AsInteger = Infor.TotalCount;
            sevenSegmentInteger2.Value.AsInteger = Infor.OkCount;
            sevenSegmentInteger3.Value.AsInteger = Infor.NgCount;
            return;
        }

        private void SendData()
        {
            //CheckItem.Slide = true;
            //CheckItem.Recline = true;
            //CheckItem.Usb = true;
            //TData.Slide.Test = true;
            //TData.Slide.Result = RESULT.PASS;
            //TData.Recline.Test = true;
            //TData.Recline.Result = RESULT.REJECT;
            //TData.Usb.Test = true;
            //TData.Usb.Result = RESULT.PASS;


            string s = MESCtrl.CrateData(
                   Serial: Infor.OkCount,
                   TestCount: Infor.TotalCount,
                   Min_1: CheckItem.Slide == true ? 1 : -9999,
                   Max_1: CheckItem.Slide == true ? 1 : -9999,
                   Min_2: CheckItem.Recline == true ? 1 : -9999,
                   Max_2: CheckItem.Recline == true ? 1 : -9999,
                   Min_3: CheckItem.Usb == true ? 1 : -9999,
                   Max_3: CheckItem.Usb == true ? 1 : -9999,
                   Min_4: -9999,
                   Max_4: -9999,
                   Min_5: -9999,
                   Max_5: -9999,
                   Min_6: -9999,
                   Max_6: -9999,
                   Min_7: -9999,
                   Max_7: -9999,
                   Min_8: -9999,
                   Max_8: -9999,
                   Min_9: -9999,
                   Max_9: -9999,
                   Min_10: -9999,
                   Max_10: -9999,
                   Min_11: -9999,
                   Max_11: -9999,
                   Min_12: -9999,
                   Max_12: -9999,
                   Min_13: -9999,
                   Max_13: -9999,
                   Min_14: -9999,
                   Max_14: -9999,
                   Min_15: -9999,
                   Max_15: -9999,
                   Value1: CheckItem.Slide == true ? ((TData.Slide.Test == true) && (TData.Slide.Result == RESULT.PASS) ? 1 : 9) : -9999,
                   Value2: CheckItem.Recline == true ? ((TData.Recline.Test == true) && (TData.Recline.Result == RESULT.PASS) ? 1 : 9) : -9999,
                   Value3: CheckItem.Usb == true ? ((TData.Usb.Test == true) && (TData.Usb.Result == RESULT.PASS) ? 1 : 9) : -9999,
                   Value4: -9999,
                   Value5: -9999,
                   Value6: -9999,
                   Value7: -9999,
                   Value8: -9999,
                   Value9: -9999,
                   Value10: -9999,
                   Value11: -9999,
                   Value12: -9999,
                   Value13: -9999,
                   Value14: -9999,
                   Value15: -9999,
                   Result: (short)TData.Result
                   );
            if (IOPort.GetAuto == true)
            {
                //if (MESCtrl.isServerConnection == true) MESCtrl.ServerSend = s;
                if (MESCtrl.isClientConnection == true) MESCtrl.Write = s + ",1";
            }
            else
            {
                if (MESCtrl.isClientConnection == true) MESCtrl.Write = s + ",2";
            }

            SaveLogData = "Sending - " + DateTime.Now.ToLongDateString() + " " + DateTime.Now.ToLongTimeString();
            SaveLogData = s;
            return;
        }

        private string SaveLogData
        {
            set
            {
                string xPath = Program.LOG_PATH.ToString() + "\\" + DateTime.Now.ToString("yyyyMMdd") + ".txt";
                using (FileStream fp = File.Open(xPath, FileMode.Append, FileAccess.Write))
                {
                    StreamWriter writer = new StreamWriter(fp);
                    writer.Write(value + "\n");
                    writer.Close();
                    fp.Close();
                }

                //using (File.Open(xPath, FileMode.Open))
                //{
                //    File.AppendAllText(value + "\n", xPath);
                //}
            }
        }

        private bool OffFlag = false;
        
        private void WalkInToSlideCheck()
        {
            if (SpecOutputFlag == false)
            {
                led5.Visible = true;
                led10.Visible = false;
                led5.Value.AsBoolean = false;
                led10.Value.AsBoolean = false;
                ViewTimeToFirst = ComF.timeGetTimems();
                ViewTimeToLast = ComF.timeGetTimems();

                StepTimeToFirst = ComF.timeGetTimems();
                StepTimeToLast = ComF.timeGetTimems();
                SpecOutputFlag = true;
                label5.Text = "슬라이드 스위치를 동작시켜 주십시오.";
                label7.Text = "슬라이드 스위치를 ON 시켜 주십시오.";
                OffFlag = false;
            }
            else
            {
                float Curr = pMeter.GetPSeat;

                plot1.Channels[0].AddXY(plot1.Channels[0].Count, Curr);
                plot1.XAxes[0].Tracking.ZoomToFitAll();
                plot1.YAxes[0].Tracking.ZoomToFitAll();
                if (Curr <= mSpec.OffCurrent) OffFlag = true;
                                
                StepTimeToLast = ComF.timeGetTimems();
                if ((mSpec.TestTime * 1000F) <= (StepTimeToLast - StepTimeToFirst))
                {
                    fpSpread1.ActiveSheet.Cells[2, 3].Text = "NG";
                    fpSpread1.ActiveSheet.Cells[2, 4].Text = "NG";
                    fpSpread1.ActiveSheet.Cells[2, 4].ForeColor = Color.Red;
                    TData.Slide.Result = RESULT.REJECT;
                    TData.Slide.Test = true;
                    TData.Slide.Data = Curr;
                    label7.Text = "슬라이드 스위치를 OFF 시켜 주십시오.";
                    SpecOutputFlag = false;
                    Step++;
                }
                else if (mSpec.SlideSpec <= Curr)
                {
                    if (OffFlag == true)
                    {
                        fpSpread1.ActiveSheet.Cells[2, 3].Text = "OK";
                        fpSpread1.ActiveSheet.Cells[2, 4].Text = "OK";
                        fpSpread1.ActiveSheet.Cells[2, 4].ForeColor = Color.Lime;
                        label7.Text = "슬라이드 스위치를 OFF 시켜 주십시오.";
                        TData.Slide.Result = RESULT.PASS;
                        TData.Slide.Test = true;
                        TData.Slide.Data = Curr;
                        SpecOutputFlag = false;
                        Step++;
                    }
                }
            }
            return;
        }

        
        private void WalkInToReclineCheck()
        {
            if (SpecOutputFlag == false)
            {
                StepTimeToFirst = ComF.timeGetTimems();
                StepTimeToLast = ComF.timeGetTimems();
                SpecOutputFlag = true;
                label5.Text = "리클라인 스위치를 동작시켜 주십시오.";
                label7.Text = "리클라인 스위치를 ON 시켜 주십시오.";
                OffFlag = false;

                led5.Visible = false;
                led10.Visible = true;
                led5.Value.AsBoolean = false;
                led10.Value.AsBoolean = false;
                ViewTimeToFirst = ComF.timeGetTimems();
                ViewTimeToLast = ComF.timeGetTimems();
            }
            else
            {
                float Curr = pMeter.GetPSeat;

                plot1.Channels[0].AddXY(plot1.Channels[0].Count, Curr);
                plot1.XAxes[0].Tracking.ZoomToFitAll(); 
                plot1.YAxes[0].Tracking.ZoomToFitAll();

                if (Curr <= mSpec.OffCurrent) OffFlag = true;

                StepTimeToLast = ComF.timeGetTimems();
                if ((mSpec.TestTime * 1000F) <= (StepTimeToLast - StepTimeToFirst))
                {
                    fpSpread1.ActiveSheet.Cells[3, 3].Text = "NG";
                    fpSpread1.ActiveSheet.Cells[3, 4].Text = "NG";
                    fpSpread1.ActiveSheet.Cells[3, 4].ForeColor = Color.Red;
                    TData.Recline.Result = RESULT.REJECT;
                    label7.Text = "리클라인 스위치를 OFF 시켜 주십시오.";
                    TData.Recline.Test = true;
                    TData.Recline.Data = Curr;
                    SpecOutputFlag = false;
                    Step++;
                }
                else if (mSpec.ReclineSpec <= Curr)
                {
                    if (OffFlag == true)
                    {
                        fpSpread1.ActiveSheet.Cells[3, 3].Text = "OK";
                        fpSpread1.ActiveSheet.Cells[3, 4].Text = "OK";
                        label7.Text = "리클라인 스위치를 OFF 시켜 주십시오.";
                        fpSpread1.ActiveSheet.Cells[3, 4].ForeColor = Color.Lime;
                        TData.Recline.Result = RESULT.PASS;
                        TData.Recline.Test = true;
                        TData.Recline.Data = Curr;
                        SpecOutputFlag = false;
                        Step++;
                    }
                }
            }
            return;
        }

       
        
        private void DisplayStatus(short Result)
        {
            switch (Result)
            {
                case RESULT.TEST:
                    label16.ForeColor = Color.Yellow;
                    label16.Text = "검사중";
                    break;
                case RESULT.PASS:
                    label16.ForeColor = Color.Lime;
                    label16.Text = "OK";
                    break;
                case RESULT.REJECT:
                    label16.ForeColor = Color.Red;
                    label16.Text = "NG";
                    break;
                default:
                    label16.ForeColor = Color.White;
                    label16.Text = "";
                    break;
            }
            return;
        }


        private int RowCount { get; set; }

        private void CreateFileName()
        {
            //string Path = Program.DATA_PATH.ToString() + "\\" + DateTime.Now.ToString("yyyyMMdd") + ".xls";
            string Path = Program.DATA_PATH.ToString() + "\\" + DateTime.Now.ToString("yyyyMM") + ".xls";

            //if ((Infor.DataName != "") && (Infor.DataName != null))
            //{
            //    if (File.Exists(Infor.DataName))
            //    {
            //        if (Infor.DataName != Path)
            //        {
            //            Infor.Date = DateTime.Now.ToString("yyyyMMdd");
            //            Infor.DataName = Path;
            //            Infor.TotalCount = 0;
            //            Infor.OkCount = 0;
            //            Infor.NgCount = 0;
            //            Infor.Day = DateTime.Now.Day;
            //            SaveInfor();
            //            sevenSegmentInteger1.Value.AsInteger = Infor.TotalCount;
            //            sevenSegmentInteger2.Value.AsInteger = Infor.OkCount;
            //            sevenSegmentInteger3.Value.AsInteger = Infor.NgCount;
            //        }
            //    }
            //}
            //else
            //{
            //    Infor.Date = DateTime.Now.ToString("yyyyMMdd");

            //    Infor.DataName = Path;
            //    Infor.TotalCount = 0;
            //    Infor.OkCount = 0;
            //    Infor.NgCount = 0;
            //    Infor.Day = DateTime.Now.Day;
            //    SaveInfor();
            //    sevenSegmentInteger1.Value.AsInteger = Infor.TotalCount;
            //    sevenSegmentInteger2.Value.AsInteger = Infor.OkCount;
            //    sevenSegmentInteger3.Value.AsInteger = Infor.NgCount;
            //}

            if (File.Exists(Path) == false)
            {
                CreateDataFile(Path);
            }
            else
            {
                fpSpread2.OpenExcel(Path);
                fpSpread2.ActiveSheet.Protect = false;
                RowCount = 6;
                for (int i = RowCount; i < fpSpread2.ActiveSheet.RowCount; i++)
                {
                    if (fpSpread2.ActiveSheet.Cells[RowCount, 0].Text == "") break;
                    if (fpSpread2.ActiveSheet.Cells[RowCount, 0].Text == null) break;
                    RowCount++;
                }

                int Col = 0;

                for (int i = 0; i < fpSpread2.ActiveSheet.ColumnCount; i++)
                {
                    if (fpSpread2.ActiveSheet.Cells[4, Col].Text == "HEIGHT")
                        break;
                    else Col++;
                }
                fpSpread2.ActiveSheet.ColumnCount = Col + 1;
            }
            return;
        }

        private void SaveData()
        {
            string Path = Program.DATA_PATH.ToString() + "\\" + DateTime.Now.ToString("yyyyMM") + ".xls";


            //if ((Infor.DataName != "") && (Infor.DataName != null))
            //{
            //    if (File.Exists(Infor.DataName))
            //    {
            //        if (Infor.DataName != Path)
            //        {
            //            Infor.Date = DateTime.Now.ToString("yyyyMMdd");
            //            Infor.DataName = Path;
            //            Infor.TotalCount = 1;
            //            if (TData.Result == RESULT.PASS)
            //                Infor.OkCount = 1;
            //            else Infor.NgCount = 1;
            //            Infor.Day = DateTime.Now.Day;
            //            SaveInfor();

            //            sevenSegmentInteger1.Value.AsInteger = Infor.TotalCount;
            //            sevenSegmentInteger2.Value.AsInteger = Infor.OkCount;
            //            sevenSegmentInteger3.Value.AsInteger = Infor.NgCount;
            //        }
            //        else
            //        {
            //            SaveInfor();
            //        }
            //    }
            //    else
            //    {
            //        SaveInfor();
            //    }
            //}
            //else
            //{
            //    Infor.Date = DateTime.Now.ToString("yyyyMMdd");
            //    Infor.DataName = Path;
            //    Infor.TotalCount = 1;
            //    if (TData.Result == RESULT.PASS)
            //        Infor.OkCount = 1;
            //    else Infor.NgCount = 1;
            //    sevenSegmentInteger1.Value.AsInteger = Infor.TotalCount;
            //    sevenSegmentInteger2.Value.AsInteger = Infor.OkCount;
            //    sevenSegmentInteger3.Value.AsInteger = Infor.NgCount;
            //    Infor.Day = DateTime.Now.Day;
            //    SaveInfor();
            //}

            //if (Infor.DataName != Path)
            //{
            //    Infor.Date = DateTime.Now.ToString("yyyyMMdd");
            //    Infor.DataName = Path;
            //    Infor.TotalCount = 1;
            //    if (TData.Result == RESULT.PASS)
            //        Infor.OkCount = 1;
            //    else Infor.NgCount = 1;
            //    sevenSegmentInteger1.Value.AsInteger = Infor.TotalCount;
            //    sevenSegmentInteger2.Value.AsInteger = Infor.OkCount;
            //    sevenSegmentInteger3.Value.AsInteger = Infor.NgCount;
            //    Infor.Day = DateTime.Now.Day;
            //    SaveInfor();
            //}

            if (File.Exists(Path) == false) CreateDataFile(Path);


            int Col = 0;

            fpSpread2.ActiveSheet.RowCount = RowCount + 1;

            fpSpread2.ActiveSheet.SetRowHeight(RowCount, 21);
            //if(fpSpread2.ActiveSheet.ColumnCount != 10) fpSpread2.ActiveSheet.ColumnCount = 10;

            for (int i = 0; i < fpSpread2.ActiveSheet.ColumnCount; i++)
            {
                fpSpread2.ActiveSheet.Cells[RowCount, i].CellType = new FarPoint.Win.Spread.CellType.TextCellType();
                fpSpread2.ActiveSheet.Cells[RowCount, i].VerticalAlignment = FarPoint.Win.Spread.CellVerticalAlignment.Center;
                fpSpread2.ActiveSheet.Cells[RowCount, i].HorizontalAlignment = FarPoint.Win.Spread.CellHorizontalAlignment.Center;
                fpSpread2.ActiveSheet.Cells[RowCount, i].Border = LineBorderToData;
                fpSpread2.ActiveSheet.Cells[RowCount, i].Text = "";
            }
            //No.
            fpSpread2.ActiveSheet.Cells[RowCount, Col].BackColor = Color.White;
            fpSpread2.ActiveSheet.Cells[RowCount, Col].ForeColor = Color.Black;
            fpSpread2.ActiveSheet.Cells[RowCount, Col].Text = ((RowCount - 6) + 1).ToString();
            Col++;

            //Time
            fpSpread2.ActiveSheet.Cells[RowCount, Col].BackColor = Color.White;
            fpSpread2.ActiveSheet.Cells[RowCount, Col].ForeColor = Color.Black;
            fpSpread2.ActiveSheet.Cells[RowCount, Col].Text = DateTime.Now.ToString("yyyy-MM-dd") + " " + DateTime.Now.ToLongTimeString();
            Col++;

            //Barcode
            fpSpread2.ActiveSheet.Cells[RowCount, Col].BackColor = Color.White;
            fpSpread2.ActiveSheet.Cells[RowCount, Col].ForeColor = Color.Black;
            fpSpread2.ActiveSheet.Cells[RowCount, Col].Text = label17.Text;
            Col++;

            //자동, 소동
            fpSpread2.ActiveSheet.Cells[RowCount, Col].BackColor = Color.White;
            fpSpread2.ActiveSheet.Cells[RowCount, Col].ForeColor = Color.Black;
            if (IOPort.GetAuto == true)
                fpSpread2.ActiveSheet.Cells[RowCount, Col].Text = "자동";
            else fpSpread2.ActiveSheet.Cells[RowCount, Col].Text = "수동";
            Col++;

            if (TData.Result == RESULT.PASS)
            {
                fpSpread2.ActiveSheet.Cells[RowCount, Col].BackColor = Color.White;
                fpSpread2.ActiveSheet.Cells[RowCount, Col].ForeColor = Color.Black;
            }
            else if (TData.Result == RESULT.REJECT)
            {
                fpSpread2.ActiveSheet.Cells[RowCount, Col].BackColor = Color.Red;
                fpSpread2.ActiveSheet.Cells[RowCount, Col].ForeColor = Color.White;
            }
            if (TData.Result == RESULT.PASS)
                fpSpread2.ActiveSheet.Cells[RowCount, Col].Text = "O.K";
            else if (TData.Result == RESULT.REJECT)
                fpSpread2.ActiveSheet.Cells[RowCount, Col].Text = "N.G";
            Col++;

            if (TData.Slide.Test == true)
            {
                if (TData.Slide.Result != RESULT.REJECT)
                {
                    fpSpread2.ActiveSheet.Cells[RowCount, Col].BackColor = Color.White;
                    fpSpread2.ActiveSheet.Cells[RowCount, Col].ForeColor = Color.Black;
                }
                else
                {
                    fpSpread2.ActiveSheet.Cells[RowCount, Col].BackColor = Color.Red;
                    fpSpread2.ActiveSheet.Cells[RowCount, Col].ForeColor = Color.White;
                }
                if (TData.Slide.Result != RESULT.REJECT)
                    fpSpread2.ActiveSheet.Cells[RowCount, Col].Text = "O.K";
                else fpSpread2.ActiveSheet.Cells[RowCount, Col].Text = "N.G";
            }
            Col++;
            if (TData.Recline.Test == true)
            {
                if (TData.Recline.Result != RESULT.REJECT)
                {
                    fpSpread2.ActiveSheet.Cells[RowCount, Col].BackColor = Color.White;
                    fpSpread2.ActiveSheet.Cells[RowCount, Col].ForeColor = Color.Black;
                }
                else
                {
                    fpSpread2.ActiveSheet.Cells[RowCount, Col].BackColor = Color.Red;
                    fpSpread2.ActiveSheet.Cells[RowCount, Col].ForeColor = Color.White;
                }
                if (TData.Recline.Result != RESULT.REJECT)
                    fpSpread2.ActiveSheet.Cells[RowCount, Col].Text = "O.K";
                else fpSpread2.ActiveSheet.Cells[RowCount, Col].Text = "N.G";
            }
            Col++;
            if (TData.Usb.Test == true)
            {
                if (TData.Usb.Result != RESULT.REJECT)
                {
                    fpSpread2.ActiveSheet.Cells[RowCount, Col].BackColor = Color.White;
                    fpSpread2.ActiveSheet.Cells[RowCount, Col].ForeColor = Color.Black;
                }
                else
                {
                    fpSpread2.ActiveSheet.Cells[RowCount, Col].BackColor = Color.Red;
                    fpSpread2.ActiveSheet.Cells[RowCount, Col].ForeColor = Color.White;
                }
                if (TData.Usb.Result != RESULT.REJECT)
                    fpSpread2.ActiveSheet.Cells[RowCount, Col].Text = "O.K";
                else fpSpread2.ActiveSheet.Cells[RowCount, Col].Text = "N.G";
            }
            Col++;
            

            //if(fpSpread2.ActiveSheet.ColumnCount < Col) fpSpread2.ActiveSheet.ColumnCount = Col;

            RowCount++;

            fpSpread2.SaveExcel(Path);
            return;
        }


        private FarPoint.Win.LineBorder LineBorderToHeader = new FarPoint.Win.LineBorder(Color.Black, 1/*RowHeight*/, true, true, true, true);//line color,line style,left,top,right,buttom                       
        private FarPoint.Win.LineBorder LineBorderToData = new FarPoint.Win.LineBorder(Color.Black, 1/*RowHeight*/, true, false, true, true);//line color,line style,left,top,right,buttom                       
        private void CreateDataFile(string dName)
        {
            fpSpread2.ActiveSheet.Reset();
            fpSpread2.ActiveSheet.Protect = false;

            fpSpread2.SuspendLayout();

            fpSpread2.ActiveSheet.RowCount = 9;

            //용지 방향
            fpSpread2.ActiveSheet.PrintInfo.Orientation = FarPoint.Win.Spread.PrintOrientation.Landscape;
            //프린트 할 때 가로,세로 중앙에 프린트 할 수 있도록 설정
            fpSpread2.ActiveSheet.PrintInfo.Centering = FarPoint.Win.Spread.Centering.Horizontal; //좌/우 중앙                        
            //fpSpread2.ActiveSheet.PrintInfo.PrintCenterOnPageV = false; //Top 쪽으로간다. 만약 true로 설정할 경우 상,하 중간에 프린트가 된다.

            //여백
            fpSpread2.ActiveSheet.PrintInfo.Margin.Bottom = 1;
            fpSpread2.ActiveSheet.PrintInfo.Margin.Left = 1;
            fpSpread2.ActiveSheet.PrintInfo.Margin.Right = 1;
            fpSpread2.ActiveSheet.PrintInfo.Margin.Top = 2;

            //프린트에서 컬러 표시
            fpSpread2.ActiveSheet.PrintInfo.ShowColor = true;
            //프린트에서 셀 라인 표시여부 (true일경우 내가 그린 라인 말고 셀에 사각 표시 라인도 같이 프린트가 된다.
            fpSpread2.ActiveSheet.PrintInfo.ShowGrid = false;

            fpSpread2.ActiveSheet.VerticalGridLine = new FarPoint.Win.Spread.GridLine(FarPoint.Win.Spread.GridLineType.None);
            fpSpread2.ActiveSheet.HorizontalGridLine = new FarPoint.Win.Spread.GridLine(FarPoint.Win.Spread.GridLineType.None);
            //용지 넓이에 페이지 맞춤
            fpSpread2.ActiveSheet.PrintInfo.UseSmartPrint = true;

            //그리드를 표시할 경우 저장할 때나 프린트 할 때 화면에 같이 표시,그리드가 프린트 되기 때문에 지저분해 보인다.
            fpSpread2.ActiveSheet.PrintInfo.ShowColumnFooter = FarPoint.Win.Spread.PrintHeader.Hide;
            fpSpread2.ActiveSheet.PrintInfo.ShowColumnFooterEachPage = false;


            //헤더와 밖같 라인이 같이 프린트 되지 않도록 한다.
            fpSpread2.ActiveSheet.PrintInfo.ShowBorder = false;
            fpSpread2.ActiveSheet.PrintInfo.ShowColumnHeader = FarPoint.Win.Spread.PrintHeader.Hide;
            fpSpread2.ActiveSheet.PrintInfo.ShowRowHeader = FarPoint.Win.Spread.PrintHeader.Hide;
            fpSpread2.ActiveSheet.PrintInfo.ShowShadows = false;
            fpSpread2.ActiveSheet.PrintInfo.ShowTitle = FarPoint.Win.Spread.PrintTitle.Hide;
            fpSpread2.ActiveSheet.PrintInfo.ShowSubtitle = FarPoint.Win.Spread.PrintTitle.Hide;

            //시트 보호를 해지 한다.
            //fpSpread2.ActiveSheet.PrintInfo.PrintType = FarPoint.Win.Spread.PrintType.All;
            //fpSpread2.ActiveSheet.PrintInfo.SmartPrintRules.Add(new ReadOnlyAttribute(false));
            //axfpSpread1.Protect = false;


            //for (int i = 0; i < 24; i++) fpSpread2.ActiveSheet.SetColumnWidth(i, 80);

            //틀 고정
            fpSpread2.ActiveSheet.FrozenColumnCount = 3;
            fpSpread2.ActiveSheet.FrozenRowCount = 6;
            fpSpread2.ActiveSheet.Cells[1, 0].VerticalAlignment = FarPoint.Win.Spread.CellVerticalAlignment.Center;
            fpSpread2.ActiveSheet.Cells[1, 0].HorizontalAlignment = FarPoint.Win.Spread.CellHorizontalAlignment.Right;
            fpSpread2.ActiveSheet.Cells[1, 0].Text = "날짜 :";
            fpSpread2.ActiveSheet.AddSpanCell(1, 1, 1, 22);

            fpSpread2.ActiveSheet.Cells[1, 1].CellType = new FarPoint.Win.Spread.CellType.EditBaseCellType();
            fpSpread2.ActiveSheet.Cells[1, 1].Text = DateTime.Now.ToLongDateString();
            fpSpread2.ActiveSheet.Cells[1, 1].VerticalAlignment = FarPoint.Win.Spread.CellVerticalAlignment.Center;
            fpSpread2.ActiveSheet.Cells[1, 1].HorizontalAlignment = FarPoint.Win.Spread.CellHorizontalAlignment.Left;

            int Col;

            Col = 0;
            //No
            fpSpread2.ActiveSheet.SetRowHeight(3, 31);
            fpSpread2.ActiveSheet.SetRowHeight(4, 31);
            fpSpread2.ActiveSheet.SetRowHeight(5, 31);
            fpSpread2.ActiveSheet.AddSpanCell(3, Col, 3, 1);
            fpSpread2.ActiveSheet.SetColumnWidth(Col, 100);
            fpSpread2.ActiveSheet.Cells[3, Col].CellType = new FarPoint.Win.Spread.CellType.EditBaseCellType();
            fpSpread2.ActiveSheet.Cells[3, Col].VerticalAlignment = FarPoint.Win.Spread.CellVerticalAlignment.Center;
            fpSpread2.ActiveSheet.Cells[3, Col].HorizontalAlignment = FarPoint.Win.Spread.CellHorizontalAlignment.Center;
            fpSpread2.ActiveSheet.Cells[3, Col].Text = "NO.";
            fpSpread2.ActiveSheet.Cells[3, Col].BackColor = Color.WhiteSmoke;
            fpSpread2.ActiveSheet.Cells[3, Col].ForeColor = Color.Black;
            fpSpread2.ActiveSheet.Cells[3, Col].Border = LineBorderToHeader;
            Col++;

            //Time
            fpSpread2.ActiveSheet.AddSpanCell(3, Col, 3, 1);
            fpSpread2.ActiveSheet.SetColumnWidth(Col, 300);
            fpSpread2.ActiveSheet.Cells[3, Col].CellType = new FarPoint.Win.Spread.CellType.EditBaseCellType();
            fpSpread2.ActiveSheet.Cells[3, Col].VerticalAlignment = FarPoint.Win.Spread.CellVerticalAlignment.Center;
            fpSpread2.ActiveSheet.Cells[3, Col].HorizontalAlignment = FarPoint.Win.Spread.CellHorizontalAlignment.Center;
            fpSpread2.ActiveSheet.Cells[3, Col].Text = "생산 시간";
            fpSpread2.ActiveSheet.Cells[3, Col].BackColor = Color.WhiteSmoke;
            fpSpread2.ActiveSheet.Cells[3, Col].ForeColor = Color.Black;
            fpSpread2.ActiveSheet.Cells[3, Col].Border = LineBorderToHeader;
            Col++;

            //바코드
            fpSpread2.ActiveSheet.AddSpanCell(3, Col, 3, 1);
            fpSpread2.ActiveSheet.SetColumnWidth(Col, 400);
            fpSpread2.ActiveSheet.Cells[3, Col].CellType = new FarPoint.Win.Spread.CellType.EditBaseCellType();
            fpSpread2.ActiveSheet.Cells[3, Col].VerticalAlignment = FarPoint.Win.Spread.CellVerticalAlignment.Center;
            fpSpread2.ActiveSheet.Cells[3, Col].HorizontalAlignment = FarPoint.Win.Spread.CellHorizontalAlignment.Center;
            fpSpread2.ActiveSheet.Cells[3, Col].Text = "바코드";
            fpSpread2.ActiveSheet.Cells[3, Col].BackColor = Color.WhiteSmoke;
            fpSpread2.ActiveSheet.Cells[3, Col].ForeColor = Color.Black;
            fpSpread2.ActiveSheet.Cells[3, Col].Border = LineBorderToHeader;
            Col++;
            //자동,소동
            fpSpread2.ActiveSheet.AddSpanCell(3, Col, 3, 1);
            fpSpread2.ActiveSheet.SetColumnWidth(Col, 100);
            fpSpread2.ActiveSheet.Cells[3, Col].CellType = new FarPoint.Win.Spread.CellType.EditBaseCellType();
            fpSpread2.ActiveSheet.Cells[3, Col].VerticalAlignment = FarPoint.Win.Spread.CellVerticalAlignment.Center;
            fpSpread2.ActiveSheet.Cells[3, Col].HorizontalAlignment = FarPoint.Win.Spread.CellHorizontalAlignment.Center;
            fpSpread2.ActiveSheet.Cells[3, Col].Text = "자동/수동";
            fpSpread2.ActiveSheet.Cells[3, Col].BackColor = Color.WhiteSmoke;
            fpSpread2.ActiveSheet.Cells[3, Col].ForeColor = Color.Black;
            fpSpread2.ActiveSheet.Cells[3, Col].Border = LineBorderToHeader;
            Col++;

            //판정
            fpSpread2.ActiveSheet.AddSpanCell(3, Col, 3, 1);
            fpSpread2.ActiveSheet.Cells[3, Col].CellType = new FarPoint.Win.Spread.CellType.EditBaseCellType();
            fpSpread2.ActiveSheet.Cells[3, Col].VerticalAlignment = FarPoint.Win.Spread.CellVerticalAlignment.Center;
            fpSpread2.ActiveSheet.Cells[3, Col].HorizontalAlignment = FarPoint.Win.Spread.CellHorizontalAlignment.Center;
            fpSpread2.ActiveSheet.Cells[3, Col].Text = "판정";
            fpSpread2.ActiveSheet.Cells[3, Col].BackColor = Color.WhiteSmoke;
            fpSpread2.ActiveSheet.Cells[3, Col].ForeColor = Color.Black;
            fpSpread2.ActiveSheet.Cells[3, Col].Border = LineBorderToHeader;

            Col++;

            //위크인
            fpSpread2.ActiveSheet.AddSpanCell(3, Col, 1, 3);
            fpSpread2.ActiveSheet.Cells[3, Col].CellType = new FarPoint.Win.Spread.CellType.EditBaseCellType();
            fpSpread2.ActiveSheet.Cells[3, Col].VerticalAlignment = FarPoint.Win.Spread.CellVerticalAlignment.Center;
            fpSpread2.ActiveSheet.Cells[3, Col].HorizontalAlignment = FarPoint.Win.Spread.CellHorizontalAlignment.Center;
            fpSpread2.ActiveSheet.Cells[3, Col].Text = "워크인";
            fpSpread2.ActiveSheet.Cells[3, Col].BackColor = Color.WhiteSmoke;
            fpSpread2.ActiveSheet.Cells[3, Col].ForeColor = Color.Black;
            fpSpread2.ActiveSheet.Cells[3, Col].Border = LineBorderToHeader;

            fpSpread2.ActiveSheet.SetColumnWidth(Col, 100);
            fpSpread2.ActiveSheet.AddSpanCell(4, Col, 2, 1);
            fpSpread2.ActiveSheet.Cells[4, Col].CellType = new FarPoint.Win.Spread.CellType.EditBaseCellType();
            fpSpread2.ActiveSheet.Cells[4, Col].VerticalAlignment = FarPoint.Win.Spread.CellVerticalAlignment.Center;
            fpSpread2.ActiveSheet.Cells[4, Col].HorizontalAlignment = FarPoint.Win.Spread.CellHorizontalAlignment.Center;
            fpSpread2.ActiveSheet.Cells[4, Col].Text = "SLIDE";
            fpSpread2.ActiveSheet.Cells[4, Col].BackColor = Color.WhiteSmoke;
            fpSpread2.ActiveSheet.Cells[4, Col].ForeColor = Color.Black;
            fpSpread2.ActiveSheet.Cells[4, Col].Border = LineBorderToData;

            Col++;
            fpSpread2.ActiveSheet.SetColumnWidth(Col, 100);
            fpSpread2.ActiveSheet.AddSpanCell(4, Col, 2, 1);
            fpSpread2.ActiveSheet.Cells[4, Col].CellType = new FarPoint.Win.Spread.CellType.EditBaseCellType();
            fpSpread2.ActiveSheet.Cells[4, Col].VerticalAlignment = FarPoint.Win.Spread.CellVerticalAlignment.Center;
            fpSpread2.ActiveSheet.Cells[4, Col].HorizontalAlignment = FarPoint.Win.Spread.CellHorizontalAlignment.Center;
            fpSpread2.ActiveSheet.Cells[4, Col].Text = "RECLINE";
            fpSpread2.ActiveSheet.Cells[4, Col].BackColor = Color.WhiteSmoke;
            fpSpread2.ActiveSheet.Cells[4, Col].ForeColor = Color.Black;
            fpSpread2.ActiveSheet.Cells[4, Col].Border = LineBorderToHeader;
            Col++;

            fpSpread2.ActiveSheet.AddSpanCell(4, Col, 2, 1);
            fpSpread2.ActiveSheet.SetColumnWidth(Col, 100);
            fpSpread2.ActiveSheet.Cells[4, Col].CellType = new FarPoint.Win.Spread.CellType.EditBaseCellType();
            fpSpread2.ActiveSheet.Cells[4, Col].VerticalAlignment = FarPoint.Win.Spread.CellVerticalAlignment.Center;
            fpSpread2.ActiveSheet.Cells[4, Col].HorizontalAlignment = FarPoint.Win.Spread.CellHorizontalAlignment.Center;
            fpSpread2.ActiveSheet.Cells[4, Col].Text = "USB";
            fpSpread2.ActiveSheet.Cells[4, Col].BackColor = Color.WhiteSmoke;
            fpSpread2.ActiveSheet.Cells[4, Col].ForeColor = Color.Black;
            fpSpread2.ActiveSheet.Cells[4, Col].Border = LineBorderToHeader;
            Col++;
            
            fpSpread2.ActiveSheet.ColumnCount = Col;
            //Header
            fpSpread2.ActiveSheet.AddSpanCell(0, 0, 1, Col);
            fpSpread2.ActiveSheet.SetRowHeight(0, 100);
            fpSpread2.ActiveSheet.Cells[0, 0].Font = new Font("맑은 고딕", 26);
            fpSpread2.ActiveSheet.SetText(0, 0, "레포트");
            fpSpread2.ActiveSheet.Cells[0, 0].VerticalAlignment = FarPoint.Win.Spread.CellVerticalAlignment.Center;
            fpSpread2.ActiveSheet.Cells[0, 0].HorizontalAlignment = FarPoint.Win.Spread.CellHorizontalAlignment.Center;
            RowCount = 6;

            fpSpread2.ResumeLayout();
            fpSpread2.SaveExcel(dName);
            return;
        }

        private void SaveInfor()
        {
            string Path = Program.INFOR_PATH.ToString() + "\\" + DateTime.Now.ToString("yyyyMMdd") + ".inf";


            TIniFile Ini = new TIniFile(Path);

            Ini.WriteInteger("COUNT", "TOTAL", Infor.TotalCount);
            Ini.WriteInteger("COUNT", "OK", Infor.TotalCount - Infor.NgCount);
            Ini.WriteInteger("COUNT", "NG", Infor.NgCount);

            Ini.WriteString("NAME", "DATA", Infor.DataName);
            Ini.WriteString("NAME", "DATE", Infor.Date);
            Ini.WriteInteger("NAME", "DAY", Infor.Day);
            return;
        }

        private void OpenInfor()
        {
            string Path = Program.INFOR_PATH.ToString() + "\\" + DateTime.Now.ToString("yyyyMMdd") + ".inf";
            string dPath = Program.DATA_PATH.ToString() + "\\" + DateTime.Now.ToString("yyyyMM") + ".xls";

            if (File.Exists(Path) == false)
            {
                Infor.Date = DateTime.Now.ToString("yyyyMMdd");
                Infor.DataName = dPath;
                Infor.TotalCount = 0;
                Infor.OkCount = 0;
                Infor.NgCount = 0;
                Infor.Day = DateTime.Now.Day;
                SaveInfor();
                sevenSegmentInteger1.Value.AsInteger = Infor.TotalCount;
                sevenSegmentInteger2.Value.AsInteger = Infor.OkCount;
                sevenSegmentInteger3.Value.AsInteger = Infor.NgCount;
            }
            else
            {
                TIniFile Ini = new TIniFile(Path);

                if (Ini.ReadInteger("COUNT", "TOTAL", ref Infor.TotalCount) == false) Infor.TotalCount = 0;
                if (Ini.ReadInteger("COUNT", "OK", ref Infor.OkCount) == false) Infor.OkCount = 0;
                if (Ini.ReadInteger("COUNT", "NG", ref Infor.NgCount) == false) Infor.NgCount = 0;

                if (Ini.ReadString("NAME", "DATA", ref Infor.DataName) == false) Infor.DataName = dPath;
                if (Ini.ReadString("NAME", "DATE", ref Infor.Date) == false) Infor.Date = DateTime.Now.ToString("yyyyMMdd");
                if (Ini.ReadInteger("NAME", "DAY", ref Infor.Day) == false) Infor.Day = DateTime.Now.Day;
            }
            

            if (File.Exists(dPath) == false)
            {
                CreateFileName();
            }
            else
            {
                fpSpread2.OpenExcel(dPath);
                fpSpread2.ActiveSheet.Protect = false;

                RowCount = 6;
                for (int i = RowCount; i < fpSpread2.ActiveSheet.RowCount; i++)
                {
                    if (fpSpread2.ActiveSheet.Cells[RowCount, 0].Text == "") break;
                    if (fpSpread2.ActiveSheet.Cells[RowCount, 0].Text == null) break;
                    RowCount++;
                }

                int Col = 0;

                for (int i = 0; i < fpSpread2.ActiveSheet.ColumnCount; i++)
                {
                    if (fpSpread2.ActiveSheet.Cells[4, Col].Text == "HEIGHT")
                        break;
                    else Col++;
                }
                fpSpread2.ActiveSheet.ColumnCount = Col + 1;
            }
            return;
        }

        //private bool JigUpFlag { get; set; }
        //private short JigDownCount { get; set; }

        private void timer2_Tick(object sender, EventArgs e)
        {
            try
            {
                timer2.Enabled = false;
                if (panel12.Visible == false)
                {
                    if (panel2.Visible == false)
                    {
                        if (IOPort.GetAuto == true)
                        {
                            if (toolStrip1.Enabled == true) toolStrip1.Enabled = false;
                        }
                    }

                    if (IOPort.GetAuto == false)
                    {
                        if (toolStrip1.Enabled == false) toolStrip1.Enabled = true;
                        if (led2.Indicator.Text != "수동")
                        {
                            led2.Indicator.Text = "수동";
                            IOPort.SetBatt = true;
                        }

                        if (RunningFlag == false) CheckManualIO();
                    }
                    else
                    {
                        if (led2.Indicator.Text != "자동")
                        {
                            led2.Indicator.Text = "자동";
                            IOPort.SetBatt = false;
                            CheckPopData();
                        }
                    }

                    if (led4.Value.AsBoolean != IOPort.GetProductIn) led4.Value.AsBoolean = IOPort.GetProductIn;
                    if (IOPort.SetProductIn != IOPort.GetProductIn) IOPort.SetProductIn = IOPort.GetProductIn;
                    //if (led9.Value.AsBoolean != JigUpFlag) led9.Value.AsBoolean = JigUpFlag;
                    if (led9.Value.AsBoolean != IOPort.GetJigUp) led9.Value.AsBoolean = IOPort.GetJigUp;
                    if (led6.Value.AsBoolean != IOPort.SetTestOk) led6.Value.AsBoolean = IOPort.SetTestOk;
                    if (led7.Value.AsBoolean != IOPort.SetProductIn) led7.Value.AsBoolean = IOPort.SetProductIn;
                    //if (led8.Value.AsBoolean != IOPort.GetUsb) led8.Value.AsBoolean = IOPort.GetUsb;

                    if (mSpec.UsbCurrent <= pMeter.GetPSeat)
                    {
                        if (led8.Value.AsBoolean == false) led8.Value.AsBoolean = true;
                    }
                    else
                    {
                        if (led8.Value.AsBoolean == true) led8.Value.AsBoolean = false;
                    }
                }
                else
                {
                    DisplaySelfIO();                    
                }

                if (MESCtrl != null)
                {
                    if (SelfForm == null)
                    {
                        {
                            if (MESCtrl.isClientConnection == true)
                            {
                                if (MESCtrl.isReading == true)
                                {
                                    if (RunningFlag == false)
                                    {
                                        PopData = MESCtrl.GetReadData;
                                        CheckPopData();
                                        SaveLogData = "Receive - " + DateTime.Now.ToLongDateString() + " " + DateTime.Now.ToLongTimeString();
                                        SaveLogData = MESCtrl.SourceData;
                                    }
                                    MESCtrl.isReading = false;
                                }                                
                            }
                        }
                    }
                }

                if (panel12.Visible == false)
                {
                    if (IOPort.GetResetSW == true)
                    {
                        if (ResetButtonOn == false)
                        {
                            if (RunningFlag == false)
                            {
                                if (label16.Text != "")
                                {
                                    DisplayStatus(RESULT.CLEAR);
                                    InitScreen();
                                    PopDataCheckFlag = OldPopDataCheckFlag;
                                }
                            }
                            else
                            {
                                StopSetting();
                            }
                        }
                        if (ResetButtonOn == false) ResetButtonOn = true;
                    }
                    else
                    {
                        if (ResetButtonOn == true) ResetButtonOn = false;
                    }

                    if (IOPort.GetStartSW == true)
                    {
                        if (StartButtonOn == false)
                        {
                            if (RunningFlag == false)
                            {
                                if (label16.Text != "")
                                {
                                    IOPort.SetTestOk = true;
                                }
                                else
                                {
                                    if (IOPort.GetAuto == true)
                                    {
                                        if (/*(IOPort.GetJigUp == true) && */(PopDataCheckFlag == true)) StartSetting();
                                    }
                                    else
                                    {
                                        //if (IOPort.GetJigUp == true) 
                                        StartSetting();
                                    }
                                }
                            }
                        }
                    }
                    if (StartButtonOn == false) StartButtonOn = true;
                }
                else
                {
                    if (StartButtonOn == true) StartButtonOn = false;
                }


                if (SelfForm != null)
                {
                    byte[] InData = IOPort.GetInData;

                    for (short i = 0; i < 16; i++)
                    {
                        Iocomp.Instrumentation.Standard.Led Led1 = FindLed("Board1In" + (i + 1).ToString());
                        if (Led1 != null)
                        {
                            bool Value = (InData[i / 8] & (0x01 << (i % 8))) == (0x01 << (i % 8)) ? true : false;
                            if (Led1.Value.AsBoolean != Value) Led1.Value.AsBoolean = Value;
                        }
                    }

                    if (sevenSegmentAnalog1.Value.AsDouble != pMeter.GetBatt) sevenSegmentAnalog1.Value.AsDouble = pMeter.GetBatt;
                    if (sevenSegmentAnalog3.Value.AsDouble != pMeter.GetPSeat) sevenSegmentAnalog3.Value.AsDouble = pMeter.GetPSeat;


                    if (MESCtrl != null)
                    {
                        if (MESCtrl.isClientConnection == true)
                        {
                            if (MESCtrl.isReading == true)
                            {
                                MESCtrl.isReading = false;
                                label35.Text = MESCtrl.SourceData;
                            }
                        }
                    }
                    return;
                }
                else
                {
                    if (sevenSegmentAnalog2.Value.AsDouble != pMeter.GetBatt) sevenSegmentAnalog2.Value.AsDouble = pMeter.GetBatt;
                    if (sevenSegmentAnalog5.Value.AsDouble != pMeter.GetPSeat) sevenSegmentAnalog5.Value.AsDouble = pMeter.GetPSeat;
                }
            }
            catch { }
            finally { timer2.Enabled = !ExitFlag; }
        }

        private void CheckManualIO()
        {
            bool Flag = false;

            if(IOPort.GetWalkIn == true)
            {
                if (CheckItem.Slide == false) Flag = true;
                if (CheckItem.Slide == false) CheckItem.Slide = true;
                if (CheckItem.Recline == false) CheckItem.Recline = true;
                if (CheckItem.Usb == false) CheckItem.Usb = true;
            }
            else
            {
                if (CheckItem.Slide == true) Flag = true;
                if (CheckItem.Slide == true) CheckItem.Slide = false;
                if (CheckItem.Recline == true) CheckItem.Recline = false;
                if (CheckItem.Usb == true) CheckItem.Usb = false;
            }

            if(IOPort.GetLHSellect ==  true)
            {
                if(comboBox3.SelectedItem != null)
                {
                    if (comboBox3.SelectedItem.ToString() != "LH") comboBox3.SelectedItem = "LH";
                }
                else
                {
                    comboBox3.SelectedItem = "LH";
                }
            }
            else
            {
                if (comboBox3.SelectedItem != null)
                {
                    if (comboBox3.SelectedItem.ToString() != "RH") comboBox3.SelectedItem = "RH";
                }
                else
                {
                    comboBox3.SelectedItem = "RH";
                }
            }
            
            if (Flag == true) DisplaySpec();
            return;
        }

        private MES_Control.__ReadMesData__ PopData = new MES_Control.__ReadMesData__()
        {
            Barcode = null,
            Check = null,
            Date = null,
            LineCode = null,
            MachineNo = null
        };


        private void CheckPopData()
        {
            if (PopData.Check == null) return;
            if (IOPort.SetYellowLamp == true) IOPort.SetYellowLamp = false;
            if (IOPort.SetGreenLamp == true) IOPort.SetGreenLamp = false;
            if (IOPort.SetRedLamp == true) IOPort.SetRedLamp = false;
            if (IOPort.SetTestOk == true) IOPort.SetTestOk = false;
            if (panel4.Visible == true) panel4.Visible = false;

            if (IOPort.SetBatt == true) IOPort.SetBatt = false;
            if (IOPort.SetING == true) IOPort.SetING = false;

            DisplayStatus(RESULT.CLEAR);
            InitScreen();

            label18.Text = label17.Text;
            label17.Text = PopData.Barcode;
            label23.Text = PopData.Check;            

            if (label5.Text != "대기중 입니다.")
            {
                label5.Text = "대기중 입니다.";
                InitScreen();
                sevenSegmentInteger4.Value.AsInteger = 0;
                IOPort.SetGreenLamp = false;
                //outportb(IO_OUT.YELLOW, false);
                IOPort.SetYellowLamp = false;
                IOPort.SetRedLamp = false;
            }
            if (label16.Text != "")
            {
                label16.Text = "";
                label16.BackColor = Color.Black;
                label16.ForeColor = Color.Gray;
                RunningFlag = false;
            }

            label16.Text = "";

            if (IOPort.GetAuto == true)
            {
                PopDataCheckFlag = true;
                if (PopData.Check.Substring(0, 1) == "1")
                {
                    CheckItem.Slide = true;
                }
                else
                {
                    CheckItem.Slide = false;
                }
                if (PopData.Check.Substring(1, 1) == "1")
                {
                    CheckItem.Recline = true;
                }
                else
                {
                    CheckItem.Recline = false;
                }

                if (PopData.Check.Substring(2, 1) == "1")
                {
                    CheckItem.Usb = true;
                }
                else
                {
                    CheckItem.Usb = false;
                }


                if (PopData.Check.Substring(16, 1) == "1")
                {
                    if (comboBox3.SelectedItem != null)
                    {
                        if (comboBox3.SelectedItem.ToString() != "RH") comboBox3.SelectedItem = "RH";
                    }
                    else
                    {
                        comboBox3.SelectedItem = "RH";
                    }
                    CheckItem.LHRH = true;
                }
                else
                {
                    if (comboBox3.SelectedItem != null)
                    {
                        if (comboBox3.SelectedItem.ToString() != "LH") comboBox3.SelectedItem = "LH";
                    }
                    else
                    {
                        comboBox3.SelectedItem = "LH";
                    }
                    CheckItem.LHRH = false;
                }

                if (PopData.Check.Substring(15, 1) == "1")
                {
                    if (comboBox2.SelectedItem != null)
                    {
                        if (comboBox2.SelectedItem.ToString() != "RHD") comboBox2.SelectedItem = "RHD";
                    }
                    else
                    {
                        comboBox3.SelectedItem = "RHD";
                    }
                    CheckItem.LHDRHD = true;
                }
                else
                {
                    if (comboBox2.SelectedItem != null)
                    {
                        if (comboBox2.SelectedItem.ToString() != "LHD") comboBox2.SelectedItem = "LHD";
                    }
                    else
                    {
                        comboBox2.SelectedItem = "LHD";
                    }
                }

                DisplaySpec();
                if ((CheckItem.Slide == false) && (CheckItem.Recline == false) && (CheckItem.Usb == false))
                {
                    label5.Text = "검사 사양이 아닙니다.";
                    PopDataCheckFlag = false;
                }
            }
            else
            {
                OldPopDataCheckFlag = true;
            }
            return;
        }

        private Form SelfForm = null;
        private void toolStripButton8_Click(object sender, EventArgs e)
        {
            SelfForm = new Form()
            {
                Owner = this,
                Text = "사용자 점검 모드 입니다.",
                WindowState = FormWindowState.Normal,
                HelpButton = false,
                MaximizeBox = false,
                MinimizeBox = false,
                ControlBox = false,
                FormBorderStyle = FormBorderStyle.FixedSingle,
                Size = new Size(panel12.Width + 10, panel12.Height + 38),
                StartPosition = FormStartPosition.Manual,
                BackColor = Color.WhiteSmoke
            };

            panel12.Visible = true;
            panel12.BringToFront();
            panel12.Parent = SelfForm;
            panel12.Location = new Point(1, 1);
            toolStrip1.Enabled = false;

            EventHandler SelfFormClose = delegate (object sender2, EventArgs e2)
            {
                SelfForm.Close();
            };

            imageButton1.Click += SelfFormClose;

            SelfForm.FormClosing += delegate (object sender1, FormClosingEventArgs e1)
            {
                panel12.Parent = this;
                panel12.Visible = false;
                imageButton1.Click -= SelfFormClose;
                e1.Cancel = false;
                SelfForm.Dispose();
                SelfForm = null;
                IOPort.IOInit();
                toolStrip1.Enabled = true;
            };

            SelfForm.Shown += delegate (object sender3, EventArgs e3)
            {
                SelfForm.Location = new Point
                (
                    this.Left + (this.Width / 2) - (SelfForm.Width / 2),
                    (this.Height / 2) - (SelfForm.Height / 2)
                );
            };

            for (short i = 0; i < 32; i++)
            {
                CheckBox Chk = FindCheckBox("Board1Out" + (i + 1).ToString());
                if (Chk != null) Chk.Checked = IOPort.OutputCheck(i);
            }

            SelfForm.Show();
            return;
        }

        private Control FindCompnent(string name, Control.ControlCollection control)
        {
            Queue<Control.ControlCollection> queue = new Queue<Control.ControlCollection>();
            queue.Enqueue(control);

            while (0 < queue.Count)
            {
                Control.ControlCollection xcontrols = (Control.ControlCollection)queue.Dequeue();

                if (xcontrols == null || xcontrols.Count == 0) continue;

                if (xcontrols.Find(name, true).FirstOrDefault() != null)
                {
                    return xcontrols.Find(name, true).FirstOrDefault();
                }
            }
            return null;
        }

        private Iocomp.Instrumentation.Standard.Led FindLed(string name)
        {
            Control Comp = FindCompnent(name, SelfForm.Controls);

            if (Comp != null)
            {
                Iocomp.Instrumentation.Standard.Led Led = Comp as Iocomp.Instrumentation.Standard.Led;
                return Led;
            }

            return null;
        }

        private CheckBox FindCheckBox(string name)
        {
            Control Comp = FindCompnent(name, SelfForm.Controls);

            if (Comp != null)
            {
                CheckBox cBox = Comp as CheckBox;
                return cBox;
            }

            return null;
        }

     

        private void toolStripButton7_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start("explorer.exe", Program.DATA_PATH.ToString());
            return;
        }

        private void MainForm_Shown(object sender, EventArgs e)
        {
            this.Location = new Point(0, 0);
            this.Size = new Size(1280, this.Height);
            return;
        }


      

        private void toolStripButton1_Click(object sender, EventArgs e)
        {
            OpenFormClose();
            panel1.Visible = true;
            panel2.Visible = false;
            return;
        }

        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            //Infor.Date = DateTime.Now.ToString("yyyyMMdd");
            //Infor.DataName = Path;
            Infor.TotalCount = 0;
            Infor.OkCount = 0;
            Infor.NgCount = 0;
            //Infor.Day = DateTime.Now.Day;
            SaveInfor();
            sevenSegmentInteger1.Value.AsInteger = Infor.TotalCount;
            sevenSegmentInteger2.Value.AsInteger = Infor.OkCount;
            sevenSegmentInteger3.Value.AsInteger = Infor.NgCount;
            return;
        }

        private void switch1_ValueChanged(object sender, EventArgs e)
        {
            var Value = (sender as NationalInstruments.UI.WindowsForms.Switch).Tag;
            bool Status = (sender as NationalInstruments.UI.WindowsForms.Switch).Value;

            int Pos = 0;

            if (int.TryParse((string)Value, out Pos) == false) Pos = 0;

            if (0 < Pos) IOPort.outportb((short)(Pos - 1), Status);

            switch (Pos - 1)
            {
                case 0: led11.Value = Status; break;
                case 1: led12.Value = Status; break;
                case 2: led13.Value = Status; break;
                case 3: led14.Value = Status; break;
                case 4: led15.Value = Status; break;
                case 5: led16.Value = Status; break;
                case 6: led18.Value = Status; break;
                case 7: led19.Value = Status; break;
                case 8: led20.Value = Status; break;
                case 9: led21.Value = Status; break;
                case 10: led22.Value = Status; break;
                case 11: led23.Value = Status; break;
                case 12: led24.Value = Status; break;
                case 13: led25.Value = Status; break;
                case 14: led26.Value = Status; break;
                case 15: led27.Value = Status; break;
            }
            return;
        }

        private void DisplaySelfIO()
        {
            byte[] In = IOPort.GetInData;

            bool Flag;
            for(int i = 0;i < 16;i++)
            {
                if ((byte)(In[i / 8] & (0x01 << (i % 8))) == (byte)(0x01 << (i % 8)))
                    Flag = true;
                else Flag = false;

                switch(i)
                {
                    case 0:
                        if (Board1In1.Value.AsBoolean != Flag) Board1In1.Value.AsBoolean = Flag;
                        break;
                    case 1:
                        if (Board1In2.Value.AsBoolean != Flag) Board1In2.Value.AsBoolean = Flag;
                        break;
                    case 2:
                        if (Board1In3.Value.AsBoolean != Flag) Board1In3.Value.AsBoolean = Flag;
                        break;
                    case 3:
                        if (Board1In4.Value.AsBoolean != Flag) Board1In4.Value.AsBoolean = Flag;
                        break;
                    case 4:
                        if (Board1In5.Value.AsBoolean != Flag) Board1In5.Value.AsBoolean = Flag;
                        break;
                    case 5:
                        if (Board1In6.Value.AsBoolean != Flag) Board1In6.Value.AsBoolean = Flag;
                        break;
                    case 6:
                        if (Board1In7.Value.AsBoolean != Flag) Board1In7.Value.AsBoolean = Flag;
                        break;
                    case 7:
                        if (Board1In8.Value.AsBoolean != Flag) Board1In8.Value.AsBoolean = Flag;
                        break;
                    case 8:
                        if (Board1In9.Value.AsBoolean != Flag) Board1In9.Value.AsBoolean = Flag;
                        break;
                    case 9:
                        if (Board1In10.Value.AsBoolean != Flag) Board1In10.Value.AsBoolean = Flag;
                        break;
                    case 10:
                        if (Board1In11.Value.AsBoolean != Flag) Board1In11.Value.AsBoolean = Flag;
                        break;
                    case 11:
                        if (Board1In12.Value.AsBoolean != Flag) Board1In12.Value.AsBoolean = Flag;
                        break;
                    case 12:
                        if (Board1In13.Value.AsBoolean != Flag) Board1In13.Value.AsBoolean = Flag;
                        break;
                    case 13:
                        if (Board1In14.Value.AsBoolean != Flag) Board1In14.Value.AsBoolean = Flag;
                        break;
                    case 14:
                        if (Board1In15.Value.AsBoolean != Flag) Board1In15.Value.AsBoolean = Flag;
                        break;
                    case 15:
                        if (Board1In16.Value.AsBoolean != Flag) Board1In16.Value.AsBoolean = Flag;
                        break;
                }
                
            }
            return;
        }

     

        private void 시작프로그램등록ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ComF.StartProgramRegistrySet();
            return;
        }

        private void 시작프로그램삭제ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ComF.StartProgramRegistryDel();
            return;
        }

        private void 윈도우재부팅ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ExitFlag = true;
            ComF.timedelay(1000);
#if PROGRAM_RUNNING
            if (IOPort != null) IOPort.CloseIO();
#endif
            ComF.WindowRestartToDelay(0);
            return;
        }

        private void 윈도우종료ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ExitFlag = true;
            ComF.timedelay(1000);
#if PROGRAM_RUNNING
            if (IOPort != null) IOPort.CloseIO();
#endif
            ComF.WindowExit();
            return;
        }
    }


    public class KALMAN_FILETER
    {
        private struct myKalmanFilterType
        {
            public float z_Din;
            public float Q;
            public float R;
            public float A;
            public float B_uk;
            public float H;
            public float x_Predict;
            public float Xk;
            public float p_Predict;
            public float Pk;
            public float K_gain;
        }

        private myKalmanFilterType KalmanStruct = new myKalmanFilterType();

        public KALMAN_FILETER()
        {

        }

        //public myKalmanFilterType[] myKalmanBuff = new myKalmanFilterType[5];
        //public myKalmanFilterType myKalmanBuff = new myKalmanFilterType();

        ~KALMAN_FILETER()
        {

        }

        //public const double Speed = 0.01;
        public const double Speed = 0.15;
        private void init_kalmanFiltering(ref myKalmanFilterType kf)
        {
            //kf->Q = pow(0.01,2);
            kf.Q = (float)Math.Pow(Speed, 2);
            kf.R = (float)Math.Pow(0.5, 2);
            kf.A = 1;
            kf.B_uk = 0;
            kf.H = (float)1.0;
            kf.Xk = 0;     //25
            kf.Pk = (float)1.0;
            kf.K_gain = (float)1.0;
            return;
        }

        private float kalmanFilter(ref myKalmanFilterType kf)
        {

            kf.x_Predict = (kf.A * kf.Xk) + kf.B_uk;

            kf.p_Predict = (kf.A * kf.Pk) + kf.Q;

            kf.K_gain = kf.p_Predict / (kf.H * kf.p_Predict + kf.R);

            kf.Xk = kf.x_Predict + kf.K_gain * (kf.z_Din - kf.x_Predict);

            kf.Pk = (1 - kf.K_gain) * kf.p_Predict;

            return kf.Xk;

        }

        private float kalmanFilter_(float getdata, ref myKalmanFilterType myKalmanBuff)
        {
            myKalmanBuff.z_Din = getdata;

            return kalmanFilter(ref myKalmanBuff);
        }

        //초기화 함수
        private void init_kalmanFilter(ref myKalmanFilterType myKalmanBuff)
        {
            //init_kalmanFiltering(ref myKalmanBuff[0]);
            //init_kalmanFiltering(ref myKalmanBuff[1]);
            //init_kalmanFiltering(ref myKalmanBuff[2]);
            //init_kalmanFiltering(ref myKalmanBuff[3]);
            //init_kalmanFiltering(ref myKalmanBuff[4]);
            init_kalmanFiltering(ref myKalmanBuff);
            return;
        }

        public void Init()
        {
            init_kalmanFilter(ref KalmanStruct);
            return;
        }

        public float CheckData(float Data)
        {
            //float rData = kalmanFilter_(Data, ref KalmanStruct);
            //rData = KalmanStruct.Xk;
            //return rData;
            return Data;
        }
    }
}
