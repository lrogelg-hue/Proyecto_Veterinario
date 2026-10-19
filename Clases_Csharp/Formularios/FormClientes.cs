using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using SistemaVeterinaria.Datos;

namespace SistemaVeterinaria.Formularios
{
    /// <summary>
    /// CRUD completo de Propietarios (Clientes): Create, Read, Update y Delete
    /// con validación de campos y limpieza tras cada operación.
    /// </summary>
    public class FormClientes : Form
    {
        private readonly TextBox txtNombre = new TextBox();
        private readonly TextBox txtTelefono = new TextBox();
        private readonly TextBox txtDireccion = new TextBox();
        private readonly DataGridView dgvClientes = new DataGridView();
        private readonly Button btnAgregar = new Button();
        private readonly Button btnActualizar = new Button();
        private readonly Button btnEliminar = new Button();
        private readonly Button btnLimpiar = new Button();

        public FormClientes()
        {
            Text = "Gestión de Propietarios (Clientes)";
            ClientSize = new Size(720, 470);
            StartPosition = FormStartPosition.CenterScreen;
            FormBorderStyle = FormBorderStyle.FixedSingle;
            MaximizeBox = false;

            AgregarEtiqueta("Nombre completo:", 20, 25);
            txtNombre.SetBounds(160, 22, 300, 25);

            AgregarEtiqueta("Teléfono:", 20, 60);
            txtTelefono.SetBounds(160, 57, 300, 25);

            AgregarEtiqueta("Dirección:", 20, 95);
            txtDireccion.SetBounds(160, 92, 300, 25);

            ConfigurarBoton(btnAgregar, "Agregar", 20, 135, btnAgregar_Click);
            ConfigurarBoton(btnActualizar, "Actualizar", 130, 135, btnActualizar_Click);
            ConfigurarBoton(btnEliminar, "Eliminar", 250, 135, btnEliminar_Click);
            ConfigurarBoton(btnLimpiar, "Limpiar", 370, 135, (s, e) => LimpiarCampos());

            dgvClientes.SetBounds(20, 180, 670, 260);
            ConfigurarGrid(dgvClientes);
            dgvClientes.CellClick += DgvClientes_CellClick;

            Controls.AddRange(new Control[]
            {
                txtNombre, txtTelefono, txtDireccion,
                btnAgregar, btnActualizar, btnEliminar, btnLimpiar, dgvClientes
            });

            CargarGrid();
        }

        // ---------- READ: llena el DataGridView desde el repositorio ----------
        private void CargarGrid()
        {
            List<Cliente> clientes = RepositorioVeterinaria.CargarClientes();
            dgvClientes.DataSource = null;
            dgvClientes.DataSource = clientes;
            dgvClientes.Columns["Mascotas"]?.Visible = false;   // columna de objetos (no se muestra)
            dgvClientes.ClearSelection();
        }

        // ---------- Al hacer clic en una fila, se carga en los TextBox ----------
        private void DgvClientes_CellClick(object? sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return;
            if (dgvClientes.Rows[e.RowIndex].DataBoundItem is not Cliente c) return;
            txtNombre.Text = c.Nombre;
            txtTelefono.Text = c.Telefono;
            txtDireccion.Text = c.Direccion;
        }

        // ---------- CREATE ----------
        private void btnAgregar_Click(object? sender, EventArgs e)
        {
            if (Validacion.CamposVacios(txtNombre.Text, txtTelefono.Text, txtDireccion.Text))
            {
                MessageBox.Show("Todos los campos son obligatorios.",
                    "Validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            try
            {
                string id = RepositorioVeterinaria.SiguienteId("clientes", "C");
                Cliente nuevo = new Cliente(id, txtNombre.Text.Trim(), txtTelefono.Text.Trim(), txtDireccion.Text.Trim());
                RepositorioVeterinaria.InsertarCliente(nuevo);
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
            Cliente? seleccionado = dgvClientes.CurrentRow?.DataBoundItem as Cliente;
            if (seleccionado == null)
            {
                MessageBox.Show("Seleccione un cliente en la tabla.",
                    "Validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            if (Validacion.CamposVacios(txtNombre.Text, txtTelefono.Text, txtDireccion.Text))
            {
                MessageBox.Show("Todos los campos son obligatorios.",
                    "Validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            try
            {
                Cliente actualizado = new Cliente(seleccionado.IdCliente, txtNombre.Text.Trim(),
                                                  txtTelefono.Text.Trim(), txtDireccion.Text.Trim());
                RepositorioVeterinaria.ActualizarCliente(actualizado);
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
            Cliente? seleccionado = dgvClientes.CurrentRow?.DataBoundItem as Cliente;
            if (seleccionado == null)
            {
                MessageBox.Show("Seleccione un cliente en la tabla.",
                    "Validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            DialogResult confirmacion = MessageBox.Show(
                $"¿Seguro que desea eliminar a {seleccionado.Nombre}?",
                "Confirmar eliminación", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (confirmacion != DialogResult.Yes) return;

            try
            {
                if (!RepositorioVeterinaria.EliminarCliente(seleccionado.IdCliente))
                {
                    MessageBox.Show("No se puede eliminar: el cliente aún tiene mascotas registradas.",
                        "Operación cancelada", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
                CargarGrid();
                LimpiarCampos();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al eliminar: " + ex.Message, "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // ---------- Limpieza del formulario tras cada operación ----------
        private void LimpiarCampos()
        {
            txtNombre.Clear();
            txtTelefono.Clear();
            txtDireccion.Clear();
            dgvClientes.ClearSelection();
            txtNombre.Focus();
        }

        // ---------- Auxiliares de diseño ----------
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

        internal static void ConfigurarGrid(DataGridView grid)
        {
            grid.ReadOnly = true;
            grid.AllowUserToAddRows = false;
            grid.AllowUserToDeleteRows = false;
            grid.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            grid.MultiSelect = false;
            grid.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            grid.RowHeadersVisible = false;
        }
    }
}
