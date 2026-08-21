/***************************************************************************
File: Ge2Virtools.h

Author:  Romain Sididris - Virtools 2000 

Class Definition

This file defines a set of helper classes that can be use when dealing with 
exporter/importer to Virtools Format. 

The Export2Virtools class contains methods to help you generate Virtools objects
and store them in a database. Each created can be assigned a key (void*)
to retrieve it afterward.

+ The Virtools mesh format has some constraints about vertex format :
one normal,color/uv per vertex and use triangle faces only, so you may also
find useful the VirtoolsTransitionMesh class which can be used to construct
a mesh without these limitations (Any number of normals/colors/uv per vertex)
and polygon face.

  Virtools SDK 														 	 
  Copyright (c) Virtools 2000, All Rights Reserved.					 	
***************************************************************************/
#ifndef GENERICEXPORT_H
#define GENERICEXPORT_H

// Declare the generic structures for Import/Export.
#include "CKAll.h"
#include "GeFileLog.h"

#define MAX_UVCHANNEL_COUNT 8
#define SMALL_FLOAT 0.000001f

//------------- Type Definitions ---------------------

typedef XArray<int> IntArray;
class Export2Virtools;
class VirtoolsCompositeMaterial;
// Hash table definitions

// Hash Tables (Key -> Object )
typedef XHashTable<CKObject*,void*> XObjectHashTable;			
typedef XHashTable<CKObject*,void*>::Iterator XObjectHashTableIt;		
typedef XHashTable<CKObject*,void*>::Pair XObjectHashTablePair;	
// Hash Tables (Key -> int)
typedef XHashTable<int,void*> XIntHashTable;			
typedef XHashTable<int,void*>::Iterator XIntHashTableIt;		
typedef XHashTable<int,void*>::Pair XIntHashTablePair;	
// Hash Tables (texture filename -> texture)
typedef XHashTable<CKTexture*,XString> XTextureHashTable;			
typedef XHashTable<CKTexture*,XString>::Iterator XTextureHashTableIt;		
typedef XHashTable<CKTexture*,XString>::Pair XTextureHashTablePair;

// Hash Tables (CKMaterial -> VirtoolsCompositeMaterial )
typedef XHashTable<int,CKMaterial*> XCmpMatHashTable;			
typedef XHashTable<int,CKMaterial*>::Iterator XCmpMatHashTableIt;		
typedef XHashTable<int,CKMaterial*>::Pair XCmpMatHashTablePair;	



// Triangle indices
class TriIndices {
public:
	int idx[3];
	TriIndices() { idx[0]=idx[1]=idx[2]=0; }
	TriIndices(int a,int b,int c) {	idx[0]=a;	idx[1]=b; 	idx[2]=c; }
};

// The same with a pointer to the CKMaterial which gives
// the color/lighting properties of the face
class TriFace: public TriIndices {
public:
	CKMaterial* material;
	TriFace():material(NULL) { }
	TriFace(int a,int b,int c,CKMaterial* mat):TriIndices(a,b,c),material(mat){}
};

//CompositeMaterialInfo, used internally by SplitMeshByCompositeMaterial
class CompositeMtlInfo{
public:
	VirtoolsCompositeMaterial* c;
	XArray<int> faces;
	XArray<int> usedV;
	XArray<int> remapV;
};

enum EControlPointType
{
	eBezierCP		=0,
	eCatmullRomCP	=1,
	eHermiteCP		=2,
	eBSplineCP		=3
};

enum EAnimType
{
	ePosition		=0,
	eRotation		=1,
	eScale			=2,
	eScaleAxis		=3,
	eMorph			=4
};

enum EAxis
{
	ePositiveX,
	ePositiveY,
	ePositiveZ,
	
	eNegativeX,
	eNegativeY,
	eNegativeZ
};

enum ETextureResizeMethod
{
	eNearestPowerOfTwo	=0,
	eLowerPowerOfTwo	=1,
	eHigherPowerOfTwo	=2
};

//Use to tell ReduceAnimationKeys the method to compute the algorithm's effectivness
enum EEvalMethod
{
	eAuto		=0,
	eQuality	=1,
	eFileSize	=2
};

/***************************************************************************
VirtoolsVertex
 Transition class for constructing vertices.
 This class is used internally by the exporter when generating meshes but 
 can be useful when dealing with morph animations.
***************************************************************************/
class VirtoolsVertex {
public:
	VxVector Pos;						// Vertex Position
	VxVector Norm;						// Vertex Normal
	VxColor  Col;						// Vertex Color
	VxColor	 Uvs[MAX_UVCHANNEL_COUNT];	// Vertex Texture coordinates

// These are different indices of the position,normal and color in 
// the source format (that is in the m_Vertices,m_Normals,m_VertexColors
// array sof VirtoolsTransitionMesh class)
	int	OriginalPosIndex;						// -1 if invalid	
	int	OriginalNormIndex;						//  " "     "
	int OriginalColIndex;						//  " "     "
	int OriginalUvIndex[MAX_UVCHANNEL_COUNT];	//  " "     "
// This is the index of the vertice in NbFace*3 array (used internally)
	int FlatIndex;
// If this vertex is equal to an existing vertex => Index of the this 
// vertex or -1 (used internally)
	int ReAssign;
};


