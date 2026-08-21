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


#include "C4Graphics.h"
#include "C4Attributes.h"
#include "C4Shaders.h"
#include "C4LightObjects.h"
#include "C4SpaceObjects.h"


using namespace C4;


namespace C4
{
	template <> Heap Memory<ShaderData>::heap("ShaderData", MemoryMgr::CalculatePoolSize(128, sizeof(ShaderData)), kHeapMutexless);
	template class Memory<ShaderData>;
}


int32 VertexBuffer::totalVertexBufferCount = 0;
unsigned_int32 VertexBuffer::totalVertexBufferMemory = 0;

List<VertexBuffer> VertexBuffer::vertexBufferList;
List<OcclusionQuery> OcclusionQuery::occlusionQueryList;
List<ShaderData> ShaderData::shaderDataList;


const PaintEnvironment Renderable::nullPaintEnvironment = {Transform4D(1.0F, 0.0F, 0.0F, 0.0F, 0.0F, 1.0F, 0.0F, 0.0F, 0.0F, 0.0F, 1.0F, 0.0F), nullptr};
const AmbientEnvironment Renderable::nullAmbientEnvironment = {kShaderAmbient, nullptr};

const ConstVector4D Renderable::nullRenderParameterTable[kMaxRenderParameterCount] = {{0.0F}};
const ConstVector4D Renderable::nullTexcoordParameterTable[kMaxTexcoordParameterCount] = {{1.0F, 1.0F, 0.0F, 0.0F}};
const ConstVector4D Renderable::nullTerrainParameterTable[kMaxTerrainParameterCount] = {{1.0F, 1.0F, 1.0F, 1.0F}, {1.0F, 1.0F, 1.0F, 1.0F}};


VertexBuffer::VertexBuffer(unsigned_int32 flags) : VertexBufferObject((flags & kVertexBufferIndex) ? Render::kVertexBufferTargetIndex : Render::kVertexBufferTargetAttribute, (flags & kVertexBufferDynamic) ? Render::kVertexBufferUsageDynamic : Render::kVertexBufferUsageStatic)
{
	activeFlag = false;
	bufferSize = 0;
}

VertexBuffer::~VertexBuffer()
{
	Deactivate();
}

void VertexBuffer::Activate(void)
{
	if (!activeFlag)
	{
		Render::VertexBufferObject::Construct();
		
		if (AllocateStorage(bufferSize))
		{
			activeFlag = true;
			
			totalVertexBufferCount++;
			totalVertexBufferMemory += bufferSize;
			
			PostEvent();
		}
		else
		{
			Render::VertexBufferObject::Destruct();
		}
	}
}

void VertexBuffer::Deactivate(void)
{
	if (activeFlag)
	{
		activeFlag = false;
		
		totalVertexBufferCount--;
		totalVertexBufferMemory -= bufferSize;
		
		TheGraphicsMgr->InvalidateVertexBuffer(this);
		Render::VertexBufferObject::Destruct();
	}
}

void VertexBuffer::Initialize(unsigned_int32 size, unsigned_int32 stride, ObserverType *observer)
{
	vertexStride = stride;
	
	SetObserver(observer);
	vertexBufferList.Append(this);
	
	if (!activeFlag)
	{
		bufferSize = size;
		Activate();
	}
	else
	{
		if (bufferSize != size)
		{
			if (AllocateStorage(size))
			{
				totalVertexBufferMemory += size - bufferSize;
				bufferSize = size;
			}
			else 
			{
				Deactivate(); 
				return; 
			} 
		}
		 
		PostEvent();
	}
}
 
void VertexBuffer::DeactivateAll(void)
{
	VertexBuffer *vertexBuffer = vertexBufferList.First();
	while (vertexBuffer) 
	{
		vertexBuffer->Deactivate();
		vertexBuffer = vertexBuffer->Next();
	}
}

void VertexBuffer::ReactivateAll(void)
{
	VertexBuffer *vertexBuffer = vertexBufferList.First();
	while (vertexBuffer)
	{
		vertexBuffer->Activate();
		vertexBuffer = vertexBuffer->Next();
	}
}


OcclusionQuery::OcclusionQuery(RenderProc *proc, void *cookie)
{
	renderProc = proc;
	renderCookie = cookie;
	
	activeFlag = false;
	occlusionQueryList.Append(this);
}

OcclusionQuery::~OcclusionQuery()
{
	Deactivate();
}

void OcclusionQuery::Activate(void)
{
	if (!activeFlag)
	{
		activeFlag = true;
		Render::QueryObject::Construct();
	}
}

void OcclusionQuery::Deactivate(void)
{
	if (activeFlag)
	{
		activeFlag = false;
		Render::QueryObject::Destruct();
	}
}

void OcclusionQuery::DeactivateAll(void)
{
	OcclusionQuery *query = occlusionQueryList.First();
	while (query)
	{
		query->Deactivate();
		query = query->Next();
	}
}


ShaderProgramData::ShaderProgramData()
{
	vertexProgram = nullptr;
	fragmentProgram = nullptr;
}

ShaderProgramData::~ShaderProgramData()
{
	if (vertexProgram) vertexProgram->Release();
	if (fragmentProgram) fragmentProgram->Release();
}


ShaderData::ShaderData(ShaderData **pointer, unsigned_int32 blend, unsigned_int32 material)
{
	*pointer = this;
	shaderDataPointer = pointer;
	shaderDataList.Append(this);
	
	blendState = blend;
	materialState = material;
	
	variantMask = 0;
	indexBuffer = nullptr;
	textureUnitCount = 0;
	shaderStateDataCount = 0;
	
	for (machine a = 0; a < kMaxShaderArrayCount; a++)
	{
		vertexBuffer[a] = nullptr;
		shaderArray[a] = nullptr;
	}
}

ShaderData::~ShaderData()
{
	*shaderDataPointer = nullptr;
}


void ShaderData::AddStateFunction(ShaderStateFunc *func, const void *cookie)
{
	int32 count = shaderStateDataCount;
	for (machine a = 0; a < count; a++) if (shaderStateData[a].stateFunc == func) return;
	
	Assert(count < kMaxShaderStateDataCount, "State function table overflow");
	
	shaderStateData[count].stateFunc = func;
	shaderStateData[count].stateCookie = cookie;
	shaderStateDataCount = count + 1;
}


RenderSegment::RenderSegment(unsigned_int32 state)
{
	nextSegment = nullptr;
	
	materialState = state;
	
	materialObject = nullptr;
	materialAttributeList = nullptr;
	
	for (machine type = 0; type < kShaderTypeCount; type++)
	{
		for (machine level = 0; level < kMaxShaderDetailLevelCount; level++) shaderData[type][level] = nullptr;
	}
}

RenderSegment::~RenderSegment()
{
	for (machine type = 0; type < kShaderTypeCount; type++)
	{
		for (machine level = 0; level < kMaxShaderDetailLevelCount; level++) delete shaderData[type][level];
	}
}

ShaderData *RenderSegment::InitShaderData(Renderable *renderable, ShaderType type, ShaderVariant variant)
{
	int32 level = renderable->GetShaderDetailLevel();
	
	if (type <= kShaderLastAmbient)
	{
		if (renderable->GetShaderFlags() & kShaderAmbientEffect) return (InitEffectShaderData(renderable, type, variant, level));
		return (InitAmbientShaderData(renderable, type, variant, level));
	}
	else if (type <= kShaderLastLight)
	{
		return (InitLightShaderData(renderable, type, variant, level));
	}
	
	return (InitPlainShaderData(renderable, type, level));
}

void RenderSegment::InvalidateShaderData(void)
{
	for (machine type = 0; type < kShaderTypeCount; type++)
	{
		for (machine level = 0; level < kMaxShaderDetailLevelCount; level++)
		{
			delete shaderData[type][level];
		}
	}
}

void RenderSegment::InvalidateAmbientShaderData(void)
{
	for (machine type = 0; type < kShaderLastAmbient; type++)
	{
		for (machine level = 0; level < kMaxShaderDetailLevelCount; level++)
		{
			delete shaderData[type][level];
		}
	}
}

unsigned_int32 RenderSegment::GetShaderDataMaterialState(ShaderType type)
{
	unsigned_int32 state = materialState;
	if (materialObject)
	{
		const MaterialObject *material = *materialObject;
		if (material) state |= material->GetMaterialFlags();
	}
	
	if (type >= kShaderFirstPlain)
	{
		state &= ~(kMaterialAlphaCoverage | kMaterialSampleShading);
	}
	else
	{
		if (!TheGraphicsMgr->GetCapabilities()->extensionFlag[kExtensionSampleShading]) state &= ~kMaterialSampleShading;
		else if (state & kMaterialSampleShading) state &= ~kMaterialAlphaCoverage;
	}
	
	return (state);
}

