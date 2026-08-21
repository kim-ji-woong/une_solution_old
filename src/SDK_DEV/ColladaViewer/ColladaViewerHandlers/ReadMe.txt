========================================================================
    ACTIVE TEMPLATE LIBRARY : ColladaViewerHandlers 프로젝트 개요
========================================================================

사용자가 DLL(동적 연결 라이브러리)을 만드는 데 있어 출발점으로 사용하도록
응용 프로그램 마법사에서 이 ColladaViewerHandlers 프로젝트를 만들었습니다.

이 파일에는 프로젝트를 구성하는 각각의 파일에 들어 있는
요약 설명이 포함되어 있습니다.

ColladaViewerHandlers.vcxproj
    응용 프로그램 마법사를 사용하여 생성한 VC++ 프로젝트의 기본 프로젝트 파일입니다.
    해당 파일을 생성한 Visual C++의 버전 정보를 비롯하여
    응용 프로그램 마법사에서 선택한 플랫폼, 구성 및
    프로젝트 기능에 대한 정보가 들어 있습니다.

ColladaViewerHandlers.vcxproj.filters
    응용 프로그램 마법사를 사용하여 생성된 VC++ 프로젝트의 필터 파일입니다. 
    이 파일에는 프로젝트의 파일과 필터 간의 연결 정보가 들어 
    있습니다. 이러한 연결은 특정 노드에서 유사한 확장명으로 그룹화된
    파일을 표시하기 위해 IDE에서 사용됩니다. 예를 들어 ".cpp" 파일은 "소스 파일”
    필터와 연결되어 있습니다.

ColladaViewerHandlers.idl
    이 파일에는 형식 라이브러리의 IDL 정의 및,
    프로젝트에서 정의한 인터페이스와 보조 클래스가 들어 있습니다.
    이 파일은 MIDL 컴파일러에 의해 처리되어 다음을 생성합니다.
        C++ 인터페이스 정의 및 GUID 선언   (ColladaViewerHandlers.h)
        GUID 정의                  (ColladaViewerHandlers_i.c)
        형식 라이브러리                 (ColladaViewerHandlers.tlb)
        마샬링 코드                   (ColladaViewerHandlers_p.c 및 dlldata.c)

ColladaViewerHandlers.h
    이 파일에는 ColladaViewerHandlers.idl에서 정의된 항목의 C++ 인터페이스 정의 및
    GUID 선언이 포함됩니다. 컴파일하는 동안 MIDL에 의해 다시 생성됩니다.

ColladaViewerHandlers.cpp
    이 파일에는 개체 맵 및 DLL 내보내기 구현이 들어 있습니다.

ColladaViewerHandlers.rc
    프로그램에서 사용하는 모든
    Microsoft Windows 리소스의 목록입니다.

ColladaViewerHandlers.def
    이 모듈 정의 파일에서는 DLL에서 필요한 내보내기에 대한 정보를
    링커에 제공합니다. 다음에 대한 내보내기가 포함되어 있습니다.
        DllGetClassObject
        DllCanUnloadNow
        DllRegisterServer
        DllUnregisterServer
        DllInstall

/////////////////////////////////////////////////////////////////////////////
기타 표준 파일:

StdAfx.h, StdAfx.cpp
    ColladaViewerHandlers.pch라는 이름의 PCH(미리 컴파일된 헤더) 파일과
    미리 컴파일된 형식 파일인 StdAfx.obj를 빌드하는 데 사용됩니다.

Resource.h
    리소스 ID를 정의하는 표준 헤더 파일입니다.

/////////////////////////////////////////////////////////////////////////////
기타 참고:

	MFC 지원 옵션에서는 MFC 라이브러리를 기초 응용 프로그램으로 빌드하여
	MFC 클래스, 개체 및 함수를 사용할 수 있도록 합니다.
/////////////////////////////////////////////////////////////////////////////
