
var table;

$(document).ready(function () {

    $("#dateFrom").datepicker({
        changeMonth: true,
        changeYear: true
    });

    $("#dateTo").datepicker({
        changeMonth: true,
        changeYear: true
    });

    var resposne = financialSummary();

    table = $("#tblReport").DataTable({

        "bSort": false,
        "displayLength": 50,
        "aaData": resposne,
        "aoOrder": [[0, 'asc']],
        dom: 'Bfrtip',
        buttons: [
            'print',
            'copyHtml5',
            'excelHtml5',
            'csvHtml5',
            'pdfHtml5'
        ],
        "aoColumnDefs": [{
        targets: [0],
        visible: false
        }],
            rowGroup: {
            dataSrc: function (row) {
            return '<span class="group" style="color:#008080;" id="' + row.id + '" >' + row.name + '</span>';
        }
        },
        "aoColumns": [
                    {"mData": "name" },
                    {"mData": "year" },
                    { "mData": "monthName" },
                    { "mData": "totalCost" },
                    { "mData": "totalRevenue" }
        ]
    });

    $('#tblReport tbody').on('click', 'tr.dtrg-group', function () {

        var practitionerId = $('td span.group',this).attr("id");
        var dateFrom = $('#dateFrom').val();
        var dateTo = $('#dateTo').val();
        var base_url = window.location.origin;
        window.location.href = base_url + "/Report/AppointmentHistory?id=" + practitionerId + "&dateFrom=" + dateFrom + "&dateTo=" + dateTo;
    });

});

function financialSummary() {
    var result;
    $.ajax({
        type: "GET",
        url: "/Report/GetFinancialSummary",
        cache: false,
        async: false,
        success: function (data) {
            //console.log(data);
            result = data;
        }
    });
    return result;
}

function filterReport() {

    // Initialization  
    var filterModel = new Object();
    filterModel.practitionerId = $('#Practitioner').val();
    filterModel.dateFrom = $('#dateFrom').val();
    filterModel.dateTo = $('#dateTo').val();

    if (filterModel.practitionerId != "") {
        $.ajax({
            type: "POST",
            cache: false,
            url: "/Report/FilterFinancialReport",
            data: JSON.stringify(filterModel),
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            success: function (response) {
                table.clear().rows.add(response).draw();
            },
            failure: function (response) {
                alert(response.responseText);
            },
            error: function (response) {
                alert(response.responseText);
            }
        });
    }
    else {
        alert("Please select a Practitioner");
    }
}