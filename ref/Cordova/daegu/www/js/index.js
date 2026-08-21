/*
 * Licensed to the Apache Software Foundation (ASF) under one
 * or more contributor license agreements.  See the NOTICE file
 * distributed with this work for additional information
 * regarding copyright ownership.  The ASF licenses this file
 * to you under the Apache License, Version 2.0 (the
 * "License"); you may not use this file except in compliance
 * with the License.  You may obtain a copy of the License at
 *
 * http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing,
 * software distributed under the License is distributed on an
 * "AS IS" BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY
 * KIND, either express or implied.  See the License for the
 * specific language governing permissions and limitations
 * under the License.
 */

function loadmain () {
    var selval = $("#selmanual option:selected").val();
//    document.removeEventListener("backbutton", onBackKeyDown);
    window.localStorage.setItem("manual", selval);
    window.location = "mainpage.html";

}

function confirmCallback (buttonIndex) {
   
    if (buttonIndex == 1) {

        navigator.app.exitApp();

    } else {

        return;
    }
}

function onBackKeyDown (e) {
   

    navigator.notification.confirm("종료하시겠습니까?", confirmCallback, '종료',['예','아니오']);

}


var app = {

    // Application Constructor
    initialize: function() {
        document.addEventListener('deviceready', this.onDeviceReady.bind(this), false);
    },
    
    onDeviceReady: function() {
        this.receivedEvent('deviceready');
    },
    
    copysuccess : function ()
    {            
        alert("success");
    },
    
    copyerror : function (e)
    {        
        alert("Error Code = "+JSON.stringify(e));        
    },
   
    receivedEvent: function(id) {

        document.addEventListener("backbutton", onBackKeyDown, false);
    }
    
};

app.initialize();
