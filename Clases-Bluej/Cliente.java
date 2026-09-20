import java.util.ArrayList;

public class Cliente {
    private String idCliente;
    private String nombre;
    private String telefono;
    private String direccion;
    private ArrayList<Mascota> mascotas;

    public Cliente(String idCliente, String nombre, String telefono, String direccion) {
        this.idCliente = idCliente;
        this.nombre = nombre;
        this.telefono = telefono;
        this.direccion = direccion;
        this.mascotas = new ArrayList<>();
    }

    public void agregarMascota(Mascota mascota) {
        this.mascotas.add(mascota);
    }

    public String getIdCliente() { return idCliente; }
    public String getNombre() { return nombre; }
    public String getTelefono() { return telefono; }
    public String getDireccion() { return direccion; }
    public ArrayList<Mascota> getMascotas() { return mascotas; }
}