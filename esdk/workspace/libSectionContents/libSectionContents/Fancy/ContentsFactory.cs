using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using UnE.SOP.Sections;
using Sections;
using System.Drawing;

namespace SectionContents.Fancy
{
    public class ContentsFactory : ISectionContentsFactory
    {
        public ISectionContents CreateSectionContents(Section section, ISectionContentsOwner owner)
        {
            ComponentContents contents = new ComponentContents(section, owner);
            //contents.ContentsOwner = owner;
            return contents;
        }

        // sections에 있는 모든 Section들에 대한 ComponentContents를 한꺼번에 만든다.
        public Dictionary<Section, ISectionContents> CreateSectionContents(List<Section> sections, Control parent, ISectionContentsOwner owner)
        {
            Dictionary<Section, ISectionContents> dicSectionContents = new Dictionary<Section, ISectionContents>();
            int y = 0;
            ComponentContents prev = null;

            foreach (Section section in sections)
            {
                Section.ComponentType type = section.GetComponentType();

                if (type == Section.ComponentType.ANNOTATION ||
                    type == Section.ComponentType.GROUP ||
                    type == Section.ComponentType.LINK ||
                    type == Section.ComponentType.NONE)
                    continue;

                ComponentContents contents = new ComponentContents(section, owner);

                contents.Location = new Point(0, y);
                contents.Size = new Size(parent.ClientSize.Width, contents.Size.Height);
                contents.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
                //contents.ContentsOwner = owner;
                parent.Controls.Add(contents);

                // 10은 Padding
                y += contents.Size.Height + 10;
                dicSectionContents[section] = contents;

                if (prev != null)
                    prev.NextContents = contents;

                prev = contents;
            }

            return dicSectionContents;
        }

        // 하나의 Section에 대해서만 ComponentContents를 만든다.
        public ISectionContents CreateSectionContents(Section section, Control parent, ISectionContentsOwner owner)
        {
            Section.ComponentType type = section.GetComponentType();

            if (type == Section.ComponentType.ANNOTATION ||
                type == Section.ComponentType.GROUP ||
                type == Section.ComponentType.LINK ||
                type == Section.ComponentType.NONE)
                return null;

            int nControlCount = parent.Controls.Count;

            int y = 0;
            ComponentContents prev = null;

            for (int i=nControlCount-1;i>=0;i--)
            {
                Control ctrl = parent.Controls[i];

                if (ctrl is ComponentContents)
                {
                    prev = (ComponentContents)ctrl;
                    y = prev.Location.Y + prev.Size.Height + 10;
                    break;
                }
            }

            ComponentContents contents = new ComponentContents(section, owner);

            contents.Location = new Point(0, y);
            contents.Size = new Size(parent.ClientSize.Width, contents.Size.Height);
            contents.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            //contents.ContentsOwner = owner;
            parent.Controls.Add(contents);

            // 10은 Padding
            y += contents.Size.Height + 10;

            if (prev != null)
                prev.NextContents = contents;

            return contents;
        }
    }
}
