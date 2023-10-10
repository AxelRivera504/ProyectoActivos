
function Limpiar() {
    setTimeout(() => {
        $("#txtNombre").val("");
        $("#txtApellido").val("");
        $("#txtUsua").val("");
        $("#txtPassword").val("");
        LimpiarLb();
    }, "500");
   
    $('#modal-default').modal("hide");
}

function LimpiarLb() {
    $('#lb1').attr("hidden", "");
    $('#lb2').attr("hidden", "");
    $('#lb3').attr("hidden", "");
    $('#lb4').attr("hidden", "");
}

function LimpiarLbEdit() {
    $('#lb1Edit').attr("hidden", "");
    $('#lb2Edit').attr("hidden", "");
    $('#lb3Edit').attr("hidden", "");
}

function Validar() {
    var isValid = true;
    var Nombre = $('#txtNombre').val();
    var Apellido = $('#txtApellido').val();
    var Usuario = $('#txtUsua').val();
    var Contra = $('#txtPassword').val();
    var mensaje = "";
    if (Nombre == "") {
        isValid = false;
        mensaje += "Nombre.\n";
        $('#lb1').removeAttr("hidden");
    }
    if (Nombre != "") {                                                                                                         
        $('#lb1').attr("hidden", "");
    }
    if (Apellido == "") {
        isValid = false;
        mensaje += "Apellido.\n";
        $('#lb2').removeAttr("hidden");
    }
    if (Apellido != "") {
        $('#lb2').attr("hidden", "");
    }
    if (Usuario == "") {
        isValid = false;
        mensaje += "Usuario.\n";
        $('#lb3').removeAttr("hidden");
    }
    if (Usuario != "") {
        $('#lb3').attr("hidden", "");
    }
    if (Contra == "") {
        isValid = false;
        mensaje += "Contraseña.\n";
        $('#lb4').removeAttr("hidden");
    }
    if (Contra != "") {
        $('#lb4').attr("hidden", "");
    }

    if (isValid) {
        AgregarRegistro(Nombre, Apellido, Usuario, Contra);
    } else {
        MostrarMensajeWarning('Faltan los siguientes campos por llenar:\n' + mensaje);
    }
}

function AgregarRegistro(Nombre, Apellido, Usuario, contra) {
    LimpiarLb();
    $.ajax({
        url: '/Usuarios/Create/',
        type: 'POST',
        data: { usua_UsuarioNombre: Nombre, usua_UsuarioApellido: Apellido, usua_Usuario: Usuario, usua_Contra: contra },
        success: function (result) {
            if (result.success && result.correcto) {
                localStorage.setItem("Agregar", 1);
                window.location.reload();
            } if (result.success == false && result.correcto == false) {
                $('#lb3').removeAttr("hidden");
                $('.toastrDefaultSuccess').click(function () {
                    toastr.success('El nombre de usuario que desea digitar ya existe');
                });
            }
        }
    });
}

function cargarDataEditar(usua_Id, UsuarioMod) {
    $.ajax({
        url: '/Usuarios/CargarDatosEditar/',
        type: 'POST',
        data: { id: usua_Id },
        success: function (result) {
            $('#txtNombreEdit').val(result.usuaNombre);
            $('#txtApellidoEdit').val(result.usuaApellido);
            $('#txtUsuaEdit').val(result.Usuario);
            $('#txtUsuaIdEdit').val(result.Usua_Id);
            $('#modal-Edit').modal("show");
        }
    });
}

function ValidarEditar() {
    var isValid = true;
    var Nombre = $('#txtNombreEdit').val();
    var Apellido = $('#txtApellidoEdit').val();
    var Usuario = $('#txtUsuaEdit').val();
    var usua_Id = $('#txtUsuaIdEdit').val();
    var mensaje = "";
    if (Nombre == "") {
        isValid = false;
        mensaje += "Nombre.\n";
        $('#lb1Edit').removeAttr("hidden");
    }
    if (Nombre != "") {
        $('#lb1Edit').attr("hidden", "");
    }
    if (Apellido == "") {
        isValid = false;
        mensaje += "Apellido.\n";
        $('#lb2Edit').removeAttr("hidden");
    }
    if (Apellido != "") {
        $('#lb2Edit').attr("hidden", "");
    }
    if (Usuario == "") {
        isValid = false;
        mensaje += "Usuario.\n";
        $('#lb3Edit').removeAttr("hidden");
    }
    if (Usuario != "") {
        $('#lb3Edit').attr("hidden", "");
    }
   
    if (isValid) {
        EditarRegistro(usua_Id, Nombre, Apellido);
        localStorage.setItem("Editar", 1);
    } else {
        MostrarMensajeWarning('Faltan los siguientes campos por llenar:' + mensaje);
    }
}

function EditarRegistro(usua_Id,Nombre, Apellido, Usuario) {
    LimpiarLb();
    $.ajax({
        url: '/Usuarios/Editar/',
        type: 'POST',
        data: { usua_Id: usua_Id,usua_UsuarioNombre: Nombre, usua_UsuarioApellido: Apellido },
        success: function (result) {
            if (result.success && result.correcto) {                            
                LimpiarLbEdit();
                localStorage.setItem("Editar", 1);
                window.location.reload();
            } if (result.success == false && result.correcto == false) {
                $('#lb3').removeAttr("hidden");
                $('.toastrDefaultSuccess').click(function () {
                    toastr.success('El nombre de usuario que desea digitar ya existe');
                });
            }
        }
    });
}

function cargarData(usua_Id, usua_Login) {
    var numero = usua_Id;
    var numero1 = usua_Login;
    if (numero == numero1) {
        console.log("No se puede");
        MostrarMensajeWarning('No puede desactivar este usuario porque esta siendo utilizado en este momento');
    } else {
        $('#modal-Disabled').modal("show");
        var id = numero.toString();
        $('#txtUsuaId').text(id);
    }
}

function cargarDataHabilitar(usua_Id) {
    var numero = usua_Id;
    var id = numero.toString();
    $('#txtUsuaIdHabilitar').text(id);
}

function ModalDeshabilitar() {
    var usua_Id = $('#txtUsuaId').text();
    $.ajax({
        url: '/Usuarios/Deshabilitar/',
        type: 'POST',
        data: { id: usua_Id },
        success: function (result) {
            if (result.success && result.correcto) {
                localStorage.setItem("Deshabilitar", 1);
                window.location.reload();
            } if (result.success == false && result.correcto == false) {
                console.log("Toast de error");
            }
        }
    });
}

function ModalHabilitar() {
    var usua_Id = $('#txtUsuaIdHabilitar').text();
    $.ajax({
        url: '/Usuarios/Habilitar/',
        type: 'POST',
        data: { id: usua_Id },
        success: function (result) {
            if (result.success && result.correcto) {
                localStorage.setItem("Habilitar", 1);
                window.location.reload();
            } if (result.success == false && result.correcto == false) {
                console.log("Toast de error");
            }
        }
    });
}

function activarToast() {
    console.log("toast")
    
}