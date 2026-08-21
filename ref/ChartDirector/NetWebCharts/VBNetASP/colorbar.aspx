<%@ Page Language="VB" Debug="true" %>
<%@ Import Namespace="ChartDirector" %>
<%@ Register TagPrefix="chart" Namespace="ChartDirector" Assembly="netchartdir" %>
<script runat="server">

'
' Page Load event handler
'
Protected Sub Page_Load(ByVal sender As System.Object, ByVal e As System.EventArgs)

    ' The data for the bar chart
    Dim data() As Double = {85, 156, 179.5, 211, 123}

    ' The labels for the bar chart
    Dim labels() As String = {"Mon", "Tue", "Wed", "Thu", "Fri"}

    ' The colors for the bar chart
    Dim colors() As Integer = {&Hb8bc9c, &Ha0bdc4, &H999966, &H333366, &Hc3c3e6}

    ' Create a XYChart object of size 300 x 220 pixels. Use golden background color.
    ' Use a 2 pixel 3D border.
    Dim c As XYChart = New XYChart(300, 220, Chart.goldColor(), -1, 2)

    ' Add a title box using 10 point Arial Bold font. Set the background color to
    ' metallic blue (9999FF) Use a 1 pixel 3D border.
    c.addTitle("Daily Network Load", "Arial Bold", 10).setBackground( _
        Chart.metalColor(&H9999ff), -1, 1)

    ' Set the plotarea at (40, 40) and of 240 x 150 pixels in size
    c.setPlotArea(40, 40, 240, 150)

    ' Add a multi-color bar chart layer using the given data and colors. Use a 1
    ' pixel 3D border for the bars.
    c.addBarLayer3(data, colors).setBorderColor(-1, 1)

    ' Set the labels on the x axis.
    c.xAxis().setLabels(labels)

    ' Output the chart
    WebChartViewer1.Image = c.makeWebImage(Chart.PNG)

    ' Include tool tip for the chart
    WebChartViewer1.ImageMap = c.getHTMLImageMap("", "", _
        "title='{xLabel}: {value} GBytes'")

End Sub

</script>
<html xmlns="http://www.w3.org/1999/xhtml">
<head>
    <title>Multi-Color Bar Chart</title>
</head>
<body style="margin:5px 0px 0px 5px">
    <div style="font-size:18pt; font-family:verdana; font-weight:bold">
        Multi-Color Bar Chart
    </div>
    <hr style="border:solid 1px #000080" />
    <div style="font-size:10pt; font-family:verdana; margin-bottom:1.5em">
        <a href='viewsource.aspx?file=<%=Request("SCRIPT_NAME")%>'>View Source Code</a>
    </div>
    <chart:WebChartViewer id="WebChartViewer1" runat="server" />
</body>
</html>

