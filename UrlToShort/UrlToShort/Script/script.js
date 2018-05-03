$("#btn_login").click(function () {
    var upData = {
        Name: $("#name").val(),
        Pass:$("#pass").val()
    };
    $.post("/Main/Login", upData, function (data) {
        if (data["result"]) {
            location.href = "/manager";
        }
        else {
            $("#login_tip").text(data["message"]);
        }
    });
});
$("#s_go").click(function () {
    location.href = "/manager?key=" + $("#s_key").val();
});
var currentID = 0;
function DelTip(id)
{
    $("#del_tip").modal();
    currentID = id;
}
$("#del_ok").click(function () {
    $.post("/Main/Del?id="+currentID, function (data) {
        if (data["result"])
            location.reload();
    });
});
document.body.onkeydown = function () {
    if (event.keyCode == 13) {
        $("#s_go").trigger("click");
        $("#btn_login").trigger("click");
    }
}
$("#btn_toshort").click(function () {
    var info = {
        Long: $("#old_url").val(),
        Short: $("#new_url").val()
    };
    $.post("/Home/ToShort", info, function (data) {
        if (data["result"]) {
            $("#tip_long").text(location.host+"/" + data["message"]);
            $("#new_url").val(data["message"]);
        }
        else {
            $("#tip_long").text(data["message"]);
        }
    });
});
$("#btn_tolong").click(function () {
    var info = {
        Short: $("#short_url").val()
    };
    $.post("/Home/ToLong", info, function (data) {
        $("#tip_short").text(data["message"]);
    });
});
$("#btn_chgpass").click(function () {
    var info = {
        Name:$("#new_pass").val(),
        Pass:$("#old_pass").val(),
        NewPass: $("#new1_pass").val()
    };
    $.post("/Main/Change", info, function (data) {
        $("#tip_chgpass").text(data["message"]);
    });
});