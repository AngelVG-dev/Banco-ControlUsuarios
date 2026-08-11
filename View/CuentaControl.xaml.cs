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
using Microsoft.VisualBasic;

namespace Banco.View
{
    /// <summary>
    /// Lógica de interacción para CuentaControl.xaml
    /// </summary>
    public partial class CuentaControl : UserControl
    {
        List<Cuenta> _lstCuenta;
        Cuenta _objCuenta;
        bool bandera = false;
        bool banderaEditar = false;
        int cuentaID = 0;
        public CuentaControl()
        {
            InitializeComponent();
            Loaded += ucCuenta_Loaded;
        }

        private void limpiarCampos()
        {
            txtClaveBancaria.Text = "";
            txtFechaApertura.Text = "";
            txtSaldo.Text = "";
            cmbNombreCliente.SelectedItem = null;
            cmbNombreCliente.Text = "Seleccione..";
            cmbTipoCuenta.SelectedItem = null;
            cmbTipoCuenta.Text = "Seleccione..";
            banderaEditar = false;
            cuentaID = 0;

        }

        private void obtenerCampos()
        {
            cuentaID = _objCuenta.CuentaID;
            txtClaveBancaria.Text = _objCuenta.ClaveBancaria;
            txtSaldo.Text = _objCuenta.Saldo.ToString("F2");
            txtFechaApertura.Text = _objCuenta.FechaApertura.ToString("yyyy-MM-dd HH:mm:ss");

            cmbNombreCliente.SelectedValue = _objCuenta.ClienteID;

            foreach (ComboBoxItem item in cmbTipoCuenta.Items)
            {
                if ((string)item.Content == _objCuenta.TipoCuenta)
                {
                    cmbTipoCuenta.SelectedItem = item;
                    break;
                }
            }
        }
        void llenarTablaCuentas(List<Cuenta> listaCuentas)
        {
            dgCuentas.ItemsSource = null; 
            dgCuentas.ItemsSource = listaCuentas;
        }

        private bool validarCampos()
        {

            if (txtSaldo.Text.Equals(""))
            {
                MessageBox.Show("El saldo del cliente es requerido para la cuenta.");
                txtSaldo.Focus();
                return false;
            }
            if (!(cmbNombreCliente.SelectedIndex >= 0))
            {
                MessageBox.Show("El nombre del cliente es requerido para la cuenta.");
                cmbNombreCliente.Focus();
                return false;
            }
            if (!(cmbTipoCuenta.SelectedIndex >= 0))
            {
                MessageBox.Show("El tipo de cuenta es requerido para la cuenta.");
                cmbNombreCliente.Focus();
                return false;
            }
            return true;
        }



        private void cargarDatos()
        {
            CuentaDAL cuentaPro = new CuentaDAL();
            _lstCuenta = cuentaPro.ObtenerCuentas();
            llenarTablaCuentas(_lstCuenta);

        }

        private void statusCampos(bool casitodos, bool habilitarFecha)
        {
            txtClaveBancaria.IsEnabled = casitodos;
            txtSaldo.IsEnabled = casitodos;
            cmbTipoCuenta.IsEnabled = casitodos;
            cmbNombreCliente.IsEnabled = casitodos;

            txtFechaApertura.IsEnabled = habilitarFecha;
        }
        private string GenerarClaveBancaria()
        {
            Random rnd = new Random();

            string banco = rnd.Next(100, 999).ToString();           
            string sucursal = rnd.Next(1000, 9999).ToString();      
            string cuenta = rnd.Next(0, 999999999).ToString("D10"); 
            string verificador = rnd.Next(0, 9).ToString();         

            return banco + sucursal + cuenta + verificador;         
        }
        private bool ExisteClaveEnLista(string clave)
        {
            return _lstCuenta.Any(c => c.ClaveBancaria == clave);
        }
        private string GenerarClaveBancariaUnica()
        {
            string clave;
            do
            {
                clave = GenerarClaveBancaria();
            } while (ExisteClaveEnLista(clave));
            return clave;
        }



        private void btnSalirCuenta_Click(object sender, RoutedEventArgs e)
        {
            var parent = this.Parent as Panel;
            if (parent != null)
            {
                parent.Children.Remove(this);
            }
        }

