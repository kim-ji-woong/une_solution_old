// UBMLSample.cpp : 콘솔 응용 프로그램에 대한 진입점을 정의합니다.
//

#include "stdafx.h"
#include <UBML/Writer.h>
#include <UBML/Reader.h>
#include <UBML/UData.h>
#include <string.h>
#include <string>
#include <iostream>
#include <Objbase.h>
#include "MeshFactory.h"
#include <math.h>

using namespace std;
using namespace UnE::UBML;

// Sample Data, int + short array(크기 고정, 배열 크기 10) + float + char array(크기 고정되지 않음)
// [AA123]
// LONG SHORT_ARR_FIXED 10 FLOAT CHAR_ARR
bool WriteBinary(wchar_t* strFilePath, bool toXML = false)
{
	Element* pElement = new Element;
	pElement->MakeTag(L"AB", 123);

	int nData1 = 10;
	short arr2[10] = {1, 2, 3, 4, -5, 6, 7, 8, 9, 10};
	float fData3 = 0.01f;
	char* str4 = "1부터 10까지";

	Segment* seg[4] = {new Segment, new Segment, new Segment, new Segment};

	seg[0]->SetType(__LONG);
	seg[1]->SetType(__SHORT_ARR);
	seg[2]->SetType(__FLOAT);
	seg[3]->SetType(__CHAR_ARR);

	seg[0]->AddData(nData1);
	
	for (int i=0;i<10;i++)
	{
		seg[1]->AddData(arr2[i]);
	}

	seg[2]->AddData(fData3);
	
	int nLen = (int)strlen(str4);

	for (int i=0;i<nLen;i++)
	{
		seg[3]->AddData(str4[i]);
	}

	for (int i=0;i<4;i++)
	{
		pElement->AddData(seg[i]);
	}

	Writer writer;
	writer.AddElement(pElement);
	bool isSuccess = writer.WriteFile(strFilePath);

	if (!isSuccess)
		return false;

	if (toXML)
	{
		return writer.ToXML(L"writer.xml");
	}

	return isSuccess;
}

template <class T>
void PrintType(const Segment* pSegment, unsigned int nArrayCount)
{
	for (unsigned int i=0;i<nArrayCount;i++)
	{
		if (i > 0) cout << " " << *(T*)pSegment->GetData(i);
		else cout << *(T*)pSegment->GetData(i);
	}

	cout << endl;
}

template <class T, class S>
void PrintType2(const Segment* pSegment, unsigned int nArrayCount)
{
	for (unsigned int i=0;i<nArrayCount;i++)
	{
		if (i > 0) cout << " " << (S)*(T*)pSegment->GetData(i);
		else cout << (S)*(T*)pSegment->GetData(i);
	}

	cout << endl;
}

