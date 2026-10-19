using System;
using System.Globalization;

namespace SistemaVeterinaria
{
    /// <summary>
    /// Validaciones compartidas entre la interfaz de consola y los formularios WinForms.
    /// </summary>
    public static class Validacion
    {
        /// <summary>Verifica que la fecha tenga el formato DD/MM/AAAA y sea una fecha real.</summary>
        public static bool FechaValida(string texto)
        {
            return DateTime.TryParseExact(texto, "dd/MM/yyyy",
                CultureInfo.InvariantCulture, DateTimeStyles.None, out _);
        }

        /// <summary>Verifica que el texto sea un número decimal mayor a cero.</summary>
        public static bool NumeroPositivo(string? texto, out double valor)
        {
            return double.TryParse(texto, out valor) && valor > 0;
        }

        /// <summary>Verifica que el texto sea un entero mayor a cero.</summary>
        public static bool EnteroPositivo(string? texto, out int valor)
        {
            return int.TryParse(texto, out valor) && valor > 0;
        }

        /// <summary>Devuelve true si alguno de los campos está vacío o solo contiene espacios.</summary>
        public static bool CamposVacios(params string?[] campos)
        {
            foreach (string? campo in campos)
            {
                if (string.IsNullOrWhiteSpace(campo)) return true;
            }
            return false;
        }
    }
}
