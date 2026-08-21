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


#include "C4Shaders.h"
#include "C4Graphics.h"


using namespace C4;


char ShaderAttribute::sourceStorage[kMaxShaderSourceSize];
unsigned_int32 ShaderAttribute::signatureStorage[kMaxShaderSignatureSize];


ShaderAttribute::ShaderAttribute() : Attribute(kAttributeShader)
{
}

ShaderAttribute::ShaderAttribute(const ShaderAttribute& shaderAttribute) : Attribute(shaderAttribute)
{
	for (machine a = 0; a < kShaderGraphCount; a++) CloneShader(&shaderAttribute.shaderGraph[a], &shaderGraph[a]);
}

ShaderAttribute::~ShaderAttribute()
{
}

Attribute *ShaderAttribute::Replicate(void) const
{
	return (new ShaderAttribute(*this));
}

void ShaderAttribute::Pack(Packer& data, unsigned_int32 packFlags) const
{
	Attribute::Pack(data, packFlags);
	
	for (machine index = 0; index < kShaderGraphCount; index++)
	{
		PackHandle handle = data.BeginChunk('SHD0' + index);
		PackShader(&shaderGraph[index], data, packFlags);
		data.EndChunk(handle);
	}
	
	data << TerminatorChunk;
}

void ShaderAttribute::Unpack(Unpacker& data, unsigned_int32 unpackFlags)
{
	Attribute::Unpack(data, unpackFlags);
	UnpackChunkList<ShaderAttribute>(data, unpackFlags);
}

bool ShaderAttribute::UnpackChunk(const ChunkHeader *chunkHeader, Unpacker& data, unsigned_int32 unpackFlags)
{
	unsigned_int32 index = chunkHeader->chunkType - 'SHD0';
	if (index < (unsigned_int32) kShaderGraphCount)
	{
		Process **processTable = UnpackShader(&shaderGraph[index], data, unpackFlags);
		delete[] processTable;
		return (true);
	}
	
	return (false);
}

void *ShaderAttribute::BeginSettingsUnpack(void)
{
	for (machine index = kShaderGraphCount - 1; index >= 0; index--) shaderGraph[index].Purge();
	return (Attribute::BeginSettingsUnpack());
}

void ShaderAttribute::PackShader(const ShaderGraph *graph, Packer& data, unsigned_int32 packFlags)
{
	int32 processCount = 0;
	int32 routeCount = 0;
	
	const Process *process = graph->GetFirstElement();
	while (process)
	{
		if ((process->GetProcessType() != kProcessSection) || (packFlags & kPackEditor))
		{
			routeCount += process->GetIncomingEdgeCount();
			process->processIndex = processCount;
			processCount++;
		}
		
		process = process->GetNextElement();
	}
	
	data << processCount;
	data << routeCount;
	
	process = graph->GetFirstElement();
	while (process)
	{
		if ((process->GetProcessType() != kProcessSection) || (packFlags & kPackEditor))
		{
			PackHandle handle = data.BeginSection();
			process->PackType(data);
			process->Pack(data, packFlags);
			data.EndSection(handle);
		}
		
		process = process->GetNextElement();
	} 
	
	process = graph->GetFirstElement(); 
	while (process) 
	{ 
		if ((process->GetProcessType() != kProcessSection) || (packFlags & kPackEditor))
		{ 
			const Route *route = process->GetFirstIncomingEdge();
			while (route)
			{
				data << route->GetStartElement()->processIndex; 
				data << route->GetFinishElement()->processIndex;
				
				PackHandle handle = data.BeginSection();
				route->Pack(data, packFlags); 
				data.EndSection(handle);
				
				route = route->GetNextIncomingEdge();
			}
		}
		
		process = process->GetNextElement();
	}
}

Process **ShaderAttribute::UnpackShader(ShaderGraph *graph, Unpacker& data, unsigned_int32 unpackFlags)
{
	int32		processCount;
	int32		routeCount;
	Process		**processTable;
	
	data >> processCount;
	data >> routeCount;
	
	processTable = new Process *[processCount];
	
	for (machine a = 0; a < processCount; a++)
	{
		unsigned_int32	size;
		
		data >> size;
		data.SetMark();
		
		if ((data.GetType() != kProcessSection) || (unpackFlags & kUnpackEditor))
		{
			Process *process = Process::Construct(data);
			if (process)
			{
				process->Unpack(++data, unpackFlags);
				processTable[a] = process;
				graph->AddElement(process);
				continue;
			}
		}
		
		data.Skip(size);
		processTable[a] = nullptr;
	}
	
	for (machine a = 0; a < routeCount; a++)
	{
		unsigned_int32	startIndex;
		unsigned_int32	finishIndex;
		unsigned_int32	size;
		
		data >> startIndex;
		data >> finishIndex;
		
		data >> size;
		data.SetMark();
		
		Process *startNode = processTable[startIndex];
		Process *finishNode = processTable[finishIndex];
		if ((startNode) && (finishNode))
		{
			Route *route = new Route(startNode, finishNode);
			route->Unpack(data, unpackFlags);
		}
		else
		{
			data.Skip(size);
		}
	}
	
	return (processTable);
}

bool ShaderAttribute::operator ==(const Attribute& attribute) const
{
	if (attribute.GetAttributeType() == kAttributeShader)
	{
		const ShaderAttribute *shaderAttribute = static_cast<const ShaderAttribute *>(&attribute);
		
		for (machine a = 0; a < kShaderGraphCount; a++)
		{
			int32 count = 0;
			const Process *process = shaderGraph[a].GetFirstElement();
			while (process)
			{
				process->processIndex = count++;
				process = process->GetNextElement();
			}
			
			int32 attributeCount = 0;
			const Process *attributeProcess = shaderAttribute->shaderGraph[a].GetFirstElement();
			while (attributeProcess)
			{
				attributeProcess->processIndex = attributeCount++;
				attributeProcess = attributeProcess->GetNextElement();
			}
			
			if (count != attributeCount) return (false);
			
			process = shaderGraph[a].GetFirstElement();
			attributeProcess = shaderAttribute->shaderGraph[a].GetFirstElement();
			while (process)
			{
				if (!(*process == *attributeProcess)) return (false);
				
				process = process->GetNextElement();
				attributeProcess = attributeProcess->GetNextElement();
			}
		}
		
		return (true);
	}
	
	return (false);
}

void ShaderAttribute::CloneShader(const ShaderGraph *sourceGraph, ShaderGraph *destinGraph, bool reference)
{
	const Process *process = sourceGraph->GetFirstElement();
	if (reference)
	{
		while (process)
		{
			Process *clone = process->Clone();
			process->cloneProcess = clone;
			destinGraph->AddElement(clone);
			clone->ReferenceStateParams(process);
			
			process = process->GetNextElement();
		}
	}
	else
	{
		while (process)
		{
			Process *clone = process->Clone();
			process->cloneProcess = clone;
			destinGraph->AddElement(clone);
			
			process = process->GetNextElement();
		}
	}
	
	process = sourceGraph->GetFirstElement();
	while (process)
	{
		Process *clone = process->cloneProcess;
		
		const Route *route = process->GetFirstIncomingEdge();
		while (route)
		{
			new Route(*route, route->GetStartElement()->cloneProcess, clone);
			route = route->GetNextIncomingEdge();
		}
		
		process = process->GetNextElement();
	}
}

ShaderResult ShaderAttribute::PrepareProcessPorts(const Process *process, const ShaderCompileData *compileData)
{
	int32 level = compileData->detailLevel;
	
	int32 portCount = process->GetPortCount();
	for (machine a = 0; a < portCount; a++)
	{
		Route *route = process->GetPortRoute(a);
		if (route)
		{
			if ((level > 0) && (route->GetRouteFlags() & kRouteHighDetail)) delete route;
		}
		else
		{
			if (!(process->GetPortFlags(a) & kProcessPortOptional)) return (kShaderIncomplete);
		}
	}
	
	return (kShaderOkay);
}

ShaderResult ShaderAttribute::PrepareAmbientShader(const ShaderCompileData *compileData, ShaderGraph *graph, List<Process> *terminalList)
{
	OutputProcess	*outputProcess[4];
	
	unsigned_int32 materialState = compileData->shaderData->materialState;
	unsigned_int32 targetDisableMask = TheGraphicsMgr->GetTargetDisableMask();
	
	Process *ambientProcess = nullptr;
	Process *ambientAlphaProcess = nullptr;
	Process *alphaTestProcess = nullptr;
	Process *glowProcess = nullptr;
	int32 outputProcessCount = 0;
	
	Process *process = graph->GetFirstElement();
	while (process)
	{
		ShaderResult result = PrepareProcessPorts(process, compileData);
		if (result != kShaderOkay) return (result);
		
		if (process->GetBaseProcessType() == kProcessOutput)
		{
			switch (process->GetProcessType())
			{
				case kProcessAmbientOutput:
					
					ambientProcess = process;
					delete process->GetPortRoute(1);		// [HACK] For now, always delete N input.
					break;
				
				case kProcessEmissionOutput:
				case kProcessEnvironmentOutput:
				case kProcessTerrainEnvironmentOutput:
					
					if (process->GetPortRoute(0)) outputProcess[outputProcessCount++] = static_cast<OutputProcess *>(process);
					break;
				
				case kProcessReflectionOutput:
					
					if (process->GetPortRoute(0))
					{
						if ((targetDisableMask & (1 << kRenderTargetReflection)) == 0) outputProcess[outputProcessCount++] = static_cast<OutputProcess *>(process);
						else process->PurgeIncomingEdges();
					}
					
					break;
				
				case kProcessRefractionOutput:
					
					if (process->GetPortRoute(0))
					{
						if ((targetDisableMask & (1 << kRenderTargetRefraction)) == 0) outputProcess[outputProcessCount++] = static_cast<OutputProcess *>(process);
						else process->PurgeIncomingEdges();
					}
					
					break;
				
				case kProcessAlphaTestOutput:
					
					if (process->GetFirstIncomingEdge())
					{
						if (materialState & kMaterialAlphaTest) alphaTestProcess = process;
					}
					
					break;
				
				case kProcessAmbientAlphaOutput:
					
					if (process->GetFirstIncomingEdge()) ambientAlphaProcess = process;
					break;
				
				case kProcessGlowOutput:
					
					if (process->GetFirstIncomingEdge())
					{
						if ((materialState & (kMaterialAlphaTest | kMaterialEmissionGlow)) == kMaterialEmissionGlow) glowProcess = process;
					}
					
					break;
			}
		}
		
		process = process->GetNextElement();
	}
	
	if (glowProcess)
	{
		terminalList->Append(glowProcess);
	}
	else
	{
		compileData->shaderData->materialState &= ~kMaterialEmissionGlow;
		
		if (alphaTestProcess)
		{
			terminalList->Append(alphaTestProcess);
			if ((ambientAlphaProcess) && (!(materialState & kMaterialAlphaCoverage))) terminalList->Append(ambientAlphaProcess);
		}
		else if (ambientAlphaProcess)
		{
			terminalList->Append(ambientAlphaProcess);
		}
	}
	
	if ((materialState & kMaterialVertexAmbientOcclusion) && (compileData->renderable->AttributeArrayEnabled(kArrayColor0)))
	{
		AmbientOcclusionOutputProcess *occlusionProcess = new AmbientOcclusionOutputProcess;
		graph->AddElement(occlusionProcess);
		
		new Route(ambientProcess, occlusionProcess, 0);
		ambientProcess = occlusionProcess;
	}
	
	Process *finalProcess = ambientProcess;
	
	if (outputProcessCount == 1)
	{
		finalProcess = new AddOutputProcess;
		graph->AddElement(finalProcess);
		
		new Route(ambientProcess, finalProcess, 0);
		new Route(outputProcess[0], finalProcess, 1);
	}
	else if (outputProcessCount == 2)
	{
		Process *addProcess = new AddOutputProcess;
		graph->AddElement(addProcess);
		
		new Route(outputProcess[0], addProcess, 0);
		new Route(outputProcess[1], addProcess, 1);
		
		finalProcess = new AddOutputProcess;
		graph->AddElement(finalProcess);
		
		new Route(ambientProcess, finalProcess, 0);
		new Route(addProcess, finalProcess, 1);
	}
	else if (outputProcessCount == 3)
	{
		Process *addProcess1 = new AddOutputProcess;
		graph->AddElement(addProcess1);
		
		new Route(outputProcess[0], addProcess1, 0);
		new Route(outputProcess[1], addProcess1, 1);
		
		Process *addProcess2 = new AddOutputProcess;
		graph->AddElement(addProcess2);
		
		new Route(outputProcess[2], addProcess2, 0);
		new Route(ambientProcess, addProcess2, 1);
		
		finalProcess = new AddOutputProcess;
		graph->AddElement(finalProcess);
		
		new Route(addProcess1, finalProcess, 0);
		new Route(addProcess2, finalProcess, 1);
	}
	else if (outputProcessCount == 4)
	{
		Process *addProcess1 = new AddOutputProcess;
		graph->AddElement(addProcess1);
		
		new Route(outputProcess[0], addProcess1, 0);
		new Route(outputProcess[1], addProcess1, 1);
		
		Process *addProcess2 = new AddOutputProcess;
		graph->AddElement(addProcess2);
		
		new Route(outputProcess[2], addProcess2, 0);
		new Route(outputProcess[3], addProcess2, 1);
		
		Process *addProcess3 = new AddOutputProcess;
		graph->AddElement(addProcess3);
		
		new Route(addProcess1, addProcess3, 0);
		new Route(addProcess2, addProcess3, 1);
		
		finalProcess = new AddOutputProcess;
		graph->AddElement(finalProcess);
		
		new Route(ambientProcess, finalProcess, 0);
		new Route(addProcess3, finalProcess, 1);
	}
	
	if (compileData->shaderVariant != kShaderVariantNormal)
	{
		Process		*fogProcess;
		Process		*blendProcess;
		
		if (compileData->shaderVariant == kShaderVariantConstantFog) fogProcess = new ConstantFogProcess;
		else fogProcess = new LinearFogProcess;
		graph->AddElement(fogProcess);
		
		const Renderable *renderable = compileData->renderable;
		
		if (renderable->GetShaderFlags() & kShaderAlphaFogFraction) blendProcess = new AlphaFogProcess;
		else if (GetBlendDest(renderable->GetAmbientBlendState()) != kBlendOne) blendProcess = new AmbientFogProcess;
		else blendProcess = new LightFogProcess;
		graph->AddElement(blendProcess);
		
		new Route(finalProcess, blendProcess, 0);
		new Route(fogProcess, blendProcess, 1);
		finalProcess = blendProcess;
	}
	
	terminalList->Append(finalProcess);
	return (kShaderOkay);
}

ShaderResult ShaderAttribute::PrepareLightShader(const ShaderCompileData *compileData, ShaderGraph *graph, List<Process> *terminalList)
{
	unsigned_int32 materialState = compileData->shaderData->materialState;
	
	Process *lightProcess = nullptr;
	Process *bloomProcess = nullptr;
	
	Process *process = graph->GetFirstElement();
	while (process)
	{
		ShaderResult result = PrepareProcessPorts(process, compileData);
		if (result != kShaderOkay) return (result);
		
		if (process->GetBaseProcessType() == kProcessOutput)
		{
			switch (process->GetProcessType())
			{
				case kProcessLightOutput:
					
					lightProcess = process;
					break;
				
				case kProcessAlphaTestOutput:
				
					if (process->GetFirstIncomingEdge())
					{
						if ((materialState & (kMaterialAlphaTest | kMaterialSpecularBloom)) == kMaterialAlphaTest) terminalList->Append(process);
						else process->PurgeIncomingEdges();
					}
					
					break;
				
				case kProcessBloomOutput:
					
					if (process->GetFirstIncomingEdge())
					{
						if (materialState & kMaterialSpecularBloom)
						{
							bloomProcess = process;
							terminalList->Append(process);
						}
						else
						{
							process->PurgeIncomingEdges();
						}
					}
					
					break;
			}
		}
		
		process = process->GetNextElement();
	}
	
	Process *finalProcess = lightProcess;
	if (compileData->shaderVariant != kShaderVariantNormal)
	{
		Process		*fogProcess;
		
		if (compileData->shaderVariant == kShaderVariantConstantFog) fogProcess = new ConstantFogProcess;
		else fogProcess = new LinearFogProcess;
		graph->AddElement(fogProcess);
		
		Process *blendProcess = new LightFogProcess;
		graph->AddElement(blendProcess);
		
		new Route(finalProcess, blendProcess, 0);
		new Route(fogProcess, blendProcess, 1);
		finalProcess = blendProcess;
	}
	
	if (bloomProcess)
	{
		ShaderData *shaderData = compileData->shaderData;
		shaderData->blendState = (shaderData->blendState & kBlendColorMask) | kBlendAlphaAccumulate;
		new Route(lightProcess, bloomProcess);
	}
	else
	{
		compileData->shaderData->materialState &= ~kMaterialSpecularBloom;
	}
	
	terminalList->Append(finalProcess);
	return (kShaderOkay);
}

ShaderResult ShaderAttribute::PreparePlainShader(ShaderType type, const ShaderCompileData *compileData, ShaderGraph *graph, List<Process> *terminalList)
{
	if (type == kShaderShadowMap)
	{
		graph->AddElement(new NullOutputProcess);
	}
	else if (type == kShaderStructure)
	{
		graph->AddElement(new StructureOutputProcess);
	}
	
	Process *alphaTestOutput = nullptr;
	Process *impostorDepthOutput = nullptr;
	
	Process *process = graph->GetFirstElement();
	while (process)
	{
		ShaderResult result = PrepareProcessPorts(process, compileData);
		if (result != kShaderOkay) return (result);
		
		ProcessType type = process->GetBaseProcessType();
		if (type == kProcessOutput)
		{
			switch (process->GetProcessType())
			{
				case kProcessNullOutput:
				case kProcessStructureOutput:
					
					terminalList->Append(process);
					break;
				
				case kProcessAlphaTestOutput:
					
					if (process->GetFirstIncomingEdge())
					{
						alphaTestOutput = process;
						terminalList->Append(process);
					}
					
					break;
					
				case kProcessImpostorDepthOutput:
					
					if (process->GetFirstIncomingEdge())
					{
						impostorDepthOutput = process;
						terminalList->Append(process);
					}
					
					break;
			}
		}
		
		process = process->GetNextElement();
	}
	
	if ((impostorDepthOutput) && (alphaTestOutput)) terminalList->Remove(alphaTestOutput);
	
	return (kShaderOkay);
}

Process *ShaderAttribute::FindDerivedInterpolant(ProcessType type, int32 count, Process *const *interpolant)
{
	for (machine a = 0; a < count; a++)
	{
		Process *process = *interpolant;
		if (process->GetProcessType() == type) return (process);
		interpolant++;
	}
	
	return (nullptr);
}

void ShaderAttribute::OrganizeDerivedInterpolants(const ShaderCompileData *compileData, ShaderGraph *graph)
{
	Process		*interpolantProcess[20];
	
	int32 interpolantCount = 0;
	
	Process *process = graph->GetFirstElement();
	while (process)
	{
		ProcessType		interpolantType[4];
		
		int32 count = process->GenerateDerivedInterpolantTypes(compileData, interpolantType);
		Assert(count <= 4, "Interpolant type array overflow");
		
		for (machine a = 0; a < count; a++)
		{
			ProcessType type = interpolantType[a];
			Process *interpolant = FindDerivedInterpolant(type, interpolantCount, interpolantProcess);
			if (!interpolant)
			{
				interpolant = Process::New(type);
				graph->AddElement(interpolant);
				
				Assert(interpolantCount < 20, "Interpolant process array overflow");
				interpolantProcess[interpolantCount++] = interpolant;
			}
			
			new Route(interpolant, process);
		}
		
		if (process->GetBaseProcessType() == kProcessDerived)
		{
			Process *interpolant = FindDerivedInterpolant(process->GetProcessType(), interpolantCount, interpolantProcess);
			if (interpolant)
			{
				Process *next = process->GetNextElement();
				
				if (interpolant != process)
				{
					for (;;)
					{
						Route *route = process->GetFirstOutgoingEdge();
						if (!route) break;
						
						route->SetStartElement(interpolant);
					}
						
					delete process;
				}
				
				process = next;
				continue;
			}
			
			Assert(interpolantCount < 20, "Interpolant process array overflow");
			interpolantProcess[interpolantCount++] = process;
		}
		
		process = process->GetNextElement();
	}
}

