using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace KpxPipeMonitoring.Popups
{
    public partial class AlarmClear : FormBase
    {
        public static string[] occurenceTypeString = 
        {
            "본선작업 중단 및 재개",
            "작업시작 버튼 지연 누름",
            "일시적 압력, 유량 증가",
            "기타"
        };

        public int occurenceType
        {
            get 
            {
                if (radioButton1.Checked)
                    return 0;
                else if (radioButton2.Checked)
                    return 1;
                else if (radioButton3.Checked)
                    return 2;
                else if (radioButton4.Checked)
                    return 3;

                return 0;
            }
        }
        public string comment { get { return textComment.Text; } }

        public AlarmClear(string msg)
        {
            InitializeComponent();

            radioButton1.Text = occurenceTypeString[0];
            radioButton2.Text = occurenceTypeString[1];
            radioButton3.Text = occurenceTypeString[2];
            radioButton4.Text = occurenceTypeString[3];

            labelMsg.Text = msg;
        }

        private void btn_ok_Click(object sender, EventArgs e)
        {
            string comment = textComment.Text;
            if (comment == "")
            {
                UnE.Utility.UMessageBox.Show(MainForm_Tank.Instance, "알람 해결 내용을 입력해 주세요.", "확인", MessageBoxButtons.OK);
                return;
            }

            this.DialogResult = DialogResult.OK;
            OnClose();
        }

        private void btn_cancle_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            OnClose();
        }

        private void radioButton_CheckedChanged(object sender, EventArgs e)
        {
            RadioButton radio = sender as RadioButton;
            if (radio == null)
                return;

            if (radio == radioButton4)
                textComment.Text = "";
            else
                textComment.Text = radio.Text;
        }
    }
}