ShaderData *RenderSegment::InitAmbientShaderData(Renderable *renderable, ShaderType type, ShaderVariant variant, int32 level)
{
	ShaderData **slot = &shaderData[type][level];
	ShaderData *shaderData = *slot;
	if (!shaderData)
	{
		unsigned_int32 blendState = renderable->GetAmbientBlendState();
		shaderData = new ShaderData(slot, blendState, GetShaderDataMaterialState(type));
	}
	
	const Attribute *primaryAttribute = (materialAttributeList) ? materialAttributeList->First() : nullptr;
	const MaterialObject *object = (materialObject) ? *materialObject : nullptr;
	if (object)
	{
		const Attribute *attribute = object->GetFirstAttribute();
		if (attribute) primaryAttribute = attribute;
	}
	
	if ((!primaryAttribute) || (primaryAttribute->GetAttributeType() != kAttributeShader))
	{
		ShaderAttribute		shaderAttribute;
		ShaderGraph			shaderGraph;
		Process				*process[kAmbientGraphProcessCount];
		
		ShaderAttribute::BuildAmbientShaderGraph(renderable, this, object, materialAttributeList, &shaderGraph, process);
		shaderAttribute.CompileShader(type, variant, level, renderable, this, shaderData, &shaderGraph);
	}
	else
	{
		static_cast<const ShaderAttribute *>(primaryAttribute)->CompileShader(type, variant, level, renderable, this, shaderData);
	}
	
	renderable->groupKey[kGroupKeyAmbient][level] = GetPointerAddress(shaderData->programData[variant].fragmentProgram);
	
	shaderData->variantMask |= 1 << variant;
	return (shaderData);
}

ShaderData *RenderSegment::InitEffectShaderData(Renderable *renderable, ShaderType type, ShaderVariant variant, int32 level)
{
	ShaderData **slot = &shaderData[type][level];
	ShaderData *shaderData = *slot;
	if (!shaderData)
	{
		unsigned_int32 blendState = (renderable->GetAmbientBlendState() & kBlendColorMask) | kBlendAlphaPreserve;
		shaderData = new ShaderData(slot, blendState, GetShaderDataMaterialState(type));
	}
	
	const Attribute *primaryAttribute = (materialAttributeList) ? materialAttributeList->First() : nullptr;
	const MaterialObject *object = (materialObject) ? *materialObject : nullptr;
	if (object)
	{
		const Attribute *attribute = object->GetFirstAttribute();
		if (attribute) primaryAttribute = attribute;
	}
	
	if ((!primaryAttribute) || (primaryAttribute->GetAttributeType() != kAttributeShader))
	{
		ShaderAttribute		shaderAttribute;
		ShaderGraph			shaderGraph;
		
		ShaderAttribute::BuildEffectShaderGraph(renderable, this, object, materialAttributeList, &shaderGraph);
		shaderAttribute.CompileShader(type, variant, level, renderable, this, shaderData, &shaderGraph);
	}
	else
	{
		static_cast<const ShaderAttribute *>(primaryAttribute)->CompileShader(type, variant, level, renderable, this, shaderData);
	}
	
	OcclusionQuery *occlusionQuery = renderable->GetOcclusionQuery();
	if ((occlusionQuery) && (TheGraphicsMgr->GetCapabilities()->capabilityFlag[kCapabilityOcclusionQuery]))
	{
		occlusionQuery->Activate();
		shaderData->AddStateFunction(&Renderable::StateFunc_SetOcclusionQuery);
	}
	
	renderable->groupKey[kGroupKeyAmbient][level] = GetPointerAddress(shaderData->programData[variant].fragmentProgram);
	
	shaderData->variantMask |= 1 << variant;
	return (shaderData);
}

ShaderData *RenderSegment::InitLightShaderData(Renderable *renderable, ShaderType type, ShaderVariant variant, int32 level)
{
	ShaderData **slot = &shaderData[type][level];
	ShaderData *shaderData = *slot;
	if (!shaderData)
	{
		unsigned_int32 blendState = (renderable->GetLightBlendState() & kBlendColorMask) | kBlendAlphaPreserve;
		shaderData = new ShaderData(slot, blendState, GetShaderDataMaterialState(type));
	}
	
	const Attribute *primaryAttribute = (materialAttributeList) ? materialAttributeList->First() : nullptr;
	const MaterialObject *object = (materialObject) ? *materialObject : nullptr;
	if (object)
	{
		const Attribute *attribute = object->GetFirstAttribute();
		if (attribute) primaryAttribute = attribute;
	}
	
	if ((!primaryAttribute) || (primaryAttribute->GetAttributeType() != kAttributeShader))
	{
		ShaderAttribute		shaderAttribute;
		ShaderGraph			shaderGraph;
		Process				*process[kLightGraphProcessCount];
		
		ShaderAttribute::BuildLightShaderGraph(renderable, this, object, materialAttributeList, &shaderGraph, process);
		shaderAttribute.CompileShader(type, variant, level, renderable, this, shaderData, &shaderGraph);
	}
	else
	{
		static_cast<const ShaderAttribute *>(primaryAttribute)->CompileShader(type, variant, level, renderable, this, shaderData);
	}
	
	renderable->groupKey[kGroupKeyLight][level] = GetPointerAddress(shaderData->programData[variant].fragmentProgram);
	
	shaderData->variantMask |= 1 << variant;
	return (shaderData);
}

ShaderData *RenderSegment::InitPlainShaderData(Renderable *renderable, ShaderType type, int32 level)
{
	ShaderData **slot = &shaderData[type][level];
	ShaderData *shaderData = *slot;
	if (!shaderData)
	{
		shaderData = new ShaderData(slot, kBlendReplace, GetShaderDataMaterialState(type));
	}
	
	const Attribute *primaryAttribute = (materialAttributeList) ? materialAttributeList->First() : nullptr;
	const MaterialObject *object = (materialObject) ? *materialObject : nullptr;
	if (object)
	{
		const Attribute *attribute = object->GetFirstAttribute();
		if (attribute) primaryAttribute = attribute;
	}
	
	if ((!primaryAttribute) || (primaryAttribute->GetAttributeType() != kAttributeShader))
	{
		ShaderAttribute		shaderAttribute;
		ShaderGraph			shaderGraph;
		
		ShaderAttribute::BuildPlainShaderGraph(renderable, this, object, materialAttributeList, &shaderGraph);
		shaderAttribute.CompileShader(type, kShaderVariantNormal, level, renderable, this, shaderData, &shaderGraph);
	}
	else
	{
		static_cast<const ShaderAttribute *>(primaryAttribute)->CompileShader(type, kShaderVariantNormal, level, renderable, this, shaderData);
	}
	
	shaderData->variantMask |= 1 << kShaderVariantNormal;
	return (shaderData);
}


Renderable::Renderable(RenderType type, unsigned_int32 state)
{
	renderType = type;
	renderState = state;
	renderableFlags = 0;
	shaderFlags = 0;
	
	ambientBlendState = kBlendReplace;
	lightBlendState = kBlendAccumulate;
	
	transformable = nullptr;
	previousWorldTransform = nullptr;
	
	paintEnvironment = &nullPaintEnvironment;
	ambientEnvironment = &nullAmbientEnvironment;
	
	transparentAttachment = nullptr;
	transparentPosition = nullptr;
	
	for (machine a = 0; a < kVertexBufferCount; a++) vertexBuffer[a] = nullptr;
	
	for (machine a = 0; a < kMaxAttributeArrayCount; a++)
	{
		attributeArray[a] = nullptr;
		componentCount[a] = 0;
	}
	
	dynamicArrayFlags = 0;
	
	renderParameter = &nullRenderParameterTable[0];
	texcoordParameter = &nullTexcoordParameterTable[0];
	terrainParameter = &nullTerrainParameterTable[0];
	
	occlusionQuery = nullptr;
	wireColor = nullptr;
	
	shaderDetailLevel = 0;
	shaderDetailParameter = 1.0F;
	
	for (machine a = 0; a < kMaxGroupKeyCount; a++)
	{
		for (machine b = 0; b < kMaxShaderDetailLevelCount; b++) groupKey[a][b] = 0;
	}
}

Renderable::~Renderable()
{
}

int32 Renderable::SetShaderArray(ShaderData *data, int32 shaderIndex, int32 renderIndex) const
{
	int32 count = componentCount[renderIndex];
	if (count != 0)
	{
		data->componentCount[shaderIndex] = count;
		
		const VertexBuffer *buffer = vertexBuffer[(dynamicArrayFlags >> renderIndex) & 1];
		if (buffer)
		{
			data->vertexBuffer[shaderIndex] = buffer;
			data->shaderOffset[shaderIndex] = &attributeOffset[renderIndex];
		}
		else
		{
			data->shaderArray[shaderIndex] = &attributeArray[renderIndex];
		}
	}
	
	return (count);
}

