namespace SistemaVeterinaria
{
    public class Veterinario
    {
        public string IdVeterinario { get; private set; }
        public string Nombre { get; private set; }
        public string Especialidad { get; private set; }

        public Veterinario(string idVeterinario, string nombre, string especialidad)
        {
            IdVeterinario = idVeterinario;
            Nombre = nombre;
            Especialidad = especialidad;
        }
    }
}