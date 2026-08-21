<%@ Page Language="VB" Debug="true" %>
<%@ Import Namespace="ChartDirector" %>
<%@ Register TagPrefix="chart" Namespace="ChartDirector" Assembly="netchartdir" %>
<script runat="server">

'
' Page Load event handler
'
Protected Sub Page_Load(ByVal sender As System.Object, ByVal e As System.EventArgs)

    ' The data for the line chart
    Dim data0() As Double = {42, 49, 33, 38, 64, 56, 29, 41, 44, 57, 59, 42}
    Dim data1() As Double = {65, 75, 47, 34, 42, 49, 73, 62, 90, 69, 66, 78}
    Dim data2() As Double = {36, 28, 25, 28, 38, 20, 22, 30, 25, 33, 30, 24}
    Dim labels() As String = {"Jan", "Feb", "Mar", "Apr", "May", "Jun", "Jul", _
        "Aug", "Sep", "Oct", "Nov", "Dec"}

    ' Create a XYChart object of size 600 x 400 pixels
    Dim c As XYChart = New XYChart(600, 400)

    ' Add a title to the chart using 18 pts Times Bold Italic font
    Dim title As ChartDirector.TextBox = c.addTitle("Product Line Global Revenue", _
        "Times New Roman Bold Italic", 18)

    ' Tentatively set the plotarea at (50, 55) and of (chart_width - 100) x
    ' (chart_height - 120) pixels in size. Use a vertical gradient color from sky
    ' blue (aaccff) t0 light blue (f9f9ff) as background. Set both horizontal and
    ' vertical grid lines to dotted semi-transprent black (aa000000).
    Dim plotArea As PlotArea = c.setPlotArea(50, 55, c.getWidth() - 100, _
        c.getHeight() - 120, c.linearGradientColor(0, 55, 0, 55 + c.getHeight() - _
        120, &Haaccff, &Hf9fcff), -1, -1, c.dashLineColor(&Haa000000, Chart.DotLine _
        ), -1)

    ' Add a legend box and anchored the top center at the horizontal center of the
    ' chart, just under the title. Use 10pts Arial Bold as font, with transparent
    ' background and border.
    Dim legendBox As LegendBox = c.addLegend(c.getWidth() / 2, title.getHeight(), _
        False, "Arial Bold", 10)
    legendBox.setAlignment(Chart.TopCenter)
    legendBox.setBackground(Chart.Transparent, Chart.Transparent)

    ' Set y-axis title using 10 points Arial Bold Italic font, label style to 8
    ' points Arial Bold, and axis color to transparent
    c.yAxis().setTitle("Revenue (USD millions)", "Arial Bold Italic", 10)
    c.yAxis().setLabelStyle("Arial Bold", 8)
    c.yAxis().setColors(Chart.Transparent)

    ' Set y-axis tick density to 30 pixels. ChartDirector auto-scaling will use this
    ' as the guideline when putting ticks on the y-axis.
    c.yAxis().setTickDensity(30)

    ' Add a line layer to the chart
    Dim layer As LineLayer = c.addLineLayer2()

    ' Set the line width to 3 pixels
    layer.setLineWidth(3)

    ' Add the three data sets to the line layer, using circles, diamands and X shapes
    ' as symbols
    layer.addDataSet(data0, &Hff0000, "Quantum Computer").setDataSymbol( _
        Chart.CircleSymbol, 9)
    layer.addDataSet(data1, &H00ff00, "Atom Synthesizer").setDataSymbol( _
        Chart.DiamondSymbol, 11)
    layer.addDataSet(data2, &Hff6600, "Proton Cannon").setDataSymbol( _
        Chart.Cross2Shape(), 11)

    ' Set the x axis labels
    c.xAxis().setLabels(labels)

    ' Convert the labels on the x-axis to a CDMLTable
    Dim table As CDMLTable = c.xAxis().makeLabelTable()

    ' Set the default top/bottom margins of the cells to 3 pixels
    table.getStyle().setMargin2(0, 0, 3, 3)

    ' Use Arial Bold as the font for the first row
    table.getRowStyle(0).setFontStyle("Arial Bold")

    '
    ' We can add more information to the table. In this sample code, we add the data
    ' series and the legend icons to the table.
    '

    ' Add 3 more rows to the table. Set the background of the 1st and 3rd rows to
    ' light grey (eeeeee).
    table.appendRow().setBackground(&Heeeeee, Chart.LineColor)
    table.appendRow()
    table.appendRow().setBackground(&Heeeeee, Chart.LineColor)

    ' Put the values of the 3 data series to the cells in the 3 rows
    For i As Integer = 0 To UBound(data0)
        table.setText(i, 1, CStr(data0(i)))
        table.setText(i, 2, CStr(data1(i)))
        table.setText(i, 3, CStr(data2(i)))
    Next

    ' Insert a column on the left for the legend icons. Use 5 pixels left/right
    ' margins and 3 pixels top/bottom margins for the cells in this column.
    table.insertCol(0).setMargin2(5, 5, 3, 3)

    ' The top cell is set to transparent, so it is invisible
    table.getCell(0, 0).setBackground(Chart.Transparent, Chart.Transparent)

    ' The other 3 cells are set to the legend icons of the 3 data series
    table.setText(0, 1, layer.getLegendIcon(0))
    table.setText(0, 2, layer.getLegendIcon(1))
    table.setText(0, 3, layer.getLegendIcon(2))

    ' Layout legend box first, so we can get its size
    c.layoutLegend()

    ' Adjust the plot area size, such that the bounding box (inclusive of axes) is 2
    ' pixels from the left, right and bottom edge, and is just under the legend box.
    c.packPlotArea(2, legendBox.getTopY() + legendBox.getHeight(), c.getWidth() - _
        3, c.getHeight() - 3)

    ' After determining the exact plot area position, we may adjust the legend box
    ' and the title positions so that they are centered relative to the plot area
    ' (instead of the chart)
    legendBox.setPos(plotArea.getLeftX() + (plotArea.getWidth() - _
        legendBox.getWidth()) / 2, legendBox.getTopY())
    title.setPos(plotArea.getLeftX() + (plotArea.getWidth() - title.getWidth()) / _
        2, title.getTopY())

    ' Output the chart
    WebChartViewer1.Image = c.makeWebImage(Chart.PNG)

    ' Include tool tip for the chart
    WebChartViewer1.ImageMap = c.getHTMLImageMap("", "", _
        "title='Revenue of {dataSetName} in {xLabel}: US$ {value}M'")

End Sub

</script>
<html xmlns="http://www.w3.org/1999/xhtml">
<head>
    <title>Data Table (1)</title>
</head>
<body style="margin:5px 0px 0px 5px">
    <div style="font-size:18pt; font-family:verdana; font-weight:bold">
        Data Table (1)
    </div>
    <hr style="border:solid 1px #000080" />
    <div style="font-size:10pt; font-family:verdana; margin-bottom:1.5em">
        <a href='viewsource.aspx?file=<%=Request("SCRIPT_NAME")%>'>View Source Code</a>
    </div>
    <chart:WebChartViewer id="WebChartViewer1" runat="server" />
</body>
</html>

