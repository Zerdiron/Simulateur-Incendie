using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Navigation;

namespace TP9
{
	/// <summary>
	/// Interaction logic for Menu.xaml
	/// </summary>
	public partial class Menu : Page
	{
		// Attributs.
		/// <summary>
		/// Booléen vérifiant si la vitesse est incorrect ou non.
		/// </summary>
		private bool vitesseIncorrect = true;

		/// <summary>
		/// La vitessu du vent.
		/// </summary>
		private double vitesse;

		/// <summary>
		/// La sauvegarde des couleurs au moment du changement de vent.
		/// </summary>
		private Brush[,] SaveDesCouleurs;

		/// <summary>
		/// La sauvegarde des couleurs pour le bouton de réinitialisation.
		/// </summary>
		private Brush[,] SaveReinitialiser;

		/// <summary>
		/// La sauvegarde du nombre d'étape.
		/// </summary>
		private short SaveNombreEtape;

		// Constructeurs.
		/// <summary>
		/// Constructeur de base.
		/// </summary>
		public Menu()
		{
			InitializeComponent();
			ProportionVerte.Value = 80;
			ProportionMarron.Value = 10;
			ProportionBleue.Value = 10;
		}

		/// <summary>
		/// Constructeur quand on veut changer de vent.
		/// </summary>
		/// <param name="_SaveReinitialiser">Sauvegarde de la map pour le bouton réinitialiser.</param>
		/// <param name="_NombreEtape">Save du nombre d'étape.</param>
		/// <param name="_SaveDesCouleurs">Save des couleurs en cours.</param>
		public Menu(Brush[,] _SaveReinitialiser, short _NombreEtape, Brush[,] _SaveDesCouleurs)
		{
			SaveReinitialiser = _SaveReinitialiser;
			SaveDesCouleurs = _SaveDesCouleurs;
			SaveNombreEtape = _NombreEtape;
			InitializeComponent();
			/// Nord-Ouest.
			//NO.Click -= FirstChoice_Click;
			//NO.Click -= SecondChoice_Click;
			NO.Click += SecondChoice_Click;
			/// Nord.
			//N.Click -= FirstChoice_Click;
			//N.Click -= SecondChoice_Click;
			N.Click += SecondChoice_Click;
			/// Nord-Est.
			//NE.Click -= FirstChoice_Click;
			//NE.Click -= SecondChoice_Click;
			NE.Click += SecondChoice_Click;
			/// Est.
			//E.Click -= FirstChoice_Click;
			//E.Click -= SecondChoice_Click;
			E.Click += SecondChoice_Click;
			/// Ouest.
			O.Click -= FirstChoice_Click;
			O.Click -= SecondChoice_Click;
			O.Click += SecondChoice_Click;
			/// Sud-Est.
			//SE.Click -= FirstChoice_Click;
			//SE.Click -= SecondChoice_Click;
			SE.Click += SecondChoice_Click;
			/// Sud.
			//S.Click -= FirstChoice_Click;
			//S.Click -= SecondChoice_Click;
			S.Click += SecondChoice_Click;
			/// Sud-Ouest.
			//SO.Click -= FirstChoice_Click;
			//SO.Click -= SecondChoice_Click;
			SO.Click += SecondChoice_Click;
		}

		// Fonctions.
		/// <summary>
		/// Action des boutons permettant de choisir la direction du vent..
		/// </summary>
		/// <param name="direction">La direction du vent.</param>
		/// <param name="Second">Si c'est l'action du bouton normal ou non.</param>
		private void Action(string direction, bool Second = false)
		{
			if (!vitesseIncorrect && !Second)
			{
				Simulateur Simul = new Simulateur(vitesse, direction, ProportionVerte.Value, ProportionMarron.Value);
				NavigationService.Navigate(Simul);
			}
			else if (!vitesseIncorrect && Second)
			{
				Simulateur Simul = new Simulateur(vitesse, direction, SaveDesCouleurs, SaveNombreEtape, SaveReinitialiser, ProportionVerte.Value, ProportionMarron.Value);
				NavigationService.Navigate(Simul);
			}
			else MessageBox.Show(string.Format("Entrez une vitesse entre 0,0 et 300,0 !"), "Nombre invalide", MessageBoxButton.OK);
		}

		// Evènements.
		/// <summary>
		/// Evènement de la textBox de vitesse.
		/// </summary>
		private void TextBox_LostFocus(object sender, RoutedEventArgs e)
		{
			bool ConversionReussie;
			ConversionReussie = double.TryParse(Vitesse.Text, out vitesse);
			if (!ConversionReussie)
				MessageBox.Show("Entrez une vitesse au format 0,0 !", "Nombre invalide", MessageBoxButton.OK);
			else if (ConversionReussie && vitesse > 0 && vitesse < 300)
				vitesseIncorrect = false;
			else
				MessageBox.Show("La vitesse dépasse 300 ou est inférieure à 0.", "Nombre invalide", MessageBoxButton.OK);
		}

		/// <summary>
		/// Action First.
		/// </summary>
		private void FirstChoice_Click(object sender, RoutedEventArgs e)
		{
			if (sender.Equals(NO))
				Action("Nord-Ouest");
			else if (sender.Equals(N))
				Action("Nord");
			else if (sender.Equals(NE))
				Action("Nord-Est");
			else if (sender.Equals(O))
				Action("Ouest");
			else if (sender.Equals(E))
				Action("Est");
			else if (sender.Equals(SO))
				Action("Sud-Ouest");
			else if (sender.Equals(S))
				Action("Sud");
			else if (sender.Equals(SE))
				Action("Sud-Est");
		}

		/// <summary>
		/// Action Second.
		/// </summary>
		private void SecondChoice_Click(object sender, RoutedEventArgs e)
		{
			if (sender.Equals(NO))
				Action("Nord-Ouest", true);
			else if (sender.Equals(N))
				Action("Nord", true);
			else if (sender.Equals(NE))
				Action("Nord-Est", true);
			else if (sender.Equals(O))
				Action("Ouest", true);
			else if (sender.Equals(E))
				Action("Est", true);
			else if (sender.Equals(SO))
				Action("Sud-Ouest", true);
			else if (sender.Equals(S))
				Action("Sud", true);
			else if (sender.Equals(SE))
				Action("Sud-Est", true);
		}

		/// <summary>
		/// Met a jour les sliders en fonctions des autres.
		/// </summary>
		private void Proportion_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
		{
			if (sender.Equals(ProportionVerte))
			{
				double v = ProportionVerte.Value;
				ProportionMarron.Value = 100 - ProportionBleue.Value - v;
			}
			if (sender.Equals(ProportionBleue))
			{
				double b = ProportionBleue.Value;
				ProportionMarron.Value = 100 - b - ProportionVerte.Value;
			}
			if (sender.Equals(ProportionMarron))
			{
				double m = ProportionMarron.Value;
				ProportionBleue.Value = 100 - m - ProportionVerte.Value;
			}
		}
	}
}
