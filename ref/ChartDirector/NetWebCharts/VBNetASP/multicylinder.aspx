<%@ Page Language="VB" Debug="true" %>
<%@ Import Namespace="ChartDirector" %>
<%@ Register TagPrefix="chart" Namespace="ChartDirector" Assembly="netchartdir" %>
<script runat="server">

'
' Page Load event handler
'
Protected Sub Page_Load(ByVal sender As System.Object, ByVal e As System.EventArgs)

    ' Data for the chart
    Dim data0() As Double = {100, 125, 245, 147, 67, 96, 160, 145, 97, 167, 220, _
        125}
    Dim data1() As Double = {85, 156, 179, 211, 123, 225, 127, 99, 111, 260, 175, _
        156}
    Dim data2() As Double = {97, 87, 56, 267, 157, 157, 67, 156, 77, 87, 197, 87}
    Dim labels() As String = {"Jan", "Feb", "Mar", "Apr", "May", "Jun", "Jul", _
        "Aug", "Sept", "Oct", "Nov", "Dec"}

    ' Create a XYChart object of size 560 x 280 pixels.
    Dim c As XYChart = New XYChart(560, 280)

    ' Add a title to the chart using 14 pts Arial Bold Italic font
    c.addTitle("     Average Weekly Network Load", "Arial Bold Italic", 14)

    ' Set the plotarea at (50, 50) and of 500 x 200 pixels in size. Use alternating
    ' light grey (f8f8f8) / white (ffffff) background. Set border to transparent and
    ' use grey (CCCCCC) dotted lines as horizontal and vertical grid lines
    c.setPlotArea(50, 50, 500, 200, &Hffffff, &Hf8f8f8, Chart.Transparent, _
        c.dashLineColor(&Hcccccc, Chart.DotLine), c.dashLineColor(&Hcccccc, _
        Chart.DotLine))

    ' Add a legend box at (50, 22) using horizontal layout. Use 10 pt Arial Bold
    ' Italic font, with transparent background
    c.addLegend(50, 22, False, "Arial Bold Italic", 10).setBackground( _
        Chart.Transparent)

    ' Set the x axis labels
    c.xAxis().setLabels(labels)

    ' Draw the ticks between label positions (instead of at label positions)
    c.xAxis().setTickOffset(0.5)

    ' Add axis title
    c.yAxis().setTitle("Throughput (MBytes Per Hour)")

    ' Set axis line width to 2 pixels
    c.xAxis().setWidth(2)
    c.yAxis().setWidth(2)

    ' Add a multi-bar layer with 3 data sets
    Dim layer As BarLayer = c.addBarLayer2(Chart.Side)
    layer.addDataSet(data0, &Hff0000, "Server #1")
    layer.addDataSet(data1, &H00ff00, "Server #2")
    layer.addDataSet(data2, &H0000ff, "Server #3")

    ' Set bar shape to circular (cylinder)
    layer.setBarShape(Chart.CircleShape)

    ' Configure the bars within a group to touch each others (no gap)
    layer.setBarGap(0.2, Chart.TouchBar)

    ' Output the chart
    WebChartViewer1.Image = c.makeWebImage(Chart.PNG)

    ' Include tool tip for the chart
    WebChartViewer1.ImageMap = c.getHTMLImageMap("", "", _
        "title='{dataSetName} on {xLabel}: {value} MBytes/hour'")

End Sub

</script>
<html xmlns="http://www.w3.org/1999/xhtml">
<head>
    <title>Multi-Cylinder Chart</title>
</head>
<body style="margin:5px 0px 0px 5px">
    <div style="font-size:18pt; font-family:verdana; font-weight:bold">
        Multi-Cylinder Chart
    </div>
    <hr style="border:solid 1px #000080" />
    <div style="font-size:10pt; font-family:verdana; margin-bottom:1.5em">
        <a href='viewsource.aspx?file=<%=Request("SCRIPT_NAME")%>'>View Source Code</a>
    </div>
    <chart:WebChartViewer id="WebChartViewer1" runat="server" />
</body>
</html>

