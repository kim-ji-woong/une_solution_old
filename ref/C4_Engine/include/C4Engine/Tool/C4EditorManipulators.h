//=============================================================
//
// C4 Engine version 2.9.1
// Copyright 1999-2012, by Terathon Software LLC
//
// This file is part of the C4 Engine and is provided under the
// terms of the license agreement entered by the registed user.
//
// Unauthorized redistribution of source code is strictly
// prohibited. Violators will be prosecuted.
//
//=============================================================


#ifndef C4EditorManipulators_h
#define C4EditorManipulators_h


#include "C4Manipulator.h"
#include "C4Skybox.h"
#include "C4Impostors.h"
#include "C4EditorBase.h"


namespace C4
{
	const float kSizeEpsilon = 1.0 / 1024.0F;
	
	
	const float kGraphBoxWidth = 112.0F;
	const float kGraphBoxHeight = 16.0F;
	
	
	enum
	{
		kWidgetManipulator					= 'MANP'
	};
	
	
	enum
	{
		kManipulatorHilited					= kManipulatorBaseState << 0,
		kManipulatorTempSelected			= kManipulatorBaseState << 1,
		kManipulatorTarget					= kManipulatorBaseState << 2,
		kManipulatorShowGizmo				= kManipulatorBaseState << 3,
		kManipulatorShowIcon				= kManipulatorBaseState << 4,
		kManipulatorConnectorSelected		= kManipulatorBaseState << 5,
		kManipulatorDeleted					= kManipulatorBaseState << 6,
		kManipulatorUpdated					= kManipulatorBaseState << 7,
		kManipulatorGraphValid				= kManipulatorBaseState << 8,
		kManipulatorForceRender				= kManipulatorBaseState << 9
	};
	
	
	enum
	{
		kManipulatorLockedTransform			= 1 << 0,
		kManipulatorLockedSubtree			= 1 << 1,
		kManipulatorLockedController		= 1 << 2,
		kManipulatorModifiablePlacement		= 1 << 3
	};
	
	
	enum
	{
		kMaxManipulatorHandleCount			= 20,
		kHandleOrigin						= -1
	};
	
	
	enum
	{
		kManipulatorHandleNegativeX			= 1 << 0,
		kManipulatorHandlePositiveX			= 1 << 1,
		kManipulatorHandleNegativeY			= 1 << 2,
		kManipulatorHandlePositiveY			= 1 << 3,
		kManipulatorHandleNegativeZ			= 1 << 4,
		kManipulatorHandlePositiveZ			= 1 << 5,
		kManipulatorHandleSecondary			= 1 << 6,
		kManipulatorHandleNonzeroX			= kManipulatorHandleNegativeX | kManipulatorHandlePositiveX,
		kManipulatorHandleNonzeroY			= kManipulatorHandleNegativeY | kManipulatorHandlePositiveY,
		kManipulatorHandleNonzeroZ			= kManipulatorHandleNegativeZ | kManipulatorHandlePositiveZ
	};
	
	
	enum
	{
		kManipulatorResizeCenter			= 1 << 0,
		kManipulatorResizeConstrain			= 1 << 1
	};
	
	
	enum
	{
		kEditorSelectionObject,
		kEditorSelectionSurface,
		kEditorSelectionFace,
		kEditorSelectionVertex
	};
	
	
	class Camera;
	class Skybox;
	class PhysicsNode;
	class EditorManipulator;
		
	
	class EditorGizmo
	{
		private:
			
			const EditorManipulator			*gizmoManipulator;
			
			Box3D							gizmoBox;
			Vector4D						gizmoScaleVector;
			 
			int32							hiliteEdgeIndex;
			 
			List<Attribute>					gizmoAttributeList; 
			TextureMapAttribute				gizmoTextureMap; 
			Renderable						gizmoRenderable;
			ColorRGB						gizmoColor[32]; 
			
