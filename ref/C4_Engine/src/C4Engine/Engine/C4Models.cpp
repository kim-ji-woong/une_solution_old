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


#include "C4World.h"
#include "C4Configuration.h"


using namespace C4;


namespace C4
{
	template class Registrable<Model, ModelRegistration>;
}


ResourceDescriptor ModelResource::descriptor("mdl", 0, 0, "C4/missing");


ModelResource::ModelResource(const char *name, ResourceCatalog *catalog) : Resource<ModelResource>(name, catalog)
{
}

ModelResource::~ModelResource()
{
}


Bone::Bone() : Node(kNodeBone)
{
	skeletonRoot = nullptr;
	boundingBox.Set(Zero3D, Zero3D);
}

Bone::Bone(const Bone& bone) : Node(bone)
{
	skeletonRoot = nullptr;
	boundingBox.Set(Zero3D, Zero3D);
}

Bone::~Bone()
{
}

Node *Bone::Replicate(void) const
{
	return (new Bone(*this));
}

void Bone::Pack(Packer& data, unsigned_int32 packFlags) const
{
	Node::Pack(data, packFlags);
	
	data << TerminatorChunk;
}

void Bone::Unpack(Unpacker& data, unsigned_int32 unpackFlags)
{
	Node::Unpack(data, unpackFlags);
	UnpackChunkList<Bone>(data, unpackFlags);
}

bool Bone::UnpackChunk(const ChunkHeader *chunkHeader, Unpacker& data, unsigned_int32 unpackFlags)
{
	#if C4LEGACY
	
		switch (chunkHeader->chunkType)
		{
			case 'HASH':
			case 'INDX':
			{
				unsigned_int32	hash;
				
				data >> hash;
				SetNodeHash(hash);
				return (true);
			}
		}
	
	#endif
	
	return (false);
}

void Bone::CalculatePostTransform(void)
{
	modelTransform = skeletonRoot->GetInverseWorldTransform() * GetWorldTransform();
}

bool Bone::CalculateBoundingBox(Box3D *box) const
{
	//box->Set(boundingBox.min - Vector3D(0.5F, 0.5F, 0.5F), boundingBox.max + Vector3D(0.5F, 0.5F, 0.5F));
	*box = boundingBox;
	return (true);
}

bool Bone::CalculateBoundingSphere(BoundingSphere *sphere) const
{
	sphere->SetCenter((boundingBox.min + boundingBox.max) * 0.5F);
	sphere->SetRadius(Magnitude(boundingBox.max - boundingBox.min) * 0.5F);
	return (true);
}

void Bone::Preprocess(void)
{ 
	Node::Preprocess();
	 
	Node *node = GetSuperNode(); 
	while (node) 
	{
		if (node->GetNodeType() == kNodeModel) 
		{
			skeletonRoot = node;
			SetActiveUpdateFlags(kUpdateTransform | kUpdatePostTransform | kUpdateBoundingSphere);
			break; 
		}
		
		node = node->GetSuperNode();
	} 
	
	if ((!skeletonRoot) && (GetManipulator()))
	{
		node = GetSuperNode();
		while (node)
		{
			if (node->GetNodeType() == kNodeZone)
			{
				skeletonRoot = node;
				SetActiveUpdateFlags(kUpdateTransform | kUpdatePostTransform | kUpdateBoundingSphere);
				break;
			}
			
			node = node->GetSuperNode();
		}
	}
}


SkinController::SkinController() :
		Controller(kControllerSkin),
		skinUpdateJob(&SkinUpdateJob, &FinalizeUpdate, this),
		dynamicVertexBuffer(kVertexBufferAttribute | kVertexBufferDynamic),
		dynamicVertexBufferObserver(this, &SkinController::FillDynamicVertexBuffer)
{
	skinStorage = nullptr;
	
	motionBlurFlag = false;
	handednessFlag = false;
}

SkinController::SkinController(const SkinController& skinController) :
		Controller(skinController),
		skinUpdateJob(&SkinUpdateJob, &FinalizeUpdate, this),
		dynamicVertexBuffer(kVertexBufferAttribute | kVertexBufferDynamic),
		dynamicVertexBufferObserver(this, &SkinController::FillDynamicVertexBuffer)
{
	skinStorage = nullptr;
	
	motionBlurFlag = false;
	handednessFlag = false;
}

SkinController::~SkinController()
{
	delete[] skinStorage;
}

Controller *SkinController::Replicate(void) const
{
	return (new SkinController(*this));
}

bool SkinController::ValidNode(const Node *node)
{
	if (node->GetNodeType() == kNodeGeometry)
	{
		const Geometry *geometry = static_cast<const Geometry *>(node);
		if (geometry->GetGeometryType() == kGeometryMesh)
		{
			return (geometry->GetObject()->GetGeometryLevel(0)->GetWeightData() != nullptr);
		}
	}
	
	return (false);
}

void SkinController::Pack(Packer& data, unsigned_int32 packFlags) const
{
	Controller::Pack(data, packFlags);
	
	data << TerminatorChunk;
}

void SkinController::Unpack(Unpacker& data, unsigned_int32 unpackFlags)
{
	Controller::Unpack(data, unpackFlags);
	UnpackChunkList<SkinController>(data, unpackFlags);
}

bool SkinController::UnpackChunk(const ChunkHeader *chunkHeader, Unpacker& data, unsigned_int32 unpackFlags)
{
	return (false);
}

