namespace SistemaVeterinaria
{
    public class Producto
    {
        public string IdProducto { get; private set; }
        public string Nombre { get; private set; }
        public int CantidadStock { get; private set; }
        public double PrecioUnitario { get; private set; }

        public Producto(string idProducto, string nombre, int cantidadStock, double precioUnitario)
        {
            IdProducto = idProducto;
            Nombre = nombre;
            CantidadStock = cantidadStock;
            PrecioUnitario = precioUnitario;
        }

        public void Reabastecer(int cantidad)
        {
            CantidadStock += cantidad;
        }

        public bool ReducirStock(int cantidad)
        {
            if (CantidadStock >= cantidad)
            {
                CantidadStock -= cantidad;
                return true;
            }
            return false;
        }
    }
}