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


#ifndef C4EditorUndo_h
#define C4EditorUndo_h


#include "C4EditorManipulators.h"
#include "C4Impostors.h"
#include "C4Particles.h"
#include "C4Terrain.h"
#include "C4Zones.h"
#include "C4Paint.h"
#include "C4Instances.h"


namespace C4
{
	enum UndoType
	{
		kUndoNone = -1,
		kUndoCreate,
		kUndoMove,
		kUndoSize,
		kUndoResize,
		kUndoPaste,
		kUndoDelete,
		kUndoGroup,
		kUndoConnect,
		kUndoReparent,
		kUndoZoneVertex,
		kUndoPortalVertex,
		kUndoMaterial,
		kUndoGeometry,
		kUndoTexture,
		kUndoPaint,
		kUndoPath,
		kUndoTubeEffect,
		kUndoReplaceWorld,
		kUndoAssociatePaintSpace,
		kUndoTerrainPaint,
		kUndoTerrainRebuild,
		kUndoWaterRebuild,
		kUndoNodeInfo
	};
	
	
	class Editor;
	
	
	class NodeReference : public ListElement<NodeReference>
	{
		private:
			
			Node	*reference;
		
		public:
			
			NodeReference(Node *node)
			{
				reference = node;
			}
			
			Node *GetNode(void) const
			{
				return (reference);
			}
	};
	
	
	class NodeTransformReference : public NodeReference
	{
		private:
			
			Transform4D		transform;
		
		public:
			
			NodeTransformReference(Node *node) : NodeReference(node)
			{
				transform = node->GetNodeTransform();
			}
			
			const Transform4D& GetTransform(void) const
			{
				return (transform);
			}
	};
	
	
	class UndoData : public ListElement<UndoData>
	{
		private:
			
			UndoType		undoType;
			bool			coupledFlag;
		
		protected:
			
			C4EDITORAPI UndoData(UndoType type);
		
		public:
			
			C4EDITORAPI virtual ~UndoData();
			 
			UndoType GetUndoType(void) const
			{ 
				return (undoType); 
			} 
			
			bool Coupled(void) const 
			{
				return (coupledFlag);
			}
			 
			void SetCoupledFlag(bool flag)
			{
				coupledFlag = flag;
			} 
			
			virtual void Undo(Editor *editor) = 0;
	};
	
	
	class CreateUndoData : public UndoData
	{
		private:
			
			List<NodeReference>		createdList;
		
		public:
			
			C4EDITORAPI CreateUndoData();
			C4EDITORAPI CreateUndoData(Node *node);
			C4EDITORAPI CreateUndoData(const List<NodeReference> *referenceList);
			C4EDITORAPI ~CreateUndoData();
			
			const List<NodeReference> *GetCreatedList(void) const
			{
				return (&createdList);
			}
			
			void AddNode(Node *node)
			{
				createdList.Append(new NodeReference(node));
			}
			
			void Undo(Editor *editor);
	};
	
	
	class MoveUndoData : public UndoData
	{
		private:
			
			class PathReference : public NodeTransformReference
			{
				private:
					
					Path		path;
				
				public:
					
					PathReference(PathMarker *marker);
					
					const Path *GetPath(void) const
					{
						return (&path);
					}
			};
			
			List<NodeReference>		movedList;
		
		public:
			
			C4EDITORAPI MoveUndoData(Node *node);
			C4EDITORAPI MoveUndoData(const List<NodeReference> *referenceList);
			C4EDITORAPI ~MoveUndoData();
			
			const List<NodeReference> *GetNodeList(void) const
			{
				return (&movedList);
			}
			
			void Undo(Editor *editor);
	};
	
	
	class SizeUndoData : public UndoData
	{
		private:
			
			Node	*sizeNode;
			int32	sizeCount;
			float	objectSize[kMaxObjectSizeCount];
		
		public:
			
			C4EDITORAPI SizeUndoData(Node *node);
			C4EDITORAPI ~SizeUndoData();
			
			void Undo(Editor *editor);
	};
	
	
	class ResizeUndoData : public UndoData
	{
		private:
			
