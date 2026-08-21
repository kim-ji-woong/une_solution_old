// SampleDoc.cpp : CSampleDoc 클래스의 구현
//

#include "stdafx.h"
#include "Sample.h"

#include "SampleDoc.h"

//#ifdef _DEBUG
//#define new DEBUG_NEW
//#endif


// CSampleDoc

IMPLEMENT_DYNCREATE(CSampleDoc, CDocument)

BEGIN_MESSAGE_MAP(CSampleDoc, CDocument)
END_MESSAGE_MAP()


// CSampleDoc 생성/소멸

CSampleDoc::CSampleDoc()
{
	// TODO: 여기에 일회성 생성 코드를 추가합니다.
	Handle(Graphic3d_WNTGraphicDevice) theGraphicDevice = 
		((CSampleApp*)AfxGetApp())->GetGraphicDevice();
	
	myViewer = new V3d_Viewer(theGraphicDevice,(short *) "Visu3D");
	myViewer->SetDefaultLights();
	myViewer->SetLightOn();
	myAISContext =new AIS_InteractiveContext(myViewer);
}

CSampleDoc::~CSampleDoc()
{
}

BOOL CSampleDoc::OnNewDocument()
{
	if (!CDocument::OnNewDocument())
		return FALSE;

	// TODO: 여기에 재초기화 코드를 추가합니다.
	// SDI 문서는 이 문서를 다시 사용합니다.

	return TRUE;
}




// CSampleDoc serialization

void CSampleDoc::Serialize(CArchive& ar)
{
	if (ar.IsStoring())
	{
		// TODO: 여기에 저장 코드를 추가합니다.
	}
	else
	{
		// TODO: 여기에 로딩 코드를 추가합니다.
	}
}


// CSampleDoc 진단

#ifdef _DEBUG
void CSampleDoc::AssertValid() const
{
	CDocument::AssertValid();
}

void CSampleDoc::Dump(CDumpContext& dc) const
{
	CDocument::Dump(dc);
}
#endif //_DEBUG


// CSampleDoc 명령
void CSampleDoc::PreProcess (DisplayType aDisplayType)
{
	if (aDisplayType == No2D3D )
    {   
      GetAISContext()->EraseAll(Standard_False);
	  //Put3DOnTop(); 
    }

    if (aDisplayType == a2DNo3D)
    { 
      //GetISessionContext()->EraseAll();
	  //Put2DOnTop();
    }

    if (aDisplayType != No2D3D && aDisplayType != a2D3D)
    {  
      //Minimize3D();
    }

    if (aDisplayType != a2DNo3D && aDisplayType != a2D3D)
    {  
      //Minimize2D();
    }

    if (aDisplayType == a2D3D)
    {
      GetAISContext()->EraseAll(Standard_False);
      //GetISessionContext()->EraseAll();
      //Put3DOnTop(false); 
      //Put2DOnTop(false);
    }
}
