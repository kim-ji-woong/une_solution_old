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


#include "C4GeometryObjects.h"
#include "C4Bounding.h"
#include "C4Topology.h"


using namespace C4;


namespace C4
{
	template <> Heap Memory<GeometryVertex>::heap("GeometryVertex");
	template class Memory<GeometryVertex>;
	
	template <> Heap Memory<GeometryOctree>::heap("GeometryOctree", MemoryMgr::CalculatePoolSize(128, sizeof(GeometryOctree)));
	template class Memory<GeometryOctree>;
}


void C4::Reverse(TextureAlignData *data)
{
	Reverse(&data->alignMode);
	Reverse(&data->alignPlane);
}


void C4::Reverse(SurfaceData *data)
{
	Reverse(&data->surfaceFlags);
	Reverse(&data->materialIndex);
	Reverse(&data->textureAlignData[0]);
	Reverse(&data->textureAlignData[1]);
}


void C4::Reverse(ArrayDescriptor *desc)
{
	Reverse(&desc->identifier);
	Reverse(&desc->elementCount);
	Reverse(&desc->elementSize);
	Reverse(&desc->componentCount);
}


void C4::Reverse(SegmentData *sd)
{
	Reverse(&sd->materialIndex);
	Reverse(&sd->faceStart);
	Reverse(&sd->faceCount);
}


void C4::Reverse(BoneWeight *bw)
{
	Reverse(&bw->boneIndex);
	Reverse(&bw->weight);
}


GeometryVertex::GeometryVertex()
{
	normal.Set(0.0F, 0.0F, 1.0F);
	color.Set(0.0F, 0.0F, 0.0F, 0.0F);
}

GeometryVertex::~GeometryVertex()
{
}


GeometryPolygon::GeometryPolygon()
{
}

GeometryPolygon::~GeometryPolygon()
{
}


GeometrySurface::GeometrySurface(unsigned_int32 flags)
{
	surfaceFlags = flags;
	texcoordCount = 1;
}

GeometrySurface::~GeometrySurface()
{
}


GeometryOctree::GeometryOctree() : indexArray(8)
{
}

GeometryOctree::GeometryOctree(const Box3D& bounds) : indexArray(8)
{
	octreeCenter = (bounds.min + bounds.max) * 0.5F;
	octreeSize = (bounds.max - bounds.min) * 0.5F;
}

GeometryOctree::GeometryOctree(const Point3D& center, const Vector3D& size) : indexArray(8)
{
	octreeCenter = center; 
	octreeSize = size;
} 
 
GeometryOctree::GeometryOctree(const GeometryOctree *octree, int32 subnodeIndex) : indexArray(8) 
{
	const Vector3D& size = octree->GetSize(); 
	float x = size.x * 0.5F;
	float y = size.y * 0.5F;
	float z = size.z * 0.5F;
	octreeSize.Set(x, y, z); 
	
	if (!(subnodeIndex & kOctantX)) x = -x;
	if (!(subnodeIndex & kOctantY)) y = -y;
	if (!(subnodeIndex & kOctantZ)) z = -z; 
	
	const Point3D& center = octree->GetCenter();
	octreeCenter.Set(center.x + x, center.y + y, center.z + z);
}

GeometryOctree::~GeometryOctree()
{
}

void GeometryOctree::Purge(void)
{
	Octree<GeometryOctree>::Purge();
	indexArray.Purge();
}

int32 GeometryOctree::ClassifyPoint(const Vector3D& p) const
{
	int32 index = (p.x > octreeCenter.x) ? kOctantX : 0;
	if (p.y > octreeCenter.y) index |= kOctantY;
	if (p.z > octreeCenter.z) index |= kOctantZ;
	return (index);
}

int32 GeometryOctree::ClassifySphere(const Point3D& p, float r) const
{
	int32 index = 0;
	
	float cx = octreeCenter.x;
	if (p.x > cx + r) index |= kOctantX;
	else if (p.x >= cx - r) return (-1);
	
	float cy = octreeCenter.y;
	if (p.y > cy + r) index |= kOctantY;
	else if (p.y >= cy - r) return (-1);
	
	float cz = octreeCenter.z;
	if (p.z > cz + r) index |= kOctantZ;
	else if (p.z >= cz - r) return (-1);
	
	return (index);
}

GeometryOctree *GeometryOctree::FindNodeContainingPoint(const Vector3D& p, int32 maxDepth)
{
	if (maxDepth > 1)
	{
		int32 subnodeIndex = ClassifyPoint(p);
		GeometryOctree *subnode = GetSubnode(subnodeIndex);
		if (!subnode)
		{
			subnode = new GeometryOctree(this, subnodeIndex);
			SetSubnode(subnodeIndex, subnode);
		}
		
		return (subnode->FindNodeContainingPoint(p, maxDepth - 1));
	}
	
	return (this);
}

GeometryOctree *GeometryOctree::FindNodeContainingSphere(const Point3D& p, float r, int32 maxDepth)
{
	if (maxDepth > 1)
	{
		int32 subnodeIndex = ClassifySphere(p, r);
		if (subnodeIndex >= 0)
		{
			GeometryOctree *subnode = GetSubnode(subnodeIndex);
			if (!subnode)
			{
				subnode = new GeometryOctree(this, subnodeIndex);
				SetSubnode(subnodeIndex, subnode);
			}
			
			return (subnode->FindNodeContainingSphere(p, r, maxDepth - 1));
		}
	}
	
	return (this);
}

GeometryOctree *GeometryOctree::FindNodeContainingEdge(const Vector3D& p1, const Vector3D& p2, int32 maxDepth)
{
	if (maxDepth > 1)
	{
		int32 subnodeIndex1 = ClassifyPoint(p1);
		int32 subnodeIndex2 = ClassifyPoint(p2);
		if (subnodeIndex1 == subnodeIndex2)
		{
			GeometryOctree *subnode = GetSubnode(subnodeIndex1);
			if (!subnode)
			{
				subnode = new GeometryOctree(this, subnodeIndex1);
				SetSubnode(subnodeIndex1, subnode);
			}
			
			return (subnode->FindNodeContainingEdge(p1, p2, maxDepth - 1));
		}
	}
	
	return (this);
}

GeometryOctree *GeometryOctree::FindNodeContainingTriangle(const Vector3D& p1, const Vector3D& p2, const Vector3D& p3, int32 maxDepth)
{
	if (maxDepth > 1)
	{
		int32 subnodeIndex1 = ClassifyPoint(p1);
		int32 subnodeIndex2 = ClassifyPoint(p2);
		int32 subnodeIndex3 = ClassifyPoint(p3);
		if ((subnodeIndex1 == subnodeIndex2) && (subnodeIndex1 == subnodeIndex3))
		{
			GeometryOctree *subnode = GetSubnode(subnodeIndex1);
			if (!subnode)
			{
				subnode = new GeometryOctree(this, subnodeIndex1);
				SetSubnode(subnodeIndex1, subnode);
			}
			
			return (subnode->FindNodeContainingTriangle(p1, p2, p3, maxDepth - 1));
		}
	}
	
	return (this);
}


GeometryLevel::GeometryLevel()
{
	levelStorage = nullptr;
}

GeometryLevel::~GeometryLevel()
{
	delete[] levelStorage;
}

inline bool GeometryLevel::TangentsSimilar(const Vector4D& t1, const Vector4D& t2)
{
	return ((t1.w == t2.w) && (t1.GetVector3D() * t2.GetVector3D() > 0.0F));
}

GeometryArrayType GeometryLevel::GetArrayType(const ArrayDescriptor *desc, int32 *count)
{
	int32 elementCount = desc->elementCount;
	
	switch (desc->identifier)
	{
		case kArrayColor0:
		case kArrayColor1:
		case kArrayColor2:
			
			if (desc->componentCount == 1)
			{
				*count = elementCount * 4;
				return (kGeometryByte);
			}
			
			break;
			
		case kArraySharedIndex:
		case kArraySurfaceIndex:
		case kArrayPlaneIndex:
			
			*count = elementCount;
			return (kGeometryShort);
		
		case kArrayNodeHash:
			
			*count = elementCount;
			return ((desc->elementSize == 4) ? kGeometryLong : kGeometryShort);
		
		case kArrayFace:
			
			*count = elementCount;
			return ((desc->elementSize == sizeof(Triangle)) ? kGeometryTriangle : kGeometryQuad);
		
		case kArraySegment:
			
			*count = elementCount;
			return (kGeometrySegment);
		
		case kArrayEdge:
			
			*count = elementCount;
			return (kGeometryEdge);
	}
	
	*count = elementCount * desc->componentCount;
	return (kGeometryFloat);
}

void GeometryLevel::Pack(Packer& data, unsigned_int32 packFlags) const
{
	int32 arrayCount = 0;
	unsigned_int32 size = 12;
	for (machine index = 0; index < kMaxGeometryArrayCount; index++)
	{
		const ArrayBundle *bundle = &arrayBundle[index];
		if ((bundle->pointer) && (bundle->descriptor.elementCount != 0))
		{
			arrayCount++;
			size += sizeof(ArrayDescriptor);
			size += (bundle->descriptor.elementCount * bundle->descriptor.elementSize + 3) & ~3;
		}
	}
	
	data << ChunkHeader('ARAY', size);
	data << vertexCount;
	data << arrayCount;
	data << weightDataSize;
	
	for (machine index = 0; index < kMaxGeometryArrayCount; index++)
	{
		const ArrayBundle *bundle = &arrayBundle[index];
		if ((bundle->pointer) && (bundle->descriptor.elementCount != 0)) data << bundle->descriptor;
	}
	
	for (machine index = 0; index < kMaxGeometryArrayCount; index++)
	{
		const ArrayBundle *bundle = &arrayBundle[index];
		const void *array = bundle->pointer;
		if ((array) && (bundle->descriptor.elementCount != 0))
		{
			static const unsigned_int16 zero = 0;
			
			int32	count;
			
			GeometryArrayType type = GetArrayType(&bundle->descriptor, &count);
			switch (type)
			{
				case kGeometryFloat:
					
					data.WriteArray(count, static_cast<const float *>(array));
					break;
				
				case kGeometryByte:
					
					data.WriteArray((count + 3) & ~3, static_cast<const unsigned_int8 *>(array));
					break;
				
				case kGeometryShort:
					
					data.WriteArray((count + 1) & ~1, static_cast<const unsigned_int16 *>(array));
					break;
				
				case kGeometryLong:
					
					data.WriteArray(count, static_cast<const unsigned_int32 *>(array));
					break;
				
				case kGeometryTriangle:
				
					data.WriteArray(count, static_cast<const Triangle *>(array));
					if (count & 1) data << zero;
					break;
				
				case kGeometryQuad:
					
					data.WriteArray(count, static_cast<const Quad *>(array));
					break;
				
				case kGeometrySegment:
					
					data.WriteArray(count, static_cast<const SegmentData *>(array));
					break;
				
				case kGeometryEdge:
					
					data.WriteArray(count, static_cast<const Edge *>(array));
					break;
			}
		}
	}
	
	if (weightData)
	{
		data << ChunkHeader('SKIN', weightDataSize);
		
		const WeightedVertex *weightedVertex = weightData;
		for (machine a = 0; a < vertexCount; a++)
		{
			data << weightedVertex->boneCount;
			int32 boneCount = weightedVertex->boneCount;
			
			const BoneWeight *boneWeight = weightedVertex->boneWeight;
			for (machine b = 0; b < boneCount; b++, boneWeight++) data << *boneWeight;
			
			weightedVertex = reinterpret_cast<const WeightedVertex *>(boneWeight);
		}
	}
	
	data << TerminatorChunk;
}

void GeometryLevel::Unpack(Unpacker& data, unsigned_int32 unpackFlags)
{
	UnpackChunkList<GeometryLevel>(data, unpackFlags);
	
	#if C4LEGACY
	
		if (arrayBundle[12].pointer)	// Clear ambient transition array
		{
			arrayBundle[12].descriptor.elementCount = 0;
			arrayBundle[12].pointer = nullptr;
			attributeArrayCount--;
		}
	
	#endif
}

