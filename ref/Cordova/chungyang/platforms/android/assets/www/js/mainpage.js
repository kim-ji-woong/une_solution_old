

whichManaul = window.localStorage.getItem("manual");

dbtempname = "js/manualdb.js";

if(whichManaul == 1) {
    dbtempname = "js/manualdb.js";
} else {
    dbtempname = "js/firemanualdb.js";
}

/****** 런타임에 동적으로 스크립트로 된 데이터 파일을 로딩하기 위한 방법 : by hypark ******/
var script = document.createElement("script");
script.addEventListener("load", function(event) {
    console.log("Script finished loading and executing");
    var titledata =  colData.title;
    document.getElementById("titlespan").innerHTML = "" + titledata;
 
    Dexie.exists(dbname).then(function(exists) {    
    
        if (exists) {
            console.log("Database exists");
            db = new Dexie(dbname);
            db.version(1).stores({
                manualcollection : "++id, org_list, process, detail_name, process_code, action_name, action_code, htmldata, detail_code"    
            });
        }
        else {
            console.log("Database doesn't exist!. Make DB");
            db = new Dexie(dbname);
            db.version(1).stores({
                manualcollection : "++id, org_list, process, detail_name, process_code, action_name, action_code, htmldata, detail_code"    
            });        

            //데이터 생성 
            for(var i =0; i < colData.datas.length; i++) {
                db.manualcollection.add({
                    org_list : colData.datas[i].org_list,
                    process : colData.datas[i].process,
                    detail_name : colData.datas[i].detail_name,
                    process_code : colData.datas[i].process_code,
                    action_name : colData.datas[i].action_name,
                    action_code : colData.datas[i].action_code,
                    htmldata : colData.datas[i].htmldata,
                    detail_code : colData.datas[i].detail_code            
                });
            } 
        }
    //    uniqueOrglist = new Set();
        //해당 매뉴얼의 조직리스트 전체 가져오기
        db.manualcollection.toArray(function(arrayresult) {

            for(var i=0; i < arrayresult.length; i++) {
                for(var j = 0; j < arrayresult[i].org_list.length; j++) {
                    uniqueOrglist.add(arrayresult[i].org_list[j].trim());
                }
            }
            for (let item of uniqueOrglist) $("#orgselect").append("<option value='"+item+"'>"+item+"</option>");

        });
        
        document.addEventListener("backbutton", onBack, false);
    }).catch(function (error) {
        console.error("Oops, an error occurred when trying to check database existance");
    });
    
});

script.src = dbtempname;
script.async = true;
document.getElementsByTagName("script")[0].parentNode.appendChild(script);



backstage = "#action5";
currenttab = null;
currentPage = "#action1";
db = null;

contentsElement = document.getElementById('action1');

dbname = "testdb2" + whichManaul;

uniqueOrglist = new Set();

$("#orgselect").change(function() {
    
    var selectedDepart = $("#orgselect option:selected").text();  
    
    
    db.manualcollection.orderBy('action_code').filter(function (oneitem) {
        
        if($.inArray(selectedDepart, oneitem.org_list)>=0) return true;        
        return false; 
        
    }).sortBy('process_code', function (arrayResult) {
        
        var htmlstring = makeHtmlString(arrayResult);
        var tmpstr = "";
        var searchResultDivElement = document.getElementById('orgresult');       
       
        while (searchResultDivElement.firstChild) {   
            searchResultDivElement.removeChild(searchResultDivElement.firstChild);
        }

        searchResultDivElement.innerHTML = tmpstr + htmlstring;
        currentPage = "#actionorg";
    });   
      
    
});
    
function searchtabopen () {
    backstage = currentPage;
    var searchResultDivElement = document.getElementById('searchresult');       
       
    while (searchResultDivElement.firstChild) {   
        searchResultDivElement.removeChild(searchResultDivElement.firstChild);
    }
//    currentPage = "actionsearch";
   
}

function orgtabopen () {
    
}
       

