"use strict";
var map;
var objects = document.getElementsByClassName("object");
var currentLoc;
var bounds = [];
var userMenu = [];
var currObj;
var objId;
var objName;
var objStatus;
var objDateStatus;
var markers;
var message = {
  objectIsset: "Данный объект присутвует на карте",
  objectNotIsset: "Кликните по карте для добавления объекта",
  objectFromSearh: "Выбирите объект в дереве",
  objectSaveFromTree: "Нажмите на кнопку 'Сохранить' для добавления объекта на карту",
  objectsNotFound: "У данного региона нет объектов на карте",
  gragMarker: "Перетаскивайте маркер",
  updateObject: "",
  saveobject: "Объект сохранен на карте",
  deleteobject: "Объект удален"
};
// возвращаем данные обновленных объектов
function getRefreshStatus() {
  $.ajax({
    type: "POST",
    url: "/Map/NewRefresh",
    success: function(response) {
      response.forEach(function(i) {
        var date = new Date(parseInt(i.LastRequestDate.substr(6)));
        date = dateFormat(date, "dd.mm.yyyy HH:MM:ss");
        if (i.LastRequestStatus.StatusCode !== 0) {
          if (sessionStorage.getItem(i.ID) !== null) {
            // console.log(sessionStorage.getItem(i.ID));
            //map.messagebox.show(createMarkerStatusName(i.LastRequestStatus.Status) + " " + i.Name + " (" + date + ")");
            sessionStorage.setItem(i.ID,
              JSON.stringify({
                'lat': [i.XY.Y, i.XY.X],
                'name': [i.Name],
                'status': [i.LastRequestStatus.Status],
                'date': [date]
              }));
            updateMarker(i.ID, i.LastRequestStatus.StatusCode);
          }
        }
      });
    }
    });
}

setInterval("getRefreshStatus()", "20000");

// обновляем маркер(удаляем из слоя, потом создаем новый по ид)
function updateMarker(id) {
  if (arguments[1]) createMarker(JSON.parse(sessionStorage.getItem(id)), id);
  else {
    markers.eachLayer(function(layer) {
          if (id == layer.options.objectId) {
            markers.removeLayer(layer);
            createMarker(JSON.parse(sessionStorage.getItem(id)), id);
          }
        });
  }
}

// рисуем маркеры
function createMarker(obj, id) {
    bounds.push([obj.lat[0], obj.lat[1]]);
  return L.marker([obj.lat[0], obj.lat[1]],
    {
      icon: createMarkerIcon(obj.status[0]),
      title: obj.name[0] +
        ": " +
        createMarkerStatusName(obj.status[0]) +
        " - " +
        obj.date[0],
      alt: obj.name[0],
      riseOnHover: true,
      objectId: id,
      contextmenu: true,
      contextmenuItems: userMenu
    })
    .addTo(markers)
    .on("click", openMarkerPopup)
    .bindPopup();
}

// рисуем маркеры на карте при инициализации
function createPoints(map) {
  markers = L.featureGroup().addTo(map);
  // перебираем все объекты из хранилища
  for (var i = 0; i < sessionStorage.length; i++) {
    if ($.isNumeric(sessionStorage.key(i))) { // берем только строки с числовым ключем т.к. есть еще данные
        createMarker(JSON.parse(sessionStorage.getItem(sessionStorage.key(i))), sessionStorage.key(i));
    }
  }
}

// рисуем маркеры для дерева
function updateTreeObject() {
  for (var prop in objects) {
    if (typeof objects[prop] === "object" && sessionStorage.getItem(objects[prop].getAttribute("data-id"))) {
      objects[prop].innerHTML += " <i style=\"float: none; margin: 0 !important; color: inherit;\"" +
        " class=\"fa fa-map-marker\"></i>";
    }
  }
}

window.onload = updateTreeObject;

//создание объекта при выборе в дереве документов
function createObjectFromTree(e) {
    if ((sessionStorage.getItem(e.target.getAttribute("data-id")) === null) && $('#CreateObject').is('div')) {
        $(".leaflet-container").css("cursor", "url(/Content/tm/js/images/marker-icon.png), pointer");
    // console.log(e.target.getAttribute("data-id"));
    currObj = e.target;
    objId = e.target.getAttribute("data-id");
    objName = e.target.textContent;
    objStatus = e.target.getAttribute("data-status");
    objDateStatus = e.target.getAttribute("data-datestatus");
    map.on("click", saveObjectFromTree);
    //map.messagebox.show(message.objectNotIsset);
  } else {
//    map.messagebox.show(message.objectIsset);
    markers.eachLayer(function(layer) {
      if (e.target.getAttribute("data-id") == layer.options.objectId) {
        layer.bindPopup(layer.options.title);
        layer.openPopup();
        layer.update();
      }
    });

    map.off("click");
    setCenter(null, sessionStorage.getItem(e.target.getAttribute("data-id")));
  }
}

