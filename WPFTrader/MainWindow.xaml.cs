using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using GestionnaireBDD;
using MetierTrader;

namespace WPFTrader
{
    /// <summary>
    /// Logique d'interaction pour MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        GstBdd unGstBdd;

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            unGstBdd = new GstBdd();
            lstTraders.ItemsSource = unGstBdd.getAllTraders();
            
        }

        private void lstTraders_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            lstActions.ItemsSource = unGstBdd.getAllActionsByTrader((lstTraders.SelectedItem as Trader).NumTrader);
            lstActionsNonPossedees.ItemsSource = unGstBdd.getAllActionsNonPossedees((lstTraders.SelectedItem as Trader).NumTrader);
            txtTotalPortefeuille.Text = unGstBdd.getTotalPortefeuille((lstTraders.SelectedItem as Trader).NumTrader).ToString();
        }

        private void lstActions_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if(unGstBdd.getCoursReel((lstActions.SelectedItem as ActionPerso).NumAction) == (lstActions.SelectedItem as ActionPerso).PrixAchat)
            {
                imgAction.Source = new BitmapImage(new Uri("Images/Moyen.png", UriKind.RelativeOrAbsolute));
            }
            else if(unGstBdd.getCoursReel((lstActions.SelectedItem as ActionPerso).NumAction) < (lstActions.SelectedItem as ActionPerso).PrixAchat)
            {
                imgAction.Source = new BitmapImage(new Uri("Images/Haut.png", UriKind.RelativeOrAbsolute));
            }
            else
            {
                imgAction.Source = new BitmapImage(new Uri("Images/Bas.png", UriKind.RelativeOrAbsolute));
            }
        }

        private void btnVendre_Click(object sender, RoutedEventArgs e)
        {
            if(lstActions.SelectedItem as ActionPerso == null)
            {
                MessageBox.Show("Sélectionner une action", "Erreur de saisie", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            else
            {
                if(txtQuantiteVendue.Text == "")
                {
                    MessageBox.Show("Sélectionner une quantite", "Erreur de saisie", MessageBoxButton.OK, MessageBoxImage.Error);
                }
                else
                {
                    if(Convert.ToInt16(txtQuantiteVendue.Text) > (lstActions.SelectedItem as ActionPerso).Quantite)
                    {
                        MessageBox.Show("Impossible de vendre ce que vous possèdez", "Erreur de saisie", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                    else
                    {
                        if(Convert.ToInt16(txtQuantiteVendue.Text) == (lstActions.SelectedItem as ActionPerso).Quantite)
                        {
                            lstActions.ItemsSource = unGstBdd.getAllActionsByTrader((lstTraders.SelectedItem as Trader).NumTrader);
                            txtTotalPortefeuille.Text = unGstBdd.getTotalPortefeuille((lstTraders.SelectedItem as Trader).NumTrader).ToString();
                            unGstBdd.SupprimerActionAcheter((lstActions.SelectedItem as ActionPerso).NumAction, (lstTraders.SelectedItem as Trader).NumTrader);
                        }
                        else
                        {
                            if (Convert.ToInt16(txtQuantiteVendue.Text) < (lstActions.SelectedItem as ActionPerso).Quantite)
                            {
                                unGstBdd.UpdateQuantite((lstActions.SelectedItem as ActionPerso).NumAction, (lstTraders.SelectedItem as Trader).NumTrader, Convert.ToInt32(txtQuantiteVendue.Text));
                                txtTotalPortefeuille.Text = "";
                                txtTotalPortefeuille.Text = unGstBdd.getTotalPortefeuille((lstTraders.SelectedItem as Trader).NumTrader).ToString();
                                lstActions.ItemsSource = "";
                                lstActions.ItemsSource = unGstBdd.getAllActionsByTrader((lstTraders.SelectedItem as Trader).NumTrader);
                            }
                        }
                    }
                }
            }
        }

        private void btnAcheter_Click(object sender, RoutedEventArgs e)
        {
            if(lstActionsNonPossedees.SelectedItem as MetierTrader.Action == null)
            {
                MessageBox.Show("Sélectionner une action", "Erreur de saisie", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            else
            {
                if(txtPrixAchat.Text == "")
                {
                    MessageBox.Show("Sélectionner un prix", "Erreur de saisie", MessageBoxButton.OK, MessageBoxImage.Error);
                }
                else
                {
                    if(txtQuantiteAchetee.Text == "")
                    {
                        MessageBox.Show("Sélectionner une quantite", "Erreur de saisie", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                    else
                    {
                        MessageBox.Show("Action enregistrée", "Information", MessageBoxButton.OK, MessageBoxImage.Error);
                        unGstBdd.AcheterAction((lstActions.SelectedItem as ActionPerso).NumAction, (lstTraders.SelectedItem as Trader).NumTrader, Convert.ToDouble(txtPrixAchat.Text), Convert.ToInt32(txtQuantiteAchetee.Text));
                        lstActions.ItemsSource = "";
                        lstActions.ItemsSource = unGstBdd.getAllActionsByTrader((lstTraders.SelectedItem as Trader).NumTrader);
                        txtTotalPortefeuille.Text = "";
                        txtTotalPortefeuille.Text = unGstBdd.getTotalPortefeuille((lstTraders.SelectedItem as Trader).NumTrader).ToString();
                        lstActionsNonPossedees.ItemsSource = "";
                        lstActionsNonPossedees.ItemsSource = unGstBdd.getAllActionsNonPossedees((lstTraders.SelectedItem as Trader).NumTrader);

                    }
                }
            }
        }
    }
}
