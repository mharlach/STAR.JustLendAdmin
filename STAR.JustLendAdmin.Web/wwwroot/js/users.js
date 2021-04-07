
function defer(method) {
    if (window.jQuery) {
        method();
    } else {
        setTimeout(function () { defer(method) }, 50);
    }
}

window.onload = function () {
    defer(loadCompanies);
    defer(loadTooltips);
}

function loadTooltips() {
    $('[data-toggle="tooltip"]').tooltip();
}

function loadCompanies() {
    if ($(".companyComboBox option").length <= 1) {
        console.log("Loading companies...");
        $('.btn').attr('disabled');
        $.ajax({
            type: 'get',
            url: '/user/companyfilteroptions',
            success: function (payload, status, jqXhr) {
                $('.companyComboBox').empty();
                $('.companyComboBox').html(payload);
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

function getUsersFromCompany() {
    var request = {
        CompanyId: $('#companyComboBox').val()
    };
    getUsersPartialView(request, "/user/GetAllCompanyUsersPartialView");
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
    getUsersPartialView(request,"/user/GetFilteredUsersPartialView");
}

function getUsersPartialView(request, url) {
    $('.btn').attr('disabled');
    $.ajax({
        type: 'post',
        url: url,
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