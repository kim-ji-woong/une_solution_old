// G2ClientDoc.cpp : implementation of the CG2ClientDoc class
//

#include "stdafx.h"
#include "G2Client.h"

#include "G2ClientDoc.h"

#ifdef _DEBUG
#define new DEBUG_NEW
#endif


// CG2ClientDoc

IMPLEMENT_DYNCREATE(CG2ClientDoc, CDocument)

BEGIN_MESSAGE_MAP(CG2ClientDoc, CDocument)
END_MESSAGE_MAP()


// CG2ClientDoc construction/destruction

CG2ClientDoc::CG2ClientDoc()
{
	// TODO: add one-time construction code here

}

CG2ClientDoc::~CG2ClientDoc()
{
}

BOOL CG2ClientDoc::OnNewDocument()
{
	if (!CDocument::OnNewDocument())
		return FALSE;

	// TODO: add reinitialization code here
	// (SDI documents will reuse this document)

	return TRUE;
}




// CG2ClientDoc serialization

void CG2ClientDoc::Serialize(CArchive& ar)
{
	if (ar.IsStoring())
	{
		// TODO: add storing code here
	}
	else
	{
		// TODO: add loading code here
	}
}


// CG2ClientDoc diagnostics

#ifdef _DEBUG
void CG2ClientDoc::AssertValid() const
{
	CDocument::AssertValid();
}

void CG2ClientDoc::Dump(CDumpContext& dc) const
{
	CDocument::Dump(dc);
}
#endif //_DEBUG


// CG2ClientDoc commands
