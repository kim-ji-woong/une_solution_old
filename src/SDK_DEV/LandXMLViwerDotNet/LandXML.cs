using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Xml;
using System.IO;
using System.Windows.Forms;
using UserCtrls;

namespace UBMLViewer
{
	class LandXML
	{
		private XmlNamespaceManager nsmgr;
		private XmlDocument xDoc;
		private PointManager ptMgr = new PointManager();

		private OracleManager m_dbMgr = null;

		public void LoadLandXmlFile(string sfilename, OracleManager dbMgr = null)
		{
			m_dbMgr = dbMgr;
			try
			{				

				xDoc = new XmlDocument();

				FileStream fs = new FileStream(sfilename, FileMode.Open, FileAccess.Read, FileShare.None, 9024, false);

				xDoc.Load(fs);
				xDoc.Normalize();

				XmlElement xele = xDoc.DocumentElement;

				nsmgr = new XmlNamespaceManager(xDoc.NameTable);

				if (xDoc.DocumentElement.NamespaceURI == "http://www.landxml.org/schema/LandXML-1.1")
				{
					nsmgr.AddNamespace("lx", "http://www.landxml.org/schema/LandXML-1.1");
				}
				else
				{
					nsmgr.AddNamespace("lx", "http://www.landxml.org/schema/LandXML-1.2");
				}

				LoadAllSurfaces(ref xele);
				LoadAllCgPointGroups(ref xele);
				LoadAllParcelGroups(ref xele);
				LoadAllAlignmentGroups(ref xele);

				fs.Close();			
			}
			catch (Exception err)
			{
				MessageBox.Show(err.Message + "\n" + err.StackTrace);				
			}
		}

		private void LoadAllCgPointGroups(ref XmlElement xele)
		{
			XmlNodeList cgList = xele.SelectNodes(".//lx:CgPoints", nsmgr);
			IEnumerator cgIe = cgList.GetEnumerator();
			while (cgIe.MoveNext())
			{
				XmlNode xmlCgGroup = (XmlNode)cgIe.Current;
				TreeNode treeGroupNode = new TreeNode(xmlCgGroup.Name, 0, 0);
				treeGroupNode.Checked = true;

				LoadAllCgPoints(ref treeGroupNode, ref xmlCgGroup);

				//this.treePlan.Nodes.Add(treeGroupNode);
			}
		}

		private void LoadAllCgPoints(ref TreeNode oCgGroup, ref XmlNode xmlCgPointGroup)
		{
			XmlNodeList cgList = xmlCgPointGroup.SelectNodes(".//lx:CgPoint", nsmgr);
			IEnumerator cgIe = cgList.GetEnumerator();
			while (cgIe.MoveNext())
			{
				Parcel ptObj = new Parcel();

				XmlNode cgNode = (XmlNode)cgIe.Current;


				string strName = cgNode.Attributes.GetNamedItem("name").Value;
				ptObj.Name = strName;

				CgPoint cgPt = Utils.GetDrawPoint(cgNode.InnerText);
				cgPt.PType = CgPoint.PointType.COGO;
				cgPt.PointName = strName;

				ptMgr.AddCgPoint(cgPt);

				if (m_dbMgr != null)
					m_dbMgr.InsertData(cgPt);

				GeometryElement de = new GeometryElement();

				XmlNode attrMonument = cgNode.Attributes.GetNamedItem("pntSurv");
				ptObj.GeomType = cgNode.Name;
				de.DrawElementType = GeometryElement.ElementType.POINT;

				if (attrMonument != null)
				{
					ptObj.GeomType = attrMonument.Value;
					de.DrawElementType = GeometryElement.ElementType.MONUMENT;
				}
				de.CenterPoint = cgPt;
				de.ID = strName;

				ptObj.AddDrawElement(de);
				
				TreeNode cgTreeNode = new TreeNode(strName, 0, 0);
				cgTreeNode.Checked = true;
				oCgGroup.Nodes.Add(cgTreeNode);
			}
		}

