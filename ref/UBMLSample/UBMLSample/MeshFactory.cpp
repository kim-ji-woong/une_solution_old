#include "StdAfx.h"
#include "MeshFactory.h"
#include <algorithm>
#include <UBML/UData.h>
#include <UBML/Writer.h>
#include <UBML/Reader.h>
#include <map>
#include <BaseTsd.h>

using namespace UnE;
using namespace UnE::UBML;

const int MeshFactory::_3DVERTICES_BASE_INDEX	= 0;
const int MeshFactory::_2DVERTICES_BASE_INDEX	= 10000000;
const int MeshFactory::TEXTURE_BASE_INDEX		= 20000000;
const int MeshFactory::MATERIAL_BASE_INDEX		= 30000000;
const int MeshFactory::MESH_BASE_INDEX			= 40000000;
const int MeshFactory::LAYER_BASE_INDEX			= 50000000;
const int MeshFactory::OBJECT_BASE_INDEX		= 60000000;

std::map<int, std::wstring> TAG_MAP;
std::map<int, std::wstring> TAG_COMMENT_MAP;



static int MakeTag(const char* strHeader, int num, const wchar_t* strTagDescription)//, const wchar_t* strComment)
{
	Element element;
	if (!element.MakeTag(strHeader, num))
		return 0;

	int nTag = element.GetTag();
	TAG_MAP[nTag] = strTagDescription;
	//TAG_COMMENT_MAP[nTag] = strComment;
	return nTag;
}

const int MeshFactory::AZIMUTH_TAG					= MakeTag("AA", 0, L"방위각");
const int MeshFactory::ADDRESS_TAG					= MakeTag("AB", 0, L"주소");
const int MeshFactory::BODY_TAG						= MakeTag("BA", 0, L"Body");
const int MeshFactory::BUILDING_NAME_TAG			= MakeTag("BB", 0, L"건물명");
const int MeshFactory::COORDINATES_SYSTEM_TAG		= MakeTag("CA", 0, L"좌표계");
const int MeshFactory::FACE_TAG						= MakeTag("FA", 0, L"Face");
const int MeshFactory::HEADER_TAG					= MakeTag("HA", 0, L"Header");
const int MeshFactory::HEIGHT_ABOVE_SEA_LEVEL_TAG	= MakeTag("HB", 0, L"해발고도");
const int MeshFactory::LATITUDE_TAG					= MakeTag("LA", 0, L"위도");
const int MeshFactory::LONGITUDE_TAG				= MakeTag("LB", 0, L"경도");
const int MeshFactory::LAYER_GROUP_TAG				= MakeTag("LC", 0, L"LayerGroup");
const int MeshFactory::LAYER_TAG					= MakeTag("LD", 0, L"Layer");
const int MeshFactory::MATERIAL_GROUP_TAG			= MakeTag("MA", 0, L"MaterialGroup");
const int MeshFactory::MATERIAL0_TAG				= MakeTag("MB", 0, L"Material");		// Diffuse, Ambient
const int MeshFactory::MATERIAL10_TAG				= MakeTag("MB", 10, L"Material");	// Diffuse, Ambient, Specular
const int MeshFactory::MATERIAL20_TAG				= MakeTag("MB", 20, L"Material");	// Diffuse, Ambient, Specular, Emissive
const int MeshFactory::MATERIAL30_TAG				= MakeTag("MB", 30, L"Material");	// Diffuse, Ambient, Emissive
const int MeshFactory::MATERIAL40_TAG				= MakeTag("MB", 40, L"Material");	// Diffuse, Ambient, Texture
const int MeshFactory::MATERIAL50_TAG				= MakeTag("MB", 50, L"Material");	// Diffuse, Ambient, Specular, Texture
const int MeshFactory::MATERIAL60_TAG				= MakeTag("MB", 60, L"Material");	// Diffuse, Ambient, Specular, Emissive, Texture
const int MeshFactory::MATERIAL70_TAG				= MakeTag("MB", 70, L"Material");	// Diffuse, Ambient, Emissive, Texture
const int MeshFactory::MATERIAL_DIFFUSE_TAG			= MakeTag("ME", 0, L"Diffuse");
const int MeshFactory::MATERIAL_AMBIENT_TAG			= MakeTag("MF", 0, L"Ambient");
const int MeshFactory::MATERIAL_SPECULAR_TAG		= MakeTag("MG", 0, L"Specular");
const int MeshFactory::MATERIAL_SHININESS_TAG		= MakeTag("MH", 0, L"Shininess");
const int MeshFactory::MATERIAL_TEXTURE_TAG			= MakeTag("MI", 0, L"Texture");
const int MeshFactory::MATERIAL_EMISSIVE_TAG		= MakeTag("MJ", 0, L"Emissive");
const int MeshFactory::MATRIX_TAG					= MakeTag("MK", 0, L"Matrix");

const int MeshFactory::MESH_GROUP_TAG				= MakeTag("MC", 0, L"MeshGroup");
const int MeshFactory::MESH_TAG						= MakeTag("MD", 0, L"Mesh");

const int MeshFactory::OBJECT_GROUP_TAG				= MakeTag("OA", 0, L"ObjectGroup");
const int MeshFactory::OBJECT_TAG					= MakeTag("OB", 0, L"Object");
const int MeshFactory::OBJECT_ATTR_GROUP_TAG		= MakeTag("OC", 0, L"ObjectAttrGroup");
const int MeshFactory::OBJECT_ATTR_TAG				= MakeTag("OD", 0, L"ObjectAttr");
const int MeshFactory::OBJECT_MESH_GROUP_TAG		= MakeTag("OE", 0, L"ObjectMeshGroup");
const int MeshFactory::OBJECT_MESH_TAG				= MakeTag("OF", 0, L"ObjectMesh");
const int MeshFactory::TEXTURE_GROUP_TAG			= MakeTag("TA", 0, L"TextureGroup");
const int MeshFactory::TEXTURE_TAG					= MakeTag("TB", 0, L"Texture");
const int MeshFactory::_3DVERTICES_GROUP_TAG		= MakeTag("TC", 0, L"VertexGroup");
const int MeshFactory::_3DVERTICES_TAG				= MakeTag("TD", 0, L"VertexList");
const int MeshFactory::_3DVERTEX_TAG				= MakeTag("TE", 0, L"Vertex3D");
const int MeshFactory::_2DVERTICES_GROUP_TAG		= MakeTag("TF", 0, L"_2DVerticesGroup");
const int MeshFactory::_2DVERTICES_TAG				= MakeTag("TG", 0, L"_2DVertices");
const int MeshFactory::_2DVERTEX_TAG				= MakeTag("TH", 0, L"_2DVertex");
const int MeshFactory::_3DFACES_TAG					= MakeTag("TI", 0, L"FaceList");
const int MeshFactory::_2DFACES_TAG					= MakeTag("TJ", 0, L"_2DFaces");
const int MeshFactory::TEXTURE_OPTION_TAG			= MakeTag("TK", 0, L"TextureOption");
const int MeshFactory::UNIT_OF_LENGTH_TAG			= MakeTag("UA", 0, L"길이단위");
const int MeshFactory::VERSION_TAG					= MakeTag("VA", 0, L"Version");


Vertex::Vertex(float x, float y, float z, float nx, float ny, float nz)
{
	this->x = x;
	this->y = y;
	this->z = z;
	this->nx = nx;
	this->ny = ny;
	this->nz = nz;
}

Vertices::Vertices()
{
	m_nID = -1;
}

Texture::Texture()
{
	m_nTextureID = -1;
	m_strImagePath = L"";
}

Material::Material()
{
	m_nMaterialID = -1;
	m_strMaterialName = L"";

	for (int i=0;i<4;i++)
	{
		m_arrDiffuseColor[i] = 1.0f;
		m_arrAmbientColor[i] = 1.0f;
		m_arrSpecularColor[i] = 0.0f;
		m_arrEmissiveColor[i] = 0.0f;
	}

	m_nShininess = 0;
	m_pTexture = 0;

	m_useSpecular = m_useEmissive = m_useTexture = false;
}

int Material::GetElementTag() const
{
	if (m_useSpecular)
	{
		if (m_useEmissive)
		{
			if (m_useTexture)
			{
				return MeshFactory::MATERIAL60_TAG;
			}
			else
			{
				return MeshFactory::MATERIAL20_TAG;
			}
		}
		else
		{
			if (m_useTexture)
			{
				return MeshFactory::MATERIAL50_TAG;
			}
			else
			{
				return MeshFactory::MATERIAL10_TAG;
			}
		}
	}
	else
	{
		if (m_useEmissive)
		{
			if (m_useTexture)
			{
				return MeshFactory::MATERIAL70_TAG;
			}
			else
			{
				return MeshFactory::MATERIAL30_TAG;
			}
		}
		else
		{
			if (m_useTexture)
			{
				return MeshFactory::MATERIAL40_TAG;
			}
			else
			{
				return MeshFactory::MATERIAL0_TAG;
			}
		}
	}

	return 0;
}

Face::Face()
{
	m_useSmoothShading = true;
	m_useCulling = true;

	m_fTextureScaleX = m_fTextureScaleY = 1.0f;
	m_fOffsetX = m_fOffsetY = 0.0f;
}

Mesh::Mesh()
{
	m_nMeshID = -1;
	m_p3DVertices = 0;
	m_p2DVertices = 0;
}

Layer::Layer()
{
	m_nLayerID = -1;
	m_layerType = UnknownLayer;
	m_strLayerName = L"";
	m_strDescription = L"";
	m_pMaterial = 0;
	m_pParentLayer = 0;
}

Object::Object()
{
	m_nObjectID = -1;
	m_objType = UnknownObject;
	m_pLayer = 0;
	m_pOwnMaterial = 0;
	m_strObjectName = L"";
}

MeshFactory::MeshFactory(void)
{
	m_strMeshVersion = L"V1.0";
	m_unitOfLength = MM;
	m_dAzimuth = 0.0;
	m_dLatitude = 37.0;		// 위도(Degree), 0보다 크면 북위, 작으면 남위
	m_dLongitude = 132.0;	// 경도(Degree)
	m_dHeightAboveSeaLevel = 0.0;	// 해발고도(한국 기준)
	m_strBuildingName = L"";
	m_strBuildingAddress = L"";
	m_isRightHandSystem = true;
	m_strError = L"";
}

MeshFactory::~MeshFactory(void)
{
	size_t size3DVertices = m_vec3DVertices.size();
	size_t size2DVertices = m_vec2DVertices.size();
	size_t sizeTexture = m_vecTexture.size();
	size_t sizeMaterial = m_vecMaterial.size();
	size_t sizeMesh = m_vecMesh.size();
	size_t sizeLayer = m_vecLayer.size();
	size_t sizeObject = m_vecObject.size();

	for (size_t i=0;i<size3DVertices;i++)
	{
		delete m_vec3DVertices[i];
	}

	for (size_t i=0;i<size2DVertices;i++)
	{
		delete m_vec2DVertices[i];
	}

	for (size_t i=0;i<sizeTexture;i++)
	{
		delete m_vecTexture[i];
	}

	for (size_t i=0;i<sizeMaterial;i++)
	{
		delete m_vecMaterial[i];
	}

	for (size_t i=0;i<sizeMesh;i++)
	{
		delete m_vecMesh[i];
	}

	for (size_t i=0;i<sizeLayer;i++)
	{
		delete m_vecLayer[i];
	}

	for (size_t i=0;i<sizeObject;i++)
	{
		delete m_vecObject[i];
	}
}

const std::wstring& MeshFactory::GetErrorString() const
{
	return m_strError;
}

const Texture* MeshFactory::FindTexture(int nTextureID) const
{
	int nTextureCount = (int)m_vecTexture.size();

	for (int i=0;i<nTextureCount;i++)
	{
		const Texture* pTexture = m_vecTexture[i];
		if (pTexture == 0) continue;

		if (pTexture->m_nTextureID == nTextureID)
			return pTexture;
	}

	return 0;
}

const Layer* MeshFactory::FindLayer(int nLayerID) const
{
	int nLayerCount = (int)m_vecLayer.size();

	for (int i=0;i<nLayerCount;i++)
	{
		const Layer* pLayer = m_vecLayer[i];
		if (pLayer == 0) continue;

		if (pLayer->m_nLayerID == nLayerID)
			return pLayer;
	}

	return 0;
}

const Material* MeshFactory::FindMaterial(int nMaterialID) const
{
	int nMaterialCount = (int)m_vecMaterial.size();

	for (int i=0;i<nMaterialCount;i++)
	{
		const Material* pMaterial = m_vecMaterial[i];
		if (pMaterial == 0) continue;

		if (pMaterial->m_nMaterialID == nMaterialID)
			return pMaterial;
	}

	return 0;
}

const Mesh* MeshFactory::FindMesh(int nMeshID) const
{
	int nMeshCount = (int)m_vecMesh.size();

	for (int i=0;i<nMeshCount;i++)
	{
		const Mesh* pMesh = m_vecMesh[i];
		if (pMesh == 0) continue;

		if (pMesh->m_nMeshID == nMeshID)
			return pMesh;
	}

	return 0;
}

const Vertices* MeshFactory::FindVertices(int nVerticesID, bool is3D) const
{
	const std::vector<Vertices*>& rVecVertices = is3D ? m_vec3DVertices : m_vec2DVertices;

	int nVerticesCount = (int)rVecVertices.size();

	for (int i=0;i<nVerticesCount;i++)
	{
		const Vertices* pVertices = rVecVertices[i];
		if (pVertices == 0) continue;

		if (pVertices->m_nID == nVerticesID)
			return pVertices;
	}

	return 0;
}

void MeshFactory::SetMeshVersion(std::wstring strMeshVersion)
{
	m_strMeshVersion = strMeshVersion;
}

void MeshFactory::SetUnitOfLength(UnitOfLength nUnit)
{
	m_unitOfLength = nUnit;
}
//127.04804420
//37.18192868
void MeshFactory::SetAzimuth(double dDegree)
{
	m_dAzimuth = dDegree;
}

void MeshFactory::SetLatitude(double dDegree)
{
	m_dLatitude = dDegree;
}

void MeshFactory::SetLongitude(double dDegree)
{
	m_dLongitude = dDegree;
}

void MeshFactory::SetHeightAboveSeaLevel(double dHeight)
{
	m_dHeightAboveSeaLevel = dHeight;
}

