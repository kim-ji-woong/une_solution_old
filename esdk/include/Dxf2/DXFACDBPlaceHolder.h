#pragma once

namespace DXF
{
	namespace OBJECTS
	{
		class ObjectManager;

		class ACDBPlaceHolder : public Object
		{
		public:
			ACDBPlaceHolder(int nHandle, int nSoftPointer, ObjectManager* pMgr);
			virtual ~ACDBPlaceHolder(void);

		protected:
			int m_nHandle;
			int m_nSoftPointer;
		};
	}
}
