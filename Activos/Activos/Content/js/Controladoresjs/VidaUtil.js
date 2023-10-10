function Limpiar() {
    setTimeout(() => {
        $("#txtDescripcion").val("");
        $("#txtVidaUtil").val("");
        $("#txtObjeto").val("");
        LimpiarLb();
    }, "500");

    $('#modal-default').modal("hide");
}

function LimpiarLb() {
    $('#lb1').attr("hidden", "");
    $('#lb2').attr("hidden", "");
    $('#lb3').attr("hidden", "");
}

function LimpiarLbEdit() {
    $('#lb1Edit').attr("hidden", "");
    $('#lb2Edit').attr("hidden", "");
    $('#lb3Edit').attr("hidden", "");
}


function Validar() {
    var isValid = true;
    var Objeto = $('#txtObjeto').val();
    var VidaUtil = $('#txtVidaUtil').val();
    var Descripcion = $('#txtDescripcion').val();
    var mensaje = "";
    if (Objeto == "") {
        isValid = false;
        mensaje += "Objeto.\n";
        $('#lb1').removeAttr("hidden");
    }
    if (Objeto != "") {
        $('#lb1').attr("hidden", "");
    }
    if (VidaUtil == "") {
        isValid = false;
        mensaje += "Vida Util.\n";
        $('#lb2').removeAttr("hidden");
    }
    if (VidaUtil != "") {
        $('#lb2').attr("hidden", "");
    }
    if (Descripcion == "") {
        isValid = false;
        mensaje += "Descripcion.\n";
        $('#lb3').removeAttr("hidden");
    }
    if (Descripcion != "") {
        $('#lb3').attr("hidden", "");
    }
    if (isValid) {
        AgregarRegistro(Objeto, Descripcion, VidaUtil);
    } else {
        MostrarMensajeWarning('Faltan los siguientes campos por llenar:\n' + mensaje);
    }
}


function AgregarRegistro(Objeto, Descripcion, VidaUtil) {
    LimpiarLb();
    $.ajax({
        url: '/VidaUtil/Create/',
        type: 'POST',
        data: { Objeto: Objeto, Descripcion: Descripcion, VidaUtil: VidaUtil },
        success: function (result) {
            if (result.success && result.correcto == 1) {
                localStorage.setItem("AgregarVida", 1);
                window.location.reload();
             } if (result.success == false && result.correcto == 2) {
                $('#lb1').removeAttr("hidden");
                $('#lb2').removeAttr("hidden");
                MostrarMensajeWarning("Ya existe una regsitro con el mismo objeto y vida util");
            } if (result.success == false && result.correcto == 0) {
                MostrarMensajeDanger("Hubo un error");
            }
        }
    });
}



function cargarDataEditar(viut_Id) {
    $.ajax({
        url: '/VidaUtil/CargarDatosEditar/',
        type: 'POST',
        data: { id: viut_Id },
        success: function (result) {
            $('#txtVidaUtilIdEdit').val(result.viut_Id);
            $('#txtDescripcionEdit').val(result.Descripcion);
            $('#txtObjetoEdit').val(result.Objeto);
            $('#txtVidaUtilEdit').val(result.vidaUtil);
            $('#modal-EditVida').modal("show");
        }
    });
}


function ValidarEditar() {
    var isValid = true;
    var Objeto = $('#txtObjetoEdit').val();
    var VidaUtil = $('#txtVidaUtilEdit').val();
    var Descripcion = $('#txtDescripcionEdit').val();
    var VidaUtilId = $('#txtVidaUtilIdEdit').val();
    var mensaje = "";
    if (Objeto == "") {
        isValid = false;
        mensaje += "Objeto.\n";
        $('#lb1Edit').removeAttr("hidden");
    }
    if (Objeto != "") {
        $('#lb1Edit').attr("hidden", "");
    }
    if (VidaUtil == "") {
        isValid = false;
        mensaje += "Vida Util.\n";
        $('#lb2Edit').removeAttr("hidden");
    }
    if (VidaUtil != "") {
        $('#lb2Edit').attr("hidden", "");
    }
    if (Descripcion == "") {
        isValid = false;
        mensaje += "Descripcion.\n";
        $('#lb3Edit').removeAttr("hidden");
    }
    if (Descripcion != "") {
        $('#lb3Edit').attr("hidden", "");
    }

    if (isValid) {
        EditarRegistro(Objeto, Descripcion, VidaUtil, VidaUtilId);
        localStorage.setItem("Editar", 1);
    } else {
        MostrarMensajeWarning('Faltan los siguientes campos por llenar:' + mensaje);
    }
}


function EditarRegistro(Objeto, Descripcion, VidaUtil, VidaUtilId) {
    LimpiarLb();
    $.ajax({
        url: '/VidaUtil/Editar/',
        type: 'POST',
        data: { Objeto: Objeto, Descripcion: Descripcion, VidaUtil: VidaUtil, VidaUtilId: VidaUtilId },
        success: function (result) {
            if (result.success && result.correcto) {
                LimpiarLbEdit();
                localStorage.setItem("EditarVida", 1);
                window.location.reload();
            } if (result.success == false && result.correcto == false) {
                MostrarMensajeWarning("Error al editar el registro");
            }
        }
    });
}