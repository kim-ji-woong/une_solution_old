using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace UnE.SOP.Sections
{
	public class TabPageManager
	{
		private SortedList mPageList = new SortedList();
		private SortedList mPageVirtualList = new SortedList();

		protected static TabPageManager instance = null;
		public static TabPageManager Instance
		{
			get
			{
				if (instance == null)
				{
					instance = new TabPageManager();						
				}
				return instance;
			}
		}

		public void ChangeWaterMark(bool bUse)
		{
			foreach (TabPage page in mPageList.Values)
			{
				SectionTabPage tabpage = (SectionTabPage)page;
				tabpage.UseWaterMark = bUse;
				tabpage.Refresh();
			}
			foreach (TabPage page in mPageVirtualList.Values)
			{
				SectionTabPage tabpage = (SectionTabPage)page;
				tabpage.UseWaterMark = bUse;
				tabpage.Refresh();
			}
		}

		public void ChangeVirtualMode(TabPage page, bool vVirtual)
		{
			//SectionTabPage tabpage = (SectionTabPage)page;
			//RemovePage(page, !vVirtual);
			//AddPage(page, !vVirtual);
		}

		public IList GetAliveList(bool bReal)
		{
			if (bReal == true)
				return mPageList.GetKeyList();
			return mPageVirtualList.GetKeyList();
		}

		public void SetUsePage(int nActionStepID, bool bUse, bool bReal)
		{
			SectionTabPage page = (SectionTabPage)GetPage(nActionStepID, bReal);
			if( page != null)
			{
				if (bUse == true)
					page.State = TabPageState.USE;
				else
					page.State = TabPageState.NOUSE;
			}               
		}

		public TabPage GetPage(int nActionStepID, bool bReal)
		{
			if (bReal == true)
			{
				int nIdx = mPageList.IndexOfKey(nActionStepID);
				if (nIdx < 0)
					return null;
				TabPage page = (TabPage)mPageList.GetByIndex(nIdx);
				return page;
			}
			else
			{
				int nIdx = mPageVirtualList.IndexOfKey(nActionStepID);
				if (nIdx < 0)
					return null;
				TabPage page = (TabPage)mPageVirtualList.GetByIndex(nIdx);
				return page;
			}
		}

		public void AddPage(TabPage page, bool bReal)
		{
			if (bReal == true)
			{
				SectionTabPage tabPage = (SectionTabPage)page;
				if (mPageList.ContainsKey(tabPage.ActionStepID))
					return;
				mPageList.Add(tabPage.ActionStepID, page);
			}
			else
			{
				SectionTabPage tabPage = (SectionTabPage)page;
				if (mPageVirtualList.ContainsKey(tabPage.ActionStepID))
					return;
				mPageVirtualList.Add(tabPage.ActionStepID, page);
			}				
		}
        


		public void RemovePage(int nActionStepID, bool bReal)
		{
			SectionTabPage tabPage = (SectionTabPage)GetPage(nActionStepID, bReal);
            if (tabPage != null)
            {
                tabPage.LinkedZoneID = -1;
                tabPage.LinkedZoneName = "";
                RemovePage(tabPage, bReal);
            }
		}

		public void RemovePage(TabPage page, bool bReal)
		{
			if (bReal == true)
			{
				SectionTabPage tabPage = (SectionTabPage)page;
				int nIdx = mPageList.IndexOfValue(tabPage);
				if (nIdx < 0)
					return;

                tabPage.LinkedZoneID = -1;
                tabPage.LinkedZoneName = "";

				mPageList.RemoveAt(nIdx);
			}
			else
			{
				SectionTabPage tabPage = (SectionTabPage)page;
				int nIdx = mPageVirtualList.IndexOfValue(tabPage);
				if (nIdx < 0)
					return;

                tabPage.LinkedZoneID = -1;
                tabPage.LinkedZoneName = "";

				mPageVirtualList.RemoveAt(nIdx);
			}				
		}
	}
}    
