﻿var dataTable;

$(document).ready(function () {
    loadDataTable();

})

function loadDataTable() {

    dataTable = $('#tblData').DataTable({
        "ajax": {
            "url": "/Admin/Categoery/GetAll"


        },
        "columns": [

            { "data": "name", "width": "70%" },
            {
                "data": "id",
                "render": function (data) {
                    return `
<div class="text-center">
<a href ="/Admin/Categoery/Upsert/${data}" class="btn btn-info">
<i class="fas fa-edit"></i>
</a>
<a class="btn btn-danger" onclick =Delete('/Admin/Categoery/Delete/${data}')>
<i class="fas fa-trash-alt"></i>
</a>

</div>


`;
                }

            }

        ]

    })

}

function Delete(url) {
    /*alert(url);*/
    swal({
        title: "Want to delete data",
        text: "Delete Information",
        icon: "warning",
        buttons: true,
        dangerModel:true
    }).then((willDelete) => {
        if (willDelete) {

            $.ajax({
                url: url,
                type: "DELETE",
                success: function (data) {

                    if (data.success) {
                        toastr.success(data.message);
                        dataTable.ajax.reload();

                    }
                    else {
                        toastr.error(data.message);
                       

                    }
                }

                })

        }

    })
}