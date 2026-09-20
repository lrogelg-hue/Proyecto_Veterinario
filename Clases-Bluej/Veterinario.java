public class Veterinario {
    private String idVeterinario;
    private String nombre;
    private String especialidad;

    public Veterinario(String idVeterinario, String nombre, String especialidad) {
        this.idVeterinario = idVeterinario;
        this.nombre = nombre;
        this.especialidad = especialidad;
    }

    public String getIdVeterinario() { return idVeterinario; }
    public String getNombre() { return nombre; }
    public String getEspecialidad() { return especialidad; }
}