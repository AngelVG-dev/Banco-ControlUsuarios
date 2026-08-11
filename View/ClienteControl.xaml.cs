using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics.Contracts;
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
using static Mysqlx.Crud.Order.Types;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Banco.View
{
    /// <summary>
    /// Lógica de interacción para ClienteControl.xaml
    /// </summary>
    public partial class ClienteControl : UserControl
    {
        List<Cliente> _lstCliente;
        Cliente _objCliente;
        bool bandera = false;
        bool banderaEditar = false;
        int usuarioID = 0;
        public ClienteControl()
        {
            InitializeComponent();
        }



        private void btnGuardar_Click(object sender, RoutedEventArgs e)
        {
            if (validarCampos())
            {
                string nombre = txtNombre.Text;
                string apellidos = txtApellidos.Text;
                string celular = txtCelular.Text;
                string email = txtEmail.Text;
                string direccion = txtDireccion.Text;

                if (banderaEditar && usuarioID != 0)
                {
                    Cliente clienteDatos = new Cliente
                    {
                        ClienteID = usuarioID,
                        Nombre = nombre,
                        Apellidos = apellidos,
                        Email = email,
                        Celular = celular,
                        Direccion = direccion
                    };

                    try
                    {
                        ClienteDAL clientePro = new ClienteDAL();
                        clientePro.ActualizarCliente(clienteDatos);
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
                    Cliente clienteDatos = new Cliente
                    {
                        Nombre = nombre,
                        Apellidos = apellidos,
                        Email = email,
                        Celular = celular,
                        Direccion = direccion
                    };

                    try
                    {
                        ClienteDAL clientePro = new ClienteDAL();
                        clientePro.InsertarCliente(clienteDatos);
                        MessageBox.Show("Ingresado correctamente el cliente.");
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Error al insertar cliente: " + ex.Message);
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
              

        

        private void limpiarCampos()
        {
            txtNombre.Text = "";
            txtApellidos.Text = "";
            txtCelular.Text = "";
            txtEmail.Text = "";
            txtDireccion.Text = "";
            banderaEditar = false;
            usuarioID = 0;
        }

        private void obtenerCampos()
        {
            usuarioID = _objCliente.ClienteID;
            txtNombre.Text = _objCliente.Nombre;
            txtApellidos.Text = _objCliente.Apellidos;
            txtCelular.Text = _objCliente.Celular;
            txtEmail.Text = _objCliente.Email;
            txtDireccion.Text = _objCliente.Direccion;
        }
        void llenarTablaCliente(List<Cliente> listaCliente)
        {
            dgClientes.ItemsSource = null; 
            dgClientes.ItemsSource = listaCliente;
        }

        private bool validarCampos() 
        {
            if (txtNombre.Text.Equals(""))
            {
                MessageBox.Show("El nombre del cliente es requerido.");
                txtNombre.Focus();
                return false;
            }
            if (txtApellidos.Text.Equals(""))
            {
                MessageBox.Show("Los apellidos o el apellido del cliente es requerido.");
                txtApellidos.Focus();
                return false;
            }
            if (txtCelular.Text.Equals(""))
            {
                MessageBox.Show("El numero del celular del cliente es requerido.");
                txtCelular.Focus();
                return false;
            }
            if (txtEmail.Text.Equals(""))
            {
                MessageBox.Show("El correo electronico del cliente es requerido");
                txtEmail.Focus();
                return false;
            }
            if (txtDireccion.Text.Equals(""))
            {
                MessageBox.Show("El correo direccion del cliente es requerido");
                txtDireccion.Focus();
                return false;
            }
            return true;
        }

        


        private void cargarDatos()
        {
            ClienteDAL clientePro = new ClienteDAL();
            _lstCliente = clientePro.ObtenerClientes();
            llenarTablaCliente(_lstCliente);

        }

        private void btnEditar_Click(object sender, RoutedEventArgs e)
        {
            btnGuardar.IsEnabled = true;
            banderaEditar = true;

        }

        private void btnLimpiar_Click(object sender, RoutedEventArgs e)
        {
            limpiarCampos();
        }

        private void btnSalir_Click(object sender, RoutedEventArgs e)
        {
            var parent = this.Parent as Panel;
            if (parent != null)
            {
                parent.Children.Remove(this);
            }
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            cargarDatos();
            bandera=true;
        }

        private void dgClientes_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (bandera && dgClientes.SelectedItem is Cliente seleccionado)
            {
                _objCliente = seleccionado;
                obtenerCampos(); 
                banderaEditar = true;
                btnGuardar.IsEnabled = false; 
            }
        }

        private void txtNombre_KeyUp(object sender, KeyEventArgs e)
        {
            bandera = false;
            ClienteDAL clientePro = new ClienteDAL();
            _lstCliente = clientePro.buscarClientes(txtNombre.Text);
            llenarTablaCliente(_lstCliente);
            bandera = true;
            
        }

        private void txtCelular_PreviewTextInput_1(object sender, TextCompositionEventArgs e)
        {
                if (!char.IsDigit(e.Text, 0))
                {
                    e.Handled = true; 
                }
        }
    }
}
