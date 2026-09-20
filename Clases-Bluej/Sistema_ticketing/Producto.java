public class Producto {
    private String idProducto;
    private String nombre;
    private int cantidadStock;
    private double precioUnitario;

    public Producto(String idProducto, String nombre, int cantidadStock, double precioUnitario) {
        this.idProducto = idProducto;
        this.nombre = nombre;
        this.cantidadStock = cantidadStock;
        this.precioUnitario = precioUnitario;
    }

    public void reabastecer(int cantidad) {
        this.cantidadStock += cantidad;
    }

    public boolean reducirStock(int cantidad) {
        if (this.cantidadStock >= cantidad) {
            this.cantidadStock -= cantidad;
            return true;
        }
        return false;
    }

    public String getIdProducto() { return idProducto; }
    public String getNombre() { return nombre; }
    public int getCantidadStock() { return cantidadStock; }
    public double getPrecioUnitario() { return precioUnitario; }
}