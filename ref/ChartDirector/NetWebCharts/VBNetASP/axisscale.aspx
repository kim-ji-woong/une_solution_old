<%@ Page Language="VB" Debug="true" %>
<%@ Import Namespace="ChartDirector" %>
<%@ Register TagPrefix="chart" Namespace="ChartDirector" Assembly="netchartdir" %>
<script runat="server">

'
' Create chart
'
Protected Sub createChart(viewer As WebChartViewer, img As String)

    ' The data for the chart
    Dim data() As Double = {5.5, 3.5, -3.7, 1.7, -1.4, 3.3}
    Dim labels() As String = {"Jan", "Feb", "Mar", "Apr", "May", "Jun"}

    ' Create a XYChart object of size 200 x 190 pixels
    Dim c As XYChart = New XYChart(200, 190)

    ' Set the plot area at (30, 20) and of size 140 x 140 pixels
    c.setPlotArea(30, 20, 140, 140)

    ' Configure the axis as according to the input parameter
    If img = "0" Then
        c.addTitle("No Axis Extension", "Arial", 8)
    ElseIf img = "1" Then
        c.addTitle("Top/Bottom Extensions = 0/0", "Arial", 8)
        ' Reserve 20% margin at top of plot area when auto-scaling
        c.yAxis().setAutoScale(0, 0)
    ElseIf img = "2" Then
        c.addTitle("Top/Bottom Extensions = 0.2/0.2", "Arial", 8)
        ' Reserve 20% margin at top and bottom of plot area when auto-scaling
        c.yAxis().setAutoScale(0.2, 0.2)
    ElseIf img = "3" Then
        c.addTitle("Axis Top Margin = 15", "Arial", 8)
        ' Reserve 15 pixels at top of plot area
        c.yAxis().setMargin(15)
    Else
        c.addTitle("Manual Scale -5 to 10", "Arial", 8)
        ' Set the y axis to scale from -5 to 10, with ticks every 5 units
        c.yAxis().setLinearScale(-5, 10, 5)
    End If

    ' Set the labels on the x axis
    c.xAxis().setLabels(labels)

    ' Add a color bar layer using the given data. Use a 1 pixel 3D border for the
    ' bars.
    c.addBarLayer3(data).setBorderColor(-1, 1)

    ' Output the chart
    viewer.Image = c.makeWebImage(Chart.PNG)

    ' Include tool tip for the chart
    viewer.ImageMap = c.getHTMLImageMap("", "", _
        "title='ROI for {xLabel}: {value}%'")

End Sub

'
' Page Load event handler
'
Protected Sub Page_Load(ByVal sender As System.Object, ByVal e As System.EventArgs)

    createChart(WebChartViewer0, "0")
    createChart(WebChartViewer1, "1")
    createChart(WebChartViewer2, "2")
    createChart(WebChartViewer3, "3")
    createChart(WebChartViewer4, "4")

End Sub

</script>
<html xmlns="http://www.w3.org/1999/xhtml">
<head>
    <title>Y-Axis Scaling</title>
</head>
<body style="margin:5px 0px 0px 5px">
    <div style="font-size:18pt; font-family:verdana; font-weight:bold">
        Y-Axis Scaling
    </div>
    <hr style="border:solid 1px #000080" />
    <div style="font-size:10pt; font-family:verdana; margin-bottom:1.5em">
        <a href='viewsource.aspx?file=<%=Request("SCRIPT_NAME")%>'>View Source Code</a>
    </div>
    <chart:WebChartViewer id="WebChartViewer0" runat="server" />
    <chart:WebChartViewer id="WebChartViewer1" runat="server" />
    <chart:WebChartViewer id="WebChartViewer2" runat="server" />
    <chart:WebChartViewer id="WebChartViewer3" runat="server" />
    <chart:WebChartViewer id="WebChartViewer4" runat="server" />
</body>
</html>

