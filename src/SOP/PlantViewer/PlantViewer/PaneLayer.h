#pragma once
#include "afxcmn.h"
#include "Resource.h"
#include "MFCHelper\Grid\GridCtrl.h"
#include "GridCtrlEx.h"

// PaneLayer 대화 상자입니다.

// typedef struct _GRID_INFO
// {
// 	int nImage;
// 	CString strName;
// }Grid_info;
// 
// typedef std::vector<Grid_info> GridList;

class PaneLayer : public CPaneDialog
{
	DECLARE_DYNAMIC(PaneLayer)

public:
	PaneLayer(CWnd* pParent = NULL);   // 표준 생성자입니다.
	virtual ~PaneLayer();

// 대화 상자 데이터입니다.
	enum { IDD = IDD_PANELAYER };

protected:
	virtual void DoDataExchange(CDataExchange* pDX);    // DDX/DDV 지원입니다.
	//afx_msg void OnGridClick(NMHDR *pNotifyStruct, LRESULT* pResult);

	DECLARE_MESSAGE_MAP()

public:
	afx_msg void OnSize(UINT nType, int cx, int cy);
	afx_msg int OnCreate(LPCREATESTRUCT lpCreateStruct);

	void InitGrid();
	void AddItem();
	void DeleteItem();
protected:
	CImageList m_imageList;

	CGridCtrlEx m_gridLayer;
	SYSTEMTIME m_time;
	
private:
	CStatic m_staticGrid;
	//GridList m_GridList;
public:
	afx_msg void OnLButtonDown(UINT nFlags, CPoint point);
	virtual BOOL OnNotify(WPARAM wParam, LPARAM lParam, LRESULT* pResult);
};