/*****************************************************************************
VirtoolsTransitionMesh
 Helper class for building geometry mesh. Virtools forces the mesh to have only
 1 texture coordinate and  1 normal or color per vertex and triangle faces.
 This class will help you convert from other formats where a vertex can have 
 more than one normal per vertex or polygon faces.

 Usage :
 - Create a VirtoolsTransitionMesh with a pointer to the Export2Virtools interface
   and specify if it should be prelit (vertex color ) or not (using normals)
  
 - Fill information about geometry using the 
   AddPosition,AddNormal,AddVertexColor and AddUv methods

 - Fill face data. A face is a set of 3 or more indices to the previous tables.
   If the incoming format is similar to Virtools and already ensures one 
   normal/color/uv per vertex use the same indices for AddFace and AddUVFace/
   AddNormalFace/AddColorFace. 

 - Call GenerateVirtoolsData which will generate the m_VirtoolsVertices and m_VirtoolsFaces
   arrays. In the general cases you won't have to access these arrays.

 - You can add the generated mesh to the exporter database with the 
   Export2Virtools::AddMesh(...) method

 - When dealing with morph animations you may avoid to regenerate a transition mesh
   for a morph key by only updating the source arrays (m_Vertices,m_Normals)
   and call RefreshVirtoolsVerticesPosition(...) which will update the
   m_VirtoolsVertices array accordingly to the new positions.

 - When using face materials having channels applied to differents faces
  
/********************************************************************************/
class VirtoolsTransitionMesh {
public:
// Constructor
	VirtoolsTransitionMesh(Export2Virtools* exporter=NULL,BOOL PrelitMesh=FALSE):Exporter(exporter),m_ChannelCount(0),m_PrelitMesh(PrelitMesh),m_HasLightMap(FALSE) {}

//-- Reset all datas. Prelit Mode doesn't change
	void Reset();

//-- Filling geometry
	void AddPosition(VxVector& p)		{ m_Vertices.PushBack(p); }
	void AddNormal(VxVector& n)			{ m_Normals.PushBack(n); }
	void AddVertexColor(VxColor& c)		{ m_VertexColors.PushBack(c); }
	void AddUv(VxUV& uv,int channel=0)	{ m_Uvs[channel].PushBack(VxColor(uv.u,uv.v,0.f,0.f)); }
	bool AddChannelComponent(VxColor &pColor, int pChannel){m_Uvs[pChannel].PushBack(pColor);return true;}
//-- Faces...
	void AddFace(int* indices,int indexcount,CKMaterial* m);
	void AddFace(int index1,int index2,int index3,CKMaterial* m);
	void SetFaceMaterial(int index,CKMaterial* m) { m_Faces[index].material = m; }

	void AddUVFace(int* indices,int indexcount,int channel=0);
	void AddUVFace(int index1,int index2,int index3,int channel=0);
	void AddColorFace(int* indices,int indexcount);
	void AddColorFace(int index1,int index2,int index3);
	void AddNormalFace(int* indices,int indexcount);
	void AddNormalFace(int index1,int index2,int index3);


//-- Additionnal Materials (Multi-texture)
	void AddChannelMaterial(CKMaterial* m) { m_Channels.PushBack(m); m_ExtendedChannelsTypes.PushBack(0); }
//--Extended Channels
	BOOL AddExtendedChannel(int pNumComponent=1){ if(pNumComponent<1 || pNumComponent>4) return false; m_ExtendedChannelsTypes.PushBack(pNumComponent); m_Channels.PushBack(NULL);return true;}

	
// Construct arrays of Virtools Faces and Vertices
	BOOL GenerateVirtoolsData( BOOL iOptimizeVertexSharing=TRUE );
// Only recomputes vertices and normals ( GenerateVirtoolsData must have been called at least once...)
	BOOL RefreshVirtoolsVerticesPosition();
//
	const IntArray& GetGeneratedIndices(int OriginalVertexIndex) { return m_RemapIndices[OriginalVertexIndex]; }

//-- Source Data
	XArray<VxVector>	m_Vertices;					// List of Vertex positions
	XArray<VxVector>	m_Normals;					// List of normals
	XArray<VxColor>		m_VertexColors;				// List of vertex colors
	XArray<VxColor>		m_Uvs[MAX_UVCHANNEL_COUNT]; // List of texture coordinates per channel

	XArray<TriFace>		m_Faces;		// List of triangle faces
	XArray<TriIndices>	m_NormalFaces;	// There must be an equal number face for normals,uv and colors or none if not used;	
	XArray<TriIndices>	m_ColorFaces;	// ""
	XArray<TriIndices>	m_UVFaces[MAX_UVCHANNEL_COUNT];	
	
