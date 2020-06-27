using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace 워크인_1열
{
    public partial class SpecSetting : Form
    {
        private MyInterface mControl = null;
        private __Spec__ mSpec = new __Spec__();
        private string mName = string.Empty;
        private bool ModelBoxChangeFlag = false;
        public SpecSetting()
        {
            InitializeComponent();
        }
        public SpecSetting(MyInterface mControl)
        {
            InitializeComponent();
            this.mControl = mControl;
        }

        private void SpecSetting_Load(object sender, EventArgs e)
        {
            mControl.공용함수.ReadFileListNotExt(Program.SPEC_PATH.ToString(), "*.Spc", COMMON_FUCTION.FileSortMode.FILENAME_ODERBY);
            List<string> FList = mControl.공용함수.GetFileList;

            if (0 < FList.Count)
            {
                ModelBoxChangeFlag = true;
                comboBox1.Items.Clear();
                foreach (string s in FList) comboBox1.Items.Add(s);

                if (0 < comboBox1.Items.Count)
                {
                    if (0 < comboBox1.Items.Count)
                    {
                        if ((mName != null) && (mName != "") && (mName != string.Empty))
                        {
                            if (comboBox1.Items.Contains(mName) == true) comboBox1.SelectedItem = mName;
                        }
                    }

                }
                if (mName != null)
                {
                    string sName = Program.SPEC_PATH.ToString() + "\\" + mName + ".Spc";
                }
            }
            DisplaySpec();
            ModelBoxChangeFlag = false;
            return;
        }

        private void SpecSetting_FormClosing(object sender, FormClosingEventArgs e)
        {
            e.Cancel = false;
            return;
        }

        private void imageButton1_Click(object sender, EventArgs e)
        {
            //저장
            ClearSpec();
            MoveSpec();

            string sName;
            string ModName = null;
            if (comboBox1.SelectedItem == null)
            {
                if (MessageBox.Show("선택된 모델이 없습니다.\n모델을 생성하시겠습니까?", "경고", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.Yes)
                {
                    if (InputBox.Show("모델명 입력", "모델명", ref ModName) == DialogResult.OK)
                    {
                        if ((ModName != null) && (ModName != "") && (ModName != string.Empty))
                        {
                            mName = ModName;
                            sName = Program.SPEC_PATH.ToString() + "\\" + ModName + ".Spc";
                            mSpec.CarName = ModName;
                            mControl.공용함수.SaveSpec(TSpec: mSpec, Name: sName);
                        }
                    }
                }
            }
            else
            {
                ModName = comboBox1.SelectedItem.ToString();
                sName = Program.SPEC_PATH.ToString() + "\\" + comboBox1.SelectedItem.ToString() + ".Spc";
                mSpec.CarName = ModName;
                mControl.공용함수.SaveSpec(TSpec: mSpec, Name: sName);
            }
            return;
        }

        private void MoveSpec()
        {
            if (float.TryParse(fpSpread1.ActiveSheet.Cells[2, 2].Text, out mSpec.SlideSpec) == false) mSpec.SlideSpec = 1;
            if (float.TryParse(fpSpread1.ActiveSheet.Cells[3, 2].Text, out mSpec.ReclineSpec) == false) mSpec.ReclineSpec = 1;
            if (float.TryParse(fpSpread1.ActiveSheet.Cells[4, 2].Text, out mSpec.OffCurrent) == false) mSpec.OffCurrent = 0.4F;
            if (float.TryParse(fpSpread1.ActiveSheet.Cells[5, 2].Text, out mSpec.UsbCurrent) == false) mSpec.UsbCurrent = 0.2F;
            if (float.TryParse(fpSpread1.ActiveSheet.Cells[6, 2].Text, out mSpec.DelayTime) == false) mSpec.DelayTime = 1;
            if (float.TryParse(fpSpread1.ActiveSheet.Cells[7, 2].Text, out mSpec.TestTime) == false) mSpec.TestTime = 1;
            return;
        }

        private void ClearSpec()
        {
            mSpec.CarName = "";
            mSpec.SlideSpec = 0;
            mSpec.ReclineSpec = 0;
            mSpec.TestTime = 0;
            mSpec.DelayTime = 0;
            mSpec.OffCurrent = 0;
            mSpec.UsbCurrent = 0;
            return;
        }

        private void DisplaySpec()
        {
            fpSpread1.ActiveSheet.Cells[2, 2].Text = mSpec.SlideSpec.ToString("0.0");
            fpSpread1.ActiveSheet.Cells[3, 2].Text = mSpec.ReclineSpec.ToString("0.0");
            fpSpread1.ActiveSheet.Cells[4, 2].Text = mSpec.OffCurrent.ToString("0.0");
            fpSpread1.ActiveSheet.Cells[5, 2].Text = mSpec.UsbCurrent.ToString("0.0");
            fpSpread1.ActiveSheet.Cells[6, 2].Text = mSpec.DelayTime.ToString("0.0");
            fpSpread1.ActiveSheet.Cells[7, 2].Text = mSpec.TestTime.ToString("0.0");
            return;
        }

        private void imageButton2_Click(object sender, EventArgs e)
        {
            //생성
            string ModName = null;
            if (InputBox.Show("모델명 입력", "모델명", ref ModName) == DialogResult.OK)
            {
                if ((ModName != null) && (ModName != "") && (ModName != string.Empty))
                {
                    string sName;
                    mName = ModName;
                    sName = Program.SPEC_PATH.ToString() + "\\" + ModName + ".Spc";

                    ModelBoxChangeFlag = true;
                    comboBox1.Items.Add(ModName);

                    ClearSpec();
                    DisplaySpec();
                    mSpec.CarName = mName;

                    mControl.공용함수.SaveSpec(TSpec: mSpec, Name: sName);
                    ModelBoxChangeFlag = false;
                }
            }
            return;
        }

        private void imageButton4_Click(object sender, EventArgs e)
        {
            //다른 이름으로 저장
            ClearSpec();
            MoveSpec();

            string ModName = null;
            if (InputBox.Show("모델명 입력", "모델명", ref ModName) == DialogResult.OK)
            {
                if ((ModName != null) && (ModName != "") && (ModName != string.Empty))
                {
                    string sName;
                    mName = ModName;
                    sName = Program.SPEC_PATH.ToString() + "\\" + ModName + ".Spc";

                    MoveSpec();
                    mSpec.CarName = ModName;
                    mControl.공용함수.SaveSpec(TSpec: mSpec, Name: sName);
                    ModelBoxChangeFlag = true;
                    comboBox1.Items.Add(ModName);
                    comboBox1.SelectedItem = ModName;
                    ModelBoxChangeFlag = false;
                }
            }
            return;
        }

        private void imageButton3_Click(object sender, EventArgs e)
        {
            //삭제
            if (comboBox1.SelectedItem == null) return;
            string sName = Program.SPEC_PATH.ToString() + "\\" + comboBox1.SelectedItem.ToString() + ".Spc";

            if (File.Exists(sName) == true) File.Delete(sName);

            ClearSpec();
            DisplaySpec();
            comboBox1.Items.Remove(comboBox1.SelectedItem);
            return;
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (ModelBoxChangeFlag == true) return;
            string s = comboBox1.SelectedItem.ToString();

            string sName = Program.SPEC_PATH.ToString() + "\\" + s + ".Spc";

            ClearSpec();
            if (File.Exists(sName) == true) mControl.공용함수.OpenSpec(sName, ref mSpec);
            mName = mSpec.CarName;
            DisplaySpec();
            return;
        }
    }
}
