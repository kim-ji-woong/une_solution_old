#pragma once


#include <CoreAPI.h>
#include "Ogre.h"
#include "VisiblityMask.h"

#define VIRTUALCAM_MINFOV 1 // 최소 0도 까지 가능
#define VIRTUALCAM_MAXFOV 170 // 최대 180도 까지 가능

#ifndef OGRE_DELETE_ARRAY
#define OGRE_DELETE_ARRAY(p) { if(p) { delete[] (p);   (p)=NULL; } }
#endif

#define RENDER_QUEUE_OUTLINE_GLOW_OBJECTS	RENDER_QUEUE_4 + 1
#define RENDER_QUEUE_OUTLINE_GLOW_GLOWS		RENDER_QUEUE_4 + 2
#define LAST_STENCIL_OP_RENDER_QUEUE		RENDER_QUEUE_OUTLINE_GLOW_GLOWS

namespace UnE
{
	namespace Core
	{
		// 컴포넌트들을 식별
		enum ComponentType
		{
			COMTYPE_CAMERA       = 1 << 0,     // virtual camera
			COMTYPE_VLIGHT       = 1 << 1,     // vertex light
			COMTYPE_MLIGHT       = 1 << 2,     // map light
			COMTYPE_OBJECT       = 1 << 3,     // object
			COMTYPE_EFFECTOBJECT = 1 << 8,  // effect object
			COMTYPE_LIGHTOBJECT  = 1 << 10,  // light object
			COMTYPE_ALL          = 0xFFFFFFFF, // all component
		};

		// 몇몇 컴포넌트들은 선택의 가시화를 달리 할 수 있다.
		enum ComponentSelectType{
			CST_BoundingBox,    // 바운딩박스를 통해 선택 가시화
			CST_EmphasizeColor, // 색상 톤 변경을 통해 선택 가시화
		};

		// 픽킹이나 컴포넌트 선택에 사용되는 세부 오브젝트
		enum ComponentSubObject
		{
			CSO_NOTHING,
			CSO_BODY,
			CSO_TARGET,
			CSO_ALL
		};

		// 컴포넌트의 이동함수에 사용되는 이동방향
		enum MoveDirection
		{
			MOVEDIR_LEFT_RIGHT,
			MOVEDIR_FORWARD_BACKWARD,
			MOVEDIR_UP_DOWN
		};

		// CBaseCamera를 상속받은 카메라들을 식별
		enum CameraType
		{
			CAMTYPE_NAVI,
			CAMTYPE_VIRTUAL,
		};

		enum AnimationPlayState{
			APS_PLAY,
			APS_STOP,
		};

		// 픽킹결과 구조체
		struct ComponentPickResert
		{
			bool           bHit;
			float          dist;
			ComponentSubObject cso;
			ComponentPickResert()
			{ 
				bHit = false; 
				dist = 0; 
				cso = CSO_NOTHING;
			}
		};

		struct TringleIndex{
			long index[3];
		};

	
		class CORE_API CBaseCamera
		{
		public:
			CBaseCamera(CameraType type):mCamType(type){}
			~CBaseCamera(void){}

			/** 상속받은 카메라의 종류를 리턴 */
			CameraType GetCameraType(){return mCamType;}
			virtual void ReSize(float width, float heingt) = 0;
			virtual Ogre::Camera* GetOgreCamera() = 0;
			virtual Ogre::String GetCameraName() = 0;
			virtual Ogre::String GetDisplayName() = 0;
			virtual void SetVisiblityMask(Ogre::uint32 masks) = 0;
			virtual Ogre::uint32 GetVisiblityMask() = 0;

		protected:
			CameraType mCamType;
		};

		/* 대부분의 컴포넌트들은 이 클래스를 상속 받는다. */
		class CORE_API UCoreComponent : public Ogre::FrameListener
		{
		public:
			UCoreComponent(ComponentType type):mCompType(type){}
			~UCoreComponent(void){}

			/** 상속받은 컴포넌트의 종류를 리턴 */
			ComponentType GetComponentType(){return mCompType;}

			virtual ComponentPickResert Picking(Ogre::Ray pickRay) = 0;
			virtual void SelectComponent(ComponentSubObject cso) = 0;
			virtual void DeSelect() = 0;
			virtual Ogre::SceneNode* GetBodyNode() = 0;
			virtual Ogre::SceneNode* GetTargetNode(){return NULL;}
			virtual void Move(float dist, MoveDirection direction, Ogre::Camera* pCurrViewCam = NULL) = 0;
			virtual void Move(const Ogre::Vector3& vMove) = 0;
			virtual void Release() = 0;
			virtual void Restore() = 0;

        
		protected:
			ComponentType mCompType;
		};

		class CORE_API UCoreUtil
		{
		public:
			UCoreUtil(void){}
			~UCoreUtil(void){}

