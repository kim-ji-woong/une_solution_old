	function _uploadFile(){
		
		var option = {
		type : 'POST',
		url : '/AQM/File/map/new',
		dataType : 'json',
		success : function(data) {
			done(data);
			}
		};
		$(fileform).ajaxSubmit(option);
	}
	
	function _done(resultJson) {
		if (typeof(attachResult) == 'undefined') { //Virtual Function
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
			attachResult(_mockdata);
		}
		else
		{
			alert("파일 첨부 오류가 발생하였습니다.\n서버 관리자에게 문의바랍니다.");
		}
	}

	$(function() {
		$('#NodeAddName').keypress(function(event) {
			if (event.keyCode == 13) {
	   			event.preventDefault();
			}
		});
	    $('#NodeAddName').keyup(function() {
	        var txt = $('#NodeAddName').val();
	        $('#NodeAddName').val(txt.replace(/[\n\r]+/g, " "));
	    });
	    
		var dialog;		
		dialog = $("#popup2").dialog({
			autoOpen : false,
			height : 460,
			width : 740,
			modal : true,
			resizable: true,			
			dialogClass : 'no-close popup',
			open: function(){
				$('.ui-widget-overlay').bind('click', function() {
                	$('#popup2').dialog('close');
            	});
			},			
			buttons : {				
			},
			close : function() {
			}
		});	
	});
	function showAddNode()
	{
		$("#popup2").dialog('open');
	}