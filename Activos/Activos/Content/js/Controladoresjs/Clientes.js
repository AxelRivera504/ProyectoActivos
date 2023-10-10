
$("#clie_RTNCliente").focusout(function () {
    var texto = $('#clie_RTNCliente').val();
    console.log(texto);
    $.ajax({
        url: '/Clientes/ValidarIdentificacionExiste/',
        method: 'POST',
        data: { RTNCliente: texto },
        success: function (result) {
            if (result.success) {
                $("#lblError").removeAttr("hidden");
                $("#btnGuardar").attr("disabled");
            } else {
                $("#btnGuardar").removeAttr("disabled", "");
                $('#lblError').attr("hidden", "");
            }
        }
    });
});