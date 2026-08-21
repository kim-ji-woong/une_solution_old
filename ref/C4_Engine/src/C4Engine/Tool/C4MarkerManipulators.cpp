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


#include "C4MarkerManipulators.h"
#include "C4WorldEditor.h"


using namespace C4;


namespace
{
	const float kPathRenderRadius = 0.015625F;
	const float kPathPointMinRenderSize = 0.015625F;
	
	
	const ConstColorRGBA kPathRenderColor = {0.25F, 1.0F, 0.25F, 1.0F};
	const ConstColorRGBA kTangentRenderColor = {0.75F, 0.75F, 0.75F, 1.0F};
	const ConstColorRGBA kPointRenderColor = {0.5F, 0.5F, 0.5F, 1.0F};
	
	
	const TextureHeader pathTextureHeader =
	{
		kTexture2D,
		kTextureForceHighQuality,
		kTextureSemanticEmission,
		kTextureSemanticTransparency,
		kTextureI8,
		8, 4, 1,
		{kTextureClamp, kTextureRepeat, kTextureRepeat},
		4
	};
	
	
	const unsigned_int8 pathTextureImage[43] =
	{
		0x00, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0x00,
		0x00, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0x00,
		0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
		0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
		0x80, 0xFF, 0xFF, 0x80,
		0x00, 0x00, 0x00, 0x00,
		0x80, 0x80,
		0x80
	};
}


MarkerManipulator::MarkerManipulator(Marker *marker, const char *iconName) : EditorManipulator(marker, iconName)
{
}

MarkerManipulator::~MarkerManipulator()
{
}

Manipulator *MarkerManipulator::Construct(Marker *marker)
{
	switch (marker->GetMarkerType())
	{
		case kMarkerLocator:
			
			return (new LocatorMarkerManipulator(static_cast<LocatorMarker *>(marker)));
		
		case kMarkerConnection:
			
			return (new MarkerManipulator(marker, "WorldEditor/marker/Connection"));
		
		case kMarkerCube:
			
			return (new MarkerManipulator(marker, "WorldEditor/marker/Cube"));
		
		case kMarkerPath:
			
			return (new PathManipulator(static_cast<PathMarker *>(marker)));
	}
	
	return (nullptr);
}

const char *MarkerManipulator::GetDefaultNodeName(void) const
{
	return (TheWorldEditor->GetStringTable()->GetString(StringID('NAME', kNodeMarker, GetTargetNode()->GetMarkerType())));
}

void MarkerManipulator::Preprocess(void)
{
	SetManipulatorState(GetManipulatorState() | kManipulatorShowIcon);
	EditorManipulator::Preprocess();
}


LocatorMarkerManipulator::LocatorMarkerManipulator(LocatorMarker *marker) : MarkerManipulator(marker, "WorldEditor/marker/Locator")
{
	SetManipulatorFlags(kManipulatorModifiablePlacement);
}

LocatorMarkerManipulator::~LocatorMarkerManipulator()
{
}

const char *LocatorMarkerManipulator::GetDefaultNodeName(void) const
{
	const LocatorRegistration *registration = LocatorMarker::FindRegistration(GetTargetNode()->GetLocatorType());
	if (registration) 
	{
		const char *name = registration->GetLocatorName(); 
		if (name) return (name); 
	} 
	
	return (MarkerManipulator::GetDefaultNodeName()); 
}


