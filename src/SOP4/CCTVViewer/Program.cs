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
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            try
            {
                int nSiteID = 2, nCCTVID = -1;
                string szTitle = "";
                IntPtr hWnd = IntPtr.Zero;

                if (ParseArguments(args, ref nSiteID, ref nCCTVID, ref szTitle, ref hWnd))
                {
                    UnE.CCTV.CCTVLoader loader = new UnE.CCTV.CCTVLoader(nSiteID);
                    UnE.CCTV.CCTV cctv = loader.LoadCCTV(nCCTVID);
                    if (cctv != null && hWnd != IntPtr.Zero)
                    {
                        UnE.CCTV.BigCCTVCtrl form = new UnE.CCTV.BigCCTVCtrl(cctv, hWnd);
                        form.Text = szTitle;
                        form.TopLevel = false;

                        SetParent(form.Handle, hWnd);
                        form.SetParentHandle(hWnd);

                        //form.FormBorderStyle = FormBorderStyle.Sizable;
                        form.Show();

                        //System.Threading.Thread.Sleep(1000);
                        
                        //form.Connect();
                        Application.Run(form);
                    }
                    else if (hWnd == IntPtr.Zero)
                    {
                        Application.Run(new UnE.CCTV.BigCCTVCtrl(cctv, hWnd));
                    }
                    else
                    {
                        if( cctv != null)
                        {

                            UnE.CCTV.BigCCTVCtrl form = new UnE.CCTV.BigCCTVCtrl(cctv, hWnd);
                            Application.Run(form);
                         
                        }
                        
                    }
                }    
            }
            catch(Exception ex)
            {
                //MessageBox.Show(ex.Message);
                //MessageBox.Show(ex.StackTrace);              
            }
        }

        /*private static int nSiteID = 2;
        private static int nCCTVID = -1;
        private static string szTitle = "";
        private static IntPtr hWnd; */

        static bool ParseArguments(string[] args, ref int nSiteID, ref int nCCTVID, ref string szTitle, ref IntPtr hWnd)
        {
            if (args.Length < 4)
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
