#include "StdAfx.h"
#include "Layer.h"
#include "IShapeOwner.h"
#include "Shape.h"
#include "Block.h"
#include "DXFControl.h"
#include "ShapeGroup.h"
#include "EntityFactory.h"

using namespace System::Collections;
using namespace UnE::Geometry;

BEGIN_NS(DXFDotNet)

Layer::Layer(IShapeOwner^ owner)
{
	m_owner = owner;
	m_lineType = nullptr;
	Init();
}

Layer::Layer(IShapeOwner^ owner, LineType^ lineType)
{
	m_owner = owner;
	m_lineType = lineType;
	Init();
}

Layer::~Layer(void)
{
	Reset();
}

void Layer::Add(Shape^ obj)
{
	m_listObject->Add(obj);
	obj->SetLayer(this);

	Block^ pBlock = m_owner->GetCurrentBlock();
	if (pBlock != nullptr) pBlock->Add(obj);
	obj->SetBlock(pBlock);

	obj->SetOwner(m_owner);
}

void Layer::Reset()
{
	m_listObject->Clear();
}

void Layer::Init()
{
	m_strLayerName = System::String::Format(L"{0:X}", this->GetHashCode());
	m_listObject = gcnew System::Collections::ArrayList();

	m_isHidden = false;
	m_isLock = false;
	m_isFrozen = false;
	m_isVisibleGroup = true;

	m_color = System::Drawing::Color::White;

	m_useGroupItem = false;
	m_listGroup = gcnew System::Collections::ArrayList();
	m_shpGroupOption = nullptr;
}

LineType^ Layer::GetLineType()
{
	return m_lineType;
}

void Layer::SetLineType(LineType^ type)
{
	m_lineType = type;
}

/*System::Drawing::Color^ Layer::GetLineColor()
{
	return m_color;
}

bool Layer::IsFrozen()
{
	return m_isFrozen;
}

bool Layer::IsLock()
{
	return m_isLock;
}

void Layer::SetLineColor(System::Drawing::Color^ col)
{
	m_color = col;
}

void Layer::SetFrozen(bool isFrozen)
{
	m_isFrozen = isFrozen;
}

void Layer::SetLock(bool isLock)
{
	m_isLock = isLock;
}*/

// pObject가 Layer에 존재하면 pObject를 삭제하고 true를 리턴한다.
bool Layer::Remove(Shape^ obj)
{
	if (obj == nullptr || m_isLock) return false;

	// 잠겨있는 Block 내에 존재하는 Object는 삭제할 수 없다.
	if (obj->GetBlock())
	{
		if (obj->GetBlock()->Lock)
			return false;
		else
			obj->GetBlock()->Remove(obj);
	}

	for each (Shape^ shp in m_listObject)
	{
		if (obj == shp)
		{
			m_listObject->Remove(obj);
			return true;
		}
	}

	return false;
}

// 모든 Object를 삭제하면 true,
// 삭제하지 못한 Object가 존재하면 false를 리턴한다.
bool Layer::RemoveAll()
{
	if (m_isLock) return false;

	for each (Shape^ obj in m_listObject)
	{
		if (obj->GetBlock())
		{
			if (obj->GetBlock()->Lock)
				continue;
			else
				obj->GetBlock()->Remove(obj);
		}
	}

	m_listObject->Clear();

	return m_listObject->Count == 0 ? true : false;
}

// pObject가 Layer에 존재하면 true를 리턴한다.
bool Layer::Find(Shape^ obj)
{
	return m_listObject->Contains(obj);
}

Shape^ Layer::SelectObject(double x, double y)
{
	double _x = x, _y = y;

	for each (ShapeGroup^ group in m_listGroup)
	{
		if (group->GetBlock())
		{
			UnE::Geometry::Vertex2D^ vOrigin = group->GetBlock()->OriginVertex;
			_x = x - vOrigin->x;
			_y = y - vOrigin->y;
		}
		else
		{
			_x = x;
			_y = y;
		}

		if (group->Visible && group->HitTest(_x, _y))
			return group;
	}

	for each (Shape^ obj in m_listObject)
	{
		if (obj->GetBlock())
		{
			UnE::Geometry::Vertex2D^ vOrigin = obj->GetBlock()->OriginVertex;
			_x = x - vOrigin->x;
			_y = y - vOrigin->y;
		}
		else
		{
			_x = x;
			_y = y;
		}

		if (obj->Visible && obj->HitTest(_x, _y))
			return obj;
	}

	return nullptr;
}

// 모든 객체들을 현재의 위치로부터 (x, y) 만큼 이동시킨다.
void Layer::MoveAll(double x, double y)
{
	for each (Shape^ obj in m_listObject)
	{
		obj->Move(x, y);
	}
}

void Layer::ClearShapeGroup()
{
	for each (ShapeGroup^ group in m_listGroup)
	{
		int nShapeCount = group->GetShapeCount();

		for (int i=0;i<nShapeCount;i++)
		{
			Shape^ shape = group->GetShape(i);

			if (shape)
				shape->Visible = true;
		}
	}

	m_listGroup->Clear();
}

void Layer::CalcGroup(int nGroupItemMinCount, int nGroupItemDistance)
{
	CalcGroup(nGroupItemMinCount, nGroupItemDistance, m_shpGroupOption);
}