void PrintArray(const Segment* pSegment, unsigned int nArrayCount)
{
	switch (pSegment->GetType())
	{
	case __BYTE:
	case __BYTE_ARR:
	case __BYTE_ARR_FIXED:
		PrintType2<char, int>(pSegment, nArrayCount);
		break;

	case __UBYTE:
	case __UBYTE_ARR:
	case __UBYTE_ARR_FIXED:
		PrintType2<unsigned char, int>(pSegment, nArrayCount);
		break;

	case __SHORT:
	case __SHORT_ARR:
	case __SHORT_ARR_FIXED:
		PrintType<short>(pSegment, nArrayCount);
		break;

	case __USHORT:
	case __USHORT_ARR:
	case __USHORT_ARR_FIXED:
		PrintType<unsigned short>(pSegment, nArrayCount);
		break;

	case __LONG:
	case __LONG_ARR:
	case __LONG_ARR_FIXED:
		PrintType<int>(pSegment, nArrayCount);
		break;

	case __ULONG:
	case __ULONG_ARR:
	case __ULONG_ARR_FIXED:
		PrintType<int>(pSegment, nArrayCount);
		break;

	case __LONGLONG:
	case __LONGLONG_ARR:
	case __LONGLONG_ARR_FIXED:
		PrintType<__int64>(pSegment, nArrayCount);
		break;

	case __ULONGLONG:
	case __ULONGLONG_ARR:
	case __ULONGLONG_ARR_FIXED:
		PrintType<__int64>(pSegment, nArrayCount);
		break;

	case __FLOAT:
	case __FLOAT_ARR:
	case __FLOAT_ARR_FIXED:
		PrintType<float>(pSegment, nArrayCount);
		break;

	case __DOUBLE:
	case __DOUBLE_ARR:
	case __DOUBLE_ARR_FIXED:
		PrintType<double>(pSegment, nArrayCount);
		break;

	case __BOOL:
	case __BOOL_ARR:
	case __BOOL_ARR_FIXED:
		PrintType<bool>(pSegment, nArrayCount);
		break;

	case __CHAR:
	case __CHAR_ARR:
	case __CHAR_ARR_FIXED:
		for (unsigned int i=0;i<nArrayCount;i++)
		{
			cout << *(char*)pSegment->GetData(i);
		}
		cout << endl;
		break;

	case __WCHAR:
	case __WCHAR_ARR:
	case __WCHAR_ARR_FIXED:
		for (unsigned int i=0;i<nArrayCount;i++)
		{
			wcout << *(char*)pSegment->GetData(i);
		}
		wcout << endl;
		break;
	}
}

bool PrintSegment(const Segment* pSegment, int nDepthCount)
{
	unsigned int nDataCount = pSegment->GetDataCount();

	bool isArray;
	wstring strTypeName = pSegment->GetTypeTagString(isArray);

	for (int i=0;i<nDepthCount;i++) wcout << L"\t";

	if (isArray)
		wcout << strTypeName << L" Array(" << nDataCount << L") : ";
	else wcout << strTypeName << L" : ";

	PrintArray(pSegment, nDataCount);

	return true;
}

bool PrintElement(const Element* pElement, int nDepthCount)
{
	wstring strTag = pElement->GetTagString();
	for (int i=0;i<nDepthCount;i++) wcout << L"\t";
	wcout << L"[" << strTag << L"]" << endl;
		
	unsigned int nDataCount = pElement->GetDataCount();

	for (unsigned int j=0;j<nDataCount;j++)
	{
		const UData* pData = pElement->GetData(j);
		if (pData == 0) continue;

		if (pData->GetClassType() == UData::ELEMENT)
		{
			if (!PrintElement((const Element*)pData, nDepthCount + 1))
				return false;
		}
		else
		{
			if (!PrintSegment((const Segment*)pData, nDepthCount + 1))
				return false;
		}
	}

	return true;
}

bool ReadBinary(wchar_t* strFilePath, bool toXML = false)
{
	Reader reader;
	if (!reader.ReadFile(strFilePath))
		return false;

	//bool isArray;
	unsigned int nElementCount = reader.GetElementCount();

	for (unsigned int i=0;i<nElementCount;i++)
	{
		const Element* pElement = reader.GetElement(i);
		if (pElement == 0) continue;

		wstring strTag = pElement->GetTagString();
		wcout << L"[" << strTag << L"]" << endl;
		
		unsigned int nDataCount = pElement->GetDataCount();

		for (unsigned int j=0;j<nDataCount;j++)
		{
			const UData* pData = pElement->GetData(j);
			if (pData == 0) continue;

			if (pData->GetClassType() == UData::ELEMENT)
			{
				if (!PrintElement((const Element*)pData, 1))
					return false;
			}
			else
			{
				if (!PrintSegment((const Segment*)pData, 1))
					return false;
			}

			/*unsigned int nDataCount = pSegment->GetDataCount();

			wstring strTypeName = pSegment->GetTypeTagString(isArray);

			if (isArray)
				wcout << L"\t" << strTypeName << L" Array(" << nDataCount << L") : ";
			else wcout << L"\t" << strTypeName << L" : ";

			PrintArray(pSegment, nDataCount);*/
		}
	}

	if (toXML)
	{
		return reader.ToXML(L"reader.xml");
	}

	return true;
}

