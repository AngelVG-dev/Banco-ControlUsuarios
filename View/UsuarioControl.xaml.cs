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
using CajeroDAL.Cajerodal;

namespace Banco.View
{
    /// <summary>
    /// Lógica de interacción para UsuarioControl.xaml
    /// </summary>
    public partial class UsuarioControl : UserControl
    {
        List<UsuarioSistema> _lstUsuario;
        UsuarioSistema _objUsuario;
        bool bandera = false;
        bool banderaEditar = false;
        int usuarioID = 0;
        public UsuarioControl()
        {
            InitializeComponent();
            try
            {
                cargarDatos();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al cargar UsuarioControl: " + ex.Message);
            }
        }


        private bool validarCampos()
        {
            if (txtNombre.Text.Equals(""))
            {
                MessageBox.Show("El nombre del usuario es requerido.");
                txtNombre.Focus();
                return false;
            }
            if (txtUsuario.Text.Equals(""))
            {
                MessageBox.Show("El apodo del nombre del usuario es requerido.");
                txtUsuario.Focus();
                return false;
            }
            if (pwdContraseña.Password.Equals(""))
            {
                MessageBox.Show("La contraseña del usuario es requerido");
                pwdContraseña.Focus();
                return false;
            }
            if (!(cmbRol.SelectedIndex >= 0))
            {
                MessageBox.Show("El tipo de rol del usuario.");
                cmbRol.Focus();
                return false;
            }
            if (!(cmbEstado.SelectedIndex >= 0))
            {
                MessageBox.Show("El estado del usuario es requido");
                cmbEstado.Focus();
                return false;
            }
            return true;
        }

        void llenarTablaUsuarios(List<UsuarioSistema> listaUsuarioSistemas)
        {
            dgUsuarios.ItemsSource = null;
            dgUsuarios.ItemsSource = listaUsuarioSistemas;
        }

        private void cargarDatos()
        {
            UsuarioSistemaDAL clientePro = new UsuarioSistemaDAL();
            _lstUsuario = clientePro.ObtenerUsuarios();
            llenarTablaUsuarios(_lstUsuario);

        }

        private void limpiarCampos()
        {
            txtNombre.Text = "";
            txtUsuario.Text = "";
            pwdContraseña.Password = "";
            cmbRol.SelectedItem = null;
            cmbRol.Text = "Seleccione..";
            cmbEstado.SelectedItem = null;
            cmbEstado.Text = "Seleccione..";
            banderaEditar = false;
            usuarioID = 0;
        }

        private void obtenerCampos()
        {
            usuarioID = _objUsuario.UsuarioID;
            txtNombre.Text = _objUsuario.Nombre;
            txtUsuario.Text = _objUsuario.Usuario;
            pwdContraseña.Password = _objUsuario.Contraseña;
            foreach (ComboBoxItem item in cmbRol.Items)
            {
                if ((string)item.Content == _objUsuario.Rol)
                {
                    cmbRol.SelectedItem = item;
                    break;
                }
            }
            foreach (ComboBoxItem item in cmbEstado.Items)
            {
                if ((string)item.Content == _objUsuario.Estado)
                {
                    cmbEstado.SelectedItem = item;
                    break;
                }
            }

        }

        private void btnGuardarUsuario_Click(object sender, RoutedEventArgs e)
        {
            if (validarCampos())
            {
                string nombre = txtNombre.Text;
                string usuario = txtUsuario.Text;
                string contraseña = pwdContraseña.Password;
                string rol = cmbRol.Text;
                string estado = cmbEstado.Text;

                if (banderaEditar && usuarioID != 0)
                {
                    UsuarioSistema usuarioDatos = new UsuarioSistema
                    {
                        UsuarioID = usuarioID,
                        Nombre = nombre,
                        Usuario = usuario,
                        Rol = rol,
                        Contraseña = contraseña,
                        Estado = estado
                    };

                    try
                    {
                        UsuarioSistemaDAL usuarioPro = new UsuarioSistemaDAL();
                        usuarioPro.ActualizarUsuario(usuarioDatos);
                        MessageBox.Show("Actualizado correctamente el cliente.");
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Error al actualizar cliente: " + ex.Message);
                    }

                    limpiarCampos();
                    banderaEditar = false;
                    usuarioID = 0;
                    cargarDatos();
                }
                else
                {
                    UsuarioSistema usuarioDatos = new UsuarioSistema
                    {
                        UsuarioID = usuarioID,
                        Nombre = nombre,
                        Usuario = usuario,
                        Rol = rol,
                        Contraseña = contraseña,
                        Estado = estado
                    };

                    try
                    {
                        UsuarioSistemaDAL usuarioPro = new UsuarioSistemaDAL();
                        usuarioPro.InsertarUsuario(usuarioDatos);
                        MessageBox.Show("Ingresado correctamente el usuario.");
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Error al insertar usuario: " + ex.Message);
                    }

                    limpiarCampos();
                    banderaEditar = false;
                    usuarioID = 0;
                    cargarDatos();
                }
            }
            else
            {
                MessageBox.Show("Por favor completa los campos faltantes.");
            }
        }
    

        private void btnEditarUsuario_Click(object sender, RoutedEventArgs e)
        {
            btnGuardarUsuario.IsEnabled = true;
            banderaEditar = true;
        }

        private void btnLimpiarUsuario_Click(object sender, RoutedEventArgs e)
        {
            limpiarCampos();
        }


        private void btnSalirUsuario_Click(object sender, RoutedEventArgs e)
        {
            var parent = this.Parent as Panel;
            if (parent != null)
            {
                parent.Children.Remove(this);
            }
        }

        private void ucUsuario_Loaded(object sender, RoutedEventArgs e)
        {
            cargarDatos();
            bandera = true;
        }

        private void dgUsuarios_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (bandera && dgUsuarios.SelectedItem is UsuarioSistema seleccionado)
            {
                _objUsuario = seleccionado;
                obtenerCampos();
                banderaEditar = true;
                btnGuardarUsuario.IsEnabled = false;
            }
        }

        private void txtNombre_KeyUp(object sender, KeyEventArgs e)
        {
            if (bandera && dgUsuarios.SelectedItem is UsuarioSistema seleccionado)
            {
                _objUsuario = seleccionado;
                obtenerCampos();
                banderaEditar = true;
                btnGuardarUsuario.IsEnabled = false;
            }
        }
    }
}
