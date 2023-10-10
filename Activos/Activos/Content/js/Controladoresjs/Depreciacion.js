function CargarDataActivo(acvo_Id) {
    $('#txtacov_Id').val(acvo_Id);
}


function BajarActivo() {
    var activo = $('#txtacov_Id').val();
    $.ajax({
        url: '/DepreciacionActivos/BajarActivo/',
        type: 'POST',
        data: { acvo_Id: activo },
        success: function (result) {
            if (result.activoBajado) {
                localStorage.setItem("Activo", 1);
                window.location.reload();
            } else {
                MostrarMensajeWarning("Hubo un error");
            }
        }
    });
}