PathManipulator::PathManipulator(PathMarker *path) : 
		EditorManipulator(path, "WorldEditor/path/Path"),
		pathDiffuseColor(kPathRenderColor, kAttributeMutable),
		pathTextureMap(&pathTextureHeader, pathTextureImage),
		tangentTextureMap(&pathTextureHeader, pathTextureImage), 
		pathRenderable(kRenderTriangleStrip, kRenderDepthTest | kRenderDepthInhibit),
		tangentRenderable(kRenderQuads),
		pointRenderable(kRenderQuads)
{
	pathStorage = nullptr;
	pointSelectionArray = nullptr;
	maxSelectedPointCount = 0;
	
	pathAttributeList.Append(&pathDiffuseColor);
	pathAttributeList.Append(&pathTextureMap);
	pathRenderable.SetMaterialAttributeList(&pathAttributeList);
	pathRenderable.SetAmbientBlendState(BlendState(kBlendOne, kBlendInvSourceAlpha));
	pathRenderable.SetShaderFlags(kShaderAmbientEffect | kShaderVertexPolyboard | kShaderOrthoPolyboard | kShaderScaleVertex);
	pathRenderable.SetRenderParameterPointer(&pathSizeVector);
	pathRenderable.SetTransformable(path);
	
	tangentAttributeList.Append(&tangentTextureMap);
	tangentRenderable.SetMaterialAttributeList(&tangentAttributeList);
	tangentRenderable.SetAmbientBlendState(BlendState(kBlendOne, kBlendInvSourceAlpha));
	tangentRenderable.SetShaderFlags(kShaderAmbientEffect | kShaderVertexPolyboard | kShaderLinearPolyboard | kShaderOrthoPolyboard | kShaderScaleVertex);
	tangentRenderable.SetRenderParameterPointer(&pathSizeVector);
	tangentRenderable.SetTransformable(path);
	
	pointRenderable.SetAmbientBlendState(BlendState(kBlendOne, kBlendInvSourceAlpha));
	pointRenderable.SetShaderFlags(kShaderAmbientEffect | kShaderVertexBillboard | kShaderScaleVertex);
	pointRenderable.SetRenderParameterPointer(&pointSizeVector);
	pointRenderable.SetTransformable(path);
}

PathManipulator::~PathManipulator()
{
	delete[] pointSelectionArray;
	delete[] pathStorage;
}

const char *PathManipulator::GetDefaultNodeName(void) const
{
	return (TheWorldEditor->GetStringTable()->GetString(StringID('NAME', kMarkerPath)));
}

bool PathManipulator::CalculateNodeSphere(BoundingSphere *sphere) const
{
	Box3D	bounds;
	
	GetTargetNode()->GetPath()->GetBoundingBox(&bounds);
	
	sphere->SetCenter((bounds.min + bounds.max) * 0.5F);
	sphere->SetRadius(Magnitude(bounds.max - bounds.min) * 0.5F);
	return (true);
}

void PathManipulator::Select(void)
{
	EditorManipulator::Select();
	Invalidate();
	
	pathDiffuseColor.SetDiffuseColor(K::white);
}

void PathManipulator::Unselect(void)
{
	EditorManipulator::Unselect();
	
	delete[] pointSelectionArray;
	pointSelectionArray = nullptr;
	maxSelectedPointCount = 0;
	
	pathDiffuseColor.SetDiffuseColor(kPathRenderColor);
}

const PathComponent *PathManipulator::GetControlPointComponent(const Path *path, int32 *index)
{
	int32 i = *index;
	
	const PathComponent *component = path->GetFirstPathComponent();
	while (component)
	{
		int32 count = component->GetControlPointCount();
		if (i < count)
		{
			*index = i;
			return (component);
		}
		
		i -= count;
		component = component->Next();
	}
	
	return (nullptr);
}

void PathManipulator::SelectControlPoint(int32 index, bool selectTangent)
{
	if (index >= 0)
	{
		const Path *path = GetTargetNode()->GetPath();
		
		if (!pointSelectionArray)
		{
			int32 pointCount = 0;
			const PathComponent *component = path->GetFirstPathComponent();
			while (component)
			{
				pointCount += component->GetControlPointCount();
				component = component->Next();
			}
			
			maxSelectedPointCount = pointCount;
			pointSelectionArray = new float[pointCount];
			for (machine a = 0; a < pointCount; a++) pointSelectionArray[a] = 0.0F;
		}
		
		pointSelectionArray[index] = 1.0F;
		
		if (selectTangent)
		{
			int32 pointIndex = index;
			const PathComponent *component = GetControlPointComponent(path, &pointIndex);
			if (component)
			{
				PathType type = component->GetPathType();
				if (type == kPathElliptical)
				{
					if (pointIndex == 0) pointSelectionArray[index + 2] = 1.0F;
				}
				else if (type == kPathBezier)
				{
					if (pointIndex == 0) pointSelectionArray[index + 1] = 1.0F;
					else if (pointIndex == 3) pointSelectionArray[index - 1] = 1.0F;
				}
			}
		}
		
		UpdateControlPointSelection();
	}
}