void MeshFactory::SetBuildingName(std::wstring strBuildingName)
{
	m_strBuildingName = strBuildingName;
}

void MeshFactory::SetBuildeingAddr(std::wstring strBuildingAddr)
{
	m_strBuildingAddress = strBuildingAddr;
}

void MeshFactory::SetHandSystem(bool isRight)
{
	m_isRightHandSystem = isRight;
}

void MeshFactory::Add3DVertices(Vertices* pVertices, bool sameCheck)
{
	if (pVertices == 0) return;

	if (!sameCheck) m_vec3DVertices.push_back(pVertices);
	else
	{
		if (std::find(m_vec3DVertices.begin(), m_vec3DVertices.end(), pVertices) == m_vec3DVertices.end())
		{
			m_vec3DVertices.push_back(pVertices);
		}
	}
}

void MeshFactory::Add2DVertices(Vertices* pVertices, bool sameCheck)
{
	if (pVertices == 0) return;

	if (!sameCheck) m_vec2DVertices.push_back(pVertices);
	else
	{
		if (std::find(m_vec2DVertices.begin(), m_vec2DVertices.end(), pVertices) == m_vec2DVertices.end())
		{
			m_vec2DVertices.push_back(pVertices);
		}
	}
}

void MeshFactory::AddTexture(Texture* pTexture, bool sameCheck)
{
	if (pTexture == 0) return;

	if (!sameCheck) m_vecTexture.push_back(pTexture);
	else
	{
		if (std::find(m_vecTexture.begin(), m_vecTexture.end(), pTexture) == m_vecTexture.end())
		{
			m_vecTexture.push_back(pTexture);
		}
	}
}

void MeshFactory::AddMaterial(Material* pMaterial, bool sameCheck)
{
	if (pMaterial == 0) return;

	if (!sameCheck) m_vecMaterial.push_back(pMaterial);
	else
	{
		if (std::find(m_vecMaterial.begin(), m_vecMaterial.end(), pMaterial) == m_vecMaterial.end())
		{
			m_vecMaterial.push_back(pMaterial);
		}
	}
}

void MeshFactory::AddMesh(Mesh* pMesh, bool sameCheck)
{
	if (pMesh == 0) return;

	if (!sameCheck) m_vecMesh.push_back(pMesh);
	else
	{
		if (std::find(m_vecMesh.begin(), m_vecMesh.end(), pMesh) == m_vecMesh.end())
		{
			m_vecMesh.push_back(pMesh);
		}
	}
}

void MeshFactory::AddLayer(Layer* pLayer, bool sameCheck)
{
	if (pLayer == 0) return;

	if (!sameCheck) m_vecLayer.push_back(pLayer);
	else
	{
		if (std::find(m_vecLayer.begin(), m_vecLayer.end(), pLayer) == m_vecLayer.end())
		{
			m_vecLayer.push_back(pLayer);
		}
	}
}

void MeshFactory::AddObject(Object* pObject, bool sameCheck)
{
	if (pObject == 0) return;

	if (!sameCheck) m_vecObject.push_back(pObject);
	else
	{
		if (std::find(m_vecObject.begin(), m_vecObject.end(), pObject) == m_vecObject.end())
		{
			m_vecObject.push_back(pObject);
		}
	}
}

bool MeshFactory::Write(std::wstring strPath)
{
	int size3DVertices = (int)m_vec3DVertices.size();
	int size2DVertices = (int)m_vec2DVertices.size();
	int sizeTexture = (int)m_vecTexture.size();
	int sizeMaterial = (int)m_vecMaterial.size();
	int sizeMesh = (int)m_vecMesh.size();
	int sizeLayer = (int)m_vecLayer.size();
	int sizeObject = (int)m_vecObject.size();

	for (int i=0;i<size3DVertices;i++)
	{
		m_vec3DVertices[i]->m_nID = i + _3DVERTICES_BASE_INDEX;
	}

	for (int i=0;i<size2DVertices;i++)
	{
		m_vec2DVertices[i]->m_nID = i + _2DVERTICES_BASE_INDEX;
	}

	for (int i=0;i<sizeTexture;i++)
	{
		m_vecTexture[i]->m_nTextureID = i + TEXTURE_BASE_INDEX;
	}

	for (int i=0;i<sizeMaterial;i++)
	{
		m_vecMaterial[i]->m_nMaterialID = i + MATERIAL_BASE_INDEX;
	}

	for (int i=0;i<sizeMesh;i++)
	{
		m_vecMesh[i]->m_nMeshID = i + MESH_BASE_INDEX;
	}

	for (int i=0;i<sizeLayer;i++)
	{
		m_vecLayer[i]->m_nLayerID = i + LAYER_BASE_INDEX;
	}

	for (int i=0;i<sizeObject;i++)
	{
		m_vecObject[i]->m_nObjectID = i + OBJECT_BASE_INDEX;
	}

	Writer writer;

	if (!MakeHeader(writer))
		return false;

	if (!MakeBody(writer))
		return false;
		
	writer.ToPrettyXML(L"pretty_sample.xml");
	//writer.ToXML(L"sample.xml");

	return writer.WriteFile(strPath.c_str());
}

bool MeshFactory::MakeHeader(Writer& rWriter)
{
	Element* pHeader = new Element(HEADER_TAG);
	pHeader->SetDescription(TAG_MAP[HEADER_TAG]);
	if (!MakeVersion(pHeader))
		goto RETURN_FALSE;
	if (!MakeUnitOfLength(pHeader))
		goto RETURN_FALSE;
	if (!MakeAzimuth(pHeader))
		goto RETURN_FALSE;
	if (!MakeLatitude(pHeader))
		goto RETURN_FALSE;
	if (!MakeLongitude(pHeader))
		goto RETURN_FALSE;
	if (!MakeHASL(pHeader))
		goto RETURN_FALSE;
	if (!MakeBuildingName(pHeader))
		goto RETURN_FALSE;
	if (!MakeBuildingAddress(pHeader))
		goto RETURN_FALSE;
	if (!MakeCoordinatesSystem(pHeader))
		goto RETURN_FALSE;

	rWriter.AddElement(pHeader);
	return true;

RETURN_FALSE:
	delete pHeader;
	return false;
}

bool MeshFactory::MakeVersion(Element* pParentElement)
{
	Element* pElement = new Element(VERSION_TAG);
	pElement->SetDescription(TAG_MAP[VERSION_TAG]);

	Segment* pSegment = new Segment(__WCHAR_ARR);
	
	int nLen = (int)m_strMeshVersion.length();

	for (int i=0;i<nLen;i++)
	{
		pSegment->AddData(m_strMeshVersion.at(i));
	}
	pSegment->SetTagDescription(L"Text");
	pElement->AddData(pSegment);
	pParentElement->AddData(pElement);
	return true;
}

bool MeshFactory::MakeUnitOfLength(Element* pParentElement)
{
	Element* pElement = new Element(UNIT_OF_LENGTH_TAG);
	pElement->SetDescription(TAG_MAP[UNIT_OF_LENGTH_TAG]);

	Segment* pSegment = new Segment(__LONG);
	pSegment->AddData((int)m_unitOfLength);
	pSegment->SetTagDescription(L"Text");
	pElement->AddData(pSegment);
	pParentElement->AddData(pElement);
	return true;
}

bool MeshFactory::MakeAzimuth(Element* pParentElement)
{
	Element* pElement = new Element(AZIMUTH_TAG);
	pElement->SetDescription(TAG_MAP[AZIMUTH_TAG]);

	Segment* pSegment = new Segment(__DOUBLE);
	pSegment->AddData(m_dAzimuth);
	pSegment->SetTagDescription(L"Text");
	pElement->AddData(pSegment);
	pParentElement->AddData(pElement);
	return true;
}

bool MeshFactory::MakeLatitude(Element* pParentElement)
{
	Element* pElement = new Element(LATITUDE_TAG);
	pElement->SetDescription(TAG_MAP[LATITUDE_TAG]);

	Segment* pSegment = new Segment(__DOUBLE);
	pSegment->AddData(m_dLatitude);
	pSegment->SetTagDescription(L"Text");
	pElement->AddData(pSegment);
	pParentElement->AddData(pElement);
	return true;
}

bool MeshFactory::MakeLongitude(Element* pParentElement)
{
	Element* pElement = new Element(LONGITUDE_TAG);
	pElement->SetDescription(TAG_MAP[LONGITUDE_TAG]);

	Segment* pSegment = new Segment(__DOUBLE);
	pSegment->AddData(m_dLongitude);
	pSegment->SetTagDescription(L"Text");
	pElement->AddData(pSegment);
	pParentElement->AddData(pElement);
	return true;
}

bool MeshFactory::MakeHASL(Element* pParentElement)
{
	Element* pElement = new Element(HEIGHT_ABOVE_SEA_LEVEL_TAG);
	pElement->SetDescription(TAG_MAP[HEIGHT_ABOVE_SEA_LEVEL_TAG]);

	Segment* pSegment = new Segment(__DOUBLE);
	pSegment->AddData(m_dHeightAboveSeaLevel);
	pSegment->SetTagDescription(L"Text");
	pElement->AddData(pSegment);
	pParentElement->AddData(pElement);
	return true;
}

bool MeshFactory::MakeBuildingName(Element* pParentElement)
{
	Element* pElement = new Element(BUILDING_NAME_TAG);
	pElement->SetDescription(TAG_MAP[BUILDING_NAME_TAG]);

	Segment* pSegment = new Segment(__WCHAR_ARR);
	
	int nLen = (int)m_strBuildingName.length();

	for (int i=0;i<nLen;i++)
	{
		pSegment->AddData(m_strBuildingName.at(i));
	}
	pSegment->SetTagDescription(L"Text");
	pElement->AddData(pSegment);
	pParentElement->AddData(pElement);
	return true;
}

bool MeshFactory::MakeBuildingAddress(Element* pParentElement)
{
	Element* pElement = new Element(ADDRESS_TAG);
	pElement->SetDescription(TAG_MAP[ADDRESS_TAG]);

	Segment* pSegment = new Segment(__WCHAR_ARR);
	
	int nLen = (int)m_strBuildingAddress.length();

	for (int i=0;i<nLen;i++)
	{
		pSegment->AddData(m_strBuildingAddress.at(i));
	}
	pSegment->SetTagDescription(L"Text");
	pElement->AddData(pSegment);
	pParentElement->AddData(pElement);
	return true;
}

bool MeshFactory::MakeCoordinatesSystem(Element* pParentElement)
{
	Element* pElement = new Element(COORDINATES_SYSTEM_TAG);
	pElement->SetDescription(TAG_MAP[COORDINATES_SYSTEM_TAG]);

	Segment* pSegment = new Segment(__BOOL);
	pSegment->AddData(m_isRightHandSystem);
	pSegment->SetTagDescription(L"Text");
	pElement->AddData(pSegment);
	pParentElement->AddData(pElement);
	return true;
}

bool MeshFactory::MakeBody(Writer& rWriter)
{
	Element* pBody = new Element(BODY_TAG);
	pBody->SetDescription(TAG_MAP[BODY_TAG]);

	if (!Make3DVerticesGroup(pBody))
		goto RETURN_FALSE;
	//if (!Make2DVerticesGroup(pBody))
	//	goto RETURN_FALSE;
	if (!MakeTextureGroup(pBody))
		goto RETURN_FALSE;
	if (!MakeMaterialGroup(pBody))
		goto RETURN_FALSE;
	if (!MakeMeshGroup(pBody))
		goto RETURN_FALSE;
	if (!MakeLayerGroup(pBody))
		goto RETURN_FALSE;
	if (!MakeObjectGroup(pBody))
		goto RETURN_FALSE;

	rWriter.AddElement(pBody);
	return true;

RETURN_FALSE:
	delete pBody;
	return false;
}

bool MeshFactory::Make3DVerticesGroup(Element* pParentElement)
{
	Element* p3DVerticesGroup = new Element(_3DVERTICES_GROUP_TAG);
	p3DVerticesGroup->SetDescription(TAG_MAP[_3DVERTICES_GROUP_TAG]);

	unsigned int nGroupSize = m_vec3DVertices.size();

	for (unsigned int i=0;i<nGroupSize;i++)
	{
		Vertices* pVertices = m_vec3DVertices[i];
		if (pVertices == 0) continue;

		if (!Make3DVertices(*pVertices, p3DVerticesGroup))
		{
			delete p3DVerticesGroup;
			return false;
		}
	}

	pParentElement->AddData(p3DVerticesGroup);
	return true;
}

bool MeshFactory::Make3DVertices(Vertices& rVertices, Element* pParentElement)
{
	Element* p3DVertices = new Element(_3DVERTICES_TAG);
	p3DVertices->SetDescription(TAG_MAP[_3DVERTICES_TAG]);

	Segment* pVerticesID = new Segment(__LONG);
	pVerticesID->AddData(rVertices.m_nID);
	pVerticesID->SetTagName(L"id");
	pVerticesID->SetTagDescription(L"Attr");

	p3DVertices->AddData(pVerticesID);
	
	int nVertexCount = (int)rVertices.m_vecVertex.size();

	for (int i=0;i<nVertexCount;i++)
	{
		if (!Make3DVertex(rVertices.m_vecVertex[i], p3DVertices))
		{
			delete p3DVertices;
			return false;
		}
	}

	pParentElement->AddData(p3DVertices);
	return true;
}

bool MeshFactory::Make3DVertex(Vertex& rVertex, Element* pParentElement)
{
	Element* p3DVertex = new Element(_3DVERTEX_TAG);
	p3DVertex->SetDescription(TAG_MAP[_3DVERTEX_TAG]);


	Segment* pX = new Segment(__FLOAT);
	pX->AddData(rVertex.x);
	pX->SetTagName(L"x");
	pX->SetTagDescription(L"Attr");

	Segment* pY = new Segment(__FLOAT);
	pY->AddData(rVertex.y);
	pY->SetTagName(L"y");
	pY->SetTagDescription(L"Attr");

	Segment* pZ = new Segment(__FLOAT);
	pZ->AddData(rVertex.z);
	pZ->SetTagName(L"z");
	pZ->SetTagDescription(L"Attr");

	Segment* pNX = new Segment(__FLOAT);
	pNX->AddData(rVertex.nx);
	pNX->SetTagName(L"nx");
	pNX->SetTagDescription(L"Attr");

	Segment* pNY = new Segment(__FLOAT);
	pNY->AddData(rVertex.ny);
	pNY->SetTagName(L"ny");
	pNY->SetTagDescription(L"Attr");

	Segment* pNZ = new Segment(__FLOAT);
	pNZ->AddData(rVertex.nz);
	pNZ->SetTagName(L"nz");
	pNZ->SetTagDescription(L"Attr");

	p3DVertex->AddData(pX);
	p3DVertex->AddData(pY);
	p3DVertex->AddData(pZ);
	p3DVertex->AddData(pNX);
	p3DVertex->AddData(pNY);
	p3DVertex->AddData(pNZ);

	pParentElement->AddData(p3DVertex);
	return true;
}

