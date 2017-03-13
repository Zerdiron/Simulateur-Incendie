using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Navigation;
using System.Windows.Threading;

namespace TP9
{
	/// <summary>
	/// Interaction logic for Simulateur.xaml
	/// </summary>
	public partial class Simulateur : Page
	{
		// Attributs.
		// ### Relatif au vent ### \\
		/// <summary>
		/// La vitesse du vent en km/h.
		/// </summary>
		private double _vitesseVent;

		/// <summary>
		/// La direction du vent.
		/// </summary>
		private string _vent;

		/// <summary>
		/// Petite Variation dans le tableau en fonction des directions.
		/// </summary>
		private short _nord = -1;

		/// <summary>
		/// Petite Variation dans le tableau en fonction des directions.
		/// </summary>
		private short _sud = 1;

		/// <summary>
		/// Petite Variation dans le tableau en fonction des directions.
		/// </summary>
		private short _ouest = -1;

		/// <summary>
		/// Petite Variation dans le tableau en fonction des directions.
		/// </summary>
		private short _est = 1;

		/// <summary>
		/// Mutliplicateur dû au vent.
		/// </summary>
		private int _multiplicateur;

		/// <summary>
		/// Le décalage Vertical causé par le vent.
		/// </summary>
		private int _decalageVentVertical = 0;

		/// <summary>
		/// Le décalage Horizontal causé par le vent.
		/// </summary>
		private int _decalageVentHorizontal = 0;

		// ### Les proportions ### \\
		/// <summary>
		/// La proportion de forêt.
		/// </summary>
		private double ProportionVerte = 80;

		/// <summary>
		/// La proportion de plaine.
		/// </summary>
		private double ProportionMarron = 10;

		// ### Le compteur d'étapes. ### \\
		/// <summary>
		/// Le nombre actuel d'étape.
		/// </summary>
		private short _nombreEtape = 0;

		// ### Les tableaux du simulateur. ### \\
		/// <summary>
		/// Le tableau contenant les cases.
		/// </summary>
		public Label[,] LesCases = new Label[9, 6];

		/// <summary>
		/// Le Tableau de sauvergarde de la map en cours.
		/// </summary>
		public Brush[,] SaveLesCases = new Brush[9, 6];

		/// <summary>
		/// La sauvegarde pour le bouton réinitialiser.
		/// </summary>
		public Brush[,] _SaveReinitialiser = new Brush[9, 6];

		// ### Le nombre aléatoire. ### \\
		/// <summary>
		/// Le randomizer.
		/// </summary>
		private readonly Random _randomize = new Random();

		// ### Les couleurs. ### \\
		/// <summary>
		/// Couleur Verte.
		/// </summary>
		private static SolidColorBrush _vert = Brushes.Green;

		/// <summary>
		/// Couleur Rouge.
		/// </summary>
		private static SolidColorBrush _rouge = Brushes.Red;

		/// <summary>
		/// Couleur Marron.
		/// </summary>
		private static SolidColorBrush _marron = Brushes.SaddleBrown;

		/// <summary>
		/// Couleur Bleue.
		/// </summary>
		private static SolidColorBrush _bleu = Brushes.Blue;

		/// <summary>
		/// Couleur Orange.
		/// </summary>
		private static SolidColorBrush _orange = Brushes.Orange;

		/// <summary>
		/// Timer de la carte;
		/// </summary>
		private DispatcherTimer Time;
		
		// Constructeurs.
		/// <summary>
		/// Constructeur du Simulateur.
		/// </summary>
		/// <param name="vitesse">La vitesse du vent.</param>
		/// <param name="direction">La direction du vent.</param>
		public Simulateur(double vitesse, string direction, double _proportionVerte, double _proportionMarron)
		{
			_vent = direction;
			_vitesseVent = vitesse;

			_assignationDecalageVent();

			InitializeComponent();

			Direction.Content = _vent;
			Vitesse.Content = string.Format("{0} km/h", _vitesseVent);

			ProportionVerte = _proportionVerte;
			ProportionMarron = _proportionMarron;

			Time = new DispatcherTimer();
			Time.Interval = TimeSpan.FromSeconds(1);
			Time.Tick += new EventHandler(Repandre);

			_creerZone();
			MakeSave();
		}

