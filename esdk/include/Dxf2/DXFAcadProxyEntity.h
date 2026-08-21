#pragma once

// ObjectARX 기반의 AutoCAD ADT(Architectural Desktop) 환경에서 작성한 Object Entity는
// CAD 화면에서 직접 불러올 수 없다. 이러한 데이터가 DWG 파일에 포함되어 있을 경우
// ACAD_PROXY_ENTITY로 처리한다.
// 현재 버전은 지원하지 않는다.

namespace DXF
{
	namespace ENTITIES
	{
		class AcadProxyEntity : public Entity
		{
		public:
			AcadProxyEntity(void);
			virtual ~AcadProxyEntity(void);

		public:
			virtual void Init();
		};
	}
}
