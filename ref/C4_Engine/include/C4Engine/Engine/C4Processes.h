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


#ifndef C4Processes_h
#define C4Processes_h


//# \component	Graphics Manager
//# \prefix		GraphicsMgr/


#include "C4Renderable.h"


namespace C4
{
	#if C4OPENGL
	
		#define TEX2D			"texture2D"
		#define TEX3D			"texture3D"
		#define TEXRECT			"texture2DRect"
		#define TEXCUBE			"textureCube"
	
	#else
	
		#define TEX2D			"tex2D"
		#define TEX3D			"tex3D"
		#define TEXRECT			"texRECT"
		#define TEXCUBE			"texCUBE"
	
	#endif
	
	
	typedef Type	ProcessType;
	typedef Type	ProcessGroup;
	
	
	enum
	{
		kProcessSection						= 'SECT'
	};
	
	
	enum
	{
		kProcessConstant					= 'CNST',
		kProcessScalar						= 'SCLR',
		kProcessVector						= 'VCTR',
		kProcessColor						= 'COLR'
	};
	
	
	enum
	{
		kProcessParameter					= 'PARM',
		kProcessVertexColor					= 'VCOL',
		kProcessFragmentPosition			= 'FPOS',
		kProcessDetailLevel					= 'DETL',
		kProcessTime						= 'TIME'
	};
	
	
	enum
	{
		kProcessTextureMap					= 'TEXM',
		kProcessNormalMap					= 'NRMM',
		kProcessImpostorTexture				= 'ITEX',
		kProcessImpostorNormal				= 'INRM',
		kProcessTerrainTexture				= 'TTEX',
		kProcessTerrainNormal				= 'TNRM',
		kProcessTerrainNormal2				= 'TNM2',
		kProcessTerrainNormal3				= 'TNM3',
		kProcessPaintTexture				= 'PTEX',
		kProcessMerge2						= 'MRG2',
		kProcessMerge3						= 'MRG3',
		kProcessMerge4						= 'MRG4'
	};
	
	
	enum
	{
		kProcessInterpolant					= 'TERP',
		kProcessDerived						= 'DERV',
		kProcessTexcoord0					= 'TEX0',
		kProcessTexcoord1					= 'TEX1',
		kProcessRawTexcoord					= 'RTXC',
		kProcessImpostorTexcoord			= 'IMPT',
		kProcessImpostorBlend				= 'IBLD',
		kProcessImpostorDepth				= 'IDEP',
		kProcessTerrainTexcoord				= 'TERA',
		kProcessTriplanarBlend				= 'TBLD',
		kProcessPaintTexcoord				= 'PTXC',
		kProcessVertexGeometry				= 'GEOM',
		kProcessObjectPosition				= 'POSI',
		kProcessWorldPosition				= 'WPOS',
		kProcessObjectNormal				= 'NRML',
		kProcessObjectTangent				= 'TANG',
		kProcessObjectBitangent				= 'BTNG',
		kProcessWorldNormal					= 'WNRM',
		kProcessWorldTangent				= 'WTAN',
		kProcessWorldBitangent				= 'WBTN',
		kProcessTangentLightDirection		= 'LDIR',
		kProcessTangentViewDirection		= 'VDIR',
		kProcessTangentHalfwayDirection		= 'HDIR',
		kProcessObjectLightDirection		= 'OLDR', 
		kProcessObjectViewDirection			= 'OVDR',
		kProcessObjectHalfwayDirection		= 'OHDR', 
		kProcessTerrainLightDirection		= 'TLDR', 
		kProcessTerrainViewDirection		= 'TVDR', 
		kProcessTerrainHalfwayDirection		= 'THDR'
	}; 
	
	
	enum
	{	 
		kProcessAbsolute					= 'ABS ',
		kProcessAdd							= 'ADD ',
		kProcessSubtract					= 'SUB ',
		kProcessAverage						= 'AVG ', 
		kProcessInvert						= 'INV ',
		kProcessExpand						= 'VEX ',
		kProcessMultiply					= 'MUL ',
		kProcessMultiplyAdd					= 'MAD ',
		kProcessLerp						= 'LRP ',
		kProcessDivide						= 'DIV ',
		kProcessDot3						= 'DP3 ',
		kProcessDot4						= 'DP4 ',
		kProcessCross						= 'XPD ',
		kProcessReciprocal					= 'RCP ',
		kProcessReciprocalSquareRoot		= 'RSQ ',
		kProcessSquareRoot					= 'SQR ',
		kProcessMagnitude					= 'MAG ',
		kProcessNormalize					= 'NRM ',
		kProcessFloor						= 'FLR ',
		kProcessRound						= 'RND ',
		kProcessFraction					= 'FRC ',
		kProcessSaturate					= 'SAT ',
		kProcessMinimum						= 'MIN ',
		kProcessMaximum						= 'MAX ',
		kProcessSetLessThan					= 'SLT ',
		kProcessSetGreaterEqual				= 'SGE ',
		kProcessSine						= 'SIN ',
		kProcessCosine						= 'COS ',
		kProcessExp2						= 'EX2 ',
		kProcessLog2						= 'LG2 ',
		kProcessPower						= 'POW '
	};
	
	
	enum
	{
		kProcessDiffuse						= 'DIFF',
		kProcessSpecular					= 'SPEC',
		kProcessMicrofacet					= 'MCFT',
		kProcessTerrainDiffuse				= 'TDIF',
		kProcessTerrainSpecular				= 'TSPC',
		kProcessGenerateImpostorNormal		= 'GINM',
		kProcessCombineNormals				= 'CNRM',
		kProcessFrontNormal					= 'FNRM',
		kProcessReflectVector				= 'RVEC',
		kProcessLinearRamp					= 'RAMP',
		kProcessSmoothParameter				= 'SMTH',
		kProcessSteepParameter				= 'STEP',
		kProcessWorldTransform				= 'WXFM',
		kProcessDeltaDepth					= 'DLTA',
		kProcessParallax					= 'PLAX',
		kProcessHorizon						= 'HRZN',
		kProcessFire						= 'FIRE',
		kProcessDistortion					= 'DSTN'
	};
	
	
	enum
	{
		kProcessTerminal					= 'TERM',
		kProcessKill						= 'KILL',
		kProcessImpostorTransition			= 'IMPX',
		kProcessGeometryTransition			= 'GEOX'
	};
	
	
	enum
	{
		kProcessFrameBuffer					= 'FBUF'
	};
	
	
	enum
	{
		kProcessOutput						= 'OUTP',
		kProcessNullOutput					= 'NULO',
		kProcessAddOutput					= 'ADDO',
		kProcessAmbientOutput				= 'AMBT',
		kProcessAmbientAlphaOutput			= 'ALFA',
		kProcessAmbientOcclusionOutput		= 'OCCL',
		kProcessEmissionOutput				= 'EMIS',
		kProcessReflectionOutput			= 'REFL',
		kProcessRefractionOutput			= 'REFR',
		kProcessEnvironmentOutput			= 'ENVR',
		kProcessTerrainEnvironmentOutput	= 'TENV',
		kProcessGlowOutput					= 'GLOW',
		kProcessImpostorDepthOutput			= 'IMPZ',
		kProcessLightOutput					= 'LITE',
		kProcessBloomOutput					= 'BLOM',
		kProcessAlphaTestOutput				= 'ATST',
		kProcessStructureOutput				= 'STRC'
	};
	
	
	enum
	{
		kProcessConstantFog					= 'CFOG',
		kProcessLinearFog					= 'LFOG',
		kProcessAmbientFog					= 'AMBF',
		kProcessLightFog					= 'LITF',
		kProcessAlphaFog					= 'ALPF'
	};
	
		
	enum
	{
		kProcessColorPost					= 'PCOL',
		kProcessDistortPost					= 'PDST',
		kProcessMotionBlurPost				= 'PMBL',
		kProcessExtractPost					= 'PEXT',
		kProcessGlowPost					= 'PGLW',
		kProcessTransformPost				= 'PXFM'
	};
	
	
	//# \enum	RouteFlags
	
	enum
	{
		kRouteHighDetail					= 1 << 0		//## The route only exists at the highest detail level.
	};
	
	
	//# \enum	ProcessFlags
	
	enum
	{
		kProcessHighDetail					= 1 << 0,		//## The process only functions at the highest detail level. This only affects certain processes.
		kProcessLowDetail					= 1 << 1		//## The process only functions at the lowest detail level. This only affects certain processes.
	};
	
	
	//# \enum	ProcessPortFlags
	
	enum
	{
		kProcessPortOptional				= 1 << 0,		//## The input to the port is optional for all levels of detail.
		kProcessPortOmissible				= 1 << 1		//## The input to the port must exist at the highest detail, but can be omitted at lower detail levels.
	};
	
	
	enum
	{
		kShaderSourcePrimaryColor			= 1 << 0,
		kShaderSourceSecondaryColor			= 1 << 1
	};
	
	
	#if C4PLAYSTATION3
	
		enum
		{
			kProcessDependentTexture		= 1 << 0
		};
	
	#endif
	
	
	enum
	{
		kShaderGraphAmbient,
		kShaderGraphLight,
		kShaderGraphCount
	};
	
	
	enum
	{
		kMaxProcessPortCount				= 4,
		kProcessPortHiddenDependency		= kMaxProcessPortCount,
		
		kMaxProcessLiteralCount				= 2,
		kMaxShaderLiteralCount				= 16,
		
		kMaxProcessInterpolantCount			= 6,
		kMaxProcessTextureCount				= 4,
		kMaxProcessCodeCount				= 5
	};
	
	
	class Route;
	class Process;
	
	
	typedef Graph<Process, Route> ShaderGraph;
	
	
	struct SwizzleData
	{
		char				size;
		bool				negate;
		bool				absolute;
		unsigned_int8		component[4];
	};
	
	
	struct LiteralData
	{
		Type				literalType;
		float				literalValue;
	};
	
	
	struct InterpolantData
	{
		Type				interpolantType;
		int32				texcoordIndex;
		SwizzleData			swizzleData;
	};
	
	
	struct ProcessData
	{
		int32							registerCount;
		int32							preregisterCount;
		int32							temporaryCount;
		int32							literalCount;
		int32							interpolantCount;
		int32							textureCount;
		int32							passthruPort;
		
		int32							outputSize;
		int32							inputSize[kMaxProcessPortCount];
		
		LiteralData						literalData[kMaxProcessLiteralCount];
		Type							interpolantType[kMaxProcessInterpolantCount];
		const Render::TextureObject		*textureObject[kMaxProcessTextureCount];
		
		int32							outputRegister;
		mutable int32					outputCount;
		
		unsigned_int8					textureUnit[kMaxProcessTextureCount];
	};
	
	
	struct ShaderAllocationData
	{
		int32					maxRegister;
		int32					temporaryCount;
		int32					vdirCount;
		
		#if C4PLAYSTATION3
		
			unsigned_int32		dependentTextureMask;
		
		#endif
		
		int32					literalCount;
		LiteralData				literalData[kMaxShaderLiteralCount];
		
		int32					interpolantCount;
		InterpolantData			interpolantData[kMaxShaderInterpolantCount];
	};
	
	
	struct ShaderCompileData
	{
		const Renderable		*renderable;
		const RenderSegment		*renderSegment;
		ShaderData				*shaderData;
		
		ShaderType				shaderType;
		ShaderVariant			shaderVariant;
		int32					detailLevel;
		
		mutable bool			programFlag;
		mutable unsigned_int32	shaderSourceFlags;
	};
	
	
	class C4_API ProcessRegistration : public Registration<Process, ProcessRegistration>
	{
		private:
			
			const char		*processName;
			ProcessGroup	processGroup;
		
		protected:
			
			ProcessRegistration(ProcessType type, const char *name, ProcessGroup group = 0);
		
		public:
			
			~ProcessRegistration();
			
			ProcessType GetProcessType(void) const
			{
				return (GetRegistrableType());
			}
			
			const char *GetProcessName(void) const
			{
				return (processName);
			}
			
			ProcessGroup GetProcessGroup(void) const
			{
				return (processGroup);
			}
			
			virtual bool ValidShader(int32 shader) const = 0;
	};
	
	
	template <class classType> class ProcessReg : public ProcessRegistration
	{
		public:
			
			ProcessReg(ProcessType type, const char *name, ProcessGroup group = 0) : ProcessRegistration(type, name, group)
			{
			}
			
			Process *Construct(void) const
			{
				return (new classType);
			}
			
			bool ValidShader(int32 shader) const
			{
				return (classType::ValidShader(GetProcessType(), shader));
			}
	};
	
	
	//# \class	Route	Represents the data flow from one process to another in a shader graph.
	//
	//# The $Route$ class represents the data flow from one process to another in a shader graph.
	//
	//# \def	class Route : public GraphEdge<Process, Route>, public Packable, public Configurable, public Memory<Process>
	//
	//# \ctor	Route(Process *start, Process *finish, int32 port);
	//
	//# \param	start		A pointer to the process for which the route is an output.
	//# \param	finish		A pointer to the process for which the route is an input.
	//# \param	port		The index of the input port to which the route is connected.
	//
	//# \desc
	//# A $Route$ object is an edge in a shader graph that connects two processes. It represents the data flow from
	//# the output of one process to a specific input port of another process.
	//#
	//# When a route is initially constructed, the output process from which the route originates must be specified by
	//# the $start$ parameter, and the input process to which it is connected must be specified by the $finish$ parameter.
	//# The $port$ parameter specifies which port of the input process the route is connected to, and it must be in the
	//# range [0,&nbsp;<i>n</i>&nbsp;&minus;&nbsp;1], where <i>n</i> is the number of ports belonging to the input process.
	//#
	//# A route carries information about how data is modified before it is sent to the destination process.
	//# The data can be negated, and it can have a swizzle applied to it.
	//
	//# \base	Utilities/GraphEdge<Process, Route>		A route is an edge in a graph.
	//# \base	ResourceMgr/Packable					Routes can be packed for storage in resources.
	//# \base	InterfaceMgr/Configurable				Routes can be configured by the user in the Shader Editor.
	//# \base	MemoryMgr/Memory<Process>				Components of a shader graph are stored in a dedicated heap.
	//
	//# \also	$@Process@$
	
	
	//# \function	Route::GetRouteFlags		Returns the route flags.
	//
	//# \proto	unsigned_int32 GetRouteFlags(void) const;
	//
	//# \desc
	//# The $GetRouteFlags$ function returns the flags for a route, which can be a combination (through logical OR)
	//# of the following values.
	//
	//# \table	RouteFlags
	//
	//# The route flags are initially set to 0.
	//
	//# \also	$@Route::SetRouteFlags@$
	
	
	//# \function	Route::SetRouteFlags		Sets the route flags.
	//
	//# \proto	void SetRouteFlags(unsigned_int32 flags);
	//
	//# \param	flags	The new route flags.
	//
	//# \desc
	//# The $SetRouteFlags$ function sets the flags for a route, which can be a combination (through logical OR)
	//# of the following values.
	//
	//# \table	RouteFlags
	//
	//# The route flags are initially set to 0.
	//
	//# \also	$@Route::GetRouteFlags@$
	
	
	//# \function	Route::GetRouteNegation		Returns the route negation flag.
	//
	//# \proto	bool GetRouteNegation(void) const;
	//
	//# \desc
	//# The $GetRouteNegation$ function returns a boolean value indicating whether the route negates its data.
	//# If the return value is $true$, then the data is negated.
	//#
	//# Initially, a route does not negate its data.
	//
	//# \also	$@Route::SetRouteNegation@$
	//# \also	$@Route::GetRouteSwizzle@$
	//# \also	$@Route::SetRouteSwizzle@$
	
	
	//# \function	Route::SetRouteNegation		Sets the route negation flag.
	//
	//# \proto	void SetRouteNegation(bool negation);
	//
	//# \param	negation	The new negation flag.
	//
	//# \desc
	//# The $SetRouteNegation$ function specifies whether the route negates its data.
	//# If the value of the $negation$ parameter is $true$, then the data is negated.
	//#
	//# Initially, a route does not negate its data.
	//
	//# \also	$@Route::GetRouteNegation@$
	//# \also	$@Route::GetRouteSwizzle@$
	//# \also	$@Route::SetRouteSwizzle@$
	
	
	//# \function	Route::GetRouteSwizzle		Returns the route swizzle code.
	//
	//# \proto	unsigned_int32 GetRouteSwizzle(void) const;
	//
	//# \desc
	//# The $GetRouteSwizzle$ function returns a four-character code containing the route swizzle code.
	//# The swizzle code always consists of four lower-case letters that can be a combination of letters
	//# in the set {x, y, z, w, r, g, b, a}.
	//#
	//# Initially, the route swizzle is $'xyzw'$.
	//
	//# \also	$@Route::SetRouteSwizzle@$
	//# \also	$@Route::GetRouteNegation@$
	//# \also	$@Route::SetRouteNegation@$
	
	
	//# \function	Route::SetRouteSwizzle		Returns the route swizzle code.
	//
	//# \proto	void SetRouteSwizzle(unsigned_int32 swizzle);
	//
	//# \param	swizzle		The new swizzle code, consisting of four lower-case letters.
	//
	//# \desc
	//# The $SetRouteSwizzle$ function sets the route swizzle code to that specified by the $swizzle$ parameter.
	//# The swizzle code must consists of four lower-case letters that is a combination of letters
	//# in the set {x, y, z, w, r, g, b, a}.
	//#
	//# Initially, the route swizzle is $'xyzw'$.
	//
	//# \also	$@Route::SetRouteSwizzle@$
	//# \also	$@Route::GetRouteNegation@$
	//# \also	$@Route::SetRouteNegation@$
	
	
	class C4_API Route : public GraphEdge<Process, Route>, public Packable, public Configurable, public Memory<Process>
	{
		friend class ShaderAttribute;
		
