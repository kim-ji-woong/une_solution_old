#include "stdafx.h"
//////////////////////////////////////////////////////////////////////////
// BASE Lib
#include <Poco/String.h>
//////////////////////////////////////////////////////////////////////////
// ASSIMP
#include <assimp/cimport.h>
#include <assimp/Importer.hpp>
#include <assimp/ai_assert.h>
#include <assimp/cfileio.h>
#include <assimp/postprocess.h>
#include <assimp/scene.h>
#include <assimp/IOSystem.hpp>
#include <assimp/IOStream.hpp>
#include <assimp/LogStream.hpp>
#include <assimp/DefaultLogger.hpp>
//////////////////////////////////////////////////////////////////////////
// CORE API
#include "UAssimpLoader.h"
#include "UBaseView.h"
#include "DefaultLogger.hpp"
#include "UDB.h"
#include "UBaseModel.h"
#include "UScene.h"
#include "UEntity.h"
//////////////////////////////////////////////////////////////////////////
// OGRE
#include "OgreDataStream.h"
#include "OgreImage.h"
#include "OgreTexture.h"
#include "OgreTextureManager.h"
#include "OgreMaterial.h"
#include "OgreMaterialManager.h"
#include "OgreLog.h"
#include "OgreLogManager.h"
#include "OgreHardwareBuffer.h"
#include "OgreMesh.h"
#include "OgreSubMesh.h"
#include "OgreDefaultHardwareBufferManager.h"
#include "OgreMeshManager.h"
#include "OgreSceneManager.h"
#include <OgreStringConverter.h>
#include <OgreSkeletonManager.h>
#include "OgreMeshSerializer.h"
#include "OgreSkeletonSerializer.h"
#include "OgreAnimation.h"
#include "OgreAnimationTrack.h"
#include "OgreKeyFrame.h"
#include <boost/tuple/tuple.hpp>

namespace UnE
{
	namespace Core
	{
		//////////////////////////////////////////////////////////////////////////
		// UFileLoader Implementation
		UFileLoader::UFileLoader()
		{
		}
		//-----------------------------------------------------------------------
		UFileLoader::~UFileLoader()
		{
		}
		//////////////////////////////////////////////////////////////////////////

		typedef boost::tuple< aiVectorKey*, aiQuatKey*, aiVectorKey* > KeyframeData;
		typedef std::map< float, KeyframeData > KeyframesMap;
		
		template <int v>
		struct Int2Type
		{
			enum { value = v };
		};