		private void LoadAllParcelGroups(ref XmlElement xele)
		{
			XmlNode xmlParcelGroup = xele.SelectSingleNode(".//lx:Parcels", nsmgr);				
			TreeNode treeGroupNode = new TreeNode(xmlParcelGroup.Name, 0, 0);
			treeGroupNode.Checked = true;
				
			XmlNodeList parcelList = xmlParcelGroup.ChildNodes;
			IEnumerator parcelIe = parcelList.GetEnumerator();
			while (parcelIe.MoveNext())
			{
				XmlNode parcelNode = (XmlNode)parcelIe.Current;
				XmlNode nameNode = parcelNode.Attributes.GetNamedItem("name");
				string szName = "";
				if( nameNode != null)
					szName = nameNode.Value;

				TreeNode oTreeParcel = new TreeNode(szName, 0, 0);
				oTreeParcel.Checked = true;
				treeGroupNode.Nodes.Add(oTreeParcel);

				Parcel parcelObj = new Parcel();
				parcelObj.GeomType = "Parcel";
				parcelObj.Name = szName;
				oTreeParcel.Tag = parcelObj;
				
				try
				{
					XmlNodeList parcelsList = parcelNode.ChildNodes;
					IEnumerator parcelsIe = parcelsList.GetEnumerator();
					while (parcelsIe.MoveNext())
					{
						XmlNode parcelsNode = (XmlNode)parcelsIe.Current;
						if (parcelsNode.Name == "Parcels")
						{
							LoadAllParcels(ref oTreeParcel, ref parcelsNode);
						}
					}
				}
				catch (System.Exception ex)
				{					
				}
				
				if( m_dbMgr != null)
					m_dbMgr.InsertData(parcelObj);

			}
		}

		private void LoadAllParcels(ref TreeNode oTreeNode, ref XmlNode xmlParcels)
		{
			XmlNodeList parcelList = xmlParcels.ChildNodes;
			IEnumerator parcelIe = parcelList.GetEnumerator();
			while (parcelIe.MoveNext())
			{
				XmlNode parcelNode = (XmlNode)parcelIe.Current;
				XmlNode coordGeom = (XmlNode)parcelNode.SelectSingleNode(".//lx:CoordGeom", nsmgr);

				Parcel parcelObj = (Parcel)oTreeNode.Tag;
				
				this.LoadCoordGeometryElement(ref coordGeom, ref parcelObj);

				XmlNode nameNode = parcelNode.Attributes.GetNamedItem("name");
				string szName = "";
				if (nameNode != null)
					szName = nameNode.Value;
				TreeNode oTreeParcel = new TreeNode(szName, 0, 0);
				oTreeParcel.Checked = true;

				oTreeNode.Nodes.Add(oTreeParcel);
			}
		}

		private void LoadAllAlignmentGroups(ref XmlElement xele)
		{
			XmlNodeList cgList = xele.SelectNodes(".//lx:Alignments", nsmgr);
			IEnumerator cgIe = cgList.GetEnumerator();
			while (cgIe.MoveNext())
			{
				XmlNode xmlGroup = (XmlNode)cgIe.Current;
				TreeNode treeGroupNode = new TreeNode(xmlGroup.Name, 0, 0);
				treeGroupNode.Checked = true;

				LoadAllAlignments(ref treeGroupNode, ref xmlGroup);

				//this.treePlan.Nodes.Add(treeGroupNode);
			}
		}

		private void LoadAllSurfaces(ref XmlElement xele)
		{
			XmlNodeList cgList = xele.SelectNodes(".//lx:Surfaces", nsmgr);
			IEnumerator cgIe = cgList.GetEnumerator();
			while (cgIe.MoveNext())
			{
				XmlNode xmlGroup = (XmlNode)cgIe.Current;
				TreeNode treeGroupNode = new TreeNode(xmlGroup.Name, 0, 0);
				treeGroupNode.Checked = true;

				LoadSurfaceGroup(ref treeGroupNode, ref xmlGroup);

				//this.treePlan.Nodes.Add(treeGroupNode);
			}
		}

