<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="test.aspx.cs" Inherits="TrainingSystem.test" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
    <script runat="server">

    void Page_Load(Object sender, EventArgs e)
    {
        // Manually register the event-handling method for
        // the Click event of the Button control.
        Button1.Click += new EventHandler(this.GreetingBtn_Click);
    }
    
    void GreetingBtn_Click(Object sender,
                           EventArgs e)
    {
        runApp();
    } 
    </script>
<head runat="server">
<meta http-equiv="Content-Type" content="text/html; charset=utf-8"/>
    <title></title>
</head>
    <!-- -->
<%@ Import NameSpace="System.Data.SqlClient"   %>
<%@ Import Namespace="System.Diagnostics" %>
<body>
    <form id="form1" runat="server">        
    <div>
        <asp:Button runat="server" Text="Click me" ID="Button1"/>
    </div>
    </form>
</body>
        <script runat="server">        
        void runApp()
        {
    ProcessStartInfo startinfo = new ProcessStartInfo();
    startinfo.FileName = @"C:\Users\김영\Documents\Visual Studio 2013\Projects\TrainingSystem\TrainingSystem\bin\AspNetCaller.exe";            
    startinfo.WorkingDirectory = System.IO.Path.GetDirectoryName(startinfo.FileName);    
    Process myProcess = Process.Start(startinfo);
    //myProcess.Start();
        }
    </script>
</html>
