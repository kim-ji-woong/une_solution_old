//=============================================================
//
// C4 Engine version 2.9.1
// Copyright 1999-2012, by Terathon Software LLC
//
// This copy is licensed to the following:
//
//     Registered user: Soo Ki Kim
//     Maximum number of users: 1
//     License #C4T0035002
//
// License is granted under terms of the license agreement
// entered by the registed user.
//
// Unauthorized redistribution of source code is strictly
// prohibited. Violators will be prosecuted.
//
//=============================================================


#include "C4Cloth.h"
#include "C4Forces.h"
#include "C4World.h"
#include "C4Configuration.h"


using namespace C4;


namespace
{
	enum
	{
		kClothTimeStep			= 10,
		kClothUpdateInterval	= 4000
	};
	
	
	const float kClothDeltaTime = (float) kClothTimeStep;
	const float kClothSquaredDeltaTime = kClothDeltaTime * kClothDeltaTime;
	const float kClothInverseDeltaTime = 1.0F / kClothDeltaTime;
}


const char C4::kConnectorKeyWind[] = "%Wind";


ClothGeometryObject::ClothGeometryObject() : PrimitiveGeometryObject(kPrimitiveCloth)
{
	flexibilityFlags = kClothLowerLeftCorner | kClothLowerRightCorner | kClothUpperRightCorner | kClothUpperLeftCorner;
	clothFlexibility = nullptr;
	
	SetStaticSurfaceData(2, staticSurfaceData);
}

ClothGeometryObject::ClothGeometryObject(const Vector2D& size, int32 width, int32 height) : PrimitiveGeometryObject(kPrimitiveCloth)
{
	clothSize = size;
	clothFlexibility = nullptr;
	
	SetFieldSize(width, height);
	SetFlexibilityFlags(kClothLowerLeftCorner | kClothLowerRightCorner | kClothUpperRightCorner | kClothUpperLeftCorner);
	
	SetGeometryFlags(kGeometryMarkingInhibit | kGeometryDynamic | kGeometryTwoSidedPlaneArray);
	
	SetStaticSurfaceData(2, staticSurfaceData, true);
	
	staticSurfaceData[0].textureAlignData[0].alignMode = kTextureAlignNatural;
	staticSurfaceData[0].textureAlignData[0].alignPlane.Set(1.0F, 0.0F, 0.0F, 0.0F);
	staticSurfaceData[0].textureAlignData[1].alignMode = kTextureAlignNatural;
	staticSurfaceData[0].textureAlignData[1].alignPlane.Set(0.0F, 1.0F, 0.0F, 0.0F);
	
	staticSurfaceData[1].textureAlignData[0].alignMode = kTextureAlignNatural;
	staticSurfaceData[1].textureAlignData[0].alignPlane.Set(-1.0F, 0.0F, 0.0F, 1.0F);
	staticSurfaceData[1].textureAlignData[1].alignMode = kTextureAlignNatural;
	staticSurfaceData[1].textureAlignData[1].alignPlane.Set(0.0F, 1.0F, 0.0F, 0.0F);
}

ClothGeometryObject::~ClothGeometryObject()
{
	delete[] clothFlexibility;
}

void ClothGeometryObject::Pack(Packer& data, unsigned_int32 packFlags) const
{
	PrimitiveGeometryObject::Pack(data, packFlags);
	
	data << ChunkHeader('SIZE', sizeof(Vector2D));
	data << clothSize;
	
	data << ChunkHeader('FFLG', 4);
	data << flexibilityFlags;
	
	int32 massCount = GetFieldWidth() * GetFieldHeight();
	data << ChunkHeader('FLEX', massCount * 4);
	data.WriteArray(massCount, clothFlexibility);
	
	data << TerminatorChunk;
}

void ClothGeometryObject::Unpack(Unpacker& data, unsigned_int32 unpackFlags)
{
	PrimitiveGeometryObject::Unpack(data, unpackFlags);
	UnpackChunkList<ClothGeometryObject>(data, unpackFlags);
}

bool ClothGeometryObject::UnpackChunk(const ChunkHeader *chunkHeader, Unpacker& data, unsigned_int32 unpackFlags)
{
	switch (chunkHeader->chunkType)
	{
		case 'SIZE':
			
			data >> clothSize;
			return (true);
		
		case 'FFLG':
			
			data >> flexibilityFlags;
			return (true);
		
		case 'FLEX':
		{ 
			int32 massCount = GetFieldWidth() * GetFieldHeight();
			clothFlexibility = new float[massCount]; 
			data.ReadArray(massCount, clothFlexibility); 
			return (true); 
		}
	} 
	
	return (false);
}
 
void *ClothGeometryObject::BeginSettingsUnpack(void)
{
	delete[] clothFlexibility;
	clothFlexibility = nullptr; 
	
	return (PrimitiveGeometryObject::BeginSettingsUnpack());
}

int32 ClothGeometryObject::GetCategorySettingCount(Type category) const
{
	int32 count = PrimitiveGeometryObject::GetCategorySettingCount(category);
	if (category == kObjectGeometry) count += 9;
	return (count);
}

Setting *ClothGeometryObject::GetCategorySetting(Type category, int32 index, unsigned_int32 flags) const
{
	if (category == kObjectGeometry)
	{
		int32 count = PrimitiveGeometryObject::GetCategorySettingCount(kObjectGeometry);
		if (index >= count)
		{
			if (flags & kConfigurationScript) return (nullptr);
			
			const StringTable *table = TheInterfaceMgr->GetStringTable();
			
			if (index == count)
			{
				const char *title = table->GetString(StringID(kObjectGeometry, kPrimitiveCloth));
				return (new HeadingSetting(kPrimitiveCloth, title));
			}
			
			if (index == count + 1)
			{
				const char *title = table->GetString(StringID(kObjectGeometry, kPrimitiveCloth, 'COR1'));
				return (new BooleanSetting('COR1', ((flexibilityFlags & kClothLowerLeftCorner) != 0), title));
			}
			
			if (index == count + 2)
			{
				const char *title = table->GetString(StringID(kObjectGeometry, kPrimitiveCloth, 'COR2'));
				return (new BooleanSetting('COR2', ((flexibilityFlags & kClothLowerRightCorner) != 0), title));
			}
			
			if (index == count + 3)
			{
				const char *title = table->GetString(StringID(kObjectGeometry, kPrimitiveCloth, 'COR3'));
				return (new BooleanSetting('COR3', ((flexibilityFlags & kClothUpperRightCorner) != 0), title));
			}
			
			if (index == count + 4)
			{
				const char *title = table->GetString(StringID(kObjectGeometry, kPrimitiveCloth, 'COR4'));
				return (new BooleanSetting('COR4', ((flexibilityFlags & kClothUpperLeftCorner) != 0), title));
			}
			
			if (index == count + 5)
			{
				const char *title = table->GetString(StringID(kObjectGeometry, kPrimitiveCloth, 'EDG1'));
				return (new BooleanSetting('EDG1', ((flexibilityFlags & kClothBottomEdge) != 0), title));
			}
			
			if (index == count + 6)
			{
				const char *title = table->GetString(StringID(kObjectGeometry, kPrimitiveCloth, 'EDG2'));
				return (new BooleanSetting('EDG2', ((flexibilityFlags & kClothRightEdge) != 0), title));
			}
			
			if (index == count + 7)
			{
				const char *title = table->GetString(StringID(kObjectGeometry, kPrimitiveCloth, 'EDG3'));
				return (new BooleanSetting('EDG3', ((flexibilityFlags & kClothTopEdge) != 0), title));
			}
			
			if (index == count + 8)
			{
				const char *title = table->GetString(StringID(kObjectGeometry, kPrimitiveCloth, 'EDG4'));
				return (new BooleanSetting('EDG4', ((flexibilityFlags & kClothLeftEdge) != 0), title));
			}
			
			return (nullptr);
		}
	}
	
	return (PrimitiveGeometryObject::GetCategorySetting(category, index, flags));
}

void ClothGeometryObject::SetCategorySetting(Type category, const Setting *setting)
{
	if (category == kObjectGeometry)
	{
		Type identifier = setting->GetSettingIdentifier();
		
		if (identifier == 'COR1')
		{
			bool b = static_cast<const BooleanSetting *>(setting)->GetBooleanValue();
			if (b) flexibilityFlags |= kClothLowerLeftCorner;
			else flexibilityFlags &= ~kClothLowerLeftCorner;
		}
		else if (identifier == 'COR2')
		{
			bool b = static_cast<const BooleanSetting *>(setting)->GetBooleanValue();
			if (b) flexibilityFlags |= kClothLowerRightCorner;
			else flexibilityFlags &= ~kClothLowerRightCorner;
		}
		else if (identifier == 'COR3')
		{
			bool b = static_cast<const BooleanSetting *>(setting)->GetBooleanValue();
			if (b) flexibilityFlags |= kClothUpperRightCorner;
			else flexibilityFlags &= ~kClothUpperRightCorner;
		}
		else if (identifier == 'COR4')
		{
			bool b = static_cast<const BooleanSetting *>(setting)->GetBooleanValue();
			if (b) flexibilityFlags |= kClothUpperLeftCorner;
			else flexibilityFlags &= ~kClothUpperLeftCorner;
		}
		else if (identifier == 'EDG1')
		{
			bool b = static_cast<const BooleanSetting *>(setting)->GetBooleanValue();
			if (b) flexibilityFlags |= kClothBottomEdge;
			else flexibilityFlags &= ~kClothBottomEdge;
		}
		else if (identifier == 'EDG2')
		{
			bool b = static_cast<const BooleanSetting *>(setting)->GetBooleanValue();
			if (b) flexibilityFlags |= kClothRightEdge;
			else flexibilityFlags &= ~kClothRightEdge;
		}
		else if (identifier == 'EDG3')
		{
			bool b = static_cast<const BooleanSetting *>(setting)->GetBooleanValue();
			if (b) flexibilityFlags |= kClothTopEdge;
			else flexibilityFlags &= ~kClothTopEdge;
		}
		else if (identifier == 'EDG4')
		{
			bool b = static_cast<const BooleanSetting *>(setting)->GetBooleanValue();
			if (b) flexibilityFlags |= kClothLeftEdge;
			else flexibilityFlags &= ~kClothLeftEdge;
			
			SetFieldSize(GetFieldWidth(), GetFieldHeight());
		}
		else
		{
			PrimitiveGeometryObject::SetCategorySetting(kObjectGeometry, setting);
		}
	}
	else
	{
		PrimitiveGeometryObject::SetCategorySetting(category, setting);
	}
}

int32 ClothGeometryObject::GetObjectSize(float *size) const
{
	size[0] = clothSize.x;
	size[1] = clothSize.y;
	return (2);
}

void ClothGeometryObject::SetObjectSize(const float *size)
{
	clothSize.x = size[0];
	clothSize.y = size[1];
}

