using System;
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
using System.Runtime.Serialization.Formatters;
using static System.Windows.Forms.LinkLabel;

namespace Livin_paris_WinFormsApp
{
    public partial class Form2 : Form
    {
        #region Attributs
        private Graphe<int> graphe;

        private GMapControl gmap;
        private GMapOverlay overlayStations;
        private Dictionary<string, GMarkerGoogle> marqueursStations;
        private GMapOverlay overlayLignes;
        private Dictionary<string, List<Noeud<int>>> lignes;
        private HashSet<string> lignesAffichees;
        private Dictionary<string, Color> couleursLignes;
        private GMapOverlay overlayPlusCourtChemin;

        private Panel panelLignes;
        private Dictionary<string, CheckBox> boutonsLignes = new Dictionary<string, CheckBox>();

        private Panel panelBas;
        private Button btnAfficherStations;
        private bool afficherStations = true;
        private Button btnExporter;
        private TextBox txtStationDepart;
        private TextBox txtStationArrivee;
        private Button btnCalculerChemin;
        private List<Noeud<int>> cheminActuel = new List<Noeud<int>>();
        private Button btnAfficherChemin;
        #endregion

        /// <summary>
        /// Cette méthode permet de créer une fenêtre microsoft form pour l'affichage d'un plan interractif du metro et de son fond de carte.
        /// </summary>
        /// <param name="noeudsFile">Lien vers le fichier csv contenant les noeuds du graphe</param>
        /// <param name="arcsFile">Lien vers le fichier csv contenant les arcs du graphe</param>
        public Form2(string noeudsFile, string arcsFile)
        {
            InitializeComponent();

            gmap = new GMapControl
            {
                Dock = DockStyle.Fill,
                MinZoom = 11,
                MaxZoom = 16,
                Zoom = 12,
                MapProvider = GMap.NET.MapProviders.GMapProviders.OpenStreetMap,
                Position = new PointLatLng(48.8566, 2.3522),
            };

            graphe = new Graphe<int>(noeudsFile, arcsFile);

            overlayStations = new GMapOverlay();
            AfficherStations();

            lignes = new Dictionary<string, List<Noeud<int>>>();
            couleursLignes = new Dictionary<string, Color>();
            AfficherLignes();

            this.Controls.Add(gmap);
            gmap.Overlays.Add(overlayLignes);
            gmap.Overlays.Add(overlayStations);

            AjouterPanelBas();
            AjouterBoutonsLignes();

            overlayPlusCourtChemin = new GMapOverlay("chemin");
            gmap.Overlays.Add(overlayPlusCourtChemin);

            this.Text = "Visualisation du Graphe";
            this.Width = 1000;
            this.Height = 700;
        }

        #region Affichage carte
        /// <summary>
        /// Cette fonction crée un marqueur pour chaque station.
        /// </summary>
        private void AfficherStations()
        {
            marqueursStations = new Dictionary<string, GMarkerGoogle>();
            foreach (var station in graphe.GetNoeuds())
            {
                string nomLigne = station.Ligne;

                if (marqueursStations.TryGetValue(station.Nom, out var existingMarker))
                {
                    existingMarker.ToolTipText += ", " + nomLigne;
                }
                else
                {
                    var marker = new GMarkerGoogle(
                        new PointLatLng(station.Latitude, station.Longitude),
                        GMarkerGoogleType.blue_dot);
                    marker.ToolTipText = station.Nom + Environment.NewLine + "Ligne : " + nomLigne;
                    marker.ToolTipMode = MarkerTooltipMode.OnMouseOver;
                    overlayStations.Markers.Add(marker);
                    marqueursStations[station.Nom] = marker;
                }
            }
        }

        /// <summary>
        /// Cette fonction permet de regrouper les noeuds en fonction de leur ligne.
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
            AssignerCouleursLignes();
        }