void ShaderAttribute::OptimizeTextureMaps(const ShaderGraph *graph)
{
	List<TextureMapProcess>		textureMapList;
	
	Process *process = graph->GetFirstElement();
	while (process)
	{
		if (process->GetBaseProcessType() == kProcessTextureMap)
		{
			if (process->GetPortCount() > 0)
			{
				const Route *inputRoute = process->GetPortRoute(0);
				if (inputRoute)
				{
					const Process *inputProcess = inputRoute->GetStartElement();
					TextureMapProcess *textureMapProcess = static_cast<TextureMapProcess *>(process);
					
					TextureMapProcess *previousTextureMap = textureMapList.First();
					while (previousTextureMap)
					{
						if (*textureMapProcess == *previousTextureMap)
						{
							const Route *previousRoute = previousTextureMap->GetPortRoute(0);
							if (*inputRoute == *previousRoute)
							{
								const Process *previousInputProcess = previousRoute->GetStartElement();
								if ((inputProcess == previousInputProcess) || ((inputProcess->GetBaseProcessType() == kProcessInterpolant) && (inputProcess->GetProcessType() == previousInputProcess->GetProcessType())))
								{
									Route *outputRoute = textureMapProcess->GetFirstOutgoingEdge();
									while (outputRoute)
									{
										Route *nextRoute = outputRoute->GetNextOutgoingEdge();
										outputRoute->SetStartElement(previousTextureMap);
										outputRoute = nextRoute;
									}
									
									goto next;
								}
							}
						}
						
						previousTextureMap = previousTextureMap->ListElement<TextureMapProcess>::Next();
					}
					
					textureMapList.Append(textureMapProcess);
				}
			}
			else
			{
				TextureMapProcess *textureMapProcess = static_cast<TextureMapProcess *>(process);
				
				TextureMapProcess *previousTextureMap = textureMapList.First();
				while (previousTextureMap)
				{
					if (*textureMapProcess == *previousTextureMap)
					{
						Route *outputRoute = textureMapProcess->GetFirstOutgoingEdge();
						while (outputRoute)
						{
							Route *nextRoute = outputRoute->GetNextOutgoingEdge();
							outputRoute->SetStartElement(previousTextureMap);
							outputRoute = nextRoute;
						}
						
						goto next;
					}
					
					previousTextureMap = previousTextureMap->ListElement<TextureMapProcess>::Next();
				}
				
				textureMapList.Append(textureMapProcess);
			}
		}
		
		next:
		process = process->GetNextElement();
	}
	
	textureMapList.RemoveAll();
}

void ShaderAttribute::EliminateDeadCode(const ShaderGraph *graph, List<Process> *terminalList)
{
	List<Process>	deadList;
	
	Process *process = graph->GetFirstElement();
	while (process)
	{
		if ((!process->GetFirstOutgoingEdge()) && (!terminalList->Member(process)))
		{
			if (process->GetBaseProcessType() == kProcessTerminal) terminalList->Append(process);
			else deadList.Append(process);
		}
		
		process = process->GetNextElement();
	}
	
	for (;;)
	{
		process = deadList.First();
		if (!process) break;
		
		Route *route = process->GetFirstIncomingEdge();
		while (route)
		{
			Route *next = route->GetNextIncomingEdge();
			
			Process *predecessor = route->GetStartElement();
			delete route;
			
			if (!predecessor->GetFirstOutgoingEdge()) deadList.Append(predecessor);
			
			route = next;
		}
		
		delete process;
	}
}

void ShaderAttribute::CalculatePathLengths(const ShaderGraph *graph, List<Process> *processList, List<Process> *readyList)
{
	Process *process = graph->GetFirstElement();
	while (process)
	{
		process->readyCount = (int16) process->GetOutgoingEdgeCount();
		process = process->GetNextElement();
	}
	
	for (;;)
	{
		process = processList->First();
		if (!process) break;
		
		int32 pathLength = 0;
		
		#if C4PLAYSTATION3
		
			unsigned_int32 compileFlags = 0;
		
		#endif
		
		const Route *route = process->GetFirstOutgoingEdge();
		while (route)
		{
			const Process *successor = route->GetFinishElement();
			int32 len = successor->pathLength + 1;
			pathLength = Max(pathLength, len);
			
			#if C4PLAYSTATION3
			
				compileFlags |= successor->GetPortCompileFlags(route->GetRoutePort()) | successor->compileFlags;
			
			#endif
			
			route = route->GetNextOutgoingEdge();
		}
		
		process->pathLength = (int16) pathLength;
		
		#if C4PLAYSTATION3
		
			process->compileFlags = compileFlags;
		
		#endif
		
		route = process->GetFirstIncomingEdge();
		if (route)
		{
			int32 count = 0;
			do
			{
				Process *predecessor = route->GetStartElement();
				if (--predecessor->readyCount == 0) processList->Append(predecessor);
				
				count++;
				route = route->GetNextIncomingEdge();
			} while (route);
			
			process->readyCount = (int16) count;
			processList->Remove(process);
		}
		else
		{
			Process *ready = readyList->Last();
			while (ready)
			{
				if (ready->pathLength >= pathLength)
				{
					readyList->InsertAfter(process, ready);
					goto next;
				}
				
				ready = ready->Previous();
			}
			
			readyList->Prepend(process);
		}
		
		next:;
	}
}

int32 ShaderAttribute::ScheduleShader(const ShaderCompileData *compileData, List<Process> *readyList, List<Process> *scheduleList, unsigned_int32 *shaderSignature)
{
	int32 processCount = 0;
	unsigned_int32 *signature = &shaderSignature[1];
	
	for (;;)
	{
		Process *process = readyList->First();
		if (!process) break;
		
		scheduleList->Append(process);
		process->processIndex = processCount;
		processCount++;
		
		process->GenerateSourceData(compileData);
		signature += process->GenerateProcessSignature(compileData, signature);
		
		const Route *route = process->GetFirstOutgoingEdge();
		while (route)
		{
			Process *successor = route->GetFinishElement();
			if (--successor->readyCount == 0)
			{
				int32 pathLength = successor->pathLength;
				
				Process *ready = readyList->Last();
				while (ready)
				{
					if (ready->pathLength >= pathLength)
					{
						readyList->InsertAfter(successor, ready);
						goto next;
					}
					
					ready = ready->Previous();
				}
				
				readyList->Prepend(successor);
			}
			
			next:
			route = route->GetNextOutgoingEdge();
		}
	}
	
	shaderSignature[0] = signature - shaderSignature - 1;
	return (processCount);
}

bool ShaderAttribute::AllocateOutputRegister(ProcessData *data, unsigned_int8 *registerLive)
{
	for (machine a = 0; a < kMaxShaderRegisterCount; a++)
	{
		if (registerLive[a] == 0)
		{
			registerLive[a] = 1;
			data->outputRegister = a;
			return (true);
		}
	}
	
	return (false);
}

bool ShaderAttribute::AllocateInterpolant(Type type, int32 size, ShaderAllocationData *allocData, bool (*usage)[4])
{
	int32 count = allocData->interpolantCount;
	if (count == kMaxShaderInterpolantCount) return (false);
	
	allocData->interpolantCount = count + 1;
	InterpolantData *data = &allocData->interpolantData[count];
	
	if ((type != kProcessTexcoord0) || (size != 4))
	{
		if (size >= 3)
		{
			for (machine a = 0; a < kMaxShaderTexcoordCount; a++)
			{
				if (*reinterpret_cast<int32 *>(usage) == 0)
				{
					data->interpolantType = type;
					data->texcoordIndex = a;
					
					data->swizzleData.size = size;
					data->swizzleData.negate = false;
					data->swizzleData.absolute = false;
					data->swizzleData.component[0] = 0;
					data->swizzleData.component[1] = 1;
					data->swizzleData.component[2] = 2;
					data->swizzleData.component[3] = 3;
					
					(*usage)[0] = true;
					(*usage)[1] = true;
					(*usage)[2] = true;
					if (size == 4) (*usage)[3] = true;
					return (true);
				}
				
				usage++;
			}
		}
		else if (size == 2)
		{
			for (machine a = 0; a < kMaxShaderTexcoordCount; a++)
			{
				if (*reinterpret_cast<int16 *>(usage) == 0)
				{
					data->interpolantType = type;
					data->texcoordIndex = a;
					
					data->swizzleData.size = 2;
					data->swizzleData.negate = false;
					data->swizzleData.absolute = false;
					data->swizzleData.component[0] = 0;
					data->swizzleData.component[1] = 1;
					data->swizzleData.component[2] = 1;
					data->swizzleData.component[3] = 1;
					
					(*usage)[0] = true;
					(*usage)[1] = true;
					return (true);
				}
				else if (*reinterpret_cast<int16 *>(&(*usage)[2]) == 0)
				{
					data->interpolantType = type;
					data->texcoordIndex = a;
					
					data->swizzleData.size = 2;
					data->swizzleData.negate = false;
					data->swizzleData.absolute = false;
					data->swizzleData.component[0] = 2;
					data->swizzleData.component[1] = 3;
					data->swizzleData.component[2] = 3;
					data->swizzleData.component[3] = 3;
					
					(*usage)[2] = true;
					(*usage)[3] = true;
					return (true);
				}
				
				usage++;
			}
		}
		else
		{
			for (machine a = 0; a < kMaxShaderTexcoordCount; a++)
			{
				if (*reinterpret_cast<unsigned_int32 *>(usage) != 0x01010101)
				{
					for (machine b = 0; b < 4; b++)
					{
						if (!(*usage)[b])
						{
							data->interpolantType = type;
							data->texcoordIndex = a;
							
							data->swizzleData.size = 1;
							data->swizzleData.negate = false;
							data->swizzleData.absolute = false;
							data->swizzleData.component[0] = b;
							data->swizzleData.component[1] = b;
							data->swizzleData.component[2] = b;
							data->swizzleData.component[3] = b;
							
							(*usage)[b] = true;
							return (true);
						}
					}
				}
				
				usage++;
			}
		}
	}
	else
	{
		data->interpolantType = kProcessTexcoord0;
		data->texcoordIndex = 0;
		
		data->swizzleData.size = 4;
		data->swizzleData.negate = false;
		data->swizzleData.absolute = false;
		data->swizzleData.component[0] = 0;
		data->swizzleData.component[1] = 1;
		data->swizzleData.component[2] = 2;
		data->swizzleData.component[3] = 3;
		
		return (true);
	}
	
	return (false);
}

int32 ShaderAttribute::AllocateTextureUnit(ShaderData *shaderData, const Render::TextureObject *textureObject)
{
	int32 count = shaderData->textureUnitCount;
	for (machine a = 0; a < count; a++)
	{
		if (shaderData->textureObject[a] == textureObject) return (a);
	}
	
	if (count == kMaxShaderTextureCount) return (-1);
	
	shaderData->textureUnitCount = count + 1;
	shaderData->textureObject[count] = textureObject;
	return (count);
}

ShaderResult ShaderAttribute::AllocateShaderResources(const ShaderCompileData *compileData, ShaderAllocationData *allocData, int32 processCount, ProcessData *processData, const List<Process> *scheduleList)
{
	union
	{
		unsigned_int8	registerLive[kMaxShaderRegisterCount];
		unsigned_int32	registerLiveLong[kMaxShaderRegisterCount / 4];
	};
	
	union
	{
		bool	texcoordUsage[kMaxShaderTexcoordCount][4];
		int32	texcoordUsageLong[kMaxShaderTexcoordCount];
	};
	
	int32	interpolantCount[4];
	Type	interpolantType[4][kMaxShaderInterpolantCount];
	
	allocData->maxRegister = -1;
	allocData->temporaryCount = 0;
	allocData->vdirCount = 0;
	
	#if C4PLAYSTATION3
	
		allocData->dependentTextureMask = 0;
	
	#endif
	
	allocData->literalCount = 0;
	allocData->interpolantCount = 0;
	
	for (machine a = 0; a < kMaxShaderRegisterCount / 4; a++) registerLiveLong[a] = 0;
	for (machine a = 0; a < kMaxShaderTexcoordCount; a++) texcoordUsageLong[a] = 0;
	for (machine a = 0; a < 4; a++) interpolantCount[a] = 0;
	
	const Renderable *renderable = compileData->renderable;
	bool pointSprite = ((renderable) && (renderable->GetRenderType() == kRenderPoints));
	if (pointSprite) texcoordUsageLong[0] = 0x01010101;
	
	const Process *process = scheduleList->First();
	while (process)
	{
		processData->registerCount = 0;
		processData->preregisterCount = 0;
		processData->temporaryCount = 0;
		processData->literalCount = 0;
		processData->interpolantCount = 0;
		processData->textureCount = 0;
		processData->passthruPort = -1;
		processData->outputRegister = -1;
		processData->outputCount = 0;
		
		process->processData = processData;
		process->GenerateProcessData(compileData, processData);
		
		if (processData->preregisterCount != 0)
		{
			if (!AllocateOutputRegister(processData, registerLive)) return (kShaderRegisterOverflow);
			
			allocData->maxRegister = Max(allocData->maxRegister, processData->outputRegister);
			processData->outputCount = process->GetOutgoingEdgeCount();
		}
		
		const Route *route = process->GetFirstIncomingEdge();
		while (route)
		{
			const ProcessData *predecessorData = route->GetStartElement()->GetProcessData();
			
			int32 predecessorRegister = predecessorData->outputRegister;
			if ((predecessorRegister >= 0) && (--predecessorData->outputCount == 0)) registerLive[predecessorRegister]--;
			
			if (route->GetRoutePort() == processData->passthruPort)
			{
				if (predecessorRegister >= 0) registerLive[predecessorRegister]++;
				
				processData->outputCount = process->GetOutgoingEdgeCount();
				processData->outputRegister = predecessorRegister;
			}
			
			route = route->GetNextIncomingEdge();
		}
		
		if (processData->registerCount != 0)
		{
			if (!AllocateOutputRegister(processData, registerLive)) return (kShaderRegisterOverflow);
			
			allocData->maxRegister = Max(allocData->maxRegister, processData->outputRegister);
			processData->outputCount = process->GetOutgoingEdgeCount();
		}
		
		allocData->temporaryCount = Max(allocData->temporaryCount, processData->temporaryCount);
		
		int32 literalCount = processData->literalCount;
		if (literalCount > 0)
		{
			int32 start = allocData->literalCount;
			int32 count = start + literalCount;
			if (count > kMaxShaderLiteralCount) return (kShaderLiteralOverflow);
			allocData->literalCount = count;
			
			for (machine a = 0; a < literalCount; a++) allocData->literalData[start + a] = processData->literalData[a];
		}
		
		for (machine a = 0; a < processData->interpolantCount; a++)
		{
			Type type = processData->interpolantType[a];
			allocData->vdirCount += (type == kProcessTangentViewDirection);
			
			int32 size = InterpolantProcess::GetInterpolantSize(type);
			Assert(size != 0, "Unrecognized interpolant");
			
			if ((type == kProcessTexcoord0) && (pointSprite)) size = 3;
			else size--;
			
			int32 count = interpolantCount[size];
			for (machine b = 0; b < count; b++)
			{
				if (interpolantType[size][b] == type) goto next;
			}
			
			if (count == kMaxShaderInterpolantCount) return (kShaderTexcoordOverflow);
			
			interpolantType[size][count] = type;
			interpolantCount[size] = count + 1;
			
			next:;
		}
		
		for (machine a = 0; a < processData->textureCount; a++)
		{
			int32 unit = AllocateTextureUnit(compileData->shaderData, processData->textureObject[a]);
			if (unit < 0) return (kShaderTextureUnitOverflow);
			processData->textureUnit[a] = (unsigned_int8) unit;
			
			if ((a == 0) && (process->GetBaseProcessType() == kProcessTextureMap)) *static_cast<const TextureMapProcess *>(process)->signatureUnit = unit;
			
			#if C4PLAYSTATION3
			
				if (process->compileFlags & kProcessDependentTexture) allocData->dependentTextureMask |= 1 << unit;
			
			#endif
		}
		
		process = process->Next();
		processData++;
	}
	
	for (machine a = 3; a >= 0; a--)
	{
		int32 count = interpolantCount[a];
		for (machine b = 0; b < count; b++)
		{
			if (!AllocateInterpolant(interpolantType[a][b], a + 1, allocData, texcoordUsage)) return (kShaderTexcoordOverflow);
		}
	}
	
	return (kShaderOkay);
}

int32 ShaderAttribute::GenerateSwizzleData(const char *code, SwizzleData *swizzleData)
{
	machine count = 0;
	for (; count < 4; count++)
	{
		int32 c = code[count] - 'a';
		if ((unsigned_int32) c >= 26U) break;
		swizzleData->component[count] = Route::swizzleTable[c];
	}
	
	swizzleData->size = count;
	return (count);
}

int32 ShaderAttribute::GenerateLiteralConstantValue(Type type, const ShaderAllocationData *allocData, char *value)
{
	int32 count = allocData->literalCount;
	const LiteralData *literalData = allocData->literalData;
	for (machine a = 0; a < count; a++)
	{
		if (literalData->literalType == type)
		{
			return (Text::FloatToString(literalData->literalValue, value, 15));
		}
		
		literalData++;
	}
	
	Assert(false, "Literal type not found");
	return (0);
}

void ShaderAttribute::GenerateShaderCode(const ShaderCompileData *compileData, const ShaderAllocationData *allocData, const ProcessData *processData, const List<Process> *scheduleList, char *program, int32 *length)
{
	static const char *const textureUnitName[2][kMaxShaderTextureCount] =
	{
		{"texture0", "texture1", "texture2", "texture3", "texture4",
		 "texture5", "texture6", "texture7", "texture8", "texture9"},
		 
		{"texture[0]", "texture[1]", "texture[2]", "texture[3]", "texture[4]",
		 "texture[5]", "texture[6]", "texture[7]", "texture[8]", "texture[9]"}
	};
	
	static const char *const textureTargetName[2][Render::kTextureTargetCount] =
	{
		{TEX2D, TEX3D, TEXRECT, TEXCUBE, "texture2DArray"},
		{"2D", "3D", "RECT", "CUBE", "ARRAY2D"}
	};
	
	const char *start = program;
	
	const Process *process = scheduleList->First();
	while (process)
	{
		const char	*codeArray[kMaxProcessCodeCount];
		
		int32 codeCount = (compileData->programFlag) ? process->GenerateProgramCode(compileData, codeArray) : process->GenerateShaderCode(compileData, codeArray);
		Assert(codeCount <= kMaxProcessCodeCount, "Shader code array overflow");
		
		for (machine codeIndex = 0; codeIndex < codeCount; codeIndex++)
		{
			const char *code = codeArray[codeIndex];
			for (;;)
			{
				SwizzleData		swizzleData;
				
				int32 c = *code++;
				if (c == 0) break;
				
				if (c == '%')
				{
					c = *code++;
					int32 n = c - '0';
					if ((unsigned_int32) n < 10U)
					{
						swizzleData.size = processData->inputSize[n];
						swizzleData.absolute = false;
						swizzleData.negate = (code[-3] == '-');
						program -= swizzleData.negate;
						
						if (*code != '.')
						{
							for (machine a = 0; a < 4; a++) swizzleData.component[a] = (unsigned_int8) a;
						}
						else
						{
							code++;
							code += GenerateSwizzleData(code, &swizzleData);
						}
						
						const Route *route = process->GetPortRoute(n);
						program += route->GenerateOutputIdentifier(compileData, allocData, &swizzleData, program);
						continue;
					}
					else
					{
						if ((c == 'I') && (code[0] == 'M') && (code[1] == 'G'))
						{
							int32 unit = processData->textureUnit[code[2] - '0'];
							program += Text::CopyText(textureUnitName[compileData->programFlag][unit], program);
							code += 3;
							continue;
						}
						else if ((c == 'T') && (code[0] == 'R') && (code[1] == 'G'))
						{
							const Render::TextureObject *textureObject = processData->textureObject[code[2] - '0'];
							program += Text::CopyText(textureTargetName[compileData->programFlag][textureObject->GetTextureTargetIndex()], program);
							code += 3;
							continue;
						}
					}
				}
				else if (c == '#')
				{
					program += process->GenerateOutputIdentifier(compileData, allocData, nullptr, program);
					
					c = *code;
					if (c != '#')
					{
						int32 size = processData->outputSize;
						if ((size != 4) && (c != '.'))
						{
							*program++ = '.';
							for (machine a = 0; a < size; a++) *program++ = Route::GetSwizzleChar(a);
						}
					}
					else
					{
						code++;
					}
					
					continue;
				}
				else if (c == '$')
				{
					Type type = (code[0] << 24) | (code[1] << 16) | (code[2] << 8) | code[3];
					code += 4;
					
					if (*code == '.')
					{
						swizzleData.absolute = false;
						swizzleData.negate = (code[-3] == '-');
						program -= swizzleData.negate;
						
						code++;
						code += GenerateSwizzleData(code, &swizzleData);
						program += InterpolantProcess::GetInterpolantName(type, compileData, allocData, program, &swizzleData);
					}
					else
					{
						program += InterpolantProcess::GetInterpolantName(type, compileData, allocData, program);
					}
					
					continue;
				}
				else if (c == '&')
				{
					Type type = (code[0] << 24) | (code[1] << 16) | (code[2] << 8) | code[3];
					code += 4;
					
					program += GenerateLiteralConstantValue(type, allocData, program);
					continue;
				}
				
				*program++ = c;
			}
		}
		
		process = process->Next();
		processData++;
	}
	
	*length = program - start;
}

