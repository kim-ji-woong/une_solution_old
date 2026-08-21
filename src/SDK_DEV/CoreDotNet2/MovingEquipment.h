#pragma once


#include "Layer.h"

using namespace System;

namespace Core
{

	public ref class EquipmentLayer : public Layer
	{
	protected:
		EquipmentLayer(){}
	public:
		EquipmentLayer(int nLayerID);
		virtual ~EquipmentLayer();

		
		virtual void Add(int nObjID) override;
		void  Add( int nObjID , int nType);
		virtual void Remove(int nObjID) override;
		void Remove( int nObjID , int nType);

		virtual void SetVisible(bool bShow) override;

		virtual void SetLOD(int nLevel) override;
	};



	public ref class MovingEquipment
	{
	internal:
		MovingEquipment(int nID);
		int Create(System::String^ body);
		void SetInitLocation(float x, float y, float z);
		void SetLocation(float x, float y, float z);
	protected:
		int m_nID;
		System::String^ m_szBodyName;
		System::String^ m_szEquipName;


		float m_fMinValue;
		float m_fMaxValue;
		int m_nTextID;
		int m_nLOD;
		bool m_bVisible;
	public:

		virtual ~MovingEquipment(void);

		property int EquipID
		{
			int get() { return m_nID; }
		}		
		//////////////////////////////////////////////////////////////////////////
		// z = 미터 (0~10 사이의 값 )
		//////////////////////////////////////////////////////////////////////////
		int SetLocation(float z);
		float GetLocation();

		void SetMaxValue(float maxValue);
		float GetMaxValue();

		void SetMinValue(float minValue);
		float GetMinValue();

		bool Select();

		void ClearSelect();

		void Delete();

		bool GetVisible();
		void OnVisible(bool m_bVisible);

		void SetLOD(int nLOD);

		System::Collections::ArrayList^ MovingEquipment::GetBound();
	};


	public ref class MovingEquipmentManager
	{
	protected:
		MovingEquipment^ m_Equipment;
	
		MovingEquipmentManager();
		void CreateMovingEquipment();
	public:

		static MovingEquipmentManager^ m_Instance;
		property static MovingEquipmentManager^  Instance
		{
			MovingEquipmentManager^ get() 
			{
				if( m_Instance == nullptr)
					m_Instance = gcnew MovingEquipmentManager();
				return m_Instance; 
			}
		}
				
		~MovingEquipmentManager();

		MovingEquipment^ GetEquipment(int idx);
	};
}

