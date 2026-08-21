using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;

namespace StringMix
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private string MixString(string strText1, string strText2, int nCount1, int nCount2)
        {
            StringBuilder str1 = new StringBuilder();


            //원래 문자열의 길이를 저장함
            int nLength1 = strText1.Length;
            int nLength2 = strText2.Length;

            Random r = new Random();

            char[] arRandom = {'a','b','c','d','e','f','g','f','i','j','k','l','m','n','o','p','q','r','s','t','u','v','w','x','y','z'
                     ,'A','B','C','D','E','F','G','F','I','J','K','L','M','N','O','P','Q','R','S','T','U','V','W','X','Y','Z'
                     ,'1','2','3','4','5','6','7','8','9','!','@','#','$','%','&'};

            //string strRandomString = arRandom[r.Next(arRandom.Length)].ToString();
            //string strRandomString = "X";

            int nPadding = 0;


            //1, 글자 수가 배수에 맞는지 검사
            if (strText1.Length % nCount1 != 0)
            {
                nPadding = nCount1 - (strText1.Length % nCount1);
                for (int i = 0; i < nPadding; i++)
                {
                    strText1 += arRandom[r.Next(arRandom.Length)].ToString(); ;
                }
            }

            if (strText2.Length % nCount2 != 0)
            {
                nPadding = nCount2 - (strText2.Length % nCount2);
                for (int i = 0; i < nPadding; i++)
                {
                    strText2 += arRandom[r.Next(arRandom.Length)].ToString(); ;
                }
            }

            int nQuota = strText1.Length / nCount1;
            int nQuota2 = strText2.Length / nCount2;

            int nShareCount = 0;
            if (nQuota != nQuota2)
            {
                if (nQuota > nQuota2)
                {
                    int nLength = strText2.Length * nQuota;
                    int nAddLength = nLength - strText2.Length;

                    for (int i = 0; i < nAddLength; i++)
                    {
                        strText2 += arRandom[r.Next(arRandom.Length)].ToString(); ;
                    }
                    nShareCount = nQuota;
                }
                if (nQuota < nQuota2)
                {
                    int nLength = strText1.Length * nQuota2;
                    int nAddLength = nLength - strText1.Length;

                    for (int i = 0; i < nAddLength; i++)
                    {
                        strText1 += arRandom[r.Next(arRandom.Length)].ToString(); ;
                    }
                    nShareCount = nQuota2;
                }
            }
            else
            {
                nShareCount = nQuota;
            }

            char[] n1 = strText1.ToCharArray(0, strText1.Length);
            char[] n2 = strText2.ToCharArray(0, strText2.Length);

            int nTemp = 0;
            int nTemp2 = 0;
            //3. 문자열 섞기
            for (int i = 0; i < nShareCount; i++)
            {
                for (int k = nTemp; k < nTemp + nCount1; k++)
                {
                    str1.Append(n1[k].ToString());
                }

                for (int j = nTemp2; j < nTemp2 + nCount2; j++)
                {
                    str1.Append(n2[j].ToString());
                }

                nTemp += nCount1;
                nTemp2 += nCount2;
            }


            //원래 문자열의 길이, n:m 저장.
            if (label4.Text.Length == 0)
                label4.Text = nLength1.ToString() + "," + nLength2.ToString() + "," + nCount1.ToString() + "," + nCount2.ToString();
            else
                label4.Text += "," + nLength1.ToString() + "," + nLength2.ToString() + "," + nCount1.ToString() + "," + nCount2.ToString();

            return str1.ToString();
        }

        private void DivisionString(string strMixText, out string strText1, out string strText2, int n, int m)
        {
            strText1 = "";
            strText2 = "";

            string[] ar = new string[strMixText.Length / (n + m)];

            int nTemp = 0;
            for (int i = 0; i < ar.Length; i++)
            {
                string strTemp = strMixText.Substring(nTemp, n + m);

                strText1 += strTemp.Substring(0, n);
                strText2 += strTemp.Substring(n, m);

                nTemp = (i + 1) * (n + m);
            }
        }

        private string CompareString(string strText1, string strText2)
        {
            int nLongLenght = 0;
            int nShortLenght = 0;


            string strText = "";

            int nCount1 = Convert.ToInt32(textBox4.Text);
            int nCount2 = Convert.ToInt32(textBox5.Text);

            if (strText1.Length >= strText2.Length)
            {
                nLongLenght = strText1.Length;
                nShortLenght = strText2.Length;

                strText = MixString(strText1, strText2, nCount1, nCount2);
            }
            else if (strText2.Length >= strText1.Length)
            {
                nLongLenght = strText2.Length;
                nShortLenght = strText1.Length;

                strText = MixString(strText2, strText1, nCount1, nCount2);
            }

            return strText;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (textBox1.Text == "")
            {
                return;
            }
            if (textBox2.Text == "")
            {
                return;
            }
            if (textBox3.Text == "")
            {
                return;
            }
            if (textBox4.Text == "")
            {
                return;
            }
            if (textBox5.Text == "")
            {
                return;
            }

            string strText1 = textBox1.Text;
            string strText2 = textBox2.Text;
            string strText3 = textBox3.Text;

            string strID = MixString(strText1);
            string strPwd = MixString(strText2);
            string strCode = MixString(strText3);

            label1.Text = "";
            label4.Text = "";

            int nCount1 = Convert.ToInt32(textBox4.Text);
            int nCount2 = Convert.ToInt32(textBox5.Text);

            string strMixText = MixString(strID, strPwd, nCount1, nCount2);

            string strMixText2 = MixString(strMixText, strCode, nCount1, nCount2);


            label1.Text = strMixText2;
            button1.Text = strMixText;

            StreamWriter sw = new StreamWriter(@"C:\PreSafeTemp\b.txt");
            sw.WriteLine(strMixText2 + "," + label4.Text);
            sw.Close();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            string strText1 = "";
            string strText2 = "";



            //int nLength1 = Convert.ToInt32(str[0]);
            //int nLength2 = Convert.ToInt32(str[1]);
            //int nCount1 = Convert.ToInt32(str[2]);
            //int nCount2 = Convert.ToInt32(str[3]);
            //int nLength3 = Convert.ToInt32(str[4]);
            //int nLength4 = Convert.ToInt32(str[5]);

            string strMixString = "";
            int nLength1 = 0;
            int nLength2 = 0;
            int nCount1 = 0;
            int nCount2 = 0;
            int nLength3 = 0;
            int nLength4 = 0;

            StreamReader sr = new StreamReader(@"C:\PreSafeTemp\b.txt");

            while (sr.Peek() >= 0)
            {
                string[] str = sr.ReadLine().ToString().Split(new char[] { ',' });

                strMixString = str[0].ToString();
                nLength1 = Convert.ToInt32(str[1]);
                nLength2 = Convert.ToInt32(str[2]);
                nCount1 = Convert.ToInt32(str[3]);
                nCount2 = Convert.ToInt32(str[4]);
                nLength3 = Convert.ToInt32(str[5]);
                nLength4 = Convert.ToInt32(str[6]);
                //ar[n] = Convert.ToInt32(sr.ReadLine().ToString());
            }
            sr.Close();


            //마지막에 섞었던 문자부터 분리시킴
            //섞인 문자, 분해한 문자1, 분해한 문자2, n(n:m), m(n:m)
            DivisionString(strMixString, out strText1, out strText2, nCount1, nCount2);


            strText1 = strText1.Substring(0, nLength3);
            strText2 = strText2.Substring(0, nLength4);

            string strID = "";
            string strPwd = "";
            string strCode = "";
            string strIDPwd = "";

            strIDPwd = strText1;

            char[] c = strText2.ToCharArray();

            //원래 문자열을 찾는다.
            for (int i = 0; i < c.Length; i += 2)
            {
                strCode += c[i].ToString();
            }
            label5.Text = strCode;

            DivisionString(strIDPwd, out strText1, out strText2, nCount1, nCount2);

            strText1 = strText1.Substring(0, nLength1);
            strText2 = strText2.Substring(0, nLength2);


            char[] c2 = strText1.ToCharArray();

            //원래 문자열을 찾는다.
            for (int i = 0; i < c2.Length; i += 2)
            {
                strID += c2[i].ToString();
            } 

            char[] c3 = strText2.ToCharArray();

            //원래 문자열을 찾는다.
            for (int i = 0; i < c3.Length; i += 2)
            {
                strPwd += c3[i].ToString();
            }

            label2.Text = strID;
            label3.Text = strPwd;

            //원래 문자 길이로 제대로 된 문자열 구함.

            //if (strLongText.GetHashCode() == nOriHash1)
            //{
            //    label2.Text = strLongText;
            //    label3.Text = strShortText;
            //}
            //else if (strShortText.GetHashCode() == nOriHash1)
            //{
            //    label2.Text = strShortText;
            //    label3.Text = strLongText;
            //}

        }

        private string MixString(string strText)
        {
            Random r = new Random();

            char[] arRandom = {'a','b','c','d','e','f','g','f','i','j','k','l','m','n','o','p','q','r','s','t','u','v','w','x','y','z'
                     ,'A','B','C','D','E','F','G','F','I','J','K','L','M','N','O','P','Q','R','S','T','U','V','W','X','Y','Z'
                     ,'1','2','3','4','5','6','7','8','9','!','@','#','$','%','&'};

            int nTextLength = strText.Length;
            char[] c = strText.ToCharArray();

            char[] arrChar = new char[nTextLength * 2];
            int[] arrIndex = new int[nTextLength];

            int nCount = 0;
            for (int i = 0; i < nTextLength * 2; i += 2)
            {
                arrChar[i] = c[nCount];
                arrChar[i+1] = arRandom[r.Next(arRandom.Length)];
                arrIndex[nCount] = i;
                nCount++;
            }

            string strMixString = "";
            for (int i = 0; i < arrChar.Length; i++ )
            {
                strMixString += arrChar[i];
            }

            return strMixString;
        }
    }
}