		int UAssimpLoader::msBoneCount = 0;
		//-----------------------------------------------------------------------
		Ogre::String toString(const aiColor4D& colour)
		{
			return	Ogre::StringConverter::toString(colour.r) + " " +  
			Ogre::StringConverter::toString(colour.g) + " " + 
			Ogre::StringConverter::toString(colour.b) + " " + 
			Ogre::StringConverter::toString(colour.a);
		}
		//-----------------------------------------------------------------------	
		UAssimpLoader::UAssimpLoader()
		{
			//mSkeletonRootNode = NULL;
		}
		//-----------------------------------------------------------------------
		UAssimpLoader::~UAssimpLoader()
		{
		}
		//-----------------------------------------------------------------------
		bool UAssimpLoader::convert( HWND hWnd , const Ogre::String& filename, const Ogre::String& customAnimationName /*= ""*/, int loaderParams /*= (LP_REVERSE_FACE_INDEX | LP_GENERATE_MATERIALS_AS_CODE) */ )
		{
			m_hWnd = hWnd;
			mLoaderParams = loaderParams;
			mCustomAnimationName = customAnimationName;
			if ((mLoaderParams & LP_USE_LAST_RUN_NODE_DERIVED_TRANSFORMS) == false)
			{
				mNodeDerivedTransformByName.clear();
			}
		
			Ogre::String extension;
			Ogre::StringUtil::splitFullFilename(filename, mBasename, extension, mPath);	
			mBasename = mBasename + "_" + extension;
		
			std::string logfile = logPath + "Import.log";

#ifdef DEBUG
			Assimp::DefaultLogger::create(logfile.c_str(), Assimp::Logger::VERBOSE);
#else
			Assimp::DefaultLogger::create(logfile.c_str(), Assimp::Logger::NORMAL);
#endif
			Assimp::DefaultLogger::get()->info("Logging asses");

			Ogre::LogManager::getSingleton().logMessage("*** Loading ass file... ***");
			Ogre::LogManager::getSingleton().logMessage("Filename " + filename);

			const aiScene *scene;

			Assimp::Importer importer;
		
			aiPropertyStore* props = aiCreatePropertyStore();
			aiSetImportPropertyInteger(props, AI_CONFIG_IMPORT_TER_MAKE_UVS, 1);
			aiSetImportPropertyFloat( props, AI_CONFIG_PP_GSN_MAX_SMOOTHING_ANGLE, 30.0f);
			aiSetImportPropertyInteger(props, AI_CONFIG_PP_SBP_REMOVE, aiPrimitiveType_LINE | aiPrimitiveType_POINT );
			aiSetImportPropertyInteger(props, AI_CONFIG_GLOB_MEASURE_TIME, 1);
	
			unsigned int ppsteps = 
				
				//aiProcess_CalcTangentSpace		   | // calculate tangents and bitangents if possible
				//aiProcess_JoinIdenticalVertices    | // join identical vertices/ optimize indexing
				//aiProcess_ValidateDataStructure    | // perform a full validation of the loader's output
				//aiProcess_ImproveCacheLocality     | // improve the cache locality of the output vertices
				//aiProcess_RemoveRedundantMaterials | // remove redundant materials
				aiProcess_FindDegenerates          | // remove degenerated polygons from the import
				aiProcess_FindInvalidData          | // detect invalid model data, such as invalid normal vectors
				aiProcess_GenUVCoords              | // convert spherical, cylindrical, box and planar mapping to proper UVs
				aiProcess_TransformUVCoords        | // preprocess UV transformations (scaling, translation ...)
#ifdef HSMS
				aiProcess_FindInstances            | // search for instanced meshes and remove them by references to one master
#endif
				//aiProcess_LimitBoneWeights         | // limit bone weights to 4 per vertex
				//aiProcess_OptimizeMeshes		   | // join small meshes, if possible;
				//aiProcess_SplitByBoneCount         | // split meshes with too many bones. Necessary for our (limited) hardware skinning shader
				aiProcess_GenSmoothNormals		   | // generate smooth normal vectors if not existing
				aiProcess_SplitLargeMeshes         | // split large, unrenderable meshes into submeshes
				aiProcess_Triangulate			   | // triangulate polygons with more than 3 edges
				aiProcess_ConvertToLeftHanded	   | // convert everything to D3D left handed space
				aiProcess_SortByPType              | // make 'clean' meshes which consist of a single typ of primitives
#ifdef HSMS
				aiProcess_PreTransformVertices     |
#endif
				
				0;

			scene = importer.ReadFile(filename, ppsteps  );			
			aiReleasePropertyStore(props);

			// If the import failed, report it
			if( !scene)
			{
				Ogre::LogManager::getSingleton().logMessage("AssImp importer failed with the following message:");
				Ogre::LogManager::getSingleton().logMessage(importer.GetErrorString() );
				return false;
			}
	
			grabNodeNamesFromNode(scene, scene->mRootNode);
			grabBoneNamesFromNode(scene, scene->mRootNode);

			computeNodesDerivedTransform(scene, scene->mRootNode, scene->mRootNode->mTransformation);
		
			if(mBonesByName.size())
			{
				mSkeleton = Ogre::SkeletonManager::getSingleton().create("conversion", Ogre::ResourceGroupManager::DEFAULT_RESOURCE_GROUP_NAME);
		
				msBoneCount = 0;
				createBonesFromNode(scene, scene->mRootNode);
				msBoneCount = 0;
				createBoneHiearchy(scene, scene->mRootNode);

				if(scene->HasAnimations())
				{
					for(unsigned int i = 0; i < scene->mNumAnimations; ++i)
					{
						parseAnimation(scene, i, scene->mAnimations[i]);
					}
				}
			}
	
			loadDataFromNode(scene, scene->mRootNode, mPath, true);

			Ogre::LogManager::getSingleton().logMessage("*** Finished loading ass file ***");
			Assimp::DefaultLogger::kill();

			if(!mSkeleton.isNull())
			{			
				unsigned short numBones = mSkeleton->getNumBones();
				unsigned short i;
				for (i = 0; i < numBones; ++i)
				{
					Ogre::Bone* pBone = mSkeleton->getBone(i);
					assert(pBone);
				}
				Ogre::SkeletonSerializer binSer;
				binSer.exportSkeleton(mSkeleton.getPointer(), mPath + mBasename + ".skeleton");
			}
	
			Ogre::MeshSerializer meshSer;
			for(MeshVector::iterator it = mMeshes.begin(); it != mMeshes.end(); ++it)
			{
				Ogre::MeshPtr mMesh = *it;
				if(mBonesByName.size())
				{
					mMesh->setSkeletonName(mBasename + ".skeleton");
				}
		
				Ogre::Mesh::SubMeshIterator smIt = mMesh->getSubMeshIterator();
				while (smIt.hasMoreElements())
				{
					Ogre::SubMesh* sm = smIt.getNext();
					if (!sm->useSharedVertices)
					{			
						// Automatic
						Ogre::VertexDeclaration* newDcl = sm->vertexData->vertexDeclaration->getAutoOrganisedDeclaration(mMesh->hasSkeleton(), mMesh->hasVertexAnimation(), mMesh->getSharedVertexDataAnimationIncludesNormals());
						if (*newDcl != *(sm->vertexData->vertexDeclaration))
						{
							// Usages don't matter here since we're only exporting
							Ogre::BufferUsageList bufferUsages;
							for (size_t u = 0; u <= newDcl->getMaxSource(); ++u)
								bufferUsages.push_back(Ogre::HardwareBuffer::HBU_STATIC_WRITE_ONLY);
							sm->vertexData->reorganiseBuffers(newDcl, bufferUsages);
						}
					}
				}
				//meshSer.exportMesh(mMesh.getPointer(), mPath + mBasename + ".mesh");
			}
		
			// serialise the materials
			if(mLoaderParams & !LP_GENERATE_MATERIALS_AS_CODE)
			{
				Ogre::MaterialSerializer ms;
				std::vector<Ogre::String> exportedNames;
		
				for(MeshVector::iterator it = mMeshes.begin(); it != mMeshes.end(); ++it)
				{
					Ogre::MeshPtr mMesh = *it;
			
					// queue up the materials for serialise
					Ogre::MaterialManager *mmptr = Ogre::MaterialManager::getSingletonPtr();
					Ogre::Mesh::SubMeshIterator it2 = mMesh->getSubMeshIterator();
					while(it2.hasMoreElements())
					{
						Ogre::SubMesh* sm = it2.getNext();
						Ogre::String matName(sm->getMaterialName());
						if (std::find(exportedNames.begin(), exportedNames.end(), matName) == exportedNames.end())
						{
							Ogre::MaterialPtr materialPtr = mmptr->getByName(matName);
							materialPtr->getTechnique(0)->setLightingEnabled(false);
							ms.queueForExport(materialPtr);
							exportedNames.push_back(matName);
						}
					}
				}
		
				if(exportedNames.size())
				{
					ms.exportQueued(mPath + mBasename + ".material", true);
				}
			}

			// clean up	
			mMeshes.clear();
			mMaterialCode = "";
			mBonesByName.clear();
			mBoneNodesByName.clear();
			boneMap.clear();
			mSkeleton = Ogre::SkeletonPtr(NULL);
			mCustomAnimationName = "";
			// etc...	
			Ogre::MeshManager::getSingleton().removeUnreferencedResources();
			Ogre::SkeletonManager::getSingleton().removeUnreferencedResources();
	
			return true;
		}
		//-----------------------------------------------------------------------		
		// T should be a Loki::Int2Type<>
		template< typename T > void GetInterpolationIterators(KeyframesMap& keyframes, 
															  KeyframesMap::iterator it, 
															  KeyframesMap::reverse_iterator& front, 
															  KeyframesMap::iterator& back)
		{
			front = KeyframesMap::reverse_iterator(it);
	
			front++;
			for(front; front != keyframes.rend(); front++)
			{
				if(boost::get< T::value >(front->second) != NULL)
				{
					break;
				}
			}
	
			back = it;
			back++;
			for(back; back != keyframes.end(); back++)
			{
				if(boost::get< T::value >(back->second) != NULL)
				{
					break;
				}
			}
		}
		//-----------------------------------------------------------------------
		aiVector3D getTranslate(aiNodeAnim* node_anim, KeyframesMap& keyframes, KeyframesMap::iterator it)
		{
			aiVectorKey* translateKey = boost::get<0>(it->second);
			aiVector3D vect;
			if(translateKey)
			{
				vect = translateKey->mValue;
			}
			else
			{
				KeyframesMap::reverse_iterator front;
				KeyframesMap::iterator back;
		
		
				GetInterpolationIterators< Int2Type<0> > (keyframes, it, front, back);
		
				KeyframesMap::reverse_iterator rend = keyframes.rend();
				KeyframesMap::iterator end = keyframes.end();
				aiVectorKey* frontKey = NULL;
				aiVectorKey* backKey = NULL;
		
				if(front != rend)
					frontKey = boost::get<0>(front->second);
		
				if(back != end)
					backKey = boost::get<0>(back->second);
		
				// got 2 keys can interpolate
				if(frontKey && backKey)
				{			
					float prop = (float)((it->first - frontKey->mTime) / (backKey->mTime - frontKey->mTime));
					vect = ((backKey->mValue - frontKey->mValue) * prop) + frontKey->mValue;
				}
		
				else if(frontKey)
				{
					vect = frontKey->mValue;
				}
				else if(backKey)
				{
					vect = backKey->mValue;
				}
			}
	
			return vect;
		}
		//-----------------------------------------------------------------------
		aiQuaternion getRotate(aiNodeAnim* node_anim, KeyframesMap& keyframes, KeyframesMap::iterator it)
		{
			aiQuatKey* rotationKey = boost::get<1>(it->second);
			aiQuaternion rot;
			if(rotationKey)
			{
				rot = rotationKey->mValue;
			}
			else
			{
				KeyframesMap::reverse_iterator front;
				KeyframesMap::iterator back;
		
				GetInterpolationIterators< Int2Type<1> > (keyframes, it, front, back);
		
				KeyframesMap::reverse_iterator rend = keyframes.rend();
				KeyframesMap::iterator end = keyframes.end();
				aiQuatKey* frontKey = NULL;
				aiQuatKey* backKey = NULL;
		
				if(front != rend)
					frontKey = boost::get<1>(front->second);
		
				if(back != end)
					backKey = boost::get<1>(back->second);
		
				// got 2 keys can interpolate
				if(frontKey && backKey)
				{	
					float prop = (float)((it->first - frontKey->mTime) / (backKey->mTime - frontKey->mTime));
					aiQuaternion::Interpolate(rot, frontKey->mValue, backKey->mValue, prop);
				}
		
				else if(frontKey)
				{
					rot = frontKey->mValue;
				}
				else if(backKey)
				{
					rot = backKey->mValue;
				}
			}	
			return rot;
		}
		//-----------------------------------------------------------------------
		void UAssimpLoader::parseAnimation (const aiScene* mScene, int index, aiAnimation* anim)
		{
			// DefBonePose a matrix that represents the local bone transform (can build from Ogre bone components)
			// PoseToKey a matrix representing the keyframe translation
			// What assimp stores aiNodeAnim IS the decomposed form of the transform (DefBonePose * PoseToKey)
			// To get PoseToKey which is what Ogre needs we'ed have to build the transform from components in 
			// aiNodeAnim and then DefBonePose.Inverse() * aiNodeAnim(generated transform) will be the right transform

			Ogre::String animName; 
			if(mCustomAnimationName != "")
			{
				animName = mCustomAnimationName;
				if(index >= 1)
				{
					animName += Ogre::StringConverter::toString(index);
				}
			}
			else
			{
				animName = Ogre::String(anim->mName.data);
			}
			if(animName.length() < 1)
			{
				animName = "Animation" + Ogre::StringConverter::toString(index);
			}
		
			Ogre::LogManager::getSingleton().logMessage("Animation name = '" + animName + "'");
			Ogre::LogManager::getSingleton().logMessage("duration = " + Ogre::StringConverter::toString(Ogre::Real(anim->mDuration)));
			Ogre::LogManager::getSingleton().logMessage("tick/sec = " + Ogre::StringConverter::toString(Ogre::Real(anim->mTicksPerSecond)));
			Ogre::LogManager::getSingleton().logMessage("channels = " + Ogre::StringConverter::toString(anim->mNumChannels));
	
			Ogre::Animation* animation;
	
			float cutTime = 0.0;
			if(mLoaderParams & LP_CUT_ANIMATION_WHERE_NO_FURTHER_CHANGE)
			{
				for (unsigned int i = 1; i < (int)anim->mNumChannels; i++)
				{
					aiNodeAnim* node_anim = anim->mChannels[i];
			
					// times of the equality check
					float timePos = 0.0;
					float timeRot = 0.0;

					for(unsigned int j = 1; j < node_anim->mNumPositionKeys; j++)
					{
						if( node_anim->mPositionKeys[j] != node_anim->mPositionKeys[j-1])
						{
							timePos = (float)(node_anim->mPositionKeys[j].mTime);
						}
					}
			
					for(unsigned int k = 1; k < node_anim->mNumRotationKeys; k++)
					{
						if( node_anim->mRotationKeys[k] != node_anim->mRotationKeys[k-1])
						{
							timeRot = (float)(node_anim->mRotationKeys[k].mTime);
						}
					}
			
					if(timePos > cutTime){ cutTime = timePos; }
					if(timeRot > cutTime){ cutTime = timeRot; }
				}
		
				animation = mSkeleton->createAnimation(Ogre::String(animName), Ogre::Real(cutTime));
			}
			else
			{
				cutTime = Ogre::Math::POS_INFINITY;
				animation = mSkeleton->createAnimation(Ogre::String(animName), Ogre::Real(anim->mDuration));
			}
	
			animation->setInterpolationMode(Ogre::Animation::IM_LINEAR);
	
			Ogre::LogManager::getSingleton().logMessage("Cut Time " + Ogre::StringConverter::toString(cutTime));
	
			for (int i = 0; i < (int)anim->mNumChannels; i++)
			{
				Ogre::TransformKeyFrame* keyframe;
		
				aiNodeAnim* node_anim = anim->mChannels[i];
				Ogre::LogManager::getSingleton().logMessage("Channel " + Ogre::StringConverter::toString(i));
				Ogre::LogManager::getSingleton().logMessage("affecting node: " + Ogre::String(node_anim->mNodeName.data));

				Ogre::String boneName = Ogre::String(node_anim->mNodeName.data);
		
				if(mSkeleton->hasBone(boneName))
				{
					Ogre::Bone* bone = mSkeleton->getBone(boneName);
					Ogre::Matrix4 defBonePoseInv;
					defBonePoseInv.makeInverseTransform(bone->getPosition(), bone->getScale(), bone->getOrientation());
			
					Ogre::NodeAnimationTrack* track = animation->createNodeTrack(i, bone);

					// Ogre needs translate rotate and scale for each keyframe in the track
					KeyframesMap keyframes;
			
					for(unsigned int j = 0; j < node_anim->mNumPositionKeys; j++)
					{
						keyframes[ node_anim->mPositionKeys[j].mTime ] = KeyframeData( &(node_anim->mPositionKeys[j]), NULL, NULL);
					}
			
					for(unsigned int k = 0; k < node_anim->mNumRotationKeys; k++)
					{
						KeyframesMap::iterator it = keyframes.find(node_anim->mRotationKeys[k].mTime);
						if(it != keyframes.end())
						{
							boost::get<1>(it->second) = &(node_anim->mRotationKeys[k]);
						}
						else
						{
							keyframes[ node_anim->mRotationKeys[k].mTime ] = KeyframeData( NULL, &(node_anim->mRotationKeys[k]), NULL );
						}
					}
			
					for(unsigned int m = 0; m < node_anim->mNumScalingKeys; m++)
					{
						KeyframesMap::iterator it = keyframes.find(node_anim->mScalingKeys[m].mTime);
						if(it != keyframes.end())
						{
							boost::get<2>(it->second) = &(node_anim->mScalingKeys[m]);
						}
						else
						{
							keyframes[ node_anim->mRotationKeys[m].mTime ] = KeyframeData( NULL, NULL, &(node_anim->mScalingKeys[m]) );
						}
					}
						
					KeyframesMap::iterator it = keyframes.begin();
					KeyframesMap::iterator it_end = keyframes.end();
					for(it; it != it_end; ++it)
					{
						if(it->first < cutTime)	// or should it be <= 
						{
							aiVector3D aiTrans = getTranslate( node_anim, keyframes, it );
					
							Ogre::Vector3 trans(aiTrans.x, aiTrans.y, aiTrans.z);
					
							aiQuaternion aiRot = getRotate(node_anim, keyframes, it);
							Ogre::Quaternion rot(aiRot.w, aiRot.x, aiRot.y, aiRot.z);
							Ogre::Vector3 scale(1,1,1);	// ignore scale for now
					
							Ogre::Vector3 transCopy = trans;
					
							Ogre::Matrix4 fullTransform;
							fullTransform.makeTransform(trans, scale, rot);
					
							Ogre::Matrix4 poseTokey = defBonePoseInv * fullTransform;
							poseTokey.decomposition(trans, scale, rot);
					
							keyframe = track->createNodeKeyFrame(Ogre::Real(it->first));
					
							// weirdness with the root bone, But this seems to work
							if(mSkeleton->getRootBone()->getName() == boneName)
							{
								trans = transCopy - bone->getPosition();
							}

							keyframe->setTranslate(trans);
							keyframe->setRotation(rot);
						}
					}

				} // if bone exists
		
			} // loop through channels	
			mSkeleton->optimiseAllAnimations();	
		}
		//-----------------------------------------------------------------------
		void UAssimpLoader::markAllChildNodesAsNeeded(const aiNode *pNode)
		{
			flagNodeAsNeeded(pNode->mName.data);
			// Traverse all child nodes of the current node instance
			for (unsigned int childIdx = 0; childIdx < pNode->mNumChildren; ++childIdx )
			{
				const aiNode *pChildNode = pNode->mChildren[ childIdx ];
				markAllChildNodesAsNeeded(pChildNode);
			}
		}
		//-----------------------------------------------------------------------
		void UAssimpLoader::grabNodeNamesFromNode(const aiScene* mScene, const aiNode* pNode)
		{
			boneNode bNode;
			bNode.node = const_cast<aiNode*>(pNode);
			if(NULL != pNode->mParent)
			{
				bNode.parent = const_cast<aiNode*>(pNode->mParent);
			}
			bNode.isNeeded = false;
			boneMap.insert(std::pair<Ogre::String, boneNode>(Ogre::String(pNode->mName.data), bNode));
			mBoneNodesByName[pNode->mName.data] = pNode;
			Ogre::LogManager::getSingleton().logMessage("Node " + Ogre::String(pNode->mName.data) + " found.");

			// Traverse all child nodes of the current node instance
			for (unsigned int childIdx = 0 ; childIdx < pNode->mNumChildren ; ++childIdx )
			{
				const aiNode *pChildNode = pNode->mChildren[ childIdx ];
				grabNodeNamesFromNode(mScene, pChildNode);
			}
		}
		//-----------------------------------------------------------------------
		void UAssimpLoader::computeNodesDerivedTransform(const aiScene* mScene,  const aiNode *pNode, const aiMatrix4x4 accTransform)
		{
			if(mNodeDerivedTransformByName.find(pNode->mName.data) == mNodeDerivedTransformByName.end())
			{
				mNodeDerivedTransformByName[pNode->mName.data] = accTransform;
			}
			for (unsigned int childIdx = 0; childIdx < pNode->mNumChildren; ++childIdx )
			{
				const aiNode *pChildNode = pNode->mChildren[ childIdx ];
				computeNodesDerivedTransform(mScene, pChildNode, accTransform * pChildNode->mTransformation);
			}
		}
		//-----------------------------------------------------------------------
		void UAssimpLoader::createBonesFromNode(const aiScene* mScene,  const aiNode *pNode)
		{
			if(isNodeNeeded(pNode->mName.data))
			{				
				Ogre::Bone* bone = mSkeleton->createBone(Ogre::String(pNode->mName.data), msBoneCount);
		
				aiQuaternion rot;
				aiVector3D pos;
				aiVector3D scale;	
		
				// above should be the same as
				aiMatrix4x4 aiM = pNode->mTransformation;		
				aiM.Decompose(scale, rot, pos);		
				if (!aiM.IsIdentity())
				{
					bone->setPosition(pos.x, pos.y, pos.z);
					bone->setOrientation(rot.w, rot.x, rot.y, rot.z);
				}				
				Ogre::LogManager::getSingleton().logMessage(Ogre::StringConverter::toString(msBoneCount) + ") Creating bone '" + Ogre::String(pNode->mName.data) + "'");
				msBoneCount++;
			}
			// Traverse all child nodes of the current node instance
			for (unsigned int childIdx = 0; childIdx < pNode->mNumChildren; ++childIdx )
			{
				const aiNode *pChildNode = pNode->mChildren[ childIdx ];
				createBonesFromNode(mScene, pChildNode);
			}
		}
		//-----------------------------------------------------------------------
		void UAssimpLoader::createBoneHiearchy(const aiScene* mScene,  const aiNode *pNode)
		{
			if(isNodeNeeded(pNode->mName.data))
			{
				Ogre::Bone* parent = 0;
				Ogre::Bone* child = 0;
				if(pNode->mParent)
				{
					if(mSkeleton->hasBone(pNode->mParent->mName.data))
					{
						parent = mSkeleton->getBone(pNode->mParent->mName.data);
					}
				}
				if(mSkeleton->hasBone(pNode->mName.data))
				{
					child = mSkeleton->getBone(pNode->mName.data);
				}
				if(parent && child)
				{
					parent->addChild(child);
				}
			}
			// Traverse all child nodes of the current node instance
			for ( unsigned int childIdx = 0; childIdx < pNode->mNumChildren; childIdx++ )
			{
				const aiNode *pChildNode = pNode->mChildren[ childIdx ];
				createBoneHiearchy(mScene, pChildNode);
			}
		}
		//-----------------------------------------------------------------------
		void UAssimpLoader::flagNodeAsNeeded(const char* name)
		{
			boneMapType::iterator iter = boneMap.find(Ogre::String(name));
			if( iter != boneMap.end())
			{
				iter->second.isNeeded = true;
			}
		}
		//-----------------------------------------------------------------------
		bool UAssimpLoader::isNodeNeeded(const char* name)
		{
			boneMapType::iterator iter = boneMap.find(Ogre::String(name));
			if( iter != boneMap.end())
			{
				return iter->second.isNeeded;
			}
			return false;
		}
		//-----------------------------------------------------------------------
		void UAssimpLoader::grabBoneNamesFromNode(const aiScene* mScene,  const aiNode *pNode)
		{
			static int meshNum = 0;
			meshNum++;
			if(pNode->mNumMeshes > 0)
			{
				for (unsigned int idx = 0; idx < pNode->mNumMeshes; ++idx )
				{
					aiMesh *pAIMesh = mScene->mMeshes[ pNode->mMeshes[ idx ] ];
			
					if(pAIMesh->HasBones())
					{
						for ( Ogre::uint32 i=0; i < pAIMesh->mNumBones; ++i )
						{
							aiBone *pAIBone = pAIMesh->mBones[ i ];
							if ( NULL != pAIBone )
							{
								mBonesByName[pAIBone->mName.data] = pAIBone;

								Ogre::LogManager::getSingleton().logMessage(Ogre::StringConverter::toString(i) + ") REAL BONE with name : " + Ogre::String(pAIBone->mName.data));

								// flag this node and all parents of this node as needed, until we reach the node holding the mesh, or the parent.
								aiNode* node = mScene->mRootNode->FindNode(pAIBone->mName.data);
								while(node)
								{
									if(node->mName.data == pNode->mName.data)
									{
										// flagNodeAsNeeded(node->mName.data);
										// Set mSkeletonRootNode to this node, which is the same node as the one holding the mesh
										//mSkeletonRootNode = node;
										break;
									}
									if(node->mName.data == pNode->mParent->mName.data)
									{
										//flagNodeAsNeeded(node->mName.data);
										// Set mSkeletonRootNode to this node, which is the parent node to the node holding the mesh
										//mSkeletonRootNode = node;
										break;
									}

									// Not a root node, flag this as needed and continue to the parent
									flagNodeAsNeeded(node->mName.data);
									node = node->mParent;
								}

								// Flag all children of this node as needed
								node = mScene->mRootNode->FindNode(pAIBone->mName.data);
								markAllChildNodesAsNeeded(node);

							} // if we have a valid bone
						} // loop over bones
					} // if this mesh has bones
				} // loop over meshes
			} // if this node has meshes

			// Traverse all child nodes of the current node instance
			for (unsigned int childIdx = 0; childIdx < pNode->mNumChildren; childIdx++ )
			{
				const aiNode *pChildNode = pNode->mChildren[ childIdx ];
				grabBoneNamesFromNode(mScene, pChildNode);
			}
		}
		//-----------------------------------------------------------------------
		Ogre::String ReplaceSpaces(const Ogre::String& s)
		{
			Ogre::String res(s);
			replace(res.begin(), res.end(), ' ', '_');
			return res;
		}
		//-----------------------------------------------------------------------
		Ogre::MaterialPtr UAssimpLoader::createMaterialByScript(int index, const aiMaterial* mat)
		{
			// Create a material in code as using script inheritance variable substitution and other goodies		
			Ogre::MaterialManager* matMgr = Ogre::MaterialManager::getSingletonPtr();
			Ogre::String materialName = mBasename + "#" + Ogre::StringConverter::toString(index);
			Ogre::MaterialPtr matPtr;
			if(matMgr->resourceExists(materialName))
			{
				matPtr = matMgr->getByName(materialName);
				if(matPtr->isLoaded())
				{
					return matPtr;
				}
			}	
			
			Ogre::String code;
	
			aiColor4D c(0.0f, 0.0f, 0.0f, 1.0);    
			aiGetMaterialColor(mat, AI_MATKEY_COLOR_AMBIENT,  &c);
			code += "\tset $ambient_value \"" + toString(c) + "\"\n";
	
			c = aiColor4D(0.0f, 0.0f, 0.0f, 1.0f);    
			aiGetMaterialColor(mat, AI_MATKEY_COLOR_DIFFUSE, &c);
			code += "\tset $diffuse_value \"" + toString(c) + "\"\n";
	
			c = aiColor4D(0.0f, 0.0f, 0.0f, 1.0f);
			aiGetMaterialColor(mat, AI_MATKEY_COLOR_SPECULAR, &c);
			code += "\tset $specular_value \"" + toString(c) + "\"\n";
	
			c = aiColor4D(0.0f, 0.0f, 0.0f, 1.0f);
			aiGetMaterialColor(mat, AI_MATKEY_COLOR_EMISSIVE, &c);
			code += "\tset $emissive_value \"" + toString(c) + "\"\n";
	
	
			// Specifies the type of the texture to be retrieved ( e.g. diffuse, specular, height map ...) 
			enum aiTextureType type = aiTextureType_DIFFUSE;		
	
			// Index of the texture to be retrieved. The function fails if there is no texture of that type with this index. 
			// GetTextureCount() can be used to determine the number of textures per texture type. 
	
			// Receives the path to the texture. NULL is a valid value. 
			aiString path;
	
			// The texture mapping. NULL is allowed as value. 
			aiTextureMapping mapping = aiTextureMapping_UV;
	
			// Receives the UV index of the texture. NULL is a valid value. 
			unsigned int uvindex = 0;
	
			// Receives the blend factor for the texture NULL is a valid value. 
			float blend = 1.0f;
	
			// Receives the texture operation to be performed between this texture and the previous texture. NULL is allowed as value. 
			aiTextureOp op = aiTextureOp_Multiply;
	
			// Receives the mapping modes to be used for the texture. The parameter may be NULL but if it is a valid pointer it 
			// MUST point to an array of 3 aiTextureMapMode's (one for each axis: UVW order (=XYZ)). 
			aiTextureMapMode mapmode =  aiTextureMapMode_Wrap;
	
			// For now assuming at most that only one diffuse texture exists
			if (mat->GetTexture(type, 0, &path, &mapping, &uvindex, &blend, &op, &mapmode) == AI_SUCCESS)
			{
				Ogre::String texBasename, texExtention, texPath;
				
				Ogre::String szPatah = Ogre::String(path.data);
				Ogre::StringUtil::splitFullFilename(szPatah, texBasename, texExtention, texPath);
		
				Ogre::String texName = texBasename + "." + texExtention;
		
				//code += "\tset $diffuse_map \"" + texName + "\"\n";
				code += "\tset $diffuse_map \"" + szPatah + "\"\n";
		
				int twoSided = 0;
				mat->Get(AI_MATKEY_TWOSIDED, twoSided);
				if(twoSided != 0)
				{
					code += "set $cull_hardware_value none\n";
				}
				
				Ogre::ResourceGroupManager::getSingleton().addResourceLocation( szPatah, "FileSystem", "General");
				// no infomation on the alpha channel in the texture will have to load the texture and look at it
			}

			if( materialName == "")
				return matPtr;
	
			code = "material " + materialName + " : base\n{\n" + code + "}\n\n";
			mMaterialCode += code;

			// compile the material
			code = "import * from base.material\n" + code;
		
			Ogre::DataStreamPtr stream(OGRE_NEW Ogre::MemoryDataStream(const_cast<void*>(static_cast<const void*>(code.c_str())),
													   code.length() * sizeof(char), false));

			try
			{
				Ogre::MaterialManager::getSingleton().parseScript(stream, Ogre::ResourceGroupManager::DEFAULT_RESOURCE_GROUP_NAME);
				Ogre::MaterialPtr omat = Ogre::MaterialManager::getSingleton().getByName(materialName);
				omat->compile();
				omat->load();
				return omat;
			}
			catch(Ogre::Exception& e)
			{

			}
			return matPtr;			
		}
		//-----------------------------------------------------------------------
		Ogre::MaterialPtr UAssimpLoader::createMaterial(int index, const aiMaterial* mat, const Ogre::String& mDir)
		{
			static int dummyMatCount = 0;

			// extreme fallback texture -- 2x2 hot pink
			static Ogre::uint8 s_RGB[] = {128, 0, 255, 128, 0, 255, 128, 0, 255, 128, 0, 255};

			std::ostringstream matname;
			Ogre::MaterialManager* omatMgr =  Ogre::MaterialManager::getSingletonPtr();
			enum aiTextureType type = aiTextureType_DIFFUSE;
			static aiString path;
			aiTextureMapping mapping = aiTextureMapping_UV;       // the mapping (should be uv for now)
			unsigned int uvindex = 0;                             // the texture uv index channel
			float blend = 1.0f;                                   // blend
			aiTextureOp op = aiTextureOp_Multiply;                // op
			aiTextureMapMode mapmode[2] =  { aiTextureMapMode_Wrap, aiTextureMapMode_Wrap };    // mapmode
			std::ostringstream texname;

			aiString szPath;
			if(AI_SUCCESS == aiGetMaterialString(mat, AI_MATKEY_TEXTURE_DIFFUSE(0), &szPath))
			{
				Ogre::LogManager::getSingleton().logMessage("Using aiGetMaterialString : Found texture " + Ogre::String(szPath.data) + " for channel " + Ogre::StringConverter::toString(uvindex));
			}
			if(szPath.length < 1)
			{
				Ogre::LogManager::getSingleton().logMessage("Didn't find any texture units...");
				szPath = Ogre::String("dummyMat" + Ogre::StringConverter::toString(dummyMatCount)).c_str();
				dummyMatCount++;
			}

			Ogre::String basename;
			Ogre::String outPath;
			Ogre::StringUtil::splitFilename(Ogre::String(szPath.data), basename, outPath);
			Ogre::LogManager::getSingleton().logMessage("Creating " + basename);

			Ogre::ResourceManager::ResourceCreateOrRetrieveResult status = omatMgr->createOrRetrieve(ReplaceSpaces(basename), "General", true);
			Ogre::MaterialPtr omat = status.first;
			if (!status.second)
				return omat;

			// ambient
			aiColor4D clr(1.0f, 1.0f, 1.0f, 1.0);
			//Ambient is usually way too low! FIX ME!
			if (mat->GetTexture(type, 0, &path) != AI_SUCCESS)
				aiGetMaterialColor(mat, AI_MATKEY_COLOR_AMBIENT,  &clr);
			omat->setAmbient(clr.r, clr.g, clr.b);

			// diffuse
			clr = aiColor4D(1.0f, 1.0f, 1.0f, 1.0f);
			if(AI_SUCCESS == aiGetMaterialColor(mat, AI_MATKEY_COLOR_DIFFUSE, &clr))
			{
				omat->setDiffuse(clr.r, clr.g, clr.b, clr.a);
			}

			// specular
			clr = aiColor4D(1.0f, 1.0f, 1.0f, 1.0f);
			if(AI_SUCCESS == aiGetMaterialColor(mat, AI_MATKEY_COLOR_SPECULAR, &clr))
			{
				omat->setSpecular(clr.r, clr.g, clr.b, clr.a);
			}

			// emissive
			clr = aiColor4D(1.0f, 1.0f, 1.0f, 1.0f);
			if(AI_SUCCESS == aiGetMaterialColor(mat, AI_MATKEY_COLOR_EMISSIVE, &clr))
			{
				omat->setSelfIllumination(clr.r, clr.g, clr.b);
			}

			float fShininess;
			if(AI_SUCCESS == aiGetMaterialFloat(mat, AI_MATKEY_SHININESS, &fShininess))
			{
				omat->setShininess(Ogre::Real(fShininess));
			}

			if (mat->GetTexture(type, 0, &path) == AI_SUCCESS)
			{
				Ogre::LogManager::getSingleton().logMessage("Found texture " + Ogre::String(path.data) + " for channel " + Ogre::StringConverter::toString(uvindex));
				if(AI_SUCCESS == aiGetMaterialString(mat, AI_MATKEY_TEXTURE_DIFFUSE(0), &szPath))
				{
					Ogre::LogManager::getSingleton().logMessage("Using aiGetMaterialString : Found texture " + Ogre::String(szPath.data) + " for channel " + Ogre::StringConverter::toString(uvindex));
				}

				// attempt to load the image
				Ogre::Image image;
				// possibly if we fail to actually find it, pop up a box?
				Ogre::String pathname(mDir + path.data);

				std::ifstream imgstream;
				imgstream.open(pathname.c_str(), std::ios::binary);
				if(!imgstream.is_open())
				{
					//imgstream.open(Ogre::String(mDir + Ogre::String("\\") + Ogre::String(path.data)).c_str(), std::ios::binary);
					imgstream.open(Ogre::String(mDir + Ogre::String(path.data)).c_str(), std::ios::binary);
				}

				if (imgstream.is_open())
				{
					// Wrap as a stream
					Ogre::DataStreamPtr strm(OGRE_NEW Ogre::FileStreamDataStream(pathname.c_str(), &imgstream, false));
					if (!strm->size() || strm->size() == 0xffffffff)
					{
						// fall back to our very simple and very hardcoded hot-pink version
						Ogre::DataStreamPtr altStrm(OGRE_NEW Ogre::MemoryDataStream(s_RGB, sizeof(s_RGB)));
						image.loadRawData(altStrm, 2, 2, Ogre::PF_R8G8B8);
						Ogre::LogManager::getSingleton().logMessage("Could not load texture, falling back to hotpink");
					} 
					else
					{
						// extract extension from filename
						size_t pos = pathname.find_last_of('.');
						Ogre::String ext = pathname.substr(pos+1);
						image.load(strm, ext);
						imgstream.close();
					}
				} 
				else
				{
					// fall back to our very simple and very hardcoded hot-pink version
					Ogre::DataStreamPtr altStrm(OGRE_NEW Ogre::MemoryDataStream(s_RGB, sizeof(s_RGB)));
					image.loadRawData(altStrm, 2, 2, Ogre::PF_R8G8B8);
					Ogre::LogManager::getSingleton().logMessage("Could not load texture, falling back to hotpink - 2");
				}
				Ogre::ResourceGroupManager::getSingleton().addResourceLocation( pathname , "FileSystem", "General");
				Ogre::TextureManager::getSingleton().loadImage(basename, Ogre::ResourceGroupManager::DEFAULT_RESOURCE_GROUP_NAME, image);
				//TODO: save this to materials/textures ?
				Ogre::TextureUnitState* texUnitState = omat->getTechnique(0)->getPass(0)->createTextureUnitState(basename);
			}
			
			omat->getTechnique(0)->getPass(0)->setDepthCheckEnabled(true);
			omat->getTechnique(0)->getPass(0)->setDepthWriteEnabled(true);			
			omat->load();
			return omat;
		}				
		//-----------------------------------------------------------------------
		bool UAssimpLoader::createSubMesh(const Ogre::String& name, int index, const aiNode* pNode, const aiMesh *mesh, const aiMaterial* mat, Ogre::MeshPtr mMesh, Ogre::AxisAlignedBox& mAAB, const Ogre::String& mDir)
		{
			// if animated all submeshes must have bone weights
			if(mBonesByName.size() && !mesh->HasBones())
			{
				Ogre::LogManager::getSingleton().logMessage("Skipping Mesh " + Ogre::String(mesh->mName.data) + "with no bone weights");
				return false; 
			}
		
			Ogre::MaterialPtr matptr = createMaterial(mesh->mMaterialIndex, mat, mDir);		
			
			// now begin the object definition
			// We create a submesh per material
			Ogre::SubMesh* submesh = mMesh->createSubMesh(name + Ogre::StringConverter::toString(index));		
			
			// prime pointers to vertex related data
			aiVector3D *vec = mesh->mVertices;
			aiVector3D *norm = mesh->mNormals;
			aiVector3D *uv = mesh->mTextureCoords[0];
			//aiColor4D *col = mesh->mColors[0];

			// We must create the vertex data, indicating how many vertices there will be
			submesh->useSharedVertices = false;
			submesh->vertexData = new Ogre::VertexData();
			submesh->vertexData->vertexStart = 0;
			submesh->vertexData->vertexCount = mesh->mNumVertices;

			// We must now declare what the vertex data contains
			Ogre::VertexDeclaration* declaration = submesh->vertexData->vertexDeclaration;
			static const unsigned short source = 0;
			size_t offset = 0;
			offset += declaration->addElement(source,offset,Ogre::VET_FLOAT3,Ogre::VES_POSITION).getSize();

			Ogre::LogManager::getSingleton().logMessage(Ogre::StringConverter::toString(mesh->mNumVertices) + " vertices");
			if (norm)
			{
				Ogre::LogManager::getSingleton().logMessage(Ogre::StringConverter::toString(mesh->mNumVertices) + " normals");
				offset += declaration->addElement(source,offset,Ogre::VET_FLOAT3,Ogre::VES_NORMAL).getSize();
			}

			if (uv)
			{
				Ogre::LogManager::getSingleton().logMessage(Ogre::StringConverter::toString(mesh->mNumVertices) + " uvs");
				offset += declaration->addElement(source,offset,Ogre::VET_FLOAT2,Ogre::VES_TEXTURE_COORDINATES).getSize();
			}	

			// We create the hardware vertex buffer
			Ogre::HardwareVertexBufferSharedPtr vbuffer =
				Ogre::HardwareBufferManager::getSingleton().createVertexBuffer(declaration->getVertexSize(source), // == offset
				submesh->vertexData->vertexCount,   // == nbVertices
				Ogre::HardwareBuffer::HBU_STATIC_WRITE_ONLY);

			aiMatrix4x4 aiM = mNodeDerivedTransformByName.find(pNode->mName.data)->second;	

			// Now we get access to the buffer to fill it.  During so we record the bounding box.
			float* vdata = static_cast<float*>(vbuffer->lock(Ogre::HardwareBuffer::HBL_DISCARD));
			for (size_t i=0;i < mesh->mNumVertices; ++i)
			{
				// Position
				aiVector3D vect;
				vect.x = vec->x;
				vect.y = vec->y;
				vect.z = vec->z;
		
				vect *= aiM;
		
				Ogre::Vector3 position( vect.x, vect.y, -vect.z );
				*vdata++ = vect.x;
				*vdata++ = vect.y;
				*vdata++ = -vect.z;
				mAAB.merge(position);
				vec++;

				// Normal
				if (norm)
				{
					vect.x = norm->x;
					vect.y = norm->y;
					vect.z = norm->z;

					vect *= aiM;
			
					*vdata++ = -vect.x;
					*vdata++ = -vect.y;
					*vdata++ = -vect.z;
					norm++;			
				}

				// uvs
				if (uv)
				{
					*vdata++ = uv->x;
					*vdata++ = uv->y;
					uv++;
				}
			}

			vbuffer->unlock();
			submesh->vertexData->vertexBufferBinding->setBinding(source,vbuffer);

			Ogre::LogManager::getSingleton().logMessage(Ogre::StringConverter::toString(mesh->mNumFaces) + " faces");
			aiFace *f = mesh->mFaces;
			int nNumIdx = f->mNumIndices;
			// Creates the index data
			submesh->indexData->indexStart = 0;
			submesh->indexData->indexCount = mesh->mNumFaces * nNumIdx;
			submesh->indexData->indexBuffer =
				Ogre::HardwareBufferManager::getSingleton().createIndexBuffer(Ogre::HardwareIndexBuffer::IT_16BIT,
				submesh->indexData->indexCount,
				Ogre::HardwareBuffer::HBU_STATIC_WRITE_ONLY);
			Ogre::uint16* idata = static_cast<Ogre::uint16*>(submesh->indexData->indexBuffer->lock(Ogre::HardwareBuffer::HBL_DISCARD));

			Ogre::LogManager::getSingleton().logMessage(Ogre::StringConverter::toString(mesh->mNumFaces) + " faces");

			aiFace *firstFace = mesh->mFaces;
			// poke in the face data
			for (size_t i=0; i < mesh->mNumFaces;++i)
			{
				if(f->mNumIndices == 2)
				{					
					if( mLoaderParams & LP_REVERSE_FACE_INDEX)
					{
						*idata++ = f->mIndices[0];
						*idata++ = f->mIndices[1];
					}
					else
					{
						*idata++ = f->mIndices[0];
						*idata++ = f->mIndices[1];
					}	
				}
				else if( f->mNumIndices == 3)
				{
					if( mLoaderParams & LP_REVERSE_FACE_INDEX)
					{
						*idata++ = f->mIndices[0];
						*idata++ = f->mIndices[2];
						*idata++ = f->mIndices[1];
					}
					else
					{
						*idata++ = f->mIndices[0];
						*idata++ = f->mIndices[1];
						*idata++ = f->mIndices[2];
					}				
				}
				
				f++;
			}
			submesh->indexData->indexBuffer->unlock();

			// set bone weigths
			if(mesh->HasBones())
			{
				for ( Ogre::uint32 i=0; i < mesh->mNumBones; i++ )
				{
					aiBone *pAIBone = mesh->mBones[ i ];
					if ( NULL != pAIBone )
					{
						Ogre::String bname = pAIBone->mName.data;
						for ( Ogre::uint32 weightIdx = 0; weightIdx < pAIBone->mNumWeights; weightIdx++ )
						{
							aiVertexWeight aiWeight = pAIBone->mWeights[ weightIdx ];

							Ogre::VertexBoneAssignment vba;
							vba.vertexIndex = aiWeight.mVertexId;
							vba.boneIndex = mSkeleton->getBone(bname)->getHandle();
							vba.weight= aiWeight.mWeight;

							submesh->addBoneAssignment(vba);
						}
					}
				}
			} // if mesh has bones

			// Finally we set a material to the submesh
			submesh->setMaterialName(matptr->getName());
			return true;
		}
		//-----------------------------------------------------------------------
		bool UAssimpLoader::CheckMeshResouces(const aiNode* pNode, unsigned long& id)
		{
			char buf[256];
			sprintf(buf, "%s#%ld", pNode->mName.data, id);
			if( Ogre::MeshManager::getSingleton().resourceExists(buf) == true )
			{
				id = UnE::Core::UDB::GetUDB()->GetNextCookie();
				return CheckMeshResouces(pNode, id);
			}
			return true;
		}
		//-----------------------------------------------------------------------
		void UAssimpLoader::loadDataFromNode(const aiScene* mScene,  const aiNode *pNode, const Ogre::String& mDir,bool bCreateEntity)
		{			
			char buf[512];
			sprintf(buf, "%s", "");
			if(pNode->mNumMeshes > 0)
			{				
				unsigned long nMesh = UnE::Core::UDB::GetUDB()->GetNextCookie();
				Ogre::MeshPtr mesh;
				Ogre::AxisAlignedBox mAAB;		
				
				sprintf(buf, "%d#%s#%ld", (int)m_hWnd, pNode->mName.data, nMesh);
				mesh = Ogre::MeshManager::getSingleton().createManual(buf, "General");								
				mMeshes.push_back(mesh);
				
				mAAB = mesh->getBounds();
		
				for (unsigned int idx = 0; idx < pNode->mNumMeshes; ++idx )
				{
					aiMesh *pAIMesh = mScene->mMeshes[ pNode->mMeshes[ idx ] ];
					
					if( pAIMesh->mPrimitiveTypes < 2 )
						continue;
					Ogre::LogManager::getSingleton().logMessage("SubMesh " + Ogre::StringConverter::toString(idx) + " for mesh '" + Ogre::String(pNode->mName.data) + "'");
			
					// Create a material instance for the mesh.
					const aiMaterial *pAIMaterial = mScene->mMaterials[ pAIMesh->mMaterialIndex ];
					createSubMesh(pAIMesh->mName.data, idx, pNode, pAIMesh, pAIMaterial, mesh, mAAB, mDir);
					
					if( idx == 0)
						sprintf(buf, "%d#%s#%ld", (int)m_hWnd, pAIMesh->mName.data, nMesh);
				}			

				// We must indicate the bounding box
				mesh->_setBounds(mAAB);
				mesh->_setBoundingSphereRadius((mAAB.getMaximum()- mAAB.getMinimum()).length() / 2.0f);
				
				WndCtx * pCtx = GetWndContext(m_hWnd);
				pCtx->aabb.merge(mAAB);
				
				std::string buildName = pNode->mName.data;
				std::string er1 = "_";
				std::string er2 = "_PIVOT";
				std::string r1 = "";

				if( buildName.length() > 0)
				{
					const char * c = buildName.c_str();
					if( c[0] == 'z')
					{
						buildName = Poco::replace(buildName, "z", "");
					}
				}
				
				std::string bName = Poco::replace(buildName, er2, r1);
				
				char buf2[512];
				sprintf(buf2, "%d#%s#%d", (int)m_hWnd, pNode->mName.data,nMesh);
				std::string szName = std::string(buf2);
				if( szName == "")
				{
					sprintf(buf2, "%d#DAE_SCENENODE#%ld", (int)m_hWnd,  nMesh);
					szName = std::string(buf2);
				}	
				
				Ogre::Entity* pEntity	= pCtx->sceneMgr->createEntity(szName.c_str(), mesh);

				UnE::Core::RenderableContext entityContext;
				entityContext.selected = false;
				entityContext.ignoreViewDetail = false;

				if( pEntity->getNumSubEntities() > 0)
				{
					Ogre::MaterialPtr ptrMat = pEntity->getSubEntity(0)->getMaterial();
					if( ! ptrMat.isNull() )
					{
						ptrMat->getTechnique(0)->setLightingEnabled(true);	
					}	
					pEntity->getSubEntity(0)->setUserAny(Ogre::Any(entityContext));	
				}
				pEntity->setUserAny(Ogre::Any(entityContext));
				
				UnE::Core::UEntity * pObject = new UnE::Core::UEntity();
				pObject->SetInternal(pEntity);
				pObject->SetAlias(bName);
				pObject->SetName(szName);
				pObject->InitAmimationState();
				pCtx->objectManager->AddUObject(pObject);				
				
				Ogre::SceneNode* pSNode	= pCtx->sceneMgr->getRootSceneNode()->createChildSceneNode(szName.c_str());
				pSNode->attachObject( pEntity );			
				pSNode->_updateBounds();

				Ogre::LogManager::getSingleton().logMessage("NodeName : " + szName);

				UnE::Core::UBaseModel* pModel = UnE::Core::UDB::GetBaseModel((int)m_hWnd);
				UnE::Core::USceneNodeManager * pUSeneMan = pModel->GetSecneManager();
				UnE::Core::USceneNode * pUNode = pUSeneMan->GetRootSceneNode()->CreateChild(szName);
				pUNode->SetAliasName(bName);
				pUNode->SetTag(pSNode);
			}

			// Traverse all child nodes of the current node instance
			for ( unsigned int childIdx = 0; childIdx < pNode->mNumChildren; childIdx++ )
			{
				const aiNode *pChildNode = pNode->mChildren[ childIdx ];
				loadDataFromNode(mScene, pChildNode, mDir);
			}
		}
		//-----------------------------------------------------------------------
		Ogre::String UAssimpLoader::getExtensionList()
		{
			aiString s;
			Assimp::Importer importer;
			importer.GetExtensionList(s);
			return Ogre::String(s.data);
		}
		//-----------------------------------------------------------------------

		
	}
}

