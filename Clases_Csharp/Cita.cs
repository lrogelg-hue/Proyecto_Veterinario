namespace SistemaVeterinaria
{
    public class Cita
    {
        public string IdCita { get; private set; }
        public string Fecha { get; private set; }
        public string Hora { get; private set; }
        public Mascota Mascota { get; private set; }
        public Veterinario Veterinario { get; private set; }
        public bool Ocupado { get; private set; }

        public Cita(string idCita, string fecha, string hora, Mascota mascota, Veterinario veterinario)
        {
            IdCita = idCita;
            Fecha = fecha;
            Hora = hora;
            Mascota = mascota;
            Veterinario = veterinario;
            Ocupado = true;
        }
    }
}