unsigned_int32 Renderable::BuildVertexTransform(ShaderData *data, VertexAssembly *assembly) const
{
	SetShaderArray(data, kShaderArrayPosition0, kArrayVertex);
	SetShaderArray(data, kShaderArrayPosition1, kArrayPosition1);
	
	unsigned_int32 stateFlags = 0;
	
	if (shaderFlags & kShaderVertexBillboard)
	{
		stateFlags |= kShaderStateCameraDirections;
		
		int32 componentCount = SetShaderArray(data, kShaderArrayOffset, kArrayBillboard);
		if (componentCount != 0)
		{
			if (shaderFlags & kShaderVertexInfinite)
			{
				assembly->AddSnippet(&VertexProgram::calculateBillboardPosition);
				assembly->AddSnippet(&VertexProgram::modelviewProjectTransformInfinite);
			}
			else
			{
				if (shaderFlags & kShaderScaleVertex)
				{
					stateFlags |= kShaderStateVertexScaleOffset;
					assembly->AddSnippet(&VertexProgram::calculateBillboardScalePosition);
				}
				else
				{
					if (componentCount == 2) assembly->AddSnippet(&VertexProgram::calculateBillboardPosition);
					else assembly->AddSnippet(&VertexProgram::calculateLightedBillboardPosition);
				}
				
				assembly->AddSnippet(&VertexProgram::modelviewProjectTransformHomogeneous);
			}
		}
		else
		{
			if (shaderFlags & kShaderVertexInfinite)
			{
				assembly->AddSnippet(&VertexProgram::calculateVertexBillboardPosition);
				assembly->AddSnippet(&VertexProgram::modelviewProjectTransformInfinite);
			}
			else
			{
				if (shaderFlags & kShaderScaleVertex)
				{
					stateFlags |= kShaderStateVertexScaleOffset;
					assembly->AddSnippet(&VertexProgram::calculateVertexBillboardScalePosition);
				}
				else
				{
					assembly->AddSnippet(&VertexProgram::calculateVertexBillboardPosition);
				}
				
				assembly->AddSnippet(&VertexProgram::modelviewProjectTransformHomogeneous);
			}
		}
	}
	else if (shaderFlags & kShaderVertexPostboard)
	{
		stateFlags |= kShaderStateCameraPosition4D;
		SetShaderArray(data, kShaderArrayRadius, kArrayRadius);
		
		if (shaderFlags & kShaderScaleVertex)
		{
			stateFlags |= kShaderStateVertexScaleOffset;
			assembly->AddSnippet(&VertexProgram::calculatePostboardScalePosition);
		}
		else
		{
			assembly->AddSnippet(&VertexProgram::calculatePostboardPosition);
		}
		
		assembly->AddSnippet(&VertexProgram::modelviewProjectTransformHomogeneous);
	}
	else if (shaderFlags & kShaderVertexPolyboard)
	{
		SetShaderArray(data, kShaderArrayTangent, kArrayTangent);
		
		if (shaderFlags & kShaderOrthoPolyboard)
		{
			stateFlags |= kShaderStateCameraPosition4D;
			
			if (shaderFlags & kShaderScaleVertex)
			{
				stateFlags |= kShaderStateVertexScaleOffset;
				assembly->AddSnippet(&VertexProgram::scaleVertexCalculateCameraDirection4D);
				
				if (shaderFlags & kShaderLinearPolyboard) assembly->AddSnippet(&VertexProgram::calculateLinearPolyboardNormal);
				else assembly->AddSnippet(&VertexProgram::calculatePolyboardNormal);
				
				assembly->AddSnippet(&VertexProgram::calculatePolyboardScalePosition);
			}
			else
			{
				assembly->AddSnippet(&VertexProgram::calculateCameraDirection4D);
				
				if (shaderFlags & kShaderLinearPolyboard) assembly->AddSnippet(&VertexProgram::calculateLinearPolyboardNormal);
				else assembly->AddSnippet(&VertexProgram::calculatePolyboardNormal);
				
				assembly->AddSnippet(&VertexProgram::calculatePolyboardPosition);
			}
			
			assembly->AddSnippet(&VertexProgram::modelviewProjectTransformHomogeneous);
		}
		else
		{
			stateFlags |= kShaderStateCameraPosition;
			assembly->AddSnippet(&VertexProgram::calculateCameraDirection);
			
			if (shaderFlags & kShaderLinearPolyboard) assembly->AddSnippet(&VertexProgram::calculateLinearPolyboardNormal);
			else assembly->AddSnippet(&VertexProgram::calculatePolyboardNormal);
			
			assembly->AddSnippet(&VertexProgram::calculatePolyboardPosition);
			if (shaderFlags & kShaderVertexInfinite) assembly->AddSnippet(&VertexProgram::modelviewProjectTransformInfinite);
			else assembly->AddSnippet(&VertexProgram::modelviewProjectTransformHomogeneous);
		}
	}
	else if (shaderFlags & kShaderScaleVertex)
	{
		stateFlags |= kShaderStateVertexScaleOffset;
		
		if (shaderFlags & kShaderOffsetVertex)
		{
			SetShaderArray(data, kShaderArrayOffset, kArrayOffset);
			assembly->AddSnippet(&VertexProgram::calculateScaleOffsetPosition);
		}
		else
		{
			assembly->AddSnippet(&VertexProgram::calculateScalePosition);
		}
		
		assembly->AddSnippet(&VertexProgram::modelviewProjectTransformHomogeneous);
	}
	else if (shaderFlags & kShaderNormalExpandVertex)
	{
		stateFlags |= kShaderStateVertexScaleOffset;
		assembly->AddSnippet(&VertexProgram::calculateExpandNormalPosition);
		assembly->AddSnippet(&VertexProgram::modelviewProjectTransformHomogeneous);
	}
	else if (shaderFlags & kShaderTerrainBorder)
	{
		stateFlags |= kShaderStateTerrainBorder;
		SetShaderArray(data, kShaderArrayColor2, kArrayColor2);
		
		assembly->AddSnippet(&VertexProgram::calculateTerrainBorderPosition);
		assembly->AddSnippet(&VertexProgram::modelviewProjectTransformHomogeneous);
	}
	else if (shaderFlags & kShaderWaterElevation)
	{
		assembly->AddSnippet(&VertexProgram::calculateWaterHeightPosition);
		assembly->AddSnippet(&VertexProgram::modelviewProjectTransformHomogeneous);
	}
	else
	{
		if (shaderFlags & kShaderVertexInfinite) assembly->AddSnippet(&VertexProgram::modelviewProjectTransformInfinite);
		else assembly->AddSnippet(&VertexProgram::modelviewProjectTransform);
	}
	
	const VertexBuffer *buffer = vertexBuffer[kVertexBufferIndexArray];
	if ((buffer) && (buffer->Active())) data->indexBuffer = buffer;
	
	return (stateFlags);
}

unsigned_int32 Renderable::BuildTexcoord0Transform(const RenderSegment *segment, ShaderData *data, VertexAssembly *assembly, unsigned_int32 stateFlags) const
{
	static const VertexSnippet *snippetTable[8] =
	{
		&VertexProgram::copyPrimaryTexcoord0, &VertexProgram::transformPrimaryTexcoord0,
		&VertexProgram::animatePrimaryTexcoord0, &VertexProgram::transformAnimatePrimaryTexcoord0,
		&VertexProgram::generateTexcoord0, &VertexProgram::generateTransformTexcoord0,
		&VertexProgram::generateAnimateTexcoord0, &VertexProgram::generateTransformAnimateTexcoord0
	};
	
	SetShaderArray(data, kShaderArrayTexture0, kArrayTexture0);
	unsigned_int32 snippetIndex = 0;
	
	const MaterialObject *const *materialPointer = segment->GetMaterialObjectPointer();
	if (materialPointer)
	{
		const MaterialObject *materialObject = *materialPointer;
		if (materialObject)
		{
			const Vector2D& scale = materialObject->GetTexcoordScale(0);
			const Vector2D& offset = materialObject->GetTexcoordOffset(0);
			if ((scale.x != 1.0F) || (scale.y != 1.0F) || (offset.x != 0.0F) || (offset.y != 0.0F))
			{
				snippetIndex = 1;
				stateFlags |= kShaderStateTexcoordTransform0;
			}
			
			if (materialObject->GetMaterialFlags() & kMaterialAnimateTexcoord0)
			{
				snippetIndex |= 2;
				stateFlags |= kShaderStateTexcoordVelocity0;
			}
		}
	}
	
	if (shaderFlags & kShaderGenerateTexcoord)
	{
		snippetIndex |= 4;
		stateFlags |= kShaderStateTexcoordGenerate;
		
		if ((snippetIndex == 6) && (!(stateFlags & kShaderStateBaseTexcoord)))
		{
			stateFlags |= kShaderStateBaseTexcoord;
			assembly->AddSnippet(&VertexProgram::generateBaseTexcoord);
		}
	}
	
	assembly->AddSnippet(snippetTable[snippetIndex]);
	return (stateFlags);
}

