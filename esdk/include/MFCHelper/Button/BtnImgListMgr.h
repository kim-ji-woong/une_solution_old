/*
버튼 이미지리스트 관리 클래스.
사용법은 PaneMaterial.cpp, PaneOpening.cpp, PaneFacility.cpp의 OnInitDialog를 참조 바람.
제작 : 2010-06-11 박선욱
*/

#pragma once
#include <memory>
#include <map>

// PNG 리소스 사용하려면 define
#define USE_XTP_GRAPHICS_LIBRARY

// 이미지는 반드시 5개의 버튼 상태를 가져야 함.
// 0 : 보통
// 1 : 커서 롤오버
// 2 : 눌려짐(선택됨)
// 3 : 비활성
// 4 : 기본?

#define GetBtnImgListMgr	(BtnImgListMgr::getInstance())

class BtnImgListMgr
{
protected:
	typedef std::map<UINT, CImageList*> ImageListMap;
	static std::auto_ptr<BtnImgListMgr> mInstance;
	ImageListMap mImageLists;
	RECT mMargin;
	BOOL mIsThemed;

	BtnImgListMgr(void);
	ImageListMap::iterator _get(UINT nImageResourceId);
	bool _createImageList(UINT nImageResourceId, CBitmap& bmImagem, int cx, int cy, COLORREF crMask = CLR_NONE);

public:
	static BtnImgListMgr& getInstance();

	// 지정한 이미지 리소스를 불러온다. cx와 cy는 이미지의 전체 크기가 아닌, 하나의 상태를 나타내는 영역의 크기를 지정한다.
	bool loadBmp(UINT nImageResourceId, int cx, int cy, COLORREF crMask = RGB(255, 0, 255));
	bool loadPng(HINSTANCE hInstance, UINT nImageResourceId, int cx, int cy);

	// 이미지 로드돼 있는지 체크
	bool isLoaded(UINT nImageResourceId);

	// 지정한 이미지 리소스 언로드
	void unload(UINT nImageResourceId);
	void unloadAll();

	// 마진 설정. 기본값은 0,0,0,0
	void setMargin(int left, int top, int right, int bottom);

	// 지정한 버튼 컨트롤에 불러온 이미지를 설정
	bool set(CButton& rButton, UINT nImageResourceId);
	bool set(CDialog* pDlg, UINT nButtonResourceId, UINT nImageResourceId);

	void themeChanged();

// 	void unset(CButton& rButton);
// 	void unset(CDialog* pDlg, UINT nButtonResourceId);

	~BtnImgListMgr(void);
};
