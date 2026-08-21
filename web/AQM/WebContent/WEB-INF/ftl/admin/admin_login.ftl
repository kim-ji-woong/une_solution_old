<#ftl encoding="utf-8">
<!DOCTYPE html>
<html lang=en>

<head>
    <meta charset=utf-8>
    <meta content="IE=edge" http-equiv=X-UA-Compatible>
    <meta name="viewport" content="width=device-width, initial-scale=1, maximum-scale=1, user-scalable=no">
    <title>U&amp;E</title>
    <#include "../inc/include.ftl">
    
    <script>
	function check_num(form){
	    if(form.number.value == "")
	    {	    	
	    	$("#message").html("<p>비밀번호를 입력하세요</p>");
	        $("#popupNoResult").dialog('open');
	    }
	    else
	    {
	    	form.submit();
	    }
	    return false;
	}
	
	$(function() {
			var dialog;		
			dialog = $("#popupNoResult").dialog({
				autoOpen : false,
				height : 150,
				width : 323,
				modal : true,
				resizable: false,			
				dialogClass : 'no-close popup',
				open: function(){
					$('.ui-widget-overlay').bind('click', function() {
	                	$('#popupNoResult').dialog('close');
	            	});
				},			
				buttons : {				
				},
				close : function() {
				}
			});	
		});
	
	function closeNoResult(){
		$('#popupNoResult').dialog('close');
	}

	$(document).ready();
	</script>
</head>

<!--[if (lte ie 9) ]>      <body class="ie9">          <![endif]-->
<!--[if (gt IE 9) ]> <body >       <![endif]-->
    <div id="wrap">
    	<div class="popup" id="popupNoResult">           
            <div class="message empty">
                <div id="message"><p>일치하지 않는 등록 번호입니다.</p></div>
                <a href="#" onclick="closeNoResult();"><img src="${Context.contextPath}/images/btn_ok.png" alt="확인" width="122" height="26" /></a>
            </div>
        </div>
        <#include "../inc/logo.ftl">        
        <div class="container">
            <#include "../inc/gnb.ftl">
            <!-- gnb -->
            <div class="content">
                <div class="breadcrumbs breadcrumbs_no_margin">
                   홈 > <strong>관리자</strong>
                </div>
                <div class="well">
                    <div class="admin_num_check_box">
                        <form class="" action="${Context.contextPath}/Admin" method="POST" onsubmit="return check_num(this)">
                            <h2 class="title">관리자 등록 번호</h2>
                            <div class="list"><input type="password" name="number" /></div>
                            <div class="submit"><input type="image" src="${Context.contextPath}/images/btn_ok_2.png" /></div>
                        </form>
                    </div>
                </div>
                <#include "../inc/footer.ftl">
            </div><!-- content -->            
        </div> <!-- container -->
    </div><!-- wrap -->
</body>
</html>

