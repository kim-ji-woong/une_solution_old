<#ftl encoding="utf-8">
<script>
$("span.ui-dialog-title").text('${LocationName}');
</script> 
<div class="message ">
 <#if SensorValue??>
  <table  height="100%"> 
  <tr>  
  <th><table width="311" height="250">
    <tr height="20px;"></tr>
    <tr>
		<th colspan ="2" > 지역평균 실내상태 </th>
		<#if SensorValue.status == 0>
		<th><font color="#0099FF"> 없음</font> </th>
		<#elseif SensorValue.status gt 90>
		<th><font color="#0099FF"> 좋음</font> </th>
		<#elseif SensorValue.status gt 80>
		<th><font color="#33CC00"> 보통</font> </th>
		<#elseif SensorValue.status gt 70>
		<th><font color="#CEC90B"> 주의</font> </th>
		<#elseif SensorValue.status gt 0>
		<th><font color="#33CC00"> 나쁨</font> </th>
		</#if>
    </tr>
    <tr>
		<th>산소</th>
		<th><font color="#0099FF">${ SensorValue.value(0) }</font></th>
		<!-- <th><font color="#0099FF"> 좋음</font></th>-->
    </tr>
    <tr>
		<th> 이산화탄소 </th>
		<th><font color="#0099FF">${ SensorValue.value(1) }</font></th>
		<!-- <th><font color="#0099FF"> 좋음</font></th>-->
    </tr>
    <tr>
		<th>TVOC</th>
		<th><font color="#0099FF">${ SensorValue.value(2) }</font></th>
		<!-- <th><font color="#0099FF">좋음</font> </th>-->
    </tr>
    <tr>
		<th>미세먼지</th>
		<th><font color="#0099FF">${ SensorValue.value(3) }</font></th>
		<!-- <th><font color="#0099FF">좋음</font></th>-->
    </tr>
    <tr>
		<th>폼알데히드</th>
		<th><font color="#0099FF">${ SensorValue.value(4) }</font></th>
		<!-- <th><font color="#0099FF">좋음</font></th> -->
    </tr>
  	<tr>
		<th>라돈</th>
		<th><font color="#0099FF">${ SensorValue.value(5) }</font></th>
		<!-- <th><font color="#0099FF">좋음</font></th> -->
	</tr>
  </table></th>
  </tr>
  </table>
</#if>
</div>
