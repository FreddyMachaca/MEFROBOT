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
using Microsoft.VisualBasic;

namespace MEFRobot
{
    /// <summary>
    /// Lógica de interacción para MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        List<Paquete> paquetes;
        Robot robot;
        EstacionRecarga estacion;
        Random r;
        public MainWindow()
        {
            InitializeComponent();
            r = new Random();
            
            int cantidadPaquetes = SolicitarValorEntero("Ingrese la cantidad de paquetes:", 20, 1, 100);
            int bateriaTotal = SolicitarValorEntero("Ingrese la batería total del robot:", 1000, 500, 2000);
            int umbralBateria = SolicitarValorEntero("Ingrese la batería mínima para recargar:", 350, 100, bateriaTotal/2);
            
            int x = r.Next(0, 700);
            int y = r.Next(0, 400);
            estacion = new EstacionRecarga(x, y);
            Terreno.Children.Add(estacion);
        
            x = r.Next(0, 700);
            y = r.Next(0, 400);
            robot = new Robot(x, y, bateriaTotal, umbralBateria);
            Terreno.Children.Add(robot);
            
            paquetes = new List<Paquete>();
            for (int i = 1; i <= cantidadPaquetes; i++)
            {
                x = r.Next(0, 700);
                y = r.Next(0, 400);
                Paquete p = new Paquete(x, y);
                paquetes.Add(p);
                Terreno.Children.Add(p);
            }
            
            robot.ActualizaDatos += Robot_ActualizaDatos;
            robot.IniciarRecolecion(paquetes, estacion);
        }

        private void Robot_ActualizaDatos(object sender, string e)
        {
            lblEstado.Content = e;
        }
        
        private int SolicitarValorEntero(string mensaje, int valorPredeterminado, int minimo, int maximo)
        {
            string input = Interaction.InputBox(mensaje, "Configuración", valorPredeterminado.ToString());
            int valor;
            
            if (string.IsNullOrEmpty(input) || !int.TryParse(input, out valor) || valor < minimo || valor > maximo)
            {
                MessageBox.Show($"Valor inválido. Se usará el valor predeterminado: {valorPredeterminado}", "Advertencia", MessageBoxButton.OK, MessageBoxImage.Warning);
                return valorPredeterminado;
            }
            
            return valor;
        }
    }
}