void Layer::AddShapeGroup(ArrayList^ arrGroupItems, DXFDotNet::ShapeGroupOption^ option)
{
	if (arrGroupItems == nullptr)
		return;

	ShapeGroup^ group = m_owner->GetShapeFactory()->CreateShapeGroup(option);
	
	group->SetOwner(m_owner);

	Vertex2D^ vTL = nullptr;
	Vertex2D^ vBR = nullptr;
			
	for each (Shape^ shape in arrGroupItems)
	{
		shape->Visible = false;
		group->AddShape(shape);

		Vertex2D^ vPos = shape->Position;

		if (vTL == nullptr)
		{
			vTL = gcnew Vertex2D(vPos->x, vPos->y);
			vBR = gcnew Vertex2D(vPos->x, vPos->y);
		}
		else
		{
			if (vTL->x > vPos->x)
				vTL->x = vPos->x;
			if (vTL->y > vPos->y)
				vTL->y = vPos->y;
			if (vBR->x < vPos->x)
				vBR->x = vPos->x;
			if (vBR->y < vPos->y)
				vBR->y = vPos->y;
		}
	}

	group->ID = group->GetHashCode();
	group->Selectable = true;
	group->Position = (vTL + vBR) / 2;

	m_listGroup->Add(group);
}

void Layer::CalcGroup(int nGroupItemMinCount, int nGroupItemDistance, DXFDotNet::ShapeGroupOption^ option)
{
	ClearShapeGroup();

	if (!m_useGroupItem)
		return;

	ArrayList^ arrShapes = gcnew ArrayList();

	for each (Shape^ obj in m_listObject)
	{
		arrShapes->Add(obj);
	}

	System::Drawing::Point ptOrigin = m_owner->GlobalToScreen(gcnew Vertex2D(0, 0));

	while (arrShapes->Count > 0)
	{
		Shape^ obj = (Shape^)arrShapes[0];

		ArrayList^ arrGroupItems = GetGroupItems(nGroupItemMinCount, nGroupItemDistance, obj, arrShapes, ptOrigin);
		AddShapeGroup(arrGroupItems, option);
	}
}

int Layer::GetDistance(System::Drawing::Point% pt1, System::Drawing::Point% pt2)
{
	int x = (pt2.X - pt1.X) * (pt2.X - pt1.X);
	int y = (pt2.Y - pt1.Y) * (pt2.Y - pt1.Y);
	return (int)System::Math::Sqrt(x + y);
}

ArrayList^ Layer::GetGroupItems(int nGroupItemMinCount, int nGroupItemDistance, Shape^ obj, ArrayList^ arrShapes, System::Drawing::Point% ptOrigin)
{
	arrShapes->Remove(obj);

	ArrayList^ arrGroupItems = nullptr;
	Vertex2D^ vPos1 = obj->Position;

	for each (Shape^ shp in arrShapes)
	{
		Vertex2D^ vPos2 = shp->Position;

		double dLen = vPos1->GetDistance(vPos2);

		System::Drawing::Point ptPos = m_owner->GlobalToScreen(gcnew Vertex2D(0, dLen));
		int nLen = GetDistance(ptOrigin, ptPos);

		if (nLen <= nGroupItemDistance)
		{
			if (arrGroupItems == nullptr)
			{
				arrGroupItems = gcnew ArrayList();
				arrGroupItems->Add(obj);
			}

			arrGroupItems->Add(shp);
		}
	}

	if (arrGroupItems != nullptr)
	{
		int nItemCount = arrGroupItems->Count;

		for (int i=1;i<nItemCount;i++)
		{
			Shape^ shp = (Shape^)arrGroupItems[i];
			arrShapes->Remove(shp);
		}

		for (int i=1;i<nItemCount;i++)
		{
			Shape^ shp = (Shape^)arrGroupItems[i];
			GetGroupItems(nGroupItemMinCount, nGroupItemDistance, shp, arrShapes, arrGroupItems, ptOrigin);
		}

		nItemCount = arrGroupItems->Count;

		// Group을 생성하기 위한 최소 개수에 미달되면
		// Group을 생성하지 않는다.
		if (nItemCount < nGroupItemMinCount)
		{
			// 첫번째 Item은 obj이므로 arrShapes에 다시 넣지 않는다.
			for (int i=1;i<nItemCount;i++)
			{
				Shape^ shp = (Shape^)arrGroupItems[i];
				arrShapes->Add(shp);
			}

			return nullptr;
		}
	}

	return arrGroupItems;
}

void Layer::GetGroupItems(int nGroupItemMinCount, int nGroupItemDistance, Shape^ obj, ArrayList^ arrShapes, ArrayList^ arrGroupItems, System::Drawing::Point% ptOrigin)
{
	int nItemCountOrigin = arrGroupItems->Count;

	Vertex2D^ vPos1 = obj->Position;

	for each (Shape^ shp in arrShapes)
	{
		Vertex2D^ vPos2 = shp->Position;

		double dLen = vPos1->GetDistance(vPos2);

		if (dLen <= nGroupItemDistance)
		{
			arrGroupItems->Add(shp);
		}
	}

	int nItemCount2 = arrGroupItems->Count;

	for (int i=nItemCountOrigin;i<nItemCount2;i++)
	{
		Shape^ shp = (Shape^)arrGroupItems[i];
		arrShapes->Remove(shp);
	}

	for (int i=nItemCountOrigin;i<nItemCount2;i++)
	{
		Shape^ shp = (Shape^)arrGroupItems[i];
		GetGroupItems(nGroupItemMinCount, nGroupItemDistance, shp, arrShapes, arrGroupItems, ptOrigin);
	}
}

/*void Layer::SetLayerName(System::String^ strLayerName)
{
	System::String^ strName;

	if (strLayerName == L"DefPoints"))
	{
		strName = L"_";
		strName += strLayerName;
	}
	else
		strName = strLayerName;

	m_strLayerName = strName;
}

System::String^ Layer::GetLayerName()
{
	return m_strLayerName.data();
}

void Layer::SetOwner(IPainter^ owner)
{
	m_owner = owner;
}

IPainter^ Layer::GetOwner()
{
	return m_owner;
}*/

END_NS