unsigned_int32 Renderable::BuildTexcoord1Transform(const RenderSegment *segment, ShaderData *data, VertexAssembly *assembly, unsigned_int32 stateFlags) const
{
	static const VertexSnippet *snippetTable[12] =
	{
		&VertexProgram::copyPrimaryTexcoord1, &VertexProgram::transformPrimaryTexcoord1,
		&VertexProgram::animatePrimaryTexcoord1, &VertexProgram::transformAnimatePrimaryTexcoord1,
		&VertexProgram::generateTexcoord1, &VertexProgram::generateTransformTexcoord1,
		&VertexProgram::generateAnimateTexcoord1, &VertexProgram::generateTransformAnimateTexcoord1,
		&VertexProgram::copySecondaryTexcoord1, &VertexProgram::transformSecondaryTexcoord1,
		&VertexProgram::animateSecondaryTexcoord1, &VertexProgram::transformAnimateSecondaryTexcoord1
	};
	
	SetShaderArray(data, kShaderArrayTexture0, kArrayTexture0);
	unsigned_int32 snippetIndex = (SetShaderArray(data, kShaderArrayTexture1, kArrayTexture1) != 0) ? 8 : 0;
	
	const MaterialObject *const *materialPointer = segment->GetMaterialObjectPointer();
	if (materialPointer)
	{
		const MaterialObject *materialObject = *materialPointer;
		if (materialObject)
		{
			const Vector2D& scale = materialObject->GetTexcoordScale(1);
			const Vector2D& offset = materialObject->GetTexcoordOffset(1);
			if ((scale.x != 1.0F) || (scale.y != 1.0F) || (offset.x != 0.0F) || (offset.y != 0.0F))
			{
				snippetIndex |= 1;
				stateFlags |= kShaderStateTexcoordTransform1;
			}
			
			if (materialObject->GetMaterialFlags() & kMaterialAnimateTexcoord1)
			{
				snippetIndex |= 2;
				stateFlags |= kShaderStateTexcoordVelocity1;
			}
		}
	}
	
	if ((shaderFlags & kShaderGenerateTexcoord) && (snippetIndex < 8))
	{
		snippetIndex |= 4;
		stateFlags |= kShaderStateTexcoordGenerate;
		
		if ((snippetIndex == 6) && (!(stateFlags & kShaderStateBaseTexcoord)))
		{
			stateFlags |= kShaderStateBaseTexcoord;
			assembly->AddSnippet(&VertexProgram::generateBaseTexcoord);
		}
	}
	
	assembly->AddSnippet(snippetTable[snippetIndex]);
	return (stateFlags);
}

void Renderable::StateFunc_CopyCameraPosition(const Renderable *renderable, const void *cookie)
{
	const Point3D& position = TheGraphicsMgr->GetCameraTransformable()->GetWorldPosition();
	Render::SetVertexProgramParameter4f(kVertexParamCameraPosition, position.x, position.y, position.z, 1.0F);
}

void Renderable::StateFunc_CopyCameraDirections(const Renderable *renderable, const void *cookie)
{
	const Transform4D& cameraTransform = TheGraphicsMgr->GetCameraTransformable()->GetWorldTransform();
	
	const Vector3D& right = cameraTransform[0];
	const Vector3D& down = cameraTransform[1];
	Render::SetVertexProgramParameter4f(kVertexParamCameraRight, right.x, right.y, right.z, 0.0F);
	Render::SetVertexProgramParameter4f(kVertexParamCameraDown, down.x, down.y, down.z, 0.0F);
}

void Renderable::StateFunc_CopyCameraPositionAndDirections(const Renderable *renderable, const void *cookie)
{
	const Transform4D& cameraTransform = TheGraphicsMgr->GetCameraTransformable()->GetWorldTransform();
	
	const Point3D& position = cameraTransform.GetTranslation();
	Render::SetVertexProgramParameter4f(kVertexParamCameraPosition, position.x, position.y, position.z, 1.0F);
	
	const Vector3D& right = cameraTransform[0];
	const Vector3D& down = cameraTransform[1];
	Render::SetVertexProgramParameter4f(kVertexParamCameraRight, right.x, right.y, right.z, 0.0F);
	Render::SetVertexProgramParameter4f(kVertexParamCameraDown, down.x, down.y, down.z, 0.0F);
}

void Renderable::StateFunc_TransformCameraPosition(const Renderable *renderable, const void *cookie)
{
	const Transformable *geometryTransformable = renderable->GetTransformable();
	
	Point3D position = geometryTransformable->GetInverseWorldTransform() * TheGraphicsMgr->GetCameraTransformable()->GetWorldPosition();
	Render::SetVertexProgramParameter4f(kVertexParamCameraPosition, position.x, position.y, position.z, 1.0F);
}

void Renderable::StateFunc_TransformCameraDirections(const Renderable *renderable, const void *cookie)
{
	const Transformable *geometryTransformable = renderable->GetTransformable();
	const Transform4D& cameraTransform = TheGraphicsMgr->GetCameraTransformable()->GetWorldTransform();
	
	const Transform4D& inverse = geometryTransformable->GetInverseWorldTransform();
	Vector3D right = inverse * cameraTransform[0];
	Vector3D down = inverse * cameraTransform[1];
	Render::SetVertexProgramParameter4f(kVertexParamCameraRight, right.x, right.y, right.z, 0.0F);
	Render::SetVertexProgramParameter4f(kVertexParamCameraDown, down.x, down.y, down.z, 0.0F);
}

void Renderable::StateFunc_TransformCameraPositionAndDirections(const Renderable *renderable, const void *cookie)
{
	const Transformable *geometryTransformable = renderable->GetTransformable();
	const Transform4D& cameraTransform = TheGraphicsMgr->GetCameraTransformable()->GetWorldTransform();
	
	Point3D position = geometryTransformable->GetInverseWorldTransform() * cameraTransform.GetTranslation();
	Render::SetVertexProgramParameter4f(kVertexParamCameraPosition, position.x, position.y, position.z, 1.0F);
	
	const Transform4D& inverse = geometryTransformable->GetInverseWorldTransform();
	Vector3D right = inverse * cameraTransform[0];
	Vector3D down = inverse * cameraTransform[1];
	Render::SetVertexProgramParameter4f(kVertexParamCameraRight, right.x, right.y, right.z, 0.0F);
	Render::SetVertexProgramParameter4f(kVertexParamCameraDown, down.x, down.y, down.z, 0.0F);
}

void Renderable::StateFunc_CopyCameraPosition4D(const Renderable *renderable, const void *cookie)
{
	Render::SetVertexProgramParameter4fv(kVertexParamCameraPosition, &TheGraphicsMgr->GetCameraPosition4D().x);
}

void Renderable::StateFunc_TransformCameraPosition4D(const Renderable *renderable, const void *cookie)
{
	Vector4D position = renderable->GetTransformable()->GetInverseWorldTransform() * TheGraphicsMgr->GetCameraPosition4D();
	Render::SetVertexProgramParameter4fv(kVertexParamCameraPosition, &position.x);
}

void Renderable::StateFunc_CopyCameraMatrix(const Renderable *renderable, const void *cookie)
{
	const Transform4D& transform = TheGraphicsMgr->GetCameraTransformable()->GetInverseWorldTransform();
	Render::SetVertexProgramParameter4f(kVertexParamMatrixCamera, transform(0,0), transform(0,1), transform(0,2), transform(0,3));
	Render::SetVertexProgramParameter4f(kVertexParamMatrixCamera + 1, transform(1,0), transform(1,1), transform(1,2), transform(1,3));
	Render::SetVertexProgramParameter4f(kVertexParamMatrixCamera + 2, transform(2,0), transform(2,1), transform(2,2), transform(2,3));
}

void Renderable::StateFunc_TransformCameraMatrix(const Renderable *renderable, const void *cookie)
{
	const Transformable *geometryTransformable = renderable->GetTransformable();
	const Transform4D& cameraTransform = TheGraphicsMgr->GetCameraTransformable()->GetInverseWorldTransform();
	
	Transform4D transform = cameraTransform * geometryTransformable->GetWorldTransform();
	Render::SetVertexProgramParameter4f(kVertexParamMatrixCamera, transform(0,0), transform(0,1), transform(0,2), transform(0,3));
	Render::SetVertexProgramParameter4f(kVertexParamMatrixCamera + 1, transform(1,0), transform(1,1), transform(1,2), transform(1,3));
	Render::SetVertexProgramParameter4f(kVertexParamMatrixCamera + 2, transform(2,0), transform(2,1), transform(2,2), transform(2,3));
}