bool GeometryLevel::UnpackChunk(const ChunkHeader *chunkHeader, Unpacker& data, unsigned_int32 unpackFlags)
{
	switch (chunkHeader->chunkType)
	{
		case 'ARAY':
		{
			int32				vertCount;
			int32				arrayCount;
			unsigned_int32		weightSize;
			ArrayDescriptor		arrayDesc[kMaxGeometryArrayCount];
			
			data >> vertCount;
			data >> arrayCount;
			data >> weightSize;
			
			data.ReadArray(arrayCount, arrayDesc);
			AllocateStorage(vertCount, arrayCount, arrayDesc, weightSize);
			
			for (machine a = 0; a < arrayCount; a++)
			{
				int32	count;
				
				const ArrayDescriptor *desc = &arrayDesc[a];
				void *array = arrayBundle[desc->identifier].pointer;
				
				GeometryArrayType type = GetArrayType(desc, &count);
				switch (type)
				{
					case kGeometryFloat:
						
						data.ReadArray(count, static_cast<float *>(array));
						break;
					
					case kGeometryByte:
						
						data.ReadArray((count + 3) & ~3, static_cast<unsigned_int8 *>(array));
						break;
					
					case kGeometryShort:
						
						data.ReadArray((count + 1) & ~1, static_cast<unsigned_int16 *>(array));
						break;
					
					case kGeometryLong:
						
						data.ReadArray(count, static_cast<unsigned_int32 *>(array));
						break;
					
					case kGeometryTriangle:
						
						data.ReadArray(count, static_cast<Triangle *>(array));
						if (count & 1) data += 2;
						break;
					
					case kGeometryQuad:
						
						data.ReadArray(count, static_cast<Quad *>(array));
						break;
					
					case kGeometrySegment:
						
						data.ReadArray(count, static_cast<SegmentData *>(array));
						break;
					
					case kGeometryEdge:
						
						data.ReadArray(count, static_cast<Edge *>(array));
						break;
				}
				
				#if C4LEGACY
				
					if ((desc->identifier == kArrayNodeHash) && (desc->elementSize == 2))
					{
						unsigned_int32 *longArray = static_cast<unsigned_int32 *>(array);
						unsigned_int16 *shortArray = static_cast<unsigned_int16 *>(array);
						for (machine b = count - 1; b >= 0; b--) longArray[b] = shortArray[b];
					}
				
				#endif
			}
			
			return (true);
		}
		
		case 'SKIN':
		{
			WeightedVertex *weightedVertex = weightData;
			
			for (machine a = 0; a < vertexCount; a++)
			{
				data >> weightedVertex->boneCount;
				int32 boneCount = weightedVertex->boneCount;
				
				BoneWeight *boneWeight = weightedVertex->boneWeight;
				for (machine b = 0; b < boneCount; b++, boneWeight++) data >> *boneWeight;
				
				weightedVertex = reinterpret_cast<WeightedVertex *>(boneWeight);
			}
			
			return (true);
		}
	}
	
	return (false);
}

void *GeometryLevel::BeginSettingsUnpack(void)
{
	delete[] levelStorage;
	levelStorage = nullptr;
	
	return (nullptr);
}

void GeometryLevel::AllocateStorage(int32 vertCount, int32 arrayCount, const ArrayDescriptor *arrayDesc, unsigned_int32 weightSize)
{
	delete[] levelStorage;
	levelStorage = nullptr;
	
	vertexCount = vertCount;
	faceOffset = 0;
	
	weightDataSize = weightSize;
	weightData = nullptr;
	
	unsigned_int32 size = 0;
	for (machine a = 0; a < arrayCount; a++)
	{
		const ArrayDescriptor *desc = &arrayDesc[a];
		
		#if C4LEGACY
		
			if (desc->identifier != kArrayNodeHash) size += (desc->elementCount * desc->elementSize + 15) & ~15;
			else size += (desc->elementCount * 4 + 15) & ~15;
		
		#else
		
			size += (desc->elementCount * desc->elementSize + 15) & ~15;
		
		#endif
	}
	
	for (machine index = 0; index < kMaxGeometryArrayCount; index++)
	{
		arrayBundle[index].descriptor.elementCount = 0;
		arrayBundle[index].pointer = nullptr;
	}
	
	int32 attributeCount = 0;
	
	size += weightSize;
	if (size != 0)
	{
		char *pointer = new char[size];
		levelStorage = pointer;
		
		for (machine a = 0; a < arrayCount; a++)
		{
			const ArrayDescriptor *desc = &arrayDesc[a];
			int32 count = desc->elementCount;
			if (count != 0)
			{
				int32 index = desc->identifier;
				
				arrayBundle[index].descriptor = *desc;
				arrayBundle[index].pointer = pointer;
				
				unsigned_int32 logicalSize = count * desc->elementSize;
				
				#if C4LEGACY
				
					if (index == kArrayNodeHash)
					{
						logicalSize = count * 4;
						arrayBundle[index].descriptor.elementSize = 4;
					}
					
				#endif
				
				unsigned_int32 physicalSize = (logicalSize + 15) & ~15;
				
				for (unsigned_machine b = logicalSize; b < physicalSize; b++) pointer[b] = 0;
				pointer += physicalSize;
				
				if (index < kMaxAttributeArrayCount)
				{
					attributeArrayIndex[attributeCount] = (unsigned_int8) index;
					attributeOffset[attributeCount] = 0;
					attributeCount++;
				}
			}
		}
		
		if (weightSize != 0) weightData = reinterpret_cast<WeightedVertex *>(pointer);
	}
	
	attributeArrayCount = attributeCount;
}

void GeometryLevel::AllocateStorage(const GeometryLevel *inputLevel, int32 arrayCount, const ArrayDescriptor *arrayDesc, unsigned_int32 weightSize)
{
	bool	copyFlag[kMaxGeometryArrayCount];
	
	delete[] levelStorage;
	levelStorage = nullptr;
	
	vertexCount = inputLevel->GetVertexCount();
	faceOffset = 0;
	
	weightDataSize = 0;
	weightData = nullptr;
	
	unsigned_int32 size = 0;
	for (machine a = 0; a < arrayCount; a++)
	{
		const ArrayDescriptor *desc = &arrayDesc[a];
		size += (desc->elementCount * desc->elementSize + 15) & ~15;
	}
	
	for (machine index = 0; index < kMaxGeometryArrayCount; index++)
	{
		copyFlag[index] = false;
		
		const ArrayBundle *bundle = inputLevel->GetArrayBundle(index);
		if ((bundle->pointer) && (bundle->descriptor.elementCount != 0))
		{
			for (machine a = 0; a < arrayCount; a++)
			{
				if (arrayDesc[a].identifier == index) goto next;
			}
			
			copyFlag[index] = true;
			size += (bundle->GetArraySize() + 15) & ~15;
		}
		
		next:;
	}
	
	for (machine index = 0; index < kMaxGeometryArrayCount; index++)
	{
		arrayBundle[index].descriptor.elementCount = 0;
		arrayBundle[index].pointer = nullptr;
	}
	
	int32 attributeCount = 0;
	
	if (weightSize == 0) size += inputLevel->GetWeightDataSize();
	else size += weightSize;
	
	if (size != 0)
	{
		char *pointer = new char[size];
		levelStorage = pointer;
		
		for (machine a = 0; a < arrayCount; a++)
		{
			const ArrayDescriptor *desc = &arrayDesc[a];
			int32 count = desc->elementCount;
			if (count != 0)
			{
				int32 index = desc->identifier;
				
				arrayBundle[index].descriptor = *desc;
				arrayBundle[index].pointer = pointer;
				
				unsigned_int32 logicalSize = count * desc->elementSize;
				unsigned_int32 physicalSize = (logicalSize + 15) & ~15;
				
				for (unsigned_machine b = logicalSize; b < physicalSize; b++) pointer[b] = 0;
				pointer += physicalSize;
				
				if (index < kMaxAttributeArrayCount)
				{
					attributeArrayIndex[attributeCount] = (unsigned_int8) index;
					attributeOffset[attributeCount] = 0;
					attributeCount++;
				}
			}
		}
		
		for (machine index = 0; index < kMaxGeometryArrayCount; index++)
		{
			const ArrayBundle *bundle = inputLevel->GetArrayBundle(index);
			if (copyFlag[index])
			{
				arrayBundle[index].descriptor = bundle->descriptor;
				arrayBundle[index].pointer = pointer;
				
				unsigned_int32 logicalSize = bundle->GetArraySize();
				unsigned_int32 physicalSize = (logicalSize + 15) & ~15;
				
				MemoryMgr::CopyMemory(bundle->pointer, pointer, logicalSize);
				for (unsigned_machine a = logicalSize; a < physicalSize; a++) pointer[a] = 0;
				pointer += physicalSize;
				
				if (index < kMaxAttributeArrayCount)
				{
					attributeArrayIndex[attributeCount] = (unsigned_int8) index;
					attributeOffset[attributeCount] = 0;
					attributeCount++;
				}
			}
		}
		
		if (weightSize != 0)
		{
			weightDataSize = weightSize;
			weightData = reinterpret_cast<WeightedVertex *>(pointer);
		}
		else
		{
			weightSize = inputLevel->GetWeightDataSize();
			if (weightSize != 0)
			{
				weightDataSize = weightSize;
				weightData = reinterpret_cast<WeightedVertex *>(pointer);
				MemoryMgr::CopyMemory(inputLevel->GetWeightData(), weightData, weightSize);
			}
		}
	}
	
	attributeArrayCount = attributeCount;
}

void GeometryLevel::CopyGeometryLevel(const GeometryLevel *geometryLevel, unsigned_int32 exclusionMask)
{
	ArrayDescriptor		desc[kMaxGeometryArrayCount];
	
	int32 arrayCount = 0;
	for (machine a = 0; a < kMaxGeometryArrayCount; a++)
	{
		if ((exclusionMask & 1) == 0)
		{
			const ArrayBundle *bundle = geometryLevel->GetArrayBundle(a);
			if (bundle->pointer)
			{
				desc[arrayCount] = bundle->descriptor;
				arrayCount++;
			}
		}
		
		exclusionMask >>= 1;
	}
	
	int32 count = geometryLevel->GetVertexCount();
	unsigned_int32 weightSize = geometryLevel->GetWeightDataSize();
	AllocateStorage(count, arrayCount, desc, weightSize);
	
	for (machine a = 0; a < arrayCount; a++)
	{
		int32 index = desc[a].identifier;
		const ArrayBundle *bundle = geometryLevel->GetArrayBundle(index);
		MemoryMgr::CopyMemory(bundle->pointer, arrayBundle[index].pointer, bundle->GetArraySize());
	}
	
	if (weightSize != 0) MemoryMgr::CopyMemory(geometryLevel->GetWeightData(), weightData, weightSize);
}

void GeometryLevel::CopyRigidGeometryLevel(const GeometryLevel *geometryLevel, unsigned_int32 exclusionMask)
{
	ArrayDescriptor		desc[kMaxGeometryArrayCount];
	
	exclusionMask |= (1 << kArrayNodeHash) | (1 << kArrayInverseBindTransform);
	
	int32 arrayCount = 0;
	for (machine a = 0; a < kMaxGeometryArrayCount; a++)
	{
		if ((exclusionMask & 1) == 0)
		{
			const ArrayBundle *bundle = geometryLevel->GetArrayBundle(a);
			if (bundle->pointer)
			{
				desc[arrayCount] = bundle->descriptor;
				arrayCount++;
			}
		}
		
		exclusionMask >>= 1;
	}
	
	int32 count = geometryLevel->GetVertexCount();
	AllocateStorage(count, arrayCount, desc);
	
	for (machine a = 0; a < arrayCount; a++)
	{
		int32 index = desc[a].identifier;
		const ArrayBundle *bundle = geometryLevel->GetArrayBundle(index);
		MemoryMgr::CopyMemory(bundle->pointer, arrayBundle[index].pointer, bundle->GetArraySize());
	}
}

void GeometryLevel::TransformGeometryLevel(const Transform4D& transform)
{
	Transform4D inverse = Inverse(transform);
	
	int32 vertCount = GetVertexCount();
	Point3D *vertex = GetArray<Point3D>(kArrayVertex);
	
	Vector3D *normal = GetArray<Vector3D>(kArrayNormal);
	if (normal)
	{
		for (machine a = 0; a < vertCount; a++)
		{
			*vertex = transform * *vertex;
			*normal = (*normal * inverse).Normalize();
			vertex++;
			normal++;
		}
	}
	else
	{
		for (machine a = 0; a < vertCount; a++)
		{
			*vertex = transform * *vertex;
			vertex++;
		}
	}
	
	Antivector4D *plane = GetArray<Antivector4D>(kArrayPlane);
	if (plane)
	{
		int32 planeCount = GetArrayDescriptor(kArrayPlane)->elementCount;
		for (machine a = 0; a < planeCount; a++)
		{
			*plane = *plane * inverse;
			plane->Standardize();
			plane++;
		}
	}
}

