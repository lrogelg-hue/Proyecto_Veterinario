using System.Collections.Generic;

namespace SistemaVeterinaria
{
    public class Cliente
    {
        public string IdCliente { get; private set; }
        public string Nombre { get; private set; }
        public string Telefono { get; private set; }
        public string Direccion { get; private set; }
        public List<Mascota> Mascotas { get; private set; }

        public Cliente(string idCliente, string nombre, string telefono, string direccion)
        {
            IdCliente = idCliente;
            Nombre = nombre;
            Telefono = telefono;
            Direccion = direccion;
            Mascotas = new List<Mascota>();
        }

        public void AgregarMascota(Mascota mascota)
        {
            Mascotas.Add(mascota);
        }
    }
}