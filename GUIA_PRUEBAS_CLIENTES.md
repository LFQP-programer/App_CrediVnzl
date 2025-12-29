# ?? Guía de Pruebas - Módulo de Clientes (Modificar y Eliminar)

## ?? Checklist de Pruebas

### ? Pruebas Funcionales - Modificar Cliente

#### **Test 1: Modificar desde Lista de Clientes (Swipe)**
- [ ] Abrir la página de Clientes
- [ ] Deslizar un cliente hacia la izquierda
- [ ] Verificar que aparecen los botones "Editar" (azul) y "Eliminar" (rojo)
- [ ] Presionar "Editar"
- [ ] Verificar que se abre la página de Editar Cliente con datos pre-cargados
- [ ] Modificar el nombre del cliente
- [ ] Presionar "Guardar Cambios"
- [ ] Verificar mensaje de éxito
- [ ] Verificar que regresa a la lista de clientes
- [ ] Verificar que el nombre se actualizó en la lista

#### **Test 2: Modificar desde Detalle de Cliente**
- [ ] Abrir la página de Clientes
- [ ] Hacer tap en un cliente para abrir sus detalles
- [ ] Verificar que existe el botón "?? Modificar" (azul)
- [ ] Presionar el botón "Modificar"
- [ ] Verificar que se abre la página de Editar Cliente con datos pre-cargados
- [ ] Modificar el teléfono del cliente
- [ ] Presionar "Guardar Cambios"
- [ ] Verificar mensaje de éxito
- [ ] Verificar que regresa a la página de detalles
- [ ] Verificar que el teléfono se actualizó en los detalles

#### **Test 3: Validaciones en Edición**
- [ ] Abrir página de edición de un cliente
- [ ] Borrar el nombre completo
- [ ] Intentar guardar
- [ ] Verificar mensaje de error: "El nombre completo es requerido"
- [ ] Restaurar el nombre, borrar el teléfono
- [ ] Intentar guardar
- [ ] Verificar mensaje de error: "El teléfono es requerido"
- [ ] Restaurar el teléfono, borrar la cédula
- [ ] Intentar guardar
- [ ] Verificar mensaje de error: "La cédula/DNI es requerida"

#### **Test 4: Cancelar Edición**
- [ ] Abrir página de edición de un cliente
- [ ] Modificar algunos campos
- [ ] Presionar "Cancelar"
- [ ] Verificar que regresa sin guardar cambios
- [ ] Verificar que los datos originales se mantienen

---

### ? Pruebas Funcionales - Eliminar Cliente

#### **Test 5: Eliminar Cliente SIN Préstamos (desde Lista)**
- [ ] Crear un cliente nuevo sin préstamos
- [ ] En la lista de clientes, deslizar el cliente hacia la izquierda
- [ ] Presionar "Eliminar" (rojo)
- [ ] Verificar diálogo de confirmación simple
- [ ] Verificar mensaje: "¿Está seguro de eliminar a [Nombre]? Esta acción no se puede deshacer."
- [ ] Presionar "Cancelar"
- [ ] Verificar que el cliente NO se eliminó
- [ ] Repetir el proceso
- [ ] Presionar "Sí, eliminar"
- [ ] Verificar mensaje de éxito
- [ ] Verificar que el cliente desapareció de la lista

#### **Test 6: Eliminar Cliente CON Préstamos (desde Lista)**
- [ ] Seleccionar un cliente con préstamos activos
- [ ] Deslizar el cliente hacia la izquierda
- [ ] Presionar "Eliminar" (rojo)
- [ ] Verificar diálogo de advertencia extendido
- [ ] Verificar que muestra: número de préstamos activos
- [ ] Verificar que muestra: monto total de deuda
- [ ] Verificar advertencia de eliminación en cascada
- [ ] Presionar "Cancelar"
- [ ] Verificar que el cliente NO se eliminó
- [ ] Repetir el proceso
- [ ] Presionar "Sí, eliminar"
- [ ] Verificar mensaje de éxito
- [ ] Verificar que el cliente desapareció de la lista