void ClothGeometryObject::Build(Geometry *geometry)
{
	ArrayDescriptor		desc[9];
	
	int32 levelCount = Min(GetBuildLevelCount(), 2);
	SetGeometryLevelCount(levelCount);
	
	int32 materialIndex1 = GetSurfaceData(0)->materialIndex;
	int32 materialIndex2 = GetSurfaceData(1)->materialIndex;
	
	for (machine level = 0; level < levelCount; level++)
	{
		int32 width = GetFieldWidth();
		int32 height = GetFieldHeight();
		
		if (level < levelCount - 1)
		{
			width = width * 2 - 1;
			height = height * 2 - 1;
		}
		
		int32 vertexCount = width * height * 2;
		int32 planeCount = (width - 1) * (height - 1) * 2;
		int32 edgeCount = ((width - 1) * height + width * (height - 1) + planeCount) * 2;
		int32 triangleCount = planeCount * 2;
		
		desc[0].identifier = kArrayVertex;
		desc[0].elementCount = vertexCount;
		desc[0].elementSize = sizeof(Point3D);
		desc[0].componentCount = 3;
		
		desc[1].identifier = kArrayNormal;
		desc[1].elementCount = vertexCount;
		desc[1].elementSize = sizeof(Vector3D);
		desc[1].componentCount = 3;
		
		desc[2].identifier = kArrayTangent;
		desc[2].elementCount = vertexCount;
		desc[2].elementSize = sizeof(Vector4D);
		desc[2].componentCount = 4;
		
		desc[3].identifier = kArrayTexture0;
		desc[3].elementCount = vertexCount;
		desc[3].elementSize = sizeof(Point2D);
		desc[3].componentCount = 2;
		
		desc[4].identifier = kArrayFace;
		desc[4].elementCount = triangleCount;
		desc[4].elementSize = sizeof(Triangle);
		desc[4].componentCount = 1;
		
		desc[5].identifier = kArraySurfaceIndex;
		desc[5].elementCount = vertexCount;
		desc[5].elementSize = 2;
		desc[5].componentCount = 1;
		
		int32 arrayCount = 6;
		if (!(GetGeometryFlags() & kGeometryShadowInhibit))
		{
			desc[arrayCount].identifier = kArrayEdge;
			desc[arrayCount].elementCount = edgeCount;
			desc[arrayCount].elementSize = sizeof(Edge);
			desc[arrayCount].componentCount = 1;
			arrayCount++;
			
			desc[arrayCount].identifier = kArrayPlane;
			desc[arrayCount].elementCount = planeCount;
			desc[arrayCount].elementSize = sizeof(Vector4D);
			desc[arrayCount].componentCount = 4;
			arrayCount++;
		}
		
		if (materialIndex1 != materialIndex2)
		{
			desc[arrayCount].identifier = kArraySegment;
			desc[arrayCount].elementCount = 2;
			desc[arrayCount].elementSize = sizeof(SegmentData);
			desc[arrayCount].componentCount = 1;
			arrayCount++;
		}
		
		GeometryLevel *geometryLevel = GetGeometryLevel(level);
		geometryLevel->AllocateStorage(vertexCount, arrayCount, desc);
		
		Point3D *vertex = geometryLevel->GetArray<Point3D>(kArrayVertex);
		Vector3D *normal = geometryLevel->GetArray<Vector3D>(kArrayNormal);
		Vector4D *tangent = geometryLevel->GetArray<Vector4D>(kArrayTangent);
		Point2D *texcoord = geometryLevel->GetArray<Point2D>(kArrayTexture0);
		unsigned_int16 *surfaceIndex = geometryLevel->GetArray<unsigned_int16>(kArraySurfaceIndex) - 1;
		
		float dx = 1.0F / (float) (width - 1);
		float dy = 1.0F / (float) (height - 1);
		float gx = GetClothSize().x * dx;
		float gy = GetClothSize().y * dy;
		
		for (machine j = 0; j < height; j++)
		{
			float fj = (float) j;
			
			for (machine i = 0; i < width; i++)
			{
				float fi = (float) i;
				
				vertex->Set(fi * gx, fj * gy, 0.0F);
				normal->Set(0.0F, 0.0F, 1.0F);
				tangent->Set(1.0F, 0.0F, 0.0F, 1.0F);
				texcoord->Set(fi * dx, fj * dy);
				*++surfaceIndex = 0;
				
				vertex++;
				normal++;
				tangent++;
				texcoord++;
			}
		}
		
		for (machine j = 0; j < height; j++)
		{
			float fj = (float) j;
			
			for (machine i = 0; i < width; i++)
			{
				float fi = (float) i;
				
				vertex->Set(fi * gx, fj * gy, 0.0F);
				normal->Set(0.0F, 0.0F, -1.0F);
				tangent->Set(1.0F, 0.0F, 0.0F, -1.0F);
				texcoord->Set(fi * dx, fj * dy);
				*++surfaceIndex = 1;
				
				vertex++;
				normal++;
				tangent++;
				texcoord++;
			}
		}
		
		Triangle *triangle = geometryLevel->GetArray<Triangle>(kArrayFace);
		
		for (machine j = 0; j < height - 1; j++)
		{
			int32 k = j * width;
			for (machine i = 0; i < width - 1; i++)
			{
				if (((i + j) & 1) == 0)
				{
					triangle[0].Set(k + i, k + width + i + 1, k + width + i);
					triangle[1].Set(k + i, k + i + 1, k + width + i + 1);
				}
				else
				{
					triangle[0].Set(k + i, k + i + 1, k + width + i);
					triangle[1].Set(k + i + 1, k + width + i + 1, k + width + i);
				}
				
				triangle += 2;
			}
		}
		
		int32 offset = width * height;
		for (machine j = 0; j < height - 1; j++)
		{
			int32 k = j * width + offset;
			for (machine i = 0; i < width - 1; i++)
			{
				if (((i + j) & 1) == 0)
				{
					triangle[0].Set(k + i, k + width + i, k + width + i + 1);
					triangle[1].Set(k + i, k + width + i + 1, k + i + 1);
				}
				else
				{
					triangle[0].Set(k + i, k + width + i, k + i + 1);
					triangle[1].Set(k + i + 1, k + width + i, k + width + i + 1);
				}
				
				triangle += 2;
			}
		}
		
		geometryLevel->GenerateTexcoords(geometry, this);
		geometryLevel->TransformTexcoords(geometry, this);
		
		if (!(GetGeometryFlags() & kGeometryShadowInhibit))
		{
			unsigned_int32 *remapTable = new unsigned_int32[vertexCount];
			for (machine a = 0; a < vertexCount; a++) remapTable[a] = a;
			
			offset = width * height;
			int32 top = width * (height - 1);
			for (machine a = 0; a < width; a++)
			{
				remapTable[a + offset] = a;
				remapTable[a + top + offset] = a + top;
			}
			
			for (machine a = 0; a < height; a++)
			{
				remapTable[a * width + offset] = a * width;
				remapTable[(a + 1) * width - 1 + offset] = (a + 1) * width - 1;
			}
			
			geometryLevel->CalculateEdgeArray(remapTable);
			delete[] remapTable;
			
			Antivector4D *plane = geometryLevel->GetArray<Antivector4D>(kArrayPlane);
			for (machine a = 0; a < planeCount; a++) plane[a].Set(0.0F, 0.0F, 1.0F, 0.0F);
		}
		
		if (materialIndex1 != materialIndex2)
		{
			SegmentData *segmentData = geometryLevel->GetArray<SegmentData>(kArraySegment);
			int32 count = triangleCount / 2;
			
			if (materialIndex1 < materialIndex2)
			{
				segmentData[0].materialIndex = materialIndex1;
				segmentData[0].faceStart = 0;
				segmentData[0].faceCount = count;
				
				segmentData[1].materialIndex = materialIndex2;
				segmentData[1].faceStart = count;
				segmentData[1].faceCount = count;
			}
			else
			{
				segmentData[0].materialIndex = materialIndex2;
				segmentData[0].faceStart = count;
				segmentData[0].faceCount = count;
				
				segmentData[1].materialIndex = materialIndex1;
				segmentData[1].faceStart = 0;
				segmentData[1].faceCount = count;
			}
		}
	}
}

void ClothGeometryObject::SetFieldSize(int32 width, int32 height)
{
	delete[] clothFlexibility;
	
	width = Min(width, kMaxClothSize);
	height = Min(height, kMaxClothSize);
	SetMaxSubdiv(width - 1, height - 1);
	
	int32 massCount = width * height;
	float *flexibility = new float[massCount];
	clothFlexibility = flexibility;
	
	UpdateFlexibility();
}

void ClothGeometryObject::UpdateFlexibility(void)
{
	int32 fieldWidth = GetFieldWidth();
	int32 fieldHeight = GetFieldHeight();
	int32 massCount = fieldWidth * fieldHeight;
	
	float *flexibility = clothFlexibility;
	for (machine a = 0; a < massCount; a++) flexibility[a] = 1.0F;
	
	unsigned_int32 flags = flexibilityFlags;
	
	if (flags & kClothLowerLeftCorner) flexibility[0] = 0.0F;
	if (flags & kClothLowerRightCorner) flexibility[fieldWidth - 1] = 0.0F;
	if (flags & kClothUpperRightCorner) flexibility[massCount - 1] = 0.0F;
	if (flags & kClothUpperLeftCorner) flexibility[massCount - fieldWidth] = 0.0F;
	
	if (flags & kClothBottomEdge)
	{
		for (machine a = 0; a < fieldWidth; a++) flexibility[a] = 0.0F;
	}
	
	if (flags & kClothRightEdge)
	{
		for (machine a = 1; a <= fieldHeight; a++) flexibility[a * fieldWidth - 1] = 0.0F;
	}
	
	if (flags & kClothTopEdge)
	{
		for (machine a = 0; a < fieldWidth; a++) flexibility[massCount - fieldWidth + a] = 0.0F;
	}
	
	if (flags & kClothLeftEdge)
	{
		for (machine a = 0; a < fieldHeight; a++) flexibility[a * fieldWidth] = 0.0F;
	}
}


ClothGeometry::ClothGeometry() : PrimitiveGeometry(kPrimitiveCloth)
{
	clothCenter = nullptr;
}

ClothGeometry::ClothGeometry(const Vector2D& size, int32 width, int32 height) : PrimitiveGeometry(kPrimitiveCloth)
{
	SetNewObject(new ClothGeometryObject(size, width, height));
	SetController(new ClothController);
	
	clothCenter = nullptr;
}

ClothGeometry::ClothGeometry(const ClothGeometry& clothGeometry) : PrimitiveGeometry(clothGeometry)
{
	clothCenter = nullptr;
}

ClothGeometry::~ClothGeometry()
{
}

Node *ClothGeometry::Replicate(void) const
{
	return (new ClothGeometry(*this));
}

bool ClothGeometry::CalculateBoundingBox(Box3D *box) const
{
	const Vector2D& clothSize = GetObject()->GetClothSize();
	
	if (clothCenter)
	{
		float r = Magnitude(clothSize) * 0.75F;
		box->min.Set(clothCenter->x - r, clothCenter->y - r, clothCenter->z - r);
		box->max.Set(clothCenter->x + r, clothCenter->y + r, clothCenter->z + r);
	}
	else
	{
		box->min.Set(0.0F, 0.0F, 0.0F);
		box->max.Set(clothSize, 0.0F);
	}
	
	return (true);
}

