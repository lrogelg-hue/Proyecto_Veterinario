import java.util.ArrayList;

public class GestorReportes {

    public void generarReporteCitas(ArrayList<Cita> citas) {
        System.out.println("--- REPORTE DE CITAS REGISTRADAS ---");
        for (Cita c : citas) {
            System.out.println("Fecha: " + c.getFecha() + " | Hora: " + c.getHora() + 
                               " | Mascota: " + c.getMascota().getNombre() + 
                               " | Veterinario: " + c.getVeterinario().getNombre());
        }
    }

    public void generarReporteInventarioBajo(ArrayList<Producto> inventario, int limiteMinimo) {
        System.out.println("--- REPORTE DE PRODUCTOS CON STOCK BAJO ---");
        for (Producto p : inventario) {
            if (p.getCantidadStock() <= limiteMinimo) {
                System.out.println("⚠️ Alerta: " + p.getNombre() + " | Stock actual: " + p.getCantidadStock());
            }
        }
    }
}