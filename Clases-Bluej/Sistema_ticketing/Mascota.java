import java.util.ArrayList;

public class Mascota {
    private String idMascota;
    private String nombre;
    private String especie;
    private String raza;
    private int edad;
    private ArrayList<Consulta> historialClinico;

    public Mascota(String idMascota, String nombre, String especie, String raza, int edad) {
        this.idMascota = idMascota;
        this.nombre = nombre;
        this.especie = especie;
        this.raza = raza;
        this.edad = edad;
        this.historialClinico = new ArrayList<>();
    }

    public void agregarConsulta(Consulta consulta) {
        this.historialClinico.add(consulta);
    }

    public String getIdMascota() { return idMascota; }
    public String getNombre() { return nombre; }
    public String getEspecie() { return especie; }
    public String getRaza() { return raza; }
    public int getEdad() { return edad; }
    public ArrayList<Consulta> getHistorialClinico() { return historialClinico; }
}