void GeometryLevel::TranslateGeometryLevel(const Vector3D& translation)
{
	int32 vertCount = GetVertexCount();
	Point3D *vertex = GetArray<Point3D>(kArrayVertex);
	for (machine a = 0; a < vertCount; a++)
	{
		*vertex += translation;
		vertex++;
	}
	
	Antivector4D *plane = GetArray<Antivector4D>(kArrayPlane);
	if (plane)
	{
		int32 planeCount = GetArrayDescriptor(kArrayPlane)->elementCount;
		for (machine a = 0; a < planeCount; a++)
		{
			plane->w -= *plane ^ translation;
			plane++;
		}
	}
}

void GeometryLevel::ScaleGeometryLevel(const Vector3D& scale)
{
	Vector3D inverse(1.0F / scale.x, 1.0F / scale.y, 1.0F / scale.z);
	
	int32 vertCount = GetVertexCount();
	Point3D *vertex = GetArray<Point3D>(kArrayVertex);
	
	Vector3D *normal = GetArray<Vector3D>(kArrayNormal);
	if (normal)
	{
		for (machine a = 0; a < vertCount; a++)
		{
			*vertex &= scale;
			*normal = (*normal & inverse).Normalize();
			vertex++;
			normal++;
		}
	}
	else
	{
		for (machine a = 0; a < vertCount; a++)
		{
			*vertex &= scale;
			vertex++;
		}
	}
	
	Antivector4D *plane = GetArray<Antivector4D>(kArrayPlane);
	if (plane)
	{
		int32 planeCount = GetArrayDescriptor(kArrayPlane)->elementCount;
		for (machine a = 0; a < planeCount; a++)
		{
			plane->GetAntivector3D() &= inverse;
			plane->Standardize();
			plane++;
		}
	}
}

void GeometryLevel::InvertGeometryLevel(void)
{
	int32 vertCount = GetVertexCount();
	
	Vector3D *normal = GetArray<Vector3D>(kArrayNormal);
	if (normal)
	{
		for (machine a = 0; a < vertCount; a++)
		{
			*normal = -*normal;
			normal++;
		}
	}
	
	Vector4D *tangent = GetArray<Vector4D>(kArrayTangent);
	if ((tangent) && (GetArrayDescriptor(kArrayTangent)->componentCount == 4))
	{
		for (machine a = 0; a < vertCount; a++)
		{
			tangent->w = -tangent->w;
			tangent++;
		}
	}
	
	float *handedness = GetArray<float>(kArrayHandedness);
	if (handedness)
	{
		for (machine a = 0; a < vertCount; a++)
		{
			*handedness = -*handedness;
			handedness++;
		}
	}
	
	int32 faceCount = GetFaceCount();
	Triangle *triangle = GetArray<Triangle>(kArrayFace);
	for (machine a = 0; a < faceCount; a++)
	{
		int32 t = triangle->index[1];
		triangle->index[1] = triangle->index[2];
		triangle->index[2] = (unsigned_int16) t;
		triangle++;
	}
	
	Edge *edge = GetArray<Edge>(kArrayEdge);
	if (edge)
	{
		int32 edgeCount = GetArrayDescriptor(kArrayEdge)->elementCount;
		for (machine a = 0; a < edgeCount; a++)
		{
			int32 t = edge->faceIndex[0];
			edge->faceIndex[0] = edge->faceIndex[1];
			edge->faceIndex[1] = (unsigned_int16) t;
			edge++;
		}
	}
	
	Antivector4D *plane = GetArray<Antivector4D>(kArrayPlane);
	if (plane)
	{
		int32 planeCount = GetArrayDescriptor(kArrayPlane)->elementCount;
		for (machine a = 0; a < planeCount; a++)
		{
			*plane = -*plane;
			plane++;
		}
	}
}

void GeometryLevel::WeldGeometryLevel(float epsilon)
{
	const unsigned_int16 *surfaceIndex = GetArray<unsigned_int16>(kArraySurfaceIndex);
	if (surfaceIndex)
	{
		Box3D	bounds;
		
		int32 vertCount = GetVertexCount();
		Point3D *vertex = GetArray<Point3D>(kArrayVertex);
		
		bounds.Calculate(vertCount, vertex);
		GeometryOctree vertexOctree(bounds);
		
		float e2 = epsilon * epsilon;
		
		for (machine a = 0; a < vertCount; a++)
		{
			unsigned_int32 si = surfaceIndex[a];
			
			const Point3D& position = vertex[a];
			GeometryOctree *octreeNode = vertexOctree.FindNodeContainingSphere(position, epsilon);
			
			int32 indexCount = octreeNode->GetIndexCount();
			const unsigned_int32 *indexArray = octreeNode->GetIndexArray();
			for (machine b = 0; b < indexCount; b++)
			{
				unsigned_int32 index = indexArray[b];
				if ((surfaceIndex[index] != si) && (SquaredMag(vertex[index] - position) < e2))
				{
					vertex[a] = vertex[index];
					goto next;
				}
			}
			
			octreeNode->AddIndex(a);
			next:;
		}
	}
}

void GeometryLevel::MendGeometryLevel(float vertexEpsilon, float normalEpsilon, float texcoordEpsilon)
{
	Box3D	bounds;
	
	int32 vertCount = GetVertexCount();
	Point3D *vertex = GetArray<Point3D>(kArrayVertex);
	
	bounds.Calculate(vertCount, vertex);
	GeometryOctree vertexOctree(bounds);
	
	float e2 = vertexEpsilon * vertexEpsilon;
	normalEpsilon = 1.0F - normalEpsilon;
	
	Vector3D *normal = GetArray<Vector3D>(kArrayNormal);
	Vector4D *tangent = GetArray<Vector4D>(kArrayTangent);
	Point2D *texcoord = GetArray<Point2D>(kArrayTexture0);
	const unsigned_int16 *surfaceIndex = GetArray<unsigned_int16>(kArraySurfaceIndex);
	
	if (normal)
	{
		if (tangent)
		{
			for (machine a = 0; a < vertCount; a++)
			{
				const Point3D& position = vertex[a];
				GeometryOctree *octreeNode = vertexOctree.FindNodeContainingSphere(position, vertexEpsilon);
				
				int32 indexCount = octreeNode->GetIndexCount();
				const unsigned_int32 *indexArray = octreeNode->GetIndexArray();
				
				const Vector3D& nrml = normal[a];
				const Vector4D& tang = tangent[a];
				const Point2D& texc = texcoord[a];
				
				for (machine b = 0; b < indexCount; b++)
				{
					unsigned_int32 index = indexArray[b];
					if ((!surfaceIndex) || (surfaceIndex[index] == surfaceIndex[a]))
					{
						if ((SquaredMag(vertex[index] - position) < e2) && (normal[index] * nrml > normalEpsilon) && (TangentsSimilar(tangent[index], tang)))
						{
							if ((Fabs(texcoord[index].x - texc.x) < texcoordEpsilon) && (Fabs(texcoord[index].y - texc.y) < texcoordEpsilon))
							{
								vertex[a] = vertex[index];
								normal[a] = normal[index];
								texcoord[a] = texcoord[index];
								goto next1;
							}
						}
					}
				}
				
				octreeNode->AddIndex(a);
				next1:;
			}
		}
		else
		{
			for (machine a = 0; a < vertCount; a++)
			{
				const Point3D& position = vertex[a];
				GeometryOctree *octreeNode = vertexOctree.FindNodeContainingSphere(position, vertexEpsilon);
				
				int32 indexCount = octreeNode->GetIndexCount();
				const unsigned_int32 *indexArray = octreeNode->GetIndexArray();
				
				const Vector3D& nrml = normal[a];
				const Point2D& texc = texcoord[a];
				
				for (machine b = 0; b < indexCount; b++)
				{
					unsigned_int32 index = indexArray[b];
					if ((!surfaceIndex) || (surfaceIndex[index] == surfaceIndex[a]))
					{
						if ((SquaredMag(vertex[index] - position) < e2) && (normal[index] * nrml > normalEpsilon))
						{
							if ((Fabs(texcoord[index].x - texc.x) < texcoordEpsilon) && (Fabs(texcoord[index].y - texc.y) < texcoordEpsilon))
							{
								vertex[a] = vertex[index];
								normal[a] = normal[index];
								texcoord[a] = texcoord[index];
								goto next2;
							}
						}
					}
				}
				
				octreeNode->AddIndex(a);
				next2:;
			}
		}
	}
	else
	{
		if (tangent)
		{
			for (machine a = 0; a < vertCount; a++)
			{
				const Point3D& position = vertex[a];
				GeometryOctree *octreeNode = vertexOctree.FindNodeContainingSphere(position, vertexEpsilon);
				
				int32 indexCount = octreeNode->GetIndexCount();
				const unsigned_int32 *indexArray = octreeNode->GetIndexArray();
				
				const Vector4D& tang = tangent[a];
				const Point2D& texc = texcoord[a];
				
				for (machine b = 0; b < indexCount; b++)
				{
					unsigned_int32 index = indexArray[b];
					if ((!surfaceIndex) || (surfaceIndex[index] == surfaceIndex[a]))
					{
						if ((SquaredMag(vertex[index] - position) < e2) && (TangentsSimilar(tangent[index], tang)))
						{
							if ((Fabs(texcoord[index].x - texc.x) < texcoordEpsilon) && (Fabs(texcoord[index].y - texc.y) < texcoordEpsilon))
							{
								vertex[a] = vertex[index];
								texcoord[a] = texcoord[index];
								goto next3;
							}
						}
					}
				}
				
				octreeNode->AddIndex(a);
				next3:;
			}
		}
		else
		{
			for (machine a = 0; a < vertCount; a++)
			{
				const Point3D& position = vertex[a];
				GeometryOctree *octreeNode = vertexOctree.FindNodeContainingSphere(position, vertexEpsilon);
				
				int32 indexCount = octreeNode->GetIndexCount();
				const unsigned_int32 *indexArray = octreeNode->GetIndexArray();
				
				const Point2D& texc = texcoord[a];
				
				for (machine b = 0; b < indexCount; b++)
				{
					unsigned_int32 index = indexArray[b];
					if ((!surfaceIndex) || (surfaceIndex[index] == surfaceIndex[a]))
					{
						if (SquaredMag(vertex[index] - position) < e2)
						{
							if ((Fabs(texcoord[index].x - texc.x) < texcoordEpsilon) && (Fabs(texcoord[index].y - texc.y) < texcoordEpsilon))
							{
								vertex[a] = vertex[index];
								texcoord[a] = texcoord[index];
								goto next4;
							}
						}
					}
				}
				
				octreeNode->AddIndex(a);
				next4:;
			}
		}
	}
}

