#pragma once

namespace Utility
{
	class Vertex3D;
}

namespace DXF
{
	namespace ENTITIES
	{
		class PolyLine : public Entity
		{
		public:
			PolyLine(bool bLWPoly);
			virtual ~PolyLine(void);

		public:
			virtual void Write(Utility::FileManager* pMgr);
			virtual void Init();
			virtual bool ReadDatai(int nCode, int nData);
			virtual bool ReadDatad(int nCode, double dData);
			virtual bool ReadDatas(int nCode, wchar_t* strData);
			virtual void SetHandle(int nHandle);

		public:
			void AddPoint(Utility::Vertex2D pt);
			void AddPoint(double dX, double dY);
			int GetPointSize() const;
			// pID : 좌표 정보가 담긴 링크드 리스트 노드의 포인터
			// Return 값 : true(좌표를 얻어오는데 성공)
			//             false(더 이상 얻어올 좌표가 없다.)
			bool GetPoint(void*& pID, double* pX, double* pY, double* pBulge);
			void SetClosed(bool bClosed);
			bool GetClosed();
			double GetConstantWidth() const;
			void SetConstantWidth(double dWidth);
			void ReadVertex(bool bVertex);

		protected:
			Utility::Vertex3D m_vNormal;
			int m_nPointSize;
			// m_pt[2]는 bulge 값이다.
			std::list<Utility::Vertex3D> m_list;
			bool m_bClosed;			// 닫힌 폴리선인가?
			double m_dConstantWidth;// 고정폭
			bool m_bLWPoly;			// LWPolyLine인가?
			bool m_bReadVertex;
			bool m_bSetHandle;		// Handle이 정해졌는가?
			std::wstring m_strVertexLayerName;

		private:
			double m_dTemp;
			std::list<Utility::Vertex3D>::iterator m_vertexIter;
			bool m_readClosed;
		};
	}
}
