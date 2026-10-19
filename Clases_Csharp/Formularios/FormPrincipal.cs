using System;
using System.Drawing;
using System.Windows.Forms;
using SistemaVeterinaria.Formularios;

namespace SistemaVeterinaria.Formularios
{
    /// <summary>
    /// Ventana principal: abre cada módulo del sistema como ventana modal.
    /// </summary>
    public class FormPrincipal : Form
    {
        public FormPrincipal()
        {
            Text = "Sistema Clínico Veterinario - Interfaz Gráfica";
            ClientSize = new Size(520, 420);
            StartPosition = FormStartPosition.CenterScreen;
            FormBorderStyle = FormBorderStyle.FixedSingle;
            MaximizeBox = false;

            Label titulo = new Label
            {
                Text = "SISTEMA CLÍNICO VETERINARIO",
                Font = new Font("Segoe UI", 14, FontStyle.Bold),
                AutoSize = false,
                Location = new Point(90, 20),
                Size = new Size(350, 30),
                TextAlign = ContentAlignment.MiddleCenter
            };
            Controls.Add(titulo);

            AgregarBoton("🐾 Propietarios y Mascotas", 60, 70, () => new FormClientes().ShowDialog());
            AgregarBoton("🐕 Gestión de Mascotas", 60, 120, () => new FormMascotas().ShowDialog());
            AgregarBoton("📅 Agenda de Citas", 60, 170, () => new FormCitas().ShowDialog());
            AgregarBoton("🩺 Consultas / Historial", 60, 220, () => new FormConsultas().ShowDialog());
            AgregarBoton("💊 Inventario", 60, 270, () => new FormInventario().ShowDialog());
            AgregarBoton("📊 Reportes", 60, 320, () => new FormReportes().ShowDialog());

            Label pie = new Label
            {
                Text = "Al cerrar esta ventana regresará a la consola.",
                Location = new Point(120, 380),
                AutoSize = true
            };
            Controls.Add(pie);
        }

        private void AgregarBoton(string texto, int x, int y, Action accion)
        {
            Button boton = new Button
            {
                Text = texto,
                Location = new Point(x, y),
                Size = new Size(400, 40),
                Font = new Font("Segoe UI", 10)
            };
            boton.Click += (s, e) => accion();
            Controls.Add(boton);
        }
    }
}