			List<Attribute>					boxAttributeList;
			DiffuseAttribute				boxDiffuseColor;
			TextureMapAttribute				boxTextureMap; 
			Renderable						boxRenderable;
			Point3D							boxVertex[48];
			Vector4D						boxTangent[48];
			Point2D							boxTexcoord[48]; 
			
			Renderable						faceRenderable;
			Point3D							faceVertex[24];
			ColorRGB						faceColor[24];
			
			List<Attribute>					edgeAttributeList;
			DiffuseAttribute				edgeDiffuseColor;
			TextureMapAttribute				edgeTextureMap;
			Renderable						edgeRenderable;
			Point3D							edgeVertex[4];
			Vector4D						edgeTangent[4];
			Point2D							edgeTexcoord[4];
			
			Renderable						handleRenderable;
			Point3D							handleVertex[12];
			ColorRGB						handleColor[12];
			
			static const ConstPoint3D		gizmoVertex[32];
			static const ConstVector4D		gizmoTangent[32];
			static const ConstPoint2D		gizmoTexcoord[32];
			
			void UpdateColor(void);
			
			void RenderBox(const ManipulatorRenderData *renderData);
		
		public:
			
			EditorGizmo(const EditorManipulator *manipulator);
			~EditorGizmo();
			
			const Box3D& GetGizmoBox(void) const
			{
				return (gizmoBox);
			}
			
			const Transformable *GetTransformable(void) const
			{
				return (gizmoRenderable.GetTransformable());
			}
			
			void HiliteMovers(unsigned_int32 mask);
			void HiliteRotators(unsigned_int32 mask);
			
			int32 PickMover(const ManipulatorViewportData *viewportData, const Ray *ray) const;
			int32 PickRotator(const ManipulatorViewportData *viewportData, const Ray *ray) const;
			
			void HiliteFace(int32 face, float intensity = 1.0F);
			void HiliteEdge(int32 edge, float intensity = 1.0F);
			int32 PickFace(const Ray *ray, Point3D *point = nullptr) const;
			int32 PickEdge(const Ray *ray, Point3D *point = nullptr) const;
			
			void Render(const ManipulatorRenderData *renderData);
	};
	
	
	class EditorConnector
	{
		private:
			
			Connector						*connectorObject;
			const Node						*connectorNode;
			int32							connectorIndex;
			
			Widget							groupWidget;
			LineWidget						lineWidget1;
			LineWidget						lineWidget2;
			QuadWidget						backgroundWidget;
			BorderWidget					borderWidget;
			TextWidget						textWidget;
			
			List<Attribute>					lineAttributeList;
			DiffuseAttribute				lineDiffuseColor;
			TextureMapAttribute				lineTextureMap;
			Renderable						lineRenderable;
			Point3D							lineVertex[66];
			Vector4D						lineTangent[66];
			Point2D							lineTexcoord[66];
			
			Point3D GetConnectorPosition(const Transform4D& cameraTransform, float scale) const;
		
		public:
			
			EditorConnector(const EditorManipulator *manipulator, Connector *connector, int32 index);
			~EditorConnector();
			
			Node *GetConnectorTarget(void) const
			{
				return (connectorObject->GetConnectorTarget());
			}
			
			void Select(void);
			void Unselect(void);
			
			bool Pick(const ManipulatorViewportData *viewportData, const Ray *ray) const;
			
			void RenderBox(const ManipulatorViewportData *viewportData, List<Renderable> *renderList);
			void RenderLine(const ManipulatorViewportData *viewportData, List<Renderable> *renderList);
	};
	
	
	class ManipulatorWidget : public RenderableWidget
	{
		private:
			
			const EditorManipulator		*editorManipulator;
			float						viewportScale;
			
			List<Attribute>				attributeList;
			DiffuseAttribute			diffuseAttribute;
			
			Point3D						manipulatorVertex[28];
		
		public:
			
			ManipulatorWidget(EditorManipulator *manipulator);
			~ManipulatorWidget();
			
			void SetViewportScale(float scale)
			{
				viewportScale = scale;
				SetBuildFlag();
			}
			
