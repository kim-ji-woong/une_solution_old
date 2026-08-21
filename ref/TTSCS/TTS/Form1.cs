using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Microsoft.Speech;
using Microsoft.Speech.Synthesis;
using System.Diagnostics;

namespace TTS
{
    public partial class Form1 : Form
    {
        SpeechSynthesizer ts = new SpeechSynthesizer();
        public delegate void ChangeTextEvent(string text);

        public event ChangeTextEvent OnChangeText;
        public Form1()
        {
            InitializeComponent();
            
            ts.SelectVoice("Microsoft Server Speech Text to Speech Voice (ko-KR, Heami)");
            ts.SetOutputToDefaultAudioDevice();

            ts.SpeakProgress += SpeakProgressEvent;
            ts.StateChanged += StateChanged;
            ts.SpeakCompleted += SpeakEndEvent;
            OnChangeText += ChangeText;
         }
        public void ChangeText(string text)
        {
            label1.Text = text;
        }

        private void SpeakEndEvent(object sender, SpeakCompletedEventArgs e)
        {
            Debug.WriteLine("DDDD");           
        }

        private void SpeakProgressEvent(object sender, SpeakProgressEventArgs e)
        {
            int nCount = e.CharacterCount;
            int nPos = e.CharacterPosition;

            string szText = nPos.ToString() + "%";
            
            object [] param ={szText};
            Invoke(OnChangeText, param);
        }

        private void StateChanged(object sender, StateChangedEventArgs e)
        {
            int i = 0;
            i++;
        }
        private void btnPlay_Click(object sender, EventArgs e)
        {
            int nCount = int.Parse(textBox2.Text);
            //ts.SetOutputToWaveFile("c:\\temp\\test.wav");
            
            DateTime dtNow = DateTime.Now;
            //Debug.WriteLine(dtNow.Ticks);

            for (int i = 0; i < nCount; i++ )
                ts.SpeakAsync(textBox1.Text);
            
            DateTime dtNow2 = DateTime.Now;
            //Debug.WriteLine(dtNow2.Ticks);
                   
        }

        private void button1_Click(object sender, EventArgs e)
        {
            ts.SpeakAsyncCancelAll();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            ts.Pause();            
           
        }

        private void button3_Click(object sender, EventArgs e)
        {
            ts.Resume();
        }
    }
}