bool MeshFactory::Make2DVerticesGroup(Element* pParentElement)
{
	Element* p2DVerticesGroup = new Element(_2DVERTICES_GROUP_TAG);
	p2DVerticesGroup->SetDescription(TAG_MAP[_2DVERTICES_GROUP_TAG]);

	unsigned int nGroupSize = m_vec2DVertices.size();

	for (unsigned int i=0;i<nGroupSize;i++)
	{
		Vertices* pVertices = m_vec2DVertices[i];
		if (pVertices == 0) continue;

		if (!Make2DVertices(*pVertices, p2DVerticesGroup))
		{
			delete p2DVerticesGroup;
			return false;
		}
	}

	pParentElement->AddData(p2DVerticesGroup);
	return true;
}

bool MeshFactory::Make2DVertices(Vertices& rVertices, Element* pParentElement)
{
	Element* p2DVertices = new Element(_2DVERTICES_TAG);
	p2DVertices->SetDescription(TAG_MAP[_2DVERTICES_TAG]);

	Segment* pVerticesID = new Segment(__LONG);
	pVerticesID->AddData(rVertices.m_nID);

	p2DVertices->AddData(pVerticesID);

	int nVertexCount = (int)rVertices.m_vecVertex.size();

	for (int i=0;i<nVertexCount;i++)
	{
		if (!Make2DVertex(rVertices.m_vecVertex[i], p2DVertices))
		{
			delete p2DVertices;
			return false;
		}
	}

	pParentElement->AddData(p2DVertices);
	return true;
}

bool MeshFactory::Make2DVertex(Vertex& rVertex, Element* pParentElement)
{
	Element* p2DVertex = new Element(_2DVERTEX_TAG);
	p2DVertex->SetDescription(TAG_MAP[_2DVERTEX_TAG]);

	Segment* pX = new Segment(__FLOAT);
	pX->AddData(rVertex.x);

	Segment* pY = new Segment(__FLOAT);
	pY->AddData(rVertex.y);

	p2DVertex->AddData(pX);
	p2DVertex->AddData(pY);
	
	pParentElement->AddData(p2DVertex);
	return true;
}

bool MeshFactory::MakeTextureGroup(Element* pParentElement)
{
	Element* pTextureGroup = new Element(TEXTURE_GROUP_TAG);
	pTextureGroup->SetDescription(TAG_MAP[TEXTURE_GROUP_TAG]);

	int nTextureCount = (int)m_vecTexture.size();

	for (int i=0;i<nTextureCount;i++)
	{
		Texture* pTexture = m_vecTexture[i];
		if (pTexture == 0) continue;

		if (!MakeTexture(*pTexture, pTextureGroup))
		{
			delete pTextureGroup;
			return false;
		}
	}

	pParentElement->AddData(pTextureGroup);
	return true;
}

bool MeshFactory::MakeTexture(Texture& rTexture, Element* pParentElement)
{
	Element* pTexture = new Element(TEXTURE_TAG);
	pTexture->SetDescription(TAG_MAP[TEXTURE_TAG]);

	Segment* pTextureID = new Segment(__LONG);
	pTextureID->AddData(rTexture.m_nTextureID);
	pTextureID->SetTagName(L"id");
	pTextureID->SetTagDescription(L"Attr");

	Segment* pTexturePath = new Segment(__WCHAR_ARR);
	//pTexturePath->SetName(L"path");
	pTexturePath->SetTagDescription(L"Text");

	int nLen = (int)rTexture.m_strImagePath.length();

	for (int i=0;i<nLen;i++)
	{
		pTexturePath->AddData(rTexture.m_strImagePath.at(i));
	}

	pTexture->AddData(pTextureID);
	pTexture->AddData(pTexturePath);
	pParentElement->AddData(pTexture);

	return true;
}

bool MeshFactory::MakeMaterialGroup(Element* pParentElement)
{
	Element* pMaterialGroup = new Element(MATERIAL_GROUP_TAG);
	pMaterialGroup->SetDescription(TAG_MAP[MATERIAL_GROUP_TAG]);

	int nMaterialCount = (int)m_vecMaterial.size();

	for (int i=0;i<nMaterialCount;i++)
	{
		Material* pMaterial = m_vecMaterial[i];
		if (pMaterial == 0) continue;

		if (!MakeMaterial(*pMaterial, pMaterialGroup))
		{
			delete pMaterialGroup;
			return false;
		}
	}

	pParentElement->AddData(pMaterialGroup);
	return true;
}


bool MeshFactory::MakeMaterial(Material& rMaterial, Element* pParentElement)
{
	int nElementTag = rMaterial.GetElementTag();
	Element* pMaterial = new Element(nElementTag);
	pMaterial->SetDescription(TAG_MAP[nElementTag]);

	Segment* pMaterialID = new Segment(__LONG);
	pMaterialID->AddData(rMaterial.m_nMaterialID);
	pMaterialID->SetTagName(L"id");
	pMaterialID->SetTagDescription(L"Attr");

	Segment* pMaterialName = new Segment(__WCHAR_ARR);
	pMaterialName->SetTagName(L"name");
	pMaterialName->SetTagDescription(L"Attr");

	int nNameLen = (int)rMaterial.m_strMaterialName.length();

	for (int i=0;i<nNameLen;i++)
	{
		pMaterialName->AddData(rMaterial.m_strMaterialName.at(i));
	}

	Element * pDiffuse = new Element(MATERIAL_DIFFUSE_TAG);
	pDiffuse->SetDescription(TAG_MAP[MATERIAL_DIFFUSE_TAG]);

	Element * pAmbient = new Element(MATERIAL_AMBIENT_TAG);
	pAmbient->SetDescription(TAG_MAP[MATERIAL_AMBIENT_TAG]);
	
	pMaterial->AddData(pMaterialID);
	pMaterial->AddData(pMaterialName);

	// ADD AMBIENT, DIFFUSE
	std::wstring szTemp[4] = { L"r", L"g", L"b", L"a"};
	float        fEmDefaultValue[4] = { 0.0f, 0.0f, 0.0f , 0.0f};
	for (int i=0;i<4;i++)
	{
		Segment* pDiffuseSeg = new Segment(__FLOAT);		
		pDiffuseSeg->SetTagDescription(L"Attr");
		pDiffuseSeg->SetTagName(szTemp[i]);
		pDiffuseSeg->AddData(rMaterial.m_arrDiffuseColor[i]);

		pDiffuse->AddData(pDiffuseSeg);

		Segment* pAmbientSeg = new Segment(__FLOAT);
		pAmbientSeg->SetTagName(szTemp[i]);
		pAmbientSeg->SetTagDescription(L"Attr");		
		pAmbientSeg->AddData(rMaterial.m_arrAmbientColor[i]);

		pAmbient->AddData(pAmbientSeg);
	}
	pMaterial->AddData(pDiffuse);
	pMaterial->AddData(pAmbient);

	if (rMaterial.m_useSpecular)
	{
		Element * pSpecular = new Element(MATERIAL_SPECULAR_TAG);
		pSpecular->SetDescription(TAG_MAP[MATERIAL_SPECULAR_TAG]);
		
		for (int i=0;i<4;i++)
		{
			Segment* pSpecularSeg = new Segment(__FLOAT);
			pSpecularSeg->SetTagName(szTemp[i]);
			pSpecularSeg->SetTagDescription(L"Attr");
			pSpecularSeg->AddData(rMaterial.m_arrSpecularColor[i]);

			pSpecular->AddData(pSpecularSeg);
		}

		Element * pShininess = new Element(MATERIAL_SHININESS_TAG);
		pShininess->SetDescription(TAG_MAP[MATERIAL_SHININESS_TAG]);
		
		Segment* pShininessSeg = new Segment(__LONG);
		pShininessSeg->AddData(rMaterial.m_nShininess);
		pShininessSeg->SetTagDescription(L"Text");
		pShininess->AddData(pShininessSeg);

		
		pSpecular->AddData(pShininess);
		pMaterial->AddData(pSpecular);
	}

	
	if (rMaterial.m_useEmissive)
	{
		Element * pEmissive = new Element(MATERIAL_EMISSIVE_TAG);
		pEmissive->SetDescription(TAG_MAP[MATERIAL_EMISSIVE_TAG]);

		for (int i = 0; i<4; i++)
		{
			Segment* pEmissiveSeg = new Segment(__FLOAT_ARR);
			pEmissiveSeg->SetTagName(szTemp[i]);
			pEmissiveSeg->SetTagDescription(L"Attr");
			if (rMaterial.m_useEmissive)
				pEmissiveSeg->AddData(rMaterial.m_arrEmissiveColor[i]);
			else
				pEmissiveSeg->AddData(fEmDefaultValue[i]);	

			pEmissive->AddData(pEmissiveSeg);
		}

		pMaterial->AddData(pEmissive);
	}

	if (rMaterial.m_useTexture && rMaterial.m_pTexture)
	{
		Element * pTexture = new Element(MATERIAL_TEXTURE_TAG);
		pTexture->SetDescription(TAG_MAP[MATERIAL_TEXTURE_TAG]);
		
		if(rMaterial.m_useTexture && rMaterial.m_pTexture)
		{
			Segment* pTextureSeg = new Segment(__LONG);		
			pTextureSeg->SetTagDescription(L"Text");
			pTextureSeg->AddData(rMaterial.m_pTexture->m_nTextureID);
			pTexture->AddData(pTextureSeg);
		}	

		Segment* pTextureSeg2 = new Segment(__BOOL);
		pTextureSeg2->SetTagName(L"use");
		pTextureSeg2->SetTagDescription(L"Attr");
		pTextureSeg2->AddData(rMaterial.m_useTexture == true ? 1 : 0);		
		pTexture->AddData(pTextureSeg2);
		pMaterial->AddData(pTexture);
	}

	pParentElement->AddData(pMaterial);
	return true;
}

//
//bool MeshFactory::MakeMaterial(Material& rMaterial, Element* pParentElement)
//{
//	int nElementTag = rMaterial.GetElementTag();
//	Element* pMaterial = new Element(nElementTag);
//	pMaterial->SetDescription(TAG_MAP[nElementTag]);
//
//	Segment* pMaterialID = new Segment(__LONG);
//	pMaterialID->AddData(rMaterial.m_nMaterialID);
//	pMaterialID->SetName(L"id");
//	pMaterialID->SetDescription(L"Attr");
//
//	Segment* pMaterialName = new Segment(__WCHAR_ARR);
//	pMaterialName->SetName(L"name");
//	pMaterialName->SetDescription(L"Attr");
//
//	int nNameLen = (int)rMaterial.m_strMaterialName.length();
//
//	for (int i=0;i<nNameLen;i++)
//	{
//		pMaterialName->AddData(rMaterial.m_strMaterialName.at(i));
//	}
//	
//	Segment* pDiffuse = new Segment(__FLOAT_ARR);
//	pDiffuse->SetName(L"diffuse");
//	pDiffuse->SetDescription(L"Attr");
//
//	Segment* pAmbient = new Segment(__FLOAT_ARR);
//	pAmbient->SetName(L"ambient");
//	pAmbient->SetDescription(L"Attr");
//
//	pMaterial->AddData(pMaterialID);
//	pMaterial->AddData(pMaterialName);
//	pMaterial->AddData(pDiffuse);
//	pMaterial->AddData(pAmbient);
//
//	for (int i=0;i<4;i++)
//	{
//		pDiffuse->AddData(rMaterial.m_arrDiffuseColor[i]);
//		pAmbient->AddData(rMaterial.m_arrAmbientColor[i]);
//	}
//
//	if (rMaterial.m_useSpecular)
//	{
//		Segment* pSpecular = new Segment(__FLOAT_ARR);
//		pSpecular->SetName(L"specular");
//		pSpecular->SetDescription(L"Attr");
//
//		for (int i=0;i<4;i++)
//		{
//			pSpecular->AddData(rMaterial.m_arrSpecularColor[i]);
//		}
//
//		Segment* pShininess = new Segment(__LONG);
//		pShininess->AddData(rMaterial.m_nShininess);
//		pShininess->SetName(L"shininess");
//		pShininess->SetDescription(L"Attr");
//
//		pMaterial->AddData(pSpecular);
//		pMaterial->AddData(pShininess);
//	}
//
//	if (rMaterial.m_useEmissive)
//	{
//		Segment* pEmissive = new Segment(__FLOAT_ARR);
//		pEmissive->SetName(L"emissive");
//		pEmissive->SetDescription(L"Attr");
//		for (int i=0;i<4;i++)
//		{
//			pEmissive->AddData(rMaterial.m_arrEmissiveColor[i]);
//		}
//
//		pMaterial->AddData(pEmissive);
//	}
//
//	if (rMaterial.m_useTexture && rMaterial.m_pTexture)
//	{
//		Segment* pTexture = new Segment(__LONG);
//		pTexture->SetName(L"texture");
//		pTexture->SetDescription(L"Attr");
//
//		pTexture->AddData(rMaterial.m_pTexture->m_nTextureID);
//		pMaterial->AddData(pTexture);
//	}
//
//	pParentElement->AddData(pMaterial);
//	return true;
//}

bool MeshFactory::MakeMeshGroup(Element* pParentElement)
{
	Element* pMeshGroup = new Element(MESH_GROUP_TAG);
	pMeshGroup->SetDescription(TAG_MAP[MESH_GROUP_TAG]);
	
	int nMeshCount = (int)m_vecMesh.size();

	for (int i=0;i<nMeshCount;i++)
	{
		Mesh* pMesh = m_vecMesh[i];
		if (pMesh == 0) continue;

		if (!MakeMesh(*pMesh, pMeshGroup))
		{
			delete pMeshGroup;
			return false;
		}
	}

	pParentElement->AddData(pMeshGroup);
	return true;
}

