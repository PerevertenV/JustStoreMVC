var dataTable;

$(document).ready(function ()
{
    var url = window.location.search;
    if (url.includes("inprocess"))
    {
        loadDataTable("inprocess");
    }
    else
    {
        if (url.includes("completed"))
        {
            loadDataTable("completed");
        }
        else
        {
            if (url.includes("pending"))
            {
                loadDataTable("pending");
            }
            else
            {
                if (url.includes("approved"))
                {
                    loadDataTable("approved");
                }
                else
                {
                    loadDataTable("all");
                }
            }
        }
    }
});

function loadDataTable(status)
{
    dataTable = $('#tblData').DataTable(
        {
            "ajax": { url: '/admin/order/getall?status=' + status },
        "columns": [
            {
                data: 'id',
                "width": "5%",
                "createdCell": function (td, cellData, rowData, row, col) {
                    $(td).css('color', 'white');
                }
            },
            {
                data: 'name',
                "width": "15%",
                "createdCell": function (td, cellData, rowData, row, col) {
                    $(td).css('color', 'white');
                }
            },
            {
                data: 'phoneNumber',
                "width": "15%",
                "createdCell": function (td, cellData, rowData, row, col) {
                    $(td).css('color', 'white');
                }
            },
            {
                data: 'applicationUser.email',
                "width": "20%",
                "createdCell": function (td, cellData, rowData, row, col) {
                    $(td).css('color', 'white');
                }
            },
            {
                data: 'orderStatus',
                "width": "15%",
                "createdCell": function (td, cellData, rowData, row, col) {
                    $(td).css('color', 'white');
                }
            },
            {
                data: 'orderTotal',
                "width": "15%",
                "createdCell": function (td, cellData, rowData, row, col) {
                    $(td).css('color', 'white');
                }
            },
            {
                data: 'id',
                "render": function (data) {
                    return `<div class="w-75 btn-group" role="group">
                        <a href="/admin/order/details?orderid=${data}" class="btn btn-primary mx-2"> <i class="bi bi-pen"></i></a>
                    </div>`
                },
                "width": "15%"
            }
        ]
    });
}