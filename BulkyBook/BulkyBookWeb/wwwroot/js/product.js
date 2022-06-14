var dataTable;

$(document).ready(function () {
    $('#myTable').DataTable({
        "ajax": {
            "url": "/Admin/Product/GetAll"
        },
        "columns": [
            { "data": "title"},
            { "data": "isbn"},
            { "data": "price"},
            { "data": "author"},
            { "data": "category.name"},
            {
                "data": "id",
                "render": function (data) {
                    return `
                        <div class="w-100 btn-group" role="group">
							<a href="/Admin/Product/Upsert?id=${data}" class="btn btn-primary mx-2 rounded w-100"><i class="bi bi-pencil-square"></i> Edit</a>
							<a href="/Admin/Product/Delete?id=${data}" class="btn btn-danger mx-2 rounded w-100"><i class="bi bi-file-earmark-x"></i> Delete</a>
						</div>
                        `
                }
            },
        ]
    });
});