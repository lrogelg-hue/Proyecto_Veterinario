using System;
using System.Collections.Generic;
using Microsoft.Data.Sqlite;
using System.Windows.Forms;
using SistemaVeterinaria.Datos;
using SistemaVeterinaria.Formularios;

namespace SistemaVeterinaria
{
    class Program
    {
        [STAThread]
        static void Main(string[] args)
        {
            // Estilos de Windows Forms (se llama una sola vez, antes de crear controles)
            ApplicationConfiguration.Initialize();

            // Crea la base de datos SQLite (si no existe) y carga los datos guardados
            BaseDatos.Inicializar();

            List<Veterinario> veterinarios = RepositorioVeterinaria.CargarVeterinarios();
            List<Producto> inventario = RepositorioVeterinaria.CargarProductos();
            List<Cliente> clientes = RepositorioVeterinaria.CargarClientes();
            List<Mascota> mascotasGlobal = RepositorioVeterinaria.CargarMascotas(clientes);
            List<Cita> citas = RepositorioVeterinaria.CargarCitas(mascotasGlobal, veterinarios);
            RepositorioVeterinaria.CargarConsultas(mascotasGlobal);
            GestorReportes gestorReportes = new GestorReportes();

            bool salir = false;

            while (!salir)
            {
                LimpiarPantalla();
                Console.WriteLine("=========================================");
                Console.WriteLine("   SISTEMA CLÍNICO VETERINARIO - MENÚ    ");
                Console.WriteLine("=========================================");
                Console.WriteLine("📁 Base de datos: veterinaria.db (los datos se guardan al cerrar)");
                Console.WriteLine("-----------------------------------------");
                Console.WriteLine("1. Registrar Propietario (Cliente)");
                Console.WriteLine("2. Registrar Mascota a un Cliente");
                Console.WriteLine("3. Agendar Cita Médica");
                Console.WriteLine("4. Registrar Consulta Médica (Atención)");
                Console.WriteLine("5. Consultar Historial Clínico de una Mascota");
                Console.WriteLine("6. Gestionar Inventario (Ver / Reabastecer)");
                Console.WriteLine("7. Generar Reportes");
                Console.WriteLine("0. Abrir interfaz gráfica (Windows Forms)");
                Console.WriteLine("8. Salir");
                Console.Write("\nSeleccione una opción (0-8): ");

                string? opcionLeida = Console.ReadLine();
                if (opcionLeida == null)
                {
                    // Se cerró la entrada de la consola (fin de archivo): se sale del sistema
                    break;
                }
                string opcion = opcionLeida;
                Console.WriteLine();

                try
                {
                    switch (opcion)
                    {
                        case "1":
                            Console.WriteLine("--- REGISTRO DE PROPIETARIO ---");

                            // ID generado automáticamente por el sistema (correlativo desde la base de datos)
                            string idCliente = RepositorioVeterinaria.SiguienteId("clientes", "C");

                            string nombreCli = LeerTextoObligatorio("Nombre completo: ");
                            string telefono = LeerTextoObligatorio("Teléfono: ");
                            string direccion = LeerTextoObligatorio("Dirección: ");

                            Cliente nuevoCliente = new Cliente(idCliente, nombreCli.Trim(),
                                                              telefono.Trim(), direccion.Trim());
                            // Primero se guarda en la base de datos; si falla, no se toca la memoria
                            RepositorioVeterinaria.InsertarCliente(nuevoCliente);
                            clientes.Add(nuevoCliente);

                            Console.WriteLine($"\n¡Propietario registrado con éxito! (ID asignado: {idCliente})");
                            break;

                        case "2":
                            Console.WriteLine("--- REGISTRO DE MASCOTA ---");
                            if (clientes.Count == 0)
                            {
                                Console.WriteLine("⚠️ No hay clientes registrados. Debe registrar un cliente primero (Opción 1).");
                                break;
                            }

                            Console.WriteLine("Clientes registrados:");
                            foreach (var c in clientes)
                            {
                                Console.WriteLine($"- ID: {c.IdCliente} | Nombre: {c.Nombre} | Teléfono: {c.Telefono}");
                            }

                            Console.Write("\nIngrese el ID del cliente propietario (ej. C001): ");
                            string idBuscado = Console.ReadLine() ?? "";
                            Cliente? clienteEncontrado = clientes.Find(c => c.IdCliente.Equals(idBuscado, StringComparison.OrdinalIgnoreCase));

                            if (clienteEncontrado != null)
                            {
                                string idMascota = RepositorioVeterinaria.SiguienteId("pacientes", "M");

                                string nombreMas = LeerTextoObligatorio("Nombre de la mascota: ");
                                string especie = LeerTextoObligatorio("Especie (Perro/Gato/etc.): ");
                                string raza = LeerTextoObligatorio("Raza: ");

                                // Texto libre para admitir "6 meses", "1 año", etc.
                                string edad = LeerTextoObligatorio("Edad (ej. '6 meses' o '2 años'): ");

                                Mascota nuevaMascota = new Mascota(idMascota, nombreMas.Trim(), especie.Trim(),
                                                                   raza.Trim(), edad.Trim());
                                RepositorioVeterinaria.InsertarMascota(nuevaMascota, clienteEncontrado.IdCliente);
                                clienteEncontrado.AgregarMascota(nuevaMascota);
                                mascotasGlobal.Add(nuevaMascota);

                                Console.WriteLine($"\n¡Mascota '{nombreMas}' vinculada exitosamente a {clienteEncontrado.Nombre}! (ID asignado: {idMascota})");
                            }
                            else
                            {
                                Console.WriteLine("❌ Cliente no encontrado con ese ID.");
                            }
                            break;

                        case "3":
                            Console.WriteLine("--- AGENDAR CITA MÉDICA ---");
                            if (mascotasGlobal.Count == 0)
                            {
                                Console.WriteLine("⚠️ No hay mascotas registradas en el sistema.");
                                break;
                            }
                            if (veterinarios.Count == 0)
                            {
                                Console.WriteLine("⚠️ No hay veterinarios registrados en el sistema.");
                                break;
                            }

                            string idCita = RepositorioVeterinaria.SiguienteId("citas", "CIT-");
                            string fechaCita = LeerFecha("Fecha de la cita (DD/MM/AAAA): ");
                            string horaCita = LeerTextoObligatorio("Hora (ej. 10:30 AM): ");

                            Console.WriteLine("\nMascotas disponibles:");
                            foreach (var m in mascotasGlobal)
                            {
                                Console.WriteLine($"- ID: {m.IdMascota} | Nombre: {m.Nombre} ({m.Especie})");
                            }
                            Console.Write("Ingrese el ID de la mascota: ");
                            string idMasCita = Console.ReadLine() ?? "";
                            Mascota? mascotaCita = mascotasGlobal.Find(m => m.IdMascota.Equals(idMasCita, StringComparison.OrdinalIgnoreCase));

                            if (mascotaCita != null)
                            {
                                Veterinario vetAsignado = veterinarios[0]; // Veterinario por defecto
                                Cita nuevaCita = new Cita(idCita, fechaCita, horaCita, mascotaCita, vetAsignado);
                                RepositorioVeterinaria.InsertarCita(nuevaCita);
                                citas.Add(nuevaCita);
                                Console.WriteLine($"\n¡Cita agendada con éxito! (ID asignado: {idCita})");
                            }
                            else
                            {
                                Console.WriteLine("❌ Mascota no encontrada.");
                            }
                            break;

                        case "4":
                            Console.WriteLine("--- REGISTRAR CONSULTA MÉDICA ---");
                            if (mascotasGlobal.Count == 0)
                            {
                                Console.WriteLine("⚠️ No hay mascotas para atender.");
                                break;
                            }

                            Console.WriteLine("Mascotas registradas:");
                            foreach (var m in mascotasGlobal)
                            {
                                Console.WriteLine($"- ID: {m.IdMascota} | Nombre: {m.Nombre}");
                            }
                            Console.Write("Ingrese el ID de la mascota a atender: ");
                            string idMasCons = Console.ReadLine() ?? "";
                            Mascota? mascotaAtencion = mascotasGlobal.Find(m => m.IdMascota.Equals(idMasCons, StringComparison.OrdinalIgnoreCase));

                            if (mascotaAtencion != null)
                            {
                                string idCons = RepositorioVeterinaria.SiguienteId("consultas", "CONS-");

                                string fechaCons = LeerFecha("Fecha de la consulta (DD/MM/AAAA): ");

                                // Validación de números con re-intento (evita crash con texto vacío o inválido)
                                double peso = LeerNumeroPositivo("Peso (kg): ");
                                double temp = LeerNumeroPositivo("Temperatura (°C): ");

                                string sintomas = LeerTextoObligatorio("Síntomas: ");
                                string diagnostico = LeerTextoObligatorio("Diagnóstico: ");

                                Console.Write("¿Requiere medicamentos o receta? (s/n): ");
                                string recetaReq = Console.ReadLine() ?? "";
                                string farmaco = "", dosis = "", frecuencia = "";

                                if (recetaReq.ToLower() == "s")
                                {
                                    farmaco = LeerTextoObligatorio("Nombre del fármaco: ");
                                    dosis = LeerTextoObligatorio("Dosis: ");
                                    frecuencia = LeerTextoObligatorio("Frecuencia: ");
                                }

                                Consulta nuevaConsulta = new Consulta(idCons, fechaCons, peso, temp,
                                                                      sintomas.Trim(), diagnostico.Trim(),
                                                                      farmaco.Trim(), dosis.Trim(), frecuencia.Trim());
                                RepositorioVeterinaria.InsertarConsulta(nuevaConsulta, mascotaAtencion.IdMascota);
                                mascotaAtencion.AgregarConsulta(nuevaConsulta);

                                Console.WriteLine("\n¡Consulta registrada y guardada en el historial clínico de la mascota!");
                            }
                            else
                            {
                                Console.WriteLine("❌ Mascota no encontrada.");
                            }
                            break;

                        case "5":
                            Console.WriteLine("--- CONSULTAR HISTORIAL CLÍNICO ---");
                            Console.Write("Ingrese el ID de la mascota (ej. M001): ");
                            string idHistorial = Console.ReadLine() ?? "";
                            Mascota? masHistorial = mascotasGlobal.Find(m => m.IdMascota.Equals(idHistorial, StringComparison.OrdinalIgnoreCase));

                            if (masHistorial != null)
                            {
                                Console.WriteLine($"\nHistorial de: {masHistorial.Nombre} ({masHistorial.Especie}, {masHistorial.Raza})");
                                if (masHistorial.HistorialClinico.Count == 0)
                                {
                                    Console.WriteLine("La mascota no tiene consultas registradas.");
                                }
                                else
                                {
                                    foreach (var con in masHistorial.HistorialClinico)
                                    {
                                        Console.WriteLine("-----------------------------------");
                                        con.MostrarResumen();
                                    }
                                }
                            }
                            else
                            {
                                Console.WriteLine("❌ Mascota no encontrada.");
                            }
                            break;

                        case "6":
                            Console.WriteLine("--- GESTIÓN DE INVENTARIO ---");
                            Console.WriteLine("Inventario actual:");
                            foreach (var p in inventario)
                            {
                                Console.WriteLine($"- [{p.IdProducto}] {p.Nombre} | Stock: {p.CantidadStock} | Precio: Q{p.PrecioUnitario}");
                            }
                            Console.Write("\n¿Desea reabastecer algún producto? (s/n): ");
                            if ((Console.ReadLine() ?? "").ToLower() == "s")
                            {
                                Console.Write("Ingrese el ID del producto (ej. P001): ");
                                string idProd = Console.ReadLine() ?? "";
                                Producto? prodEncontrado = inventario.Find(p => p.IdProducto.Equals(idProd, StringComparison.OrdinalIgnoreCase));
                                if (prodEncontrado != null)
                                {
                                    int cant = LeerEnteroPositivo("Cantidad a sumar al stock: ");

                                    RepositorioVeterinaria.ActualizarStock(prodEncontrado.IdProducto, cant);
                                    prodEncontrado.Reabastecer(cant);
                                    Console.WriteLine("¡Stock actualizado con éxito!");
                                }
                                else
                                {
                                    Console.WriteLine("❌ Producto no encontrado.");
                                }
                            }
                            break;

                        case "7":
                            Console.WriteLine("--- REPORTES DEL SISTEMA ---");
                            gestorReportes.GenerarReporteCitas(citas);
                            Console.WriteLine();
                            gestorReportes.GenerarReporteInventarioBajo(inventario, 5);
                            break;

                        case "0":
                            // Interfaz gráfica (Windows Forms): se abre como ventana modal
                            Console.WriteLine("Abriendo la interfaz gráfica... (al cerrarla regresará a la consola)");
                            Application.Run(new FormPrincipal());

                            // Se recargan los datos por si se modificaron desde la interfaz gráfica
                            veterinarios = RepositorioVeterinaria.CargarVeterinarios();
                            inventario = RepositorioVeterinaria.CargarProductos();
                            clientes = RepositorioVeterinaria.CargarClientes();
                            mascotasGlobal = RepositorioVeterinaria.CargarMascotas(clientes);
                            citas = RepositorioVeterinaria.CargarCitas(mascotasGlobal, veterinarios);
                            RepositorioVeterinaria.CargarConsultas(mascotasGlobal);
                            Console.WriteLine("Interfaz gráfica cerrada. Datos recargados desde la base de datos.");
                            break;

                        case "8":
                            salir = true;
                            Console.WriteLine("Saliendo del sistema. ¡Hasta luego!");
                            break;

                        default:
                            Console.WriteLine("⚠️ Opción no válida. Intente de nuevo.");
                            break;
                    }
                }
                catch (SqliteException ex)
                {
                    Console.WriteLine($"❌ Error de la base de datos: {ex.Message}");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"❌ Error inesperado: {ex.Message}");
                }

                if (!salir)
                {
                    if (Console.IsInputRedirected)
                    {
                        // Entrada redirigida (pruebas automáticas): no hay tecla que esperar
                        Console.WriteLine();
                    }
                    else
                    {
                        Console.Write("\nPresione cualquier tecla para regresar al menú...");
                        Console.ReadKey();
                    }
                }
            }
        }

