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
    public partial class ControlTarjeta : UserControl
    {
        List<Tarjeta> _lstTarjeta;
        Tarjeta _objTarjeta;
        bool bandera = false;
        bool banderaEditar = false;
        int tarjetaID = 0;
        public ControlTarjeta()
        {
            InitializeComponent();
            Loaded += ucTarjeta_Loaded;
        }

        private void limpiarCampos()
        {
            txtNumeroTarjeta.Text = "";
            txtFechaExpiracion.Text = "";
            txtCVV.Text = "";
            cmbClaveBancaria.SelectedItem = null;
            cmbClaveBancaria.Text = "Seleccione..";
            cmbEstado.SelectedItem = null;
            cmbEstado.Text = "Seleccione..";
            txtPINHash.Text = "";
            banderaEditar = false;
            tarjetaID = 0;
        }

        private void obtenerCampos()
        {
            tarjetaID = _objTarjeta.TarjetaID;
            txtNumeroTarjeta.Text = _objTarjeta.NumeroTarjeta;
            txtFechaExpiracion.Text = _objTarjeta.FechaExpiracion.ToString("yyyy-MM-dd");
            txtCVV.Text = _objTarjeta.CVV;
            cmbClaveBancaria.SelectedValue = _objTarjeta.CuentaID;
            txtPINHash.Text = _objTarjeta.PINHash;
            foreach (ComboBoxItem item in cmbEstado.Items)
            {
                if (item.Tag != null && item.Tag.ToString() == _objTarjeta.Estado.ToString())
                {
                    cmbEstado.SelectedItem = item;
                    break;
                }
            }
        }

            void llenarTablaTarjetas(List<Tarjeta> listaTarjetas)
            {
                dgTarjetas.ItemsSource = null;
                dgTarjetas.ItemsSource = listaTarjetas;
               
            }
        

        private bool validarCampos()
        {
            if (txtPINHash.Text == "")
            {
                MessageBox.Show("El PINHash es requerido.");
                txtPINHash.Focus();
                return false;
            }

            if (txtCVV.Text == "")
            {
                MessageBox.Show("El CVV es requerido.");
                txtCVV.Focus();
                return false;
            }

            if (!(cmbClaveBancaria.SelectedIndex >= 0))
            {
                MessageBox.Show("Debes seleccionar una clave bancaria.");
                cmbClaveBancaria.Focus();
                return false;
            }

            return true;
        }

        private void cargarDatos()
        {
            TarjetaDAL tarjetaDAL = new TarjetaDAL();
            _lstTarjeta = tarjetaDAL.ObtenerTarjetas();
            llenarTablaTarjetas(_lstTarjeta);
        }
        private void cargarCuentas()
        {
            CuentaDAL cuentaDAL = new CuentaDAL();
            var listaCuentas = cuentaDAL.ObtenerCuentas();

            cmbClaveBancaria.ItemsSource = listaCuentas
                .Select(c => new
                {
                    cuentaID = c.CuentaID,
                    ClaveBancaria = c.ClaveBancaria
                })
                .ToList();

            cmbClaveBancaria.DisplayMemberPath = "ClaveBancaria";
            cmbClaveBancaria.SelectedValuePath = "cuentaID";
        }

        private void statusCampos(bool casitodo, bool habilitarFechaExp)
        {
            txtNumeroTarjeta.IsEnabled = casitodo;
            txtCVV.IsEnabled = casitodo;
            cmbClaveBancaria.IsEnabled = casitodo;

            txtFechaExpiracion.IsEnabled = habilitarFechaExp;
        }

        private string GenerarNumeroTarjeta()
        {
            Random rnd = new Random();

          
            string bloque1 = rnd.Next(1000, 9999).ToString();
            string bloque2 = rnd.Next(1000, 9999).ToString();
            string bloque3 = rnd.Next(1000, 9999).ToString();
            string bloque4 = rnd.Next(1000, 9999).ToString();

            return bloque1 + bloque2 + bloque3 + bloque4;
        }

        private bool ExisteNumeroEnLista(string numero)
        {
            return _lstTarjeta.Any(t => t.NumeroTarjeta == numero);
        }

        private string GenerarNumeroTarjetaUnico()
        {
            string numero;
            do
            {
                numero = GenerarNumeroTarjeta();
            } while (ExisteNumeroEnLista(numero));
            return numero;
        }



        private void btnGuardarTarjeta_Click(object sender, RoutedEventArgs e)
        {
            if (validarCampos())
            {
                string numeroTarjeta = txtNumeroTarjeta.Text;
                string cvv = txtCVV.Text;
                int cuentaID = (int)cmbClaveBancaria.SelectedValue;
                string PINHas = txtPINHash.Text;
                int estado = cmbEstado.SelectedIndex;
                int estadoPrueba = 1;
                if (estado == 1) 
                {
                    estadoPrueba = 0;
                }





                TarjetaDAL tarjetaDAL = new TarjetaDAL();

                if (banderaEditar && tarjetaID != 0)
                {
                    
                    DateTime fechaExpiracion = DateTime.Parse(txtFechaExpiracion.Text);

                    Tarjeta tarjetaDatos = new Tarjeta
                    {
                        TarjetaID = tarjetaID,
                        CuentaID = cuentaID,
                        NumeroTarjeta = numeroTarjeta,
                        CVV = cvv,
                        FechaExpiracion = fechaExpiracion,
                        PINHash = PINHas,
                        Estado = estadoPrueba

                    };

                    tarjetaDAL.ActualizarTarjeta(tarjetaDatos);
                    MessageBox.Show("Se actualizó la tarjeta.");
                }
                else
                {
                    DateTime fechaExpiracion = DateTime.Now.AddYears(3);
                    txtFechaExpiracion.Text = fechaExpiracion.ToString("yyyy-MM-dd");
                    numeroTarjeta = GenerarNumeroTarjetaUnico(); 
                    txtNumeroTarjeta.Text = numeroTarjeta;

                    Tarjeta tarjetaDatos = new Tarjeta
                    {
                        CuentaID = cuentaID,
                        NumeroTarjeta = numeroTarjeta,
                        CVV = cvv,
                        FechaExpiracion = fechaExpiracion,
                        PINHash = PINHas,
                        Estado = estadoPrueba
                    };

                    tarjetaDAL.InsertarTarjeta(tarjetaDatos);
                    MessageBox.Show("Se ingresó la tarjeta.");
                }

                banderaEditar = false;
                tarjetaID = 0;
                cargarDatos();

               
                statusCampos(true, false);
            }
        }

        private void btnEditarTarjeta_Click(object sender, RoutedEventArgs e)
        {
            btnGuardarTarjeta.IsEnabled = true;
            statusCampos(true, true); 
            banderaEditar = true;
        }

        private void btnLimpiarTarjeta_Click(object sender, RoutedEventArgs e)
        {
            limpiarCampos();
        }

        private void btnBuscarTarjeta_Click(object sender, RoutedEventArgs e)
        {
            string inputNumero = Interaction.InputBox("Ingresa parte o todo el número de tarjeta:", "Buscar Tarjeta", "");

            if (string.IsNullOrWhiteSpace(inputNumero))
            {
                MessageBox.Show("No se ingresó ningún número de tarjeta.", "Advertencia", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var tarjetasFiltradas = _lstTarjeta
                .Where(t => t.NumeroTarjeta.Contains(inputNumero))
                .ToList();

            if (tarjetasFiltradas.Count == 0)
            {
                MessageBox.Show("No se encontró ninguna tarjeta que coincida.", "Sin resultados", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            else
            {
                llenarTablaTarjetas(tarjetasFiltradas);
            }
        }

        private void btnSalirTarjeta_Click(object sender, RoutedEventArgs e)
        {
            var parent = this.Parent as Panel;
            if (parent != null)
            {
                parent.Children.Remove(this);
            }
        }

        private void ucTarjeta_Loaded(object sender, RoutedEventArgs e)
        {
            statusCampos(true, false); 
            cargarCuentas();
            cargarDatos();
            bandera = true;
        }

        private void dgTarjetas_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (bandera && dgTarjetas.SelectedItem is Tarjeta seleccionada)
            {
                _objTarjeta = seleccionada;
                obtenerCampos();
                banderaEditar = true;
                btnGuardarTarjeta.IsEnabled = false;
            }
        }

        private void txtCVV_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            if (!char.IsDigit(e.Text, 0))
            {
                e.Handled = true; 
            }
        }

        private void txtPINHash_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            if (!char.IsDigit(e.Text, 0))
            {
                e.Handled = true; 
            }
        }
    }
}