			void Preprocess(void);
			void Build(void);
	};
	
	
	class EditorManipulator : public Manipulator, public ListElement<EditorManipulator>
	{
		private:
			
			unsigned_int32						manipulatorFlags;
			
			Editor								*worldEditor;
			EditorGizmo							*editorGizmo;
			
			int32								selectionType;
			Vector4D							manipulatorScaleVector;
			
			float								originalSize[kMaxObjectSizeCount];
			Point3D								originalPosition;
			
			BoundingSphere						nodeSphere;
			BoundingSphere						treeSphere;
			const BoundingSphere				*nodeSpherePointer;
			const BoundingSphere				*treeSpherePointer;
			
			List<Attribute>						iconAttributeList;
			TextureMapAttribute					iconTextureMap;
			Renderable							iconRenderable;
			
			List<Attribute>						markerAttributeList;
			DiffuseAttribute					markerDiffuseColor;
			TextureMapAttribute					markerTextureMap;
			Renderable							markerRenderable;
			
			int32								handleCount;
			Renderable							handleRenderable;
			Point3D								handleVertex[kMaxManipulatorHandleCount * 4];
			
			int32								connectorCount;
			int32								connectorSelection;
			char								*connectorStorage;
			EditorConnector						*editorConnector;
			List<Attribute>						connectorAttributeList;
			TextureMapAttribute					connectorTextureMap;
			Renderable							connectorRenderable;
			
			bool								graphValid;
			float								graphWidth;
			float								graphHeight;
			ImageWidget							graphBackground;
			ImageWidget							graphImage;
			TextWidget							graphText;
			ManipulatorWidget					graphBorder;
			GuiButtonWidget						graphCollapseButton;
			WidgetObserver<EditorManipulator>	graphCollapseObserver;
			
			static const ConstPoint3D markerVertex[4];
			static const ConstPoint2D markerTexcoord[4];
			static const ConstVector2D markerBillboard[4];
			static const ConstPoint2D iconTexcoord[4];
			static const ConstVector2D iconBillboard[4];
			
			void UpdateGraphColor(void);
			void HandleGraphCollapseEvent(Widget *widget, const WidgetEventData *eventData);
			
			C4EDITORAPI virtual int32 GetHandleTable(Point3D *handle) const;
		
		protected:
			
			enum
			{
				kManipulatorBoxVertexCount		= 64,
				kManipulatorBoxTriangleCount	= 144
			};
			
			C4EDITORAPI static const ConstPoint3D manipulatorBoxVertex[kManipulatorBoxVertexCount];
			C4EDITORAPI static const ConstPoint3D manipulatorCenterBoxVertex[kManipulatorBoxVertexCount];
			C4EDITORAPI static const ConstVector3D manipulatorBoxOffset[kManipulatorBoxVertexCount];
			C4EDITORAPI static const Triangle manipulatorBoxTriangle[kManipulatorBoxTriangleCount];
			
			void SetManipulatorFlags(unsigned_int32 flags)
			{
				manipulatorFlags = flags;
			}
			
			void SetSelectionType(int32 type)
			{
				selectionType = type;
			}
			
			void AllocateConnectorStorage(void);
			void ReleaseConnectorStorage(void);
			
			void HiliteSubtree(void);
			void UnhiliteSubtree(void);
			
			C4EDITORAPI virtual bool CalculateNodeSphere(BoundingSphere *sphere) const;
			
			C4EDITORAPI static bool PickLineSegment(const Ray *ray, const Point3D& p1, const Point3D& p2, float r2, float *param);
			C4EDITORAPI bool RegionPickLineSegment(const Region *region, const Point3D& p1, const Point3D& p2) const;
		
		public:
			
			static const ConstVector2D handleBillboard[kMaxManipulatorHandleCount * 4];
			
			C4EDITORAPI EditorManipulator(Node *node, const char *iconName);
			C4EDITORAPI virtual ~EditorManipulator();
			
			static Manipulator *Construct(Node *node, unsigned_int32 flags);
			
