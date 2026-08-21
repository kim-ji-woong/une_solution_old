using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace UnE.Util
{
    internal class CommandProcessor
    {
        // Unity에서 보내오는 명령어 세트 -> SendMessage에서 사용되는 목록, 함수 형태
        // Unity에서 정의되는 Send함수를 그대로 사용하는것이 log비교가 수월함. 
        private string[] cmdList = { "Pick", "SelectObject", "UnSelectObject", "OnMouseRightDown", "OnMouseRightUp", // 5
                                       "OnMouseLeftDown", "OnMouseLeftUp", "OnMouseMiddleDown", "OnMouseMiddleUp", "EnterObject", // 10
                                       "LeaveObject",  "3DPosition" , "GetLastID", "ReadyToRead"  , "MainCameraPoisition",  // 15
                                       "MainCameraAngles", "MainCameraDirection"                               
                                   };
      

        private static CommandProcessor m_Instance = null;
        public static CommandProcessor Instance
        {
            get 
            { 
                if( m_Instance == null)
                {
                    m_Instance = new CommandProcessor();
                }
                return CommandProcessor.m_Instance; 
            }
        }
        
        private CommandProcessor()
        {
        }        

        /// <summary>
        /// Unity에 등록되었지만 처리되지않는 무인자 Command에 대해 등록및 호출 처리를 수행 (디버그 및 개발자용)
        /// </summary>
        /// <param name="szCommand"></param>
        /// <param name="action"></param>
        public void RegisterCommand(string szCommand, Action action)
        {
        }

        /// <summary>
        /// 함수형태의 Unity의 명령어에서 인사를 string[] 로 생성하는 함수
        /// </summary>
        /// <param name="szInput">Unity에서 입력값</param>
        /// <param name="szCmd">처리할 Command</param>
        /// <returns>인자 리스트</returns>
        private string[] parseArgument(string szInput, string szCmd)
        {
            string szTemp = szInput.Replace(szCmd, "");
            szTemp = szTemp.Replace("(", "");
            szTemp = szTemp.Replace(")", "");

            string [] szArgList = szTemp.Split(new char[]{','}, StringSplitOptions.RemoveEmptyEntries);
            return szArgList;
        }
        
        /// <summary>
        /// Unity에서 받은 Command를 처리하는 함수
        /// 명령어에 따라 적절한 Panel4Unity의 함수를 호출하여 준다.
        /// </summary>
        /// <param name="szInputCmd"></param>
        /// <param name="callback"></param>
        public void ProcessCommand(string szInputCmd, Panel4Unity callback)
        {
            UnE.Util.Panel4Unity form = (UnE.Util.Panel4Unity)callback;
            if (form == null)
                return;

            if (szInputCmd.StartsWith(cmdList[0]))
            {
            }
            else if(szInputCmd.StartsWith(cmdList[1]))
            {
                string[] args = parseArgument(szInputCmd, cmdList[1]);
                string szValue = args[0].Replace("'", "");
                szValue = szValue.Replace("\"", "");
                System.Diagnostics.Trace.WriteLine("Select Object : " + szValue);
            }
            else if (szInputCmd.StartsWith(cmdList[2]))
            {
                string[] args = parseArgument(szInputCmd, cmdList[2]);
                string szValue = args[0].Replace("'", "");
                szValue = szValue.Replace("\"", "");
                System.Diagnostics.Trace.WriteLine("Unselect Object : " + szValue);
            }
            else if (szInputCmd.StartsWith(cmdList[3]))
            {
                string[] args = parseArgument(szInputCmd, cmdList[3]);
                float nX = 0.0f;
                float nY = 0.0f;
                float.TryParse(args[0], out nX);
                float.TryParse(args[1], out nY);                
                form.OnPostRightMouseDown((int)nX, (int)nY);
            }
            else if (szInputCmd.StartsWith(cmdList[4]))
            {
                string[] args = parseArgument(szInputCmd, cmdList[4]);
                float nX = 0.0f;
                float nY = 0.0f;
                float.TryParse(args[0], out nX);
                float.TryParse(args[1], out nY);                
                form.OnPostRightMouseUp((int)nX, (int)nY);
            }
            else if (szInputCmd.StartsWith(cmdList[5]))
            {
                string[] args = parseArgument(szInputCmd, cmdList[5]);

                float nX = 0.0f;
                float nY = 0.0f;
                float.TryParse(args[0], out nX);
                float.TryParse(args[1], out nY);               
                form.OnPostLeftMouseDown((int)nX, (int)nY);
            }
            else if (szInputCmd.StartsWith(cmdList[6]))
            {
                string[] args = parseArgument(szInputCmd, cmdList[6]);

                float nX = 0.0f;
                float nY = 0.0f;
                float.TryParse(args[0], out nX);
                float.TryParse(args[1], out nY);                
                form.OnPostLeftMouseUp((int)nX, (int)nY);
            }
            else if (szInputCmd.StartsWith(cmdList[10]))
            {
                string[] args = parseArgument(szInputCmd, cmdList[10]);
                string szValue = args[0].Replace("'", "");
                szValue = szValue.Replace("\"", "");
                form.SetLeaveObject(szValue);
            }
            else if (szInputCmd.StartsWith(cmdList[11]))
            {
                string[] args = parseArgument(szInputCmd, cmdList[11]);

                float nX = 0.0f;
                float nY = 0.0f;
                float nZ = 0.0f;
                float.TryParse(args[0], out nX);
                float.TryParse(args[1], out nY);
                float.TryParse(args[2], out nZ);                
                form.OnPoistionPick(nX, nY, nZ);
            }
            else if (szInputCmd.StartsWith(cmdList[12]))
            {
                string[] args = parseArgument(szInputCmd, cmdList[12]);

                int nID = -1;
                int.TryParse(args[0], out nID);
                form.OnReciveLastID(nID);

                System.Diagnostics.Trace.WriteLine("GetLastID : " + nID);
            }

            else if (szInputCmd.StartsWith(cmdList[13]))
            {
                string[] args = parseArgument(szInputCmd, cmdList[13]);
                form.OnReadyToSend();
            }
            else if (szInputCmd.StartsWith(cmdList[14]))
            {
                string[] args = parseArgument(szInputCmd, cmdList[14]);
                float nX = 0.0f;
                float nY = 0.0f;
                float nZ = 0.0f;
                float.TryParse(args[0], out nX);
                float.TryParse(args[1], out nY);
                float.TryParse(args[2], out nZ);
                form.OnReciveCameraPosition(nX, nY, nZ);
            }
            else if (szInputCmd.StartsWith(cmdList[15]))
            {
                string[] args = parseArgument(szInputCmd, cmdList[15]);
                float nX = 0.0f;
                float nY = 0.0f;
                float nZ = 0.0f;
                float.TryParse(args[0], out nX);
                float.TryParse(args[1], out nY);
                float.TryParse(args[2], out nZ);
                form.OnReciveCameraOrientaion(nX, nY, nZ);
            }
            else if (szInputCmd.StartsWith(cmdList[16]))
            {
                string[] args = parseArgument(szInputCmd, cmdList[16]);
                float nX = 0.0f;
                float nY = 0.0f;
                float nZ = 0.0f;
                float.TryParse(args[0], out nX);
                float.TryParse(args[1], out nY);
                float.TryParse(args[2], out nZ);
                form.OnReciveCameraDirection(nX, nY, nZ);
            }
        }
    }
}
