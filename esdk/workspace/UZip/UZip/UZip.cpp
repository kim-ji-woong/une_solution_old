// 기본 DLL 파일입니다.

#include "stdafx.h"
#include <map>
#include <utility>

#include <iostream>
#include <fstream>

#include <atlcoll.h>
using namespace ATL;
#include "UZip.h"


#include "Poco/AutoPtr.h"
#include "Poco/Zip/Decompress.h"
#include "Poco/Zip/ZipLocalFileHeader.h"
#include "Poco/Zip/ZipArchive.h"
#include "Poco/Path.h"
#include "Poco/File.h"
#include "Poco/Delegate.h"
#include "Poco/Zip/Compress.h"

wchar_t* ToWcharArray(System::String^ str)
{
	if (str == nullptr)
		return 0;

	int nLen = str->Length;
	wchar_t* wstr = new wchar_t[nLen + 1];

	array<wchar_t>^ arr = str->ToCharArray();

	for (int i=0;i<nLen;i++)
		wstr[i] = arr[i];
	wstr[nLen] = 0;

	return wstr;
}

System::String^ ToSystemString(wchar_t* str)
{
	if (str == 0)
		return nullptr;

	System::String^ _str = gcnew System::String(L"");

	for (int i=0;str[i] != 0;i++)
	{
		_str += str[i];
	}

	return _str;
}

int WideToMulti(char* pszDst, const wchar_t* pwzIn, UINT uCodepage)
{
	int nReqLen = (int)WideCharToMultiByte(uCodepage, 0, pwzIn, (int)wcslen(pwzIn), NULL, 0, NULL, NULL);
	int nLen    = (int)WideCharToMultiByte(uCodepage, 0, pwzIn, (int)wcslen(pwzIn), pszDst, nReqLen, NULL, NULL); 
	if(nLen)
		pszDst[nLen] = 0;
	return nLen;
} 



bool Core::UZip::ExtractFile( System::String^ arName, System::String^ path )
{
	Poco::Path outputDir;

	//USES_CONVERSION;
	char buf[4096];
	wchar_t * t = ToWcharArray(path);
	WideToMulti(buf, t, CP_ACP);
	outputDir.parseDirectory(buf);
	delete [] t;

	wchar_t * f = ToWcharArray(arName);
	WideToMulti(buf, f, CP_ACP);
	std::ifstream in(buf, std::ios::binary);
	delete [] f;

	try
	{
		Poco::Zip::Decompress c(in, outputDir);	
		c.decompressAllFiles();
		return true;
	}
	catch (CException* e)
	{
	}
	return false;
}

bool Core::UZip::CompressFile( System::String^ arName, System::Collections::ArrayList^ arFileOrDirList )
{
	if( arFileOrDirList == nullptr)
		return false;
	char buf[4096];
	wchar_t * f = ToWcharArray(arName);
	WideToMulti(buf, f, CP_ACP);
	std::ofstream out(buf, std::ios::binary);
	delete [] f;

	try
	{
		Poco::Zip::Compress c(out, true);

		for(int i = 0; i < arFileOrDirList->Count ; i++)
		{
			System::String^ szPath = arFileOrDirList[i]->ToString();

			Poco::Path aFile;
			wchar_t * t = ToWcharArray(szPath);
			WideToMulti(buf, t, CP_ACP);
			aFile.parse(buf);
			c.addFile(aFile, aFile.getFileName());
			delete [] t;
		}		
		c.close(); 
		return true;
	}
	catch (CException* e)
	{
	}
	return false;
}


bool Core::UZip::CompressRecusive( System::String^ arName, System::Collections::ArrayList^ arFileOrDirList )
{
	if( arFileOrDirList == nullptr)
		return false;
	char buf[4096];
	wchar_t * f = ToWcharArray(arName);
	WideToMulti(buf, f, CP_ACP);
	std::ofstream out(buf, std::ios::binary);
	delete [] f;

	try
	{
		Poco::Zip::Compress c(out, true);

		for(int i = 0; i < arFileOrDirList->Count ; i++)
		{
			System::String^ szPath = arFileOrDirList[i]->ToString();

			Poco::Path aFile;
			wchar_t * t = ToWcharArray(szPath);
			WideToMulti(buf, t, CP_ACP);
			aFile.parse(buf);
			//c.addFile(aFile, aFile.getFileName());
			c.addRecursive(aFile);
			delete [] t;
		}		
		c.close(); 
		return true;
	}
	catch (CException* e)
	{
	}
	return false;
}

