using System;
using System.Collections.Generic;

namespace SistemaVeterinaria
{
    public class GestorReportes
    {
        public void GenerarReporteCitas(List<Cita> citas)
        {
            Console.WriteLine("--- REPORTE DE CITAS REGISTRADAS ---");
            foreach (var c in citas)
            {
                Console.WriteLine($"Fecha: {c.Fecha} | Hora: {c.Hora} | Mascota: {c.Mascota.Nombre} | Veterinario: {c.Veterinario.Nombre}");
            }
        }

        public void GenerarReporteInventarioBajo(List<Producto> inventario, int limiteMinimo)
        {
            Console.WriteLine("--- REPORTE DE PRODUCTOS CON STOCK BAJO ---");
            foreach (var p in inventario)
            {
                if (p.CantidadStock <= limiteMinimo)
                {
                    Console.WriteLine($"⚠️ Alerta: {p.Nombre} | Stock actual: {p.CantidadStock}");
                }
            }
        }
    }
}