void SkinController::Preprocess(void)
{
	SetControllerFlags(GetControllerFlags() | kControllerLocal);
	Controller::Preprocess();
	
	MeshGeometry *geometry = GetTargetNode();
	geometry->SetShaderFlags(geometry->GetShaderFlags() | kShaderNormalizeBasisVectors);
	geometry->SetDynamicArrayFlags((1 << kArrayVertex) | (1 << kArrayPrevious) | (1 << kArrayNormal) | (1 << kArrayTangent));
	geometry->SetMotionBlurBox(&skinBoundingBox);
	
	GeometryObject *object = geometry->GetObject();
	object->SetGeometryFlags(object->GetGeometryFlags() | kGeometryDynamic);
	
	const GeometryLevel *geometryLevel = object->GetGeometryLevel(0);
	int32 vertexCount = geometryLevel->GetVertexCount();
	
	dynamicVertexBuffer.Initialize(vertexCount * sizeof(DynamicVertex), sizeof(DynamicVertex));
	geometry->SetVertexBuffer(kVertexBufferDynamicArray, &dynamicVertexBuffer);
	
	int32 faceCount = geometryLevel->GetFaceCount();
	int32 boneCount = geometryLevel->GetArrayDescriptor(kArrayNodeHash)->elementCount;
	skinBoneCount = boneCount;
	
	unsigned_int32 size = (sizeof(Transform4D) + sizeof(Bone *)) * boneCount;
	size += (sizeof(Point3D) * 2 + sizeof(Vector3D) + sizeof(Vector4D)) * vertexCount;
	
	bool shadow = !(object->GetGeometryFlags() & kGeometryShadowInhibit);
	if (shadow) size += sizeof(Antivector4D) * faceCount;
	
	delete[] skinStorage;
	char *pointer = new char[size];
	skinStorage = pointer;
	
	if (boneCount > 0)
	{
		transformTable = reinterpret_cast<Transform4D *>(pointer);
		pointer += sizeof(Transform4D) * boneCount;
		
		skinBoneTable = reinterpret_cast<Bone **>(pointer);
		pointer += sizeof(Bone *) * boneCount;
		
		Node *rootNode = geometry->GetSuperNode();
		while (rootNode)
		{
			NodeType type = rootNode->GetNodeType();
			if ((type == kNodeModel) || (type == kNodeZone)) break;
			rootNode = rootNode->GetSuperNode();
		}
		
		for (machine a = 0; a < boneCount; a++) skinBoneTable[a] = nullptr;
		
		if (rootNode)
		{
			geometry->SetVisibilityProc(&Node::BoxVisible);
			geometry->SetOcclusionProc(&Node::BoxOccluded);
			geometry->SetPostTransformProc(&CalculatePostTransform);
			
			const unsigned_int32 *nodeHash = geometryLevel->GetArray<unsigned_int32>(kArrayNodeHash);
			
			if (rootNode->GetNodeType() == kNodeModel)
			{
				const Model *model = static_cast<const Model *>(rootNode);
				for (machine a = 0; a < boneCount; a++)
				{
					Node *node = model->FindNode(nodeHash[a]);
					if ((node) && (node->GetNodeType() == kNodeBone))
					{
						skinBoneTable[a] = static_cast<Bone *>(node);
					}
				}
			}
			else
			{
				for (machine a = 0; a < boneCount; a++)
				{
					unsigned_int32 hash = nodeHash[a];
					
					Node *node = rootNode->GetFirstSubnode();
					while (node)
					{
						NodeType type = node->GetNodeType();
						if (type == kNodeBone)
						{
							if (node->GetNodeHash() == hash)
							{
								skinBoneTable[a] = static_cast<Bone *>(node);
								break;
							}
						}
						else if (type == kNodeModel)
						{
							node = rootNode->GetNextLevelNode(node);
							continue;
						}
						
						node = rootNode->GetNextNode(node);
					}
				}
			}
		}
	}
	
	skinVertexArray[0].descriptor.identifier = kArrayVertex;
	skinVertexArray[0].descriptor.elementCount = vertexCount;
	skinVertexArray[0].descriptor.elementSize = sizeof(Point3D);
	skinVertexArray[0].descriptor.componentCount = 3;
	skinVertexArray[0].pointer = pointer;
	
	pointer += sizeof(Point3D) * vertexCount;
	
	skinVertexArray[1].descriptor.identifier = kArrayVertex;
	skinVertexArray[1].descriptor.elementCount = vertexCount;
	skinVertexArray[1].descriptor.elementSize = sizeof(Point3D);
	skinVertexArray[1].descriptor.componentCount = 3;
	skinVertexArray[1].pointer = pointer;
	
	pointer += sizeof(Point3D) * vertexCount;
	
	skinNormalArray.descriptor.identifier = kArrayNormal;
	skinNormalArray.descriptor.elementCount = vertexCount;
	skinNormalArray.descriptor.elementSize = sizeof(Vector3D);
	skinNormalArray.descriptor.componentCount = 3;
	skinNormalArray.pointer = pointer;
	
	pointer += sizeof(Vector3D) * vertexCount;
	
	skinTangentArray.descriptor.identifier = kArrayTangent;
	skinTangentArray.descriptor.elementCount = vertexCount;
	skinTangentArray.descriptor.elementSize = sizeof(Vector4D);
	skinTangentArray.descriptor.componentCount = 4;
	skinTangentArray.pointer = pointer;
	
	pointer += sizeof(Vector4D) * vertexCount;
	
	if (shadow)
	{
		skinPlaneArray.descriptor.identifier = kArrayPlane;
		skinPlaneArray.descriptor.elementCount = faceCount;
		skinPlaneArray.descriptor.elementSize = sizeof(Antivector4D);
		skinPlaneArray.descriptor.componentCount = 4;
		skinPlaneArray.pointer = pointer;
	}
	
	geometry->SetAttributeArray(kArrayNormal, static_cast<Vector3D *>(skinNormalArray.pointer));
	geometry->SetAttributeArray(kArrayTangent, static_cast<Vector4D *>(skinTangentArray.pointer));
	
	geometry->SetArrayBundle(kArrayNormal, &skinNormalArray);
	geometry->SetArrayBundle(kArrayTangent, &skinTangentArray);
	if (shadow) geometry->SetArrayBundle(kArrayPlane, &skinPlaneArray);
	
	CalculateBoneBoundingBoxes();
}

void SkinController::Neutralize(void)
{
	dynamicVertexBuffer.Deactivate();
	
	delete[] skinStorage;
	skinStorage = nullptr;
	
	Controller::Neutralize();
}

void SkinController::CalculateBoneBoundingBoxes(void) const
{
	const GeometryLevel *geometryLevel = GetTargetNode()->GetObject()->GetGeometryLevel(0);
	const Transform4D *transform = geometryLevel->GetArray<Transform4D>(kArrayInverseBindTransform);
	const Point3D *bindPosition = geometryLevel->GetArray<Point3D>(kArrayVertex);
	const WeightedVertex *weightedVertex = geometryLevel->GetWeightData();
	
	int32 boneCount = skinBoneCount;
	Box3D *box = new Box3D[boneCount];
	for (machine a = 0; a < boneCount; a++) box[a].Set(Zero3D, Zero3D);
	
	int32 vertexCount = geometryLevel->GetVertexCount();
	for (machine a = 0; a < vertexCount; a++)
	{
		int32 boneCount = weightedVertex->boneCount;
		const BoneWeight *boneWeight = weightedVertex->boneWeight;
		
		#if C4SIMD
		
			float4 position = SimdLoadUnaligned(&bindPosition->x);
		
		#endif
		
		do
		{
			int32 index = boneWeight->boneIndex;
			Box3D& b = box[index];
			
			#if C4SIMD
			
				const float *m = &transform[index](0,0);
				float4 p = SimdTransformPoint3D(SimdLoad(m, 0), SimdLoad(m, 4), SimdLoad(m, 8), SimdLoad(m, 12), position);
				SimdStore3D(SimdMin(SimdLoadUnaligned(&b.min.x), p), &b.min.x);
				SimdStore3D(SimdMax(SimdLoadUnaligned(&b.max.x), p), &b.max.x);
			
			#else
			
				Point3D p = transform[index] * *bindPosition;
				b.min.x = Fmin(b.min.x, p.x);
				b.min.y = Fmin(b.min.y, p.y);
				b.min.z = Fmin(b.min.z, p.z);
				b.max.x = Fmax(b.max.x, p.x);
				b.max.y = Fmax(b.max.y, p.y);
				b.max.z = Fmax(b.max.z, p.z);
			
			#endif
			
			boneWeight++;
		} while (--boneCount > 0);
		
		weightedVertex = reinterpret_cast<const WeightedVertex *>(boneWeight);
		bindPosition++;
	}
	
	for (machine a = 0; a < boneCount; a++)
	{
		Bone *bone = skinBoneTable[a];
		if (bone) bone->SetBoundingBox(box[a]);
	}
	
	delete[] box;
}

