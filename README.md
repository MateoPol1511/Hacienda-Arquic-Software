## Roles y Responsabilidades

| Rol | Responde por | Encargado/a |
|-----|--------------|-------------|
| **Arquitecto de dominio** | Identificación de responsabilidades y límites de cada clase (SRP), modelo del dominio, jerarquías de herencia y su validez frente a LSP. | Mateo Acevedo |
| **Arquitecto de dependencias** | Mapa de dependencias, abstracciones (interfaces), inversión e inyección de dependencias, composition root (DIP, ISP). | Juan Pablo Aristizabal |
| **Ingeniero de comportamiento** | Pruebas de caracterización, evidencia de que la conducta observable se preservó, escenarios de ejecución del programa principal. | Luis Guillermo velez |
| **Integrador y evidencia** | Consistencia diagrama–código, estructura del entregable, bitácora de uso de IA, métricas antes/después. | Mateo Polanco |


Enlace al Video: https://youtu.be/yuiFwFLtOTk


## Ejecución del proyecto

### Requisitos
- .NET 8 SDK instalado
- Windows
- Visual Studio 2022 o VS Code (opcional)

### Pasos para ejecutar
Desde la carpeta raíz del proyecto:

```powershell
cd "c:\Users\Usuario\Downloads\Hacienda_TOBE_CodigoRediseñado\Hacienda_TOBE"
dotnet restore
dotnet build Hacienda_TOBE.sln
dotnet run --project p_mvcHacienda\p_mvcHacienda.csproj
