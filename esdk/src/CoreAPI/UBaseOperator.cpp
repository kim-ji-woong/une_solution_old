#include "StdAfx.h"
#include "UBaseOperator.h"
#include "UBaseView.h"

using namespace UnE::Core;

UBaseOperator::UBaseOperator(void)
{
	m_pTargetView = NULL;
}


UBaseOperator::~UBaseOperator(void)
{
}

UOpType UnE::Core::UBaseOperator::GetType()
{
	return eOp_None;
}

void UnE::Core::UBaseOperator::Reset()
{
}
