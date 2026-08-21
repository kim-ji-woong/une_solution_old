// ReadFileManager.h

#pragma once

using namespace System;
using namespace System::IO;
using namespace System::Collections::Generic;

int main(array<System::String ^> ^args)
{
	return 0;
};
namespace ReadFileManager {

	public ref class ChartField
	{
	public:

		ChartField(int nPipeID, int nTankID, DateTime^ dtTimeStamp, double dPressure, double dFlow)
		{
			this->m_nPipeID = nPipeID;
			this->m_nTankID = nTankID;
			this->m_dtTimeStamp = dtTimeStamp;			
			this->m_dPressure = dPressure;
			this->m_dFlow = dFlow;
		};
		 
		int nPipeID()
		{
			return m_nPipeID;
		}
		int nTankID()
		{
			return m_nTankID;
		}
		DateTime^ dtTimeStamp()
		{
			return m_dtTimeStamp;
		}
		double dPressure()
		{
			return m_dPressure;
		}
		double dFlow()
		{
			return m_dFlow;
		}
	private:		
		int m_nPipeID;
		int m_nTankID;
		DateTime^ m_dtTimeStamp;
		double m_dPressure;
		double m_dFlow;
	};
	public ref class ReadHistory
	{
		// TODO: 여기에 이 클래스에 대한 메서드를 추가합니다.
	public:
		List<ChartField^>^ ReadPressure(String^ path, int pipeID); 

	public:
		List<ChartField^>^ ReadFlow(String^ path, int tankID);
	}; 
}
