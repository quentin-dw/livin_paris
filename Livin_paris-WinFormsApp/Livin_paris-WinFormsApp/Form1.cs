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

        private Dictionary<int, PointF> positions;
        private Dictionary<string, List<Noeud<int>>> lignes;
        private HashSet<string> lignesAffichees;
        private Dictionary<string, CheckBox> boutonsLignes = new Dictionary<string, CheckBox>();
        private Dictionary<string, Color> couleursLignes;

        private float zoomFactor = 1.0f;
        private Point panOffset = new Point(0, 0);
        private Point lastMousePosition;

        private Button btnAfficherNoms;
        private Button btnExporter;
        private Panel panelBas;
        private bool afficherNoms = true;

        public Form1(string noeudsFile, string arcsFile)
        {
            InitializeComponent();
            this.Text = "Visualisation du Graphe";
            this.Width = 800;
            this.Height = 600;
            this.DoubleBuffered = true; 

            graphe = new Graphe<int>(noeudsFile, arcsFile);
            positions = new Dictionary<int, PointF>();
            lignes = new Dictionary<string, List<Noeud<int>>>();
            couleursLignes = new Dictionary<string, Color>();

            CalculerPositions();
            CalculerLignes();
            AssignerCouleursLignes();
            AjouterBoutonsLignes();

            panelBas = new Panel
            {
                Dock = DockStyle.Bottom,
                Height = 60,
                Padding = new Padding(5)
            };
            btnAfficherNoms = new Button
            {
                Text = "Masquer les noms",
                Dock = DockStyle.Left,
                Width = 120
            };
            btnAfficherNoms.Click += BtnAfficherNoms_Click;
            panelBas.Controls.Add(btnAfficherNoms);
            btnExporter = new Button
            {
                Text = "Exporter",
                Dock = DockStyle.Right, 
                Width = 120
            };
            btnExporter.Click += BtnExporter_Click;
            panelBas.Controls.Add(btnExporter);
            this.Controls.Add(panelBas);

            this.MouseWheel += GrapheForm_MouseWheel;
            this.MouseDown += GrapheForm_MouseDown;
            this.MouseMove += GrapheForm_MouseMove;
        }
        /// <summary>
        /// Fonction permettant de calculer la position des stations sur le plan en fonction de leur latitude et longitude.
        /// </summary>
        private void CalculerPositions()
        {
            float minLat = float.MaxValue, maxLat = float.MinValue;
            float minLon = float.MaxValue, maxLon = float.MinValue;

            foreach (var noeud in graphe.GetNoeuds())
            {
                minLat = Math.Min(minLat, (float)noeud.Latitude);
                maxLat = Math.Max(maxLat, (float)noeud.Latitude);
                minLon = Math.Min(minLon, (float)noeud.Longitude);
                maxLon = Math.Max(maxLon, (float)noeud.Longitude);
            }

            float largeur = this.ClientSize.Width - 100;
            float hauteur = this.ClientSize.Height - 100;

            foreach (var noeud in graphe.GetNoeuds())
            {
                float x = (float)((noeud.Longitude - minLon) / (maxLon - minLon) * largeur) + 50;
                float y = (float)((1 - (noeud.Latitude - minLat) / (maxLat - minLat)) * hauteur) + 50;
                positions[noeud.Id] = new PointF(x, y);
            }
        }

        /// <summary>
        /// Cette fonction permet de regrouper les noeuds en fonction de leur ligne
        /// </summary>
        private void CalculerLignes()
        {
            lignes = new Dictionary<string, List<Noeud<int>>>();

            foreach (var noeud in graphe.GetNoeuds())
            {
                string nomLigne = noeud.Ligne;

                if (!lignes.ContainsKey(nomLigne))
                {
                    lignes[nomLigne] = new List<Noeud<int>>();
                }

                lignes[nomLigne].Add(noeud);
            }
            lignesAffichees = new HashSet<string>(lignes.Keys);
        }

        /// <summary>
        /// Cette fonction permet de donner une couleur a chaque ligne
        /// </summary>
        private void AssignerCouleursLignes()
        {
            Random random = new Random();
            foreach (var ligne in lignes.Keys)
            {
                couleursLignes[ligne] = Color.FromArgb(random.Next(0, 256), random.Next(0, 256), random.Next(0, 256));
            }
        }

        /// <summary>
        /// Cette fonction sert à créer les boutons pour chacune des lignes permettant ensuite d'activer ou désactiver leur affichage
        /// </summary>
        private void AjouterBoutonsLignes()
        {
            int y = 10;
            Panel panel = new Panel
            {
                AutoScroll = true,
                Width = 150,
                Height = this.ClientSize.Height,
                Dock = DockStyle.Right
            };

            foreach (var ligne in lignes.Keys)
            {
                CheckBox checkBox = new CheckBox
                {
                    Text = ligne,
                    Checked = true,
                    Location = new Point(10, y),
                    AutoSize = true
                };

                checkBox.BackColor = couleursLignes[ligne]; 
                checkBox.ForeColor = Color.White;

                checkBox.CheckedChanged += (sender, e) =>
                {
                    if (checkBox.Checked)
                        lignesAffichees.Add(ligne);
                    else
                        lignesAffichees.Remove(ligne);

                    this.Invalidate();
                };

                boutonsLignes[ligne] = checkBox;
                panel.Controls.Add(checkBox);
                y += 25;
            }

            this.Controls.Add(panel);
        }

        /// <summary>
        /// Cette fonction permet de dessiner le graphe (Noeuds et Liens)
        /// </summary>
        /// <param name="e"></param>
        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            Graphics g = e.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;
            g.TranslateTransform(panOffset.X, panOffset.Y);
            g.ScaleTransform(zoomFactor, zoomFactor);

            foreach (var lien in graphe.GetLiens())
            {
                if (lignesAffichees.Contains(lien.Noeud1.Ligne))
                {
                    Pen pen = new Pen(couleursLignes[lien.Noeud1.Ligne], 2);
                    PointF p1 = positions[lien.Noeud1.Id];
                    PointF p2 = positions[lien.Noeud2.Id];
                    g.DrawLine(pen, p1, p2);
                }
            }

            foreach (var noeud in graphe.GetNoeuds())
            {
                if (lignesAffichees.Contains(noeud.Ligne))
                {
                    PointF pos = positions[noeud.Id];
                    g.FillEllipse(Brushes.Blue, pos.X - 5, pos.Y - 5, 10, 10);
                    if (afficherNoms)
                    {
                        g.DrawString(noeud.Nom, Font, Brushes.Black, pos.X + 8, pos.Y - 8);
                    }
                }
            }
        }

        /// <summary>
        /// Cette fonction permet d'exporter le graphe sous forme d'image .png en cliquant sur un bouton
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnExporter_Click(object sender, EventArgs e)
        {
            using (Bitmap bmp = new Bitmap(this.ClientSize.Width, this.ClientSize.Height))
            {
                this.DrawToBitmap(bmp, new Rectangle(0, 0, bmp.Width, bmp.Height));
                bmp.Save("graphe.png");
                MessageBox.Show("Graphe exporté sous 'graphe.png'", "Export", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void BtnAfficherNoms_Click(object sender, EventArgs e)
        {
            afficherNoms = !afficherNoms;
            if (afficherNoms)
            {
                btnAfficherNoms.Text = "Masquer les noms";
            }
            else
            {
                btnAfficherNoms.Text = "Afficher les noms";
            }
            this.Invalidate(); 
        }


        /// <summary>
        /// Cette fonction permet d'utiliser la molette pour zoomer / dézoomer
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void GrapheForm_MouseWheel(object sender, MouseEventArgs e)
        {
            zoomFactor += (e.Delta > 0) ? 0.1f : -0.1f;
            zoomFactor = Math.Max(0.5f, Math.Min(2.0f, zoomFactor));
            this.Invalidate();
        }

        /// <summary>
        /// Cette fonction permet de savoir quand un clic est fait pour pouvoir dans déplacer la vue du graphe
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void GrapheForm_MouseDown(object sender, MouseEventArgs e)
        {
            lastMousePosition = e.Location;
        }

        /// <summary>
        /// Cette fonction permet de déplacer la vue avec la souris
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
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