#### **Test 7: Eliminar Cliente SIN Préstamos (desde Detalle)**
- [ ] Crear un cliente nuevo sin préstamos
- [ ] Abrir los detalles del cliente
- [ ] Presionar botón "??? Eliminar" (rojo)
- [ ] Verificar diálogo de confirmación simple
- [ ] Presionar "Cancelar"
- [ ] Verificar que permanece en la página de detalles
- [ ] Presionar nuevamente "Eliminar"
- [ ] Presionar "Sí, eliminar"
- [ ] Verificar mensaje de éxito
- [ ] Verificar que regresa a la lista de clientes
- [ ] Verificar que el cliente ya no está en la lista

#### **Test 8: Eliminar Cliente CON Préstamos (desde Detalle)**
- [ ] Seleccionar un cliente con préstamos activos
- [ ] Abrir los detalles del cliente
- [ ] Presionar botón "??? Eliminar" (rojo)
- [ ] Verificar diálogo de advertencia con datos de préstamos
- [ ] Presionar "Cancelar"
- [ ] Verificar que permanece en la página de detalles
- [ ] Presionar nuevamente "Eliminar"
- [ ] Presionar "Sí, eliminar"
- [ ] Verificar mensaje de éxito
- [ ] Verificar que regresa a la lista de clientes
- [ ] Verificar que el cliente ya no está en la lista

---

### ? Pruebas de Integridad de Datos

#### **Test 9: Verificar Eliminación en Cascada**
- [ ] Crear un cliente con préstamos activos y pagos registrados
- [ ] Anotar el ID del cliente (desde base de datos o logs)
- [ ] Eliminar el cliente
- [ ] Verificar en base de datos que NO existen:
  - [ ] Registros del cliente
  - [ ] Préstamos del cliente
  - [ ] Pagos programados del cliente
  - [ ] Historial de pagos del cliente

#### **Test 10: Modificación NO Afecta Préstamos**
- [ ] Seleccionar un cliente con préstamos activos
- [ ] Anotar los montos de los préstamos
- [ ] Modificar el nombre y teléfono del cliente
- [ ] Guardar cambios
- [ ] Verificar que los préstamos mantienen:
  - [ ] Mismos montos
  - [ ] Mismas fechas
  - [ ] Mismos estados
  - [ ] Mismo historial de pagos

---

### ? Pruebas de UI/UX

