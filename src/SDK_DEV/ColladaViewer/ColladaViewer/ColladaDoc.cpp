
#include "stdafx.h"

#ifndef SHARED_HANDLERS
#include "ColladaApp.h"
#endif

#include "ColladaDoc.h"

#include "AssimpView.h"
using namespace AssimpView;

namespace AssimpView
{
	extern void OpenAsset();

	extern COLORREF g_aclCustomColors[16] /*= {0}*/;
}

#include <propkey.h>

#ifdef _DEBUG
#define new DEBUG_NEW
#endif


IMPLEMENT_DYNCREATE(ColladaDoc, CDocument)

BEGIN_MESSAGE_MAP(ColladaDoc, CDocument)
	ON_COMMAND(ID_FILE_OPEN, &ColladaDoc::OnFileOpen)
END_MESSAGE_MAP()


ColladaDoc::ColladaDoc()
{
	
}

ColladaDoc::~ColladaDoc()
{
}

BOOL ColladaDoc::OnNewDocument()
{
	if (!CDocument::OnNewDocument())
		return FALSE;
	//OpenAsset();
	return TRUE;
}


void ColladaDoc::OnFileOpen()
{
	OpenAsset();
}


BOOL ColladaDoc::OnOpenDocument(LPCTSTR lpszPathName)
{
	if (!CDocument::OnOpenDocument(lpszPathName))
		return FALSE;
	

	return TRUE;
}

void ColladaDoc::OnFileSave()
{
		
}



// ColladaDoc serialization

void ColladaDoc::Serialize(CArchive& ar)
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


#ifdef SHARED_HANDLERS
// 축소판 그림을 지원합니다.
void ColladaDoc::OnDrawThumbnail(CDC& dc, LPRECT lprcBounds)
{
	// 문서의 데이터를 그리려면 이 코드를 수정하십시오.
	dc.FillSolidRect(lprcBounds, RGB(255, 255, 255));

	CString strText = _T("TODO: implement thumbnail drawing here");
	LOGFONT lf;

	CFont* pDefaultGUIFont = CFont::FromHandle((HFONT) GetStockObject(DEFAULT_GUI_FONT));
	pDefaultGUIFont->GetLogFont(&lf);
	lf.lfHeight = 36;

	CFont fontDraw;
	fontDraw.CreateFontIndirect(&lf);

	CFont* pOldFont = dc.SelectObject(&fontDraw);
	dc.DrawText(strText, lprcBounds, DT_CENTER | DT_WORDBREAK);
	dc.SelectObject(pOldFont);
}

// 검색 처리기를 지원합니다.
void ColladaDoc::InitializeSearchContent()
{
	CString strSearchContent;
	// 문서의 데이터에서 검색 콘텐츠를 설정합니다.
	// 콘텐츠 부분은 ";"로 구분되어야 합니다.

	// 예: strSearchContent = _T("point;rectangle;circle;ole object;");
	SetSearchContent(strSearchContent);
}

void ColladaDoc::SetSearchContent(const CString& value)
{
	if (value.IsEmpty())
	{
		RemoveChunk(PKEY_Search_Contents.fmtid, PKEY_Search_Contents.pid);
	}
	else
	{
		CMFCFilterChunkValueImpl *pChunk = NULL;
		ATLTRY(pChunk = new CMFCFilterChunkValueImpl);
		if (pChunk != NULL)
		{
			pChunk->SetTextValue(PKEY_Search_Contents, value, CHUNK_TEXT);
			SetChunkValue(pChunk);
		}
	}
}

#endif // SHARED_HANDLERS

// ColladaDoc 진단

#ifdef _DEBUG
void ColladaDoc::AssertValid() const
{
	CDocument::AssertValid();
}

void ColladaDoc::Dump(CDumpContext& dc) const
{
	CDocument::Dump(dc);
}
#endif //_DEBUG


//-------------------------------------------------------------------------------
// Let the user choose the baclground color for the viewer
//-------------------------------------------------------------------------------
void ColladaDoc::ChooseBGColor()
{
	RegSetValueExA(GetRootReg(),"LastSkyBoxSrc",0,REG_SZ,(const BYTE*)"",MAX_PATH);
	RegSetValueExA(GetRootReg(),"LastTextureSrc",0,REG_SZ,(const BYTE*)"",MAX_PATH);

	D3DCOLOR clrColor;
	DisplayColorDialog(&clrColor);
	CBackgroundPainter::Instance().SetColor(clrColor);

	RegSetValueExA(GetRootReg(),"Color",0,REG_DWORD,(const BYTE*)&clrColor,4);
	return;
}

//-------------------------------------------------------------------------------
// Let the user choose a color in a windows standard color dialog
//-------------------------------------------------------------------------------
void ColladaDoc::DisplayColorDialog(D3DCOLOR* pclrResult)
{
	CHOOSECOLOR clr;
	clr.lStructSize = sizeof(CHOOSECOLOR);
	clr.hwndOwner = g_hView;
	clr.Flags = CC_RGBINIT | CC_FULLOPEN;
	clr.rgbResult = RGB((*pclrResult >> 16) & 0xff,(*pclrResult >> 8) & 0xff,*pclrResult & 0xff);
	clr.lpCustColors = g_aclCustomColors;
	clr.lpfnHook = NULL;
	clr.lpTemplateName = NULL;
	clr.lCustData = NULL;

	ChooseColor(&clr);
	*pclrResult = D3DCOLOR_ARGB(0xFF,GetRValue(clr.rgbResult),GetGValue(clr.rgbResult),GetBValue(clr.rgbResult));
	return;
}

//-------------------------------------------------------------------------------
// Let the user choose a color in a windows standard color dialog
//-------------------------------------------------------------------------------
void ColladaDoc::DisplayColorDialog(D3DXVECTOR4* pclrResult)
{
	CHOOSECOLOR clr;
	clr.lStructSize = sizeof(CHOOSECOLOR);
	clr.hwndOwner = g_hView;
	clr.Flags = CC_RGBINIT | CC_FULLOPEN;
	clr.rgbResult = RGB(clamp<unsigned char>(pclrResult->x * 255.0f),
		clamp<unsigned char>(pclrResult->y * 255.0f),
		clamp<unsigned char>(pclrResult->z * 255.0f));
	clr.lpCustColors = g_aclCustomColors;
	clr.lpfnHook = NULL;
	clr.lpTemplateName = NULL;
	clr.lCustData = NULL;

	ChooseColor(&clr);

	pclrResult->x = GetRValue(clr.rgbResult) / 255.0f;
	pclrResult->y = GetGValue(clr.rgbResult) / 255.0f;
	pclrResult->z = GetBValue(clr.rgbResult) / 255.0f;

	return;
}
