#pragma once

#include "Layer.h"

using namespace System;

namespace Core
{
	public ref class APLayer : public Layer
	{
	protected:
		APLayer(){}
	public:
		APLayer(int nLayerID);
		virtual ~APLayer();


		virtual void Add(int nObjID) override;
		virtual void Remove(int nObjID) override;

		virtual void SetVisible(bool bShow) override;

		virtual void SetLOD(int nLevel) override;
	};


	public ref class AP
	{
	protected:
		AP(){}

	private:
		int m_nIconID;
		int m_nTextID;

		int m_nLOD;
		bool m_bVisible;
	protected:
		

		int m_nWorkID;
		System::String^ m_szIconPath;
		System::String^ m_szName;

		float pX, pY, pZ;
	public:
		AP(System::String^ szName);
		virtual ~AP(void);

		property int WorkID
		{
			int get() { return m_nWorkID; }
		}
	
		int CreateAP( System::String^ szPath);

		void SetLocation(float x, float y, float z);

		bool Select();

		void ClearSelect();

		void Delete();

		void OnVisible(bool m_bVisible);

		void SetLOD(int nLOD);
	};

	public ref class APManager
	{
	protected:
		System::Collections::ArrayList^ m_APList;
		APManager();
	public:
		
		static APManager^ m_Instance;
		property static APManager^  Instance
		{
			APManager^ get()
			{
				if( m_Instance == nullptr)
					m_Instance = gcnew APManager();
				return m_Instance; 
			}
		}

		property System::Collections::ArrayList^  APs
		{
			System::Collections::ArrayList^ get() { return m_APList; }
		}

		AP^ GetAP(int nID);

		void AddAP(AP^ ap);

		void RemoveAP(AP^ ap);
		void RemoveAP(int nID);
	};
};