public class Consulta {
    private String idConsulta;
    private String fecha;
    private double peso;
    private double temperatura;
    private String sintomas;
    private String diagnostico;
    private String farmaco;
    private String dosis;
    private String frecuencia;

    public Consulta(String idConsulta, String fecha, double peso, double temperatura, 
                    String sintomas, String diagnostico, String farmaco, String dosis, String frecuencia) {
        this.idConsulta = idConsulta;
        this.fecha = fecha;
        this.peso = peso;
        this.temperatura = temperatura;
        this.sintomas = sintomas;
        this.diagnostico = diagnostico;
        this.farmaco = farmaco;
        this.dosis = dosis;
        this.frecuencia = frecuencia;
    }

    public void mostrarResumen() {
        System.out.println("Consulta ID: " + idConsulta);
        System.out.println("Fecha: " + fecha + " | Diagnóstico: " + diagnostico);
        System.out.println("Signos vitales -> Peso: " + peso + "kg, Temp: " + temperatura + "°C");
        System.out.println("Síntomas: " + sintomas);
        if (farmaco != null && !farmaco.isEmpty()) {
            System.out.println("Receta: " + farmaco + " | Dosis: " + dosis + " | Frecuencia: " + frecuencia);
        }
    }

    public String getIdConsulta() { return idConsulta; }
    public String getFecha() { return fecha; }
    public String getDiagnostico() { return diagnostico; }
}