void SetMeshHeader(MeshFactory& rFactory)
{
	rFactory.SetBuildingName(L"U&E 서울 사무실");
	rFactory.SetBuildeingAddr(L"서울시 용산구 서계동 229-8");
}

Vertices* g_pVertices3D = new Vertices;
Vertices* g_pVertices2D = new Vertices;

void SetMeshVertex(MeshFactory& rFactory)
{
	float fNormal = (float)(1.0 / sqrt(3.0));

	Vertex cubeCoord[8] = {
		Vertex(0.0f, 0.0f, 0.0f, -fNormal, -fNormal, -fNormal), Vertex(0.0f, 100.0f, 0.0f, -fNormal, fNormal, -fNormal), 
		Vertex(100.0f, 100.0f, 0.0f, fNormal, fNormal, -fNormal), Vertex(100.0f, 0.0f, 0.0f, fNormal, -fNormal, -fNormal),
		Vertex(0.0f, 0.0f, 100.0f, -fNormal, -fNormal, fNormal), Vertex(0.0f, 100.0f, 100.0f, -fNormal, fNormal, fNormal), 
		Vertex(100.0f, 100.0f, 100.0f, fNormal, fNormal, fNormal), Vertex(100.0f, 0.0f, 100.0f, fNormal, -fNormal, fNormal)};

	g_pVertices2D->m_nID = 1;
	g_pVertices3D->m_nID = 2;

	for (int i=0;i<4;i++)
	{
		g_pVertices2D->m_vecVertex.push_back(cubeCoord[i]);
		g_pVertices3D->m_vecVertex.push_back(cubeCoord[i]);
	}

	for (int i=4;i<8;i++)
	{
		g_pVertices3D->m_vecVertex.push_back(cubeCoord[i]);
	}

	rFactory.Add2DVertices(g_pVertices2D, false);
	rFactory.Add3DVertices(g_pVertices3D, false);
}

Texture* g_arrTexture[2] = {new Texture, new Texture};

void SetMeshTexture(MeshFactory& rFactory)
{
	std::wstring strPath[2] = {L"다빈치/모나리자.jpg", L"고흐/해바라기.jpg"};

	for (int i=0;i<2;i++)
	{
		g_arrTexture[i]->m_nTextureID = 3 + i;
		g_arrTexture[i]->m_strImagePath = strPath[i];
		rFactory.AddTexture(g_arrTexture[i]);
	}
}

Material* g_arrMaterial[6] = {new Material, new Material, new Material, new Material, new Material, new Material};

void SetMeshMaterial(MeshFactory& rFactory)
{
	for (int i=0;i<6;i++)
	{
		g_arrMaterial[i]->m_nMaterialID = 5 + i;
		g_arrMaterial[i]->m_arrAmbientColor[0] = g_arrMaterial[i]->m_arrAmbientColor[1] = g_arrMaterial[i]->m_arrAmbientColor[2] = g_arrMaterial[i]->m_arrAmbientColor[3] = 1.0f;

		g_arrMaterial[i]->m_arrDiffuseColor[0] = (rand() % 255) / 255.0f;
		g_arrMaterial[i]->m_arrDiffuseColor[1] = (rand() % 255) / 255.0f;
		g_arrMaterial[i]->m_arrDiffuseColor[2] = (rand() % 255) / 255.0f;
		g_arrMaterial[i]->m_arrDiffuseColor[3] = 1.0f;

		g_arrMaterial[i]->m_arrEmissiveColor[0] = (rand() % 255) / 255.0f;
		g_arrMaterial[i]->m_arrEmissiveColor[1] = (rand() % 255) / 255.0f;
		g_arrMaterial[i]->m_arrEmissiveColor[2] = (rand() % 255) / 255.0f;
		g_arrMaterial[i]->m_arrEmissiveColor[3] = 1.0f;

		g_arrMaterial[i]->m_arrSpecularColor[0] = (rand() % 255) / 255.0f;
		g_arrMaterial[i]->m_arrSpecularColor[1] = (rand() % 255) / 255.0f;
		g_arrMaterial[i]->m_arrSpecularColor[2] = (rand() % 255) / 255.0f;
		g_arrMaterial[i]->m_arrSpecularColor[3] = 1.0f;

		g_arrMaterial[i]->m_nShininess = rand() % 128;
		g_arrMaterial[i]->m_pTexture = g_arrTexture[i % 2];

		g_arrMaterial[i]->m_useSpecular = rand() % 2 ? true : false;
		g_arrMaterial[i]->m_useEmissive = rand() % 2 ? true : false;
		g_arrMaterial[i]->m_useTexture = rand() % 2 ? true : false;

		rFactory.AddMaterial(g_arrMaterial[i], false);
	}
}

