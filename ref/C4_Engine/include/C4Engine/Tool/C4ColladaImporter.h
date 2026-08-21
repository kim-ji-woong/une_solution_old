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


#ifndef C4ColladaImporter_h
#define C4ColladaImporter_h


#include "C4EditorPlugins.h"
#include "C4Geometries.h"
#include "C4Cameras.h"


extern "C"
{
	C4MODULEEXPORT C4::Plugin *ConstructPlugin(void);
}


namespace C4
{
	class ColladaContext;
	class EditorObject;
	
	
	namespace Collada
	{
		typedef Type	ElementType;
		typedef Type	LibraryType;
		typedef Type	NodeType;
		typedef Type	LightDataType;
		typedef Type	FacesType;
		typedef Type	SemanticType;
		typedef Type	ParamType;
		typedef Type	MethodType;
		typedef Type	TechniqueType;
		typedef Type	AxisType;
		
		
		enum
		{
			kElementAsset				= 'asst',
			kElementTechnique			= 'tech',
			kElementExtra				= 'xtra',
			kElementMesh				= 'mesh',
			kElementGeometry			= 'geom',
			kElementController			= 'ctlr',
			kElementNameArray			= 'name',
			kElementFloatArray			= 'flta',
			kElementAccessor			= 'accs',
			kElementSource				= 'sorc',
			kElementInput				= 'inpt',
			kElementSampler				= 'samp',
			kElementChannel				= 'chan',
			kElementAnimation			= 'anim',
			kElementLibrary				= 'libr',
			kElementIndexArray			= 'indx',
			kElementCountArray			= 'cont',
			kElementJoints				= 'join',
			kElementCombiner			= 'comb',
			kElementVertices			= 'vert',
			kElementBindShapeMatrix		= 'bshm',
			kElementSkin				= 'skin',
			kElementTargets				= 'targ',
			kElementMorph				= 'mrph',
			kElementFaces				= 'face',
			kElementControlVertices		= 'cvrt',
			kElementSpline				= 'spln',
			kElementInstance			= 'inst',
			kElementNode				= 'node',
			kElementScene				= 'scen',
			kElementMatrix				= 'mtrx',
			kElementTranslate			= 'xslt',
			kElementRotate				= 'rota',
			kElementScale				= 'scal',
			kElementLookat				= 'look',
			kElementUpAxis				= 'upax',
			kElementUnit				= 'unit',
			kElementInitFrom			= 'init',
			kElementImage				= 'imag',
			kElementTexture				= 'txtr',
			kElementMaterial			= 'matl',
			kElementBindMaterial		= 'bmat',
			kElementShader				= 'shdr',
			kElementPass				= 'pass',
			kElementProgram				= 'prog',
			kElementShadingModel		= 'smod',
			kElementEffect				= 'efct',
			kElementCommonProfile		= 'cpro',
			kElementMaterialProperty	= 'mprp',
			kElementParam				= 'parm',
			kElementNewParam			= 'nprm',
			kElementSurface				= 'surf',
			kElementSampler2D			= 'smp2',
			kElementWrap				= 'wrap',
			kElementLight				= 'lght',
			kElementLightData			= 'ldat',
			kElementCamera				= 'camr',
			kElementOptics				= 'optc',
			kElementPerspective			= 'pers',
			kElementRoot				= 'root'
		};
		
		
		enum
		{ 
			kLibraryGeometry			= 'geom',
			kLibraryMaterial			= 'matl', 
			kLibraryImage				= 'imag', 
			kLibraryTexture				= 'txtr', 
			kLibraryEffect				= 'efct',
			kLibraryLight				= 'lght', 
			kLibraryController			= 'ctlr',
			kLibraryAnimation			= 'anim',
			kLibraryVisualScene			= 'scen'
		}; 
		
		
		enum
		{ 
			kNodeNode					= 'node',
			kNodeJoint					= 'join',
			kNodeMarker					= 'mark'
		};
		
		
		enum
		{
			kLightDataDirectional		= 'drct',
			kLightDataPoint				= 'pont',
			kLightDataSpot				= 'spot'
		};
		
		
		enum
		{
			kFacesTriangles				= 'tris',
			kFacesPolygons				= 'poly',
			kFacesPolylist				= 'list'
		};
		
		
		enum
		{
			kSemanticVertex				= 'vert',
			kSemanticPosition			= 'posi',
			kSemanticNormal				= 'norm',
			kSemanticColor				= 'colr',
			kSemanticTexcoord			= 'texc',
			kSemanticJoint				= 'join',
			kSemanticWeight				= 'wght',
			kSemanticInvBindMatrix		= 'ibnd',
			kSemanticBindPosition		= 'bpos',
			kSemanticBindNormal			= 'bnrm',
			kSemanticJointWeight		= 'jtwt',
			kSemanticMorphTarget		= 'mptg',
			kSemanticMorphWeight		= 'mpwt',
			kSemanticImage				= 'imag',
			kSemanticTexture			= 'txtr',
			kSemanticInput				= 'iput',
			kSemanticOutput				= 'oput',
			kSemanticInterpolation		= 'itrp',
			kSemanticInTangent			= 'itan',
			kSemanticOutTangent			= 'otan'
		};
		
		
		enum
		{
			kParamFloat					= 'flot',
			kParamColor					= 'colr',
			kParamDiffuse				= 'diff',
			kParamSpecular				= 'spec',
			kParamShininess				= 'shin',
			kParamEmission				= 'emis',
			kParamBump					= 'bump',
			kParamFalloffAngle			= 'angl',
			kParamAspectRatio			= 'aspc',
			kParamXFov					= 'xfov',
			kParamYFov					= 'yfov',
			kParamZNear					= 'zner',
			kParamZFar					= 'zfar'
		};
		
		
		enum
		{
			kMethodNormalized			= 'nrml',
			kMethodRelative				= 'rela'
		};
		
		
		enum
		{
			kTechniqueCommon			= 'comm'
		};
		
		
		enum
		{
			kAxisX						= 'xxxx',
			kAxisY						= 'yyyy',
			kAxisZ						= 'zzzz'
		};
		
		
		enum
		{
			kMaxIdentifierLength		= 255,
			kMaxStringLength			= 255
		};
		
		
		enum
		{
			kImportTextureMaps			= 1 << 0,
			kImportMergeMaterials		= 1 << 1,
			kImportReuseNamedMaterials	= 1 << 2
		};
		
		
		class Node;
		class Input;
		class Skin;
		class Scene;
		class CountArray;
		class BindMaterial;
		class NewParam;
		
		
		class Element : public Tree<Element>
		{
			private:
				
