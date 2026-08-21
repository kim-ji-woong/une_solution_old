#pragma once

namespace DXF
{
	namespace OBJECTS
	{
		class ObjectManager;
		class PlotSettings;

		class Layout : public Object
		{
		public:
			Layout(ObjectManager* pMgr);
			Layout(wchar_t* strLayoutName, int nOrder, int nBlockHandle, ObjectManager* pMgr);
			virtual ~Layout(void);

		public:
			void AddPlotSettings(PlotSettings* pPlot);
			PlotSettings* GetPlotSettings();
			void SetData();
			int GetBlockHandle();
			wchar_t* GetLayoutName();

		public:
			virtual void ReadDatai(int nCode, int nData);
			virtual void ReadDatad(int nCode, double dData);
			virtual void ReadDatas(int nCode, wchar_t* strData);

		protected:
			void Init();

		public:
			static wchar_t* GetSubClassName();

		protected:
			int m_nBlockHandle;
			std::wstring m_strLayoutName;
			int m_nOrder;

			PlotSettings* m_pPlotSettings;
			double m_dNumerator, m_dDenominator;
			bool m_readNumerator, m_readDenominator;

		protected:
			static std::wstring m_strSubClassName;
		};
	}
}
