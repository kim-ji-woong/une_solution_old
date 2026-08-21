#include "StdAfx.h"
#include "Block.h"
#include "Shape.h"
#include "LineType.h"

using namespace System::Collections;

BEGIN_NS(DXFDotNet)

Block::Block(DXFControl^ ctrl)
{
	m_isHidden = false;
	m_isLock   = false;
	m_vOrigin  = gcnew UnE::Geometry::Vertex2D(0.0, 0.0);

	m_listObj  = gcnew ArrayList();
	m_lineType = gcnew LineType(ctrl);
	m_color = System::Drawing::Color::White;
	m_ctrl = ctrl;

	m_strBlockName = L"";
}

Block::~Block(void)
{
	RemoveAll();
}

void Block::Add(Shape^ pObj)
{
	if (pObj != nullptr)
		m_listObj->Add(pObj);
}

bool Block::Remove(Shape^ pObj)
{
	if (pObj == nullptr) return false;
	pObj->SetBlock(nullptr);

	if (m_listObj->Contains(pObj))
	{
		m_listObj->Remove(pObj);
		return true;
	}

	return false;
}

void Block::RemoveAll()
{
	m_listObj->Clear();
}

void Block::SetLineType(LineType^ lineType)
{
	m_lineType = lineType;
}

LineType^ Block::GetLineType()
{
	return m_lineType;
}

END_NS