				ElementType						elementType;
				ColladaContext					*colladaContext;
				
				String<kMaxIdentifierLength>	identifier;
				String<kMaxIdentifierLength>	subIdentifier;
			
			protected:
				
				Scene *GetOwningScene(void) const;
				
				Element *FindElement(const char *id, Element *root = nullptr);
				Element *FindSubElement(const char *id, Element *root = nullptr);
				Node *FindNamedElement(const char *name, Element *root = nullptr);
			
			public:
				
				Element(ElementType type = 0, ColladaContext *context = nullptr);
				~Element();
				
				ElementType GetElementType(void) const
				{
					return (elementType);
				}
				
				ColladaContext *GetColladaContext(void) const
				{
					return (colladaContext);
				}
				
				const String<kMaxIdentifierLength>& GetIdentifier(void) const
				{
					return (identifier);
				}
				
				void SetIdentifier(const char *id)
				{
					identifier = id;
				}
				
				const String<kMaxIdentifierLength>& GetSubIdentifier(void) const
				{
					return (subIdentifier);
				}
				
				void SetSubIdentifier(const char *id)
				{
					subIdentifier = id;
				}
				
				virtual void Preprocess(void);
				virtual void ProcessContent(const char *& text);
		};
		
		
		class TargetingElement : public Element
		{
			private:
				
				Element							*targetElement;
				Element							*targetSubElement;
				
				int32							targetComponent;
				
				String<kMaxIdentifierLength>	targetIdentifier;
			
			protected:
				
				TargetingElement(ElementType type);
			
			public:
				
				~TargetingElement();
				
				Element *GetTargetElement(void) const
				{
					return (targetElement);
				}
				
				Element *GetTargetSubElement(void) const
				{
					return (targetSubElement);
				}
				
				int32 GetTargetComponent(void) const
				{
					return (targetComponent);
				}
				
				const String<kMaxIdentifierLength>& GetTargetIdentifier(void) const
				{
					return (targetIdentifier);
				}
				
				void SetTargetIdentifier(const char *identifier)
				{
					targetIdentifier = identifier;
				}
				
				void Preprocess(void);
		};
		
		
		class InterleavingElement : public Element
		{
			private:
				
				int32				itemCount;
				int32				inputCount;
				
				const CountArray	*countArray;
			
			protected:
				
				InterleavingElement(ElementType type);
				
				virtual void ProcessInput(int32 index, const Input *input) = 0;
			
			public:
				
				~InterleavingElement();
				
				int32 GetItemCount(void) const
				{
					return (itemCount);
				}
				
				void SetItemCount(int32 count)
				{
					itemCount = count;
				}
				
				int32 GetInputCount(void) const
				{
					return (inputCount);
				}
				
				const CountArray *GetCountArray(void) const
				{
					return (countArray);
				}
				
				void Preprocess(void);
		};
		
		
		class MaterialAttributeElement : public Element
		{
			protected:
				
				MaterialAttributeElement(ElementType type, ColladaContext *context = nullptr);
			
			public:
				
				~MaterialAttributeElement();
				
				virtual void BuildAttributeList(List<C4::Attribute> *attributeList) const = 0;
		};
		
		
		class Asset : public Element
		{
			public:
				
				Asset();
				~Asset();
		};
		
		
		class Technique : public Element
		{
			private:
				
				TechniqueType		techniqueType;
			
			public:
				
				Technique(TechniqueType type = 0);
				~Technique();
				
				TechniqueType GetTechniqueType(void) const
				{
					return (techniqueType);
				}
				
				void SetTechniqueType(TechniqueType type)
				{
					techniqueType = type;
				}
		};
		
		
		class Mesh : public Element
		{
			public:
				
				Mesh();
				~Mesh();
				