void PathManipulator::UnselectControlPoint(int32 index, bool unselectTangent)
{
	if (index >= 0)
	{
		pointSelectionArray[index] = 0.0F;
		
		if (unselectTangent)
		{
			int32 pointIndex = index;
			const PathComponent *component = GetControlPointComponent(GetTargetNode()->GetPath(), &pointIndex);
			if (component)
			{
				PathType type = component->GetPathType();
				if (type == kPathElliptical)
				{
					if (pointIndex == 0) pointSelectionArray[index + 2] = 0.0F;
				}
				else if (type == kPathBezier)
				{
					if (pointIndex == 0) pointSelectionArray[index + 1] = 0.0F;
					else if (pointIndex == 3) pointSelectionArray[index - 1] = 0.0F;
				}
			}
		}
		
		UpdateControlPointSelection();
	}
}

void PathManipulator::UpdateControlPointSelection(void)
{
	GetEditor()->InvalidateNode(GetTargetNode());
	
	int32 selectedPointCount = 0;
	int32 pointCount = maxSelectedPointCount;
	for (machine a = 0; a < pointCount; a++) selectedPointCount += (pointSelectionArray[a] != 0.0F);
	
	SetSelectionType((selectedPointCount != 0) ? kEditorSelectionVertex : kEditorSelectionObject);
}

void PathManipulator::MoveSelectedControlPoints(const Vector3D& delta, bool maintainTangents)
{
	const float *selection = pointSelectionArray;
	
	PathComponent *component = GetTargetNode()->GetPath()->GetFirstPathComponent();
	while (component)
	{
		PathType type = component->GetPathType();
		if (type == kPathLinear)
		{
			LinearPathComponent *linearComponent = static_cast<LinearPathComponent *>(component);
			for (machine a = 0; a < 2; a++)
			{
				float strength = selection[a];
				if ((strength > 0.0F) || (maintainTangents)) linearComponent->SetControlPoint(a, linearComponent->GetControlPoint(a) + delta * strength);
			}
			
			selection += 2;
		}
		else if (type == kPathElliptical)
		{
			EllipticalPathComponent *ellipticalComponent = static_cast<EllipticalPathComponent *>(component);
			for (machine a = 0; a < 3; a++)
			{
				float strength = selection[a];
				if ((strength > 0.0F) || (maintainTangents)) ellipticalComponent->SetControlPoint(a, ellipticalComponent->GetControlPoint(a) + delta * strength);
			}
			
			selection += 3;
		}
		else if (type == kPathBezier)
		{
			BezierPathComponent *bezierComponent = static_cast<BezierPathComponent *>(component);
			for (machine a = 0; a < 4; a++)
			{
				float strength = selection[a];
				if (strength > 0.0F) bezierComponent->SetControlPoint(a, bezierComponent->GetControlPoint(a) + delta * strength);
				
				if (maintainTangents)
				{
					if (a == 1)
					{
						PathComponent *prev = component->Previous();
						if ((prev) && (prev->GetPathType() == kPathBezier) && (selection[a - 2] == 0.0F) && (selection[a - 3] == 0.0F))
						{
							BezierPathComponent *prevBezier = static_cast<BezierPathComponent *>(prev);
							prevBezier->SetControlPoint(2, prevBezier->GetControlPoint(2) - delta * strength);
						}
					}
					else if (a == 2)
					{
						PathComponent *next = component->Next();
						if ((next) && (next->GetPathType() == kPathBezier) && (selection[a + 2] == 0.0F) && (selection[a + 3] == 0.0F))
						{
							BezierPathComponent *nextBezier = static_cast<BezierPathComponent *>(next);
							nextBezier->SetControlPoint(1, nextBezier->GetControlPoint(1) - delta * strength);
						}
					}
				}
			}
			
			selection += 4;
		}
		
		component = component->Next();
	}
	
	GetEditor()->InvalidateNode(GetTargetNode());
}