		/// <summary>
		/// Constructeur du Simulateur après Chagement de direction du vent.
		/// </summary>
		/// <param name="vitesse">La nouvelle vitesse du vent.</param>
		/// <param name="direction">La nouvelle direction du vent.</param>
		/// <param name="SaveDesCouleurs">La sauvegarde des couleurs en cours.</param>
		/// <param name="SaveNombreEtape">La sauvegarde de l'étape en cours.</param>
		/// <param name="SaveReinitialiser">La sauvergade de la carte en cours sans feu.</param>
		public Simulateur(double vitesse, string direction, Brush[,] SaveDesCouleurs, short SaveNombreEtape, Brush[,] SaveReinitialiser, double _proportionVerte, double _proportionMarron)
		{
			_vent = direction;
			_vitesseVent = vitesse;
			_nombreEtape = SaveNombreEtape;
			_SaveReinitialiser = SaveReinitialiser;

			_assignationDecalageVent();

			InitializeComponent();

			if (_nombreEtape > 0)
			{
				EtatSuivant.Content = string.Format("Etape {0}", _nombreEtape);
				EtatSuivant.Background = Brushes.Coral;
			}

			Direction.Content = _vent;
			Vitesse.Content = string.Format("{0} km/h", _vitesseVent);

			ProportionVerte = _proportionVerte;
			ProportionMarron = _proportionMarron;

			Time = new DispatcherTimer();
			Time.Interval = TimeSpan.FromSeconds(1);
			Time.Tick += new EventHandler(Repandre);

			_creerZone();
			for (short i = 0; i < 9; i++)
				for (short j = 0; j < 6; j++)
				{
					if (LesCases[i, j].Background == _vert)
						LesCases[i, j].ToolTip = "Une belle forêt !";
					else if (LesCases[i, j].Background == _marron)
						LesCases[i, j].ToolTip = "Une jolie plaine !";
					else if (LesCases[i, j].Background == _bleu)
						LesCases[i, j].ToolTip = "Un magnifique point d'eau.";
					else if (LesCases[i, j].Background == _rouge)
						LesCases[i, j].ToolTip = "L'orgine du Feu !";
					else if (LesCases[i, j].Background == _orange)
						LesCases[i, j].ToolTip = "Le Feu se répand ! Attention !";
					LesCases[i, j].Background = SaveDesCouleurs[i, j];
				}
		}

		// Fonctions.
		/// <summary>
		/// Génère une couleur aléatoire.
		/// </summary>
		/// <returns></returns>
		private SolidColorBrush _generationCouleurAleatoire()
		{
			SolidColorBrush colori;
			int Kouleur = _randomize.Next(100);

			if (Kouleur < ProportionVerte)
				colori = _vert;
			else if (Kouleur < ProportionVerte + ProportionMarron)
				colori = _marron;
			else
				colori = _bleu;
			return colori;
		}

		/// <summary>
		/// Créer une zone.
		/// </summary>
		private void _creerZone()
		{
			for (short i = 0; i < 9; i++)
				for (short j = 0; j < 6; j++)
					_creerCase(i, j, _generationCouleurAleatoire());
		}

		/// <summary>
		/// Créer une case de la zone.
		/// </summary>
		/// <param name="i">Le i de la boucle.</param>
		/// <param name="j">Le j de la boucle.</param>
		/// <param name="Kouleur">La couleur de la case.</param>
		private void _creerCase(short i, short j, SolidColorBrush Kouleur)
		{
			Label Case = new Label();
			Case.Background = Kouleur;
			Case.Name = string.Format("Case_{0}{1}", i, j);
			if (Kouleur == _vert)
				Case.ToolTip = "Une belle forêt !";
			else if (Kouleur == _marron)
				Case.ToolTip = "Une jolie plaine !";
			else if (Kouleur == _bleu)
				Case.ToolTip = "Un magnifique point d'eau.";
			Grid.SetColumn(Case, i);
			Grid.SetRow(Case, j);
			Grid_Two.Children.Add(Case);
			LesCases[i, j] = Case;
		}