void Renderable::StateFunc_CopyWorldMatrix(const Renderable *renderable, const void *cookie)
{
	Render::SetVertexProgramParameter4fv(kVertexParamMatrixWorld, &K::identity_4D[0].x);
	Render::SetVertexProgramParameter4fv(kVertexParamMatrixWorld + 1, &K::identity_4D[1].x);
	Render::SetVertexProgramParameter4fv(kVertexParamMatrixWorld + 2, &K::identity_4D[2].x);
}

void Renderable::StateFunc_TransformWorldMatrix(const Renderable *renderable, const void *cookie)
{
	const Transform4D& transform = renderable->GetTransformable()->GetWorldTransform();
	Render::SetVertexProgramParameter4f(kVertexParamMatrixWorld, transform(0,0), transform(0,1), transform(0,2), transform(0,3));
	Render::SetVertexProgramParameter4f(kVertexParamMatrixWorld + 1, transform(1,0), transform(1,1), transform(1,2), transform(1,3));
	Render::SetVertexProgramParameter4f(kVertexParamMatrixWorld + 2, transform(2,0), transform(2,1), transform(2,2), transform(2,3));
}

void Renderable::StateFunc_TransformTexcoord0(const Renderable *renderable, const void *cookie)
{
	const MaterialObject *object = static_cast<const MaterialObject *>(cookie);
	
	const Vector2D& scale = object->GetTexcoordScale(0);
	const Vector2D& offset = object->GetTexcoordOffset(0);
	Render::SetVertexProgramParameter4f(kVertexParamTexcoordTransform0, scale.x, scale.y, offset.x, offset.y);
}

void Renderable::StateFunc_AnimateTexcoord0(const Renderable *renderable, const void *cookie)
{
	const MaterialObject *object = static_cast<const MaterialObject *>(cookie);
	
	const Vector2D& velocity = object->GetTexcoordVelocity(0);
	Render::SetVertexProgramParameter4f(kVertexParamTexcoordVelocity0, velocity.x, velocity.y, 0.0F, 0.0F);
}

void Renderable::StateFunc_TransformAnimateTexcoord0(const Renderable *renderable, const void *cookie)
{
	const MaterialObject *object = static_cast<const MaterialObject *>(cookie);
	
	const Vector2D& scale = object->GetTexcoordScale(0);
	const Vector2D& offset = object->GetTexcoordOffset(0);
	Render::SetVertexProgramParameter4f(kVertexParamTexcoordTransform0, scale.x, scale.y, offset.x, offset.y);
	
	const Vector2D& velocity = object->GetTexcoordVelocity(0);
	Render::SetVertexProgramParameter4f(kVertexParamTexcoordVelocity0, velocity.x, velocity.y, 0.0F, 0.0F);
}

void Renderable::StateFunc_TransformTexcoord1(const Renderable *renderable, const void *cookie)
{
	const MaterialObject *object = static_cast<const MaterialObject *>(cookie);
	
	const Vector2D& scale = object->GetTexcoordScale(1);
	const Vector2D& offset = object->GetTexcoordOffset(1);
	Render::SetVertexProgramParameter4f(kVertexParamTexcoordTransform1, scale.x, scale.y, offset.x, offset.y);
}

void Renderable::StateFunc_AnimateTexcoord1(const Renderable *renderable, const void *cookie)
{
	const MaterialObject *object = static_cast<const MaterialObject *>(cookie);
	
	const Vector2D& velocity = object->GetTexcoordVelocity(1);
	Render::SetVertexProgramParameter4f(kVertexParamTexcoordVelocity1, velocity.x, velocity.y, 0.0F, 0.0F);
}

void Renderable::StateFunc_TransformAnimateTexcoord1(const Renderable *renderable, const void *cookie)
{
	const MaterialObject *object = static_cast<const MaterialObject *>(cookie);
	
	const Vector2D& scale = object->GetTexcoordScale(1);
	const Vector2D& offset = object->GetTexcoordOffset(1);
	Render::SetVertexProgramParameter4f(kVertexParamTexcoordTransform1, scale.x, scale.y, offset.x, offset.y);
	
	const Vector2D& velocity = object->GetTexcoordVelocity(1);
	Render::SetVertexProgramParameter4f(kVertexParamTexcoordVelocity1, velocity.x, velocity.y, 0.0F, 0.0F);
}

void Renderable::StateFunc_ScaleTerrainTexcoord(const Renderable *renderable, const void *cookie)
{
	const MaterialObject *object = static_cast<const MaterialObject *>(cookie);
	Render::SetVertexProgramParameter4f(kVertexParamTerrainTexcoordScale, object->GetTexcoordGeneration().x, 0.0F, 0.0F, 0.0F);
}

void Renderable::StateFunc_GenerateTexcoord(const Renderable *renderable, const void *cookie)
{
	Render::SetVertexProgramParameter4fv(kVertexParamTexcoordGenerate, &renderable->GetTexcoordParameterPointer()->x);
}

void Renderable::StateFunc_GenerateTransformTexcoord0(const Renderable *renderable, const void *cookie)
{
	const MaterialObject *object = static_cast<const MaterialObject *>(cookie);
	
	const Vector2D& scale = object->GetTexcoordScale(0);
	const Vector2D& offset = object->GetTexcoordOffset(0);
	const Vector4D *param = renderable->GetTexcoordParameterPointer();
	Render::SetVertexProgramParameter4f(kVertexParamTexcoordTransform0, param->x * scale.x, param->y * scale.y, offset.x, offset.y);
}

void Renderable::StateFunc_GenerateAnimateTexcoord0(const Renderable *renderable, const void *cookie)
{
	const MaterialObject *object = static_cast<const MaterialObject *>(cookie);
	
	const Vector2D& velocity = object->GetTexcoordVelocity(0);
	Render::SetVertexProgramParameter4f(kVertexParamTexcoordVelocity0, velocity.x, velocity.y, 0.0F, 0.0F);
	Render::SetVertexProgramParameter4fv(kVertexParamTexcoordGenerate, &renderable->GetTexcoordParameterPointer()->x);
}

void Renderable::StateFunc_GenerateTransformAnimateTexcoord0(const Renderable *renderable, const void *cookie)
{
	const MaterialObject *object = static_cast<const MaterialObject *>(cookie);
	
	const Vector2D& scale = object->GetTexcoordScale(0);
	const Vector2D& offset = object->GetTexcoordOffset(0);
	const Vector4D *param = renderable->GetTexcoordParameterPointer();
	Render::SetVertexProgramParameter4f(kVertexParamTexcoordTransform0, param->x * scale.x, param->y * scale.y, offset.x, offset.y);
	
	const Vector2D& velocity = object->GetTexcoordVelocity(0);
	Render::SetVertexProgramParameter4f(kVertexParamTexcoordVelocity0, velocity.x, velocity.y, 0.0F, 0.0F);
}

void Renderable::StateFunc_GenerateTransformTexcoord1(const Renderable *renderable, const void *cookie)
{
	const MaterialObject *object = static_cast<const MaterialObject *>(cookie);
	
	const Vector2D& scale = object->GetTexcoordScale(1);
	const Vector2D& offset = object->GetTexcoordOffset(1);
	const Vector4D *param = renderable->GetTexcoordParameterPointer();
	Render::SetVertexProgramParameter4f(kVertexParamTexcoordTransform1, param->x * scale.x, param->y * scale.y, offset.x, offset.y);
}

void Renderable::StateFunc_GenerateAnimateTexcoord1(const Renderable *renderable, const void *cookie)
{
	const MaterialObject *object = static_cast<const MaterialObject *>(cookie);
	
	const Vector2D& velocity = object->GetTexcoordVelocity(1);
	Render::SetVertexProgramParameter4f(kVertexParamTexcoordVelocity1, velocity.x, velocity.y, 0.0F, 0.0F);
	Render::SetVertexProgramParameter4fv(kVertexParamTexcoordGenerate, &renderable->GetTexcoordParameterPointer()->x);
}

void Renderable::StateFunc_GenerateTransformAnimateTexcoord1(const Renderable *renderable, const void *cookie)
{
	const MaterialObject *object = static_cast<const MaterialObject *>(cookie);
	
	const Vector2D& scale = object->GetTexcoordScale(1);
	const Vector2D& offset = object->GetTexcoordOffset(1);
	const Vector4D *param = renderable->GetTexcoordParameterPointer();
	Render::SetVertexProgramParameter4f(kVertexParamTexcoordTransform1, param->x * scale.x, param->y * scale.y, offset.x, offset.y);
	
	const Vector2D& velocity = object->GetTexcoordVelocity(1);
	Render::SetVertexProgramParameter4f(kVertexParamTexcoordVelocity1, velocity.x, velocity.y, 0.0F, 0.0F);
}