//вешаем событие на клик по дереву объектов
$("#multi-derevo li a")
  .on("click",
    function(e) {
      if (e.currentTarget.hasAttribute("data-region")) {
        $(".leaflet-container").css("cursor", "pointer");
        map.off("click", saveObjectFromTree);
        drawPointsForMap(e);
        e.stopPropagation();
      }
      if (e.currentTarget.hasAttribute("data-location")) {
        $(".leaflet-container").css("cursor", "pointer");
        map.off("click", saveObjectFromTree);
        drawPointsForMap(e);
        e.stopPropagation();
      }
      if (e.currentTarget.hasAttribute("data-object")) {        
        createObjectFromTree(e);
        e.stopPropagation();
      }
      if (e.currentTarget.getAttribute("data-id") == 0) {
        map.fitBounds(bounds);
      }
    });

//выделение области на карте
function drawPointsForMap(e) {
  var objects = e.currentTarget.closest("li").getElementsByClassName("object");
  var ids = [];
  for (var obj in objects) {
    if (objects.hasOwnProperty(obj))
      ids.push(objects[obj].getAttribute("data-id"));
  }
  if (ids.length > 0) {
    map.fitBounds(createBoundsForMap(ids));
  }
}

//собираем все объекты принадлежащие выделенное области
function createBoundsForMap(arrIds) {
  var points = [];
  arrIds.forEach(function(i) {
    if (sessionStorage.getItem(i) !== null) {
      var item = JSON.parse(sessionStorage.getItem(i));
      points.push([item.lat[0], item.lat[1]]);
    }
  });
  if (points.length > 0) {
    return points;
  }
  //map.messagebox.show(message.objectsNotFound);
  return bounds;
}

//сохранение объектов при выборе из дерева объектов
function saveObjectFromTree(e) {
  map.off("click", saveObjectFromTree);
  //console.log(objName);
  currentLoc = e.latlng;
  L.popup()
    .setLatLng(e.latlng)
    .setContent("<p class=\"text-center\">Сохранить маркер для: " +
      objName +
      " ?</p>" +
      "<div class=\"btn-group text-center\" role=\"group\" aria-label=\"...\" style=\"min-width: 175px;\">" +
      "<button id=\"saveObject\" onclick=\"saveObj(" +
      objId +
      ", " +
      currentLoc.lng +
      ", " +
      currentLoc.lat +
      ", 'CreateObject')\" class=\"btn btn-sm btn-success btn-default\"><i class=\"fa fa-floppy-o\"></i> Сохранить</button>" +
      "<button id=\"resetObject\" onclick=\"closePop()\" class=\"btn btn-danger btn-sm btn-default\"><i class=\"fa fa-times\"></i>" +
      " Отмена</button>" +
      "</div>")
    .openOn(map);
}

//сохранение объектов при выборе из дерева объектов
function deleteObjectFromMap(e) {
    var objectID = e.relatedTarget.options.objectId;
    var title = JSON.parse(sessionStorage.getItem(objectID)).name[0];
    map.off("click", deleteObjectFromMap);
    //console.log(objName);    
    L.popup()
        .setLatLng(e.latlng)
        .setContent("<p class=\"text-center\">Удалить маркер для: " +
        title +
        " ?</p>" +
        "<div class=\"btn-group text-center\" role=\"group\" aria-label=\"...\" style=\"min-width: 175px;\">" +
        "<button id=\"deleteObject\" onclick=\"deleteObject('" +
        objectID +
        "')\" class=\"btn btn-sm btn-success btn-default\"><i class=\"fa fa-floppy-o\"></i> Удалить</button>" +
        "<button id=\"resetObject\" onclick=\"closePop()\" class=\"btn btn-danger btn-sm btn-default\"><i class=\"fa fa-times\"></i>" +
        " Отмена</button>" +
        "</div>")
        .openOn(map);
}

function closePop() {
  map.closePopup();
}


//кнопка для сохраниения объекта после перетаскивания
var SaveButtonAfterDrag = L.Control.extend({
  onAdd: function(map) {
    var container = L.DomUtil.create("button", "btn btn-default btn-sm leaflet-bar");
    container.innerHTML = "<i class=\"fa fa-floppy-o\"></i> Сохранить";
    container.id = "saveAfterDrag";
    //map.messagebox.show(message.objectSaveFromTree);
    container.marker = this.options.obj; // создаем свойство с маркером, который перекинули в конструктор
    container.addEventListener("click", this._save);

    return container;
  },

  _save: function(e) {
    sessionStorage.setItem(objId, "");
    map.removeControl(L.DomUtil.get(e.target));
    saveObj(objId, currentLoc.lng, currentLoc.lat, "UpdateObject");
    e.target.marker.dragging.disable(); // выключаем перетаскивание
  }
});

