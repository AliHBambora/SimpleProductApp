$(document).ready(function () {
    $("#submit").click(function () {
        var formdata = new FormData();
        formdata.append('USERNAME', $("#USERNAME").val());
        formdata.append('PASSWORD', $("#PASSWORD").val());
        $.ajax({
            url: "/api/Login",
            type: 'POST',
            datatype: 'json',
            processData: false,
            contentType: false,
            data: formdata,
            success: function (data, textStatus, xhr) {
                if (data.status == "Success") {
                    Swal.fire({
                        icon: 'success',
                        title: 'Logged in successfully',
                    })
                    sessionStorage.setItem("token", data.token);
                    window.location.href = "Product/ProductsList";
                }
                else {
                    Swal.fire({
                        icon: 'error',
                        title: 'Authentication failed',
                    })
                }
                console.log(data);
            },
            error: function (data, textStatus, xhr) {
                console.log(data);
            }
        });
    })

    
})