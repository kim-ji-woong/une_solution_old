let stream = new MediaStream();

let suuid = $('#suuid').val();

let config = {
  iceServers: [{
    urls: []
  }]
};

const pc = new RTCPeerConnection(config);
pc.onnegotiationneeded = handleNegotiationNeededEvent;

let log = msg => {
    console.log(msg);
  //document.getElementById('div').innerHTML += msg + '<br>'
}

function getParam(paramName) {
    let params = location.search.substr(location.search.indexOf("?") + 1);
    let sval = "";
    params = params.split("&");

    for (var i = 0; i < params.length; i++) {
        const temp = params[i].split("=");
        if ([temp[0]] == paramName) {
            sval = temp[1];
        }
    }

    return sval;
}

function getSize() {
    let width = getParam('w');
    let height = getParam('h');

    if (width === null || width === undefined || width.length === 0) {
        width = 600;
    }
    else {
        width = parseInt(width);
    }

    if (height === null || height === undefined || height.length === 0) {
        height = 0;
    }
    else {
        height = parseInt(height);
    }

    return [width, height];
}

pc.ontrack = function(event) {
  stream.addTrack(event.track);
  videoElem.srcObject = stream;
  log(event.streams.length + ' track is delivered');

    if (event.track.kind === 'video') {
        const [width, height] = getSize();
        const video = document.getElementById('videoElem');
        video.muted = true;
        video.autoplay = true;
        video.controls = false;
        video.width = width;

        const col = video.parentNode.parentNode;
        col.setAttribute("style", "padding-left:0; padding-right:0; background-color:black;");

        if (height > 0) {
            video.height = height;
        }
    }
}

pc.oniceconnectionstatechange = e => log(pc.iceConnectionState)

async function handleNegotiationNeededEvent() {
  let offer = await pc.createOffer();
  await pc.setLocalDescription(offer);
  getRemoteSdp();
}

$(document).ready(function() {
  $('#' + suuid).addClass('active');
  getCodecInfo();
});


function getCodecInfo() {
  $.get("../codec/" + suuid, function(data) {
    try {
      data = JSON.parse(data);
    } catch (e) {
      console.log(e);
    } finally {
      $.each(data,function(index,value){
        pc.addTransceiver(value.Type, {
          'direction': 'sendrecv'
        })
      })
      //send ping becouse PION not handle RTCSessionDescription.close()
      sendChannel = pc.createDataChannel('foo');
      sendChannel.onclose = () => console.log('sendChannel has closed');
      sendChannel.onopen = () => {
        console.log('sendChannel has opened');
        sendChannel.send('ping');
        setInterval(() => {
          sendChannel.send('ping');
        }, 1000)
      }
      sendChannel.onmessage = e => log(`Message from DataChannel '${sendChannel.label}' payload '${e.data}'`);
    }
  });
}

let sendChannel = null;

function getRemoteSdp() {
  $.post("../receiver/"+ suuid, {
    suuid: suuid,
    data: btoa(pc.localDescription.sdp)
  }, function(data) {
    try {
      pc.setRemoteDescription(new RTCSessionDescription({
        type: 'answer',
        sdp: atob(data)
      }))
    } catch (e) {
      console.warn(e);
    }
  });
}