		private:
			
			unsigned_int32		routeFlags;
			int32				routePort;
			
			bool				routeNegation;
			unsigned_int32		routeSwizzle;
			
			static const unsigned_int8 swizzleTable[26];
			
			static bool SwizzleFilter(unsigned_int32 code);
		
		public:
			
			Route(Process *start, Process *finish, int32 port = kProcessPortHiddenDependency);
			Route(const Route& route, Process *start, Process *finish);
			~Route();
			
			static char GetSwizzleChar(int32 c)
			{
				return ((char) ('w' + ((c + 1) & 3)));
			}
			
			unsigned_int32 GetRouteFlags(void) const
			{
				return (routeFlags);
			}
			
			void SetRouteFlags(unsigned_int32 flags)
			{
				routeFlags = flags;
			}
			
			int32 GetRoutePort(void) const
			{
				return (routePort);
			}
			
			void SetRoutePort(int32 port)
			{
				routePort = port;
			}
			
			bool GetRouteNegation(void) const
			{
				return (routeNegation);
			}
			
			void SetRouteNegation(bool negation)
			{
				routeNegation = negation;
			}
			
			unsigned_int32 GetRouteSwizzle(void) const
			{
				return (routeSwizzle);
			}
			
			void SetRouteSwizzle(unsigned_int32 swizzle)
			{
				routeSwizzle = swizzle;
			}
			
			void Pack(Packer& data, unsigned_int32 packFlags) const;
			void Unpack(Unpacker& data, unsigned_int32 unpackFlags);
			bool UnpackChunk(const ChunkHeader *chunkHeader, Unpacker& data, unsigned_int32 unpackFlags);
			
			int32 GetSettingCount(void) const;
			Setting *GetSetting(int32 index) const;
			void SetSetting(const Setting *setting);
			
			bool operator ==(const Route& route) const;
			
			int32 GenerateOutputSize(void) const;
			int32 GenerateOutputIdentifier(const ShaderCompileData *compileData, const ShaderAllocationData *allocData, SwizzleData *swizzleData, char *name) const;
			unsigned_int32 GenerateRouteSignature(void) const;
	};
	
	
	//# \class	Process		Represents an operation performed on data in a shader graph.
	//
	//# The $Process$ class represents an operation performed on data in a shader graph.
	//
	//# \def	class Process : public GraphElement<Process, Route>, public ListElement<Process>, public Packable,
	//# \def2	public Configurable, public Registrable<Process, ProcessRegistration>, public Memory<Process>
	//
	//# \ctor	Process(ProcessType type);
	//
	//# \param	type	The type of the process.
	//
	//# \desc
	//# A $Process$ object is a node in a shader graph that represents some kind of operation or data source.
	//# Each type of process has between 0 and 4 input ports through which data can be received from other processes.
	//# Most types of processes generate an output value that can be sent to the input ports of other processes.
	//
	//# \base	Utilities/GraphElement<Process, Route>				A process is an element in a graph.
	//# \base	Utilities/ListElement<Process>						Used internally by the Graphics Manager.
	//# \base	ResourceMgr/Packable								Processes can be packed for storage in resources.
	//# \base	InterfaceMgr/Configurable							Processes can be configured by the user in the Shader Editor.
	//# \base	System/Registrable<Process, ProcessRegistration>	Process types are registered for display in the Shader Editor.
	//# \base	MemoryMgr/Memory<Process>							Components of a shader graph are stored in a dedicated heap.
	//
	//# \also	$@Route@$
	//
	//# \wiki	Shader_Editor					Shader Editor
	//# \wiki	Basic_Shader_Processes			List of Basic Shader Processes
	//# \wiki	Mathematical_Shader_Processes	List of Mathematical Shader Processes
	//# \wiki	Complex_Shader_Processes		List of Complex Shader Processes
	//# \wiki	Interpolant_Shader_Processes	List of Interpolant Shader Processes
	
	
	//# \function	Process::GetProcessType		Returns the type of a process.
	//
	//# \proto	ProcessType GetProcessType(void) const;
	//
	//# \desc
	//# The $GetProcessType$ function returns the type of a process.
	
	
	//# \function	Process::GetProcessFlags		Returns the process flags.
	//
	//# \proto	unsigned_int32 GetProcessFlags(void) const;
	//
	//# \desc
	//# The $GetProcessFlags$ function returns the flags for a process, which can be a combination (through logical OR)
	//# of the following values.
	//
	//# \table	ProcessFlags
	//
	//# The process flags are initially set to 0.
	//
	//# \also	$@Process::SetProcessFlags@$
	
	
	//# \function	Process::SetProcessFlags		Sets the process flags.
	//
	//# \proto	void SetProcessFlags(unsigned_int32 flags);
	//
	//# \param	flags	The new process flags.
	//
	//# \desc
	//# The $SetProcessFlags$ function sets the flags for a process, which can be a combination (through logical OR)
	//# of the following values.
	//
	//# \table	ProcessFlags
	//
	//# The process flags are initially set to 0.
	//
	//# \also	$@Process::GetProcessFlags@$
	
	
	//# \function	Process::GetPortRoute		Returns the route connected to an input port.
	//
	//# \proto	Route *GetPortRoute(int32 port) const;
	//
	//# \param	port	The index of the input port.
	//
	//# \desc
	//# The $GetPortRoute$ function returns a pointer to the route connected to the input port having the
	//# index specified by the $port$ parameter. If the input port has no route connected to it, or the
	//# process does not have an input port with the given index, then the return value is $nullptr$.
	//
	//# \also	$@Process::GetPortCount@$
	//# \also	$@Process::GetPortFlags@$
	
	
	//# \function	Process::GetPortCount		Returns the number of input ports for a process.
	//
	//# \proto	virtual int32 GetPortCount(void) const;
	//
	//# \desc
	//# The $GetPortCount$ function returns the number of input ports possessed by a process.
	//# The return value can be between 0 and 4, inclusive.
	//
	//# \also	$@Process::GetPortRoute@$
	//# \also	$@Process::GetPortFlags@$
	
	
	//# \function	Process::GetPortFlags		Returns the port flags for a specific input port.
	//
	//# \proto	virtual unsigned_int32 GetPortFlags(int32 index) const;
	//
	//# \param	index	The port index.
	//
	//# \desc
	//# The $GetPortFlags$ function returns the flags for the input port having the index specified
	//# by the $index$ parameter. The flags can be a combination (through logical OR) of the
	//# following values.
	//
	//# \table	ProcessPortFlags
	//
	//# \also	$@Process::GetPortRoute@$
	//# \also	$@Process::GetPortCount@$
	
	
	class C4_API Process : public GraphElement<Process, Route>, public ListElement<Process>, public Packable, public Configurable, public Registrable<Process, ProcessRegistration>, public Memory<Process>
	{
		friend class ShaderAttribute;
		
		private:
			
			ProcessType			processType;
			ProcessType			baseProcessType;
			
			unsigned_int32		processFlags;
			
			union
			{
				mutable int32				processIndex;
				mutable Process				*cloneProcess;
				mutable const ProcessData	*processData;
			};
			
			Point2D				processPosition;
			String<>			processComment;
			
			int16				readyCount;
			int16				pathLength;
			
			#if C4PLAYSTATION3
			
				unsigned_int32	compileFlags;
			
			#endif
			
			virtual Process *Replicate(void) const = 0;
		
		protected:
			
			Process(ProcessType type);
			Process(const Process& process);
			
			void SetBaseProcessType(ProcessType type)
			{
				baseProcessType = type;
			}
			
			static int32 PregenerateOutputIdentifier(const SwizzleData *swizzleData, char *name);
			static int32 PostgenerateOutputIdentifier(const ShaderCompileData *compileData, const SwizzleData *swizzleData, char *name);
		
		public:
			
			virtual ~Process();
			
			using ListElement<Process>::Previous;
			using ListElement<Process>::Next;
			
			ProcessType GetProcessType(void) const
			{
				return (processType);
			}
			
			ProcessType GetBaseProcessType(void) const
			{
				return (baseProcessType);
			}
			
			unsigned_int32 GetProcessFlags(void) const
			{
				return (processFlags);
			}
			
			void SetProcessFlags(unsigned_int32 flags)
			{
				processFlags = flags;
			}
			
			int32 GetProcessIndex(void) const
			{
				return (processIndex);
			}
			
			Process *GetCloneProcess(void) const
			{
				return (cloneProcess);
			}
			
			void SetCloneProcess(Process *process)
			{
				cloneProcess = process;
			}
			
			const ProcessData *GetProcessData(void) const
			{
				return (processData);
			}
			
			const Point2D& GetProcessPosition(void) const
			{
				return (processPosition);
			}
			
			void SetProcessPosition(const Point2D& position)
			{
				processPosition = position;
			}
			
			const char *GetProcessComment(void) const
			{
				return (processComment);
			}
			
			void SetProcessComment(const char *comment)
			{
				processComment = comment;
			}
			
			Process *Clone(void) const
			{
				return (Replicate());
			}
			
			static Process *New(ProcessType type);
			
			static bool ValidShader(ProcessType type, int32 shader);
			static void RegisterStandardProcesses(void);
			
			void PackType(Packer& data) const;
			void Pack(Packer& data, unsigned_int32 packFlags) const;
			void Unpack(Unpacker& data, unsigned_int32 unpackFlags);
			bool UnpackChunk(const ChunkHeader *chunkHeader, Unpacker& data, unsigned_int32 unpackFlags);
			void *BeginSettingsUnpack(void);
			
			int32 GetSettingCount(void) const;
			Setting *GetSetting(int32 index) const;
			void SetSetting(const Setting *setting);
			
			virtual bool operator ==(const Process& process) const;
			
			Route *GetPortRoute(int32 port) const;
			virtual int32 GetPortCount(void) const;
			virtual unsigned_int32 GetPortFlags(int32 index) const;
			virtual const char *GetPortName(int32 index) const;
			
			#if C4PLAYSTATION3
			
				virtual unsigned_int32 GetPortCompileFlags(int32 index) const;
			
			#endif
			
			virtual void ReferenceStateParams(const Process *process);
			
			virtual void GenerateSourceData(const ShaderCompileData *compileData) const;
			virtual int32 GenerateProcessSignature(const ShaderCompileData *compileData, unsigned_int32 *signature) const;
			virtual int32 GenerateDerivedInterpolantTypes(const ShaderCompileData *compileData, ProcessType *type) const;
			virtual void GenerateProcessData(const ShaderCompileData *compileData, ProcessData *data) const;
			virtual int32 GenerateOutputIdentifier(const ShaderCompileData *compileData, const ShaderAllocationData *allocData, SwizzleData *swizzleData, char *name) const;
			
			virtual int32 GenerateProgramCode(const ShaderCompileData *compileData, const char **programCode) const;
			virtual int32 GenerateShaderCode(const ShaderCompileData *compileData, const char **shaderCode) const;
	};
	
	
	class C4_API SectionProcess : public Process
	{
		private:
			
			float		sectionWidth;
			float		sectionHeight;
			
			ColorRGBA	sectionColor;
			
			SectionProcess(const SectionProcess& sectionProcess);
			
			Process *Replicate(void) const override;
		
		public:
			
			SectionProcess();
			~SectionProcess();
			
			float GetSectionWidth(void) const
			{
				return (sectionWidth);
			}
			
			float GetSectionHeight(void) const
			{
				return (sectionHeight);
			}
			
			void SetSectionSize(float width, float height)
			{
				sectionWidth = width;
				sectionHeight = height;
			}
			
			const ColorRGBA& GetSectionColor(void) const
			{
				return (sectionColor);
			}
			
			void SetSectionColor(const ColorRGBA& color)
			{
				sectionColor = color;
			}
			
			void Pack(Packer& data, unsigned_int32 packFlags) const;
			void Unpack(Unpacker& data, unsigned_int32 unpackFlags);
			bool UnpackChunk(const ChunkHeader *chunkHeader, Unpacker& data, unsigned_int32 unpackFlags);
			
			int32 GetSettingCount(void) const;
			Setting *GetSetting(int32 index) const;
			void SetSetting(const Setting *setting);
	};
	
	
	class C4_API ConstantProcess : public Process
	{
		private:
			
			int32			parameterSlot;
		
		protected:
			
			const float		*parameterData;
			
			static const char *const constantIdentifier[2][kMaxShaderConstantCount];
			
			static ShaderData::ShaderStateFunc *const scalarStateFunction[kMaxShaderConstantCount];
			static ShaderData::ShaderStateFunc *const vectorStateFunction[kMaxShaderConstantCount];
			
			ConstantProcess(ProcessType type);
			ConstantProcess(const ConstantProcess& constantProcess);
			
			static void StateFunc_LoadScalar0(const Renderable *renderable, const void *cookie);
			static void StateFunc_LoadScalar1(const Renderable *renderable, const void *cookie);
			static void StateFunc_LoadScalar2(const Renderable *renderable, const void *cookie);
			static void StateFunc_LoadScalar3(const Renderable *renderable, const void *cookie);
			static void StateFunc_LoadScalar4(const Renderable *renderable, const void *cookie);
			static void StateFunc_LoadScalar5(const Renderable *renderable, const void *cookie);
			static void StateFunc_LoadScalar6(const Renderable *renderable, const void *cookie);
			static void StateFunc_LoadScalar7(const Renderable *renderable, const void *cookie);
			
			static void StateFunc_LoadVector0(const Renderable *renderable, const void *cookie);
			static void StateFunc_LoadVector1(const Renderable *renderable, const void *cookie);
			static void StateFunc_LoadVector2(const Renderable *renderable, const void *cookie);
			static void StateFunc_LoadVector3(const Renderable *renderable, const void *cookie);
			static void StateFunc_LoadVector4(const Renderable *renderable, const void *cookie);
			static void StateFunc_LoadVector5(const Renderable *renderable, const void *cookie);
			static void StateFunc_LoadVector6(const Renderable *renderable, const void *cookie);
			static void StateFunc_LoadVector7(const Renderable *renderable, const void *cookie);
		
		public:
			
			~ConstantProcess();
			
			int32 GetParameterSlot(void) const
			{
				return (parameterSlot);
			}
			
			void SetParameterSlot(int32 slot)
			{
				parameterSlot = slot;
			}
			
			void SetParameterData(const float *data)
			{
				parameterData = data;
			}
			
			virtual void SetParameterValue(const Vector4D& param) = 0;
			
			void Pack(Packer& data, unsigned_int32 packFlags) const;
			void Unpack(Unpacker& data, unsigned_int32 unpackFlags);
			
