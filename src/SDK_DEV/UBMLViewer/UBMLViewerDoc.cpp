#include "stdafx.h"
#ifndef SHARED_HANDLERS
#include "UBMLViewer.h"
#endif

#include "UBMLViewerDoc.h"

#include "UBaseDriver.h"
#include "UBaseView.h"
using namespace UnE::Core;

#include <propkey.h>

#ifdef _DEBUG
#define new DEBUG_NEW
#endif


IMPLEMENT_DYNCREATE(CUBMLViewerDoc, CDocument)

BEGIN_MESSAGE_MAP(CUBMLViewerDoc, CDocument)
	ON_COMMAND(ID_FILE_SAVE,	OnFileSave)	
	ON_COMMAND(ID_FILE_SAVE_AS,	OnFileSaveAs)
	ON_COMMAND(ID_FILE_CLOSE,	OnFileClose)
	ON_COMMAND(ID_FILE_OPEN,	OnFileOpen)
END_MESSAGE_MAP()



CUBMLViewerDoc::CUBMLViewerDoc()
{

}

CUBMLViewerDoc::~CUBMLViewerDoc()
{
}

BOOL CUBMLViewerDoc::OnNewDocument()
{
	if (!CDocument::OnNewDocument())
		return FALSE;

	return TRUE;
}

void CUBMLViewerDoc::Serialize(CArchive& ar)
{
	if (ar.IsStoring())
	{
		
	}
	else
	{
		
	}
}

#ifdef SHARED_HANDLERS
void CUBMLViewerDoc::OnDrawThumbnail(CDC& dc, LPRECT lprcBounds)
{
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

void CUBMLViewerDoc::InitializeSearchContent()
{
	CString strSearchContent;
	SetSearchContent(strSearchContent);
}

void CUBMLViewerDoc::SetSearchContent(const CString& value)
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

#ifdef _DEBUG
void CUBMLViewerDoc::AssertValid() const
{
	CDocument::AssertValid();
}

void CUBMLViewerDoc::Dump(CDumpContext& dc) const
{
	CDocument::Dump(dc);
}
#endif //_DEBUG



// DOCUMENT COMMAND REDIRECTION
void CUBMLViewerDoc::OnFileSave() {
	UBaseDriver::Instance().ChangeRenderer(eRS_DIRECT9);
	//AfxGetApp()->GetMainWnd()->SendMessage(WM_COMMAND, WM_OGRE3D_FILE_SAVEEX,  0);
}
void CUBMLViewerDoc::OnFileSaveAs() {
	UBaseDriver::Instance().ChangeRenderer(eRS_DIRECT11);
	//AfxGetApp()->GetMainWnd()->SendMessage(WM_COMMAND, WM_OGRE3D_FILE_SAVEAS,  0);
}
void CUBMLViewerDoc::OnFileClose() {
	

}

void CUBMLViewerDoc::OnFileOpen()
{
	//UBaseDriver::Instance().ChangeRenderer(eRS_OPENGL);
	UBaseDriver::Instance().ChangeRenderer(eRS_DIRECT9);
}