void GeometryLevel::UnifyGeometryLevel(const GeometryLevel *inputLevel)
{
	ArrayDescriptor		desc[kMaxGeometryTexcoordCount + 7];
	const Point2D		*inputTexcoord[kMaxGeometryTexcoordCount];
	Point2D				*outputTexcoord[kMaxGeometryTexcoordCount];
	Box3D				bounds;
	
	int32 vertCount = inputLevel->GetVertexCount();
	const Point3D *inputVertex = inputLevel->GetArray<Point3D>(kArrayVertex);
	
	bounds.Calculate(vertCount, inputVertex);
	GeometryOctree vertexOctree(bounds);
	
	Buffer buffer(vertCount * 4);
	unsigned_int32 *remapTable = static_cast<unsigned_int32 *>(*buffer);
	
	const Color4C *inputColor = inputLevel->GetArray<Color4C>(kArrayColor0);
	const Vector3D *inputNormal = inputLevel->GetArray<Vector3D>(kArrayNormal);
	const Vector4D *inputTangent = inputLevel->GetArray<Vector4D>(kArrayTangent);
	if ((inputTangent) && (inputLevel->GetArrayDescriptor(kArrayTangent)->componentCount != 4)) inputTangent = nullptr;
	for (machine a = 0; a < kMaxGeometryTexcoordCount; a++) inputTexcoord[a] = inputLevel->GetArray<Point2D>(kArrayTexture0 + a);
	
	int32 unifiedCount = 0;
	unsigned_int32 weightSize = 0;
	const WeightedVertex **weightDataTable = nullptr;
	
	const WeightedVertex *weightedVertex = inputLevel->GetWeightData();
	if (weightedVertex)
	{
		weightDataTable = new const WeightedVertex *[vertCount];
		
		for (machine a = 0; a < vertCount; a++)
		{
			weightDataTable[a] = weightedVertex;
			weightedVertex = reinterpret_cast<const WeightedVertex *>(weightedVertex->boneWeight + weightedVertex->boneCount);
		}
	}
	
	const unsigned_int16 *inputSurfaceIndex = inputLevel->GetArray<unsigned_int16>(kArraySurfaceIndex);
	if (inputSurfaceIndex)
	{
		unsigned_int32 currentSurface = inputSurfaceIndex[0];
		
		for (machine a = 0; a < vertCount; a++)
		{
			unsigned_int32 surface = inputSurfaceIndex[a];
			if (surface != currentSurface)
			{
				currentSurface = surface;
				vertexOctree.Purge();
			}
			
			const Point3D& position = inputVertex[a];
			GeometryOctree *octreeNode = vertexOctree.FindNodeContainingPoint(position);
			
			unsigned_int32 vertexIndex = unifiedCount;
			int32 indexCount = octreeNode->GetIndexCount();
			const unsigned_int32 *indexArray = octreeNode->GetIndexArray();
			for (machine b = 0; b < indexCount; b++)
			{
				unsigned_int32 index = indexArray[b];
				if (inputVertex[index] != position) continue;
				if (inputTexcoord[0][index] != inputTexcoord[0][a]) continue;
				if ((inputTexcoord[1]) && (inputTexcoord[1][index] != inputTexcoord[1][a])) continue;
				if ((inputNormal) && (inputNormal[index] != inputNormal[a])) continue;
				if ((inputColor) && (inputColor[index] != inputColor[a])) continue;
				if ((inputTangent) && (!TangentsSimilar(inputTangent[index], inputTangent[a]))) continue;
				
				vertexIndex = remapTable[index];
				break;
			}
			
			remapTable[a] = vertexIndex;
			if (vertexIndex == unifiedCount)
			{
				unifiedCount++;
				octreeNode->AddIndex(a);
				if (weightDataTable) weightSize += weightDataTable[a]->GetSize();
			}
		}
	}
	else
	{
		for (machine a = 0; a < vertCount; a++)
		{
			const Point3D& position = inputVertex[a];
			GeometryOctree *octreeNode = vertexOctree.FindNodeContainingPoint(position);
			
			unsigned_int32 vertexIndex = unifiedCount;
			int32 indexCount = octreeNode->GetIndexCount();
			const unsigned_int32 *indexArray = octreeNode->GetIndexArray();
			for (machine b = 0; b < indexCount; b++)
			{
				unsigned_int32 index = indexArray[b];
				if (inputVertex[index] != position) continue;
				if (inputTexcoord[0][index] != inputTexcoord[0][a]) continue;
				if ((inputTexcoord[1]) && (inputTexcoord[1][index] != inputTexcoord[1][a])) continue;
				if ((inputNormal) && (inputNormal[index] != inputNormal[a])) continue;
				if ((inputColor) && (inputColor[index] != inputColor[a])) continue;
				if ((inputTangent) && (!TangentsSimilar(inputTangent[index], inputTangent[a]))) continue;
				
				vertexIndex = remapTable[index];
				break;
			}
			
			remapTable[a] = vertexIndex;
			if (vertexIndex == unifiedCount)
			{
				unifiedCount++;
				octreeNode->AddIndex(a);
				if (weightDataTable) weightSize += weightDataTable[a]->GetSize();
			}
		}
	}
	
	desc[0].identifier = kArrayVertex;
	desc[0].elementCount = unifiedCount;
	desc[0].elementSize = sizeof(Point3D);
	desc[0].componentCount = 3;
	
	int32 arrayCount = 1;
	if (inputNormal)
	{
		desc[1].identifier = kArrayNormal;
		desc[1].elementCount = unifiedCount;
		desc[1].elementSize = sizeof(Vector3D);
		desc[1].componentCount = 3;
		
		arrayCount = 2;
	}
	
	if (inputColor)
	{
		desc[arrayCount].identifier = kArrayColor0;
		desc[arrayCount].elementCount = unifiedCount;
		desc[arrayCount].elementSize = 4;
		desc[arrayCount].componentCount = 1;
		arrayCount++;
	}
	
	desc[arrayCount].identifier = kArrayTexture0;
	desc[arrayCount].elementCount = unifiedCount;
	desc[arrayCount].elementSize = sizeof(Point2D);
	desc[arrayCount].componentCount = 2;
	arrayCount++;
	
	if (inputTexcoord[1])
	{
		desc[arrayCount].identifier = kArrayTexture1;
		desc[arrayCount].elementCount = unifiedCount;
		desc[arrayCount].elementSize = sizeof(Point2D);
		desc[arrayCount].componentCount = 2;
		arrayCount++;
	}
	
	if (inputSurfaceIndex)
	{
		desc[arrayCount].identifier = kArraySurfaceIndex;
		desc[arrayCount].elementCount = unifiedCount;
		desc[arrayCount].elementSize = 2;
		desc[arrayCount].componentCount = 1;
		arrayCount++;
	}
	
	const Triangle *inputTriangle = inputLevel->GetArray<Triangle>(kArrayFace);
	if (inputTriangle)
	{
		int32 inputFaceCount = inputLevel->GetFaceCount();
		int32 outputFaceCount = 0;
		
		for (machine a = 0; a < inputFaceCount; a++)
		{
			unsigned_int32 i1 = remapTable[inputTriangle[a].index[0]];
			unsigned_int32 i2 = remapTable[inputTriangle[a].index[1]];
			unsigned_int32 i3 = remapTable[inputTriangle[a].index[2]];
			
			if ((i1 != i2) && (i1 != i3) && (i2 != i3)) outputFaceCount++;
		}
		
		desc[arrayCount].elementCount = outputFaceCount;
	}
	else
	{
		desc[arrayCount].elementCount = vertCount / 3;
	}
	
	desc[arrayCount].identifier = kArrayFace;
	desc[arrayCount].elementSize = sizeof(Triangle);
	desc[arrayCount].componentCount = 1;
	arrayCount++;
	
	const ArrayBundle *nodeHashBundle = inputLevel->GetArrayBundle(kArrayNodeHash);
	if (nodeHashBundle->pointer)
	{
		desc[arrayCount] = nodeHashBundle->descriptor;
		arrayCount++;
	}
	
	const ArrayBundle *inverseBindTransformBundle = inputLevel->GetArrayBundle(kArrayInverseBindTransform);
	if (inverseBindTransformBundle->pointer)
	{
		desc[arrayCount] = inverseBindTransformBundle->descriptor;
		arrayCount++;
	}
	
	AllocateStorage(unifiedCount, arrayCount, desc, weightSize);
	
	Point3D *outputVertex = GetArray<Point3D>(kArrayVertex);
	Vector3D *outputNormal = GetArray<Vector3D>(kArrayNormal);
	Color4C *outputColor = GetArray<Color4C>(kArrayColor0);
	for (machine a = 0; a < kMaxGeometryTexcoordCount; a++) outputTexcoord[a] = GetArray<Point2D>(kArrayTexture0 + a);
	unsigned_int16 *outputSurfaceIndex = GetArray<unsigned_int16>(kArraySurfaceIndex);
	
	unifiedCount = 0;
	WeightedVertex *wv = GetWeightData();
	
	for (machine a = 0; a < vertCount; a++)
	{
		unsigned_int32 index = remapTable[a];
		if (index == unifiedCount)
		{
			outputVertex[unifiedCount] = inputVertex[a];
			outputTexcoord[0][unifiedCount] = inputTexcoord[0][a];
			if (outputTexcoord[1]) outputTexcoord[1][unifiedCount] = inputTexcoord[1][a];
			if (outputNormal) outputNormal[unifiedCount] = inputNormal[a];
			if (outputColor) outputColor[unifiedCount] = inputColor[a];
			if (outputSurfaceIndex) outputSurfaceIndex[unifiedCount] = inputSurfaceIndex[a];
			
			if (wv)
			{
				weightedVertex = weightDataTable[a];
				int32 boneCount = weightedVertex->boneCount;
				
				wv->boneCount = boneCount;
				for (machine b = 0; b < boneCount; b++) wv->boneWeight[b] = weightedVertex->boneWeight[b];
				wv = reinterpret_cast<WeightedVertex *>(wv->boneWeight + boneCount);
			}
			
			unifiedCount++;
		}
	}
	
	Triangle *outputTriangle = GetArray<Triangle>(kArrayFace);
	if (inputTriangle)
	{
		int32 inputFaceCount = inputLevel->GetFaceCount();
		int32 outputFaceCount = 0;
		
		for (machine a = 0; a < inputFaceCount; a++)
		{
			unsigned_int32 i1 = remapTable[inputTriangle[a].index[0]];
			unsigned_int32 i2 = remapTable[inputTriangle[a].index[1]];
			unsigned_int32 i3 = remapTable[inputTriangle[a].index[2]];
			
			if ((i1 != i2) && (i1 != i3) && (i2 != i3))
			{
				outputTriangle[outputFaceCount].Set(i1, i2, i3);
				outputFaceCount++;
			}
		}
	}
	else
	{
		unsigned_int32 index = 0;
		int32 faceCount = GetFaceCount();
		for (machine a = 0; a < faceCount; a++)
		{
			outputTriangle[a].Set(remapTable[index], remapTable[index + 1], remapTable[index + 2]);
			index += 3;
		}
	}
	
	const void *pointer = nodeHashBundle->pointer;
	if (pointer) MemoryMgr::CopyMemory(pointer, GetArray(kArrayNodeHash), nodeHashBundle->GetArraySize());
	
	pointer = inverseBindTransformBundle->pointer;
	if (pointer) MemoryMgr::CopyMemory(pointer, GetArray(kArrayInverseBindTransform), inverseBindTransformBundle->GetArraySize());
	
	delete[] weightDataTable;
}

bool GeometryLevel::GetTexcoordTransform(const TextureAlignData *alignData, const Transformable *transformable, Antivector4D *plane)
{
	switch (alignData->alignMode)
	{
		case kTextureAlignNatural:
			
			*plane = alignData->alignPlane;
			return (false);
		
		case kTextureAlignObjectPlane:
			
			*plane = alignData->alignPlane;
			break;
		
		case kTextureAlignWorldPlane:
			
			*plane = alignData->alignPlane * transformable->GetWorldTransform();
			break;
		
		case kTextureAlignGlobalObjectPlane:
			
			*plane = alignData->alignPlane;
			plane->w -= *plane ^ transformable->GetInverseWorldTransform().GetTranslation().GetVector3D();
			break;
	}
	
	return (true);
}

bool GeometryLevel::GenerateTexcoords(const Transformable *transformable, const GeometryObject *object)
{
	Point2D *texcoord = GetArray<Point2D>(kArrayTexture0);
	if (texcoord)
	{
		int32 surfaceCount = object->GetSurfaceCount();
		
		unsigned_int32 size = (sizeof(Antivector4D) * 2 + 2) * surfaceCount;
		Buffer buffer(size);
		
		Antivector4D *planeS = static_cast<Antivector4D *>(*buffer);
		Antivector4D *planeT = planeS + surfaceCount;
		bool *generateS = reinterpret_cast<bool *>(planeT + surfaceCount);
		bool *generateT = generateS + surfaceCount;
		
		int32 genCount = 0;
		for (machine a = 0; a < surfaceCount; a++)
		{
			const SurfaceData *surfaceData = object->GetSurfaceData(a);
			
			bool gen = GetTexcoordTransform(&surfaceData->textureAlignData[0], transformable, &planeS[a]);
			generateS[a] = gen;
			genCount += gen;
			
			gen = GetTexcoordTransform(&surfaceData->textureAlignData[1], transformable, &planeT[a]);
			generateT[a] = gen;
			genCount += gen;
		}
		
		if (genCount != 0)
		{
			const Point3D *vertex = GetArray<Point3D>(kArrayVertex);
			
			const unsigned_int16 *surfaceIndex = GetArray<unsigned_int16>(kArraySurfaceIndex);
			if (surfaceIndex)
			{
				for (machine a = 0; a < vertexCount; a++)
				{
					unsigned_int32 index = surfaceIndex[a];
					if (generateS[index]) texcoord[a].x = planeS[index] ^ vertex[a];
					if (generateT[index]) texcoord[a].y = planeT[index] ^ vertex[a];
				}
			}
			else
			{
				bool sgen = generateS[0];
				bool tgen = generateT[0];
				
				if (sgen & tgen)
				{
					for (machine a = 0; a < vertexCount; a++)
					{
						texcoord[a].x = planeS[0] ^ vertex[a];
						texcoord[a].y = planeT[0] ^ vertex[a];
					}
				}
				else if (sgen)
				{
					for (machine a = 0; a < vertexCount; a++) texcoord[a].x = planeS[0] ^ vertex[a];
				}
				else if (tgen)
				{
					for (machine a = 0; a < vertexCount; a++) texcoord[a].y = planeT[0] ^ vertex[a];
				}
			}
			
			return (true);
		}
	}
	
	return (false);
}

