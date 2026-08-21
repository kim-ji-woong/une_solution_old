using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Timers;
using System.Reflection;
using System.IO;

namespace libTTS
{
    public class Broadcast : IDisposable
    {
        private LocalTTS mTtsManager = null;
        private bool bProcess = false;
       
        public Broadcast(string serverIP, string serverPort)
        { 
            if( mTtsManager == null)
            {
                mTtsManager = new LocalTTS();
                mTtsManager.ConnectServer(serverIP, serverPort);
            } 
        }

        public void Dispose()
        {
            mTtsManager.Dispose();
        }


        public void AddSpeech(string szMsg, int nCount, bool bUseSiren)
        {
            if( mTtsManager != null)
                mTtsManager.AddSpeech(szMsg, nCount, bUseSiren);
        }


	}
}
