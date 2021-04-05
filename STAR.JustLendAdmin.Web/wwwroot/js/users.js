
function loadCompanies() {
    if ($("#companyComboBox option").length <= 1) {
        console.log("Loading companies...");
        $('.btn').attr('disabled');
        $.ajax({
            type: 'get',
            url: 'user/companyfilteroptions',
            success: function (payload, status, jqXhr) {
                $('#companyComboBox').empty();
                $('#companyComboBox').html(payload);
            },
            error: function (data, status, jqXhr) {
                alert(status);
            },
            complete: function () {
                $('.btn').removeAttr('disabled');
            }
        });
    }

}

function getUsers() {
    var request = {
        CompanyId: $('#companyComboBox').val(),
        Filter: $('#filterTextBox').val(),
        Active: $('#activeCheckbox').is(':checked'),
        Agents: $('#agentsCheckbox').is(':checked'),
        Processors: $('#processorsCheckbox').is(':checked'),
        TeamLeads: $('#teamLeadsCheckbox').is(":checked"),
        TeamManagers: $('#teamManagersCheckbox').is(":checked"),
        SupressTeam: $('#supressTeamCheckbox').is(":checked"),
        Underwriters: $('#underwritersCheckbox').is(':checked'),
        Funders: $("#fundersCheckbox").is(":checked"),
        LoanSetup: $("#loanSetupCheckbox").is(":checked"),
        ExecutiveManagers: $("#executiveManagersCheckbox").is(":checked")
    };
    $('.btn').attr('disabled');
    $.ajax({
        type: 'post',
        url: 'user/UsersGridPartialView',
        data: { request: request },
        success: function (payload, staus, jqXhr) {
            $("#gridUsersPartial").empty();
            $("#gridUsersPartial").html(payload);
        },
        error: function (data, status, jqXhr) {
            alert(status);
        },
        complete: function () {
            $('.btn').removeAttr('disabled');
        }
    });
}