bool MeshFactory::MakeMesh(Mesh& rMesh, Element* pParentElement)
{
	Element* pMesh = new Element(MESH_TAG);
	pMesh->SetDescription(TAG_MAP[MESH_TAG]);

	Segment* pMeshID = new Segment(__LONG);
	pMeshID->AddData(rMesh.m_nMeshID);
	pMeshID->SetTagName(L"id");
	pMeshID->SetTagDescription(L"Attr");	

	Element* p3DFaces = new Element(_3DFACES_TAG);
	p3DFaces->SetDescription(TAG_MAP[_3DFACES_TAG]);
	//Element* p2DFaces = new Element(_2DFACES_TAG);
	//p2DFaces->SetDescription(TAG_MAP[_2DFACES_TAG]);

	if (rMesh.m_p3DVertices == 0 || rMesh.m_p2DVertices == 0)
		goto RETURN_FALSE;

	Segment* p3DVerticesID = new Segment(__LONG);
	p3DVerticesID->AddData(rMesh.m_p3DVertices->m_nID);
	p3DVerticesID->SetTagName(L"id");
	p3DVerticesID->SetTagDescription(L"Attr");	

	p3DFaces->AddData(p3DVerticesID);

	//Segment* p2DVerticesID = new Segment(__LONG);
	//p2DVerticesID->AddData(rMesh.m_p2DVertices->m_nID);
	//p2DFaces->AddData(p2DVerticesID);

	int n3DFaceCount = (int)rMesh.m_vec3DFace.size();

	for (int i=0;i<n3DFaceCount;i++)
	{
		Face& rFace = rMesh.m_vec3DFace[i];
		
		if (!MakeFace(rFace, p3DFaces))
			goto RETURN_FALSE;
	}

	//int n2DFaceCount = (int)rMesh.m_vec2DFace.size();

	//for (int i=0;i<n2DFaceCount;i++)
	//{
	//	Face& rFace = rMesh.m_vec2DFace[i];
		
	//	if (!MakeFace(rFace, p2DFaces))
	//		goto RETURN_FALSE;
	//}

	pMesh->AddData(pMeshID);
	pMesh->AddData(p3DFaces);
	//pMesh->AddData(p2DFaces);

	pParentElement->AddData(pMesh);
	return true;

RETURN_FALSE:
	delete pMesh;
	delete pMeshID;
	delete p3DFaces;
	//delete p2DFaces;
	return false;
}

bool MeshFactory::MakeFace(Face& rFace, Element* pParentElement)
{
	Element* pFace = new Element(FACE_TAG);
	pFace->SetDescription(TAG_MAP[FACE_TAG]);

	Segment* pV1 = new Segment(__LONG);
	pV1->AddData(rFace.v1);
	pV1->SetTagName(L"v1");
	pV1->SetTagDescription(L"Attr");	

	Segment* pV2 = new Segment(__LONG);
	pV2->AddData(rFace.v2);
	pV2->SetTagName(L"v2");
	pV2->SetTagDescription(L"Attr");

	Segment* pV3 = new Segment(__LONG);
	pV3->AddData(rFace.v3);
	pV3->SetTagName(L"v3");
	pV3->SetTagDescription(L"Attr");

	Segment* pV1U = new Segment(__FLOAT);
	pV1U->AddData(rFace.v1u);
	pV1U->SetTagName(L"v1u");
	pV1U->SetTagDescription(L"Attr");

	Segment* pV1V = new Segment(__FLOAT);
	pV1V->AddData(rFace.v1v);
	pV1V->SetTagName(L"v1v");
	pV1V->SetTagDescription(L"Attr");

	Segment* pV2U = new Segment(__FLOAT);
	pV2U->AddData(rFace.v2u);
	pV2U->SetTagName(L"v2u");
	pV2U->SetTagDescription(L"Attr");

	Segment* pV2V = new Segment(__FLOAT);
	pV2V->AddData(rFace.v2v);
	pV2V->SetTagName(L"v2v");
	pV2V->SetTagDescription(L"Attr");

	Segment* pV3U = new Segment(__FLOAT);
	pV3U->AddData(rFace.v3u);
	pV3U->SetTagName(L"v3u");
	pV3U->SetTagDescription(L"Attr");

	Segment* pV3V = new Segment(__FLOAT);
	pV3V->AddData(rFace.v3v);
	pV3V->SetTagName(L"v3v");
	pV3V->SetTagDescription(L"Attr");

	pFace->AddData(pV1);
	pFace->AddData(pV2);
	pFace->AddData(pV3);
	pFace->AddData(pV1U);
	pFace->AddData(pV1V);
	pFace->AddData(pV2U);
	pFace->AddData(pV2V);
	pFace->AddData(pV3U);
	pFace->AddData(pV3V);

	if (!rFace.m_useSmoothShading)
	{
		Segment* pSmoothShading = new Segment(__BOOL);
		pSmoothShading->AddData(rFace.m_useSmoothShading);
		pFace->AddData(pSmoothShading);
	}

	if (!rFace.m_useCulling)
	{
		Segment* pCulling = new Segment(__BOOL);
		pCulling->AddData(rFace.m_useCulling);
		pFace->AddData(pCulling);
	}

	if (!MakeTextureOption(rFace, pFace))
	{
		delete pFace;
		return false;
	}

	pParentElement->AddData(pFace);
	return true;
}

bool MeshFactory::MakeTextureOption(Face& rFace, Element* pParentElement)
{
	if (rFace.m_fTextureScaleX == 1.0f && rFace.m_fTextureScaleY == 1.0f && rFace.m_fOffsetX == 0.0f && rFace.m_fOffsetY == 0.0f)
		return true;

	Element* pTextureOption = new Element(TEXTURE_OPTION_TAG);
	pTextureOption->SetDescription(TAG_MAP[TEXTURE_OPTION_TAG]);

	Segment* pScaleX = new Segment(__FLOAT);
	pScaleX->AddData(rFace.m_fTextureScaleX);

	Segment* pScaleY = new Segment(__FLOAT);
	pScaleY->AddData(rFace.m_fTextureScaleY);

	Segment* pOffsetX = new Segment(__FLOAT);
	pOffsetX->AddData(rFace.m_fOffsetX);

	Segment* pOffsetY = new Segment(__FLOAT);
	pOffsetY->AddData(rFace.m_fOffsetY);

	pTextureOption->AddData(pScaleX);
	pTextureOption->AddData(pScaleY);
	pTextureOption->AddData(pOffsetX);
	pTextureOption->AddData(pOffsetY);

	pParentElement->AddData(pTextureOption);
	return true;
}

bool MeshFactory::MakeLayerGroup(Element* pParentElement)
{
	Element* pLayerGroup = new Element(LAYER_GROUP_TAG);
	pLayerGroup->SetDescription(TAG_MAP[LAYER_GROUP_TAG]);

	int nLayerCount = (int)m_vecLayer.size();

	for (int i=0;i<nLayerCount;i++)
	{
		Layer* pLayer = m_vecLayer[i];
		if (pLayer == 0) continue;

		if (!MakeLayer(*pLayer, pLayerGroup))
		{
			delete pLayerGroup;
			return false;
		}
	}

	pParentElement->AddData(pLayerGroup);
	return true;
}

bool MeshFactory::MakeLayer(Layer& rLayer, Element* pParentElement)
{
	Element* pLayer = new Element(LAYER_TAG);
	pLayer->SetDescription(TAG_MAP[LAYER_TAG]);

	Segment* pLayerID = new Segment(__LONG);
	pLayerID->AddData(rLayer.m_nLayerID);
	pLayerID->SetTagName(L"id");
	pLayerID->SetTagDescription(L"Attr");
	
	pLayer->AddData(pLayerID);

	Segment* pLayerType = new Segment(__LONG);
	pLayerType->AddData((int)rLayer.m_layerType);
	pLayerType->SetTagName(L"type");
	pLayerType->SetTagDescription(L"Attr");

	pLayer->AddData(pLayerType);


	Segment* pLayerName = new Segment(__WCHAR_ARR);	
	int nNameLen = (int)rLayer.m_strLayerName.length();
	for (int i=0;i<nNameLen;i++)
	{
		pLayerName->AddData(rLayer.m_strLayerName.at(i));
	}
	pLayerName->SetTagName(L"name");
	pLayerName->SetTagDescription(L"Attr");

	pLayer->AddData(pLayerName);

	Segment* pLayerDescription = new Segment(__WCHAR_ARR);	
	int nDescLen = (int)rLayer.m_strDescription.length();
	for (int i=0;i<nDescLen;i++)
	{
		pLayerDescription->AddData(rLayer.m_strDescription.at(i));
	}

	//pLayerDescription->SetName(L"desc");
	pLayerDescription->SetTagDescription(L"Text");
	pLayer->AddData(pLayerDescription);


	int NULL_ID = 0;

	Segment* pMaterialID = new Segment(__LONG);
	pMaterialID->AddData(rLayer.m_pMaterial ? rLayer.m_pMaterial->m_nMaterialID : NULL_ID);
	pMaterialID->SetTagName(L"material_id");
	pMaterialID->SetTagDescription(L"Attr");

	pLayer->AddData(pMaterialID);

	Segment* pParentLayerID = new Segment(__LONG);
	pParentLayerID->AddData(rLayer.m_pParentLayer ? rLayer.m_pParentLayer->m_nLayerID : NULL_ID);
	pParentLayerID->SetTagName(L"parent");
	pParentLayerID->SetTagDescription(L"Attr");
	pLayer->AddData(pParentLayerID);

	int nChildLayerCount = (int)rLayer.m_vecChildLayer.size();

	for (int i=0;i<nChildLayerCount;i++)
	{
		Layer* pChildLayer = rLayer.m_vecChildLayer[i];
		if (pChildLayer == 0) continue;

		Segment* pChildLayerID = new Segment(__LONG);
		pChildLayerID->AddData(pChildLayer->m_nLayerID);
		pLayer->AddData(pChildLayerID);
	}

	pParentElement->AddData(pLayer);
	return true;
}

bool MeshFactory::MakeObjectGroup(Element* pParentElement)
{
	Element* pObjectGroup = new Element(OBJECT_GROUP_TAG);
	pObjectGroup->SetDescription(TAG_MAP[OBJECT_GROUP_TAG]);

	int nObjectCount = (int)m_vecObject.size();

	for (int i=0;i<nObjectCount;i++)
	{
		Object* pObject = m_vecObject[i];
		if (pObject == 0) continue;

		if (!MakeObject(*pObject, pObjectGroup))
		{
			delete pObjectGroup;
			return false;
		}
	}

	pParentElement->AddData(pObjectGroup);
	return true;
}

bool MeshFactory::MakeObject(Object& rObject, Element* pParentElement)
{
	Element* pObject = new Element(OBJECT_TAG);
	pObject->SetDescription(TAG_MAP[OBJECT_TAG]);

	Segment* pObjectID = new Segment(__LONG);
	pObjectID->AddData(rObject.m_nObjectID);
	pObjectID->SetTagName(L"id");
	pObjectID->SetTagDescription(L"Attr");
	pObject->AddData(pObjectID);

	Segment* pObjectType = new Segment(__LONG);
	pObjectType->AddData((int)rObject.m_objType);
	pObjectType->SetTagName(L"type");
	pObjectType->SetTagDescription(L"Attr");
	pObject->AddData(pObjectType);

	int NULL_ID = 0;

	Segment* pLayerID = new Segment(__LONG);
	pLayerID->AddData(rObject.m_pLayer ? rObject.m_pLayer->m_nLayerID : NULL_ID);
	pLayerID->SetTagName(L"layer_id");
	pLayerID->SetTagDescription(L"Attr");
	pObject->AddData(pLayerID);

	Segment* pOwnMaterialID = new Segment(__LONG);
	pOwnMaterialID->AddData(rObject.m_pOwnMaterial ? rObject.m_pOwnMaterial->m_nMaterialID : NULL_ID);
	pOwnMaterialID->SetTagName(L"material_id");
	pOwnMaterialID->SetTagDescription(L"Attr");
	pObject->AddData(pOwnMaterialID);

	Segment* pObjectName = new Segment(__WCHAR_ARR);
	int nNameLen = (int)rObject.m_strObjectName.length();
	for (int i=0;i<nNameLen;i++)
	{
		pObjectName->AddData(rObject.m_strObjectName.at(i));
	}	
	pObjectName->SetTagName(L"name");
	pObjectName->SetTagDescription(L"Attr");
	pObject->AddData(pObjectName);

	if (!MakeObjectAttrGroup(rObject, pObject))
	{
		delete pObject;
		return false;
	}

	if (!MakeObjectMeshGroup(rObject, pObject))
	{
		delete pObject;
		return false;
	}

	pParentElement->AddData(pObject);
	return true;
}

bool MeshFactory::MakeObjectAttrGroup(Object& rObject, Element* pParentElement)
{
	Element* pObjAttrGroup = new Element(OBJECT_ATTR_GROUP_TAG);
	pObjAttrGroup->SetDescription(TAG_MAP[OBJECT_ATTR_GROUP_TAG]);

	int nAttrGroup = (int)rObject.m_vecAttr.size();

	for (int i=0;i<nAttrGroup;i++)
	{
		Object::Attr& rAttr = rObject.m_vecAttr[i];

		if (!MakeObjectAttr(rAttr, pObjAttrGroup))
		{
			delete pObjAttrGroup;
			return false;
		}
	}

	pParentElement->AddData(pObjAttrGroup);
	return true;
}

bool MeshFactory::MakeObjectAttr(Object::Attr& rAttr, Element* pParentElement)
{
	Element* pObjAttr = new Element(OBJECT_ATTR_TAG);
	pObjAttr->SetDescription(TAG_MAP[OBJECT_ATTR_TAG]);

	Segment* pAttrName = new Segment(__WCHAR_ARR);
	int nNameLen = (int)rAttr.m_strAttrName.length();
	pAttrName->SetTagName(L"name");
	pAttrName->SetTagDescription(L"Attr");

	for (int i=0;i<nNameLen;i++)
	{
		pAttrName->AddData(rAttr.m_strAttrName.at(i));
	}

	Segment* pAttrData = new Segment(__WCHAR_ARR);
	int nDataLen = (int)rAttr.m_strAttrData.length();

	for (int i=0;i<nDataLen;i++)
	{
		pAttrData->AddData(rAttr.m_strAttrData.at(i));
	}
	pAttrData->SetTagName(L"data");
	pAttrData->SetTagDescription(L"Attr");

	pObjAttr->AddData(pAttrName);
	pObjAttr->AddData(pAttrData);

	pParentElement->AddData(pObjAttr);
	return true;
}