function makeHtmlString(jsonArray) {
    
    var htmlstring ="";
    for(var i=0;i<jsonArray.length;i++) {
        var temp = jsonArray[i];
        var orglist = "";
        var orgstrlist = "[";
        for(var j = 0; j < temp.org_list.length; j++) {            
            orglist = orglist + "["+temp.org_list[j]+"] ";   
            if(j>=1)
                orglist = orglist + ",";
            orgstrlist = orgstrlist + "'"+temp.org_list[j]+"'";
        }
        orgstrlist = orgstrlist + "]"
        var panelheadingstr = "panel-heading-symptom";
        if(temp.process_code == 2) panelheadingstr = "panel-heading-init";
        if(temp.process_code == 3) panelheadingstr = "panel-heading-emergency";
        if(temp.process_code == 4) panelheadingstr = "panel-heading-recovery";


        htmlstring = htmlstring + "<div class=\"panel panel-info\">" +
            "<div class=\"panel-heading " + panelheadingstr + " title\"> 조치목록 : " + temp.action_name + " [" + temp.action_code + "]</div>" +
            "<div class=\"panel-body\">" +    
            "<a role=\"presentation\" data-toggle=\"tab\" href=\"#action5\" onclick=\"processHWPdetail("+ temp.action_code + ", " + temp.detail_code + ", '" + temp.org_list[0] +"');\">" + temp.detail_name + " [" + temp.action_code + "-" + temp.detail_code + "]" +            
            "</a></div>" +
            "<div class=\"panel-footer\">" + orglist +    
            "</div></div>";

    }
    return htmlstring;
}

function test () {
  
    alert("aaa");
    openTabAction();

}


function searchManual () {    
    
    var searchtext = $('#search').val().trim();
    backstage = currentPage;
    
    
    db.manualcollection.orderBy('action_code').filter(function (oneitem) {
        
        if(oneitem.action_name.includes(searchtext)) return true;
        if(oneitem.detail_name.includes(searchtext)) return true;
        if(oneitem.process.includes(searchtext)) return true;
        return false; 
        
    }).sortBy('process_code', function (arrayResult) {
        
        var htmlstring = makeHtmlString(arrayResult);    
        
        var tmpstr = "";
        var searchResultDivElement = document.getElementById('searchresult');       
       
        while (searchResultDivElement.firstChild) {   
            searchResultDivElement.removeChild(searchResultDivElement.firstChild);
        }

        searchResultDivElement.innerHTML = tmpstr + htmlstring;
        currentPage = "#actionsearch";
    });   
    
    
}




function initialProcessPage(code) {
    
    backstage = currentPage;
    var contents = document.getElementById('action'+code);
    contentsElement = contents;

    while (contents.firstChild) {

        contents.removeChild(contents.firstChild);
    }
    
    db.manualcollection.where({process_code:code}).sortBy('action_code', appendProcessContents);

    currentPage = '#action'+code;
}


function appendProcessContents (result) {

    var htmlstring = makeHtmlString(result);     
    var tmpstr = "";                
    contentsElement.innerHTML = tmpstr + htmlstring;
    
}

detailcontents = document.getElementById('action5');

function processHWPdetail(actioncode, detailcode, org) {
    
    backstage = currentPage;
    
    while (detailcontents.firstChild) {
        detailcontents.removeChild(detailcontents.firstChild);
    }

    db.manualcollection.where({ action_code : actioncode, detail_code : detailcode})
        .filter(function (oneitem) {
            return oneitem.org_list[0] === org;
    }).first(appendDetailContents);       
    
}


function appendDetailContents(item) {
    
    var base64decode = decodeURIComponent(escape(window.atob(item.htmldata)));    
    detailcontents.innerHTML = base64decode;
    currentPage = "#action5";
    $("a[href='" + currentPage + "']").tab("show");
    $("body").scrollTop(0);
}



function openTabAction() {
    
    $("a[href='" + backstage + "']").tab("show");
}

function onBack(e) {    
    
    if(currentPage != "#action5") 
        window.location = "index.html";
    if(backstage != "#action5")
       openTabAction();   

}

$('a[data-toggle="tab"]').on('shown.bs.tab', function (e) {
    
    currentPage = $(e.target).attr('href');
});