using System.Collections.Generic;

namespace SistemaVeterinaria
{
    public class Mascota
    {
        public string IdMascota { get; private set; }
        public string Nombre { get; private set; }
        public string Especie { get; private set; }
        public string Raza { get; private set; }
        public string Edad { get; private set; }
        public List<Consulta> HistorialClinico { get; private set; }

        // El constructor ahora recibe 'string edad'
        public Mascota(string idMascota, string nombre, string especie, string raza, string edad)
        {
            IdMascota = idMascota;
            Nombre = nombre;
            Especie = especie;
            Raza = raza;
            Edad = edad;
            HistorialClinico = new List<Consulta>();
        }

        public void AgregarConsulta(Consulta consulta)
        {
            HistorialClinico.Add(consulta);
        }
    }
}