using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DBUtility2;

namespace libTTS
{
    public interface ITTSManager
    {
        SpeechState State
        {
            get;
        }

        void Initialize();
        void CheckRequest(WebDBManager dbMgr);
        void AddSpeech(WebDBManager dbMgr, BroadcastMessage message);
        void StopSpeech();
        void PauseSpeech();
        void ResumeSpeech();
        // Siren 음원
        void SetSirenFile(string strFilePath);
        void SetSpeed(int nSpeed);
        int GetSpeed();
        void SetVolume(int nVolume);
        int GetVolume();
    }

    public enum SpeechState
    {
        STANDBY = 1,
        PLAY = 2,
        STOP = 3,
        PAUSE = 4,
        REPEAT = 5
    }

    // 방송이 실행되고 있는 도중에 다른 방송실행 요청이 올 경우에 대한 처리방안
    public enum MultiMode
    {
        // 기존 방송을 즉시 종료시키고 새로운 방송을 실행한다.
        STOP_N_NEW_PLAY = 1,
        // 기존 방송이 끝날때까지 기다렸다가 새로운 방송을 실행한다.
        WAIT_N_NEW_PLAY = 2,
        // 기존 방송이 실행중이면 새로운 방송 요청은 무시한다.
        IGNORE_NEW_PLAY = 3
    }
}
