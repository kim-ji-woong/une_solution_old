// dllmain.h : 모듈 클래스의 선언입니다.

class CColladaViewerHandlersModule : public ATL::CAtlDllModuleT< CColladaViewerHandlersModule >
{
public :
	DECLARE_LIBID(LIBID_ColladaViewerHandlersLib)
	DECLARE_REGISTRY_APPID_RESOURCEID(IDR_COLLADAVIEWERHANDLERS, "{D9E8DD9E-46DB-4F5F-A665-EE62D6AD3AD4}")
};

extern class CColladaViewerHandlersModule _AtlModule;
