Imports System
Imports Microsoft.VisualBasic
Imports ChartDirector

Public Class anglepie
    Implements DemoModule

    'Name of demo module
    Public Function getName() As String Implements DemoModule.getName
        Return "Start Angle and Direction"
    End Function

    'Number of charts produced in this demo module
    Public Function getNoOfCharts() As Integer Implements DemoModule.getNoOfCharts
        Return 2
    End Function

    'Main code for creating charts
    Public Sub createChart(viewer As WinChartViewer, img As String) _
        Implements DemoModule.createChart

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
        Dim labels() As String = {"Labor", "Licenses", "Taxes", "Legal", _
            "Insurance", "Facilities", "Production"}

        ' Create a PieChart object of size 280 x 240 pixels
        Dim c As PieChart = New PieChart(280, 240)

        ' Set the center of the pie at (140, 130) and the radius to 80 pixels
        c.setPieSize(140, 130, 80)

        ' Add a title to the pie to show the start angle and direction
        If clockwise Then
            c.addTitle("Start Angle = " & angle & _
                " degrees<*br*>Direction = Clockwise")
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
        viewer.Chart = c

        'include tool tip for the chart
        viewer.ImageMap = c.getHTMLImageMap("clickable", "", _
            "title='{label}: US${value}K ({percent}%)'")

    End Sub

End Class