        private void btnGuardarCuenta_Click(object sender, RoutedEventArgs e)
        {
         
            if (validarCampos())
            {
                string claveBancaria = txtClaveBancaria.Text;
                decimal saldo = decimal.Parse(txtSaldo.Text);
                string tipoCuenta = cmbTipoCuenta.Text;
                int clienteID = (int)cmbNombreCliente.SelectedValue;

                CuentaDAL cuentaPro = new CuentaDAL();

                if (banderaEditar && cuentaID != 0)
                {
                    
                    DateTime fechaApertura = DateTime.Parse(txtFechaApertura.Text);
                    

                    Cuenta cuentaDatos = new Cuenta
                    {
                        CuentaID = cuentaID,
                        ClienteID = clienteID,
                        ClaveBancaria = claveBancaria,
                        TipoCuenta = tipoCuenta,
                        Saldo = saldo,
                        FechaApertura = fechaApertura
                    };

                    cuentaPro.ActualizarCuenta(cuentaDatos);
                    MessageBox.Show("Se actualizó la cuenta");
                }
                else
                {
                    DateTime fechaApertura = DateTime.Now;
                    txtFechaApertura.Text = fechaApertura.ToString("yyyy-MM-dd HH:mm:ss");
                    string claveBanca = GenerarClaveBancariaUnica();
                    txtClaveBancaria.Text = claveBanca;


                    Cuenta cuentaDatos = new Cuenta
                    {
                        ClienteID = clienteID,
                        ClaveBancaria = claveBanca,
                        TipoCuenta = tipoCuenta,
                        Saldo = saldo,
                        FechaApertura = fechaApertura
                    };

                    cuentaPro.InsertarCuenta(cuentaDatos);
                    MessageBox.Show("Se ingresó la cuenta");
                }

                limpiarCampos();
                banderaEditar = false;
                cuentaID = 0;
                cargarDatos();
                statusCampos(true, false);
            }
        }

        private void cargarClientes()
        {
            ClienteDAL clienteDAL = new ClienteDAL();
            var listaClientes = clienteDAL.ObtenerClientes();

            cmbNombreCliente.ItemsSource = listaClientes
                .Select(c => new
                {
                    ClienteID = c.ClienteID,
                    NombreCompleto = $"{c.Nombre} {c.Apellidos}"
                })
                .ToList();

            cmbNombreCliente.DisplayMemberPath = "NombreCompleto";
            cmbNombreCliente.SelectedValuePath = "ClienteID";
        }
        private void ucCuenta_Loaded(object sender, RoutedEventArgs e)
        {
            statusCampos(true, false);
            cargarClientes();
            cargarDatos();
            bandera = true;
        }

        private void dgCuentas_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (bandera && dgCuentas.SelectedItem is Cuenta seleccionado)
            {
                _objCuenta = seleccionado;
                obtenerCampos(); 
                banderaEditar = true;
                btnGuardarCuenta.IsEnabled = false; 
            }

        }

        private void btnEditarCuenta_Click(object sender, RoutedEventArgs e)
        {
            btnGuardarCuenta.IsEnabled = true;
            statusCampos(true,true);
            banderaEditar = true;
        }

        private void btnLimpiarCuenta_Click(object sender, RoutedEventArgs e)
        {
            limpiarCampos();
        }

        private void btnBuscarCuenta_Click(object sender, RoutedEventArgs e)
        {
            string inputClave = Interaction.InputBox("Ingresa parte o toda la clave bancaria:", "Buscar Clave Bancaria", "");

            if (string.IsNullOrWhiteSpace(inputClave))
            {
                MessageBox.Show("No se ingresó ninguna clave bancaria.", "Advertencia", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var cuentasFiltradas = _lstCuenta
                .Where(c => c.ClaveBancaria.Contains(inputClave))
                .ToList();

            if (cuentasFiltradas.Count == 0)
            {
                MessageBox.Show("No se encontró ninguna cuenta que coincida.", "Sin resultados", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            else
            {
                llenarTablaCuentas(cuentasFiltradas);
            }
        }

        private void txtSaldo_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {

            if (char.IsDigit(e.Text, 0))
            {
                e.Handled = false;
            }
            else if (e.Text == "." &&
                     !((sender as TextBox).Text.Contains(".")) &&
                     (sender as TextBox).Text.Length > 0)
            {
                e.Handled = false;
            }

            else
            {
                e.Handled = true;
            }
        }
    }
}
