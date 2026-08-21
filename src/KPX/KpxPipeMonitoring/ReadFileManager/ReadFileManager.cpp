// 기본 DLL 파일입니다.

#include "stdafx.h"
#include <string>
#include "ReadFileManager.h"
#include <stdio.h>  
#include <iostream>  
using namespace std;
using namespace System;

namespace ReadFileManager {
	List<ChartField^>^ ReadHistory::ReadFlow(String^ path, int tankID)
	{
		try
		{
			using namespace Runtime::InteropServices;
			char* strFilePath = (char*)(Marshal::StringToHGlobalAnsi(path)).ToPointer();

			FILE* fp = fopen(strFilePath, "rb");

			List<ChartField^>^ list = gcnew List<ChartField^>();

			if (fp != NULL)
			{
				// 1. File Size 읽기			
				fseek(fp, 0, SEEK_END);
				int nFileSize = ftell(fp);
				if (nFileSize == 0)
					return list;

				fseek(fp, 0, SEEK_SET); 

				// 2. FIle Size 만큼 메모리 할당
				char* buf = new char[nFileSize]; 

				// 3. File 읽기 
				if (buf != NULL)
				{
					size_t result = fread(buf, 1, nFileSize, fp);

					int nIndex = 0;
					while (nIndex < nFileSize)
					{
						long long time;
						float flow;
						float temp;
						float level;
						int pipeId;
						float pressure;

						memcpy(&time, &buf[nIndex], 8);
						nIndex += 8;

						DateTime^ dt = gcnew DateTime(time);

						memcpy(&flow, &buf[nIndex], sizeof(float));
						nIndex += sizeof(float);

						memcpy(&temp, &buf[nIndex], sizeof(float));
						nIndex += sizeof(float);

						memcpy(&level, &buf[nIndex], sizeof(float));
						nIndex += sizeof(float);

						memcpy(&pipeId, &buf[nIndex], sizeof(int));
						nIndex += sizeof(int);

						memcpy(&pressure, &buf[nIndex], sizeof(float));
						nIndex += sizeof(float);

						if (flow == -999 || flow == -9999) flow = 0;
						if (pressure == -999 || pressure == -9999) pressure = 0;

						// 4. ChartField List 만들기
						list->Add(gcnew ChartField(pipeId, tankID, dt, pressure, flow));
					}
				}

				// 5. 메모리 해제
				fclose(fp);
				delete[] buf; 
			}

			return list;
		}
		catch (Exception^ e)
		{ 
			System::Diagnostics::Trace::WriteLine("ReadFlow file Exception " + e);
			return nullptr;
		}
	}
	List<ChartField^>^ ReadHistory::ReadPressure(String^ path, int pipeID)
	{
		try
		{
			using namespace Runtime::InteropServices;
			char* strFilePath = (char*)(Marshal::StringToHGlobalAnsi(path)).ToPointer();

			FILE* fp = fopen(strFilePath, "rb");

			List<ChartField^>^ list = gcnew List<ChartField^>();

			if (fp != NULL)
			{
				// 1. File Size 읽기			
				fseek(fp, 0, SEEK_END);
				int nFileSize = ftell(fp);
				if (nFileSize == 0)
					return list;

				fseek(fp, 0, SEEK_SET); 

				// 2. FIle Size 만큼 메모리 할당
				char* buf = new char[nFileSize]; 

				// 3. File 읽기 
				if (buf != NULL)
				{
					size_t result = fread(buf, 1, nFileSize, fp);

					int nIndex = 0;
					while (nIndex < nFileSize)
					{
						long long time;
						float pressure;
						float flow; 
						int tankId;						

						memcpy(&time, &buf[nIndex], 8);
						nIndex += 8;

						DateTime^ dt = gcnew DateTime(time);

						memcpy(&pressure, &buf[nIndex], sizeof(float));
						nIndex += sizeof(float);

						memcpy(&flow, &buf[nIndex], sizeof(float));
						nIndex += sizeof(float);
						 
						memcpy(&tankId, &buf[nIndex], sizeof(int));
						nIndex += sizeof(int); 

						if (pressure == -999 || pressure == -9999) pressure = 0;
						if (flow == -999 || flow == -9999) flow = 0;						

						// 4. ChartField List 만들기
						list->Add(gcnew ChartField(pipeID, tankId, dt, pressure, flow));
					}
				}

				// 5. 메모리 해제
				fclose(fp);
				delete[] buf;
			}

			return list;
		}
		catch (Exception^ e)
		{
			System::Diagnostics::Trace::WriteLine("ReadPressure file Exception " + e);
			return nullptr;
		}
	}
}