//сохранение объекта
function saveObj(id, lng, lat, status) {
  if (status === "CreateObject") { // добавляем сразу маркер на карту
    sessionStorage.setItem(objId,
        JSON.stringify({
          'lat': [lat,lng],
          'name': [objName],
          'status': [objStatus],
          'date': [objDateStatus]
        }));
    updateMarker(objId, true);
    currObj.innerHTML += " <i style=\"float: none; margin: 0 !important; color: inherit;\"" +
                                 " class=\"fa fa-map-marker\"></i>";
  }
  if (status === "UpdateObject") {
    var currItem = sessionStorage.getItem(id);
    sessionStorage.setItem(id, JSON.stringify({
                                         'lat': [lat,lng],
                                         'name': currItem.name,
                                         'status': currItem.status,
                                         'date': currItem.date
                                       }));
  }

  map.closePopup();
  $.ajax({
    type: "GET",
    url: "/Map/" + status,
    data: `ObjectID=${id}&Longitude=${lng}&Latitude=${lat}`,
    success: function() {
      //map.messagebox.show(message.saveobject);
    }
  });
}

//инициализация карты
(function () {
  map = L.map("map",
    {
      contextmenu: true,
      messagebox: true,
      minZoom: 7,
      attributionControl: false
    })
    .setView([53.902828, 27.561274], 10);

  var popup = L.popup();
  //map.messagebox.options.timeout = 5000;
  // Try HTML5 geolocation.
  if (navigator.geolocation) {
    navigator.geolocation.getCurrentPosition(function(position) {
      var pos = {
        lat: position.coords.latitude,
        lng: position.coords.longitude
      };

      popup.setLatLng(pos);
      popup.setContent("Вы тут.");
      popup.openOn(map);

      map.setView([pos.lat, pos.lng], 14);
    });
  } else {
    // Browser doesn't support Geolocation
    handleLocationError(false, popup, map.getCenter());
  }

  //пользовательское меню в соответствие с правами
  userMenu.push({
      text: "Посмотреть текущие данные",
      index: 0,
      iconCls: "fa fa-eye",
      callback: onClickMarker
  });
  if ($('#UpdateObject').is('div'))
      userMenu.push({
          text: "Редактировать координаты",
          index: 1,
          iconCls: "fa fa-pencil",
          callback: dragMarker
      });
  if ($('#DeleteObject').is('div'))
      userMenu.push({
          text: "Удалить",
          index: 2,
          iconCls: "fa fa-trash",
          callback: deleteObjectFromMap//deleteObject
      });
  // рисуем маркеры
  createPoints(map);

//  подключаем поиск
  var GoogleSearch = L.Control.extend({
    onAdd: function() {
      var element = document.createElement("input");
      element.id = "pac-input";
      element.placeholder = "Введите адрес для поиска...";

      return element;
    }
  });

  (new GoogleSearch).addTo(map);

  var input = document.getElementById("pac-input");
  var searchBox = new google.maps.places.SearchBox(input);

  searchBox.addListener('places_changed',
    function() {
      var places = searchBox.getPlaces();

      if (places.length === 0) {
        return;
      }

      var group = L.featureGroup();

      places.forEach(function(place) {
        // Create a marker for each place.
        // console.log(places);
        // console.log(place.geometry.location.lat() + " / " + place.geometry.location.lng());
        var marker = L.marker([
          place.geometry.location.lat(),
          place.geometry.location.lng()
        ],
        {
          contextmenu: false,
          contextmenuItems: [
            {
              text: 'Сохранить объект',
              index: 0,
              callback: saveObject
            }
          ]
        });
        group.addLayer(marker);
      });

      group.addTo(map);
      map.fitBounds(group.getBounds());

    });

//добавляем слой карты
  var mapLayer = L.tileLayer('http://{s}.tile.osm.org/{z}/{x}/{y}.png');
  map.addLayer(mapLayer);

//рисуем точки
  map.fitBounds(bounds);
})();

map.on("popupclose, popupopen",
  function(e) {
    $(".leaflet-container").css("cursor", "pointer");
  });

function handleLocationError(browserHasGeolocation, popup, pos) {
  popup.setView([pos.lat, pos.lng], 14);
  popup.setContent(browserHasGeolocation
    ? "Ошибка: Мы не смогли найти вашу локацию."
    : "Ошибка: Ваш браузер не поддерживает поиск геопозиции.");
}

