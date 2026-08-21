

#pragma once
#include <string>
#include <vector>

class Vertex
{
public:
	Vertex(float x = 0.0f, float y = 0.0f, float z = 0.0f, float nx = 0.0f, float ny = 0.0f, float nz = 0.0f);

public:
	float x, y, z;
	float nx, ny, nz;
};

class Vertices
{
public:
	Vertices();

public:
	int m_nID;
	std::vector<Vertex> m_vecVertex;
};

class Texture
{
public:
	Texture();

public:
	int m_nTextureID;
	std::wstring m_strImagePath;
};

class Material
{
public:
	Material();

public:
	int GetElementTag() const;

public:
	int m_nMaterialID;
	std::wstring m_strMaterialName;
	float m_arrDiffuseColor[4];	// rgba
	float m_arrAmbientColor[4];
	float m_arrSpecularColor[4];
	float m_arrEmissiveColor[4];
	int m_nShininess;
	Texture* m_pTexture;

	bool m_useSpecular;
	bool m_useEmissive;
	bool m_useTexture;
};

class Face
{
public:
	Face();

public:
	int v1, v2, v3;
	float v1u, v1v, v2u, v2v, v3u, v3v;
	bool m_useSmoothShading;
	bool m_useCulling;

	float m_fTextureScaleX, m_fTextureScaleY;
	float m_fOffsetX, m_fOffsetY;
};

class Mesh
{
public:
	Mesh();

public:
	int m_nMeshID;
	Vertices* m_p3DVertices;
	Vertices* m_p2DVertices;
	std::vector<Face> m_vec3DFace;
	std::vector<Face> m_vec2DFace;
};

class Layer
{
public:
	enum LayerType {UnknownLayer = 0, FloorLayer, ObjectLayer};

public:
	Layer();

public:
	int m_nLayerID;
	LayerType m_layerType;
	std::wstring m_strLayerName;
	std::wstring m_strDescription;
	Material* m_pMaterial;
	Layer* m_pParentLayer;
	std::vector<Layer*> m_vecChildLayer;
};

class Object
{
public:
	enum ObjectType {UnknownObject = 0, Wall, Space, Facility, Column, Beam, Stair, Elevator, Slab, CurtainWall, Door, Window, Roof, Ceiling, Duct};

	class Attr
	{
	public:
		std::wstring m_strAttrName;
		std::wstring m_strAttrData;
	};

	class ObjectMesh
	{
	public:
		Mesh* m_pMesh;
		float m_arrPosition[3];
		// 객체의 회전값
		// [xAxis x, y, z]
		// [yAxis x, y, z]
		// [zAxis x, y, z]
		float m_arrLocalAxis[9];
		// x, y, z
		float m_arrScale[3];
	};

public:
	Object();

public:
	int m_nObjectID;
	ObjectType m_objType;
	Layer* m_pLayer;
	Material* m_pOwnMaterial;
	std::wstring m_strObjectName;

	std::vector<Attr> m_vecAttr;
	std::vector<ObjectMesh> m_vecMesh;
};

namespace UnE
{
	namespace UBML
	{
		class Reader;
		class Writer;
		class Element;
	}
}

class UBMLFactory
{
public:
	enum UnitOfLength {MM = 0, CM, M, KM, INCH, FEET, MILE};

	static const int _3DVERTICES_BASE_INDEX;
	static const int _2DVERTICES_BASE_INDEX;
	static const int TEXTURE_BASE_INDEX;
	static const int MATERIAL_BASE_INDEX;
	static const int MESH_BASE_INDEX;
	static const int LAYER_BASE_INDEX;
	static const int OBJECT_BASE_INDEX;

	static const int AZIMUTH_TAG;
	static const int ADDRESS_TAG;
	static const int BODY_TAG;
	static const int BUILDING_NAME_TAG;
	static const int COORDINATES_SYSTEM_TAG;
	static const int FACE_TAG;
	static const int HEADER_TAG;
	static const int HEIGHT_ABOVE_SEA_LEVEL_TAG;
	static const int LATITUDE_TAG;
	static const int LONGITUDE_TAG;
	static const int LAYER_GROUP_TAG;
	static const int LAYER_TAG;
	static const int MATERIAL_GROUP_TAG;
	static const int MATERIAL0_TAG;		// Diffuse, Ambient
	static const int MATERIAL10_TAG;	// Diffuse, Ambient, Specular
	static const int MATERIAL20_TAG;	// Diffuse, Ambient, Specular, Emissive
	static const int MATERIAL30_TAG;	// Diffuse, Ambient, Emissive
	static const int MATERIAL40_TAG;	// Diffuse, Ambient, Texture
	static const int MATERIAL50_TAG;	// Diffuse, Ambient, Specular, Texture
	static const int MATERIAL60_TAG;	// Diffuse, Ambient, Specular, Emissive, Texture
	static const int MATERIAL70_TAG;	// Diffuse, Ambient, Emissive, Texture
	static const int MATERIAL_DIFFUSE_TAG;
	static const int MATERIAL_AMBIENT_TAG;
	static const int MATERIAL_SPECULAR_TAG;
	static const int MATERIAL_SHININESS_TAG;
	static const int MATERIAL_TEXTURE_TAG;
	static const int MATERIAL_EMISSIVE_TAG;

