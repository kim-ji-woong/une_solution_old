1. config.json 수정
   - "server":"ice_servers" 삭제

2. web/static/js/app.js 수정
   - iceServers: [{urls: ["stun:..."]}] => iceServers: [{urls: []}] 으로 변경
   - log 함수 수정(let log = msg => ...) => console.log()로 변경
   - getParam(...) 함수 추가
   - getSize() 함수 추가
   - pc.ontrack 함수에 아래부분 추가
     if (event.track.kind === 'video') {
        const [width, height] = getSize();
        const video = document.getElementById('videoElem');
        video.muted = true;
        video.autoplay = true;
        video.controls = false;
        video.width = width;

        if (height > 0) {
            video.height = height;
        }
    }

3. Debug / Relase 모드 전환
   - http.go 파일의 serverHTTP()
   - DebugMode
     => router := gin.Default()
   - ReleaseMode
     => gin.SetMode(gin.ReleaseMode)
          router := gin.New()

4. web/templates/player.tmpl 수정
   - <h2 align...>Play Stream ...</h2> 삭제
   - <div class="row">...</div> 삭제
   - <button>Start Session</button> 삭제
   - <video> tag의 style="width..." 부분 삭제

5. CCTV url : http://127.0.0.1:8083/stream/player/suuid?w=생략가능&h=생략가능