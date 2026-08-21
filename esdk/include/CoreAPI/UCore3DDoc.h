#pragma once

#include "CoreAPI.h"


#define WM_OGRE3D_VIEW_AABB			(WM_USER + 1000)
#define WM_OGRE3D_USE_TRACKBALL		(WM_USER + 1001)


#define WM_OGRE3D_FILE_SAVEEX		(WM_USER + 2000)
#define WM_OGRE3D_FILE_SAVEAS		(WM_USER + 2001)
#define WM_OGRE3D_FILE_CLOSEEX		(WM_USER + 2002)


class CORE_API UCore3DDoc : public CDocument
{
protected: 
	UCore3DDoc();
	DECLARE_DYNCREATE(UCore3DDoc)

	DECLARE_MESSAGE_MAP()

public:
	virtual ~UCore3DDoc();

#ifdef _DEBUG
	virtual void AssertValid() const;
	virtual void Dump(CDumpContext& dc) const;
#endif

	afx_msg void OnFileSave();
	afx_msg void OnFileSaveAs();
	afx_msg void OnFileClose();

};

