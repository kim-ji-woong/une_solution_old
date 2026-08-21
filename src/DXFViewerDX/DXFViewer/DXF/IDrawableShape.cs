using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DXFViewer
{
    public interface IDrawableShape
    {
        bool Draw(SharpDX.Direct2D1.RenderTarget g, bool bDrawText);

        bool CreateDXResource();
        
        bool DiscardDXResource();

        DXFDotNet.Shape GetShapeObject();


    }
}
