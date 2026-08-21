using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sections;
using System.Drawing;

namespace SectionStyle.Fancy
{
    public class StyleFactory : IShapeStyleFactory
    {
        public StyleFactory()
        {
            Style.InitStateColor();
        }

        public IShapeStyler CreateShapeStyler(Section section, Shape shape)
        {
            if (section == null || shape == null)
                return null;

            if (section is SectionEndPoint && shape is ShapeEndPoint)
                return new StyleEndPoint((SectionEndPoint)section, (ShapeEndPoint)shape);
            else if (section is SectionProcess && shape is ShapeProcess)
                return new StyleProcess((SectionProcess)section, (ShapeProcess)shape);
            else if (section is SectionDecision)
                return new StyleDecision((SectionDecision)section, shape);
            else if (section is SectionAnnotation && shape is ShapeAnnotation)
                return new StyleAnnotation((SectionAnnotation)section, (ShapeAnnotation)shape);
            else if (section is SectionInternal && shape is ShapeInternal)
                return new StyleInternal((SectionInternal)section, (ShapeInternal)shape);

            return null;
        }
    }
}
