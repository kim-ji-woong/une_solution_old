<#ftl encoding="utf-8">
<#if SensorDataList??>
<#list SensorDataList as value>
	<#assign nItem = 0>
	<#list value as item>

		<#assign classname = "progress_on progress_disable">
		<#if item.qualityGrade == 1>
			<#assign classname = "progress_on progress_good">
		</#if>
		<#if item.qualityGrade == 2>
			<#assign classname = "progress_on progress_normal">
		</#if>
		<#if item.qualityGrade == 3>
			<#assign classname = "progress_on progress_warning">
		</#if>
		<#if item.qualityGrade == 4>
			<#assign classname = "progress_on progress_danger">
		</#if>		
 		<#if nItem == 0> 			
<div id="SensorTimeData">
	<div class="pollutant">
    	<h3>오염물질 측정 정보 </h3>
	    <div class="caption"><small>유지기준</small>
	        <span class="normal">보통</span>
	        <span class="warning">주의</span>
	        <span class="danger">나쁨</span>
	    </div>
	    <div class="item" id="radon">
	        <h4>라돈</h4>
	        <#if item.sensorValue gt 0.0 >
	        <div class="value" id="radonValue">${item.sensorValue} pCi/l</div>
	        <#else>
	        <div class="value" id="radonValue"> - </div>
	        </#if>
	        <div class="progress" id="radonGraph">	        	
	            <div class="${classname}" style="width:${item.percentValue}%" id="radonProgress"></div>
	        </div>
	    </div>
	    </#if>
	    <#if nItem == 1>
	    <div class="item" id="nitrogendioxide">
	        <h4>산소</h4>
	        <#if item.sensorValue gt 0.0 >
	        <div class="value" id="nitrogendioxideValue">${item.sensorValue} %</div>
	        <#else>
	        <div class="value" id="nitrogendioxideValue"> - </div>
	        </#if>
	        <div class="progress" id="nitrogendioxideGraph">
	            <div class="${classname}" style="width:${item.percentValue}%" id="nitrogendioxideProgress"></div>
	        </div>
	    </div>
	    </#if>
	    <#if nItem == 2>
	    <div class="item" id="formaldehyde">
	        <h4>홈알데하이드</h4>
	        <#if item.sensorValue gt 0.0 >
	        <div class="value" id="formaldehydeValue">${item.sensorValue} ppb</div>
	        <#else>
	        <div class="value" id="formaldehydeValue"> - </div>
	        </#if>
	        <div class="progress" id="formaldehydeGraph">
	            <div class="${classname}" style="width:${item.percentValue}%" id="formaldehydeProgress"></div>
	        </div>
	    </div>
	    </#if>
	    <#if nItem == 3>
	    <div class="item" id="carbonmonoxide">
	        <h4>TVOC</h4>
	        <#if item.sensorValue gt 0>
	        <div class="value" id="carbonmonoxideValue">${item.sensorValue} ppb</div>
	        <#else>
	        <div class="value" id="carbonmonoxideValue"> - </div>
	        </#if>	      
	        <div class="progress" id="carbonmonoxideGraph">
	            <div class="${classname}" style="width:${item.percentValue}%" id="carbonmonoxideProgress"></div>
	        </div>
	    </div>
    	</#if>
    	<#if nItem == 4>
	    <div class="item" id="carbondioxide">
	        <h4>이산화탄소</h4>
	        <#if item.sensorValue gt 0>
	        <div class="value" id="carbondioxideValue">${item.sensorValue} ppm</div>
	        <#else>
	        <div class="value" id="carbondioxideValue"> - </div>
	        </#if>
	        <div class="progress" id="carbondioxideGraph">
	            <div class="${classname}" style="width:${item.percentValue}%" id="carbondioxideProgress"></div>
	        </div>
	    </div>
	    </#if>
	    <#if nItem == 5>
	    <div class="item" id="microdust">
	        <h4>미세먼지</h4>
	        <#if item.sensorValue gt 0>
	        <div class="value" id="microdustValue">${item.sensorValue} ug/m3</div>
	        <#else>
	        <div class="value" id="microdustValue"> - </div>
	        </#if>
	        <div class="progress" id="microdustGraph">
	            <div class="${classname}" style="width:${item.percentValue}%" id="microdustProgress"></div>
	        </div>
	    </div>
    </div><!-- pollutant -->
</div>
	    </#if>
    	<#assign nItem = nItem + 1>
	</#list>
	</#list>

</#if>