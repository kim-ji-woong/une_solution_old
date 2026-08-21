using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

// Section 그리기 작업시 사용자 정의 그리기 동작을 수행하고자 할때 사용되는 객체

namespace Sections
{
    public interface ISectionPainter
    {
        void Draw(System.Drawing.Graphics g);

    }
}