				C4::MeshGeometry *BuildGeometry(Collada::Scene *scene, const List<BindMaterial> *materialBindingList, Collada::Skin *skin = nullptr) const;
		};
		
		
		class Geometry : public Element
		{
			private:
				
				String<kMaxIdentifierLength>	geometryName;
			
			public:
				
				Geometry();
				~Geometry();
				
				const String<kMaxIdentifierLength>& GetGeometryName(void) const
				{
					return (geometryName);
				}
				
				void SetGeometryName(const char *name)
				{
					geometryName = name;
				}
				
				C4::MeshGeometry *BuildGeometry(Collada::Scene *scene, const List<BindMaterial> *materialBindingList, Collada::Skin *skin = nullptr) const;
				C4::PathMarker *BuildPath(void) const;
		};
		
		
		class Controller : public TargetingElement
		{
			public:
				
				Controller();
				~Controller();
		};
		
		
		class NameArray : public Element
		{
			private:
				
				int32							nameCount;
				int32							arraySize;
				String<kMaxIdentifierLength>	*nameArray;
			
			public:
				
				NameArray();
				~NameArray();
				
				const String<kMaxIdentifierLength> *GetArray(void) const
				{
					return (nameArray);
				}
				
				void SetArraySize(int32 size);
				
				void ProcessContent(const char *& text);
		};
		
		
		class FloatArray : public Element
		{
			private:
				
				int32		floatCount;
				int32		arraySize;
				float		*floatArray;
			
			public:
				
				FloatArray();
				~FloatArray();
				
				const float *GetArray(void) const
				{
					return (floatArray);
				}
				
				void SetArraySize(int32 size);
				
				void ProcessContent(const char *& text);
		};
		
		
		class Accessor : public Element, public ListElement<Accessor>
		{
			private:
				
				int32							accessCount;
				int32							accessStride;
				
				Element							*sourceElement;
				String<kMaxIdentifierLength>	sourceIdentifier;
			
			public:
				
				Accessor();
				~Accessor();
				
				int32 GetAccessCount(void) const
				{
					return (accessCount);
				}
				
				void SetAccessCount(int32 count)
				{
					accessCount = count;
				}
				
				int32 GetAccessStride(void) const
				{
					return (accessStride);
				}
				
				void SetAccessStride(int32 stride)
				{
					accessStride = stride;
				}
				
				const Element *GetSourceElement(void) const
				{
					return (sourceElement);
				}
				
				const String<kMaxIdentifierLength>& GetSourceIdentifier(void) const
				{
					return (sourceIdentifier);
				}
				
				void SetSourceIdentifier(const char *identifier)
				{
					sourceIdentifier = identifier;
				}
				
				void Preprocess(void);
				
				const char *GetNameValue(int32 index) const;
				const float *GetFloatValue(int32 index) const;
		};
		
		
		class Source : public Element
		{
			private:
				
				List<Accessor>				accessorList;
				const NewParam				*newParamElement;
				String<kMaxStringLength>	paramIdentifier;
			
			public:
				
				Source();
				~Source();
				
				const Accessor *GetFirstAccessor(void) const
				{
					return (accessorList.First());
				}
				
				const NewParam *GetNewParamElement(void) const
				{
					return (newParamElement);
				}
				
				void Preprocess(void);
				void ProcessContent(const char *& text);
		};
		
		
		class Input : public Element, public ListElement<Input>
		{
			private:
				
				int32							inputIndex;
				SemanticType					inputSemantic;
				int32							texcoordSet;
				
				Element							*sourceElement;
				String<kMaxIdentifierLength>	sourceIdentifier;
			
			public:
				
				Input();
				~Input();
				
				int32 GetInputIndex(void) const
				{
					return (inputIndex);
				}
				
				void SetInputIndex(int32 index)
				{
					inputIndex = index;
				}
				
				SemanticType GetInputSemantic(void) const
				{
					return (inputSemantic);
				}
				
				void SetInputSemantic(SemanticType semantic)
				{
					inputSemantic = semantic;
				}
				
				int32 GetTexcoordSet(void) const
				{
					return (texcoordSet);
				}
				
				void SetTexcoordSet(int32 set)
				{
					texcoordSet = set;
				}
				
				const Element *GetSourceElement(void) const
				{
					return (sourceElement);
				}
				
				const String<kMaxIdentifierLength>& GetSourceIdentifier(void) const
				{
					return (sourceIdentifier);
				}
				
				void SetSourceIdentifier(const char *identifier)
				{
					sourceIdentifier = identifier;
				}
				
				void Preprocess(void);
		};
		
		
		class Sampler : public Element
		{
			private:
				
				const Accessor		*inputAccessor;
				const Accessor		*outputAccessor;
				const Accessor		*interpolationAccessor;
				const Accessor		*inTangentAccessor;
				const Accessor		*outTangentAccessor;
				
				float				beginTime;
				float				endTime;
			
			public:
				
				Sampler();
				~Sampler();
				
				float GetBeginTime(void) const
				{
					return (beginTime);
				}
				
				float GetEndTime(void) const
				{
					return (endTime);
				}
				
				void Preprocess(void);
				
