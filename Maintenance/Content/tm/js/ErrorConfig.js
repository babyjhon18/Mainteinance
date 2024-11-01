var errors = {
    serverError: "<div class='text-center'><h4><i class='fa fa-chain-broken'></i> Запрос выполнен неудачно, пожалуйста, обратитесь в службу поддержки.</h4></div>"
}

$(document).ajaxError(function (event, jqxhr, settings, exception) {
  //console.log(settings);
  if (settings.url !== "../Object/ViewCounters?DeviceID=undefined" && settings.url !== "../Object/ViewCurrentData?ObjectID=undefined") {
    bootbox.alert(errors.serverError);
  }
});