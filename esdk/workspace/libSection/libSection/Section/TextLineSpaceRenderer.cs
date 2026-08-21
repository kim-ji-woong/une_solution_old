using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Sections
{

    /// <summary>
    /// 줄간격을 제공하기 위해 Text를 표시하는 사각영역안에 줄간격을 포함하여 Tetxt를 그려주는 Class
    /// 실시간 사용시 속도에 문제가 있으므로 개선이 필요함, 2015-09-03 edited by skkim
    /// </summary>
    internal class TextLineSpaceRenderer
    {
        private float CharacterWidth(Graphics g, string szTarget, Font font)
        {
            float lineWidth = g.MeasureString(szTarget, font).Width;
            return lineWidth;
        }

        private List<string> SplitLineText(string szText, SizeF size, Font font)
        {
            List<string> allLines = new List<string>();

            using (Graphics g = Graphics.FromHwnd(IntPtr.Zero))
            {
                float lineWidth = 0.0f;
                int textLength = 0;
                int beginIndex = 0;

                string[] lines = szText.Split(new char[] { '\n' });
                for (int j = 0; j < lines.Length; j++)
                {
                    // 캐리지리턴은 제거
                    string szTarget = lines[j].Replace("\r", "");
                    // 측정된 라인 길이
                    lineWidth = 0.0f;
                    // 실제 자르게 되는 길이
                    textLength = 0;
                    // 다음라인으로 넘길 문자열의 시작 index
                    beginIndex = 0;
                    // 현재까지 읽은 길이
                    int readLength = 0;

                    char[] charts = lines[j].ToCharArray();
                    for (int i = 0; i < charts.Length; i++)
                    {
                        // 한문자씩 추가하여 길이를 측정한다.
                        string szMesure = szTarget.Substring(beginIndex, i - beginIndex);
                        lineWidth = CharacterWidth(g, szMesure, font);
                        if (size.Width < lineWidth)
                        {
                            if (i != 0)
                            {
                                // 1개 적은 길이로 자른다.
                                textLength = (i - 1) - beginIndex;
                                string line = lines[j].Substring(beginIndex, textLength);
                                allLines.Add(szTarget);

                                // 새로운 시작 index는 한개 이전 문자부터 임
                                beginIndex = i - 1;
                                // 자른 길이를 저장한다.
                                readLength += textLength;
                            }
                            else
                            {
                                // Width가 한글자길이 보다 짧으므로 표현할 Text가 없다.
                                return null;
                            }
                        }
                    }

                    // 짜르고 남은것(또는 짤리지 않는것)을 저장한다.
                    int nExtraLength = lines[j].Length - readLength;
                    string szTemp = lines[j].Substring(beginIndex, nExtraLength);
                    allLines.Add(szTemp);
                }

            }
            return allLines;
        }

        private List<RectangleF> CalcTextRect(List<string> allLines, Font font, RectangleF rect, float lineSpacing)
        {
            List<RectangleF> allRects = new List<RectangleF>();

            using (Graphics g = Graphics.FromHwnd(IntPtr.Zero))
            {
                // 전체 Text영역의 높이
                float fullHeight = rect.Height;
                // 현재 폰트에서 한줄의 높이
                float lineHeight = g.MeasureString("X", font).Height;

                // 전체 텍스트 영역에 몇개를 표시할 것인가? 
                float fCount = fullHeight / (lineHeight + lineSpacing);
                int nCount = (int)Math.Round(Math.Floor(fCount));
                if (nCount > allLines.Count)
                    nCount = allLines.Count;

                // 표시할 개수 만큼 Rect를 생성한다.
                for (int i = 0; i < nCount; i++)
                {
                    float width = rect.Width;
                    float height = (lineHeight + lineSpacing);

                    float x = rect.X;
                    float y = rect.Y + (i * height);

                    RectangleF rectLine = new RectangleF(x, y, width, height);
                    allRects.Add(rectLine);
                }
            }
            return allRects;
        }

        public void DrawText(Graphics g, string lines, Font font, Brush brush, RectangleF textRect, float lineSpacing, StringFormat format = null)
        {
            SizeF size = textRect.Size;

            if (size.Width == 0 || size.Height == 0)
                return;

            // 지정된 Rect영역에 들어갈 Text라인을 구한다.
            List<string> allLines = SplitLineText(lines, size, font);
            if (allLines == null)
                return;

            // 각각의 라인을 그릴 Rectangle을 생성한다.
            List<RectangleF> rectLines = CalcTextRect(allLines, font, textRect, lineSpacing);
            if (rectLines == null)
                return;

            // 텍스트를 표시한다.
            for (int i = 0; i < rectLines.Count; i++)
            {
                string szText = allLines[i];
                RectangleF rect = rectLines[i];
                if (format != null)
                {
                    g.DrawString(szText, font, brush, rect, format);
                }
                else
                {
                    g.DrawString(szText, font, brush, rect);
                }
            }
        }
    }
}
