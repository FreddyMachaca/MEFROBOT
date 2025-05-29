using System.Windows.Controls;
using System.Windows.Media;

namespace MEFRobot
{
    /// <summary>
    /// Lógica de interacción para EstacionRecarga.xaml
    /// </summary>
    public partial class EstacionRecarga : UserControl
    {
        public int X {get; private set; }
        public int Y { get; private set; }
        public EstacionRecarga(int x, int y)
        {
            InitializeComponent();
            X = x;
            Y = y;
            TranslateTransform translate = new TranslateTransform(x,y);
            this.RenderTransform = translate;
            this.Height = 30;
            this.Width = 30;
        }
    }
}