		/// <summary>
		/// Assigne la direction du vent en terme d'indices.
		/// </summary>
		private void _assignationDecalageVent()
		{
			if (_vitesseVent < 100)
				_multiplicateur = 1;
			else if (_vitesseVent < 200)
				_multiplicateur = 2;
			else
				_multiplicateur = 3;

			if (_vent == "Nord")
			{
				_decalageVentVertical = _nord;
				_decalageVentHorizontal = 0;
			}
			else if (_vent == "Ouest")
			{
				_decalageVentVertical = 0;
				_decalageVentHorizontal = _ouest;
			}
			else if (_vent == "Est")
			{
				_decalageVentVertical = 0;
				_decalageVentHorizontal = _est;
			}
			else if (_vent == "Sud")
			{
				_decalageVentVertical = _sud;
				_decalageVentHorizontal = 0;
			}
			else if (_vent == "Nord-Ouest")
			{
				_decalageVentVertical = _nord;
				_decalageVentHorizontal = _ouest;
			}
			else if (_vent == "Nord-Est")
			{
				_decalageVentVertical = _nord;
				_decalageVentHorizontal = _est;
			}
			else if (_vent == "Sud-Ouest")
			{
				_decalageVentVertical = _sud;
				_decalageVentHorizontal = _ouest;
			}
			else if (_vent == "Sud-Est")
			{
				_decalageVentVertical = _sud;
				_decalageVentHorizontal = _est;
			}
		}

		/// <summary>
		/// Met le feu à une case aléatoire du tableau.
		/// </summary>
		private void _setFIRE()
		{
			int newFirei, newFirej; bool FiringForEffect = false;
			do
			{
				newFirei = _randomize.Next(0, 9);
				newFirej = _randomize.Next(0, 6);
				if (LesCases[newFirei, newFirej].Background == _vert)
				{
					LesCases[newFirei, newFirej].Background = _rouge;
					LesCases[newFirei, newFirej].ToolTip = "L'orgine du Feu !";
					FiringForEffect = true;
				}
			} while (!FiringForEffect);
		}

		/// <summary>
		/// Remet les boutons à leurs états d'ogirines.
		/// </summary>
		private void Reinitialiser_button()
		{
			EtatSuivant.Background = _rouge;
			EtatSuivant.Content = "SET FIRE";
			_nombreEtape = 0;
			Time.Stop();
		}

		/// <summary>
		/// Effectue une sauvegarde vierge de la carte en cours.
		/// </summary>
		private void MakeSave()
		{
			for (short i = 0; i < 9; i++)
				for (short j = 0; j < 6; j++)
				{
					SaveLesCases[i, j] = LesCases[i, j].Background;
					_SaveReinitialiser[i, j] = LesCases[i, j].Background;
				}
		}

