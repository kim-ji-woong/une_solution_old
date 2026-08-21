#include "stdafx.h"
#include "MonitorInfo.h"
#include <iostream>
#include <windows.h>
#include <wingdi.h>

using namespace System;
using namespace System::Collections;
using namespace System::Collections::Generic;
using namespace Microsoft::Win32;

namespace UnE
{
	namespace Hardware
	{
		MonitorInfo::MonitorInfo()
		{
		}

		/*
			Gets GDI Device name from Source (e.g. \\.\DISPLAY4).
		*/
		wchar_t* getGDIDeviceNameFromSource(LUID adapterId, UINT32 sourceId) {
			DISPLAYCONFIG_SOURCE_DEVICE_NAME deviceName;
			DISPLAYCONFIG_DEVICE_INFO_HEADER header;
			header.size = sizeof(DISPLAYCONFIG_SOURCE_DEVICE_NAME);
			header.adapterId = adapterId;
			header.id = sourceId;
			header.type = DISPLAYCONFIG_DEVICE_INFO_GET_SOURCE_NAME;
			deviceName.header = header;
			DisplayConfigGetDeviceInfo((DISPLAYCONFIG_DEVICE_INFO_HEADER*)&deviceName);
			//printf("  GDI Device name: ");
			//wprintf(deviceName.viewGdiDeviceName);
			//puts("");
			return deviceName.viewGdiDeviceName;
		}

		/*
			Gets Device Path from Target
			e.g. \\?\DISPLAY#SAM0304#5&9a89472&0&UID33554704#{e6f07b5f-ee97-4a90-b076-33f57bf4eaa7}
		*/
		wchar_t* getMonitorDevicePathFromTarget(LUID adapterId, UINT32 targetId) {
			DISPLAYCONFIG_TARGET_DEVICE_NAME deviceName;
			DISPLAYCONFIG_DEVICE_INFO_HEADER header;
			header.size = sizeof(DISPLAYCONFIG_TARGET_DEVICE_NAME);
			header.adapterId = adapterId;
			header.id = targetId;
			header.type = DISPLAYCONFIG_DEVICE_INFO_GET_TARGET_NAME;
			deviceName.header = header;
			DisplayConfigGetDeviceInfo((DISPLAYCONFIG_DEVICE_INFO_HEADER*)&deviceName);
			//printf("  monitor device path: ");
			//wprintf(deviceName.monitorDevicePath);
			//puts("");
			return deviceName.monitorDevicePath;
		}

		bool CheckDeviceName(int nIndex, wchar_t* strDeviceName)
		{
			static const wchar_t* target = L"DISPLAY";

			int nSrcLen = wcslen(strDeviceName);
			int nTrgLen = wcslen(target);

			int nTargetIndex = 0;
			int nBeginIndex = 0;

			for (int i = 0; i < nSrcLen; i++)
			{
				wchar_t chS = strDeviceName[i];
				wchar_t chT = target[nTargetIndex];

				if (chS == chT)
				{
					nTargetIndex++;

					if (nTargetIndex == nTrgLen)
					{
						nBeginIndex = i + 1;
						break;
					}
				}
			}

			if (nTargetIndex < nTrgLen)
				return false;

			int number = 0;

			for (int i = nBeginIndex; i < nSrcLen; i++)
			{
				wchar_t ch = strDeviceName[i];
				number = number * 10 + (int)(ch - '0');
			}

			return nIndex == number - 1;
		}

		std::wstring GetMonitorName(wchar_t* strDevicePath)
		{
			int len = wcslen(strDevicePath);
			int nIndex = -1;
			std::wstring str = L"";

			for (int i = 0; i < len; i++)
			{
				wchar_t ch = strDevicePath[i];

				if (ch == '#')
				{
					if (nIndex < 0)
						nIndex = i;
					else
						return str;
				}
				else if (nIndex >= 0)
				{
					str += ch;
				}
			}

			return L"";
		}