bool MeshFactory::MakeObjectMeshGroup(Object& rObject, Element* pParentElement)
{
	Element* pObjMeshGroup = new Element(OBJECT_MESH_GROUP_TAG);
	pObjMeshGroup->SetDescription(TAG_MAP[OBJECT_MESH_GROUP_TAG]);

	int nMeshCount = (int)rObject.m_vecMesh.size();

	for (int i=0;i<nMeshCount;i++)
	{
		Object::ObjectMesh& rMesh = rObject.m_vecMesh[i];

		if (!MakeObjectMesh(rMesh, pObjMeshGroup))
		{
			delete pObjMeshGroup;
			return false;
		}
	}

	pParentElement->AddData(pObjMeshGroup);
	return true;
}

bool MeshFactory::MakeObjectMesh(Object::ObjectMesh& rMesh, Element* pParentElement)
{
	Element* pObjMesh = new Element(OBJECT_MESH_TAG);
	pObjMesh->SetDescription(TAG_MAP[OBJECT_MESH_TAG]);

	Segment* pMeshID = new Segment(__LONG);
	pMeshID->AddData(rMesh.m_pMesh->m_nMeshID);
	pMeshID->SetTagName(L"id");
	pMeshID->SetTagDescription(L"Attr");
	pObjMesh->AddData(pMeshID);

	std::wstring xyz[3] = { L"tx", L"ty", L"tz"};
	
	for (int i=0;i<3;i++)
	{
		Segment* pObjPos = new Segment(__FLOAT);
		pObjPos->AddData(rMesh.m_arrPosition[i]);
		pObjPos->SetTagName(xyz[i]);
		pObjPos->SetTagDescription(L"Attr");
		pObjMesh->AddData(pObjPos);
	}
	
	Element* pMatrix = new Element(MATRIX_TAG);
	pMatrix->SetDescription(TAG_MAP[MATRIX_TAG]);
	Segment* pObjRotate = new Segment(__FLOAT_ARR);
	for (int i = 0 ; i < 9 ; i++)
	{
		pObjRotate->AddData(rMesh.m_arrLocalAxis[i]);		
	}
	pObjRotate->SetTagName(L"data");
	pObjRotate->SetTagDescription(L"Attr");	
	pMatrix->AddData(pObjRotate);
	pObjMesh->AddData(pMatrix);
	

	std::wstring sxyz[3] = { L"sx", L"sy", L"sz"};
	
	for (int i=0;i<3;i++)
	{
		Segment* pObjScale = new Segment(__FLOAT);
		pObjScale->AddData(rMesh.m_arrScale[i]);
		pObjScale->SetTagName(sxyz[i]);
		pObjScale->SetTagDescription(L"Attr");
		pObjMesh->AddData(pObjScale);
	}

	pParentElement->AddData(pObjMesh);
	return true;
}

bool MeshFactory::Read(std::wstring strPath, Reader& rReader, FILE* fp, CStopwatch& watch)
{
	fprintf(fp, "Read Begin, Elapsed Time : %.2lf\r\n", watch.CurrentElapsedTimeSec());

	if (!rReader.ReadFile(strPath.c_str()))
		return false;

	fprintf(fp, "Read File, Elapsed Time : %.2lf\r\n", watch.CurrentElapsedTimeSec());

	unsigned int nElementCount = rReader.GetElementCount();

	for (unsigned int i=0;i<nElementCount;i++)
	{
		Element* pElement = (Element*)rReader.GetElement(i);

		if (pElement->GetTag() == HEADER_TAG)
		{
			if (!ReadHeader(*pElement))
				return false;

			fprintf(fp, "ReadHeader, Elapsed Time : %.2lf\r\n", watch.CurrentElapsedTimeSec());
		}
		else
		{
			if (!ReadBody(*pElement, fp, watch))
				return false;
		}
	}

	return true;
}

bool MeshFactory::ReadHeader(Element& rElement)
{
	rElement.SetDescription(TAG_MAP[rElement.GetTag()]);

	unsigned int nDataCount = rElement.GetDataCount();

	for (unsigned int i=0;i<nDataCount;i++)
	{
		const UData* pData = rElement.GetData(i);
		if (pData == 0) continue;

		if (pData->GetClassType() != UData::ELEMENT)
		{
			m_strError = L"Header 아래에 Segment가 존재합니다. Header에는 Element만 있어야 합니다.";
			return false;
		}

		Element* pElement = (Element*)pData;
		int nTag = pElement->GetTag();

		if (nTag == VERSION_TAG)
		{
			if (!ReadMeshVersion(*pElement))
				return false;
		}
		else if (nTag == UNIT_OF_LENGTH_TAG)
		{
			if (!ReadUnitOfLength(*pElement))
				return false;
		}
		else if (nTag == AZIMUTH_TAG)
		{
			if (!ReadAzimuth(*pElement))
				return false;
		}
		else if (nTag == LATITUDE_TAG)
		{
			if (!ReadLatitude(*pElement))
				return false;
		}
		else if (nTag == LONGITUDE_TAG)
		{
			if (!ReadLongitude(*pElement))
				return false;
		}
		else if (nTag == HEIGHT_ABOVE_SEA_LEVEL_TAG)
		{
			if (!ReadHASL(*pElement))
				return false;
		}
		else if (nTag == BUILDING_NAME_TAG)
		{
			if (!ReadBuildingName(*pElement))
				return false;
		}
		else if (nTag == ADDRESS_TAG)
		{
			if (!ReadAddress(*pElement))
				return false;
		}
		else if (nTag == COORDINATES_SYSTEM_TAG)
		{
			if (!ReadCoordinatesSystem(*pElement))
				return false;
		}
		else
		{
			wchar_t strError[256];
			swprintf(strError, 256, L"Header 안에 알 수 없는 Element(%s)가 존재합니다.", pElement->GetTagString().c_str());
			m_strError = strError;
			return false;
		}
	}

	return true;
}

template <class T>
static bool ReadUnitSegment(const std::wstring& strTagDesc, const Segment* pSegment, T& rTargetData, DataType dataType, const wchar_t* strDataType, std::wstring& strErrorMessage)
{
	DataType type = pSegment->GetType();

	if (type != dataType)
	{
		wchar_t strError[256];
		swprintf(strError, 256, L"%s 아래에 %s Type이 아닌 데이터가 존재합니다.(%d)", strTagDesc.c_str(), strDataType, type);
		strErrorMessage = strError;
		return false;
	}

	unsigned int nDataCount = pSegment->GetDataCount();

	if (nDataCount == 0)
	{
		wchar_t strError[256];
		swprintf(strError, 256, L"%s 아래에 데이터가 존재하지 않습니다.", strTagDesc.c_str());
		strErrorMessage = strError;
		return false;
	}

	rTargetData = *(T*)pSegment->GetData(0);
	return true;
}

template <class T>
static bool ReadArraySegment(const std::wstring& strTagDesc, const Segment* pSegment, T* arrData, DataType dataType, const wchar_t* strDataType, std::wstring& strErrorMessage, unsigned int nArrayCountFixed = 0)
{
	DataType type = pSegment->GetType();

	if (type != dataType)
	{
		wchar_t strError[256];
		swprintf(strError, 256, L"%s 아래에 %s Type이 아닌 데이터가 존재합니다.(%d)", strTagDesc.c_str(), strDataType, type);
		strErrorMessage = strError;
		return false;
	}

	unsigned int nDataCount = pSegment->GetDataCount();

	if (nArrayCountFixed)
	{
		if (nDataCount != nArrayCountFixed)
		{
			wchar_t strError[256];
			swprintf(strError, 256, L"%s 아래에는 %d개의 배열 Segment가 존재하여야만 합니다. 현재는 %d개가 있습니다.", strTagDesc.c_str(), nArrayCountFixed, nDataCount);
			strErrorMessage = strError;
			return false;
		}
		else
			nDataCount = nArrayCountFixed;
	}

	for (unsigned int i=0;i<nDataCount;i++)
	{
		arrData[i] = *(T*)pSegment->GetData(i);
	}

	return true;
}

template <class T>
static bool ReadUnitData(Element& rElement, T& rTargetData, DataType dataType, const wchar_t* strDataType, std::wstring& strErrorMessage, unsigned int nBeginIndex = 0)
{
	const std::wstring& strTagDesc = TAG_MAP[rElement.GetTag()];
	rElement.SetDescription(strTagDesc);

	unsigned int nDataCount = rElement.GetDataCount();

	for (unsigned int i=nBeginIndex;i<nDataCount;i++)
	{
		const UData* pData = rElement.GetData(i);
		if (pData == 0) continue;

		if (pData->GetClassType() != UData::SEGMENT)
		{
			wchar_t strError[256];
			swprintf(strError, 256, L"%s 아래에 Element가 존재합니다. %s에는 Segment만 있어야 합니다.", strTagDesc.c_str(), strTagDesc.c_str());
			strErrorMessage = strError;
			return false;
		}

		if (!ReadUnitSegment(strTagDesc, (const Segment*)pData, rTargetData, dataType, strDataType, strErrorMessage))
			return false;

		return true;
	}

	return false;
}

static bool ReadString(Element& rElement, std::wstring& rTargetData, std::wstring& strErrorMessage, unsigned int nBeginIndex = 0)
{
	const std::wstring& strTagDesc = TAG_MAP[rElement.GetTag()];
	rElement.SetDescription(strTagDesc);

	unsigned int nDataCount = rElement.GetDataCount();

	for (unsigned int i=nBeginIndex;i<nDataCount;i++)
	{
		const UData* pData = rElement.GetData(i);
		if (pData == 0) continue;

		if (pData->GetClassType() != UData::SEGMENT)
		{
			wchar_t strError[256];
			swprintf(strError, 256, L"%s 아래에 Element가 존재합니다. %s에는 Segment만 있어야 합니다.", strTagDesc.c_str(), strTagDesc.c_str());
			strErrorMessage = strError;
			return false;
		}

		const Segment* pSegment = (const Segment*)pData;
		DataType type = pSegment->GetType();

		if (type != __WCHAR_ARR && type != __WCHAR_ARR_FIXED)
		{
			wchar_t strError[256];
			swprintf(strError, 256, L"%s 아래에 Unicode 문자열이 아닌 데이터가 존재합니다.(%d)", strTagDesc.c_str(), type);
			strErrorMessage = strError;
			return false;
		}

		unsigned int nDataCount = pSegment->GetDataCount();

		for (unsigned int j=0;j<nDataCount;j++)
		{
			//rTargetData.append(*(wchar_t*)pSegment->GetData(j));
			rTargetData += *(wchar_t*)pSegment->GetData(j);
		}

		return true;
	}

	return false;
}

template <class T>
static bool ReadArrayData(Element& rElement, T* arrData, DataType dataType, const wchar_t* strDataType, std::wstring& strErrorMessage, unsigned int nArrayCountFixed = 0, unsigned int nDataIndex = 0)
{
	const std::wstring& strTagDesc = TAG_MAP[rElement.GetTag()];
	rElement.SetDescription(strTagDesc);

	unsigned int nDataCount = rElement.GetDataCount();

	if (nDataIndex >= nDataCount)
	{
NO_EXIST:
		wchar_t strError[256];
		swprintf(strError, 256, L"%s 아래에 %d번 Data가 존재하지 않습니다. 총 %d개의 Data가 존재합니다.", strTagDesc.c_str(), nDataIndex, nDataCount);
		strErrorMessage = strError;
		return false;
	}

	const UData* pData = rElement.GetData(nDataIndex);
	if (pData == 0) goto NO_EXIST;

	return ReadArraySegment(strTagDesc, (const Segment*)pData, arrData, dataType, strDataType, strErrorMessage, nArrayCountFixed);
}

template <class T>
static bool ReadArrayPtrData(Element& rElement, T* arrData[], DataType dataType, const wchar_t* strDataType, std::wstring& strErrorMessage, unsigned int nArrayCountFixed = 0, unsigned int nBeginIndex = 0)
{
	const std::wstring& strTagDesc = TAG_MAP[rElement.GetTag()];
	rElement.SetDescription(strTagDesc);

	unsigned int nDataCount = rElement.GetDataCount();

	if (nArrayCountFixed > 0)
	{
		if (nDataCount < nBeginIndex + nArrayCountFixed)
		{
			wchar_t strError[256];
			swprintf(strError, 256, L"%s 아래에는 %d개의 Segment가 존재하여야만 합니다. 현재는 %d개만 있습니다.", strTagDesc.c_str(), nBeginIndex + nArrayCountFixed, nDataCount);
			strErrorMessage = strError;
			return false;
		}
		else
			nDataCount = nBeginIndex + nArrayCountFixed;
	}

	for (unsigned int i=nBeginIndex, j=0;i<nDataCount;i++)
	{
		const UData* pData = rElement.GetData(i);
		if (pData == 0) continue;

		if (pData->GetClassType() != UData::SEGMENT)
		{
			wchar_t strError[256];
			swprintf(strError, 256, L"%s 아래에 Element가 존재합니다. %s에는 Segment만 있어야 합니다.", strTagDesc.c_str(), strTagDesc.c_str());
			strErrorMessage = strError;
			return false;
		}

		if (!ReadUnitSegment(strTagDesc, (const Segment*)pData, *arrData[j++], dataType, strDataType, strErrorMessage))
			return false;
	}

	return true;
}

bool MeshFactory::ReadMeshVersion(Element& rElement)
{
	return ReadString(rElement, m_strMeshVersion, m_strError);
	
}

bool MeshFactory::ReadUnitOfLength(Element& rElement)
{
	int nUnitOfLength;
	if (!ReadUnitData<int>(rElement, nUnitOfLength, __LONG, L"LONG", m_strError))
		return false;

	m_unitOfLength = (UnitOfLength)nUnitOfLength;
	return true;
}

bool MeshFactory::ReadAzimuth(Element& rElement)
{
	return ReadUnitData<double>(rElement, m_dAzimuth, __DOUBLE, L"DOUBLE", m_strError);
}

bool MeshFactory::ReadLatitude(Element& rElement)
{
	return ReadUnitData<double>(rElement, m_dLatitude, __DOUBLE, L"DOUBLE", m_strError);
}