	static const int MESH_GROUP_TAG;
	static const int MESH_TAG;

	static const int MATRIX_TAG;

	static const int OBJECT_GROUP_TAG;
	static const int OBJECT_TAG;
	static const int OBJECT_ATTR_GROUP_TAG;
	static const int OBJECT_ATTR_TAG;
	static const int OBJECT_MESH_GROUP_TAG;
	static const int OBJECT_MESH_TAG;
	static const int TEXTURE_GROUP_TAG;
	static const int TEXTURE_TAG;
	static const int _3DVERTICES_GROUP_TAG;
	static const int _3DVERTICES_TAG;
	static const int _3DVERTEX_TAG;
	static const int _2DVERTICES_GROUP_TAG;
	static const int _2DVERTICES_TAG;
	static const int _2DVERTEX_TAG;
	static const int _3DFACES_TAG;
	static const int _2DFACES_TAG;
	static const int TEXTURE_OPTION_TAG;
	static const int UNIT_OF_LENGTH_TAG;
	static const int VERSION_TAG;

public:
	UBMLFactory(void);
	virtual ~UBMLFactory(void);

public:	// General
	const std::wstring& GetErrorString() const;

protected:
	const Texture* FindTexture(int nTextureID) const;
	const Layer* FindLayer(int nLayerID) const;
	const Material* FindMaterial(int nMaterialID) const;
	const Mesh* FindMesh(int nMeshID) const;
	const Vertices* FindVertices(int nVerticesID, bool is3D) const;

public:	// Write
	void SetMeshVersion(std::wstring strMeshVersion);
	void SetUnitOfLength(UnitOfLength nUnit);
	void SetAzimuth(double dDegree);
	void SetLatitude(double dDegree);
	void SetLongitude(double dDegree);
	void SetHeightAboveSeaLevel(double dHeight);
	void SetBuildingName(std::wstring strBuildingName);
	void SetBuildeingAddr(std::wstring strBuildingAddr);
	void SetHandSystem(bool isRight);

	void Add3DVertices(Vertices* pVertices, bool sameCheck = true);
	void Add2DVertices(Vertices* pVertices, bool sameCheck = true);
	void AddTexture(Texture* pTexture, bool sameCheck = true);
	void AddMaterial(Material* pMaterial, bool sameCheck = true);
	void AddMesh(Mesh* pMesh, bool sameCheck = true);
	void AddLayer(Layer* pLayer, bool sameCheck = true);
	void AddObject(Object* pObject, bool sameCheck = true);

	bool Write(std::wstring strPath);

public:	// Read
	bool Read(std::wstring strPath, UnE::UBML::Reader& rReader);

protected:	// Write
	bool MakeHeader(UnE::UBML::Writer& rWriter);
	bool MakeBody(UnE::UBML::Writer& rWriter);

	// Header
	bool MakeVersion(UnE::UBML::Element* pParentElement);
	bool MakeUnitOfLength(UnE::UBML::Element* pParentElement);
	bool MakeAzimuth(UnE::UBML::Element* pParentElement);
	bool MakeLatitude(UnE::UBML::Element* pParentElement);
	bool MakeLongitude(UnE::UBML::Element* pParentElement);
	bool MakeHASL(UnE::UBML::Element* pParentElement);
	bool MakeBuildingName(UnE::UBML::Element* pParentElement);
	bool MakeBuildingAddress(UnE::UBML::Element* pParentElement);
	bool MakeCoordinatesSystem(UnE::UBML::Element* pParentElement);

	// Body(Vertex)
	bool Make3DVerticesGroup(UnE::UBML::Element* pParentElement);
	bool Make3DVertices(Vertices& rVertices, UnE::UBML::Element* pParentElement);
	bool Make3DVertex(Vertex& rVertex, UnE::UBML::Element* pParentElement);
	bool Make2DVerticesGroup(UnE::UBML::Element* pParentElement);
	bool Make2DVertices(Vertices& rVertices, UnE::UBML::Element* pParentElement);
	bool Make2DVertex(Vertex& rVertex, UnE::UBML::Element* pParentElement);

	// Body(Texture)
	bool MakeTextureGroup(UnE::UBML::Element* pParentElement);
	bool MakeTexture(Texture& rTexture, UnE::UBML::Element* pParentElement);

	// Body(Material)
	bool MakeMaterialGroup(UnE::UBML::Element* pParentElement);
	bool MakeMaterial(Material& rMaterial, UnE::UBML::Element* pParentElement);

