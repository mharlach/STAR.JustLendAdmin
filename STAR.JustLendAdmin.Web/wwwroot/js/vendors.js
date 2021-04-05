function displayVendors() {
    $(".btn").attr("disabled");
    var url = "vendor/listvendors?selection=" + $("#vendorSelection option:selected").val();
    $.ajax({
        type: "get",
        url: url,
        success: function (data, status, jqXhr) {
            $("#gridPartialView").empty();
            $("#gridPartialView").html(data);
        },
        error: function (data, status, jqXhr) {
            alert(status);
        },
        complete: function () {
            $(".btn").removeAttr("disabled");
        }
    });
}