			unsigned_int32 GetManipulatorFlags(void) const
			{
				return (manipulatorFlags);
			}
			
			Editor *GetEditor(void) const
			{
				return (worldEditor);
			}
			
			EditorGizmo *GetGizmo(void) const
			{
				return (editorGizmo);
			}
			
			int32 GetSelectionType(void) const
			{
				return (selectionType);
			}
			
			const Vector4D& GetManipulatorScaleVector(void) const
			{
				return (manipulatorScaleVector);
			}
			
			const BoundingSphere *GetNodeSphere(void) const
			{
				return (nodeSpherePointer);
			}
			
			const BoundingSphere *GetTreeSphere(void) const
			{
				return (treeSpherePointer);
			}
			
			const float *GetOriginalSize(void) const
			{
				return (originalSize);
			}
			
			const Point3D& GetOriginalPosition(void) const
			{
				return (originalPosition);
			}
			
			const char *GetIconName(void) const
			{
				return (iconTextureMap.GetTextureName());
			}
			
			int32 GetHandleCount(void) const
			{
				return (handleCount);
			}
			
			const Point3D& GetHandlePosition(int32 index) const
			{
				return (handleVertex[index * 4]);
			}
			
			int32 GetConnectorCount(void) const
			{
				return (connectorCount);
			}
			
			Node *GetConnectorTarget(int32 index) const
			{
				return (editorConnector[index].GetConnectorTarget());
			}
			
			int32 GetConnectorSelection(void) const
			{
				return (connectorSelection);
			}
			
			Node *GetConnectorSelectionTarget(void) const
			{
				return (GetConnectorTarget(connectorSelection));
			}
			
			bool SetConnectorSelectionTarget(Node *node)
			{
				return (SetConnectorTarget(connectorSelection, node));
			}
			
			float GetGraphWidth(void) const
			{
				return (graphWidth);
			}
			
			float GetGraphHeight(void) const
			{
				return (graphHeight);
			}
			
			const Point3D& GetGraphPosition(void) const
			{
				return (graphBackground.GetWidgetPosition());
			}
			
			C4EDITORAPI void Pack(Packer& data, unsigned_int32 packFlags) const;
			C4EDITORAPI void Unpack(Unpacker& data, unsigned_int32 unpackFlags);
			bool UnpackChunk(const ChunkHeader *chunkHeader, Unpacker& data, unsigned_int32 unpackFlags);
			
			C4EDITORAPI virtual const char *GetDefaultNodeName(void) const;
			
			C4EDITORAPI void Preprocess(void);
			C4EDITORAPI void Invalidate(void);
			C4EDITORAPI void InvalidateGraph(void);
			C4EDITORAPI virtual void InvalidateNode(void);
			
			C4EDITORAPI void EnableGizmo(void);
			C4EDITORAPI void DisableGizmo(void);
			
			C4EDITORAPI virtual void Update(void);
			C4EDITORAPI void UpdateGraph(void);
			
			C4EDITORAPI virtual void Show(void);
			C4EDITORAPI virtual void Hide(void);
			
			C4EDITORAPI bool PredecessorSelected(void) const;
			
			C4EDITORAPI virtual void Select(void);
			C4EDITORAPI virtual void Unselect(void);
			
			C4EDITORAPI virtual void HandleDelete(bool undoable);
			C4EDITORAPI virtual void HandleUndelete(void);
			
			C4EDITORAPI virtual void HandleSizeUpdate(int32 count, const float *size);
			
			C4EDITORAPI virtual void HandleSettingsUpdate(void);
			C4EDITORAPI virtual void HandleConnectorUpdate(void);
			
			C4EDITORAPI virtual bool MaterialSettable(void) const;
			C4EDITORAPI virtual bool MaterialRemovable(void) const;
			C4EDITORAPI virtual const MaterialObject *PickupMaterial(void) const;
			C4EDITORAPI virtual void SetMaterial(MaterialObject *materialObject);
			C4EDITORAPI virtual void RemoveMaterial(void);
			