        // ------------------------------------------------------------------
        // LECTURA SEGURA DE DATOS DESDE LA CONSOLA
        // ------------------------------------------------------------------

        /// <summary>
        /// Pide un texto obligatorio: repite la petición mientras esté vacío.
        /// Si la entrada termina (EOF), cierra el sistema para no guardar datos incompletos.
        /// </summary>
        static string LeerTextoObligatorio(string etiqueta)
        {
            while (true)
            {
                Console.Write(etiqueta);
                string? texto = Console.ReadLine();
                if (texto == null) SalirPorFinDeEntrada();
                if (!string.IsNullOrWhiteSpace(texto!)) return texto!.Trim();
                Console.WriteLine("❌ El campo es obligatorio. Intente de nuevo.");
            }
        }

        /// <summary>
        /// Pide una fecha con formato DD/MM/AAAA, repitiendo mientras no sea válida.
        /// </summary>
        static string LeerFecha(string etiqueta)
        {
            while (true)
            {
                string fecha = LeerTextoObligatorio(etiqueta);
                if (Validacion.FechaValida(fecha)) return fecha;
                Console.WriteLine("❌ Fecha no válida. Use el formato DD/MM/AAAA (ej. 20/10/2026).");
            }
        }

        /// <summary>
        /// Pide un número decimal mayor a cero, repitiendo mientras no sea válido.
        /// </summary>
        static double LeerNumeroPositivo(string etiqueta)
        {
            while (true)
            {
                Console.Write(etiqueta);
                string? texto = Console.ReadLine();
                if (texto == null) SalirPorFinDeEntrada();
                if (double.TryParse(texto, out double valor) && valor > 0) return valor;
                Console.WriteLine($"❌ '{texto}' no es válido. Ingrese un número mayor a cero (ej. 12.5).");
            }
        }