bool PathManipulator::PickControlPoint(const Ray *ray, PickData *data) const
{
	float r = ray->radius;
	if (r == 0.0F) r = kPathRenderRadius * 4.0F;
	else r = r * 6.0F;
	
	const Path *path = GetTargetNode()->GetPath();
	
	int32 pointIndex = 0;
	const PathComponent *component = path->GetFirstPathComponent();
	while (component)
	{
		const PathComponent *nextComponent = component->Next();
		
		PathType type = component->GetPathType();
		if (type == kPathLinear)
		{
			const LinearPathComponent *linearComponent = static_cast<const LinearPathComponent *>(component);
			for (machine a = 0; a < 2; a++)
			{
				float	t1, t2;
				
				if (Math::IntersectRayAndSphere(ray, linearComponent->GetControlPoint(a), r, &t1, &t2))
				{
					if ((t1 > ray->tmin) && (t1 < ray->tmax))
					{
						data->rayParam = t1;
						data->pickIndex[0] = pointIndex;
						data->pickIndex[1] = ((a == 1) && (nextComponent)) ? pointIndex + 1 : -1;
						return (true);
					}
				}
				
				pointIndex++;
			}
		}
		else if (type == kPathElliptical)
		{
			const EllipticalPathComponent *ellipticalComponent = static_cast<const EllipticalPathComponent *>(component);
			for (machine a = 0; a < 3; a++)
			{
				float	t1, t2;
				
				if (Math::IntersectRayAndSphere(ray, ellipticalComponent->GetControlPoint(a), r, &t1, &t2))
				{
					if ((t1 > ray->tmin) && (t1 < ray->tmax))
					{
						data->rayParam = t1;
						data->pickIndex[0] = pointIndex;
						data->pickIndex[1] = ((a == 1) && (nextComponent)) ? pointIndex + 2 : -1;
						return (true);
					}
				}
				
				pointIndex++;
			}
		}
		else if (type == kPathBezier)
		{
			const BezierPathComponent *bezierComponent = static_cast<const BezierPathComponent *>(component);
			for (machine a = 0; a < 4; a++)
			{
				float	t1, t2;
				
				if (Math::IntersectRayAndSphere(ray, bezierComponent->GetControlPoint(a), r, &t1, &t2))
				{
					if ((t1 > ray->tmin) && (t1 < ray->tmax))
					{
						data->rayParam = t1;
						data->pickIndex[0] = pointIndex;
						data->pickIndex[1] = ((a == 3) && (nextComponent)) ? pointIndex + 1 : -1;
						return (true);
					}
				}
				
				pointIndex++;
			}
		}
		
		component = nextComponent;
	}
	
	return (false);
}

bool PathManipulator::Pick(const Ray *ray, PickData *data) const
{
	if ((Selected()) && (PickControlPoint(ray, data))) return (true);
	
	float r = ray->radius;
	if (r == 0.0F) r = kPathRenderRadius * 4.0F;
	float r2 = r * r;
	
	const Path *path = GetTargetNode()->GetPath();
	const PathComponent *component = path->GetFirstPathComponent();
	while (component)
	{
		Box3D	bounds;
		float	u1, u2;
		
		component->GetBoundingBox(&bounds);
		
		if (Math::IntersectRayAndSphere(ray, (bounds.min + bounds.max) * 0.5F, Magnitude(bounds.max - bounds.min) * 0.5F, &u1, &u2))
		{
			PathType type = component->GetPathType();
			if (type == kPathLinear)
			{
				const LinearPathComponent *linearComponent = static_cast<const LinearPathComponent *>(component);
				const Point3D& p1 = linearComponent->GetControlPoint(0);
				const Point3D& p2 = linearComponent->GetControlPoint(1);
				
				if (PickLineSegment(ray, p1, p2, r2, &data->rayParam))
				{
					data->pickIndex[0] = -1;
					return (true);
				}
			}
			else
			{
				Point3D p1 = component->GetPosition(0.0F);
				Vector3D t1 = component->GetTangent(0.0F);
				
				for (machine a = 1; a <= 32; a++)
				{
					float t = (float) a * 0.03125F;
					Point3D p2 = component->GetPosition(t);
					Vector3D t2 = component->GetTangent(t);
					Vector3D dp = p2 - p1;
					
					if (Math::CalculateNearestParameters(ray->origin, ray->direction, p1, dp, &u1, &u2))
					{
						if ((u1 > ray->tmin) && (u1 < ray->tmax))
						{
							Point3D q = ray->origin + ray->direction * u1;
							if (SquaredMag(q - p1 - dp * u2) < r2)
							{
								if (((Antivector4D(t1, p1) ^ q) > 0.0F) && ((Antivector4D(t2, p2) ^ q) < 0.0F))
								{
									data->rayParam = u1;
									data->pickIndex[0] = -1;
									return (true);
								}
							}
						}
					}
					
					p1 = p2;
					t1 = t2;
				}
			}
		}
		
		component = component->Next();
	}
	
	return (false);
}

