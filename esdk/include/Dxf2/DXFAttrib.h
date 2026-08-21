#pragma once

namespace DXF
{
	namespace ENTITIES
	{
		class Attrib : public Entity
		{
		public:
			Attrib(void);
			virtual ~Attrib(void);

		public:
			virtual void Init();
		};
	}
}