	XArray<CKMaterial*> m_Channels;
	XArray<VirtoolsCompositeMaterial*>	m_CompositeMaterials;
	XArray<int>			m_ExtendedChannelsTypes;	// Type 1,2,3,4 for float1 float2 float3 or float 4
	
	BOOL				m_PrelitMesh;

//-- Generated Data
	XClassArray<IntArray>			m_RemapIndices;		// This array has the size of the given m_Vertices, 
														// and contains the list of generated indices for a given
														// original vertex.
	XClassArray<VirtoolsVertex>		m_VirtoolsVertices; // Array of vertex as used in Virtools (Pos,Normal,UV) 
	XArray<TriFace>					m_VirtoolsFaces;
	int								m_ChannelCount;

	BOOL							m_HasLightMap;		// whether the mesh has a lightmap channel

protected:
	BOOL CompVtx(const VirtoolsVertex& v1,const VirtoolsVertex& v2);
	Export2Virtools* Exporter;
};


/*****************************************************************************
VirtoolsTransitionPatchMesh
 Helper class for building patchmesh geometry. No vertex color.

 Usage :
 - Create a VirtoolsTransitionPatchMesh with a pointer to the Export2Virtools interface.
 - Set ucount and vcount using SetUVCount(u,v)
 - Set the type of control points you'll be using with SetControlPointsType(p).
   Bezier control points are native while Hermite, BSpline and CatmullRom require conversion
   to the bezier form. This conversion is done internally.
 - Fill information about geometry using: 
   
	AddControlPoints
     Add all the corner control points. Add them in the following order

		for(v=0; v<vcount;v++)
		for(u=0; u<vcount;u++)
		{{
			AddControlPoints( evalknotposition( u,v ) );
		}}

	AddEdgeControlPoint(...)
	 Depending of the type of control points, Vecs will represent:
	  -Tangents for Hermite
	  -Position on surface for Catmul Rom and BSpline
	  -Bezier Control Points for Bezier
		//Adding U Vecs
		for(v=0; v< vcount;v++)
		for(u=0; u< ucount-1;u++)
		{{
			AddEdgeControlPoint(...);
			AddEdgeControlPoint(...);
		}}

		//Adding V Vecs
		for(v=0; v< vcount-1;v++)
		for(u=0; u< ucount;u++)
		{{
			AddEdgeControlPoint(...);
			AddEdgeControlPoint(...);
		}}

	AddUV(...)
	 Add all the UVs per channel in the same order as ControlPoints. 
	 There should be one UV per corner point .
	 Warning:When you skip channels (ie: use 0.... use 5) the skipped channel (ie: 1,2,3,4)
	 will still be add!!

  - Set the iteration level using SetIteration(). default= 3, min= 0, max= 20

 - You can add the generated mesh to the exporter database with the 
   Export2Virtools::AddPatch(...) method

/********************************************************************************/
class VirtoolsTransitionPatchMesh{
public:
//-- Constructor
	VirtoolsTransitionPatchMesh(Export2Virtools* exporter=NULL);

//-- Control Points
	void AddControlPoint(const VxVector &pControlPoint);
	void AddEdgeControlPoint(const VxVector &pControlPoint);

//-- Edge
	void AddEdge(int Vert0, int Vert1, int Vec0, int Vec1);

//-- Patch
	void AddPatch(int E0, int E1, int E2, int E3);

//-- Texture Coordinates
	bool AddUV(VxUV &pUV, int pChannel);
	bool AddChannelComponent(VxColor &pColor, int pChannel);

//-- Iteration Level
	BOOL		SetIterationLevel(int pLevel);

//-- Additionnal Materials (Multi-texture)
	void AddChannelMaterial(CKMaterial* m) { m_Channels.PushBack(m); m_ExtendedChannelsTypes.PushBack(0); }

//-- Extended Channels
	BOOL AddExtendedChannel(int pNumComponent=1){ if(pNumComponent<1 || pNumComponent>4) return false; m_ExtendedChannelsTypes.PushBack(pNumComponent); m_Channels.PushBack(NULL);return true;}

//-- Utilities
	bool Generate( bool AutoCalculateVec, bool TransformEvalIntoCP=true );
	void Reset();// Reset will clear everything except m_PatchMesh and m_Exporter
	void SetUVCount(int u, int v) {m_UCount=u; m_VCount=v;}
	void SetControlPointsType(int p) {m_ControlPointsType=p;}

//-- Source Data
	XArray<VxVector>	m_ControlPoints;			// List of all control points
	XArray<VxVector>	m_Vecs;						// List of all Vecs control points
	XArray<VxColor>		m_UVs[MAX_UVCHANNEL_COUNT];	// List corner UVs
	XArray<CKPatch>		m_Patches;					// List of all temporary patches using indices of m_ControlPoints
	XArray<CKPatchEdge >m_Edges;					// List of all temporary edges using indices of m_ControlPoints
	//XArray<int>			m_Corners;			
	//XArray<int>			m_Interior_Edges;
	XArray<CKMaterial*> m_Channels;
	XArray<int>			m_ExtendedChannelsTypes;	// Type 1,2,3,4 for float1 float2 float3 or float 4

//-- Misc Data
	int					m_ChannelCount;
	int					m_IterationLevel;
	int					m_UCount;
	int					m_VCount;
	int					m_ControlPointsType;


protected:
	Export2Virtools*	m_Exporter;		//A pointer to a valid Ge2Virtools with a valid CKContext
	bool ComputeBezierCoeff(float t, float B[4]);
	bool ComputeDiffBezierCoeff(float t, float B[4]);
	void TransformEvaluations();
};

