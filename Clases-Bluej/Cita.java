public class Cita {
    private String idCita;
    private String fecha;
    private String hora;
    private Mascota mascota;
    private Veterinario veterinario;
    private boolean ocupado;

    public Cita(String idCita, String fecha, String hora, Mascota mascota, Veterinario veterinario) {
        this.idCita = idCita;
        this.fecha = fecha;
        this.hora = hora;
        this.mascota = mascota;
        this.veterinario = veterinario;
        this.ocupado = true; // Indica que el horario está reservado
    }

    public String getIdCita() { return idCita; }
    public String getFecha() { return fecha; }
    public String getHora() { return hora; }
    public Mascota getMascota() { return mascota; }
    public Veterinario getVeterinario() { return veterinario; }
    public boolean isOcupado() { return ocupado; }
}