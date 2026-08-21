using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace ShapeFileReader.Drawing
{
    public enum PointDrawingType { CIRCLE = 0, RECTANGLE };

    public interface IShapeAttrib
    {
        Color GetLineColor();
        Color GetFillColor();
        double GetPointSize();
        PointDrawingType GetPointDrawingType();
        int GetLineThickness();
    }
}