			int32 GetSettingCount(void) const;
			Setting *GetSetting(int32 index) const;
			void SetSetting(const Setting *setting);
			
			bool operator ==(const Process& process) const;
			
			int32 GenerateProcessSignature(const ShaderCompileData *compileData, unsigned_int32 *signature) const;
	};
	
	
	class C4_API ScalarProcess : public ConstantProcess
	{
		private:
			
			float		scalarValue;
			
			ScalarProcess(const ScalarProcess& scalarProcess);
			
			Process *Replicate(void) const override;
		
		public:
			
			ScalarProcess();
			~ScalarProcess();
			
			float GetScalarValue(void) const
			{
				return (scalarValue);
			}
			
			void SetScalarValue(float value)
			{
				scalarValue = value;
			}
			
			void SetParameterValue(const Vector4D& param);
			
			void Pack(Packer& data, unsigned_int32 packFlags) const;
			void Unpack(Unpacker& data, unsigned_int32 unpackFlags);
			
			int32 GetSettingCount(void) const;
			Setting *GetSetting(int32 index) const;
			void SetSetting(const Setting *setting);
			
			bool operator ==(const Process& process) const;
			
			void ReferenceStateParams(const Process *process);
			
			int32 GenerateProcessSignature(const ShaderCompileData *compileData, unsigned_int32 *signature) const;
			void GenerateProcessData(const ShaderCompileData *compileData, ProcessData *data) const;
			int32 GenerateOutputIdentifier(const ShaderCompileData *compileData, const ShaderAllocationData *allocData, SwizzleData *swizzleData, char *name) const;
	};
	
	
	class C4_API VectorProcess : public ConstantProcess
	{
		private:
			
			Vector4D	vectorValue;
			
			VectorProcess(const VectorProcess& vectorProcess);
			
			Process *Replicate(void) const override;
		
		public:
			
			VectorProcess();
			~VectorProcess();
			
			const Vector4D& GetVectorValue(void) const
			{
				return (vectorValue);
			}
			
			void SetVectorValue(const Vector4D& value)
			{
				vectorValue = value;
			}
			
			void SetParameterValue(const Vector4D& param);
			
			void Pack(Packer& data, unsigned_int32 packFlags) const;
			void Unpack(Unpacker& data, unsigned_int32 unpackFlags);
			
			int32 GetSettingCount(void) const;
			Setting *GetSetting(int32 index) const;
			void SetSetting(const Setting *setting);
			
			bool operator ==(const Process& process) const;
			
			void ReferenceStateParams(const Process *process);
			
			int32 GenerateProcessSignature(const ShaderCompileData *compileData, unsigned_int32 *signature) const;
			void GenerateProcessData(const ShaderCompileData *compileData, ProcessData *data) const;
			int32 GenerateOutputIdentifier(const ShaderCompileData *compileData, const ShaderAllocationData *allocData, SwizzleData *swizzleData, char *name) const;
	};
	
	
	class C4_API ColorProcess : public ConstantProcess
	{
		private:
			
			ColorRGBA	colorValue;
			
			ColorProcess(const ColorProcess& colorProcess);
			
			Process *Replicate(void) const override;
		
		public:
			
			ColorProcess();
			~ColorProcess();
			
			const ColorRGBA& GetColorValue(void) const
			{
				return (colorValue);
			}
			
			void SetColorValue(const ColorRGBA& value)
			{
				colorValue = value;
			}
			
			void SetParameterValue(const Vector4D& param);
			
			void Pack(Packer& data, unsigned_int32 packFlags) const;
			void Unpack(Unpacker& data, unsigned_int32 unpackFlags);
			
			int32 GetSettingCount(void) const;
			Setting *GetSetting(int32 index) const;
			void SetSetting(const Setting *setting);
			
			bool operator ==(const Process& process) const;
			
			void ReferenceStateParams(const Process *process);
			
			int32 GenerateProcessSignature(const ShaderCompileData *compileData, unsigned_int32 *signature) const;
			void GenerateProcessData(const ShaderCompileData *compileData, ProcessData *data) const;
			int32 GenerateOutputIdentifier(const ShaderCompileData *compileData, const ShaderAllocationData *allocData, SwizzleData *swizzleData, char *name) const;
	};
	
	
	class C4_API TimeProcess : public Process
	{
		private:
			
			TimeProcess(const TimeProcess& timeProcess);
			
			Process *Replicate(void) const override;
		
		public:
			
			TimeProcess();
			~TimeProcess();
			
			void GenerateProcessData(const ShaderCompileData *compileData, ProcessData *data) const;
			int32 GenerateOutputIdentifier(const ShaderCompileData *compileData, const ShaderAllocationData *allocData, SwizzleData *swizzleData, char *name) const;
	};
	
	
	class C4_API DetailLevelProcess : public Process
	{
		private:
			
			DetailLevelProcess(const DetailLevelProcess& detailLevelProcess);
			
			Process *Replicate(void) const override;
			
			static void StateFunc_SetDetailLevelParam(const Renderable *renderable, const void *cookie);
		
		public:
			
			DetailLevelProcess();
			~DetailLevelProcess();
			
			void GenerateProcessData(const ShaderCompileData *compileData, ProcessData *data) const;
			int32 GenerateOutputIdentifier(const ShaderCompileData *compileData, const ShaderAllocationData *allocData, SwizzleData *swizzleData, char *name) const;
	};
	
	
	class C4_API TextureMapProcess : public Process, public ListElement<TextureMapProcess>
	{
		friend class ShaderAttribute;
		
		private:
			
			mutable Texture			*textureObject;
			ResourceName			textureName;
			
			mutable unsigned_int32	*signatureUnit;
			
			Process *Replicate(void) const override;
		
		protected:
			
			TextureMapProcess(ProcessType type);
			TextureMapProcess(const TextureMapProcess& textureMapProcess);
		
		public:
			
			TextureMapProcess();
			~TextureMapProcess();
			
			const ResourceName& GetTextureName(void) const
			{
				return (textureName);
			}
			
			void Pack(Packer& data, unsigned_int32 packFlags) const;
			void Unpack(Unpacker& data, unsigned_int32 unpackFlags);
			void *BeginSettingsUnpack(void);
			
			int32 GetSettingCount(void) const;
			Setting *GetSetting(int32 index) const;
			void SetSetting(const Setting *setting);
			
			bool operator ==(const Process& process) const;
			
			static int32 GetTexcoordSize(const Texture *texture);
			
			Texture *GetTexture(void) const;
			
			void SetTexture(const char *name);
			void SetTexture(Texture *texture);
			void SetTexture(const TextureHeader *header, const void *image = nullptr);
			
			int32 GetPortCount(void) const;
			const char *GetPortName(int32 index) const;
			
			#if C4PLAYSTATION3
			
				unsigned_int32 GetPortCompileFlags(int32 index) const;
			
			#endif
			
			#if C4OPENGL
			
				void GenerateSourceData(const ShaderCompileData *compileData) const;
			
			#endif
			
			int32 GenerateProcessSignature(const ShaderCompileData *compileData, unsigned_int32 *signature) const;
			void GenerateProcessData(const ShaderCompileData *compileData, ProcessData *data) const;
			
			int32 GenerateProgramCode(const ShaderCompileData *compileData, const char **programCode) const;
			int32 GenerateShaderCode(const ShaderCompileData *compileData, const char **shaderCode) const;
	};
	
	
	class C4_API NormalMapProcess : public TextureMapProcess
	{
		private:
			
			NormalMapProcess(const NormalMapProcess& normalMapProcess);
			
			Process *Replicate(void) const override;
		
		public:
			
			NormalMapProcess();
			~NormalMapProcess();
			
			int32 GenerateProgramCode(const ShaderCompileData *compileData, const char **programCode) const;
			int32 GenerateShaderCode(const ShaderCompileData *compileData, const char **shaderCode) const;
	};
	
	
	class C4_API ImpostorTextureProcess : public TextureMapProcess
	{
		private:
			
			Texture		*screenTextureObject;
			
			Process *Replicate(void) const override;
		
		protected:
			
			ImpostorTextureProcess(ProcessType type);
			ImpostorTextureProcess(const ImpostorTextureProcess& impostorTextureProcess);
		
		public:
			
			ImpostorTextureProcess();
			~ImpostorTextureProcess();
			
			int32 GetPortCount(void) const;
			
			int32 GenerateDerivedInterpolantTypes(const ShaderCompileData *compileData, ProcessType *type) const;
			void GenerateProcessData(const ShaderCompileData *compileData, ProcessData *data) const;
			
			int32 GenerateProgramCode(const ShaderCompileData *compileData, const char **programCode) const;
			int32 GenerateShaderCode(const ShaderCompileData *compileData, const char **shaderCode) const;
	};
	
	
	class C4_API ImpostorNormalProcess : public ImpostorTextureProcess
	{
		private:
			
			ImpostorNormalProcess(const ImpostorNormalProcess& impostorNormalProcess);
			
			Process *Replicate(void) const override;
		
		public:
			
			ImpostorNormalProcess();
			~ImpostorNormalProcess();
			
			int32 GenerateProgramCode(const ShaderCompileData *compileData, const char **programCode) const;
			int32 GenerateShaderCode(const ShaderCompileData *compileData, const char **shaderCode) const;
	};
	
	
	class C4_API TerrainTextureProcess : public TextureMapProcess
	{
		private:
			
			Process *Replicate(void) const override;
		
		protected:
			
			enum
			{
				kTerrainBlendFull,
				kTerrainBlendPrimary,
				kTerrainBlendSecondary
			};
			
			int32		blendMode;
			
			TerrainTextureProcess(ProcessType type);
			TerrainTextureProcess(const TerrainTextureProcess& terrainTextureProcess);
		
		public:
			
			TerrainTextureProcess();
			~TerrainTextureProcess();
			
			void Pack(Packer& data, unsigned_int32 packFlags) const;
			void Unpack(Unpacker& data, unsigned_int32 unpackFlags);
			
			int32 GetSettingCount(void) const;
			Setting *GetSetting(int32 index) const;
			void SetSetting(const Setting *setting);
			
			bool operator ==(const Process& process) const;
			
			int32 GetPortCount(void) const;
			
			#if C4OPENGL
			
				void GenerateSourceData(const ShaderCompileData *compileData) const;
			
			#endif
			
			int32 GenerateProcessSignature(const ShaderCompileData *compileData, unsigned_int32 *signature) const;
			int32 GenerateDerivedInterpolantTypes(const ShaderCompileData *compileData, ProcessType *type) const;
			void GenerateProcessData(const ShaderCompileData *compileData, ProcessData *data) const;
			
			int32 GenerateProgramCode(const ShaderCompileData *compileData, const char **programCode) const;
			int32 GenerateShaderCode(const ShaderCompileData *compileData, const char **shaderCode) const;
	};
	
	
	class C4_API TerrainNormalProcess : public TerrainTextureProcess
	{
		private:
			
			TerrainNormalProcess(const TerrainNormalProcess& terrainNormalProcess);
			
			Process *Replicate(void) const override;
		
		public:
			
			TerrainNormalProcess();
			~TerrainNormalProcess();
			
			void GenerateProcessData(const ShaderCompileData *compileData, ProcessData *data) const;
			
			int32 GenerateProgramCode(const ShaderCompileData *compileData, const char **programCode) const;
			int32 GenerateShaderCode(const ShaderCompileData *compileData, const char **shaderCode) const;
	};
	
	
	class C4_API TerrainNormal2Process : public TerrainTextureProcess
	{
		private:
			
			TerrainNormal2Process(const TerrainNormal2Process& terrainNormal2Process);
			
			Process *Replicate(void) const override;
		
		public:
			
			TerrainNormal2Process();
			~TerrainNormal2Process();
			
			void GenerateProcessData(const ShaderCompileData *compileData, ProcessData *data) const;
			
			int32 GenerateProgramCode(const ShaderCompileData *compileData, const char **programCode) const;
			int32 GenerateShaderCode(const ShaderCompileData *compileData, const char **shaderCode) const;
	};
	
	
	class C4_API TerrainNormal3Process : public TerrainTextureProcess
	{
		private:
			
			TerrainNormal3Process(const TerrainNormal3Process& terrainNormal3Process);
			
			Process *Replicate(void) const override;
		
		public:
			
			TerrainNormal3Process();
			~TerrainNormal3Process();
			
			void GenerateProcessData(const ShaderCompileData *compileData, ProcessData *data) const;
			
			int32 GenerateProgramCode(const ShaderCompileData *compileData, const char **programCode) const;
			int32 GenerateShaderCode(const ShaderCompileData *compileData, const char **shaderCode) const;
	};
	
	
	class C4_API PaintTextureProcess : public Process
	{
		private:
			
			PaintTextureProcess(const PaintTextureProcess& paintTextureProcess);
			
			Process *Replicate(void) const override;
		
		public:
			
			PaintTextureProcess();
			~PaintTextureProcess();
			
			int32 GetPortCount(void) const;
			const char *GetPortName(int32 index) const;
			
			#if C4PLAYSTATION3
			
				unsigned_int32 GetPortCompileFlags(int32 index) const;
			
			#endif
			
			void GenerateProcessData(const ShaderCompileData *compileData, ProcessData *data) const;
			
			int32 GenerateProgramCode(const ShaderCompileData *compileData, const char **programCode) const;
			int32 GenerateShaderCode(const ShaderCompileData *compileData, const char **shaderCode) const;
	};
	
	
	class C4_API Merge2Process : public Process
	{
		private:
			
			Merge2Process(const Merge2Process& merge2Process);
			
			Process *Replicate(void) const override;
		
		public:
			
			Merge2Process();
			~Merge2Process();
			
			int32 GetPortCount(void) const;
			const char *GetPortName(int32 index) const;
			
			void GenerateProcessData(const ShaderCompileData *compileData, ProcessData *data) const;
			
			int32 GenerateProgramCode(const ShaderCompileData *compileData, const char **programCode) const;
			int32 GenerateShaderCode(const ShaderCompileData *compileData, const char **shaderCode) const;
	};
	
	
	class C4_API Merge3Process : public Process
	{
		private:
			
			Merge3Process(const Merge3Process& merge3Process);
			
			Process *Replicate(void) const override;
		
		public:
			
			Merge3Process();
			~Merge3Process();
			
			int32 GetPortCount(void) const;
			const char *GetPortName(int32 index) const;
			
			void GenerateProcessData(const ShaderCompileData *compileData, ProcessData *data) const;
			
			int32 GenerateProgramCode(const ShaderCompileData *compileData, const char **programCode) const;
			int32 GenerateShaderCode(const ShaderCompileData *compileData, const char **shaderCode) const;
	};
	
	
	class C4_API Merge4Process : public Process
	{
		private:
			
			Merge4Process(const Merge4Process& merge4Process);
			
			Process *Replicate(void) const override;
		
		public:
			
			Merge4Process();
			~Merge4Process();
			
			int32 GetPortCount(void) const;
			const char *GetPortName(int32 index) const;
			
			void GenerateProcessData(const ShaderCompileData *compileData, ProcessData *data) const;
			
			int32 GenerateProgramCode(const ShaderCompileData *compileData, const char **programCode) const;
			int32 GenerateShaderCode(const ShaderCompileData *compileData, const char **shaderCode) const;
	};
	
	
	class C4_API VertexColorProcess : public Process
	{
		private:
			
			VertexColorProcess(const VertexColorProcess& vertexColorProcess);
			
			Process *Replicate(void) const override;
		
		public:
			
			VertexColorProcess();
			~VertexColorProcess();
			
			void GenerateSourceData(const ShaderCompileData *compileData) const;
			
			void GenerateProcessData(const ShaderCompileData *compileData, ProcessData *data) const;
			int32 GenerateOutputIdentifier(const ShaderCompileData *compileData, const ShaderAllocationData *allocData, SwizzleData *swizzleData, char *name) const;
	};
	
	
	class C4_API FragmentPositionProcess : public Process
	{
		private:
			
			FragmentPositionProcess(const FragmentPositionProcess& fragmentPositionProcess);
			
			Process *Replicate(void) const override;
		
		public:
			
			FragmentPositionProcess();
			~FragmentPositionProcess();
			
