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
                UnE.Control.CCTVCtrl.InitializeApp();

                int nSiteID = 2, nCCTVID = -1, nPositionIndex = -1;
                string szTitle = "";
                IntPtr hWnd = IntPtr.Zero;
                bool EquipZoneCCTVMode = false;
                int EquipZoneID = -1;
                

                if (ParseArguments(args, ref nSiteID, ref nCCTVID, ref szTitle, ref hWnd, ref nPositionIndex, ref EquipZoneCCTVMode,ref EquipZoneID))
                {
                    CCTVMessageFilter m_msgFilter = new CCTVMessageFilter();                    

                    UnE.CCTV.CCTVLoader loader = new UnE.CCTV.CCTVLoader(nSiteID);
                    UnE.CCTV.CCTV cctv = loader.LoadCCTV(nCCTVID);
                    string strDefaultPreset = loader.GetDefaultPreset(EquipZoneID, nPositionIndex);                                        

                    if (cctv != null && hWnd != IntPtr.Zero)
                    {
                        Form form = LoadForm(cctv, hWnd, nPositionIndex, EquipZoneCCTVMode, EquipZoneID, strDefaultPreset);
                        //UnE.CCTV.BigCCTVCtrl form = new UnE.CCTV.BigCCTVCtrl(cctv, hWnd, nPositionIndex);
                        form.Text = szTitle;
                        form.TopLevel = false;

                        SetParent(form.Handle, hWnd);

                        //if (form is UnE.CCTV.BigCCTVCtrl)
                        {
                            ((UnE.CCTV.BigCCTVCtrlOwner)form).SetParentHandle(hWnd);
                        }
                        /*else if (form is UnE.CCTV.ITXNvrCtrl)
                        {
                            ((UnE.CCTV.ITXNvrCtrl)form).SetParentHandle(hWnd);
                        }
                        else if (form is UnE.CCTV.IdisNvrCtrl)
                        {
                            ((UnE.CCTV.IdisNvrCtrl)form).SetParentHandle(hWnd);
                        }*/

                        //form.FormBorderStyle = FormBorderStyle.Sizable;
                        form.Show();

                        //System.Threading.Thread.Sleep(1000);
                        
                        //form.Connect();

                        m_msgFilter.hWnd = form.Handle;
                        Application.AddMessageFilter(m_msgFilter);

                        Application.Run(form);
                    }
                    else if (hWnd == IntPtr.Zero)
                    {
                        Form form = LoadForm(cctv, hWnd, nPositionIndex, EquipZoneCCTVMode, EquipZoneID, strDefaultPreset);
                        m_msgFilter.hWnd = form.Handle;
                        Application.AddMessageFilter(m_msgFilter);

                        Application.Run(form);
                        //Application.Run(new UnE.CCTV.BigCCTVCtrl(cctv, hWnd, nPositionIndex));
                    }
                    else
                    {
                        if( cctv != null)
                        {
                            Form form = LoadForm(cctv, hWnd, nPositionIndex, EquipZoneCCTVMode, EquipZoneID, strDefaultPreset);
                            //UnE.CCTV.BigCCTVCtrl form = new UnE.CCTV.BigCCTVCtrl(cctv, hWnd, nPositionIndex);
                            m_msgFilter.hWnd = form.Handle;
                            Application.AddMessageFilter(m_msgFilter);
                            
                            Application.Run(form);
                         
                        }
                        
                    }
                }

                UnE.Control.CCTVCtrl.FinalizeApp();
            }
            catch(Exception ex)
            {
                System.Diagnostics.Trace.WriteLine(ex.Message);
                //MessageBox.Show(ex.Message);
                //MessageBox.Show(ex.StackTrace);              
            }
        }

        static Form LoadForm(UnE.CCTV.CCTV cctv, IntPtr hWnd, int nPositionIndex, bool nEquipZoneCCTVMode, int nEquipZoneID,string nDefaultPReset)
        {
            Form frm = new UnE.CCTV.BigCCTVCtrlOwner(cctv, hWnd, nPositionIndex, nEquipZoneCCTVMode, nEquipZoneID, nDefaultPReset);
            return frm;
        }

        /*private static int nSiteID = 2;
        private static int nCCTVID = -1;
        private static string szTitle = "";
        private static IntPtr hWnd; */

        static bool ParseArguments(string[] args, ref int nSiteID, ref int nCCTVID, ref string szTitle, ref IntPtr hWnd, ref int nPositionIndex, ref bool nEquipZoneCCTVMode, ref int nEquipZoneID)
        {
            if (args.Length < 5)
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

                if (int.TryParse(args[4], out nPositionIndex) == false)
                    return false;

                if (args.Length > 5)
                {
                    try
                    {
                        bool.TryParse(args[5], out nEquipZoneCCTVMode);
                    }
                    catch 
                    {
                        nEquipZoneCCTVMode = false;
                    }
                }

                if(args.Length > 6)
                {
                    try
                    {
                        nEquipZoneID = int.Parse(args[6]);
                    }
                    catch 
                    { 
                        nEquipZoneID = -1; 
                    }
                }
                
            }catch(Exception ex)
            {
                //MessageBox.Show(ex.Message);
                //MessageBox.Show(ex.StackTrace);  
            }            
            return true;
        }
    }

    public class CCTVMessageFilter : IMessageFilter
    {
        [DllImport("user32.dll")]
        static extern IntPtr SendMessage(IntPtr hWnd, int Msg, IntPtr wParam, IntPtr lParam);

        private static int WM_LBUTTONDOWN = 0x201; //Left mousebutton down
        private static int WM_LBUTTONUP = 0x202; //Left mousebutton up
        private static int WM_LBUTTONDBLCLK = 0x203; //Left mousebutton doubleclick
        private static int WM_RBUTTONDOWN = 0x204; //Right mousebutton down
        private static int WM_RBUTTONUP = 0x205;  //Right mousebutton up
        private static int WM_RBUTTONDBLCLK = 0x206; //Right mousebutton doubleclick
        private static int WM_KEYDOWN = 0x0100;
        private static int WM_CHAR = 0x0102;

        public enum VKeys : int
        {
            VK_DELETE = 0x2E,  //DEL key
        }

        public IntPtr hWnd;

        public bool PreFilterMessage(ref Message msg)
        {
            if (msg.Msg == 512)//WM_MOUSEMOVE
            {
            }
            else if (msg.Msg == 513)//WM_LBUTTONDOWN
            {
            }
            else if (msg.Msg == 514)//WM_LBUTTONUP
            {
            }
            else if (msg.Msg == 516)//WM_RBUTTONDOWN
            {
                SendMessage(hWnd, WM_RBUTTONDOWN, msg.WParam, msg.LParam);
            }
            else if (msg.Msg == 517)//WM_RBUTTONUP
            {
                SendMessage(hWnd, WM_RBUTTONUP, msg.WParam, msg.LParam);
            }
            else if (msg.Msg == WM_KEYDOWN)
            {
                if ((int)msg.WParam == (int)VKeys.VK_DELETE)
                {
                    SendMessage(hWnd, WM_KEYDOWN, (IntPtr)VKeys.VK_DELETE, IntPtr.Zero);
                }
            }

            return false;
        }
    }


}