bool ClothGeometry::CalculateBoundingSphere(BoundingSphere *sphere) const
{
	const Vector2D& clothSize = GetObject()->GetClothSize();
	
	if (clothCenter)
	{
		sphere->SetCenter(*clothCenter);
		sphere->SetRadius(Magnitude(clothSize) * 0.75F);
	}
	else
	{
		float x = clothSize.x * 0.5F;
		float y = clothSize.y * 0.5F;
		
		sphere->SetCenter(x, y, 0.0F);
		sphere->SetRadius(Sqrt(x * x + y * y));
	}
	
	return (true);
}

int32 ClothGeometry::GetInternalConnectorCount(void) const
{
	return (PrimitiveGeometry::GetInternalConnectorCount() + 1);
}

const char *ClothGeometry::GetInternalConnectorKey(int32 index) const
{
	int32 count = PrimitiveGeometry::GetInternalConnectorCount();
	if (index < count) return (PrimitiveGeometry::GetInternalConnectorKey(index));
	
	if (index == count) return (kConnectorKeyWind);
	return (nullptr);
}

bool ClothGeometry::ValidConnectedNode(const ConnectorKey& key, const Node *node) const
{
	if (key == kConnectorKeyWind)
	{
		if (node->GetNodeType() == kNodeField)
		{
			const Field *field = static_cast<const Field *>(node);
			const Force *force = field->GetForce();
			return ((force) && (force->GetForceType() == kForceWind));
		}
		
		return (false);
	}
	
	return (PrimitiveGeometry::ValidConnectedNode(key, node));
}

void ClothGeometry::Preprocess(void)
{
	PrimitiveGeometry::Preprocess();
	
	const Controller *controller = GetController();
	if ((controller) && (controller->GetControllerType() == kControllerCloth))
	{
		const ClothController *clothController = static_cast<const ClothController *>(controller);
		const ClothGeometryObject *object = GetObject();
		clothCenter = &clothController->GetClothPosition()[object->GetFieldWidth() * object->GetFieldHeight() / 2];
	}
}

void ClothGeometry::CalculateInfiniteShadowFrontArray(const Vector3D& lightDirection)
{
	const ArrayBundle *planeBundle = GetArrayBundle(kArrayPlane);
	const Antivector4D *plane = static_cast<Antivector4D *>(planeBundle->pointer);
	int32 planeCount = planeBundle->descriptor.elementCount;
	
	bool *front1 = GetShadowFrontArray();
	bool *front2 = front1 + planeCount;
	
	for (machine a = 0; a < planeCount; a++)
	{
		bool f = ((plane[a] ^ lightDirection) > 0.0F);
		front1[a] = f;
		front2[a] = !f;
	}
}

void ClothGeometry::CalculatePointShadowFrontArray(const Point3D& lightPosition)
{
	const ArrayBundle *planeBundle = GetArrayBundle(kArrayPlane);
	const Antivector4D *plane = static_cast<Antivector4D *>(planeBundle->pointer);
	int32 planeCount = planeBundle->descriptor.elementCount;
	
	bool *front1 = GetShadowFrontArray();
	bool *front2 = front1 + planeCount;
	
	for (machine a = 0; a < planeCount; a++)
	{
		bool f = ((plane[a] ^ lightPosition) > 0.0F);
		front1[a] = f;
		front2[a] = !f;
	}
}


ClothController::ClothController() :
		Controller(kControllerCloth),
		clothUpdateJob(&ClothUpdateJob, &FinalizeUpdate, this),
		dynamicVertexBuffer(kVertexBufferAttribute | kVertexBufferDynamic),
		dynamicVertexBufferObserver(this, &ClothController::FillDynamicVertexBuffer)
{
	fieldStorage = nullptr;
	springStorage = nullptr;
	vertexStorage = nullptr;
	massCount = 0;
	
	viscosityConstant = 0.001F;
	connectConstant = 0.002F;
	shearConstant = 0.002F;
	bendConstant = 0.001F;
	
	gravityMultiplier = 1.0F;
}

ClothController::ClothController(const ClothController& clothController) :
		Controller(clothController),
		clothUpdateJob(&ClothUpdateJob, &FinalizeUpdate, this),
		dynamicVertexBuffer(kVertexBufferAttribute | kVertexBufferDynamic),
		dynamicVertexBufferObserver(this, &ClothController::FillDynamicVertexBuffer)
{
	fieldStorage = nullptr;
	springStorage = nullptr;
	vertexStorage = nullptr;
	massCount = 0;
	
	viscosityConstant = clothController.viscosityConstant;
	connectConstant = clothController.connectConstant;
	shearConstant = clothController.shearConstant;
	bendConstant = clothController.bendConstant;
	
	gravityMultiplier = clothController.gravityMultiplier;
}

ClothController::~ClothController()
{
	delete[] vertexStorage;
	delete[] springStorage;
	delete[] fieldStorage;
}

Controller *ClothController::Replicate(void) const
{
	return (new ClothController(*this));
}

bool ClothController::ValidNode(const Node *node)
{
	if (node->GetNodeType() == kNodeGeometry)
	{
		const Geometry *geometry = static_cast<const Geometry *>(node);
		if ((geometry->GetGeometryType() == kGeometryPrimitive) && (static_cast<const PrimitiveGeometry *>(geometry)->GetPrimitiveType() == kPrimitiveCloth)) return (true);
	}
	
	return (false);
}

void ClothController::Pack(Packer& data, unsigned_int32 packFlags) const
{
	Controller::Pack(data, packFlags);
	
	data << ChunkHeader('PARM', 16);
	data << viscosityConstant;
	data << connectConstant;
	data << shearConstant;
	data << bendConstant;
	
	if ((fieldStorage) && (!GetTargetNode()->GetManipulator()))
	{
		data << ChunkHeader('FELD', 4 + massCount * (sizeof(Point3D) + sizeof(Vector3D)));
		data << massCount;
		
		data.WriteArray(massCount, clothPosition[0]);
		data.WriteArray(massCount, clothPosition[1]);
	}
	
	data << ChunkHeader('GRAV', 4);
	data << gravityMultiplier;
	
	data << TerminatorChunk;
}

void ClothController::Unpack(Unpacker& data, unsigned_int32 unpackFlags)
{
	Controller::Unpack(data, unpackFlags);
	UnpackChunkList<ClothController>(data, unpackFlags);
}

bool ClothController::UnpackChunk(const ChunkHeader *chunkHeader, Unpacker& data, unsigned_int32 unpackFlags)
{
	switch (chunkHeader->chunkType)
	{
		case 'PARM':
			
			data >> viscosityConstant;
			data >> connectConstant;
			data >> shearConstant;
			data >> bendConstant;
			return (true);
		
		case 'FELD':
			
			data >> massCount;
			AllocateFieldStorage();
			
			data.ReadArray(massCount, clothPosition[0]);
			data.ReadArray(massCount, clothPosition[1]);
			
			return (true);
		
		case 'GRAV':
			
			data >> gravityMultiplier;
			return (true);
	}
	
	return (false);
}

int32 ClothController::GetSettingCount(void) const
{
	return (5);
}

Setting *ClothController::GetSetting(int32 index) const
{
	const StringTable *table = TheInterfaceMgr->GetStringTable();
	
	if (index == 0)
	{
		const char *title = table->GetString(StringID('CTRL', kControllerCloth, 'GRAV'));
		return (new TextSetting('GRAV', gravityMultiplier, title));
	}
	
	if (index == 1)
	{
		const char *title = table->GetString(StringID('CTRL', kControllerCloth, 'VISC'));
		return (new FloatSetting('VISC', viscosityConstant, title, 0.0F, 0.01F, 0.0001F));
	}
	
	if (index == 2)
	{
		const char *title = table->GetString(StringID('CTRL', kControllerCloth, 'CONN'));
		return (new FloatSetting('CONN', connectConstant, title, 0.0F, 0.01F, 0.0001F));
	}
	
	if (index == 3)
	{
		const char *title = table->GetString(StringID('CTRL', kControllerCloth, 'SHER'));
		return (new FloatSetting('SHER', shearConstant, title, 0.0F, 0.01F, 0.0001F));
	}
	
	if (index == 4)
	{
		const char *title = table->GetString(StringID('CTRL', kControllerCloth, 'BEND'));
		return (new FloatSetting('BEND', bendConstant, title, 0.0F, 0.01F, 0.0001F));
	}
	
	return (nullptr);
}

void ClothController::SetSetting(const Setting *setting)
{
	Type identifier = setting->GetSettingIdentifier();
	
	if (identifier == 'GRAV')
	{
		const char *text = static_cast<const TextSetting *>(setting)->GetText();
		gravityMultiplier = FmaxZero(Text::StringToFloat(text));
	}
	else if (identifier == 'VISC')
	{
		viscosityConstant = static_cast<const FloatSetting *>(setting)->GetFloatValue();
	}
	else if (identifier == 'CONN')
	{
		connectConstant = static_cast<const FloatSetting *>(setting)->GetFloatValue();
	}
	else if (identifier == 'SHER')
	{
		shearConstant = static_cast<const FloatSetting *>(setting)->GetFloatValue();
	}
	else if (identifier == 'BEND')
	{
		bendConstant = static_cast<const FloatSetting *>(setting)->GetFloatValue();
	}
}

void ClothController::AllocateFieldStorage(void)
{
	#if C4SIMD
	
		fieldStorage = new char[massCount * (sizeof(Point3D) * 2 + sizeof(Vector3D) + 16)];
		clothForce = reinterpret_cast<float4 *>(fieldStorage);
	
	#else
	
		fieldStorage = new char[massCount * (sizeof(Point3D) * 2 + sizeof(Vector3D) * 2)];
		clothForce = reinterpret_cast<Vector3D *>(fieldStorage);
	
	#endif
	
	clothPosition[0] = reinterpret_cast<Point3D *>(clothForce + massCount);
	clothPosition[1] = clothPosition[0] + massCount;
	clothBitangent = clothPosition[1] + massCount;
}

void ClothController::AllocateSpringStorage(void)
{
	springStorage = new char[(connectCount + shearCount + bendCount) * sizeof(SpringData)];
	
	connectSpring = reinterpret_cast<SpringData *>(springStorage);
	shearSpring = connectSpring + connectCount;
	bendSpring = shearSpring + shearCount;
}

