using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnE.View.Content;

namespace UnE.Util.Unity
{  
    public class LayerManager : ILayerManager
    {
        Panel4Unity mParent = null;
    
        List<Layer> m_LayerList = new List<Layer>();

        public List<Layer> Layers
        {
          get { return m_LayerList; }
        }

        public UnE.Util.Unity.Panel4Unity ParentView
        {
          get { return mParent; }
        }
            
        public LayerManager()
        {
        }

        public LayerManager(Panel4Unity view)
        {
            mParent = view;
        }

		public ILayer GetLayer(int nID)
        {
            int nCount  = m_LayerList.Count;
		    for( int i = 0; i < nCount; i++)
		    {
			    Layer node = (Layer)m_LayerList[i];	
			    if( node.ID == nID)
				    return node;
		    }
            return null;
        }

        public void AddLayer(Layer layer)
        {
            if (!m_LayerList.Contains(layer))
            {
                layer.Parent = this;
                m_LayerList.Add(layer);
            }
        }

        public void AddLayer(int nID, bool bText)
        {
            int nCount  = m_LayerList.Count;
		    for( int i = 0; i < nCount; i++)
		    {
			    Layer node = (Layer)m_LayerList[i];	
			    if( node.ID == nID)
				    return;
		    }

		    Layer layer = new Layer(nID, bText);
		    layer.Parent = this;
		    m_LayerList.Add(layer);
        }

        public void AddLayer(int nID, bool bText, float nHideLODDist, float nShowLODDist)
        {
            int nCount = m_LayerList.Count;
		    for (int i = 0; i < nCount; i++)
		    {
			    Layer node = (Layer)m_LayerList[i];
			    if (node.ID == nID)
				    return;
		    }

		    Layer layer = new Layer(nID, bText, nHideLODDist, nShowLODDist);
		    layer.Parent = this;
		    m_LayerList.Add(layer);
        }

        public void RemoveLayer(Layer layer)
        {
            if (layer != null)
                m_LayerList.Remove(layer);
        }

        public void RemoveLayer(int nID)
        {
            ILayer layer = GetLayer(nID);
            if (layer != null)
                m_LayerList.Remove((Layer)layer);
        }

        public void ShowAllLayer()
        {
            int nCount = m_LayerList.Count;
            for (int i = 0; i < nCount; i++)
            {
                Layer node = (Layer)m_LayerList[i];
                node.SetVisible(true);
            }
        }

        public void ShowLayer(int nID)
        {
            ILayer layer = GetLayer(nID);
            if (layer != null)
            {
                layer.SetVisible(true);
            }
        }

        public void HideAllLayer()
        {
            int nCount  = m_LayerList.Count;
		    for( int i = 0; i < nCount; i++)
		    {
			    Layer node = (Layer)m_LayerList[i];	
			    node.SetVisible(false);
		    }
        }

        public void HideLayer(int nID)
        {
            ILayer  layer = GetLayer(nID);
            if (layer != null)
                layer.SetVisible(false);
        }

        public void RemoveLayerChild(int nObjID)
        {
            int nCount  = m_LayerList.Count;
		    for( int i = 0; i < nCount; i++)
		    {
			    Layer node = (Layer)m_LayerList[i];	
			    node.Objects.Remove(nObjID);			
		    }
        }
    }

    public class Layer : ILayer
    {
        private LayerManager m_Parent = null;

        public LayerManager Parent
        {
          get { return m_Parent; }  
          set { m_Parent = value; }
        }

        private ArrayList m_ObjList = new ArrayList();

        public ArrayList Objects
        {
            get { return m_ObjList; }
        }
		
        private bool m_bVisible = false;		
        private int m_nID = -1;
        
        public int ID
        {
            get { return m_nID; }
        }
		
        private bool m_bText = false;
        public bool IsText
        {
          get { return m_bText; }

        }
		private int m_nType = 0;
        public int Type 
        {
          get { return m_nType; }
        }

		int m_nLod;

		float m_nShortDist;
		float m_nLongDist;

        private Layer(){}
		public Layer(int nId, bool bText)
        {
            m_nID = nId;
            m_bText = bText;
            if( bText == true)
			    m_nType = 2;
		    else
			    m_nType = 1;
            m_Parent = null;
        }
     
		public Layer(int nId, bool bText, float nHideLODDist, float nShowLODDist)
        {
            m_nID = nId;
            m_bText = bText;
            m_nShortDist = nHideLODDist;
            m_nLongDist = nShowLODDist;
            m_nType = 3;
            m_Parent = null;
        }


		public virtual void Add(int nObjID)
        {
            if (!m_ObjList.Contains(nObjID))
                m_ObjList.Add(nObjID);
        }
		public virtual void Remove(int nObjID)
        {
            if (!m_ObjList.Contains(nObjID))
                m_ObjList.Remove(nObjID);
        }

		public virtual void SetVisible(bool bShow)
        {
            if (Parent != null)
            {
                m_Parent.ParentView.ShowLayer(m_nID,m_nType, bShow);
                //int nCount = m_ObjList.Count;
                //for (int i = 0; i < nCount; i++)
                //{
                //    int id = (int)m_ObjList[i];
                //    if (m_nType == 2)
                //        ;//m_Parent.ParentView.ShowTextPOI(id, bShow);
                //    else if (m_nType == 1)
                //        m_Parent.ParentView.ShowIconPOI(id, bShow);
                //    else if (m_nType == 3)
                //    {
                //        if (bShow == true)
                //            ;//m_Parent.ParentView.SetTextPOILOD(id, bShow, m_nShortDist);
                //        else
                //            ;// m_Parent.ParentView.SetTextPOILOD(id, bShow, m_nLongDist);

                //    }
                //}
            }
        }

		public virtual void SetLOD(int nLevel)
        {
            m_nLod = nLevel;
        }
    }
}