/********************************************************************************
VirtoolsSkin
VirtoolsSkinVertex
 + Structure reprensenting a skin.
 + Use internally as a temporary structure
 + The current bone matrices must be in the bind pose
*******************************************************************************/
class VirtoolsSkinVertex{
public:
	VirtoolsSkinVertex(){}

	VirtoolsSkinVertex(const VirtoolsSkinVertex &v){
		m_Bones= v.m_Bones;
		m_Weights= v.m_Weights;
		m_IntialPos= v.m_IntialPos;
	}

	VirtoolsSkinVertex(CKSkinVertexData* s)
	{
		for(int i=0; i<s->GetBoneCount(); i++){
			m_Bones.PushBack(		s->GetBone(i));
			m_Weights.PushBack(	s->GetWeight(i));
		}

		m_IntialPos= s->GetInitialPos();
	}

	XArray<int>		m_Bones;
	XArray<float>	m_Weights;
	VxVector		m_IntialPos;
};

class VirtoolsSkin{
public:
	VirtoolsSkin(){}
	VirtoolsSkin(CKSkin* s, CK3dEntity* owner)
	{	int i;
		for(i=0; i<s->GetBoneCount(); i++){
			CK3dEntity* e=s->GetBoneData(i)->GetBone();
			m_Bones.PushBack( e );
			m_BonesIntialMatrix.PushBack( e->GetInverseWorldMatrix() );
		}

		for(i=0; i<s->GetVertexCount(); i++){
			CKSkinVertexData* svd=s->GetVertexData(i);
			VirtoolsSkinVertex vsv(svd);
			m_Vertex.PushBack( vsv );
		}

		for(i=0; i<s->GetNormalCount(); i++)
			m_VertexNormals.PushBack( s->GetNormal(i) );

		m_ObjectInitialMatrix= owner->GetInverseWorldMatrix();
	}

	XArray<CK3dEntity*>				m_Bones;
	XArray<VxMatrix>				m_BonesIntialMatrix;
	XClassArray<VirtoolsSkinVertex>	m_Vertex;
	XArray<VxVector>				m_VertexNormals;
	VxMatrix						m_ObjectInitialMatrix;
};

/********************************************************************************
VirtoolsTexture

 + Structure reprensenting a texture with a separated alpha map.
 There is also an opacity member 
 Exporter will create new .tga every time a texture is used with an alpha map.
*******************************************************************************/
class VirtoolsTexture {
public:
	CKTexture*  m_Diffuse;
	CKTexture*	m_Alpha;
	float		m_Opacity;

	CKTexture*	m_RealTexture;
};

/********************************************************************************
VirtoolsMaterialEffect
 +Structure holding generic information of material effect
 +Parameters are stored as string and the CKParameter String function is used for conversion
 +Textures are considered the same as other parameters and you must use texture names.
 +Use the same parameter and effects names as in the Dev Interface... except for "Bump Textur..." parameter in "FloorDotProduct3 Ligthing" effect where you should use "Bump Texture"
 +This can be used as a basis for shader effects to exposed the variables.
*******************************************************************************/
class VirtoolsMaterialEffect {
public:
	void AddParameter( const char* name, const char* value );	
	const char* GetParameter(const char* name);	
	
	XString m_EffectName;
	XHashTable<XString,XString> m_Parameters;
};

/********************************************************************************
VirtoolsMaterial

 + Structure that hold all the available parameters for a CKMaterial
 Exporter can either use this structure to store all their parameters or
 directly used the appropriate CKMaterial methods. The default constructor
 sets all the members to their default value in Virtools so you can only setup your specific needs.
*******************************************************************************/
class VirtoolsMaterial {
public:
	VxColor		m_Diffuse;
	VxColor		m_Specular;
	VxColor		m_Ambient;
	VxColor		m_Emissive;
	float		m_SpecularPower;
	
	CKTexture*  m_Texture;
	VirtoolsMaterialEffect m_Effect;

	BOOL			m_TwoSided;

	VXFILL_MODE		m_FillMode;
	VXSHADE_MODE	m_ShadeMode;

	BOOL			m_AlphaBlending;
	VXBLEND_MODE	m_SrcBlendFactor;
	VXBLEND_MODE	m_DstBlendFactor;
	BOOL			m_PerspectiveCorrection;