		/// <summary>
		/// On répand le feu !
		/// </summary>
		/// <param name="TableauIndiceFire">Le tableau qui contient les coordonées 'i' du feu.</param>
		/// <param name="TableauJndiceFire">Le tableau qui contient les coordonées 'j' du feu.</param>
		/// <param name="i">Les indices 'i' des boucles.</param>
		/// <param name="j">Les indices 'j' des boucles.</param>
		private void RepandreFeu(short[] TableauIndiceFire, short[] TableauJndiceFire, short i, short j)
		{
			bool toConquer = false;
			if (_multiplicateur == 1 && _nombreEtape % 8 == 0)
				toConquer = true;
			else if (_multiplicateur == 2 && _nombreEtape % 4 == 0)
				toConquer = true;
			else if (_multiplicateur == 3 && _nombreEtape % 2 == 0)
				toConquer = true;
			if (toConquer)
			{
				if (TableauIndiceFire[i] + 1 < 9)
					if (LesCases[TableauIndiceFire[i] + 1, TableauJndiceFire[j]].Background == _vert)
					{
						LesCases[TableauIndiceFire[i] + 1, TableauJndiceFire[j]].Background = _orange;
						LesCases[TableauIndiceFire[i] + 1, TableauJndiceFire[j]].ToolTip = "Le Feu se répand ! Attention !";
					}
				if (TableauIndiceFire[i] - 1 >= 0)
					if (LesCases[TableauIndiceFire[i] - 1, TableauJndiceFire[j]].Background == _vert)
					{
						LesCases[TableauIndiceFire[i] - 1, TableauJndiceFire[j]].Background = _orange;
						LesCases[TableauIndiceFire[i] - 1, TableauJndiceFire[j]].ToolTip = "Le Feu se répand ! Attention !";
					}
				if (TableauJndiceFire[j] + 1 < 6)
					if (LesCases[TableauIndiceFire[i], TableauJndiceFire[j] + 1].Background == _vert)
					{
						LesCases[TableauIndiceFire[i], TableauJndiceFire[j] + 1].Background = _orange;
						LesCases[TableauIndiceFire[i], TableauJndiceFire[j] + 1].ToolTip = "Le Feu se répand ! Attention !";
					}
				if (TableauJndiceFire[j] - 1 >= 0)
					if (LesCases[TableauIndiceFire[i], TableauJndiceFire[j] - 1].Background == _vert)
					{
						LesCases[TableauIndiceFire[i], TableauJndiceFire[j] - 1].Background = _orange;
						LesCases[TableauIndiceFire[i], TableauJndiceFire[j] - 1].ToolTip = "Le Feu se répand ! Attention !";
					}

			}

			if ((TableauIndiceFire[i] + _decalageVentHorizontal < 9 && TableauIndiceFire[i] + _decalageVentHorizontal >= 0) && (TableauJndiceFire[j] + _decalageVentVertical < 6 && TableauJndiceFire[j] + _decalageVentVertical >= 0))
				if (LesCases[TableauIndiceFire[i] + _decalageVentHorizontal, TableauJndiceFire[j] + _decalageVentVertical].Background == _vert)
				{
					LesCases[TableauIndiceFire[i] + _decalageVentHorizontal, TableauJndiceFire[j] + _decalageVentVertical].Background = _orange;
					LesCases[TableauIndiceFire[i] + _decalageVentHorizontal, TableauJndiceFire[j] + _decalageVentVertical].ToolTip = "Le Feu se répand ! Attention !";
				}
		}

