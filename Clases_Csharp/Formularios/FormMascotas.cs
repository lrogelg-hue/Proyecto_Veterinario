using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using SistemaVeterinaria.Datos;

namespace SistemaVeterinaria.Formularios
{
    /// <summary>
    /// CRUD de Mascotas con selector de propietario (combo) y validación de campos.
    /// </summary>
    public class FormMascotas : Form
    {
        private readonly ComboBox cboClientes = new ComboBox();
        private readonly TextBox txtNombre = new TextBox();
        private readonly TextBox txtEspecie = new TextBox();
        private readonly TextBox txtRaza = new TextBox();
        private readonly TextBox txtEdad = new TextBox();
        private readonly DataGridView dgvMascotas = new DataGridView();
        private readonly Button btnAgregar = new Button();
        private readonly Button btnActualizar = new Button();
        private readonly Button btnEliminar = new Button();
        private readonly Button btnLimpiar = new Button();

        private List<Cliente> _clientes = new List<Cliente>();

        public FormMascotas()
        {
            Text = "Gestión de Mascotas";
            ClientSize = new Size(760, 500);
            StartPosition = FormStartPosition.CenterScreen;
            FormBorderStyle = FormBorderStyle.FixedSingle;
            MaximizeBox = false;

            AgregarEtiqueta("Propietario:", 20, 25);
            cboClientes.SetBounds(140, 22, 240, 25);
            cboClientes.DropDownStyle = ComboBoxStyle.DropDownList;
            cboClientes.DisplayMember = "Nombre";

            AgregarEtiqueta("Nombre:", 20, 60);
            txtNombre.SetBounds(140, 57, 240, 25);

            AgregarEtiqueta("Especie:", 400, 25);
            txtEspecie.SetBounds(490, 22, 240, 25);

            AgregarEtiqueta("Raza:", 400, 60);
            txtRaza.SetBounds(490, 57, 240, 25);

            AgregarEtiqueta("Edad:", 400, 95);
            txtEdad.SetBounds(490, 92, 240, 25);

            AgregarEtiqueta("Ej. edad: 3 años / 5 meses", 20, 95);

            ConfigurarBoton(btnAgregar, "Agregar", 20, 135, btnAgregar_Click);
            ConfigurarBoton(btnActualizar, "Actualizar", 130, 135, btnActualizar_Click);
            ConfigurarBoton(btnEliminar, "Eliminar", 250, 135, btnEliminar_Click);
            ConfigurarBoton(btnLimpiar, "Limpiar", 370, 135, (s, e) => LimpiarCampos());

            dgvMascotas.SetBounds(20, 180, 710, 290);
            FormClientes.ConfigurarGrid(dgvMascotas);
            dgvMascotas.CellClick += DgvMascotas_CellClick;

            Controls.AddRange(new Control[]
            {
                cboClientes, txtNombre, txtEspecie, txtRaza, txtEdad,
                btnAgregar, btnActualizar, btnEliminar, btnLimpiar, dgvMascotas
            });

            CargarDatos();
        }

        // ---------- READ ----------
        private void CargarDatos()
        {
            _clientes = RepositorioVeterinaria.CargarClientes();
            // CargarMascotas vincula cada mascota a su cliente y la devuelve para la tabla
            List<Mascota> mascotas = RepositorioVeterinaria.CargarMascotas(_clientes);

            cboClientes.DataSource = null;
            cboClientes.DataSource = _clientes;

            dgvMascotas.DataSource = null;
            dgvMascotas.DataSource = mascotas;
            dgvMascotas.Columns["HistorialClinico"]?.Visible = false;   // columna de objetos
            dgvMascotas.ClearSelection();
        }

        private void DgvMascotas_CellClick(object? sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return;
            if (dgvMascotas.Rows[e.RowIndex].DataBoundItem is not Mascota m) return;

            txtNombre.Text = m.Nombre;
            txtEspecie.Text = m.Especie;
            txtRaza.Text = m.Raza;
            txtEdad.Text = m.Edad;

            // Selecciona al propietario actual en el combo
            Cliente? dueno = _clientes.FirstOrDefault(c => c.Mascotas.Any(x => x.IdMascota == m.IdMascota));
            if (dueno != null) cboClientes.SelectedItem = dueno;
        }

        // ---------- CREATE ----------
        private void btnAgregar_Click(object? sender, EventArgs e)
        {
            if (Validacion.CamposVacios(txtNombre.Text, txtEspecie.Text, txtRaza.Text, txtEdad.Text))
            {
                MessageBox.Show("Todos los campos son obligatorios.",
                    "Validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            if (cboClientes.SelectedItem is not Cliente dueno)
            {
                MessageBox.Show("Seleccione un propietario.",
                    "Validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            try
            {
                string id = RepositorioVeterinaria.SiguienteId("pacientes", "M");
                Mascota nueva = new Mascota(id, txtNombre.Text.Trim(), txtEspecie.Text.Trim(),
                                            txtRaza.Text.Trim(), txtEdad.Text.Trim());
                RepositorioVeterinaria.InsertarMascota(nueva, dueno.IdCliente);
                CargarDatos();
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
            Mascota? seleccionada = dgvMascotas.CurrentRow?.DataBoundItem as Mascota;
            if (seleccionada == null)
            {
                MessageBox.Show("Seleccione una mascota en la tabla.",
                    "Validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            if (Validacion.CamposVacios(txtNombre.Text, txtEspecie.Text, txtRaza.Text, txtEdad.Text))
            {
                MessageBox.Show("Todos los campos son obligatorios.",
                    "Validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            if (cboClientes.SelectedItem is not Cliente dueno)
            {
                MessageBox.Show("Seleccione un propietario.",
                    "Validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            try
            {
                Mascota actualizada = new Mascota(seleccionada.IdMascota, txtNombre.Text.Trim(),
                                                  txtEspecie.Text.Trim(), txtRaza.Text.Trim(),
                                                  txtEdad.Text.Trim());
                RepositorioVeterinaria.ActualizarMascota(actualizada, dueno.IdCliente);
                CargarDatos();
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
            Mascota? seleccionada = dgvMascotas.CurrentRow?.DataBoundItem as Mascota;
            if (seleccionada == null)
            {
                MessageBox.Show("Seleccione una mascota en la tabla.",
                    "Validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            DialogResult confirmacion = MessageBox.Show(
                $"¿Seguro que desea eliminar a {seleccionada.Nombre}?\n" +
                "Se borrará también su historial clínico y sus citas.",
                "Confirmar eliminación", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (confirmacion != DialogResult.Yes) return;

            try
            {
                RepositorioVeterinaria.EliminarMascota(seleccionada.IdMascota);
                CargarDatos();
                LimpiarCampos();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al eliminar: " + ex.Message, "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void LimpiarCampos()
        {
            txtNombre.Clear();
            txtEspecie.Clear();
            txtRaza.Clear();
            txtEdad.Clear();
            dgvMascotas.ClearSelection();
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
