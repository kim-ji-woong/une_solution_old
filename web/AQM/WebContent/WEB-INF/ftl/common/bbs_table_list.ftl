<#ftl encoding="utf-8">
<#if PageList ??>
<#assign nTotalCount = ( PageCount ) * 10 - ExtraCount + 1>
<#list PageList?keys as key >	
	<#assign nCount = 0>
	<#assign value = PageList[key] >
	<div id="tablelist">
	<table id="table_${key}">
		 <thead>
            <tr>
                <th width="150px" class="num">번호</th>
                <th>제목</th>
                <th width="50px">첨부</td>
                <th width="150px">작성일</th>
                <th width="150px">조회</th>
            </tr>
        </thead>
		<tbody>	
		    <#list value as items>		    		    
		    <#assign x = nCount % 2>
		    <#if items.postType == 1>
			    <#if x == 0>
			     	<tr class="bg_gray">
		     	<#else>
			     	<tr>
			    </#if>		      
	     		<#assign nCount = nCount + 1>
	     		<#assign nTotalCount = nTotalCount - 1>
		     	<td>${nTotalCount}</td>
		  	<#else>
		  	    <#assign nTotalCount = nTotalCount - 1>
			    <tr class="notice">
			     <td>공지</td>
		  	</#if>
                    <td><a href="${Context.contextPath}/${Target}/post/detail/${items.id}">${items.title}</a></td>
                    <#if items.hasFile == true>
                  	<td><img src="${Context.contextPath}/images/icon_file.png" width="18px" height="20px" /></td>
                  	<#else>
                  	<td></td>
                  	</#if>
                    <td class="date">${items.timeStamp?date?iso_utc}</td>
                    <td class="hits">${items.readCount}</td>					
				</tr>
			  
			</#list>		
		</tbody>
	</table> 
</div> 
</#list>
<div class="pagination">
    <ul>
        <li class="first">
            <a href=""><img src="${Context.contextPath}/images/btn_first.png" width="10" height="8" alt="첫페이지" /></a>
        </li>
        <li class="prev">
            <a href=""><img src="${Context.contextPath}/images/btn_prev.png" width="5" height="8" alt="이전페이지" /></a>
        </li>                            
        <li class="next">
            <a href=""><img src="${Context.contextPath}/images/btn_next.png" width="5" height="8" alt="다음페이지" /></a>
        </li>
        <li class="last">
            <a href=""><img src="${Context.contextPath}/images/btn_last.png" width="10" height="8" alt="마지막페이지" /></a>
        </li>
    </ul>
</div>
<!-- pagination -->
<#else>
<div id="tablelist">
	<table>
		 <thead>
            <tr>
                <th width="150px" class="num">번호</th>
                <th>제목</th>
                <th width="150px">작성일</th>
                <th width="150px">조회</th>
            </tr>
        </thead>
		<tbody>	
		</tbody>
	</table> 
</div> 
<div class="pagination">    
</div>
</#if>