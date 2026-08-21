using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DXFViewer
{
    public abstract class Shape : DXFDotNet.Shape, IDrawableShape
    {
        public Shape() : base()
        {

        }
        public abstract bool Draw(SharpDX.Direct2D1.RenderTarget g, bool bDrawText);
        public abstract bool CreateDXResource();
        public abstract bool DiscardDXResource();
        public abstract DXFDotNet.Shape GetShapeObject();
    }
}