void SetFaceTexCoord(Face* arrFace)
{
	arrFace[0].v1u = 1.0f;
	arrFace[0].v1v = 1.0f;
	arrFace[0].v2u = 0.0f;
	arrFace[0].v2v = 1.0f;
	arrFace[0].v3u = 1.0f;
	arrFace[0].v3v = 0.0f;

	arrFace[1].v1u = 1.0f;
	arrFace[1].v1v = 0.0f;
	arrFace[1].v2u = 0.0f;
	arrFace[1].v2v = 1.0f;
	arrFace[1].v3u = 0.0f;
	arrFace[1].v3v = 0.0f;
}

Mesh* g_pMesh = new Mesh;

void SetMeshFace(MeshFactory& rFactory)
{
	for( int j= 0 ; j< 10000;j++)
	{
		g_pMesh = new Mesh;
		g_pMesh->m_nMeshID = j+11;
		g_pMesh->m_p2DVertices = g_pVertices2D;
		g_pMesh->m_p3DVertices = g_pVertices3D;

		Face faceLeft[2], faceRight[2], faceFront[2], faceBack[2], faceBottom[2], faceTop[2];

		faceLeft[0].v1 = 0;
		faceLeft[0].v2 = 1;
		faceLeft[0].v3 = 4;
		faceLeft[1].v1 = 4;
		faceLeft[1].v2 = 1;
		faceLeft[1].v3 = 5;

		faceRight[0].v1 = 2;
		faceRight[0].v2 = 3;
		faceRight[0].v3 = 6;
		faceRight[1].v1 = 6;
		faceRight[1].v2 = 3;
		faceRight[1].v3 = 7;

		faceFront[0].v1 = 3;
		faceFront[0].v2 = 0;
		faceFront[0].v3 = 7;
		faceFront[1].v1 = 7;
		faceFront[1].v2 = 0;
		faceFront[1].v3 = 4;

		faceBack[0].v1 = 1;
		faceBack[0].v2 = 2;
		faceBack[0].v3 = 5;
		faceBack[1].v1 = 5;
		faceBack[1].v2 = 2;
		faceBack[1].v3 = 6;

		faceBottom[0].v1 = 2;
		faceBottom[0].v2 = 1;
		faceBottom[0].v3 = 3;
		faceBottom[1].v1 = 3;
		faceBottom[1].v2 = 1;
		faceBottom[1].v3 = 0;

		faceTop[0].v1 = 7;
		faceTop[0].v2 = 4;
		faceTop[0].v3 = 6;
		faceTop[1].v1 = 6;
		faceTop[1].v2 = 4;
		faceTop[1].v3 = 5;

		SetFaceTexCoord(faceLeft);
		SetFaceTexCoord(faceRight);
		SetFaceTexCoord(faceFront);
		SetFaceTexCoord(faceBack);
		SetFaceTexCoord(faceTop);
		SetFaceTexCoord(faceBottom);

		for (int i=0;i<2;i++)
		{
			g_pMesh->m_vec3DFace.push_back(faceLeft[i]);
			g_pMesh->m_vec3DFace.push_back(faceRight[i]);
			g_pMesh->m_vec3DFace.push_back(faceFront[i]);
			g_pMesh->m_vec3DFace.push_back(faceBack[i]);
			g_pMesh->m_vec3DFace.push_back(faceTop[i]);
			g_pMesh->m_vec3DFace.push_back(faceBottom[i]);
		}

		Face arrFace2D[2];

		arrFace2D[0].v1 = 3;
		arrFace2D[0].v2 = 0;
		arrFace2D[0].v3 = 2;
		arrFace2D[1].v1 = 2;
		arrFace2D[1].v2 = 0;
		arrFace2D[1].v3 = 1;

		SetFaceTexCoord(arrFace2D);

		g_pMesh->m_vec2DFace.push_back(arrFace2D[0]);
		g_pMesh->m_vec2DFace.push_back(arrFace2D[1]);
		rFactory.AddMesh(g_pMesh);
	}
	
}