		private void Repandre(object sender, EventArgs e)
		{
			if (_nombreEtape > 0) // Pour les étapes suivantes on fait se déplacer le feu.
			{
				/// Tableaux contenant les indices où sont situés les cases enflammées.
				short[] TableauIndiceFire;
				short[] TableauJndiceFire;

				/// Indices de progressions des tableaux d'indices qui s'incrémente uniquement quand une case enflammée est trouvée.
				short indiceFIRE = 0;
				short jndiceFIRE = 0;

				/// En fonction de la vitesse du vent.
				short _ajoutDecalageV = 0;
				short _ajoutDecalageH = 0;

				/// Compte le nombre de cases enflammées.
				short compteurFIRE = 0;

				/// Boucle pour compter le nombre de cases enflammées
				for (short i = 0; i < 9; i++)
					for (short j = 0; j < 6; j++)
						if (LesCases[i, j].Background == _rouge || LesCases[i, j].Background == _orange)
							compteurFIRE++;

				/// On créer les tableaux en fonction du nombre de cases trouvées.
				TableauIndiceFire = new short[compteurFIRE];
				TableauJndiceFire = new short[compteurFIRE];

				/// Puis on stocke les indices où sont les cases dans le tableau.
				for (short i = 0; i < 9; i++)
					for (short j = 0; j < 6; j++)
						if (LesCases[i, j].Background == _rouge || LesCases[i, j].Background == _orange)
						{
							TableauIndiceFire[indiceFIRE] = i;
							TableauJndiceFire[jndiceFIRE] = j;
							indiceFIRE++; jndiceFIRE++;
						}

				/// On ajuste les décalages Horizontaux.
				if (_decalageVentHorizontal > 0)
					_ajoutDecalageH = 1;
				else if (_decalageVentHorizontal < 0)
					_ajoutDecalageH = -1;
				else
					_ajoutDecalageH = 0;

				/// On ajuste les décalages Verticaux.
				if (_decalageVentVertical > 0)
					_ajoutDecalageV = 1;
				else if (_decalageVentVertical < 0)
					_ajoutDecalageV = -1;
				else
					_ajoutDecalageV = 0;

				/// Puis on transforme les cases mémorisées en cases enflammées.
				for (short i = 0; i < TableauIndiceFire.Length; i++)
				{
					for (short j = 0; j < TableauJndiceFire.Length; j++)
					{

						RepandreFeu(TableauIndiceFire, TableauJndiceFire, i, j);

						if (_multiplicateur == 2 || _multiplicateur == 3)
						{
							_decalageVentHorizontal += _ajoutDecalageH; _decalageVentVertical += _ajoutDecalageV;

							RepandreFeu(TableauIndiceFire, TableauJndiceFire, i, j);

							_decalageVentHorizontal -= _ajoutDecalageH; _decalageVentVertical -= _ajoutDecalageV;
						}

						if (_multiplicateur == 3)
						{
							_decalageVentHorizontal += _ajoutDecalageH * 2; _decalageVentVertical += _ajoutDecalageV * 2;

							RepandreFeu(TableauIndiceFire, TableauJndiceFire, i, j);

							_decalageVentHorizontal -= _ajoutDecalageH * 2; _decalageVentVertical -= _ajoutDecalageV * 2;
						}
					}
				}

				EtatSuivant.Content = string.Format("Etape {0}", ++_nombreEtape);
			}
		}

		// Evènements.
		/// <summary>
		/// Evènement lors du click sur le bouton SET FIRE.
		/// </summary>
		private void EtatSuivant_Click(object sender, RoutedEventArgs e)
		{
			if (_nombreEtape == 0) // Si c'est le départ de feu, on créer le feu et on change le bouton.
			{
				EtatSuivant.Content = string.Format("Etape {0}", ++_nombreEtape);
				EtatSuivant.Background = Brushes.Coral;
				EtatSuivant.ToolTip = "Affiche l'étape en cours.";
				_setFIRE();
				
				Time.Start();
			}
		}

		/// <summary>
		/// Réinitialise le map en cours en vierge.
		/// </summary>
		private void Reinitialiser_Click(object sender, RoutedEventArgs e)
		{
			Reinitialiser_button();
			for (short i = 0; i < 9; i++)
				for (short j = 0; j < 6; j++)
				{
					if (_SaveReinitialiser[i, j] == _vert)
						LesCases[i, j].ToolTip = "Une belle forêt !";
					else if (_SaveReinitialiser[i, j] == _marron)
						LesCases[i, j].ToolTip = "Une jolie plaine !";
					else if (_SaveReinitialiser[i, j] == _bleu)
						LesCases[i, j].ToolTip = "Un magnifique point d'eau.";
					LesCases[i, j].Background = _SaveReinitialiser[i, j];
				}
		}

		/// <summary>
		/// Génère une nouvelle map.
		/// </summary>
		private void NewMap_Click(object sender, RoutedEventArgs e)
		{
			Reinitialiser_button();
			_creerZone();
			MakeSave();
		}

		/// <summary>
		/// Modifie le vent.
		/// </summary>
		private void NewWind_Click(object sender, RoutedEventArgs e)
		{
			Brush[,] ActualColor = new Brush[9, 6];

			for (short i = 0; i < 9; i++)
				for (short j = 0; j < 6; j++)
					ActualColor[i, j] = LesCases[i, j].Background;

			Menu ChoixVent = new Menu(SaveLesCases, _nombreEtape, ActualColor);
			NavigationService.Navigate(ChoixVent);
		}
	}
}