int32 ShaderAttribute::GenerateShaderProlog(const ShaderCompileData *compileData, const ShaderAllocationData *allocData, char *program)
{
	int32 len = 0;
	
	if (compileData->programFlag)
	{
		const bool *extensionFlag = TheGraphicsMgr->GetCapabilities()->extensionFlag;
		if (extensionFlag[kExtensionGpuProgram4])
		{
			len = Text::CopyText("!!NVfp4.0\nOPTION ARB_precision_hint_fastest;\n", program);
		}
		else
		{
			len = Text::CopyText("!!ARBfp1.0\nOPTION ARB_precision_hint_fastest;\n", program);
			if (extensionFlag[kExtensionFragmentProgram2]) len += Text::CopyText("OPTION NV_fragment_program2;\n", program + len);
		}
		
		ShaderType type = compileData->shaderType;
		if ((type == kShaderDepthLight) || (type == kShaderLandscapeLight)) len += Text::CopyText("OPTION ARB_fragment_program_shadow;\n", program + len);
		
		len += Text::CopyText("TEMP temp", program + len);
	}
	else
	{
		#if C4OPENGL
			
			static const char prolog2[] =
			{
				"uniform vec4 param[" FRAGMENT_PARAM_COUNT "];\n"
				"void main()\n"
				"{\n"
				"vec4 temp"
			};
			
			static const char *const samplerParams[kShaderTypeCount + 1] =
			{
				// kShaderNone
				"uniform sampler2DRect colorTexture;\n"
				"uniform sampler2DRect velocityTexture;\n"
				"uniform sampler2DRect distortionTexture;\n"
				"uniform sampler2DRect glowTexture;\n",
				
				// kShaderAmbient
				"",
				
				// kShaderAmbientGradient
				"",
				
				// kShaderAmbientSpace
				"uniform sampler3D ambientTexture1;\n"
				"uniform sampler3D ambientTexture2;\n",
				
				// kShaderInfiniteLight
				"",
				
				// kShaderDepthLight
				"uniform sampler2DShadow shadowTexture;\n",
				
				// kShaderLandscapeLight
				"uniform sampler2DShadow shadowTexture;\n",
				
				// kShaderPointLight
				"",
				
				// kShaderCubeLight
				"uniform samplerCube projectionCUBE;\n",
				
				// kShaderSpotLight
				"uniform sampler2D projection2D;\n",
				
				// kShaderShadowMap
				"",
				
				// kShaderStructure
				""
			};
			
			static const char *const samplerTarget[Render::kTextureTargetCount] =
			{
				"uniform sampler2D ", "uniform sampler3D ", "uniform sampler2DRect ", "uniform samplerCube ", "uniform sampler2DArray "
			};
			
			static const char *const textureName[kMaxShaderTextureCount] =
			{
				"texture0;\n", "texture1;\n", "texture2;\n", "texture3;\n", "texture4;\n",
				"texture5;\n", "texture6;\n", "texture7;\n", "texture8;\n", "texture9;\n"
			};
		
		#else
		
			static const char prolog[] =
			{
				"struct resultStruct\n"
				"{\n"
					"half4 color : COLOR;\n"
					"float depth : DEPTH;\n"
				"};\n"
				
				"struct fragmentStruct\n"
				"{\n"
					"float4		position	: WPOS;\n"
					"half4		color		: COL0;\n"
					"half4		color1		: COL1;\n"
					"float4		texcoord	: TEX0;\n"
					"float4		texcoord1	: TEX1;\n"
					"float4		texcoord2	: TEX2;\n"
					"float4		texcoord3	: TEX3;\n"
					"float4		texcoord4	: TEX4;\n"
					"float4		texcoord5	: TEX5;\n"
					"float4		texcoord6	: TEX6;\n"
					"float4		texcoord7	: TEX7;\n"
				"};\n"
				
				"resultStruct main(fragmentStruct fragment, uniform float4 param[" FRAGMENT_PARAM_COUNT "] : C0"
			};
			
			static const char prolog2[] =
			{
				")\n"
				"{\n"
				"resultStruct result;\n"
				"float4 temp"
			};
			
			static const char *const samplerParams[kShaderTypeCount + 1] =
			{
				// kShaderNone
				", samplerRECT colorTexture : TEXUNIT0, samplerRECT velocityTexture : TEXUNIT1, samplerRECT distortionTexture : TEXUNIT2, samplerRECT glowTexture : TEXUNIT3",
				
				// kShaderAmbient
				"",
				
				// kShaderAmbientGradient
				"",
				
				// kShaderAmbientSpace
				", sampler3D ambientTexture1 : TEXUNIT" TEXTURE_UNIT_AMBIENT_SPACE1 ", sampler3D ambientTexture2 : TEXUNIT" TEXTURE_UNIT_AMBIENT_SPACE2,
				
				// kShaderInfiniteLight
				"",
				
				// kShaderDepthLight
				", sampler2D shadowTexture : TEXUNIT" TEXTURE_UNIT_LIGHT_PROJECTION,
				
				// kShaderLandscapeLight
				", sampler2D shadowTexture : TEXUNIT" TEXTURE_UNIT_LIGHT_PROJECTION,
				
				// kShaderPointLight
				"",
				
				// kShaderCubeLight
				", samplerCUBE projectionCUBE : TEXUNIT" TEXTURE_UNIT_LIGHT_PROJECTION,
				
				// kShaderSpotLight
				", sampler2D projection2D : TEXUNIT" TEXTURE_UNIT_LIGHT_PROJECTION,
				
				// kShaderShadowMap
				"",
				
				// kShaderStructure
				""
			};
			
			static const char *const samplerTarget[Render::kTextureTargetCount] =
			{
				", sampler2D ", ", sampler3D ", ", samplerRECT ", ", samplerCUBE ", ", sampler2D "
			};
			
			static const char *const textureName[kMaxShaderTextureCount] =
			{
				"texture0 : TEXUNIT0", "texture1 : TEXUNIT1", "texture2 : TEXUNIT2", "texture3 : TEXUNIT3", "texture4 : TEXUNIT4",
				"texture5 : TEXUNIT5", "texture6 : TEXUNIT6", "texture7 : TEXUNIT7", "texture8 : TEXUNIT8", "texture9 : TEXUNIT9"
			};
		
		#endif
		
		#if C4OPENGL
		
			len = Text::CopyText("#version 120\n#extension GL_ARB_texture_rectangle : require\n", program);
			if (TheGraphicsMgr->GetCapabilities()->extensionFlag[kExtensionGpuShader4]) len += Text::CopyText("#extension GL_EXT_gpu_shader4 : require\n", program + len);
		
		#else
		
			len += Text::CopyText(prolog, program + len);
		
		#endif
		
		len += Text::CopyText(samplerParams[compileData->shaderType + 1], program + len);
		
		const ShaderData *shaderData = compileData->shaderData;
		if (shaderData)
		{
			int32 unitCount = shaderData->textureUnitCount;
			for (machine a = 0; a < unitCount; a++)
			{
				const Render::TextureObject *textureObject = shaderData->textureObject[a];
				len += Text::CopyText(samplerTarget[textureObject->GetTextureTargetIndex()], program + len);
				len += Text::CopyText(textureName[a], program + len);
			}
		}
		
		len += Text::CopyText(prolog2, program + len);
	}
	
	int32 count = allocData->temporaryCount;
	for (machine a = 1; a <= count; a++)
	{
		len += Text::CopyText(", tmp", program + len);
		program[len++] = (char) (a + '0');
	}
	
	count = allocData->maxRegister;
	for (machine a = 0; a <= count; a++)
	{
		len += Text::CopyText(", r", program + len);
		if (a < 10)
		{
			program[len++] = (char) (a + '0');
		}
		else
		{
			int32 d = a / 10;
			program[len++] = (char) (d + '0');
			program[len++] = (char) (a - d * 10 + '0');
		}
	}
	
	len += Text::CopyText(";\n", program + len);
	
	return (len);
}

int32 ShaderAttribute::GenerateShaderEpilog(const ShaderCompileData *compileData, char *program)
{
	if (compileData->programFlag) return (Text::CopyText("END", program));
	
	#if C4OPENGL
	
		return (Text::CopyText("}\n", program));
	
	#else
	
		return (Text::CopyText("return result;\n}\n", program));
	
	#endif
}

int32 ShaderAttribute::GenerateVertexOutputName(Type type, const ShaderAllocationData *allocData, int32 mask, char *name)
{
	static const char *const texcoordName[kMaxShaderTexcoordCount] =
	{
		"result.texcoord[0]",
		"result.texcoord[1]",
		"result.texcoord[2]",
		"result.texcoord[3]",
		"result.texcoord[4]",
		"result.texcoord[5]",
		"result.texcoord[6]",
		"result.texcoord[7]"
	};
	
	int32 count = allocData->interpolantCount;
	const InterpolantData *interpolantData = allocData->interpolantData;
	for (machine a = 0; a < count; a++)
	{
		if (interpolantData->interpolantType == type)
		{
			int32 len = Text::CopyText(texcoordName[interpolantData->texcoordIndex], name);
			
			int32 size = interpolantData->swizzleData.size;
			if ((size < 4) && (mask >= 0))
			{
				name[len++] = '.';
				
				if (mask == 0)
				{
					for (machine a = 0; a < size; a++) name[len++] = Route::GetSwizzleChar(interpolantData->swizzleData.component[a]);
				}
				else
				{
					Assert((mask == 'x') || (mask == 'y'), "Illegal write mask");
					
					name[len++] = Route::GetSwizzleChar(interpolantData->swizzleData.component[mask - 'x']);
				}
			}
			
			return (len);
		}
		
		interpolantData++;
	}
	
	Assert(false, "Interpolant not found");
	return (0);
}

unsigned_int32 ShaderAttribute::GenerateVertexProgram(const ShaderCompileData *compileData, const ShaderAllocationData *allocData, VertexProgram **vertexProgram)
{
	union
	{
		bool			snippetFlag[kVertexSnippetCount];
		unsigned_int32	flagStorage[(kVertexSnippetCount + 3) / 4];
	};
	
	VertexAssembly assembly(signatureStorage);
	
	for (machine a = 0; a < (kVertexSnippetCount + 3) / 4; a++) flagStorage[a] = 0;
	
	unsigned_int32 stateFlags = 0;
	const Renderable *renderable = compileData->renderable;
	unsigned_int32 shaderFlags = renderable->GetShaderFlags();
	
	bool normalFlag = ((shaderFlags & (kShaderWaterElevation | kShaderNormalExpandVertex)) != 0);
	bool tangentFlag = false;
	bool bitangentFlag = false;
	
	for (machine a = 0; a < allocData->interpolantCount; a++)
	{
		switch (allocData->interpolantData[a].interpolantType)
		{
			case 'TEX0':
				
				if (renderable->GetRenderType() != kRenderPoints) stateFlags = renderable->BuildTexcoord0Transform(compileData->renderSegment, compileData->shaderData, &assembly, stateFlags);
				break;
			
			case 'TEX1':
				
				stateFlags = renderable->BuildTexcoord1Transform(compileData->renderSegment, compileData->shaderData, &assembly, stateFlags);
				break;
			
			case 'POSI':
				
				snippetFlag[kVertexSnippetOutputObjectPosition] = true;
				break;
			
			case 'NRML':
				
				snippetFlag[kVertexSnippetOutputObjectNormal] = true;
				normalFlag = true;
				break;
			
			case 'TANG':
				
				snippetFlag[kVertexSnippetOutputObjectTangent] = true;
				tangentFlag = true;
				break;
			
			case 'BTNG':
				
				snippetFlag[kVertexSnippetOutputObjectBitangent] = true;
				bitangentFlag = true;
				break;
			
			case 'WPOS':
				
				snippetFlag[kVertexSnippetOutputWorldPosition] = true;
				stateFlags |= kShaderStateWorldTransform;
				break;
			
			case 'WNRM':
				
				snippetFlag[kVertexSnippetOutputWorldNormal] = true;
				stateFlags |= kShaderStateWorldTransform;
				normalFlag = true;
				break;
			
			case 'WTAN':
				
				snippetFlag[kVertexSnippetOutputWorldTangent] = true;
				stateFlags |= kShaderStateWorldTransform;
				tangentFlag = true;
				break;
			
			case 'WBTN':
				
				snippetFlag[kVertexSnippetOutputWorldBitangent] = true;
				stateFlags |= kShaderStateWorldTransform;
				bitangentFlag = true;
				break;
			
			case 'NRMC':
				
				snippetFlag[kVertexSnippetOutputCameraNormal] = true;
				stateFlags |= kShaderStateCameraTransform;
				normalFlag = true;
				break;
			
			case 'GEOM':
				
				snippetFlag[kVertexSnippetOutputVertexGeometry] = true;
				break;
			
			case 'IDEP':
				
				snippetFlag[kVertexSnippetOutputImpostorDepth] = true;
				stateFlags |= kShaderStateCameraTransform;
				break;
			
			case 'IRAD':
				
				snippetFlag[kVertexSnippetOutputImpostorRadius] = true;
				stateFlags |= kShaderStateImpostorRadius;
				break;
			
			case 'ISRD':
				
				snippetFlag[kVertexSnippetOutputImpostorShadowRadius] = true;
				break;
			
			case 'LDIR':
				
				if (compileData->shaderType >= kShaderFirstPointLight)
				{
					snippetFlag[kVertexSnippetCalculateObjectPointLightDirection] = true;
					
					if (!(shaderFlags & kShaderVertexBillboard))
					{
						snippetFlag[kVertexSnippetOutputTangentPointLightDirection] = true;
						bitangentFlag = true;
					}
					else
					{
						snippetFlag[kVertexSnippetCalculateObjectViewDirection] = true;
						snippetFlag[kVertexSnippetOutputBillboardPointLightDirection] = true;
						stateFlags |= kShaderStateCameraPosition;
					}
				}
				else
				{
					if (!(shaderFlags & kShaderVertexBillboard))
					{
						snippetFlag[kVertexSnippetOutputTangentInfiniteLightDirection] = true;
						bitangentFlag = true;
					}
					else
					{
						snippetFlag[kVertexSnippetCalculateObjectViewDirection] = true;
						snippetFlag[kVertexSnippetOutputBillboardInfiniteLightDirection] = true;
						stateFlags |= kShaderStateCameraPosition;
					}
				}
				
				break;
			
			case 'VDIR':
				
				snippetFlag[kVertexSnippetCalculateObjectViewDirection] = true;
				snippetFlag[kVertexSnippetOutputTangentViewDirection] = true;
				stateFlags |= kShaderStateCameraPosition;
				
				if (compileData->shaderVariant == kShaderVariantNormal) bitangentFlag = true;
				break;
			
			case 'OLDR':
				
				if (compileData->shaderType >= kShaderFirstPointLight)
				{
					snippetFlag[kVertexSnippetCalculateObjectPointLightDirection] = true;
					snippetFlag[kVertexSnippetOutputObjectPointLightDirection] = true;
				}
				else
				{
					snippetFlag[kVertexSnippetOutputObjectInfiniteLightDirection] = true;
				}
				
				break;
			
			case 'OVDR':
				
				snippetFlag[kVertexSnippetCalculateObjectViewDirection] = true;
				snippetFlag[kVertexSnippetOutputObjectViewDirection] = true;
				stateFlags |= kShaderStateCameraPosition;
				break;
			
			case 'TLDR':
				
				snippetFlag[kVertexSnippetCalculateTerrainTangentData] = true;
				
				if (compileData->shaderType >= kShaderFirstPointLight) snippetFlag[kVertexSnippetOutputTerrainPointLightDirection] = true;
				else snippetFlag[kVertexSnippetOutputTerrainInfiniteLightDirection] = true;
				
				normalFlag = true;
				break;
			
			case 'TVDR':
				
				snippetFlag[kVertexSnippetCalculateTerrainTangentData] = true;
				snippetFlag[kVertexSnippetOutputTerrainViewDirection] = true;
				stateFlags |= kShaderStateCameraPosition;
				break;
			
			case 'TWNM':
				
				snippetFlag[kVertexSnippetCalculateTerrainTangentData] = true;
				snippetFlag[kVertexSnippetOutputTerrainWorldTangentFrame] = true;
				break;
			
			case 'RTXC':
				
				renderable->SetShaderArray(compileData->shaderData, kShaderArrayTexture0, kArrayTexture0);
				snippetFlag[kVertexSnippetOutputRawTexcoords] = true;
				break;
			
			case 'TERA':
				
				snippetFlag[kVertexSnippetOutputTerrainTexcoords] = true;
				stateFlags |= kShaderStateTerrainTexcoordScale;
				break;
			
			case 'IMPT':
				
				renderable->SetShaderArray(compileData->shaderData, kShaderArrayRadius, kArrayRadius);
				renderable->SetShaderArray(compileData->shaderData, kShaderArrayTexture0, kArrayTexture0);
				snippetFlag[kVertexSnippetOutputImpostorTexcoords] = true;
				stateFlags |= kShaderStateCameraPosition4D;
				break;
			
			case 'IXBL':
				
				snippetFlag[kVertexSnippetOutputImpostorTransitionBlend] = true;
				stateFlags |= kShaderStateImpostorTransition;
				break;
			
			case 'GITX':
				
				snippetFlag[kVertexSnippetOutputGeometryImpostorTexcoords] = true;
				stateFlags |= kShaderStateGeometryTransition;
				break;
			
			case 'PTXC':
				
				snippetFlag[kVertexSnippetOutputPaintTexcoords] = true;
				stateFlags |= kShaderStatePaintTransform;
				break;
			
			case 'FIRE':
				
				if (shaderFlags & kShaderFireArrays)
				{
					renderable->SetShaderArray(compileData->shaderData, kShaderArrayRadius, kArrayRadius);
					renderable->SetShaderArray(compileData->shaderData, kShaderArrayTexture0, kArrayTexture0);
					renderable->SetShaderArray(compileData->shaderData, kShaderArrayTexture1, kArrayTexture1);
					snippetFlag[kVertexSnippetOutputFireArrayTexcoords] = true;
				}
				else
				{
					renderable->SetShaderArray(compileData->shaderData, kShaderArrayTexture0, kArrayTexture0);
					snippetFlag[kVertexSnippetOutputFireTexcoords] = true;
				}
				
				break;
			
			case 'WARP':
				
				snippetFlag[kVertexSnippetCalculateObjectViewDirection] = true;
				snippetFlag[kVertexSnippetCalculateCameraDistance] = true;
				snippetFlag[kVertexSnippetOutputCameraWarpFunction] = true;
				stateFlags |= kShaderStateCameraPosition | kShaderStateCameraDirections;
				normalFlag = true;
				break;
			
			case 'RGHT':
				
				snippetFlag[kVertexSnippetCalculateObjectViewDirection] = true;
				snippetFlag[kVertexSnippetCalculateCameraDistance] = true;
				snippetFlag[kVertexSnippetOutputCameraBumpWarpFunction] = true;
				stateFlags |= kShaderStateCameraPosition | kShaderStateCameraDirections;
				bitangentFlag = true;
				break;
			
			case 'DDEP':
				
				snippetFlag[kVertexSnippetOutputDistortionDepth] = true;
				break;
			
			case 'ATTN':
				
				if (compileData->shaderType != kShaderSpotLight)
				{
					snippetFlag[kVertexSnippetCalculateObjectPointLightDirection] = true;
					snippetFlag[kVertexSnippetOutputPointLightAttenuation] = true;
				}
				else
				{
					snippetFlag[kVertexSnippetOutputSpotLightAttenuation] = true;
				}
				
				break;
			
			case 'SHAD':
				
				snippetFlag[kVertexSnippetOutputDepthProjectTexcoord] = true;
				break;
			
			case 'LAND':
				
				snippetFlag[kVertexSnippetOutputLandscapeProjectTexcoord] = true;
				break;
			
			case 'PROJ':
				
				if (compileData->shaderType == kShaderCubeLight) snippetFlag[kVertexSnippetOutputCubeProjectTexcoord] = true;
				else snippetFlag[kVertexSnippetOutputSpotProjectTexcoord] = true;
				break;
			
			case 'AMGD':
				
				snippetFlag[kVertexSnippetOutputAmbientGradientDistance] = true;
				break;
			
			case 'AMBT':
				
				snippetFlag[kVertexSnippetOutputAmbientSpaceVector] = true;
				normalFlag = true;
				break;
			
			case 'FDTP':
				
				if (!(shaderFlags & kShaderVertexInfinite))
				{
					if (compileData->shaderVariant == kShaderVariantConstantFog) snippetFlag[kVertexSnippetOutputFiniteConstantFogFactors] = true;
					else snippetFlag[kVertexSnippetOutputFiniteLinearFogFactors] = true;
				}
				else
				{
					if (compileData->shaderVariant == kShaderVariantConstantFog) snippetFlag[kVertexSnippetOutputInfiniteConstantFogFactors] = true;
					else snippetFlag[kVertexSnippetOutputInfiniteLinearFogFactors] = true;
				}
				
				break;
			
			case 'VELA':
				
				if (renderable->AttributeArrayEnabled(kArrayPrevious))
				{
					renderable->SetShaderArray(compileData->shaderData, kShaderArrayPrevious, kArrayPrevious);
					snippetFlag[kVertexSnippetDeformMotionBlurTransform] = true;
				}
				else if (renderable->AttributeArrayEnabled(kArrayVelocity))
				{
					renderable->SetShaderArray(compileData->shaderData, kShaderArrayVelocity, kArrayVelocity);
					snippetFlag[kVertexSnippetVelocityMotionBlurTransform] = true;
				}
				else
				{
					if (!(shaderFlags & kShaderVertexInfinite)) snippetFlag[kVertexSnippetMotionBlurTransform] = true;
					else snippetFlag[kVertexSnippetInfiniteMotionBlurTransform] = true;
				}
				
				break;
		}
	}
	
	if (compileData->shaderVariant != kShaderVariantNormal)
	{
		snippetFlag[kVertexSnippetOutputTangentViewDirection] = false;
		
		if (allocData->vdirCount > 1)
		{
			snippetFlag[kVertexSnippetOutputTangentViewFogDirection] = true;
			bitangentFlag = true;
		}
		else
		{
			snippetFlag[kVertexSnippetOutputAlternateViewFogDirection] = true;
		}
	}
	
	tangentFlag |= bitangentFlag;
	normalFlag |= tangentFlag;
	
	if (normalFlag)
	{
		if (shaderFlags & kShaderVertexPostboard)
		{
			assembly.AddSnippet(&VertexProgram::generateImpostorFrame);
			stateFlags |= kShaderStateCameraPosition;
		}
		else
		{
			renderable->SetShaderArray(compileData->shaderData, kShaderArrayNormal, kArrayNormal);
			
			if (tangentFlag)
			{
				if (shaderFlags & kShaderGenerateTangent)
				{
					if (shaderFlags & kShaderNormalizeBasisVectors) assembly.AddSnippet(&VertexProgram::normalizeNormal);
					
					assembly.AddSnippet(&VertexProgram::generateTangent);
					assembly.AddSnippet(&VertexProgram::calculateBitangent);
				}
				else
				{
					renderable->SetShaderArray(compileData->shaderData, kShaderArrayTangent, kArrayTangent);
					
					if (renderable->GetComponentCount(kArrayTangent) == 3)
					{
						if (shaderFlags & kShaderNormalizeBasisVectors)
						{
							assembly.AddSnippet(&VertexProgram::normalizeNormal);
							assembly.AddSnippet(&VertexProgram::normalizeTangent);
						}
						
						assembly.AddSnippet(&VertexProgram::calculateBitangent);
					}
					else
					{
						if (shaderFlags & kShaderNormalizeBasisVectors)
						{
							assembly.AddSnippet(&VertexProgram::normalizeNormal);
							assembly.AddSnippet(&VertexProgram::orthonormalizeTangent);
						}
						
						assembly.AddSnippet(&VertexProgram::calculateBitangent);
						assembly.AddSnippet(&VertexProgram::adjustBitangent);
					}
				}
			}
			else
			{
				if (shaderFlags & kShaderNormalizeBasisVectors) assembly.AddSnippet(&VertexProgram::normalizeNormal);
			}
		}
	}
	
	stateFlags |= renderable->BuildVertexTransform(compileData->shaderData, &assembly);
	
	for (machine a = 0; a < kVertexSnippetCount; a++)
	{
		if (snippetFlag[a]) assembly.AddSnippet(&VertexProgram::vertexSnippet[a]);
	}
	
	if (compileData->shaderSourceFlags & kShaderSourcePrimaryColor)
	{
		renderable->SetShaderArray(compileData->shaderData, kShaderArrayColor0, kArrayColor0);
		assembly.AddSnippet(&VertexProgram::outputPrimaryColor);
		
		if (compileData->shaderSourceFlags & kShaderSourceSecondaryColor)
		{
			renderable->SetShaderArray(compileData->shaderData, kShaderArrayColor1, kArrayColor1);
			assembly.AddSnippet(&VertexProgram::outputSecondaryColor);
		}
	}
	
	if (renderable->GetRenderType() == kRenderPoints)
	{
		renderable->SetShaderArray(compileData->shaderData, kShaderArrayRadius, kArrayRadius);
		
		if (!(shaderFlags & kShaderVertexInfinite)) assembly.AddSnippet(&VertexProgram::outputPointSize);
		else assembly.AddSnippet(&VertexProgram::outputInfinitePointSize);
	}
	
	int32 snippetCount = signatureStorage[0];
	unsigned_int32 *signature = &signatureStorage[snippetCount + 1];
	
	int32 interpolantCount = allocData->interpolantCount;
	signatureStorage[0] = snippetCount + interpolantCount * 2;
	
	for (machine a = 0; a < interpolantCount; a++)
	{
		const InterpolantData *data = &allocData->interpolantData[a];
		signature[0] = data->interpolantType;
		signature[1] = data->texcoordIndex | (data->swizzleData.component[0] << 8);
		signature += 2;
	}
	
	VertexProgram *program = VertexProgram::Get(signatureStorage);
	if (!program)
	{
		#if C4OPENGL
		
			static const char prolog[] =
			{
				"!!ARBvp1.0\n"
				"TEMP temp;\n"
			};
			
			static const char epilog[] =
			{
				"END"
			};
			
			const char *positionText = "vertex.attrib[0]";
			const char *normalText = "vertex.attrib[2]";
			const char *tangentText = "vertex.attrib[6]";
		
		#else
		
			static const char prolog[] =
			{
				"struct resultStruct\n"
				"{\n"
				"float4 position : HPOS;\n"
				"float4 color0 : COL0;\n"
				"float4 color1 : COL1;\n"
				"float pointsize : PSIZ;\n"
				"float4 texcoord[8] : TEX0;\n"
				"};\n"
				
				"resultStruct main(float4 attrib[16] : ATTR0, uniform float4 param[" VERTEX_PARAM_COUNT "] : C0)\n"
				"{\n"
				"resultStruct result;\n"
				"float4 temp;\n"
			};
			
			static const char epilog[] =
			{
				"return result;\n"
				"}\n"
			};
			
			const char *positionText = "attrib[0]";
			const char *normalText = "attrib[2]";
			const char *tangentText = "attrib[6]";
		
		#endif
		
		int32 len = Text::CopyText(prolog, sourceStorage);
		char *string = sourceStorage + len;
		
		for (machine a = 0; a < snippetCount; a++)
		{
			#if C4OPENGL
			
				const char *code = assembly.vertexSnippet[a]->programCode;
			
			#else
			
				const char *code = assembly.vertexSnippet[a]->shaderCode;
			
			#endif
			
			for (;;)
			{
				int32 c = *code++;
				if (c == 0) break;
				
				if (c == '%')
				{
					if ((code[0] == 'O') && (code[1] == 'P') && (code[2] == 'O') && (code[3] == 'S'))
					{
						string += Text::CopyText(positionText, string);
						code += 4;
					}
					else if ((code[0] == 'N') && (code[1] == 'R') && (code[2] == 'M') && (code[3] == 'L'))
					{
						string += Text::CopyText(normalText, string);
						code += 4;
					}
					else if ((code[0] == 'T') && (code[1] == 'A') && (code[2] == 'N') && (code[3] == 'G'))
					{
						string += Text::CopyText(tangentText, string);
						code += 4;
					}
				}
				else if (c == '$')
				{
					Type type = (code[0] << 24) | (code[1] << 16) | (code[2] << 8) | code[3];
					code += 4;
					
					c = code[0];
					if (c != ':')
					{
						string += GenerateVertexOutputName(type, allocData, -(c == '.'), string);
					}
					else
					{
						string += GenerateVertexOutputName(type, allocData, code[1], string);
						code += 2;
					}
				}
				else
				{
					*string++ = (char) c;
				}
			}
			
			unsigned_int32 snippetFlags = assembly.vertexSnippet[a]->flags;
			if (snippetFlags & kVertexSnippetPositionFlag) positionText = "opos";
			if (snippetFlags & kVertexSnippetNormalFlag) normalText = "nrml";
			if (snippetFlags & kVertexSnippetTangentFlag) tangentText = "tang";
		}
		
		string += Text::CopyText(epilog, string);
		
		unsigned_int32 size = string - sourceStorage;
		Assert(size < kMaxShaderSourceSize, "Program string overflow");
		
		program = VertexProgram::New(sourceStorage, size, signatureStorage);
	}
	
	*vertexProgram = program;
	return (stateFlags);
}