			void GenerateProcessData(const ShaderCompileData *compileData, ProcessData *data) const;
			int32 GenerateOutputIdentifier(const ShaderCompileData *compileData, const ShaderAllocationData *allocData, SwizzleData *swizzleData, char *name) const;
	};
	
	
	class C4_API InterpolantProcess : public Process
	{
		protected:
			
			InterpolantProcess(ProcessType type);
			InterpolantProcess(const InterpolantProcess& interpolantProcess);
		
		public:
			
			~InterpolantProcess();
			
			static int32 GetInterpolantSize(Type type);
			static int32 GetInterpolantName(Type type, const ShaderCompileData *compileData, const ShaderAllocationData *allocData, char *name, SwizzleData *swizzleData = nullptr);
			
			void GenerateProcessData(const ShaderCompileData *compileData, ProcessData *data) const;
			int32 GenerateOutputIdentifier(const ShaderCompileData *compileData, const ShaderAllocationData *allocData, SwizzleData *swizzleData, char *name) const;
	};
	
	
	class C4_API Texcoord0Process : public InterpolantProcess
	{
		private:
			
			Texcoord0Process(const Texcoord0Process& texcoord0Process);
			
			Process *Replicate(void) const override;
		
		public:
			
			Texcoord0Process();
			~Texcoord0Process();
	};
	
	
	class C4_API Texcoord1Process : public InterpolantProcess
	{
		private:
			
			Texcoord1Process(const Texcoord1Process& texcoord1Process);
			
			Process *Replicate(void) const override;
		
		public:
			
			Texcoord1Process();
			~Texcoord1Process();
	};
	
	
	class C4_API RawTexcoordProcess : public InterpolantProcess
	{
		private:
			
			RawTexcoordProcess(const RawTexcoordProcess& rawTexcoordProcess);
			
			Process *Replicate(void) const override;
		
		public:
			
			RawTexcoordProcess();
			~RawTexcoordProcess();
	};
	
	
	class C4_API ImpostorTexcoordProcess : public InterpolantProcess
	{
		private:
			
			ImpostorTexcoordProcess(const ImpostorTexcoordProcess& impostorTexcoordProcess);
			
			Process *Replicate(void) const override;
		
		public:
			
			ImpostorTexcoordProcess();
			~ImpostorTexcoordProcess();
	};
	
	
	class C4_API ImpostorBlendProcess : public InterpolantProcess
	{
		private:
			
			Texture		*textureObject;
			
			ImpostorBlendProcess(const ImpostorBlendProcess& impostorBlendProcess);
			
			Process *Replicate(void) const override;
		
		public:
			
			ImpostorBlendProcess();
			~ImpostorBlendProcess();
			
			void GenerateProcessData(const ShaderCompileData *compileData, ProcessData *data) const;
			
			int32 GenerateProgramCode(const ShaderCompileData *compileData, const char **programCode) const;
			int32 GenerateShaderCode(const ShaderCompileData *compileData, const char **shaderCode) const;
	};
	
	
	class C4_API TerrainTexcoordProcess : public InterpolantProcess
	{
		private:
			
			TerrainTexcoordProcess(const TerrainTexcoordProcess& terrainTexcoordProcess);
			
			Process *Replicate(void) const override;
			
			bool GetTexturePaletteSize(int32 *size) const;
		
		public:
			
			TerrainTexcoordProcess();
			~TerrainTexcoordProcess();
			
			void GenerateSourceData(const ShaderCompileData *compileData) const;
			
			int32 GenerateProcessSignature(const ShaderCompileData *compileData, unsigned_int32 *signature) const;
			void GenerateProcessData(const ShaderCompileData *compileData, ProcessData *data) const;
			
			int32 GenerateProgramCode(const ShaderCompileData *compileData, const char **programCode) const;
			int32 GenerateShaderCode(const ShaderCompileData *compileData, const char **shaderCode) const;
	};
	
	
	class C4_API TriplanarBlendProcess : public InterpolantProcess
	{
		private:
			
			TriplanarBlendProcess(const TriplanarBlendProcess& triplanarBlendProcess);
			
			Process *Replicate(void) const override;
		
		public:
			
			TriplanarBlendProcess();
			~TriplanarBlendProcess();
			
			void GenerateProcessData(const ShaderCompileData *compileData, ProcessData *data) const;
			
			int32 GenerateProgramCode(const ShaderCompileData *compileData, const char **programCode) const;
			int32 GenerateShaderCode(const ShaderCompileData *compileData, const char **shaderCode) const;
	};
	
	
	class C4_API PaintTexcoordProcess : public InterpolantProcess
	{
		private:
			
			PaintTexcoordProcess(const PaintTexcoordProcess& paintTexcoordProcess);
			
			Process *Replicate(void) const override;
		
		public:
			
			PaintTexcoordProcess();
			~PaintTexcoordProcess();
	};
	
	
	class C4_API VertexGeometryProcess : public InterpolantProcess
	{
		private:
			
			VertexGeometryProcess(const VertexGeometryProcess& vertexGeometryProcess);
			
			Process *Replicate(void) const override;
		
		public:
			
			VertexGeometryProcess();
			~VertexGeometryProcess();
	};
	
	
	class C4_API ObjectPositionProcess : public InterpolantProcess
	{
		private:
			
			ObjectPositionProcess(const ObjectPositionProcess& objectPositionProcess);
			
			Process *Replicate(void) const override;
		
		public:
			
			ObjectPositionProcess();
			~ObjectPositionProcess();
	};
	
	
	class C4_API WorldPositionProcess : public InterpolantProcess
	{
		private:
			
			WorldPositionProcess(const WorldPositionProcess& worldPositionProcess);
			
			Process *Replicate(void) const override;
		
		public:
			
			WorldPositionProcess();
			~WorldPositionProcess();
	};
	
	
	class C4_API ObjectNormalProcess : public InterpolantProcess
	{
		private:
			
			ObjectNormalProcess(const ObjectNormalProcess& objectNormalProcess);
			
			Process *Replicate(void) const override;
		
		public:
			
			ObjectNormalProcess();
			~ObjectNormalProcess();
			
			int32 GenerateOutputIdentifier(const ShaderCompileData *compileData, const ShaderAllocationData *allocData, SwizzleData *swizzleData, char *name) const;
			
			int32 GenerateProgramCode(const ShaderCompileData *compileData, const char **programCode) const;
			int32 GenerateShaderCode(const ShaderCompileData *compileData, const char **shaderCode) const;
	};
	
	
	class C4_API ObjectTangentProcess : public InterpolantProcess
	{
		private:
			
			ObjectTangentProcess(const ObjectTangentProcess& objectTangentProcess);
			
			Process *Replicate(void) const override;
		
		public:
			
			ObjectTangentProcess();
			~ObjectTangentProcess();
			
			int32 GenerateOutputIdentifier(const ShaderCompileData *compileData, const ShaderAllocationData *allocData, SwizzleData *swizzleData, char *name) const;
			
			int32 GenerateProgramCode(const ShaderCompileData *compileData, const char **programCode) const;
			int32 GenerateShaderCode(const ShaderCompileData *compileData, const char **shaderCode) const;
	};
	
	
	class C4_API ObjectBitangentProcess : public InterpolantProcess
	{
		private:
			
			ObjectBitangentProcess(const ObjectBitangentProcess& objectBitangentProcess);
			
			Process *Replicate(void) const override;
		
		public:
			
			ObjectBitangentProcess();
			~ObjectBitangentProcess();
			
			int32 GenerateOutputIdentifier(const ShaderCompileData *compileData, const ShaderAllocationData *allocData, SwizzleData *swizzleData, char *name) const;
			
			int32 GenerateProgramCode(const ShaderCompileData *compileData, const char **programCode) const;
			int32 GenerateShaderCode(const ShaderCompileData *compileData, const char **shaderCode) const;
	};
	
	
	class C4_API WorldNormalProcess : public InterpolantProcess
	{
		private:
			
			WorldNormalProcess(const WorldNormalProcess& worldNormalProcess);
			
			Process *Replicate(void) const override;
		
		public:
			
			WorldNormalProcess();
			~WorldNormalProcess();
			
			int32 GenerateOutputIdentifier(const ShaderCompileData *compileData, const ShaderAllocationData *allocData, SwizzleData *swizzleData, char *name) const;
			
			int32 GenerateProgramCode(const ShaderCompileData *compileData, const char **programCode) const;
			int32 GenerateShaderCode(const ShaderCompileData *compileData, const char **shaderCode) const;
	};
	
	
	class C4_API WorldTangentProcess : public InterpolantProcess
	{
		private:
			
			WorldTangentProcess(const WorldTangentProcess& worldTangentProcess);
			
			Process *Replicate(void) const override;
		
		public:
			
			WorldTangentProcess();
			~WorldTangentProcess();
			
			int32 GenerateOutputIdentifier(const ShaderCompileData *compileData, const ShaderAllocationData *allocData, SwizzleData *swizzleData, char *name) const;
			
			int32 GenerateProgramCode(const ShaderCompileData *compileData, const char **programCode) const;
			int32 GenerateShaderCode(const ShaderCompileData *compileData, const char **shaderCode) const;
	};
	
	
	class C4_API WorldBitangentProcess : public InterpolantProcess
	{
		private:
			
			WorldBitangentProcess(const WorldBitangentProcess& worldBitangentProcess);
			
			Process *Replicate(void) const override;
		
		public:
			
			WorldBitangentProcess();
			~WorldBitangentProcess();
			
			int32 GenerateOutputIdentifier(const ShaderCompileData *compileData, const ShaderAllocationData *allocData, SwizzleData *swizzleData, char *name) const;
			
			int32 GenerateProgramCode(const ShaderCompileData *compileData, const char **programCode) const;
			int32 GenerateShaderCode(const ShaderCompileData *compileData, const char **shaderCode) const;
	};
	
	
	class C4_API TangentLightDirectionProcess : public InterpolantProcess
	{
		private:
			
			TangentLightDirectionProcess(const TangentLightDirectionProcess& tangentLightDirectionProcess);
			
			Process *Replicate(void) const override;
		
		public:
			
			TangentLightDirectionProcess();
			~TangentLightDirectionProcess();
			
			static bool ValidShader(ProcessType type, int32 shader);
			
			int32 GenerateOutputIdentifier(const ShaderCompileData *compileData, const ShaderAllocationData *allocData, SwizzleData *swizzleData, char *name) const;
			
			int32 GenerateProgramCode(const ShaderCompileData *compileData, const char **programCode) const;
			int32 GenerateShaderCode(const ShaderCompileData *compileData, const char **shaderCode) const;
	};
	
	
	class C4_API TangentViewDirectionProcess : public InterpolantProcess
	{
		private:
			
			TangentViewDirectionProcess(const TangentViewDirectionProcess& tangentViewDirectionProcess);
			
			Process *Replicate(void) const override;
		
		public:
			
			TangentViewDirectionProcess();
			~TangentViewDirectionProcess();
			
			int32 GenerateOutputIdentifier(const ShaderCompileData *compileData, const ShaderAllocationData *allocData, SwizzleData *swizzleData, char *name) const;
			
			int32 GenerateProgramCode(const ShaderCompileData *compileData, const char **programCode) const;
			int32 GenerateShaderCode(const ShaderCompileData *compileData, const char **shaderCode) const;
	};
	
	
	class C4_API TangentHalfwayDirectionProcess : public InterpolantProcess
	{
		private:
			
			TangentHalfwayDirectionProcess(const TangentHalfwayDirectionProcess& tangentHalfwayDirectionProcess);
			
			Process *Replicate(void) const override;
		
		public:
			
			TangentHalfwayDirectionProcess();
			~TangentHalfwayDirectionProcess();
			
			static bool ValidShader(ProcessType type, int32 shader);
			
			int32 GenerateDerivedInterpolantTypes(const ShaderCompileData *compileData, ProcessType *type) const;
			void GenerateProcessData(const ShaderCompileData *compileData, ProcessData *data) const;
			int32 GenerateOutputIdentifier(const ShaderCompileData *compileData, const ShaderAllocationData *allocData, SwizzleData *swizzleData, char *name) const;
			
			int32 GenerateProgramCode(const ShaderCompileData *compileData, const char **programCode) const;
			int32 GenerateShaderCode(const ShaderCompileData *compileData, const char **shaderCode) const;
	};
	
	
	class C4_API ObjectLightDirectionProcess : public InterpolantProcess
	{
		private:
			
			ObjectLightDirectionProcess(const ObjectLightDirectionProcess& objectLightDirectionProcess);
			
			Process *Replicate(void) const override;
		
		public:
			
			ObjectLightDirectionProcess();
			~ObjectLightDirectionProcess();
			
			static bool ValidShader(ProcessType type, int32 shader);
			
			int32 GenerateOutputIdentifier(const ShaderCompileData *compileData, const ShaderAllocationData *allocData, SwizzleData *swizzleData, char *name) const;
			
			int32 GenerateProgramCode(const ShaderCompileData *compileData, const char **programCode) const;
			int32 GenerateShaderCode(const ShaderCompileData *compileData, const char **shaderCode) const;
	};
	
	
	class C4_API ObjectViewDirectionProcess : public InterpolantProcess
	{
		private:
			
			ObjectViewDirectionProcess(const ObjectViewDirectionProcess& objectViewDirectionProcess);
			
			Process *Replicate(void) const override;
		
		public:
			
			ObjectViewDirectionProcess();
			~ObjectViewDirectionProcess();
			
			int32 GenerateOutputIdentifier(const ShaderCompileData *compileData, const ShaderAllocationData *allocData, SwizzleData *swizzleData, char *name) const;
			
			int32 GenerateProgramCode(const ShaderCompileData *compileData, const char **programCode) const;
			int32 GenerateShaderCode(const ShaderCompileData *compileData, const char **shaderCode) const;
	};
	
	
	class C4_API ObjectHalfwayDirectionProcess : public InterpolantProcess
	{
		private:
			
			ObjectHalfwayDirectionProcess(const ObjectHalfwayDirectionProcess& objectHalfwayDirectionProcess);
			
			Process *Replicate(void) const override;
		
		public:
			
			ObjectHalfwayDirectionProcess();
			~ObjectHalfwayDirectionProcess();
			
			static bool ValidShader(ProcessType type, int32 shader);
			
			int32 GenerateDerivedInterpolantTypes(const ShaderCompileData *compileData, ProcessType *type) const;
			void GenerateProcessData(const ShaderCompileData *compileData, ProcessData *data) const;
			int32 GenerateOutputIdentifier(const ShaderCompileData *compileData, const ShaderAllocationData *allocData, SwizzleData *swizzleData, char *name) const;
			
			int32 GenerateProgramCode(const ShaderCompileData *compileData, const char **programCode) const;
			int32 GenerateShaderCode(const ShaderCompileData *compileData, const char **shaderCode) const;
	};
	
	
	class C4_API TerrainLightDirectionProcess : public InterpolantProcess
	{
		private:
			
			TerrainLightDirectionProcess(const TerrainLightDirectionProcess& terrainLightDirectionProcess);
			
			Process *Replicate(void) const override;
		
		public:
			
			TerrainLightDirectionProcess();
			~TerrainLightDirectionProcess();
			
			void GenerateProcessData(const ShaderCompileData *compileData, ProcessData *data) const;
			
			int32 GenerateProgramCode(const ShaderCompileData *compileData, const char **programCode) const;
			int32 GenerateShaderCode(const ShaderCompileData *compileData, const char **shaderCode) const;
	};
	
	
	class C4_API TerrainViewDirectionProcess : public InterpolantProcess
	{
		private:
			
			TerrainViewDirectionProcess(const TerrainViewDirectionProcess& terrainViewDirectionProcess);
			
			Process *Replicate(void) const override;
		
		public:
			
			TerrainViewDirectionProcess();
			~TerrainViewDirectionProcess();
			
			void GenerateProcessData(const ShaderCompileData *compileData, ProcessData *data) const;
			
			int32 GenerateProgramCode(const ShaderCompileData *compileData, const char **programCode) const;
			int32 GenerateShaderCode(const ShaderCompileData *compileData, const char **shaderCode) const;
	};
	
	
	class C4_API TerrainHalfwayDirectionProcess : public InterpolantProcess
	{
		private:
			
