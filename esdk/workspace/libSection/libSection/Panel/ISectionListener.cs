using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;

namespace Sections
{
    public interface ISectionListener
    {
        void OnSelectedArrow(Arrow arrow);
        void OnSelectedSection(Section section);
        void SetCurrentPanel(PanelSection panel);

        void OnSelectedSectionList(ArrayList arSectionList);
    }
}
