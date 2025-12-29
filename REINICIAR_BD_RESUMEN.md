# ?? Reiniciar Base de Datos - RESUMEN RÁPIDO

## ? Guía Rápida (30 segundos)

### Opción 1: Desde la App (Recomendado) ?

```
1. Dashboard ? Icono ?? (arriba derecha)
2. Presionar "Limpiar Datos" o "Reiniciar Base de Datos"
3. Confirmar dos veces
4. ? ¡Listo! Base de datos limpia
```

### Opción 2: Manual (Android)

```bash
adb shell run-as com.companyname.app_credivnzl rm /data/data/com.companyname.app_credivnzl/files/prestafacil.db3
```

### Opción 3: Manual (Windows)

```powershell
# Eliminar archivo
Remove-Item "$env:LOCALAPPDATA\Packages\[App Package]\LocalState\prestafacil.db3" -Force
```

---

## ?? ¿Qué método usar?

| Necesitas | Usa esto |
|-----------|----------|
| ?? Empezar con datos limpios | **Limpiar Datos** |
| ?? Resolver errores de BD | **Reiniciar BD Completa** |
| ? Más rápido | **Limpiar Datos** |
| ??? Más seguro | **Reiniciar BD Completa** |

---

## ? Verificación Rápida

Después de limpiar, verifica:
- [ ] Total Clientes: **0**
- [ ] Total Préstamos: **0**
- [ ] Total Pagos: **0**
- [ ] Historial: **0**

---

## ?? Recordatorios

- ? **NO** se puede recuperar datos después
- ? **SÍ** pide confirmación dos veces
- ? **SÍ** muestra cuántos datos se eliminarán
- ? Tarda solo 2-5 segundos

---

## ?? Problemas Comunes

**"No veo el botón de configuración"**
? Dashboard ? Icono ?? arriba derecha

**"Los datos no se eliminaron"**
? Presionar "Actualizar Información" o reiniciar app

**"Error al limpiar"**
? Usar "Reiniciar BD Completa" en su lugar

---

## ?? Acceso Rápido

```
Dashboard ? ?? ? Configuración
```

---

**Documentación completa:** Ver `GUIA_REINICIAR_BASE_DATOS.md`
