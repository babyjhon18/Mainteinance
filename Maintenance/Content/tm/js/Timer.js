var prevNowPlaying = null;
function parseTime_bv(timestamp) {
    if (timestamp < 0) timestamp = 0;
 
    var day = Math.floor( (timestamp/60/60) / 24);
    var hour = Math.floor(timestamp/60/60);
    var mins = Math.floor((timestamp - hour*60*60)/60);
    var secs = Math.floor(timestamp - hour*60*60 - mins*60); 
    var left_hour = Math.floor( (timestamp - day*24*60*60) / 60 / 60 );
 
    $('span.afss_day_bv').text(day);
    $('span.afss_hours_bv').text(left_hour);
 
    if(String(mins).length > 1)
        $('span.afss_mins_bv').text(mins);
    else
        $('span.afss_mins_bv').text("0" + mins);
    if(String(secs).length > 1)
        $('span.afss_secs_bv').text(secs);
    else
        $('span.afss_secs_bv').text("0" + secs);
}

function clearTimer() {
    $('span.afss_day_bv').text("");
    $('span.afss_hours_bv').text("");
}

function TimerForUser(time) {
    if (prevNowPlaying) {
        clearInterval(prevNowPlaying);
    }

    prevNowPlaying = setInterval(function () {
        time = time - 1;
        parseTime_bv(time);
        if (time <= 0) {
            $(location).attr('href', '/Auth/Login');
            //window.location.href("");
        }
    }, 1000);

    return prevNowPlaying;
}

function alertClose() {
    $(".alert").alert("close");
}

//close alert after 4sec
setTimeout(
    alertClose.bind()
, 4000);
