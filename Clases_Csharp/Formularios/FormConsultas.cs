using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using SistemaVeterinaria.Datos;

namespace SistemaVeterinaria.Formularios
{
    /// <summary>
    /// CRUD de Consultas (historial clínico) con validación numérica de
    /// peso y temperatura y receta opcional.
    /// </summary>
    public class FormConsultas : Form
    {
        private readonly ComboBox cboMascotas = new ComboBox();
        private readonly TextBox txtFecha = new TextBox();
        private readonly TextBox txtPeso = new TextBox();
        private readonly TextBox txtTemperatura = new TextBox();
        private readonly TextBox txtSintomas = new TextBox();
        private readonly TextBox txtDiagnostico = new TextBox();
        private readonly CheckBox chkReceta = new CheckBox();
        private readonly TextBox txtFarmaco = new TextBox();
        private readonly TextBox txtDosis = new TextBox();
        private readonly TextBox txtFrecuencia = new TextBox();
        private readonly DataGridView dgvConsultas = new DataGridView();
        private readonly Button btnAgregar = new Button();
        private readonly Button btnActualizar = new Button();
        private readonly Button btnEliminar = new Button();
        private readonly Button btnLimpiar = new Button();

        private List<Mascota> _mascotas = new List<Mascota>();

        public FormConsultas()
        {
            Text = "Consultas / Historial Clínico";
            ClientSize = new Size(780, 560);
            StartPosition = FormStartPosition.CenterScreen;
            FormBorderStyle = FormBorderStyle.FixedSingle;
            MaximizeBox = false;

            AgregarEtiqueta("Mascota:", 20, 25);
            cboMascotas.SetBounds(140, 22, 220, 25);
            cboMascotas.DropDownStyle = ComboBoxStyle.DropDownList;
            cboMascotas.DisplayMember = "Nombre";

            AgregarEtiqueta("Fecha (DD/MM/AAAA):", 390, 25);
            txtFecha.SetBounds(540, 22, 130, 25);

            AgregarEtiqueta("Peso (kg):", 20, 60);
            txtPeso.SetBounds(140, 57, 90, 25);

            AgregarEtiqueta("Temp. (°C):", 250, 60);
            txtTemperatura.SetBounds(350, 57, 90, 25);

            AgregarEtiqueta("Síntomas:", 20, 95);
            txtSintomas.SetBounds(140, 92, 530, 25);

            AgregarEtiqueta("Diagnóstico:", 20, 130);
            txtDiagnostico.SetBounds(140, 127, 530, 25);

            chkReceta.Text = "¿Recetar fármaco?";
            chkReceta.SetBounds(20, 162, 180, 22);
            chkReceta.CheckedChanged += (s, e) =>
            {
                txtFarmaco.Enabled = chkReceta.Checked;
                txtDosis.Enabled = chkReceta.Checked;
                txtFrecuencia.Enabled = chkReceta.Checked;
            };

            AgregarEtiqueta("Fármaco:", 210, 165);
            txtFarmaco.SetBounds(290, 162, 160, 25);
            AgregarEtiqueta("Dosis:", 470, 165);
            txtDosis.SetBounds(520, 162, 70, 25);
            AgregarEtiqueta("Frec.:", 610, 165);
            txtFrecuencia.SetBounds(665, 162, 75, 25);
            txtFarmaco.Enabled = false;
            txtDosis.Enabled = false;
            txtFrecuencia.Enabled = false;

            ConfigurarBoton(btnAgregar, "Agregar", 20, 200, btnAgregar_Click);
            ConfigurarBoton(btnActualizar, "Actualizar", 130, 200, btnActualizar_Click);
            ConfigurarBoton(btnEliminar, "Eliminar", 250, 200, btnEliminar_Click);
            ConfigurarBoton(btnLimpiar, "Limpiar", 370, 200, (s, e) => LimpiarCampos());

            dgvConsultas.SetBounds(20, 245, 735, 290);
            FormClientes.ConfigurarGrid(dgvConsultas);
            dgvConsultas.CellClick += DgvConsultas_CellClick;

            Controls.AddRange(new Control[]
            {
                cboMascotas, txtFecha, txtPeso, txtTemperatura, txtSintomas, txtDiagnostico,
                chkReceta, txtFarmaco, txtDosis, txtFrecuencia,
                btnAgregar, btnActualizar, btnEliminar, btnLimpiar, dgvConsultas
            });

            CargarDatos();
        }

        // ---------- READ ----------
        private void CargarDatos()
        {
            List<Cliente> clientes = RepositorioVeterinaria.CargarClientes();
            _mascotas = RepositorioVeterinaria.CargarMascotas(clientes);
            RepositorioVeterinaria.CargarConsultas(_mascotas);

            // CargarConsultas vincula cada consulta a su mascota; se aplanan para la tabla
            List<Consulta> consultas = new List<Consulta>();
            foreach (Mascota m in _mascotas)
            {
                consultas.AddRange(m.HistorialClinico);
            }

            cboMascotas.DataSource = null;
            cboMascotas.DataSource = _mascotas;

            dgvConsultas.DataSource = null;
            dgvConsultas.DataSource = consultas;
            dgvConsultas.ClearSelection();
        }