		private void LoadSurfaceGroup(ref TreeNode oTreeNode, ref XmlNode xmlNode)
		{
			XmlNodeList cgList = xmlNode.SelectNodes(".//lx:Surface", nsmgr);
			IEnumerator cgIe = cgList.GetEnumerator();
			while (cgIe.MoveNext())
			{
				XmlNode xmlSurface = (XmlNode)cgIe.Current;
				string sSurfaceName = xmlSurface.Attributes.GetNamedItem("name").Value;
				TreeNode treeGroupNode = new TreeNode(sSurfaceName, 0, 0);
				treeGroupNode.Checked = true;

				LoadSurface(ref treeGroupNode, xmlSurface);

				oTreeNode.Nodes.Add(treeGroupNode);
			}
		}

		private void LoadSurface(ref TreeNode oTreeNode, XmlNode xmlNode)
		{
			try
			{

				string strCheck = xmlNode.Name;
				string strSurfName = xmlNode.Attributes.GetNamedItem("name").Value;
				TreeNode defTreeNode = new TreeNode("Definition", 0, 0);
				defTreeNode.Checked = true;

				XmlNode xmlPntsNode = xmlNode.SelectSingleNode(".//lx:Pnts", nsmgr);
				if (xmlPntsNode != null)
				{
					TreeNode pntsNode = new TreeNode("Pnts", 0, 0);
					pntsNode.Checked = true;
					LoadSurfacePnts(strSurfName, ref pntsNode, xmlPntsNode);
					defTreeNode.Nodes.Add(pntsNode);
				}

				XmlNode xmlFacesNode = xmlNode.SelectSingleNode(".//lx:Faces", nsmgr);
				if (xmlFacesNode != null)
				{
					TreeNode facesNode = new TreeNode("Faces", 0, 0);
					facesNode.Checked = true;
					LoadSurfaceFaces(strSurfName, ref facesNode, xmlFacesNode);
					defTreeNode.Nodes.Add(facesNode);
				}
				if (xmlPntsNode != null || xmlFacesNode != null)
				{
					oTreeNode.Nodes.Add(defTreeNode);
				}
			}
			catch (Exception err)
			{
				MessageBox.Show("error in LoadSurface()" + err.Message + "\n" + err.StackTrace);
			}
		}

		private void LoadPnt2dList(ref XmlNode oPnt2dList, ref Parcel dobj)
		{
			string strPnts = oPnt2dList.InnerText;
			char[] space = { ' ' };
			string[] sPnts = strPnts.Split(space);

			GeometryElement de = new GeometryElement();
			CgPoint prevPt = new CgPoint();

			for (int i = 1; i < sPnts.Length; i = i + 2)
			{
				string sThisOffset = RemoveSpaces(sPnts[i - 1]);
				string sElev = this.RemoveSpaces(sPnts[i]);
				CgPoint surfPt = Utils.GetDrawPoint(sElev + " " + sThisOffset);

				if (i == 1)
				{
					de.StartPoint = surfPt;
				}
				else
				{
					de = new GeometryElement();
					de.DrawElementType = GeometryElement.ElementType.LINE;
					de.StartPoint = prevPt;
					de.EndPoint = surfPt;

					dobj.AddDrawElement(de);
				}
				prevPt = surfPt;
			}
		}

		private void LoadSurfacePnts(string strSurfName, ref TreeNode oTreeNode, XmlNode xmlNode)
		{
			try
			{
				XmlNodeList cgList = xmlNode.SelectNodes(".//lx:P", nsmgr);
				int iTest = 0;
				for (int i = 0; i < cgList.Count; i++)
				{
					Parcel ptObj = new Parcel();

					XmlNode cgNode = (XmlNode)cgList[i];

					string strName = cgNode.Attributes.GetNamedItem("id").Value;

					if (strName.IndexOf("1024") >= 0)
					{
						int r = 0;
					}

					ptObj.Name = strName;
					ptObj.GeomType = cgNode.Name;

					CgPoint cgPt = Utils.GetDrawPoint(cgNode.InnerText);
					cgPt.PType = CgPoint.PointType.P;
					cgPt.PointName = RemoveSpaces(strName);
					cgPt.ParentName = RemoveSpaces(strSurfName);
					
					ptMgr.AddGrPoint(cgPt);

					GeometryElement de = new GeometryElement();
					de.DrawElementType = GeometryElement.ElementType.XPOINT;
					de.CenterPoint = cgPt;
					de.ID = strName;

					ptObj.AddDrawElement(de);
					//int iIndex = graphicsPlan.AddDrawObject(ptObj);

					//TreeNode cgTreeNode = new TreeNode(strName, 0, 0);
					//cgTreeNode.Checked = true;
					//cgTreeNode.Tag = iIndex;
					//oTreeNode.Nodes.Add(cgTreeNode);
				}
			}
			catch (Exception err)
			{
				MessageBox.Show("error in LoadSurfacePnts()" + err.Message + "\n" + err.StackTrace);
			}

		}

