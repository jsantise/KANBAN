// This is to sort the cards on the board using JQuery UI Sortable
$(".sortable").sortable({
    revert: true,
    placeholder: 'drag-place-holder',
    forcePlaceholderSize: true,
    connectWith: ".sortable",
    helper: function (event, element) {
        return $(element).clone().addClass('dragging');
    },
    start: function (e, ui) {
        ui.item.show().addClass('ghost')
    },
    stop: function (e, ui) {
        ui.item.show().removeClass('ghost')
    },
    handle: "span",
    update: function (event, ui) {
        var data = $(this).sortable('serialize');
        $.ajax({ // this is a post
            url: $('#reorderCardsURL').text(),
            type: 'POST',
            contentType: 'application/json',
            data: JSON.stringify({ serializedData: data, BoardID: $('#selectedBoardID').text(), Parent: $(this).attr('id').split("_")[1] }),
            error: function (err, result) {
                alert("Error: " + err.responseText);
            }
        });
    },
    cursor: 'move'
}).disableSelection();

// This is to reorder the sections on the section edit screen
$(".sortable-section").sortable({
    revert: true,
    placeholder: 'drag-place-holder',
    forcePlaceholderSize: true,
    connectWith: ".sortable-sub-section",
    helper: function (event, element) {
        return $(element).clone().addClass('dragging');
    },
    start: function (e, ui) {
        ui.item.show().addClass('ghost')
    },
    stop: function (e, ui) {
        ui.item.show().removeClass('ghost')
    },
    handle: "span",
    axis: "y",
    update: function (event, ui) {
        var data = $(this).sortable('serialize');
        $.ajax({
            url: $('#reorderSectionsURL').text(),
            type: 'POST',
            contentType: 'application/json',
            data: JSON.stringify({ serializedData: data, BoardID: $('#selectedBoardID').text(), ParentID: $(this).attr('id').split("_")[1] }),
            success: function (response) {
                window.location.href = response.Url;
            },
            error: function (err, result) {
                alert("Error: " + err.responseText);
            }
        });
    },
    cursor: 'move'
}).disableSelection();

// this is to reorder the nested sections on the edit screen
$(".sortable-sub-section").sortable({
    revert: true,
    placeholder: 'drag-place-holder',
    forcePlaceholderSize: true,
    connectWith: ".sortable-section",
    helper: function (event, element) {
        return $(element).clone().addClass('dragging');
    },
    start: function (e, ui) {
        ui.item.show().addClass('ghost')
    },
    stop: function (e, ui) {
        ui.item.show().removeClass('ghost')
    },
    handle: "span",
    axis: "y",
    update: function (event, ui) {
        var data = $(this).sortable('serialize');
        $.ajax({
            url: $('#reorderSectionsURL').text(),
            type: 'POST',
            contentType: 'application/json',
            data: JSON.stringify({ serializedData: data, BoardID: $('#selectedBoardID').text(), ParentID: $(this).attr('id').split("_")[1] }),
            error: function (err, result) {
                alert("Error: " + err.responseText);
            }
        });
    },
    cursor: 'move'
}).disableSelection();

// this is to reorder the boards
$(".sortable-board").sortable({
    revert: true,
    placeholder: 'drag-place-holder',
    forcePlaceholderSize: true,
    connectWith: ".sortable-board",
    helper: function (event, element) {
        return $(element).clone().addClass('dragging');
    },
    start: function (e, ui) {
        ui.item.show().addClass('ghost')
    },
    stop: function (e, ui) {
        ui.item.show().removeClass('ghost')
    },
    handle: "span",
    axis: "y",
    update: function (event, ui) {
        var data = $(this).sortable('serialize');
        $.ajax({
            url: $('#reorderBoardsURL').text(),
            type: 'POST',
            contentType: 'application/json',
            data: JSON.stringify({ serializedData: data, BoardID: $('#selectedBoardID').text() }),
            error: function (err, result) {
                alert("Error: " + err.responseText);
            }
        });
    },
    cursor: 'move'
}).disableSelection();

