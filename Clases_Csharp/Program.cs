using System;
using System.Collections.Generic;

namespace SistemaVeterinaria
{
    class Program
    {
        static void Main(string[] args)
        {
            List<Cliente> clientes = new List<Cliente>();
            List<Mascota> mascotasGlobal = new List<Mascota>();
            List<Veterinario> veterinarios = new List<Veterinario>();
            List<Cita> citas = new List<Cita>();
            List<Producto> inventario = new List<Producto>();
            GestorReportes gestorReportes = new GestorReportes();

            // Datos por defecto
            veterinarios.Add(new Veterinario("V001", "Dra. Sofia Morales", "Cirugía y Medicina General"));
            inventario.Add(new Producto("P001", "Protector Gástrico", 3, 45.00));
            inventario.Add(new Producto("P002", "Antipulgas", 10, 120.00));

            bool salir = false;

            while (!salir)
            {
                Console.Clear();
                Console.WriteLine("=========================================");
                Console.WriteLine("   SISTEMA CLÍNICO VETERINARIO - MENÚ    ");
                Console.WriteLine("=========================================");
                Console.WriteLine("1. Registrar Propietario (Cliente)");
                Console.WriteLine("2. Registrar Mascota a un Cliente");
                Console.WriteLine("3. Agendar Cita Médica");
                Console.WriteLine("4. Registrar Consulta Médica (Atención)");
                Console.WriteLine("5. Consultar Historial Clínico de una Mascota");
                Console.WriteLine("6. Gestionar Inventario (Ver / Reabastecer)");
                Console.WriteLine("7. Generar Reportes");
                Console.WriteLine("8. Salir");
                Console.Write("\nSeleccione una opción (1-8): ");

                string opcion = Console.ReadLine();
                Console.WriteLine();

                switch (opcion)
                {
                    case "1":
                        Console.WriteLine("--- REGISTRO DE PROPIETARIO ---");

                        // ID generado automáticamente por el sistema
                        string idCliente = "C00" + (clientes.Count + 1);

                        Console.Write("Nombre completo: ");
                        string nombreCli = Console.ReadLine();
                        Console.Write("Teléfono: ");
                        string telefono = Console.ReadLine();
                        Console.Write("Dirección: ");
                        string direccion = Console.ReadLine();

                        Cliente nuevoCliente = new Cliente(idCliente, nombreCli, telefono, direccion);
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
                        string idBuscado = Console.ReadLine();
                        Cliente clienteEncontrado = clientes.Find(c => c.IdCliente.Equals(idBuscado, StringComparison.OrdinalIgnoreCase));

                        if (clienteEncontrado != null)
                        {
                            string idMascota = "M001" + (mascotasGlobal.Count + 1); // o el correlativo que prefieras

                            Console.Write("Nombre de la mascota: ");
                            string nombreMas = Console.ReadLine();
                            Console.Write("Especie (Perro/Gato/etc.): ");
                            string especie = Console.ReadLine();
                            Console.Write("Raza: ");
                            string raza = Console.ReadLine();

                            // AQUÍ ESTÁ EL CAMBIO: Recibe texto libre para admitir "6 meses", "1 año", etc.
                            Console.Write("Edad (ej. '6 meses' o '2 años'): ");
                            string edad = Console.ReadLine();

                            Mascota nuevaMascota = new Mascota(idMascota, nombreMas, especie, raza, edad);
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

                        string idCita = "CIT-00" + (citas.Count + 1);
                        Console.Write("Fecha de la cita (DD/MM/AAAA): ");
                        string fechaCita = Console.ReadLine();
                        Console.Write("Hora (ej. 10:30 AM): ");
                        string horaCita = Console.ReadLine();

                        Console.WriteLine("\nMascotas disponibles:");
                        foreach (var m in mascotasGlobal)
                        {
                            Console.WriteLine($"- ID: {m.IdMascota} | Nombre: {m.Nombre} ({m.Especie})");
                        }
                        Console.Write("Ingrese el ID de la mascota: ");
                        string idMasCita = Console.ReadLine();
                        Mascota mascotaCita = mascotasGlobal.Find(m => m.IdMascota.Equals(idMasCita, StringComparison.OrdinalIgnoreCase));

                        if (mascotaCita != null)
                        {
                            Veterinario vetAsignado = veterinarios[0]; // Veterinario por defecto
                            Cita nuevaCita = new Cita(idCita, fechaCita, horaCita, mascotaCita, vetAsignado);
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
                        string idMasCons = Console.ReadLine();
                        Mascota mascotaAtencion = mascotasGlobal.Find(m => m.IdMascota.Equals(idMasCons, StringComparison.OrdinalIgnoreCase));

                        if (mascotaAtencion != null)
                        {
                            string idCons = "CONS-00" + (mascotaAtencion.HistorialClinico.Count + 1);
                            Console.Write("Fecha de hoy: ");
                            string fechaCons = Console.ReadLine();
                            Console.Write("Peso (kg): ");
                            double peso = double.Parse(Console.ReadLine());
                            Console.Write("Temperatura (°C): ");
                            double temp = double.Parse(Console.ReadLine());
                            Console.Write("Síntomas: ");
                            string sintomas = Console.ReadLine();
                            Console.Write("Diagnóstico: ");
                            string diagnostico = Console.ReadLine();

                            Console.Write("¿Requiere medicamentos o receta? (s/n): ");
                            string recetaReq = Console.ReadLine();
                            string farmaco = "", dosis = "", frecuencia = "";

                            if (recetaReq.ToLower() == "s")
                            {
                                Console.Write("Nombre del fármaco: ");
                                farmaco = Console.ReadLine();
                                Console.Write("Dosis: ");
                                dosis = Console.ReadLine();
                                Console.Write("Frecuencia: ");
                                frecuencia = Console.ReadLine();
                            }

                            Consulta nuevaConsulta = new Consulta(idCons, fechaCons, peso, temp, sintomas, diagnostico, farmaco, dosis, frecuencia);
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
                        string idHistorial = Console.ReadLine();
                        Mascota masHistorial = mascotasGlobal.Find(m => m.IdMascota.Equals(idHistorial, StringComparison.OrdinalIgnoreCase));

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
                        if (Console.ReadLine().ToLower() == "s")
                        {
                            Console.Write("Ingrese el ID del producto (ej. P001): ");
                            string idProd = Console.ReadLine();
                            Producto prodEncontrado = inventario.Find(p => p.IdProducto.Equals(idProd, StringComparison.OrdinalIgnoreCase));
                            if (prodEncontrado != null)
                            {
                                Console.Write("Cantidad a sumar al stock: ");
                                int cant = int.Parse(Console.ReadLine());
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

                    case "8":
                        salir = true;
                        Console.WriteLine("Saliendo del sistema. ¡Hasta luego!");
                        break;

                    default:
                        Console.WriteLine("⚠️ Opción no válida. Intente de nuevo.");
                        break;
                }

                if (!salir)
                {
                    Console.Write("\nPresione cualquier tecla para regresar al menú...");
                    Console.ReadKey();
                }
            }
        }
    }
}