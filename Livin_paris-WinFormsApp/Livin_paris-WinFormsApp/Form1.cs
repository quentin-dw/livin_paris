using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using System.Windows.Forms;
using System.Globalization;
using System.Diagnostics;
using GMap.NET.WindowsForms.Markers;
using GMap.NET.WindowsForms;
using GMap.NET;

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
        private Panel panelLignes;

        private float zoomFactor = 1.0f;
        private Point panOffset = new Point(0, 0);
        private Point lastMousePosition;

        private Button btnAfficherNoms;
        private bool afficherNoms = true;
        private Button btnExporter;
        private TextBox txtStationDepart;
        private TextBox txtStationArrivee;
        private Button btnCalculerChemin;
        private List<Noeud<int>> cheminActuel = new List<Noeud<int>>();
        private Panel panelBas;


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
            AjouterPanelBas();
            
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
            panelLignes = new Panel
            {
                AutoScroll = true,
                Width = 150,
                Height = this.ClientSize.Height,
                Dock = DockStyle.Right,
                BackColor = Color.LightGray
            };

            Button btnToggleAll = new Button
            {
                Text = "Afficher / Cacher tout",
                Location = new Point(10, y),
                Width = 130,
                Height = 30
            };
            btnToggleAll.Click += (sender, e) =>
            {
                bool toutAffiche = lignesAffichees.Count == lignes.Count;
                if (toutAffiche)
                {
                    lignesAffichees.Clear();
                    foreach (var checkBox in boutonsLignes.Values)
                        checkBox.Checked = false;
                }
                else
                {
                    lignesAffichees = new HashSet<string>(lignes.Keys);
                    foreach (var checkBox in boutonsLignes.Values)
                        checkBox.Checked = true;
                }

                this.Invalidate();
            };
            panelLignes.Controls.Add(btnToggleAll);
            y += 40;

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
                panelLignes.Controls.Add(checkBox);
                y += 25;
            }
            this.Controls.Add(panelLignes);
        }

        private void CalculerChemin(string stationDepart, string stationArrivee)
        {
            if (string.IsNullOrWhiteSpace(stationDepart) || string.IsNullOrWhiteSpace(stationArrivee))
            {
                MessageBox.Show("Veuillez entrer deux noms de stations valides.", "Erreur", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var noeudDepart = graphe.GetNoeuds().FirstOrDefault(n => n.Nom.Equals(stationDepart, StringComparison.OrdinalIgnoreCase));
            var noeudArrivee = graphe.GetNoeuds().FirstOrDefault(n => n.Nom.Equals(stationArrivee, StringComparison.OrdinalIgnoreCase));

            if (noeudDepart == null || noeudArrivee == null)
            {
                MessageBox.Show("Une ou les deux stations sont introuvables.", "Erreur", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            (cheminActuel, int coutTotal) = graphe.TrouverMeilleurChemin(stationDepart, stationArrivee);

            if (cheminActuel.Count == 0)
            {
                MessageBox.Show("Aucun chemin trouvé entre ces deux stations.", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            MessageBox.Show("Chemin trouvé ! Coût total : " + coutTotal + " minutes \nStation parcourues : " + AfficherCheminActuel(), "Résultat", MessageBoxButtons.OK, MessageBoxIcon.Information);
            this.Invalidate();
        }

        private void BtnCalculerChemin_Click(object sender, EventArgs e)
        {
            CalculerChemin(txtStationDepart.Text, txtStationArrivee.Text);
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

            if (cheminActuel.Count > 1)
            {
                Pen cheminPen = new Pen(Color.Red, 3);

                for (int i = 0; i < cheminActuel.Count - 1; i++)
                {
                    PointF p1 = positions[cheminActuel[i].Id];
                    PointF p2 = positions[cheminActuel[i + 1].Id];
                    g.DrawLine(cheminPen, p1, p2);
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

        /// <summary>
        /// Méthode permettant l'affichage d'une liste (ici utilisé pour afficher le chemin trouvée par l'algorithme de parcours)
        /// </summary>
        private string AfficherCheminActuel()
        {
            if (cheminActuel == null || cheminActuel.Count == 0)
            {
                return "Aucun chemin trouvé.";
            }
            return string.Join(" -> ", cheminActuel.Select(n => n.Nom));
        }

        /// <summary>
        /// Cette méthode crée un panel en bas de la fenêtre et y implémente différents boutons
        /// </summary>
        private void AjouterPanelBas()
        {
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
            txtStationDepart = new TextBox
            {
                Width = 120,
                PlaceholderText = "Station départ",
                Location = new Point(150, 15)
            };
            panelBas.Controls.Add(txtStationDepart);
            txtStationArrivee = new TextBox
            {
                Width = 120,
                PlaceholderText = "Station arrivée",
                Location = new Point(280, 15)
            };
            panelBas.Controls.Add(txtStationArrivee);
            btnCalculerChemin = new Button
            {
                Text = "Calculer",
                Width = 100,
                Location = new Point(420, 12)
            };
            btnCalculerChemin.Click += BtnCalculerChemin_Click;
            panelBas.Controls.Add(btnCalculerChemin);

            this.Controls.Add(panelBas);
        }
    }
}