void SkinController::CalculatePostTransform(MeshGeometry *meshGeometry)
{
	SkinController *controller = static_cast<SkinController *>(meshGeometry->GetController());
	
	int32 boneCount = controller->skinBoneCount;
	const Bone *const *boneTable = controller->skinBoneTable;
	
	for (machine a = 0; a < boneCount; a++)
	{
		const Bone *bone = boneTable[a];
		if (bone)
		{
			Box3D box = bone->GetWorldBoundingBox();
			
			for (++a; a < boneCount; a++)
			{
				bone = boneTable[a];
				if (bone) box.Union(bone->GetWorldBoundingBox());
			}
			
			meshGeometry->SetWorldBoundingBox(box);
			break;
		}
	}
}

void SkinController::StopMotion(void)
{
	motionBlurFlag = false;
	
	MeshGeometry *geometry = GetTargetNode();
	ArrayBundle *vertexBundle = &skinVertexArray[vertexParity];
	
	geometry->SetArrayBundle(kArrayVertex, vertexBundle);
	geometry->SetArrayBundle(kArrayPrevious, vertexBundle);
	
	const Point3D *vertex = static_cast<Point3D *>(vertexBundle->pointer);
	geometry->SetAttributeArray(kArrayVertex, vertex);
	geometry->SetAttributeArray(kArrayPrevious, vertex);
	
	geometry->SetAttributeOffset(kArrayVertex, 0);
	geometry->SetAttributeOffset(kArrayPrevious, 0);
}

void SkinController::Update(void)
{
	Controller::Update();
	
	MeshGeometry *geometry = GetTargetNode();
	if (geometry->GetObject()->GetGeometryLevel(geometry->GetDetailLevel())->GetWeightData())
	{
		unsigned_int32 parity = vertexParity;
		
		ArrayBundle *vertexBundle = &skinVertexArray[parity];
		geometry->SetArrayBundle(kArrayVertex, vertexBundle);
		geometry->SetAttributeArray(kArrayVertex, static_cast<Point3D *>(vertexBundle->pointer));
		geometry->SetAttributeOffset(kArrayVertex, 0);
		
		if (motionBlurFlag)
		{
			ArrayBundle *previousBundle = &skinVertexArray[parity ^ 1];
			geometry->SetArrayBundle(kArrayPrevious, previousBundle);
			geometry->SetAttributeArray(kArrayPrevious, static_cast<Point3D *>(previousBundle->pointer));
			geometry->SetAttributeOffset(kArrayPrevious, sizeof(Point3D));
		}
		else
		{
			motionBlurFlag = true;
			
			geometry->SetArrayBundle(kArrayPrevious, vertexBundle);
			geometry->SetAttributeArray(kArrayPrevious, static_cast<Point3D *>(vertexBundle->pointer));
			geometry->SetAttributeOffset(kArrayPrevious, 0);
		}
		
		geometry->GetWorld()->SubmitWorldJob(&skinUpdateJob);
	}
}

