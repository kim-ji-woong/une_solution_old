#pragma once

#include "resource.h"
#include "DlgGradient.h"
#include "afxcmn.h"

#include "MySliderControl.h"

// CSettingDialog 대화 상자입니다.

class CSettingDialog : public CDlgGradient
{
	DECLARE_DYNAMIC(CSettingDialog)

public:
	CSettingDialog(CWnd* pParent = NULL);   // 표준 생성자입니다.
	virtual ~CSettingDialog();

// 대화 상자 데이터입니다.
	enum { IDD = IDD_SETTING };

protected:
	virtual void DoDataExchange(CDataExchange* pDX);    // DDX/DDV 지원입니다.

	DECLARE_MESSAGE_MAP()
public:

	afx_msg void OnToggleNormals();
	afx_msg void OnToggleAutoRotate();
	afx_msg void OnToggleFPSView();
	afx_msg void OnToggleMultipleLights();
	afx_msg void OnToggleLightRotate();
	afx_msg void OnToggleTransparency();
	afx_msg void OnToggleLowQuality();
	afx_msg void OnToggleSpecular();
	afx_msg void OnToggleMats();
	afx_msg void OnToggleSkeleton();
	afx_msg void OnToggleCulling();
	afx_msg void OnToggleWireFrame();
	afx_msg void OnToggleMS();
	afx_msg void OnToggleUIState();

	virtual void ChangeSytle();
	void InitUI();
	CMySliderControl m_sliderAni;
	afx_msg void OnEnChangeEdit();


	

	
	afx_msg void OnEnUpdateEvert();
};