			static Ogre::Radian ToRadian(float angle, bool bDegree);
			static void MakeBoundingBox(Ogre::ManualObject* pMo, const Ogre::AxisAlignedBox& box, const Ogre::ColourValue& boxColor = Ogre::ColourValue(1,1,0), bool bNoDepth = true);

			/** Entity의 축에 정렬되지 않은 바운딩박스를 픽킹, 직육면체에 가까운 형태라면 추천 */
			static std::pair<bool, float> UtilPickEntity(const Ogre::Ray& pickRay, const Ogre::Entity* pEntity);

			/** Entity의 정점정보들을 이용한 정밀한 픽킹(Animation object도 정밀 픽킹) */
			static std::pair<bool, float> UtilPickEntityEx(const Ogre::Ray& pickRay, const Ogre::Entity* pEntity);

			static void GetMeshInformationEx(const Ogre::Entity* pEntity, size_t &vertex_count, Ogre::Vector3* &vertices, size_t &index_count, unsigned long* &indices, bool bUseScale = true, bool bOnlyParentScale = false);

			/** 여러개의 면으로 구성된 볼륨에 대해 광선이 교차하는지 검사 */
			static std::pair<bool, float> UtilIntersect(const Ogre::PlaneBoundedVolume& vol, const Ogre::Ray& ray);

			/** 여러개의 면으로 구성된 볼륨에 대해 한 점이 내부에 위치하는지 검사 */
			static bool UtilIntersect(const Ogre::PlaneBoundedVolume& vol, const Ogre::Vector3& pos);

			/** 여러개의 면으로 구성된 볼륨에 대해 구가 내부에 위치하는지 검사 */
			static bool UtilIntersect(const Ogre::PlaneBoundedVolume& vol, const Ogre::Vector3& pos, float radius);

			/** 두 점으로 구성된 선에 대해 광선이 교차하는지 검사 
			@param ray : 광선
			@param p1 : 선을 구성하는 점
			@param p2 : 선을 구성하는 점
			@param snapSens : 광선과 선의 교차를 판단할 때의 허용 오차
			*/
			static bool UtilIntersectLine(const Ogre::Ray& ray, const Ogre::Vector3& p1, const Ogre::Vector3& p2, float snapSens);

			/** 광선과 네 점으로 이루어진 폴리곤에 대한 교차 검사 */
			static bool UtilIntersectPolygon(const Ogre::Ray& ray, const Ogre::Vector3& p1, const Ogre::Vector3& p2, const Ogre::Vector3& p3, const Ogre::Vector3& p4);

			/** 멀티바이트의 Ogre::String 을 유니코드의 Ogre::DisplayString으로 변환 */
			static Ogre::DisplayString StringToDisplayString(Ogre::String str);

			/** 0~360 사이의 값으로 변경 */
			static float ReCalAngle(float angle);

			/** 특정 노드의 하위노드를 탐색하여 특정 면과 교차하는 object들을 찾아낸다.
			@param outObj : 면과 교차하는 오브젝트들을 담는다.
			@param pSceneNode : 탐색할 노드들의 상위노드
			@param containNames : 탐색하고자 하는 노드 이름 리스트, 해당 이름이 포함되어있는 노드만 탐색, 비어있을 경우 모든 하위노드 탐색
			@param exceptNames : 탐색에서 제외하고자 하는 노드 이름 리스트, 해당 이름이 포함되어 있는 노드는 탐색하지 않음. 비어있을 경우 모든 하위노드 탐색
			@param plane : 교차검사를 할 면
			@param bCompairPlane : false 이면 면과 비교를 하지 않고 하위 노드의 모든 오브젝트 탐색
			*/
			static void UtilSearchPlaneIntersectObject(std::vector<Ogre::MovableObject*>& outObjs, Ogre::SceneNode* pSceneNode, 
				std::vector<Ogre::String> containNames, std::vector<Ogre::String> exceptNames, const Ogre::Plane& plane, bool bCompairPlane = true);

			static void UtilSearchAdjWall(std::vector<Ogre::MovableObject*>& outObjs, Ogre::SceneNode* pSceneNode, Ogre::MovableObject* pObj);
			static void UtilSearchAdjWall2(std::vector<Ogre::MovableObject*>& outObjs, Ogre::MovableObject* pObj);

			static bool EqualVector3(const Ogre::Vector3& p1, const Ogre::Vector3& p2);

			/** 오브젝트 리스트에서 특정 면과 교차하는 선 검출
			@param outLines : 검출 된 라인들을 담는다.
			@param objs : 면과 교차하는 라인을 검출하고자하는 오브젝트 리스트
			@param plane : 오브젝트와 교차 검사를 할 면
			@param rootNodeScale : rootNode의 scale 이 변경되어 있다면 변경된 스케일을 넣어준다.
			*/
			static void UtilGetCrossLineToPlane(std::vector<std::pair<Ogre::Vector3, Ogre::Vector3>>& outLines, 
				const std::vector<Ogre::MovableObject*>& objs, const Ogre::Plane& plane, Ogre::Vector3 rootNodeScale = Ogre::Vector3(1,1,1));

