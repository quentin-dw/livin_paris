using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using System.Windows.Forms;
using System.Globalization;


namespace Livin_paris_WinFormsApp
{
    public partial class Form1 : Form
    {
        private Graphe<int> graphe;
        private Dictionary<int, PointF> positions; // Stocke les positions des noeuds

        private float zoomFactor = 1.0f;
        private Point panOffset = new Point(0, 0);
        private Point lastMousePosition;

        public Form1(string noeudsFile, string arcsFile)
        {
            InitializeComponent();
            this.Text = "Visualisation du Graphe";
            this.Width = 800;
            this.Height = 600;
            this.DoubleBuffered = true; // Évite le scintillement

            graphe = new Graphe<int>(noeudsFile, arcsFile);
            positions = new Dictionary<int, PointF>();

            // Calculer les positions des noeuds pour l'affichage
            CalculerPositions();

            // Ajouter un bouton Exporter
            Button btnExporter = new Button();
            btnExporter.Text = "Exporter";
            btnExporter.Dock = DockStyle.Bottom;
            btnExporter.Click += BtnExporter_Click;
            this.Controls.Add(btnExporter);

            // Gérer les événements de la souris pour zoom & déplacement
            this.MouseWheel += GrapheForm_MouseWheel;
            this.MouseDown += GrapheForm_MouseDown;
            this.MouseMove += GrapheForm_MouseMove;
        }

        private void CalculerPositions()
        {
            float minLat = float.MaxValue, maxLat = float.MinValue;
            float minLon = float.MaxValue, maxLon = float.MinValue;

            // Trouver les limites du graphe
            foreach (var noeud in graphe.GetNoeuds())
            {
                minLat = Math.Min(minLat, (float)noeud.Latitude);
                maxLat = Math.Max(maxLat, (float)noeud.Latitude);
                minLon = Math.Min(minLon, (float)noeud.Longitude);
                maxLon = Math.Max(maxLon, (float)noeud.Longitude);
            }

            // Normalisation pour afficher correctement les noeuds
            float largeur = this.ClientSize.Width - 100;
            float hauteur = this.ClientSize.Height - 100;

            foreach (var noeud in graphe.GetNoeuds())
            {
                float x = (float)((noeud.Longitude - minLon) / (maxLon - minLon) * largeur) + 50;
                float y = (float)((1 - (noeud.Latitude - minLat) / (maxLat - minLat)) * hauteur) + 50;
                positions[noeud.Id] = new PointF(x, y);
            }
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            Graphics g = e.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;
            g.TranslateTransform(panOffset.X, panOffset.Y);
            g.ScaleTransform(zoomFactor, zoomFactor);

            // Dessiner les liens
            Pen pen = new Pen(Color.Gray, 2);
            foreach (var lien in graphe.GetLiens())
            {
                PointF p1 = positions[lien.Noeud1.Id];
                PointF p2 = positions[lien.Noeud2.Id];
                g.DrawLine(pen, p1, p2);
            }

            // Dessiner les noeuds
            foreach (var noeud in graphe.GetNoeuds())
            {
                PointF pos = positions[noeud.Id];
                g.FillEllipse(Brushes.Blue, pos.X - 5, pos.Y - 5, 10, 10);
                g.DrawString(noeud.Nom, Font, Brushes.Black, pos.X + 8, pos.Y - 8);
            }
        }

        private void BtnExporter_Click(object sender, EventArgs e)
        {
            using (Bitmap bmp = new Bitmap(this.ClientSize.Width, this.ClientSize.Height))
            {
                this.DrawToBitmap(bmp, new Rectangle(0, 0, bmp.Width, bmp.Height));
                bmp.Save("graphe.png");
                MessageBox.Show("Graphe exporté sous 'graphe.png'", "Export", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void GrapheForm_MouseWheel(object sender, MouseEventArgs e)
        {
            zoomFactor += (e.Delta > 0) ? 0.1f : -0.1f;
            zoomFactor = Math.Max(0.5f, Math.Min(2.0f, zoomFactor));
            this.Invalidate();
        }

        private void GrapheForm_MouseDown(object sender, MouseEventArgs e)
        {
            lastMousePosition = e.Location;
        }

        private void GrapheForm_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                panOffset.X += e.X - lastMousePosition.X;
                panOffset.Y += e.Y - lastMousePosition.Y;
                lastMousePosition = e.Location;
                this.Invalidate();
            }
        }
    }
}