	VXTEXTURE_FILTERMODE	m_MinFilterMode;
	VXTEXTURE_FILTERMODE	m_MagFilterMode;
	VXTEXTURE_BLENDMODE		m_TextureBlendMode;

	VXCMPFUNC				m_ZFunc;
	BOOL					m_ZWriteEnable;
	

	VirtoolsMaterial();



	bool AddParameter(char* paramName, CKParameter* p)
	{
		if(paramName==NULL || p==NULL) return false;
		char * buf; int c;
		c =p->GetStringValue(NULL);	
		
		if(c<=0) return false;

		buf = new char[c];
		p->GetStringValue(buf);
		m_Effect.AddParameter(paramName, buf );
		delete [] buf;
		buf=NULL;
		
		return true;
	}
	

	
};


/********************************************************************************
VirtoolsChannelMaterial
+ Specify the properties of a mesh channel.. but linked to a material instead of a mesh 
/*********************************************************************************/
class VirtoolsChannelMaterial{
public:
	VirtoolsChannelMaterial(CKMaterial* mat=NULL, BOOL Active=true, BOOL Lit=true, VXBLEND_MODE srcBlend=VXBLEND_SRCALPHA, VXBLEND_MODE dstBlend=VXBLEND_INVSRCALPHA):m_CKMaterial(mat),m_ChannelActive(Active),m_ChannelLit(Lit), m_SrcBlend(srcBlend), m_DstBlend(dstBlend), m_Channel(-1){}
	void SetChannel(int c){m_Channel=c;}
	int	 GetChannel(){return m_Channel;}
	
	CKMaterial*		m_CKMaterial;
	BOOL			m_ChannelActive;
	BOOL			m_ChannelLit;
	VXBLEND_MODE	m_SrcBlend;
	VXBLEND_MODE	m_DstBlend;
	int				m_Channel;
};

/********************************************************************************
VirtoolsCompositeMaterial
+ A composite material includes multiple Virtools Material to easily create materials
  channels.
+ When more than one composite material are applied to faces, the mesh will be splited in
  smaller ones.
/*********************************************************************************/
class VirtoolsCompositeMaterial{
public:
	VirtoolsCompositeMaterial(){};
	
	VirtoolsCompositeMaterial(CKMaterial* mat, const XArray<CKMaterial*> &subMaterials){
		m_Material= mat;
		for(int i=0; i< subMaterials.Size(); i++)
			AddSubMaterial( subMaterials[i], true, true );
	}

	int AddSubMaterial( CKMaterial* mat, BOOL Active, BOOL Lit)	
	{
		if(mat==NULL) return -1;

		m_SubMaterials.PushBack( VirtoolsChannelMaterial(mat, Active, Lit) );
		
		return m_SubMaterials.Size()-1;
	
	}
	
	int GetSubMaterialCount(){ return m_SubMaterials.Size();}
	
	CKMaterial* m_Material;
	XArray<VirtoolsChannelMaterial>	m_SubMaterials;


};

/********************************************************************************
VirtoolsLevel

 + Structure holding temporary Level Settings such as Ambient Lighting and Fog Settings
*******************************************************************************/
class VirtoolsLevel{
public:
	//Default Constructor
	VirtoolsLevel(){
		m_ClrAmbient		=VxColor(0.f, 0.f, 0.f);

		m_ClrBackGround		=VxColor(128.f, 128.f, 128.f); 
		m_TxtrBackground	=NULL;
					
		m_ClrFog			=VxColor(0.f, 0.f, 0.f);  
		m_FogMode			=VXFOG_NONE; 
		m_FogDensity		=1.f;
		m_FogStart			=1.f;
		m_FogEnd			=100.f;

		m_StartCamera		=NULL;

		m_GlobalTint		=VxColor(1.f, 1.f, 1.f);
		m_TintLevel			=1.f;
	};

	//Member Variables
	VxColor			m_ClrAmbient;

	VxColor			m_ClrBackGround; 
	CKTexture*		m_TxtrBackground;
				
	VxColor			m_ClrFog; 
	VXFOG_MODE		m_FogMode; 
	float			m_FogDensity;
	float			m_FogStart;
	float			m_FogEnd;

	CKCamera*		m_StartCamera;

	//Theses values are from Max but you can use them as global values to modulate all the lights colors
	//The modulation is done in ProduceCMO()
	VxColor			m_GlobalTint;
	float			m_TintLevel;

};

//-----------------------------------------------//
#define FINDHASH(list,type)\
if(list.Size()==0) return NULL;\
XObjectHashTableIt it=list.Find(Key);\
return  (it==list.End()) ? NULL : (type)*it;\

