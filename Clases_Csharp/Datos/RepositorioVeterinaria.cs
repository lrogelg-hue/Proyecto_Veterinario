using System.Collections.Generic;
using Microsoft.Data.Sqlite;

namespace SistemaVeterinaria.Datos
{
    /// <summary>
    /// Acceso a datos del sistema: guarda y consulta la información
    /// de la base de datos SQLite usando parámetros SQL (sin concatenar texto).
    /// </summary>
    public static class RepositorioVeterinaria
    {
        // ---------------------------------------------------------------
        // IDs CORRELATIVOS
        // ---------------------------------------------------------------

        /// <summary>
        /// Devuelve el siguiente ID con formato de 3 dígitos, por ejemplo:
        /// clientes -> "C001", mascotas -> "M001", citas -> "CIT-001".
        /// El número se calcula a partir del ID más alto existente en la tabla,
        /// por lo que siempre queda único aunque se registren muchos registros.
        /// (El nombre de la tabla proviene de las constantes de esta clase, no del usuario.)
        /// </summary>
        public static string SiguienteId(string tabla, string prefijo)
        {
            using (SqliteConnection conexion = BaseDatos.AbrirConexion())
            using (SqliteCommand cmd = conexion.CreateCommand())
            {
                cmd.CommandText = $"SELECT id FROM {tabla} WHERE id LIKE @prefijo;";
                cmd.Parameters.AddWithValue("@prefijo", prefijo + "%");

                int maximo = 0;

                using (SqliteDataReader lector = cmd.ExecuteReader())
                {
                    while (lector.Read())
                    {
                        string id = lector.GetString(0);
                        string numero = id.Substring(prefijo.Length);
                        if (int.TryParse(numero, out int valor) && valor > maximo)
                        {
                            maximo = valor;
                        }
                    }
                }

                return prefijo + (maximo + 1).ToString("D3");
            }
        }

        // ---------------------------------------------------------------
        // INSERCIONES
        // ---------------------------------------------------------------

        public static void InsertarCliente(Cliente cliente)
        {
            using (SqliteConnection conexion = BaseDatos.AbrirConexion())
            using (SqliteCommand cmd = conexion.CreateCommand())
            {
                cmd.CommandText = @"
                    INSERT INTO clientes (id, nombre, telefono, direccion)
                    VALUES (@id, @nombre, @telefono, @direccion);";
                cmd.Parameters.AddWithValue("@id", cliente.IdCliente);
                cmd.Parameters.AddWithValue("@nombre", cliente.Nombre);
                cmd.Parameters.AddWithValue("@telefono", cliente.Telefono);
                cmd.Parameters.AddWithValue("@direccion", cliente.Direccion);
                cmd.ExecuteNonQuery();
            }
        }

        public static void InsertarMascota(Mascota mascota, string idCliente)
        {
            using (SqliteConnection conexion = BaseDatos.AbrirConexion())
            using (SqliteCommand cmd = conexion.CreateCommand())
            {
                cmd.CommandText = @"
                    INSERT INTO mascotas (id, nombre, especie, raza, edad, id_cliente)
                    VALUES (@id, @nombre, @especie, @raza, @edad, @id_cliente);";
                cmd.Parameters.AddWithValue("@id", mascota.IdMascota);
                cmd.Parameters.AddWithValue("@nombre", mascota.Nombre);
                cmd.Parameters.AddWithValue("@especie", mascota.Especie);
                cmd.Parameters.AddWithValue("@raza", mascota.Raza);
                cmd.Parameters.AddWithValue("@edad", mascota.Edad);
                cmd.Parameters.AddWithValue("@id_cliente", idCliente);
                cmd.ExecuteNonQuery();
            }
        }

