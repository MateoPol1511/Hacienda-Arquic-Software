# ?? Sistema de Gestión de Hacienda Ganadera

Sistema completo MVC en .NET 8 para la gestión integral de una hacienda ganadera, integrado con la biblioteca de clases `Bib_Hacienda`.

## ?? Características

### Módulos Implementados

#### 1. ?? **Gestión de Potreros**
- Crear potreros según tipo de ganado (Ternero, Novillo, Cebón)
- Visualizar estado y ocupación
- Organización por tipo de ganado

#### 2. ?? **Gestión de Reses**
- Registro de ganado por potrero
- Alimentación de reses (incrementa peso)
- Venta de reses con registro automático
- Clasificación automática por edad:
  - **Terneros**: 0-12 meses
  - **Cebones**: 13-48 meses
  - **Novillos**: 49+ meses

#### 3. ?? **Gestión de Vacunas**
- Inventario de vacunas bacterianas y vivas
- Control de fechas de vencimiento
- Aplicación de vacunas con límites por tipo de res:
  - **Terneros**: Máx. 3 bacterianas, 1 viva
  - **Cebones**: Máx. 2 bacterianas, 2 vivas
  - **Novillos**: Máx. 1 bacteriana, 4 vivas
- Prevención de vacunas duplicadas o vencidas

#### 4. ?? **Registro de Ventas**
- Historial completo de ventas
- Estadísticas por período
- Cálculo automático de precio por kilogramo
- Resumen mensual

#### 5. ?? **Gestión de Usuarios**
- Registro de usuarios del sistema
- Validación de contraseñas
- Control de acceso

## ??? Estructura del Proyecto

```
p_mvcHacienda/
??? Controllers/   # Controladores MVC
?   ??? PotreroController.cs
???? ResController.cs
?   ??? VacunaController.cs
?   ??? VentaController.cs
?   ??? UsuarioController.cs
??? Servicios/          # Lógica de negocio
?   ??? PersistenciaService.cs
?   ??? PotreroService.cs
?   ??? ResService.cs
?   ??? VacunaService.cs
?   ??? VentaService.cs
?   ??? UsuarioService.cs
??? Views/  # Vistas Razor
?   ??? Potreros/
?   ?   ??? Index.cshtml
?   ?   ??? crear.cshtml
?   ?   ??? Details.cshtml
?   ??? Reses/
?   ?   ??? Index.cshtml
? ?   ??? crear.cshtml
?   ??? Vacunas/
?   ?   ??? Index.cshtml
?   ?   ??? crear.cshtml
?   ?   ??? Aplicar.cshtml
?   ??? Ventas/
?   ?   ??? Index.cshtml
???? Usuarios/
?       ??? Index.cshtml
?       ??? crear.cshtml
??? Archivos/        # Persistencia en archivos .txt (JSON)
?   ??? Potreros.txt
?   ??? Reses.txt
?   ??? Vacunas.txt
?   ??? Ventas.txt
?   ??? Usuarios.txt
??? Program.cs            # Configuración e inyección de dependencias
```

## ?? Cómo Ejecutar

### Requisitos
- .NET 8 SDK
- Visual Studio 2022 (recomendado) o VS Code
- Biblioteca `Bib_Hacienda.dll` en `../Bib_Hacienda/Bib_Hacienda/bin/Debug/`

### Pasos

1. **Abrir el proyecto**
   ```bash
   cd p_mvcHacienda
   ```

2. **Restaurar paquetes**
   ```bash
   dotnet restore
   ```

3. **Compilar**
   ```bash
   dotnet build
   ```

4. **Ejecutar**
   ```bash
   dotnet run
   ```

5. **Acceder al sistema**
   - Abre tu navegador en: `https://localhost:5001` o `http://localhost:5000`
   - La página principal mostrará todos los módulos disponibles

## ?? Persistencia de Datos