				bool CalculateValue(float time, float *value, ElementType type) const;
		};
		
		
		class Channel : public TargetingElement, public ListElement<Channel>
		{
			private:
				
				Element							*sourceElement;
				String<kMaxIdentifierLength>	sourceIdentifier;
			
			public:
				
				Channel();
				~Channel();
				
				void SetSourceIdentifier(const char *identifier)
				{
					sourceIdentifier = identifier;
				}
				
				void Preprocess(void);
				
				void SetTargetValue(float time) const;
		};
		
		
		class Animation : public Element, public Tree<Animation>, public ListElement<Animation>
		{
			private:
				
				List<Channel>		channelList;
				
				float				beginTime;
				float				endTime;
			
			public:
				
				Animation();
				~Animation();
				
				const Channel *GetFirstChannel(void) const
				{
					return (channelList.First());
				}
				
				float GetBeginTime(void) const
				{
					return (beginTime);
				}
				
				float GetEndTime(void) const
				{
					return (endTime);
				}
				
				void Preprocess(void);
				
				void SetAnimationTransforms(float time) const;
		};
		
		
		class Library : public Element
		{
			private:
				
				LibraryType			libraryType;
				
				List<Animation>		animationList;
				
				float				beginTime;
				float				endTime;
			
			public:
				
				Library(LibraryType type = 0);
				~Library();
				
				LibraryType GetLibraryType(void) const
				{
					return (libraryType);
				}
				
				void SetLibraryType(LibraryType type)
				{
					libraryType = type;
				}
				
				Animation *GetFirstAnimation(void) const
				{
					return (animationList.First());
				}
				
				float GetBeginTime(void) const
				{
					return (beginTime);
				}
				
				float GetEndTime(void) const
				{
					return (endTime);
				}
				
				void Preprocess(void);
				
				void SetAnimationTransforms(float time) const;
		};
		
		
		class IndexArray : public Element
		{
			private:
				
				Array<int32>		indexArray;
			
			public:
				
				IndexArray();
				~IndexArray();
				
				int32 GetIndexCount(void) const
				{
					return (indexArray.GetElementCount());
				}
				
				int32 GetIndexValue(int32 index) const
				{
					return (indexArray[index]);
				}
				
				void AddIndex(int32 index)
				{
					indexArray.AddElement(index);
				}
				
				void ProcessContent(const char *& text);
		};
		
		
		class CountArray : public Element
		{
			private:
				
				Array<int32>		countArray;
			
			public:
				
				CountArray();
				~CountArray();
				
				int32 GetArraySize(void) const
				{
					return (countArray.GetElementCount());
				}
				
				const int32 *GetArray(void) const
				{
					return (countArray);
				}
				
				void ProcessContent(const char *& text);
		};
		
		
		class Joints : public Element
		{
			public:
				
				Joints();
				~Joints();
		};
		
		
		class Combiner : public InterleavingElement
		{
			private:
				
				int32				jointIndex;
				const Accessor		*jointAccessor;
				
				int32				weightIndex;
				const Accessor		*weightAccessor;
				
				int32				weightedVertexCount;
				unsigned_int32		weightDataSize;
				
				void ProcessInput(int32 index, const Input *input);
				
				int32 CountVertexWeights(int32 start, int32 count, const Collada::IndexArray *indexArray) const;
				C4::WeightedVertex *GenerateVertexWeights(int32 maxBoneIndex, C4::WeightedVertex *weightedVertex, int32 start, int32 count, const Collada::IndexArray *indexArray) const;
			
			public:
				
				Combiner();
				~Combiner();
				
				int32 GetWeightedVertexCount(void) const
				{
					return (weightedVertexCount);
				}
				
				unsigned_int32 GetWeightDataSize(void) const
				{
					return (weightDataSize);
				}
				
				void Preprocess(void);
				
				bool GenerateWeightData(int32 maxBoneIndex, C4::WeightedVertex *weightedVertex, const C4::WeightedVertex **weightDataTable) const;
		};
		
		
		class Vertices : public Element
		{
			public:
				
				Vertices();
				~Vertices();
		};
		
		
		class BindShapeMatrix : public Element
		{
			private:
				
				Transform4D		bindShapeMatrix;
			
			public:
				
				BindShapeMatrix();
				~BindShapeMatrix();
				
				const Transform4D& GetMatrix(void) const
				{
					return (bindShapeMatrix);
				}
				
				void Preprocess(void);
				void ProcessContent(const char *& text);
		};
		
		
		class Skin : public Element
		{
			private:
				
				const Accessor					*bindPositionAccessor;
				const Accessor					*bindNormalAccessor;
				const Accessor					*invBindMatrixAccessor;
				const Accessor					*jointNameAccessor;
				const Combiner					*jointWeightCombiner;
				const BindShapeMatrix			*bindShapeMatrix;
				
				char							*skinStorage;
				C4::SkinData					skinData;
				
				Element							*sourceElement;
				String<kMaxIdentifierLength>	sourceIdentifier;
				
				void ProcessInputs(const Element *root);
				void PreprocessJointWeightSource(const Element *source);
			
			public:
				
				Skin();
				~Skin();
				
				const Element *GetSourceElement(void) const
				{
					return (sourceElement);
				}
				
