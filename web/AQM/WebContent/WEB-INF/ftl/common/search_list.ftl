<#ftl encoding="utf-8">
<#if LocationList ??>
<script>
	$(function() { 
	var id;
    <#list LocationList as location>
		id = '${location.ID?long?c}';
		contentString = '${location.name}';
     	addMapMarker(map,'yellow',new google.maps.LatLng(${location.locationX},${location.locationY}), id, contentString); 
	</#list>
    });
</script>
	<#if PageList ??>
	<#list PageList?keys as key >
		<#assign value = PageList[key] >
		<div id="tablelist">
		<table id="table_${key}">
			<thead>
				<tr>
					<th>학교명</th>
					<th>연락처</th>
					<th>주소</th>
					<th>지도보기</th>
				</tr>
			</thead>
			<tbody>		
				<#assign nCount = 0>
			    
			    <#list value as location>
			    <#assign x = nCount % 2>
			    <#if x == 0>
			     	<tr>
		     	<#else>
			     	<tr class="bg_gray">
			    </#if>			     	
			     	<#assign nCount = nCount + 1>
			     		<td class="clickable_td" align="center"><a href="#" onclick="viewDetail('${location.ID}','${location.name}');return false;">${location.name}</a></td>
			     		<td align=center>${location.phone}</td>
			     		<td align=center>${location.address} ${location.detailAddress}</td>
				     	<td class="btn_map_direct">
					     	<a href="#" onclick="moveMap(${location.locationX},${location.locationY});return false;">
					     	<img src="${Context.contextPath}/images/btn_map.png" width="21" height="25" alt="지도보기""/>
					     	</a>
				     	</td>
					</tr>
				</#list>		
			</tbody>
		</table> 
		</div> 
	</#list>
	</#if>
<#else>

   <div id="tablelist"> 
	<table>
	<thead>
		<tr>
			<th>학교명</th>
			<th>연락처</th>
			<th>주소</th>
			<th>지도보기</th>
		</tr>
	</thead>
	<tbody>
	</tbody>
	</table>
	</div> 
</#if>
<div id="pagination" class="pagination">				
	<ul>
	<#if LocationList??> 
	<li id="first" class="first"><a href="#"><img src="${Context.contextPath}/images/btn_first.png" width="10" height="8" alt="첫페이지" /></a></li>
	<li id="prev" class="prev"><a href="#"><img src="${Context.contextPath}/images/btn_prev.png" width="5" height="8" alt="이전페이지" /></a></li>
	<li id="next" class="next"><a href="#"><img src="${Context.contextPath}/images/btn_next.png" width="5" height="8" alt="다음페이지" /></a></li>
	<li id="last" class="last"><a href="#"><img src="${Context.contextPath}/images/btn_last.png" width="10" height="8" alt="마지막페이지" /></a></li>	
	</#if>
	</ul>
</div>
<!-- pagination -->