void SkinController::SkinUpdateJob(Job *job, void *cookie)
{
	SkinController *controller = static_cast<SkinController *>(cookie);
	
	MeshGeometry *geometry = controller->GetTargetNode();
	const GeometryObject *object = geometry->GetObject();
	const GeometryLevel *geometryLevel = object->GetGeometryLevel(geometry->GetDetailLevel());
	const Transform4D *transform = geometryLevel->GetArray<Transform4D>(kArrayInverseBindTransform);
	
	const Bone *const *boneTable = controller->skinBoneTable;
	Transform4D *transformTable = controller->transformTable;
	
	int32 boneCount = controller->skinBoneCount;
	for (machine a = 0; a < boneCount; a++)
	{
		const Bone *bone = boneTable[a];
		transformTable[a] = (bone) ? bone->GetModelTransform() * transform[a] : transform[a];
	}
	
	Point3D *restrict vertex = static_cast<Point3D *>(controller->skinVertexArray[controller->vertexParity].pointer);
	Vector3D *restrict normal = static_cast<Vector3D *>(controller->skinNormalArray.pointer);
	Vector4D *restrict tangent = static_cast<Vector4D *>(controller->skinTangentArray.pointer);
	
	const Point3D *bindPosition = geometryLevel->GetArray<Point3D>(kArrayVertex);
	const Vector3D *bindNormal = geometryLevel->GetArray<Vector3D>(kArrayNormal);
	const Vector4D *bindTangent = geometryLevel->GetArray<Vector4D>(kArrayTangent);
	const WeightedVertex *weightedVertex = geometryLevel->GetWeightData();
	
	#if C4SIMD
	
		float4 minBounds = SimdLoadSmearScalar(&K::infinity);
		float4 maxBounds = SimdLoadSmearScalar(&K::minus_infinity);
	
	#else
	
		Point3D minBounds(K::infinity, K::infinity, K::infinity);
		Point3D maxBounds(K::minus_infinity, K::minus_infinity, K::minus_infinity);
	
	#endif
	
	int32 vertexCount = geometryLevel->GetVertexCount();
	for (machine a = 0; a < vertexCount; a++)
	{
		#if C4SIMD
		
			float4 bpos = SimdLoadUnaligned(&bindPosition->x);
			float4 bnrm = SimdLoadUnaligned(&bindNormal->x);
			float4 btan = SimdLoadUnaligned(&bindTangent->x);
			
			int32 boneCount = weightedVertex->boneCount;
			const BoneWeight *boneWeight = weightedVertex->boneWeight;
			
			const float *transform = &transformTable[boneWeight->boneIndex](0,0);
			register float4 c1 = SimdLoad(transform, 0);
			register float4 c2 = SimdLoad(transform, 4);
			register float4 c3 = SimdLoad(transform, 8);
			register float4 c4 = SimdLoad(transform, 12);
			
			float total = boneWeight->weight;
			float4 weight = SimdLoadSmearScalar(&boneWeight->weight);
			float4 vrtx = SimdMul(SimdTransformPoint3D(c1, c2, c3, c4, bpos), weight);
			float4 nrml = SimdMul(SimdTransformVector3D(c1, c2, c3, bnrm), weight);
			float4 tang = SimdMul(SimdTransformVector3D(c1, c2, c3, btan), weight);
			
			while (boneWeight++, --boneCount)
			{
				transform = &transformTable[boneWeight->boneIndex](0,0);
				c1 = SimdLoad(transform, 0);
				c2 = SimdLoad(transform, 4);
				c3 = SimdLoad(transform, 8);
				c4 = SimdLoad(transform, 12);
				
				total += boneWeight->weight;
				weight = SimdLoadSmearScalar(&boneWeight->weight);
				vrtx = SimdMadd(SimdTransformPoint3D(c1, c2, c3, c4, bpos), weight, vrtx);
				nrml = SimdMadd(SimdTransformVector3D(c1, c2, c3, bnrm), weight, nrml);
				tang = SimdMadd(SimdTransformVector3D(c1, c2, c3, btan), weight, tang);
			}
			
			SimdStore3D(vrtx, &vertex->x);
			SimdStore3D(nrml, &normal->x);
			SimdStore3D(tang, &tangent->x);
			SimdStoreW(btan, &tangent->x, 3);
			
			minBounds = SimdMin(minBounds, vrtx);
			maxBounds = SimdMax(maxBounds, vrtx);
		
		#else
		
			int32 boneCount = weightedVertex->boneCount;
			const BoneWeight *boneWeight = weightedVertex->boneWeight;
			
			float weight = boneWeight->weight;
			const Transform4D *transform = &transformTable[boneWeight->boneIndex];
			*vertex = *transform * *bindPosition * weight;
			*normal = *transform * *bindNormal * weight;
			tangent->Set(*transform * bindTangent->GetVector3D() * weight, bindTangent->w);
			
			while (boneWeight++, --boneCount)
			{
				weight = boneWeight->weight;
				transform = &transformTable[boneWeight->boneIndex];
				*vertex += *transform * *bindPosition * weight;
				*normal += *transform * *bindNormal * weight;
				tangent->GetVector3D() += *transform * bindTangent->GetVector3D() * weight;
			}
			
			minBounds.x = Fmin(minBounds.x, vertex->x);
			minBounds.y = Fmin(minBounds.y, vertex->y);
			minBounds.z = Fmin(minBounds.z, vertex->z);
			maxBounds.x = Fmax(maxBounds.x, vertex->x);
			maxBounds.y = Fmax(maxBounds.y, vertex->y);
			maxBounds.z = Fmax(maxBounds.z, vertex->z);
		
		#endif
		
		weightedVertex = reinterpret_cast<const WeightedVertex *>(boneWeight);
		
		vertex++;
		normal++;
		tangent++;
		bindPosition++;
		bindNormal++;
		bindTangent++;
	}
	
	#if C4SIMD
	
		SimdStore3D(minBounds, &controller->skinBoundingBox.min.x);
		SimdStore3D(maxBounds, &controller->skinBoundingBox.max.x);
	
	#else
	
		controller->skinBoundingBox.Set(minBounds, maxBounds);
	
	#endif
	
	if (!(object->GetGeometryFlags() & kGeometryShadowInhibit))
	{
		int32 triangleCount = geometryLevel->GetFaceCount();
		const Triangle *triangle = geometryLevel->GetArray<Triangle>(kArrayFace);
		
		vertex = static_cast<Point3D *>(controller->skinVertexArray[controller->vertexParity].pointer);
		Antivector4D *restrict plane = static_cast<Antivector4D *>(controller->skinPlaneArray.pointer);
		
		for (machine a = 0; a < triangleCount; a++)
		{
			unsigned_int32 i1 = triangle->index[0];
			unsigned_int32 i2 = triangle->index[1];
			unsigned_int32 i3 = triangle->index[2];
			plane->Set(vertex[i1], vertex[i2], vertex[i3]);
			
			plane++;
			triangle++;
		}
	}
}

void SkinController::FinalizeUpdate(Job *job, void *cookie)
{
	SkinController *controller = static_cast<SkinController *>(cookie);
	controller->vertexParity ^= 1;
	
	if (controller->dynamicVertexBuffer.Active())
	{
		controller->dynamicVertexBuffer.SetObserver(&controller->dynamicVertexBufferObserver);
		controller->FillDynamicVertexBuffer(&controller->dynamicVertexBuffer);
	}
}

void SkinController::FillDynamicVertexBuffer(VertexBuffer *vertexBuffer)
{
	DynamicVertex *restrict buffer = static_cast<DynamicVertex *>(vertexBuffer->BeginUpdate());
	
	const Geometry *geometry = GetTargetNode();
	int32 vertexCount = geometry->GetVertexCount();
	
	const Point3D *vertex = geometry->GetAttributeArray<Point3D>(kArrayVertex);
	const Point3D *previous = geometry->GetAttributeArray<Point3D>(kArrayPrevious);
	const Vector3D *normal = geometry->GetAttributeArray<Vector3D>(kArrayNormal);
	const Vector4D *tangent = geometry->GetAttributeArray<Vector4D>(kArrayTangent);
	
	for (machine a = 0; a < vertexCount; a++)
	{
		buffer[a].vertex = vertex[a];
		buffer[a].previous = previous[a];
		buffer[a].normal = normal[a];
		buffer[a].tangent = tangent[a];
	}
	
	vertexBuffer->EndUpdate();
}

void SkinController::SetDetailLevel(int32 level)
{
	vertexParity = 0;
	motionBlurFlag = false;
	handednessFlag = true;
	
	Geometry *geometry = static_cast<Geometry *>(GetTargetNode());
	const GeometryObject *object = geometry->GetObject();
	
	geometry->SetAttributeOffset(kArrayNormal, sizeof(Point3D) * 2);
	geometry->SetAttributeOffset(kArrayTangent, sizeof(Point3D) * 2 + sizeof(Vector3D));
	
	const GeometryLevel *geometryLevel = object->GetGeometryLevel(level);
	int32 vertexCount = geometryLevel->GetVertexCount();
	
	skinVertexArray[0].descriptor.elementCount = vertexCount;
	skinVertexArray[1].descriptor.elementCount = vertexCount;
	skinNormalArray.descriptor.elementCount = vertexCount;
	skinTangentArray.descriptor.elementCount = vertexCount;
	skinPlaneArray.descriptor.elementCount = geometryLevel->GetFaceCount();
	
	Controller::SetDetailLevel(level);
}


ModelRegistration::ModelRegistration(ModelType type, const char *name, const char *rsrcName, unsigned_int32 flags, ControllerType contType) : Registration<Model, ModelRegistration>(type)
{
	modelFlags = flags;
	modelName = name;
	resourceName = rsrcName;
	controllerType = contType;
	
	prototypeModel = nullptr;
	if (modelFlags & kModelPrecache) LoadPrototype();
}

ModelRegistration::~ModelRegistration()
{
	cloneList.RemoveAll();
	delete prototypeModel;
}

