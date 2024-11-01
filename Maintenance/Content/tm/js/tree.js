/*



(c) 2009 r3code.habrahabr.ru 
По вопросам модификации под задачу обращайтесь 

Скрипт: Построение дерева по готовому HTML списку.
r3code.habrahabr.ru/blog/59823/

Выделяем узлы имющие поддеревья и добавляем у ним метку.
Определяет поведение узлов дерева при клике на них. 
 - Изменяет состояние маркера раскрытия (открыт/закрыт).
 - Узлы содержащие в себе другие узлы, по клику разворачиваются 
  или сворачиваются, в зависимости от текущего состояния. 
 - При переходе с одного узла на другой снимается выделение (.current) 
  и пеходит на выбранный узел.
 - Определяет последний узел с поддеревом и скрывает соединительную 
  линию до следующего узла этого уровня.
 - Сохранаяет состояние узлов (откр./закр.) и выбранный узел в cookies.
 - При установленных cookies состояние узлов восстанавливается при загрузке. 

 19/05/2009
*/
//=================================================================================

$(document).ready(function () {
    /* Расставляем маркеры на узлах, имющих внутри себя поддерево.
      Выбираем элементы 'li' которые имеют вложенные 'ul', ставим для них 
      маркер, т.е. находим в этом 'li' вложенный тег 'a' 
      и в него дописываем маркер '<em class="marker"></em>'.
      a:first используется, чтобы узлам ниже 1го уровня вложенности 
      маркеры не добавлялись повторно. 
    */
    var root = $('#multi-derevo');
    // уникальные идентификаторы всем узлам, сквозная нумерация (Nested set)
    $('li', root).each(function (index) {
        this.id = 'n' + index;
    });
    $('li:has("ul")', root).find('span:first').prepend('<i class="fa fa-plus-square-o"></i>').addClass('bold-font');

    // выбрать текущий узел
    var currentId = $.cookie('current_node');
    var currentLink = $.cookie('current_link');
    if ($("#data").length > 0) {
        if (currentLink) {
            //$('#' + currentId).find('a:first').toggleClass('current');
            $('li span a', root)
                .each(function () {
                    if (+$(this).data('id') === +currentLink) {
                        $(this).toggleClass('current');
                    }
                });
        }
    } else {
        if (currentId) {
            $('#' + currentId).find('a:first').toggleClass('current');
            //console.log(current_id);
            //var current_link = $('#' + current_id + ' span a').data('id');
        }
    }


    // вешаем событие на клик по ссылке
    //-----------------------------------
    $('li span i', root).on('click', function () {
        toggleNode(this.parentNode.parentNode);
    });

    $('li span a', root).on('click', function (e) {
        // снимаем выделение предыдущего узла
        $('a.current', root).removeClass('current');
        var a = $('a:first', this.parentNode);

        //console.log(this.parentNode);
        //alert(a.parents('li').get(0).tagName+"#"+a.parents('li').attr('id'));
        setCookie('current_node', a.parents('li').attr('id') || null);
        setCookie('current_link', a.data('id'));
        setCookie('current_class_li', a.parents('li').attr('class'));
        if (e.ctrlKey) {
            a.removeClass("current");
        } else {
            a.toggleClass('current');
        }
        // 
    });
    //postLoad(); // функция раскрытия по текущему url
    opensNodes(); // открыть по данным cookie
});

//---------------------------------------------------------------------------------
// Выделил функцию разворачивания дерева в отдельную  
function toggleNode(node) {// node= li
    prepareLast(node);
    // анимация раскрытия узла и изменение состояния маркера
    var ul = $('ul:first', node);// Находим поддерево
    if (ul.length) {// поддерево есть
        ul.slideToggle(0); //свернуть или развернуть
        // Меняем сосотояние маркера на закрыто/открыто
        var em = $('i:first', node);// this = 'li span'
        // было em.hasClass('open')?em.removeClass('open'):em.addClass('open');
        em.toggleClass('fa-plus-square-o').toggleClass('fa-minus-square-o');
        saveTreeState();
    }
}

// функция обработки последнего узла в уровне
function prepareLast(node) {
    /* если это последний узел уровня, то соединительную линию к следующему
    рисовать не нужно */
    $(node).each(function () {
        if (!$(this).next().length) {
            /* берем корень разветвления <li>, в нем находим поддерево <ul>,
             выбираем прямых потомков ul > li, назначаем им класс 'last' */
            $(this).find('ul:first > li').addClass('last');
        }
    });
}
// функция разворачивания дерева до выбранной ранее ссылки
function postLoad(e) {
    //setCookie("open_nodes", null);
    var a = null;
    $('#multi-derevo li span a').each(function (item) {
        // сравниваем адрес страницы и ссылку из атрибута
        if ($(this).data("id") == e) {
            a = $(this);
        }
    });
    // если узел не виден, то разворачиваем дерево 
    if ($(a).is(':hidden') || $(a).parents(':hidden').length) {
        var li = $(a).parents().filter('li');
        // prepareLast(li);
        toggleNode(li);
    }
    // выделим выбранный узел
    if (a) {
        $(a).toggleClass('current');
    }
    else { // первый показ, выберем первую ссылку (можно убрать если не нужно)
        $('#multi-derevo li span a:first').toggleClass('current');
    }
}

// подготовка информации о сосотояниях узлов
function GetOpenedNodes(items) { // li:has('ul')
    var str = [];
    $(items).each(function () {
        var res = $(this).attr('id');
        //console.log(res);
        var state = $('i:first', this).hasClass('fa-minus-square-o') ? 1 : '';
        if (res && state) {
            //console.log(state);
            str.push(res);
        }
    });
    return str.join(',');
}

// сохранить полный список открытых узлов
function saveTreeState() {
    var openId = GetOpenedNodes($('#multi-derevo li:has("ul")')) || null;
    setCookie("open_nodes", openId);
    //console.log(open_id);
    return false;
}

// раскрытие узлов по указанному списку
function opensNodes() {
    // читаем куки и открываем узлы
    var openNodes = $.cookie("open_nodes");
    if (openNodes) {
        var nodes = openNodes.split(',');
        //console.log(nodes);
        if (nodes[0]) {
            for (var node in nodes) {
                if (nodes.hasOwnProperty(node)) {
                    nodes[node] = '#' + nodes[node];
                }
            }
            var ids = nodes.join(',');
            $(ids).each(function () {
                toggleNode($(this));
            });
        }
        $('#multi-derevo').scrollTop($.cookie('scroll'));
    }
    return false;
}

// настройки хранить в Cookies 1 день
function setCookie(name, value) {
    var date = new Date();
    var minutes = 30;
    date.setTime(date.getTime() + (minutes * 60 * 1000));
    $.cookie(name, value, { expires: date, path: '/'});
}