			C4EDITORAPI virtual Box3D CalculateNodeBoundingBox(void) const;
			C4EDITORAPI virtual Box3D CalculateWorldBoundingBox(void) const;
			
			C4EDITORAPI static void AdjustBoundingBox(Box3D *box);
			
			C4EDITORAPI virtual bool Pick(const Ray *ray, PickData *data) const;
			C4EDITORAPI virtual bool RegionPick(const Region *region) const;
			
			C4EDITORAPI virtual void GetHandleData(int32 index, ManipulatorHandleData *handleData) const;
			C4EDITORAPI virtual void BeginResize(const ManipulatorResizeData *resizeData);
			C4EDITORAPI virtual bool Resize(const ManipulatorResizeData *resizeData);
			
			C4EDITORAPI void UpdateConnectors(void);
			C4EDITORAPI void SelectConnector(int32 index, bool toggle = false);
			C4EDITORAPI void UnselectConnector(void);
			
			C4EDITORAPI bool SetConnectorTarget(int32 index, Node *target);
			bool PickConnector(const ManipulatorViewportData *viewportData, const Ray *ray, PickData *pickData) const;
			
			Box2D GetGraphBox(void) const;
			
			C4EDITORAPI void ExpandSubgraph(void);
			C4EDITORAPI void CollapseSubgraph(void);
			
			Node *PickGraphNode(const ManipulatorViewportData *viewportData, const Ray *ray, Widget **widget = nullptr);
			void SelectGraphNodes(float left, float right, float top, float bottom, unsigned_int32 state = 0);
			
			C4EDITORAPI virtual void Render(const ManipulatorRenderData *renderData);
			void RenderGraph(const ManipulatorViewportData *viewportData, List<Renderable> *renderList);
			
			C4EDITORAPI static void Install(Editor *editor, Node *root, bool recursive = true);
	};
	
	
	class GroupManipulator : public EditorManipulator
	{
		private:
			
			Vector4D				groupSizeVector;
			List<Attribute>			groupAttributeList;
			DiffuseAttribute		groupDiffuseColor;
			TextureMapAttribute		groupTextureMap;
			Renderable				groupRenderable;
			
			Point3D					groupVertex[48];
			
			bool CalculateNodeSphere(BoundingSphere *sphere) const;
		
		public:
			
			GroupManipulator(Node *node);
			~GroupManipulator();
			
			const char *GetDefaultNodeName(void) const;
			
			void Select(void);
			void Unselect(void);
			
			bool Pick(const Ray *ray, PickData *data) const;
			bool RegionPick(const Region *region) const;
			
			void Update(void);
			void Render(const ManipulatorRenderData *renderData);
	};
	
	
	class SkyboxManipulator : public EditorManipulator
	{
		public:
			
			SkyboxManipulator(Skybox *skybox);
			~SkyboxManipulator();
			
			Skybox *GetTargetNode(void) const
			{
				return (static_cast<Skybox *>(EditorManipulator::GetTargetNode()));
			}
			
			const char *GetDefaultNodeName(void) const;
			
			void Preprocess(void);
			
			void HandleDelete(bool undoable);
			void HandleUndelete(void);
			void HandleSettingsUpdate(void);
			
			bool MaterialSettable(void) const;
			bool MaterialRemovable(void) const;
			const MaterialObject *PickupMaterial(void) const;
			void SetMaterial(MaterialObject *materialObject);
			void RemoveMaterial(void);
	};
	
	
	class ImpostorManipulator : public EditorManipulator
	{
		public:
			
			ImpostorManipulator(Impostor *impostor);
			~ImpostorManipulator();
			
			Impostor *GetTargetNode(void) const
			{
				return (static_cast<Impostor *>(EditorManipulator::GetTargetNode()));
			}
			
			const char *GetDefaultNodeName(void) const;
			
			void Preprocess(void);
			
			bool MaterialSettable(void) const;
			const MaterialObject *PickupMaterial(void) const;
			void SetMaterial(MaterialObject *materialObject);
	};
}


#endif

// ZYURVUR
