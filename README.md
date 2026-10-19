# Sistema de Control Clínico Veterinario 🐾

Sistema orientado a objetos para la gestión integral de una clínica veterinaria, desarrollado con una versión interactiva en **C# (.NET)** y la modelación estructural en **Java (BlueJ)**.

---

## 📋 Características Principales

- **Registro de Propietarios y Mascotas:** Gestión automatizada con asignación de códigos correlativos y soporte para edades flexibles (meses y años).
- **Agendamiento de Citas:** Programación de turnos médicos vinculados a pacientes y veterinarios.
- **Consulta Médica e Historial Clínico:** Registro detallado de peso, temperatura, síntomas, diagnósticos y recetas médicas guardadas en el expediente del paciente.
- **Gestión de Inventario:** Control de stock de medicamentos e insumos con opciones de reabastecimiento.
- **Generación de Reportes:** Informes consolidados de citas y productos con existencias bajas.
- **Persistencia de Datos (SQLite):** Toda la información se guarda en una base de datos local (`veterinaria.db`), creada automáticamente la primera vez que se ejecuta el programa. Los datos **no se pierden al cerrar la aplicación**.
- **Interfaz Gráfica (Windows Forms):** Además del menú de consola, el sistema incluye una interfaz gráfica con tablas (`DataGridView`) para trabajar los mismos datos de forma visual (opción `0` del menú).
- **CRUD completo:** Alta, consulta, actualización y baja de clientes, mascotas, citas, consultas e inventario, con SQL parametrizado y borrado en cascada del historial clínico.
- **Validaciones y manejo de errores:** Campos obligatorios, formato de fecha `DD/MM/AAAA`, números positivos para peso/temperatura/stock/precio, confirmación de eliminaciones y manejo de excepciones con mensajes al usuario.

---

## 🛠️ Tecnologías y Paradigmas

- **Lenguajes:** C# (.NET 10) y Java.
- **Entornos:** Visual Studio Code y BlueJ.
- **Interfaz de usuario:** Menú de consola + ventanas Windows Forms (`Clases_Csharp/Formularios/`).
- **Base de datos:** SQLite mediante el paquete `Microsoft.Data.Sqlite` (capa de acceso a datos en `Clases_Csharp/Datos/`).
- **Principios Aplicados:** 
  - Programación Orientada a Objetos (POO)
  - Encapsulamiento (`private set`)
  - Agregación y Asociaciones
  - Listas Dinámicas (`List<T>` / `ArrayList`)
  - Patrón Repositorio con SQL parametrizado (acceso a datos)
  - Manejo de excepciones (`try/catch`) y validación de entradas

---

## ▶️ Cómo ejecutar

```bash
dotnet run --project Clases_Csharp
```

1. Se muestra el **menú de consola** (opciones 1-7 y 8 para salir).
2. La opción **`0`** abre la **interfaz gráfica (Windows Forms)**; al cerrar la ventana se regresa a la consola y los datos se recargan desde la base de datos, de modo que ambas interfaces trabajan sobre la misma información.

## 🗂️ Formularios (CRUD visual)

| Formulario | Módulo | Operaciones |
|---|---|---|
| `FormPrincipal` | Ventana de acceso | Abre cada módulo |
| `FormClientes` | Propietarios | Alta, edición, baja, tabla |
| `FormMascotas` | Mascotas | Alta, edición, baja con dueño en combo |
| `FormCitas` | Agenda | Alta, edición, baja con validación de fecha |
| `FormConsultas` | Historial clínico | Alta, edición, baja con receta opcional |
| `FormInventario` | Productos | Alta, edición, baja y reabastecimiento |
| `FormReportes` | Informes | Citas y stock bajo |

Todas las operaciones usan los métodos del repositorio (`Datos/RepositorioVeterinaria.cs`) con **consultas parametrizadas**, validan campos obligatorios y muestran errores con `MessageBox` sin dejar inconsistencias en memoria (primero se escribe en la base de datos).