        /// <summary>
        /// Pide un número entero mayor a cero, repitiendo mientras no sea válido.
        /// </summary>
        static int LeerEnteroPositivo(string etiqueta)
        {
            while (true)
            {
                Console.Write(etiqueta);
                string? texto = Console.ReadLine();
                if (texto == null) SalirPorFinDeEntrada();
                if (int.TryParse(texto, out int valor) && valor > 0) return valor;
                Console.WriteLine($"❌ '{texto}' no es válido. Ingrese un número entero mayor a cero.");
            }
        }

        /// <summary>
        /// Cierra el sistema cuando la entrada está cerrada (EOF),
        /// evitando bucles infinitos o registros con datos vacíos.
        /// </summary>
        static void SalirPorFinDeEntrada()
        {
            Console.WriteLine("\nFin de la entrada de la consola. Cerrando el sistema...");
            Environment.Exit(0);
        }

        /// <summary>
        /// Limpia la pantalla sin crashear si la consola no lo permite
        /// (por ejemplo, cuando la salida está redirigida en pruebas).
        /// </summary>
        static void LimpiarPantalla()
        {
            try
            {
                Console.Clear();
            }
            catch (IOException)
            {
                // La consola no soporta limpiar la pantalla; se continúa normalmente
            }
        }
    }
}