/********************************************************************************
Export2Virtools

This is the main utility class used to generate your data to Virtools format.

Usage:
   - Create your mesh,entity,material,etc...  using the Add(Object) methods. You
   may assign a key (a void* pointer) to the created object so you can access it later
   using the Get(Object)ByKey methods

   - You can create a global animation that will contain all the objects animations
   with CreateGlobalAnimation. 

   - Once all the objects have been createdyou can fill a CKObjectArray of objects
   to load/save with the GenerateObjects methods.

TODO : Utility functions for PatchMesh & Skin creation.
********************************************************************************/
class Export2Virtools {
friend class  VirtoolsTransitionMesh;
public:
	Export2Virtools(CKContext* context, BOOL createLevel);
	BOOL				GenerateObjects(CKObjectArray* array,BOOL AnimationsOnly=FALSE, BOOL ProduceCMO=FALSE);
	
//--- Geometry
	CKMesh*				AddMesh(VirtoolsTransitionMesh* mesh,const char* Name=NULL,void* Key=NULL,CKBOOL KeepTM=TRUE);
//--- TODO A transition Patch (with autosmooth)  to avoid giving the Edges and 	Vec control points
	CKPatchMesh*		AddPatchMesh(const char* Name=NULL,void* Key=NULL);
	CKPatchMesh*		AddPatchMesh(VirtoolsTransitionPatchMesh* patchmesh, const char* Name=NULL,void* Key=NULL/*, CKBOOL KeepTransitionMesh=TRUE*/);

//--- Material & Textures
	CKMaterial*			AddMaterial(VirtoolsMaterial* mat,const char* Name=NULL,void* Key=NULL);
	BOOL				AddMaterial(CKMaterial* ckmat,VirtoolsMaterial* mat,const char* Name=NULL,void* Key=NULL);
	VirtoolsCompositeMaterial*	AddCompositeMaterial( VirtoolsCompositeMaterial cMat);
	CKTexture*			AddTexture(const char* TextureFileName,const char* Name=NULL,void* Key=NULL);
	CKTexture*			AddTexture(int Width,int Height,const char* Name=NULL,void* Key=NULL);
	CKTexture*			AddTextureAlphaMap(CKTexture* colorMap, CKTexture* alphaMap, float alphaMapOpacity);
	int					GenerateMaterialEffect(VirtoolsMaterial* vMat, CKMaterial* ckMat);
	int					GenerateMaterialShader(VirtoolsMaterial* vMat, CKMaterial* ckMat, void (*TextProcessCallBack) (CKMaterial*) );
	

//--- 3D Entities
	CK3dEntity*			AddDummyObject(const char* Name=NULL,void* Key=NULL);
	CK3dEntity*			RegisterDummyObject(CK3dEntity* Dummy, void* Key=NULL);
	CKPlace*			AddPlace(const char* Name=NULL,void* Key=NULL);
	CK3dObject*			Add3dObject(const char* Name=NULL,void* Key=NULL);
	CKSprite3D*			Add3dSprite(const char* Name=NULL,void* Key=NULL);
	CKBodyPart*			AddCharacterBodyPart(const char* Name=NULL,void* Key=NULL);

	CKLight*			AddLight(BOOL Target,VXLIGHT_TYPE Type,const char* Name=NULL,void* Key=NULL, BOOL iCreateTarget=TRUE);
	CKCamera*			AddCamera(BOOL Target,const char* Name=NULL,void* Key=NULL, BOOL iCreateTarget=TRUE);

	CKCurve*			AddCurve(const char* Name=NULL,void* Key=NULL);
	CKCurvePoint*		AddCurvePoint(CKCurve* curve,const char* Name=NULL,void* Key=NULL);

//--- Animation	
	CKObjectAnimation*		AddObjectAnimation(CK3dEntity* AnimatedEntity,const char* Name=NULL,void* Key=NULL);
	void				AddObjectAnimation(CKObjectAnimation* anim,void* Key=NULL);


//---- Utilities
	CKGroup*			AddGroup(const char* Name=NULL,void* Key=NULL);
	CKKeyedAnimation*		CreateGlobalAnimation(const char* Name);	
	CKCharacter*			CreateCharacter(const char* Name);	
	void				SetCharacter(CKCharacter* character);
	
//---- Automatic Mesh Spliting and Channel Application with composite material.	
	void	SetSplitMesh(bool v){m_SplitMesh=v;}
	bool	GetSplitMesh(){return m_SplitMesh;}
	
//---- Parameters Utilities	
	CKParameter* GetObjParameter( CKObject* v, CK_CLASSID iClassID=CKCID_OBJECT);
	CKParameter* GetFloatParameter( float v);
	CKParameter* GetIntParameter( int v);
	CKParameter* GetStringParameter( char* v);
	
//----PostProcess-- May need to be put elsewhere later on---------------------------------------------------------------------------
	int			ForceTextureResize(ETextureResizeMethod Method, bool Square);
	
	//Reduce Animation Keys based on a Threshold
	int			ReduceAnimKeys( CKKeyedAnimation* pAnim, float pThreshold, int pPrecision, EEvalMethod method= eAuto  );
	int			RemoveSingleKeyAnimation( CKKeyedAnimation* pAnim  );

