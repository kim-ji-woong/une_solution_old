<%@ Page Language="VB" Debug="true" %>
<%@ Import Namespace="ChartDirector" %>
<%@ Register TagPrefix="chart" Namespace="ChartDirector" Assembly="netchartdir" %>
<script runat="server">

'
' Create chart
'
Protected Sub createChart(viewer As WebChartViewer, img As String)

    ' Determine the starting angle and direction based on input parameter
    Dim angle As Integer = 0
    Dim clockwise As Boolean = True
    If img <> "0" Then
        angle = 90
        clockwise = False
    End If

    ' The data for the pie chart
    Dim data() As Double = {25, 18, 15, 12, 8, 30, 35}

    ' The labels for the pie chart
    Dim labels() As String = {"Labor", "Licenses", "Taxes", "Legal", "Insurance", _
        "Facilities", "Production"}

    ' Create a PieChart object of size 280 x 240 pixels
    Dim c As PieChart = New PieChart(280, 240)

    ' Set the center of the pie at (140, 130) and the radius to 80 pixels
    c.setPieSize(140, 130, 80)

    ' Add a title to the pie to show the start angle and direction
    If clockwise Then
        c.addTitle("Start Angle = " & angle & " degrees<*br*>Direction = Clockwise")
    Else
        c.addTitle("Start Angle = " & angle & _
            " degrees<*br*>Direction = AntiClockwise")
    End If

    ' Set the pie start angle and direction
    c.setStartAngle(angle, clockwise)

    ' Draw the pie in 3D
    c.set3D()

    ' Set the pie data and the pie labels
    c.setData(data, labels)

    ' Explode the 1st sector (index = 0)
    c.setExplode(0)

    ' Output the chart
    viewer.Image = c.makeWebImage(Chart.PNG)

    ' Include tool tip for the chart
    viewer.ImageMap = c.getHTMLImageMap("", "", _
        "title='{label}: US${value}K ({percent}%)'")

End Sub

'
' Page Load event handler
'
Protected Sub Page_Load(ByVal sender As System.Object, ByVal e As System.EventArgs)

    createChart(WebChartViewer0, "0")
    createChart(WebChartViewer1, "1")

End Sub

</script>
<html xmlns="http://www.w3.org/1999/xhtml">
<head>
    <title>Start Angle and Direction</title>
</head>
<body style="margin:5px 0px 0px 5px">
    <div style="font-size:18pt; font-family:verdana; font-weight:bold">
        Start Angle and Direction
    </div>
    <hr style="border:solid 1px #000080" />
    <div style="font-size:10pt; font-family:verdana; margin-bottom:1.5em">
        <a href='viewsource.aspx?file=<%=Request("SCRIPT_NAME")%>'>View Source Code</a>
    </div>
    <chart:WebChartViewer id="WebChartViewer0" runat="server" />
    <chart:WebChartViewer id="WebChartViewer1" runat="server" />
</body>
</html>

