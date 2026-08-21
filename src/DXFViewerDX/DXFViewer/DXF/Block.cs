using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DXFViewer
{
    public class Block : DXFDotNet.Block
    {
        protected DXFViewer.DXFControl m_Ctrl = null;

        public Block(DXFViewer.DXFControl ctrl) : base(ctrl)
        {
            m_Ctrl = ctrl;
        }

        public override void Add(DXFDotNet.Shape pObj)
        {
            base.Add(pObj);
        }

        public override bool Remove(DXFDotNet.Shape pObj)
        {
            bool bResult = base.Remove(pObj);

            return bResult;
        }

        public override void RemoveAll()
        {
            base.RemoveAll();
        }
    }
}
