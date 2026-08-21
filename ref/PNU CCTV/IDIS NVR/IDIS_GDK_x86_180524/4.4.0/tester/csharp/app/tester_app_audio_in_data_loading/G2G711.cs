using System;
using System.Collections.Generic;
using System.Text;
using GDK;

namespace GDK_tester
{
    class G2G711
    {
       
        public enum PCM_TYPE
        {
            PCM_ULAW = 0,
            PCM_ALAW
        };

        public G2G711()
        {

        }
        public G2G711(ref audio_codec_info sc, PCM_TYPE type)
        {
            type = PCM_ULAW;
        }
        ~G2G711() { }
    }
}
