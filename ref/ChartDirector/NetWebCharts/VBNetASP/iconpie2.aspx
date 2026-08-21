<%@ Page Language="VB" Debug="true" %>
<%@ Import Namespace="ChartDirector" %>
<%@ Register TagPrefix="chart" Namespace="ChartDirector" Assembly="netchartdir" %>
<script runat="server">

'
' Page Load event handler
'
Protected Sub Page_Load(ByVal sender As System.Object, ByVal e As System.EventArgs)

    ' The data for the pie chart
    Dim data() As Double = {28, 45, 5, 1, 12}

    ' The labels for the pie chart
    Dim labels() As String = {"Excellent", "Good", "Bad", "Very Bad", "Neutral"}

    ' The icons for the sectors
    Dim icons() As String = {"laugh.png", "smile.png", "sad.png", "angry.png", _
        "nocomment.png"}

    ' Create a PieChart object of size 560 x 300 pixels, with a silver background,
    ' black border, 1 pxiel 3D border effect and rounded corners
    Dim c As PieChart = New PieChart(560, 300, Chart.silverColor(), &H000000, 1)
    c.setRoundedFrame()

    ' Set default directory for loading images from current script directory
    Call c.setSearchPath(Server.MapPath("."))

    ' Set the center of the pie at (280, 150) and the radius to 120 pixels
    c.setPieSize(280, 150, 120)

    ' Add a title box with title written in CDML, on a sky blue (A0C8FF) background
    ' with glass effect
    c.addTitle( _
        "<*block,valign=absmiddle*><*img=doc.png*> Customer Survey: " & _
        "<*font=Times New Roman Italic,color=000000*>Do you like our " & _
        "<*font,color=dd0000*>Hyper<*super*>TM<*/font*> molecules?", _
        "Times New Roman Bold Italic", 15, &H000080).setBackground(&Ha0c8ff, _
        &H000000, Chart.glassEffect())

    ' Add a logo to the chart written in CDML as the bottom title aligned to the
    ' bottom right
    c.addTitle2(Chart.BottomRight, _
        "<*block,valign=absmiddle*><*img=molecule.png*> <*block*><*color=FF*>" & _
        "<*font=Times New Roman Bold Italic,size=12*>Molecular Engineering<*br*>" & _
        "<*font=Arial,size=10*>Creating better molecules")

    ' Set the pie data and the pie labels
    c.setData(data, labels)

    ' Set 3D style
    c.set3D()

    ' Use the side label layout method
    c.setLabelLayout(Chart.SideLayout)

    ' Set the label background color to transparent
    c.setLabelStyle().setBackground(Chart.Transparent)

    ' Add icons to the chart as a custom field
    c.addExtraField(icons)

    ' Configure the sector labels using CDML to include the icon images
    c.setLabelFormat( _
        "<*block,valign=absmiddle*><*img={field0}*> {label} ({percent|0}%)")

    ' Explode the 3rd and 4th sectors as a group (index = 2 and 3)
    c.setExplodeGroup(2, 3)

    ' Set the start angle to 135 degrees may improve layout when there are many small
    ' sectors at the end of the data array (that is, data sorted in descending
    ' order). It is because this makes the small sectors position near the horizontal
    ' axis, where the text label has the least tendency to overlap. For data sorted
    ' in ascending order, a start angle of 45 degrees can be used instead.
    c.setStartAngle(135)

    ' Output the chart
    WebChartViewer1.Image = c.makeWebImage(Chart.PNG)

    ' Include tool tip for the chart
    WebChartViewer1.ImageMap = c.getHTMLImageMap("", "", _
        "title='{label}: {value} responses ({percent}%)'")

End Sub

</script>
<html xmlns="http://www.w3.org/1999/xhtml">
<head>
    <title>Icon Pie Chart (2)</title>
</head>
<body style="margin:5px 0px 0px 5px">
    <div style="font-size:18pt; font-family:verdana; font-weight:bold">
        Icon Pie Chart (2)
    </div>
    <hr style="border:solid 1px #000080" />
    <div style="font-size:10pt; font-family:verdana; margin-bottom:1.5em">
        <a href='viewsource.aspx?file=<%=Request("SCRIPT_NAME")%>'>View Source Code</a>
    </div>
    <chart:WebChartViewer id="WebChartViewer1" runat="server" />
</body>
</html>

