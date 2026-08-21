#pragma once


#include "Layer.h"

using namespace System;

namespace Core
{
	public ref class Crane
	{
	internal:
		Crane(int nID);
		int CreateCrane(System::String^ body, System::String^ line,  System::String^ pin);
		void SetInitLocation(float x, float y, float z);
		void SetLocation(float x, float y, float z);
	protected:
		int m_nID;
		System::String^ m_szBodyName;
		System::String^ m_szLineName;
		System::String^ m_szPinName;
		
		bool m_bVisible;
		int m_nTextID;
		int m_nLOD;
		System::String^ m_szCraneName;

		bool m_bfirst;
		float m_initZlocation;
	public:
		
		virtual ~Crane(void);

		property int CraneID
		{
			int get() { return m_nID; }
		}		
		
		int SetHookLocation(float z);

		float GetHookLocation();
		
		int SetLocation(float x);
		
		float GetLocation();


		bool Select();

		void ClearSelect();

		void Delete();

		bool GetVisible();
		void OnVisible(bool m_bVisible);

		void SetLOD(int nLOD);

		System::Collections::ArrayList^ Crane::GetBound();
		System::Collections::ArrayList^ Crane::GetHookBound();
	};


	public ref class CraneManager
	{
	protected:
		Crane^ m_Crane1;
		Crane^ m_Crane2;
		
		CraneManager();
		void CreateCrane();
	public:

		static CraneManager^ m_Instance;
		property static CraneManager^  Instance
		{
			CraneManager^ get() 
			{
				if( m_Instance == nullptr)
					m_Instance = gcnew CraneManager();
				return m_Instance; 
			}
		}		
		~CraneManager();

		Crane^ GetCrane(int idx);
	};
}

