Imports System
Imports Microsoft.VisualBasic
Imports ChartDirector

Public Class builtinsymbols
    Implements DemoModule

    'Name of demo module
    Public Function getName() As String Implements DemoModule.getName
        Return "Built-in Symbols"
    End Function

    'Number of charts produced in this demo module
    Public Function getNoOfCharts() As Integer Implements DemoModule.getNoOfCharts
        Return 1
    End Function

    'Main code for creating chart.
    'Note: the argument img is unused because this demo only has 1 chart.
    Public Sub createChart(viewer As WinChartViewer, img As String) _
        Implements DemoModule.createChart

        ' Some ChartDirector built-in symbols
        Dim symbols() As Integer = {Chart.CircleShape, Chart.GlassSphereShape, _
            Chart.GlassSphere2Shape, Chart.SolidSphereShape, Chart.SquareShape, _
            Chart.DiamondShape, Chart.TriangleShape, Chart.RightTriangleShape, _
            Chart.LeftTriangleShape, Chart.InvertedTriangleShape, Chart.StarShape(3 _
            ), Chart.StarShape(4), Chart.StarShape(5), Chart.StarShape(6), _
            Chart.StarShape(7), Chart.StarShape(8), Chart.StarShape(9), _
            Chart.StarShape(10), Chart.PolygonShape(5), Chart.Polygon2Shape(5), _
            Chart.PolygonShape(6), Chart.Polygon2Shape(6), Chart.CrossShape(0.1), _
            Chart.CrossShape(0.2), Chart.CrossShape(0.3), Chart.CrossShape(0.4), _
            Chart.CrossShape(0.5), Chart.CrossShape(0.6), Chart.CrossShape(0.7), _
            Chart.Cross2Shape(0.1), Chart.Cross2Shape(0.2), Chart.Cross2Shape(0.3), _
            Chart.Cross2Shape(0.4), Chart.Cross2Shape(0.5), Chart.Cross2Shape(0.6), _
            Chart.Cross2Shape(0.7)}

        ' Create a XYChart object of size 450 x 400 pixels
        Dim c As XYChart = New XYChart(450, 400)

        ' Set the plotarea at (55, 40) and of size 350 x 300 pixels, with a light
        ' grey border (0xc0c0c0). Turn on both horizontal and vertical grid lines
        ' with light grey color (0xc0c0c0)
        c.setPlotArea(55, 40, 350, 300, -1, -1, &Hc0c0c0, &Hc0c0c0, -1)

        ' Add a title to the chart using 18 pts Times Bold Itatic font.
        c.addTitle("Built-in Symbols", "Times New Roman Bold Italic", 18)

        ' Set the axes line width to 3 pixels
        c.xAxis().setWidth(3)
        c.yAxis().setWidth(3)

        ' Ensure the ticks are at least 1 unit part (integer ticks)
        c.xAxis().setMinTickInc(1)
        c.yAxis().setMinTickInc(1)

        ' Add each symbol as a separate scatter layer.
        For i As Integer = 0 To UBound(symbols)
            c.addScatterLayer(New Double() {i Mod 6 + 1}, New Double() {Int(i / 6 + _
                1)}, "", symbols(i), 15)
        Next

        ' Output the chart
        viewer.Chart = c

        'include tool tip for the chart
        viewer.ImageMap = c.getHTMLImageMap("clickable", "", _
            "title='(x, y) = ({x}, {value})'")

    End Sub

End Class

