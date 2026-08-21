<%@ Page Language="VB" Debug="true" %>
<%@ Import Namespace="ChartDirector" %>
<%@ Register TagPrefix="chart" Namespace="ChartDirector" Assembly="netchartdir" %>
<script runat="server">

'
' Page Load event handler
'
Protected Sub Page_Load(ByVal sender As System.Object, ByVal e As System.EventArgs)

    ' The data for the chart
    Dim data0() As Double = {32, 39, 23, 28, 41, 38, 26, 35, 29}
    Dim data1() As Double = {50, 55, 47, 34, 47, 53, 38, 40, 51}

    ' The labels for the chart
    Dim labels() As String = {"0", "1", "2", "3", "4", "5", "6", "7", "8"}

    ' Create a XYChart object of size 600 x 300 pixels, with a pale red (ffdddd)
    ' background, black border, 1 pixel 3D border effect and rounded corners.
    Dim c As XYChart = New XYChart(600, 300, &Hffdddd, &H000000, 1)
    c.setRoundedFrame()

    ' Set default directory for loading images from current script directory
    Call c.setSearchPath(Server.MapPath("."))

    ' Set the plotarea at (55, 58) and of size 520 x 195 pixels, with white (ffffff)
    ' background. Set horizontal and vertical grid lines to grey (cccccc).
    c.setPlotArea(55, 58, 520, 195, &Hffffff, -1, -1, &Hcccccc, &Hcccccc)

    ' Add a legend box at (55, 32) (top of the chart) with horizontal layout. Use 9
    ' pts Arial Bold font. Set the background and border color to Transparent.
    c.addLegend(55, 32, False, "Arial Bold", 9).setBackground(Chart.Transparent)

    ' Add a title box to the chart using 15 pts Times Bold Italic font. The title is
    ' in CDML and includes embedded images for highlight. The text is white (ffffff)
    ' on a dark red (880000) background, with soft lighting effect from the right
    ' side.
    c.addTitle( _
        "<*block,valign=absmiddle*><*img=star.png*><*img=star.png*> Performance " & _
        "Enhancer <*img=star.png*><*img=star.png*><*/*>", _
        "Times New Roman Bold Italic", 15, &Hffffff).setBackground(&H880000, -1, _
        Chart.softLighting(Chart.Right))

    ' Add a title to the y axis
    c.yAxis().setTitle("Energy Concentration (KJ per liter)")

    ' Set the labels on the x axis
    c.xAxis().setLabels(labels)

    ' Add a title to the x axis using CMDL
    c.xAxis().setTitle( _
        "<*block,valign=absmiddle*><*img=clock.png*>  Elapsed Time (hour)<*/*>")

    ' Set the axes width to 2 pixels
    c.xAxis().setWidth(2)
    c.yAxis().setWidth(2)

    ' Add a spline layer to the chart
    Dim layer As SplineLayer = c.addSplineLayer()

    ' Set the default line width to 2 pixels
    layer.setLineWidth(2)

    ' Add a data set to the spline layer, using blue (0000c0) as the line color, with
    ' yellow (ffff00) circle symbols.
    layer.addDataSet(data1, &H0000c0, "Target Group").setDataSymbol( _
        Chart.CircleSymbol, 9, &Hffff00)

    ' Add a data set to the spline layer, using brown (982810) as the line color,
    ' with pink (f040f0) diamond symbols.
    layer.addDataSet(data0, &H982810, "Control Group").setDataSymbol( _
        Chart.DiamondSymbol, 11, &Hf040f0)

    ' Add a custom CDML text at the bottom right of the plot area as the logo
    c.addText(575, 250, _
        "<*block,valign=absmiddle*><*img=small_molecule.png*> <*block*>" & _
        "<*font=Times New Roman Bold Italic,size=10,color=804040*>Molecular" & _
        "<*br*>Engineering<*/*>").setAlignment(Chart.BottomRight)

    ' Output the chart
    WebChartViewer1.Image = c.makeWebImage(Chart.PNG)

    ' Include tool tip for the chart
    WebChartViewer1.ImageMap = c.getHTMLImageMap("", "", _
        "title='{dataSetName} at t = {xLabel} hour: {value} KJ/liter'")

End Sub

</script>
<html xmlns="http://www.w3.org/1999/xhtml">
<head>
    <title>Spline Line Chart</title>
</head>
<body style="margin:5px 0px 0px 5px">
    <div style="font-size:18pt; font-family:verdana; font-weight:bold">
        Spline Line Chart
    </div>
    <hr style="border:solid 1px #000080" />
    <div style="font-size:10pt; font-family:verdana; margin-bottom:1.5em">
        <a href='viewsource.aspx?file=<%=Request("SCRIPT_NAME")%>'>View Source Code</a>
    </div>
    <chart:WebChartViewer id="WebChartViewer1" runat="server" />
</body>
</html>

