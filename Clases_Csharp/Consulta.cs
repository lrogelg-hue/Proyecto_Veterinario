using System;

namespace SistemaVeterinaria
{
    public class Consulta
    {
        public string IdConsulta { get; private set; }
        public string Fecha { get; private set; }
        public double Peso { get; private set; }
        public double Temperatura { get; private set; }
        public string Sintomas { get; private set; }
        public string Diagnostico { get; private set; }
        public string Farmaco { get; private set; }
        public string Dosis { get; private set; }
        public string Frecuencia { get; private set; }

        public Consulta(string idConsulta, string fecha, double peso, double temperatura,
                        string sintomas, string diagnostico, string farmaco, string dosis, string frecuencia)
        {
            IdConsulta = idConsulta;
            Fecha = fecha;
            Peso = peso;
            Temperatura = temperatura;
            Sintomas = sintomas;
            Diagnostico = diagnostico;
            Farmaco = farmaco;
            Dosis = dosis;
            Frecuencia = frecuencia;
        }

        public void MostrarResumen()
        {
            Console.WriteLine("Consulta ID: " + IdConsulta);
            Console.WriteLine("Fecha: " + Fecha + " | Diagnóstico: " + Diagnostico);
            Console.WriteLine("Signos vitales -> Peso: " + Peso + "kg, Temp: " + Temperatura + "°C");
            Console.WriteLine("Síntomas: " + Sintomas);
            if (!string.IsNullOrEmpty(Farmaco))
            {
                Console.WriteLine("Receta: " + Farmaco + " | Dosis: " + Dosis + " | Frecuencia: " + Frecuencia);
            }
        }
    }
}