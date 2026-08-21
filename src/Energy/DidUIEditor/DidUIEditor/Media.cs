using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DidUIEditor
{
    public enum MediaType { None = 0, Image = 1, Movie }
    public class Media
    {
        private int m_nSequence = -1;
        /// <summary>
        /// 순서
        /// </summary>
        public int Sequence
        {
            get { return m_nSequence; }
            set { m_nSequence = value; }
        }

        private MediaType m_mediaType = MediaType.None;
        public MediaType MediaType
        {
            get { return m_mediaType; }
            set { m_mediaType = value; }
        }

        private Point m_mediaLocation { get; set; }
        public Point MediaLocation
        {
            get { return m_mediaLocation; }
            set { m_mediaLocation = value; }
        }

        private Size m_mediaSize { get; set; }
        public Size MediaSize
        {
            get { return m_mediaSize; }
            set { m_mediaSize = value; }
        }

        private string m_strFile { get; set; }
        public string File
        {
            get { return m_strFile; }
            set { m_strFile = value; }
        }

        private int m_nRunningSeconds { get; set; }
        /// <summary>
        /// 재생시간.
        /// 생략가능, 생략시 Page 끝날때까지 재생
        /// </summary>
        public int RunningSeconds
        {
            get { return m_nRunningSeconds; }
            set { m_nRunningSeconds = value; }
        }

        private int m_nBeginSeconds { get; set; }
        /// <summary>
        /// 동영상을 시작이후 몇초부터 재생할 것인가?
        /// 생략가능, 생략시 처음부터 재생
        /// </summary>
        public int BeginSeconds
        {
            get { return m_nBeginSeconds; }
            set { m_nBeginSeconds = value; }
        }

        public void GetSeconds(ref int refRunningSec, ref int refBeginSec)
        {
            refRunningSec = m_nRunningSeconds;
            refBeginSec = m_nBeginSeconds;
        }
    }
}
