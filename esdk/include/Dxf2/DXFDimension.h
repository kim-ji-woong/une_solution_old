#pragma once

namespace Utility
{
	class Vertex3D;
}

namespace DXF
{
	namespace TABLES
	{
		class TableManager;
	}

	namespace BLOCKS
	{
		class BlockData;
		class BlockManager;
	}

	namespace ENTITIES
	{
		// 0에서 6까지의 값은 치수 유형을 나타내는 정수 값
		// 값 32, 64, 128은 정수 값에 추가된 비트 값
		enum DimType {NORMAL = 0, ALIGNED = 1, ANGULAR = 2, DIAMETER = 3, RADIUS = 4, ANGULAR_3_POINT = 5,
			ORDINATE = 6, BLOCK_REF = 32, ORDINATE_TYPE = 64, USER_DEFINED = 128};

		// 치수 문자를 어디에 부착시킬 것인가?
		enum DimAttachType {TOP_LEFT = 1, TOP_CENTER = 2, TOP_RIGHT = 3, MIDDLE_LEFT = 4,
							MIDDLE_CENTER = 5, MIDDLE_RIGHT = 6, BOTTOM_LEFT = 7,
							BOTTOM_CENTER = 8, BOTTOM_RIGHT = 9};

		class DimensionItem
		{
		public:
			virtual void Write(Utility::FileManager* pMgr) = 0;

		protected:
			std::wstring m_strSubClassName;
		};

		class RadialDimension : public DimensionItem
		{
		public:
			RadialDimension(bool bRadial);
			virtual ~RadialDimension();

		public:
			virtual void Write(Utility::FileManager* pMgr);

		public:
			void SetData(const Utility::Vertex3D& ptObj, double dLength);

		protected:
			void Init();

		protected:
			bool m_bRadial;
			Utility::Vertex3D m_ptDefinition;
			double m_dLength;
		};

		class AngularDimension : public DimensionItem
		{
		public:
			AngularDimension();
			virtual ~AngularDimension();

		public:
			virtual void Write(Utility::FileManager* pMgr);

		public:
			// ptRightLineBegin : 오른쪽 선의 양 끝점 가운데 원의 중점과 가까운 점
			// ptRightLineEnd : 오른쪽 선의 양 끝점 가운데 원의 중점과 먼 점
			// ptLeftLineBegin : 왼쪽 선의 양 끝점 가운데 원의 중점과 가까운 점
			// ptArcBegin : 호의 양 끝점 가운데 아무점이나 상관없음
			void SetData(const Utility::Vertex3D& ptRightLineBegin, const Utility::Vertex3D& ptRightLineEnd, const Utility::Vertex3D& ptLeftLineBegin, const Utility::Vertex3D& ptArcBegin);

		protected:
			void Init();

		protected:
			Utility::Vertex3D m_ptRightLineBegin;// 오른쪽 선의 양 끝점 가운데 원의 중점과 가까운 점
			Utility::Vertex3D m_ptRightLineEnd;	// 오른쪽 선의 양 끝점 가운데 원의 중점과 먼 점
			Utility::Vertex3D m_ptLeftLineBegin;	// 왼쪽 선의 양 끝점 가운데 원의 중점과 가까운 점
			Utility::Vertex3D m_ptArcBegin;		// 호의 양 끝점 가운데 아무점이나 상관없음
		};

		class AlignedDimension : public DimensionItem
		{
		public:
			AlignedDimension();
			virtual ~AlignedDimension();

		public:
			virtual void Write(Utility::FileManager* pMgr);

		public:
			void SetData(const Utility::Vertex3D& ptObj1, const Utility::Vertex3D& ptObj2);

		protected:
			void Init();

		protected:
			Utility::Vertex3D m_ptDefinition1;	// 작은 값(13,23,33)
			Utility::Vertex3D m_ptDefinition2;	// 큰 값(14,24,34)
		};

		class LinearAndRotatedDimension : public DimensionItem
		{
		public:
			LinearAndRotatedDimension();
			virtual ~LinearAndRotatedDimension();

		public:
			virtual void Write(Utility::FileManager* pMgr);

		public:
			void SetData(const Utility::Vertex3D& ptObj1, const Utility::Vertex3D& ptObj2, bool bVertical);

		protected:
			void Init();

		protected:
			std::wstring m_strSubClassName2;
			Utility::Vertex3D m_ptDefinition1;	// 작은 값(13,23,33)
			Utility::Vertex3D m_ptDefinition2;	// 큰 값(14,24,34)
			double m_dAngle;			// Degree(수직이면 270.0, 수평이면 0.0)
		};

		class Dimension : public Entity
		{
		public:
			Dimension(TABLES::TableManager* pTblMgr, BLOCKS::BlockManager* pBlkMgr, wchar_t* strLayerName);
			virtual ~Dimension(void);
			Dimension(const Dimension& rhs);
			void operator= (const Dimension& rhs);

		public:
			virtual void Write(Utility::FileManager* pMgr);
			virtual void Init();

		public:
			void SetBlockName(wchar_t* strBlockName);
			void SetDefinitionPoint(double x, double y, double z);
			void SetTextMiddlePoint(double x, double y, double z);
			void SetDimensionType(int nType);
			void SetAttachmentType(int nType);
			void SetActualMeasurement(double dActualMeasurement);
			void SetDimLineStyle(wchar_t* strDimLineStyle);
			void SetUserDefinedText(wchar_t* strUserDefined);
			void SetDimensionItem(DimensionItem* pItem);

			BLOCKS::BlockData* GetBlockData();

		private:
			DimensionItem* m_pDimItem;
			int* m_pRefCount;

		protected:
			std::wstring m_strBlockName;
			std::wstring m_strUserDefined;
			double m_dDefPoint[3];
			double m_dTextMidPoint[3];
			int m_nDimType;
			int m_nAttachType;
			double m_dActualMeasurement;
			std::wstring m_strDimLineStyle;
			BLOCKS::BlockData* m_pBlkData;
		};
	}
}
