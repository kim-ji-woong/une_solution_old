#include "StdAfx.h"
#include "UCore3DDoc.h"


#ifdef _DEBUG
#define new DEBUG_NEW
#endif


IMPLEMENT_DYNCREATE(UCore3DDoc, CDocument)

BEGIN_MESSAGE_MAP(UCore3DDoc, CDocument)
	ON_COMMAND(ID_FILE_SAVE,	OnFileSave)	
	ON_COMMAND(ID_FILE_SAVE_AS,	OnFileSaveAs)
	ON_COMMAND(ID_FILE_CLOSE,	OnFileClose)
END_MESSAGE_MAP()


UCore3DDoc::UCore3DDoc()
{
	

}

UCore3DDoc::~UCore3DDoc()
{
}


#ifdef _DEBUG
void UCore3DDoc::AssertValid() const
{
	CDocument::AssertValid();
}

void UCore3DDoc::Dump(CDumpContext& dc) const
{
	CDocument::Dump(dc);
}
#endif //_DEBUG



// DOCUMENT COMMAND REDIRECTION
void UCore3DDoc::OnFileSave() {
	AfxGetApp()->GetMainWnd()->SendMessage(WM_COMMAND, WM_OGRE3D_FILE_SAVEEX,  0);
}
void UCore3DDoc::OnFileSaveAs() {
	AfxGetApp()->GetMainWnd()->SendMessage(WM_COMMAND, WM_OGRE3D_FILE_SAVEAS,  0);
}
void UCore3DDoc::OnFileClose() {
	AfxGetApp()->GetMainWnd()->SendMessage(WM_COMMAND, WM_OGRE3D_FILE_CLOSEEX,  0);
}