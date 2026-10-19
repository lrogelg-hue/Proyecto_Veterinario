using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using SistemaVeterinaria.Datos;

namespace SistemaVeterinaria.Formularios
{
    /// <summary>
    /// CRUD de Citas con validación de formato de fecha (DD/MM/AAAA) y hora.
    /// </summary>
    public class FormCitas : Form
    {
        private readonly ComboBox cboMascotas = new ComboBox();
        private readonly ComboBox cboVeterinarios = new ComboBox();
        private readonly TextBox txtFecha = new TextBox();
        private readonly TextBox txtHora = new TextBox();
        private readonly DataGridView dgvCitas = new DataGridView();
        private readonly Button btnAgregar = new Button();
        private readonly Button btnActualizar = new Button();
        private readonly Button btnEliminar = new Button();
        private readonly Button btnLimpiar = new Button();

        private List<Mascota> _mascotas = new List<Mascota>();
        private List<Veterinario> _veterinarios = new List<Veterinario>();

        public FormCitas()
        {
            Text = "Agenda de Citas";
            ClientSize = new Size(760, 500);
            StartPosition = FormStartPosition.CenterScreen;
            FormBorderStyle = FormBorderStyle.FixedSingle;
            MaximizeBox = false;

            AgregarEtiqueta("Mascota:", 20, 25);
            cboMascotas.SetBounds(140, 22, 240, 25);
            cboMascotas.DropDownStyle = ComboBoxStyle.DropDownList;
            cboMascotas.DisplayMember = "Nombre";

            AgregarEtiqueta("Veterinario:", 400, 25);
            cboVeterinarios.SetBounds(510, 22, 230, 25);
            cboVeterinarios.DropDownStyle = ComboBoxStyle.DropDownList;
            cboVeterinarios.DisplayMember = "Nombre";

            AgregarEtiqueta("Fecha (DD/MM/AAAA):", 20, 60);
            txtFecha.SetBounds(160, 57, 220, 25);

            AgregarEtiqueta("Hora (HH:MM):", 400, 60);
            txtHora.SetBounds(510, 57, 100, 25);

            AgregarEtiqueta("Ej: 20/10/2026", 160, 90);

            ConfigurarBoton(btnAgregar, "Agregar", 20, 125, btnAgregar_Click);
            ConfigurarBoton(btnActualizar, "Actualizar", 130, 125, btnActualizar_Click);
            ConfigurarBoton(btnEliminar, "Eliminar", 250, 125, btnEliminar_Click);
            ConfigurarBoton(btnLimpiar, "Limpiar", 370, 125, (s, e) => LimpiarCampos());

            dgvCitas.SetBounds(20, 170, 710, 300);
            FormClientes.ConfigurarGrid(dgvCitas);
            dgvCitas.CellClick += DgvCitas_CellClick;

            Controls.AddRange(new Control[]
            {
                cboMascotas, cboVeterinarios, txtFecha, txtHora,
                btnAgregar, btnActualizar, btnEliminar, btnLimpiar, dgvCitas
            });

            CargarDatos();
        }

        // ---------- READ ----------
        private void CargarDatos()
        {
            List<Cliente> clientes = RepositorioVeterinaria.CargarClientes();
            _mascotas = RepositorioVeterinaria.CargarMascotas(clientes);
            _veterinarios = RepositorioVeterinaria.CargarVeterinarios();

            cboMascotas.DataSource = null;
            cboMascotas.DataSource = _mascotas;
            cboVeterinarios.DataSource = null;
            cboVeterinarios.DataSource = _veterinarios;

            dgvCitas.DataSource = null;
            dgvCitas.DataSource = RepositorioVeterinaria.CargarCitas(_mascotas, _veterinarios);
            dgvCitas.Columns["Mascota"]?.Visible = false;        // columna de objetos
            dgvCitas.Columns["Veterinario"]?.Visible = false;    // columna de objetos
            dgvCitas.ClearSelection();
        }

        private void DgvCitas_CellClick(object? sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return;
            if (dgvCitas.Rows[e.RowIndex].DataBoundItem is not Cita c) return;
            txtFecha.Text = c.Fecha;
            txtHora.Text = c.Hora;
            cboMascotas.SelectedItem = c.Mascota;
            cboVeterinarios.SelectedItem = c.Veterinario;
        }

        // ---------- Validación común de la cita ----------
        private bool DatosValidos()
        {
            if (Validacion.CamposVacios(txtFecha.Text, txtHora.Text))
            {
                MessageBox.Show("Fecha y hora son obligatorias.",
                    "Validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }
            if (!Validacion.FechaValida(txtFecha.Text.Trim()))
            {
                MessageBox.Show("La fecha debe tener el formato DD/MM/AAAA (ej. 20/10/2026).",
                    "Validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }
            if (cboMascotas.SelectedItem is not Mascota)
            {
                MessageBox.Show("Seleccione una mascota.",
                    "Validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }
            if (cboVeterinarios.SelectedItem is not Veterinario)
            {
                MessageBox.Show("Seleccione un veterinario.",
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
                string id = RepositorioVeterinaria.SiguienteId("citas", "CIT-");
                Cita nueva = new Cita(id, txtFecha.Text.Trim(), txtHora.Text.Trim(),
                                      (Mascota)cboMascotas.SelectedItem!,
                                      (Veterinario)cboVeterinarios.SelectedItem!);
                RepositorioVeterinaria.InsertarCita(nueva);
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
            Cita? seleccionada = dgvCitas.CurrentRow?.DataBoundItem as Cita;
            if (seleccionada == null)
            {
                MessageBox.Show("Seleccione una cita en la tabla.",
                    "Validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            if (!DatosValidos()) return;
            try
            {
                Cita actualizada = new Cita(seleccionada.IdCita, txtFecha.Text.Trim(), txtHora.Text.Trim(),
                                            (Mascota)cboMascotas.SelectedItem!,
                                            (Veterinario)cboVeterinarios.SelectedItem!);
                RepositorioVeterinaria.ActualizarCita(actualizada);
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
            Cita? seleccionada = dgvCitas.CurrentRow?.DataBoundItem as Cita;
            if (seleccionada == null)
            {
                MessageBox.Show("Seleccione una cita en la tabla.",
                    "Validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            DialogResult confirmacion = MessageBox.Show(
                $"¿Eliminar la cita del {seleccionada.Fecha} a las {seleccionada.Hora}?",
                "Confirmar eliminación", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (confirmacion != DialogResult.Yes) return;

            try
            {
                RepositorioVeterinaria.EliminarCita(seleccionada.IdCita);
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
            txtFecha.Clear();
            txtHora.Clear();
            dgvCitas.ClearSelection();
            txtFecha.Focus();
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
