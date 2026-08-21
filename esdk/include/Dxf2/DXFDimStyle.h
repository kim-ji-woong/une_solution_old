#pragma once

namespace DXF
{
	namespace TABLES
	{
		class BlockRecord;
		class TableManager;

		class DimStyle : public Table
		{
		public:
			class Entity
			{
			public:
				Entity(DimStyle* pTable, int nStyleHandle);

			public:
				void Write(Utility::FileManager* pMgr, BlockRecord* pBlockRecord) const;

			protected:
				void Init();

			public:
				void SetDimStyleName(wchar_t* strDimStyle);
				void SetArrowSize(double dArrowSize);
				void SetBaseLineSpace(double dSpace);
				void SetTIH(int nTIH);
				void SetTOH(int nTOH);
				void SetZIN(int nZIN);
				void SetFontSize(double dFontSize);
				void SetCenterMarkSize(double dSize);
				void SetAltF(double dAltf);
				void SetTextSpace(double dSpace);
				void SetSignificant(int nSignificant);
				void SetTextDistance(bool bFar);
				void SetTDec(int nTDec);
				void SetAltTD(int nAltTD);
				void SetDSep(int nDSep);
				void SetTolJ(int nTolJ);
				void SetTZin(int nTZin);
				void SetTofl(int nTofl);
				void SetSpaceFromObject(double dSpace);
				void SetExtendedLength(double dLength);
				void SetTextColor(int nACI);
				void SetArrowType(ArrowType type);

				wchar_t* GetDimStyleName();
				double GetArrowSize();
				double GetBaseLineSpace();
				int GetTIH();
				int GetTOH();
				int GetZIN();
				double GetFontSize();
				double GetCenterMarkSize();
				double GetAltF();
				double GetTextSpace();
				int GetSignificant();
				bool GetTextDistance();
				int GetTDec();
				int GetAltTD();
				int GetDSep();
				int GetTolJ();
				int GetTZin();
				int GetHandle();
				int GetStyleHandle();
				double GetSpaceFromObject();
				// ACI
				int GetTextColor();
				ArrowType GetArrowType() const;

			protected:
				DimStyle* m_pParent;
				std::wstring m_strDimStyle;
				int m_nHandle;
				double m_dArrowSize;		// 화살표 크기
				double m_dBaseLineSpace;	// 연속 치수 기입시 기준선 간격
				double m_dExtendedLength;	// 치수 보조선이 치수선보다 얼만큼 더 그려질 것인가?
				int m_nTIH;					// On(1)  : 치수 문자는 항상 화면의 가로축과 평행한 방향으로 표시
											// Off(0) : 치수 문자는 치수선과 평행한 방향으로 표시
				int m_nTOH;					// On(1)  : 치수선 내에 치수 문자가 들어가지 못할 경우 화면의 가로축과 평행한 방향으로 표시
											// Off(0) : 치수선과 평행한 방향으로 표시
				int m_nTAD;					// 1 : 치수선과 객체 사이에 치수 문자 입력
											// 2 : 객체로부터 치수선 바깥쪽에 치수 문자 입력
				int m_nZIN;					// 피트 구간 억제
				double m_dFontSize;			// 치수 문자 크기
				double m_dCenter;			// 치수선이 곡선일 경우 중심 마크의 크기
				double m_dAltf;				// 대체 단위 치수값
											// 가령 측정값이 1이고, 대체 단위값이 2.54라면
											// 표시는 2.54가 된다.(1인치를 센티미터로 출력하는 경우)
				double m_dGap;				// 치수선과 치수 문자 사이의 간격
				int m_nAltd;				// m_dAltf의 소수점 뒤 자리수
				int m_nTofl;				// On(1) : 치수선이 치수 보조선 바깥쪽에 그려진다.
											// Off(0) : 치수 보조선 사이에 그려진다.
				int m_nDec;					// 치수값의 소수점 뒤 자리수
				double m_dSpaceFromObject;
				int m_nTDec;
				int m_nAltTD;
				int m_nDSep;
				int m_nTolJ;
				int m_nTZin;
				int m_nStyleHandle;			// 참조된 Style 객체의 핸들
				int m_nTextColor;			// ACI
				ArrowType m_arrowType;
			};

		public:
			DimStyle(TableManager* pMgr);
			virtual ~DimStyle(void);

		public:
			void Init();
			void Write(Utility::FileManager* pMgr);

		public:
			virtual void ReadDatai(int nCode, int nData) {}
			virtual void ReadDatad(int nCode, double dData) {}
			virtual void ReadDatas(int nCode, wchar_t* strData) {}

		public:
			void AddEntity(const Entity& rEntity);
			Entity* GetEntity(wchar_t* strStyleName);
			void SetBlockRecord(BlockRecord* pBlockRecord);

		protected:
			std::list<Entity> m_list;
			//std::list<DXFData> m_list;
			BlockRecord* m_pBlockRecord;
		};
	}
}
