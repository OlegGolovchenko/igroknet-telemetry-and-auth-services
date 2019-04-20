$(document).ready(function () {
    $("#lblLogin").text("Login :");
    $("#lblKey").text("Your key :");
    $("#btnSubmit").click(function loadData() {
        var link = "/api/user/request?email=";
        link += $("#txtLogin").val();
        $.get(link, function (data, status) {
            $("#txtKey").text(data.licence.key);
        });
    });
});