Los datos se guardan automáticamente en archivos JSON en la carpeta `Archivos/`:

- **Potreros.txt**: Información de potreros y reses asociadas
- **Vacunas.txt**: Inventario de vacunas disponibles
- **Ventas.txt**: Historial completo de ventas
- **Usuarios.txt**: Usuarios del sistema

### Carga Automática
Al iniciar la aplicación, se cargan automáticamente todos los datos existentes.

### Guardado Automático
Cada operación (crear, actualizar, eliminar) guarda inmediatamente los cambios.

## ?? Características de la Interfaz

### Diseño Temático Ganadero
- **Colores:**
  - Verde (#2e7d32) - Potreros y naturaleza
  - Marrón (#8b4513) - Ganado y tierra
  - Azul (#1976d2) - Vacunas y salud
  - Verde claro (#4caf50) - Ventas y éxito
  - Púrpura (#6a1b9a) - Usuarios y sistema

### Elementos Visuales
- Emojis temáticos (??, ??, ??, ??, ??)
- Tarjetas con estadísticas en tiempo real
- Tablas responsivas con información clara
- Alertas de confirmación y errores
- Modales para acciones importantes

### Responsivo
- Adaptado para escritorio, tablet y móvil
- Bootstrap 5 para diseño responsive

## ?? Funcionalidades Destacadas

### Control de Reglas de Negocio
El sistema respeta todas las reglas definidas en `Bib_Hacienda`:

1. **Potreros por Tipo de Ganado**
   - Ternero (0-12 meses)
   - Cebón (13-48 meses)
   - Novillo (49+ meses)

2. **Límites de Vacunación**
   - Validación automática según tipo de res
   - Control de vacunas vencidas
   - Prevención de duplicados

3. **Eventos del Sistema**
   - Peso mínimo alcanzado
   - Peso ideal para venta
   - Potrero medio lleno
   - Potrero lleno
   - Vacunación completa

4. **Validaciones**
   - Edad apropiada para el potrero
   - Capacidad máxima
   - Fechas de vencimiento
   - Datos requeridos

## ?? Tecnologías Utilizadas

- **.NET 8** - Framework principal
- **ASP.NET Core MVC** - Patrón arquitectónico
- **Razor Pages** - Motor de vistas
- **Bootstrap 5** - Framework CSS
- **jQuery** - Manipulación DOM
- **System.Text.Json** - Serialización JSON
- **Bib_Hacienda** - Biblioteca de lógica de negocio

## ?? Estadísticas del Sistema

Cada módulo muestra estadísticas relevantes:

- **Potreros**: Total, reses, vacíos, ocupados
- **Reses**: Total por tipo, peso promedio
- **Vacunas**: Total, bacterianas, vivas, vigentes, vencidas
- **Ventas**: Total ventas, monto total, promedio, mensuales
- **Usuarios**: Total registrados

## ?? Solución de Problemas

### Error: "No se puede encontrar Bib_Hacienda.dll"
- Verifica que la DLL esté en: `../Bib_Hacienda/Bib_Hacienda/bin/Debug/Bib_Hacienda.dll`
- Compila primero el proyecto `Bib_Hacienda`

### Los datos no se guardan
- Verifica que la carpeta `Archivos/` tenga permisos de escritura
- Los archivos .txt se crean automáticamente si no existen

### Error al cargar datos
- Verifica que los archivos .txt tengan formato JSON válido
- Puedes eliminar los archivos para empezar desde cero

## ?? Autores

Sistema desarrollado como proyecto de paradigmas de programación, integrando:
- Programación Orientada a Objetos (POO)
- Programación Orientada a Eventos (POE)
- Patrón Modelo-Vista-Controlador (MVC)
- Programación Orientada a Aspectos (POA)

## ?? Licencia

Proyecto académico - Universidad [Nombre]

---

**?? Sistema de Hacienda Ganadera v1.0**  
*Gestión profesional para el campo colombiano*
