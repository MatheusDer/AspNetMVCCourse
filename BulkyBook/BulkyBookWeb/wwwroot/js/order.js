var dataTable;

$(document).ready(function () {
    var url = window.location.search;
    if (url.includes("inprocess")) {
        loadDataTable("inprocess");
        return;
    }
    if (url.includes("pending")) {
        loadDataTable("pending");
        return;
    }
    if (url.includes("completed")) {
        loadDataTable("completed");
        return;
    }
    if (url.includes("approved")) {
        loadDataTable("approved");
        return;
    }

    loadDataTable("all");
})

function loadDataTable(status) {
    dataTable = $('#myTable').DataTable({
        "ajax": {
            "url": "/Admin/Order/GetAll?status=" + status
        },
        "columns": [
            { "data": "id" },
            { "data": "name" },
            { "data": "phoneNumber" },
            { "data": "applicationUser.email" },
            { "data": "orderStatus" },
            { "data": "orderTotal" },
            {
                "data": "id",
                "render": function (data) {
                    return `
                        <div class="w-100 btn-group" role="group">
							<a href="/Admin/Order/Details?orderId=${data}" class="btn btn-primary mx-2 rounded w-100"><i class="bi bi-folder-symlink"></i></a>
						</div>
                        `
                }
            },
        ]
    });
};
