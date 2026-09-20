import java.util.ArrayList;

public class Main {
    public static void main(String[] args) {
        System.out.println("=== INICIANDO PRUEBAS DEL SISTEMA VETERINARIO ===\n");

        // 1. Registrar cliente y mascota
        Cliente cliente1 = new Cliente("C001", "Carlos Gomez", "5544-3322", "Zona 10, Ciudad");
        Mascota mascota1 = new Mascota("M001", "Max", "Perro", "Golden Retriever", 2);
        cliente1.agregarMascota(mascota1);
        System.out.println("Cliente y mascota registrados con éxito.");

        // 2. Registrar un Veterinario
        Veterinario vet1 = new Veterinario("V001", "Dra. Sofia Morales", "Cirugía y Medicina General");
        System.out.println("Veterinario asignado: " + vet1.getNombre());

        // 3. Agendar una Cita
        Cita cita1 = new Cita("CIT-001", "20/09/2026", "10:30 AM", mascota1, vet1);
        ArrayList<Cita> listaCitas = new ArrayList<>();
        listaCitas.add(cita1);

        // 4. Registrar una Consulta Médica
        Consulta consulta1 = new Consulta(
            "CONS-001", "20/09/2026", 18.5, 38.7, 
            "Vómito y decaimiento", "Gastritis leve", 
            "Protector gástrico", "1 tableta", "Cada 24 horas"
        );
        mascota1.agregarConsulta(consulta1);

        // 5. Registrar productos en el inventario
        Producto prod1 = new Producto("P001", "Protector Gástrico", 3, 45.00);
        Producto prod2 = new Producto("P002", "Antipulgas", 12, 120.00);
        ArrayList<Producto> inventario = new ArrayList<>();
        inventario.add(prod1);
        inventario.add(prod2);

        // 6. Generar Reportes (Prueba de GestorReportes)
        System.out.println("\n-------------------------------------------");
        GestorReportes reportes = new GestorReportes();
        reportes.generarReporteCitas(listaCitas);
        
        System.out.println("\n-------------------------------------------");
        consulta1.mostrarResumen();

        System.out.println("\n-------------------------------------------");
        reportes.generarReporteInventarioBajo(inventario, 5); // Límite mínimo de alerta: 5

        System.out.println("\n=== PRUEBAS FINALIZADAS CON ÉXITO ===");
    }
}