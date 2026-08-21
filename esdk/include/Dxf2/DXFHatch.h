#pragma once

namespace Utility
{
	class Vertex3D;
}

namespace DXF
{
	namespace ENTITIES
	{
		enum HatchStyle {NORMAL_STYLE = 0, OUTER_STYLE = 1, IGNORE_STYLE = 2};
		enum HatchPatternType {USER_DEFINE = 0, PRE_DEFINED = 1, CUSTOM = 2};

		class Hatch : public Entity
		{
		public:
			class HatchPattern
			{
			public:
				void SetData(double dAngle, double x, double y, double dOffsetX, double dOffsetY);

			public:
				double m_dPatternAngle;	// Degree
				Utility::Vertex2D m_ptBase;
				double m_dOffsetX;
				double m_dOffsetY;
				std::list<double> m_listDashLength;
			};

			class HatchPatternGroup
			{
			public:
				std::wstring m_strPatternName;
				std::list<HatchPattern> m_listPattern;
			};

			class Boundary
			{
			public:
				enum BoundaryType { POLYLINE = 0, LINEEDGE, ARCEDGE, EARCEDGE, SPLINEEDGE };

			public:
				Boundary();

			public:
				virtual void Write(Utility::FileManager* pMgr, int nSolidFill) = 0;
				virtual BoundaryType GetBoundaryType() = 0;
				virtual int GetObjectSize();
				virtual void WriteHandle(Utility::FileManager* pMgr);

			public:
				int m_nObjectHandle;
			};

			class PolyLineType : public Boundary
			{
			public:
				PolyLineType();
				PolyLineType(int nPointSize);
				~PolyLineType();
				PolyLineType(const PolyLineType& rhs);
				void operator= (const PolyLineType& rhs);

			public:
				// 좌표 대입이 끝난후 실행한다.
				void SetClosedFlag();
				void Write(Utility::FileManager* pMgr, int nSolidFill);
				BoundaryType GetBoundaryType();

			public:
				int m_nHasBulge;
				int m_nClosed;
				int m_nPointSize;
				//double m_dBulge;
				double* m_pArrX;
				double* m_pArrY;
				double* m_pArrBulge;

			protected:
				int* m_pRefCount;
				int m_nPointIndex;
				friend class Hatch;
			};

			class LineEdge : public Boundary
			{
			public:
				virtual int GetObjectSize();
				virtual void WriteHandle(Utility::FileManager* pMgr);

			public:
				void Write(Utility::FileManager* pMgr, int nSolidFill);
				BoundaryType GetBoundaryType();
				void AddLine(const Utility::Vertex2D& ptBegin, const Utility::Vertex2D& ptEnd, int nObjectHandle);

			public:
				std::list<Utility::Vertex2D> m_listBeginPoint;
				std::list<Utility::Vertex2D> m_listEndPoint;
				std::list<int> m_listObjectHandle;
				//double m_dBeginPoint[2];
				//double m_dEndPoint[2];
			};

			class ArcEdge : public Boundary
			{
			public:
				ArcEdge();
				// dBeginAngle, dEndAngle : Degree
				// dAngle : Radian
				ArcEdge(const Utility::Vertex2D& ptCenter, const Utility::Vertex2D& ptBegin, const Utility::Vertex2D& ptEnd, double dAngle, bool bCircle, double dBeginAngle, double dEndAngle);

			public:
				void Write(Utility::FileManager* pMgr, int nSolidFill);
				BoundaryType GetBoundaryType();

			public:
				// 호의 시작점과 끝점
				// 원일 경우 임의의 점과 그 점으로 부터 중점의 반대방향에 있는 점이 된다.
				Utility::Vertex2D m_ptBegin;
				Utility::Vertex2D m_ptEnd;
				Utility::Vertex2D m_ptCenter;
				// 호의 각
				double m_dAngle;		// Radian
				double m_dBeginAngle;	// Degree
				double m_dEndAngle;		// Degree
				bool m_bCircle;			// 원인가?
				double m_dRadius;
				int m_nDirection;		// 시계 반대방향 : 1, 시계 방향 : 0
			};

			class EArcEdge : public Boundary
			{
			public:
				EArcEdge();
				EArcEdge(double dCenterX, double dCenterY, double dLongX, double dLongY, double dRatio, double dBeginAngle, double dEndAngle, int nDirection = true);

			public:
				void Write(Utility::FileManager* pMgr, int nSolidFill);
				BoundaryType GetBoundaryType();

