#pragma once

namespace DXF
{
	namespace ENTITIES
	{
		class AttDef : public Entity
		{
		public:
			AttDef(void);
			virtual ~AttDef(void);

		public:
			virtual void Init();
		};
	}
}
