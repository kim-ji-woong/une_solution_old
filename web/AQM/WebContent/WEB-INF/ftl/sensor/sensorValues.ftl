<#ftl encoding="utf-8">
<#if SensorDataList??>
<div id="SensorTimeData">
	<div class="pollutant">
	<h3>오염물질 측정 정보 </h3>
    <div class="caption"><small>유지기준</small>
        <span class="normal">보통</span>
        <span class="warning">주의</span>
        <span class="danger">나쁨</span>
    </div>
 <#assign nItem = 0>
<#list SensorDataList as item >		
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
    <div class="item" id="radonRt">
        <h4>라돈</h4>
        <#if item.sensorValue gt 0.0 >
        <div class="value" id="radonValueRt">${item.sensorValue} pCi/l</div>
        <#else>
        <div class="value" id="radonValueRt"> - </div>
        </#if>
        <div class="progress" id="radonGraphRt">	        	
            <div class="${classname}" style="width:${item.percentValue}%" id="radonProgress"></div>
        </div>
    </div>
    </#if>
    <#if nItem == 1>
    <div class="item" id="nitrogendioxideRt">
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
    <div class="item" id="carbonmonoxideRt">
        <h4>TVOC</h4>
        <#if item.sensorValue gt 0>
        <div class="value" id="carbonmonoxideValueRt">${item.sensorValue} ppb</div>
        <#else>
        <div class="value" id="carbonmonoxideValueRt"> - </div>
        </#if>	      
        <div class="progress" id="carbonmonoxideGraphRt">
            <div class="${classname}" style="width:${item.percentValue}%" id="carbonmonoxideProgress"></div>
        </div>
    </div>
	</#if>
	<#if nItem == 4>
    <div class="item" id="carbondioxideRt">
        <h4>이산화탄소</h4>
        <#if item.sensorValue gt 0>
        <div class="value" id="carbondioxideValueRt">${item.sensorValue} ppm</div>
        <#else>
        <div class="value" id="carbondioxideValueRt"> - </div>
        </#if>
        <div class="progress" id="carbondioxideGraphRt">
            <div class="${classname}" style="width:${item.percentValue}%" id="carbondioxideProgress"></div>
        </div>
    </div>
    </#if>
    <#if nItem == 5>
    <div class="item" id="microdustRt">
        <h4>미세먼지</h4>
        <#if item.sensorValue gt 0>
        <div class="value" id="microdustValueRt">${item.sensorValue} ug/m3</div>
        <#else>
        <div class="value" id="microdustValueRt"> - </div>
        </#if>
        <div class="progress" id="microdustGraph">
            <div class="${classname}" style="width:${item.percentValue}%" id="microdustProgress"></div>
        </div>
    </div>
    </#if>
	<#assign nItem = nItem + 1>
	</#list>
    </div><!-- pollutant -->
</div>
</#if>