Model *ModelRegistration::Construct(void) const
{
	return (new Model(GetModelType()));
}

void ModelRegistration::LoadPrototype(void)
{
	prototypeModel = Model::New(resourceName, GetModelType());
	if (prototypeModel)
	{
		const Node *node = prototypeModel->GetFirstSubnode();
		while (node)
		{
			if (node->GetNodeType() == kNodeGeometry) static_cast<const Geometry *>(node)->GetObject()->SetPrototypeFlag();
			node = prototypeModel->GetNextNode(node);
		}
	}
}

void ModelRegistration::Reload(void)
{
	delete prototypeModel;
	prototypeModel = nullptr;
	
	if (modelFlags & kModelPrecache) LoadPrototype();
}

Model *ModelRegistration::Clone(Model *model)
{
	if (!prototypeModel)
	{
		LoadPrototype();
		if (!prototypeModel) return (nullptr);
	}
	
	if (model) prototypeModel->CloneSubtree(model);
	else model = static_cast<Model *>(prototypeModel->Clone());
	
	cloneList.Append(model);
	return (model);
}

void ModelRegistration::Retire(Model *model)
{
	if (cloneList.Member(model))
	{
		cloneList.Remove(model);
		if ((cloneList.Empty()) && (!(modelFlags & kModelPrecache)))
		{
			delete prototypeModel;
			prototypeModel = nullptr;
		}
	}
}


Model::Model(ModelType type) : Node(kNodeModel)
{
	modelType = type;
	modelState = 0;
	
	rootAnimator = nullptr;
	modelHashTable = nullptr;
	animatedNodeCount = 0;
}

Model::Model(const Model& model) : Node(model)
{
	modelType = model.modelType;
	modelState = 0;
	
	rootAnimator = nullptr;
	modelHashTable = nullptr;
	animatedNodeCount = 0;
}

Model::~Model()
{
	delete[] modelHashTable;
	
	if (ListElement<Model>::GetOwningList())
	{
		ModelRegistration *registration = FindRegistration(modelType);
		if (registration) registration->Retire(this);
	}
}

Node *Model::Replicate(void) const
{
	return (new Model(*this));
}

Model *Model::Construct(Unpacker& data, unsigned_int32 unpackFlags)
{
	ModelType type = data.GetType();
	if (type != kModelGeneric) return (new Model(type));
	return (new GenericModel);
}

void Model::PackType(Packer& data) const
{
	Node::PackType(data);
	data << modelType;
}

void Model::Pack(Packer& data, unsigned_int32 packFlags) const
{
	Node::Pack(data, packFlags);
	
	data << TerminatorChunk;
}

void Model::Unpack(Unpacker& data, unsigned_int32 unpackFlags)
{
	Node::Unpack(data, unpackFlags);
	UnpackChunkList<Model>(data, unpackFlags);
}

bool Model::UnpackChunk(const ChunkHeader *chunkHeader, Unpacker& data, unsigned_int32 unpackFlags)
{
	#if C4LEGACY
	
		switch (chunkHeader->chunkType)
		{
			case 'TYPE':
				
				data >> modelType;
				return (true);
		}
	
	#endif
	
	return (false);
}

void Model::Load(World *world)
{
	if (!(modelState & kModelLoaded))
	{
		ModelRegistration *registration = FindRegistration(modelType);
		if (registration)
		{
			registration->Clone(this);
			if (!Enabled()) Disable();
		}
	}
}

void Model::Unload(void)
{
	modelState &= ~kModelLoaded;
	ListElement<Model>::Detach();
	PurgeSubtree();
}

void Model::Preprocess(void)
{
	unsigned_int16		bucketSize[kModelHashBucketCount];
	
	unsigned_int32 updateFlags = GetActiveUpdateFlags() & ~kUpdateBoundingSphere;
	
	if (!GetManipulator())
	{
		Node *super = GetSuperNode();
		if (super)
		{
			World *world = super->GetWorld();
			if (world)
			{
				Load(world);
				InitTransform();
				
				if (super->GetNodeType() == kNodeZone) updateFlags |= kUpdateVisibility;
			}
		}
	}
	
	SetActiveUpdateFlags(updateFlags);
	
	delete[] modelHashTable;
	modelHashTable = nullptr;
	
	int32 subnodeCount = 0;
	int32 animatedCount = 0;
	for (machine a = 0; a < kModelHashBucketCount; a++) bucketSize[a] = 0;
	
	Node *node = GetFirstSubnode();
	while (node)
	{
		if (node->GetNodeType() == kNodeModel)
		{
			node = GetNextLevelNode(node);
		}
		else
		{
			unsigned_int32 hash = node->GetNodeHash();
			if (hash != 0)
			{
				subnodeCount++;
				bucketSize[hash & (kModelHashBucketCount - 1)]++;
				
				if (!(node->GetNodeFlags() & kNodeAnimateInhibit)) animatedCount++;
			}
			
			node = GetNextNode(node);
		}
	}
	
	animatedNodeCount = animatedCount;
	if (subnodeCount + animatedCount != 0)
	{
		modelHashTable = new Node *[subnodeCount + animatedCount];
		animatedNodeTable = modelHashTable + subnodeCount;
		
		unsigned_int32 start = 0;
		for (machine a = 0; a < kModelHashBucketCount; a++)
		{
			unsigned_int32 count = bucketSize[a];
			bucketSize[a] = 0;
			
			hashBucket[a].count = (unsigned_int16) count;
			hashBucket[a].start = (unsigned_int16) start;
			start += count;
		}
		
		animatedCount = 0;
		node = GetFirstSubnode();
		while (node)
		{
			if (node->GetNodeType() == kNodeModel)
			{
				node = GetNextLevelNode(node);
			}
			else
			{
				unsigned_int32 hash = node->GetNodeHash();
				if (hash != 0)
				{
					hash &= kModelHashBucketCount - 1;
					modelHashTable[hashBucket[hash].start + bucketSize[hash]++] = node;
					
					if (!(node->GetNodeFlags() & kNodeAnimateInhibit)) animatedNodeTable[animatedCount++] = node;
				}
				
				node = GetNextNode(node);
			}
		}
	}
	
	SetNodeFlags(GetNodeFlags() | (kNodeDynamicVisibility | kNodeVisibilitySite));
	Node::Preprocess();
	BondVisibility();
}

void Model::Neutralize(void)
{
	Unload();
	
	delete[] modelHashTable;
	modelHashTable = nullptr;
	animatedNodeCount = 0;
	
	SetActiveUpdateFlags(GetActiveUpdateFlags() & ~kUpdateVisibility);
	Node::Neutralize();
}

Node *Model::FindNode(unsigned_int32 hash) const
{
	const HashBucket *bucket = &hashBucket[hash & (kModelHashBucketCount - 1)];
	Node *const *nodeTable = modelHashTable + bucket->start;
	unsigned_int32 count = bucket->count;
	
	for (unsigned_machine a = 0; a < count; a++)
	{
		Node *node = nodeTable[a];
		if (node->GetNodeHash() == hash) return (node);
	}
	
	return (nullptr);
}