			class ResizedReference : public ListElement<ResizedReference>
			{
				private:
					
					Node			*reference;
					Transform4D		transform;
					float			objectSize[kMaxObjectSizeCount];
				
				public:
					
					ResizedReference(Node *node);
					
					Node *GetNode(void) const
					{
						return (reference);
					}
					
					const Transform4D& GetTransform(void) const
					{
						return (transform);
					}
					
					const float *GetObjectSize(void) const
					{
						return (objectSize);
					}
			};
			
			class ResizedGeometryReference : public ResizedReference
			{
				private:
					
					GeometryLevel		*geometryLevel;
				
				public:
					
					ResizedGeometryReference(Geometry *geometry);
					~ResizedGeometryReference();
					
					const GeometryLevel *GetGeometryLevel(int32 level) const
					{
						return (&geometryLevel[level]);
					}
			};
			
			class ResizedMeshReference : public ResizedGeometryReference
			{
				private:
					
					BoundingSphere		boundingSphere;
					Box3D				boundingBox;
				
				public:
					
					ResizedMeshReference(MeshGeometry *mesh);
					
					const BoundingSphere *GetBoundingSphere(void) const
					{
						return (&boundingSphere);
					}
					
					const Box3D& GetBoundingBox(void) const
					{
						return (boundingBox);
					}
			};
			
			class ResizedPolygonZoneReference : public ResizedReference
			{
				private:
					
					Point3D		zoneVertex[kMaxZoneVertexCount];
				
				public:
					
					ResizedPolygonZoneReference(PolygonZone *polygon);
					
					const Point3D *GetVertexArray(void) const
					{
						return (zoneVertex);
					}
			};
			
			class ResizedPortalReference : public ResizedReference
			{
				private:
					
					Point3D		portalVertex[kMaxPortalVertexCount];
				
				public:
					
					ResizedPortalReference(Portal *portal);
					
					const Point3D *GetVertexArray(void) const
					{
						return (portalVertex);
					}
			};
			
			class AffectedReference : public ListElement<AffectedReference>
			{
				private:
					
					Node		*reference;
					Point3D		position;
				
				public:
					
					AffectedReference(Node *node);
					
					Node *GetNode(void) const
					{
						return (reference);
					}
					
					const Point3D& GetPosition(void) const
					{
						return (position);
					}
			};
			
			List<ResizedReference>		resizedList;
			List<AffectedReference>		affectedList;
		
		public:
			
			C4EDITORAPI ResizeUndoData(Node *node);
			C4EDITORAPI ResizeUndoData(const List<NodeReference> *referenceList);
			C4EDITORAPI ~ResizeUndoData();
			
			void AddNode(Node *node);
			
			void Undo(Editor *editor);
	};
	
	
	class PasteUndoData : public UndoData
	{
		private:
			
			List<NodeReference>		pastedList;
		
		public:
			
			C4EDITORAPI PasteUndoData(const List<NodeReference> *referenceList);
			C4EDITORAPI ~PasteUndoData();
			
			void Undo(Editor *editor);
	};
	
	
	class DeleteUndoData : public UndoData
	{
		private:
			
			struct OutgoingConnectorData
			{
				Connector		*outgoingConnector;
				Node			*targetNode;
				
				OutgoingConnectorData(Connector *connector)
				{
					outgoingConnector = connector;
					targetNode = connector->GetConnectorTarget();
				}
			};
			
			struct IncomingConnectorData
			{
				ConnectorKey	connectorKey;
				Node			*connectorNode;
				
				IncomingConnectorData(const Connector *connector)
				{
					connectorKey = connector->GetConnectorKey();
					connectorNode = connector->GetStartElement()->GetNode();
				}
			};
			
			class AffectedReference : public ListElement<AffectedReference>
			{
				private:
					
					Node							*reference;
					Node							*superNode;
					Transform4D						nodeTransform;
					
					bool							deletedFlag;
					
					Array<Link<Node> *>				linkArray;
					Array<OutgoingConnectorData>	outgoingConnectorArray;
					Array<IncomingConnectorData>	incomingConnectorArray;
				
