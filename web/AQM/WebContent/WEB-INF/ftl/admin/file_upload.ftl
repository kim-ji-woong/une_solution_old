<#ftl encoding="utf-8">
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN"
        "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml" xml:lang="en" lang="en">
<head>
<meta http-equiv="Content-Type" content="text/html; charset=utf-8">
<title>안내사항 - 파일 첨부</title>
<link rel="stylesheet" href="/AQM/daumeditor/css/popup.css" type="text/css"  charset="utf-8"/>
<script src="${Context.contextPath}/js/jquery-1.12.4.js"></script>
<script src="/AQM/daumeditor/js/popup.js" type="text/javascript" charset="utf-8"></script>

<script src="${Context.contextPath}/js/jquery.form.min.js"></script>
<script type="text/javascript">
// <![CDATA[	
	
	function uploadFile(){
		
		var option = {
		type : 'POST',
		url : '/AQM/File/new',
		dataType : 'json',
		success : function(data) {
			done(data);
			}
		};
		$(fileform).ajaxSubmit(option);
	}

		
	function done(resultJson) {
		if (typeof(execAttach) == 'undefined') { //Virtual Function
	        return;
	    }
		if(resultJson.Result > 0)
		{
			var _mockdata = {				
				'attachurl': resultJson.Upload.UploadInfo.attachurl,
				'filemime': resultJson.Upload.UploadInfo.filemime,
				'filename': resultJson.Upload.UploadInfo.filename,
				'filesize': resultJson.Upload.UploadInfo.filesize,
				'fileid' : resultJson.Upload.UploadInfo.fileid
			};
			execAttach(_mockdata);
			closeWindow();
		}
		else
		{
			alert("파일 첨부 오류가 발생하였습니다.\n서버 관리자에게 문의바랍니다.");
			closeWindow();
		}
	}

	function initUploader(){
	    var _opener = PopupUtil.getOpener();
	    if (!_opener) {
	        alert('잘못된 경로로 접근하셨습니다.');
	        return;
	    }
	    
	    var _attacher = getAttacher('file', _opener);
	    registerAction(_attacher);
	}
	
</script>
</head>
<body onload="initUploader();">
<div class="wrapper">
	<div class="header">
		<h1>파일 첨부</h1>
	</div>	
	<div class="body">
		<dl class="alert">
		    <dt>파일 첨부 확인</dt>
		    <div id="fileupload">
				<form id="fileform" method="post" enctype="multipart/form-data" action="/AQM/File/new">			
					<input type="file" name="upload_file">
				</form>
			</div>
		</dl>
	</div>	
	
	<div class="footer">
		<p><a href="#" onclick="closeWindow();" title="닫기" class="close">닫기</a></p>
		<ul>
			<li class="submit"><a href="#" onclick="uploadFile();return false;" title="등록" class="btnlink">등록</a> </li>
			<li class="cancel"><a href="#" onclick="closeWindow();" title="취소" class="btnlink">취소</a></li>
		</ul>
	</div>
</div>
</body>
</html>