int32 Model::GetAnimationIndex(const Node *node) const
{
	int32 count = animatedNodeCount;
	for (machine a = 0; a < count; a++)
	{
		if (animatedNodeTable[a] == node) return (a);
	}
	
	return (-1);
}

void Model::SetRootAnimator(Animator *animator)
{
	rootAnimator = animator;
	if (animator)
	{
		animator->Preprocess();
		animator->Invalidate();
	}
}

void Model::Animate(void)
{
	if (rootAnimator)
	{
		rootAnimator->Premove();
		
		if (rootAnimator)
		{
			rootAnimator->Update();
			rootAnimator->Move();
			
			const AnimatorTransform *const *outputTable = rootAnimator->GetOutputTable();
			if (outputTable)
			{
				int32 start = rootAnimator->GetNodeStart();
				int32 count = rootAnimator->GetNodeCount();
				
				Node *const *targetTable = animatedNodeTable + start;
				for (machine a = 0; a < count; a++)
				{
					const AnimatorTransform *transform = outputTable[a];
					if (transform) targetTable[a]->SetNodeTransform(transform->rotation.GetRotationScaleMatrix(), transform->position);
				}
			}
			
			Invalidate();
			
			Node *node = GetFirstSubnode();
			while (node)
			{
				if (node->GetNodeType() == kNodeGeometry)
				{
					Controller *controller = node->GetController();
					if (controller) controller->Invalidate();
				}
				
				node = node->Next();
			}
		}
	}
}

Model *Model::New(const char *name, ModelType type, unsigned_int32 unpackFlags)
{
	if( name == nullptr )
		return (nullptr);
	ModelResource *resource = ModelResource::Get(name);
	if (resource)
	{
		Model *model = static_cast<Model *>(Node::UnpackTree(resource->GetData(), unpackFlags | (kUnpackNonpersistent | kUnpackExternal)));
		resource->Release();
		
		if (type != kModelUnknown) model->modelType = type;
		
		model->modelState = kModelLoaded;
		model->SetNodeFlags(model->GetNodeFlags() & ~kNodeNonpersistent);
		return (model);
	}
	
	return (nullptr);
}

Model *Model::Get(ModelType type)
{
	ModelRegistration *registration = FindRegistration(type);
	if (registration)
	{
		Model *model = registration->Clone();
		model->modelState = kModelLoaded;
		return (model);
	}
	
	return (nullptr);
}


GenericModel::GenericModel() : Model(kModelGeneric)
{
}

GenericModel::GenericModel(const char *name) : Model(kModelGeneric)
{
	modelName = name;
}

GenericModel::GenericModel(const GenericModel& genericModel) : Model(genericModel)
{
	modelName = genericModel.modelName;
	
	List<GenericModel> *list = genericModel.ListElement<GenericModel>::GetOwningList();
	if (list) list->Append(this);
}

GenericModel::~GenericModel()
{
}

Node *GenericModel::Replicate(void) const
{
	return (new GenericModel(*this));
}

void GenericModel::Pack(Packer& data, unsigned_int32 packFlags) const
{
	Model::Pack(data, packFlags);
	
	PackHandle handle = data.BeginChunk('MODL');
	data << modelName;
	data.EndChunk(handle);
	
	data << TerminatorChunk;
}

void GenericModel::Unpack(Unpacker& data, unsigned_int32 unpackFlags)
{
	Model::Unpack(data, unpackFlags);
	UnpackChunkList<GenericModel>(data, unpackFlags);
}

bool GenericModel::UnpackChunk(const ChunkHeader *chunkHeader, Unpacker& data, unsigned_int32 unpackFlags)
{
	switch (chunkHeader->chunkType)
	{
		case 'MODL':
			
			data >> modelName;
			return (true);
	}
	
	return (false);
}

int32 GenericModel::GetCategoryCount(void) const
{
	return (Model::GetCategoryCount() + 1);
}

Type GenericModel::GetCategoryType(int32 index, const char **title) const
{
	int32 count = Model::GetCategoryCount();
	if (index == count)
	{
		*title = TheInterfaceMgr->GetStringTable()->GetString(StringID(kNodeModel));
		return (kNodeModel);
	}
	
	return (Model::GetCategoryType(index, title));
}

int32 GenericModel::GetCategorySettingCount(Type category) const
{
	if (category == kNodeModel) return (2);
	return (Model::GetCategorySettingCount(category));
}

Setting *GenericModel::GetCategorySetting(Type category, int32 index, unsigned_int32 flags) const
{
	if (category == kNodeModel)
	{
		if (flags & kConfigurationScript) return (nullptr);
		
		const StringTable *table = TheInterfaceMgr->GetStringTable();
		
		if (index == 0)
		{
			const char *title = table->GetString(StringID(kNodeModel, 'GMOD'));
			return (new HeadingSetting('GMOD', title));
		}
		
		if (index == 1)
		{
			const char *title = table->GetString(StringID(kNodeModel, 'GMOD', 'MODL'));
			const char *picker = table->GetString(StringID(kNodeModel, 'GMOD', 'PICK'));
			return (new ResourceSetting('MODL', modelName, title, picker, ModelResource::GetDescriptor()));
		}
		
		return (nullptr);
	}
	
	return (Model::GetCategorySetting(category, index, flags));
}

void GenericModel::SetCategorySetting(Type category, const Setting *setting)
{
	if (category == kNodeModel)
	{
		Type identifier = setting->GetSettingIdentifier();
		
		if (identifier == 'MODL')
		{
			modelName = static_cast<const ResourceSetting *>(setting)->GetResourceName();
		}
	}
	else
	{
		Model::SetCategorySetting(category, setting);
	}
}

void GenericModel::Load(World *world)
{
	if ((modelName[0] != 0) && (!ListElement<GenericModel>::GetOwningList()))
	{
		Node *instanceRoot = world->NewGenericModel(modelName, this);
		if (instanceRoot)
		{
			for (;;)
			{
				Node *node = instanceRoot->GetFirstSubnode();
				if (!node) break;
				
				AddSubnode(node);
			}
			
			delete instanceRoot;
		}
	}
}

void GenericModel::Unload(void)
{
	ListElement<GenericModel>::Detach();
	PurgeSubtree();
}


AnimationController::AnimationController() : Controller(kControllerAnimation)
{
	animationMode = kInterpolatorStop;
	animationName[0] = 0;
	frameAnimator = nullptr;
}

AnimationController::AnimationController(const AnimationController& animationController) : Controller(animationController)
{
	animationMode = kInterpolatorStop;
	animationName[0] = 0;
	frameAnimator = nullptr;
}

AnimationController::~AnimationController()
{
	delete frameAnimator;
}