bool MeshFactory::ReadLongitude(Element& rElement)
{
	return ReadUnitData<double>(rElement, m_dLongitude, __DOUBLE, L"DOUBLE", m_strError);
}

bool MeshFactory::ReadHASL(Element& rElement)
{
	return ReadUnitData<double>(rElement, m_dHeightAboveSeaLevel, __DOUBLE, L"DOUBLE", m_strError);
}

bool MeshFactory::ReadBuildingName(Element& rElement)
{
	return ReadString(rElement, m_strBuildingName, m_strError);
}

bool MeshFactory::ReadAddress(Element& rElement)
{
	return ReadString(rElement, m_strBuildingAddress, m_strError);
}

bool MeshFactory::ReadCoordinatesSystem(Element& rElement)
{
	return ReadUnitData<bool>(rElement, m_isRightHandSystem, __BOOL, L"BOOL", m_strError);
}

bool MeshFactory::ReadBody(Element& rElement, FILE* fp, CStopwatch& watch)
{
	rElement.SetDescription(TAG_MAP[rElement.GetTag()]);

	unsigned int nDataCount = rElement.GetDataCount();

	fprintf(fp, "ReadBody2, DataCount : %d, Elapsed Time : %.2lf\r\n", nDataCount, watch.CurrentElapsedTimeSec());

	for (unsigned int i=0;i<nDataCount;i++)
	{
		const UData* pData = rElement.GetData(i);
		if (pData == 0) continue;

		if (pData->GetClassType() != UData::ELEMENT)
		{
			m_strError = L"Body 아래에 Segment가 존재합니다. Body에는 Element만 있어야 합니다.";
			return false;
		}

		Element* pElement = (Element*)pData;
		int nTag = pElement->GetTag();

		if (nTag == _3DVERTICES_GROUP_TAG)
		{
			if (!Read3DVerticesGroup(*pElement))
				return false;

			fprintf(fp, "_3DVERTICES_GROUP_TAG Elapsed Time : %.2lf\r\n", watch.CurrentElapsedTimeSec());
		}
		//else if (nTag == _2DVERTICES_GROUP_TAG)
		//{
		//	if (!Read2DVerticesGroup(*pElement))
		//		return false;
		//}
		else if (nTag == TEXTURE_GROUP_TAG)
		{
			if (!ReadTextureGroup(*pElement))
				return false;

			fprintf(fp, "TEXTURE_GROUP_TAG Elapsed Time : %.2lf\r\n", watch.CurrentElapsedTimeSec());
		}
		else if (nTag == MATERIAL_GROUP_TAG)
		{
			if (!ReadMaterialGroup(*pElement))
				return false;

			fprintf(fp, "MATERIAL_GROUP_TAG Elapsed Time : %.2lf\r\n", watch.CurrentElapsedTimeSec());
		}
		else if (nTag == MESH_GROUP_TAG)
		{
			if (!ReadMeshGroup(*pElement))
				return false;

			fprintf(fp, "MESH_GROUP_TAG Elapsed Time : %.2lf\r\n", watch.CurrentElapsedTimeSec());
		}
		else if (nTag == LAYER_GROUP_TAG)
		{
			if (!ReadLayerGroup(*pElement))
				return false;

			fprintf(fp, "LAYER_GROUP_TAG Elapsed Time : %.2lf\r\n", watch.CurrentElapsedTimeSec());
		}
		else if (nTag == OBJECT_GROUP_TAG)
		{
			if (!ReadObjectGroup(*pElement))
				return false;

			fprintf(fp, "OBJECT_GROUP_TAG Elapsed Time : %.2lf\r\n", watch.CurrentElapsedTimeSec());
		}
		else
		{
			wchar_t strError[256];
			swprintf(strError, 256, L"Body 안에 알 수 없는 Element(%s)가 존재합니다.", pElement->GetTagString().c_str());
			m_strError = strError;
			return false;
		}
	}

	return true;
}

bool MeshFactory::ReadObjectGroup(Element& rElement)
{
	rElement.SetDescription(TAG_MAP[rElement.GetTag()]);

	unsigned int nDataCount = rElement.GetDataCount();

	for (unsigned int i=0;i<nDataCount;i++)
	{
		const UData* pData = rElement.GetData(i);
		if (pData == 0) continue;

		if (pData->GetClassType() != UData::ELEMENT)
		{
			m_strError = L"ObjectGroup 아래에 Segment가 존재합니다. ObjectGroup에는 Element만 있어야 합니다.";
			return false;
		}

		Element* pElement = (Element*)pData;
		int nTag = pElement->GetTag();

		if (nTag == OBJECT_TAG)
		{
			if (!ReadObject(*pElement))
				return false;
		}
		else
		{
			wchar_t strError[256];
			swprintf(strError, 256, L"ObjectGroup 안에 알 수 없는 Element(%s)가 존재합니다.", pElement->GetTagString().c_str());
			m_strError = strError;
			return false;
		}
	}

	return true;
}

bool MeshFactory::ReadObject(Element& rElement)
{
	const std::wstring& strTagDesc = TAG_MAP[rElement.GetTag()];
	rElement.SetDescription(strTagDesc);

	Object* pObject = new ::Object;

	if (!ReadUnitData<int>(rElement, pObject->m_nObjectID, __LONG, L"LONG", m_strError))
		goto RETURN_FALSE;
	
	int nObjectType;
	if (!ReadUnitData<int>(rElement, nObjectType, __LONG, L"LONG", m_strError, 1))
		goto RETURN_FALSE;

	pObject->m_objType = (Object::ObjectType)nObjectType;

	int nLayerID;
	if (!ReadUnitData<int>(rElement, nLayerID, __LONG, L"LONG", m_strError, 2))
		goto RETURN_FALSE;

	if (nLayerID)
	{
		Layer* pLayer = (Layer*)FindLayer(nLayerID);

		if (pLayer == 0)
		{
			wchar_t strError[256];
			swprintf(strError, 256, L"Object ID(%d) 아래에 존재하지 않는 Layer ID(%d)가 있습니다.", pObject->m_nObjectID, nLayerID);
			m_strError = strError;
			goto RETURN_FALSE;
		}

		pObject->m_pLayer = pLayer;
	}

	int nOwnMaterialID;
	if (!ReadUnitData<int>(rElement, nOwnMaterialID, __LONG, L"LONG", m_strError, 3))
		goto RETURN_FALSE;

	if (nOwnMaterialID)
	{
		Material* pMaterial = (Material*)FindMaterial(nOwnMaterialID);

		if (pMaterial == 0)
		{
			wchar_t strError[256];
			swprintf(strError, 256, L"Object ID(%d) 아래에 존재하지 않는 Material ID(%d)가 있습니다.", pObject->m_nObjectID, nOwnMaterialID);
			m_strError = strError;
			goto RETURN_FALSE;
		}

		pObject->m_pOwnMaterial = pMaterial;
	}

	if (!ReadString(rElement, pObject->m_strObjectName, m_strError, 4))
		goto RETURN_FALSE;
	
	unsigned int nDataCount = rElement.GetDataCount();

	for (unsigned int i=5;i<nDataCount;i++)
	{
		const UData* pData = rElement.GetData(i);
		if (pData == 0) continue;

		if (pData->GetClassType() == UData::SEGMENT)
		{
			wchar_t strError[256];
			swprintf(strError, 256, L"Object ID(%d) 아래에 정의되지 않은 %d번 Segment가 존재합니다.", pObject->m_nObjectID, i);
			m_strError = strError;
			goto RETURN_FALSE;
		}

		Element* pElement = (Element*)pData;
		int nElementTag = pElement->GetTag();

		if (nElementTag == OBJECT_ATTR_GROUP_TAG)
		{
			if (!ReadObjectAttrGroup(*pElement, *pObject))
				goto RETURN_FALSE;
		}
		else if (nElementTag == OBJECT_MESH_GROUP_TAG)
		{
			if (!ReadObjectMeshGroup(*pElement, *pObject))
				goto RETURN_FALSE;
		}
		else
		{
			wchar_t strError[256];
			swprintf(strError, 256, L"Object ID(%d) 아래에 정의되지 않은 Element(%s)가 존재합니다.", pObject->m_nObjectID, pElement->GetTagString().c_str());
			m_strError = strError;
			return false;
		}
	}

	m_vecObject.push_back(pObject);
	return true;

RETURN_FALSE:
	delete pObject;
	return false;
}

bool MeshFactory::ReadObjectMeshGroup(Element& rElement, Object& rObject)
{
	const std::wstring& strElementDesc = TAG_MAP[rElement.GetTag()];
	rElement.SetDescription(strElementDesc);

	unsigned int nDataCount = rElement.GetDataCount();

	for (unsigned int i=0;i<nDataCount;i++)
	{
		const UData* pData = rElement.GetData(i);
		if (pData == 0) continue;

		if (pData->GetClassType() != UData::ELEMENT)
		{
			wchar_t strError[256];
			swprintf(strError, 256, L"Object ID(%d) 아래에 있는 %s 아래에 Segment가 존재합니다. %s 아래에는 하위 Element만 존재할 수 있습니다.", rObject.m_nObjectID, strElementDesc.c_str(), strElementDesc.c_str());
			m_strError = strError;
			return false;
		}

		Element* pElement = (Element*)pData;
		int nElementTag = pElement->GetTag();

		if (nElementTag == OBJECT_MESH_TAG)
		{
			if (!ReadObjectMesh(*pElement, rObject))
				return false;
		}
		else
		{
			wchar_t strError[256];
			swprintf(strError, 256, L"Object ID(%d) 아래에 있는 %s 아래에 정의되지 않은 Element(%s)가 존재합니다.", rObject.m_nObjectID, strElementDesc.c_str(), pElement->GetTagString().c_str());
			m_strError = strError;
			return false;
		}
	}

	return true;
}

bool MeshFactory::ReadObjectMesh(Element& rElement, Object& rObject)
{
	const std::wstring& strElementDesc = TAG_MAP[rElement.GetTag()];
	rElement.SetDescription(strElementDesc);

	Object::ObjectMesh mesh;
	int nCurIdx = 0;
	int nMeshID;
	if (!ReadUnitData<int>(rElement, nMeshID, __LONG, L"LONG", m_strError, nCurIdx++))
		return false;

	if (nMeshID)
	{
		Mesh* pMesh = (Mesh*)FindMesh(nMeshID);

		if (pMesh == 0)
		{
			wchar_t strError[256];
			swprintf(strError, 256, L"Object ID %d 안에 알 수 없는 Mesh ID(%d)가 존재합니다.", rObject.m_nObjectID, nMeshID);
			m_strError = strError;
			return false;
		}

		mesh.m_pMesh = pMesh;
	}
		
	for( int i =0 ;  i < 3 ; i++)
	{
		if (!ReadUnitData<float>(rElement, mesh.m_arrPosition[i], __FLOAT, L"FLOAT", m_strError, nCurIdx++))
			return false;
	}	
	
	const UData* pData = rElement.GetData(nCurIdx++);		
	Element* pElement = (Element*)pData;
	const std::wstring& strSubElementDesc = TAG_MAP[pElement->GetTag()];
	if (!ReadArrayData<float>(*pElement, mesh.m_arrLocalAxis, __FLOAT_ARR, L"FLOAT", m_strError, 9))
		return false;

	//if (!ReadArraySegment<float>(strElementDesc, (const Segment*)pData, mesh.m_arrScale, __FLOAT_ARR, L"FLOAT", m_strError, 3))
	//	return false;

	for( int i =0 ;  i < 3 ; i++)
	{
		if (!ReadUnitData<float>(rElement, mesh.m_arrScale[i], __FLOAT, L"FLOAT", m_strError, nCurIdx++))
			return false;
	}	

	rObject.m_vecMesh.push_back(mesh);
	return true;
}

bool MeshFactory::ReadObjectAttrGroup(Element& rElement, Object& rObject)
{
	const std::wstring& strElementDesc = TAG_MAP[rElement.GetTag()];
	rElement.SetDescription(strElementDesc);

	unsigned int nDataCount = rElement.GetDataCount();

	for (unsigned int i=0;i<nDataCount;i++)
	{
		const UData* pData = rElement.GetData(i);
		if (pData == 0) continue;

		if (pData->GetClassType() != UData::ELEMENT)
		{
			wchar_t strError[256];
			swprintf(strError, 256, L"Object ID(%d) 아래에 있는 %s 아래에 Segment가 존재합니다. %s 아래에는 하위 Element만 존재할 수 있습니다.", rObject.m_nObjectID, strElementDesc.c_str(), strElementDesc.c_str());
			m_strError = strError;
			return false;
		}

		Element* pElement = (Element*)pData;
		int nElementTag = pElement->GetTag();

		if (nElementTag == OBJECT_ATTR_TAG)
		{
			if (!ReadObjectAttr(*pElement, rObject))
				return false;
		}
		else
		{
			wchar_t strError[256];
			swprintf(strError, 256, L"Object ID(%d) 아래에 있는 %s 아래에 정의되지 않은 Element(%s)가 존재합니다.", rObject.m_nObjectID, strElementDesc.c_str(), pElement->GetTagString().c_str());
			m_strError = strError;
			return false;
		}
	}

	return true;
}

bool MeshFactory::ReadObjectAttr(Element& rElement, Object& rObject)
{
	const std::wstring& strElementDesc = TAG_MAP[rElement.GetTag()];
	rElement.SetDescription(strElementDesc);

	Object::Attr attr;

	if (!ReadString(rElement, attr.m_strAttrName, m_strError))
	{
		wchar_t strError[256];
		swprintf(strError, 256, L"Object ID(%d) 아래에 속성 이름을 얻어올 수 없는 Object 속성 Tag가 있습니다.", rObject.m_nObjectID);
		m_strError = strError;
		return false;
	}

	if (!ReadString(rElement, attr.m_strAttrData, m_strError))
	{
		wchar_t strError[256];
		swprintf(strError, 256, L"Object ID(%d) 아래에 속성 Data를 얻어올 수 없는 Object 속성 Tag가 있습니다.", rObject.m_nObjectID);
		m_strError = strError;
		return false;
	}

	rObject.m_vecAttr.push_back(attr);
	return true;
}

