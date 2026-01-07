namespace App_CrediVnzl
{
    /// <summary>
    /// Clase centralizada con emojis seguros para usar en toda la aplicación.
    /// Estos emojis están probados para funcionar correctamente en Android, iOS, Windows y macOS.
    /// </summary>
    public static class Emojis
    {
        // USUARIOS Y PERSONAS
        public const string Usuario = "??";
        public const string UsuarioGrupo = "??";
        public const string Administrador = "?????";
        public const string Cliente = "??";
        public const string Personas = "???????????";

        // DINERO Y FINANZAS
        public const string Dinero = "??";
        public const string DineroBolsa = "??";
        public const string DineroAlas = "??";
        public const string Billete = "??";
        public const string Moneda = "??";
        public const string TarjetaCredito = "??";
        public const string Banco = "??";
        public const string Grafico = "??";
        public const string GraficoAscendente = "??";
        public const string GraficoDescendente = "??";

        // ESTADO Y ACCIONES
        public const string Check = "?";
        public const string Cruz = "?";
        public const string Advertencia = "??";
        public const string Prohibido = "??";
        public const string Candado = "??";
        public const string CandadoAbierto = "??";
        public const string Llave = "??";
        public const string Estrella = "?";
        public const string EstrellaRellena = "??";
        public const string Fuego = "??";

        // NAVEGACIÓN
        public const string Casa = "??";
        public const string Flecha = "??";
        public const string FlechaIzquierda = "??";
        public const string FlechaArriba = "??";
        public const string FlechaAbajo = "??";
        public const string Menu = "?";
        public const string Buscar = "??";
        public const string Lupa = "??";

        // CALENDARIO Y TIEMPO
        public const string Calendario = "??";
        public const string Reloj = "?";
        public const string RelojArena = "?";
        public const string Cronometro = "??";
        public const string Campana = "??";
        public const string Alarma = "?";

        // DOCUMENTOS Y ARCHIVOS
        public const string Documento = "??";
        public const string Carpeta = "??";
        public const string Portapapeles = "??";
        public const string Libro = "??";
        public const string Libreta = "??";
        public const string Etiqueta = "???";

        // COMUNICACIÓN
        public const string Mensaje = "??";
        public const string Telefono = "??";
        public const string Correo = "??";
        public const string Email = "??";
        public const string WhatsApp = "??";
        public const string Notificacion = "??";

        // EMOCIONES Y ESTADO
        public const string Feliz = "??";
        public const string Triste = "??";
        public const string Enojado = "??";
        public const string Pensando = "??";
        public const string Celebrar = "??";
        public const string Confeti = "??";
        public const string Regalo = "??";

        // CONFIGURACIÓN Y HERRAMIENTAS
        public const string Configuracion = "??";
        public const string Herramientas = "??";
        public const string Ajustes = "???";
        public const string Engranaje = "??";

        // INFORMACIÓN
        public const string Informacion = "??";
        public const string Pregunta = "?";
        public const string Exclamacion = "?";
        public const string Bombilla = "??";
        public const string Pin = "??";

        // ACCIONES COMUNES
        public const string Agregar = "?";
        public const string Eliminar = "?";
        public const string Editar = "??";
        public const string Guardar = "??";
        public const string Imprimir = "???";
        public const string Compartir = "??";
        public const string Descargar = "??";
        public const string Subir = "??";

        // CATEGORÍAS DE NEGOCIO
        public const string Prestamo = "??";
        public const string Pago = "??";
        public const string Interes = "??";
        public const string Capital = "??";
        public const string Ganancia = "??";
        public const string Perdida = "??";
        public const string Balance = "??";

        // ESTADOS DE PRÉSTAMO
        public const string Activo = "?";
        public const string Completado = "??";
        public const string Vencido = "??";
        public const string Pendiente = "?";
        public const string Cancelado = "?";

        // OTROS
        public const string Salir = "??";
        public const string Cerrar = "?";
        public const string Actualizar = "??";
        public const string Sincronizar = "??";
        public const string Ojo = "???";
        public const string OjoTachado = "??";
        public const string Corazon = "??";
        public const string CorazonRoto = "??";

        /// <summary>
        /// Obtiene un emoji según el estado del pago
        /// </summary>
        public static string GetEmojiPorEstadoPago(string estado)
        {
            return estado switch
            {
                "Pagado" => Activo,
                "Pendiente" => Pendiente,
                "Vencido" => Vencido,
                _ => Pregunta
            };
        }

        /// <summary>
        /// Obtiene un emoji según el estado del préstamo
        /// </summary>
        public static string GetEmojiPorEstadoPrestamo(string estado)
        {
            return estado switch
            {
                "Activo" => Activo,
                "Completado" => Completado,
                "Vencido" => Vencido,
                "Cancelado" => Cancelado,
                _ => Pregunta
            };
        }

        /// <summary>
        /// Obtiene un emoji de color según un valor porcentual
        /// </summary>
        public static string GetEmojiPorPorcentaje(int porcentaje)
        {
            return porcentaje switch
            {
                >= 80 => EstrellaRellena,
                >= 60 => Estrella,
                >= 40 => Advertencia,
                >= 20 => Triste,
                _ => Enojado
            };
        }
    }
}