				public:
					
					AffectedReference(Node *node, bool deleted);
					~AffectedReference();
					
					Node *GetNode(void) const
					{
						return (reference);
					}
					
					Node *GetSuperNode(void) const
					{
						return (superNode);
					}
					
					const Transform4D& GetNodeTransform(void) const
					{
						return (nodeTransform);
					}
					
					bool GetDeletedFlag(void) const
					{
						return (deletedFlag);
					}
					
					const Array<Link<Node> *>& GetLinkArray(void) const
					{
						return (linkArray);
					}
					
					const Array<OutgoingConnectorData>& GetOutgoingConnectorArray(void) const
					{
						return (outgoingConnectorArray);
					}
					
					const Array<IncomingConnectorData>& GetIncomingConnectorArray(void) const
					{
						return (incomingConnectorArray);
					}
			};
			
			List<AffectedReference>		affectedList;
		
		public:
			
			C4EDITORAPI DeleteUndoData(const List<NodeReference> *referenceList);
			C4EDITORAPI ~DeleteUndoData();
			
			void Undo(Editor *editor);
	};
	
	
	class GroupUndoData : public UndoData
	{
		private:
			
			List<NodeReference>		groupList;
		
		public:
			
			C4EDITORAPI GroupUndoData();
			C4EDITORAPI ~GroupUndoData();
			
			void AddGroup(Node *group)
			{
				groupList.Append(new NodeReference(group));
			}
			
			void Undo(Editor *editor);
	};
	
	
	class ConnectUndoData : public UndoData
	{
		private:
			
			struct ConnectorData
			{
				int32		connectorIndex;
				Node		*targetNode;
				
				ConnectorData(int32 index, Node *node)
				{
					connectorIndex = index;
					targetNode = node;
				}
			};
			
			class ConnectedReference : public ListElement<ConnectedReference>
			{
				private:
					
					Node					*reference;
					Array<ConnectorData>	connectorArray;
				
				public:
					
					ConnectedReference(Node *node);
					~ConnectedReference();
					
					Node *GetNode(void) const
					{
						return (reference);
					}
					
					const Array<ConnectorData>& GetConnectorArray(void) const
					{
						return (connectorArray);
					}
			};
			
			List<ConnectedReference>	connectedList;
		
		public:
			
			C4EDITORAPI ConnectUndoData(const List<EditorManipulator> *manipulatorList);
			C4EDITORAPI ConnectUndoData(const List<NodeReference> *referenceList);
			C4EDITORAPI ~ConnectUndoData();
			
			void Undo(Editor *editor);
	};
	
	
	class ReparentUndoData : public UndoData
	{
		private:
			
			class MovedReference : public ListElement<MovedReference>
			{
				private:
					
					Node			*reference;
					Node			*superNode;
					Zone			*owningZone;
					Transform4D		transform;
				
				public:
					
					MovedReference(Node *node);
					
					Node *GetNode(void) const
					{
						return (reference);
					}
					
					Node *GetSuperNode(void) const
					{
						return (superNode);
					}
					
					Zone *GetOwningZone(void) const
					{
						return (owningZone);
					}
					
					const Transform4D& GetTransform(void) const
					{
						return (transform);
					}
			};
			
			List<MovedReference>	movedList;
		
		public:
			
			C4EDITORAPI ReparentUndoData();
			C4EDITORAPI ReparentUndoData(const List<NodeReference> *referenceList);
			C4EDITORAPI ~ReparentUndoData();
			
			void AddNode(Node *node);
			
			void Undo(Editor *editor);
	};
	
	
	class ZoneVertexUndoData : public UndoData
	{
		private:
			
			PolygonZone		*zoneNode;
			
			int32			zoneVertexCount;
			Point3D			zoneVertex[kMaxZoneVertexCount];
		
		public:
			
			C4EDITORAPI ZoneVertexUndoData(PolygonZone *polygon);
			C4EDITORAPI ~ZoneVertexUndoData();
			