        public static void InsertarCita(Cita cita)
        {
            using (SqliteConnection conexion = BaseDatos.AbrirConexion())
            using (SqliteCommand cmd = conexion.CreateCommand())
            {
                cmd.CommandText = @"
                    INSERT INTO citas (id, fecha, hora, id_mascota, id_veterinario)
                    VALUES (@id, @fecha, @hora, @id_mascota, @id_veterinario);";
                cmd.Parameters.AddWithValue("@id", cita.IdCita);
                cmd.Parameters.AddWithValue("@fecha", cita.Fecha);
                cmd.Parameters.AddWithValue("@hora", cita.Hora);
                cmd.Parameters.AddWithValue("@id_mascota", cita.Mascota.IdMascota);
                cmd.Parameters.AddWithValue("@id_veterinario", cita.Veterinario.IdVeterinario);
                cmd.ExecuteNonQuery();
            }
        }

        public static void InsertarConsulta(Consulta consulta, string idMascota)
        {
            using (SqliteConnection conexion = BaseDatos.AbrirConexion())
            using (SqliteCommand cmd = conexion.CreateCommand())
            {
                cmd.CommandText = @"
                    INSERT INTO consultas (id, id_mascota, fecha, peso, temperatura,
                                           sintomas, diagnostico, farmaco, dosis, frecuencia)
                    VALUES (@id, @id_mascota, @fecha, @peso, @temperatura,
                            @sintomas, @diagnostico, @farmaco, @dosis, @frecuencia);";
                cmd.Parameters.AddWithValue("@id", consulta.IdConsulta);
                cmd.Parameters.AddWithValue("@id_mascota", idMascota);
                cmd.Parameters.AddWithValue("@fecha", consulta.Fecha);
                cmd.Parameters.AddWithValue("@peso", consulta.Peso);
                cmd.Parameters.AddWithValue("@temperatura", consulta.Temperatura);
                cmd.Parameters.AddWithValue("@sintomas", consulta.Sintomas);
                cmd.Parameters.AddWithValue("@diagnostico", consulta.Diagnostico);
                cmd.Parameters.AddWithValue("@farmaco", consulta.Farmaco ?? "");
                cmd.Parameters.AddWithValue("@dosis", consulta.Dosis ?? "");
                cmd.Parameters.AddWithValue("@frecuencia", consulta.Frecuencia ?? "");
                cmd.ExecuteNonQuery();
            }
        }

        /// <summary>
        /// Suma la cantidad indicada al stock actual del producto.
        /// </summary>
        public static void ActualizarStock(string idProducto, int cantidad)
        {
            using (SqliteConnection conexion = BaseDatos.AbrirConexion())
            using (SqliteCommand cmd = conexion.CreateCommand())
            {
                cmd.CommandText = "UPDATE productos SET stock = stock + @cantidad WHERE id = @id;";
                cmd.Parameters.AddWithValue("@cantidad", cantidad);
                cmd.Parameters.AddWithValue("@id", idProducto);
                cmd.ExecuteNonQuery();
            }
        }

        // ---------------------------------------------------------------
        // CONSULTAS (CARGA DE DATOS)
        // ---------------------------------------------------------------

        public static List<Veterinario> CargarVeterinarios()
        {
            List<Veterinario> veterinarios = new List<Veterinario>();

            using (SqliteConnection conexion = BaseDatos.AbrirConexion())
            using (SqliteCommand cmd = conexion.CreateCommand())
            {
                cmd.CommandText = "SELECT id, nombre, especialidad FROM veterinarios;";

                using (SqliteDataReader lector = cmd.ExecuteReader())
                {
                    while (lector.Read())
                    {
                        veterinarios.Add(new Veterinario(
                            lector.GetString(0),
                            lector.GetString(1),
                            lector.GetString(2)));
                    }
                }
            }

            return veterinarios;
        }

        public static List<Producto> CargarProductos()
        {
            List<Producto> productos = new List<Producto>();

            using (SqliteConnection conexion = BaseDatos.AbrirConexion())
            using (SqliteCommand cmd = conexion.CreateCommand())
            {
                cmd.CommandText = "SELECT id, nombre, stock, precio FROM productos;";

                using (SqliteDataReader lector = cmd.ExecuteReader())
                {
                    while (lector.Read())
                    {
                        productos.Add(new Producto(
                            lector.GetString(0),
                            lector.GetString(1),
                            lector.GetInt32(2),
                            lector.GetDouble(3)));
                    }
                }
            }

            return productos;
        }