		private void LoadSurfaceFaces(string sSurfName, ref TreeNode oTreeNode, XmlNode xmlNode)
		{
			try
			{
				XmlNodeList fList = xmlNode.SelectNodes("./lx:F", nsmgr);
				IEnumerator fIe = fList.GetEnumerator();
				while (fIe.MoveNext())
				{
					XmlNode fNode = (XmlNode)fIe.Current;
					string str = fNode.InnerText;

					char[] spaces = { ' ' };
					string[] strs = str.Split(spaces);

					if (strs.Length < 3)
					{
						MessageBox.Show("Error in split");
						return;
					}

					string sOne = RemoveSpaces(sSurfName + "." + strs[0]);
					string sTwo = RemoveSpaces(sSurfName + "." + strs[1]);
					string sThree = RemoveSpaces(sSurfName + "." + strs[2]);

					CgPoint ptOne = ptMgr.GetGrPoint(sOne);
					CgPoint ptTwo = ptMgr.GetGrPoint(sTwo);
					CgPoint ptThree = ptMgr.GetGrPoint(sThree);					

					if (ptOne != null && ptTwo != null && ptThree != null)
					{

						Parcel dObj = new Parcel();
						dObj.GeomType = "F";

						GeometryElement de = new GeometryElement();
						de.DrawElementType = GeometryElement.ElementType.LINE;
						de.StartPoint = ptOne;
						de.EndPoint = ptTwo;


						GeometryElement de2 = new GeometryElement();
						de2.DrawElementType = GeometryElement.ElementType.LINE;
						de2.StartPoint = ptTwo;
						de2.EndPoint = ptThree;

						GeometryElement de3 = new GeometryElement();
						de3.DrawElementType = GeometryElement.ElementType.LINE;
						de3.StartPoint = ptThree;
						de3.EndPoint = ptOne;


						dObj.AddDrawElement(de);
						dObj.AddDrawElement(de2);
						dObj.AddDrawElement(de3);

						//int iIndex = graphicsPlan.AddDrawObject(dObj);

						//TreeNode oTreeFace = new TreeNode("F", 0, 0);
						//oTreeFace.Checked = true;
						//oTreeFace.Tag = iIndex;

						//oTreeNode.Nodes.Add(oTreeFace);

						//DrawObject drwObj = new DrawObject();
					}
					else
					{
						if (ptOne == null)
						{
							Console.WriteLine("Face ERROR, P id not found: {0}", sOne);
							// MessageBox.Show(sOne);
						}

						if (ptTwo == null)
						{
							Console.WriteLine("Face ERROR, P id not found: {0}", sTwo);
							//MessageBox.Show(sTwo);
						}

						if (ptThree == null)
						{
							Console.WriteLine("Face ERROR, P id not found: {0}", sThree);
							// MessageBox.Show(sThree);
						}
					}

				}
			}
			catch (Exception err)
			{
				MessageBox.Show("error in LoadSurfaceFaces()" + err.Message + "\n" + err.StackTrace);
			}

		}

