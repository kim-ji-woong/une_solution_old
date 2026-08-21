#pragma once

//-- stl ---
#include <string.h>
#include <math.h>
#include <stdio.h>
#include <list>
#include <map>
#include <iostream>
#include <list>

//-- forward declartation ---

namespace DXF
{
	class DXFManager;
	class SectionManager;
	class DXFOptions;

	namespace TABLES
	{
		class Table;
		class Layer;
		class VPort;
		class BlockRecord;
		class DimStyle;
		class Style;
	}	

	namespace BLOCKS
	{
		class BlockManager;
		class BlockData;
	}

	namespace OBJECTS
	{
		class ObjectManager;
	}

	namespace ENTITIES
	{
		class Entity;
	}

	struct _DXFData;
}

namespace Utility
{
	class FileManager;
}

//-----

#include "DXFTable.h"
#include "DXFLType.h"
#include "DXFEntity.h"
//-----
#include "DXF3DFace.h"
#include "DXF3DSolid.h"
#include "DXFAcadProxyEntity.h"
//-----
#include "DXFManager.h"
#include "DXFObject.h"
#include "DXFDictionary.h"
#include "DXFACDBDictionaryWDFLT.h"
#include "DXFACDBPlaceHolder.h"
#include "DXFACI.h"
#include "DXFAppID.h"
//-----
#include "DXFRoundRect.h"
#include "DXFCircle.h"
#include "DXFArc.h"
//-----
#include "DXFDimension.h"
#include "DXFArcDimension.h"
#include "DXFAttDef.h"
#include "DXFAttrib.h"
#include "DXFBlockData.h"
//-----
#include "DXFSectionManager.h"
#include "DXFBlockManager.h"
#include "DXFBlockRecord.h"
#include "DXFBody.h"
#include "DXFClassManager.h"
#include "DXFDimStyle.h"
#include "DXFEllipse.h"
#include "DXFEntityManager.h"
#include "DXFGeometry.h"
#include "DXFHatch.h"
//-----
#include "DXFHeaderData.h"
#include "DXFHeader.h"
#include "DXFImage.h"
#include "DXFInsert.h"
#include "DXFLayer.h"
#include "DXFLayout.h"
#include "DXFLine.h"
#include "DXFPoint.h"
#include "DXFLineType.h"
#include "DXFMLineStyle.h"
#include "DXFMText.h"
#include "DXFObjectManager.h"
#include "DXFPlotSettings.h"
#include "DXFPolyLine.h"
#include "DXFSolid.h"
#include "DXFStyle.h"
#include "DXFTableManager.h"
#include "DXFText.h"
#include "DXFUCS.h"
#include "DXFView.h"
#include "DXFVPort.h"
#include "FileManager.h"
#include "Vertex.h"
#include "DXFCommon.h"