			void Undo(Editor *editor);
	};
	
	
	class PortalVertexUndoData : public UndoData
	{
		private:
			
			Portal		*portalNode;
			
			int32		portalVertexCount;
			Point3D		portalVertex[kMaxPortalVertexCount];
		
		public:
			
			C4EDITORAPI PortalVertexUndoData(Portal *portal);
			C4EDITORAPI ~PortalVertexUndoData();
			
			void Undo(Editor *editor);
	};
	
	
	class MaterialUndoData : public UndoData
	{
		private:
			
			class GeometryReference : public ListElement<GeometryReference>
			{
				private:
					
					Geometry			*reference;
					
					int32				materialCount;
					char				*materialStorage;
					MaterialObject		**materialObject;
					unsigned_int32		*materialIndex;
				
				public:
					
					GeometryReference(Geometry *geometry);
					~GeometryReference();
					
					Geometry *GetGeometry(void) const
					{
						return (reference);
					}
					
					int32 GetMaterialCount(void) const
					{
						return (materialCount);
					}
					
					MaterialObject *GetMaterialObject(unsigned_int32 index) const
					{
						return (materialObject[index]);
					}
					
					unsigned_int32 GetMaterialIndex(unsigned_int32 surface) const
					{
						return (materialIndex[surface]);
					}
			};
			
			class SkyboxReference : public ListElement<SkyboxReference>
			{
				private:
					
					Skybox			*reference;
					MaterialObject	*materialObject;
				
				public:
					
					SkyboxReference(Skybox *skybox);
					~SkyboxReference();
					
					Skybox *GetSkybox(void) const
					{
						return (reference);
					}
					
					MaterialObject *GetMaterialObject(void) const
					{
						return (materialObject);
					}
			};
			
			class ImpostorReference : public ListElement<ImpostorReference>
			{
				private:
					
					Impostor		*reference;
					MaterialObject	*materialObject;
				
				public:
					
					ImpostorReference(Impostor *impostor);
					~ImpostorReference();
					
					Impostor *GetImpostor(void) const
					{
						return (reference);
					}
					
					MaterialObject *GetMaterialObject(void) const
					{
						return (materialObject);
					}
			};
			
			class ParticleSystemReference : public ListElement<ParticleSystemReference>
			{
				private:
					
					ParticleSystem	*reference;
					MaterialObject	*materialObject;
				
				public:
					
					ParticleSystemReference(ParticleSystem *particleSystem);
					~ParticleSystemReference();
					
					ParticleSystem *GetParticleSystem(void) const
					{
						return (reference);
					}
					
					MaterialObject *GetMaterialObject(void) const
					{
						return (materialObject);
					}
			};
			
			class ReplaceMaterialModifierReference : public ListElement<ReplaceMaterialModifierReference>
			{
				private:
					
					Instance					*instance;
					ReplaceMaterialModifier		*reference;
					MaterialObject				*materialObject;
				
				public:
					
					ReplaceMaterialModifierReference(Instance *node, ReplaceMaterialModifier *replaceMaterialModifier);
					~ReplaceMaterialModifierReference();
					
					Instance *GetInstance(void) const
					{
						return (instance);
					}
					
					ReplaceMaterialModifier *GetReplaceMaterialModifier(void) const
					{
						return (reference);
					}
					
					MaterialObject *GetMaterialObject(void) const
					{
						return (materialObject);
					}
			};
			
			List<GeometryReference>					geometryList;
			List<SkyboxReference>					skyboxList;
			List<ImpostorReference>					impostorList;
			List<ParticleSystemReference>			particleSystemList;
			List<ReplaceMaterialModifierReference>	replaceMaterialModifierList;
		
		public:
			
			C4EDITORAPI MaterialUndoData(const List<NodeReference> *referenceList);
			C4EDITORAPI ~MaterialUndoData();
			
			void Undo(Editor *editor);
	};
	
	
	class GeometryUndoData : public UndoData
	{
		private:
			
			class GeometryReference : public ListElement<GeometryReference>
			{
				private:
					
