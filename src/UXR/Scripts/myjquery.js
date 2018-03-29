$.fn.whenCheckedEnable = function(disabled_id) {
    $(this).change(function () { $(this).ifCheckedEnable(disabled_id) });
};

$.fn.ifCheckedEnable = function (disabled_id) {
    if ($(this).is(':checked')) {
        $(disabled_id).removeAttr('disabled');
    } else {
        $(disabled_id).attr('disabled', 'disabled');
    }
};

function loadDefinitionEditor(textAreaId) {
    return CodeMirror.fromTextArea(document.getElementById(textAreaId), {
        lineNumbers: true,
        mode: "javascript"
    });
}