			public:
				double m_dCenterPoint[2];
				double m_dLongAxisPoint[2];		// 장축의 끝점
				double m_dRatio;				// 단축 대 장축비 (short / long)
				double m_dBeginAngle;	// Degree
				double m_dEndAngle;		// Degree
				int m_nDirection;		// 시계 반대방향 : 1, 시계 방향 : 0
			};

			class SplineEdge : public Boundary
			{
			public:
				SplineEdge();
				SplineEdge(int nAngle, int nRational, int nPeriodic, int nNumOfKnots, int nNumOfControlPoints, double dWeight = 1.0);
				~SplineEdge();
				SplineEdge(const SplineEdge& rhs);
				void operator= (const SplineEdge& rhs);

			public:
				void Write(Utility::FileManager* pMgr, int nSolidFill);
				BoundaryType GetBoundaryType();

			public:
				int m_nAngle;	// Degree
				int m_nRational;
				int m_nPeriodic;
				int m_nNumOfKnots;
				int m_nNumOfControlPoints;
				double m_dWeight;
				double* m_pArrKnots;
				double* m_pArrControlPointX;
				double* m_pArrControlPointY;

			private:
				int m_nKnotIndex;
				int m_nControlPointIndex;

			protected:
				int* m_pRefCount;
				friend class Hatch;
			};

			class BoundaryManager
			{
			public:
				~BoundaryManager();

			public:
				std::list<Boundary*> m_list;
			};

		public:
			Hatch(void);
			virtual ~Hatch(void);

		public:
			virtual void Write(Utility::FileManager* pMgr);
			virtual void Init();
			virtual bool ReadDatai(int nCode, int nData);
			virtual bool ReadDatad(int nCode, double dData);
			virtual bool ReadDatas(int nCode, wchar_t* strData);

		public:
			void SetElevationPoint(double x, double y, double z);
			void SetHatchPatternName(wchar_t* strHatchPattern);
			void SetSolidFillFlag(int nSolidFill);
			void SetAssociativityFlag(int nAssociativity);
			void SetBoundaryPathType(int nBoundaryPathType);
			void AddBoundary(BoundaryManager* pMgr);
			void SetHatchStyle(int nHatchStyle);
			void SetHatchPatternType(int nPatternType);
			void SetHatchPatternAngle(double dPatternAngle);
			void SetPatternScale(double dScale);
			void SetDoublePattern(bool bDouble);

			void SetNoObject(bool bNoObject);
			void SetNoObjectCircle(const Utility::Vertex2D& ptCenter, const Utility::Vertex2D& pt1, const Utility::Vertex2D& pt2);

			double GetPatternScale();
			// pID : Entity 정보를 담고 있는 링크드 리스트 노드의 포인터
			Entity* GetBoundaryEntity(void*& pID);
			bool IsSolidType();
			std::wstring& GetHatchPatternName();

			// pID : BoundaryManager 정보를 담고 있는 링크드 리스트 노드의 포인터
			BoundaryManager* GetBoundaryManager(void*& pID);

		protected:
			void WriteHatchPattern(Utility::FileManager* pMgr);

		protected:
			//static HatchPatternGroup* MakeHatchPattern();

		protected:
			double m_dElevationPoint[3];
			Utility::Vertex3D m_vExtrusionDirection;	// 돌출 방향
			std::wstring m_strHatchPattern;
			// 1 : Solid
			// 0 : Pattern
			int m_nSolidFill;
			int m_nAssociativity;
			int m_nBoundary;				// 경계 루프의 개수
			int m_nBoundaryPathType;
			std::list<BoundaryManager*> m_list;
			int m_nHatchStyle;
			int m_nHatchPatternType;
			double m_dHatchPatternAngle;
			double m_dPatternScale;
			bool m_bDoublePattern;

			// 경계 객체가 없이 Solid 원으로 이루어진 경우
			// m_bNoObject => true
			// m_ptArrCircle[0] : 원의 중점
			// m_ptArrCircle[1] : 원의 한 점
			// m_ptArrCircle[2] : 원의 반대편 점
			bool m_bNoObject;
			Utility::Vertex2D m_ptArrCircle[3];

			// 오직 읽기 용도로만 사용됨
			bool m_bBoundaryRead;
			std::list<Entity*> m_listBoundaryEntity;

		private:
			std::list<Entity*>::iterator m_pIterEntity;
			std::list<BoundaryManager*>::iterator m_pIterBoundaryManager;

			BoundaryManager* m_pCurrentBoundaryManager;
			Boundary* m_pCurrentBoundary;

		private:
			// Hatch Pattern Generation
			//static int m_nHatchPatternSize;
			//static HatchPatternGroup *m_pArrHatchPatternGroup;
		};
	}
}