				void SetSourceIdentifier(const char *identifier)
				{
					sourceIdentifier = identifier;
				}
				
				const Accessor *GetBindPositionAccessor(void) const
				{
					return (bindPositionAccessor);
				}
				
				const Accessor *GetBindNormalAccessor(void) const
				{
					return (bindNormalAccessor);
				}
				
				void Preprocess(void);
				
				const C4::SkinData *GetSkinData(Collada::Scene *scene);
		};
		
		
		class Targets : public Element
		{
			public:
				
				Targets();
				~Targets();
		};
		
		
		class Morph : public Element
		{
			private:
				
				MethodType		methodType;
			
			public:
				
				Morph();
				~Morph();
				
				MethodType GetMorphMethod(void) const
				{
					return (methodType);
				}
				
				void SetMorphMethod(MethodType type)
				{
					methodType = type;
				}
		};
		
		
		class Faces : public InterleavingElement
		{
			private:
				
				FacesType						facesType;
				
				int32							positionIndex;
				const Accessor					*positionAccessor;
				
				int32							normalIndex;
				const Accessor					*normalAccessor;
				
				int32							colorIndex;
				const Accessor					*colorAccessor;
				
				int32							texcoordIndex[kMaxGeometryTexcoordCount];
				const Accessor					*texcoordAccessor[kMaxGeometryTexcoordCount];
				
				Element							*materialElement;
				String<kMaxIdentifierLength>	materialIdentifier;
				
				void ProcessInput(int32 index, const Input *input);
				
				GeometryPolygon *BuildPolygon(const int32 *entry, const IndexArray *indexArray, const Accessor *posAccessor, const Accessor *nrmAccessor) const;
			
			public:
				
				Faces(FacesType type);
				~Faces();
				
				FacesType GetFacesType(void) const
				{
					return (facesType);
				}
				
				Element *GetMaterialElement(void) const
				{
					return (materialElement);
				}
				
				const String<kMaxIdentifierLength>& GetMaterialIdentifier(void) const
				{
					return (materialIdentifier);
				}
				
				void SetMaterialIdentifier(const char *identifier)
				{
					materialIdentifier = identifier;
				}
				
				void Preprocess(void);
				
				GeometrySurface *BuildSurface(const Collada::Scene *scene, const Collada::Skin *skin = nullptr) const;
		};
		
		
		class ControlVertices : public Element
		{
			public:
				
				ControlVertices();
				~ControlVertices();
		};
		
		
		class Spline : public Element
		{
			private:
				
				bool			closedFlag;
				
				const Accessor	*positionAccessor;
			
			public:
				
				Spline();
				~Spline();
				
				bool GetClosedFlag(void) const
				{
					return (closedFlag);
				}
				
				void SetClosedFlag(bool closed)
				{
					closedFlag = closed;
				}
				
				void Preprocess(void);
				
				void BuildPathComponents(Path *path) const;
		};
		
		
		class Instance : public Element, public ListElement<Instance>
		{
			private:
				
				List<BindMaterial>				materialBindingList;
				
				Element							*resourceElement;
				String<kMaxIdentifierLength>	resourceIdentifier;
				
				String<kMaxIdentifierLength>	symbolIdentifier;
			
			public:
				
				Instance();
				~Instance();
				
				const BindMaterial *GetFirstMaterialBinding(void) const
				{
					return (materialBindingList.First());
				}
				
				Element *GetResourceElement(void) const
				{
					return (resourceElement);
				}
				
				const String<kMaxIdentifierLength>& GetResourceIdentifier(void) const
				{
					return (resourceIdentifier);
				}
				
				void SetResourceIdentifier(const char *identifier)
				{
					resourceIdentifier = identifier;
				}
				
				const String<kMaxIdentifierLength>& GetSymbolIdentifier(void) const
				{
					return (symbolIdentifier);
				}
				
				void SetSymbolIdentifier(const char *identifier)
				{
					symbolIdentifier = identifier;
				}
				
				void Preprocess(void);
				
				C4::Node *BuildInstance(Collada::Scene *scene) const;
		};
		
		
		class Node : public Element, public ListElement<Node>
		{
			private:
				
				NodeType						nodeType;
				unsigned_int32					nodeHash;
				String<kMaxIdentifierLength>	nodeName;
				
				void AddToScene(C4::Node *node, C4::Node *superNode) const;
			
			public:
				
				Node();
				~Node();
				
				NodeType GetNodeType(void) const
				{
					return (nodeType);
				}
				
				void SetNodeType(NodeType type)
				{
					nodeType = type;
				}
				
				unsigned_int32 GetNodeHash(void) const
				{
					return (nodeHash);
				}
				
				void SetNodeHash(unsigned_int32 hash)
				{
					nodeHash = hash;
				}
				
				const String<kMaxIdentifierLength>& GetNodeName(void) const
				{
					return (nodeName);
				}
				
				void SetNodeName(const char *name)
				{
					nodeName = name;
				}
					
				void Preprocess(void);
			
				Transform4D EvaluateTransform(void) const;
				void BuildNode(Collada::Scene *scene, C4::Node *superNode);
		};
		
		
		class Scene : public Element
		{
			private:
				