void ShaderAttribute::BuildStateFunctionList(const ShaderCompileData *compileData, unsigned_int32 stateFlags)
{
	ShaderData *shaderData = compileData->shaderData;
	
	if (compileData->renderable->GetTransformable())
	{
		if (stateFlags & kShaderStateCameraPosition)
		{
			if (stateFlags & kShaderStateCameraDirections) shaderData->AddStateFunction(&Renderable::StateFunc_TransformCameraPositionAndDirections);
			else shaderData->AddStateFunction(&Renderable::StateFunc_TransformCameraPosition);
		}
		else if (stateFlags & kShaderStateCameraDirections)
		{
			shaderData->AddStateFunction(&Renderable::StateFunc_TransformCameraDirections);
		}
		
		if (stateFlags & kShaderStateCameraPosition4D) shaderData->AddStateFunction(&Renderable::StateFunc_TransformCameraPosition4D);
		if (stateFlags & kShaderStateCameraTransform) shaderData->AddStateFunction(&Renderable::StateFunc_TransformCameraMatrix);
		if (stateFlags & kShaderStateWorldTransform) shaderData->AddStateFunction(&Renderable::StateFunc_TransformWorldMatrix);
		if (stateFlags & kShaderStatePaintTransform) shaderData->AddStateFunction(&Renderable::StateFunc_TransformPaintSpace);
		if (stateFlags & kShaderStateGeometryTransition) shaderData->AddStateFunction(&Renderable::StateFunc_TransformGeometryTransition);
		
		switch (compileData->shaderType)
		{
			case kShaderInfiniteLight:
				
				shaderData->AddStateFunction(&Renderable::StateFunc_ConfigureTransformInfiniteLight);
				break;
			
			case kShaderDepthLight:
				
				shaderData->AddStateFunction(&Renderable::StateFunc_ConfigureTransformDepthLight);
				break;
			
			case kShaderLandscapeLight:
				
				shaderData->AddStateFunction(&Renderable::StateFunc_ConfigureTransformLandscapeLight);
				break;
			
			case kShaderPointLight:
				
				shaderData->AddStateFunction(&Renderable::StateFunc_ConfigureTransformPointLight);
				break;
			
			case kShaderCubeLight:
				
				shaderData->AddStateFunction(&Renderable::StateFunc_ConfigureTransformCubeLight);
				break;
			
			case kShaderSpotLight:
				
				shaderData->AddStateFunction(&Renderable::StateFunc_ConfigureTransformSpotLight);
				break;
		}
	}
	else
	{
		if (stateFlags & kShaderStateCameraPosition)
		{
			if (stateFlags & kShaderStateCameraDirections) shaderData->AddStateFunction(&Renderable::StateFunc_CopyCameraPositionAndDirections);
			else shaderData->AddStateFunction(&Renderable::StateFunc_CopyCameraPosition);
		}
		else if (stateFlags & kShaderStateCameraDirections)
		{
			shaderData->AddStateFunction(&Renderable::StateFunc_CopyCameraDirections);
		}
		
		if (stateFlags & kShaderStateCameraPosition4D) shaderData->AddStateFunction(&Renderable::StateFunc_CopyCameraPosition4D);
		if (stateFlags & kShaderStateCameraTransform) shaderData->AddStateFunction(&Renderable::StateFunc_CopyCameraMatrix);
		if (stateFlags & kShaderStateWorldTransform) shaderData->AddStateFunction(&Renderable::StateFunc_CopyWorldMatrix);
		if (stateFlags & kShaderStatePaintTransform) shaderData->AddStateFunction(&Renderable::StateFunc_CopyPaintSpace);
		if (stateFlags & kShaderStateGeometryTransition) shaderData->AddStateFunction(&Renderable::StateFunc_CopyGeometryTransition);
		
		switch (compileData->shaderType)
		{
			case kShaderInfiniteLight:
				
				shaderData->AddStateFunction(&Renderable::StateFunc_ConfigureInfiniteLight);
				break;
			
			case kShaderDepthLight:
				
				shaderData->AddStateFunction(&Renderable::StateFunc_ConfigureDepthLight);
				break;
			
			case kShaderLandscapeLight:
				
				if (stateFlags & kShaderStateImpostorRadius) shaderData->AddStateFunction(&Renderable::StateFunc_ConfigureLandscapeLightImpostor);
				else shaderData->AddStateFunction(&Renderable::StateFunc_ConfigureLandscapeLight);
				break;
			
			case kShaderPointLight:
				
				shaderData->AddStateFunction(&Renderable::StateFunc_ConfigurePointLight);
				break;
			
			case kShaderCubeLight:
				
				shaderData->AddStateFunction(&Renderable::StateFunc_ConfigureCubeLight);
				break;
			
			case kShaderSpotLight:
				
				shaderData->AddStateFunction(&Renderable::StateFunc_ConfigureSpotLight);
				break;
		}
	}
	
	if (stateFlags & (kShaderStateTexcoordTransform0 | kShaderStateTexcoordTransform1 | kShaderStateTexcoordVelocity0 | kShaderStateTexcoordVelocity1))
	{
		const MaterialObject *materialObject = *compileData->renderSegment->GetMaterialObjectPointer();
		if (materialObject)
		{
			if (stateFlags & kShaderStateTexcoordGenerate) 
			{
				if (stateFlags & kShaderStateTexcoordTransform0)
				{
					if (stateFlags & kShaderStateTexcoordVelocity0) shaderData->AddStateFunction(&Renderable::StateFunc_GenerateTransformAnimateTexcoord0, materialObject);
					else shaderData->AddStateFunction(&Renderable::StateFunc_GenerateTransformTexcoord0, materialObject);
				}
				else if (stateFlags & kShaderStateTexcoordVelocity0)
				{
					if (stateFlags & kShaderStateTexcoordVelocity1) shaderData->AddStateFunction(&Renderable::StateFunc_GenerateAnimateDualTexcoords, materialObject);
					else shaderData->AddStateFunction(&Renderable::StateFunc_GenerateAnimateTexcoord0, materialObject);
				}
				
				if (stateFlags & kShaderStateTexcoordTransform1)
				{
					if (stateFlags & kShaderStateTexcoordVelocity1) shaderData->AddStateFunction(&Renderable::StateFunc_GenerateTransformAnimateTexcoord1, materialObject);
					else shaderData->AddStateFunction(&Renderable::StateFunc_GenerateTransformTexcoord1, materialObject);
				}
				else if (stateFlags & kShaderStateTexcoordVelocity1)
				{
					if (!(stateFlags & kShaderStateTexcoordVelocity0)) shaderData->AddStateFunction(&Renderable::StateFunc_GenerateAnimateTexcoord1, materialObject);
				}
			}
			else
			{
				if (stateFlags & kShaderStateTexcoordTransform0)
				{
					if (stateFlags & kShaderStateTexcoordVelocity0) shaderData->AddStateFunction(&Renderable::StateFunc_TransformAnimateTexcoord0, materialObject);
					else shaderData->AddStateFunction(&Renderable::StateFunc_TransformTexcoord0, materialObject);
				}
				else if (stateFlags & kShaderStateTexcoordVelocity0)
				{
					shaderData->AddStateFunction(&Renderable::StateFunc_AnimateTexcoord0, materialObject);
				}
				
				if (stateFlags & kShaderStateTexcoordTransform1)
				{
					if (stateFlags & kShaderStateTexcoordVelocity1) shaderData->AddStateFunction(&Renderable::StateFunc_TransformAnimateTexcoord1, materialObject);
					else shaderData->AddStateFunction(&Renderable::StateFunc_TransformTexcoord1, materialObject);
				}
				else if (stateFlags & kShaderStateTexcoordVelocity1)
				{
					shaderData->AddStateFunction(&Renderable::StateFunc_AnimateTexcoord1, materialObject);
				}
			}
		}
	}
	else
	{
		if (stateFlags & kShaderStateTexcoordGenerate) shaderData->AddStateFunction(&Renderable::StateFunc_GenerateTexcoord);
	}
	
	if (stateFlags & kShaderStateTerrainTexcoordScale)
	{
		const MaterialObject *materialObject = *compileData->renderSegment->GetMaterialObjectPointer();
		if (materialObject) shaderData->AddStateFunction(&Renderable::StateFunc_ScaleTerrainTexcoord, materialObject);
	}
	
	if (stateFlags & kShaderStateVertexScaleOffset) shaderData->AddStateFunction(&Renderable::StateFunc_CopyVertexScaleOffset);
	if (stateFlags & kShaderStateTerrainBorder) shaderData->AddStateFunction(&Renderable::StateFunc_CopyTerrainParameters);
	if (stateFlags & kShaderStateImpostorTransition) shaderData->AddStateFunction(&Renderable::StateFunc_CopyImpostorTransition);
}

#if C4OPENGL

	void ShaderAttribute::BindShaderUniforms(ShaderType type, FragmentProgram *fragmentShader)
	{
		static const char *const textureName[kMaxShaderTextureCount] =
		{
			"texture0", "texture1", "texture2", "texture3", "texture4",
			"texture5", "texture6", "texture7", "texture8", "texture9"
		};
		
		Render::BindFragmentShader(fragmentShader);
		
		switch (type)
		{
			case kShaderNone:
				
				Render::SetFragmentShaderTextureUnit(fragmentShader, "colorTexture", 0);
				Render::SetFragmentShaderTextureUnit(fragmentShader, "velocityTexture", 1);
				Render::SetFragmentShaderTextureUnit(fragmentShader, "distortionTexture", 2);
				Render::SetFragmentShaderTextureUnit(fragmentShader, "glowTexture", 3);
				break;
			
			case kShaderAmbientSpace:
				
				Render::SetFragmentShaderTextureUnit(fragmentShader, "ambientTexture1", kTextureUnitAmbientSpace1);
				Render::SetFragmentShaderTextureUnit(fragmentShader, "ambientTexture2", kTextureUnitAmbientSpace2);
				break;
			
			case kShaderDepthLight:
			case kShaderLandscapeLight:
				
				Render::SetFragmentShaderTextureUnit(fragmentShader, "shadowTexture", kTextureUnitLightProjection);
				break;
			
			case kShaderCubeLight:
				
				Render::SetFragmentShaderTextureUnit(fragmentShader, "projectionCUBE", kTextureUnitLightProjection);
				break;
			
			case kShaderSpotLight:
				
				Render::SetFragmentShaderTextureUnit(fragmentShader, "projection2D", kTextureUnitLightProjection);
				break;
		}
		
		for (machine a = 0; a < kMaxShaderTextureCount; a++)
		{
			Render::SetFragmentShaderTextureUnit(fragmentShader, textureName[a], a);
		}
		
		Render::UnbindFragmentShader();
	}

#endif