		private string RemoveSpaces(string s)
		{
			string sRet = "";
			string sLower = s.ToLower();
			char[] lowers = sLower.ToCharArray();
			char[] chars = s.ToCharArray();

			for (int i = 0; i < chars.Length; i++)
			{
				char c = lowers[i];
				if (c == 'a' ||
					c == 'b' ||
					c == 'c' ||
					c == 'd' ||
					c == 'e' ||
					c == 'f' ||
					c == 'g' ||
					c == 'h' ||
					c == 'i' ||
					c == 'j' ||
					c == 'k' ||
					c == 'l' ||
					c == 'm' ||
					c == 'n' ||
					c == 'o' ||
					c == 'p' ||
					c == 'q' ||
					c == 'r' ||
					c == 's' ||
					c == 't' ||
					c == 'u' ||
					c == 'v' ||
					c == 'w' ||
					c == 'x' ||
					c == 'y' ||
					c == 'z' ||
					c == '1' ||
					c == '2' ||
					c == '3' ||
					c == '4' ||
					c == '5' ||
					c == '6' ||
					c == '7' ||
					c == '8' ||
					c == '9' ||
					c == '0' ||
					c == '.')
				{
					sRet += chars[i];
				}
			}
			return sRet;
		}

		private void LoadAllAlignments(ref TreeNode oTreeNode, ref XmlNode xmlNode)
		{
			XmlNodeList aliList = xmlNode.SelectNodes(".//lx:Alignment", nsmgr);
			IEnumerator aliIe = aliList.GetEnumerator();
			while (aliIe.MoveNext())
			{
				XmlNode aliNode = (XmlNode)aliIe.Current;
				string sAliName = aliNode.Attributes.GetNamedItem("name").Value;
				XmlNode coordGeom = (XmlNode)aliNode.SelectSingleNode(".//lx:CoordGeom", nsmgr);

				XmlNodeList profNodeList = aliNode.SelectNodes("lx:Profile", nsmgr);
				this.LoadProfiles(profNodeList);

				Parcel aliObj = new Parcel();
				aliObj.Name = sAliName;
				aliObj.GeomType = aliNode.Name;

				this.LoadCoordGeometryElement(ref coordGeom, ref aliObj);


				//int iIndex = graphicsPlan.AddDrawObject(aliObj);
				//TreeNode oTreeAlign = new TreeNode(sAliName, 0, 0);
				//oTreeAlign.Checked = true;
				//oTreeAlign.Tag = iIndex;

				//oTreeNode.Nodes.Add(oTreeAlign);
			}
		}

		private void LoadProfiles(XmlNodeList oProfiles)
		{
			if (oProfiles.Item(0) != null)
			{
				LoadProfile(oProfiles.Item(0));
			}
		}

		private void LoadProfile(XmlNode oProfile)
		{
			if (oProfile.NodeType == XmlNodeType.Element)
			{
				XmlNodeList profNodes = oProfile.ChildNodes;
				XmlNode oNameAttr = oProfile.Attributes.GetNamedItem("name");

				string sProfName = oProfile.Name;
				if (oNameAttr != null)
				{
					sProfName = oNameAttr.InnerText;
				}
				TreeNode treeProfile = new TreeNode(sProfName, 0, 0);
				treeProfile.Checked = true;
				for (int i = 0; i < profNodes.Count; i++)
				{
					XmlNode profNode = (XmlNode)profNodes.Item(i);
					string sProfNode = profNode.Name;

					if (sProfNode.CompareTo("ProfAlign") == 0)
					{
						LoadProfileAlign(profNode, ref treeProfile);
					}
					else if (sProfNode.CompareTo("ProfSurf") == 0)
					{
						LoadProfileSurf(profNode, ref treeProfile);
					}

				}
				//this.treeProfiles.Nodes.Add(treeProfile);
			}
		}

		private void LoadProfileSurf(XmlNode profSurf, ref TreeNode profTreeNode)
		{
			XmlNode profAttr = profSurf.Attributes.GetNamedItem("name");
			string strName = "ProfSurf";
			if (profAttr != null)
			{
				strName = profAttr.InnerText;
			}
			Parcel profObj = new Parcel();
			profObj.Show = true;
			profObj.GeomType = "ProfSurf";

			XmlNode oPnt2dNode = profSurf.SelectSingleNode("./lx:PntList2D", nsmgr);

			if (oPnt2dNode != null)
			{
				LoadPnt2dList(ref oPnt2dNode, ref profObj);
			}

			//int iNdx = this.graphicsProfile.AddDrawObject(profObj);

			//TreeNode oProfSurfNode = new TreeNode(strName, 0, 0);
			//oProfSurfNode.Tag = iNdx;
			//profTreeNode.Nodes.Add(oProfSurfNode);
		}