        public static List<Cliente> CargarClientes()
        {
            List<Cliente> clientes = new List<Cliente>();

            using (SqliteConnection conexion = BaseDatos.AbrirConexion())
            using (SqliteCommand cmd = conexion.CreateCommand())
            {
                cmd.CommandText = "SELECT id, nombre, telefono, direccion FROM clientes;";

                using (SqliteDataReader lector = cmd.ExecuteReader())
                {
                    while (lector.Read())
                    {
                        clientes.Add(new Cliente(
                            lector.GetString(0),
                            lector.GetString(1),
                            lector.GetString(2),
                            lector.GetString(3)));
                    }
                }
            }

            return clientes;
        }

        /// <summary>
        /// Carga todas las mascotas y las vincula a su cliente correspondiente.
        /// </summary>
        public static List<Mascota> CargarMascotas(List<Cliente> clientes)
        {
            List<Mascota> mascotas = new List<Mascota>();

            using (SqliteConnection conexion = BaseDatos.AbrirConexion())
            using (SqliteCommand cmd = conexion.CreateCommand())
            {
                cmd.CommandText = "SELECT id, nombre, especie, raza, edad, id_cliente FROM mascotas;";

                using (SqliteDataReader lector = cmd.ExecuteReader())
                {
                    while (lector.Read())
                    {
                        Mascota mascota = new Mascota(
                            lector.GetString(0),
                            lector.GetString(1),
                            lector.GetString(2),
                            lector.GetString(3),
                            lector.GetString(4));

                        mascotas.Add(mascota);

                        string idCliente = lector.GetString(5);
                        Cliente? duenio = clientes.Find(c => c.IdCliente == idCliente);
                        duenio?.AgregarMascota(mascota);
                    }
                }
            }

            return mascotas;
        }

        /// <summary>
        /// Carga todas las consultas y las vincula al historial de su mascota.
        /// </summary>
        public static void CargarConsultas(List<Mascota> mascotas)
        {
            using (SqliteConnection conexion = BaseDatos.AbrirConexion())
            using (SqliteCommand cmd = conexion.CreateCommand())
            {
                cmd.CommandText = @"
                    SELECT id, id_mascota, fecha, peso, temperatura,
                           sintomas, diagnostico, farmaco, dosis, frecuencia
                    FROM consultas;";

                using (SqliteDataReader lector = cmd.ExecuteReader())
                {
                    while (lector.Read())
                    {
                        Consulta consulta = new Consulta(
                            lector.GetString(0),
                            lector.GetString(2),
                            lector.GetDouble(3),
                            lector.GetDouble(4),
                            lector.GetString(5),
                            lector.GetString(6),
                            lector.GetString(7),
                            lector.GetString(8),
                            lector.GetString(9));

                        string idMascota = lector.GetString(1);
                        Mascota? mascota = mascotas.Find(m => m.IdMascota == idMascota);
                        mascota?.AgregarConsulta(consulta);
                    }
                }
            }
        }

        /// <summary>
        /// Carga todas las citas vinculando cada una con su mascota y veterinario.
        /// </summary>
        public static List<Cita> CargarCitas(List<Mascota> mascotas, List<Veterinario> veterinarios)
        {
            List<Cita> citas = new List<Cita>();

            using (SqliteConnection conexion = BaseDatos.AbrirConexion())
            using (SqliteCommand cmd = conexion.CreateCommand())
            {
                cmd.CommandText = "SELECT id, fecha, hora, id_mascota, id_veterinario FROM citas;";

                using (SqliteDataReader lector = cmd.ExecuteReader())
                {
                    while (lector.Read())
                    {
                        string idMascota = lector.GetString(3);
                        string idVeterinario = lector.GetString(4);

                        Mascota? mascota = mascotas.Find(m => m.IdMascota == idMascota);
                        Veterinario? veterinario = veterinarios.Find(v => v.IdVeterinario == idVeterinario);

                        if (mascota != null && veterinario != null)
                        {
                            citas.Add(new Cita(
                                lector.GetString(0),
                                lector.GetString(1),
                                lector.GetString(2),
                                mascota,
                                veterinario));
                        }
                    }
                }
            }

            return citas;
        }
    }
}