        private void DgvConsultas_CellClick(object? sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return;
            if (dgvConsultas.Rows[e.RowIndex].DataBoundItem is not Consulta c) return;
            txtFecha.Text = c.Fecha;
            txtPeso.Text = c.Peso.ToString();
            txtTemperatura.Text = c.Temperatura.ToString();
            txtSintomas.Text = c.Sintomas;
            txtDiagnostico.Text = c.Diagnostico;

            bool conReceta = !string.IsNullOrWhiteSpace(c.Farmaco);
            chkReceta.Checked = conReceta;
            txtFarmaco.Text = c.Farmaco;
            txtDosis.Text = c.Dosis;
            txtFrecuencia.Text = c.Frecuencia;
        }

        // ---------- Validación común ----------
        private bool DatosValidos()
        {
            if (Validacion.CamposVacios(txtFecha.Text, txtPeso.Text, txtTemperatura.Text,
                                        txtSintomas.Text, txtDiagnostico.Text))
            {
                MessageBox.Show("Fecha, peso, temperatura, síntomas y diagnóstico son obligatorios.",
                    "Validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }
            if (!Validacion.FechaValida(txtFecha.Text.Trim()))
            {
                MessageBox.Show("La fecha debe tener el formato DD/MM/AAAA (ej. 20/10/2026).",
                    "Validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }
            if (!Validacion.NumeroPositivo(txtPeso.Text.Trim(), out _))
            {
                MessageBox.Show("El peso debe ser un número mayor a cero.",
                    "Validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }
            if (!Validacion.NumeroPositivo(txtTemperatura.Text.Trim(), out _))
            {
                MessageBox.Show("La temperatura debe ser un número mayor a cero.",
                    "Validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }
            if (cboMascotas.SelectedItem is not Mascota)
            {
                MessageBox.Show("Seleccione una mascota.",
                    "Validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }
            if (chkReceta.Checked && Validacion.CamposVacios(txtFarmaco.Text, txtDosis.Text, txtFrecuencia.Text))
            {
                MessageBox.Show("Si marca la receta, indique fármaco, dosis y frecuencia.",
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
                Mascota mascota = (Mascota)cboMascotas.SelectedItem!;
                string id = RepositorioVeterinaria.SiguienteId("consultas", "CONS-");
                Consulta nueva = ConstruirConsulta(id);
                RepositorioVeterinaria.InsertarConsulta(nueva, mascota.IdMascota);
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
            Consulta? seleccionada = dgvConsultas.CurrentRow?.DataBoundItem as Consulta;
            if (seleccionada == null)
            {
                MessageBox.Show("Seleccione una consulta en la tabla.",
                    "Validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            if (!DatosValidos()) return;
            try
            {
                Mascota mascota = (Mascota)cboMascotas.SelectedItem!;
                Consulta actualizada = ConstruirConsulta(seleccionada.IdConsulta);
                RepositorioVeterinaria.ActualizarConsulta(actualizada, mascota.IdMascota);
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
            Consulta? seleccionada = dgvConsultas.CurrentRow?.DataBoundItem as Consulta;
            if (seleccionada == null)
            {
                MessageBox.Show("Seleccione una consulta en la tabla.",
                    "Validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            DialogResult confirmacion = MessageBox.Show(
                $"¿Eliminar la consulta del {seleccionada.Fecha} ({seleccionada.Diagnostico})?",
                "Confirmar eliminación", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (confirmacion != DialogResult.Yes) return;

            try
            {
                RepositorioVeterinaria.EliminarConsulta(seleccionada.IdConsulta);
                CargarDatos();
                LimpiarCampos();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al eliminar: " + ex.Message, "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private Consulta ConstruirConsulta(string id)
        {
            bool conReceta = chkReceta.Checked;
            return new Consulta(
                id,
                txtFecha.Text.Trim(),
                double.Parse(txtPeso.Text.Trim()),
                double.Parse(txtTemperatura.Text.Trim()),
                txtSintomas.Text.Trim(),
                txtDiagnostico.Text.Trim(),
                conReceta ? txtFarmaco.Text.Trim() : "",
                conReceta ? txtDosis.Text.Trim() : "",
                conReceta ? txtFrecuencia.Text.Trim() : "");
        }

        private void LimpiarCampos()
        {
            txtFecha.Clear();
            txtPeso.Clear();
            txtTemperatura.Clear();
            txtSintomas.Clear();
            txtDiagnostico.Clear();
            chkReceta.Checked = false;
            txtFarmaco.Clear();
            txtDosis.Clear();
            txtFrecuencia.Clear();
            dgvConsultas.ClearSelection();
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
