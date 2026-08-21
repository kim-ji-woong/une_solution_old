#pragma once

#include "Layer.h"

using namespace System;

namespace Core
{
	public enum class VehicleType : int
	{
		TRUCK,
		FORKLIFT,
		OTHER
	};

	public ref class VehicleLayer : public Layer
	{
	protected:
		VehicleLayer(){}
	public:
		VehicleLayer(int nLayerID);
		virtual ~VehicleLayer();


		virtual void Add(int nObjID) override;
		virtual void Remove(int nObjID) override;

		virtual void SetVisible(bool bShow) override;

		virtual void SetLOD(int nLevel) override;
	};


	public ref class Vehicle
	{
	public:
	public:
		
	protected:
		Vehicle(){}

	private:

		int m_nTextID;
		int m_nIconID;
		int m_nLOD;
		bool m_bVisible;

		float m_nWidth;
		float m_nLength;
		float m_nHight;
		VehicleType m_nType;
		System::Collections::ArrayList^ m_arScens;
		System::Collections::ArrayList^ m_arVecs;

		bool m_bFirstLocation;
	protected:
		System::String^ m_szIconPath;
		int m_nVehicleID;
		System::String^ m_szName;
		System::String^ m_szSceneName;


	public:
		/// <summary>
		/// 
		/// </summary>
		/// <param name="szName">ÀÌ¸§</param>
		/// <param name="nType">Á¾·ù</param>
		/// <param name="nWidth">Â÷Æø(M)</param>
		/// <param name="nLength">ÀüÆø(M)</param>
		/// <param name="nHeight">³ôÀÌ(M)</param>
		Vehicle( System::String^ szName, VehicleType nType, float nWidth, float nLength, float nHeight );

		virtual ~Vehicle(void);

		property int VehicleID
		{
			int get() { return m_nVehicleID; }
		}

		int CreateVehicle( System::String^ szPath);

		void SetLocation(float x, float y, float z);

		bool Select();

		void ClearSelect();

		void Delete();

		void OnVisible(bool m_bVisible);

		void SetLOD(int nLOD);

	};

	public ref class VehicleManager
	{
	
	protected:
		System::Collections::ArrayList^ m_VehicleList;
		VehicleManager();
	public:

		static VehicleManager^ m_Instance;
		property static VehicleManager^  Instance
		{
			VehicleManager^ get() 
			{
				if( m_Instance == nullptr)
					m_Instance = gcnew VehicleManager();
				return m_Instance; 
			}
		}

		property System::Collections::ArrayList^  Vehicles
		{
			System::Collections::ArrayList^ get() { return m_VehicleList; }
		}

		Vehicle^ GetVehicle(int nID);

		void AddVehicle(Vehicle^ layer);

		void RemoveVehicle(Vehicle^ layer);
		void RemoveVehicle(int nID);
	};
};