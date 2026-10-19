using System;
using System.IO;
using Microsoft.Data.Sqlite;

namespace SistemaVeterinaria.Datos
{
    /// <summary>
    /// Configura la base de datos SQLite local (veterinaria.db).
    /// El archivo se crea junto al ejecutable solo la primera vez.
    /// </summary>
    public static class BaseDatos
    {
        // Ruta del archivo de base de datos (junto al .exe generado)
        public static string RutaArchivo { get; } =
            Path.Combine(AppContext.BaseDirectory, "veterinaria.db");

        /// <summary>
        /// Abre una conexión a la base de datos y activa las llaves foráneas.
        /// </summary>
        public static SqliteConnection AbrirConexion()
        {
            SqliteConnection conexion = new SqliteConnection($"Data Source={RutaArchivo}");
            conexion.Open();

            using (SqliteCommand pragma = conexion.CreateCommand())
            {
                pragma.CommandText = "PRAGMA foreign_keys = ON;";
                pragma.ExecuteNonQuery();
            }

            return conexion;
        }

        /// <summary>
        /// Crea las tablas (si no existen) y siembra los datos iniciales
        /// únicamente en la primera ejecución.
        /// </summary>
        public static void Inicializar()
        {
            using (SqliteConnection conexion = AbrirConexion())
            {
                using (SqliteCommand cmd = conexion.CreateCommand())
                {
                    cmd.CommandText = Esquema;
                    cmd.ExecuteNonQuery();
                }

                SembrarDatosIniciales(conexion);
            }
        }

        // Estructura de las tablas del sistema
        private const string Esquema = @"
            CREATE TABLE IF NOT EXISTS clientes (
                id        TEXT PRIMARY KEY,
                nombre    TEXT NOT NULL,
                telefono  TEXT NOT NULL,
                direccion TEXT NOT NULL
            );

            CREATE TABLE IF NOT EXISTS pacientes (
                id         TEXT PRIMARY KEY,
                nombre     TEXT NOT NULL,
                especie    TEXT NOT NULL,
                raza       TEXT NOT NULL,
                edad       TEXT NOT NULL,
                id_cliente TEXT NOT NULL,
                FOREIGN KEY (id_cliente) REFERENCES clientes(id)
            );

            CREATE TABLE IF NOT EXISTS veterinarios (
                id            TEXT PRIMARY KEY,
                nombre        TEXT NOT NULL,
                especialidad  TEXT NOT NULL
            );

            CREATE TABLE IF NOT EXISTS citas (
                id             TEXT PRIMARY KEY,
                fecha          TEXT NOT NULL,
                hora           TEXT NOT NULL,
                id_mascota     TEXT NOT NULL,
                id_veterinario TEXT NOT NULL,
                FOREIGN KEY (id_mascota)     REFERENCES pacientes(id),
                FOREIGN KEY (id_veterinario) REFERENCES veterinarios(id)
            );

            CREATE TABLE IF NOT EXISTS consultas (
                id           TEXT PRIMARY KEY,
                id_mascota   TEXT NOT NULL,
                fecha        TEXT NOT NULL,
                peso         REAL NOT NULL,
                temperatura  REAL NOT NULL,
                sintomas     TEXT NOT NULL,
                diagnostico  TEXT NOT NULL,
                farmaco      TEXT NOT NULL,
                dosis        TEXT NOT NULL,
                frecuencia   TEXT NOT NULL,
                FOREIGN KEY (id_mascota) REFERENCES pacientes(id)
            );

            CREATE TABLE IF NOT EXISTS productos (
                id      TEXT PRIMARY KEY,
                nombre  TEXT NOT NULL,
                stock   INTEGER NOT NULL,
                precio  REAL NOT NULL
            );
        ";

        // Datos por defecto: se insertan solo si las tablas están vacías
        private static void SembrarDatosIniciales(SqliteConnection conexion)
        {
            using (SqliteCommand cmd = conexion.CreateCommand())
            {
                cmd.CommandText = "SELECT COUNT(*) FROM veterinarios;";
                long totalVeterinarios = (long)cmd.ExecuteScalar()!;

                if (totalVeterinarios == 0)
                {
                    cmd.CommandText = @"
                        INSERT INTO veterinarios (id, nombre, especialidad)
                        VALUES ('V001', 'Dra. Sofia Morales', 'Cirugía y Medicina General');";
                    cmd.ExecuteNonQuery();
                }

                cmd.CommandText = "SELECT COUNT(*) FROM productos;";
                long totalProductos = (long)cmd.ExecuteScalar()!;

                if (totalProductos == 0)
                {
                    cmd.CommandText = @"
                        INSERT INTO productos (id, nombre, stock, precio) VALUES
                        ('P001', 'Protector Gástrico', 3, 45.00),
                        ('P002', 'Antipulgas', 10, 120.00);";
                    cmd.ExecuteNonQuery();
                }
            }
        }
    }
}