			TerrainHalfwayDirectionProcess(const TerrainHalfwayDirectionProcess& terrainHalfwayDirectionProcess);
			
			Process *Replicate(void) const override;
		
		public:
			
			TerrainHalfwayDirectionProcess();
			~TerrainHalfwayDirectionProcess();
			
			int32 GenerateDerivedInterpolantTypes(const ShaderCompileData *compileData, ProcessType *type) const;
			void GenerateProcessData(const ShaderCompileData *compileData, ProcessData *data) const;
			
			int32 GenerateProgramCode(const ShaderCompileData *compileData, const char **programCode) const;
			int32 GenerateShaderCode(const ShaderCompileData *compileData, const char **shaderCode) const;
	};
	
	
	class C4_API UnaryProcess : public Process
	{
		protected:
			
			UnaryProcess(ProcessType type);
			UnaryProcess(const UnaryProcess& unaryProcess);
		
		public:
			
			~UnaryProcess();
			
			int32 GetPortCount(void) const;
			const char *GetPortName(int32 index) const;
			
			void GenerateProcessData(const ShaderCompileData *compileData, ProcessData *data) const;
	};
	
	
	class C4_API BinaryProcess : public Process
	{
		protected:
			
			BinaryProcess(ProcessType type);
			BinaryProcess(const BinaryProcess& binaryProcess);
		
		public:
			
			~BinaryProcess();
			
			int32 GetPortCount(void) const;
			const char *GetPortName(int32 index) const;
			
			void GenerateProcessData(const ShaderCompileData *compileData, ProcessData *data) const;
			int32 GenerateOutputIdentifier(const ShaderCompileData *compileData, const ShaderAllocationData *allocData, SwizzleData *swizzleData, char *name) const;
	};
	
	
	class C4_API TrinaryProcess : public Process
	{
		protected:
			
			TrinaryProcess(ProcessType type);
			TrinaryProcess(const TrinaryProcess& trinaryProcess);
		
		public:
			
			~TrinaryProcess();
			
			int32 GetPortCount(void) const;
			
			void GenerateProcessData(const ShaderCompileData *compileData, ProcessData *data) const;
			int32 GenerateOutputIdentifier(const ShaderCompileData *compileData, const ShaderAllocationData *allocData, SwizzleData *swizzleData, char *name) const;
	};
	
	
	class C4_API AbsoluteProcess : public UnaryProcess
	{
		private:
			
			AbsoluteProcess(const AbsoluteProcess& absoluteProcess);
			
			Process *Replicate(void) const override;
		
		public:
			
			AbsoluteProcess();
			~AbsoluteProcess();
			
			void GenerateProcessData(const ShaderCompileData *compileData, ProcessData *data) const;
			int32 GenerateOutputIdentifier(const ShaderCompileData *compileData, const ShaderAllocationData *allocData, SwizzleData *swizzleData, char *name) const;
			
			int32 GenerateProgramCode(const ShaderCompileData *compileData, const char **programCode) const;
			int32 GenerateShaderCode(const ShaderCompileData *compileData, const char **shaderCode) const;
	};
	
	
	class C4_API InvertProcess : public UnaryProcess
	{
		private:
			
			InvertProcess(const InvertProcess& invertProcess);
			
			Process *Replicate(void) const override;
		
		public:
			
			InvertProcess();
			~InvertProcess();
			
			int32 GenerateProgramCode(const ShaderCompileData *compileData, const char **programCode) const;
			int32 GenerateShaderCode(const ShaderCompileData *compileData, const char **shaderCode) const;
	};
	
	
	class C4_API ExpandProcess : public UnaryProcess
	{
		private:
			
			ExpandProcess(const ExpandProcess& expandProcess);
			
			Process *Replicate(void) const override;
		
		public:
			
			ExpandProcess();
			~ExpandProcess();
			
			int32 GenerateProgramCode(const ShaderCompileData *compileData, const char **programCode) const;
			int32 GenerateShaderCode(const ShaderCompileData *compileData, const char **shaderCode) const;
	};
	
	
	class C4_API ReciprocalProcess : public UnaryProcess
	{
		private:
			
			ReciprocalProcess(const ReciprocalProcess& reciprocalProcess);
			
			Process *Replicate(void) const override;
		
		public:
			
			ReciprocalProcess();
			~ReciprocalProcess();
			
			void GenerateProcessData(const ShaderCompileData *compileData, ProcessData *data) const;
			
			int32 GenerateProgramCode(const ShaderCompileData *compileData, const char **programCode) const;
			int32 GenerateShaderCode(const ShaderCompileData *compileData, const char **shaderCode) const;
	};
	
	
	class C4_API ReciprocalSquareRootProcess : public UnaryProcess
	{
		private:
			
			ReciprocalSquareRootProcess(const ReciprocalSquareRootProcess& reciprocalSquareRootProcess);
			
			Process *Replicate(void) const override;
		
		public:
			
			ReciprocalSquareRootProcess();
			~ReciprocalSquareRootProcess();
			
			void GenerateProcessData(const ShaderCompileData *compileData, ProcessData *data) const;
			
			int32 GenerateProgramCode(const ShaderCompileData *compileData, const char **programCode) const;
			int32 GenerateShaderCode(const ShaderCompileData *compileData, const char **shaderCode) const;
	};
	
	
	class C4_API SquareRootProcess : public UnaryProcess
	{
		private:
			
			SquareRootProcess(const SquareRootProcess& squareRootProcess);
			
			Process *Replicate(void) const override;
		
		public:
			
			SquareRootProcess();
			~SquareRootProcess();
			
			void GenerateProcessData(const ShaderCompileData *compileData, ProcessData *data) const;
			
			int32 GenerateProgramCode(const ShaderCompileData *compileData, const char **programCode) const;
			int32 GenerateShaderCode(const ShaderCompileData *compileData, const char **shaderCode) const;
	};
	
	
	class C4_API MagnitudeProcess : public UnaryProcess
	{
		private:
			
			MagnitudeProcess(const MagnitudeProcess& magnitudeProcess);
			
			Process *Replicate(void) const override;
		
		public:
			
			MagnitudeProcess();
			~MagnitudeProcess();
			
			void GenerateProcessData(const ShaderCompileData *compileData, ProcessData *data) const;
			
			int32 GenerateProgramCode(const ShaderCompileData *compileData, const char **programCode) const;
			int32 GenerateShaderCode(const ShaderCompileData *compileData, const char **shaderCode) const;
	};
	
	
	class C4_API NormalizeProcess : public UnaryProcess
	{
		private:
			
			NormalizeProcess(const NormalizeProcess& normalizeProcess);
			
			Process *Replicate(void) const override;
		
		public:
			
			NormalizeProcess();
			~NormalizeProcess();
			
			void GenerateProcessData(const ShaderCompileData *compileData, ProcessData *data) const;
			
			int32 GenerateProgramCode(const ShaderCompileData *compileData, const char **programCode) const;
			int32 GenerateShaderCode(const ShaderCompileData *compileData, const char **shaderCode) const;
	};
	
	
	class C4_API FloorProcess : public UnaryProcess
	{
		private:
			
			FloorProcess(const FloorProcess& floorProcess);
			
			Process *Replicate(void) const override;
		
		public:
			
			FloorProcess();
			~FloorProcess();
			
			int32 GenerateProgramCode(const ShaderCompileData *compileData, const char **programCode) const;
			int32 GenerateShaderCode(const ShaderCompileData *compileData, const char **shaderCode) const;
	};
	
	
	class C4_API RoundProcess : public UnaryProcess
	{
		private:
			
			RoundProcess(const RoundProcess& roundProcess);
			
			Process *Replicate(void) const override;
		
		public:
			
			RoundProcess();
			~RoundProcess();
			
			int32 GenerateProgramCode(const ShaderCompileData *compileData, const char **programCode) const;
			int32 GenerateShaderCode(const ShaderCompileData *compileData, const char **shaderCode) const;
	};
	
	
	class C4_API FractionProcess : public UnaryProcess
	{
		private:
			
			FractionProcess(const FractionProcess& fractionProcess);
			
			Process *Replicate(void) const override;
		
		public:
			
			FractionProcess();
			~FractionProcess();
			
			int32 GenerateProgramCode(const ShaderCompileData *compileData, const char **programCode) const;
			int32 GenerateShaderCode(const ShaderCompileData *compileData, const char **shaderCode) const;
	};
	
	
	class C4_API SaturateProcess : public UnaryProcess
	{
		private:
			
			SaturateProcess(const SaturateProcess& saturateProcess);
			
			Process *Replicate(void) const override;
		
		public:
			
			SaturateProcess();
			~SaturateProcess();
			
			int32 GenerateProgramCode(const ShaderCompileData *compileData, const char **programCode) const;
			int32 GenerateShaderCode(const ShaderCompileData *compileData, const char **shaderCode) const;
	};
	
	
	class C4_API SineProcess : public UnaryProcess
	{
		private:
			
			SineProcess(const SineProcess& sineProcess);
			
			Process *Replicate(void) const override;
		
		public:
			
			SineProcess();
			~SineProcess();
			
			void GenerateProcessData(const ShaderCompileData *compileData, ProcessData *data) const;
			
			int32 GenerateProgramCode(const ShaderCompileData *compileData, const char **programCode) const;
			int32 GenerateShaderCode(const ShaderCompileData *compileData, const char **shaderCode) const;
	};
	
	
	class C4_API CosineProcess : public UnaryProcess
	{
		private:
			
			CosineProcess(const CosineProcess& cosineProcess);
			
			Process *Replicate(void) const override;
		
		public:
			
			CosineProcess();
			~CosineProcess();
			
			void GenerateProcessData(const ShaderCompileData *compileData, ProcessData *data) const;
			
			int32 GenerateProgramCode(const ShaderCompileData *compileData, const char **programCode) const;
			int32 GenerateShaderCode(const ShaderCompileData *compileData, const char **shaderCode) const;
	};
	
	
	class C4_API Exp2Process : public UnaryProcess
	{
		private:
			
			Exp2Process(const Exp2Process& exp2Process);
			
			Process *Replicate(void) const override;
		
		public:
			
			Exp2Process();
			~Exp2Process();
			
			void GenerateProcessData(const ShaderCompileData *compileData, ProcessData *data) const;
			
			int32 GenerateProgramCode(const ShaderCompileData *compileData, const char **programCode) const;
			int32 GenerateShaderCode(const ShaderCompileData *compileData, const char **shaderCode) const;
	};
	
	
	class C4_API Log2Process : public UnaryProcess
	{
		private:
			
			Log2Process(const Log2Process& log2Process);
			
			Process *Replicate(void) const override;
		
		public:
			
			Log2Process();
			~Log2Process();
			
			void GenerateProcessData(const ShaderCompileData *compileData, ProcessData *data) const;
			
			int32 GenerateProgramCode(const ShaderCompileData *compileData, const char **programCode) const;
			int32 GenerateShaderCode(const ShaderCompileData *compileData, const char **shaderCode) const;
	};
	
	
	class C4_API AddProcess : public BinaryProcess
	{
		private:
			
			AddProcess(const AddProcess& addProcess);
			
			Process *Replicate(void) const override;
		
		public:
			
			AddProcess();
			~AddProcess();
			
			unsigned_int32 GetPortFlags(int32 index) const;
			
			int32 GenerateProgramCode(const ShaderCompileData *compileData, const char **programCode) const;
			int32 GenerateShaderCode(const ShaderCompileData *compileData, const char **shaderCode) const;
	};
	
	
	class C4_API SubtractProcess : public BinaryProcess
	{
		private:
			
			SubtractProcess(const SubtractProcess& subtractProcess);
			
			Process *Replicate(void) const override;
		
		public:
			
			SubtractProcess();
			~SubtractProcess();
			
			unsigned_int32 GetPortFlags(int32 index) const;
			
			int32 GenerateProgramCode(const ShaderCompileData *compileData, const char **programCode) const;
			int32 GenerateShaderCode(const ShaderCompileData *compileData, const char **shaderCode) const;
	};
	
	
	class C4_API AverageProcess : public BinaryProcess
	{
		private:
			
			AverageProcess(const AverageProcess& averageProcess);
			
			Process *Replicate(void) const override;
		
		public:
			
			AverageProcess();
			~AverageProcess();
			
			unsigned_int32 GetPortFlags(int32 index) const;
			
			int32 GenerateProgramCode(const ShaderCompileData *compileData, const char **programCode) const;
			int32 GenerateShaderCode(const ShaderCompileData *compileData, const char **shaderCode) const;
	};
	
	
	class C4_API MultiplyProcess : public BinaryProcess
	{
		private:
			
			MultiplyProcess(const MultiplyProcess& multiplyProcess);
			
			Process *Replicate(void) const override;
		
		public:
			
			MultiplyProcess();
			~MultiplyProcess();
			
			unsigned_int32 GetPortFlags(int32 index) const;
			
			int32 GenerateProgramCode(const ShaderCompileData *compileData, const char **programCode) const;
			int32 GenerateShaderCode(const ShaderCompileData *compileData, const char **shaderCode) const;
	};
	
	
	class C4_API DivideProcess : public BinaryProcess
	{
		private:
			
			DivideProcess(const DivideProcess& divideProcess);
			
			Process *Replicate(void) const override;
		
		public:
			
			DivideProcess();
			~DivideProcess();
			
			void GenerateProcessData(const ShaderCompileData *compileData, ProcessData *data) const;
			
			int32 GenerateProgramCode(const ShaderCompileData *compileData, const char **programCode) const;
			int32 GenerateShaderCode(const ShaderCompileData *compileData, const char **shaderCode) const;
	};
	
	
	class C4_API Dot3Process : public BinaryProcess
	{
		private:
			
			Dot3Process(const Dot3Process& dot3Process);
			
			Process *Replicate(void) const override;
		
		public:
			
			Dot3Process();
			~Dot3Process();
			
			void GenerateProcessData(const ShaderCompileData *compileData, ProcessData *data) const;
			
			int32 GenerateProgramCode(const ShaderCompileData *compileData, const char **programCode) const;
			int32 GenerateShaderCode(const ShaderCompileData *compileData, const char **shaderCode) const;
	};
	
	
	class C4_API Dot4Process : public BinaryProcess
	{
		private:
			
			Dot4Process(const Dot4Process& dot3Process);
			
			Process *Replicate(void) const override;
		
		public:
			
			Dot4Process();
			~Dot4Process();
			
			void GenerateProcessData(const ShaderCompileData *compileData, ProcessData *data) const;
			
			int32 GenerateProgramCode(const ShaderCompileData *compileData, const char **programCode) const;
			int32 GenerateShaderCode(const ShaderCompileData *compileData, const char **shaderCode) const;
	};
	
	
	class C4_API CrossProcess : public BinaryProcess
	{
		private:
			
			CrossProcess(const CrossProcess& crossProcess);
			
			Process *Replicate(void) const override;
		
		public:
			
			CrossProcess();
			~CrossProcess();
			
			void GenerateProcessData(const ShaderCompileData *compileData, ProcessData *data) const;
			
			int32 GenerateProgramCode(const ShaderCompileData *compileData, const char **programCode) const;
			int32 GenerateShaderCode(const ShaderCompileData *compileData, const char **shaderCode) const;
	};
	
	
	class C4_API MinimumProcess : public BinaryProcess
	{
		private:
			
			MinimumProcess(const MinimumProcess& minimumProcess);
			
			Process *Replicate(void) const override;
		
		public:
			
			MinimumProcess();
			~MinimumProcess();
			
			unsigned_int32 GetPortFlags(int32 index) const;
			
			int32 GenerateProgramCode(const ShaderCompileData *compileData, const char **programCode) const;
			int32 GenerateShaderCode(const ShaderCompileData *compileData, const char **shaderCode) const;
	};
	
	
	class C4_API MaximumProcess : public BinaryProcess
	{
		private:
			
			MaximumProcess(const MaximumProcess& maximumProcess);
			
			Process *Replicate(void) const override;
		
		public:
			
			MaximumProcess();
			~MaximumProcess();
			
			unsigned_int32 GetPortFlags(int32 index) const;
			
			int32 GenerateProgramCode(const ShaderCompileData *compileData, const char **programCode) const;
			int32 GenerateShaderCode(const ShaderCompileData *compileData, const char **shaderCode) const;
	};
	
	
	class C4_API SetLessThanProcess : public BinaryProcess
	{
		private:
			
