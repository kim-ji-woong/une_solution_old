using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SDMS
{
    class EarthquakeSoundProcess
    {
        private static SoundPlayerEx m_player = new SoundPlayerEx();
        public static SoundPlayerEx SoundPlayer
        {
            get { return m_player; }
        }
        static public void PlaySound()
        {
            string szWavPath = FormMain.EnginPath() + "\\Media\\Sound\\FireSignalAlarm.WAV";
            if (System.IO.File.Exists(szWavPath))
            {
                m_player.SoundLocation = szWavPath;
                m_player.Play();
            }
        }
    }
}