        /// <summary>
        /// Cette fonction assigne une couleur aléatoire à chaque ligne.
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
        /// Cette méthode crée et remplit un overlay pour les lignes.
        /// </summary>
        private void AfficherLignes()
        {
            CalculerLignes();
            overlayLignes = new GMapOverlay("lignes");
            foreach (var ligne in lignes)
            {
                string nomLigne = ligne.Key;
                List<Noeud<int>> stations = ligne.Value;
                stations = stations.OrderBy(s => s.Id).ToList();
                List<PointLatLng> points = new List<PointLatLng>();
                foreach (var station in stations)
                {
                    points.Add(new PointLatLng(station.Latitude, station.Longitude));
                }
                if (points.Count >= 2)
                {
                    GMapRoute route = new GMapRoute(points, nomLigne)
                    {
                        Stroke = new Pen(couleursLignes[nomLigne], 3)
                    };
                    overlayLignes.Routes.Add(route);
                }
            }
        }
        #endregion

        #region Gestion interaction utilisateur

        #region Pannel Lignes
        /// <summary>
        /// Cette méthode crée un panel sur la droite de l'écran. 
        /// De plus elle crée directement des boutons pour chacune des lignes permettant d'activer et désactiver leur affichage.
        /// Enfin elle crée un bouton permettant d'activer ou désactiver l'affichage de l'ensemble des lignes directement.
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