void ClothController::Preprocess(void)
{
	Controller::Preprocess();
	
	ClothGeometry *clothGeometry = GetTargetNode();
	clothGeometry->SetShaderFlags(clothGeometry->GetShaderFlags() | kShaderNormalizeBasisVectors);
	clothGeometry->SetDynamicArrayFlags((1 << kArrayVertex) | (1 << kArrayVelocity) | (1 << kArrayNormal) | (1 << kArrayTangent));
	
	ClothGeometryObject *object = clothGeometry->GetObject();
	const GeometryLevel *geometryLevel = object->GetGeometryLevel(0);
	int32 vertexCount = geometryLevel->GetVertexCount();
	
	dynamicVertexBuffer.Initialize(vertexCount * sizeof(DynamicVertex), sizeof(DynamicVertex));
	clothGeometry->SetVertexBuffer(kVertexBufferDynamicArray, &dynamicVertexBuffer);
	
	int32 width = object->GetFieldWidth();
	int32 height = object->GetFieldHeight();
	massCount = width * height;
	
	const Vector2D& size = object->GetClothSize();
	
	connectDistance = Fmin(size.x / (float) (width - 1), size.y / (float) (height - 1));
	shearDistance = connectDistance * K::sqrt_2;
	bendDistance = connectDistance * 2.0F;
	
	physicsController = nullptr;
	const World *world = clothGeometry->GetWorld();
	if (world)
	{
		PhysicsNode *physicsNode = world->GetRootNode()->GetPhysicsNode();
		if (physicsNode)
		{
			Controller *controller = physicsNode->GetController();
			if ((controller) && (controller->GetControllerType() == kControllerPhysics)) physicsController = static_cast<PhysicsController *>(controller);
		}
	}
	
	windForceField = nullptr;
	const Node *node = clothGeometry->GetConnectedNode(kConnectorKeyWind);
	if ((node) && (node->GetNodeType() == kNodeField))
	{
		const Field *field = static_cast<const Field *>(node);
		const Force *force = field->GetForce();
		if ((force) && (force->GetForceType() == kForceWind)) windForceField = field;
	}
	
	int32 planeCount = geometryLevel->GetArrayBundle(kArrayPlane)->descriptor.elementCount;
	vertexStorage = new char[vertexCount * (sizeof(Point3D) + sizeof(Vector3D) * 2 + sizeof(Vector4D)) + planeCount * sizeof(Antivector4D)];
	
	vertexBundle.descriptor.identifier = kArrayVertex;
	vertexBundle.descriptor.elementCount = vertexCount;
	vertexBundle.descriptor.elementSize = sizeof(Point3D);
	vertexBundle.descriptor.componentCount = 3;
	vertexBundle.pointer = vertexStorage;
	
	velocityBundle.descriptor.identifier = kArrayVelocity;
	velocityBundle.descriptor.elementCount = vertexCount;
	velocityBundle.descriptor.elementSize = sizeof(Vector3D);
	velocityBundle.descriptor.componentCount = 3;
	velocityBundle.pointer = static_cast<Point3D *>(vertexBundle.pointer) + vertexCount;
	
	normalBundle.descriptor.identifier = kArrayNormal;
	normalBundle.descriptor.elementCount = vertexCount;
	normalBundle.descriptor.elementSize = sizeof(Vector3D);
	normalBundle.descriptor.componentCount = 3;
	normalBundle.pointer = static_cast<Vector3D *>(velocityBundle.pointer) + vertexCount;
	
	tangentBundle.descriptor.identifier = kArrayTangent;
	tangentBundle.descriptor.elementCount = vertexCount;
	tangentBundle.descriptor.elementSize = sizeof(Vector4D);
	tangentBundle.descriptor.componentCount = 4;
	tangentBundle.pointer = static_cast<Vector3D *>(normalBundle.pointer) + vertexCount;
	
	planeBundle.descriptor.identifier = kArrayPlane;
	planeBundle.descriptor.elementCount = planeCount;
	planeBundle.descriptor.elementSize = sizeof(Antivector4D);
	planeBundle.descriptor.componentCount = 4;
	planeBundle.pointer = static_cast<Antivector4D *>(tangentBundle.pointer) + vertexCount;
	
	const Vector4D *tangentArray = geometryLevel->GetArray<Vector4D>(kArrayTangent);
	Vector4D *clothTangent = static_cast<Vector4D *>(tangentBundle.pointer);
	for (machine a = 0; a < vertexCount; a++) clothTangent[a].w = tangentArray[a].w;
	
	if (clothGeometry->GetManipulator())
	{
		delete[] fieldStorage;
		fieldStorage = nullptr;
	}
	
	if (!fieldStorage)
	{
		AllocateFieldStorage();
		
		geometryLevel = object->GetGeometryLevel(object->GetGeometryLevelCount() - 1);
		const Point3D *vertexArray = geometryLevel->GetArray<Point3D>(kArrayVertex);
		
		Point3D *position = clothPosition[0];
		Point3D *previous = clothPosition[1];
		
		for (machine j = 0; j < height; j++)
		{
			for (machine i = 0; i < width; i++)
			{
				*position = *vertexArray;
				*previous = *vertexArray;
				
				vertexArray++;
				position++;
				previous++;
			}
		}
	}
	
	if (!springStorage)
	{
		connectCount = (width - 1) * height + width * (height - 1);
		shearCount = 2 * (width - 1) * (height - 1);
		bendCount = connectCount - width - height;
		AllocateSpringStorage();
		
		SpringData *springData = connectSpring;
		for (machine j = 0; j < height; j++)
		{
			int32 base = j * width;
			for (machine i = 1; i < width; i++)
			{
				springData->massIndex1 = (unsigned_int16) (base + i - 1);
				springData->massIndex2 = (unsigned_int16) (base + i);
				springData++;
			}
		}
		
		for (machine i = 0; i < width; i++)
		{
			for (machine j = 1; j < height; j++)
			{
				springData->massIndex1 = (unsigned_int16) (i + (j - 1) * width);
				springData->massIndex2 = (unsigned_int16) (i + j * width);
				springData++;
			}
		}
		
		springData = shearSpring;
		for (machine j = 1; j < height; j++)
		{
			int32 base = j * width;
			for (machine i = 1; i < width; i++)
			{
				springData->massIndex1 = (unsigned_int16) (base - width + i - 1);
				springData->massIndex2 = (unsigned_int16) (base + i);
				springData++;
				
				springData->massIndex1 = (unsigned_int16) (base - width + i);
				springData->massIndex2 = (unsigned_int16) (base + i - 1);
				springData++;
			}
		}
		
		springData = bendSpring;
		for (machine j = 0; j < height; j++)
		{
			int32 base = j * width;
			for (machine i = 2; i < width; i++)
			{
				springData->massIndex1 = (unsigned_int16) (base + i - 2);
				springData->massIndex2 = (unsigned_int16) (base + i);
				springData++;
			}
		}
		
		for (machine i = 0; i < width; i++)
		{
			for (machine j = 2; j < height; j++)
			{
				springData->massIndex1 = (unsigned_int16) (i + (j - 2) * width);
				springData->massIndex2 = (unsigned_int16) (i + j * width);
				springData++;
			}
		}
	}
	
	clothTime = 0;
	updateTime = kClothUpdateInterval;
}

void ClothController::Neutralize(void)
{
	dynamicVertexBuffer.Deactivate();
	
	delete[] vertexStorage;
	vertexStorage = nullptr;
	
	delete[] springStorage;
	springStorage = nullptr;
	
	delete[] fieldStorage;
	fieldStorage = nullptr;
	
	Controller::Neutralize();
}

void ClothController::Move(void)
{
	int32 dt = TheTimeMgr->GetDeltaTime();
	
	int32 time = clothTime + dt;
	int32 passCount = time / kClothTimeStep;
	clothTime = time - passCount * kClothTimeStep;
	
	time = updateTime + dt;
	if (time < kClothUpdateInterval)
	{
		updateTime = time;
		if (passCount > 0)
		{
			passCount = Min(passCount, 3);
			
			ClothGeometry *clothGeometry = GetTargetNode();
			clothGeometry->GetWorld()->IncrementWorldCounter(kWorldCounterClothMove);
			
			Vector3D gravity = CalculateGravityForce();
			Vector3D wind = CalculateWindForce();
			
			Point3D *restrict position = clothPosition[0];
			Point3D *restrict previous = clothPosition[1];
			
			#if C4SIMD
			
				float4 *restrict force = clothForce;
				
				register const float4 one = SimdLoadConstant<0x3F800000>();
				do
				{
					float4 gravityForce = SimdLoadUnaligned(&gravity.x);
					float4 windForce = SimdLoadUnaligned(&wind.x);
					float4 inverseTime = SimdLoadSmearScalar(&kClothInverseDeltaTime);
					float4 mu = SimdLoadSmearScalar(&viscosityConstant);
					
					for (machine a = 0; a < massCount; a++)
					{
						float4 velocity = SimdMul(SimdSub(SimdLoadUnaligned(&position[a].x), SimdLoadUnaligned(&previous[a].x)), inverseTime);
						force[a] = SimdMadd(SimdSub(windForce, velocity), mu, gravityForce);
					}
					
					float4 k = SimdLoadSmearScalar(&connectConstant);
					float4 d = SimdLoadSmearScalar(&connectDistance);
					
					const SpringData *springData = connectSpring;
					for (machine a = 0; a < connectCount; a++)
					{
						int32 index1 = springData->massIndex1;
						int32 index2 = springData->massIndex2;
						
						float4 p1 = SimdLoadUnaligned(&position[index1].x);
						float4 p2 = SimdLoadUnaligned(&position[index2].x);
						
						float4 dp = SimdSub(p2, p1);
						float4 f = SimdMul(dp, SimdMul(k, (SimdSub(one, SimdMul(d, SimdSmearX(SimdInverseSqrtScalar(SimdDot3D(dp, dp))))))));
						
						force[index1] = SimdAdd(force[index1], f);
						force[index2] = SimdSub(force[index2], f);
						
						springData++;
					}
					
					k = SimdLoadSmearScalar(&shearConstant);
					d = SimdLoadSmearScalar(&shearDistance);
					
					springData = shearSpring;
					for (machine a = 0; a < shearCount; a++)
					{
						int32 index1 = springData->massIndex1;
						int32 index2 = springData->massIndex2;
						
						float4 p1 = SimdLoadUnaligned(&position[index1].x);
						float4 p2 = SimdLoadUnaligned(&position[index2].x);
						
						float4 dp = SimdSub(p2, p1);
						float4 f = SimdMul(dp, SimdMul(k, (SimdSub(one, SimdMul(d, SimdSmearX(SimdInverseSqrtScalar(SimdDot3D(dp, dp))))))));
						
						force[index1] = SimdAdd(force[index1], f);
						force[index2] = SimdSub(force[index2], f);
						
						springData++;
					}
					
					k = SimdLoadSmearScalar(&bendConstant);
					d = SimdLoadSmearScalar(&bendDistance);
					
					springData = bendSpring;
					for (machine a = 0; a < bendCount; a++)
					{
						int32 index1 = springData->massIndex1;
						int32 index2 = springData->massIndex2;
						
						float4 p1 = SimdLoadUnaligned(&position[index1].x);
						float4 p2 = SimdLoadUnaligned(&position[index2].x);
						
						float4 dp = SimdSub(p2, p1);
						float4 f = SimdMul(dp, SimdMul(k, (SimdSub(one, SimdMul(d, SimdSmearX(SimdInverseSqrtScalar(SimdDot3D(dp, dp))))))));
						
						force[index1] = SimdAdd(force[index1], f);
						force[index2] = SimdSub(force[index2], f);
						
						springData++;
					}
					
					float4 squaredTime = SimdLoadSmearScalar(&kClothSquaredDeltaTime);
					
					const float *flexibility = clothGeometry->GetObject()->GetClothFlexibility();
					for (machine a = 0; a < massCount; a++)
					{
						float4 t2 = SimdMul(SimdLoadSmearScalar(&flexibility[a]), squaredTime);
						float4 p = SimdLoadUnaligned(&position[a].x);
						float4 q = SimdMadd(force[a], t2, SimdAdd(p, SimdSub(p, SimdLoadUnaligned(&previous[a].x))));
						SimdStore3D(p, &previous[a].x);
						SimdStore3D(q, &position[a].x);
					}
				} while (--passCount > 0);
			
			#else
			
				Vector3D *restrict force = clothForce;
				
				float mu = viscosityConstant;
				do
				{
					for (machine a = 0; a < massCount; a++)
					{
						Vector3D velocity = (position[a] - previous[a]) * kClothInverseDeltaTime;
						force[a] = (wind - velocity) * mu + gravity;
					}
					
					float k = connectConstant;
					float d = connectDistance;
					
					const SpringData *springData = connectSpring;
					for (machine a = 0; a < connectCount; a++)
					{
						int32 index1 = springData->massIndex1;
						int32 index2 = springData->massIndex2;
						
						const Point3D& p1 = position[index1];
						const Point3D& p2 = position[index2];
						
						Vector3D dp = p2 - p1;
						Vector3D f = dp * (k * (1.0F - d * InverseMag(dp)));
						
						force[index1] += f;
						force[index2] -= f;
						
						springData++;
					}
					
					k = shearConstant;
					d = shearDistance;
					
					springData = shearSpring;
					for (machine a = 0; a < shearCount; a++)
					{
						int32 index1 = springData->massIndex1;
						int32 index2 = springData->massIndex2;
						
						const Point3D& p1 = position[index1];
						const Point3D& p2 = position[index2];
						
						Vector3D dp = p2 - p1;
						Vector3D f = dp * (k * (1.0F - d * InverseMag(dp)));
						
						force[index1] += f;
						force[index2] -= f;
						
						springData++;
					}
					
					k = bendConstant;
					d = bendDistance;
					
					springData = bendSpring;
					for (machine a = 0; a < bendCount; a++)
					{
						int32 index1 = springData->massIndex1;
						int32 index2 = springData->massIndex2;
						
						const Point3D& p1 = position[index1];
						const Point3D& p2 = position[index2];
						
						Vector3D dp = p2 - p1;
						Vector3D f = dp * (k * (1.0F - d * InverseMag(dp)));
						
						force[index1] += f;
						force[index2] -= f;
						
						springData++;
					}
					
					const float *flexibility = clothGeometry->GetObject()->GetClothFlexibility();
					for (machine a = 0; a < massCount; a++)
					{
						float t2 = flexibility[a] * kClothSquaredDeltaTime;
						Point3D q = Zero3D + (position[a] * 2.0F - previous[a] + force[a] * t2);
						previous[a] = position[a];
						position[a] = q;
					}
				} while (--passCount > 0);
			
			#endif
			
			Invalidate();
		}
	}
	else
	{
		updateTime = kClothUpdateInterval;
		Invalidate();
	}
}

