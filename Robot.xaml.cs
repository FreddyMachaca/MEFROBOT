using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Threading;

namespace MEFRobot
{
    public enum EstadoEnum
    {
        Busqueda,
        NuevaBusqueda,
        IrBateria,
        Recargar,
        Muerto,
        Aleatorio
    }
    /// <summary>
    /// Lógica de interacción para Robot.xaml
    /// </summary>
    public partial class Robot : UserControl
    {
        Random r;
        List<Paquete> paquetes;
        EstacionRecarga estacion;
        public int Bateria { get; private set; }
        public int BateriaTotal { get; private set; }
        public int UmbralBateria { get; private set; }
        public int X { get; private set; }
        public int Y { get; private set; }
        public EstadoEnum Estado { get; set; }
        readonly DispatcherTimer timer;
        public event EventHandler<String> ActualizaDatos;
        
        public Robot(int x, int y, int bateriaTotal = 1000, int umbralBateria = 350)
        {
            InitializeComponent();
            r = new Random();
            BateriaTotal = bateriaTotal;
            UmbralBateria = umbralBateria;
            RecargarBateria();
            X = x;
            Y = y;
            this.Height = 50;
            this.Width = 40;
            TranslateTransform translate = new TranslateTransform(x, y);
            this.RenderTransform = translate;
            indicador.Fill = new SolidColorBrush(Bateria < UmbralBateria ? Colors.Red : Colors.Green);
            Estado = EstadoEnum.Busqueda;
            timer = new DispatcherTimer();
            timer.Interval = new TimeSpan(0,0,0,0,5);
            timer.Tick += Timer_Tick;
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            switch(Estado)
            {
                case EstadoEnum.Busqueda:
                    int newX = X, newY = Y;
                    Paquete paqueteMasCercano = EncontrarPaqueteMasCercano();
                    
                    if (paqueteMasCercano != null)
                    {
                        if (X > paqueteMasCercano.X)
                            newX = X - 1;
                        if (X < paqueteMasCercano.X)
                            newX = X + 1;
                        if (Y > paqueteMasCercano.Y)
                            newY = Y - 1;
                        if (Y < paqueteMasCercano.Y)
                            newY = Y + 1;
                        
                        ActualizaPosicion(newX, newY);
                        
                        if (X == paqueteMasCercano.X && Y == paqueteMasCercano.Y)
                        {
                            paqueteMasCercano.Recolectado();
                            Estado = EstadoEnum.NuevaBusqueda;
                        }
                    }
                    else
                    {
                        Estado = EstadoEnum.Aleatorio;
                    }
                    
                    if (Bateria < UmbralBateria)
                    {
                        Estado = EstadoEnum.IrBateria;
                    }
                    break;
                case EstadoEnum.NuevaBusqueda:
                    if (HayPaquetesPendientes())
                    {
                        Estado = EstadoEnum.Busqueda;
                    }
                    else
                    {
                        Estado = EstadoEnum.Aleatorio;
                    }
                    break;
                case EstadoEnum.IrBateria:
                    newX = X;
                    newY = Y;
                    if (X > estacion.X)
                        newX = X - 1;
                    if (X < estacion.X)
                        newX = X + 1;
                    if (Y > estacion.Y)
                        newY = Y - 1;
                    if (Y < estacion.Y)
                        newY = Y + 1;
                    ActualizaPosicion(newX, newY);
                    if (X == estacion.X && Y == estacion.Y)
                    {
                        Estado = EstadoEnum.Recargar;
                    }
                    if (Bateria == 0)
                    {
                        Estado = EstadoEnum.Muerto;
                    }
                    break;
                case EstadoEnum.Recargar:
                    RecargarBateria();
                    Thread.Sleep(500);
                    Estado = EstadoEnum.Busqueda;
                    break;
                case EstadoEnum.Muerto:
                    timer.Stop();
                    MessageBox.Show("Ha muerto...");
                    break;
                case EstadoEnum.Aleatorio:
                    ActualizaPosicion(r.NextDouble() > 0.5? X-1:X+1, r.NextDouble() > 0.5? Y-1:Y+1);
                    if (Bateria == 0)
                    {
                        Estado = EstadoEnum.Muerto;
                    }
                    break;
                default:
                    break;
            }
            ActualizaDatos(null, "Estado: " + Estado.ToString() + "- Bateria: " + Bateria + "/" + BateriaTotal);
        }

        private Paquete EncontrarPaqueteMasCercano()
        {
            Paquete paqueteMasCercano = null;
            double distanciaMinima = double.MaxValue;
            
            foreach (Paquete p in paquetes)
            {
                if (p.Visibility == Visibility.Hidden)
                    continue;
                
                double distancia = CalcularDistancia(X, Y, p.X, p.Y);
                if (distancia < distanciaMinima)
                {
                    distanciaMinima = distancia;
                    paqueteMasCercano = p;
                }
            }
            
            return paqueteMasCercano;
        }
        
        private bool HayPaquetesPendientes()
        {
            return paquetes.Any(p => p.Visibility == Visibility.Visible);
        }
        
        private double CalcularDistancia(int x1, int y1, int x2, int y2)
        {
            return Math.Sqrt(Math.Pow(x2 - x1, 2) + Math.Pow(y2 - y1, 2));
        }

        private void ActualizaPosicion(int x, int y)
        {
            Bateria--;
            X = x;
            Y = y;
            TranslateTransform translate = new TranslateTransform(x, y);
            this.RenderTransform = translate;
            indicador.Fill = new SolidColorBrush(Bateria < UmbralBateria ? Colors.Red : Colors.Green);
        }

        private void RecargarBateria()
        {
            Bateria = BateriaTotal;
        }

        public void IniciarRecolecion(List<Paquete> paquetes, EstacionRecarga estacionRecarga)
        {
            this.paquetes = paquetes;
            this.estacion = estacionRecarga;
            timer.Start();
        }
    }
}