bool PathManipulator::RegionPick(const Region *region) const
{
	const Transform4D& worldTransform = GetTargetNode()->GetWorldTransform();
	
	const Path *path = GetTargetNode()->GetPath();
	const PathComponent *component = path->GetFirstPathComponent();
	while (component)
	{
		Box3D		bounds;
		Vector3D	axis[3];
		
		component->GetBoundingBox(&bounds);
		
		Point3D center = worldTransform * ((bounds.min + bounds.max) * 0.5F);
		axis[0] = worldTransform[0] * ((bounds.max.x - bounds.min.x) * 0.5F);
		axis[1] = worldTransform[1] * ((bounds.max.y - bounds.min.y) * 0.5F);
		axis[2] = worldTransform[2] * ((bounds.max.z - bounds.min.z) * 0.5F);
		
		if (region->BoxVisible(center, axis))
		{
			PathType type = component->GetPathType();
			if (type == kPathLinear)
			{
				const LinearPathComponent *linearComponent = static_cast<const LinearPathComponent *>(component);
				const Point3D& p1 = linearComponent->GetControlPoint(0);
				const Point3D& p2 = linearComponent->GetControlPoint(1);
				
				if (region->CylinderVisible(worldTransform * p1, worldTransform * p2, 0.0F)) return (true);
			}
			else
			{
				Point3D p1 = worldTransform * component->GetPosition(0.0F);
				
				for (machine a = 1; a <= 32; a++)
				{
					Point3D p2 = worldTransform * component->GetPosition((float) a * 0.03125F);
					if (region->CylinderVisible(p1, p2, 0.0F)) return (true);
					p1 = p2;
				}
			}
		}
		
		component = component->Next();
	}
	
	return (false);
}