bool MeshFactory::ReadLayerGroup(Element& rElement)
{
	rElement.SetDescription(TAG_MAP[rElement.GetTag()]);

	unsigned int nDataCount = rElement.GetDataCount();

	for (unsigned int i=0;i<nDataCount;i++)
	{
		const UData* pData = rElement.GetData(i);
		if (pData == 0) continue;

		if (pData->GetClassType() != UData::ELEMENT)
		{
			m_strError = L"LayerGroup 아래에 Segment가 존재합니다. LayerGroup에는 Element만 있어야 합니다.";
			return false;
		}

		Element* pElement = (Element*)pData;
		int nTag = pElement->GetTag();

		if (nTag == LAYER_TAG)
		{
			if (!ReadLayer(*pElement))
				return false;
		}
		else
		{
			wchar_t strError[256];
			swprintf(strError, 256, L"LayerGroup 안에 알 수 없는 Element(%s)가 존재합니다.", pElement->GetTagString().c_str());
			m_strError = strError;
			return false;
		}
	}

	int nLayerCount = (int)m_vecLayer.size();

	for (int i=0;i<nLayerCount;i++)
	{
		Layer* pLayer = m_vecLayer[i];
		if (pLayer == 0) continue;

		int nParentLayerID = (int)(DWORD_PTR)pLayer->m_pParentLayer;

		if (nParentLayerID)
		{
			Layer* pParentLayer = (Layer*)FindLayer(nParentLayerID);

			if (pParentLayer == 0)
			{
				wchar_t strError[256];
				swprintf(strError, 256, L"LayerID %d 안에 알 수 없는 Layer ID(%d)가 존재합니다.", pLayer->m_nLayerID, nParentLayerID);
				m_strError = strError;
				return false;
			}
			else
				pLayer->m_pParentLayer = pParentLayer;
		}

		int nChildLayerCount = (int)pLayer->m_vecChildLayer.size();

		for (int j=0;j<nChildLayerCount;j++)
		{
			std::vector<Layer*>::iterator iter = pLayer->m_vecChildLayer.begin() + j;
			int nChildLayerID = (int)(DWORD_PTR)*iter;

			Layer* pChildLayer = (Layer*)FindLayer(nChildLayerID);

			if (pChildLayer == 0)
			{
				wchar_t strError[256];
				swprintf(strError, 256, L"LayerID %d 안에 알 수 없는 Layer ID(%d)가 존재합니다.", pLayer->m_nLayerID, nParentLayerID);
				m_strError = strError;
				return false;
			}
			else
				*iter = pChildLayer;
		}
	}

	return true;
}

bool MeshFactory::ReadLayer(Element& rElement)
{
	const std::wstring& strTagDesc = TAG_MAP[rElement.GetTag()];
	rElement.SetDescription(strTagDesc);

	Layer* pLayer = new Layer;

	if (!ReadUnitData<int>(rElement, pLayer->m_nLayerID, __LONG, L"LONG", m_strError))
		goto RETURN_FALSE;
	
	int nLayerType;
	if (!ReadUnitData<int>(rElement, nLayerType, __LONG, L"LONG", m_strError, 1))
		goto RETURN_FALSE;

	pLayer->m_layerType = (Layer::LayerType)nLayerType;

	if (!ReadString(rElement, pLayer->m_strLayerName, m_strError, 2))
		goto RETURN_FALSE;

	if (!ReadString(rElement, pLayer->m_strDescription, m_strError, 3))
		goto RETURN_FALSE;

	int nMaterialID;
	if (!ReadUnitData<int>(rElement, nMaterialID, __LONG, L"LONG", m_strError, 4))
		goto RETURN_FALSE;

	if (nMaterialID)
	{
		Material* pMaterial = (Material*)FindMaterial(nMaterialID);

		if (pMaterial == 0)
		{
			wchar_t strError[256];
			swprintf(strError, 256, L"Layer ID %d 아래에 알 수 없는 Material ID(%d)가 존재합니다.", pLayer->m_nLayerID, nMaterialID);
			m_strError = strError;
			goto RETURN_FALSE;
		}

		pLayer->m_pMaterial = pMaterial;
	}

	int nParentLayerID;
	if (!ReadUnitData<int>(rElement, nParentLayerID, __LONG, L"LONG", m_strError, 5))
		goto RETURN_FALSE;

	// 현재 Layer 목록이 완성되지 않았으므로 LayerID에 해당하는 Layer 객체를 얻어올 수 없음
	// 임시로 ID를 포인터로 형변환해서 저장한 후 목록이 완성되면 실제 Layer 객체로 바꿔넣는다.
	pLayer->m_pParentLayer = (Layer*)(void*)(DWORD_PTR)nParentLayerID;

	int nChildLayerID;
	unsigned int nDataCount = rElement.GetDataCount();

	for (unsigned int i=5;i<nDataCount;i++)
	{
		const UData* pData = rElement.GetData(i);
		if (pData == 0) continue;

		if (pData->GetClassType() == UData::ELEMENT)
		{
			m_strError = L"Layer 아래에 Element가 존재합니다. Layer에는 Segment만 있어야 합니다.";
			goto RETURN_FALSE;
		}

		if (!ReadUnitSegment<int>(strTagDesc, (const Segment*)pData, nChildLayerID, __LONG, L"LONG", m_strError))
			goto RETURN_FALSE;

		if (nChildLayerID)
			pLayer->m_vecChildLayer.push_back((Layer*)(void*)(DWORD_PTR)nChildLayerID);
	}

	m_vecLayer.push_back(pLayer);
	return true;

RETURN_FALSE:
	delete pLayer;
	return false;
}

bool MeshFactory::ReadMeshGroup(Element& rElement)
{
	rElement.SetDescription(TAG_MAP[rElement.GetTag()]);

	unsigned int nDataCount = rElement.GetDataCount();

	for (unsigned int i=0;i<nDataCount;i++)
	{
		const UData* pData = rElement.GetData(i);
		if (pData == 0) continue;

		if (pData->GetClassType() != UData::ELEMENT)
		{
			m_strError = L"MeshGroup 아래에 Segment가 존재합니다. MeshGroup에는 Element만 있어야 합니다.";
			return false;
		}

		Element* pElement = (Element*)pData;
		int nTag = pElement->GetTag();

		if (nTag == MESH_TAG)
		{
			if (!ReadMesh(*pElement))
				return false;
		}
		else
		{
			wchar_t strError[256];
			swprintf(strError, 256, L"MeshGroup 안에 알 수 없는 Element(%s)가 존재합니다.", pElement->GetTagString().c_str());
			m_strError = strError;
			return false;
		}
	}

	return true;
}

bool MeshFactory::ReadMesh(Element& rElement)
{
	rElement.SetDescription(TAG_MAP[rElement.GetTag()]);

	Mesh* pMesh = new Mesh;

	unsigned int nDataCount = rElement.GetDataCount();

	if (!ReadUnitData<int>(rElement, pMesh->m_nMeshID, __LONG, L"LONG", m_strError))
		goto RETURN_FALSE;

	for (unsigned int i=1;i<nDataCount;i++)
	{
		const UData* pData = rElement.GetData(i);
		if (pData == 0) continue;

		if (pData->GetClassType() != UData::ELEMENT)
		{
			m_strError = L"Mesh 아래에 Mesh ID 이외의 Segment가 존재합니다. Mesh에는 Mesh ID와 Element만 있어야 합니다.";
			return false;
		}

		Element* pElement = (Element*)pData;
		int nTag = pElement->GetTag();

		if (nTag == _3DFACES_TAG)
		{
			if (!ReadFaces(*pElement, *pMesh, true))
				return false;
		}
		else if (nTag == _2DFACES_TAG)
		{
			if (!ReadFaces(*pElement, *pMesh, false))
				return false;
		}
		else
		{
			wchar_t strError[256];
			swprintf(strError, 256, L"Mesh 안에 알 수 없는 Element(%s)가 존재합니다.", pElement->GetTagString().c_str());
			m_strError = strError;
			return false;
		}
	}

	m_vecMesh.push_back(pMesh);
	return true;

RETURN_FALSE:
	delete pMesh;
	return false;
}

bool MeshFactory::ReadFaces(Element& rElement, Mesh& rMesh, bool is3D)
{
	const std::wstring& strTagDesc = TAG_MAP[rElement.GetTag()];
	rElement.SetDescription(strTagDesc);

	int nVerticesID;

	if (!ReadUnitData<int>(rElement, nVerticesID, __LONG, L"LONG", m_strError))
	{
		wchar_t strError[256];
		swprintf(strError, 256, L"%s 아래에 Vertices ID가 존재하지 않습니다.", strTagDesc.c_str());
		m_strError = strError;
		return false;
	}

	Vertices* pVertices = (Vertices*)FindVertices(nVerticesID, is3D);

	if (pVertices == 0)
	{
		wchar_t strError[256];
		swprintf(strError, 256, L"%s 아래에 존재하지 않는 Vertices ID(%d)가 있습니다.", strTagDesc.c_str(), nVerticesID);
		m_strError = strError;
		return false;
	}
	else
	{
		if (is3D) rMesh.m_p3DVertices = pVertices;
		else rMesh.m_p2DVertices = pVertices;
	}

	unsigned int nDataCount = rElement.GetDataCount();

	for (unsigned int i=1;i<nDataCount;i++)
	{
		const UData* pData = rElement.GetData(i);
		if (pData == 0) continue;

		if (pData->GetClassType() != UData::ELEMENT)
		{
			wchar_t strError[256];
			swprintf(strError, 256, L"%s 아래에 Vertices ID 이외의 Segment가 존재합니다. %s에는 Vertices ID와 Element만 있어야 합니다.", strTagDesc.c_str());
			m_strError = strError;
			return false;
		}

		Element* pElement = (Element*)pData;
		int nTag = pElement->GetTag();

		if (nTag == FACE_TAG)
		{
			if (!ReadFace(*pElement, rMesh, is3D))
				return false;
		}
		else
		{
			wchar_t strError[256];
			swprintf(strError, 256, L"%s 안에 알 수 없는 Element(%s)가 존재합니다.", strTagDesc.c_str(), pElement->GetTagString().c_str());
			m_strError = strError;
			return false;
		}
	}

	return true;
}

bool MeshFactory::ReadFace(Element& rElement, Mesh& rMesh, bool is3D)
{
	const std::wstring& strTagDesc = TAG_MAP[rElement.GetTag()];
	rElement.SetDescription(strTagDesc);

	Face face;

	int* arrVertexIndex[3] = {&face.v1, &face.v2, &face.v3};
	float* arrUV[6] = {&face.v1u, &face.v1v, &face.v2u, &face.v2v, &face.v3u, &face.v3v};

	if (!ReadArrayPtrData<int>(rElement, arrVertexIndex, __LONG, L"LONG", m_strError, 3, 0))
		return false;

	if (!ReadArrayPtrData<float>(rElement, arrUV, __FLOAT, L"FLOAT", m_strError, 6, 3))
		return false;

	unsigned int nDataCount = rElement.GetDataCount();

	for (unsigned int i=9;i<nDataCount;i++)
	{
		const UData* pData = rElement.GetData(i);
		if (pData == 0) continue;

		if (pData->GetClassType() == UData::ELEMENT)
		{
			if (!ReadTextureOption(*(Element*)pData, face))
				return false;
		}
		else if (i == 9)
		{
			if (!ReadUnitSegment(strTagDesc, (const Segment*)pData, face.m_useSmoothShading, __BOOL, L"BOOL", m_strError))
				return false;
		}
		else if (i == 10)
		{
			if (!ReadUnitSegment(strTagDesc, (const Segment*)pData, face.m_useCulling, __BOOL, L"BOOL", m_strError))
				return false;
		}
	}

	if (is3D) rMesh.m_vec3DFace.push_back(face);
	else rMesh.m_vec2DFace.push_back(face);
	return true;
}

bool MeshFactory::ReadTextureOption(Element& rElement, Face& rFace)
{
	const std::wstring& strTagDesc = TAG_MAP[rElement.GetTag()];
	rElement.SetDescription(strTagDesc);

	float* pTargetData;
	unsigned int nDataCount = rElement.GetDataCount();

	for (unsigned int i=0;i<nDataCount;i++)
	{
		const UData* pData = rElement.GetData(i);
		if (pData == 0) continue;

		if (pData->GetClassType() == UData::ELEMENT)
		{
			wchar_t strError[256];
			swprintf(strError, 256, L"%s 아래에 Segment가 존재합니다. %s에는 Element만 있어야 합니다.", strTagDesc.c_str());
			m_strError = strError;
			return false;
		}

		if (i == 0) pTargetData = &rFace.m_fTextureScaleX;
		else if (i == 1) pTargetData = &rFace.m_fTextureScaleY;
		else if (i == 2) pTargetData = &rFace.m_fOffsetX;
		else if (i == 3) pTargetData = &rFace.m_fOffsetY;
		else break;

		if (!ReadUnitData<float>(rElement, *pTargetData, __FLOAT, L"FLOAT", m_strError, i))
			return false;
	}

	return true;
}

bool MeshFactory::ReadMaterialGroup(Element& rElement)
{
	rElement.SetDescription(TAG_MAP[rElement.GetTag()]);

	unsigned int nDataCount = rElement.GetDataCount();

	for (unsigned int i=0;i<nDataCount;i++)
	{
		const UData* pData = rElement.GetData(i);
		if (pData == 0) continue;

		if (pData->GetClassType() != UData::ELEMENT)
		{
			m_strError = L"MaterialGroup 아래에 Segment가 존재합니다. MaterialGroup에는 Element만 있어야 합니다.";
			return false;
		}

		Element* pElement = (Element*)pData;
		int nTag = pElement->GetTag();

		if (nTag == MATERIAL0_TAG || nTag == MATERIAL10_TAG || nTag == MATERIAL20_TAG || nTag == MATERIAL30_TAG ||
			nTag == MATERIAL40_TAG || nTag == MATERIAL50_TAG || nTag == MATERIAL60_TAG || nTag == MATERIAL70_TAG)
		{
			if (!ReadMaterial(*pElement))
				return false;
		}
		else
		{
			wchar_t strError[256];
			swprintf(strError, 256, L"MaterialGroup 안에 알 수 없는 Element(%s)가 존재합니다.", pElement->GetTagString().c_str());
			m_strError = strError;
			return false;
		}
	}

	return true;
}



