using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using SistemaVeterinaria.Datos;

namespace SistemaVeterinaria.Formularios
{
    /// <summary>
    /// Genera los reportes del sistema (citas y stock bajo) en un cuadro de texto.
    /// </summary>
    public class FormReportes : Form
    {
        private readonly TextBox txtReporte = new TextBox();
        private readonly Button btnCitas = new Button();
        private readonly Button btnStock = new Button();
        private readonly Button btnTodo = new Button();

        public FormReportes()
        {
            Text = "Reportes";
            ClientSize = new Size(720, 470);
            StartPosition = FormStartPosition.CenterScreen;
            FormBorderStyle = FormBorderStyle.FixedSingle;
            MaximizeBox = false;

            ConfigurarBoton(btnCitas, "Reporte de citas", 20, 20, btnCitas_Click);
            ConfigurarBoton(btnStock, "Productos con stock bajo", 200, 20, btnStock_Click);
            ConfigurarBoton(btnTodo, "Reporte completo", 410, 20, btnTodo_Click);

            txtReporte.SetBounds(20, 65, 675, 375);
            txtReporte.Multiline = true;
            txtReporte.ReadOnly = true;
            txtReporte.ScrollBars = ScrollBars.Vertical;
            txtReporte.Font = new Font("Consolas", 10);
            txtReporte.Text = "Seleccione un reporte para generarlo.";

            Controls.AddRange(new Control[] { btnCitas, btnStock, btnTodo, txtReporte });
        }

        // ---------- READ + generación de reportes ----------
        private (List<Cita> citas, List<Producto> inventario) CargarDatosReporte()
        {
            List<Cliente> clientes = RepositorioVeterinaria.CargarClientes();
            List<Mascota> mascotas = RepositorioVeterinaria.CargarMascotas(clientes);
            List<Veterinario> veterinarios = RepositorioVeterinaria.CargarVeterinarios();
            List<Cita> citas = RepositorioVeterinaria.CargarCitas(mascotas, veterinarios);
            List<Producto> inventario = RepositorioVeterinaria.CargarProductos();
            return (citas, inventario);
        }

        private void btnCitas_Click(object? sender, EventArgs e)
        {
            try
            {
                var (citas, _) = CargarDatosReporte();
                GestorReportes gestor = new GestorReportes();
                txtReporte.Text = gestor.ObtenerReporteCitas(citas);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al generar el reporte: " + ex.Message, "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnStock_Click(object? sender, EventArgs e)
        {
            try
            {
                var (_, inventario) = CargarDatosReporte();
                GestorReportes gestor = new GestorReportes();
                txtReporte.Text = gestor.ObtenerReporteInventarioBajo(inventario, 5);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al generar el reporte: " + ex.Message, "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnTodo_Click(object? sender, EventArgs e)
        {
            try
            {
                var (citas, inventario) = CargarDatosReporte();
                GestorReportes gestor = new GestorReportes();
                txtReporte.Text = gestor.ObtenerReporteCitas(citas)
                                 + Environment.NewLine
                                 + gestor.ObtenerReporteInventarioBajo(inventario, 5);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al generar el reporte: " + ex.Message, "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void ConfigurarBoton(Button boton, string texto, int x, int y, EventHandler onClick)
        {
            boton.Text = texto;
            boton.SetBounds(x, y, 185, 32);
            boton.Click += onClick;
            Controls.Add(boton);
        }
    }
}