void PathManipulator::Update(void)
{
	if (!(GetManipulatorState() & kManipulatorUpdated))
	{
		const PathMarker *pathMarker = GetTargetNode();
		
		int32 pathCount = 0;
		int32 tangentCount = 0;
		int32 pointCount = 0;
		
		const Path *path = pathMarker->GetPath();
		const PathComponent *component = path->GetFirstPathComponent();
		while (component)
		{
			pathCount += 2;
			
			PathType type = component->GetPathType();
			if (type == kPathLinear)
			{
				pathCount += 2;
				pointCount += 8;
			}
			else if (type == kPathElliptical)
			{
				pathCount += 64;
				tangentCount += 4;
				pointCount += 12;
			}
			else if (type == kPathBezier)
			{
				pathCount += 64;
				tangentCount += 8;
				pointCount += 16;
			}
			
			component = component->Next();
		}
		
		if ((!pathStorage) || (pathVertexCount != pathCount) || (tangentVertexCount != tangentCount) || (pointVertexCount != pointCount))
		{
			delete[] pathStorage;
			pathStorage = new char[pathCount * (sizeof(Point3D) + sizeof(Vector4D) + sizeof(Point2D)) + tangentCount * (sizeof(Point3D) + sizeof(ColorRGBA) + sizeof(Vector4D) + sizeof(Point2D)) + pointCount * (sizeof(Point3D) + sizeof(ColorRGBA) + sizeof(Vector2D))];
			
			pathVertexCount = pathCount;
			pathVertex = reinterpret_cast<Point3D *>(pathStorage);
			pathTangent = reinterpret_cast<Vector4D *>(pathVertex + pathCount);
			pathTexcoord = reinterpret_cast<Point2D *>(pathTangent + pathCount);
			
			pathRenderable.SetVertexCount(pathCount);
			pathRenderable.SetAttributeArray(kArrayVertex, pathVertex);
			pathRenderable.SetAttributeArray(kArrayTangent, pathTangent);
			pathRenderable.SetAttributeArray(kArrayTexture0, pathTexcoord);
			
			tangentVertexCount = tangentCount;
			tangentVertex = reinterpret_cast<Point3D *>(pathTexcoord + pathCount);
			tangentColor = reinterpret_cast<ColorRGBA *>(tangentVertex + tangentCount);
			tangentTangent = reinterpret_cast<Vector4D *>(tangentColor + tangentCount);
			tangentTexcoord = reinterpret_cast<Point2D *>(tangentTangent + tangentCount);
			
			tangentRenderable.SetVertexCount(tangentCount);
			tangentRenderable.SetAttributeArray(kArrayVertex, tangentVertex);
			tangentRenderable.SetAttributeArray(kArrayColor0, tangentColor);
			tangentRenderable.SetAttributeArray(kArrayTangent, tangentTangent);
			tangentRenderable.SetAttributeArray(kArrayTexture0, tangentTexcoord);
			
			pointVertexCount = pointCount;
			pointVertex = reinterpret_cast<Point3D *>(tangentTexcoord + tangentCount);
			pointColor = reinterpret_cast<ColorRGBA *>(pointVertex + pointCount);
			pointBillboard = reinterpret_cast<Vector2D *>(pointColor + pointCount);
			
			pointRenderable.SetVertexCount(pointCount);
			pointRenderable.SetAttributeArray(kArrayVertex, pointVertex);
			pointRenderable.SetAttributeArray(kArrayColor0, pointColor);
			pointRenderable.SetAttributeArray(kArrayBillboard, pointBillboard);
		}
		
		Point3D *vertex = pathVertex;
		Vector4D *tangent = pathTangent;
		Point2D *texcoord = pathTexcoord;
		
		component = path->GetFirstPathComponent();
		while (component)
		{
			PathType type = component->GetPathType();
			if (type == kPathLinear)
			{
				const LinearPathComponent *linearComponent = static_cast<const LinearPathComponent *>(component);
				const Point3D& p1 = linearComponent->GetControlPoint(0);
				const Point3D& p2 = linearComponent->GetControlPoint(1);
				
				vertex[0] = vertex[1] = p1;
				
				Vector3D tang = (p2 - p1).Normalize();
				tangent[0].Set(tang, -1.0F);
				tangent[1].Set(tang, 1.0F);
				
				texcoord[0].Set(0.0F, 0.125F);
				texcoord[1].Set(1.0F, 0.125F);
				
				vertex += 2;
				tangent += 2;
				texcoord += 2;
			}
			else
			{
				for (machine a = 0; a < 32; a++)
				{
					float t = (float) a * 0.03125F;
					
					vertex[0] = vertex[1] = component->GetPosition(t);
					
					Vector3D tang = component->GetTangent(t).Normalize();
					tangent[0].Set(tang, -1.0F);
					tangent[1].Set(tang, 1.0F);
					
					texcoord[0].Set(0.0F, 0.125F);
					texcoord[1].Set(1.0F, 0.125F);
					
					vertex += 2;
					tangent += 2;
					texcoord += 2;
				}
			}
			
			vertex[0] = vertex[1] = component->GetEndPosition();
			
			Vector3D tang = component->GetEndTangent().Normalize();
			tangent[0].Set(tang, -1.0F);
			tangent[1].Set(tang, 1.0F);
			
			texcoord[0].Set(0.0F, 0.125F);
			texcoord[1].Set(1.0F, 0.125F);
			
			vertex += 2;
			tangent += 2;
			texcoord += 2;
			
			component = component->Next();
		}
		
		Point3D *tangVertex = tangentVertex;
		ColorRGBA *tangColor = tangentColor;
		Vector4D *tangTangent = tangentTangent;
		Point2D *tangTexcoord = tangentTexcoord;
		
		Point3D *ptVertex = pointVertex;
		ColorRGBA *ptColor = pointColor;
		Vector2D *ptBillboard = pointBillboard;
		
		int32 pointIndex = 0;
		component = path->GetFirstPathComponent();
		while (component)
		{
			PathType type = component->GetPathType();
			if (type == kPathLinear)
			{
				const LinearPathComponent *linearComponent = static_cast<const LinearPathComponent *>(component);
				const Point3D& p1 = linearComponent->GetControlPoint(0);
				const Point3D& p2 = linearComponent->GetControlPoint(1);
				
				for (machine a = 0; a < 4; a++)
				{
					ptVertex[a] = p1;
					ptVertex[a + 4] = p2;
				}
				
				for (machine a = 0; a < 2; a++)
				{
					if (ControlPointSelected(pointIndex))
					{
						ptColor[0] = K::white;
						ptColor[1] = K::white;
						ptColor[2] = K::white;
						ptColor[3] = K::white;
						
						ptBillboard[0].Set(4.0F, 4.0F);
						ptBillboard[1].Set(4.0F, -4.0F);
						ptBillboard[2].Set(-4.0F, -4.0F);
						ptBillboard[3].Set(-4.0F, 4.0F);
					}
					else
					{
						ptColor[0] = kPointRenderColor;
						ptColor[1] = kPointRenderColor;
						ptColor[2] = kPointRenderColor;
						ptColor[3] = kPointRenderColor;
						
						ptBillboard[0].Set(3.0F, 3.0F);
						ptBillboard[1].Set(3.0F, -3.0F);
						ptBillboard[2].Set(-3.0F, -3.0F);
						ptBillboard[3].Set(-3.0F, 3.0F);
					}
					
					ptColor += 4;
					ptBillboard += 4;
					pointIndex++;
				}
				
				ptVertex += 8;
			}
			else if (type == kPathElliptical)
			{
				const EllipticalPathComponent *ellipticalComponent = static_cast<const EllipticalPathComponent *>(component);
				const Point3D& p1 = ellipticalComponent->GetControlPoint(0);
				const Point3D& p2 = ellipticalComponent->GetControlPoint(1);
				const Point3D& p3 = ellipticalComponent->GetControlPoint(2);
				
				tangVertex[0] = tangVertex[1] = p1;
				tangVertex[2] = tangVertex[3] = p3;
				
				for (machine a = 0; a < 4; a++) tangColor[a] = kTangentRenderColor;
				
				Vector3D dp = p3 - p1;
				tangTangent[0].Set(dp, -0.5F);
				tangTangent[1].Set(dp, 0.5F);
				tangTangent[2].Set(dp, 0.5F);
				tangTangent[3].Set(dp, -0.5F);
				
				float m = Magnitude(dp) * 16.0F;
				tangTexcoord[0].Set(0.0F, 0.0F);
				tangTexcoord[1].Set(1.0F, 0.0F);
				tangTexcoord[2].Set(1.0F, m);
				tangTexcoord[3].Set(0.0F, m);
				
				tangVertex += 4;
				tangColor += 4;
				tangTangent += 4;
				tangTexcoord += 4;
				
				for (machine a = 0; a < 4; a++)
				{
					ptVertex[a] = p1;
					ptVertex[a + 4] = p2;
					ptVertex[a + 8] = p3;
				}
				
				for (machine a = 0; a < 3; a++)
				{
					if (ControlPointSelected(pointIndex))
					{
						ptColor[0] = K::white;
						ptColor[1] = K::white;
						ptColor[2] = K::white;
						ptColor[3] = K::white;
						
						ptBillboard[0].Set(4.0F, 4.0F);
						ptBillboard[1].Set(4.0F, -4.0F);
						ptBillboard[2].Set(-4.0F, -4.0F);
						ptBillboard[3].Set(-4.0F, 4.0F);
					}
					else
					{
						ptColor[0] = kPointRenderColor;
						ptColor[1] = kPointRenderColor;
						ptColor[2] = kPointRenderColor;
						ptColor[3] = kPointRenderColor;
						
						ptBillboard[0].Set(3.0F, 3.0F);
						ptBillboard[1].Set(3.0F, -3.0F);
						ptBillboard[2].Set(-3.0F, -3.0F);
						ptBillboard[3].Set(-3.0F, 3.0F);
					}
					
					ptColor += 4;
					ptBillboard += 4;
					pointIndex++;
				}
				
				ptVertex += 12;
			}
			else if (type == kPathBezier)
			{
				const BezierPathComponent *bezierComponent = static_cast<const BezierPathComponent *>(component);
				const Point3D& p1 = bezierComponent->GetControlPoint(0);
				const Point3D& p2 = bezierComponent->GetControlPoint(1);
				const Point3D& p3 = bezierComponent->GetControlPoint(2);
				const Point3D& p4 = bezierComponent->GetControlPoint(3);
				
				tangVertex[0] = tangVertex[1] = p1;
				tangVertex[2] = tangVertex[3] = p2;
				tangVertex[4] = tangVertex[5] = p4;
				tangVertex[6] = tangVertex[7] = p3;
				
				for (machine a = 0; a < 8; a++) tangColor[a] = kTangentRenderColor;
				
				Vector3D t1 = (p2 - p1).Normalize();
				tangTangent[0].Set(t1, -0.5F);
				tangTangent[1].Set(t1, 0.5F);
				tangTangent[2].Set(t1, 0.5F);
				tangTangent[3].Set(t1, -0.5F);
				
				Vector3D t2 = (p3 - p4).Normalize();
				tangTangent[4].Set(t2, -0.5F);
				tangTangent[5].Set(t2, 0.5F);
				tangTangent[6].Set(t2, 0.5F);
				tangTangent[7].Set(t2, -0.5F);
				
				float m1 = Magnitude(p2 - p1) * 16.0F;
				float m2 = Magnitude(p3 - p4) * 16.0F;
				
				tangTexcoord[0].Set(0.0F, 0.0F);
				tangTexcoord[1].Set(1.0F, 0.0F);
				tangTexcoord[2].Set(1.0F, m1);
				tangTexcoord[3].Set(0.0F, m1);
				
				tangTexcoord[4].Set(0.0F, 0.0F);
				tangTexcoord[5].Set(1.0F, 0.0F);
				tangTexcoord[6].Set(1.0F, m2);
				tangTexcoord[7].Set(0.0F, m2);
				
				tangVertex += 8;
				tangColor += 8;
				tangTangent += 8;
				tangTexcoord += 8;
				
				for (machine a = 0; a < 4; a++)
				{
					ptVertex[a] = p1;
					ptVertex[a + 4] = p2;
					ptVertex[a + 8] = p3;
					ptVertex[a + 12] = p4;
				}
				
				for (machine a = 0; a < 4; a++)
				{
					if (ControlPointSelected(pointIndex))
					{
						ptColor[0] = K::white;
						ptColor[1] = K::white;
						ptColor[2] = K::white;
						ptColor[3] = K::white;
						
						ptBillboard[0].Set(4.0F, 4.0F);
						ptBillboard[1].Set(4.0F, -4.0F);
						ptBillboard[2].Set(-4.0F, -4.0F);
						ptBillboard[3].Set(-4.0F, 4.0F);
					}
					else
					{
						ptColor[0] = kPointRenderColor;
						ptColor[1] = kPointRenderColor;
						ptColor[2] = kPointRenderColor;
						ptColor[3] = kPointRenderColor;
						
						ptBillboard[0].Set(3.0F, 3.0F);
						ptBillboard[1].Set(3.0F, -3.0F);
						ptBillboard[2].Set(-3.0F, -3.0F);
						ptBillboard[3].Set(-3.0F, 3.0F);
					}
					
					ptColor += 4;
					ptBillboard += 4;
					pointIndex++;
				}
				
				ptVertex += 16;
			}
			
			component = component->Next();
		}
	}
	
	EditorManipulator::Update();
}

void PathManipulator::Render(const ManipulatorRenderData *renderData)
{
	List<Renderable> *renderList = renderData->manipulatorList;
	if (renderList)
	{
		float scale = renderData->viewportScale;
		pathSizeVector.Set(1.0F, 1.0F, 1.0F, scale);
		pointSizeVector.Set(scale, scale, scale, scale);
		
		renderList->Append(&pathRenderable);
		if (Selected())
		{
			renderList->Append(&tangentRenderable);
			renderList->Append(&pointRenderable);
		}
	}
	
	EditorManipulator::Render(renderData);
}

// ZYURVUR
