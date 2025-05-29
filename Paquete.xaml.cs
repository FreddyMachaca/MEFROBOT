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

namespace MEFRobot
{
    /// <summary>
    /// Lógica de interacción para Paquete.xaml
    /// </summary>
    public partial class Paquete : UserControl
    {
        public int X { get; private set; }
        public int Y { get; private set; }
        public Paquete(int x, int y)
        {
            InitializeComponent();
            X = x;
            Y = y;
            TranslateTransform translate = new TranslateTransform(x, y);
            this.RenderTransform = translate;
            this.Height = 20;
            this.Width = 20;
        }
        public void Recolectado()
        {
            this.Visibility = Visibility.Hidden;
        }
    }
}
