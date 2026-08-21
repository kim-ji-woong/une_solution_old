// SampleDoc.h : CSampleDoc 클래스의 인터페이스
//


#pragma once

#include <AIS_Trihedron.hxx>
#include <AIS_InteractiveContext.hxx>
#include <V3d_Viewer.hxx>

enum DisplayType{No2DNo3D,   // 0 0 0
                 No2D3D  ,   // 0 0 1
                 a2DNo3D  ,   // 0 1 0
                 a2D3D    };  // 1 1 1

class CSampleDoc : public CDocument
{
protected: // serialization에서만 만들어집니다.
	CSampleDoc();
	DECLARE_DYNCREATE(CSampleDoc)

// 특성입니다.
public:

// 작업입니다.
public:
	Handle_AIS_InteractiveContext& GetAISContext(){ return myAISContext; };
	Handle_V3d_Viewer GetViewer()  { return myViewer; };

public:
	void PreProcess (DisplayType aDisplayType);

// 재정의입니다.
public:
	virtual BOOL OnNewDocument();
	virtual void Serialize(CArchive& ar);

// 구현입니다.
public:
	virtual ~CSampleDoc();
#ifdef _DEBUG
	virtual void AssertValid() const;
	virtual void Dump(CDumpContext& dc) const;
#endif

protected:
	Handle_V3d_Viewer myViewer;
	Handle_AIS_InteractiveContext myAISContext;

// 생성된 메시지 맵 함수
protected:
	DECLARE_MESSAGE_MAP()
};