bool MeshFactory::ReadMaterial(Element& rElement)
{
	rElement.SetDescription(TAG_MAP[rElement.GetTag()]);
	Material* pMaterial = new Material;

	const std::wstring& strTagDesc = TAG_MAP[rElement.GetTag()];

	int nElementTag = rElement.GetTag();
	unsigned int nDataCount = rElement.GetDataCount();
	int nCurrentIdx = 0;
	
	if (!ReadUnitData<int>(rElement, pMaterial->m_nMaterialID, __LONG, L"LONG", m_strError))
		goto RETURN_FALSE;
	
	nCurrentIdx++;

	if (!ReadString(rElement, pMaterial->m_strMaterialName, m_strError, nCurrentIdx++))
		goto RETURN_FALSE;

	const UData* pData = rElement.GetData(nCurrentIdx++);
	if (pData == 0)
		goto RETURN_FALSE;
	
	
	Element* pElement = (Element*)pData;
	int nTag = pElement->GetTag();

	if (nTag == MATERIAL_DIFFUSE_TAG)
	{
		for( int i = 0 ; i < 4 ; i++)
		{
			if (!ReadUnitData<float>(*pElement, (pMaterial->m_arrDiffuseColor[i]), __FLOAT, L"Float", m_strError))
				goto RETURN_FALSE;
		}
	}
	
	
	const UData* pData2 = rElement.GetData(nCurrentIdx++);
	if (pData2 == 0)
		goto RETURN_FALSE;
	Element* pElement2 = (Element*)pData2;
	for( int i = 0 ; i < 4 ; i++)
	{
		if (!ReadUnitData<float>(*pElement2, (pMaterial->m_arrAmbientColor[i]), __FLOAT, L"Float", m_strError))
			goto RETURN_FALSE;
	}


	//int nBeginIndex = 4;
	if (nDataCount == nCurrentIdx)
		goto RETURN_TRUE;
	
	if (nElementTag == MATERIAL10_TAG || nElementTag == MATERIAL20_TAG || nElementTag == MATERIAL50_TAG || nElementTag == MATERIAL60_TAG)
	{
		const UData* pData3 = rElement.GetData(nCurrentIdx++);
		if (pData3 == 0)
			goto RETURN_FALSE;
		Element* pElement3 = (Element*)pData3;
		int a = 0;
		if (!ReadSpecularColor(*pElement3, *pMaterial, a))
			goto RETURN_FALSE;
	}
	else if (nElementTag == MATERIAL30_TAG || nElementTag == MATERIAL70_TAG)
	{
		const UData* pData3 = rElement.GetData(nCurrentIdx++);
		if (pData3 == 0)
			goto RETURN_FALSE;
		Element* pElement3 = (Element*)pData3;

		for( int i = 0 ; i < 4 ; i++)
		{
			if (!ReadUnitData<float>(*pElement, (pMaterial->m_arrEmissiveColor[i]), __FLOAT, L"Float", m_strError))
				goto RETURN_FALSE;
		}		
	}
	else if (nElementTag == MATERIAL40_TAG)
	{
		const UData* pData3 = rElement.GetData(nCurrentIdx++);
		if (pData3 == 0)
			goto RETURN_FALSE;
		Element* pElement3 = (Element*)pData3;
		int a = 0;
		if (!ReadMaterialTexture(*pElement3, *pMaterial, a))
			goto RETURN_FALSE;
	}
	else
	{
		m_strError = strTagDesc + L" 데이터 구성에 오류가 있습니다.";
		goto RETURN_FALSE;
	}

	if (nDataCount == nCurrentIdx)
		goto RETURN_TRUE;

	if (nElementTag == MATERIAL20_TAG || nElementTag == MATERIAL60_TAG)
	{
		const UData* pData3 = rElement.GetData(nCurrentIdx++);
		if (pData3 == 0)
			goto RETURN_FALSE;
		Element* pElement3 = (Element*)pData3;
		for( int i = 0 ; i < 4 ; i++)
		{
			if (!ReadUnitData<float>(*pElement, (pMaterial->m_arrEmissiveColor[i]), __FLOAT, L"Float", m_strError))
				goto RETURN_FALSE;
		}		
	}
	else if (nElementTag == MATERIAL50_TAG)
	{
		const UData* pData3 = rElement.GetData(nCurrentIdx++);
		if (pData3 == 0)
			goto RETURN_FALSE;
		Element* pElement3 = (Element*)pData3;
		int a = 0;
		if (!ReadMaterialTexture(*pElement3, *pMaterial, a))
			goto RETURN_FALSE;
	}
	else
	{
		m_strError = strTagDesc + L" 데이터 구성에 오류가 있습니다.";
		goto RETURN_FALSE;
	}

	if (nDataCount == nCurrentIdx)
		goto RETURN_TRUE;

	if (nElementTag == MATERIAL60_TAG)
	{
		const UData* pData3 = rElement.GetData(nCurrentIdx++);
		if (pData3 == 0)
			goto RETURN_FALSE;
		Element* pElement3 = (Element*)pData3;
		int a = 0;
		if (!ReadMaterialTexture(*pElement3, *pMaterial, a))
			goto RETURN_FALSE;
	}
	else
	{
		m_strError = strTagDesc + L" 데이터 구성에 오류가 있습니다.";
		goto RETURN_FALSE;
	}

RETURN_TRUE:
	m_vecMaterial.push_back(pMaterial);
	return true;

RETURN_FALSE:
	delete pMaterial;
	return false;
}

bool MeshFactory::ReadSpecularColor(Element& rElement, Material& rMaterial, int& rBeginIndex)
{

	for( int i = 0 ; i < 4 ; i++)
	{
		if (!ReadUnitData<float>(rElement, (rMaterial.m_arrSpecularColor[i]), __FLOAT, L"Float", m_strError, rBeginIndex++))
			return false;
	}

	const UData* pData3 = rElement.GetData(4);
	if (pData3 == 0)
		return false;
	Element* pElement3 = (Element*)pData3;
	if (!ReadUnitData<int>(*pElement3, rMaterial.m_nShininess, __LONG, L"LONG", m_strError, 0))
		return false;

	return true;
}

bool MeshFactory::ReadMaterialTexture(Element& rElement, Material& rMaterial, int& rBeginIndex)
{
	const std::wstring& strTagDesc = TAG_MAP[rElement.GetTag()];
	rElement.SetDescription(strTagDesc);

	int nTextureID;

	if (!ReadUnitData<int>(rElement, nTextureID, __LONG, L"LONG", m_strError, rBeginIndex++))
		return false;

	if (!ReadUnitData<bool>(rElement, rMaterial.m_useTexture, __BOOL, L"BOOLEAN", m_strError, rBeginIndex++))
		return false;

	Texture* pTexture = (Texture*)FindTexture(nTextureID);

	if (pTexture == 0)
	{
		wchar_t strError[256];
		swprintf(strError, 256, L"%s 아래에 존재하지 않는 TextureID가(%d) 있습니다.", strTagDesc.c_str(), nTextureID);
		m_strError = strError;
		return false;
	}

	rMaterial.m_pTexture = pTexture;
	return true;
}

bool MeshFactory::ReadTextureGroup(Element& rElement)
{
	rElement.SetDescription(TAG_MAP[rElement.GetTag()]);

	unsigned int nDataCount = rElement.GetDataCount();

	for (unsigned int i=0;i<nDataCount;i++)
	{
		const UData* pData = rElement.GetData(i);
		if (pData == 0) continue;

		if (pData->GetClassType() != UData::ELEMENT)
		{
			m_strError = L"TextureGroup 아래에 Segment가 존재합니다. TextureGroup에는 Element만 있어야 합니다.";
			return false;
		}

		Element* pElement = (Element*)pData;
		int nTag = pElement->GetTag();

		if (nTag == TEXTURE_TAG)
		{
			if (!ReadTexture(*pElement))
				return false;
		}
		else
		{
			wchar_t strError[256];
			swprintf(strError, 256, L"TextureGroup 안에 알 수 없는 Element(%s)가 존재합니다.", pElement->GetTagString().c_str());
			m_strError = strError;
			return false;
		}
	}

	return true;
}

bool MeshFactory::ReadTexture(Element& rElement)
{
	rElement.SetDescription(TAG_MAP[rElement.GetTag()]);
	Texture* pTexture = new Texture;

	if (!ReadUnitData<int>(rElement, pTexture->m_nTextureID, __LONG, L"LONG", m_strError))
		goto RETURN_FALSE;
	if (!ReadString(rElement, pTexture->m_strImagePath, m_strError, 1))
		goto RETURN_FALSE;

	m_vecTexture.push_back(pTexture);
	return true;

RETURN_FALSE:
	delete pTexture;
	return false;
}

bool MeshFactory::Read2DVerticesGroup(Element& rElement)
{
	rElement.SetDescription(TAG_MAP[rElement.GetTag()]);

	unsigned int nDataCount = rElement.GetDataCount();

	for (unsigned int i=0;i<nDataCount;i++)
	{
		const UData* pData = rElement.GetData(i);
		if (pData == 0) continue;

		if (pData->GetClassType() != UData::ELEMENT)
		{
			m_strError = L"2DVerticesGroup 아래에 Segment가 존재합니다. 2DVerticesGroup에는 Element만 있어야 합니다.";
			return false;
		}

		Element* pElement = (Element*)pData;
		int nTag = pElement->GetTag();

		if (nTag == _2DVERTICES_TAG)
		{
			if (!Read2DVertices(*pElement))
				return false;
		}
		else
		{
			wchar_t strError[256];
			swprintf(strError, 256, L"2DVerticesGroup 안에 알 수 없는 Element(%s)가 존재합니다.", pElement->GetTagString().c_str());
			m_strError = strError;
			return false;
		}
	}

	return true;
}

bool MeshFactory::Read2DVertices(Element& rElement)
{
	const std::wstring& strTagDesc = TAG_MAP[rElement.GetTag()];
	rElement.SetDescription(strTagDesc);

	Vertices* p2DVertices = new Vertices;
	unsigned int nDataCount = rElement.GetDataCount();

	for (unsigned int i=0;i<nDataCount;i++)
	{
		const UData* pData = rElement.GetData(i);
		if (pData == 0) continue;

		if (pData->GetClassType() == UData::SEGMENT)
		{
			if (!ReadUnitSegment<int>(strTagDesc, (const Segment*)pData, p2DVertices->m_nID, __LONG, L"LONG", m_strError))
				return false;
		}
		else
		{
			Element* pElement = (Element*)pData;
			int nTag = pElement->GetTag();

			if (nTag == _2DVERTEX_TAG)
			{
				if (!Read2DVertex(*pElement, *p2DVertices))
				{
					delete p2DVertices;
					return false;
				}
			}
			else
			{
				wchar_t strError[256];
				swprintf(strError, 256, L"2DVertices 안에 알 수 없는 Element(%s)가 존재합니다.", pElement->GetTagString().c_str());
				m_strError = strError;
				delete p2DVertices;
				return false;
			}
		}
	}

	m_vec2DVertices.push_back(p2DVertices);
	return true;
}

bool MeshFactory::Read2DVertex(Element& rElement, Vertices& r2DVertices)
{
	rElement.SetDescription(TAG_MAP[rElement.GetTag()]);

	Vertex vertex;
	float* arrData[2] = {&vertex.x, &vertex.y};
	
	if (!ReadArrayPtrData<float>(rElement, arrData, __FLOAT, L"FLOAT", m_strError, 2))
		return false;

	r2DVertices.m_vecVertex.push_back(vertex);
	return true;
}

bool MeshFactory::Read3DVerticesGroup(Element& rElement)
{
	rElement.SetDescription(TAG_MAP[rElement.GetTag()]);

	unsigned int nDataCount = rElement.GetDataCount();

	for (unsigned int i=0;i<nDataCount;i++)
	{
		const UData* pData = rElement.GetData(i);
		if (pData == 0) continue;

		if (pData->GetClassType() != UData::ELEMENT)
		{
			m_strError = L"3DVerticesGroup 아래에 Segment가 존재합니다. 3DVerticesGroup에는 Element만 있어야 합니다.";
			return false;
		}

		Element* pElement = (Element*)pData;
		int nTag = pElement->GetTag();

		if (nTag == _3DVERTICES_TAG)
		{
			if (!Read3DVertices(*pElement))
				return false;
		}
		else
		{
			wchar_t strError[256];
			swprintf(strError, 256, L"3DVerticesGroup 안에 알 수 없는 Element(%s)가 존재합니다.", pElement->GetTagString().c_str());
			m_strError = strError;
			return false;
		}
	}

	return true;
}

bool MeshFactory::Read3DVertices(Element& rElement)
{
	const std::wstring& strTagDesc = TAG_MAP[rElement.GetTag()];
	rElement.SetDescription(strTagDesc);

	Vertices* p3DVertices = new Vertices;
	unsigned int nDataCount = rElement.GetDataCount();

	for (unsigned int i=0;i<nDataCount;i++)
	{
		const UData* pData = rElement.GetData(i);
		if (pData == 0) continue;

		if (pData->GetClassType() == UData::SEGMENT)
		{
			if (!ReadUnitSegment<int>(strTagDesc, (const Segment*)pData, p3DVertices->m_nID, __LONG, L"LONG", m_strError))
				return false;
		}
		else
		{
			Element* pElement = (Element*)pData;
			int nTag = pElement->GetTag();

			if (nTag == _3DVERTEX_TAG)
			{
				if (!Read3DVertex(*pElement, *p3DVertices))
				{
					delete p3DVertices;
					return false;
				}
			}
			else
			{
				wchar_t strError[256];
				swprintf(strError, 256, L"3DVertices 안에 알 수 없는 Element(%s)가 존재합니다.", pElement->GetTagString().c_str());
				m_strError = strError;
				delete p3DVertices;
				return false;
			}
		}
	}

	m_vec3DVertices.push_back(p3DVertices);
	return true;
}

bool MeshFactory::Read3DVertex(Element& rElement, Vertices& r3DVertices)
{
	rElement.SetDescription(TAG_MAP[rElement.GetTag()]);
	
	Vertex vertex;
	float* arrData[6] = {&vertex.x, &vertex.y, &vertex.z, &vertex.nx, &vertex.ny, &vertex.nz};

	for( int i = 0 ; i < 6; i++)
	{
		float temp;
		if (!ReadUnitData<float>(rElement, temp, __FLOAT, L"FLAOT", m_strError, i))
			return false;
		*(arrData[i]) = temp;
	}

	r3DVertices.m_vecVertex.push_back(vertex);
	return true;
}
