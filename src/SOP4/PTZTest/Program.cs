using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Runtime.InteropServices;


namespace CCTVViewer
{
    static class Program
    {
        [DllImport("user32.dll")]
        public static extern IntPtr SetParent(IntPtr hWndChild, IntPtr hWndNewParent);

        [STAThread]
        static void Main(string[] args)
        {
            //Application.EnableVisualStyles();
            //Application.SetCompatibleTextRenderingDefault(false);

            try
            {
                //5052464 dd 1 21
                nSiteID = 1;
                nCCTVID = 25;
                hWnd = new IntPtr(5052464);

                //if (ParseArguments(args))
                {
                    UnE.CCTV.CCTVLoader loader = new UnE.CCTV.CCTVLoader(nSiteID);
                    UnE.CCTV.CCTV cctv = loader.LoadCCTV(nCCTVID);

                    if (cctv != null && hWnd != IntPtr.Zero)
                    {

                        //MessageBox.Show("DDD");
                        UnE.CCTV.BigCCTVCtrl form = new UnE.CCTV.BigCCTVCtrl(cctv, hWnd);
                        form.CCTVLoader = loader;
                        form.Text = szTitle;
                        //form.TopLevel = false;

                        //SetParent(form.Handle, hWnd);
                       // form.SetParentHandle(hWnd);

                        //form.FormBorderStyle = FormBorderStyle.Sizable;
                        //form.Show();

                        //System.Threading.Thread.Sleep(1000);
                        
                        //form.Connect();
                        Application.Run(new TestForm(form));
                    }
                    else
                    {
                        MessageBox.Show("NULL");
                    }
                }    
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message);
                MessageBox.Show(ex.StackTrace);              
            }
        }

        private static int nSiteID = 2;
        private static int nCCTVID = -1;
        private static string szTitle = "";
        private static IntPtr hWnd; 

        static bool ParseArguments(string[] args)
        {
            if (args.Length < 3)
                return false;

            try
            {                
                szTitle = args[1];

                string szWind = args[0];

                int nHWnd = int.Parse(szWind);
                hWnd = new IntPtr(nHWnd);

                string szTemp = args[2];
                nSiteID = int.Parse(szTemp);

                string szTemp2 = args[3];
                nCCTVID = int.Parse(szTemp2);
                
            }catch(Exception ex)
            {
                //MessageBox.Show(ex.Message);
                //MessageBox.Show(ex.StackTrace);  
            }            
            return true;
        }
    }
}