void Renderable::StateFunc_GenerateAnimateDualTexcoords(const Renderable *renderable, const void *cookie)
{
	const MaterialObject *object = static_cast<const MaterialObject *>(cookie);
	
	const Vector2D& velocity0 = object->GetTexcoordVelocity(0);
	const Vector2D& velocity1 = object->GetTexcoordVelocity(1);
	Render::SetVertexProgramParameter4f(kVertexParamTexcoordVelocity0, velocity0.x, velocity0.y, 0.0F, 0.0F);
	Render::SetVertexProgramParameter4f(kVertexParamTexcoordVelocity1, velocity1.x, velocity1.y, 0.0F, 0.0F);
	Render::SetVertexProgramParameter4fv(kVertexParamTexcoordGenerate, &renderable->GetTexcoordParameterPointer()->x);
}

void Renderable::StateFunc_ConfigureInfiniteLight(const Renderable *renderable, const void *cookie)
{
	const Vector3D& lightDirection = TheGraphicsMgr->GetLightTransformable()->GetWorldTransform()[2];
	Render::SetVertexProgramParameter4f(kVertexParamLightPosition, lightDirection.x, lightDirection.y, lightDirection.z, 0.0F);
}

void Renderable::StateFunc_ConfigureTransformInfiniteLight(const Renderable *renderable, const void *cookie)
{
	const Vector3D& lightDirection = TheGraphicsMgr->GetLightTransformable()->GetWorldTransform()[2];
	const Transformable *geometryTransformable = renderable->GetTransformable();
	
	Vector3D ldir = geometryTransformable->GetInverseWorldTransform() * lightDirection;
	Render::SetVertexProgramParameter4f(kVertexParamLightPosition, ldir.x, ldir.y, ldir.z, 0.0F);
}

void Renderable::StateFunc_ConfigureDepthLight(const Renderable *renderable, const void *cookie)
{
	const Transformable *lightTransformable = TheGraphicsMgr->GetLightTransformable();
	const Vector3D& lightDirection = lightTransformable->GetWorldTransform()[2];
	
	Render::SetVertexProgramParameter4f(kVertexParamLightPosition, lightDirection.x, lightDirection.y, lightDirection.z, 0.0F);
	
	const Transform4D& m = lightTransformable->GetInverseWorldTransform();
	
	const LightShadowData *shadowData = TheGraphicsMgr->GetLightShadowData();
	float w = -shadowData->inverseShadowSize.x;
	float r = -shadowData->inverseShadowSize.z;
	
	#if !C4PLAYSTATION3
	
		float h = -kInverseMaxShadowSectionCount * shadowData->inverseShadowSize.y;
		
		Render::SetVertexProgramParameter4f(kVertexParamMatrixShadow, m(0,0) * w, m(0,1) * w, m(0,2) * w, (m(0,3) - shadowData->shadowPosition.x) * w + 0.5F);
		Render::SetVertexProgramParameter4f(kVertexParamMatrixShadow + 1, m(1,0) * h, m(1,1) * h, m(1,2) * h, (m(1,3) - shadowData->shadowPosition.y) * h + 0.5F * kInverseMaxShadowSectionCount);
		Render::SetVertexProgramParameter4f(kVertexParamMatrixShadow + 2, m(2,0) * r, m(2,1) * r, m(2,2) * r, (m(2,3) - shadowData->shadowPosition.z) * r);
	
	#else
	
		float h = kInverseMaxShadowSectionCount * shadowData->inverseShadowSize.y;
		
		Render::SetVertexProgramParameter4f(kVertexParamMatrixShadow, m(0,0) * w, m(0,1) * w, m(0,2) * w, (m(0,3) - shadowData->shadowPosition.x) * w + 0.5F);
		Render::SetVertexProgramParameter4f(kVertexParamMatrixShadow + 1, m(1,0) * h, m(1,1) * h, m(1,2) * h, (m(1,3) - shadowData->shadowPosition.y) * h + ((float) (kMaxShadowSectionCount - 1) + 0.5F) * kInverseMaxShadowSectionCount);
		Render::SetVertexProgramParameter4f(kVertexParamMatrixShadow + 2, m(2,0) * r, m(2,1) * r, m(2,2) * r, (m(2,3) - shadowData->shadowPosition.z) * r);
	
	#endif
}

void Renderable::StateFunc_ConfigureTransformDepthLight(const Renderable *renderable, const void *cookie)
{
	const Transformable *lightTransformable = TheGraphicsMgr->GetLightTransformable();
	const Transformable *geometryTransformable = renderable->GetTransformable();
	const Vector3D& lightDirection = lightTransformable->GetWorldTransform()[2];
	
	Vector3D ldir = geometryTransformable->GetInverseWorldTransform() * lightDirection;
	Render::SetVertexProgramParameter4f(kVertexParamLightPosition, ldir.x, ldir.y, ldir.z, 0.0F);
	
	Transform4D m = lightTransformable->GetInverseWorldTransform() * geometryTransformable->GetWorldTransform();
	
	const LightShadowData *shadowData = TheGraphicsMgr->GetLightShadowData();
	float w = -shadowData->inverseShadowSize.x;
	float r = -shadowData->inverseShadowSize.z;
	
	#if !C4PLAYSTATION3
	
		float h = -kInverseMaxShadowSectionCount * shadowData->inverseShadowSize.y;
		
		Render::SetVertexProgramParameter4f(kVertexParamMatrixShadow, m(0,0) * w, m(0,1) * w, m(0,2) * w, (m(0,3) - shadowData->shadowPosition.x) * w + 0.5F);
		Render::SetVertexProgramParameter4f(kVertexParamMatrixShadow + 1, m(1,0) * h, m(1,1) * h, m(1,2) * h, (m(1,3) - shadowData->shadowPosition.y) * h + 0.5F * kInverseMaxShadowSectionCount);
		Render::SetVertexProgramParameter4f(kVertexParamMatrixShadow + 2, m(2,0) * r, m(2,1) * r, m(2,2) * r, (m(2,3) - shadowData->shadowPosition.z) * r);
	
	#else
	
		float h = kInverseMaxShadowSectionCount * shadowData->inverseShadowSize.y;
		
		Render::SetVertexProgramParameter4f(kVertexParamMatrixShadow, m(0,0) * w, m(0,1) * w, m(0,2) * w, (m(0,3) - shadowData->shadowPosition.x) * w + 0.5F);
		Render::SetVertexProgramParameter4f(kVertexParamMatrixShadow + 1, m(1,0) * h, m(1,1) * h, m(1,2) * h, (m(1,3) - shadowData->shadowPosition.y) * h + ((float) (kMaxShadowSectionCount - 1) + 0.5F) * kInverseMaxShadowSectionCount);
		Render::SetVertexProgramParameter4f(kVertexParamMatrixShadow + 2, m(2,0) * r, m(2,1) * r, m(2,2) * r, (m(2,3) - shadowData->shadowPosition.z) * r);
	
	#endif
}

void Renderable::StateFunc_ConfigureLandscapeLight(const Renderable *renderable, const void *cookie)
{
	const Transformable *lightTransformable = TheGraphicsMgr->GetLightTransformable();
	const Vector3D& lightDirection = lightTransformable->GetWorldTransform()[2];
	
	Render::SetVertexProgramParameter4f(kVertexParamLightPosition, lightDirection.x, lightDirection.y, lightDirection.z, 0.0F);
	
	const Transform4D& m = lightTransformable->GetInverseWorldTransform();
	
	const LightShadowData *shadowData = TheGraphicsMgr->GetLightShadowData();
	float w = -shadowData->inverseShadowSize.x;
	float h = -shadowData->inverseShadowSize.y * kInverseMaxShadowSectionCount;
	float r = -shadowData->inverseShadowSize.z;
	float offset = 0.5F * kInverseMaxShadowSectionCount;
	
	#if C4PLAYSTATION3
	
		h = -h;
		offset = 1.0F - offset;
	
	#endif
	
	Render::SetVertexProgramParameter4f(kVertexParamMatrixShadow, m(0,0) * w, m(0,1) * w, m(0,2) * w, (m(0,3) - shadowData->shadowPosition.x) * w + 0.5F);
	Render::SetVertexProgramParameter4f(kVertexParamMatrixShadow + 1, m(1,0) * h, m(1,1) * h, m(1,2) * h, (m(1,3) - shadowData->shadowPosition.y) * h + offset);
	Render::SetVertexProgramParameter4f(kVertexParamMatrixShadow + 2, m(2,0) * r, m(2,1) * r, m(2,2) * r, (m(2,3) - shadowData->shadowPosition.z) * r);
	
	Render::SetVertexProgramParameter4fv(kVertexParamShadowSectionPlane1, &shadowData[0].sectionPlane.x);
	Render::SetVertexProgramParameter4fv(kVertexParamShadowSectionPlane2, &shadowData[1].sectionPlane.x);
	Render::SetVertexProgramParameter4fv(kVertexParamShadowSectionPlane3, &shadowData[2].sectionPlane.x);
}

