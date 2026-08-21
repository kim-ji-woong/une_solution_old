#include "StdAfx.h"
#include "BtnImgListMgr.h"
#include <Uxtheme.h>
#pragma comment(lib, "UxTheme.lib")
#if defined(USE_XTP_GRAPHICS_LIBRARY)
#include <GraphicLibrary/XTPGraphicBitmapPng.h>
#endif

std::auto_ptr<BtnImgListMgr> BtnImgListMgr::mInstance;

BtnImgListMgr& BtnImgListMgr::getInstance()
{
	if ( mInstance.get() == NULL )
		mInstance.reset(new BtnImgListMgr);

	return *mInstance;
}

BtnImgListMgr::BtnImgListMgr(void)
{
	SetRect(&mMargin, 0, 0, 0, 0);
	mIsThemed = IsThemeActive();
}

BtnImgListMgr::~BtnImgListMgr(void)
{
	unloadAll();
}

BtnImgListMgr::ImageListMap::iterator BtnImgListMgr::_get( UINT nImageResourceId )
{
	return mImageLists.find(nImageResourceId);
}

bool BtnImgListMgr::isLoaded( UINT nImageResourceId )
{
	return _get(nImageResourceId) != mImageLists.end();
}

void BtnImgListMgr::themeChanged()
{
	// TODO : 중복실행 방지... CriticalSection 넣어야할지도.

	// 고전 테마에서는 이미지 리스트의 첫 번째 이미지만 사용하는 문제가
	// 있기 때문에 테마가 변경되면 테마 활성/비활성 여부에 따라 이미지
	// 리스트의 첫 번째 아이콘을 보통상태 또는 선택상태로 바꾼다.

	const BOOL bActivated = IsThemeActive();

	if ( mIsThemed == bActivated )
		return;

	mIsThemed = bActivated;

	int indexToExtract;
	if ( bActivated )
		indexToExtract = 4;
	else
		indexToExtract = 2;

	ImageListMap::iterator i, ie = mImageLists.end();
	for ( i = mImageLists.begin() ; i != ie ; ++i )
	{
		CImageList* pImageList = i->second;

		if ( pImageList->GetImageCount() <= indexToExtract )
			continue;

		HICON hExtractedIcon = pImageList->ExtractIcon(indexToExtract);
		if ( hExtractedIcon != NULL )
		{
			pImageList->Replace(0, hExtractedIcon);
			DestroyIcon(hExtractedIcon);
		}
	}
}

bool BtnImgListMgr::_createImageList( UINT nImageResourceId, CBitmap& bmImage, int cx, int cy, COLORREF crMask/* = CLR_NONE*/ )
{
	CImageList* pImageList = new CImageList;

	if ( pImageList->Create(cx, cy, ILC_COLOR32 | ILC_MASK, 6, 0) )
	{
		pImageList->SetBkColor(CLR_NONE);
		if ( pImageList->Add(&bmImage, crMask) == -1 )
		{
			pImageList->DeleteImageList();
			delete pImageList;
			return false;
		}

		// 고전 테마에서는 0번 인덱스만 사용하는 문제(?)가 있다.
		// 따라서 2번 이미지를 0번에 복사함.
		if ( !IsThemeActive() && pImageList->GetImageCount() > 2 )
		{
			HICON hActiveIcon = pImageList->ExtractIcon(2);
			if ( hActiveIcon != NULL )
			{
				pImageList->Replace(0, hActiveIcon);
				DestroyIcon(hActiveIcon);
			}
		}
	}
	else
	{
		delete pImageList;
		return false;
	}

	mImageLists.insert(ImageListMap::value_type(nImageResourceId, pImageList));

	return true;
}

bool BtnImgListMgr::loadBmp( UINT nImageResourceId, int cx, int cy, COLORREF crMask)
{
	if ( isLoaded(nImageResourceId) )
		return true;

	CBitmap bm;
	BOOL bLoadResult = bm.LoadBitmap(nImageResourceId);

	// 지정한 리소스 로드 성공하였는가?
	ASSERT(bLoadResult == TRUE);

	if ( !bLoadResult )
		return false;

	return _createImageList(nImageResourceId, bm, cx, cy, crMask);
}

bool BtnImgListMgr::loadPng(HINSTANCE hInstance, UINT nImageResourceId, int cx, int cy )
{
#if !defined(USE_XTP_GRAPHICS_LIBRARY)
	// PNG 로드하려면 헤더파일에서 USE_XTP_GRAPHICS_LIBRARY를 define하세요!
	ASSERT(FALSE);
	return false;
#endif

	if ( isLoaded(nImageResourceId) )
		return true;

	CXTPGraphicBitmapPng bm;
	//HINSTANCE hInstance = AfxGetInstanceHandle();
	BOOL bLoadResult = bm.LoadFromResource(hInstance, FindResource(hInstance, MAKEINTRESOURCE(nImageResourceId), _T("PNG")));

	// 지정한 리소스 로드 성공하였는가?
	ASSERT(bLoadResult == TRUE);

	if ( !bLoadResult )
		return false;

	return _createImageList(nImageResourceId, bm, cx, cy);
}

void BtnImgListMgr::unload( UINT nImageResourceId )
{
	ImageListMap::iterator i = _get(nImageResourceId);

	if ( i == mImageLists.end() )
		return;

	i->second->DeleteImageList();
	delete i->second;

	mImageLists.erase(i);
}

void BtnImgListMgr::unloadAll()
{
	ImageListMap::iterator i, ie = mImageLists.end();
	for ( i = mImageLists.begin() ; i != ie ; ++i )
	{
		i->second->DeleteImageList();
		delete i->second;
	}

	mImageLists.clear();
}

void BtnImgListMgr::setMargin( int left, int top, int right, int bottom )
{
	SetRect(&mMargin, left, top, right, bottom);
}

bool BtnImgListMgr::set( CButton& rButton, UINT nImageResourceId )
{
	ImageListMap::iterator i = _get(nImageResourceId);

	if ( i == mImageLists.end() )
	{
		// nImageResourceId의 비트맵이 로드돼있지 않음.
		// 또는, 이미지를 로드할 때 사용한 리소스 ID와 버튼에 set할 때
		// 사용한 리소스 아이디가 다른지 확인.
		ASSERT( i != mImageLists.end() );
		return false;
	}

	BUTTON_IMAGELIST bi;
	bi.himl = i->second->GetSafeHandle();
	bi.margin = mMargin;
	bi.uAlign = BUTTON_IMAGELIST_ALIGN_CENTER;

	rButton.SetWindowText(NULL);

	return rButton.SetImageList(&bi) == TRUE;
}

bool BtnImgListMgr::set( CDialog* pDlg, UINT nButtonResourceId, UINT nImageResourceId )
{
	return set(*static_cast<CButton*>(pDlg->GetDlgItem(nButtonResourceId)), nImageResourceId);
}


// void BtnImgListMgr::unset( CButton& rButton )
// {
// 	//rButton.SetImageList(NULL); // 오류
// }
// 
// void BtnImgListMgr::unset( CDialog* pDlg, UINT nButtonResourceId )
// {
// 	unset(*static_cast<CButton*>(pDlg->GetDlgItem(nButtonResourceId)));
// }