		int MonitorInfo::GetUID(int nIndex, System::String^% strDeviceName, System::String^% strContainerID, System::Drawing::Point% position, System::Drawing::Size% size)
		{
			UINT32 num_of_paths = 0;
			UINT32 num_of_modes = 0;
			DISPLAYCONFIG_PATH_INFO* displayPaths = NULL;
			DISPLAYCONFIG_MODE_INFO* displayModes = NULL;

			UINT32 num_of_paths2 = 0;
			UINT32 num_of_modes2 = 0;
			DISPLAYCONFIG_PATH_INFO* displayPaths2 = NULL;
			DISPLAYCONFIG_MODE_INFO* displayModes2 = NULL;

			GetDisplayConfigBufferSizes(QDC_ALL_PATHS, &num_of_paths, &num_of_modes);


			// Allocate paths and modes dynamically
			displayPaths = (DISPLAYCONFIG_PATH_INFO*)calloc((int)num_of_paths, sizeof(DISPLAYCONFIG_PATH_INFO));
			displayModes = (DISPLAYCONFIG_MODE_INFO*)calloc((int)num_of_modes, sizeof(DISPLAYCONFIG_MODE_INFO));

			// Query for the information 
			QueryDisplayConfig(QDC_ALL_PATHS, &num_of_paths, displayPaths, &num_of_modes, displayModes, NULL);

			int nUID = 0;
			bool find = false;
			wchar_t* deviceName = 0;
			wchar_t* devicePath = 0;
			std::wstring strMonitorName = L"";

			for (int i = 0; i < num_of_modes; i++) {

				switch (displayModes[i].infoType) {

					// This case is for all sources
				case DISPLAYCONFIG_MODE_INFO_TYPE_SOURCE:
					deviceName = getGDIDeviceNameFromSource(displayModes[i].adapterId, displayModes[i].id);

					if (CheckDeviceName(nIndex, deviceName))
					{
						position.X = displayModes[i].sourceMode.position.x;
						position.Y = displayModes[i].sourceMode.position.y;
						size.Width = displayModes[i].sourceMode.width;
						size.Height = displayModes[i].sourceMode.height;
						find = true;
					}

					//printf("  Source AID: %d\r\n", displayModes[i].adapterId);
					//printf("  Source  ID: %d\r\n", displayModes[i].id);
					break;

					// This case is for all targets
				case DISPLAYCONFIG_MODE_INFO_TYPE_TARGET:
					devicePath = getMonitorDevicePathFromTarget(displayModes[i].adapterId, displayModes[i].id);
					strMonitorName = GetMonitorName(devicePath);
					nUID = (int)displayModes[i].id;
					//getFriendlyNameFromTarget(displayModes[i].adapterId, displayModes[i].id);
					//printf("  Target AID: %d\r\n", displayModes[i].adapterId);
					//printf("  Target  ID: %d\r\n", displayModes[i].id);
					break;

				default:
					//fputs("error", stderr);
					break;
				}

				if (find && nUID > 0)
					break;
			}

			free(displayPaths);
			free(displayModes);

			if (find && nUID > 0)
			{
				strDeviceName = gcnew System::String(strMonitorName.c_str());
				strContainerID = GetContainerID(nUID, strDeviceName);
				return nUID;
			}

			return 0;
		}

		String^ MonitorInfo::GetContainerID(int uid, String^ strMonitorName)
		{
			String^ strBaseKey = L"SYSTEM\\ControlSet001\\Enum\\DISPLAY";
			List<String^>^ keys = ReadRegKeys(strBaseKey);
			String^ strUID = String::Format(L"{0}", uid);

			for each(String^ strKey in keys)
			{
				if (strKey == strMonitorName)
				{
					List<String^>^ subKeys = ReadRegKeys(strBaseKey + L"\\" + strKey);

					for each(String^ strSubKey in subKeys)
					{
						if (strSubKey->EndsWith(strUID))
						{
							return ReadRegValue(strBaseKey + L"\\" + strKey + L"\\" + strSubKey, L"ContainerID");
						}
					}

					break;
				}
			}

			return "";
		}

		List<String^>^ MonitorInfo::ReadRegKeys(String^ strRegPath)
		{
			List<String^>^ keys = gcnew List<String^>();

			try
			{
				RegistryKey^ rkey = Registry::LocalMachine->OpenSubKey(strRegPath);

				if (rkey != nullptr && rkey->SubKeyCount > 0)
				{
					array<String^>^ subKeys = rkey->GetSubKeyNames();

					for each(String^ strSubKey in subKeys)
					{
						keys->Add(strSubKey);
					}
				}

				if (rkey != nullptr)
					rkey->Close();
			}
			catch (Exception^)
			{
				//System.Diagnostics.Trace.WriteLine(e.Message);
			}

			return keys;
		}

		String^ MonitorInfo::ReadRegValue(String^ strRegPath, String^ strKey)
		{
			String^ szResult = "";
			try
			{
				RegistryKey^ rkey = Registry::LocalMachine->OpenSubKey(strRegPath);
				if (rkey == nullptr)
				{
					return "";
				}
				else
				{
					szResult = (String^)rkey->GetValue(strKey);
				}
				if (rkey != nullptr)
					rkey->Close();
			}
			catch (Exception^)
			{
			}
			return szResult;
		}
	}
}