				int32		nodeCount;
				
				static void RemoveDeadNodes(C4::Node *rootNode);
			
			public:
				
				Scene();
				~Scene();
				
				int32 GetNodeCount(void) const
				{
					return (nodeCount);
				}
				
				void Preprocess(void);
				
				Node *FindNode(const char *name);
				
				void BuildNodes(const Collada::Element *rootElement, C4::Node *superNode);
				void BuildScene(C4::Node *rootNode);
		};
		
		
		class Matrix : public Element
		{
			private:
				
				Transform4D		transformMatrix;
			
			public:
				
				Matrix();
				~Matrix();
				
				const Transform4D& GetTransformMatrix(void) const
				{
					return (transformMatrix);
				}
				
				void SetTransformMatrix(const Transform4D& matrix)
				{
					transformMatrix = matrix;
				}
				
				void ProcessContent(const char *& text);
		};
		
		
		class Translate : public Element
		{
			private:
				
				Point3D		translatePosition;
			
			public:
				
				Translate();
				~Translate();
				
				const Point3D& GetTranslatePosition(void) const
				{
					return (translatePosition);
				}
				
				void SetTranslatePosition(const Point3D& position)
				{
					translatePosition = position;
				}
				
				void ProcessContent(const char *& text);
		};
		
		
		class Rotate : public Element
		{
			private:
				
				Vector3D	rotateAxis;
				float		rotateAngle;
			
			public:
				
				Rotate();
				~Rotate();
				
				const Vector3D& GetRotateAxis(void) const
				{
					return (rotateAxis);
				}
				
				void SetRotateAxis(const Vector3D& axis)
				{
					rotateAxis = axis;
				}
				
				float GetRotateAngle(void) const
				{
					return (rotateAngle);
				}
				
				void SetRotateAngle(float angle)
				{
					rotateAngle = angle;
				}
				
				void ProcessContent(const char *& text);
		};
		
		
		class Scale : public Element
		{
			private:
				
				Vector3D	scaleFactor;
			
			public:
				
				Scale();
				~Scale();
				
				const Vector3D& GetScaleFactor(void) const
				{
					return (scaleFactor);
				}
				
				void SetScaleFactor(const Vector3D& factor)
				{
					scaleFactor = factor;
				}
				
				void ProcessContent(const char *& text);
		};
		
		
		class Lookat : public Element
		{
			private:
				
				Point3D		cameraPosition;
				Point3D		targetPosition;
				Vector3D	upDirection;
			
			public:
				
				Lookat();
				~Lookat();
				
				const Point3D& GetCameraPosition(void) const
				{
					return (cameraPosition);
				}
				
				const Point3D& GetTargetPosition(void) const
				{
					return (targetPosition);
				}
				
				const Vector3D& GetUpDirection(void) const
				{
					return (upDirection);
				}
				
				void ProcessContent(const char *& text);
		};
		
		
		class UpAxis : public Element
		{
			private:
				
				AxisType	upAxis;
			
			public:
				
				UpAxis();
				~UpAxis();
				
				AxisType GetUpAxisType(void) const
				{
					return (upAxis);
				}
				
				void Preprocess(void);
				void ProcessContent(const char *& text);
		};
		
		
		class Unit : public Element
		{
			private:
				
				float		meterScale;
			
			public:
				
				Unit();
				~Unit();
				
				float GetMeterScale(void) const
				{
					return (meterScale);
				}
				
				void SetMeterScale(float scale)
				{
					meterScale = scale;
				}
				
				void Preprocess(void);
		};
		
		
		class InitFrom : public Element
		{
			private:
				
				String<kMaxStringLength>	initString;
			
			public:
				
				InitFrom();
				~InitFrom();
				
				const char *GetInitString(void) const
				{
					return (initString);
				}
				
				void ProcessContent(const char *& text);
		};
		
		
		class Image : public Element
		{
			private:
				
				String<kMaxStringLength>		sourceString;
				String<kMaxIdentifierLength>	imageName;
				
				static unsigned_int32 GetHexValue(unsigned_int32 c);
			
			public:
				
				Image();
				~Image();
				
				void SetSourceString(const char *string)
				{
					sourceString = string;
				}
				
				const char *GetImageName(void) const
				{
					return (imageName);
				}
				
				void Preprocess(void);
		};
		
		
		class Texture : public MaterialAttributeElement
		{
			private:
				
				Element								*imageElement;
				C4::String<kMaxIdentifierLength>	imageIdentifier;
			
			public:
				
				Texture(ColladaContext *context);
				~Texture();
				
				Element *GetImageElement(void) const
				{
					return (imageElement);
				}
				
				void SetImageIdentifier(const char *identifier)
				{
					imageIdentifier = identifier;
				}
				
				void Preprocess(void);
				
				void BuildAttributeList(List<C4::Attribute> *attributeList) const;
		};
		
		
		class Material : public Element
		{
			private:
				
				C4::MaterialObject				*materialObject;
				String<kMaxIdentifierLength>	materialName;
			
			public:
				
				Material(ColladaContext *context);
				~Material();
				
				const String<kMaxIdentifierLength>& GetMaterialName(void) const
				{
					return (materialName);
				}
				