::Layer* g_pLayer = new ::Layer();

void SetMeshLayer(MeshFactory& rFactory)
{
	g_pLayer->m_nLayerID = 12;
	g_pLayer->m_layerType = ::Layer::ObjectLayer;
	g_pLayer->m_strLayerName = L"Default Layer";
	g_pLayer->m_pMaterial = g_arrMaterial[0];

	rFactory.AddLayer(g_pLayer);
}

void SetMeshObject(MeshFactory& rFactory)
{
	::Object* pObject = new ::Object();

	pObject->m_nObjectID = 13;
	pObject->m_objType = ::Object::Facility;
	pObject->m_pLayer = g_pLayer;
	pObject->m_strObjectName = L"공간박스";
	
	::Object::ObjectMesh mesh;

	mesh.m_pMesh = g_pMesh;
	mesh.m_arrPosition[0] = mesh.m_arrPosition[1] = mesh.m_arrPosition[2] = 0.0f;

	mesh.m_arrLocalAxis[0] = mesh.m_arrLocalAxis[4] = mesh.m_arrLocalAxis[8] = 1.0f;
	mesh.m_arrLocalAxis[1] = mesh.m_arrLocalAxis[2] = mesh.m_arrLocalAxis[3] = mesh.m_arrLocalAxis[5] = mesh.m_arrLocalAxis[6] = mesh.m_arrLocalAxis[7] = 0.0f;

	mesh.m_arrScale[0] = mesh.m_arrScale[1] = mesh.m_arrScale[2] = 1.0f;

	pObject->m_vecMesh.push_back(mesh);
	rFactory.AddObject(pObject);
}

bool WriteMesh(const wchar_t* strFilePath)
{
	MeshFactory factory;

	SetMeshHeader(factory);
	SetMeshVertex(factory);
	SetMeshTexture(factory);
	SetMeshMaterial(factory);
	SetMeshFace(factory);
	SetMeshLayer(factory);
	SetMeshObject(factory);

	return factory.Write(strFilePath);
}

bool ReadMesh(const wchar_t* strFilePath)
{
	Reader reader;
	MeshFactory factory;
	bool isSuccess = factory.Read(strFilePath, reader);

	if (isSuccess)
	{
		reader.ToXML(L"sample.xml");
	}
	else
	{
		int i = 0;
		i++;
	}

	return isSuccess;
}


//////////////////////////////////////////////////////////////////////////

// FILE: Stopwatch.h

// DESC: Implementation of CStopwatch class, to measure C++ code

//       performances.

//////////////////////////////////////////////////////////////////////////





#pragma once







//========================================================================

// CStopwatch

//

// Class used to measure performances of C++ code.

// (This class uses high-resolution performance counters.)

//

// By Giovanni Dicanio <giovanni.dicanio@gmail.com>

//

// 2010, January 11th

//

// ----------------------------------------------------------------------

//

// To use this class, follow this pattern:

//

// 1. Create an instance of the class nearby the code you want to measure.

//

// 2. Call Start() method immediately before the code to measure.

