<#ftl encoding="utf-8">
<!DOCTYPE html>
<html lang=en>

<head>
    <meta charset=utf-8>
    <meta content="IE=edge" http-equiv=X-UA-Compatible>
    <meta name="viewport" content="width=device-width, initial-scale=1, maximum-scale=1, user-scalable=no">
    <title>U&amp;E</title>   
    
    <script src="${Context.contextPath}/js/jquery-1.12.4.js"></script>
	<script src="${Context.contextPath}/jquery-ui-1.12.1/jquery-ui.js"></script>
	
    <link rel="stylesheet" href="${Context.contextPath}/daumeditor/css/editor.css" type="text/css" charset="utf-8"/>
    <script src="${Context.contextPath}/daumeditor/js/editor_loader.js" type="text/javascript" charset="utf-8"></script>
     
    <link rel="stylesheet" href="${Context.contextPath}/css/uc.css?ver=19">
	<link rel="stylesheet" href="${Context.contextPath}/css/uidialog.css?ver=2">
	<link rel="stylesheet" href="${Context.contextPath}/jquery-ui-1.12.1/jquery-ui.css">
	
	<script src="${Context.contextPath}/js/placeholders.min.js"></script>	
	
	
    

    <script>
		function setConfig(){
			var config = { txHost: 'http://127.0.0.1:8180', /* 런타임 시 리소스들을 로딩할 때 필요한 부분으로, 경로가 변경되면 이 부분 수정이 필요. ex) http://xxx.xxx.com */ 
				txPath: '/AQM/daumeditor/',	 /* 런타임 시 리소스들을 로딩할 때 필요한 부분으로, 경로가 변경되면 이 부분 수정이 필요. ex) /xxx/xxx/ */ 
				txService: 'AQM', /* 수정필요없음. */ 
				txProject: 'AQM', /* 수정필요없음. 프로젝트가 여러개일 경우만 수정한다. */ 
				initializedId: "", /* 대부분의 경우에 빈문자열 */ 
				wrapper: "contentEditor", /* 에디터를 둘러싸고 있는 레이어 이름(에디터 컨테이너) */ 
				form: "bbs_content", /* 등록하기 위한 Form 이름 */ 
				txIconPath: "/AQM/daumeditor/images/icon/editor/", /*에디터에 사용되는 이미지 디렉터리, 필요에 따라 수정한다. */ 
				txDecoPath: "/AQM/daumeditor/images/deco/contents/", /*본문에 사용되는 이미지 디렉터리, 서비스에서 사용할 때는 완성된 컨텐츠로 배포되기 위해 절대경로로 수정한다. */ 
				canvas: 
				{ 
					styles: 
					{ 
						color: "#123456", /* 기본 글자색 */ 
						fontFamily: "굴림", /* 기본 글자체 */ 
						fontSize: "10pt", /* 기본 글자크기 */ 
						backgroundColor: "#fff", /*기본 배경색 */ 
						lineHeight: "1.5", /*기본 줄간격 */ 
						padding: "1px" /* 위지윅 영역의 여백 */ }, 
						showGuideArea: false }, 
						events: { preventUnload: false }, 
						sidebar: { attachbox: { show: true, confirmForDeleteAll: true } }, 
						size: { contentWidth: 910 /* 지정된 본문영역의 넓이가 있을 경우에 설정 */ } 
					}; 
					EditorJSLoader.ready(function(Editor) { editor = new Editor(config);});
				}

			
					
		$(function(){
    		$.ajax({
    			type:"POST",
    			url: "/AQM/daumeditor/editor.html",
    			success: function(data){
    				$("#contentEditor").html(data);
    				setConfig();
				},
				error : function(request, status, error) {
					alert("에러"); 
				}
			});
		});		
    </script>
    <script type="text/javascript">
	/* 예제용 함수 */
	function saveContent() {
		Editor.save(); // 이 함수를 호출하여 글을 등록하면 된다.
	}

	/**
	 * Editor.save()를 호출한 경우 데이터가 유효한지 검사하기 위해 부르는 콜백함수로
	 * 상황에 맞게 수정하여 사용한다.
	 * 모든 데이터가 유효할 경우에 true를 리턴한다.
	 * @function
	 * @param {Object} editor - 에디터에서 넘겨주는 editor 객체
	 * @returns {Boolean} 모든 데이터가 유효할 경우에 true
	 */
	function validForm(editor) {
		// Place your validation logic here

		// sample : validate that content exists
		var validator = new Trex.Validator();
		var content = editor.getContent();
		if (!validator.exists(content)) {
			alert('내용을 입력하세요');
			return false;
		}

		return true;
	}

	/**
	 * Editor.save()를 호출한 경우 validForm callback 이 수행된 이후
	 * 실제 form submit을 위해 form 필드를 생성, 변경하기 위해 부르는 콜백함수로
	 * 각자 상황에 맞게 적절히 응용하여 사용한다.
	 * @function
	 * @param {Object} editor - 에디터에서 넘겨주는 editor 객체
	 * @returns {Boolean} 정상적인 경우에 true
	 */
	function setForm(editor) {
        var i, input;
        var form = editor.getForm();
        var content = editor.getContent();

        // 본문 내용을 필드를 생성하여 값을 할당하는 부분
        var textarea = document.createElement('textarea');
        textarea.name = 'content';
        textarea.value = content;
        form.createField(textarea);

        /* 아래의 코드는 첨부된 데이터를 필드를 생성하여 값을 할당하는 부분으로 상황에 맞게 수정하여 사용한다.
         첨부된 데이터 중에 주어진 종류(image,file..)에 해당하는 것만 배열로 넘겨준다. */
        var images = editor.getAttachments('image');
        for (i = 0; i < images.length; i++) {
            // existStage는 현재 본문에 존재하는지 여부
            if (images[i].existStage) {
                // data는 팝업에서 execAttach 등을 통해 넘긴 데이터
                alert('attachment information - image[' + i + '] \r\n' + JSON.stringify(images[i].data));
                input = document.createElement('input');
                input.type = 'hidden';
                input.name = 'attach_image';
                input.value = images[i].data.imageurl;  // 예에서는 이미지경로만 받아서 사용
                form.createField(input);
            }
        }

        var files = editor.getAttachments('file');
        for (i = 0; i < files.length; i++) {
            input = document.createElement('input');
            input.type = 'hidden';
            input.name = 'attach_file';
            input.value = files[i].data.attachurl;
            form.createField(input);
        }
        return true;
	}