//On page load
$(function () {
    $(document).ajaxStart($.blockUI).ajaxStop($.unblockUI);

    // set the min width of the card on small columns for readability
    if ($('.card').width() >= 350) {
        $('.card').width(238);
    }

    //.hiddenPanel width
    $(".hiddenPanel").width($("#mainBody").width());

    // resize buttons to fit next to input without wrapping down to the next line.
    $("#add-board-input").height($("#add-board-button").outerHeight());
    $("#add-board-input").width($("#add-board-wrapper").width() - $("#add-board-button").outerWidth(true) - 10);

    $("#add-section-input").height($("#add-section-button").outerHeight());
    $("#add-section-input").width($("#add-section-wrapper").width() - $("#add-section-button").outerWidth(true) - 10);

    $("#create-card-section").height($("#create-card-button").outerHeight());
    $("#create-card-title").height($("#create-card-button").outerHeight());
    $("#create-card-content").height($("#create-card-button").outerHeight());
    $('#CardColor').height($("#create-card-button").outerHeight());

    //Must determine size of inputs before hiding them.
    $('#appSections.hideMe').hide();
    $(".sidebar-section-head.toggleable").click(function () {
        $(this).next().toggle();
    });

    //set focus on section input
    $("#create-card-section").focus();

    // colors in dropdown list
    $('#CardColor').val(12);
    $('#CardColor option[value="0"]').addClass("blue-light");
    $('#CardColor option[value="1"]').addClass("purple-light");
    $('#CardColor option[value="2"]').addClass("red-light");
    $('#CardColor option[value="3"]').addClass("pink-light");
    $('#CardColor option[value="4"]').addClass("deep-purple-light");
    $('#CardColor option[value="4"]').text("deep purple");
    $('#CardColor option[value="5"]').addClass("indigo-light");
    $('#CardColor option[value="6"]').addClass("light-blue-light");
    $('#CardColor option[value="6"]').text("light blue");
    $('#CardColor option[value="7"]').addClass("cyan-light");
    $('#CardColor option[value="8"]').addClass("teal-light");
    $('#CardColor option[value="9"]').addClass("green-light");
    $('#CardColor option[value="10"]').addClass("light-green-light");
    $('#CardColor option[value="10"]').text("light green");
    $('#CardColor option[value="11"]').addClass("lime-light");
    $('#CardColor option[value="12"]').addClass("yellow-light");
    $('#CardColor option[value="13"]').addClass("amber-light");
    $('#CardColor option[value="14"]').addClass("orange-light");
    $('#CardColor option[value="15"]').addClass("deep-orange-light");
    $('#CardColor option[value="15"]').text("deep orange");
    $('#CardColor option[value="16"]').addClass("brown-light");
    $('#CardColor option[value="17"]').addClass("blue-grey-light");
    $('#CardColor option[value="17"]').text("blue grey");
    $('#edit-card-color option[value="0"]').addClass("blue-light");
    $('#edit-card-color option[value="1"]').addClass("purple-light");
    $('#edit-card-color option[value="2"]').addClass("red-light");
    $('#edit-card-color option[value="3"]').addClass("pink-light");
    $('#edit-card-color option[value="4"]').addClass("deep-purple-light");
    $('#edit-card-color option[value="4"]').text("deep purple");
    $('#edit-card-color option[value="5"]').addClass("indigo-light");
    $('#edit-card-color option[value="6"]').addClass("light-blue-light");
    $('#edit-card-color option[value="6"]').text("light blue");
    $('#edit-card-color option[value="7"]').addClass("cyan-light");
    $('#edit-card-color option[value="8"]').addClass("teal-light");
    $('#edit-card-color option[value="9"]').addClass("green-light");
    $('#edit-card-color option[value="10"]').addClass("light-green-light");
    $('#edit-card-color option[value="10"]').text("light green");
    $('#edit-card-color option[value="11"]').addClass("lime-light");
    $('#edit-card-color option[value="12"]').addClass("yellow-light");
    $('#edit-card-color option[value="13"]').addClass("amber-light");
    $('#edit-card-color option[value="14"]').addClass("orange-light");
    $('#edit-card-color option[value="15"]').addClass("deep-orange-light");
    $('#edit-card-color option[value="15"]').text("deep orange");
    $('#edit-card-color option[value="16"]').addClass("brown-light");
    $('#edit-card-color option[value="17"]').addClass("blue-grey-light");
    $('#edit-card-color option[value="17"]').text("blue grey");

    // set up the delete modal
    $("#delete-board-dialog-form").hide();
    $("#delete-title").val("");
    $("#delete-id").val("");
    delete_dialog = $("#delete-board-dialog-form").dialog({
        autoOpen: false,
        height: 300,
        width: 500,
        modal: true,
        close: function () {
            $("#delete-title").val("");
            $("#delete-id").val("");
        },
        open: function (event, ui) { $('#delete-title').focus(); }
    });

    // set up the copy modal
    $("#copy-board-dialog-form").hide();
    $("#copy-id").val("");
    copy_dialog = $("#copy-board-dialog-form").dialog({
        autoOpen: false,
        modal: true,
        height: 150,
        close: function () {
            $("#copy-id").val("");
        }
    });

    // set up the delete section modal
    $("#delete-section-dialog-form").hide();
    $("#delete-section-title").val("");
    $("#delete-section-id").val("");
    delete_section_dialog = $("#delete-section-dialog-form").dialog({
        autoOpen: false,
        height: 300,
        width: 500,
        modal: true,
        close: function () {
            $("#delete-section-title").val("");
            $("#delete-section-id").val("");
        },
        open: function (event, ui) { $('#delete-section-title').focus(); }
    });


    $("#create-card-sectionID").val("");
    $("#create-card-title").val("");
    $("#create-card-content").val("");

    $("#delete-card-dialog-form").hide();
    $("#delete-card-title").val("");
    $("#delete-card-id").val("");
    delete_card_dialog = $("#delete-card-dialog-form").dialog({
        autoOpen: false,
        height: 300,
        width: 500,
        modal: true,
        close: function () {
            $("#delete-card-title").val("");
            $("#delete-card-id").val("");
        },
        open: function (event, ui) { $('#delete-card-title').focus(); }
    });
}); // end document ready

function deleteBoard(ID, title) {
    $("#delete-title").text(title);
    $("#delete-id").val(ID);
    delete_dialog.dialog("open");
}

function deleteSection(ID, title) {
    $("#delete-section-title").text(title);
    $("#delete-section-id").val(ID);
    delete_section_dialog.dialog("open");
}

function editCard(ID, title, content, cardcolor) {
    $("#edit-card-title").val(decodeURI(title));
    $("#edit-card-id").val(ID);
    $("#edit-card-content").val(decodeURI(content));
    $("#edit-card-color").val(cardcolor);
    $('#edit-card-dialog-form').show('slide', { direction: 'right' }, 500, function () { });
}

function deleteCard(ID, title) {
    $("#delete-card-title").text(title);
    $("#delete-card-id").val(ID);
    delete_card_dialog.dialog("open");
}

function toggleSideBar() {
    $('.left.col').toggle();
    if ($('.left.col').is(':visible')) {
        $('.right.col').css('left', 250);
    }
    else {
        $('.right.col').css('left', 0);
    };
}

function showPanel(element) {
    $('#' + element).show('slide', { direction: 'right' }, 500, function () { });
}
