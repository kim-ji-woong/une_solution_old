using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using SpeechLib;

namespace TTS
{
    public partial class Form1 : Form
    {
        private string m_strIniPath = "";
        private SpVoice m_currentVoice = null;

        public Form1()
        {
            InitializeComponent();
            InitConfig();
            tbSpeed_Scroll(null, null);
        }

        private void InitConfig()
        {
            string strFolder = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\SpeechLibSample";

            if (!System.IO.Directory.Exists(strFolder))
                System.IO.Directory.CreateDirectory(strFolder);

            m_strIniPath = strFolder + "\\config.ini";

            if (System.IO.File.Exists(m_strIniPath))
            {
                System.IO.StreamReader reader = new System.IO.StreamReader(m_strIniPath, Encoding.UTF8);
                string strLine = reader.ReadLine().Trim();
                reader.Close();

                textBoxFolder.Text = strLine;
            }
        }

        private void btnRead_Click(object sender, EventArgs e)
        {
            if (textBoxContents.Text.Length == 0)
                return;

            try
            {
                SpVoice voice = new SpVoice();
                voice.Rate = tbSpeed.Value;
                voice.Speak(textBoxContents.Text, SpeechVoiceSpeakFlags.SVSFlagsAsync);
                m_currentVoice = voice;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void btnSaveFile_Click(object sender, EventArgs e)
        {
            if (textBoxContents.Text.Length == 0)
            {
                MessageBox.Show("파일에 저장할 내용이 없습니다.");
                return;
            }
            else if (textBoxFolder.Text.Length == 0)
            {
                MessageBox.Show("파일을 저장할 경로를 지정해 주세요");
                return;
            }

            WriteFolder();
            string strPath = GetSavePath();
            System.Diagnostics.Trace.WriteLine(strPath);

            SpFileStream stream = null;

            try
            {
                stream = new SpFileStream();
                stream.Format.Type = SpeechAudioFormatType.SAFT22kHz16BitStereo;
                stream.Open(GetSavePath(), SpeechStreamFileMode.SSFMCreateForWrite, false);

                SpVoice voice = new SpVoice();
                voice.AudioOutputStream = stream;
                voice.Rate = tbSpeed.Value;
                voice.Speak(textBoxContents.Text, SpeechVoiceSpeakFlags.SVSFlagsAsync);
                voice.WaitUntilDone(System.Threading.Timeout.Infinite);
            }
            catch (Exception ex)
            {
                if (stream != null)
                    stream.Close();

                MessageBox.Show(ex.Message);
                return;
            }


            if (stream != null)
                stream.Close();

            MessageBox.Show("저장 성공");
        }

        private string GetSavePath()
        {
            string strFolder = textBoxFolder.Text.Trim();

            if (strFolder.EndsWith("\\") || strFolder.EndsWith("/"))
                strFolder = strFolder.Remove(strFolder.Length - 1);

            if (radioFileOverwrite.Checked)
                return strFolder + "\\Speech.wav";

            int max = -1, num;
            string[] files = System.IO.Directory.GetFiles(strFolder, "Speech???.wav", System.IO.SearchOption.TopDirectoryOnly);

            foreach (string strFilePath in files)
            {
                int len = strFilePath.Length;
                string strNumber = strFilePath.Substring(len - 7, 3);

                if (int.TryParse(strNumber, out num))
                {
                    if (num > max)
                        max = num;
                }
            }

            string strPath = string.Format("{0}\\Speech{1:000}.wav", strFolder, max + 1);
            return strPath;
        }

        private void tbSpeed_Scroll(object sender, EventArgs e)
        {
            labelSpeed.Text = "속도 " + tbSpeed.Value.ToString();
        }

        private void btnSelectFolder_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog dlg = new FolderBrowserDialog();
            dlg.Description = "음성 파일을 저장할 폴더를 선택하세요.";

            if (dlg.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                textBoxFolder.Text = dlg.SelectedPath;
                WriteFolder();
            }
        }

        private void WriteFolder()
        {
            if (m_strIniPath.Length == 0)
                return;

            System.IO.StreamWriter writer = new System.IO.StreamWriter(m_strIniPath, false, Encoding.UTF8);
            writer.Write(textBoxFolder.Text);
            writer.Close();
        }

        private void btnStop_Click(object sender, EventArgs e)
        {
            if (m_currentVoice != null && m_currentVoice.Status.RunningState != SpeechRunState.SRSEDone)
            {
                m_currentVoice.Skip("sentence", 1000000);
                m_currentVoice = null;
            }
        }
    }
}
