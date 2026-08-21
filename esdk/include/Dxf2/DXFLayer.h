#pragma once

namespace DXF
{
	enum LayerProperty {FROZEN = 1, FROZEN_BY_NEW_VIEWPORT = 2, LOCK = 4};

	namespace TABLES
	{
		class TableManager;

		class Layer : public Table
		{
		public:
			class Entity
			{
			public:
				Entity(int nHandle1, int nHandle2);
				Entity(Layer* pTable, wchar_t* strLayerName);

			public:
				void Write(Utility::FileManager* pMgr);
				void SetLayerName(wchar_t* strLayerName);
				void SetOwner(Layer* pTable);
				void SetColor(int nColor);
				void SetLineType(wchar_t* strLineType);
				void SetFlag(int nFlag);
				wchar_t* GetLayerName();
				// Return 값 : ACI(AutoCAD Color Index)
				int GetColor();
				wchar_t* GetLineType();
				int GetHandle();
				int GetFlag();
				bool IsFrozen() const;
				bool IsLocked() const;
				bool IsHidden() const;

			protected:
				std::wstring m_strLayerName;
				Layer* m_pParent;
				int m_nHandle;
				int m_nFlag;	// LayerProperty(Bit Flag)
				int m_nColor;
				std::wstring m_strLineType;
				int m_nLineWeight;
				int m_nHardPointer;
			};

		public:
			Layer(TableManager* pMgr);
			virtual ~Layer(void);
			friend class Entity;

		public:
			void Clear();
			void Init();
			void Write(Utility::FileManager* pMgr);
			// pID : Layer 정보를 담고 있는 링크드 리스트 노드의 포인터
			Entity* GetEntity(void*& pID);
			Entity* AddEntity(wchar_t* strLayerName);

		public:
			virtual void ReadDatai(int nCode, int nData);
			virtual void ReadDatad(int nCode, double dData);
			virtual void ReadDatas(int nCode, wchar_t* strData);

		protected:
			std::list<Entity> m_list;
			Entity* m_pEntity;

		private:
			std::list<Entity>::iterator m_entIter;
		};
	}
}