ShaderResult ShaderAttribute::CompileShader(ShaderType type, ShaderVariant variant, int32 level, const Renderable *renderable, const RenderSegment *renderSegment, ShaderData *shaderData, ShaderGraph *graph) const
{
	ShaderGraph				tempGraph;
	List<Process>			processList;
	List<Process>			readyList;
	ShaderCompileData		compileData;
	ShaderAllocationData	allocData;
	
	compileData.renderable = renderable;
	compileData.renderSegment = renderSegment;
	compileData.shaderData = shaderData;
	
	compileData.shaderType = type;
	compileData.shaderVariant = variant;
	compileData.detailLevel = level;
	
	#if C4OPENGL
	
		#if C4MACOS
		
			compileData.programFlag = ((type != kShaderDepthLight) && (type != kShaderLandscapeLight) && (type != kShaderShadowMap));
		
		#else
		
			compileData.programFlag = true;
		
		#endif
	
	#else
	
		compileData.programFlag = false;
	
	#endif
	
	compileData.shaderSourceFlags = 0;
	
	if (type <= kShaderLastAmbient)
	{
		if (!graph)
		{
			graph = &tempGraph;
			CloneShader(&shaderGraph[kShaderGraphAmbient], graph, true);
		}
		
		ShaderResult result = PrepareAmbientShader(&compileData, graph, &processList);
		if (result != kShaderOkay) return (result);
	}
	else if (type <= kShaderLastLight)
	{
		if (!graph)
		{
			graph = &tempGraph;
			CloneShader(&shaderGraph[kShaderGraphLight], graph, true);
		}
		
		ShaderResult result = PrepareLightShader(&compileData, graph, &processList);
		if (result != kShaderOkay) return (result);
	}
	else
	{
		if (!graph)
		{
			graph = &tempGraph;
			CloneShader(&shaderGraph[kShaderGraphAmbient], graph, true);
		}
		
		ShaderResult result = PreparePlainShader(type, &compileData, graph, &processList);
		if (result != kShaderOkay) return (result);
	}
	
	OptimizeTextureMaps(graph);
	EliminateDeadCode(graph, &processList);
	OrganizeDerivedInterpolants(&compileData, graph);
	
	CalculatePathLengths(graph, &processList, &readyList);
	int32 processCount = ScheduleShader(&compileData, &readyList, &processList, signatureStorage);
	
	if ((renderable) && (renderable->GetRenderType() == kRenderPoints))
	{
		unsigned_int32 size = signatureStorage[0];
		signatureStorage[0] = size + 1;
		signatureStorage[size] = 1;
	}
	
	Assert(signatureStorage[0] < kMaxShaderSignatureSize, "Signature overflow");
	
	ProcessData *processData = new ProcessData[processCount];
	
	ShaderResult result = AllocateShaderResources(&compileData, &allocData, processCount, processData, &processList);
	if (result == kShaderOkay)
	{
		FragmentProgram *program = FragmentProgram::Get(signatureStorage);
		if (!program)
		{
			int32 length = GenerateShaderProlog(&compileData, &allocData, sourceStorage);
			char *string = sourceStorage + length;
			
			GenerateShaderCode(&compileData, &allocData, processData, &processList, string, &length);
			
			string += length;
			string += GenerateShaderEpilog(&compileData, string);
			
			unsigned_int32 size = string - sourceStorage;
			Assert(size < kMaxShaderSourceSize, "Program string overflow");
			
			program = FragmentProgram::New(sourceStorage, size, compileData.programFlag, signatureStorage);
			
			#if C4OPENGL
			
				if (!compileData.programFlag) BindShaderUniforms(type, program);
			
			#endif
			
			#if C4PLAYSTATION3
			
				program->SetDeadFragmentTextureEnableMask(allocData.dependentTextureMask);
			
			#endif
		}
		
		shaderData->programData[variant].fragmentProgram = program;
		unsigned_int32 stateFlags = GenerateVertexProgram(&compileData, &allocData, &shaderData->programData[variant].vertexProgram);
		BuildStateFunctionList(&compileData, stateFlags);
	}
	
	delete[] processData;
	return (result);
}

ShaderResult ShaderAttribute::TestShader(ShaderType type, ShaderVariant variant, int32 level, const Renderable *renderable, const RenderSegment *renderSegment) const
{
	ShaderGraph				tempGraph;
	List<Process>			processList;
	List<Process>			readyList;
	ShaderCompileData		compileData;
	ShaderAllocationData	allocData;
	ShaderData				*shaderDataPtr;
	
	ShaderData shaderData(&shaderDataPtr, kBlendReplace, 0);
	
	compileData.renderable = renderable;
	compileData.renderSegment = renderSegment;
	compileData.shaderData = &shaderData;
	
	compileData.shaderType = type;
	compileData.shaderVariant = variant;
	compileData.detailLevel = level;
	
	#if C4OPENGL
	
		#if C4MACOS
		
			compileData.programFlag = ((type != kShaderDepthLight) && (type != kShaderLandscapeLight) && (type != kShaderShadowMap));
		
		#else
		
			compileData.programFlag = true;
		
		#endif
	
	#else
	
		compileData.programFlag = false;
	
	#endif
	
	compileData.shaderSourceFlags = 0;
	
	if (type <= kShaderLastAmbient)
	{
		CloneShader(&shaderGraph[kShaderGraphAmbient], &tempGraph, true);
		ShaderResult result = PrepareAmbientShader(&compileData, &tempGraph, &processList);
		if (result != kShaderOkay) return (result);
	}
	else if (type <= kShaderLastLight)
	{
		CloneShader(&shaderGraph[kShaderGraphLight], &tempGraph, true);
		ShaderResult result = PrepareLightShader(&compileData, &tempGraph, &processList);
		if (result != kShaderOkay) return (result);
	}
	else
	{
		CloneShader(&shaderGraph[kShaderGraphAmbient], &tempGraph, true);
		ShaderResult result = PreparePlainShader(type, &compileData, &tempGraph, &processList);
		if (result != kShaderOkay) return (result);
	}
	
	OptimizeTextureMaps(&tempGraph);
	EliminateDeadCode(&tempGraph, &processList);
	OrganizeDerivedInterpolants(&compileData, &tempGraph);
	
	CalculatePathLengths(&tempGraph, &processList, &readyList);
	int32 processCount = ScheduleShader(&compileData, &readyList, &processList, signatureStorage);
	
	if ((renderable) && (renderable->GetRenderType() == kRenderPoints))
	{
		unsigned_int32 size = signatureStorage[0];
		signatureStorage[size] = 1;
		signatureStorage[0] = size + 1;
	}
	
	ProcessData *processData = new ProcessData[processCount];
	ShaderResult result = AllocateShaderResources(&compileData, &allocData, processCount, processData, &processList);
	delete[] processData;
	return (result);
}

FragmentProgram *ShaderAttribute::CompilePostShader(const ShaderGraph *graph)
{
	List<Process>			processList;
	List<Process>			readyList;
	ShaderCompileData		compileData;
	ShaderAllocationData	allocData;
	
	compileData.renderable = nullptr;
	compileData.shaderData = nullptr;
	
	compileData.shaderType = kShaderNone;
	compileData.shaderVariant = kShaderVariantNormal;
	
	#if C4OPENGL
	
		compileData.programFlag = true;
	
	#else
	
		compileData.programFlag = false;
	
	#endif
	
	compileData.shaderSourceFlags = 0;
	
	Process *process = graph->GetFirstElement();
	while (process)
	{
		if (!process->GetFirstOutgoingEdge()) processList.Append(process);
		process = process->GetNextElement();
	}
	
	CalculatePathLengths(graph, &processList, &readyList);
	int32 processCount = ScheduleShader(&compileData, &readyList, &processList, signatureStorage);
	
	ProcessData *processData = new ProcessData[processCount];
	
	FragmentProgram *program = nullptr;
	if (AllocateShaderResources(&compileData, &allocData, processCount, processData, &processList) == kShaderOkay)
	{
		int32 length = GenerateShaderProlog(&compileData, &allocData, sourceStorage);
		char *string = sourceStorage + length;
		
		GenerateShaderCode(&compileData, &allocData, processData, &processList, string, &length);
		
		string += length;
		length = GenerateShaderEpilog(&compileData, string);
		string[length] = 0;
		
		unsigned_int32 size = string + length - sourceStorage;
		program = FragmentProgram::New(sourceStorage, size, compileData.programFlag, signatureStorage);
		
		#if C4OPENGL
		
			if (!compileData.programFlag) BindShaderUniforms(kShaderNone, program);
		
		#endif
	}
	
	delete[] processData;
	processList.RemoveAll();
	return (program);
}

Process *ShaderAttribute::BuildTextureCombiner(const MaterialObject *materialObject, ShaderGraph *graph, Process **textureMap1, Process **textureMap2, Process **textureCombiner, Process **vertexColor)
{
	if (*textureMap2)
	{
		Process		*combiner;
		
		TextureBlendMode mode = (materialObject) ? materialObject->GetTextureBlendMode() : kTextureBlendMultiply;
		switch (mode)
		{
			case kTextureBlendAdd:
				
				combiner = new AddProcess;
				new Route(*textureMap1, combiner, 0);
				new Route(*textureMap2, combiner, 1);
				break;
			
			case kTextureBlendAverage:
				
				combiner = new AverageProcess;
				new Route(*textureMap1, combiner, 0);
				new Route(*textureMap2, combiner, 1);
				break;
			
			case kTextureBlendMultiply:
				
				combiner = new MultiplyProcess;
				new Route(*textureMap1, combiner, 0);
				new Route(*textureMap2, combiner, 1);
				break;
			
			case kTextureBlendVertexAlpha:
				
				*vertexColor = new VertexColorProcess;
				graph->AddElement(*vertexColor);
				
				combiner = new LerpProcess;
				new Route(*textureMap1, combiner, 0);
				new Route(*textureMap2, combiner, 1);
				(new Route(*vertexColor, combiner, 2))->SetRouteSwizzle('aaaa');
				break;
			
			case kTextureBlendPrimaryAlpha:
				
				combiner = new LerpProcess;
				new Route(*textureMap1, combiner, 0);
				new Route(*textureMap2, combiner, 1);
				(new Route(*textureMap1, combiner, 2))->SetRouteSwizzle('aaaa');
				break;
			
			case kTextureBlendSecondaryAlpha:
				
				combiner = new LerpProcess;
				new Route(*textureMap1, combiner, 0);
				new Route(*textureMap2, combiner, 1);
				(new Route(*textureMap2, combiner, 2))->SetRouteSwizzle('aaaa');
				break;
			
			case kTextureBlendPrimaryInverseAlpha:
				
				combiner = new LerpProcess;
				new Route(*textureMap2, combiner, 0);
				new Route(*textureMap1, combiner, 1);
				(new Route(*textureMap1, combiner, 2))->SetRouteSwizzle('aaaa');
				break;
			
			case kTextureBlendSecondaryInverseAlpha:
				
				combiner = new LerpProcess;
				new Route(*textureMap2, combiner, 0);
				new Route(*textureMap1, combiner, 1);
				(new Route(*textureMap2, combiner, 2))->SetRouteSwizzle('aaaa');
				break;
		}
		
		*textureCombiner = combiner;
		graph->AddElement(combiner);
		return (combiner);
	}
	
	return (*textureMap1);
}

