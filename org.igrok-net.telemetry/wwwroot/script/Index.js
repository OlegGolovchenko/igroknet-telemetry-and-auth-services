$(document).ready(function () {
    $("#lblAdminKey").text("Admin key: ");
    $("#lblLogin").text("Login :");
    $("#lblKey").text("Your key :");
    $("#lblOS").text("OS version :");
    $("#lblNetFx").text(".Net Framework version :");
    $("#lblError").text("Error message :");
    $("#btnAdd").click(function loadData() {
        var link = "/api/adm/create?email=";
        link += $("#txtLogin").val();
        link += "&admKey=";
        link += $("#txtAdminKey").val();
        $.get(link, function (data) {
            $("#txtKey").text(data.licence.key);
            $("#txtOS").text(data.telemetry.osVersion);
            $("#txtNetFx").text(data.telemetry.netFxVersion);
            $("#spnError").text("");
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
            $("#txtOS").text(data.telemetry.osVersion);
            $("#txtNetFx").text(data.telemetry.netFxVersion);
            $("#spnError").text("");
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
                $("#txtOS").text(data.telemetry.osVersion);
                $("#txtNetFx").text(data.telemetry.netFxVersion);
                $("#spnError").text("");
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
                $("#txtOS").text(data.telemetry.osVersion);
                $("#txtNetFx").text(data.telemetry.netFxVersion);
                $("#spnError").text("");
            }).fail(function (data) {
                $("#spnError").text(data.responseText);
            });
        }).fail(function (data) {
            $("#spnError").text(data.responseText);
        });
    });
});