		private void LoadProfileAlign(XmlNode profAlign, ref TreeNode profTreeNode)
		{
			XmlNodeList childNodes = profAlign.ChildNodes;

			XmlNode profAttr = profAlign.Attributes.GetNamedItem("name");
			string strName = "ProfAlign";
			if (profAttr != null)
			{
				strName = profAttr.InnerText;
			}

			Parcel profObj = new Parcel();
			profObj.Show = true;
			profObj.GeomType = "ProfAlign";


			CgPoint prfPtOne = new CgPoint();
			CgPoint prfPtTwo = new CgPoint();

			//StringBuilder sb = new StringBuilder();

			for (int i = 0; i < childNodes.Count; i++)
			{
				XmlNode xNode = childNodes.Item(i);
				string sProfNode = xNode.Name;

				if (sProfNode.CompareTo("PVI") == 0)
				{
					if (i == 0)
					{
						prfPtOne = Utils.GetSectionPoint(xNode.InnerText);
						prfPtOne.PType = CgPoint.PointType.PVI;
						prfPtOne.PointName = "PVI";
						//sb.Append(prfPtOne.XCoordinate + " " + prfPtOne.YCoordinate + " ");

					}
					else
					{
						prfPtTwo = Utils.GetSectionPoint(xNode.InnerText);
						prfPtTwo.PType = CgPoint.PointType.PVI;
						prfPtTwo.PointName = "PVI";
						//sb.Append(prfPtTwo.XCoordinate + " " + prfPtTwo.YCoordinate + "\n");

						GeometryElement de = new GeometryElement();
						de.DrawElementType = GeometryElement.ElementType.LINE;
						de.StartPoint = prfPtOne;
						de.EndPoint = prfPtTwo;

						profObj.AddDrawElement(de);

						prfPtOne = prfPtTwo;

					}
				}
			}
			//System.Console.WriteLine(sb.ToString());
			//int iNdx = this.graphicsProfile.AddDrawObject(profObj);

			//TreeNode profTreeAlign = new TreeNode(strName, 0, 0);
			//profTreeAlign.Checked = true;
			//profTreeAlign.Tag = iNdx;

			//profTreeNode.Nodes.Add(profTreeAlign);

		}

		private void LoadCoordGeometryElement(ref XmlNode coordGeom, ref Parcel dobj)
		{
			try
			{
				XmlNodeList childlist = coordGeom.ChildNodes;
				IEnumerator childIe = childlist.GetEnumerator();
				while (childIe.MoveNext())
				{
					XmlNode childNode = (XmlNode)childIe.Current;
					string strName = childNode.Name;
					if (strName.CompareTo("Curve") == 0)
					{
						GeometryElement de = this.LoadArcNode(childNode);
						dobj.AddDrawElement(de);
					}
					else if (strName.CompareTo("Spiral") == 0)
					{
						GeometryElement de = LoadSpiralNode(childNode);
						dobj.AddDrawElement(de);
					}
					else if (strName.CompareTo("Line") == 0)
					{
						GeometryElement de = LoadLineNode(childNode);
						dobj.AddDrawElement(de);
					}
					else if (strName.CompareTo("IrregularLine") == 0)
					{
						GeometryElement de = LoadIrregularLineNode(childNode);
						dobj.AddDrawElement(de);
					}
					else if (strName.CompareTo("Chain") == 0)
					{
						GeometryElement de = LoadChainNode(childNode);
						dobj.AddDrawElement(de);
					}
				}
			}
			catch
			{
			}
		}

