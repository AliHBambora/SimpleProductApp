$(document).ready(function () {
    $('#ProductTable').DataTable();

    //On load of page get all the products via API call GetProductsList
    $.ajax({
        url: "/api/GetProductsList",
        type: 'POST',
        datatype: 'json',
        processData: false,
        contentType: false,
        headers: {
            Authorization: "Bearer ".concat(sessionStorage.getItem("token")),
        },
        success: function (data, textStatus, xhr) {
            if (data.status == "Success") {
                var products = data.products;
                var html = "";
                products.forEach(function (item) {
                    html += "<tr>";
                    html += "<td><img src=" + item.path + " style='height:40px;width:40px'></td>";
                    /*html += "<td>" + item.name + "</td>";*/
                    html += "<td>" + item.description + "</td>";
                    html += "<td>" + item.price + "</td>";
                    html += "<td>" + item.price + "</td>";
                    html += `<td><button type='button' class='btn btn-primary edit' tid='${item.id}'><i class='fa fa-pencil'></i></button></td>`;
                    html += `<td><button type='button' class='btn btn-danger delete' tid='${item.id}'><i class='fa fa-times'></i></button></td>`;
                    html += "</tr>"
                })

                $("#ProductTableBody").html(html);
                $("#AddnewProductModal").modal("hide");
            }
            else if (data.status == "No Products found") {

            }
            else {
                Swal.fire({
                    icon: 'error',
                    title: 'Error occured. Fetching products failed',
                })
            }
            console.log(data);
        },
        error: function (data, textStatus, xhr) {
            console.log(data);
        }
    });


    //When the user wants to add a new product then make API call for adding a new product
    $("#AddnewProductButton").click(function () {
        var ProductImg = $('input[name="Photo"]').get(0).files[0];
        var formdata = new FormData();
        formdata.append("Name", $("#Name").val());
        formdata.append("Description", $("#Description").val());
        formdata.append("Price", $("#Price").val());
        formdata.append("Photo", ProductImg);
        $.ajax({
            url: "/api/AddNewProduct",
            type: 'POST',
            datatype: 'json',
            processData: false,
            contentType: false,
            headers: {
                Authorization: "Bearer ".concat(sessionStorage.getItem("token")),
            },
            data: formdata,
            success: function (data, textStatus, xhr) {
                if (data.status == "Success") {
                    Swal.fire({
                        icon: 'success',
                        title: "Product successfully added"
                    });
                    console.log(data);
                    var html = "<tr>";
                    html += "<td><img src=" + data.newProduct.path + " style='height:40px;width:40px'></td>";
                    html += "<td>" + data.newProduct.name + "</td>";
                    html += "<td>" + data.newProduct.description + "</td>";
                    html += "<td>" + data.newProduct.price + "</td>";
                    html += `<td><button type='button' class='btn btn-primary edit' tid='${data.newProduct.id}'><i class='fa fa-pencil'></i></button></td>`;
                    html += `<td><button type='button' class='btn btn-danger delete' tid='${data.newProduct.id}'><i class='fa fa-times'></i></button></td>`;
                    html += "</tr>";
                    $("#ProductTableBody").append(html);
                    $("#AddnewProductModal").modal("hide");
                }
                else {
                    Swal.fire({
                        icon: 'error',
                        title: 'Error occured. Fetching products failed',
                    })
                }
                console.log(data);
            },
            error: function (data, textStatus, xhr) {
                console.log(data);
            }
        });
    });

    //When the user wants to edit an existing product
    $('body').on('click', '.edit', function () {
        $('#AddnewProductModal').modal('show');
        var formdata = new FormData();
        formdata.append("ProductID", $(this).attr("tid"));
        $.ajax({
            url: "/api/GetProductDetails",
            type: 'POST',
            datatype: 'json',
            processData: false,
            contentType: false,
            data: formdata,
            headers: {
                Authorization: "Bearer ".concat(sessionStorage.getItem("token")),
            },
            success: function (data, textStatus, xhr) {
                if (data.status == "Success") {
                    $("#Name").val(data.result.name);
                    $("#Description").val(data.result.description);
                    $("#Price").val(data.result.price);
                    $("#AddnewProductButton").click(function () {
                        var ProductImg = $('input[name="Photo"]').get(0).files[0];
                        var formdata = new FormData();
                        formdata.append("Name", $("#Name").val());
                        formdata.append("Description", $("#Description").val());
                        formdata.append("Price", $("#Price").val());
                        formdata.append("Photo", ProductImg);
                        formdata.append("ProductID", $(this).attr("tid"));
                        $.ajax({
                            url: "/api/EditProduct",
                            type: 'POST',
                            datatype: 'json',
                            processData: false,
                            contentType: false,
                            headers: {
                                Authorization: "Bearer ".concat(sessionStorage.getItem("token")),
                            },
                            data: formdata,
                            success: function (data, textStatus, xhr) {
                                if (data.status == "Success") {
                                    Swal.fire({
                                        icon: 'success',
                                        title: "Product successfully edited"
                                    });
                                    window.location.reload();
                                }
                                else {
                                    Swal.fire({
                                        icon: 'error',
                                        title: 'Error occured. Fetching products failed',
                                    })
                                }
                                console.log(data);
                            },
                            error: function (data, textStatus, xhr) {
                                console.log(data);
                            }
                        });
                    })
                }
                else {
                    Swal.fire({
                        icon: 'error',
                        title: 'Error occured. Fetching product failed',
                    })
                }
                console.log(data);
            },
            error: function (data, textStatus, xhr) {
                console.log(data);
            }
        });
    })


    //When the user wants to delete an existing product
    $('body').on('click', '.delete', function () {
        Swal.fire({
            icon: "question",
            title: "Are you sure you want to delete this product",
            showCloseButton: true,
            showCancelButton: true,
            confirmButtonText: "Yes",
            cancelButtonText: "no"
        }).then((res) => {
            if (res.isConfirmed) {
                var formdata = new FormData();
                formdata.append("ProductID", $(this).attr("tid"));
                $.ajax({
                    url: "/api/DeleteProduct",
                    type: 'POST',
                    datatype: 'json',
                    processData: false,
                    contentType: false,
                    headers: {
                        Authorization: "Bearer ".concat(sessionStorage.getItem("token")),
                    },
                    data: formdata,
                    success: function (data, textStatus, xhr) {
                        if (data.status == "Success") {
                            Swal.fire({
                                icon: 'success',
                                title: 'Deleted successfully',
                            });
                            window.location.reload();
                        }
                        else {
                            Swal.fire({
                                icon: 'error',
                                title: 'Error occured. Fetching product failed',
                            })
                        }
                        console.log(data);
                    },
                    error: function (data, textStatus, xhr) {
                        console.log(data);
                    }
                });
            }
        })
    })
});

