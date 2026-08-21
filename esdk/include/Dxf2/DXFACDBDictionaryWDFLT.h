#pragma once

namespace DXF
{
	namespace OBJECTS
	{
		class ObjectManager;

		class ACDBDictionaryWDFLT : public Dictionary
		{
		public:
			ACDBDictionaryWDFLT(ObjectManager* pMgr);
			virtual ~ACDBDictionaryWDFLT(void);

		protected:
			void Init();
		};
	}
}