void Renderable::StateFunc_ConfigureTransformLandscapeLight(const Renderable *renderable, const void *cookie)
{
	const Transformable *lightTransformable = TheGraphicsMgr->GetLightTransformable();
	const Transformable *geometryTransformable = renderable->GetTransformable();
	const Vector3D& lightDirection = lightTransformable->GetWorldTransform()[2];
	
	Vector3D ldir = geometryTransformable->GetInverseWorldTransform() * lightDirection;
	Render::SetVertexProgramParameter4f(kVertexParamLightPosition, ldir.x, ldir.y, ldir.z, 0.0F);
	
	Transform4D m = lightTransformable->GetInverseWorldTransform() * geometryTransformable->GetWorldTransform();
	
	const LightShadowData *shadowData = TheGraphicsMgr->GetLightShadowData();
	float w = -shadowData->inverseShadowSize.x;
	float h = -shadowData->inverseShadowSize.y * kInverseMaxShadowSectionCount;
	float r = -shadowData->inverseShadowSize.z;
	float offset = 0.5F * kInverseMaxShadowSectionCount;
	
	#if C4PLAYSTATION3
	
		h = -h;
		offset = 1.0F - offset;
	
	#endif
	
	Render::SetVertexProgramParameter4f(kVertexParamMatrixShadow, m(0,0) * w, m(0,1) * w, m(0,2) * w, (m(0,3) - shadowData->shadowPosition.x) * w + 0.5F);
	Render::SetVertexProgramParameter4f(kVertexParamMatrixShadow + 1, m(1,0) * h, m(1,1) * h, m(1,2) * h, (m(1,3) - shadowData->shadowPosition.y) * h + offset);
	Render::SetVertexProgramParameter4f(kVertexParamMatrixShadow + 2, m(2,0) * r, m(2,1) * r, m(2,2) * r, (m(2,3) - shadowData->shadowPosition.z) * r);
	
	Antivector4D sectionPlane1 = shadowData[0].sectionPlane * geometryTransformable->GetWorldTransform();
	Antivector4D sectionPlane2 = shadowData[1].sectionPlane * geometryTransformable->GetWorldTransform();
	Antivector4D sectionPlane3 = shadowData[2].sectionPlane * geometryTransformable->GetWorldTransform();
	Render::SetVertexProgramParameter4fv(kVertexParamShadowSectionPlane1, &sectionPlane1.x);
	Render::SetVertexProgramParameter4fv(kVertexParamShadowSectionPlane2, &sectionPlane2.x);
	Render::SetVertexProgramParameter4fv(kVertexParamShadowSectionPlane3, &sectionPlane3.x);
}


void Renderable::StateFunc_ConfigureLandscapeLightImpostor(const Renderable *renderable, const void *cookie)
{
	const Transformable *lightTransformable = TheGraphicsMgr->GetLightTransformable();
	const Vector3D& lightDirection = lightTransformable->GetWorldTransform()[2];
	
	Render::SetVertexProgramParameter4f(kVertexParamLightPosition, lightDirection.x, lightDirection.y, lightDirection.z, 0.0F);
	
	const Transform4D& m = lightTransformable->GetInverseWorldTransform();
	
	const LightShadowData *shadowData = TheGraphicsMgr->GetLightShadowData();
	float w = -shadowData->inverseShadowSize.x;
	float h = -shadowData->inverseShadowSize.y * kInverseMaxShadowSectionCount;
	float r = -shadowData->inverseShadowSize.z;
	float offset = 0.5F * kInverseMaxShadowSectionCount;
	
	#if C4PLAYSTATION3
	
		h = -h;
		offset = 1.0F - offset;
	
	#endif
	
	Render::SetVertexProgramParameter4f(kVertexParamMatrixShadow, m(0,0) * w, m(0,1) * w, m(0,2) * w, (m(0,3) - shadowData->shadowPosition.x) * w + 0.5F);
	Render::SetVertexProgramParameter4f(kVertexParamMatrixShadow + 1, m(1,0) * h, m(1,1) * h, m(1,2) * h, (m(1,3) - shadowData->shadowPosition.y) * h + offset);
	Render::SetVertexProgramParameter4f(kVertexParamMatrixShadow + 2, m(2,0) * r, m(2,1) * r, m(2,2) * r, (m(2,3) - shadowData->shadowPosition.z) * r);
	
	Render::SetVertexProgramParameter4fv(kVertexParamShadowSectionPlane1, &shadowData[0].sectionPlane.x);
	Render::SetVertexProgramParameter4fv(kVertexParamShadowSectionPlane2, &shadowData[1].sectionPlane.x);
	Render::SetVertexProgramParameter4fv(kVertexParamShadowSectionPlane3, &shadowData[2].sectionPlane.x);
}

void Renderable::StateFunc_ConfigurePointLight(const Renderable *renderable, const void *cookie)
{
	const Point3D& lightPosition = TheGraphicsMgr->GetLightTransformable()->GetWorldPosition();
	Render::SetVertexProgramParameter4f(kVertexParamLightPosition, lightPosition.x, lightPosition.y, lightPosition.z, 1.0F);
}

void Renderable::StateFunc_ConfigureTransformPointLight(const Renderable *renderable, const void *cookie)
{
	const Transformable *geometryTransformable = renderable->GetTransformable();
	Point3D lightPosition = geometryTransformable->GetInverseWorldTransform() * TheGraphicsMgr->GetLightTransformable()->GetWorldPosition();
	Render::SetVertexProgramParameter4f(kVertexParamLightPosition, lightPosition.x, lightPosition.y, lightPosition.z, 1.0F);
}

void Renderable::StateFunc_ConfigureCubeLight(const Renderable *renderable, const void *cookie)
{
	const Transformable *lightTransformable = TheGraphicsMgr->GetLightTransformable();
	
	const Point3D& lightPosition = lightTransformable->GetWorldPosition();
	Render::SetVertexProgramParameter4f(kVertexParamLightPosition, lightPosition.x, lightPosition.y, lightPosition.z, 1.0F);
	
	const Transform4D& m = lightTransformable->GetInverseWorldTransform();
	Render::SetVertexProgramParameter4f(kVertexParamMatrixLight, m(0,0), m(0,1), m(0,2), m(0,3));
	Render::SetVertexProgramParameter4f(kVertexParamMatrixLight + 1, m(1,0), m(1,1), m(1,2), m(1,3));
	Render::SetVertexProgramParameter4f(kVertexParamMatrixLight + 2, m(2,0), m(2,1), m(2,2), m(2,3));
}

void Renderable::StateFunc_ConfigureTransformCubeLight(const Renderable *renderable, const void *cookie)
{
	const Transformable *lightTransformable = TheGraphicsMgr->GetLightTransformable();
	const Transformable *geometryTransformable = renderable->GetTransformable();
	
	Point3D lightPosition = geometryTransformable->GetInverseWorldTransform() * TheGraphicsMgr->GetLightTransformable()->GetWorldPosition();
	Render::SetVertexProgramParameter4f(kVertexParamLightPosition, lightPosition.x, lightPosition.y, lightPosition.z, 1.0F);
	
	Transform4D m = lightTransformable->GetInverseWorldTransform() * geometryTransformable->GetWorldTransform();
	Render::SetVertexProgramParameter4f(kVertexParamMatrixLight, m(0,0), m(0,1), m(0,2), m(0,3));
	Render::SetVertexProgramParameter4f(kVertexParamMatrixLight + 1, m(1,0), m(1,1), m(1,2), m(1,3));
	Render::SetVertexProgramParameter4f(kVertexParamMatrixLight + 2, m(2,0), m(2,1), m(2,2), m(2,3));
}

void Renderable::StateFunc_ConfigureSpotLight(const Renderable *renderable, const void *cookie)
{
	const Transformable *lightTransformable = TheGraphicsMgr->GetLightTransformable();
	
	const Point3D& lightPosition = lightTransformable->GetWorldPosition();
	Render::SetVertexProgramParameter4f(kVertexParamLightPosition, lightPosition.x, lightPosition.y, lightPosition.z, 1.0F);
	
	const Transform4D& m = lightTransformable->GetInverseWorldTransform();
	Render::SetVertexProgramParameter4f(kVertexParamMatrixLight, m(0,0), m(0,1), m(0,2), m(0,3));
	Render::SetVertexProgramParameter4f(kVertexParamMatrixLight + 1, m(1,0), m(1,1), m(1,2), m(1,3));
	Render::SetVertexProgramParameter4f(kVertexParamMatrixLight + 2, m(2,0), m(2,1), m(2,2), m(2,3));
	
	const SpotLightObject *lightObject = static_cast<const SpotLightObject *>(TheGraphicsMgr->GetLightObject());
	float x = lightObject->GetApexTangent();
	float y = -x / lightObject->GetAspectRatio();
	
	Render::SetVertexProgramParameter4f(kVertexParamMatrixShadow, (x * m(0,0) + m(2,0)) * 0.5F, (x * m(0,1) + m(2,1)) * 0.5F, (x * m(0,2) + m(2,2)) * 0.5F, (x * m(0,3) + m(2,3)) * 0.5F);
	Render::SetVertexProgramParameter4f(kVertexParamMatrixShadow + 1, (y * m(1,0) + m(2,0)) * 0.5F, (y * m(1,1) + m(2,1)) * 0.5F, (y * m(1,2) + m(2,2)) * 0.5F, (y * m(1,3) + m(2,3)) * 0.5F);
}

