#pragma once

namespace DXF
{
	namespace ENTITIES
	{
		class Body : public Entity
		{
		public:
			Body(void);
			virtual ~Body(void);

		public:
			virtual void Init();
		};
	}
}
