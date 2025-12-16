# ?? Envío de Mensajes de WhatsApp

## Descripción General

Esta funcionalidad permite enviar recordatorios de pago y mensajes personalizados a los clientes a través de WhatsApp directamente desde la aplicación.

## Características Implementadas

### 1. **Tres Tipos de Mensajes**

#### ?? Recordatorios Automáticos
- Se envía un recordatorio automático a todos los clientes con pagos programados para hoy o mañana
- El mensaje incluye:
  - Nombre del cliente
  - Monto del pago
  - Fecha programada
  - Mensaje de cortesía
- Vista previa del mensaje antes de enviar
- Contador de mensajes a enviar

#### ?? Mensaje Individual
- Selecciona un cliente de la lista
- Escribe un mensaje personalizado
- Envío directo a WhatsApp del cliente seleccionado

#### ?? Mensaje Masivo
- Envía un mensaje personalizado a todos los clientes con pagos próximos
- El mensaje se personaliza con el nombre de cada cliente
- Ideal para notificaciones generales o cambios importantes

### 2. **Vista Previa de Mensajes**
- Muestra cómo se verá el mensaje antes de enviarlo
- Para recordatorios automáticos, se muestra un ejemplo del mensaje

### 3. **Validaciones**
- Verifica que los clientes tengan número de teléfono registrado
- Limpia y formatea los números de teléfono automáticamente
- Desactiva el botón de envío si no hay destinatarios válidos

