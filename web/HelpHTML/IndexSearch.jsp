<%@ page language="java" contentType="text/html; charset=UTF-8" pageEncoding="UTF-8"%>
<%@page import="java.io.*" %>
<%@page import="java.util.*" %>

<%!
    public int GetDirectory(String strURL, Vector files, Vector folders)
    {
		if (strURL == null)
			log("strURL is null");
		else
		{
			File folder = new File(strURL);
			File[] fileList = folder.listFiles();

			if (fileList == null)
				log(strURL + " is not exist");
			else
			{
				for (int i = 0; i < fileList.length; i++)
				{
					if (fileList[i].isDirectory())
					{
						folders.add(fileList[i].getName());
					}
					else
					{
						files.add(fileList[i].getName());
					}
				}

				return 1;
			}
		}

		return 0;
    }
%>

<%
    String strLocalPath = request.getRealPath("/");
	strLocalPath = strLocalPath.replaceAll("\\\\", "/");

	if (strLocalPath.endsWith("/"))
		strLocalPath = strLocalPath.substring(0, strLocalPath.length() - 1);

    request.setCharacterEncoding("UTF-8");
    String strURL = request.getParameter("URL_PATH");

    if (strURL != null)
    {
        int nIndex1 = strURL.indexOf("//");

        if (nIndex1 >= 0)
        {
            int nIndex2 = strURL.indexOf("/", nIndex1 + 2);

            if (nIndex2 >= 0)
                nIndex1 = nIndex2;
            else
                nIndex1 = -1;
        }
        else
        {
            int nIndex2 = strURL.indexOf("/");

            if (nIndex2 >= 0)
                nIndex1 = nIndex2;
            else
                nIndex1 = -1;
        }

        if (nIndex1 >= 0)
        {
            String strFolder = strURL.substring(nIndex1, strURL.length());
            strLocalPath += strFolder;

            if (strLocalPath.endsWith("/") == false)
                strLocalPath += "/";

            Vector files = new Vector();
            Vector folders = new Vector();

            if (GetDirectory(strLocalPath, files, folders) == 0)
				out.println("<InvalidPath/>");
			else
			{
				String strFiles = "", strFolders = "";

				for (int i=0;i<files.size();i++)
				{
					if (strFiles.length() == 0)
						strFiles = files.elementAt(i).toString();
					else
						strFiles += ";" + files.elementAt(i).toString();
				}

				for (int i=0;i<folders.size();i++)
				{
					if (strFolders.length() == 0)
						strFolders = folders.elementAt(i).toString();
					else
						strFolders += ";" + folders.elementAt(i).toString();
				}

				if (strFiles.length() > 0)
					out.println("<File>" + strFiles + "</File>");

				if (strFolders.length() > 0)
					out.println("<Folder>" + strFolders + "</Folder>");
			}
        }
    }
%>