Controller *AnimationController::Replicate(void) const
{
	return (new AnimationController(*this));
}

bool AnimationController::ValidNode(const Node *node)
{
	return (node->GetNodeType() == kNodeModel);
}

void AnimationController::RegisterFunctions(ControllerRegistration *registration)
{
	const StringTable *table = TheInterfaceMgr->GetStringTable();
	
	static FunctionReg<PlayAnimationFunction> playAnimationFunction(registration, kFunctionPlayAnimation, table->GetString(StringID('CTRL', kControllerAnimation, 'PLAY')), kFunctionRemote | kFunctionJournaled);
	static FunctionReg<StopAnimationFunction> stopAnimationFunction(registration, kFunctionStopAnimation, table->GetString(StringID('CTRL', kControllerAnimation, 'STOP')), kFunctionRemote | kFunctionJournaled);
}

void AnimationController::Pack(Packer& data, unsigned_int32 packFlags) const
{
	Controller::Pack(data, packFlags);
	
	if (animationName[0] != 0)
	{
		PackHandle handle = data.BeginChunk('NAME');
		data << animationName;
		data.EndChunk(handle);
	}
	
	if (animationMode != kInterpolatorStop)
	{
		data << ChunkHeader('MODE', 4);
		data << animationMode;
	}
	
	data << TerminatorChunk;
}

void AnimationController::Unpack(Unpacker& data, unsigned_int32 unpackFlags)
{
	Controller::Unpack(data, unpackFlags);
	UnpackChunkList<AnimationController>(data, unpackFlags);
}

bool AnimationController::UnpackChunk(const ChunkHeader *chunkHeader, Unpacker& data, unsigned_int32 unpackFlags)
{
	switch (chunkHeader->chunkType)
	{
		case 'NAME':
			
			data >> animationName;
			return (true);
		
		case 'MODE':
			
			data >> animationMode;
			return (true);
	}
	
	return (false);
}

void *AnimationController::BeginSettingsUnpack(void)
{
	animationMode = kInterpolatorStop;
	animationName[0] = 0;
	
	return (Controller::BeginSettingsUnpack());
}

int32 AnimationController::GetSettingCount(void) const
{
	return (5);
}

Setting *AnimationController::GetSetting(int32 index) const
{
	const StringTable *table = TheInterfaceMgr->GetStringTable();
	
	if (index == 0)
	{
		const char *title = table->GetString(StringID('CTRL', kControllerAnimation, 'NAME'));
		const char *picker = table->GetString(StringID('CTRL', kControllerAnimation, 'PICK'));
		return (new ResourceSetting('NAME', animationName, title, picker, AnimationResource::GetDescriptor()));
	}
	
	if (index == 1)
	{
		const char *title = table->GetString(StringID('CTRL', kControllerAnimation, 'IPLY'));
		return (new BooleanSetting('IPLY', (animationMode != kInterpolatorStop), title));
	}
	
	if (index == 2)
	{
		const char *title = table->GetString(StringID('CTRL', kControllerAnimation, 'RVRS'));
		return (new BooleanSetting('RVRS', ((animationMode & kInterpolatorBackward) != 0), title));
	}
	
	if (index == 3)
	{
		const char *title = table->GetString(StringID('CTRL', kControllerAnimation, 'LOOP'));
		return (new BooleanSetting('LOOP', ((animationMode & kInterpolatorLoop) != 0), title));
	}
	
	if (index == 4)
	{
		const char *title = table->GetString(StringID('CTRL', kControllerAnimation, 'OSCL'));
		return (new BooleanSetting('OSCL', ((animationMode & kInterpolatorOscillate) != 0), title));
	}
	
	return (nullptr);
}

void AnimationController::SetSetting(const Setting *setting)
{
	Type identifier = setting->GetSettingIdentifier();
	
	if (identifier == 'NAME')
	{
		animationName = static_cast<const ResourceSetting *>(setting)->GetResourceName();
	}
	else if (identifier == 'IPLY')
	{
		bool b = static_cast<const BooleanSetting *>(setting)->GetBooleanValue();
		if (b) animationMode = kInterpolatorForward;
		else animationMode = kInterpolatorStop;
	}
	else if (identifier == 'RVRS')
	{
		bool b = static_cast<const BooleanSetting *>(setting)->GetBooleanValue();
		if (b) animationMode = kInterpolatorBackward;
	}
	else if (identifier == 'LOOP')
	{
		bool b = static_cast<const BooleanSetting *>(setting)->GetBooleanValue();
		if (b) animationMode |= kInterpolatorLoop;
	}
	else if (identifier == 'OSCL')
	{
		bool b = static_cast<const BooleanSetting *>(setting)->GetBooleanValue();
		if (b) animationMode |= kInterpolatorOscillate;
	}
}

ControllerMessage *AnimationController::ConstructMessage(ControllerMessageType type) const
{
	switch (type)
	{
		case kAnimationMessageState:
			
			return (new AnimationStateMessage(GetControllerIndex()));
	}
	
	return (Controller::ConstructMessage(type));
}

void AnimationController::ReceiveMessage(const ControllerMessage *message)
{
	if (message->GetControllerMessageType() == kAnimationMessageState)
	{
		const AnimationStateMessage *m = static_cast<const AnimationStateMessage *>(message);
		
		animationName = m->GetAnimationName();
		animationMode = m->GetAnimationMode();
		
		frameAnimator->SetAnimation(animationName);
		Interpolator *interpolator = frameAnimator->GetFrameInterpolator();
		interpolator->SetMode(animationMode);
		interpolator->SetValue(m->GetAnimatorValue());
	}
	else
	{
		Controller::ReceiveMessage(message);
	}
}

void AnimationController::SendInitialStateMessages(Player *player) const
{
	player->SendMessage(AnimationStateMessage(GetControllerIndex(), animationName, frameAnimator->GetFrameInterpolator()->GetValue(), animationMode));
}

void AnimationController::Preprocess(void)
{
	Controller::Preprocess();
	
	Model *model = GetTargetNode();
	if (!model->GetManipulator())
	{
		if (!frameAnimator) frameAnimator = new FrameAnimator(model);
		frameAnimator->SetAnimation(animationName);
		frameAnimator->GetFrameInterpolator()->SetMode(animationMode);
		model->SetRootAnimator(frameAnimator);
	}
}

void AnimationController::Move(void)
{
	static_cast<Model *>(Controller::GetTargetNode())->Animate();
}

void AnimationController::PlayAnimation(const char *name, unsigned_int32 mode)
{
	animationName = name;
	animationMode = mode;
	
	frameAnimator->SetAnimation(name);
	frameAnimator->GetFrameInterpolator()->SetMode(mode);
}

void AnimationController::StopAnimation(void)
{
	unsigned_int32 mode = animationMode & ~(kInterpolatorForward | kInterpolatorBackward);
	animationMode = mode;
	
	frameAnimator->GetFrameInterpolator()->SetMode(mode);
}