void Renderable::StateFunc_ConfigureTransformSpotLight(const Renderable *renderable, const void *cookie)
{
	const Transformable *lightTransformable = TheGraphicsMgr->GetLightTransformable();
	const Transformable *geometryTransformable = renderable->GetTransformable();
	
	Point3D lightPosition = geometryTransformable->GetInverseWorldTransform() * TheGraphicsMgr->GetLightTransformable()->GetWorldPosition();
	Render::SetVertexProgramParameter4f(kVertexParamLightPosition, lightPosition.x, lightPosition.y, lightPosition.z, 1.0F);
	
	Transform4D m = lightTransformable->GetInverseWorldTransform() * geometryTransformable->GetWorldTransform();
	Render::SetVertexProgramParameter4f(kVertexParamMatrixLight, m(0,0), m(0,1), m(0,2), m(0,3));
	Render::SetVertexProgramParameter4f(kVertexParamMatrixLight + 1, m(1,0), m(1,1), m(1,2), m(1,3));
	Render::SetVertexProgramParameter4f(kVertexParamMatrixLight + 2, m(2,0), m(2,1), m(2,2), m(2,3));
	
	const SpotLightObject *lightObject = static_cast<const SpotLightObject *>(TheGraphicsMgr->GetLightObject());
	float x = lightObject->GetApexTangent();
	float y = -x / lightObject->GetAspectRatio();
	
	Render::SetVertexProgramParameter4f(kVertexParamMatrixShadow, (x * m(0,0) + m(2,0)) * 0.5F, (x * m(0,1) + m(2,1)) * 0.5F, (x * m(0,2) + m(2,2)) * 0.5F, (x * m(0,3) + m(2,3)) * 0.5F);
	Render::SetVertexProgramParameter4f(kVertexParamMatrixShadow + 1, (y * m(1,0) + m(2,0)) * 0.5F, (y * m(1,1) + m(2,1)) * 0.5F, (y * m(1,2) + m(2,2)) * 0.5F, (y * m(1,3) + m(2,3)) * 0.5F);
}

void Renderable::StateFunc_CopyVertexScaleOffset(const Renderable *renderable, const void *cookie)
{
	Render::SetVertexProgramParameter4fv(kVertexParamVertexScaleOffset, &renderable->GetRenderParameterPointer()->x);
}

void Renderable::StateFunc_CopyTerrainParameters(const Renderable *renderable, const void *cookie)
{
	const Vector4D *param = renderable->GetTerrainParameterPointer();
	Render::SetVertexProgramParameter4fv(kVertexParamTerrainParameter0, &param[0].x);
	Render::SetVertexProgramParameter4fv(kVertexParamTerrainParameter1, &param[1].x);
}

void Renderable::StateFunc_CopyImpostorTransition(const Renderable *renderable, const void *cookie)
{
	const Vector4D *param = renderable->GetRenderParameterPointer();
	Render::SetVertexProgramParameter4fv(kVertexParamImpostorTransition, &param[0].x);
}

void Renderable::StateFunc_CopyGeometryTransition(const Renderable *renderable, const void *cookie)
{
	const Vector4D *param = renderable->GetRenderParameterPointer();
	const Point3D& cameraPosition = TheGraphicsMgr->GetDirectCameraPosition();
	
	const Point2D& impostorPosition = param[0].GetPoint3D().GetPoint2D();
	Vector2D direction = impostorPosition - cameraPosition.GetPoint2D();
	float distance = SquaredMag(direction);
	float r = InverseSqrt(distance);
	distance *= r;
	
	float inverseDiameter = param[1].z;
	float inverseHeight = param[1].w;
	float dx = direction.x * inverseDiameter * r;
	float dy = direction.y * inverseDiameter * r;
	
	Render::SetVertexProgramParameter4f(kVertexParamImpostorPlaneS, dy, -dx, 0.0F, dx * impostorPosition.y - dy * impostorPosition.x + 0.5F);
	Render::SetVertexProgramParameter4f(kVertexParamImpostorPlaneT, 0.0F, 0.0F, inverseHeight, -param[0].w * inverseHeight);
	Render::SetFragmentProgramParameter4f(kFragmentParamImpostorDistance, distance * param[1].x + param[1].y, 0.0F, 0.0F, 0.0F);
}

void Renderable::StateFunc_TransformGeometryTransition(const Renderable *renderable, const void *cookie)
{
	const Vector4D *param = renderable->GetRenderParameterPointer();
	const Point3D& cameraPosition = TheGraphicsMgr->GetDirectCameraPosition();
	
	const Point2D& impostorPosition = param[0].GetPoint3D().GetPoint2D();
	Vector2D direction = impostorPosition - cameraPosition.GetPoint2D();
	float distance = SquaredMag(direction);
	float r = InverseSqrt(distance);
	distance *= r;
	
	float inverseDiameter = param[1].z;
	float inverseHeight = param[1].w;
	float dx = direction.x * inverseDiameter * r;
	float dy = direction.y * inverseDiameter * r;
	
	const Transform4D& transform = renderable->GetTransformable()->GetWorldTransform();
	
	Render::SetVertexProgramParameter4f(kVertexParamImpostorPlaneS, dy * transform(0,0) - dx * transform(1,0), dy * transform(0,1) - dx * transform(1,1), dy * transform(0,2) - dx * transform(1,2), dy * transform(0,3) - dx * transform(1,3) + dx * impostorPosition.y - dy * impostorPosition.x + 0.5F);
	Render::SetVertexProgramParameter4f(kVertexParamImpostorPlaneT, inverseHeight * transform(2,0), inverseHeight * transform(2,1), inverseHeight * transform(2,2), inverseHeight * transform(2,3) - param[0].w * inverseHeight);
	Render::SetFragmentProgramParameter4f(kFragmentParamImpostorDistance, distance * param[1].x + param[1].y, 0.0F, 0.0F, 0.0F);
}

void Renderable::StateFunc_CopyPaintSpace(const Renderable *renderable, const void *cookie)
{
	const Transform4D& paintTransform = renderable->paintEnvironment->paintTransform;
	
	Render::SetVertexProgramParameter4f(kVertexParamPaintPlaneS, paintTransform(0,0), paintTransform(0,1), paintTransform(0,2), paintTransform(0,3));
	Render::SetVertexProgramParameter4f(kVertexParamPaintPlaneT, paintTransform(1,0), paintTransform(1,1), paintTransform(1,2), paintTransform(1,3));
}

void Renderable::StateFunc_TransformPaintSpace(const Renderable *renderable, const void *cookie)
{
	const Transform4D& worldTransform = renderable->GetTransformable()->GetWorldTransform();
	const Transform4D& paintTransform = renderable->paintEnvironment->paintTransform;
	
	const MatrixRow4D& x = paintTransform.GetRow(0);
	const MatrixRow4D& y = paintTransform.GetRow(1);
	
	Render::SetVertexProgramParameter4f(kVertexParamPaintPlaneS, x ^ worldTransform[0], x ^ worldTransform[1], x ^ worldTransform[2], x ^ worldTransform.GetTranslation());
	Render::SetVertexProgramParameter4f(kVertexParamPaintPlaneT, y ^ worldTransform[0], y ^ worldTransform[1], y ^ worldTransform[2], y ^ worldTransform.GetTranslation());
}

void Renderable::StateFunc_SetOcclusionQuery(const Renderable *renderable, const void *cookie)
{
	TheGraphicsMgr->SetOcclusionQuery(renderable->occlusionQuery);
}

void Renderable::InvalidateShaderData(void)
{
	RenderSegment *segment = &renderSegment;
	do
	{
		segment->InvalidateShaderData();
		segment = segment->GetNextRenderSegment();
	} while (segment);
}

void Renderable::InvalidateAmbientShaderData(void)
{
	RenderSegment *segment = &renderSegment;
	do
	{
		segment->InvalidateAmbientShaderData();
		segment = segment->GetNextRenderSegment();
	} while (segment);
}

// ZYURVUR
