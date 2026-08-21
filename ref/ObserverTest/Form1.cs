using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ObserverTest
{

    public class MyValues : ICloneable
    {
        private string mValue = "";
        public string Value
        {
            get { return mValue; }
            set
            {
                mValue = value;
            }
        }

        public object Clone()
        {
            return this.MemberwiseClone();
        }
    }

    class MySubjectAdapter : UnE.Event.ValueSubjectAdapter<string>
    {
        public override int CompareTo(string target, string compare)
        {
            //if (target.Value == compare.Value)
            //    return 0;
            //else
            //    return 1;
            return Comparer<string>.Default.Compare(target, compare);
        }
    }

    public partial class Form1 : Form, UnE.Event.IValueObserver<string>
    {

        MySubjectAdapter mSubject = new MySubjectAdapter();

        private MyValues mValue = new MyValues();
        public string Value
        {
            get { return mValue.Value; }
            set
            {
                mValue.Value = value;
                mSubject.UpdateValueChanged(value);
            }
        }

        public Form1()
        {
            InitializeComponent();

            mSubject.Async = true;
            mSubject.TargetValue = mValue.Value;
            mSubject.AddObserver(this);
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            mSubject.RemoveObserver(this);
            mSubject.Dispose();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Value = textBox1.Text;            
        }


        public void OnSubscribed(object target)
        {
            int i = 0;
            i++;
        }

        public void OnUnsubscribed(object target)
        {

        }

        public void OnValueChanged(object sender, string value)
        {
            System.Diagnostics.Trace.WriteLine("ChangeValue : " + value);

            if (mSubject.Async == true)
            {
                this.Invoke((MethodInvoker)delegate
                {
                    textBox1.Text = "";
                });
            }
            else
            {
                textBox1.Text = "";
            }
        }

        
    }
}
