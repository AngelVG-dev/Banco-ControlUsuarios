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
    /// Lógica de interacción para Login.xaml
    /// </summary>
    public partial class Login : Window
    {
        public Login()
        {
            InitializeComponent();
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

        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }
         
        private void btnLogin_Click(object sender, RoutedEventArgs e)
        {
         string usuario= txtNombre.Text;
         string contra= pwdContraseña.Password;
            UsuarioSistemaDAL acceso = new UsuarioSistemaDAL();
            UsuarioSistema usuariosistema = acceso.ValidarLogin(usuario, contra);
            if (usuariosistema != null)
            {
    
                Dashboard dashboard = new Dashboard(usuariosistema);
                dashboard.Show();
                this.Close();

            }
            else
            {
                MessageBox.Show("Datos incorrectos, vuelve a intentarlo nuevamente");
            }
            
        }

        private void txtNombre_TextChanged(object sender, TextChangedEventArgs e)
        {

        }
    }
}
