using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using SistemaVeterinaria.Datos;

namespace SistemaVeterinaria.Formularios
{
    /// <summary>
    /// CRUD de Inventario con reabastecimiento rápido y validación numérica
    /// de stock y precio.
    /// </summary>
    public class FormInventario : Form
    {
        private readonly TextBox txtNombre = new TextBox();
        private readonly TextBox txtStock = new TextBox();
        private readonly TextBox txtPrecio = new TextBox();
        private readonly DataGridView dgvProductos = new DataGridView();
        private readonly Button btnAgregar = new Button();
        private readonly Button btnActualizar = new Button();
        private readonly Button btnEliminar = new Button();
        private readonly Button btnReabastecer = new Button();
        private readonly Button btnLimpiar = new Button();

        public FormInventario()
        {
            Text = "Inventario de Productos";
            ClientSize = new Size(720, 470);
            StartPosition = FormStartPosition.CenterScreen;
            FormBorderStyle = FormBorderStyle.FixedSingle;
            MaximizeBox = false;

            AgregarEtiqueta("Nombre del producto:", 20, 25);
            txtNombre.SetBounds(170, 22, 300, 25);

            AgregarEtiqueta("Stock:", 20, 60);
            txtStock.SetBounds(170, 57, 90, 25);

            AgregarEtiqueta("Precio unitario:", 290, 60);
            txtPrecio.SetBounds(400, 57, 110, 25);

            ConfigurarBoton(btnAgregar, "Agregar", 20, 105, btnAgregar_Click);
            ConfigurarBoton(btnActualizar, "Actualizar", 130, 105, btnActualizar_Click);
            ConfigurarBoton(btnEliminar, "Eliminar", 250, 105, btnEliminar_Click);
            ConfigurarBoton(btnReabastecer, "+ Stock", 370, 105, btnReabastecer_Click);
            ConfigurarBoton(btnLimpiar, "Limpiar", 490, 105, (s, e) => LimpiarCampos());

            dgvProductos.SetBounds(20, 155, 670, 290);
            FormClientes.ConfigurarGrid(dgvProductos);
            dgvProductos.CellClick += DgvProductos_CellClick;

            Controls.AddRange(new Control[]
            {
                txtNombre, txtStock, txtPrecio,
                btnAgregar, btnActualizar, btnEliminar, btnReabastecer, btnLimpiar, dgvProductos
            });

            CargarGrid();
        }

        // ---------- READ ----------
        private void CargarGrid()
        {
            List<Producto> productos = RepositorioVeterinaria.CargarProductos();
            dgvProductos.DataSource = null;
            dgvProductos.DataSource = productos;
            dgvProductos.ClearSelection();
        }

        private void DgvProductos_CellClick(object? sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return;
            if (dgvProductos.Rows[e.RowIndex].DataBoundItem is not Producto p) return;
            txtNombre.Text = p.Nombre;
            txtStock.Text = p.CantidadStock.ToString();
            txtPrecio.Text = p.PrecioUnitario.ToString();
        }

        // ---------- Validación común ----------
        private bool DatosValidos()
        {
            if (Validacion.CamposVacios(txtNombre.Text, txtStock.Text, txtPrecio.Text))
            {
                MessageBox.Show("Nombre, stock y precio son obligatorios.",
                    "Validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }
            if (!Validacion.EnteroPositivo(txtStock.Text.Trim(), out _))
            {
                MessageBox.Show("El stock debe ser un número entero mayor a cero.",
                    "Validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }
            if (!Validacion.NumeroPositivo(txtPrecio.Text.Trim(), out _))
            {
                MessageBox.Show("El precio debe ser un número mayor a cero.",
                    "Validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }
            return true;
        }

        // ---------- CREATE ----------
        private void btnAgregar_Click(object? sender, EventArgs e)
        {
            if (!DatosValidos()) return;
            try
            {
                string id = RepositorioVeterinaria.SiguienteId("productos", "P");
                Producto nuevo = new Producto(id, txtNombre.Text.Trim(),
                                              int.Parse(txtStock.Text.Trim()),
                                              double.Parse(txtPrecio.Text.Trim()));
                RepositorioVeterinaria.InsertarProducto(nuevo);
                CargarGrid();
                LimpiarCampos();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al guardar: " + ex.Message, "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // ---------- UPDATE ----------
        private void btnActualizar_Click(object? sender, EventArgs e)
        {
            Producto? seleccionado = dgvProductos.CurrentRow?.DataBoundItem as Producto;
            if (seleccionado == null)
            {
                MessageBox.Show("Seleccione un producto en la tabla.",
                    "Validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            if (!DatosValidos()) return;
            try
            {
                Producto actualizado = new Producto(seleccionado.IdProducto, txtNombre.Text.Trim(),
                                                    int.Parse(txtStock.Text.Trim()),
                                                    double.Parse(txtPrecio.Text.Trim()));
                RepositorioVeterinaria.ActualizarProducto(actualizado);
                CargarGrid();
                LimpiarCampos();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al actualizar: " + ex.Message, "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // ---------- DELETE ----------
        private void btnEliminar_Click(object? sender, EventArgs e)
        {
            Producto? seleccionado = dgvProductos.CurrentRow?.DataBoundItem as Producto;
            if (seleccionado == null)
            {
                MessageBox.Show("Seleccione un producto en la tabla.",
                    "Validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            DialogResult confirmacion = MessageBox.Show(
                $"¿Seguro que desea eliminar {seleccionado.Nombre} del inventario?",
                "Confirmar eliminación", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (confirmacion != DialogResult.Yes) return;

            try
            {
                RepositorioVeterinaria.EliminarProducto(seleccionado.IdProducto);
                CargarGrid();
                LimpiarCampos();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al eliminar: " + ex.Message, "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // ---------- Reabastecimiento rápido: suma al stock sin pisarlo ----------
        private void btnReabastecer_Click(object? sender, EventArgs e)
        {
            Producto? seleccionado = dgvProductos.CurrentRow?.DataBoundItem as Producto;
            if (seleccionado == null)
            {
                MessageBox.Show("Seleccione un producto en la tabla.",
                    "Validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            string cantidadTexto = Microsoft.VisualBasic.Interaction.InputBox(
                $"¿Cuántas unidades de {seleccionado.Nombre} desea agregar?",
                "Reabastecer", "");
            if (string.IsNullOrWhiteSpace(cantidadTexto)) return;

            if (!Validacion.EnteroPositivo(cantidadTexto.Trim(), out int cantidad))
            {
                MessageBox.Show("La cantidad debe ser un número entero mayor a cero.",
                    "Validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            try
            {
                RepositorioVeterinaria.ActualizarStock(seleccionado.IdProducto, cantidad);
                CargarGrid();
                LimpiarCampos();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al reabastecer: " + ex.Message, "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void LimpiarCampos()
        {
            txtNombre.Clear();
            txtStock.Clear();
            txtPrecio.Clear();
            dgvProductos.ClearSelection();
            txtNombre.Focus();
        }

        private void AgregarEtiqueta(string texto, int x, int y)
        {
            Label etiqueta = new Label { Text = texto, Location = new Point(x, y), AutoSize = true };
            Controls.Add(etiqueta);
        }

        private void ConfigurarBoton(Button boton, string texto, int x, int y, EventHandler onClick)
        {
            boton.Text = texto;
            boton.SetBounds(x, y, 105, 32);
            boton.Click += onClick;
            Controls.Add(boton);
        }
    }
}
