var alphaNum = /^[а-яА-ЯёЁa-zA-Z0-9]+$/;
var alpha = /^[а-яА-ЯёЁa-zA-Z]+$/;

function showResult(str, searchType, link) {
    if (inputValidate(searchType, str)) {
        document.getElementById("err").innerHTML = "";
        if (str.length === 0) {
            document.getElementById("livesearch").innerHTML = "";
            document.getElementById("err").innerHTML = "";
            document.getElementById("livesearch").style.border = "0px";
            return;
        }
        var xmlhttp;
        if (window.XMLHttpRequest) {
            // code for IE7+, Firefox, Chrome, Opera, Safari
            xmlhttp = new XMLHttpRequest();
        } else {  // code for IE6, IE5
            xmlhttp = new ActiveXObject("Microsoft.XMLHTTP");
        }
        xmlhttp.onreadystatechange = function () {
            if (xmlhttp.readyState === 4 && xmlhttp.status === 200) {
                document.getElementById("livesearch").innerHTML = xmlhttp.responseText;
            }
        }
        switch (searchType) {
            case 0:
            case "0":
            case "1":
            case "2":
            case "3":
            case "4":
                xmlhttp.open("GET", "/SystemRoutines/Search?SearchType=" + searchType + "&Condition=" + str + "&RedirectAction=" + link, true);
                break;
            case 5:
                xmlhttp.open("GET", "/SystemRoutines/Search?SearchType=" + searchType + "&Condition=" + str + "&RedirectAction=" + link, true);
                break;
        }
        xmlhttp.send();
    } else {
        if (str.length === 0) {
            document.getElementById("err").innerHTML = "";
        } else {
            document.getElementById("livesearch").innerHTML = "";
            document.getElementById("err").innerHTML = '<div class="alert alert-danger" role="alert"><span class="glyphicon glyphicon-exclamation-sign" aria-hidden="true"></span>Вы ввели неверное значение!</div>';
        }
    }

}
function inputValidate(searchT, condition) {
    switch (searchT) {
        case "0":
            if (alpha.test(condition)) {
                return true;
            }
            break;
        case "1":
            if ($.isNumeric(condition)) {
                return true;
            }
            break;
        case "2":
            if (alphaNum.test(condition)) {
                return true;
            }
            break;
    }
    return true;
}
function sendConsumerID(link, id) {
    $.ajax({
        cache: false,
        type: "get",
        url: link,
        success: function (data) {
            $(".data").html(data);
            hideSearch();
            $('[value="' + id + '"]').click();
        },
        error: function () {
            bootbox.alert("<div class='text-center'><h4><i class='fa fa-chain-broken'></i> Запрос выполнен неудачно, пожалуйста, обратитесь в службу поддержки.</h4></div>");
        }
    });
}
function sendID(link, id) {
    $.ajax({
        cache: false,
        type: 'get',
        url: link,
        success: function (data) {
            $("#data").html(data);
            hideSearch();
            openTreeNodes(id);
            var a = $('a:first', this.parentNode);
            setCookie('current_node', id);
            setCookie('current_link', id);
            
        },
        error: function () {
            bootbox.alert("<div class='text-center'><h4><i class='fa fa-chain-broken'></i> Запрос выполнен неудачно, пожалуйста, обратитесь в службу поддержки.</h4></div>");
        }
    });
}
function openTreeNodes(id) {
    var currLink = $("#multi-derevo li span a." + id);
    var str = [];
    opensNodes();
    $("#multi-derevo li span a").removeClass("current");
    currLink.toggleClass("current");
    $(currLink).parentsUntil("#n0").each(function (index, elem) {
        if (elem.id !== "") {
            console.log("Элемент: " + elem.id);
            str.push(elem.id);
        }
    });
    str.splice(0, 1);
    str.push("n0");
    var openNodes = $.cookie("open_nodes");
    setCookie("open_nodes", str);
    if (!openNodes) {
        opensNodes();
    }
}
function hideSearch() {
    $("#livesearch").hide();
}

function showSearch() {
    $("#livesearch").show();
}