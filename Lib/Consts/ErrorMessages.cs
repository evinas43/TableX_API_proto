namespace TablexAPI.Lib.Consts;

public static class ErrorMessages
{
    public const string MustBeLoggedIn = "Debes iniciar sesión para realizar esta acción.";
    public const string ForbiddenAction = "No tienes permiso para realizar esta acción.";
    public const string ValidationFailed = "La petición contiene datos no válidos.";
    public const string InternalServerError = "Se ha producido un error inesperado.";
    public const string ConstraintViolation = "La operación viola una restricción de la base de datos.";

    public const string InvalidCredentials = "Usuario o contraseña incorrectos.";
    public const string UsernameAlreadyExists = "Ya existe un usuario con ese nombre.";
    public const string UserNotFound = "Usuario no encontrado.";

    public const string MesaNotFound = "Mesa no encontrada.";
    public const string MesaNumeroAlreadyExists = "Ya existe una mesa con ese número.";
    public const string MesaHasPedidos = "No se puede eliminar una mesa que tiene pedidos registrados.";
    public const string MesaHasOpenPedido = "La mesa ya tiene un pedido abierto.";
    public const string MesaWithoutOpenPedido = "La mesa no tiene ningún pedido abierto.";

    public const string ProductoNotFound = "Producto no encontrado.";
    public const string ProductoInUse = "No se puede eliminar un producto que aparece en pedidos.";
    public const string InvalidProductoType = "Tipo de producto no válido. Valores permitidos: comestible, bebida.";

    public const string PedidoNotFound = "Pedido no encontrado.";
    public const string PedidoClosed = "El pedido está cerrado y no admite cambios.";
    public const string PedidoHasPendingPayments = "No se puede cerrar el pedido: quedan líneas pendientes de pago.";
    public const string DetalleNotFound = "Línea de pedido no encontrada.";
    public const string DetalleAlreadyPaid = "La línea ya está pagada y no se puede modificar ni eliminar.";

    public const string NothingToPay = "No hay importe pendiente que cobrar en las líneas indicadas.";
    public const string DetallesNotInPedido = "Alguna de las líneas indicadas no pertenece al pedido.";
    public const string DetallesAlreadyPaid = "Alguna de las líneas indicadas ya está pagada.";
}