	//ConvertReferential
	int			ConvertObjectReferential( CKObject* obj, EAxis XAxisInVirtools, EAxis YAxisInVirtools, EAxis ZAxisInVirtools );
	int			ConvertAllReferential( EAxis XAxisInVirtools, EAxis YAxisInVirtools, EAxis ZAxisInVirtools );

	//Rescale Scene's Units
	int			RescaleObject(CKObject* obj, float factor);
	int			RescaleScene( float factor);

	//Detect And Share Identical ...
	int			DASIMeshes( CKMesh* p, XObjectPointerArray* s, XArray<CK_ID> &toDelete );
	int			DASIMeshesGlobal();
	int			DASIAnimations( CKObjectAnimation* p, XObjectPointerArray* s );
	int			DASIAnimationsGlobal();
	int			DASIMaterials( CKMaterial* p, XObjectPointerArray* s, XArray<CK_ID> &toDelete  );
	int			DASIMaterialsGlobal();
	int			DASITextures(CKTexture* p, XObjectPointerArray* s, XArray<CK_ID> &toDelete );
	int			DASITexturesGlobal();

	//LOD
	int			FindNamedLOD( CKMesh* p, XObjectPointerArray* s, CKSTRING suffix, int numOfDigits, XArray<CK_ID> &toDelete );
	int			FindNamedLODGlobal( CKSTRING suffix, int numOfDigits );

	//ReadChannels Settings from file
	int			ReadChannels( CKSTRING filename );
	int			GetArgsFromString( const XString &s, XClassArray<XString> &Args );

	//-- Automatic HierarchicalHide on 3DEntity without meshes to speed up culing on characters
	int			AutoHideHierarchyGlobal();

	//Use this to set the level export options
	VirtoolsLevel* GetVirtoolsLevel(){return &m_VirtoolsLevelSettings;}
	
	//The log must be started and closed manually
	GeLog*	m_FileLog;
//---------------------------------------------------------------------------------------------------------------------------------
	

//---- Created Object access (GetEntityByKey works for bodyparts,3DObjects,places and dummy)
	CK3dEntity* GetEntityByKey	(void* Key)	{  FINDHASH(m_Entities	,CK3dEntity*)	}
	CKMesh*		GetMeshByKey	(void* Key)	{  FINDHASH(m_Meshes	,CKMesh*)		} 
	CKMaterial*	GetMaterialByKey(void* Key) {  FINDHASH(m_Materials	,CKMaterial*)	}
	CKTexture*	GetTextureByKey	(void* Key)	{  FINDHASH(m_Textures	,CKTexture*)	}
	CKLight*	GetLightByKey	(void* Key)	{  FINDHASH(m_Lights	,CKLight*)		}
	CKCamera*	GetCameraByKey	(void* Key)	{  FINDHASH(m_Cameras	,CKCamera*)		}
	CKGroup*	GetGroupByKey	(void* Key)	{  FINDHASH(m_Groups	,CKGroup*)		}
	CKObjectAnimation*	GetObjectAnimByKey(void* Key)	{ FINDHASH(m_ObjectAnimations	,CKObjectAnimation*) }
	
	CKCharacter*		GetCharacter()					{ return  m_Character; } 
	CKKeyedAnimation*	GetGlobalAnimation()			{ return  m_GlobalAnimation; } 
	
	VirtoolsTransitionMesh*	GetTransitionMesh(void* Key){
		XIntHashTableIt it=m_TransitionMeshIndices.Find(Key);
		return  (it==m_TransitionMeshIndices.End()) ? NULL : &m_VirtoolsTmpMesh[*it];
	}

	VirtoolsCompositeMaterial*	GetCompositeMaterialByKey(CKMaterial* Key) 
	{  
		XCmpMatHashTableIt it=m_CompositeMaterials.Find(Key);
		if(it==m_CompositeMaterials.End() ) return NULL;

		return  &(m_VirtoolsTmpComposite[*it]);
	}
	

//---- If the created animations should be applied to existing objects or 
//---- to a specific character 
	void ResolveAnimationsTargets(CKCharacter *carac = NULL);

//----- Call this to set specific creation options
	void SetObjectsCreationOptions(CK_OBJECTCREATION_OPTIONS Options) { m_CreationOptions = Options; }

//---- Functions used to create scripts, add attributes, and modify local params
	BOOL Load( CKContext *ctx, char *fileName, CKObjectArray *m_GeneratedArray );
	BOOL AttachScript( CKBeObject *beo, char *scriptName );
	BOOL SetParam( CKBeObject *beo, char *paramName, char *strValue );
	BOOL SetAttribute( CKBeObject *beo, char *attributeName, char *strValue, char *strType, char *strCategory );

	void DestroyObject(CKObject* obj, void* iKey);
//	void DestroyObject(CK_ID id);
//	void DestroyObject(CKObject* obj) {DestroyObject(obj->GetID());};
		
protected:
//-- Created Objects
	XObjectHashTable			m_Entities,m_Meshes,m_Materials,m_Textures,
								m_ObjectAnimations,m_Lights,m_Cameras,m_Groups; 