void ShaderAttribute::BuildAmbientShaderGraph(const Renderable *renderable, const RenderSegment *renderSegment, const MaterialObject *materialObject, const List<Attribute> *attributeList, ShaderGraph *graph, Process **process)
{
	const Attribute				*firstAttribute[2];
	int32						textureTexcoordIndex[2];
	int32						normalTexcoordIndex[2];
	int32						emissionTexcoordIndex;
	int32						glossTexcoordIndex;
	int32						opacityTexcoordIndex;
	const ReflectionAttribute	*reflectionAttribute;
	const RefractionAttribute	*refractionAttribute;
	Process						*finalDiffuseAlphaProduct;
	
	unsigned_int32 materialFlags = (materialObject) ? materialObject->GetMaterialFlags() : 0;
	if (renderSegment) materialFlags |= renderSegment->GetMaterialState();
	
	firstAttribute[0] = (materialObject) ? materialObject->GetFirstAttribute() : nullptr;
	firstAttribute[1] = (attributeList) ? attributeList->First() : nullptr;
	
	for (machine a = 0; a < kAmbientGraphProcessCount; a++) process[a] = nullptr;
	
	TextureSemantic textureAlphaSemantic = kTextureSemanticNone;
	const EnvironmentMapAttribute *environmentMapAttribute = nullptr;
	
	for (machine a = 0; a < 2; a++)
	{
		const Attribute *attribute = firstAttribute[a];
		while (attribute)
		{
			const Attribute *next = attribute->Next();
			for (;;)
			{
				AttributeType type = attribute->GetAttributeType();
				switch (type)
				{
					case kAttributeReference:
					{
						attribute = static_cast<const ReferenceAttribute *>(attribute)->GetReference();
						if (attribute) continue;
						break;
					}
					
					case kAttributeDiffuse:
					{
						if (!process[kAmbientGraphDiffuseColor])
						{
							process[kAmbientGraphDiffuseColor] = new ColorProcess;
							graph->AddElement(process[kAmbientGraphDiffuseColor]);
						}
						
						ColorProcess *colorProcess = static_cast<ColorProcess *>(process[kAmbientGraphDiffuseColor]);
						const DiffuseAttribute *diffuseAttribute = static_cast<const DiffuseAttribute *>(attribute);
						const ColorRGBA& diffuseColor = diffuseAttribute->GetDiffuseColor();
						
						colorProcess->SetColorValue(diffuseColor);
						if (diffuseAttribute->GetAttributeFlags() & kAttributeMutable)
						{
							colorProcess->SetParameterSlot(0);
							colorProcess->SetParameterData(&diffuseColor.red);
						}
						
						break;
					}
					
					case kAttributeEmission:
					{
						if (!process[kAmbientGraphEmissionColor])
						{
							process[kAmbientGraphEmissionColor] = new ColorProcess;
							graph->AddElement(process[kAmbientGraphEmissionColor]);
						}
						
						ColorProcess *colorProcess = static_cast<ColorProcess *>(process[kAmbientGraphEmissionColor]);
						const EmissionAttribute *emissionAttribute = static_cast<const EmissionAttribute *>(attribute);
						const ColorRGBA& emissionColor = emissionAttribute->GetEmissionColor();
						
						colorProcess->SetColorValue(emissionColor);
						if (emissionAttribute->GetAttributeFlags() & kAttributeMutable)
						{
							colorProcess->SetParameterSlot(1);
							colorProcess->SetParameterData(&emissionColor.red);
						}
						
						break;
					}
					
					case kAttributeReflection:
					{
						if (!process[kAmbientGraphReflectionColor])
						{
							process[kAmbientGraphReflectionColor] = new ColorProcess;
							graph->AddElement(process[kAmbientGraphReflectionColor]);
						}
						
						ColorProcess *colorProcess = static_cast<ColorProcess *>(process[kAmbientGraphReflectionColor]);
						reflectionAttribute = static_cast<const ReflectionAttribute *>(attribute);
						const ColorRGBA& reflectionColor = reflectionAttribute->GetReflectionColor();
						
						colorProcess->SetColorValue(reflectionColor);
						if (reflectionAttribute->GetAttributeFlags() & kAttributeMutable)
						{
							colorProcess->SetParameterSlot(2);
							colorProcess->SetParameterData(&reflectionColor.red);
						}
						
						break;
					}
					
					case kAttributeRefraction:
					{
						if (!process[kAmbientGraphRefractionColor])
						{
							process[kAmbientGraphRefractionColor] = new ColorProcess;
							graph->AddElement(process[kAmbientGraphRefractionColor]);
						}
						
						ColorProcess *colorProcess = static_cast<ColorProcess *>(process[kAmbientGraphRefractionColor]);
						refractionAttribute = static_cast<const RefractionAttribute *>(attribute);
						const ColorRGBA& refractionColor = refractionAttribute->GetRefractionColor();
						
						colorProcess->SetColorValue(refractionColor);
						if (refractionAttribute->GetAttributeFlags() & kAttributeMutable)
						{
							colorProcess->SetParameterSlot(3);
							colorProcess->SetParameterData(&refractionColor.red);
						}
						
						break;
					}
					
					case kAttributeEnvironment:
					{
						if (!process[kAmbientGraphEnvironmentColor])
						{
							process[kAmbientGraphEnvironmentColor] = new ColorProcess;
							graph->AddElement(process[kAmbientGraphEnvironmentColor]);
						}
						
						ColorProcess *colorProcess = static_cast<ColorProcess *>(process[kAmbientGraphEnvironmentColor]);
						const EnvironmentAttribute *environmentAttribute = static_cast<const EnvironmentAttribute *>(attribute);
						const ColorRGBA& environmentColor = environmentAttribute->GetEnvironmentColor();
						
						colorProcess->SetColorValue(environmentColor);
						if (environmentAttribute->GetAttributeFlags() & kAttributeMutable)
						{
							colorProcess->SetParameterSlot(4);
							colorProcess->SetParameterData(&environmentColor.red);
						}
						
						break;
					}
					
					case kAttributeTextureMap:
					case kAttributeNormalMap:
					case kAttributeEmissionMap:
					case kAttributeGlossMap:
					case kAttributeOpacityMap:
					{
						const MapAttribute *mapAttribute = static_cast<const MapAttribute *>(attribute);
						
						int32 texcoordIndex = mapAttribute->GetTexcoordIndex();
						Process *texcoordProcess = process[kAmbientGraphTexcoord1 + texcoordIndex];
						if (!texcoordProcess)
						{
							if (texcoordIndex == 0)
							{
								const Texture *texture = mapAttribute->GetTexture();
								if ((!texture) || (!renderable) || (TextureMapProcess::GetTexcoordSize(texture) == 2)) texcoordProcess = new Texcoord0Process;
								else texcoordProcess = new RawTexcoordProcess;
							}
							else
							{
								texcoordProcess = new Texcoord1Process;
							}
							
							process[kAmbientGraphTexcoord1 + texcoordIndex] = texcoordProcess;
							graph->AddElement(texcoordProcess);
						}
						
						if (type == kAttributeTextureMap)
						{
							if (!process[kAmbientGraphTextureMap1])
							{
								process[kAmbientGraphTextureMap1] = new TextureMapProcess;
								graph->AddElement(process[kAmbientGraphTextureMap1]);
								
								TextureMapProcess *textureMapProcess = static_cast<TextureMapProcess *>(process[kAmbientGraphTextureMap1]);
								
								const char *name = mapAttribute->GetTextureName();
								if (name[0] != 0) textureMapProcess->SetTexture(name);
								else textureMapProcess->SetTexture(mapAttribute->GetTexture());
								
								textureAlphaSemantic = textureMapProcess->GetTexture()->GetAlphaSemantic();
								textureTexcoordIndex[0] = texcoordIndex;
							}
							else
							{
								if (!process[kAmbientGraphTextureMap2])
								{
									process[kAmbientGraphTextureMap2] = new TextureMapProcess;
									graph->AddElement(process[kAmbientGraphTextureMap2]);
								}
								
								TextureMapProcess *textureMapProcess = static_cast<TextureMapProcess *>(process[kAmbientGraphTextureMap2]);
								
								const char *name = mapAttribute->GetTextureName();
								if (name[0] != 0) textureMapProcess->SetTexture(name);
								else textureMapProcess->SetTexture(mapAttribute->GetTexture());
								
								textureTexcoordIndex[1] = texcoordIndex;
							}
						}
						else if (type == kAttributeNormalMap)
						{
							if (!process[kAmbientGraphNormalMap1])
							{
								process[kAmbientGraphNormalMap1] = new NormalMapProcess;
								graph->AddElement(process[kAmbientGraphNormalMap1]);
								
								static_cast<TextureMapProcess *>(process[kAmbientGraphNormalMap1])->SetTexture(mapAttribute->GetTextureName());
								normalTexcoordIndex[0] = texcoordIndex;
								
								if (mapAttribute->GetTexture()->GetAlphaSemantic() == kTextureSemanticParallax)
								{
									process[kAmbientGraphParallax] = new ParallaxProcess;
									graph->AddElement(process[kAmbientGraphParallax]);
									
									static_cast<TextureMapProcess *>(process[kAmbientGraphParallax])->SetTexture(mapAttribute->GetTextureName());
								}
							}
							else
							{
								delete process[kAmbientGraphParallax];
								process[kAmbientGraphParallax] = nullptr;
								
								if (!process[kAmbientGraphNormalMap2])
								{
									process[kAmbientGraphNormalMap2] = new NormalMapProcess;
									graph->AddElement(process[kAmbientGraphNormalMap2]);
								}
								
								static_cast<TextureMapProcess *>(process[kAmbientGraphNormalMap2])->SetTexture(mapAttribute->GetTextureName());
								normalTexcoordIndex[1] = texcoordIndex;
							}
						}
						else if (type == kAttributeEmissionMap)
						{
							if (!process[kAmbientGraphEmissionMap])
							{
								process[kAmbientGraphEmissionMap] = new TextureMapProcess;
								graph->AddElement(process[kAmbientGraphEmissionMap]);
							}
							
							TextureMapProcess *textureMapProcess = static_cast<TextureMapProcess *>(process[kAmbientGraphEmissionMap]);
							
							const char *name = mapAttribute->GetTextureName();
							if (name[0] != 0) textureMapProcess->SetTexture(name);
							else textureMapProcess->SetTexture(mapAttribute->GetTexture());
							
							emissionTexcoordIndex = texcoordIndex;
						}
						else if (type == kAttributeGlossMap)
						{
							if (!process[kAmbientGraphGlossMap])
							{
								process[kAmbientGraphGlossMap] = new TextureMapProcess;
								graph->AddElement(process[kAmbientGraphGlossMap]);
							}
							
							static_cast<TextureMapProcess *>(process[kAmbientGraphGlossMap])->SetTexture(mapAttribute->GetTextureName());
							glossTexcoordIndex = texcoordIndex;
						}
						else if (type == kAttributeOpacityMap)
						{
							if (!process[kAmbientGraphOpacityMap])
							{
								process[kAmbientGraphOpacityMap] = new TextureMapProcess;
								graph->AddElement(process[kAmbientGraphOpacityMap]);
							}
							
							static_cast<TextureMapProcess *>(process[kAmbientGraphOpacityMap])->SetTexture(mapAttribute->GetTextureName());
							opacityTexcoordIndex = texcoordIndex;
						}
						
						break;
					}
					
					case kAttributeEnvironmentMap:
						
						environmentMapAttribute = static_cast<const EnvironmentMapAttribute *>(attribute);
						break;
				}
				
				break;
			}
			
			attribute = next;
		}
	}
	
	if (process[kAmbientGraphParallax])
	{
		int32 parallaxTexcoordIndex = normalTexcoordIndex[0];
		
		new Route(process[kAmbientGraphTexcoord1 + parallaxTexcoordIndex], process[kAmbientGraphParallax], 0);
		new Route(process[kAmbientGraphParallax], process[kAmbientGraphNormalMap1], 0);
		
		if (process[kAmbientGraphTextureMap1])
		{
			int32 texcoordIndex = textureTexcoordIndex[0];
			if (texcoordIndex == parallaxTexcoordIndex) new Route(process[kAmbientGraphParallax], process[kAmbientGraphTextureMap1], 0);
			else new Route(process[kAmbientGraphTexcoord1 + texcoordIndex], process[kAmbientGraphTextureMap1], 0);
			
			if (process[kAmbientGraphTextureMap2])
			{
				texcoordIndex = textureTexcoordIndex[1];
				if (texcoordIndex == parallaxTexcoordIndex) new Route(process[kAmbientGraphParallax], process[kAmbientGraphTextureMap2], 0);
				else new Route(process[kAmbientGraphTexcoord1 + texcoordIndex], process[kAmbientGraphTextureMap2], 0);
			}
		}
		
		if (process[kAmbientGraphEmissionMap])
		{
			int32 texcoordIndex = emissionTexcoordIndex;
			if (texcoordIndex == parallaxTexcoordIndex) new Route(process[kAmbientGraphParallax], process[kAmbientGraphEmissionMap], 0);
			else new Route(process[kAmbientGraphTexcoord1 + texcoordIndex], process[kAmbientGraphEmissionMap], 0);
		}
		
		if (process[kAmbientGraphGlossMap])
		{
			int32 texcoordIndex = glossTexcoordIndex;
			if (texcoordIndex == parallaxTexcoordIndex) new Route(process[kAmbientGraphParallax], process[kAmbientGraphGlossMap], 0);
			else new Route(process[kAmbientGraphTexcoord1 + texcoordIndex], process[kAmbientGraphGlossMap], 0);
		}
		
		if (process[kAmbientGraphOpacityMap])
		{
			int32 texcoordIndex = opacityTexcoordIndex;
			if (texcoordIndex == parallaxTexcoordIndex) new Route(process[kAmbientGraphParallax], process[kAmbientGraphOpacityMap], 0);
			else new Route(process[kAmbientGraphTexcoord1 + texcoordIndex], process[kAmbientGraphOpacityMap], 0);
		}
	}
	else
	{
		if (process[kAmbientGraphTextureMap1])
		{
			new Route(process[kAmbientGraphTexcoord1 + textureTexcoordIndex[0]], process[kAmbientGraphTextureMap1], 0);
			if (process[kAmbientGraphTextureMap2]) new Route(process[kAmbientGraphTexcoord1 + textureTexcoordIndex[1]], process[kAmbientGraphTextureMap2], 0);
		}
		
		if (process[kAmbientGraphNormalMap1])
		{
			new Route(process[kAmbientGraphTexcoord1 + normalTexcoordIndex[0]], process[kAmbientGraphNormalMap1], 0);
			if (process[kAmbientGraphNormalMap2]) new Route(process[kAmbientGraphTexcoord1 + normalTexcoordIndex[1]], process[kAmbientGraphNormalMap2], 0);
		}
		
		if (process[kAmbientGraphEmissionMap]) new Route(process[kAmbientGraphTexcoord1 + emissionTexcoordIndex], process[kAmbientGraphEmissionMap], 0);
		if (process[kAmbientGraphGlossMap]) new Route(process[kAmbientGraphTexcoord1 + glossTexcoordIndex], process[kAmbientGraphGlossMap], 0);
		if (process[kAmbientGraphOpacityMap]) new Route(process[kAmbientGraphTexcoord1 + opacityTexcoordIndex], process[kAmbientGraphOpacityMap], 0);
	}
	
	Process *finalTextureMap = BuildTextureCombiner(materialObject, graph, &process[kAmbientGraphTextureMap1], &process[kAmbientGraphTextureMap2], &process[kAmbientGraphTextureCombiner], &process[kAmbientGraphVertexColor]);
	
	Process *finalNormalMap = process[kAmbientGraphNormalMap1];
	if (process[kAmbientGraphNormalMap2])
	{
		process[kAmbientGraphNormalCombiner] = new CombineNormalsProcess;
		graph->AddElement(process[kAmbientGraphNormalCombiner]);
		finalNormalMap = process[kAmbientGraphNormalCombiner];
		
		new Route(process[kAmbientGraphNormalMap1], process[kAmbientGraphNormalCombiner], 0);
		new Route(process[kAmbientGraphNormalMap2], process[kAmbientGraphNormalCombiner], 1);
	}
	
	bool colorArrayEnabled = false;
	if (renderable)
	{
		colorArrayEnabled = renderable->AttributeArrayEnabled(kArrayColor0);
		if (colorArrayEnabled)
		{
			if (renderable->GetComponentCount(kArrayColor0) > 1)
			{
				if (!process[kAmbientGraphVertexColor])
				{
					process[kAmbientGraphVertexColor] = new VertexColorProcess;
					graph->AddElement(process[kAmbientGraphVertexColor]);
				}
			}
			else
			{
				colorArrayEnabled = false;
			}
		}
	}
	
	if ((materialFlags & kMaterialAlphaSemanticInhibit) && (textureAlphaSemantic != kTextureSemanticNone)) textureAlphaSemantic = kTextureSemanticTransparency;
	
	Process *finalDiffuseProduct = finalTextureMap;
	if (finalDiffuseProduct)
	{
		if (process[kAmbientGraphDiffuseColor])
		{
			process[kAmbientGraphDiffuseMultiply] = new MultiplyProcess;
			graph->AddElement(process[kAmbientGraphDiffuseMultiply]);
			finalDiffuseProduct = process[kAmbientGraphDiffuseMultiply];
			
			if (colorArrayEnabled)
			{
				process[kAmbientGraphColorMultiply] = new MultiplyProcess;
				graph->AddElement(process[kAmbientGraphColorMultiply]);
				
				new Route(process[kAmbientGraphDiffuseColor], process[kAmbientGraphColorMultiply], 0);
				new Route(process[kAmbientGraphVertexColor], process[kAmbientGraphColorMultiply], 1);
				
				new Route(finalTextureMap, process[kAmbientGraphDiffuseMultiply], 0);
				new Route(process[kAmbientGraphColorMultiply], process[kAmbientGraphDiffuseMultiply], 1);
			}
			else
			{
				new Route(finalTextureMap, process[kAmbientGraphDiffuseMultiply], 0);
				new Route(process[kAmbientGraphDiffuseColor], process[kAmbientGraphDiffuseMultiply], 1);
			}
		}
		else if (colorArrayEnabled)
		{
			process[kAmbientGraphDiffuseMultiply] = new MultiplyProcess;
			graph->AddElement(process[kAmbientGraphDiffuseMultiply]);
			finalDiffuseProduct = process[kAmbientGraphDiffuseMultiply];
			
			new Route(finalTextureMap, process[kAmbientGraphDiffuseMultiply], 0);
			new Route(process[kAmbientGraphVertexColor], process[kAmbientGraphDiffuseMultiply], 1);
		}
		
		finalDiffuseAlphaProduct = finalDiffuseProduct;
		
		if (textureAlphaSemantic == kTextureSemanticOcclusion)
		{
			process[kAmbientGraphOcclusionMultiply] = new MultiplyProcess;
			graph->AddElement(process[kAmbientGraphOcclusionMultiply]);
			
			new Route(finalDiffuseProduct, process[kAmbientGraphOcclusionMultiply], 0);
			(new Route(process[kAmbientGraphTextureMap1], process[kAmbientGraphOcclusionMultiply], 1))->SetRouteSwizzle('aaaa');
			
			finalDiffuseProduct = process[kAmbientGraphOcclusionMultiply];
		}
	}
	else
	{
		if (colorArrayEnabled)
		{
			if (process[kAmbientGraphDiffuseColor])
			{
				process[kAmbientGraphColorMultiply] = new MultiplyProcess;
				graph->AddElement(process[kAmbientGraphColorMultiply]);
				finalDiffuseProduct = process[kAmbientGraphColorMultiply];
				
				new Route(process[kAmbientGraphDiffuseColor], process[kAmbientGraphColorMultiply], 0);
				new Route(process[kAmbientGraphVertexColor], process[kAmbientGraphColorMultiply], 1);
			}
			else
			{
				finalDiffuseProduct = process[kAmbientGraphVertexColor];
			}
		}
		else
		{
			finalDiffuseProduct = process[kAmbientGraphDiffuseColor];
		}
		
		finalDiffuseAlphaProduct = finalDiffuseProduct;
	}
	
	unsigned_int32 emissionSwizzle = 'xyzw';
	Process *finalEmissionProduct = process[kAmbientGraphEmissionMap];
	if (finalEmissionProduct)
	{
		if (process[kAmbientGraphEmissionColor])
		{
			process[kAmbientGraphEmissionMultiply] = new MultiplyProcess;
			graph->AddElement(process[kAmbientGraphEmissionMultiply]);
			finalEmissionProduct = process[kAmbientGraphEmissionMultiply];
			
			new Route(process[kAmbientGraphEmissionMap], process[kAmbientGraphEmissionMultiply], 0);
			new Route(process[kAmbientGraphEmissionColor], process[kAmbientGraphEmissionMultiply], 1);
		}
	}
	else if (textureAlphaSemantic == kTextureSemanticEmission)
	{
		if (process[kAmbientGraphEmissionColor])
		{
			process[kAmbientGraphEmissionMultiply] = new MultiplyProcess;
			graph->AddElement(process[kAmbientGraphEmissionMultiply]);
			finalEmissionProduct = process[kAmbientGraphEmissionMultiply];
			
			(new Route(process[kAmbientGraphTextureMap1], process[kAmbientGraphEmissionMultiply], 0))->SetRouteSwizzle('aaaa');
			new Route(process[kAmbientGraphEmissionColor], process[kAmbientGraphEmissionMultiply], 1);
		}
		else
		{
			finalEmissionProduct = process[kAmbientGraphTextureMap1];
			emissionSwizzle = 'aaaa';
		}
	}
	else
	{
		finalEmissionProduct = process[kAmbientGraphEmissionColor];
	}
	
	Process *finalEnvironmentProduct = process[kAmbientGraphEnvironmentColor];
	if (finalEnvironmentProduct)
	{
		if ((process[kAmbientGraphGlossMap]) || (textureAlphaSemantic == kTextureSemanticGloss))
		{
			process[kAmbientGraphEnvironmentMultiply] = new MultiplyProcess;
			graph->AddElement(process[kAmbientGraphEnvironmentMultiply]);
			finalEnvironmentProduct = process[kAmbientGraphEnvironmentMultiply];
			
			if ((process[kAmbientGraphGlossMap])) new Route(process[kAmbientGraphGlossMap], process[kAmbientGraphEnvironmentMultiply], 0);
			else (new Route(process[kAmbientGraphTextureMap1], process[kAmbientGraphEnvironmentMultiply], 0))->SetRouteSwizzle('aaaa');
			new Route(process[kAmbientGraphEnvironmentColor], process[kAmbientGraphEnvironmentMultiply], 1);
		}
	}
	
	if (!finalDiffuseProduct)
	{
		process[kAmbientGraphDiffuseColor] = new ColorProcess;
		graph->AddElement(process[kAmbientGraphDiffuseColor]);
		finalDiffuseProduct = process[kAmbientGraphDiffuseColor];
		finalDiffuseAlphaProduct = finalDiffuseProduct;
	}
	
	Process *finalRefractionProduct = process[kAmbientGraphRefractionColor];
	if (finalRefractionProduct)
	{
		if ((process[kAmbientGraphOpacityMap]) || (textureAlphaSemantic == kTextureSemanticOpacity))
		{
			process[kAmbientGraphOpacityMultiply] = new MultiplyProcess;
			graph->AddElement(process[kAmbientGraphOpacityMultiply]);
			
			new Route(finalDiffuseProduct, process[kAmbientGraphOpacityMultiply], 0);
			if (process[kAmbientGraphOpacityMap]) new Route(process[kAmbientGraphOpacityMap], process[kAmbientGraphOpacityMultiply], 1);
			else (new Route(process[kAmbientGraphTextureMap1], process[kAmbientGraphOpacityMultiply], 1))->SetRouteSwizzle('aaaa');
			
			finalDiffuseProduct = process[kAmbientGraphOpacityMultiply];
			
			process[kAmbientGraphConstantUnity] = new ScalarProcess;
			graph->AddElement(process[kAmbientGraphConstantUnity]);
			
			process[kAmbientGraphOpacitySubtract] = new SubtractProcess;
			graph->AddElement(process[kAmbientGraphOpacitySubtract]);
			
			new Route(process[kAmbientGraphConstantUnity], process[kAmbientGraphOpacitySubtract], 0);
			if (process[kAmbientGraphOpacityMap]) new Route(process[kAmbientGraphOpacityMap], process[kAmbientGraphOpacitySubtract], 1);
			else (new Route(process[kAmbientGraphTextureMap1], process[kAmbientGraphOpacitySubtract], 1))->SetRouteSwizzle('aaaa');
			
			process[kAmbientGraphRefractionMultiply] = new MultiplyProcess;
			graph->AddElement(process[kAmbientGraphRefractionMultiply]);
			finalRefractionProduct = process[kAmbientGraphRefractionMultiply];
			
			new Route(process[kAmbientGraphOpacitySubtract], process[kAmbientGraphRefractionMultiply], 0);
			new Route(process[kAmbientGraphRefractionColor], process[kAmbientGraphRefractionMultiply], 1);
		}
	}
	
	process[kAmbientGraphAmbientOutput] = new AmbientOutputProcess;
	graph->AddElement(process[kAmbientGraphAmbientOutput]);
	
	process[kAmbientGraphAmbientAlphaOutput] = new AmbientAlphaOutputProcess;
	graph->AddElement(process[kAmbientGraphAmbientAlphaOutput]);
	
	new Route(finalDiffuseProduct, process[kAmbientGraphAmbientOutput], 0);
	if (finalNormalMap) new Route(finalNormalMap, process[kAmbientGraphAmbientOutput], 1);
	
	(new Route(finalDiffuseAlphaProduct, process[kAmbientGraphAmbientAlphaOutput], 0))->SetRouteSwizzle('aaaa');
	
	if (materialFlags & kMaterialAlphaTest)
	{
		process[kAmbientGraphAlphaTestOutput] = new AlphaTestOutputProcess;
		graph->AddElement(process[kAmbientGraphAlphaTestOutput]);
		
		(new Route(finalDiffuseAlphaProduct, process[kAmbientGraphAlphaTestOutput], 0))->SetRouteSwizzle('aaaa');
	}
	
	if (finalEmissionProduct)
	{
		process[kAmbientGraphEmissionOutput] = new EmissionOutputProcess;
		graph->AddElement(process[kAmbientGraphEmissionOutput]);
		
		(new Route(finalEmissionProduct, process[kAmbientGraphEmissionOutput], 0))->SetRouteSwizzle(emissionSwizzle);
		
		if (materialFlags & kMaterialEmissionGlow)
		{
			process[kAmbientGraphGlowOutput] = new GlowOutputProcess;
			graph->AddElement(process[kAmbientGraphGlowOutput]);
			
			(new Route(finalEmissionProduct, process[kAmbientGraphGlowOutput], 0))->SetRouteSwizzle('aaaa');
		}
	}
	else if ((textureAlphaSemantic == kTextureSemanticGlow) && (materialFlags & kMaterialEmissionGlow))
	{
		process[kAmbientGraphGlowOutput] = new GlowOutputProcess;
		graph->AddElement(process[kAmbientGraphGlowOutput]);
		
		(new Route(process[kAmbientGraphTextureMap1], process[kAmbientGraphGlowOutput], 0))->SetRouteSwizzle('aaaa');
	}
	
	if (process[kAmbientGraphReflectionColor])
	{
		process[kAmbientGraphReflectionOutput] = new ReflectionOutputProcess;
		graph->AddElement(process[kAmbientGraphReflectionOutput]);
		
		new Route(process[kAmbientGraphReflectionColor], process[kAmbientGraphReflectionOutput], 0);
		if (finalNormalMap) new Route(finalNormalMap, process[kAmbientGraphReflectionOutput], 1);
		
		ReflectionOutputProcess *reflectionProcess = static_cast<ReflectionOutputProcess *>(process[kAmbientGraphReflectionOutput]);
		const ReflectionAttribute::ReflectionParams *params = reflectionAttribute->GetReflectionParams();
		
		reflectionProcess->SetReflectionParams(params);
		if (renderable) reflectionProcess->SetReflectionData(params);
	}
	
	if (finalRefractionProduct)
	{
		process[kAmbientGraphRefractionOutput] = new RefractionOutputProcess;
		graph->AddElement(process[kAmbientGraphRefractionOutput]);
		
		new Route(finalRefractionProduct, process[kAmbientGraphRefractionOutput], 0);
		if (finalNormalMap) new Route(finalNormalMap, process[kAmbientGraphRefractionOutput], 1);
		
		RefractionOutputProcess *refractionProcess = static_cast<RefractionOutputProcess *>(process[kAmbientGraphRefractionOutput]);
		const RefractionAttribute::RefractionParams *params = refractionAttribute->GetRefractionParams();
		
		refractionProcess->SetRefractionParams(params);
		if (renderable) refractionProcess->SetRefractionData(params);
	}
	
	if (finalEnvironmentProduct)
	{
		process[kAmbientGraphEnvironmentOutput] = new EnvironmentOutputProcess;
		graph->AddElement(process[kAmbientGraphEnvironmentOutput]);
		
		if (environmentMapAttribute) static_cast<EnvironmentOutputProcess *>(process[kAmbientGraphEnvironmentOutput])->SetTexture(environmentMapAttribute->GetTextureName());
		
		new Route(finalEnvironmentProduct, process[kAmbientGraphEnvironmentOutput], 0);
		if (finalNormalMap) new Route(finalNormalMap, process[kAmbientGraphEnvironmentOutput], 1);
	}
}

