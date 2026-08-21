<#ftl encoding="utf-8">
<#if LocationList??>
<script>
	$(function() {   
    <#list LocationList as item>
		contentString = '${item.cityName}'; 
		<#assign status = item.status>
		<#if status == 0>			    	
			addMapMarker(map,'gray',new google.maps.LatLng(${item.locX}, ${item.locY}), contentString);
		<#elseif status gt 90>			    	
			addMapMarker(map,'sky',new google.maps.LatLng(${item.locX}, ${item.locY}), contentString);
		<#elseif status gt 80>			    	
			addMapMarker(map,'green',new google.maps.LatLng(${item.locX}, ${item.locY}), contentString);	
		<#elseif status gt 70>    	
			addMapMarker(map,'yellow',new google.maps.LatLng(${item.locX}, ${item.locY}), contentString);			
		<#elseif  status gt 0>			    	
			addMapMarker(map,'red',new google.maps.LatLng(${item.locX}, ${item.locY}), contentString);		
		</#if>
	</#list>    
    });    
</script>
</#if>