					Geometry		*reference;
					Transform4D		transform;
					unsigned_int32	primitiveFlags;
					
					int32			geometryLevelCount;
					int32			collisionLevel;
					GeometryLevel	*geometryLevel;
				
				public:
					
					GeometryReference(Geometry *geometry);
					~GeometryReference();
					
					Geometry *GetGeometry(void) const
					{
						return (reference);
					}
					
					const Transform4D& GetTransform(void) const
					{
						return (transform);
					}
					
					unsigned_int32 GetPrimitiveFlags(void) const
					{
						return (primitiveFlags);
					}
					
					int32 GetGeometryLevelCount(void) const
					{
						return (geometryLevelCount);
					}
					
					int32 GetCollisionLevel(void) const
					{
						return (collisionLevel);
					}
					
					const GeometryLevel *GetGeometryLevel(int32 level) const
					{
						return (&geometryLevel[level]);
					}
			};
			
			class TerrainReference : public GeometryReference
			{
				private:
					
					TerrainBorderRenderData		borderRenderData;
				
				public:
					
					TerrainReference(TerrainGeometry *terrain);
					
					const TerrainBorderRenderData *GetBorderRenderData(void) const
					{
						return (&borderRenderData);
					}
			};
			
			class MovedReference : public ListElement<MovedReference>
			{
				private:
					
					Node			*reference;
					Transform4D		transform;
				
				public:
					
					MovedReference(Node *node);
					
					Node *GetNode(void) const
					{
						return (reference);
					}
					
					const Transform4D& GetTransform(void) const
					{
						return (transform);
					}
			};
			
			List<GeometryReference>		geometryList;
			List<MovedReference>		movedList;
			
			void AddGeometry(Geometry *geometry);
		
		public:
			
			C4EDITORAPI GeometryUndoData(Geometry *geometry);
			C4EDITORAPI GeometryUndoData(const List<NodeReference> *referenceList, GeometryType filter = 0);
			C4EDITORAPI ~GeometryUndoData();
			
			void Undo(Editor *editor);
	};
	
	
	class TextureUndoData : public UndoData
	{
		private:
			
			class GeometryReference : public ListElement<GeometryReference>
			{
				private:
					
					Geometry			*reference;
					
					char				*textureStorage;
					Point2D				*texcoordArray;
					TextureAlignData	*textureAlignData;
				
				public:
					
					GeometryReference(Geometry *geometry);
					~GeometryReference();
					
					Geometry *GetGeometry(void) const
					{
						return (reference);
					}
					
					const Point2D *GetTexcoordArray(void) const
					{
						return (texcoordArray);
					}
					
					const TextureAlignData *GetTextureAlignData(void) const
					{
						return (textureAlignData);
					}
			};
			
			List<GeometryReference>		geometryList;
		
		public:
			
			C4EDITORAPI TextureUndoData(Geometry *geometry);
			C4EDITORAPI TextureUndoData(const List<NodeReference> *referenceList);
			C4EDITORAPI ~TextureUndoData();
			
			void Undo(Editor *editor);
	};
	
	
	class PaintUndoData : public UndoData
	{
		private:
			
			const PaintSpaceObject		*paintSpaceObject;
			
			Rect						paintBounds;
			const void					*undoImage;
		
		public:
			
			C4EDITORAPI PaintUndoData(const PaintSpaceObject *object, const Painter *painter);
			C4EDITORAPI ~PaintUndoData();
			
			void Undo(Editor *editor);
	};
	
	
	class PathUndoData : public UndoData
	{
		private:
			
			class PathReference : public ListElement<PathReference>
			{
				private:
					
					PathMarker		*reference;
					Path			path;
				
				public:
					
					PathReference(PathMarker *marker);
					~PathReference();
					
					PathMarker *GetPathMarker(void) const
					{
						return (reference);
					}
					
					const Path *GetPath(void) const
					{
						return (&path);
					}
			};
			
			List<PathReference>		pathList;
		
		public:
			
			C4EDITORAPI PathUndoData(PathMarker *marker);
			C4EDITORAPI PathUndoData(const List<NodeReference> *referenceList);
			C4EDITORAPI ~PathUndoData();
			