</script>
  	<!-- HTML5 shim and Respond.js IE8 support of HTML5 elements and media queries -->
	<!--[if lt IE 9]>
	  <script src="https://oss.maxcdn.com/html5shiv/3.7.3/html5shiv.min.js"></script>
	<![endif]-->
</head>

<!--[if (lte ie 9) ]>      <body class="ie9">          <![endif]-->
<!--[if (gt IE 9) ]> <body >       <![endif]-->
    <div id="wrap">
        <#include "../inc/logo.ftl">
        <div class="container">
            <#include "../inc/gnb.ftl">
            <!-- gnb -->
            <div class="content">
                <div class="breadcrumbs breadcrumbs_no_margin">
                   홈 > 관리자> <strong>글쓰기</strong>
                </div>
                <div class="well">
                    <div class="admin_sub_menu">
                      <h2 class="title_admin">글쓰기</h2>
                      <#include "../inc/admin_menu.ftl">
                    </div>
                    <form id="bbs_content" class="bbs_detail" action="${Context.contextPath}/Admin/post/new" method="post">
                        <table>
                          <thead>
                              <tr>
                                  <th width="73px" class="num">제목</th>
                                  <td colspan="5">
                                  	<input type="text" name="title" placeholder="제목을 입력하세요" />
                                  </td>
                              </tr>
                              <tr>
                              	  <th width="64px" class="line" >작성자</th>
                                  <td width="192px" >관리자</td>
                                  <th width="64px" class="line" >구분</th>
                                  <td width="188px"><span class="select select_none">
                                      <select id="postType" name="postType">
                                        <option value="0">공지</option>
                                        <option value="1" selected="selected">일반</option>
                                      </select>
                                    </span></td>
                                    <!--
                                  <th width="86px" class="line">첨부파일</th>
                                  <td width="299px"><img src="${Context.contextPath}/images/icon_file.png" /></td> -->
                              </tr>
                          </thead>
                          <tbody>
                              <tr>                                 
                                  <td id="contentEditor" colspan="6">                                  	
									<textarea name="contentText" id="contentText" cols="130" rows="30" placeholder="내용을 입력하세요" style="display:none;"></textarea>
                                  </td>
                              </tr>
                          </tbody>
                      </table>
                      <div class="btn_submit"><input type="image" onclick="saveContent();return false;" src="${Context.contextPath}/images/btn_confirm.png" /></div>
                    </form>
                </div><!-- well -->
                <#include "../inc/footer.ftl">
                <!-- footer -->
            </div><!-- content -->
        </div> <!-- container -->
    </div><!-- wrap -->

</body>

</html>
