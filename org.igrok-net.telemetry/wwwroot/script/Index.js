$(document).ready(function () {
    $("#lblAdminKey").text("Admin key: ");
    $("#lblLogin").text("Login :");
    $("#lblKey").text("Your key :");
    $("#btnAdd").click(function loadData() {
        var link = "/api/adm/create?email=";
        link += $("#txtLogin").val();
        link += "&admKey=";
        link += $("#txtAdminKey").val();
        $.get(link, function (data) {
            $("#txtKey").text(data.licence.key);
        }).fail(function (data) {
            $("#spnError").text(data.responseText);
        });
    });
    $("#btnSubmit").click(function loadData() {
        var link = "/api/adm?email=";
        link += $("#txtLogin").val();
        link += "&admKey=";
        link += $("#txtAdminKey").val();
        $.get(link, function (data) {
            $("#txtKey").text(data.licence.key);
        }).fail(function (data) {
            $("#spnError").text(data.responseText);
        });
    });
    $("#btnDeactivate").click(function () {
        var link = "/api/adm/resignlicence?email=";
        link += $("#txtLogin").val();
        link += "&admKey=";
        link += $("#txtAdminKey").val();
        $.post(link, function (data) {
            link = "/api/adm?email=";
            link += $("#txtLogin").val();
            link += "&admKey=";
            link += $("#txtAdminKey").val();
            $.get(link, function (data) {
                $("#txtKey").text(data.licence.key);
            }).fail(function (data) {
                $("#spnError").text(data.responseText);
            });
        }).fail(function (data) {
            $("#spnError").text(data.responseText);
        });
    });
    $("#btnActivate").click(function () {
        var link = "/api/adm/assignlicence?email=";
        link += $("#txtLogin").val();
        link += "&admKey=";
        link += $("#txtAdminKey").val();
        $.post(link, function (data) {
            link = "/api/adm?email=";
            link += $("#txtLogin").val();
            link += "&admKey=";
            link += $("#txtAdminKey").val();
            $.get(link, function (data) {
                $("#txtKey").text(data.licence.key);
            }).fail(function (data) {
                $("#spnError").text(data.responseText);
            });
        }).fail(function (data) {
            $("#spnError").text(data.responseText);
        });
    });
});