bool GeometryLevel::TransformTexcoords(const Transformable *transformable, const GeometryObject *object)
{
	int32 surfaceCount = object->GetSurfaceCount();
	
	unsigned_int32 size = (sizeof(Antivector4D) * 2 + 2) * surfaceCount;
	Buffer buffer(size);
	
	Antivector4D *planeS = static_cast<Antivector4D *>(*buffer);
	Antivector4D *planeT = planeS + surfaceCount;
	bool *transformS = reinterpret_cast<bool *>(planeT + surfaceCount);
	bool *transformT = transformS + surfaceCount;
	
	int32 transformCount = 0;
	for (machine a = 0; a < surfaceCount; a++)
	{
		const SurfaceData *surfaceData = object->GetSurfaceData(a);
		
		const Antivector4D& splane = surfaceData->textureAlignData[0].alignPlane;
		const Antivector4D& tplane = surfaceData->textureAlignData[1].alignPlane;
		
		if (surfaceData->textureAlignData[0].alignMode == kTextureAlignNatural)
		{
			if ((splane.x == 0.0F) && (tplane.x == 0.0F)) planeS[a].Set(splane.y, splane.z, 0.0F, splane.w);
			else if ((splane.y == 0.0F) && (tplane.y == 0.0F)) planeS[a].Set(splane.x, -splane.z, 0.0F, splane.w);
			else planeS[a].Set(splane.x, splane.y, 0.0F, splane.w);
			
			transformS[a] = true;
			transformCount++;
		}
		else
		{
			transformS[a] = false;
		}
		
		if (surfaceData->textureAlignData[1].alignMode == kTextureAlignNatural)
		{
			if ((splane.x == 0.0F) && (tplane.x == 0.0F)) planeT[a].Set(tplane.y, tplane.z, 0.0F, tplane.w);
			else if ((splane.y == 0.0F) && (tplane.y == 0.0F)) planeT[a].Set(tplane.x, -tplane.z, 0.0F, tplane.w);
			else planeT[a].Set(tplane.x, tplane.y, 0.0F, tplane.w);
			
			transformT[a] = true;
			transformCount++;
		}
		else
		{
			transformT[a] = false;
		}
	}
	
	if (transformCount != 0)
	{
		Point2D *texcoord = GetArray<Point2D>(kArrayTexture0);
		
		const unsigned_int16 *surfaceIndex = GetArray<unsigned_int16>(kArraySurfaceIndex);
		if (surfaceIndex)
		{
			for (machine a = 0; a < vertexCount; a++)
			{
				unsigned_int32 index = surfaceIndex[a];
				if (transformS[index]) texcoord[a].x = planeS[index] ^ texcoord[a];
				if (transformT[index]) texcoord[a].y = planeT[index] ^ texcoord[a];
			}
		}
		else
		{
			bool sxfrm = transformS[0];
			bool txfrm = transformT[0];
			
			if (sxfrm & txfrm)
			{
				for (machine a = 0; a < vertexCount; a++)
				{
					texcoord[a].x = planeS[0] ^ texcoord[a];
					texcoord[a].y = planeT[0] ^ texcoord[a];
				}
			}
			else if (sxfrm)
			{
				for (machine a = 0; a < vertexCount; a++) texcoord[a].x = planeS[0] ^ texcoord[a];
			}
			else if (txfrm)
			{
				for (machine a = 0; a < vertexCount; a++) texcoord[a].y = planeT[0] ^ texcoord[a];
			}
		}
		
		return (true);
	}
	
	return (false);
}

void GeometryLevel::CalculateEdgeArray(const unsigned_int32 *remapTable)
{
	Buffer buffer((vertexCount + GetArrayDescriptor(kArrayEdge)->elementCount) * 4);
	unsigned_int32 *firstEdge = static_cast<unsigned_int32 *>(*buffer);
	unsigned_int32 *nextEdge = firstEdge + vertexCount;
	
	for (machine a = 0; a < vertexCount; a++) firstEdge[a] = 0xFFFFFFFF;
	
	int32 faceCount = GetFaceCount();
	const Triangle *triangle = GetArray<Triangle>(kArrayFace);
	
	int32 edgeCount = 0;
	Edge *edgeArray = GetArray<Edge>(kArrayEdge);
	
	for (machine a = 0; a < faceCount; a++)
	{
		unsigned_int32 i1 = remapTable[triangle->index[2]];
		for (machine b = 0; b < 3; b++)
		{
			unsigned_int32 i2 = remapTable[triangle->index[b]];
			if (i1 < i2)
			{
				Edge *edge = &edgeArray[edgeCount];
				
				edge->vertexIndex[0] = (unsigned_int16) i1;
				edge->vertexIndex[1] = (unsigned_int16) i2;
				edge->faceIndex[0] = (unsigned_int16) a;
				edge->faceIndex[1] = (unsigned_int16) a;
				
				unsigned_int32 edgeIndex = firstEdge[i1];
				if (edgeIndex == 0xFFFFFFFF)
				{
					firstEdge[i1] = edgeCount;
				}
				else
				{
					for (;;)
					{
						unsigned_int32 index = nextEdge[edgeIndex];
						if (index == 0xFFFFFFFF)
						{
							nextEdge[edgeIndex] = edgeCount;
							break;
						}
						
						edgeIndex = index;
					}
				}
				
				nextEdge[edgeCount] = 0xFFFFFFFF;
				edgeCount++;
			}
			
			i1 = i2;
		}
		
		triangle++;
	}
	
	triangle = GetArray<Triangle>(kArrayFace);
	for (machine a = 0; a < faceCount; a++)
	{
		unsigned_int32 i1 = remapTable[triangle->index[2]];
		for (machine b = 0; b < 3; b++)
		{
			unsigned_int32 i2 = remapTable[triangle->index[b]];
			if (i1 > i2)
			{
				for (unsigned_int32 edgeIndex = firstEdge[i2]; edgeIndex != 0xFFFFFFFF; edgeIndex = nextEdge[edgeIndex])
				{
					Edge *edge = &edgeArray[edgeIndex];
					if ((edge->vertexIndex[1] == i1) && (edge->faceIndex[0] == edge->faceIndex[1]))
					{
						edge->faceIndex[1] = (unsigned_int16) a;
						break;
					}
				}
			}
			
			i1 = i2;
		}
		
		triangle++;
	}
	
	arrayBundle[kArrayEdge].descriptor.elementCount = edgeCount;
}

void GeometryLevel::BuildNormalArray(const GeometryLevel *inputLevel)
{
	ArrayDescriptor		desc[2];
	
	int32 vertCount = inputLevel->GetVertexCount();
	
	desc[0].identifier = kArrayNormal;
	desc[0].elementCount = vertCount;
	desc[0].elementSize = sizeof(Vector3D);
	desc[0].componentCount = 3;
	
	int32 arrayCount = 1;
	if (inputLevel->GetWeightData())
	{
		desc[1].identifier = kArraySharedIndex;
		desc[1].elementCount = inputLevel->GetWeightData() ? vertCount : 0;
		desc[1].elementSize = 2;
		desc[1].componentCount = 1;
		arrayCount = 2;
	}
	
	AllocateStorage(inputLevel, arrayCount, desc);
	CalculateNormalArray(inputLevel);
}

void GeometryLevel::CalculateNormalArray(const GeometryLevel *inputLevel)
{
	Box3D	bounds;
	
	if (!inputLevel) inputLevel = this;
	
	int32 vertCount = inputLevel->GetVertexCount();
	const Point3D *vertex = inputLevel->GetArray<Point3D>(kArrayVertex);
	
	bounds.Calculate(vertCount, vertex);
	GeometryOctree vertexOctree(bounds);
	
	Buffer buffer(vertCount * 4);
	unsigned_int32 *remapTable = static_cast<unsigned_int32 *>(*buffer);
	
	const unsigned_int16 *surfaceIndex = inputLevel->GetArray<unsigned_int16>(kArraySurfaceIndex);
	if (surfaceIndex)
	{
		unsigned_int32 currentSurface = surfaceIndex[0];
		
		for (machine a = 0; a < vertCount; a++)
		{
			unsigned_int32 surface = surfaceIndex[a];
			if (surface != currentSurface)
			{
				currentSurface = surface;
				vertexOctree.Purge();
			}
			
			const Point3D& position = vertex[a];
			GeometryOctree *octreeNode = vertexOctree.FindNodeContainingPoint(position);
			
			unsigned_int32 vertexIndex = a;
			int32 indexCount = octreeNode->GetIndexCount();
			const unsigned_int32 *indexArray = octreeNode->GetIndexArray();
			for (machine b = 0; b < indexCount; b++)
			{
				unsigned_int32 index = indexArray[b];
				if (vertex[index] == position)
				{
					vertexIndex = index;
					break;
				}
			}
			
			remapTable[a] = vertexIndex;
			if (vertexIndex == a) octreeNode->AddIndex(a);
		}
	}
	else
	{
		for (machine a = 0; a < vertCount; a++)
		{
			const Point3D& position = vertex[a];
			GeometryOctree *octreeNode = vertexOctree.FindNodeContainingPoint(position);
			
			unsigned_int32 vertexIndex = a;
			int32 indexCount = octreeNode->GetIndexCount();
			const unsigned_int32 *indexArray = octreeNode->GetIndexArray();
			for (machine b = 0; b < indexCount; b++)
			{
				unsigned_int32 index = indexArray[b];
				if (vertex[index] == position)
				{
					vertexIndex = index;
					break;
				}
			}
			
			remapTable[a] = vertexIndex;
			if (vertexIndex == a) octreeNode->AddIndex(a);
		}
	}
	
	const Triangle *triangle = GetArray<Triangle>(kArrayFace);
	int32 triangleCount = GetFaceCount();
	
	Vector3D *normal = GetArray<Vector3D>(kArrayNormal);
	MemoryMgr::ClearMemory(normal, vertCount * sizeof(Vector3D));
	
	for (machine a = 0; a < triangleCount; a++)
	{
		int32 i1 = remapTable[triangle->index[0]];
		int32 i2 = remapTable[triangle->index[1]];
		int32 i3 = remapTable[triangle->index[2]];
		
		const Point3D& v1 = vertex[i1];
		const Point3D& v2 = vertex[i2];
		const Point3D& v3 = vertex[i3];
		
		Vector3D nrml = (v2 - v1) % (v3 - v1);
		
		normal[i1] += nrml;
		normal[i2] += nrml;
		normal[i3] += nrml;
		
		triangle++;
	}
	
	unsigned_int16 *sharedIndex = GetArray<unsigned_int16>(kArraySharedIndex);
	if (sharedIndex)
	{
		for (machine a = 0; a < vertCount; a++)
		{
			unsigned_int32 index = remapTable[a];
			sharedIndex[a] = (unsigned_int16) index;
			
			if (index == a) normal[a].Normalize();
			else normal[a] = normal[index];
		}
	}
	else
	{
		for (machine a = 0; a < vertCount; a++)
		{
			unsigned_int32 index = remapTable[a];
			if (index == a) normal[a].Normalize();
			else normal[a] = normal[index];
		}
	}
}

void GeometryLevel::BuildTangentArray(const GeometryLevel *inputLevel)
{
	ArrayDescriptor		desc[2];
	
	int32 vertCount = inputLevel->GetVertexCount();
	
	desc[0].identifier = kArrayTangent;
	desc[0].elementCount = vertCount;
	desc[0].elementSize = sizeof(Vector4D);
	desc[0].componentCount = 4;
	
	int32 arrayCount = 1;
	if (inputLevel->GetWeightData())
	{
		desc[1].identifier = kArrayHandedness;
		desc[1].elementCount = inputLevel->GetWeightData() ? vertCount : 0;
		desc[1].elementSize = 4;
		desc[1].componentCount = 1;
		arrayCount = 2;
	}
	
	AllocateStorage(inputLevel, arrayCount, desc);
	CalculateTangentArray(inputLevel);
}