	XIntHashTable				m_TransitionMeshIndices;

//-- Hash table by filename
	XTextureHashTable			m_TextureByName;	

//-- Hash table to retreive a composite material with the base material
	XCmpMatHashTable			m_CompositeMaterials;

//-- Generated character and animation
	CKCharacter*				m_Character;
	CKKeyedAnimation*			m_GlobalAnimation;

//-- Generated List
	XObjectPointerArray			m_AllObjects;

//--Level Settings for Produce CMO
	VirtoolsLevel				m_VirtoolsLevelSettings;

//-- Internal Data
	XArray<VirtoolsVertex>				m_VerticesPool;
	CKContext*							m_Context;
	XObjectPointerArray					m_OAnimations;
	XObjectPointerArray					m_Bodyparts;
	XClassArray<VirtoolsTransitionMesh> m_VirtoolsTmpMesh;
	XClassArray<VirtoolsCompositeMaterial> m_VirtoolsTmpComposite;
	XArray< VirtoolsTexture >			m_VirtoolsTmpTexture;

//-- Creation options	
	CK_OBJECTCREATION_OPTIONS			m_CreationOptions;
	
//-- Utilities
	int	ProduceCMO( CKObjectArray* array );
	int GetForceSize(int s, ETextureResizeMethod Method);
	void AppendParents( CK3dEntity* e, XArray<CK3dEntity*> &meshParents, int n=-1);
	
//-- ReduceAnimation
	//4 algorithms to reduce keys of an animation controller
	//Last one is an example for new algorithms
	//The Compute fitting function is used to compute a fitting factor to compare algorithms together
	int ReduceAnimController(CKObjectAnimation* oa, float pThreshold, int pPrecision, EAnimType pAnimType, EEvalMethod method );
	CKANIMATION_CONTROLLER RemoveKeysThreshold	 ( CKAnimController* acOriginal, CKAnimController* &acReduce, CKObjectAnimation* oaReduce, float pThreshold );
	CKANIMATION_CONTROLLER ResampleLinear		 ( CKAnimController* acOriginal, CKAnimController* &acReduce, CKObjectAnimation* oaReduce, float pThreshold );
	CKANIMATION_CONTROLLER ResampleTCB			 ( CKAnimController* acOriginal, CKAnimController* &acReduce, CKObjectAnimation* oaReduce, float pThreshold );
	CKANIMATION_CONTROLLER ResampleBezier		 ( CKAnimController* acOriginal, CKAnimController* &acReduce, CKObjectAnimation* oaReduce, float pThreshold );
	CKANIMATION_CONTROLLER ResampleMyNewAlgorithm( CKAnimController* acOriginal, CKAnimController* &acReduce, CKObjectAnimation* oaReduce, float pThreshold );
	float ComputeFitting( CKAnimController* acOriginal, CKAnimController* acNew);
	void WriteExcelAnimController( CKAnimController* ac, const char* file, EAnimType pAnimType, int samplePerKey );

//-- SMBCM: SplitMeshByCompositeMaterial 
	bool				m_SplitMesh;
	int					SplitMeshByCompositeMaterial(CKMesh* toSplit, XArray<CKMesh*> &result);
	int					SplitMeshByCompositeMaterialGlobal();
	CKMesh*				SMBCMExtract(CKMesh* ref, CKMesh* newMesh, CompositeMtlInfo* info);
	CKMesh*				SMBCMFlatten(CKMesh* newMesh, VirtoolsCompositeMaterial*	cm);
	int					SMBCMRelink(XArray<CKMesh*> &result, XArray< CompositeMtlInfo >	&infoArray);
	CKSkin*				SMBCMRelinkSkin(VirtoolsSkin* old, CK3dEntity* ent,  CompositeMtlInfo* info );
	bool				SMBCMRelinkSkinChangeRef( CK3dEntity* cur, CK3dEntity*ent);
	CKObjectAnimation*	SMBCMRelinkMorph(CKObjectAnimation* old, CK3dEntity* ent,  CompositeMtlInfo* info, 	XArray<CKKeyedAnimation*> &AllAnimations );

//-- Component Comparaison utilities
	bool Compare(CKMesh* m1, CKMesh* m2, CKSTRING message);			//replace CKMesh::Compare( CKMesh* m )
	bool Compare(CKMaterial* m1, CKMaterial* m2, CKSTRING message); //replace CKMaterial::Compare( CKMaterial* m )
	bool Compare(CKTexture* t1, CKTexture* t2, CKSTRING message); //replace CKTexture::Compare( CKTexture* t )

//-- LOD Utilities
	bool ExtractNamedLOD( CKSTRING suffix, int numOfDigits, CKSTRING pName, CKSTRING curName );

//-- Cleanup
	CKMaterial*			RemoveMaterialIfNotUse(CKMaterial* m);
	void				RemoveUnusedMaterials();
};




#endif