#### **Test 11: Colores y Diseño**
- [ ] Verificar botón "Modificar" es AZUL (#2196F3)
- [ ] Verificar botón "Eliminar" es ROJO (#F44336)
- [ ] Verificar botón "Guardar Cambios" es VERDE (#4CAF50)
- [ ] Verificar botón "Cancelar" es GRIS
- [ ] Verificar iconos visibles y claros
- [ ] Verificar textos legibles en todos los tamaños de pantalla

#### **Test 12: Swipe Gesture**
- [ ] Verificar swipe funciona suavemente
- [ ] Verificar botones aparecen completamente
- [ ] Verificar se puede cerrar el swipe (swipe de regreso)
- [ ] Verificar no interfiere con scroll vertical
- [ ] Verificar funciona en diferentes dispositivos

#### **Test 13: Navegación**
- [ ] Verificar todas las navegaciones son fluidas
- [ ] Verificar botón "Atrás" del sistema funciona correctamente
- [ ] Verificar no hay navegaciones duplicadas
- [ ] Verificar no quedan páginas apiladas incorrectamente

---

### ? Pruebas de Rendimiento

#### **Test 14: Rendimiento con Muchos Clientes**
- [ ] Crear 50+ clientes en la base de datos
- [ ] Abrir lista de clientes
- [ ] Verificar carga rápida
- [ ] Hacer swipe en varios clientes
- [ ] Verificar animaciones fluidas
- [ ] Modificar un cliente
- [ ] Verificar actualización rápida de la lista

#### **Test 15: Rendimiento con Datos Complejos**
- [ ] Crear cliente con 10+ préstamos
- [ ] Crear 100+ pagos asociados
- [ ] Eliminar el cliente
- [ ] Verificar tiempo de eliminación razonable (< 5 segundos)
- [ ] Verificar no hay lag en la UI

---

### ? Pruebas de Error y Edge Cases

#### **Test 16: Campos Especiales**
- [ ] Modificar cliente con caracteres especiales en nombre (á, é, í, ñ, etc.)
- [ ] Guardar y verificar se mantienen correctamente
- [ ] Modificar con números en direcciones
- [ ] Modificar con símbolos en teléfono (+, -, espacios)
- [ ] Verificar todos se guardan y muestran correctamente

#### **Test 17: Campos Largos**
- [ ] Intentar guardar nombre muy largo (200+ caracteres)
- [ ] Verificar comportamiento (truncar o error claro)
- [ ] Intentar guardar dirección muy larga (500+ caracteres)
- [ ] Verificar comportamiento apropiado

#### **Test 18: Conexión y Errores**
- [ ] Simular error de base de datos (si es posible)
- [ ] Intentar eliminar cliente
- [ ] Verificar mensaje de error descriptivo
- [ ] Verificar app no se crashea
- [ ] Verificar datos no quedan en estado inconsistente

---

### ? Pruebas de Compatibilidad

#### **Test 19: Android**
- [ ] Probar todas las funcionalidades en Android
- [ ] Verificar swipe gesture funciona correctamente
- [ ] Verificar diálogos se muestran correctamente
- [ ] Verificar botones son tocables y del tamaño correcto

#### **Test 20: iOS** (si aplica)
- [ ] Probar todas las funcionalidades en iOS
- [ ] Verificar swipe gesture funciona correctamente
- [ ] Verificar diálogos nativos de iOS
- [ ] Verificar navegación con gesto de iOS

#### **Test 21: Windows** (si aplica)
- [ ] Probar todas las funcionalidades en Windows
- [ ] Verificar alternativa a swipe (menú contextual)
- [ ] Verificar diálogos de Windows
- [ ] Verificar navegación con teclado

---

## ?? Resultados Esperados

### ? Criterios de Aceptación

1. **Funcionalidad Modificar:**
   - ? Se puede acceder desde lista (swipe) y desde detalle (botón)
   - ? Todos los campos son editables
   - ? Validaciones funcionan correctamente
   - ? Cambios se guardan en base de datos
   - ? UI se actualiza inmediatamente
   - ? Mensajes de confirmación apropiados

2. **Funcionalidad Eliminar:**
   - ? Se puede acceder desde lista (swipe) y desde detalle (botón)
   - ? Confirmación obligatoria antes de eliminar
   - ? Advertencias específicas con préstamos activos
   - ? Eliminación en cascada funciona correctamente
   - ? No quedan datos huérfanos
   - ? UI se actualiza inmediatamente
   - ? Mensajes de confirmación apropiados

3. **Integridad de Datos:**
   - ? No se pierden datos durante modificaciones
   - ? Eliminación en cascada completa
   - ? No hay registros huérfanos
   - ? Préstamos no se ven afectados por modificaciones

4. **Experiencia de Usuario:**
   - ? Interfaz intuitiva y clara
   - ? Colores semánticos apropiados
   - ? Animaciones fluidas
   - ? Navegación lógica
   - ? Mensajes descriptivos
   - ? Sin errores visuales

---

## ?? Registro de Bugs

Use esta sección para registrar cualquier problema encontrado:

| # | Fecha | Test | Descripción | Prioridad | Estado |
|---|-------|------|-------------|-----------|--------|
| 1 |       |      |             |           |        |
| 2 |       |      |             |           |        |

---

## ? Aprobación Final

- [ ] Todas las pruebas funcionales pasaron
- [ ] Todas las pruebas de integridad pasaron
- [ ] Todas las pruebas de UI/UX pasaron
- [ ] Todas las pruebas de rendimiento pasaron
- [ ] Todas las pruebas de error pasaron
- [ ] Todas las pruebas de compatibilidad pasaron
- [ ] No hay bugs críticos pendientes
- [ ] Documentación completa

**Probado por:** _____________________
**Fecha:** _____________________
**Firma:** _____________________

---

## ?? Notas Adicionales

Use este espacio para cualquier observación adicional sobre las pruebas realizadas:

```
___________________________________________________________
___________________________________________________________
___________________________________________________________
___________________________________________________________
```
