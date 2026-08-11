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
using System.Windows.Shapes;
using CajeroDAL.Cajerodal;

namespace Banco.View
{
    /// <summary>
    /// Lógica de interacción para Dashboard.xaml
    /// </summary>

    public partial class Dashboard : Window

    {

        private UsuarioSistema datosUusuario;
        public Dashboard(UsuarioSistema usuarioDatos)
        {
            InitializeComponent();
            datosUusuario = usuarioDatos;
            AplicarPermisos(); 

        }
        private void AplicarPermisos()
        {
            if (datosUusuario.Rol == "Cliente")
            {
                btnCuenta.IsEnabled = false;
                btnCuenta.Visibility = Visibility.Collapsed;
                btnCliente.IsEnabled = false;
                btnCliente.Visibility = Visibility.Collapsed;
                btnUsuario.IsEnabled = false;
                btnUsuario.Visibility = Visibility.Collapsed;
                btnTarjeta.IsEnabled = false;
                btnTarjeta.Visibility = Visibility.Collapsed;
            }
        }



        private void Button_Click(object sender, RoutedEventArgs e)
        {

            ContentArea.Children.Clear();
            ContentArea.Children.Add(new ClienteControl());

        }

        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

        private void Window_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                DragMove();
            }
        }

        private void btnMinimize_Click(object sender, RoutedEventArgs e)
        {
            WindowState = WindowState.Minimized;
        }


        private void btnCuenta_Click(object sender, RoutedEventArgs e)
        {
            ContentArea.Children.Clear();
            ContentArea.Children.Add(new CuentaControl());
        }

        private void btnUsuario_Click(object sender, RoutedEventArgs e)
        {
            ContentArea.Children.Clear();
            ContentArea.Children.Add(new UsuarioControl());
        }

        private void btnTarjeta_Click(object sender, RoutedEventArgs e)
        {
            ContentArea.Children.Clear();
            ContentArea.Children.Add(new ControlTarjeta());

        }

        private void btnCerrarSesion_Click(object sender, RoutedEventArgs e)
        {
            
            var loginWindow = new Login();
            loginWindow.Show();

            
            this.Close();
        }
    }
}