Vector3D ClothController::CalculateGravityForce(void) const
{
	if (physicsController) return (GetTargetNode()->GetInverseWorldTransform() * physicsController->GetGravityAcceleration() * (gravityMultiplier * 1.0e-6F));
	return (Zero3D);
}

void ClothController::ApplyCellWind(Site *site, Vector3D& wind, unsigned_int32 fieldStamp) const
{
	const ClothGeometry *clothGeometry = GetTargetNode();
	
	const Bond *bond = site->GetFirstOutgoingEdge();
	while (bond)
	{
		Site *site = bond->GetFinishElement();
		if (site->GetWorldBoundingBox().Intersection(clothGeometry->GetWorldBoundingBox()))
		{
			if (site->GetCellIndex() < 0)
			{
				Field *field = static_cast<Field *>(site);
				
				unsigned_int32 stamp = fieldStamp;
				if (field->GetNodeStamp() != stamp)
				{
					field->SetNodeStamp(stamp);
					
					if (field->Enabled())
					{
						const Force *force = field->GetForce();
						if ((force) && (force->GetForceType() == kForceWind))
						{
							const BoundingSphere *sphere = clothGeometry->GetBoundingSphere();
							if (!field->GetObject()->ExteriorSphere(field->GetInverseWorldTransform() * sphere->GetCenter(), sphere->GetRadius()))
							{
								const WindForce *windForce = static_cast<const WindForce *>(force);
								wind += clothGeometry->GetInverseWorldTransform() * (field->GetWorldTransform() * windForce->GetWindVelocity());
							}
						}
					}
				}
			}
			else
			{
				ApplyCellWind(site, wind, fieldStamp);
			}
		}
		
		bond = bond->GetNextOutgoingEdge();
	}
}

Vector3D ClothController::CalculateWindForce(void) const
{
	Vector3D	wind;
	
	const ClothGeometry *clothGeometry = GetTargetNode();
	
	const Field *field = windForceField;
	if ((field) && (field->Enabled()))
	{
		const Transform4D& inverseTransform = clothGeometry->GetInverseWorldTransform();
		wind = inverseTransform * (field->GetWorldTransform() * static_cast<WindForce *>(field->GetForce())->GetWindVelocity());
	}
	else
	{
		wind.Set(0.0F, 0.0F, 0.0F);
	}
	
	if (physicsController)
	{
		unsigned_int32 fieldStamp = physicsController->IncrementFieldStamp();
		ApplyCellWind(clothGeometry->GetOwningZone()->GetFieldSite(), wind, fieldStamp);
	}
	
	return (wind * 0.001F);
}

void ClothController::Update(void)
{
	Controller::Update();
	updateTime = 0;
	
	World *world = GetTargetNode()->GetWorld();
	world->IncrementWorldCounter(kWorldCounterClothUpdate);
	world->SubmitWorldJob(&clothUpdateJob);
}