void ShaderAttribute::BuildLightShaderGraph(const Renderable *renderable, const RenderSegment *renderSegment, const MaterialObject *materialObject, const List<Attribute> *attributeList, ShaderGraph *graph, Process **process)
{
	const Attribute		*firstAttribute[2];
	int32				textureTexcoordIndex[2];
	int32				normalTexcoordIndex[2];
	int32				glossTexcoordIndex;
	int32				horizonTexcoordIndex;
	
	unsigned_int32 materialFlags = (materialObject) ? materialObject->GetMaterialFlags() : 0;
	if (renderSegment) materialFlags |= renderSegment->GetMaterialState();
	
	firstAttribute[0] = (materialObject) ? materialObject->GetFirstAttribute() : nullptr;
	firstAttribute[1] = (attributeList) ? attributeList->First() : nullptr;
	
	for (machine a = 0; a < kLightGraphProcessCount; a++) process[a] = nullptr;
	
	TextureSemantic textureAlphaSemantic = kTextureSemanticNone;
	TextureSemantic normalAlphaSemantic = kTextureSemanticNone;
	
	for (machine a = 0; a < 2; a++)
	{
		const Attribute *attribute = firstAttribute[a];
		while (attribute)
		{
			const Attribute *next = attribute->Next();
			for (;;)
			{
				AttributeType type = attribute->GetAttributeType();
				switch (type)
				{
					case kAttributeReference:
					{
						attribute = static_cast<const ReferenceAttribute *>(attribute)->GetReference();
						if (attribute) continue;
						break;
					}
					
					case kAttributeDiffuse:
					{
						if (!process[kLightGraphDiffuseColor])
						{
							process[kLightGraphDiffuseColor] = new ColorProcess;
							graph->AddElement(process[kLightGraphDiffuseColor]);
						}
						
						ColorProcess *colorProcess = static_cast<ColorProcess *>(process[kLightGraphDiffuseColor]);
						const DiffuseAttribute *diffuseAttribute = static_cast<const DiffuseAttribute *>(attribute);
						const ColorRGBA& diffuseColor = diffuseAttribute->GetDiffuseColor();
						
						colorProcess->SetColorValue(diffuseColor);
						if (diffuseAttribute->GetAttributeFlags() & kAttributeMutable)
						{
							colorProcess->SetParameterSlot(0);
							colorProcess->SetParameterData(&diffuseColor.red);
						}
						
						break;
					}
					
					case kAttributeSpecular:
					{
						if (!process[kLightGraphMicrofacet])
						{
							if (!process[kLightGraphSpecularColor])
							{
								process[kLightGraphSpecularColor] = new ColorProcess;
								graph->AddElement(process[kLightGraphSpecularColor]);
								
								process[kLightGraphSpecularExponent] = new ScalarProcess;
								graph->AddElement(process[kLightGraphSpecularExponent]);
							}
							
							ColorProcess *colorProcess = static_cast<ColorProcess *>(process[kLightGraphSpecularColor]);
							ScalarProcess *exponentProcess = static_cast<ScalarProcess *>(process[kLightGraphSpecularExponent]);
							
							const SpecularAttribute *specularAttribute = static_cast<const SpecularAttribute *>(attribute);
							const ColorRGBA& specularColor = specularAttribute->GetSpecularColor();
							const float& specularExponent = specularAttribute->GetSpecularExponent();
							
							colorProcess->SetColorValue(specularColor);
							exponentProcess->SetScalarValue(specularExponent);
							if (specularAttribute->GetAttributeFlags() & kAttributeMutable)
							{
								colorProcess->SetParameterSlot(1);
								colorProcess->SetParameterData(&specularColor.red);
								
								exponentProcess->SetParameterSlot(2);
								exponentProcess->SetParameterData(&specularExponent);
							}
						}
						
						break;
					}
					
					case kAttributeMicrofacet:
					{
						delete process[kLightGraphSpecularColor];
						process[kLightGraphSpecularColor] = nullptr;
						
						delete process[kLightGraphSpecularExponent];
						process[kLightGraphSpecularExponent] = nullptr;
						
						const MicrofacetAttribute *microfacetAttribute = static_cast<const MicrofacetAttribute *>(attribute);
						
						if (!process[kLightGraphMicrofacet])
						{
							process[kLightGraphMicrofacet] = new MicrofacetProcess;
							graph->AddElement(process[kLightGraphMicrofacet]);
							
							delete process[kLightGraphMicrofacetReflectivity];
							process[kLightGraphMicrofacetReflectivity] = nullptr;
							
							float reflectivity = microfacetAttribute->GetMicrofacetReflectivity();
							if (reflectivity < 1.0F)
							{
								process[kLightGraphMicrofacetReflectivity] = new ScalarProcess;
								graph->AddElement(process[kLightGraphMicrofacetReflectivity]);
								
								static_cast<ScalarProcess *>(process[kLightGraphMicrofacetReflectivity])->SetScalarValue(reflectivity);
							}
						}
						
						MicrofacetProcess *microfacetProcess = static_cast<MicrofacetProcess *>(process[kLightGraphMicrofacet]);
						const MicrofacetAttribute::MicrofacetParams *params = microfacetAttribute->GetMicrofacetParams();
						
						microfacetProcess->SetMicrofacetParams(params);
						if (renderable) microfacetProcess->SetMicrofacetData(params);
						break;
					};
					
					case kAttributeTextureMap:
					case kAttributeNormalMap:
					case kAttributeGlossMap:
					case kAttributeHorizonMap:
					{
						const MapAttribute *mapAttribute = static_cast<const MapAttribute *>(attribute);
						
						int32 texcoordIndex = mapAttribute->GetTexcoordIndex();
						Process *texcoordProcess = process[kLightGraphTexcoord1 + texcoordIndex];
						if (!texcoordProcess)
						{
							if (texcoordIndex == 0) texcoordProcess = new Texcoord0Process;
							else texcoordProcess = new Texcoord1Process;
							process[kLightGraphTexcoord1 + texcoordIndex] = texcoordProcess;
							graph->AddElement(texcoordProcess);
						}
						
						if (type == kAttributeTextureMap)
						{
							if (!process[kLightGraphTextureMap1])
							{
								process[kLightGraphTextureMap1] = new TextureMapProcess;
								graph->AddElement(process[kLightGraphTextureMap1]);
								
								TextureMapProcess *textureMapProcess = static_cast<TextureMapProcess *>(process[kLightGraphTextureMap1]);
								
								const char *name = mapAttribute->GetTextureName();
								if (name[0] != 0) textureMapProcess->SetTexture(name);
								else textureMapProcess->SetTexture(mapAttribute->GetTexture());
								
								textureAlphaSemantic = textureMapProcess->GetTexture()->GetAlphaSemantic();
								textureTexcoordIndex[0] = texcoordIndex;
							}
							else
							{
								if (!process[kLightGraphTextureMap2])
								{
									process[kLightGraphTextureMap2] = new TextureMapProcess;
									graph->AddElement(process[kLightGraphTextureMap2]);
								}
								
								TextureMapProcess *textureMapProcess = static_cast<TextureMapProcess *>(process[kLightGraphTextureMap2]);
								
								const char *name = mapAttribute->GetTextureName();
								if (name[0] != 0) textureMapProcess->SetTexture(name);
								else textureMapProcess->SetTexture(mapAttribute->GetTexture());
								
								textureTexcoordIndex[1] = texcoordIndex;
							}
						}
						else if (type == kAttributeNormalMap)
						{
							if (!process[kLightGraphNormalMap1])
							{
								process[kLightGraphNormalMap1] = new NormalMapProcess;
								graph->AddElement(process[kLightGraphNormalMap1]);
								
								NormalMapProcess *normalMapProcess = static_cast<NormalMapProcess *>(process[kLightGraphNormalMap1]);
								normalMapProcess->SetTexture(mapAttribute->GetTextureName());
								
								normalAlphaSemantic = normalMapProcess->GetTexture()->GetAlphaSemantic();
								normalTexcoordIndex[0] = texcoordIndex;
								
								if (normalAlphaSemantic == kTextureSemanticParallax)
								{
									process[kLightGraphParallax] = new ParallaxProcess;
									graph->AddElement(process[kLightGraphParallax]);
									
									static_cast<TextureMapProcess *>(process[kLightGraphParallax])->SetTexture(mapAttribute->GetTextureName());
								}
							}
							else
							{
								delete process[kLightGraphParallax];
								process[kLightGraphParallax] = nullptr;
								
								if (!process[kLightGraphNormalMap2])
								{
									process[kLightGraphNormalMap2] = new NormalMapProcess;
									graph->AddElement(process[kLightGraphNormalMap2]);
								}
								
								static_cast<TextureMapProcess *>(process[kLightGraphNormalMap2])->SetTexture(mapAttribute->GetTextureName());
								normalTexcoordIndex[1] = texcoordIndex;
							}
						}
						else if (type == kAttributeGlossMap)
						{
							if (!process[kLightGraphGlossMap])
							{
								process[kLightGraphGlossMap] = new TextureMapProcess;
								graph->AddElement(process[kLightGraphGlossMap]);
							}
							
							static_cast<TextureMapProcess *>(process[kLightGraphGlossMap])->SetTexture(mapAttribute->GetTextureName());
							glossTexcoordIndex = texcoordIndex;
						}
						else
						{
							if (!process[kLightGraphHorizon])
							{
								process[kLightGraphHorizon] = new HorizonProcess;
								graph->AddElement(process[kLightGraphHorizon]);
								
								static_cast<TextureMapProcess *>(process[kLightGraphHorizon])->SetTexture(mapAttribute->GetTextureName());
								static_cast<HorizonProcess *>(process[kLightGraphHorizon])->SetHorizonFlags(static_cast<const HorizonMapAttribute *>(attribute)->GetHorizonFlags());
								horizonTexcoordIndex = texcoordIndex;
							}
							else
							{
								static_cast<HorizonProcess *>(process[kLightGraphHorizon])->SetSecondaryTexture(mapAttribute->GetTextureName());
							}
						}
						
						break;
					}
				}
				
				break;
			}
			
			attribute = next;
		}
	}
	
	if (process[kLightGraphParallax])
	{
		int32 parallaxTexcoordIndex = normalTexcoordIndex[0];
		
		new Route(process[kLightGraphTexcoord1 + parallaxTexcoordIndex], process[kLightGraphParallax], 0);
		new Route(process[kLightGraphParallax], process[kLightGraphNormalMap1], 0);
		
		if (process[kLightGraphTextureMap1])
		{
			int32 texcoordIndex = textureTexcoordIndex[0];
			if (texcoordIndex == parallaxTexcoordIndex) new Route(process[kLightGraphParallax], process[kLightGraphTextureMap1], 0);
			else new Route(process[kLightGraphTexcoord1 + texcoordIndex], process[kLightGraphTextureMap1], 0);
			
			if (process[kLightGraphTextureMap2])
			{
				texcoordIndex = textureTexcoordIndex[1];
				if (texcoordIndex == parallaxTexcoordIndex) new Route(process[kLightGraphParallax], process[kLightGraphTextureMap2], 0);
				else new Route(process[kLightGraphTexcoord1 + texcoordIndex], process[kLightGraphTextureMap2], 0);
			}
		}
		
		if (process[kLightGraphGlossMap])
		{
			int32 texcoordIndex = glossTexcoordIndex;
			if (texcoordIndex == parallaxTexcoordIndex) new Route(process[kLightGraphParallax], process[kLightGraphGlossMap], 0);
			else new Route(process[kLightGraphTexcoord1 + texcoordIndex], process[kLightGraphGlossMap], 0);
		}
		
		if (process[kLightGraphHorizon])
		{
			int32 texcoordIndex = horizonTexcoordIndex;
			if (texcoordIndex == parallaxTexcoordIndex) new Route(process[kLightGraphParallax], process[kLightGraphHorizon], 0);
			else new Route(process[kLightGraphTexcoord1 + texcoordIndex], process[kLightGraphHorizon], 0);
		}
	}
	else
	{
		if (process[kLightGraphTextureMap1])
		{
			new Route(process[kLightGraphTexcoord1 + textureTexcoordIndex[0]], process[kLightGraphTextureMap1], 0);
			if (process[kLightGraphTextureMap2]) new Route(process[kLightGraphTexcoord1 + textureTexcoordIndex[1]], process[kLightGraphTextureMap2], 0);
		}
		
		if (process[kLightGraphNormalMap1])
		{
			new Route(process[kLightGraphTexcoord1 + normalTexcoordIndex[0]], process[kLightGraphNormalMap1], 0);
			if (process[kLightGraphNormalMap2]) new Route(process[kLightGraphTexcoord1 + normalTexcoordIndex[1]], process[kLightGraphNormalMap2], 0);
		}
		
		if (process[kLightGraphGlossMap]) new Route(process[kLightGraphTexcoord1 + glossTexcoordIndex], process[kLightGraphGlossMap], 0);
		if (process[kLightGraphHorizon]) new Route(process[kLightGraphTexcoord1 + horizonTexcoordIndex], process[kLightGraphHorizon], 0);
	}
	
	Process *finalTextureMap = BuildTextureCombiner(materialObject, graph, &process[kLightGraphTextureMap1], &process[kLightGraphTextureMap2], &process[kLightGraphTextureCombiner], &process[kLightGraphVertexColor]);
	
	Process *finalNormalMap = process[kLightGraphNormalMap1];
	if (process[kLightGraphNormalMap2])
	{
		process[kLightGraphNormalCombiner] = new CombineNormalsProcess;
		graph->AddElement(process[kLightGraphNormalCombiner]);
		finalNormalMap = process[kLightGraphNormalCombiner];
		
		new Route(process[kLightGraphNormalMap1], process[kLightGraphNormalCombiner], 0);
		new Route(process[kLightGraphNormalMap2], process[kLightGraphNormalCombiner], 1);
	}
	
	bool colorArrayEnabled = false;
	if (renderable)
	{
		colorArrayEnabled = renderable->AttributeArrayEnabled(kArrayColor0);
		if (colorArrayEnabled)
		{
			if (renderable->GetComponentCount(kArrayColor0) > 1)
			{
				if (!process[kLightGraphVertexColor])
				{
					process[kLightGraphVertexColor] = new VertexColorProcess;
					graph->AddElement(process[kLightGraphVertexColor]);
				}
			}
			else
			{
				colorArrayEnabled = false;
			}
		}
	}
	
	Process *finalDiffuseProduct = finalTextureMap;
	if (finalDiffuseProduct)
	{
		if (process[kLightGraphDiffuseColor])
		{
			process[kLightGraphDiffuseMultiply1] = new MultiplyProcess;
			graph->AddElement(process[kLightGraphDiffuseMultiply1]);
			finalDiffuseProduct = process[kLightGraphDiffuseMultiply1];
			
			if (colorArrayEnabled)
			{
				process[kLightGraphColorMultiply] = new MultiplyProcess;
				graph->AddElement(process[kLightGraphColorMultiply]);
				
				new Route(process[kLightGraphDiffuseColor], process[kLightGraphColorMultiply], 0);
				new Route(process[kLightGraphVertexColor], process[kLightGraphColorMultiply], 1);
				
				new Route(finalTextureMap, process[kLightGraphDiffuseMultiply1], 0);
				new Route(process[kLightGraphColorMultiply], process[kLightGraphDiffuseMultiply1], 1);
			}
			else
			{
				new Route(finalTextureMap, process[kLightGraphDiffuseMultiply1], 0);
				new Route(process[kLightGraphDiffuseColor], process[kLightGraphDiffuseMultiply1], 1);
			}
		}
		else if (colorArrayEnabled)
		{
			process[kLightGraphDiffuseMultiply1] = new MultiplyProcess;
			graph->AddElement(process[kLightGraphDiffuseMultiply1]);
			finalDiffuseProduct = process[kLightGraphDiffuseMultiply1];
			
			new Route(finalTextureMap, process[kLightGraphDiffuseMultiply1], 0);
			new Route(process[kLightGraphVertexColor], process[kLightGraphDiffuseMultiply1], 1);
		}
	}
	else
	{
		if (colorArrayEnabled)
		{
			if (process[kLightGraphDiffuseColor])
			{
				process[kLightGraphDiffuseMultiply1] = new MultiplyProcess;
				graph->AddElement(process[kLightGraphDiffuseMultiply1]);
				finalDiffuseProduct = process[kLightGraphDiffuseMultiply1];
				
				new Route(process[kLightGraphDiffuseColor], process[kLightGraphDiffuseMultiply1], 0);
				new Route(process[kLightGraphVertexColor], process[kLightGraphDiffuseMultiply1], 1);
			}
			else
			{
				finalDiffuseProduct = process[kLightGraphVertexColor];
			}
		}
		else
		{
			finalDiffuseProduct = process[kLightGraphDiffuseColor];
		}
	}
	
	process[kLightGraphDiffuseReflection] = new DiffuseProcess;
	graph->AddElement(process[kLightGraphDiffuseReflection]);
	Process *finalDiffuseTerm = process[kLightGraphDiffuseReflection];
	
	if (finalNormalMap) new Route(finalNormalMap, process[kLightGraphDiffuseReflection], 0);
	
	if (finalDiffuseProduct)
	{
		process[kLightGraphDiffuseMultiply2] = new MultiplyProcess;
		graph->AddElement(process[kLightGraphDiffuseMultiply2]);
		finalDiffuseTerm = process[kLightGraphDiffuseMultiply2];
		
		new Route(finalDiffuseProduct, process[kLightGraphDiffuseMultiply2], 0);
		new Route(process[kLightGraphDiffuseReflection], process[kLightGraphDiffuseMultiply2], 1);
		
		if (materialFlags & kMaterialAlphaTest)
		{
			process[kLightGraphAlphaTestOutput] = new AlphaTestOutputProcess;
			graph->AddElement(process[kLightGraphAlphaTestOutput]);
			
			(new Route(finalDiffuseProduct, process[kLightGraphAlphaTestOutput], 0))->SetRouteSwizzle('aaaa');
		}
	}
	
	Process *finalLight = finalDiffuseTerm;
	
	Process *finalSpecularTerm = process[kLightGraphMicrofacet];
	if (finalSpecularTerm)
	{
		if (finalNormalMap) new Route(finalNormalMap, process[kLightGraphMicrofacet], 0);
		
		if ((process[kLightGraphGlossMap]) || (textureAlphaSemantic == kTextureSemanticGloss) || (normalAlphaSemantic == kTextureSemanticGloss))
		{
			process[kLightGraphSpecularMultiply2] = new MultiplyProcess;
			graph->AddElement(process[kLightGraphSpecularMultiply2]);
			finalSpecularTerm = process[kLightGraphSpecularMultiply2];
			
			if (process[kLightGraphMicrofacetReflectivity])
			{
				process[kLightGraphSpecularMultiply1] = new MultiplyProcess;
				graph->AddElement(process[kLightGraphSpecularMultiply1]);
				
				new Route(process[kLightGraphMicrofacetReflectivity], process[kLightGraphSpecularMultiply1], 0);
				
				if (process[kLightGraphGlossMap]) new Route(process[kLightGraphGlossMap], process[kLightGraphSpecularMultiply1], 1);
				else if (textureAlphaSemantic == kTextureSemanticGloss) (new Route(process[kLightGraphTextureMap1], process[kLightGraphSpecularMultiply1], 1))->SetRouteSwizzle('aaaa');
				else (new Route(process[kLightGraphNormalMap1], process[kLightGraphSpecularMultiply1], 1))->SetRouteSwizzle('aaaa');
				
				new Route(process[kLightGraphMicrofacet], process[kLightGraphSpecularMultiply2], 0);
				new Route(process[kLightGraphSpecularMultiply1], process[kLightGraphSpecularMultiply2], 1);
			}
			else
			{
				new Route(process[kLightGraphMicrofacet], process[kLightGraphSpecularMultiply2], 0);
				
				if (process[kLightGraphGlossMap]) new Route(process[kLightGraphGlossMap], process[kLightGraphSpecularMultiply2], 1);
				else if (textureAlphaSemantic == kTextureSemanticGloss) (new Route(process[kLightGraphTextureMap1], process[kLightGraphSpecularMultiply2], 1))->SetRouteSwizzle('aaaa');
				else (new Route(process[kLightGraphNormalMap1], process[kLightGraphSpecularMultiply2], 1))->SetRouteSwizzle('aaaa');
			}
		}
		else if (process[kLightGraphMicrofacetReflectivity])
		{
			process[kLightGraphSpecularMultiply2] = new MultiplyProcess;
			graph->AddElement(process[kLightGraphSpecularMultiply2]);
			finalSpecularTerm = process[kLightGraphSpecularMultiply2];
			
			new Route(process[kLightGraphMicrofacet], process[kLightGraphSpecularMultiply2], 0);
			new Route(process[kLightGraphMicrofacetReflectivity], process[kLightGraphSpecularMultiply2], 1);
		}
	}
	else
	{
		Process *finalSpecularProduct = process[kLightGraphSpecularColor];
		if (finalSpecularProduct)
		{
			if ((process[kLightGraphGlossMap]) || (textureAlphaSemantic == kTextureSemanticGloss) || (normalAlphaSemantic == kTextureSemanticGloss))
			{
				process[kLightGraphSpecularMultiply1] = new MultiplyProcess;
				graph->AddElement(process[kLightGraphSpecularMultiply1]);
				finalSpecularProduct = process[kLightGraphSpecularMultiply1];
				
				if (process[kLightGraphGlossMap]) new Route(process[kLightGraphGlossMap], process[kLightGraphSpecularMultiply1], 0);
				else if (textureAlphaSemantic == kTextureSemanticGloss) (new Route(process[kLightGraphTextureMap1], process[kLightGraphSpecularMultiply1], 0))->SetRouteSwizzle('aaaa');
				else (new Route(process[kLightGraphNormalMap1], process[kLightGraphSpecularMultiply1], 0))->SetRouteSwizzle('aaaa');
				new Route(process[kLightGraphSpecularColor], process[kLightGraphSpecularMultiply1], 1);
			}
			
			process[kLightGraphSpecularReflection] = new SpecularProcess;
			graph->AddElement(process[kLightGraphSpecularReflection]);
			
			if (finalNormalMap) new Route(finalNormalMap, process[kLightGraphSpecularReflection], 0);
			new Route(process[kLightGraphSpecularExponent], process[kLightGraphSpecularReflection], 1);
			
			process[kLightGraphSpecularMultiply2] = new MultiplyProcess;
			graph->AddElement(process[kLightGraphSpecularMultiply2]);
			
			new Route(process[kLightGraphSpecularReflection], process[kLightGraphSpecularMultiply2], 0);
			new Route(finalSpecularProduct, process[kLightGraphSpecularMultiply2], 1);
			
			finalSpecularTerm = process[kLightGraphSpecularMultiply2];
		}
	}
	
	if (finalSpecularTerm)
	{
		process[kLightGraphLightSum] = new AddProcess;
		graph->AddElement(process[kLightGraphLightSum]);
		finalLight = process[kLightGraphLightSum];
		
		new Route(finalDiffuseTerm, process[kLightGraphLightSum], 0);
		new Route(finalSpecularTerm, process[kLightGraphLightSum], 1);
		
		if (materialFlags & kMaterialSpecularBloom)
		{
			process[kLightGraphBloom1] = new MaximumProcess;
			process[kLightGraphBloom2] = new MaximumProcess;
			graph->AddElement(process[kLightGraphBloom1]);
			graph->AddElement(process[kLightGraphBloom2]);
			
			(new Route(finalSpecularTerm, process[kLightGraphBloom1], 0))->SetRouteSwizzle('xxxx');
			(new Route(finalSpecularTerm, process[kLightGraphBloom1], 1))->SetRouteSwizzle('yyyy');
			
			(new Route(finalSpecularTerm, process[kLightGraphBloom2], 0))->SetRouteSwizzle('zzzz');
			new Route(process[kLightGraphBloom1], process[kLightGraphBloom2], 1);
			
			process[kLightGraphBloomOutput] = new BloomOutputProcess;
			graph->AddElement(process[kLightGraphBloomOutput]);
			
			new Route(process[kLightGraphBloom2], process[kLightGraphBloomOutput], 0);
		}
	}
	
	if (process[kLightGraphHorizon])
	{
		new Route(finalLight, process[kLightGraphHorizon], 1);
		finalLight = process[kLightGraphHorizon];
	}
	
	process[kLightGraphLightOutput] = new LightOutputProcess;
	graph->AddElement(process[kLightGraphLightOutput]);
	
	new Route(finalLight, process[kLightGraphLightOutput], 0);
}

