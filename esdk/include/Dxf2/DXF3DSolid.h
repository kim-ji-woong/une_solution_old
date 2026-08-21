#pragma once

// 3D 공간상의 물체를 나타내는 것으로, 그 데이터는 AutoDesk사에서
// 공개하지 않고 있다.
// 현재 버전에서 지원하지 않는다.

namespace DXF
{
	namespace ENTITIES
	{
		class _3DSolid : public Entity
		{
		public:
			_3DSolid(void);
			virtual ~_3DSolid(void);

		public:
			virtual void Init();
		};
	}
}
