var dataTable;

$(document).ready(function () {
    loadDataTable();
});

function loadDataTable() {
    dataTable = $('#tblData').DataTable({
        "ajax": { url: '/admin/user/getall' },
        "columns": [
            {
                data: 'name',
                "width": "15%",
                "createdCell": function (td, cellData, rowData, row, col) {
                    $(td).css('color', 'white');
                }
            },
            {
                data: 'email',
                "width": "20%",
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
                data: 'company.name',
                "width": "10%",
                "createdCell": function (td, cellData, rowData, row, col) {
                    $(td).css('color', 'white');
                }
            },
            {
                data: 'role',
                "width": "10%",
                "createdCell": function (td, cellData, rowData, row, col) {
                    $(td).css('color', 'white');
                }
            },
            {
                data: null,
                "render": function (data, type, row) {
                    var today = new Date().getTime();
                    var lockout = new Date(row.lockoutEnd).getTime();

                    if (lockout > today) {
                        return `
                            <div class="text-center">
                                <a onclick=LockUnlock('${row.id}') class="btn btn-danger text-white" style="cursor: pointer; width: 100px;"> 
                                    <i class="bi bi-lock-fill"></i> Lock
                                </a>
                                <a href="/admin/user/RoleManagment?userId=${row.id}" class="btn btn-danger text-white" style="cursor: pointer; width: 170px;">
                                    <i class="bi bi-pencil-square"></i> Permission
                                </a>
                            </div>
                        `;
                    } else {
                        return `
                            <div class="text-center">
                                <a onclick=LockUnlock('${row.id}') class="btn btn-success text-white" style="cursor: pointer; width: 100px;"> 
                                    <i class="bi bi-unlock-fill"></i> Unlock
                                </a>
                                <a href="/admin/user/RoleManagment?userId=${row.id}" class="btn btn-danger text-white" style="cursor: pointer; width: 170px;">
                                    <i class="bi bi-pencil-square"></i> Permission
                                </a>
                            </div>
                        `;
                    }
                },
                "width": "30%"
            }
        ]
    });
}

function LockUnlock(id) {
    $.ajax({
        type: "POST",
        url: '/Admin/User/LockUnlock',
        data: JSON.stringify(id),
        contentType: "application/json",
        success: function (data)
        {
            if (data.success)
            {
                toastr.success(data.message);
                dataTable.ajax.reload();
            }
            else
            {
                toastr.error('Не вдалося виконати дію');
            }
        }
    });
}