void GeometryLevel::CalculateTangentArray(const GeometryLevel *inputLevel)
{
	Box3D	bounds;
	
	if (!inputLevel) inputLevel = this;
	
	int32 vertCount = inputLevel->GetVertexCount();
	const Point3D *vertex = inputLevel->GetArray<Point3D>(kArrayVertex);
	const Vector3D *normal = inputLevel->GetArray<Vector3D>(kArrayNormal);
	
	bounds.Calculate(vertCount, vertex);
	GeometryOctree vertexOctree(bounds);
	
	Buffer buffer(vertCount * (4 + (2 * sizeof(Vector3D))));
	unsigned_int32 *remapTable = static_cast<unsigned_int32 *>(*buffer);
	
	const unsigned_int16 *surfaceIndex = inputLevel->GetArray<unsigned_int16>(kArraySurfaceIndex);
	if (surfaceIndex)
	{
		int32 currentSurface = surfaceIndex[0];
		
		for (machine a = 0; a < vertCount; a++)
		{
			int32 surface = surfaceIndex[a];
			if (surface != currentSurface)
			{
				currentSurface = surface;
				vertexOctree.Purge();
			}
			
			const Point3D& position = vertex[a];
			GeometryOctree *octreeNode = vertexOctree.FindNodeContainingPoint(position);
			
			unsigned_int32 vertexIndex = a;
			int32 indexCount = octreeNode->GetIndexCount();
			const unsigned_int32 *indexArray = octreeNode->GetIndexArray();
			for (machine b = 0; b < indexCount; b++)
			{
				unsigned_int32 index = indexArray[b];
				if ((vertex[index] == position) && (normal[index] == normal[a]))
				{
					vertexIndex = index;
					break;
				}
			}
			
			remapTable[a] = vertexIndex;
			if (vertexIndex == a) octreeNode->AddIndex(a);
		}
	}
	else
	{
		for (machine a = 0; a < vertCount; a++)
		{
			const Point3D& position = vertex[a];
			GeometryOctree *octreeNode = vertexOctree.FindNodeContainingPoint(position);
			
			unsigned_int32 vertexIndex = a;
			int32 indexCount = octreeNode->GetIndexCount();
			const unsigned_int32 *indexArray = octreeNode->GetIndexArray();
			for (machine b = 0; b < indexCount; b++)
			{
				unsigned_int32 index = indexArray[b];
				if ((vertex[index] == position) && (normal[index] == normal[a]))
				{
					vertexIndex = index;
					break;
				}
			}
			
			remapTable[a] = vertexIndex;
			if (vertexIndex == a) octreeNode->AddIndex(a);
		}
	}
	
	Vector3D *tangent = reinterpret_cast<Vector3D *>(remapTable + vertCount);
	Vector3D *bitangent = tangent + vertCount;
	MemoryMgr::ClearMemory(tangent, vertCount * (2 * sizeof(Vector3D)));
	
	const Point2D *texcoord = inputLevel->GetArray<Point2D>(kArrayTexture0);
	const Triangle *triangle = inputLevel->GetArray<Triangle>(kArrayFace);
	
	int32 faceCount = inputLevel->GetFaceCount();
	for (machine a = 0; a < faceCount; a++)
	{
		int32 i1 = triangle->index[0];
		int32 i2 = triangle->index[1];
		int32 i3 = triangle->index[2];
		
		const Point3D& v1 = vertex[i1];
		const Point3D& v2 = vertex[i2];
		const Point3D& v3 = vertex[i3];
		
		const Point2D& w1 = texcoord[i1];
		const Point2D& w2 = texcoord[i2];
		const Point2D& w3 = texcoord[i3];
		
		float x1 = v2.x - v1.x;
		float x2 = v3.x - v1.x;
		float y1 = v2.y - v1.y;
		float y2 = v3.y - v1.y;
		float z1 = v2.z - v1.z;
		float z2 = v3.z - v1.z;
		
		float s1 = w2.x - w1.x;
		float s2 = w3.x - w1.x;
		float t1 = w2.y - w1.y;
		float t2 = w3.y - w1.y;
		
		Vector3D sdir(t2 * x1 - t1 * x2, t2 * y1 - t1 * y2, t2 * z1 - t1 * z2);
		Vector3D tdir(s1 * x2 - s2 * x1, s1 * y2 - s2 * y1, s1 * z2 - s2 * z1);
		
		float r = s1 * t2 - s2 * t1;
		if (Fabs(r) > K::min_float)
		{
			r = 1.0F / r;
			sdir *= r;
			tdir *= r;
		}
		else
		{
			Vector3D nrml = ((v2 - v1) % (v3 - v1)).Normalize();
			
			if (s1 == s2)
			{
				tdir.Set(x1, y1, z1);
				sdir = tdir % nrml;
			}
			else
			{
				sdir.Set(x1, y1, z1);
				tdir = nrml % sdir;
			}
		}
		
		tangent[i1] += sdir;
		tangent[i2] += sdir;
		tangent[i3] += sdir;
		
		bitangent[i1] += tdir;
		bitangent[i2] += tdir;
		bitangent[i3] += tdir;
		
		triangle++;
	}
	
	if (GetArrayDescriptor(kArrayTangent)->componentCount == 4)
	{
		Vector4D *tangentArray = GetArray<Vector4D>(kArrayTangent);
		float *handedness = GetArray<float>(kArrayHandedness);
		
		for (machine a = 0; a < vertCount; a++)
		{
			const Vector3D& nrml = normal[a];
			const Vector3D& tang = tangent[a];
			tangentArray[a].GetVector3D() = (tang - nrml * (nrml * tang)).Normalize();
			float w = (nrml % tang * bitangent[a] < 0.0F) ? -1.0F : 1.0F;
			
			tangentArray[a].w = w;
			if (handedness) handedness[a] = tangentArray[a].w;
		}
		
		for (machine a = 0; a < vertCount; a++)
		{
			unsigned_int32 index = remapTable[a];
			if (index != a)
			{
				const Vector4D& t1 = tangentArray[index];
				const Vector4D& t2 = tangentArray[a];
				
				if (t1.w == t2.w)
				{
					Vector3D sum = t1.GetVector3D() + t2.GetVector3D();
					if (t2.GetVector3D() * sum * InverseMag(sum) > K::sqrt_2_over_2)
					{
						tangentArray[index].GetVector3D() = sum;
						continue;
					}
				}
				
				remapTable[a] = a;
			}
		}
		
		for (machine a = 0; a < vertCount; a++)
		{
			unsigned_int32 index = remapTable[a];
			if (index == a) tangentArray[a].GetVector3D().Normalize();
			else tangentArray[a] = tangentArray[index];
		}
	}
	else
	{
		Vector3D *tangentArray = GetArray<Vector3D>(kArrayTangent);
		
		for (machine a = 0; a < vertCount; a++)
		{
			const Vector3D& nrml = normal[a];
			const Vector3D& tang = tangent[a];
			tangentArray[a] = (tang - nrml * (nrml * tang)).Normalize();
		}
		
		for (machine a = 0; a < vertCount; a++)
		{
			unsigned_int32 index = remapTable[a];
			if (index != a)
			{
				const Vector3D& t1 = tangentArray[index];
				const Vector3D& t2 = tangentArray[a];
				
				Vector3D sum = t1 + t2;
				if (t2 * sum * InverseMag(sum) > K::sqrt_2_over_2)
				{
					tangentArray[index] = sum;
					continue;
				}
				
				remapTable[a] = a;
			}
		}
		
		for (machine a = 0; a < vertCount; a++)
		{
			unsigned_int32 index = remapTable[a];
			if (index == a) tangentArray[a].Normalize();
			else tangentArray[a] = tangentArray[index];
		}
	}
}

void GeometryLevel::BuildPlaneArray(const GeometryLevel *inputLevel)
{
	ArrayDescriptor		desc[2];
	
	int32 faceCount = inputLevel->GetFaceCount();
	
	Buffer buffer(faceCount * (sizeof(Antivector4D) + 4));
	Antivector4D *planeArray = static_cast<Antivector4D *>(*buffer);
	int32 *planeIndexArray = reinterpret_cast<int32 *>(planeArray + faceCount);
	
	const Triangle *triangle = inputLevel->GetArray<Triangle>(kArrayFace);
	const Point3D *vertex = inputLevel->GetArray<Point3D>(kArrayVertex);
	
	GeometryOctree planeOctree(Zero3D, Vector3D(1.0F, 1.0F, 1.0F));
	int32 planeCount = 0;
	
	for (machine a = 0; a < faceCount; a++)
	{
		const Point3D& p1 = vertex[triangle->index[0]];
		const Point3D& p2 = vertex[triangle->index[1]];
		const Point3D& p3 = vertex[triangle->index[2]];
		
		Vector3D normal = ((p2 - p1) % (p3 - p1)).Normalize();
		Antivector4D plane(normal, p1);
		
		GeometryOctree *octreeNode = planeOctree.FindNodeContainingPoint(normal);
		
		unsigned_int32 planeIndex = planeCount;
		int32 indexCount = octreeNode->GetIndexCount();
		const unsigned_int32 *indexArray = octreeNode->GetIndexArray();
		for (machine b = 0; b < indexCount; b++)
		{
			unsigned_int32 index = indexArray[b];
			const Antivector4D& prev = planeArray[index];
			if ((prev.GetAntivector3D() * plane.GetAntivector3D() > 0.999F) && (Fabs(prev.w - plane.w) < 0.001F))
			{
				planeIndex = index;
				break;
			}
		}
		
		planeIndexArray[a] = planeIndex;
		if (planeIndex == planeCount)
		{
			octreeNode->AddIndex(planeCount);
			planeArray[planeCount] = plane;
			planeCount++;
		}
		
		triangle++;
	}
	
	desc[0].identifier = kArrayPlane;
	desc[0].elementCount = planeCount;
	desc[0].elementSize = sizeof(Antivector4D);
	desc[0].componentCount = 4;
	
	desc[1].identifier = kArrayPlaneIndex;
	desc[1].elementCount = faceCount;
	desc[1].elementSize = 2;
	desc[1].componentCount = 1;
	
	AllocateStorage(inputLevel, 2, desc);
	
	MemoryMgr::CopyMemory(planeArray, arrayBundle[kArrayPlane].pointer, planeCount * sizeof(Antivector4D));
	
	unsigned_int16 *planeIndex = GetArray<unsigned_int16>(kArrayPlaneIndex);
	for (machine a = 0; a < faceCount; a++) planeIndex[a] = (unsigned_int16) planeIndexArray[a];
}