				void SetMaterialName(const char *name)
				{
					materialName = name;
				}
				
				C4::MaterialObject *GetMaterialObject(void);
		};
		
		
		class BindMaterial : public Element, public ListElement<BindMaterial>
		{
			private:
				
				List<Instance>		instanceList;
			
			public:
				
				BindMaterial();
				~BindMaterial();
				
				const Instance *GetFirstInstance(void) const
				{
					return (instanceList.First());
				}
				
				void Preprocess(void);
		};
		
		
		class Shader : public Element
		{
			public:
				
				Shader();
				~Shader();
				
				C4::MaterialObject *BuildMaterial(void) const;
		};
		
		
		class Pass : public Element
		{
			public:
				
				Pass();
				~Pass();
				
				C4::MaterialObject *BuildMaterial(void) const;
		};
		
		
		class Program : public MaterialAttributeElement
		{
			public:
				
				Program();
				~Program();
				
				void BuildAttributeList(List<C4::Attribute> *attributeList) const;
		};
		
		
		class ShadingModel : public Element
		{
			private:
				
				unsigned_int32		importFlags;
			
			public:
				
				ShadingModel(ColladaContext *context);
				~ShadingModel();
				
				C4::MaterialObject *BuildMaterial(void) const;
		};
		
		
		class Effect : public Element
		{
			public:
				
				Effect();
				~Effect();
				
				C4::MaterialObject *BuildMaterial(void) const;
		};
		
		
		class CommonProfile : public Element
		{
			public:
				
				CommonProfile();
				~CommonProfile();
		};
		
		
		class MaterialProperty : public Element
		{
			private:
				
				ParamType	propertyType;
			
			public:
				
				MaterialProperty(ParamType type);
				~MaterialProperty();
				
				ParamType GetPropertyType(void) const
				{
					return (propertyType);
				}
		};
		
		
		class Param : public Element
		{
			private:
				
				ParamType		paramType;
				
				float			floatValue[4];
			
			public:
				
				Param(ParamType type = 0);
				~Param();
				
				ParamType GetParamType(void) const
				{
					return (paramType);
				}
				
				void SetParamType(ParamType type)
				{
					paramType = type;
				}
				
				float GetFloatValue(int32 index) const
				{
					return (floatValue[index]);
				}
				
				const ColorRGB& GetColorValue(void) const
				{
					return (*reinterpret_cast<const ColorRGB *>(floatValue));
				}
				
				void ProcessContent(const char *& text);
		};
		
		
		class Surface : public Element
		{
			private:
				
				const Image		*imageElement;
			
			public:
				
				Surface();
				~Surface();
				
				const Image *GetImageElement(void) const
				{
					return (imageElement);
				}
				
				void Preprocess(void);
		};
		
		
		class Sampler2D : public Element
		{
			private:
				
				const Surface	*surfaceElement;
				
				TextureWrap		wrapMode[2];
			
			public:
				
				Sampler2D();
				~Sampler2D();
				
				const Surface *GetSurfaceElement(void) const
				{
					return (surfaceElement);
				}
				
				TextureWrap GetWrapMode(int32 index) const
				{
					return (wrapMode[index]);
				}
				
				void Preprocess(void);
		};
		
		
		class Wrap : public Element
		{
			private:
				
				int32			textureCoord;
				TextureWrap		wrapMode;
			
			public:
				
				Wrap(int32 coord);
				~Wrap();
				
				int32 GetTextureCoord(void) const
				{
					return (textureCoord);
				}
				
				TextureWrap GetWrapMode(void) const
				{
					return (wrapMode);
				}
				
				void ProcessContent(const char *& text);
		};
		
		
		class NewParam : public Element
		{
			private:
				
				const Surface		*surfaceElement;
				const Sampler2D		*sampler2DElement;
			
			public:
				
				NewParam();
				~NewParam();
				
				const Surface *GetSurfaceElement(void) const
				{
					return (surfaceElement);
				}
				
				const Sampler2D *GetSampler2DElement(void) const
				{
					return (sampler2DElement);
				}
				
				void Preprocess(void);
		};
		
		
		class Light : public Element
		{
			public:
				
				Light();
				~Light();
				
				C4::Light *BuildLight(void) const;
		};
		
		
		class LightData : public Element
		{
			private:
				
				LightDataType	lightDataType;
			
			public:
				
				LightData(LightDataType type);
				~LightData();
				
				LightDataType GetLightDataType(void) const
				{
					return (lightDataType);
				}
				
				virtual C4::Light *BuildLight(void) const = 0;
		};
		
		
		class Directional : public LightData
		{
			public:
				
				Directional();
				~Directional();
				
				C4::Light *BuildLight(void) const;
		};
		
		
		class Point : public LightData
		{
			public:
				
				Point();
				~Point();
				
				C4::Light *BuildLight(void) const;
		};
		
		
		class Spot : public LightData
		{
			public:
				
				Spot();
				~Spot();
				
				C4::Light *BuildLight(void) const;
		};
		
		
		class Camera : public Element
		{
			private:
				
				float		cameraFov;
			
			public:
				
				Camera();
				~Camera();
				