			SetLessThanProcess(const SetLessThanProcess& setLessThanProcess);
			
			Process *Replicate(void) const override;
		
		public:
			
			SetLessThanProcess();
			~SetLessThanProcess();
			
			unsigned_int32 GetPortFlags(int32 index) const;
			
			int32 GenerateProgramCode(const ShaderCompileData *compileData, const char **programCode) const;
			int32 GenerateShaderCode(const ShaderCompileData *compileData, const char **shaderCode) const;
	};
	
	
	class C4_API SetGreaterEqualProcess : public BinaryProcess
	{
		private:
			
			SetGreaterEqualProcess(const SetGreaterEqualProcess& setGreaterEqualProcess);
			
			Process *Replicate(void) const override;
		
		public:
			
			SetGreaterEqualProcess();
			~SetGreaterEqualProcess();
			
			unsigned_int32 GetPortFlags(int32 index) const;
			
			int32 GenerateProgramCode(const ShaderCompileData *compileData, const char **programCode) const;
			int32 GenerateShaderCode(const ShaderCompileData *compileData, const char **shaderCode) const;
	};
	
	
	class C4_API PowerProcess : public BinaryProcess
	{
		private:
			
			PowerProcess(const PowerProcess& powerProcess);
			
			Process *Replicate(void) const override;
		
		public:
			
			PowerProcess();
			~PowerProcess();
			
			void GenerateProcessData(const ShaderCompileData *compileData, ProcessData *data) const;
			
			int32 GenerateProgramCode(const ShaderCompileData *compileData, const char **programCode) const;
			int32 GenerateShaderCode(const ShaderCompileData *compileData, const char **shaderCode) const;
	};
	
	
	class C4_API MultiplyAddProcess : public TrinaryProcess
	{
		private:
			
			MultiplyAddProcess(const MultiplyAddProcess& multiplyAddProcess);
			
			Process *Replicate(void) const override;
		
		public:
			
			MultiplyAddProcess();
			~MultiplyAddProcess();
			
			unsigned_int32 GetPortFlags(int32 index) const;
			const char *GetPortName(int32 index) const;
			
			void GenerateProcessData(const ShaderCompileData *compileData, ProcessData *data) const;
			
			int32 GenerateProgramCode(const ShaderCompileData *compileData, const char **programCode) const;
			int32 GenerateShaderCode(const ShaderCompileData *compileData, const char **shaderCode) const;
	};
	
	
	class C4_API LerpProcess : public TrinaryProcess
	{
		private:
			
			LerpProcess(const LerpProcess& lerpProcess);
			
			Process *Replicate(void) const override;
		
		public:
			
			LerpProcess();
			~LerpProcess();
			
			unsigned_int32 GetPortFlags(int32 index) const;
			const char *GetPortName(int32 index) const;
			
			void GenerateProcessData(const ShaderCompileData *compileData, ProcessData *data) const;
			
			int32 GenerateProgramCode(const ShaderCompileData *compileData, const char **programCode) const;
			int32 GenerateShaderCode(const ShaderCompileData *compileData, const char **shaderCode) const;
	};
	
	
	class C4_API DiffuseProcess : public Process
	{
		private:
			
			DiffuseProcess(const DiffuseProcess& diffuseProcess);
			
			Process *Replicate(void) const override;
		
		public:
			
			DiffuseProcess();
			~DiffuseProcess();
			
			static bool ValidShader(ProcessType type, int32 shader);
			
			int32 GetPortCount(void) const;
			unsigned_int32 GetPortFlags(int32 index) const;
			const char *GetPortName(int32 index) const;
			
			int32 GenerateProcessSignature(const ShaderCompileData *compileData, unsigned_int32 *signature) const;
			int32 GenerateDerivedInterpolantTypes(const ShaderCompileData *compileData, ProcessType *type) const;
			void GenerateProcessData(const ShaderCompileData *compileData, ProcessData *data) const;
			
			int32 GenerateProgramCode(const ShaderCompileData *compileData, const char **programCode) const;
			int32 GenerateShaderCode(const ShaderCompileData *compileData, const char **shaderCode) const;
	};
	
	
	class C4_API SpecularProcess : public Process
	{
		private:
			
			SpecularProcess(const SpecularProcess& specularProcess);
			
			Process *Replicate(void) const override;
		
		public:
			
			SpecularProcess();
			~SpecularProcess();
			
			static bool ValidShader(ProcessType type, int32 shader);
			
			int32 GetPortCount(void) const;
			unsigned_int32 GetPortFlags(int32 index) const;
			const char *GetPortName(int32 index) const;
			
			int32 GenerateProcessSignature(const ShaderCompileData *compileData, unsigned_int32 *signature) const;
			int32 GenerateDerivedInterpolantTypes(const ShaderCompileData *compileData, ProcessType *type) const;
			void GenerateProcessData(const ShaderCompileData *compileData, ProcessData *data) const;
			
			int32 GenerateProgramCode(const ShaderCompileData *compileData, const char **programCode) const;
			int32 GenerateShaderCode(const ShaderCompileData *compileData, const char **shaderCode) const;
	};
	
	
	class C4_API MicrofacetProcess : public Process
	{
		private:
			
			MicrofacetAttribute::MicrofacetParams			microfacetParams;
			const MicrofacetAttribute::MicrofacetParams		*microfacetData;
			
			MicrofacetProcess(const MicrofacetProcess& microfacetProcess);
			
			Process *Replicate(void) const override;
		
		public:
			
			MicrofacetProcess();
			~MicrofacetProcess();
			
			void SetMicrofacetParams(const MicrofacetAttribute::MicrofacetParams *params)
			{
				microfacetParams.microfacetColor = params->microfacetColor;
				microfacetParams.microfacetSlope = params->microfacetSlope;
				microfacetParams.microfacetThreshold = params->microfacetThreshold;
			}
			
			void SetMicrofacetData(const MicrofacetAttribute::MicrofacetParams *data)
			{
				microfacetData = data;
			}
			
			static bool ValidShader(ProcessType type, int32 shader);
			
			void Pack(Packer& data, unsigned_int32 packFlags) const;
			void Unpack(Unpacker& data, unsigned_int32 unpackFlags);
			
			int32 GetSettingCount(void) const;
			Setting *GetSetting(int32 index) const;
			void SetSetting(const Setting *setting);
			
			bool operator ==(const Process& process) const;
			
			int32 GetPortCount(void) const;
			unsigned_int32 GetPortFlags(int32 index) const;
			const char *GetPortName(int32 index) const;
			
			void ReferenceStateParams(const Process *process);
			
			int32 GenerateProcessSignature(const ShaderCompileData *compileData, unsigned_int32 *signature) const;
			int32 GenerateDerivedInterpolantTypes(const ShaderCompileData *compileData, ProcessType *type) const;
			void GenerateProcessData(const ShaderCompileData *compileData, ProcessData *data) const;
			
			int32 GenerateProgramCode(const ShaderCompileData *compileData, const char **programCode) const;
			int32 GenerateShaderCode(const ShaderCompileData *compileData, const char **shaderCode) const;
	};
	
	
	class C4_API TerrainDiffuseProcess : public Process
	{
		private:
			
			TerrainDiffuseProcess(const TerrainDiffuseProcess& terrainDiffuseProcess);
			
			Process *Replicate(void) const override;
			
			bool BumpEnabled(void) const;
		
		public:
			
			TerrainDiffuseProcess();
			~TerrainDiffuseProcess();
			
			static bool ValidShader(ProcessType type, int32 shader);
			
			int32 GetPortCount(void) const;
			unsigned_int32 GetPortFlags(int32 index) const;
			const char *GetPortName(int32 index) const;
			
			int32 GenerateDerivedInterpolantTypes(const ShaderCompileData *compileData, ProcessType *type) const;
			void GenerateProcessData(const ShaderCompileData *compileData, ProcessData *data) const;
			
			int32 GenerateProgramCode(const ShaderCompileData *compileData, const char **programCode) const;
			int32 GenerateShaderCode(const ShaderCompileData *compileData, const char **shaderCode) const;
	};
	
	
	class C4_API TerrainSpecularProcess : public Process
	{
		private:
			
			TerrainSpecularProcess(const TerrainSpecularProcess& terrainSpecularProcess);
			
			Process *Replicate(void) const override;
			
			bool BumpEnabled(void) const;
		
		public:
			
			TerrainSpecularProcess();
			~TerrainSpecularProcess();
			
			static bool ValidShader(ProcessType type, int32 shader);
			
			int32 GetPortCount(void) const;
			unsigned_int32 GetPortFlags(int32 index) const;
			const char *GetPortName(int32 index) const;
			
			int32 GenerateDerivedInterpolantTypes(const ShaderCompileData *compileData, ProcessType *type) const;
			void GenerateProcessData(const ShaderCompileData *compileData, ProcessData *data) const;
			
			int32 GenerateProgramCode(const ShaderCompileData *compileData, const char **programCode) const;
			int32 GenerateShaderCode(const ShaderCompileData *compileData, const char **shaderCode) const;
	};
	
	
	class C4_API GenerateImpostorNormalProcess : public Process
	{
		private:
			
			GenerateImpostorNormalProcess(const GenerateImpostorNormalProcess& generateImpostorNormalProcess);
			
			Process *Replicate(void) const override;
		
		public:
			
			GenerateImpostorNormalProcess();
			~GenerateImpostorNormalProcess();
			
			static bool ValidShader(ProcessType type, int32 shader);
			
			void GenerateProcessData(const ShaderCompileData *compileData, ProcessData *data) const;
			
			int32 GenerateProgramCode(const ShaderCompileData *compileData, const char **programCode) const;
			int32 GenerateShaderCode(const ShaderCompileData *compileData, const char **shaderCode) const;
	};
	
	
	class C4_API ImpostorDepthProcess : public InterpolantProcess
	{
		private:
			
			ImpostorDepthProcess(const ImpostorDepthProcess& impostorDepthProcess);
			
			Process *Replicate(void) const override;
		
		public:
			
			ImpostorDepthProcess();
			~ImpostorDepthProcess();
			
			static bool ValidShader(ProcessType type, int32 shader);
	};
	
	
	class C4_API CombineNormalsProcess : public Process
	{
		private:
			
			CombineNormalsProcess(const CombineNormalsProcess& combineNormalsProcess);
			
			Process *Replicate(void) const override;
		
		public:
			
			CombineNormalsProcess();
			~CombineNormalsProcess();
			
			int32 GetPortCount(void) const;
			const char *GetPortName(int32 index) const;
			
			void GenerateProcessData(const ShaderCompileData *compileData, ProcessData *data) const;
			
			int32 GenerateProgramCode(const ShaderCompileData *compileData, const char **programCode) const;
			int32 GenerateShaderCode(const ShaderCompileData *compileData, const char **shaderCode) const;
	};
	
	
	class C4_API FrontNormalProcess : public Process
	{
		private:
			
			FrontNormalProcess(const FrontNormalProcess& frontNormalProcess);
			
			Process *Replicate(void) const override;
		
		public:
			
			FrontNormalProcess();
			~FrontNormalProcess();
			
			static bool ValidShader(ProcessType type, int32 shader);
			
			int32 GetPortCount(void) const;
			const char *GetPortName(int32 index) const;
			
			int32 GenerateDerivedInterpolantTypes(const ShaderCompileData *compileData, ProcessType *type) const;
			void GenerateProcessData(const ShaderCompileData *compileData, ProcessData *data) const;
			
			int32 GenerateProgramCode(const ShaderCompileData *compileData, const char **programCode) const;
			int32 GenerateShaderCode(const ShaderCompileData *compileData, const char **shaderCode) const;
	};
	
	
	class C4_API ReflectVectorProcess : public Process
	{
		private:
			
			ReflectVectorProcess(const ReflectVectorProcess& reflectVectorProcess);
			
			Process *Replicate(void) const override;
		
		public:
			
			ReflectVectorProcess();
			~ReflectVectorProcess();
			
			int32 GetPortCount(void) const;
			unsigned_int32 GetPortFlags(int32 index) const;
			const char *GetPortName(int32 index) const;
			
			void GenerateProcessData(const ShaderCompileData *compileData, ProcessData *data) const;
			
			int32 GenerateProgramCode(const ShaderCompileData *compileData, const char **programCode) const;
			int32 GenerateShaderCode(const ShaderCompileData *compileData, const char **shaderCode) const;
	};
	
	
	class C4_API LinearRampProcess : public UnaryProcess
	{
		private:
			
			float		rampCenter;
			float		rampWidth;
			
			LinearRampProcess(const LinearRampProcess& linearRampProcess);
			
			Process *Replicate(void) const override;
		
		public:
			
			LinearRampProcess();
			~LinearRampProcess();
			
			float GetRampCenter(void) const
			{
				return (rampCenter);
			}
			
			void SetRampCenter(float center)
			{
				rampCenter = center;
			}
			
			float GetRampWidth(void) const
			{
				return (rampWidth);
			}
			
			void SetRampWidth(float width)
			{
				rampWidth = width;
			}
			
			void Pack(Packer& data, unsigned_int32 packFlags) const;
			void Unpack(Unpacker& data, unsigned_int32 unpackFlags);
			
			int32 GetSettingCount(void) const;
			Setting *GetSetting(int32 index) const;
			void SetSetting(const Setting *setting);
			
			bool operator ==(const Process& process) const;
			
			int32 GenerateProcessSignature(const ShaderCompileData *compileData, unsigned_int32 *signature) const;
			void GenerateProcessData(const ShaderCompileData *compileData, ProcessData *data) const;
			
			int32 GenerateProgramCode(const ShaderCompileData *compileData, const char **programCode) const;
			int32 GenerateShaderCode(const ShaderCompileData *compileData, const char **shaderCode) const;
	};
	
	
	class C4_API SmoothParameterProcess : public UnaryProcess
	{
		private:
			
			SmoothParameterProcess(const SmoothParameterProcess& smoothParameterProcess);
			
			Process *Replicate(void) const override;
		
		public:
			
			SmoothParameterProcess();
			~SmoothParameterProcess();
			
			const char *GetPortName(int32 index) const;
			
			int32 GenerateProgramCode(const ShaderCompileData *compileData, const char **programCode) const;
			int32 GenerateShaderCode(const ShaderCompileData *compileData, const char **shaderCode) const;
	};
	
	
	class C4_API SteepParameterProcess : public UnaryProcess
	{
		private:
			
			SteepParameterProcess(const SteepParameterProcess& steepParameterProcess);
			
			Process *Replicate(void) const override;
		
		public:
			
			SteepParameterProcess();
			~SteepParameterProcess();
			
			const char *GetPortName(int32 index) const;
			
			int32 GenerateProgramCode(const ShaderCompileData *compileData, const char **programCode) const;
			int32 GenerateShaderCode(const ShaderCompileData *compileData, const char **shaderCode) const;
	};
	
	
	class C4_API WorldTransformProcess : public Process
	{
		private:
			
			WorldTransformProcess(const WorldTransformProcess& worldTransformProcess);
			
			Process *Replicate(void) const override;
		
		public:
			
			WorldTransformProcess();
			~WorldTransformProcess();
			
			int32 GetPortCount(void) const;
			const char *GetPortName(int32 index) const;
			
			void GenerateProcessData(const ShaderCompileData *compileData, ProcessData *data) const;
			
			int32 GenerateProgramCode(const ShaderCompileData *compileData, const char **programCode) const;
			int32 GenerateShaderCode(const ShaderCompileData *compileData, const char **shaderCode) const;
	};
	
	
	class C4_API DeltaDepthProcess : public Process
	{
		private:
			
			float		deltaScale;
			
			DeltaDepthProcess(const DeltaDepthProcess& deltaDepthProcess);
			
			Process *Replicate(void) const override;
			
			static bool StructureEffectsEnabled(void);
		
		public:
			
			DeltaDepthProcess();
			~DeltaDepthProcess();
			
			float GetDeltaScale(void) const
			{
				return (deltaScale);
			}
			
			void SetDeltaScale(float scale)
			{
				deltaScale = scale;
			}
			
