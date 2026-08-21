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


#include "C4Graph.h"


using namespace C4;


GraphElementBase::GraphElementBase()
{
}

GraphElementBase::~GraphElementBase()
{
}

GraphEdgeStart *GraphElementBase::FindOutgoingEdge(const GraphElementBase *finish) const
{
	GraphEdgeStart *edge = outgoingEdgeList.First();
	while (edge)
	{
		if (static_cast<GraphEdgeFinish *>(edge)->GetFinishElement() == finish) return (edge);
		edge = edge->Next();
	}
	
	return (nullptr);
}

GraphEdgeFinish *GraphElementBase::FindIncomingEdge(const GraphElementBase *start) const
{
	GraphEdgeFinish *edge = incomingEdgeList.First();
	while (edge)
	{
		if (edge->GetStartElement() == start) return (edge);
		edge = edge->ListElement<GraphEdgeFinish>::Next();
	}
	
	return (nullptr);
}


bool GraphBase::Predecessor(const GraphElementBase *first, const GraphElementBase *second)
{
	List<GraphElementBase>		readyList;
	List<GraphElementBase>		visitedList;
	
	readyList.Append(const_cast<GraphElementBase *>(first));
	bool result = false;
	
	for (;;)
	{
		GraphElementBase *element = readyList.First();
		if (!element) break;
		
		visitedList.Append(element);
		
		const GraphEdgeStart *edge = element->GetFirstOutgoingEdge();
		while (edge)
		{
			GraphElementBase *finish = static_cast<const GraphEdgeFinish *>(edge)->GetFinishElement();
			if (!visitedList.Member(finish))
			{
				if (finish == second)
				{
					result = true;
					goto end;
				}
				
				readyList.Append(finish);
			}
			
			edge = edge->Next();
		}
	}
	
	end:
	for (;;)
	{
		GraphElementBase *element = readyList.First();
		if (!element) break;
		
		elementList.Append(element);
	}
	
	for (;;)
	{
		GraphElementBase *element = visitedList.First();
		if (!element) break;
		
		elementList.Append(element);
	}
	
	return (result);
}

// ZYURVUR