				C4::Camera *BuildCamera(void) const;
		};
		
		
		class Optics : public Element
		{
			public:
				
				Optics();
				~Optics();
		};
		
		
		class Perspective : public Element
		{
			public:
				
				Perspective();
				~Perspective();
				
				C4::Camera *BuildCamera(void) const;
		};
		
		
		class Root : public Element
		{
			private:
				
				const UpAxis	*upAxisElement;
				const Unit		*unitElement;
				float			sceneScale;
			
			public:
				
				Root();
				~Root();
				
				const UpAxis *GetUpAxisElement(void) const
				{
					return (upAxisElement);
				}
				
				void SetUpAxisElement(const UpAxis *upAxis)
				{
					upAxisElement = upAxis;
				}
				
				AxisType GetUpAxisType(void) const
				{
					return ((upAxisElement) ? upAxisElement->GetUpAxisType() : kAxisY);
				}
				
				const Unit *GetUnitElement(void) const
				{
					return (unitElement);
				}
				
				void SetUnitElement(const Unit *unit)
				{
					unitElement = unit;
				}
				
				float GetSceneScale(void) const
				{
					return ((unitElement) ? unitElement->GetMeterScale() * sceneScale : sceneScale);
				}
				
				void SetSceneScale(float scale)
				{
					sceneScale = scale;
				}
				
				Collada::Scene *FindSceneElement(void) const;
				const Collada::Library *FindAnimationElement(void) const;
		};
	}
	
	
	class ColladaResource : public Resource<ColladaResource>
	{
		friend class Resource<ColladaResource>;
		
		private:
			
			static ResourceDescriptor	descriptor;
			
			~ColladaResource();
			
			void Preprocess(void);
		
		public:
			
			ColladaResource(const char *name, ResourceCatalog *catalog);
			
			const char *GetText(void) const
			{
				return (static_cast<const char *>(GetData()));
			}
	};
	
	
	class ColladaContext
	{
		private:
			
			const EditorObject		*editorObject;
			
			unsigned_int32			importFlags;
			float					importScale;
			
			Collada::Element *ConstructElement(const String<Collada::kMaxIdentifierLength>& elementType);
			bool ProcessAttributes(const char *& text, Collada::Element *element);
			bool ProcessContent(const char *& text, Collada::Element *element);
			Collada::Element *ProcessElement(const char *& text);
		
		public:
			
			ColladaContext(const EditorObject *object, unsigned_int32 flags = 0, float scale = 1.0F);
			~ColladaContext();
			
			const EditorObject *GetEditorObject(void) const
			{
				return (editorObject);
			}
			
			unsigned_int32 GetImportFlags(void) const
			{
				return (importFlags);
			}
			
			float GetImportScale(void) const
			{
				return (importScale);
			}
			
			C4::Attribute *NewTextureMapAttribute(Collada::ParamType type, const char *name, const Collada::Sampler2D *sampler);
			
			Collada::Root *ProcessFile(const char *text);
	};
	
	
	class ColladaWindow : public Window
	{
		private:
			
			Editor				*worldEditor;
			ResourceName		resourceName;
			
			PushButtonWidget	*okayButton;
			PushButtonWidget	*cancelButton;
			
			CheckWidget			*importTexturesBox;
			CheckWidget			*mergeMaterialsBox;
			CheckWidget			*reuseNamedMaterialsBox;
			
			EditTextWidget		*scaleBox;
		
		public:
			
			ColladaWindow(Editor *editor, const char *name);
			~ColladaWindow();
			
			void Preprocess(void);
			
			bool HandleKeyboardEvent(const KeyboardEventData *eventData);
			void HandleWidgetEvent(Widget *widget, const WidgetEventData *eventData);
	};
	
	
	class ColladaImporter : public SceneImportPlugin, public Singleton<ColladaImporter>
	{
		private:
			
			enum
			{
				kMaxAnimationBucketCount = 32
			};
			
			struct AnimationNode
			{
				const C4::Node			*modelNode;
				const Collada::Node		*importNode;
				unsigned_int32			hashValue;
				
				AnimationNode(const C4::Node *model, const Collada::Node *import, unsigned_int32 hash)
				{
					modelNode = model;
					importNode = import;
					hashValue = hash;
				}
			};
			
			struct AnimationHash
			{
				unsigned_int32			hashValue;
				int32					nodeIndex;
				
				AnimationHash(unsigned_int32 hash, int32 index)
				{
					hashValue = hash;
					nodeIndex = index;
				}
			};
			
			bool WriteAnimationResource(const char *name, const char *buffer, unsigned_int32 size);
			bool GenerateAnimation(const char *name, const Model *model, const AnimationImportData *importData, Collada::Scene *scene, const Collada::Library *animation);
		
		public:
			
			ColladaImporter();
			~ColladaImporter();
			
			const char *GetPluginName(void) const;
			const ResourceDescriptor *GetImportResourceDescriptor(SceneImportType type) const;
			
			void ImportGeometry(Editor *editor, const char *name, const GeometryImportData *importData);
			bool ImportAnimation(const char *name, Model *model, const AnimationImportData *importData);
	};
	
	
	extern ColladaImporter *TheColladaImporter;
}


#endif

// ZYURVUR
