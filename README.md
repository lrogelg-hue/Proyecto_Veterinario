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

---

## 🛠️ Tecnologías y Paradigmas

- **Lenguajes:** C# (.NET) y Java.
- **Entornos:** Visual Studio Code y BlueJ.
- **Base de datos:** SQLite mediante el paquete `Microsoft.Data.Sqlite` (capa de acceso a datos en `Clases_Csharp/Datos/`).
- **Principios Aplicados:** 
  - Programación Orientada a Objetos (POO)
  - Encapsulamiento (`private set`)
  - Agregación y Asociaciones
  - Listas Dinámicas (`List<T>` / `ArrayList`)
  - Patrón Repositorio con SQL parametrizado (acceso a datos)

