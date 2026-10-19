using System;
using System.Collections.Generic;
using System.Text;

namespace SistemaVeterinaria
{
    public class GestorReportes
    {
        // Construye el reporte como texto (reutilizable desde consola y WinForms)
        public string ObtenerReporteCitas(List<Cita> citas)
        {
            StringBuilder texto = new StringBuilder();
            texto.AppendLine("--- REPORTE DE CITAS REGISTRADAS ---");
            foreach (var c in citas)
            {
                texto.AppendLine($"Fecha: {c.Fecha} | Hora: {c.Hora} | Mascota: {c.Mascota.Nombre} | Veterinario: {c.Veterinario.Nombre}");
            }
            if (citas.Count == 0)
            {
                texto.AppendLine("No hay citas registradas.");
            }
            return texto.ToString();
        }

        public string ObtenerReporteInventarioBajo(List<Producto> inventario, int limiteMinimo)
        {
            StringBuilder texto = new StringBuilder();
            texto.AppendLine("--- REPORTE DE PRODUCTOS CON STOCK BAJO ---");
            bool hayAlertas = false;
            foreach (var p in inventario)
            {
                if (p.CantidadStock <= limiteMinimo)
                {
                    texto.AppendLine($"⚠️ Alerta: {p.Nombre} | Stock actual: {p.CantidadStock}");
                    hayAlertas = true;
                }
            }
            if (!hayAlertas)
            {
                texto.AppendLine("Todos los productos tienen stock suficiente.");
            }
            return texto.ToString();
        }

        // Versiones para la interfaz de consola
        public void GenerarReporteCitas(List<Cita> citas)
        {
            Console.WriteLine(ObtenerReporteCitas(citas).TrimEnd());
        }

        public void GenerarReporteInventarioBajo(List<Producto> inventario, int limiteMinimo)
        {
            Console.WriteLine(ObtenerReporteInventarioBajo(inventario, limiteMinimo).TrimEnd());
        }
    }
}