		private GeometryElement LoadSpiralNode(XmlNode spiralNode)
		{
			try
			{

				GeometryElement de = new GeometryElement();
				de.DrawElementType = GeometryElement.ElementType.SPIRAL;

				XmlNode startNode = spiralNode.SelectSingleNode("./lx:Start", nsmgr);
				XmlNode piNode = spiralNode.SelectSingleNode("./lx:PI", nsmgr);
				XmlNode endNode = spiralNode.SelectSingleNode("./lx:End", nsmgr);

				CgPoint stPt = null;
				CgPoint enPt = null;
				CgPoint piPt = null;

				XmlNode attrStart = startNode.Attributes.GetNamedItem("pntRef");
				XmlNode attrPI = piNode.Attributes.GetNamedItem("pntRef");
				XmlNode attrEnd = endNode.Attributes.GetNamedItem("pntRef");

				if (attrStart != null)
				{
					XmlNode xmlRefPoint = xDoc.SelectSingleNode("//lx:CgPoint[@name=\"" + attrStart.Value + "\"]", nsmgr);
					stPt = Utils.GetDrawPoint(xmlRefPoint.InnerText);
				}
				else
				{
					stPt = Utils.GetDrawPoint(startNode.InnerText);
				}

				if (attrPI != null)
				{
					XmlNode xmlRefPoint = xDoc.SelectSingleNode("//lx:CgPoint[@name=\"" + attrPI.Value + "\"]", nsmgr);
					piPt = Utils.GetDrawPoint(xmlRefPoint.InnerText);
				}
				else
				{
					piPt = Utils.GetDrawPoint(piNode.InnerText);
				}

				if (attrEnd != null)
				{
					XmlNode xmlRefPoint = xDoc.SelectSingleNode("//lx:CgPoint[@name=\"" + attrEnd.Value + "\"]", nsmgr);
					enPt = Utils.GetDrawPoint(xmlRefPoint.InnerText);
				}
				else
				{
					enPt = Utils.GetDrawPoint(endNode.InnerText);
				}

				de.StartPoint = stPt;
				de.AddDrawPoint(piPt);
				de.AddDrawPoint(piPt);
				de.EndPoint = enPt;


				return de;
			}
			catch (Exception err)
			{
				return null;
			}
		}

		private GeometryElement LoadArcNode(XmlNode curvenode)
		{
			try
			{
				XmlNode startNode = curvenode.SelectSingleNode("./lx:Start", nsmgr);
				XmlNode cenNode = curvenode.SelectSingleNode("./lx:Center", nsmgr);
				XmlNode endNode = curvenode.SelectSingleNode("./lx:End", nsmgr);

				CgPoint stPt = null;
				CgPoint enPt = null;
				CgPoint cePt = null;

				XmlNode attrStart = startNode.Attributes.GetNamedItem("pntRef");
				XmlNode attrCenter = cenNode.Attributes.GetNamedItem("pntRef");
				XmlNode attrEnd = endNode.Attributes.GetNamedItem("pntRef");

				if (attrStart != null)
				{
					XmlNode xmlRefPoint = xDoc.SelectSingleNode("//lx:CgPoint[@name=\"" + attrStart.Value + "\"]", nsmgr);
					stPt = Utils.GetDrawPoint(xmlRefPoint.InnerText);
				}
				else
				{
					stPt = Utils.GetDrawPoint(startNode.InnerText);
				}

				if (attrCenter != null)
				{
					XmlNode xmlRefPoint = xDoc.SelectSingleNode("//lx:CgPoint[@name=\"" + attrCenter.Value + "\"]", nsmgr);
					cePt = Utils.GetDrawPoint(xmlRefPoint.InnerText);
				}
				else
				{
					cePt = Utils.GetDrawPoint(cenNode.InnerText);
				}

				if (attrEnd != null)
				{
					XmlNode xmlRefPoint = xDoc.SelectSingleNode("//lx:CgPoint[@name=\"" + attrEnd.Value + "\"]", nsmgr);
					enPt = Utils.GetDrawPoint(xmlRefPoint.InnerText);
				}
				else
				{
					enPt = Utils.GetDrawPoint(endNode.InnerText);
				}

				GeometryElement de = new GeometryElement();
				de.DrawElementType = GeometryElement.ElementType.ARC;
				de.StartPoint = stPt;
				de.CenterPoint = cePt;
				de.EndPoint = enPt;

				string strRot = curvenode.Attributes.GetNamedItem("rot").Value;
				string strRad = curvenode.Attributes.GetNamedItem("radius").Value;

				string strRotLower = strRot.ToLower();
				if (strRotLower.CompareTo("ccw") == 0)
				{
					de.IsCW = false;
				}
				else
				{
					de.IsCW = true;
				}

				double dRad = Utils.ConvertStringToDouble(strRad);
				de.Radius = dRad;

				return de;
			}
			catch
			{
				return null;
			}
		}