	// Body(Mesh)
	bool MakeMeshGroup(UnE::UBML::Element* pParentElement);
	bool MakeMesh(Mesh& rMesh, UnE::UBML::Element* pParentElement);
	bool MakeFace(Face& rFace, UnE::UBML::Element* pParentElement);
	bool MakeTextureOption(Face& rFace, UnE::UBML::Element* pParentElement);

	// Body(Layer)
	bool MakeLayerGroup(UnE::UBML::Element* pParentElement);
	bool MakeLayer(Layer& rLayer, UnE::UBML::Element* pParentElement);

	// Body(Object)
	bool MakeObjectGroup(UnE::UBML::Element* pParentElement);
	bool MakeObject(Object& rObject, UnE::UBML::Element* pParentElement);
	bool MakeObjectAttrGroup(Object& rObject, UnE::UBML::Element* pParentElement);
	bool MakeObjectAttr(Object::Attr& rAttr, UnE::UBML::Element* pParentElement);
	bool MakeObjectMeshGroup(Object& rObject, UnE::UBML::Element* pParentElement);
	bool MakeObjectMesh(Object::ObjectMesh& rMesh, UnE::UBML::Element* pParentElement);

protected:	// Read
	// Header
	bool ReadHeader(UnE::UBML::Element& rElement);
	bool ReadMeshVersion(UnE::UBML::Element& rElement);
	bool ReadUnitOfLength(UnE::UBML::Element& rElement);
	bool ReadAzimuth(UnE::UBML::Element& rElement);
	bool ReadLatitude(UnE::UBML::Element& rElement);
	bool ReadLongitude(UnE::UBML::Element& rElement);
	bool ReadHASL(UnE::UBML::Element& rElement);
	bool ReadBuildingName(UnE::UBML::Element& rElement);
	bool ReadAddress(UnE::UBML::Element& rElement);
	bool ReadCoordinatesSystem(UnE::UBML::Element& rElement);

	// Body
	bool ReadBody(UnE::UBML::Element& rElement);

	// Vertex
	bool Read3DVerticesGroup(UnE::UBML::Element& rElement);
	bool Read3DVertices(UnE::UBML::Element& rElement);
	bool Read3DVertex(UnE::UBML::Element& rElement, Vertices& r3DVertices);
	bool Read2DVerticesGroup(UnE::UBML::Element& rElement);
	bool Read2DVertices(UnE::UBML::Element& rElement);
	bool Read2DVertex(UnE::UBML::Element& rElement, Vertices& r2DVertices);

	// Texture
	bool ReadTextureGroup(UnE::UBML::Element& rElement);
	bool ReadTexture(UnE::UBML::Element& rElement);

	// Material
	bool ReadMaterialGroup(UnE::UBML::Element& rElement);
	bool ReadMaterial(UnE::UBML::Element& rElement);
	bool ReadSpecularColor(UnE::UBML::Element& rElement, Material& rMaterial, int& rBeginIndex);
	bool ReadMaterialTexture(UnE::UBML::Element& rElement, Material& rMaterial, int& rBeginIndex);

	// Mesh
	bool ReadMeshGroup(UnE::UBML::Element& rElement);
	bool ReadMesh(UnE::UBML::Element& rElement);
	bool ReadFaces(UnE::UBML::Element& rElement, Mesh& rMesh, bool is3D);
	bool ReadFace(UnE::UBML::Element& rElement, Mesh& rMesh, bool is3D);
	bool ReadTextureOption(UnE::UBML::Element& rElement, Face& rFace);

	// Layer
	bool ReadLayerGroup(UnE::UBML::Element& rElement);
	bool ReadLayer(UnE::UBML::Element& rElement);

	// Object
	bool ReadObjectGroup(UnE::UBML::Element& rElement);
	bool ReadObject(UnE::UBML::Element& rElement);
	bool ReadObjectAttrGroup(UnE::UBML::Element& rElement, Object& rObject);
	bool ReadObjectAttr(UnE::UBML::Element& rElement, Object& rObject);
	bool ReadObjectMeshGroup(UnE::UBML::Element& rElement, Object& rObject);
	bool ReadObjectMesh(UnE::UBML::Element& rElement, Object& rObject);

private:
	std::wstring m_strMeshVersion;
	UnitOfLength m_unitOfLength;
	double m_dAzimuth;	// Degree
	double m_dLatitude;	// 위도(Degree), 0보다 크면 북위, 작으면 남위
	double m_dLongitude;// 경도(Degree)
	double m_dHeightAboveSeaLevel;	// 해발고도(한국 기준)
	std::wstring m_strBuildingName;
	std::wstring m_strBuildingAddress;
	bool m_isRightHandSystem;	// 오른손 좌표계인가?

	std::vector<Vertices*> m_vec3DVertices;
	std::vector<Vertices*> m_vec2DVertices;
	std::vector<Texture*> m_vecTexture;
	std::vector<Material*> m_vecMaterial;
	std::vector<Mesh*> m_vecMesh;
	std::vector<Layer*> m_vecLayer;
	std::vector<Object*> m_vecObject;

	std::wstring m_strError;
};