			void Pack(Packer& data, unsigned_int32 packFlags) const;
			void Unpack(Unpacker& data, unsigned_int32 unpackFlags);
			
			int32 GetSettingCount(void) const;
			Setting *GetSetting(int32 index) const;
			void SetSetting(const Setting *setting);
			
			bool operator ==(const Process& process) const;
			
			int32 GenerateProcessSignature(const ShaderCompileData *compileData, unsigned_int32 *signature) const;
			void GenerateProcessData(const ShaderCompileData *compileData, ProcessData *data) const;
			
			int32 GenerateProgramCode(const ShaderCompileData *compileData, const char **programCode) const;
			int32 GenerateShaderCode(const ShaderCompileData *compileData, const char **shaderCode) const;
	};
	
	
	class C4_API ParallaxProcess : public TextureMapProcess
	{
		private:
			
			ParallaxProcess(const ParallaxProcess& parallaxProcess);
			
			Process *Replicate(void) const override;
			
			bool ProcessEnabled(const ShaderCompileData *compileData) const;
			
			static void StateFunc_CalculateParallaxScale(const Renderable *renderable, const void *cookie);
		
		public:
			
			ParallaxProcess();
			~ParallaxProcess();
			
			int32 GetSettingCount(void) const;
			Setting *GetSetting(int32 index) const;
			void SetSetting(const Setting *setting);
			
			int32 GenerateProcessSignature(const ShaderCompileData *compileData, unsigned_int32 *signature) const;
			int32 GenerateDerivedInterpolantTypes(const ShaderCompileData *compileData, ProcessType *type) const;
			void GenerateProcessData(const ShaderCompileData *compileData, ProcessData *data) const;
			int32 GenerateOutputIdentifier(const ShaderCompileData *compileData, const ShaderAllocationData *allocData, SwizzleData *swizzleData, char *name) const;
			
			int32 GenerateProgramCode(const ShaderCompileData *compileData, const char **programCode) const;
			int32 GenerateShaderCode(const ShaderCompileData *compileData, const char **shaderCode) const;
	};
	
	
	class C4_API KillProcess : public Process
	{
		private:
			
			KillProcess(const KillProcess& killProcess);
			
			Process *Replicate(void) const override;
		
		public:
			
			KillProcess();
			~KillProcess();
			
			int32 GetPortCount(void) const;
			unsigned_int32 GetPortFlags(int32 index) const;
			const char *GetPortName(int32 index) const;
			
			#if C4MACOS
			
				void GenerateSourceData(const ShaderCompileData *compileData) const;
			
			#endif
			
			void GenerateProcessData(const ShaderCompileData *compileData, ProcessData *data) const;
			
			int32 GenerateProgramCode(const ShaderCompileData *compileData, const char **programCode) const;
			int32 GenerateShaderCode(const ShaderCompileData *compileData, const char **shaderCode) const;
	};
	
	
	class C4_API ImpostorTransitionProcess : public Process
	{
		private:
			
			ImpostorTransitionProcess(const ImpostorTransitionProcess& impostorTransitionProcess);
			
			Process *Replicate(void) const override;
		
		public:
			
			ImpostorTransitionProcess();
			~ImpostorTransitionProcess();
			
			#if C4MACOS
			
				void GenerateSourceData(const ShaderCompileData *compileData) const;
			
			#endif
			
			int32 GenerateDerivedInterpolantTypes(const ShaderCompileData *compileData, ProcessType *type) const;
			void GenerateProcessData(const ShaderCompileData *compileData, ProcessData *data) const;
			
			int32 GenerateProgramCode(const ShaderCompileData *compileData, const char **programCode) const;
			int32 GenerateShaderCode(const ShaderCompileData *compileData, const char **shaderCode) const;
	};
	
	
	class C4_API GeometryTransitionProcess : public Process
	{
		private:
			
			Texture		*textureObject;
			
			GeometryTransitionProcess(const GeometryTransitionProcess& geometryTransitionProcess);
			
			Process *Replicate(void) const override;
			
			bool ProcessEnabled(const ShaderCompileData *compileData) const;
		
		public:
			
			GeometryTransitionProcess();
			~GeometryTransitionProcess();
			
			int32 GetSettingCount(void) const;
			Setting *GetSetting(int32 index) const;
			void SetSetting(const Setting *setting);
			
			#if C4MACOS
			
				void GenerateSourceData(const ShaderCompileData *compileData) const;
			
			#endif
			
			int32 GenerateProcessSignature(const ShaderCompileData *compileData, unsigned_int32 *signature) const;
			void GenerateProcessData(const ShaderCompileData *compileData, ProcessData *data) const;
			
			int32 GenerateProgramCode(const ShaderCompileData *compileData, const char **programCode) const;
			int32 GenerateShaderCode(const ShaderCompileData *compileData, const char **shaderCode) const;
	};
	
	
	class C4_API FireProcess : public TextureMapProcess
	{
		private:
			
			FireAttribute::FireParams			fireParams;
			const FireAttribute::FireParams		*fireData;
			
			FireProcess(const FireProcess& fireProcess);
			
			Process *Replicate(void) const override;
			
			static void StateFunc_SetFireParams(const Renderable *renderable, const void *cookie);
		
		public:
			
			FireProcess();
			~FireProcess();
			
			void SetFireParams(const FireAttribute::FireParams *params)
			{
				fireParams.fireIntensity = params->fireIntensity;
				fireParams.noiseVelocity[0] = params->noiseVelocity[0];
				fireParams.noiseVelocity[1] = params->noiseVelocity[1];
				fireParams.noiseVelocity[2] = params->noiseVelocity[2];
			}
			
			void SetFireData(const FireAttribute::FireParams *data)
			{
				fireData = data;
			}
			
			void Pack(Packer& data, unsigned_int32 packFlags) const;
			void Unpack(Unpacker& data, unsigned_int32 unpackFlags);
			
			int32 GetPortCount(void) const;
			
			void GenerateProcessData(const ShaderCompileData *compileData, ProcessData *data) const;
			
			int32 GenerateProgramCode(const ShaderCompileData *compileData, const char **programCode) const;
			int32 GenerateShaderCode(const ShaderCompileData *compileData, const char **shaderCode) const;
	};
	
	
	class C4_API DistortionProcess : public TextureMapProcess
	{
		private:
			
			DistortionProcess(const DistortionProcess& distortionProcess);
			
			Process *Replicate(void) const override;
			
			static void StateFunc_CopyDistortionPlane(const Renderable *renderable, const void *cookie);
			static void StateFunc_TransformDistortionPlane(const Renderable *renderable, const void *cookie);
		
		public:
			
			DistortionProcess();
			~DistortionProcess();
			
			void GenerateProcessData(const ShaderCompileData *compileData, ProcessData *data) const;
			
			int32 GenerateProgramCode(const ShaderCompileData *compileData, const char **programCode) const;
			int32 GenerateShaderCode(const ShaderCompileData *compileData, const char **shaderCode) const;
	};
	
	
	class C4_API FrameBufferProcess : public Process
	{
		private:
			
			FrameBufferProcess(const FrameBufferProcess& frameBufferProcess);
			
			Process *Replicate(void) const override;
		
		public:
			
			FrameBufferProcess();
			~FrameBufferProcess();
			
			int32 GenerateProcessSignature(const ShaderCompileData *compileData, unsigned_int32 *signature) const;
			void GenerateProcessData(const ShaderCompileData *compileData, ProcessData *data) const;
			
			int32 GenerateProgramCode(const ShaderCompileData *compileData, const char **programCode) const;
			int32 GenerateShaderCode(const ShaderCompileData *compileData, const char **shaderCode) const;
	};
	
	
	class C4_API OutputProcess : public Process
	{
		protected:
			
			OutputProcess(ProcessType type);
			OutputProcess(const OutputProcess& outputProcess);
		
		public:
			
			~OutputProcess();
			
			static bool ValidShader(ProcessType type, int32 shader);
	};
	
	
	class C4_API NullOutputProcess : public OutputProcess
	{
		private:
			
			NullOutputProcess(const NullOutputProcess& nullOutputProcess);
			
			Process *Replicate(void) const override;
		
		public:
			
			NullOutputProcess();
			~NullOutputProcess();
			
			int32 GenerateProgramCode(const ShaderCompileData *compileData, const char **programCode) const;
			int32 GenerateShaderCode(const ShaderCompileData *compileData, const char **shaderCode) const;
	};
	
	
	class C4_API AddOutputProcess : public OutputProcess
	{
		private:
			
			AddOutputProcess(const AddOutputProcess& addOutputProcess);
			
			Process *Replicate(void) const override;
		
		public:
			
			AddOutputProcess();
			~AddOutputProcess();
			
			int32 GetPortCount(void) const;
			const char *GetPortName(int32 index) const;
			
			void GenerateProcessData(const ShaderCompileData *compileData, ProcessData *data) const;
			int32 GenerateOutputIdentifier(const ShaderCompileData *compileData, const ShaderAllocationData *allocData, SwizzleData *swizzleData, char *name) const;
			
			int32 GenerateProgramCode(const ShaderCompileData *compileData, const char **programCode) const;
			int32 GenerateShaderCode(const ShaderCompileData *compileData, const char **shaderCode) const;
	};
	
	
	class C4_API AmbientOutputProcess : public OutputProcess
	{
		private:
			
			AmbientOutputProcess(const AmbientOutputProcess& ambientOutputProcess);
			
			Process *Replicate(void) const override;
			
			static ShaderType GetAmbientShaderType(const ShaderCompileData *compileData);
			
			static void StateFunc_ConfigureAmbientLight(const Renderable *renderable, const void *cookie);
			static void StateFunc_ConfigureAmbientGradient(const Renderable *renderable, const void *cookie);
			static void StateFunc_ConfigureTransformAmbientGradient(const Renderable *renderable, const void *cookie);
			static void StateFunc_ConfigureAmbientSpace(const Renderable *renderable, const void *cookie);
			static void StateFunc_ConfigureTransformAmbientSpace(const Renderable *renderable, const void *cookie);
		
		public:
			
			AmbientOutputProcess();
			~AmbientOutputProcess();
			
			int32 GetPortCount(void) const;
			unsigned_int32 GetPortFlags(int32 index) const;
			const char *GetPortName(int32 index) const;
			
			int32 GenerateProcessSignature(const ShaderCompileData *compileData, unsigned_int32 *signature) const;
			void GenerateProcessData(const ShaderCompileData *compileData, ProcessData *data) const;
			int32 GenerateOutputIdentifier(const ShaderCompileData *compileData, const ShaderAllocationData *allocData, SwizzleData *swizzleData, char *name) const;
			
			int32 GenerateProgramCode(const ShaderCompileData *compileData, const char **programCode) const;
			int32 GenerateShaderCode(const ShaderCompileData *compileData, const char **shaderCode) const;
	};
	
	
	class C4_API AmbientAlphaOutputProcess : public OutputProcess
	{
		private:
			
			AmbientAlphaOutputProcess(const AmbientAlphaOutputProcess& ambientAlphaOutputProcess);
			
			Process *Replicate(void) const override;
		
		public:
			
			AmbientAlphaOutputProcess();
			~AmbientAlphaOutputProcess();
			
			int32 GetPortCount(void) const;
			unsigned_int32 GetPortFlags(int32 index) const;
			const char *GetPortName(int32 index) const;
			
			void GenerateProcessData(const ShaderCompileData *compileData, ProcessData *data) const;
			
			int32 GenerateProgramCode(const ShaderCompileData *compileData, const char **programCode) const;
			int32 GenerateShaderCode(const ShaderCompileData *compileData, const char **shaderCode) const;
	};
	
	
	class C4_API AmbientOcclusionOutputProcess : public OutputProcess
	{
		private:
			
			AmbientOcclusionOutputProcess(const AmbientOcclusionOutputProcess& ambientOcclusionOutputProcess);
			
			Process *Replicate(void) const override;
		
		public:
			
			AmbientOcclusionOutputProcess();
			~AmbientOcclusionOutputProcess();
			
			int32 GetPortCount(void) const;
			const char *GetPortName(int32 index) const;
			
			void GenerateSourceData(const ShaderCompileData *compileData) const;
			
			void GenerateProcessData(const ShaderCompileData *compileData, ProcessData *data) const;
			int32 GenerateOutputIdentifier(const ShaderCompileData *compileData, const ShaderAllocationData *allocData, SwizzleData *swizzleData, char *name) const;
			
			int32 GenerateProgramCode(const ShaderCompileData *compileData, const char **programCode) const;
			int32 GenerateShaderCode(const ShaderCompileData *compileData, const char **shaderCode) const;
	};
	
	
	class C4_API EmissionOutputProcess : public OutputProcess
	{
		private:
			
			EmissionOutputProcess(const EmissionOutputProcess& emissionOutputProcess);
			
			Process *Replicate(void) const override;
		
		public:
			
			EmissionOutputProcess();
			~EmissionOutputProcess();
			
			int32 GetPortCount(void) const;
			unsigned_int32 GetPortFlags(int32 index) const;
			const char *GetPortName(int32 index) const;
			
			void GenerateProcessData(const ShaderCompileData *compileData, ProcessData *data) const;
			int32 GenerateOutputIdentifier(const ShaderCompileData *compileData, const ShaderAllocationData *allocData, SwizzleData *swizzleData, char *name) const;
	};
	
	
	class C4_API ReflectionOutputProcess : public OutputProcess
	{
		private:
			
			ReflectionAttribute::ReflectionParams			reflectionParams;
			const ReflectionAttribute::ReflectionParams		*reflectionData;
			
			ReflectionOutputProcess(const ReflectionOutputProcess& reflectionOutputProcess);
			
			Process *Replicate(void) const override;
			
			static void StateFunc_CalculateReflectionScale(const Renderable *renderable, const void *cookie);
		
		public:
			
			ReflectionOutputProcess();
			~ReflectionOutputProcess();
			
			void SetReflectionParams(const ReflectionAttribute::ReflectionParams *params)
			{
				reflectionParams.normalIncidenceReflectivity = params->normalIncidenceReflectivity;
				reflectionParams.reflectionOffsetScale = params->reflectionOffsetScale;
			}
			
			void SetReflectionData(const ReflectionAttribute::ReflectionParams *data)
			{
				reflectionData = data;
			}
			
			void Pack(Packer& data, unsigned_int32 packFlags) const;
			void Unpack(Unpacker& data, unsigned_int32 unpackFlags);
			
			int32 GetSettingCount(void) const;
			Setting *GetSetting(int32 index) const;
			void SetSetting(const Setting *setting);
			
			bool operator ==(const Process& process) const;
			
			int32 GetPortCount(void) const;
			unsigned_int32 GetPortFlags(int32 index) const;
			const char *GetPortName(int32 index) const;
			
			void ReferenceStateParams(const Process *process);
			
			int32 GenerateProcessSignature(const ShaderCompileData *compileData, unsigned_int32 *signature) const;
			int32 GenerateDerivedInterpolantTypes(const ShaderCompileData *compileData, ProcessType *type) const;
			void GenerateProcessData(const ShaderCompileData *compileData, ProcessData *data) const;
			
			int32 GenerateProgramCode(const ShaderCompileData *compileData, const char **programCode) const;
			int32 GenerateShaderCode(const ShaderCompileData *compileData, const char **shaderCode) const;
	};
	
	
	class C4_API RefractionOutputProcess : public OutputProcess
	{
		private:
			
			RefractionAttribute::RefractionParams			refractionParams;
			const RefractionAttribute::RefractionParams		*refractionData;
			
			RefractionOutputProcess(const RefractionOutputProcess& refractionOutputProcess);
			
			Process *Replicate(void) const override;
			
			static void StateFunc_CalculateRefractionParams(const Renderable *renderable, const void *cookie);
		
		public:
			
			RefractionOutputProcess();
			~RefractionOutputProcess();
			
			void SetRefractionParams(const RefractionAttribute::RefractionParams *params)
			{
				refractionParams.refractionOffsetScale = params->refractionOffsetScale;
			}
			
			void SetRefractionData(const RefractionAttribute::RefractionParams *data)
			{
				refractionData = data;
			}
			
			void Pack(Packer& data, unsigned_int32 packFlags) const;
			void Unpack(Unpacker& data, unsigned_int32 unpackFlags);
			