//изменение позиции маркера на карте
function dragMarker(e) {
  var marker = e.relatedTarget;
  marker.closePopup();
  marker.unbindPopup();
  marker.dragging.enable();
  //map.messagebox.show(message.gragMarker);

  marker.on("dragend",
    function(event) {
      objId = marker.options.objectId;
      currentLoc = event.target._latlng;
      if (!L.DomUtil.get("saveAfterDrag")) {
        map.addControl(new SaveButtonAfterDrag({ position: "topleft", obj: marker }));
      }
    });
}

// создаем кнопку отмены действия
function saveObject(e) {
  currentLoc = e.latlng;
  if (!L.DomUtil.get("cancel")) {
    //map.messagebox.show(message.objectFromSearh);
    map.addControl(new DeleteButton);
  }
}

// создаем кнопку для сохранения объекта
function saveAjaxObject(e) {
  objId = e.target.getAttribute("data-id");
  if (!L.DomUtil.get("save")) {
    map.addControl(new SaveButton);
  }
}

//удаление объекта
function deleteObject(id) { 
  for (var obj in objects) {
    if (typeof objects[obj] === "object" && objects[obj].getAttribute("data-id") === id) {
      var child = objects[obj].lastChild;
          objects[obj].removeChild(child);
        }
  }

  $.ajax({
    type: "POST",
    url: "/Map/DeleteObject",
    data: `ObjectID=${id}`,
    success: function(response) {
        sessionStorage.removeItem(id);
        markers.eachLayer(function (layer) {
            if (id == layer.options.objectId) {
                markers.removeLayer(layer);
            }
        });
        closePop();
      //map.removeLayer(e.relatedTarget);
      //map.messagebox.show(message.deleteobject);
    }
  });
}

// отрываем маркер и устанавливаем на центр
function openMarkerPopup(e) {
  // postLoad(e.target.options.objectId);
  var objectMarker = e.target.getPopup();
  var objectData = e.target.options;
  setCenter(e);
  if (objectMarker !== null) {
    objectMarker.closePopup();
    objectMarker.setContent(objectData.title);
    objectMarker.update();
  }
}

// set center map
function setCenter(e) {
  $(".leaflet-container").css("cursor", "pointer");
  if (e != null) {
    map.setView(e.latlng, 16, { animate: true });
  } else {
    var obj = JSON.parse(arguments[1]);
    //console.log(obj);
    map.setView([obj.lat[0], obj.lat[1]], 16, { animate: true });
  }
}

// обработчик события по клику на маркер
function onClickMarker(e) {
  $.ajax({
    type: "GET",
    url: `../Object/ViewCurrentData?ObjectID=${e.relatedTarget.options.objectId}`,
    success: function(response) {
      $("#modal_dialog").find(".modal-body").html(response);
      $("#modal_dialog").modal('show');
      $("#myModalLabel").html(e.relatedTarget.options.alt);
    }
  });
}

// устанавливаем иконку для объекта
function createMarkerIcon(status) {
  var icon;
  switch (status) {
  case "ossNone":
  case 0:
    icon = '<i class="fa fa-map-marker fa-3x blue"></i>';
    break;
  case "ossInProgress":
  case 1:
    icon = '<i class="fa fa-asterisk fa-3x fa-spin blue"></i>';
    break;
  case "ossSuccessful":
  case 2:
    icon = '<i class="fa fa-map-marker fa-3x green"></i>';
    break;
  case "ossLinkError":
  case 3:
    icon = '<i class="fa fa-map-marker fa-3x orange"></i>';
    break;
  case "ossNoCarier":
  case 4:
    icon = '<i class="fa fa-map-marker fa-3x orange"></i>';
    break;
  case "ossBusy":
  case 5:
    icon = '<i class="fa fa-map-marker fa-3x vio"></i>';
    break;
  case "ossTimeout":
  case 6:
    icon = '<i class="fa fa-map-marker fa-3x blue"></i>';
    break;
  case "ossErrorData":
  case 7:
    icon = '<i class="fa fa-map-marker fa-3x red"></i>';
    break;
  default:
    icon = '<i class="fa fa-map-marker fa-3x grey"></i>';
  }

  return L.divIcon({ html: icon });
}

// устанавливаем текст опроса
function createMarkerStatusName(status) {
  var text;

  switch (status) {
  case "ossNone":
  case 0:
    text = "Не опрашивался";
    break;
  case "ossInProgress":
  case 1:
    text = "Опрашивается";
    break;
  case "ossSuccessful":
  case 2:
    text = "Успешный опрос";
    break;
  case "ossLinkError":
  case 3:
    text = "Ошибка связи";
    break;
  case "ossNoCarier":
  case 4:
    text = "Нет связи";
    break;
  case "ossBusy":
  case 5:
    text = "Номер занят";
    break;
  case "ossTimeout":
  case 6:
    text = "Таймаут";
    break;
  case "ossErrorData":
  case 7:
    text = "Ошибка данных";
    break;
  default:
    text = "неизвестен";
  }

  return text;
}