AnimationStateMessage::AnimationStateMessage(int32 controllerIndex) : ControllerMessage(AnimationController::kAnimationMessageState, controllerIndex)
{
}

AnimationStateMessage::AnimationStateMessage(int32 controllerIndex, const char *name, float value, unsigned_int32 mode) : ControllerMessage(AnimationController::kAnimationMessageState, controllerIndex)
{
	animationName = name;
	animatorValue = value;
	animationMode = mode;
}

AnimationStateMessage::~AnimationStateMessage()
{
}

void AnimationStateMessage::Compress(Compressor& data) const
{
	ControllerMessage::Compress(data);
	
	data << animationMode;
	data << animatorValue;
	data << animationName;
}

bool AnimationStateMessage::Decompress(Decompressor& data)
{
	if (ControllerMessage::Decompress(data))
	{
		data >> animationMode;
		data >> animatorValue;
		data >> animationName;
		return (true);
	}
	
	return (false);
}


PlayAnimationFunction::PlayAnimationFunction() : Function(kFunctionPlayAnimation, kControllerAnimation)
{
	animationMode = kInterpolatorStop;
	animationName[0] = 0;
}

PlayAnimationFunction::PlayAnimationFunction(const PlayAnimationFunction& playAnimationFunction) : Function(playAnimationFunction)
{
	animationMode = playAnimationFunction.animationMode;
	animationName = playAnimationFunction.animationName;
}

PlayAnimationFunction::~PlayAnimationFunction()
{
}

Function *PlayAnimationFunction::Replicate(void) const
{
	return (new PlayAnimationFunction(*this));
}
			
void PlayAnimationFunction::Pack(Packer& data, unsigned_int32 packFlags) const
{
	Function::Pack(data, packFlags);
	
	data << ChunkHeader('MODE', 4);
	data << animationMode;
	
	PackHandle handle = data.BeginChunk('NAME');
	data << animationName;
	data.EndChunk(handle);
	
	data << TerminatorChunk;
}

void PlayAnimationFunction::Unpack(Unpacker& data, unsigned_int32 unpackFlags)
{
	Function::Unpack(data, unpackFlags);
	UnpackChunkList<PlayAnimationFunction>(data, unpackFlags);
}

bool PlayAnimationFunction::UnpackChunk(const ChunkHeader *chunkHeader, Unpacker& data, unsigned_int32 unpackFlags)
{
	switch (chunkHeader->chunkType)
	{
		case 'MODE':
			
			data >> animationMode;
			return (true);
		
		case 'NAME':
			
			data >> animationName;
			return (true);
	}
	
	return (false);
}

void PlayAnimationFunction::Compress(Compressor& data) const
{
	Function::Compress(data);
	
	data << animationMode;
	data << animationName;
}

bool PlayAnimationFunction::Decompress(Decompressor& data)
{
	if (Function::Decompress(data))
	{
		data >> animationMode;
		data >> animationName;
		return (true);
	}
	
	return (false);
}

int32 PlayAnimationFunction::GetSettingCount(void) const
{
	return (4);
}

Setting *PlayAnimationFunction::GetSetting(int32 index) const
{
	const StringTable *table = TheInterfaceMgr->GetStringTable();
	
	if (index == 0)
	{
		const char *title = table->GetString(StringID('CTRL', kControllerAnimation, kFunctionPlayAnimation, 'ANAM'));
		const char *picker = table->GetString(StringID('CTRL', kControllerAnimation, 'PICK'));
		return (new ResourceSetting('ANAM', animationName, title, picker, AnimationResource::GetDescriptor()));
	}
	
	if (index == 1)
	{
		const char *title = table->GetString(StringID('CTRL', kControllerAnimation, kFunctionPlayAnimation, 'RVRS'));
		return (new BooleanSetting('RVRS', ((animationMode & kInterpolatorBackward) != 0), title));
	}
	
	if (index == 2)
	{
		const char *title = table->GetString(StringID('CTRL', kControllerAnimation, kFunctionPlayAnimation, 'LOOP'));
		return (new BooleanSetting('LOOP', ((animationMode & kInterpolatorLoop) != 0), title));
	}
	
	if (index == 3)
	{
		const char *title = table->GetString(StringID('CTRL', kControllerAnimation, kFunctionPlayAnimation, 'OSCL'));
		return (new BooleanSetting('OSCL', ((animationMode & kInterpolatorOscillate) != 0), title));
	}
	
	return (nullptr);
}

void PlayAnimationFunction::SetSetting(const Setting *setting)
{
	Type identifier = setting->GetSettingIdentifier();
	
	if (identifier == 'ANAM')
	{
		animationName = static_cast<const ResourceSetting *>(setting)->GetResourceName();
	}
	else if (identifier == 'RVRS')
	{
		bool b = static_cast<const BooleanSetting *>(setting)->GetBooleanValue();
		if (b) animationMode = kInterpolatorBackward;
		else animationMode = kInterpolatorForward;
	}
	else if (identifier == 'LOOP')
	{
		bool b = static_cast<const BooleanSetting *>(setting)->GetBooleanValue();
		if (b) animationMode |= kInterpolatorLoop;
		else animationMode &= ~kInterpolatorLoop;
	}
	else if (identifier == 'OSCL')
	{
		bool b = static_cast<const BooleanSetting *>(setting)->GetBooleanValue();
		if (b) animationMode |= kInterpolatorOscillate;
		else animationMode &= ~kInterpolatorOscillate;
	}
}

bool PlayAnimationFunction::OverridesFunction(const Function *function) const
{
	FunctionType type = function->GetFunctionType();
	return ((type == kFunctionPlayAnimation) || (type == kFunctionStopAnimation));
}

void PlayAnimationFunction::Execute(Controller *controller, FunctionMethod *method, const ScriptState *state)
{
	AnimationController *animationController = static_cast<AnimationController *>(controller);
	animationController->PlayAnimation(animationName, animationMode);
	
	CallCompletionProc();
}


StopAnimationFunction::StopAnimationFunction() : Function(kFunctionStopAnimation, kControllerAnimation)
{
}

StopAnimationFunction::StopAnimationFunction(const StopAnimationFunction& stopAnimationFunction) : Function(stopAnimationFunction)
{
}

StopAnimationFunction::~StopAnimationFunction()
{
}

Function *StopAnimationFunction::Replicate(void) const
{
	return (new StopAnimationFunction(*this));
}

bool StopAnimationFunction::OverridesFunction(const Function *function) const
{
	return (function->GetFunctionType() == kFunctionStopAnimation);
}

void StopAnimationFunction::Execute(Controller *controller, FunctionMethod *method, const ScriptState *state)
{
	AnimationController *animationController = static_cast<AnimationController *>(controller);
	animationController->StopAnimation();
	
	CallCompletionProc();
}

// ZYURVUR