void ClothController::ClothUpdateJob(Job *job, void *cookie)
{
	ClothController *controller = static_cast<ClothController *>(cookie);
	
	const ClothGeometry *cloth = controller->GetTargetNode();
	const ClothGeometryObject *object = cloth->GetObject();
	
	int32 level = cloth->GetDetailLevel();
	const GeometryLevel *geometryLevel = object->GetGeometryLevel(level);
	int32 vertexCount = geometryLevel->GetVertexCount() / 2;
	
	Point3D *restrict vertexArray = static_cast<Point3D *>(controller->vertexBundle.pointer);
	Vector3D *restrict velocityArray = static_cast<Vector3D *>(controller->velocityBundle.pointer);
	Vector3D *restrict normalArray = static_cast<Vector3D *>(controller->normalBundle.pointer);
	Vector4D *restrict tangentArray = static_cast<Vector4D *>(controller->tangentBundle.pointer);
	
	int32 width = object->GetFieldWidth();
	int32 height = object->GetFieldHeight();
	const Point3D *position = controller->clothPosition[0];
	const Point3D *previous = controller->clothPosition[1];
	
	if (level < object->GetGeometryLevelCount() - 1)
	{
		int32 row = width * 2 - 1;
		Vector3D *restrict bitangent = controller->clothBitangent;
		
		#if C4SIMD
		
			float4 inverseTime = SimdLoadSmearScalar(&kClothInverseDeltaTime);
			register const float4 half = SimdLoadConstant<0x3F000000>();
			
			float4 p = SimdLoadUnaligned(&position[0].x);
			SimdStore3D(p, &vertexArray[0].x);
			SimdStore3D(SimdMul(SimdSub(p, SimdLoadUnaligned(&previous[0].x)), inverseTime), &velocityArray[0].x);
			float4 tang = SimdSub(SimdLoadUnaligned(&position[1].x), p);
			float4 btng = SimdSub(SimdLoadUnaligned(&position[width].x), p);
			SimdStore3D(tang, &tangentArray[0].x);
			SimdStore3D(btng, &bitangent[0].x);
			SimdStore3D(SimdCross3D(tang, btng), &normalArray[0].x);
			
			position++;
			previous++;
			bitangent++;
			int32 index = 2;
			
			for (machine i = 1; i < width - 1; i++)
			{
				p = SimdLoadUnaligned(&position[0].x);
				SimdStore3D(p, &vertexArray[index].x);
				SimdStore3D(SimdMul(SimdSub(p, SimdLoadUnaligned(&previous[0].x)), inverseTime), &velocityArray[index].x);
				tang = SimdMul(SimdSub(SimdLoadUnaligned(&position[1].x), SimdLoadUnaligned(&position[-1].x)), half);
				btng = SimdSub(SimdLoadUnaligned(&position[width].x), p);
				SimdStore3D(tang, &tangentArray[index].x);
				SimdStore3D(btng, &bitangent[0].x);
				SimdStore3D(SimdCross3D(tang, btng), &normalArray[index].x);
				
				position++;
				previous++;
				bitangent++;
				index += 2;
			}
			
			p = SimdLoadUnaligned(&position[0].x);
			SimdStore3D(p, &vertexArray[index].x);
			SimdStore3D(SimdMul(SimdSub(p, SimdLoadUnaligned(&previous[0].x)), inverseTime), &velocityArray[index].x);
			tang = SimdSub(p, SimdLoadUnaligned(&position[-1].x));
			btng = SimdSub(SimdLoadUnaligned(&position[width].x), p);
			SimdStore3D(tang, &tangentArray[index].x);
			SimdStore3D(btng, &bitangent[0].x);
			SimdStore3D(SimdCross3D(tang, btng), &normalArray[index].x);
			
			position++;
			previous++;
			bitangent++;
			index += row + 1;
			
			for (machine j = 1; j < height - 1; j++)
			{
				p = SimdLoadUnaligned(&position[0].x);
				SimdStore3D(p, &vertexArray[index].x);
				SimdStore3D(SimdMul(SimdSub(p, SimdLoadUnaligned(&previous[0].x)), inverseTime), &velocityArray[index].x);
				tang = SimdSub(SimdLoadUnaligned(&position[1].x), p);
				btng = SimdMul(SimdSub(SimdLoadUnaligned(&position[width].x), SimdLoadUnaligned(&position[-width].x)), half);
				SimdStore3D(tang, &tangentArray[index].x);
				SimdStore3D(btng, &bitangent[0].x);
				SimdStore3D(SimdCross3D(tang, btng), &normalArray[index].x);
				
				position++;
				previous++;
				bitangent++;
				index += 2;
				
				for (machine i = 1; i < width - 1; i++)
				{
					p = SimdLoadUnaligned(&position[0].x);
					SimdStore3D(p, &vertexArray[index].x);
					SimdStore3D(SimdMul(SimdSub(p, SimdLoadUnaligned(&previous[0].x)), inverseTime), &velocityArray[index].x);
					tang = SimdMul(SimdSub(SimdLoadUnaligned(&position[1].x), SimdLoadUnaligned(&position[-1].x)), half);
					btng = SimdMul(SimdSub(SimdLoadUnaligned(&position[width].x), SimdLoadUnaligned(&position[-width].x)), half);
					SimdStore3D(tang, &tangentArray[index].x);
					SimdStore3D(btng, &bitangent[0].x);
					SimdStore3D(SimdCross3D(tang, btng), &normalArray[index].x);
					
					position++;
					previous++;
					bitangent++;
					index += 2;
				}
				
				p = SimdLoadUnaligned(&position[0].x);
				SimdStore3D(p, &vertexArray[index].x);
				SimdStore3D(SimdMul(SimdSub(p, SimdLoadUnaligned(&previous[0].x)), inverseTime), &velocityArray[index].x);
				tang = SimdSub(p, SimdLoadUnaligned(&position[-1].x));
				btng = SimdMul(SimdSub(SimdLoadUnaligned(&position[width].x), SimdLoadUnaligned(&position[-width].x)), half);
				SimdStore3D(tang, &tangentArray[index].x);
				SimdStore3D(btng, &bitangent[0].x);
				SimdStore3D(SimdCross3D(tang, btng), &normalArray[index].x);
				
				position++;
				previous++;
				bitangent++;
				index += row + 1;
			}
			
			p = SimdLoadUnaligned(&position[0].x);
			SimdStore3D(p, &vertexArray[index].x);
			SimdStore3D(SimdMul(SimdSub(p, SimdLoadUnaligned(&previous[0].x)), inverseTime), &velocityArray[index].x);
			tang = SimdSub(SimdLoadUnaligned(&position[1].x), p);
			btng = SimdSub(p, SimdLoadUnaligned(&position[-width].x));
			SimdStore3D(tang, &tangentArray[index].x);
			SimdStore3D(btng, &bitangent[0].x);
			SimdStore3D(SimdCross3D(tang, btng), &normalArray[index].x);
			
			position++;
			previous++;
			bitangent++;
			index += 2;
			
			for (machine i = 1; i < width - 1; i++)
			{
				p = SimdLoadUnaligned(&position[0].x);
				SimdStore3D(p, &vertexArray[index].x);
				SimdStore3D(SimdMul(SimdSub(p, SimdLoadUnaligned(&previous[0].x)), inverseTime), &velocityArray[index].x);
				tang = SimdMul(SimdSub(SimdLoadUnaligned(&position[1].x), SimdLoadUnaligned(&position[-1].x)), half);
				btng = SimdSub(p, SimdLoadUnaligned(&position[-width].x));
				SimdStore3D(tang, &tangentArray[index].x);
				SimdStore3D(btng, &bitangent[0].x);
				SimdStore3D(SimdCross3D(tang, btng), &normalArray[index].x);
				
				position++;
				previous++;
				bitangent++;
				index += 2;
			}
			
			p = SimdLoadUnaligned(&position[0].x);
			SimdStore3D(p, &vertexArray[index].x);
			SimdStore3D(SimdMul(SimdSub(p, SimdLoadUnaligned(&previous[0].x)), inverseTime), &velocityArray[index].x);
			tang = SimdSub(p, SimdLoadUnaligned(&position[-1].x));
			btng = SimdSub(p, SimdLoadUnaligned(&position[-width].x));
			SimdStore3D(tang, &tangentArray[index].x);
			SimdStore3D(btng, &bitangent[0].x);
			SimdStore3D(SimdCross3D(tang, btng), &normalArray[index].x);
			
			register const float4 quarter = SimdLoadConstant<0x3E800000>();
			register const float4 eighth = SimdLoadConstant<0x3E000000>();
			register const float4 three_halves = SimdLoadConstant<0x3FC00000>();
			
			for (machine j = 0; j < height; j++)
			{
				int32 k = j * row * 2 + 1;
				for (machine i = 1; i < width; i++)
				{
					float4 p1 = SimdLoadUnaligned(&vertexArray[k - 1].x);
					float4 p2 = SimdLoadUnaligned(&vertexArray[k + 1].x);
					float4 v1 = SimdLoadUnaligned(&velocityArray[k - 1].x);
					float4 v2 = SimdLoadUnaligned(&velocityArray[k + 1].x);
					float4 t1 = SimdLoadUnaligned(&tangentArray[k - 1].x);
					float4 t2 = SimdLoadUnaligned(&tangentArray[k + 1].x);
					
					SimdStore3D(SimdMadd(SimdAdd(p1, p2), half, SimdMul(SimdSub(t1, t2), eighth)), &vertexArray[k].x);
					SimdStore3D(SimdMul(SimdAdd(v1, v2), half), &velocityArray[k].x);
					SimdStore3D(SimdNmsub(SimdAdd(t1, t2), quarter, SimdMul(SimdSub(p2, p1), three_halves)), &tangentArray[k].x);
					k += 2;
				}
			}
			
			bitangent = controller->clothBitangent;
			Vector4D *restrict const& bitangentArray = tangentArray;
			
			for (machine i = 0; i < width; i++)
			{
				int32 k = i * 2 + row;
				for (machine j = 1; j < height; j++)
				{
					float4 p1 = SimdLoadUnaligned(&vertexArray[k - row].x);
					float4 p2 = SimdLoadUnaligned(&vertexArray[k + row].x);
					float4 v1 = SimdLoadUnaligned(&velocityArray[k - row].x);
					float4 v2 = SimdLoadUnaligned(&velocityArray[k + row].x);
					float4 b1 = SimdLoadUnaligned(&bitangent[(j - 1) * width + i].x);
					float4 b2 = SimdLoadUnaligned(&bitangent[j * width + i].x);
					
					SimdStore3D(SimdMadd(SimdAdd(p1, p2), half, SimdMul(SimdSub(b1, b2), eighth)), &vertexArray[k].x);
					SimdStore3D(SimdMul(SimdAdd(v1, v2), half), &velocityArray[k].x);
					SimdStore3D(SimdNmsub(SimdAdd(b1, b2), quarter, SimdMul(SimdSub(p2, p1), three_halves)), &bitangentArray[k].x);
					k += row * 2;
				}
			}
			
			int32 m = 1;
			for (machine i = 1; i < width; i++)
			{
				float4 p1 = SimdLoadUnaligned(&vertexArray[m].x);
				float4 p2 = SimdLoadUnaligned(&vertexArray[m + row * 2].x);
				float4 t = SimdLoadUnaligned(&tangentArray[m].x);
				
				SimdStore3D(SimdCross3D(t, SimdSub(p2, p1)), &normalArray[m].x);
				m += 2;
			}
			
			for (machine j = 1; j < height - 1; j++)
			{
				int32 k = j * row * 2 + 1;
				for (machine i = 1; i < width; i++)
				{
					float4 p1 = SimdLoadUnaligned(&vertexArray[k - row * 2].x);
					float4 p2 = SimdLoadUnaligned(&vertexArray[k + row * 2].x);
					float4 t = SimdLoadUnaligned(&tangentArray[k].x);
					
					SimdStore3D(SimdCross3D(t, SimdSub(p2, p1)), &normalArray[k].x);
					k += 2;
				}
			}
			
			m = (height - 1) * row * 2 + 1;
			for (machine i = 1; i < width; i++)
			{
				float4 p1 = SimdLoadUnaligned(&vertexArray[m - row * 2].x);
				float4 p2 = SimdLoadUnaligned(&vertexArray[m].x);
				float4 t = SimdLoadUnaligned(&tangentArray[m].x);
				
				SimdStore3D(SimdCross3D(t, SimdSub(p2, p1)), &normalArray[m].x);
				m += 2;
			}
			
			m = row;
			for (machine j = 1; j < height; j++)
			{
				float4 p1 = SimdLoadUnaligned(&vertexArray[m].x);
				float4 p2 = SimdLoadUnaligned(&vertexArray[m + 2].x);
				float4 b = SimdLoadUnaligned(&bitangentArray[m].x);
				float4 t = SimdSub(p2, p1);
				
				SimdStore3D(SimdCross3D(t, b), &normalArray[m].x);
				SimdStore3D(t, &tangentArray[m].x);
				m += row * 2;
			}
			
			for (machine i = 1; i < width - 1; i++)
			{
				int32 k = i * 2 + row;
				for (machine j = 1; j < height; j++)
				{
					float4 p1 = SimdLoadUnaligned(&vertexArray[k - 2].x);
					float4 p2 = SimdLoadUnaligned(&vertexArray[k + 2].x);
					float4 b = SimdLoadUnaligned(&bitangentArray[k].x);
					float4 t = SimdMul(SimdSub(p2, p1), half);
					
					SimdStore3D(SimdCross3D(t, b), &normalArray[k].x);
					SimdStore3D(t, &tangentArray[k].x);
					k += row * 2;
				}
			}
			
			m = (width - 1) * 2 + row;
			for (machine j = 1; j < height; j++)
			{
				float4 p1 = SimdLoadUnaligned(&vertexArray[m - 2].x);
				float4 p2 = SimdLoadUnaligned(&vertexArray[m].x);
				float4 b = SimdLoadUnaligned(&bitangentArray[m].x);
				float4 t = SimdSub(p2, p1);
				
				SimdStore3D(SimdCross3D(t, b), &normalArray[m].x);
				SimdStore3D(t, &tangentArray[m].x);
				m += row * 2;
			}
			
			for (machine j = 1; j < height; j++)
			{
				int32 k = (j * 2 - 1) * row + 1;
				for (machine i = 1; i < width; i++)
				{
					float4 p1 = SimdLoadUnaligned(&vertexArray[k - 1].x);
					float4 p2 = SimdLoadUnaligned(&vertexArray[k + 1].x);
					float4 v1 = SimdLoadUnaligned(&velocityArray[k - 1].x);
					float4 v2 = SimdLoadUnaligned(&velocityArray[k + 1].x);
					float4 t1 = SimdLoadUnaligned(&tangentArray[k - 1].x);
					float4 t2 = SimdLoadUnaligned(&tangentArray[k + 1].x);
					
					SimdStore3D(SimdMadd(SimdAdd(p1, p2), half, SimdMul(SimdSub(t1, t2), eighth)), &vertexArray[k].x);
					SimdStore3D(SimdMul(SimdAdd(v1, v2), half), &velocityArray[k].x);
					float4 tang = SimdNmsub(SimdAdd(t1, t2), quarter, SimdMul(SimdSub(p2, p1), three_halves));
					SimdStore3D(tang, &tangentArray[k].x);
					SimdStore3D(SimdCross3D(tang, SimdSub(SimdLoadUnaligned(&vertexArray[k + row].x), SimdLoadUnaligned(&vertexArray[k - row].x))), &normalArray[k].x);
					k += 2;
				}
			}
		
		#else
		
			vertexArray[0] = position[0];
			velocityArray[0] = (position[0] - previous[0]) * kClothInverseDeltaTime;
			tangentArray[0].GetVector3D() = position[1] - position[0];
			bitangent[0] = position[width] - position[0];
			normalArray[0] = tangentArray[0].GetVector3D() % bitangent[0];
			
			position++;
			previous++;
			bitangent++;
			int32 index = 2;
			
			for (machine i = 1; i < width - 1; i++)
			{
				vertexArray[index] = position[0];
				velocityArray[index] = (position[0] - previous[0]) * kClothInverseDeltaTime;
				tangentArray[index].GetVector3D() = (position[1] - position[-1]) * 0.5F;
				bitangent[0] = position[width] - position[0];
				normalArray[index] = tangentArray[index].GetVector3D() % bitangent[0];
				
				position++;
				previous++;
				bitangent++;
				index += 2;
			}
			
			vertexArray[index] = position[0];
			velocityArray[index] = (position[0] - previous[0]) * kClothInverseDeltaTime;
			tangentArray[index].GetVector3D() = position[0] - position[-1];
			bitangent[0] = position[width] - position[0];
			normalArray[index] = tangentArray[index].GetVector3D() % bitangent[0];
			
			position++;
			previous++;
			bitangent++;
			index += row + 1;
			
			for (machine j = 1; j < height - 1; j++)
			{
				vertexArray[index] = position[0];
				velocityArray[index] = (position[0] - previous[0]) * kClothInverseDeltaTime;
				tangentArray[index].GetVector3D() = position[1] - position[0];
				bitangent[0] = (position[width] - position[-width]) * 0.5F;
				normalArray[index] = tangentArray[index].GetVector3D() % bitangent[0];
				
				position++;
				previous++;
				bitangent++;
				index += 2;
				
				for (machine i = 1; i < width - 1; i++)
				{
					vertexArray[index] = position[0];
					velocityArray[index] = (position[0] - previous[0]) * kClothInverseDeltaTime;
					tangentArray[index].GetVector3D() = (position[1] - position[-1]) * 0.5F;
					bitangent[0] = (position[width] - position[-width]) * 0.5F;
					normalArray[index] = tangentArray[index].GetVector3D() % bitangent[0];
					
					position++;
					previous++;
					bitangent++;
					index += 2;
				}
				
				vertexArray[index] = position[0];
				velocityArray[index] = (position[0] - previous[0]) * kClothInverseDeltaTime;
				tangentArray[index].GetVector3D() = position[0] - position[-1];
				bitangent[0] = (position[width] - position[-width]) * 0.5F;
				normalArray[index] = tangentArray[index].GetVector3D() % bitangent[0];
				
				position++;
				previous++;
				bitangent++;
				index += row + 1;
			}
			
			vertexArray[index] = position[0];
			velocityArray[index] = (position[0] - previous[0]) * kClothInverseDeltaTime;
			tangentArray[index].GetVector3D() = position[1] - position[0];
			bitangent[0] = position[0] - position[-width];
			normalArray[index] = tangentArray[index].GetVector3D() % bitangent[0];
			
			position++;
			previous++;
			bitangent++;
			index += 2;
			
			for (machine i = 1; i < width - 1; i++)
			{
				vertexArray[index] = position[0];
				velocityArray[index] = (position[0] - previous[0]) * kClothInverseDeltaTime;
				tangentArray[index].GetVector3D() = (position[1] - position[-1]) * 0.5F;
				bitangent[0] = position[0] - position[-width];
				normalArray[index] = tangentArray[index].GetVector3D() % bitangent[0];
				
				position++;
				previous++;
				bitangent++;
				index += 2;
			}
			
			vertexArray[index] = position[0];
			velocityArray[index] = (position[0] - previous[0]) * kClothInverseDeltaTime;
			tangentArray[index].GetVector3D() = position[0] - position[-1];
			bitangent[0] = position[0] - position[-width];
			normalArray[index] = tangentArray[index].GetVector3D() % bitangent[0];
			
			for (machine j = 0; j < height; j++)
			{
				int32 k = j * row * 2 + 1;
				for (machine i = 1; i < width; i++)
				{
					const Point3D& p1 = vertexArray[k - 1];
					const Point3D& p2 = vertexArray[k + 1];
					const Vector3D& v1 = velocityArray[k - 1];
					const Vector3D& v2 = velocityArray[k + 1];
					const Vector3D& t1 = tangentArray[k - 1].GetVector3D();
					const Vector3D& t2 = tangentArray[k + 1].GetVector3D();
					
					vertexArray[k] = (p1 + p2) * 0.5F + (t1 - t2) * 0.125F;
					velocityArray[k] = (v1 + v2) * 0.5F;
					tangentArray[k].GetVector3D() = (p2 - p1) * 1.5F - (t1 + t2) * 0.25F;
					k += 2;
				}
			}
			
			bitangent = controller->clothBitangent;
			Vector4D *restrict const& bitangentArray = tangentArray;
			
			for (machine i = 0; i < width; i++)
			{
				int32 k = i * 2 + row;
				for (machine j = 1; j < height; j++)
				{
					const Point3D& p1 = vertexArray[k - row];
					const Point3D& p2 = vertexArray[k + row];
					const Vector3D& v1 = velocityArray[k - row];
					const Vector3D& v2 = velocityArray[k + row];
					const Vector3D& b1 = bitangent[(j - 1) * width + i];
					const Vector3D& b2 = bitangent[j * width + i];
					
					vertexArray[k] = (p1 + p2) * 0.5F + (b1 - b2) * 0.125F;
					velocityArray[k] = (v1 + v2) * 0.5F;
					bitangentArray[k].GetVector3D() = (p2 - p1) * 1.5F - (b1 + b2) * 0.25F;
					k += row * 2;
				}
			}
			
			int32 m = 1;
			for (machine i = 1; i < width; i++)
			{
				const Point3D& p1 = vertexArray[m];
				const Point3D& p2 = vertexArray[m + row * 2];
				
				normalArray[m] = tangentArray[m].GetVector3D() % (p2 - p1);
				m += 2;
			}
			
			for (machine j = 1; j < height - 1; j++)
			{
				int32 k = j * row * 2 + 1;
				for (machine i = 1; i < width; i++)
				{
					const Point3D& p1 = vertexArray[k - row * 2];
					const Point3D& p2 = vertexArray[k + row * 2];
					
					normalArray[k] = tangentArray[k].GetVector3D() % (p2 - p1);
					k += 2;
				}
			}
			
			m = (height - 1) * row * 2 + 1;
			for (machine i = 1; i < width; i++)
			{
				const Point3D& p1 = vertexArray[m - row * 2];
				const Point3D& p2 = vertexArray[m];
				
				normalArray[m] = tangentArray[m].GetVector3D() % (p2 - p1);
				m += 2;
			}
			
			m = row;
			for (machine j = 1; j < height; j++)
			{
				const Point3D& p1 = vertexArray[m];
				const Point3D& p2 = vertexArray[m + 2];
				
				Vector3D t = p2 - p1;
				normalArray[m] = t % bitangentArray[m].GetVector3D();
				tangentArray[m].GetVector3D() = t;
				m += row * 2;
			}
			
			for (machine i = 1; i < width - 1; i++)
			{
				int32 k = i * 2 + row;
				for (machine j = 1; j < height; j++)
				{
					const Point3D& p1 = vertexArray[k - 2];
					const Point3D& p2 = vertexArray[k + 2];
					
					Vector3D t = (p2 - p1) * 0.5F;
					normalArray[k] = t % bitangentArray[k].GetVector3D();
					tangentArray[k].GetVector3D() = t;
					k += row * 2;
				}
			}
			
			m = (width - 1) * 2 + row;
			for (machine j = 1; j < height; j++)
			{
				const Point3D& p1 = vertexArray[m - 2];
				const Point3D& p2 = vertexArray[m];
				
				Vector3D t = p2 - p1;
				normalArray[m] = t % bitangentArray[m].GetVector3D();
				tangentArray[m].GetVector3D() = t;
				m += row * 2;
			}
			
			for (machine j = 1; j < height; j++)
			{
				int32 k = (j * 2 - 1) * row + 1;
				for (machine i = 1; i < width; i++)
				{
					const Point3D& p1 = vertexArray[k - 1];
					const Point3D& p2 = vertexArray[k + 1];
					const Vector3D& v1 = velocityArray[k - 1];
					const Vector3D& v2 = velocityArray[k + 1];
					const Vector3D& t1 = tangentArray[k - 1].GetVector3D();
					const Vector3D& t2 = tangentArray[k + 1].GetVector3D();
					
					vertexArray[k] = (p1 + p2) * 0.5F + (t1 - t2) * 0.125F;
					velocityArray[k] = (v1 + v2) * 0.5F;
					tangentArray[k].GetVector3D() = (p2 - p1) * 1.5F - (t1 + t2) * 0.25F;
					normalArray[k] = tangentArray[k].GetVector3D() % (vertexArray[k + row] - vertexArray[k - row]);
					k += 2;
				}
			}
		
		#endif
	}
	else
	{
		#if C4SIMD
		
			float4 inverseTime = SimdLoadSmearScalar(&kClothInverseDeltaTime);
			
			float4 p = SimdLoadUnaligned(&position[0].x);
			SimdStore3D(p, &vertexArray[0].x);
			SimdStore3D(SimdMul(SimdSub(p, SimdLoadUnaligned(&previous[0].x)), inverseTime), &velocityArray[0].x);
			float4 tang = SimdSub(SimdLoadUnaligned(&position[1].x), p);
			float4 btng = SimdSub(SimdLoadUnaligned(&position[width].x), p);
			SimdStore3D(tang, &tangentArray[0].x);
			SimdStore3D(SimdCross3D(tang, btng), &normalArray[0].x);
			
			position++;
			previous++;
			int32 index = 1;
			
			for (machine i = 1; i < width - 1; i++)
			{
				p = SimdLoadUnaligned(&position[0].x);
				SimdStore3D(p, &vertexArray[index].x);
				SimdStore3D(SimdMul(SimdSub(p, SimdLoadUnaligned(&previous[0].x)), inverseTime), &velocityArray[index].x);
				tang = SimdSub(SimdLoadUnaligned(&position[1].x), SimdLoadUnaligned(&position[-1].x));
				btng = SimdSub(SimdLoadUnaligned(&position[width].x), p);
				SimdStore3D(tang, &tangentArray[index].x);
				SimdStore3D(SimdCross3D(tang, btng), &normalArray[index].x);
				
				position++;
				previous++;
				index++;
			}
			
			p = SimdLoadUnaligned(&position[0].x);
			SimdStore3D(p, &vertexArray[index].x);
			SimdStore3D(SimdMul(SimdSub(p, SimdLoadUnaligned(&previous[0].x)), inverseTime), &velocityArray[index].x);
			tang = SimdSub(p, SimdLoadUnaligned(&position[-1].x));
			btng = SimdSub(SimdLoadUnaligned(&position[width].x), p);
			SimdStore3D(tang, &tangentArray[index].x);
			SimdStore3D(SimdCross3D(tang, btng), &normalArray[index].x);
			
			position++;
			previous++;
			index++;
			
			for (machine j = 1; j < height - 1; j++)
			{
				p = SimdLoadUnaligned(&position[0].x);
				SimdStore3D(p, &vertexArray[index].x);
				SimdStore3D(SimdMul(SimdSub(p, SimdLoadUnaligned(&previous[0].x)), inverseTime), &velocityArray[index].x);
				tang = SimdSub(SimdLoadUnaligned(&position[1].x), p);
				btng = SimdSub(SimdLoadUnaligned(&position[width].x), SimdLoadUnaligned(&position[-width].x));
				SimdStore3D(tang, &tangentArray[index].x);
				SimdStore3D(SimdCross3D(tang, btng), &normalArray[index].x);
				
				position++;
				previous++;
				index++;
				
				for (machine i = 1; i < width - 1; i++)
				{
					p = SimdLoadUnaligned(&position[0].x);
					SimdStore3D(p, &vertexArray[index].x);
					SimdStore3D(SimdMul(SimdSub(p, SimdLoadUnaligned(&previous[0].x)), inverseTime), &velocityArray[index].x);
					tang = SimdSub(SimdLoadUnaligned(&position[1].x), SimdLoadUnaligned(&position[-1].x));
					btng = SimdSub(SimdLoadUnaligned(&position[width].x), SimdLoadUnaligned(&position[-width].x));
					SimdStore3D(tang, &tangentArray[index].x);
					SimdStore3D(SimdCross3D(tang, btng), &normalArray[index].x);
					
					position++;
					previous++;
					index++;
				}
				
				p = SimdLoadUnaligned(&position[0].x);
				SimdStore3D(p, &vertexArray[index].x);
				SimdStore3D(SimdMul(SimdSub(p, SimdLoadUnaligned(&previous[0].x)), inverseTime), &velocityArray[index].x);
				tang = SimdSub(p, SimdLoadUnaligned(&position[-1].x));
				btng = SimdSub(SimdLoadUnaligned(&position[width].x), SimdLoadUnaligned(&position[-width].x));
				SimdStore3D(tang, &tangentArray[index].x);
				SimdStore3D(SimdCross3D(tang, btng), &normalArray[index].x);
				
				position++;
				previous++;
				index++;
			}
			
			p = SimdLoadUnaligned(&position[0].x);
			SimdStore3D(p, &vertexArray[index].x);
			SimdStore3D(SimdMul(SimdSub(p, SimdLoadUnaligned(&previous[0].x)), inverseTime), &velocityArray[index].x);
			tang = SimdSub(SimdLoadUnaligned(&position[1].x), p);
			btng = SimdSub(p, SimdLoadUnaligned(&position[-width].x));
			SimdStore3D(tang, &tangentArray[index].x);
			SimdStore3D(SimdCross3D(tang, btng), &normalArray[index].x);
			
			position++;
			previous++;
			index++;
			
			for (machine i = 1; i < width - 1; i++)
			{
				p = SimdLoadUnaligned(&position[0].x);
				SimdStore3D(p, &vertexArray[index].x);
				SimdStore3D(SimdMul(SimdSub(p, SimdLoadUnaligned(&previous[0].x)), inverseTime), &velocityArray[index].x);
				tang = SimdSub(SimdLoadUnaligned(&position[1].x), SimdLoadUnaligned(&position[-1].x));
				btng = SimdSub(p, SimdLoadUnaligned(&position[-width].x));
				SimdStore3D(tang, &tangentArray[index].x);
				SimdStore3D(SimdCross3D(tang, btng), &normalArray[index].x);
				
				position++;
				previous++;
				index++;
			}
			
			p = SimdLoadUnaligned(&position[0].x);
			SimdStore3D(p, &vertexArray[index].x);
			SimdStore3D(SimdMul(SimdSub(p, SimdLoadUnaligned(&previous[0].x)), inverseTime), &velocityArray[index].x);
			tang = SimdSub(p, SimdLoadUnaligned(&position[-1].x));
			btng = SimdSub(p, SimdLoadUnaligned(&position[-width].x));
			SimdStore3D(tang, &tangentArray[index].x);
			SimdStore3D(SimdCross3D(tang, btng), &normalArray[index].x);
		
		#else
		
			vertexArray[0] = position[0];
			velocityArray[0] = (position[0] - previous[0]) * kClothInverseDeltaTime;
			tangentArray[0].GetVector3D() = position[1] - position[0];
			Vector3D bitangent = position[width] - position[0];
			normalArray[0] = tangentArray[0].GetVector3D() % bitangent;
			
			position++;
			previous++;
			int32 index = 1;
			
			for (machine i = 1; i < width - 1; i++)
			{
				vertexArray[index] = position[0];
				velocityArray[index] = (position[0] - previous[0]) * kClothInverseDeltaTime;
				tangentArray[index].GetVector3D() = position[1] - position[-1];
				bitangent = position[width] - position[0];
				normalArray[index] = tangentArray[index].GetVector3D() % bitangent;
				
				position++;
				previous++;
				index++;
			}
			
			vertexArray[index] = position[0];
			velocityArray[index] = (position[0] - previous[0]) * kClothInverseDeltaTime;
			tangentArray[index].GetVector3D() = position[0] - position[-1];
			bitangent = position[width] - position[0];
			normalArray[index] = tangentArray[index].GetVector3D() % bitangent;
			
			position++;
			previous++;
			index++;
			
			for (machine j = 1; j < height - 1; j++)
			{
				vertexArray[index] = position[0];
				velocityArray[index] = (position[0] - previous[0]) * kClothInverseDeltaTime;
				tangentArray[index].GetVector3D() = position[1] - position[0];
				bitangent = position[width] - position[-width];
				normalArray[index] = tangentArray[index].GetVector3D() % bitangent;
				
				position++;
				previous++;
				index++;
				
				for (machine i = 1; i < width - 1; i++)
				{
					vertexArray[index] = position[0];
					velocityArray[index] = (position[0] - previous[0]) * kClothInverseDeltaTime;
					tangentArray[index].GetVector3D() = position[1] - position[-1];
					bitangent = position[width] - position[-width];
					normalArray[index] = tangentArray[index].GetVector3D() % bitangent;
					
					position++;
					previous++;
					index++;
				}
				
				vertexArray[index] = position[0];
				velocityArray[index] = (position[0] - previous[0]) * kClothInverseDeltaTime;
				tangentArray[index].GetVector3D() = position[0] - position[-1];
				bitangent = position[width] - position[-width];
				normalArray[index] = tangentArray[index].GetVector3D() % bitangent;
				
				position++;
				previous++;
				index++;
			}
			
			vertexArray[index] = position[0];
			velocityArray[index] = (position[0] - previous[0]) * kClothInverseDeltaTime;
			tangentArray[index].GetVector3D() = position[1] - position[0];
			bitangent = position[0] - position[-width];
			normalArray[index] = tangentArray[index].GetVector3D() % bitangent;
			
			position++;
			previous++;
			index++;
			
			for (machine i = 1; i < width - 1; i++)
			{
				vertexArray[index] = position[0];
				velocityArray[index] = (position[0] - previous[0]) * kClothInverseDeltaTime;
				tangentArray[index].GetVector3D() = position[1] - position[-1];
				bitangent = position[0] - position[-width];
				normalArray[index] = tangentArray[index].GetVector3D() % bitangent;
				
				position++;
				previous++;
				index++;
			}
			
			vertexArray[index] = position[0];
			velocityArray[index] = (position[0] - previous[0]) * kClothInverseDeltaTime;
			tangentArray[index].GetVector3D() = position[0] - position[-1];
			bitangent = position[0] - position[-width];
			normalArray[index] = tangentArray[index].GetVector3D() % bitangent;
		
		#endif
	}
	
	for (machine a = 0; a < vertexCount; a++)
	{
		vertexArray[vertexCount] = *vertexArray;
		velocityArray[vertexCount] = *velocityArray;
		normalArray[vertexCount].Set(-normalArray->x, -normalArray->y, -normalArray->z);
		tangentArray[vertexCount].GetVector3D() = tangentArray->GetVector3D();
		
		vertexArray++;
		velocityArray++;
		normalArray++;
		tangentArray++;
	}
	
	if (!(object->GetGeometryFlags() & kGeometryShadowInhibit))
	{
		Antivector4D *restrict plane = static_cast<Antivector4D *>(controller->planeBundle.pointer);
		int32 planeCount = controller->planeBundle.descriptor.elementCount;
		
		const Point3D *vertex = static_cast<Point3D *>(controller->vertexBundle.pointer);
		const Triangle *triangle = geometryLevel->GetArray<Triangle>(kArrayFace);
		for (machine a = 0; a < planeCount; a++)
		{
			unsigned_int32 i1 = triangle->index[0];
			unsigned_int32 i2 = triangle->index[1];
			unsigned_int32 i3 = triangle->index[2];
			plane->Set(vertex[i1], vertex[i2], vertex[i3]);
			
			triangle++;
			plane++;
		}
	}
}