### 4. **Interfaz Intuitiva**
- Diseño con Material Design
- Botones de selección visual para cada tipo de mensaje
- Colores diferenciados por tipo de mensaje:
  - **Recordatorios**: Amarillo (#FFF9C4)
  - **Individual**: Verde WhatsApp (#25D366)
  - **Masivo**: Gris (#757575)
- Íconos y emojis descriptivos

## Arquitectura

### Archivos Creados

1. **Pages/EnviarMensajesPage.xaml**
   - Interfaz de usuario de la página

2. **Pages/EnviarMensajesPage.xaml.cs**
   - Code-behind de la página

3. **ViewModels/EnviarMensajesViewModel.cs**
   - Lógica de negocio
   - Gestión de estado
   - Comandos de envío

4. **Services/WhatsAppService.cs**
   - Servicio encargado del envío de mensajes
   - Limpieza y formateo de números telefónicos
   - Integración con la API de WhatsApp

5. **Converters/TipoMensajeToColorConverter.cs**
   - Converter para cambiar colores de botones según selección

6. **Converters/InvertBoolConverter.cs**
   - Converter para invertir valores booleanos

### Flujo de Datos

```
EnviarMensajesPage ? EnviarMensajesViewModel ? WhatsAppService
                                              ?
                                        DatabaseService
```

## Cómo Usar

### Desde la Aplicación

1. **Acceder a Mensajes**
   - Desde el Dashboard, toca el card "Mensajes"
   - O navega directamente a la página de mensajes

2. **Seleccionar Tipo de Mensaje**
   - Toca uno de los tres botones en la parte superior
   - La interfaz cambiará según el tipo seleccionado

3. **Enviar Recordatorios Automáticos**
   - Selecciona "Recordatorios"
   - Revisa la vista previa del mensaje
   - Verifica el contador de mensajes a enviar
   - Toca "Enviar a Todos"
   - Confirma el envío
   - Se abrirá WhatsApp para cada cliente

4. **Enviar Mensaje Individual**
   - Selecciona "Individual"
   - Elige un cliente del selector
   - Escribe tu mensaje personalizado
   - Toca "Enviar por WhatsApp"
   - Se abrirá WhatsApp con el mensaje listo

5. **Enviar Mensaje Masivo**
   - Selecciona "Masivo"
   - Escribe tu mensaje (se personalizará con el nombre de cada cliente)
   - Revisa el contador de destinatarios
   - Toca "Enviar a Todos"
   - Confirma el envío

## Integración con WhatsApp

### Cómo Funciona

La aplicación utiliza la **API de WhatsApp Web** mediante el esquema de URL:

```
https://wa.me/{NUMERO}?text={MENSAJE}
```

- **NUMERO**: El número de teléfono del destinatario (sin espacios, guiones ni el símbolo +)
- **MENSAJE**: El texto del mensaje codificado para URL

### Requisitos

- El dispositivo debe tener WhatsApp instalado
- El número de teléfono debe estar en formato internacional
- Requiere conexión a internet

### Limitaciones

- WhatsApp se abre en el navegador o aplicación
- El usuario debe presionar "Enviar" manualmente en cada conversación
- Hay un pequeño delay entre mensajes (500ms) para evitar problemas

## Configuración de Números de Teléfono

### Formato Recomendado

Los números de teléfono en la base de datos deben estar en formato internacional:

```
Venezuela: +58 414 1234567 o 584141234567
Otro país: +[código][número]
```

### Limpieza Automática

El `WhatsAppService` limpia automáticamente:
- Espacios
- Guiones (-)
- Paréntesis ( )
- Puntos (.)
- Mantiene solo dígitos y el símbolo +

## Datos Utilizados

### Base de Datos

La funcionalidad utiliza:

1. **Tabla Clientes**
   - `NombreCompleto`: Para personalizar mensajes
   - `Telefono`: Para enviar mensajes

2. **Tabla Pagos**
   - `FechaProgramada`: Para filtrar pagos próximos
   - `Estado`: Solo se envían recordatorios de pagos "Pendientes"
   - `MontoPago`: Para incluir en el mensaje
   - `ClienteId`: Para relacionar con el cliente

### Consultas

```csharp
// Obtener pagos de hoy
var pagosHoy = await _databaseService.GetPagosByFechaAsync(DateTime.Today);

// Obtener pagos de mañana
var pagosManana = await _databaseService.GetPagosByFechaAsync(DateTime.Today.AddDays(1));

// Filtrar solo pendientes
var pagosPendientes = pagos.Where(p => p.Estado == "Pendiente");
```

## Mensajes Generados

### Recordatorio Automático

```
Hola [Nombre del Cliente],

Te recordamos que tienes un pago programado para [hoy/mañana].

?? Monto: $[monto]
?? Fecha: [DD/MM/YYYY]

Gracias por tu puntualidad.
```

### Mensaje Individual

El texto que escribas se envía directamente sin modificaciones.

### Mensaje Masivo

```
Hola [Nombre del Cliente],

[Tu mensaje personalizado]
```

## Seguridad y Privacidad

- ? **No se envían mensajes automáticamente**: El usuario debe confirmar cada envío masivo
- ? **Confirmación antes de enviar**: Se muestra un diálogo de confirmación
- ? **Control del usuario**: WhatsApp se abre, pero el usuario debe presionar "Enviar"
- ? **Sin almacenamiento de conversaciones**: No se guardan las conversaciones
- ? **Privacidad**: Solo el usuario de la app tiene acceso a los números

## Futuras Mejoras

Posibles mejoras para el futuro:

1. **Plantillas de Mensajes**
   - Guardar mensajes frecuentes
   - Variables personalizables ({nombre}, {monto}, etc.)

2. **Programación de Mensajes**
   - Enviar mensajes en una fecha/hora específica
   - Recordatorios automáticos diarios

3. **Historial de Mensajes**
   - Registrar qué mensajes se enviaron
   - Fecha y hora de envío
   - Estado del mensaje

4. **Estadísticas**
   - Mensajes enviados por mes
   - Tasa de respuesta
   - Efectividad de recordatorios

5. **Integración con API de WhatsApp Business**
   - Envío automático real
   - Confirmaciones de lectura
   - Respuestas automáticas

## Solución de Problemas

### El botón "Enviar" está deshabilitado

**Causas posibles:**
- No hay clientes con pagos próximos (Recordatorios/Masivo)
- No has seleccionado un cliente (Individual)
- No has escrito un mensaje (Individual/Masivo)
- Los clientes no tienen número de teléfono registrado

**Solución:**
- Verifica que los clientes tengan números de teléfono
- Asegúrate de tener pagos programados para hoy o mañana
- Completa todos los campos requeridos

### WhatsApp no se abre

**Causas posibles:**
- WhatsApp no está instalado
- El número de teléfono está mal formateado
- No hay conexión a internet

**Solución:**
- Instala WhatsApp en el dispositivo
- Verifica el formato del número de teléfono
- Verifica la conexión a internet

### El mensaje no se personaliza

**Causas posibles:**
- Los clientes no tienen nombre registrado en la base de datos

**Solución:**
- Asegúrate de que todos los clientes tengan `NombreCompleto` en la base de datos

## Dependencias

### NuGet Packages

- `sqlite-net-pcl`: Para acceso a la base de datos
- `.NET MAUI`: Framework principal

### Permisos

No se requieren permisos especiales ya que usa el navegador/app de WhatsApp instalada.

## Testing

### Casos de Prueba

1. **Envío Individual**
   - ? Enviar mensaje a cliente con número válido
   - ? Intentar enviar sin seleccionar cliente
   - ? Intentar enviar sin escribir mensaje

2. **Envío Masivo**
   - ? Enviar a múltiples clientes
   - ? Verificar personalización de nombres
   - ? Confirmar delay entre mensajes

3. **Recordatorios**
   - ? Enviar recordatorios con pagos próximos
   - ? Verificar filtrado por fecha
   - ? Verificar solo pagos pendientes

4. **Validaciones**
   - ? Números de teléfono inválidos
   - ? Clientes sin teléfono
   - ? Sin pagos próximos

## Navegación

### Rutas Registradas

```csharp
Routing.RegisterRoute("mensajes", typeof(EnviarMensajesPage));
```

### Navegar desde código

```csharp
await Shell.Current.GoToAsync("mensajes");
```

### Desde Dashboard

Toca el card "Mensajes" ? "Enviar recordatorios"

---

**Desarrollado para App_CrediVnzl**  
**Versión**: 1.0  
**Última actualización**: Enero 2025
