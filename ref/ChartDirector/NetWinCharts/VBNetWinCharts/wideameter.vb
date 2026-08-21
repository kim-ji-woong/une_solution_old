Imports System
Imports Microsoft.VisualBasic
Imports ChartDirector

Public Class wideameter
    Implements DemoModule

    'Name of demo module
    Public Function getName() As String Implements DemoModule.getName
        Return "Wide Angular Meters"
    End Function

    'Number of charts produced in this demo module
    Public Function getNoOfCharts() As Integer Implements DemoModule.getNoOfCharts
        Return 6
    End Function

    'Main code for creating charts
    Public Sub createChart(viewer As WinChartViewer, img As String) _
        Implements DemoModule.createChart

        ' The value to display on the meter
        Dim value As Double = 6.5

        ' Create an AugularMeter object of size 200 x 100 pixels with rounded corners
        Dim m As AngularMeter = New AngularMeter(200, 100)
        m.setRoundedFrame()

        ' Set meter background according to a parameter
        If img = "0" Then
            ' Use gold background color
            m.setBackground(Chart.goldColor(), &H000000, -2)
        ElseIf img = "1" Then
            ' Use silver background color
            m.setBackground(Chart.silverColor(), &H000000, -2)
        ElseIf img = "2" Then
            ' Use metallic blue (9898E0) background color
            m.setBackground(Chart.metalColor(&H9898e0), &H000000, -2)
        ElseIf img = "3" Then
            ' Use a wood pattern as background color
            m.setBackground(m.patternColor2("wood.png"), &H000000, -2)
        ElseIf img = "4" Then
            ' Use a marble pattern as background color
            m.setBackground(m.patternColor2("marble.png"), &H000000, -2)
        Else
            ' Use a solid light purple (EEBBEE) background color
            m.setBackground(&Heebbee, &H000000, -2)
        End If

        ' Set the meter center at (100, 235), with radius 210 pixels, and span from
        ' -24 to +24 degress
        m.setMeter(100, 235, 210, -24, 24)

        ' Meter scale is 0 - 100, with a tick every 1 unit
        m.setScale(0, 10, 1)

        ' Set 0 - 6 as green (99ff99) zone, 6 - 8 as yellow (ffff00) zone, and 8 - 10
        ' as red (ff3333) zone
        m.addZone(0, 6, &H99ff99, &H808080)
        m.addZone(6, 8, &Hffff00, &H808080)
        m.addZone(8, 10, &Hff3333, &H808080)

        ' Add a title at the bottom of the meter using 10 pts Arial Bold font
        m.addTitle2(Chart.Bottom, "OUTPUT POWER LEVEL<*br*>", "Arial Bold", 10)

        ' Add a semi-transparent black (80000000) pointer at the specified value
        m.addPointer(value, &H80000000)

        ' Output the chart
        viewer.Chart = m

    End Sub

End Class