                gmap.Refresh();
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
                    foreach (var route in overlayLignes.Routes.Where(r => r.Name == ligne))
                        route.IsVisible = checkBox.Checked;
                    gmap.Refresh();
                };
                boutonsLignes[ligne] = checkBox;
                panelLignes.Controls.Add(checkBox);
                y += 25;
            }
            this.Controls.Add(panelLignes);
        }
        #endregion

        #region Panel bas

        /// <summary>
        /// Cette méthode crée le panel du bas et y implémente les différents boutons.
        /// </summary>
        private void AjouterPanelBas()
        {
            panelBas = new Panel
            {
                Dock = DockStyle.Bottom,
                Height = 60,
                Padding = new Padding(5)
            };
            btnAfficherStations = new Button
            {
                Text = "Masquer stations",
                Dock = DockStyle.Left,
                Width = 120,
            };
            btnAfficherStations.Click += BtnAfficherStation_Click;
            panelBas.Controls.Add(btnAfficherStations);
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
                Location = new Point(420, 15)
            };
            btnCalculerChemin.Click += BtnCalculerChemin_Click;
            panelBas.Controls.Add(btnCalculerChemin);
            btnAfficherChemin = new Button
            {
                Text = "Afficher chemin",
                Width = 120,
                Location = new Point(520, 15)
            };
            btnAfficherChemin.Click += BtnAfficherChemin_Click;
            panelBas.Controls.Add(btnAfficherChemin);

            this.Controls.Add(panelBas);
        }

        #region Méthodes annexe

        /// <summary>
        /// Cette méthode crée le texte lié à l'affichage du plus court chemin après avoir géré l'affichage des liens entre les stations du chemin.
        /// </summary>
        private string AfficherCheminActuel()
        {
            overlayPlusCourtChemin.Routes.Clear();
            if (cheminActuel == null || cheminActuel.Count < 2)
                return "Aucun chemin trouvé.";
            var pts = cheminActuel
                .Select(n => new PointLatLng(n.Latitude, n.Longitude))
                .ToList();
            var route = new GMapRoute(pts, "chemin")
            {
                Stroke = new Pen(Color.Red, 4)
            };
            overlayPlusCourtChemin.Routes.Add(route);
            gmap.Refresh();
            string ligneActuelle = null;
            string retour = "";
            string stationsLigne = "";
            foreach (var station in cheminActuel)
            {
                if (ligneActuelle == null)
                {
                    stationsLigne += " (" + station.Nom;
                    ligneActuelle = station.Ligne;
                    retour += "\nLigne " + ligneActuelle + " : " + station.Nom + " -> ";
                }
                else if (ligneActuelle != station.Ligne)
                {
                    stationsLigne += ")";
                    ligneActuelle = station.Ligne;
                    retour += station.Nom + stationsLigne + "\nLigne " + ligneActuelle + " : " + station.Nom + " -> ";
                    stationsLigne = " (" + station.Nom;
                }
                else if (station.Nom == txtStationArrivee.Text)
                {
                    stationsLigne += " -> " + station.Nom + ")";
                    retour += station.Nom + stationsLigne;
                }
                else
                {
                    stationsLigne += " -> " + station.Nom;
                }
            }
            return retour;
        }

        /// <summary>
        /// Cette méthode affiche une boite de texte donnant le résultat du chemin le plus court si celui ci est valable et un message d'erreur sinon.
        /// </summary>
        /// <param name="stationDepart">Station de départ du plus court chemin à calculer</param>
        /// <param name="stationArrivee">Station d'arrivée du plus court chemin à calculer</param>
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
            MessageBox.Show("Chemin trouvé ! Coût total : " + coutTotal + " minutes \nStation parcourues : " + AfficherCheminActuel() + "\nVous êtes arrivé !", "Résultat", MessageBoxButtons.OK, MessageBoxIcon.Information);
            gmap.Refresh();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="coloration">Résultat de la coloration du graphe</param>
        private void AppliquerColoration(Dictionary<Noeud<int>, int> coloration)
        {

        }

        #endregion

        #region Méthodes pour les boutons

        /// <summary>
        /// Méthode appliqué lorsque le bouton Calculer chemin est cliqué.
        /// Elle appelle la fonction Calculer chemin.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnCalculerChemin_Click(object sender, EventArgs e)
        {
            CalculerChemin(txtStationDepart.Text, txtStationArrivee.Text);
        }

        /// <summary>
        /// Méthode appliqué lorsque le bouton Afficher chemin est cliqué. 
        /// Il active un overlay comprenant uniquement les stations du plus court chemin.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnAfficherChemin_Click(object sender, EventArgs e)
        {
            if (cheminActuel == null || cheminActuel.Count == 0)
            {
                MessageBox.Show("Aucun chemin calculé à afficher.", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            foreach (var station in cheminActuel)
            {
                if (marqueursStations.TryGetValue(station.Nom, out var marker))
                {
                    overlayStations.Markers.Add(marker);
                }
            }
            gmap.Refresh();
        }

        /// <summary>
        /// Méthode appliqué lorsque le bouton Afficher / Masquer stations est cliqué.
        /// Elle change l'état de l'overlay des stations afin qu'il soit visible ou non.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnAfficherStation_Click(object sender, EventArgs e)
        {
            afficherStations = !afficherStations;
            if (afficherStations)
            {
                foreach (var marker in marqueursStations.Values)
                {
                    overlayStations.Markers.Add(marker);
                }
                (sender as Button).Text = "Masquer stations";
            }
            else
            {
                overlayStations.Markers.Clear();
                (sender as Button).Text = "Afficher stations";
            }
            gmap.Refresh();
        }

        /// <summary>
        /// Méthode appliqué lorsque le bouton Exporter est cliqué.
        /// Elle sauvegarde l'état actuel du graphe sous forme d'image .png.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnExporter_Click(object sender, EventArgs e)
        {
            using (Bitmap bmp = new Bitmap(this.ClientSize.Width, this.ClientSize.Height))
            {
                gmap.DrawToBitmap(bmp, new Rectangle(0, 0, bmp.Width, bmp.Height));
                bmp.Save("graphe.png");
                MessageBox.Show("Graphe exporté sous 'graphe.png'", "Export", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        /// <summary>
        /// Méthode appliqué lorsque le bouton Afficher coloration est cliqué.
        /// Elle permet de voir visuellement la coloration du graphe.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnAfficherColoration_Click(object sender, EventArgs e)
        {
            Dictionary<Noeud<int>, int> coloration = Graphe<int>.WelshPowell(graphe.GetListeAdjacence());
            if (coloration == null || coloration.Count == 0)
            {
                MessageBox.Show("Aucune coloration calculée.", "Coloration", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            AppliquerColoration(coloration);
        }


        #endregion

        #endregion

        #endregion

        private void Form2_Load(object sender, EventArgs e)
        {

        }
    }
}