void ShaderAttribute::BuildEffectShaderGraph(const Renderable *renderable, const RenderSegment *renderSegment, const MaterialObject *materialObject, const List<Attribute> *attributeList, ShaderGraph *graph)
{
	Process				*process[kEffectGraphProcessCount];
	const Attribute		*firstAttribute[2];
	
	unsigned_int32 materialFlags = (materialObject) ? materialObject->GetMaterialFlags() : 0;
	if (renderSegment) materialFlags |= renderSegment->GetMaterialState();
	
	firstAttribute[0] = (materialObject) ? materialObject->GetFirstAttribute() : nullptr;
	firstAttribute[1] = (attributeList) ? attributeList->First() : nullptr;
	
	for (machine a = 0; a < kEffectGraphProcessCount; a++) process[a] = nullptr;
	
	for (machine a = 0; a < 2; a++)
	{
		const Attribute *attribute = firstAttribute[a];
		while (attribute)
		{
			const Attribute *next = attribute->Next();
			for (;;)
			{
				AttributeType type = attribute->GetAttributeType();
				switch (type)
				{
					case kAttributeReference:
					{
						attribute = static_cast<const ReferenceAttribute *>(attribute)->GetReference();
						if (attribute) continue;
						break;
					}
					
					case kAttributeDiffuse:
					{
						if (!process[kEffectGraphEffectColor])
						{
							process[kEffectGraphEffectColor] = new ColorProcess;
							graph->AddElement(process[kEffectGraphEffectColor]);
						}
						
						ColorProcess *colorProcess = static_cast<ColorProcess *>(process[kEffectGraphEffectColor]);
						const DiffuseAttribute *diffuseAttribute = static_cast<const DiffuseAttribute *>(attribute);
						const ColorRGBA& diffuseColor = diffuseAttribute->GetDiffuseColor();
						
						colorProcess->SetColorValue(diffuseColor);
						if (diffuseAttribute->GetAttributeFlags() & kAttributeMutable)
						{
							colorProcess->SetParameterSlot(0);
							colorProcess->SetParameterData(&diffuseColor.red);
						}
						
						break;
					}
					
					case kAttributeTextureMap:
					{
						const MapAttribute *mapAttribute = static_cast<const MapAttribute *>(attribute);
						
						if (!process[kEffectGraphTextureMap])
						{
							if (renderable->GetShaderFlags() & kShaderDistortion) process[kEffectGraphTextureMap] = new DistortionProcess;
							else process[kEffectGraphTextureMap] = new TextureMapProcess;
							graph->AddElement(process[kEffectGraphTextureMap]);
							
							static_cast<TextureMapProcess *>(process[kEffectGraphTextureMap])->SetTexture(mapAttribute->GetTexture());
						}
						else if (renderable->GetShaderFlags() & kShaderFireArrays)
						{
							if (!process[kEffectGraphFire])
							{
								process[kEffectGraphFire] = new FireProcess;
								graph->AddElement(process[kEffectGraphFire]);
							}
							
							static_cast<TextureMapProcess *>(process[kEffectGraphFire])->SetTexture(mapAttribute->GetTexture());
						}
						
						break;
					}
					
					case kAttributeDeltaDepth:
					{
						if (!process[kEffectGraphDeltaDepth])
						{
							process[kEffectGraphDeltaDepth] = new DeltaDepthProcess;
							graph->AddElement(process[kEffectGraphDeltaDepth]);
						}
						
						DeltaDepthProcess *deltaDepthProcess = static_cast<DeltaDepthProcess *>(process[kEffectGraphDeltaDepth]);
						const DeltaDepthAttribute *deltaDepthAttribute = static_cast<const DeltaDepthAttribute *>(attribute);
						deltaDepthProcess->SetDeltaScale(deltaDepthAttribute->GetDeltaScale());
						break;
					}
					
					case kAttributeFire:
					{
						if (!process[kEffectGraphFire])
						{
							process[kEffectGraphFire] = new FireProcess;
							graph->AddElement(process[kEffectGraphFire]);
						}
						
						FireProcess *fireProcess = static_cast<FireProcess *>(process[kEffectGraphFire]);
						const FireAttribute *fireAttribute = static_cast<const FireAttribute *>(attribute);
						const FireAttribute::FireParams *params = fireAttribute->GetFireParams();
						
						fireProcess->SetFireParams(params);
						if (renderable) fireProcess->SetFireData(params);
						break;
					}
				}
				
				break;
			}
			
			attribute = next;
		}
	}
	
	Process *finalEffectProduct = process[kEffectGraphEffectColor];
	
	if ((renderable) && (renderable->AttributeArrayEnabled(kArrayColor0)))
	{
		process[kEffectGraphVertexColor] = new VertexColorProcess;
		graph->AddElement(process[kEffectGraphVertexColor]);
		
		if (finalEffectProduct)
		{
			process[kEffectGraphColorMultiply] = new MultiplyProcess;
			graph->AddElement(process[kEffectGraphColorMultiply]);
			finalEffectProduct = process[kEffectGraphColorMultiply];
			
			new Route(process[kEffectGraphEffectColor], process[kEffectGraphColorMultiply], 0);
			new Route(process[kEffectGraphVertexColor], process[kEffectGraphColorMultiply], 1);
		}
		else
		{
			finalEffectProduct = process[kEffectGraphVertexColor];
		}
	}
	
	if (process[kEffectGraphTextureMap])
	{
		if (process[kEffectGraphFire])
		{
			new Route(process[kEffectGraphFire], process[kEffectGraphTextureMap], 0);
		}
		else
		{
			process[kEffectGraphTexcoord] = new Texcoord0Process;
			graph->AddElement(process[kEffectGraphTexcoord]);
			
			new Route(process[kEffectGraphTexcoord], process[kEffectGraphTextureMap], 0);
		}
		
		if (finalEffectProduct)
		{
			process[kEffectGraphEffectMultiply] = new MultiplyProcess;
			graph->AddElement(process[kEffectGraphEffectMultiply]);
			
			if (renderable->GetShaderFlags() & kShaderDistortion)
			{
				if (process[kEffectGraphDeltaDepth])
				{
					process[kEffectGraphAlphaMultiply] = new MultiplyProcess;
					graph->AddElement(process[kEffectGraphAlphaMultiply]);
					
					new Route(process[kEffectGraphDeltaDepth], process[kEffectGraphAlphaMultiply], 0);
					(new Route(finalEffectProduct, process[kEffectGraphAlphaMultiply], 1))->SetRouteSwizzle('aaaa');
					
					new Route(process[kEffectGraphAlphaMultiply], process[kEffectGraphEffectMultiply], 0);
				}
				else
				{
					(new Route(finalEffectProduct, process[kEffectGraphEffectMultiply], 0))->SetRouteSwizzle('aaaa');
				}
			}
			else
			{
				new Route(finalEffectProduct, process[kEffectGraphEffectMultiply], 0);
			}
			
			new Route(process[kEffectGraphTextureMap], process[kEffectGraphEffectMultiply], 1);
			
			finalEffectProduct = process[kEffectGraphEffectMultiply];
		}
		else
		{
			finalEffectProduct = process[kEffectGraphTextureMap];
		}
	}
	
	process[kEffectGraphColorOutput] = new AmbientOutputProcess;
	graph->AddElement(process[kEffectGraphColorOutput]);
	
	process[kEffectGraphAlphaOutput] = new AmbientAlphaOutputProcess;
	graph->AddElement(process[kEffectGraphAlphaOutput]);
	
	if (!finalEffectProduct)
	{
		finalEffectProduct = new ColorProcess;
		process[kEffectGraphEffectColor] = finalEffectProduct;
		graph->AddElement(finalEffectProduct);
	}
	
	new Route(finalEffectProduct, process[kEffectGraphColorOutput], 0);
	
	if ((process[kEffectGraphDeltaDepth]) && (!(renderable->GetShaderFlags() & kShaderDistortion)))
	{
		process[kEffectGraphAlphaMultiply] = new MultiplyProcess;
		graph->AddElement(process[kEffectGraphAlphaMultiply]);
		
		new Route(process[kEffectGraphDeltaDepth], process[kEffectGraphAlphaMultiply], 0);
		(new Route(finalEffectProduct, process[kEffectGraphAlphaMultiply], 1))->SetRouteSwizzle('aaaa');
		
		new Route(process[kEffectGraphAlphaMultiply], process[kEffectGraphAlphaOutput], 0);
	}
	else
	{
		if (materialFlags & kMaterialAlphaTest)
		{
			process[kEffectGraphAlphaTestOutput] = new AlphaTestOutputProcess;
			graph->AddElement(process[kEffectGraphAlphaTestOutput]);
			
			(new Route(finalEffectProduct, process[kEffectGraphAlphaTestOutput], 0))->SetRouteSwizzle('aaaa');
		}
		else
		{
			(new Route(finalEffectProduct, process[kEffectGraphAlphaOutput], 0))->SetRouteSwizzle('aaaa');
		}
	}
}

void ShaderAttribute::BuildPlainShaderGraph(const Renderable *renderable, const RenderSegment *renderSegment, const MaterialObject *materialObject, const List<Attribute> *attributeList, ShaderGraph *graph)
{
	unsigned_int32 materialFlags = (materialObject) ? materialObject->GetMaterialFlags() : 0;
	if (renderSegment) materialFlags |= renderSegment->GetMaterialState();
	
	if (materialFlags & kMaterialAlphaTest)
	{
		Process				*process[kPlainGraphProcessCount];
		const Attribute		*firstAttribute[2];
		int32				textureTexcoordIndex[2];
		
		firstAttribute[0] = (materialObject) ? materialObject->GetFirstAttribute() : nullptr;
		firstAttribute[1] = (attributeList) ? attributeList->First() : nullptr;
		
		for (machine a = 0; a < kPlainGraphProcessCount; a++) process[a] = nullptr;
		
		for (machine a = 0; a < 2; a++)
		{
			const Attribute *attribute = firstAttribute[a];
			while (attribute)
			{
				AttributeType type = attribute->GetAttributeType();
				switch (type)
				{
					case kAttributeDiffuse:
					{
						if (!process[kPlainGraphDiffuseColor])
						{
							process[kPlainGraphDiffuseColor] = new ColorProcess;
							graph->AddElement(process[kPlainGraphDiffuseColor]);
						}
						
						ColorProcess *colorProcess = static_cast<ColorProcess *>(process[kPlainGraphDiffuseColor]);
						const DiffuseAttribute *diffuseAttribute = static_cast<const DiffuseAttribute *>(attribute);
						const ColorRGBA& diffuseColor = diffuseAttribute->GetDiffuseColor();
						
						colorProcess->SetColorValue(diffuseColor);
						if (diffuseAttribute->GetAttributeFlags() & kAttributeMutable)
						{
							colorProcess->SetParameterSlot(0);
							colorProcess->SetParameterData(&diffuseColor.red);
						}
						
						break;
					}
					
					case kAttributeTextureMap:
					{
						const MapAttribute *mapAttribute = static_cast<const MapAttribute *>(attribute);
						
						int32 texcoordIndex = mapAttribute->GetTexcoordIndex();
						Process *texcoordProcess = process[kPlainGraphTexcoord1 + texcoordIndex];
						if (!texcoordProcess)
						{
							if (texcoordIndex == 0) texcoordProcess = new Texcoord0Process;
							else texcoordProcess = new Texcoord1Process;
							process[kPlainGraphTexcoord1 + texcoordIndex] = texcoordProcess;
							graph->AddElement(texcoordProcess);
						}
						
						if (!process[kPlainGraphTextureMap1])
						{
							process[kPlainGraphTextureMap1] = new TextureMapProcess;
							graph->AddElement(process[kPlainGraphTextureMap1]);
							
							static_cast<TextureMapProcess *>(process[kPlainGraphTextureMap1])->SetTexture(mapAttribute->GetTextureName());
							textureTexcoordIndex[0] = texcoordIndex;
						}
						else
						{
							if (!process[kPlainGraphTextureMap2])
							{
								process[kPlainGraphTextureMap2] = new TextureMapProcess;
								graph->AddElement(process[kPlainGraphTextureMap2]);
							}
							
							static_cast<TextureMapProcess *>(process[kPlainGraphTextureMap2])->SetTexture(mapAttribute->GetTextureName());
							textureTexcoordIndex[1] = texcoordIndex;
						}
						
						break;
					}
				}
				
				attribute = attribute->Next();
			}
		}
		
		if (process[kPlainGraphTextureMap1])
		{
			new Route(process[kPlainGraphTexcoord1 + textureTexcoordIndex[0]], process[kPlainGraphTextureMap1], 0);
			if (process[kPlainGraphTextureMap2]) new Route(process[kPlainGraphTexcoord1 + textureTexcoordIndex[1]], process[kPlainGraphTextureMap2], 0);
		}
		
		Process *finalTextureMap = BuildTextureCombiner(materialObject, graph, &process[kPlainGraphTextureMap1], &process[kPlainGraphTextureMap2], &process[kPlainGraphTextureCombiner], &process[kPlainGraphVertexColor]);
		
		bool colorArrayEnabled = (renderable) ? renderable->AttributeArrayEnabled(kArrayColor0) : false;
		if ((colorArrayEnabled) && (!process[kPlainGraphVertexColor]))
		{
			process[kPlainGraphVertexColor] = new VertexColorProcess;
			graph->AddElement(process[kPlainGraphVertexColor]);
		}
		
		Process *finalDiffuseProduct = finalTextureMap;
		if (finalDiffuseProduct)
		{
			if (process[kPlainGraphDiffuseColor])
			{
				process[kPlainGraphDiffuseMultiply1] = new MultiplyProcess;
				graph->AddElement(process[kPlainGraphDiffuseMultiply1]);
				finalDiffuseProduct = process[kPlainGraphDiffuseMultiply1];
				
				if (colorArrayEnabled)
				{
					process[kPlainGraphColorMultiply] = new MultiplyProcess;
					graph->AddElement(process[kPlainGraphColorMultiply]);
					
					(new Route(process[kPlainGraphDiffuseColor], process[kPlainGraphColorMultiply], 0))->SetRouteSwizzle('aaaa');
					(new Route(process[kPlainGraphVertexColor], process[kPlainGraphColorMultiply], 1))->SetRouteSwizzle('aaaa');
					
					(new Route(finalTextureMap, process[kPlainGraphDiffuseMultiply1], 0))->SetRouteSwizzle('aaaa');
					(new Route(process[kPlainGraphColorMultiply], process[kPlainGraphDiffuseMultiply1], 1))->SetRouteSwizzle('aaaa');
				}
				else
				{
					(new Route(finalTextureMap, process[kPlainGraphDiffuseMultiply1], 0))->SetRouteSwizzle('aaaa');
					(new Route(process[kPlainGraphDiffuseColor], process[kPlainGraphDiffuseMultiply1], 1))->SetRouteSwizzle('aaaa');
				}
			}
			else if (colorArrayEnabled)
			{
				process[kPlainGraphDiffuseMultiply1] = new MultiplyProcess;
				graph->AddElement(process[kPlainGraphDiffuseMultiply1]);
				finalDiffuseProduct = process[kPlainGraphDiffuseMultiply1];
				
				(new Route(finalTextureMap, process[kPlainGraphDiffuseMultiply1], 0))->SetRouteSwizzle('aaaa');
				(new Route(process[kPlainGraphVertexColor], process[kPlainGraphDiffuseMultiply1], 1))->SetRouteSwizzle('aaaa');
			}
		}
		else
		{
			if (colorArrayEnabled)
			{
				if (process[kPlainGraphDiffuseColor])
				{
					process[kPlainGraphDiffuseMultiply1] = new MultiplyProcess;
					graph->AddElement(process[kPlainGraphDiffuseMultiply1]);
					finalDiffuseProduct = process[kPlainGraphDiffuseMultiply1];
					
					(new Route(process[kPlainGraphDiffuseColor], process[kPlainGraphDiffuseMultiply1], 0))->SetRouteSwizzle('aaaa');
					(new Route(process[kPlainGraphVertexColor], process[kPlainGraphDiffuseMultiply1], 1))->SetRouteSwizzle('aaaa');
				}
				else
				{
					finalDiffuseProduct = process[kPlainGraphVertexColor];
				}
			}
			else
			{
				finalDiffuseProduct = process[kPlainGraphDiffuseColor];
			}
		}
		
		if (finalDiffuseProduct)
		{
			process[kPlainGraphAlphaTestOutput] = new AlphaTestOutputProcess;
			graph->AddElement(process[kPlainGraphAlphaTestOutput]);
			
			(new Route(finalDiffuseProduct, process[kPlainGraphAlphaTestOutput], 0))->SetRouteSwizzle('aaaa');
		}
	}
}

void ShaderAttribute::SetParameterValue(int32 slot, const Vector4D& param)
{
	for (machine a = 0; a < kShaderGraphCount; a++)
	{
		Process *process = shaderGraph[a].GetFirstElement();
		while (process)
		{
			if (process->GetBaseProcessType() == kProcessConstant)
			{
				ConstantProcess *constantProcess = static_cast<ConstantProcess *>(process);
				if (constantProcess->GetParameterSlot() == slot) constantProcess->SetParameterValue(param);
			}
			
			process = process->GetNextElement();
		}
	}
}

// ZYURVUR