			int32 GetSettingCount(void) const;
			Setting *GetSetting(int32 index) const;
			void SetSetting(const Setting *setting);
			
			bool operator ==(const Process& process) const;
			
			int32 GetPortCount(void) const;
			unsigned_int32 GetPortFlags(int32 index) const;
			const char *GetPortName(int32 index) const;
			
			void ReferenceStateParams(const Process *process);
			
			void GenerateProcessData(const ShaderCompileData *compileData, ProcessData *data) const;
			
			int32 GenerateProgramCode(const ShaderCompileData *compileData, const char **programCode) const;
			int32 GenerateShaderCode(const ShaderCompileData *compileData, const char **shaderCode) const;
	};
	
	
	class C4_API EnvironmentOutputProcess : public OutputProcess
	{
		private:
			
			Texture				*textureObject;
			ResourceName		textureName;
			
			EnvironmentOutputProcess(const EnvironmentOutputProcess& environmentOutputProcess);
			
			Process *Replicate(void) const override;
		
		public:
			
			EnvironmentOutputProcess();
			~EnvironmentOutputProcess();
			
			const ResourceName& GetTextureName(void) const
			{
				return (textureName);
			}
			
			void Pack(Packer& data, unsigned_int32 packFlags) const;
			void Unpack(Unpacker& data, unsigned_int32 unpackFlags);
			void *BeginSettingsUnpack(void);
			
			int32 GetSettingCount(void) const;
			Setting *GetSetting(int32 index) const;
			void SetSetting(const Setting *setting);
			
			bool operator ==(const Process& process) const;
			
			void SetTexture(const char *name);
			
			int32 GetPortCount(void) const;
			unsigned_int32 GetPortFlags(int32 index) const;
			const char *GetPortName(int32 index) const;
			
			int32 GenerateProcessSignature(const ShaderCompileData *compileData, unsigned_int32 *signature) const;
			void GenerateProcessData(const ShaderCompileData *compileData, ProcessData *data) const;
			
			int32 GenerateProgramCode(const ShaderCompileData *compileData, const char **programCode) const;
			int32 GenerateShaderCode(const ShaderCompileData *compileData, const char **shaderCode) const;
	};
	
	
	class C4_API TerrainEnvironmentOutputProcess : public OutputProcess
	{
		private:
			
			Texture				*textureObject;
			ResourceName		textureName;
			
			TerrainEnvironmentOutputProcess(const TerrainEnvironmentOutputProcess& terrainEnvironmentOutputProcess);
			
			Process *Replicate(void) const override;
			
			bool BumpEnabled(void) const;
		
		public:
			
			TerrainEnvironmentOutputProcess();
			~TerrainEnvironmentOutputProcess();
			
			const ResourceName& GetTextureName(void) const
			{
				return (textureName);
			}
			
			void Pack(Packer& data, unsigned_int32 packFlags) const;
			void Unpack(Unpacker& data, unsigned_int32 unpackFlags);
			void *BeginSettingsUnpack(void);
			
			int32 GetSettingCount(void) const;
			Setting *GetSetting(int32 index) const;
			void SetSetting(const Setting *setting);
			
			bool operator ==(const Process& process) const;
			
			void SetTextureName(const char *name);
			
			int32 GetPortCount(void) const;
			unsigned_int32 GetPortFlags(int32 index) const;
			const char *GetPortName(int32 index) const;
			
			int32 GenerateDerivedInterpolantTypes(const ShaderCompileData *compileData, ProcessType *type) const;
			void GenerateProcessData(const ShaderCompileData *compileData, ProcessData *data) const;
			
			int32 GenerateProgramCode(const ShaderCompileData *compileData, const char **programCode) const;
			int32 GenerateShaderCode(const ShaderCompileData *compileData, const char **shaderCode) const;
	};
	
	
	class C4_API GlowOutputProcess : public OutputProcess
	{
		private:
			
			GlowOutputProcess(const GlowOutputProcess& glowOutputProcess);
			
			Process *Replicate(void) const override;
		
		public:
			
			GlowOutputProcess();
			~GlowOutputProcess();
			
			int32 GetPortCount(void) const;
			unsigned_int32 GetPortFlags(int32 index) const;
			const char *GetPortName(int32 index) const;
			
			void GenerateProcessData(const ShaderCompileData *compileData, ProcessData *data) const;
			
			int32 GenerateProgramCode(const ShaderCompileData *compileData, const char **programCode) const;
			int32 GenerateShaderCode(const ShaderCompileData *compileData, const char **shaderCode) const;
	};
	
	
	class C4_API ImpostorDepthOutputProcess : public OutputProcess
	{
		private:
			
			ImpostorDepthOutputProcess(const ImpostorDepthOutputProcess& impostorDepthOutputProcess);
			
			Process *Replicate(void) const override;
		
		public:
			
			ImpostorDepthOutputProcess();
			~ImpostorDepthOutputProcess();
			
			int32 GetPortCount(void) const;
			unsigned_int32 GetPortFlags(int32 index) const;
			const char *GetPortName(int32 index) const;
			
			#if C4MACOS
			
				void GenerateSourceData(const ShaderCompileData *compileData) const;
			
			#endif
			
			void GenerateProcessData(const ShaderCompileData *compileData, ProcessData *data) const;
			
			int32 GenerateProgramCode(const ShaderCompileData *compileData, const char **programCode) const;
			int32 GenerateShaderCode(const ShaderCompileData *compileData, const char **shaderCode) const;
	};
	
	
	class C4_API LightOutputProcess : public OutputProcess
	{
		private:
			
			LightOutputProcess(const LightOutputProcess& lightOutputProcess);
			
			Process *Replicate(void) const override;
		
		public:
			
			LightOutputProcess();
			~LightOutputProcess();
			
			int32 GetPortCount(void) const;
			unsigned_int32 GetPortFlags(int32 index) const;
			const char *GetPortName(int32 index) const;
			
			static ShaderType GetLightShaderType(const ShaderCompileData *compileData);
			
			#if C4OPENGL
			
				void GenerateSourceData(const ShaderCompileData *compileData) const;
			
			#endif
			
			int32 GenerateProcessSignature(const ShaderCompileData *compileData, unsigned_int32 *signature) const;
			void GenerateProcessData(const ShaderCompileData *compileData, ProcessData *data) const;
			int32 GenerateOutputIdentifier(const ShaderCompileData *compileData, const ShaderAllocationData *allocData, SwizzleData *swizzleData, char *name) const;
			
			int32 GenerateProgramCode(const ShaderCompileData *compileData, const char **programCode) const;
			int32 GenerateShaderCode(const ShaderCompileData *compileData, const char **shaderCode) const;
	};
	
	
	class C4_API BloomOutputProcess : public OutputProcess
	{
		private:
			
			BloomOutputProcess(const BloomOutputProcess& bloomOutputProcess);
			
			Process *Replicate(void) const override;
		
		public:
			
			BloomOutputProcess();
			~BloomOutputProcess();
			
			int32 GetPortCount(void) const;
			unsigned_int32 GetPortFlags(int32 index) const;
			const char *GetPortName(int32 index) const;
			
			void GenerateProcessData(const ShaderCompileData *compileData, ProcessData *data) const;
			
			int32 GenerateProgramCode(const ShaderCompileData *compileData, const char **programCode) const;
			int32 GenerateShaderCode(const ShaderCompileData *compileData, const char **shaderCode) const;
	};
	
	
	class C4_API AlphaTestOutputProcess : public OutputProcess
	{
		private:
			
			AlphaTestOutputProcess(const AlphaTestOutputProcess& alphaTestOutputProcess);
			
			Process *Replicate(void) const override;
		
		public:
			
			AlphaTestOutputProcess();
			~AlphaTestOutputProcess();
			
			int32 GetPortCount(void) const;
			unsigned_int32 GetPortFlags(int32 index) const;
			const char *GetPortName(int32 index) const;
			
			void GenerateProcessData(const ShaderCompileData *compileData, ProcessData *data) const;
			
			int32 GenerateProgramCode(const ShaderCompileData *compileData, const char **programCode) const;
			int32 GenerateShaderCode(const ShaderCompileData *compileData, const char **shaderCode) const;
	};
	
	
	class C4_API StructureOutputProcess : public OutputProcess
	{
		private:
			
			StructureOutputProcess(const StructureOutputProcess& structureOutputProcess);
			
			Process *Replicate(void) const override;
			
			static unsigned_int32 GetStructureRenderFlags(unsigned_int32 renderableFlags);
		
		public:
			
			StructureOutputProcess();
			~StructureOutputProcess();
			
			#if C4OPENGL
			
				void GenerateSourceData(const ShaderCompileData *compileData) const;
			
			#endif
			
			int32 GenerateProcessSignature(const ShaderCompileData *compileData, unsigned_int32 *signature) const;
			void GenerateProcessData(const ShaderCompileData *compileData, ProcessData *data) const;
			
			int32 GenerateProgramCode(const ShaderCompileData *compileData, const char **programCode) const;
			int32 GenerateShaderCode(const ShaderCompileData *compileData, const char **shaderCode) const;
	};
	
	
	class C4_API ConstantFogProcess : public Process
	{
		private:
			
			ConstantFogProcess(const ConstantFogProcess& constantFogProcess);
			
			Process *Replicate(void) const override;
		
		public:
			
			ConstantFogProcess();
			~ConstantFogProcess();
			
			void GenerateProcessData(const ShaderCompileData *compileData, ProcessData *data) const;
			
			int32 GenerateProgramCode(const ShaderCompileData *compileData, const char **programCode) const;
			int32 GenerateShaderCode(const ShaderCompileData *compileData, const char **shaderCode) const;
			
			static void StateFunc_CopyFogPlane(const Renderable *renderable, const void *cookie);
			static void StateFunc_TransformFogPlane(const Renderable *renderable, const void *cookie);
	};
	
	
	class C4_API LinearFogProcess : public Process
	{
		private:
			
			LinearFogProcess(const LinearFogProcess& linearFogProcess);
			
			Process *Replicate(void) const override;
		
		public:
			
			LinearFogProcess();
			~LinearFogProcess();
			
			void GenerateProcessData(const ShaderCompileData *compileData, ProcessData *data) const;
			
			int32 GenerateProgramCode(const ShaderCompileData *compileData, const char **programCode) const;
			int32 GenerateShaderCode(const ShaderCompileData *compileData, const char **shaderCode) const;
	};
	
	
	class C4_API AmbientFogProcess : public Process
	{
		private:
			
			AmbientFogProcess(const AmbientFogProcess& ambientFogProcess);
			
			Process *Replicate(void) const override;
		
		public:
			
			AmbientFogProcess();
			~AmbientFogProcess();
			
			void GenerateProcessData(const ShaderCompileData *compileData, ProcessData *data) const;
			
			int32 GenerateProgramCode(const ShaderCompileData *compileData, const char **programCode) const;
			int32 GenerateShaderCode(const ShaderCompileData *compileData, const char **shaderCode) const;
	};
	
	
	class C4_API LightFogProcess : public Process
	{
		private:
			
			LightFogProcess(const LightFogProcess& lightFogProcess);
			
			Process *Replicate(void) const override;
		
		public:
			
			LightFogProcess();
			~LightFogProcess();
			
			void GenerateProcessData(const ShaderCompileData *compileData, ProcessData *data) const;
			
			int32 GenerateProgramCode(const ShaderCompileData *compileData, const char **programCode) const;
			int32 GenerateShaderCode(const ShaderCompileData *compileData, const char **shaderCode) const;
	};
	
	
	class C4_API AlphaFogProcess : public Process
	{
		private:
			
			AlphaFogProcess(const AlphaFogProcess& alphaFogProcess);
			
			Process *Replicate(void) const override;
		
		public:
			
			AlphaFogProcess();
			~AlphaFogProcess();
			
			void GenerateProcessData(const ShaderCompileData *compileData, ProcessData *data) const;
			
			int32 GenerateProgramCode(const ShaderCompileData *compileData, const char **programCode) const;
			int32 GenerateShaderCode(const ShaderCompileData *compileData, const char **shaderCode) const;
	};
	
	
	class C4_API ColorPostProcess : public Process
	{
		private:
			
			ColorPostProcess(const ColorPostProcess& colorPostProcess);
			
			Process *Replicate(void) const override;
		
		public:
			
			ColorPostProcess();
			~ColorPostProcess();
			
			int32 GetPortCount(void) const;
			const char *GetPortName(int32 index) const;
			
			void GenerateProcessData(const ShaderCompileData *compileData, ProcessData *data) const;
			
			int32 GenerateProgramCode(const ShaderCompileData *compileData, const char **programCode) const;
			int32 GenerateShaderCode(const ShaderCompileData *compileData, const char **shaderCode) const;
	};
	
	
	class C4_API DistortPostProcess : public Process
	{
		private:
			
			DistortPostProcess(const DistortPostProcess& distortPostProcess);
			
			Process *Replicate(void) const override;
		
		public:
			
			DistortPostProcess();
			~DistortPostProcess();
			
			void GenerateProcessData(const ShaderCompileData *compileData, ProcessData *data) const;
			
			int32 GenerateProgramCode(const ShaderCompileData *compileData, const char **programCode) const;
			int32 GenerateShaderCode(const ShaderCompileData *compileData, const char **shaderCode) const;
	};
	
	
	class C4_API MotionBlurPostProcess : public Process
	{
		private:
			
			bool	gradientFlag;
			
			MotionBlurPostProcess(const MotionBlurPostProcess& motionBlurPostProcess);
			
			Process *Replicate(void) const override;
		
		public:
			
			MotionBlurPostProcess(bool gradient);
			~MotionBlurPostProcess();
			
			int32 GetPortCount(void) const;
			const char *GetPortName(int32 index) const;
			
			int32 GenerateProcessSignature(const ShaderCompileData *compileData, unsigned_int32 *signature) const;
			void GenerateProcessData(const ShaderCompileData *compileData, ProcessData *data) const;
			
			int32 GenerateProgramCode(const ShaderCompileData *compileData, const char **programCode) const;
			int32 GenerateShaderCode(const ShaderCompileData *compileData, const char **shaderCode) const;
	};
	
	
	class C4_API ExtractPostProcess : public Process
	{
		private:
			
			ExtractPostProcess(const ExtractPostProcess& extractPostProcess);
			
			Process *Replicate(void) const override;
		
		public:
			
			ExtractPostProcess();
			~ExtractPostProcess();
			
			void GenerateProcessData(const ShaderCompileData *compileData, ProcessData *data) const;
			
			int32 GenerateProgramCode(const ShaderCompileData *compileData, const char **programCode) const;
			int32 GenerateShaderCode(const ShaderCompileData *compileData, const char **shaderCode) const;
	};
	
	
	class C4_API GlowPostProcess : public Process
	{
		private:
			
			GlowPostProcess(const GlowPostProcess& glowPostProcess);
			
			Process *Replicate(void) const override;
		
		public:
			
			GlowPostProcess();
			~GlowPostProcess();
			
			int32 GetPortCount(void) const;
			const char *GetPortName(int32 index) const;
			
			void GenerateProcessData(const ShaderCompileData *compileData, ProcessData *data) const;
			
			int32 GenerateProgramCode(const ShaderCompileData *compileData, const char **programCode) const;
			int32 GenerateShaderCode(const ShaderCompileData *compileData, const char **shaderCode) const;
	};
	
	
	class C4_API TransformPostProcess : public Process
	{
		private:
			
			bool		colorMatrixFlag;
			
			TransformPostProcess(const TransformPostProcess& transformPostProcess);
			
			Process *Replicate(void) const override;
		
		public:
			
			TransformPostProcess(bool matrixFlag);
			~TransformPostProcess();
			
			int32 GetPortCount(void) const;
			const char *GetPortName(int32 index) const;
			
			int32 GenerateProcessSignature(const ShaderCompileData *compileData, unsigned_int32 *signature) const;
			void GenerateProcessData(const ShaderCompileData *compileData, ProcessData *data) const;
			
			int32 GenerateProgramCode(const ShaderCompileData *compileData, const char **programCode) const;
			int32 GenerateShaderCode(const ShaderCompileData *compileData, const char **shaderCode) const;
	};
}


#endif

// ZYURVUR