			void Undo(Editor *editor);
	};
	
	
	class TubeEffectUndoData : public UndoData
	{
		private:
			
			class TubeReference : public ListElement<TubeReference>
			{
				private:
					
					TubeEffect		*reference;
					Path			path;
				
				public:
					
					TubeReference(TubeEffect *tube);
					~TubeReference();
					
					TubeEffect *GetTubeEffect(void) const
					{
						return (reference);
					}
					
					const Path *GetPath(void) const
					{
						return (&path);
					}
			};
			
			List<TubeReference>		tubeList;
		
		public:
			
			C4EDITORAPI TubeEffectUndoData(const List<NodeReference> *referenceList);
			C4EDITORAPI ~TubeEffectUndoData();
			
			void Undo(Editor *editor);
	};
	
	
	class ReplaceWorldUndoData : public UndoData
	{
		private:
			
			class WorldReference : public ListElement<WorldReference>
			{
				private:
					
					Instance			*reference;
					ResourceName		worldName;
				
				public:
					
					WorldReference(Instance *instance);
					~WorldReference();
					
					Instance *GetInstance(void) const
					{
						return (reference);
					}
					
					const ResourceName& GetWorldName(void) const
					{
						return (worldName);
					}
			};
			
			List<WorldReference>	worldList;
		
		public:
			
			C4EDITORAPI ReplaceWorldUndoData(const List<NodeReference> *referenceList);
			C4EDITORAPI ~ReplaceWorldUndoData();
			
			void Undo(Editor *editor);
	};
	
	
	class AssociatePaintSpaceUndoData : public UndoData
	{
		private:
			
			class GeometryReference : public ListElement<GeometryReference>
			{
				private:
					
					Geometry		*reference;
					PaintSpace		*paintSpace;
				
				public:
					
					GeometryReference(Geometry *geometry);
					~GeometryReference();
					
					Geometry *GetGeometry(void) const
					{
						return (reference);
					}
					
					PaintSpace *GetPaintSpace(void) const
					{
						return (paintSpace);
					}
			};
			
			List<GeometryReference>		geometryList;
		
		public:
			
			C4EDITORAPI AssociatePaintSpaceUndoData(const List<NodeReference> *referenceList);
			C4EDITORAPI ~AssociatePaintSpaceUndoData();
			
			void Undo(Editor *editor);
	};
	
	
	class NodeInfoUndoData : public UndoData
	{
		private:
			
			struct ConnectorData
			{
				ConnectorKey	connectorKey;
				Node			*targetNode;
				
				ConnectorData(Connector *connector)
				{
					connectorKey = connector->GetConnectorKey();
					targetNode = connector->GetConnectorTarget();
				}
			};
			
			class NodeInfoReference : public ListElement<NodeInfoReference>
			{
				private:
					
					Node					*reference;
					List<NodeReference>		propertyObjectList;
					
					Package					nodePackage;
					Package					objectPackage;
					Package					propertyPackage;
					
					Array<ConnectorData>	connectorArray;
				
				public:
					
					NodeInfoReference(Node *node);
					~NodeInfoReference();
					
					Node *GetNode(void) const
					{
						return (reference);
					}
					
					const NodeReference *GetFirstPropertyObjectNode(void) const
					{
						return (propertyObjectList.First());
					}
					
					const Package *GetNodePackage(void) const
					{
						return (&nodePackage);
					}
					
					const Package *GetObjectPackage(void) const
					{
						return (&objectPackage);
					}
					
					const Package *GetPropertyPackage(void) const
					{
						return (&propertyPackage);
					}
					
					const Array<ConnectorData>& GetConnectorArray(void) const
					{
						return (connectorArray);
					}
			};
			
			List<NodeInfoReference>		nodeList;
		
		public:
			
			C4EDITORAPI NodeInfoUndoData(const List<NodeReference> *referenceList);
			C4EDITORAPI ~NodeInfoUndoData();
			
			void Undo(Editor *editor);
	};
}


#endif

// ZYURVUR