void GeometryLevel::BuildEdgeArray(const GeometryLevel *inputLevel)
{
	ArrayDescriptor		desc;
	Box3D				bounds;
	
	int32 faceCount = inputLevel->GetFaceCount();
	int32 vertCount = inputLevel->GetVertexCount();
	const Point3D *vertex = inputLevel->GetArray<Point3D>(kArrayVertex);
	
	bounds.Calculate(vertCount, vertex);
	GeometryOctree vertexOctree(bounds);
	
	Buffer buffer(vertCount * 8 + faceCount * (3 * (sizeof(Edge) + 4)));
	unsigned_int32 *remapTable = static_cast<unsigned_int32 *>(*buffer);
	
	for (machine a = 0; a < vertCount; a++)
	{
		const Point3D& position = vertex[a];
		GeometryOctree *octreeNode = vertexOctree.FindNodeContainingPoint(position);
		
		unsigned_int32 vertexIndex = a;
		int32 indexCount = octreeNode->GetIndexCount();
		const unsigned_int32 *indexArray = octreeNode->GetIndexArray();
		for (machine b = 0; b < indexCount; b++)
		{
			unsigned_int32 index = indexArray[b];
			if (vertex[index] == position)
			{
				vertexIndex = index;
				break;
			}
		}
		
		remapTable[a] = vertexIndex;
		if (vertexIndex == a) octreeNode->AddIndex(a);
	}
	
	unsigned_int32 *firstEdge = remapTable + vertCount;
	unsigned_int32 *nextEdge = firstEdge + vertCount;
	for (machine a = 0; a < vertCount; a++) firstEdge[a] = 0xFFFFFFFF;
	
	Edge *edgeArray = reinterpret_cast<Edge *>(nextEdge + faceCount * 3);
	const Triangle *triangle = inputLevel->GetArray<Triangle>(kArrayFace);
	
	int32 edgeCount = 0;
	
	const unsigned_int16 *planeIndex = inputLevel->GetArray<unsigned_int16>(kArrayPlaneIndex);
	if (planeIndex)
	{
		for (machine a = 0; a < faceCount; a++)
		{
			unsigned_int32 i1 = remapTable[triangle->index[2]];
			for (machine b = 0; b < 3; b++)
			{
				unsigned_int32 i2 = remapTable[triangle->index[b]];
				if (i1 < i2)
				{
					Edge *edge = &edgeArray[edgeCount];
					
					edge->vertexIndex[0] = (unsigned_int16) i1;
					edge->vertexIndex[1] = (unsigned_int16) i2;
					
					unsigned_int32 plane = planeIndex[a];
					edge->faceIndex[0] = (unsigned_int16) plane;
					edge->faceIndex[1] = (unsigned_int16) plane;
					
					unsigned_int32 edgeIndex = firstEdge[i1];
					if (edgeIndex == 0xFFFFFFFF)
					{
						firstEdge[i1] = edgeCount;
					}
					else
					{
						for (;;)
						{
							unsigned_int32 index = nextEdge[edgeIndex];
							if (index == 0xFFFFFFFF)
							{
								nextEdge[edgeIndex] = edgeCount;
								break;
							}
							
							edgeIndex = index;
						}
					}
					
					nextEdge[edgeCount] = 0xFFFFFFFF;
					edgeCount++;
				}
				
				i1 = i2;
			}
			
			triangle++;
		}
	}
	else
	{
		for (machine a = 0; a < faceCount; a++)
		{
			unsigned_int32 i1 = remapTable[triangle->index[2]];
			for (machine b = 0; b < 3; b++)
			{
				unsigned_int32 i2 = remapTable[triangle->index[b]];
				if (i1 < i2)
				{
					Edge *edge = &edgeArray[edgeCount];
					
					edge->vertexIndex[0] = (unsigned_int16) i1;
					edge->vertexIndex[1] = (unsigned_int16) i2;
					edge->faceIndex[0] = (unsigned_int16) a;
					edge->faceIndex[1] = (unsigned_int16) a;
					
					unsigned_int32 edgeIndex = firstEdge[i1];
					if (edgeIndex == 0xFFFFFFFF)
					{
						firstEdge[i1] = edgeCount;
					}
					else
					{
						for (;;)
						{
							unsigned_int32 index = nextEdge[edgeIndex];
							if (index == 0xFFFFFFFF)
							{
								nextEdge[edgeIndex] = edgeCount;
								break;
							}
							
							edgeIndex = index;
						}
					}
					
					nextEdge[edgeCount] = 0xFFFFFFFF;
					edgeCount++;
				}
				
				i1 = i2;
			}
			
			triangle++;
		}
	}
	
	triangle = inputLevel->GetArray<Triangle>(kArrayFace);
	int32 finalEdgeCount = edgeCount;
	
	if (planeIndex)
	{
		for (machine a = 0; a < faceCount; a++)
		{
			unsigned_int32 i1 = remapTable[triangle->index[2]];
			for (machine b = 0; b < 3; b++)
			{
				unsigned_int32 i2 = remapTable[triangle->index[b]];
				if (i1 > i2)
				{
					for (unsigned_int32 edgeIndex = firstEdge[i2]; edgeIndex != 0xFFFFFFFF; edgeIndex = nextEdge[edgeIndex])
					{
						Edge *edge = &edgeArray[edgeIndex];
						if (edge->vertexIndex[1] == i1)
						{
							unsigned_int32 plane1 = edge->faceIndex[0];
							if (plane1 == edge->faceIndex[1])
							{
								unsigned_int32 plane2 = planeIndex[a];
								if (plane1 != plane2)
								{
									edge->faceIndex[1] = (unsigned_int16) plane2;
								}
								else
								{
									edge->faceIndex[0] = 0xFFFF;
									finalEdgeCount--;
								}
								
								goto next1;
							}
						}
					}
					
					Edge *edge = &edgeArray[edgeCount];
					edge->vertexIndex[0] = (unsigned_int16) i2;
					edge->vertexIndex[1] = (unsigned_int16) i1;
					
					unsigned_int32 plane = planeIndex[a];
					edge->faceIndex[0] = (unsigned_int16) plane;
					edge->faceIndex[1] = (unsigned_int16) plane;
					
					edgeCount++;
					finalEdgeCount++;
				}
				
				next1:
				i1 = i2;
			}
			
			triangle++;
		}
	}
	else
	{
		for (machine a = 0; a < faceCount; a++)
		{
			unsigned_int32 i1 = remapTable[triangle->index[2]];
			for (machine b = 0; b < 3; b++)
			{
				unsigned_int32 i2 = remapTable[triangle->index[b]];
				if (i1 > i2)
				{
					for (unsigned_int32 edgeIndex = firstEdge[i2]; edgeIndex != 0xFFFFFFFF; edgeIndex = nextEdge[edgeIndex])
					{
						Edge *edge = &edgeArray[edgeIndex];
						if (edge->vertexIndex[1] == i1)
						{
							unsigned_int32 faceIndex = edge->faceIndex[0];
							if (faceIndex == edge->faceIndex[1])
							{
								if (faceIndex != a)
								{
									edge->faceIndex[1] = (unsigned_int16) a;
								}
								else
								{
									edge->faceIndex[0] = 0xFFFF;
									finalEdgeCount--;
								}
								
								goto next2;
							}
						}
					}
					
					Edge *edge = &edgeArray[edgeCount];
					edge->vertexIndex[0] = (unsigned_int16) i2;
					edge->vertexIndex[1] = (unsigned_int16) i1;
					edge->faceIndex[0] = (unsigned_int16) a;
					edge->faceIndex[1] = (unsigned_int16) a;
					
					edgeCount++;
					finalEdgeCount++;
				}
				
				next2:
				i1 = i2;
			}
			
			triangle++;
		}
	}
	
	desc.identifier = kArrayEdge;
	desc.elementCount = finalEdgeCount;
	desc.elementSize = sizeof(Edge);
	desc.componentCount = 1;
	
	AllocateStorage(inputLevel, 1, &desc);
	
	Edge *finalEdge = GetArray<Edge>(kArrayEdge);
	for (machine a = 0; a < edgeCount; a++)
	{
		const Edge *edge = &edgeArray[a];
		if (edge->faceIndex[0] != 0xFFFF) *finalEdge++ = *edge;
	}
}

void GeometryLevel::BuildSegmentArray(const GeometryLevel *inputLevel, int32 surfaceCount, const SurfaceData *surfaceData)
{
	ArrayDescriptor		desc[3];
	
	const unsigned_int16 *surfaceIndex = inputLevel->GetArray<unsigned_int16>(kArraySurfaceIndex);
	if ((surfaceCount > 1) && (surfaceIndex))
	{
		Buffer buffer(surfaceCount * 16);
		unsigned_int32 *surfaceRemapTable = static_cast<unsigned_int32 *>(*buffer);
		unsigned_int32 *surfaceBackmapTable = surfaceRemapTable + surfaceCount;
		
		for (machine a = 0; a < surfaceCount; a++)
		{
			int32 index = surfaceData[a].materialIndex;
			
			unsigned_int32 count = 0;
			for (machine b = 0; b < a; b++) count += (surfaceData[b].materialIndex <= index);
			for (machine b = a + 1; b < surfaceCount; b++) count += (surfaceData[b].materialIndex < index);
			
			surfaceRemapTable[a] = count;
			surfaceBackmapTable[count] = a;
		}
		
		int32 segmentCount = 1;
		unsigned_int32 prevMaterialIndex = surfaceData[surfaceBackmapTable[0]].materialIndex;
		for (machine a = 1; a < surfaceCount; a++)
		{
			unsigned_int32 materialIndex = surfaceData[surfaceBackmapTable[a]].materialIndex;
			if (materialIndex != prevMaterialIndex)
			{
				segmentCount++;
				prevMaterialIndex = materialIndex;
			}
		}
		
		if (segmentCount > 1)
		{
			int32 faceCount = inputLevel->GetFaceCount();
			
			desc[0].identifier = kArrayFace;
			desc[0].elementCount = faceCount;
			desc[0].elementSize = sizeof(Triangle);
			desc[0].componentCount = 1;
			
			desc[1].identifier = kArraySegment;
			desc[1].elementCount = segmentCount;
			desc[1].elementSize = sizeof(SegmentData);
			desc[1].componentCount = 1;
			
			int32 arrayCount = 2;
			const unsigned_int16 *inputPlaneIndex = inputLevel->GetArray<unsigned_int16>(kArrayPlaneIndex);
			if (inputPlaneIndex)
			{
				desc[2].identifier = kArrayPlaneIndex;
				desc[2].elementCount = faceCount;
				desc[2].elementSize = 2;
				desc[2].componentCount = 1;
				arrayCount = 3;
			}
			
			AllocateStorage(inputLevel, arrayCount, desc);
			
			int32 *triangleCountTable = reinterpret_cast<int32 *>(surfaceBackmapTable + surfaceCount);
			for (machine a = 0; a < surfaceCount; a++) triangleCountTable[a] = 0;
			
			const Triangle *inputTriangle = inputLevel->GetArray<Triangle>(kArrayFace);
			for (machine a = 0; a < faceCount; a++) triangleCountTable[surfaceIndex[inputTriangle[a].index[0]]]++;
			
			SegmentData *segmentData = GetArray<SegmentData>(kArraySegment);
			prevMaterialIndex = surfaceData[surfaceBackmapTable[0]].materialIndex;
			
			int32 *triangleStartTable = triangleCountTable + surfaceCount;
			triangleStartTable[surfaceBackmapTable[0]] = 0;
			
			int32 segmentFaceStart = 0;
			int32 segmentFaceCount = triangleCountTable[surfaceBackmapTable[0]];
			int32 triangleStart = segmentFaceCount;
			
			for (machine a = 1; a < surfaceCount; a++)
			{
				unsigned_int32 index = surfaceBackmapTable[a];
				triangleStartTable[index] = triangleStart;
				
				unsigned_int32 materialIndex = surfaceData[surfaceBackmapTable[a]].materialIndex;
				if (materialIndex != prevMaterialIndex)
				{
					segmentData->materialIndex = prevMaterialIndex;
					segmentData->faceStart = segmentFaceStart;
					segmentData->faceCount = segmentFaceCount;
					segmentData++;
					
					prevMaterialIndex = materialIndex;
					segmentFaceStart += segmentFaceCount;
					segmentFaceCount = 0;
				}
				
				int32 count = triangleCountTable[index];
				triangleStart += count;
				segmentFaceCount += count;
			}
			
			segmentData->materialIndex = prevMaterialIndex;
			segmentData->faceStart = segmentFaceStart;
			segmentData->faceCount = segmentFaceCount;
			
			for (machine a = 0; a < surfaceCount; a++) triangleCountTable[a] = 0;
			Triangle *outputTriangle = GetArray<Triangle>(kArrayFace);
			
			if (inputPlaneIndex)
			{
				unsigned_int16 *outputPlaneIndex = GetArray<unsigned_int16>(kArrayPlaneIndex);
				
				for (machine a = 0; a < faceCount; a++)
				{
					const Triangle& triangle = inputTriangle[a];
					unsigned_int32 index = surfaceIndex[triangle.index[0]];
					
					int32 count = triangleCountTable[index];
					unsigned_int32 i = triangleStartTable[index] + count;
					
					outputTriangle[i] = triangle;
					outputPlaneIndex[i] = inputPlaneIndex[a];
					
					triangleCountTable[index] = count + 1;
				}
			}
			else
			{
				unsigned_int32 *faceRemapTable = new unsigned_int32[faceCount];
				
				for (machine a = 0; a < faceCount; a++)
				{
					const Triangle& triangle = inputTriangle[a];
					unsigned_int32 index = surfaceIndex[triangle.index[0]];
					
					int32 count = triangleCountTable[index];
					unsigned_int32 i = triangleStartTable[index] + count;
					
					outputTriangle[i] = triangle;
					faceRemapTable[a] = i;
					
					triangleCountTable[index] = count + 1;
				}
				
				Edge *edge = GetArray<Edge>(kArrayEdge);
				if (edge)
				{
					int32 edgeCount = GetArrayDescriptor(kArrayEdge)->elementCount;
					for (machine a = 0; a < edgeCount; a++)
					{
						edge->faceIndex[0] = (unsigned_int16) faceRemapTable[edge->faceIndex[0]];
						edge->faceIndex[1] = (unsigned_int16) faceRemapTable[edge->faceIndex[1]];
						edge++;
					}
				}
				
				delete[] faceRemapTable;
			}
			
			return;
		}
	}
	
	desc[0].identifier = kArraySegment;
	desc[0].elementCount = 0;
	desc[0].elementSize = sizeof(SegmentData);
	desc[0].componentCount = 1;
	
	AllocateStorage(inputLevel, 1, desc);
}

void GeometryLevel::BuildTexcoordArray(const GeometryLevel *inputLevel, const Transformable *transformable, const GeometryObject *object)
{
	ArrayDescriptor		desc;
	
	int32 vertCount = inputLevel->GetVertexCount();
	
	desc.identifier = kArrayTexture0;
	desc.elementCount = vertCount;
	desc.elementSize = sizeof(Point2D);
	desc.componentCount = 2;
	
	AllocateStorage(inputLevel, 1, &desc);
	GenerateTexcoords(transformable, object);
}

