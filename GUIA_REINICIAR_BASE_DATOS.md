# ?? Guía para Reiniciar la Base de Datos - CrediVnzl

## ?? Tabla de Contenidos
1. [Descripción General](#descripción-general)
2. [Métodos Disponibles](#métodos-disponibles)
3. [Usar la Interfaz de la App](#usar-la-interfaz-de-la-app)
4. [Reinicio Manual (Avanzado)](#reinicio-manual-avanzado)
5. [Diferencias entre Métodos](#diferencias-entre-métodos)
6. [Casos de Uso](#casos-de-uso)
7. [Preguntas Frecuentes](#preguntas-frecuentes)

---

## ?? Descripción General

La aplicación CrediVnzl ahora incluye una funcionalidad completa para gestionar y reiniciar la base de datos. Esto es útil para:

- ? Comenzar con una aplicación limpia
- ? Eliminar todos los datos de prueba
- ? Resolver problemas con datos corruptos
- ? Preparar la app para producción
- ? Desarrollo y testing

---

## ??? Métodos Disponibles

### Método 1: **Limpiar Todos los Datos** (Recomendado)
- ? Elimina todos los registros de la base de datos
- ? Mantiene la estructura de las tablas
- ? Más rápido
- ? La app sigue funcionando inmediatamente

### Método 2: **Reiniciar Base de Datos Completa**
- ? Elimina el archivo de base de datos completamente
- ? Crea un archivo nuevo desde cero
- ? Soluciona problemas de corrupción de datos
- ? Limpieza más profunda

---

## ?? Usar la Interfaz de la App

### Paso a Paso:

#### 1?? **Acceder a Configuración**

**Opción A: Desde el Dashboard**
```
1. Abrir la aplicación
2. En el Dashboard (pantalla principal)
3. Presionar el icono ?? en la esquina superior derecha
```

**Opción B: Navegación directa (si está disponible)**
```
Ir a: Menú ? Configuración
```

#### 2?? **Ver Información de la Base de Datos**

La pantalla de configuración mostrará:

```
?? Información de la Base de Datos
?????????????????????????????????????
?? Total Clientes:           5
?? Total Préstamos:          8
?? Pagos Programados:        24
?? Historial de Pagos:       15
?????????????????????????????????????
?? Tamaño del Archivo:       245.50 KB

?? Ubicación del Archivo
C:\Users\[Tu Usuario]\AppData\Local\
Packages\[App Package]\LocalState\prestafacil.db3
```

#### 3?? **Elegir Método de Limpieza**

##### **Opción A: Limpiar Todos los Datos** ??

1. Presionar botón **"?? Limpiar Datos"**
2. Leer el diálogo de confirmación:
   ```
   ?? Confirmar Limpieza de Datos
   
   Esta acción eliminará:
   • 5 Cliente(s)
   • 8 Préstamo(s)
   • 24 Pago(s) Programado(s)
   • 15 Registro(s) de Historial
   
   Esta acción NO se puede deshacer.
   
   ¿Desea continuar?
   
   [Cancelar]  [Sí, Limpiar Todo]
   ```

3. Presionar **"Sí, Limpiar Todo"**
4. Segunda confirmación:
   ```
   ?? Última Confirmación
   
   ¿Está COMPLETAMENTE SEGURO de eliminar todos los datos?
   Esta es su última oportunidad para cancelar.
   
   [Cancelar]  [SÍ, ELIMINAR TODO]
   ```

5. Presionar **"SÍ, ELIMINAR TODO"**
6. Ver mensaje de éxito:
   ```
   ? Éxito
   
   Todos los datos han sido eliminados correctamente.
   La base de datos está ahora vacía y lista para usar.
   
   [OK]
   ```

##### **Opción B: Reiniciar Base de Datos Completa** ??

1. Presionar botón **"?? Reiniciar Base de Datos"**
2. Leer el diálogo de confirmación:
   ```
   ?? Confirmar Reinicio de Base de Datos
   
   Esta acción:
   • Eliminará el archivo de base de datos completo
   • Borrará todos los datos:
     - 5 Cliente(s)
     - 8 Préstamo(s)
     - 24 Pago(s)
     - 15 Historial(es)
   • Creará una base de datos nueva y vacía
   
   Esta acción NO se puede deshacer.
   
   ¿Desea continuar?
   
   [Cancelar]  [Sí, Reiniciar]
   ```

3. Presionar **"Sí, Reiniciar"**
4. Advertencia final:
   ```
   ?? ADVERTENCIA FINAL
   
   Está a punto de ELIMINAR PERMANENTEMENTE 
   el archivo de base de datos.
   
   Se perderá toda la información (245.50 KB).
   
   Esta es su ÚLTIMA oportunidad para cancelar.
   
   ¿Está ABSOLUTAMENTE SEGURO?
   
   [NO, Cancelar]  [SÍ, ELIMINAR TODO]
   ```

5. Presionar **"SÍ, ELIMINAR TODO"**
6. Ver mensaje de éxito:
   ```
   ? Base de Datos Reiniciada
   
   La base de datos ha sido eliminada y recreada exitosamente.
   La aplicación está ahora completamente limpia y 
   lista para usar con datos nuevos.
   
   [OK]
   ```

#### 4?? **Verificar que la Limpieza fue Exitosa**

1. Presionar botón **"?? Actualizar Información"**
2. Verificar que todos los contadores están en 0:
   ```
   ?? Total Clientes:           0
   ?? Total Préstamos:          0
   ?? Pagos Programados:        0
   ?? Historial de Pagos:       0
   ?? Tamaño del Archivo:       12.00 KB
   ```

---

## ?? Reinicio Manual (Avanzado)

### Para Desarrolladores

Si necesitas reiniciar la base de datos manualmente sin usar la interfaz:

#### **Método 1: Eliminar el Archivo Directamente**

##### En Android:
```bash
# Usando ADB (Android Debug Bridge)
adb shell run-as com.companyname.app_credivnzl rm /data/data/com.companyname.app_credivnzl/files/prestafacil.db3

# O acceder al dispositivo y eliminar manualmente
adb shell
cd /data/data/com.companyname.app_credivnzl/files/
rm prestafacil.db3
exit
```

##### En Windows:
```powershell
# Ubicación del archivo
$dbPath = "$env:LOCALAPPDATA\Packages\[Tu App Package]\LocalState\prestafacil.db3"

# Eliminar el archivo
Remove-Item -Path $dbPath -Force

# O navegar manualmente
explorer "$env:LOCALAPPDATA\Packages"
# Buscar la carpeta de tu app
# Ir a LocalState
# Eliminar prestafacil.db3
```

##### En iOS/macOS:
```bash
# Usando Simulador
# La ruta varía según la versión
~/Library/Developer/CoreSimulator/Devices/[Device-ID]/data/Containers/Data/Application/[App-ID]/Library/prestafacil.db3

# Reiniciar simulador después de eliminar
```

#### **Método 2: Usar Código Programáticamente**

```csharp
// En cualquier parte de tu código con acceso a DatabaseService

// Opción 1: Limpiar datos
await _databaseService.ReiniciarBaseDeDatosAsync();

// Opción 2: Eliminar archivo completo
await _databaseService.EliminarBaseDeDatosCompletaAsync();
```

---

## ?? Diferencias entre Métodos

| Característica | Limpiar Datos ?? | Reiniciar BD ?? |
|----------------|------------------|-----------------|
| **Velocidad** | ? Muy rápido | ?? Más lento |
| **Acción** | DELETE de registros | Elimina archivo |
| **Estructura** | ? Se mantiene | ? Se recrea |
| **Tamaño final** | ~12 KB | ~4 KB |
| **Recomendado para** | Limpieza normal | Problemas técnicos |
| **Conexión BD** | ? Mantiene | ? Cierra y recrea |
| **Seguridad** | ???? | ????? |

---

## ?? Casos de Uso

### Cuándo usar **Limpiar Datos** ??:

? **Situación Normal:**
- Terminar período de pruebas
- Datos de demostración que ya no necesitas
- Comenzar con clientes reales
- Limpieza rutinaria

? **Ejemplo:**
```
"He estado probando la app con datos de prueba.
Ahora quiero comenzar a usar la app con clientes reales."

?? Usar: Limpiar Datos
```

### Cuándo usar **Reiniciar Base de Datos** ??:

? **Situaciones Especiales:**
- Base de datos muestra comportamiento extraño
- Errores al cargar datos
- Sospecha de datos corruptos
- Actualización importante de la app
- Desarrollo: probar migración de datos

? **Ejemplo:**
```
"La app me muestra errores al cargar los clientes
y algunos datos no se ven correctamente."

?? Usar: Reiniciar Base de Datos Completa
```

---

## ? Preguntas Frecuentes

### **P1: ¿Se pueden recuperar los datos después de eliminarlos?**
**R:** ? No. La eliminación es permanente y no hay forma de recuperar los datos. Por eso la app pide confirmación dos veces.

### **P2: ¿Tengo que desinstalar la app para reiniciar la base de datos?**
**R:** ? No. Puedes usar los métodos integrados en la app. No es necesario desinstalarla.

### **P3: ¿Cuánto tiempo tarda el reinicio?**
**R:** 
- Limpiar Datos: 1-3 segundos
- Reiniciar BD Completa: 2-5 segundos

### **P4: ¿La app sigue funcionando después del reinicio?**
**R:** ? Sí. La app está lista para usar inmediatamente después. Puedes crear clientes nuevos de inmediato.

### **P5: ¿Qué pasa si tengo la app abierta en otro dispositivo?**
**R:** ?? Cada instalación tiene su propia base de datos local. Reiniciar en un dispositivo no afecta otros.

### **P6: ¿Perderé la configuración de la app?**
**R:** ? No. Solo se eliminan los datos de clientes, préstamos y pagos. La configuración se mantiene.

### **P7: ¿Puedo hacer backup antes de limpiar?**
**R:** ? Sí, pero por ahora es manual:
```
1. Ir a Configuración
2. Copiar la ruta del archivo
3. Navegar a esa ubicación
4. Copiar el archivo prestafacil.db3 a otro lugar
5. Luego puedes limpiarlo tranquilo
```

### **P8: ¿Hay algún log de lo que se eliminó?**
**R:** ? Sí, antes de eliminar, la app muestra exactamente cuántos registros se van a eliminar en el diálogo de confirmación.

### **P9: ¿Puedo cancelar la operación después de confirmar?**
**R:** ? No. Una vez que presionas el segundo "SÍ", la eliminación es inmediata e irreversible.

### **P10: ¿Afecta el rendimiento de la app?**
**R:** ? Al contrario, una base de datos limpia puede mejorar el rendimiento.

---

## ?? Información Técnica

### Ubicación del Archivo de Base de Datos

#### Android:
```
/data/data/com.companyname.app_credivnzl/files/prestafacil.db3
```

#### Windows:
```
C:\Users\[Usuario]\AppData\Local\Packages\
[App Package ID]\LocalState\prestafacil.db3
```

#### iOS:
```
/var/mobile/Containers/Data/Application/
[App ID]/Library/prestafacil.db3
```

#### macOS:
```
~/Library/Containers/[App Bundle]/Data/Library/
Application Support/prestafacil.db3
```

### Estructura de Tablas

La base de datos contiene 4 tablas principales:

1. **Cliente** - Información de clientes
2. **Prestamo** - Detalles de préstamos
3. **Pago** - Pagos programados
4. **HistorialPago** - Registro de pagos realizados

### Orden de Eliminación (Cascada)

Para mantener integridad referencial:
```
1. HistorialPago (hijos)
2. Pago (hijos)
3. Prestamo (hijos)
4. Cliente (padres)
```

---

## ?? Recomendaciones

### ? Mejores Prácticas:

1. **Antes de Producción:**
   - Limpiar todos los datos de prueba
   - Verificar que contadores están en 0
   - Probar crear un cliente real

2. **Durante Desarrollo:**
   - Usar "Limpiar Datos" regularmente
   - Usar "Reiniciar BD" solo cuando hay problemas

3. **Mantenimiento:**
   - No es necesario limpiar rutinariamente
   - Solo limpiar cuando cambies de datos de prueba a producción

### ?? Advertencias:

- ? No limpiar con clientes reales activos
- ? No limpiar si hay deudas pendientes importantes
- ? No limpiar si no estás seguro
- ? Siempre leer los diálogos de confirmación completamente

---

## ?? Solución de Problemas

### Problema: "No puedo acceder a Configuración"
**Solución:**
1. Verificar que estás en el Dashboard
2. Buscar el icono ?? en la esquina superior derecha
3. Si no aparece, actualizar la app

### Problema: "Error al limpiar la base de datos"
**Solución:**
1. Cerrar la app completamente
2. Abrir nuevamente
3. Intentar de nuevo
4. Si persiste, usar "Reiniciar BD Completa"

### Problema: "Los datos no se eliminaron"
**Solución:**
1. Presionar "?? Actualizar Información"
2. Verificar los contadores
3. Si aún hay datos, usar "Reiniciar BD Completa"

### Problema: "La app crashea después de limpiar"
**Solución:**
1. Cerrar la app
2. Eliminar el archivo manualmente (método avanzado)
3. Abrir la app - creará BD nueva automáticamente

---

## ?? Soporte

Si tienes problemas no cubiertos en esta guía:

1. Revisar los logs de la aplicación
2. Intentar "Reiniciar BD Completa" como último recurso
3. Contactar al desarrollador con:
   - Descripción del problema
   - Pasos que intentaste
   - Screenshots si es posible

---

## ?? Changelog de Funcionalidad

### Versión 1.0.0
- ? Implementado "Limpiar Datos"
- ? Implementado "Reiniciar BD Completa"
- ? Pantalla de Configuración
- ? Información de BD en tiempo real
- ? Confirmaciones de seguridad dobles
- ? Botón de acceso desde Dashboard

---

## ? Lista de Verificación Post-Reinicio

Después de reiniciar la base de datos, verifica:

- [ ] Contador de clientes en 0
- [ ] Contador de préstamos en 0
- [ ] Contador de pagos en 0
- [ ] Contador de historial en 0
- [ ] Tamaño de archivo pequeño (~4-12 KB)
- [ ] App abre sin errores
- [ ] Puedes crear un cliente nuevo
- [ ] Dashboard muestra datos correctos
- [ ] Navegación funciona correctamente

---

**¡Tu app está lista para usar con datos frescos! ??**
