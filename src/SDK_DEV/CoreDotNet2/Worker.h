#pragma once

#include "Layer.h"

using namespace System;

namespace Core
{
	public ref class WorkerLayer : public Layer
	{
	protected:
		WorkerLayer(){}
	public:
		WorkerLayer(int nLayerID);
		virtual ~WorkerLayer();


		virtual void Add(int nObjID) override;
		virtual void Remove(int nObjID) override;

		virtual void SetVisible(bool bShow) override;

		virtual void SetLOD(int nLevel) override;
	};


	public ref class Worker
	{
	protected:
		Worker(){}

	private:
		int m_nIconID;
		int m_nTextID;
		int m_nAccTextID;

		int m_nLOD;
		bool m_bVisible;
	protected:
		

		int m_nWorkID;
		System::String^ m_szIconPath;
		System::String^ m_szName;
		System::String^ m_szSceneName;

		System::String^ m_szAccText;
		bool m_bShowNameOnly;
		float pX, pY, pZ;
	public:
		Worker(System::String^ szName);
		virtual ~Worker(void);

		property int WorkID
		{
			int get() { return m_nWorkID; }
		}
	
		int Worker::CreateWorker( System::String^ szPath);

		void SetLocation(float x, float y, float z);

		bool Select();

		void ClearSelect();

		void Delete();

		void OnVisible(bool m_bVisible);

		void SetLOD(int nLOD);

		
		void SetAccidentText(System::String^ szText);
		void ToggleText(bool bNameOnly);
		bool IsShowNameOnly() { return m_bShowNameOnly; }
		void ClearSetAccidentText();

	};

	public ref class WorkerManager
	{
	protected:
		System::Collections::ArrayList^ m_WorkList;
		WorkerManager();
	public:
		
		static WorkerManager^ m_Instance;
		property static WorkerManager^  Instance
		{
			WorkerManager^ get() 
			{
				if( m_Instance == nullptr)
					m_Instance = gcnew WorkerManager();
				return m_Instance; 
			}
		}

		property System::Collections::ArrayList^  Workers
		{
			System::Collections::ArrayList^ get() { return m_WorkList; }
		}

		Worker^ GetWorker(int nID);

		void AddWorker(Worker^ layer);

		void RemoveWorker(Worker^ layer);
		void RemoveWorker(int nID);
	};
};