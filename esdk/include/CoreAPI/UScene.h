#pragma once


#include "CoreAPI.h"
#include "UMath.h"
#include <list>

namespace Ogre
{
	class  UDotSceneLoader;
}
namespace UnE
{

	namespace Core
	{

		class UAssimpLoader;
		class UBaseModel;
		

		//-------------------------------------------------------------------------
		class CORE_API UNode
		{
			friend class UAssimpLoader;
			friend class UBaseModel;
			friend class Ogre::UDotSceneLoader;

		protected:
			std::list<UNode*> mChilds;
			UNode *			  mParent;
			void *			  mTag;
			
			void SetTag(void * val);

		public:
			UNode(void);
			virtual ~UNode(void);

			unsigned int	GetNumChilds();

			UNode*	GetParent();

			UNode*  ChildAt(int idx);
		};

		//-------------------------------------------------------------------------
		class CORE_API USceneNode : public UNode
		{
			friend class UBaseModel;
			friend class UAssimpLoader;
			friend class UBaseView;
			friend class Ogre::UDotSceneLoader;
		protected:
			bool		mbIncludeScene;
			std::string mSceneName;			
			bool		mbVisible;
			std::string mAliasName;
			bool		mbShowBound;
		public:
			USceneNode();
			virtual ~USceneNode();
						
			USceneNode*	CreateChild(std::string& szName);
			USceneNode* AddChild(USceneNode* pNode);
			USceneNode* RemoveChild(std::string& szName);
			USceneNode* GetChildNode(std::string& szName);
			void		RemoveAllChilds();
			
			void		SetVisible(bool bVisible);
			bool        GetVisible() { return mbVisible; }
			std::string SceneName() { return mSceneName; }

			std::string GetAliasName() { return mAliasName; }
			void SetAliasName(std::string val) { mAliasName = val; }

			void		Attach();
			void		Dettach();
			void		UpdateScenen();


	
			void		GetPosition(UnE::Math::Vector3& pos);
			void		SetPosition(UnE::Math::Vector3& pos);

			void        Move( float x, float y, float z , bool updateOrientation = true);
			void        Translate(float x, float y, float z);
		    void        SetRotation( float r );

			void        SetHeading( UnE::Math::Vector3& vDir );
			void        GetDirection(UnE::Math::Vector3& vDir);


			void		ShowBoundingBox(bool bshow);
			void		GetBoundBox(UnE::Math::AxisAlignedBox& aabb);
			void		UpdateBound();			

			USceneNode*	GetParentScene();

		};
		//-------------------------------------------------------------------------

		class CORE_API USceneNodeManager
		{		
			friend class UBB;
			friend class UBaseView;
		protected:
			USceneNode * mRoot;
		public:
			USceneNodeManager();
			virtual ~USceneNodeManager();
			USceneNode* GetRootSceneNode();

			USceneNode* FindSceneNode(std::string szName);

		};


	}
}