void ClothController::FinalizeUpdate(Job *job, void *cookie)
{
	ClothController *controller = static_cast<ClothController *>(cookie);
	if (controller->dynamicVertexBuffer.Active())
	{
		controller->dynamicVertexBuffer.SetObserver(&controller->dynamicVertexBufferObserver);
		controller->FillDynamicVertexBuffer(&controller->dynamicVertexBuffer);
	}
}

void ClothController::FillDynamicVertexBuffer(VertexBuffer *vertexBuffer)
{
	DynamicVertex *restrict buffer = static_cast<DynamicVertex *>(vertexBuffer->BeginUpdate());
	
	const Geometry *geometry = GetTargetNode();
	int32 vertexCount = geometry->GetVertexCount();
	
	const Point3D *vertex = geometry->GetAttributeArray<Point3D>(kArrayVertex);
	const Vector3D *velocity = geometry->GetAttributeArray<Vector3D>(kArrayVelocity);
	const Vector3D *normal = geometry->GetAttributeArray<Vector3D>(kArrayNormal);
	const Vector4D *tangent = geometry->GetAttributeArray<Vector4D>(kArrayTangent);
	
	for (machine a = 0; a < vertexCount; a++)
	{
		buffer[a].vertex = vertex[a];
		buffer[a].velocity = velocity[a];
		buffer[a].normal = normal[a];
		buffer[a].tangent = tangent[a];
	}
	
	vertexBuffer->EndUpdate();
}

