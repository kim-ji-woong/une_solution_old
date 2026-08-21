// G2ClientDoc.h : interface of the CG2ClientDoc class
//


#pragma once


class CG2ClientDoc : public CDocument
{
protected: // create from serialization only
	CG2ClientDoc();
	DECLARE_DYNCREATE(CG2ClientDoc)

// Attributes
public:

// Operations
public:

// Overrides
public:
	virtual BOOL OnNewDocument();
	virtual void Serialize(CArchive& ar);

// Implementation
public:
	virtual ~CG2ClientDoc();
#ifdef _DEBUG
	virtual void AssertValid() const;
	virtual void Dump(CDumpContext& dc) const;
#endif

protected:

// Generated message map functions
protected:
	DECLARE_MESSAGE_MAP()
};