//

// 3. Call Stop() method immediately after the code to measure.

//

// 4. Call ElapsedTimeSec() or ElapsedTimeMillisec() methods to get the 

//    elapsed time (in seconds or milliseconds, respectively).

//

//

//========================================================================

class CStopwatch

{

public:



	// Does some initialization to get consistent results for all tests.

	CStopwatch()

		: m_startCount(0), m_elapsedTimeSec(0.0)

	{

		//

		// Confine the test to run on a single processor,

		// to get consistent results for all tests.

		//

		SetThreadAffinityMask(GetCurrentThread(), 1);

		SetThreadIdealProcessor(GetCurrentThread(), 0);

		Sleep(1);

	}





	// Starts measuring performance

	// (to be called before the block of code to measure).

	void Start()

	{

		// Clear total elapsed time 

		// (it is a spurious value until Stop() is called)

		m_elapsedTimeSec = 0.0;



		// Start ticking

		m_startCount = Counter();

	}



	// Stops measuring performance

	// (to be called after the block of code to measure).

	void Stop()

	{

		// Stop ticking

		LONGLONG stopCount = Counter();



		// Calculate total elapsed time since Start() was called;

		// time is measured in seconds

		m_elapsedTimeSec = (stopCount - m_startCount) * 1.0 / Frequency();



		// Clear start count (it is spurious information)

		m_startCount = 0;

	}



	// Returns total elapsed time (in seconds) in Start-Stop interval.

	double ElapsedTimeSec() const

	{

		// Total elapsed time was calculated in Stop() method.

		return m_elapsedTimeSec;

	}



	// Returns total elapsed time (in milliseconds) in Start-Stop interval.

	double ElapsedTimeMilliSec() const

	{

		// Total elapsed time was calculated in Stop() method.

		return m_elapsedTimeSec * 1000.0;

	}







	//--------------------------------------------------------------------

	// IMPLEMENTATION

	//--------------------------------------------------------------------

private:



	//

	// *** Data Members ***

	//



	// The value of counter on start ticking

	LONGLONG m_startCount;



	// The time (in seconds) elapsed in Start-Stop interval

	double m_elapsedTimeSec;







	//

	// *** Helper Methods ***

	//



	// Returns current value of high-resolution performance counter.

	LONGLONG Counter() const

	{

		LARGE_INTEGER counter;

		QueryPerformanceCounter(&counter);

		return counter.QuadPart;

	}



	// Returns the frequency (in counts per second) of the 

	// high-resolution performance counter.

	LONGLONG Frequency() const

	{

		LARGE_INTEGER frequency;

		QueryPerformanceFrequency(&frequency);

		return frequency.QuadPart;

	}





	//

	// *** Ban copy ***

	//

private:

	CStopwatch(const CStopwatch &);

	CStopwatch & operator=(const CStopwatch &);

};

int _tmain(int argc, _TCHAR* argv[])
{
	::CoInitialize(0);

	/*if (WriteBinary(L"sample.ubml", true))
		cout << "sample.ubml 쓰기 성공" << endl;
	else
		cout << "sample.ubml 쓰기 실패" << endl;

	if (ReadBinary(L"sample.ubml", true))
		cout << "sample.ubml 읽기 성공" << endl;
	else
		cout << "sample.ubml 읽기 실패" << endl;*/
	
	if (WriteMesh(L"sample.mesh"), true)
		cout << "sample.mesh 쓰기 성공" << endl;
	else
		cout << "sample.mesh 쓰기 실패" << endl;
	CStopwatch watch;
	watch.Start();
	if (ReadMesh(L"sample.mesh"))
		cout << "smaple.mesh 읽기 성공" << endl;
	else
		cout << "smaple.mesh 읽기 실패" << endl;

	watch.Stop();
	double time = watch.ElapsedTimeSec();
	FILE * fout = fopen("c:\\temp\\time.txt", "wt");
	fprintf(fout, "%lf", time);
	fclose(fout);
	return 0;
}