void ClothController::SetDetailLevel(int32 level)
{
	Geometry *geometry = GetTargetNode();
	const GeometryObject *object = geometry->GetObject();
	
	const GeometryLevel *geometryLevel = object->GetGeometryLevel(level);
	int32 vertexCount = geometryLevel->GetVertexCount();
	int32 planeCount = geometryLevel->GetArrayBundle(kArrayPlane)->descriptor.elementCount;
	
	vertexBundle.descriptor.elementCount = vertexCount;
	velocityBundle.descriptor.elementCount = vertexCount;
	normalBundle.descriptor.elementCount = vertexCount;
	tangentBundle.descriptor.elementCount = vertexCount;
	planeBundle.descriptor.elementCount = planeCount;
	
	geometry->SetAttributeOffset(kArrayVertex, 0);
	geometry->SetAttributeOffset(kArrayVelocity, sizeof(Point3D));
	geometry->SetAttributeOffset(kArrayNormal, sizeof(Point3D) + sizeof(Vector3D));
	geometry->SetAttributeOffset(kArrayTangent, sizeof(Point3D) + sizeof(Vector3D) * 2);
	
	geometry->SetAttributeArray(kArrayVertex, static_cast<Point3D *>(vertexBundle.pointer));
	geometry->SetArrayBundle(kArrayVertex, &vertexBundle);
	
	geometry->SetAttributeArray(kArrayVelocity, static_cast<Vector3D *>(velocityBundle.pointer));
	geometry->SetArrayBundle(kArrayVelocity, &velocityBundle);
	
	geometry->SetAttributeArray(kArrayNormal, static_cast<Vector3D *>(normalBundle.pointer));
	geometry->SetArrayBundle(kArrayNormal, &normalBundle);
	
	geometry->SetAttributeArray(kArrayTangent, static_cast<Vector4D *>(tangentBundle.pointer));
	geometry->SetArrayBundle(kArrayTangent, &tangentBundle);
	
	geometry->SetArrayBundle(kArrayPlane, &planeBundle);
	
	Controller::SetDetailLevel(level);
}

// ZYURVUR