			/** 특정노드의 하위 노드를 탐색하여 특정면과 교차하는 선들을 검출
			@param outLines : 검출 된 라인들을 담는다.
			@param pSceneNode : 탐색할 노드들의 상위노드
			@param containNames : 탐색하고자 하는 노드 이름 리스트, 해당 이름이 포함되어있는 노드만 탐색, 비어있을 경우 모든 하위노드 탐색
			@param exceptNames : 탐색에서 제외하고자 하는 노드 이름 리스트, 해당 이름이 포함되어 있는 노드는 탐색하지 않음. 비어있을 경우 모든 하위노드 탐색
			@param plane : 교차검사를 할 면
			@param rootNodeScale : rootNode의 scale 이 변경되어 있다면 변경된 스케일을 넣어준다.
			*/
			static void UtilGetCrossLineToPlane(std::vector<std::pair<Ogre::Vector3, Ogre::Vector3>>& outLines, Ogre::SceneNode* pSceneNode, 
				std::vector<Ogre::String> containNames, std::vector<Ogre::String> exceptNames, const Ogre::Plane& plane, Ogre::Vector3 rootNodeScale = Ogre::Vector3(1,1,1));

			/** 여러개의 라인들로 부터 닫혀있는 폴리곤 벡터들을 검출한다.
			@param outPolygons : 검출된 폴리곤 정점들과 바운딩박스를 담는다.
			@param outMins : 폴리라인 들중 제일 작은 정점을 담는다.
			@param lines : 폴리곤을 검출하고자 하는 라인 리스트.
			@param bOnlyClosePolyline : false 이면 닫혀있지 않는 폴리라인들도 검출한다.
			*/
			static void UtilLinesToPolygons(std::vector<std::pair<std::vector<Ogre::Vector3>, Ogre::AxisAlignedBox>>& outPolygons, std::vector<Ogre::Vector3>& outMins,
				std::vector<std::pair<Ogre::Vector3, Ogre::Vector3>>& lines, bool bOnlyClosePolyline = true);

			static bool FindNextLine(std::vector<Ogre::Vector3>& polyline, Ogre::AxisAlignedBox& aab, Ogre::Vector3& vMin, std::vector<std::pair<Ogre::Vector3, Ogre::Vector3>>& lines);

			static long UtilTriangulatePolygon(const std::vector<Ogre::Vector3>* points, const Ogre::Vector3& normal, TringleIndex* triangle);

			// 풍선형 폴리곤에만 적용
			static long UtilSimpleTriangulatePolygon(const std::vector<Ogre::Vector3>* points, TringleIndex* triangle);

			/** 지정된 노드 아래에 있는 Entity들의 머터리얼에 와이어프래임 패스를 추가해서 면과 와이어가 함께 보이도록 한다.
			@param pRootNode 변경하고자 하는 엔티티들이 있는 상위 노드
			@param wireColor 선의 색상
			@param bAddWire : true 이면 와이어프래임 패스를 추가, 아니면 제거
			*/
			static void UtilAddPassWireDraw(Ogre::SceneNode* pRootNode, Ogre::ColourValue wireColor, bool bAddWire);

			static void UtilOnlyColorDraw(Ogre::SceneNode* pRootNode, Ogre::ColourValue color, bool bOnlyColor, std::map<Ogre::String, std::pair<Ogre::ColourValue, Ogre::ColourValue>>& colorMap);

			static void UtilColorChange(Ogre::SceneNode* pRootNode, Ogre::ColourValue color, bool bColorChange, std::map<Ogre::String, std::vector<Ogre::MaterialPtr>>& materialMap);

			static void UtilAlphaDraw(Ogre::SceneNode* pRootNode, float alpha, bool bAlpha);

			static long UtilTriangulatePolygonEx(const std::vector<Ogre::Vector3>& points, const Ogre::Vector3& normal, TringleIndex* triangle);

			static bool UtilEqualLine(Ogre::Vector3 a1, Ogre::Vector3 a2, Ogre::Vector3 b1, Ogre::Vector3 b2);
		};

		static inline const Ogre::Vector3& _GetWorldPosition(Ogre::Node& rNode)
		{
			return rNode._getDerivedPosition();
		}

		static inline const Ogre::Quaternion& _GetWorldOrientation(Ogre::Node& rNode)
		{
			return rNode._getDerivedOrientation();
		}

		static inline void _SetNormaliseNormals(Ogre::Entity& rEntity, bool bNormals)
		{
		#if OGRE_VERSION_MAJOR == 1 && OGRE_VERSION_MINOR < 7
			rEntity.setNormaliseNormals(bNormals);
		#endif
		}
	}// namespace Core
	
}// namespace UnE