void GeometryLevel::SimplifyBoundaryEdges(const GeometryLevel *inputLevel)
{
	int32				vertCount;
	int32				triangleCount;
	Point2D				*inputTexcoord[kMaxGeometryTexcoordCount];
	ArrayDescriptor		desc[kMaxGeometryTexcoordCount + 4];
	
	TopoMesh topoMesh(inputLevel);
	
	const Point3D *inputVertex = inputLevel->GetArray<Point3D>(kArrayVertex);
	while (topoMesh.SimplifyBoundaryEdges(inputVertex)) {}
	
	Buffer buffer(inputLevel->GetVertexCount() * 4);
	unsigned_int32 *remapTable = static_cast<unsigned_int32 *>(*buffer);
	topoMesh.CompactMesh(&vertCount, &triangleCount, remapTable);
	
	desc[0].identifier = kArrayVertex;
	desc[0].elementCount = vertCount;
	desc[0].elementSize = sizeof(Point3D);
	desc[0].componentCount = 3;
	
	int32 arrayCount = 1;
	const Vector3D *inputNormal = inputLevel->GetArray<Vector3D>(kArrayNormal);
	if (inputNormal)
	{
		desc[1].identifier = kArrayNormal;
		desc[1].elementCount = vertCount;
		desc[1].elementSize = sizeof(Vector3D);
		desc[1].componentCount = 3;
		
		arrayCount = 2;
	}
	
	const Color4C *inputColor = inputLevel->GetArray<Color4C>(kArrayColor0);
	if (inputColor)
	{
		desc[arrayCount].identifier = kArrayColor0;
		desc[arrayCount].elementCount = vertCount;
		desc[arrayCount].elementSize = 4;
		desc[arrayCount].componentCount = 1;
		arrayCount++;
	}
	
	for (machine a = 0; a < kMaxGeometryTexcoordCount; a++)
	{
		inputTexcoord[a] = inputLevel->GetArray<Point2D>(kArrayTexture0 + a);
		if (inputTexcoord[a])
		{
			desc[arrayCount].identifier = kArrayTexture0 + a;
			desc[arrayCount].elementCount = vertCount;
			desc[arrayCount].elementSize = sizeof(Point2D);
			desc[arrayCount].componentCount = 2;
			arrayCount++;
		}
	}
	
	const unsigned_int16 *inputSurfaceIndex = inputLevel->GetArray<unsigned_int16>(kArraySurfaceIndex);
	if (inputSurfaceIndex)
	{
		desc[arrayCount].identifier = kArraySurfaceIndex;
		desc[arrayCount].elementCount = vertCount;
		desc[arrayCount].elementSize = 2;
		desc[arrayCount].componentCount = 1;
		arrayCount++;
	}
	
	desc[arrayCount].identifier = kArrayFace;
	desc[arrayCount].elementCount = triangleCount;
	desc[arrayCount].elementSize = sizeof(Triangle);
	desc[arrayCount].componentCount = 1;
	arrayCount++;
	
	AllocateStorage(vertCount, arrayCount, desc);
	
	Point3D *outputVertex = GetArray<Point3D>(kArrayVertex);
	for (machine a = 0; a < vertCount; a++) outputVertex[a] = inputVertex[remapTable[a]];
	
	if (inputNormal)
	{
		Vector3D *outputNormal = GetArray<Vector3D>(kArrayNormal);
		for (machine a = 0; a < vertCount; a++) outputNormal[a] = inputNormal[remapTable[a]];
	}
	
	if (inputColor)
	{
		Color4C *outputColor = GetArray<Color4C>(kArrayColor0);
		for (machine a = 0; a < vertCount; a++) outputColor[a] = inputColor[remapTable[a]];
	}
	
	for (machine a = 0; a < kMaxGeometryTexcoordCount; a++)
	{
		if (inputTexcoord[a])
		{
			Point2D *outputTexcoord = GetArray<Point2D>(kArrayTexture0 + a);
			for (machine b = 0; b < vertCount; b++) outputTexcoord[b] = inputTexcoord[a][remapTable[b]];
		}
	}
	
	if (inputSurfaceIndex)
	{
		unsigned_int16 *outputSurfaceIndex = GetArray<unsigned_int16>(kArraySurfaceIndex);
		for (machine a = 0; a < vertCount; a++) outputSurfaceIndex[a] = inputSurfaceIndex[remapTable[a]];
	}
	
	Triangle *outputTriangle = GetArray<Triangle>(kArrayFace);
	const TopoFace *topoFace = topoMesh.GetFirstFace();
	while (topoFace)
	{
		for (machine a = 0; a < 3; a++)
		{
			const TopoVertex *vertex = topoFace->GetVertex(a);
			outputTriangle->index[a] = (unsigned_int16) vertex->GetIndex();
		}
		
		outputTriangle++;
		topoFace = topoFace->Next();
	}
}

void GeometryLevel::OptimizeMesh(const GeometryLevel *inputLevel, float collapseThreshold, int32 baseTriangleCount)
{
	int32				vertCount;
	int32				triangleCount;
	Point2D				*inputTexcoord[kMaxGeometryTexcoordCount];
	ArrayDescriptor		desc[kMaxGeometryTexcoordCount + 8];
	
	int32 totalTriangleCount = inputLevel->GetFaceCount();
	if (baseTriangleCount < 0) baseTriangleCount = totalTriangleCount;
	TopoMesh topoMesh(inputLevel, baseTriangleCount);
	
	const Point3D *inputVertex = inputLevel->GetArray<Point3D>(kArrayVertex);
	topoMesh.OptimizeMesh(inputVertex, collapseThreshold);
	
	Buffer buffer(inputLevel->GetVertexCount() * 4);
	unsigned_int32 *remapTable = static_cast<unsigned_int32 *>(*buffer);
	topoMesh.CompactMesh(&vertCount, &triangleCount, remapTable);
	
	desc[0].identifier = kArrayVertex;
	desc[0].elementCount = vertCount;
	desc[0].elementSize = sizeof(Point3D);
	desc[0].componentCount = 3;
	
	int32 arrayCount = 1;
	
	const Point3D *inputPosition1 = inputLevel->GetArray<Point3D>(kArrayPosition1);
	if (inputPosition1)
	{
		desc[1].identifier = kArrayPosition1;
		desc[1].elementCount = vertCount;
		desc[1].elementSize = sizeof(Point3D);
		desc[1].componentCount = 3;
		
		arrayCount = 2;
	}
	
	const Vector3D *inputNormal = inputLevel->GetArray<Vector3D>(kArrayNormal);
	if (inputNormal)
	{
		desc[arrayCount].identifier = kArrayNormal;
		desc[arrayCount].elementCount = vertCount;
		desc[arrayCount].elementSize = sizeof(Vector3D);
		desc[arrayCount].componentCount = 3;
		arrayCount++;
	}
	
	const Color4C *inputColor0 = inputLevel->GetArray<Color4C>(kArrayColor0);
	if (inputColor0)
	{
		desc[arrayCount].identifier = kArrayColor0;
		desc[arrayCount].elementCount = vertCount;
		desc[arrayCount].elementSize = 4;
		desc[arrayCount].componentCount = 1;
		arrayCount++;
	}
	
	const Color4C *inputColor1 = inputLevel->GetArray<Color4C>(kArrayColor1);
	if (inputColor1)
	{
		desc[arrayCount].identifier = kArrayColor1;
		desc[arrayCount].elementCount = vertCount;
		desc[arrayCount].elementSize = 4;
		desc[arrayCount].componentCount = 1;
		arrayCount++;
	}
	
	const Color4C *inputColor2 = inputLevel->GetArray<Color4C>(kArrayColor2);
	if (inputColor2)
	{
		desc[arrayCount].identifier = kArrayColor2;
		desc[arrayCount].elementCount = vertCount;
		desc[arrayCount].elementSize = 4;
		desc[arrayCount].componentCount = 1;
		arrayCount++;
	}
	
	for (machine a = 0; a < kMaxGeometryTexcoordCount; a++)
	{
		inputTexcoord[a] = inputLevel->GetArray<Point2D>(kArrayTexture0 + a);
		if (inputTexcoord[a])
		{
			desc[arrayCount].identifier = kArrayTexture0 + a;
			desc[arrayCount].elementCount = vertCount;
			desc[arrayCount].elementSize = sizeof(Point2D);
			desc[arrayCount].componentCount = 2;
			arrayCount++;
		}
	}
	
	const unsigned_int16 *inputSurfaceIndex = inputLevel->GetArray<unsigned_int16>(kArraySurfaceIndex);
	if (inputSurfaceIndex)
	{
		desc[arrayCount].identifier = kArraySurfaceIndex;
		desc[arrayCount].elementCount = vertCount;
		desc[arrayCount].elementSize = 2;
		desc[arrayCount].componentCount = 1;
		arrayCount++;
	}
	
	desc[arrayCount].identifier = kArrayFace;
	desc[arrayCount].elementCount = triangleCount + totalTriangleCount - baseTriangleCount;
	desc[arrayCount].elementSize = sizeof(Triangle);
	desc[arrayCount].componentCount = 1;
	arrayCount++;
	
	AllocateStorage(vertCount, arrayCount, desc);
	
	Point3D *outputVertex = GetArray<Point3D>(kArrayVertex);
	for (machine a = 0; a < vertCount; a++) outputVertex[a] = inputVertex[remapTable[a]];
	
	if (inputPosition1)
	{
		Point3D *outputPosition1 = GetArray<Point3D>(kArrayPosition1);
		for (machine a = 0; a < vertCount; a++) outputPosition1[a] = inputPosition1[remapTable[a]];
	}
	
	if (inputNormal)
	{
		Vector3D *outputNormal = GetArray<Vector3D>(kArrayNormal);
		for (machine a = 0; a < vertCount; a++) outputNormal[a] = inputNormal[remapTable[a]];
	}
	
	if (inputColor0)
	{
		Color4C *outputColor = GetArray<Color4C>(kArrayColor0);
		for (machine a = 0; a < vertCount; a++) outputColor[a] = inputColor0[remapTable[a]];
	}
	
	if (inputColor1)
	{
		Color4C *outputColor = GetArray<Color4C>(kArrayColor1);
		for (machine a = 0; a < vertCount; a++) outputColor[a] = inputColor1[remapTable[a]];
	}
	
	if (inputColor2)
	{
		Color4C *outputColor = GetArray<Color4C>(kArrayColor2);
		for (machine a = 0; a < vertCount; a++) outputColor[a] = inputColor2[remapTable[a]];
	}
	
	for (machine a = 0; a < kMaxGeometryTexcoordCount; a++)
	{
		if (inputTexcoord[a])
		{
			Point2D *outputTexcoord = GetArray<Point2D>(kArrayTexture0 + a);
			for (machine b = 0; b < vertCount; b++) outputTexcoord[b] = inputTexcoord[a][remapTable[b]];
		}
	}
	
	if (inputSurfaceIndex)
	{
		unsigned_int16 *outputSurfaceIndex = GetArray<unsigned_int16>(kArraySurfaceIndex);
		for (machine a = 0; a < vertCount; a++) outputSurfaceIndex[a] = inputSurfaceIndex[remapTable[a]];
	}
	
	Triangle *outputTriangle = GetArray<Triangle>(kArrayFace);
	const TopoFace *topoFace = topoMesh.GetFirstFace();
	while (topoFace)
	{
		for (machine a = 0; a < 3; a++)
		{
			const TopoVertex *vertex = topoFace->GetVertex(a);
			outputTriangle->index[a] = (unsigned_int16) vertex->GetIndex();
		}
		
		outputTriangle++;
		topoFace = topoFace->Next();
	}
	
	const Triangle *inputTriangle = inputLevel->GetArray<Triangle>(kArrayFace);
	int32 vertexDelta = inputLevel->GetVertexCount() - vertCount;
	
	for (machine a = baseTriangleCount; a < totalTriangleCount; a++)
	{
		outputTriangle->Set(inputTriangle[a].index[0] - vertexDelta, inputTriangle[a].index[1] - vertexDelta, inputTriangle[a].index[2] - vertexDelta);
		outputTriangle++;
	}
}

float GeometryLevel::CalculateVolume(void) const
{
	float volume = 0.0F;
	
	const Point3D *vertex = GetArray<Point3D>(kArrayVertex);
	const Triangle *triangle = GetArray<Triangle>(kArrayFace);
	
	int32 faceCount = GetFaceCount();
	for (machine a = 0; a < faceCount; a++)
	{
		const Point3D& p1 = vertex[triangle->index[0]];
		const Point3D& p2 = vertex[triangle->index[1]];
		const Point3D& p3 = vertex[triangle->index[2]];
		
		volume += p1 * (p2 % p3);
		
		triangle++;
	}
	
	return (volume * K::one_over_6);
}

// ZYURVUR