		private GeometryElement LoadLineNode(XmlNode linenode)
		{
			try
			{
				XmlNode startNode = linenode.SelectSingleNode("./lx:Start", nsmgr);
				XmlNode endNode = linenode.SelectSingleNode("./lx:End", nsmgr);

				XmlNode attrStart = startNode.Attributes.GetNamedItem("pntRef");
				XmlNode attrEnd = endNode.Attributes.GetNamedItem("pntRef");

				CgPoint stPt = null;
				CgPoint enPt = null;

				if (attrStart != null)
				{
					XmlNode xmlRefPoint = xDoc.SelectSingleNode("//lx:CgPoint[@name=\"" + attrStart.Value + "\"]", nsmgr);
					stPt = Utils.GetDrawPoint(xmlRefPoint.InnerText);
				}
				else
				{
					stPt = Utils.GetDrawPoint(startNode.InnerText);
				}

				if (attrEnd != null)
				{
					XmlNode xmlRefPoint = xDoc.SelectSingleNode("//lx:CgPoint[@name=\"" + attrEnd.Value + "\"]", nsmgr);
					enPt = Utils.GetDrawPoint(xmlRefPoint.InnerText);
				}
				else
				{
					enPt = Utils.GetDrawPoint(endNode.InnerText);
				}

				GeometryElement de = new GeometryElement();
				de.DrawElementType = GeometryElement.ElementType.LINE;
				de.StartPoint = stPt;
				de.EndPoint = enPt;

				return de;
			}
			catch (Exception err)
			{
				System.Console.WriteLine(err.Message + "\n" + err.StackTrace);
				return null;
			}
		}

		private GeometryElement LoadIrregularLineNode(XmlNode irrLineNode)
		{
			try
			{
				GeometryElement de = new GeometryElement();
				XmlNode startNode = irrLineNode.SelectSingleNode("./lx:Start", nsmgr);
				XmlNode endNode = irrLineNode.SelectSingleNode("./lx:End", nsmgr);

				XmlNode attrStart = startNode.Attributes.GetNamedItem("pntRef");
				XmlNode attrEnd = endNode.Attributes.GetNamedItem("pntRef");

				CgPoint stPt = null;
				CgPoint enPt = null;

				if (attrStart != null)
				{
					XmlNode xmlRefPoint = xDoc.SelectSingleNode("//lx:CgPoint[@name=\"" + attrStart.Value + "\"]", nsmgr);
					stPt = Utils.GetDrawPoint(xmlRefPoint.InnerText);
				}
				else
				{
					stPt = Utils.GetDrawPoint(startNode.InnerText);
				}

				if (attrEnd != null)
				{
					XmlNode xmlRefPoint = xDoc.SelectSingleNode("//lx:CgPoint[@name=\"" + attrEnd.Value + "\"]", nsmgr);
					enPt = Utils.GetDrawPoint(xmlRefPoint.InnerText);
				}

				XmlNode xmlPtList = irrLineNode.SelectSingleNode("./lx:PntList3D", nsmgr);
				if (xmlPtList != null)
				{

					ArrayList arResult = Utils.GetDrawPoint3D(xmlPtList.InnerText);
					for (int i = 0; i < arResult.Count; i++)
					{
						CgPoint pt = (CgPoint)arResult[i];
						pt.PType = CgPoint.PointType.P;
						de.AddDrawPoint(pt);

					}
				}
				de.DrawElementType = GeometryElement.ElementType.LINESTRING;
				return de;
			}
			catch (Exception err)
			{
				return null;
			}
		}

		private GeometryElement LoadChainNode(XmlNode chainNode)
		{
			try
			{

				GeometryElement de = new GeometryElement();
				de.DrawElementType = GeometryElement.ElementType.LINESTRING;

				return de;
			}
			catch (Exception err)
			{
				return null;
			}
		}

	}
}
