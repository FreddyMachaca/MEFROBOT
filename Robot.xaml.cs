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
        Aleatorio,
        SiguiendoCamino,
        Completado
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
        
        // Algoritmo A*
        private AStarPathfinder _pathfinder;
        private List<Node> _currentPath;
        private int _currentPathIndex;
        private Node _currentTarget;
        private int _paquetesRecolectados = 0;
        private Paquete _targetPaquete = null;
        
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
            
            // Inicializar el algoritmo A*
            _pathfinder = new AStarPathfinder();
            _currentPath = new List<Node>();
            _currentPathIndex = 0;
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            switch(Estado)
            {
                case EstadoEnum.Busqueda:
                    if (!HayPaquetesPendientes())
                    {
                        Estado = EstadoEnum.Completado;
                        break;
                    }
                    
                    Paquete paqueteMasCercano = EncontrarPaqueteMasCercano();
                    
                    if (paqueteMasCercano != null)
                    {
                        _targetPaquete = paqueteMasCercano;
                        // Calcular camino hacia el paquete más cercano usando A*
                        _currentPath = _pathfinder.FindPath(X, Y, paqueteMasCercano.X, paqueteMasCercano.Y);
                        _currentPathIndex = 0;
                        _currentTarget = new Node(paqueteMasCercano.X, paqueteMasCercano.Y);
                        
                        if (_currentPath.Count > 0)
                        {
                            Estado = EstadoEnum.SiguiendoCamino;
                        }
                    }
                    else
                    {
                        Estado = EstadoEnum.IrBateria;
                    }
                    
                    if (Bateria < UmbralBateria)
                    {
                        Estado = EstadoEnum.IrBateria;
                    }
                    break;
                case EstadoEnum.NuevaBusqueda:
                    _targetPaquete = null;
                    
                    if (HayPaquetesPendientes())
                    {
                        Estado = EstadoEnum.Busqueda;
                    }
                    else
                    {
                        Estado = EstadoEnum.Completado;
                    }
                    break;
                case EstadoEnum.IrBateria:
                    _targetPaquete = null;
                    
                    if (X == estacion.X && Y == estacion.Y)
                    {
                        Estado = EstadoEnum.Recargar;
                        break; 
                    }

                    // Calcular camino hacia la estación de recarga usando A*
                    _currentPath = _pathfinder.FindPath(X, Y, estacion.X, estacion.Y);
                    _currentPathIndex = 0;
                    _currentTarget = new Node(estacion.X, estacion.Y);
                    
                    if (_currentPath.Count > 0)
                    {
                        Estado = EstadoEnum.SiguiendoCamino;
                    }
                    
                    if (Bateria == 0)
                    {
                        Estado = EstadoEnum.Muerto;
                    }
                    break;
                case EstadoEnum.SiguiendoCamino:
                    // Seguir el camino calculado por A*
                    if (_currentPathIndex < _currentPath.Count)
                    {
                        var nextNode = _currentPath[_currentPathIndex];
                        ActualizaPosicion(nextNode.X, nextNode.Y);
                        _currentPathIndex++;
                        
                        if (_currentPathIndex >= _currentPath.Count)
                        {
                            if (_targetPaquete != null && _targetPaquete.Visibility == Visibility.Visible && 
                                X == _targetPaquete.X && Y == _targetPaquete.Y)
                            {
                                _targetPaquete.Recolectado();
                                _paquetesRecolectados++;
                                _targetPaquete = null;
                                Estado = EstadoEnum.NuevaBusqueda;
                            }
                            else if (X == estacion.X && Y == estacion.Y)
                            {
                                Estado = EstadoEnum.Recargar;
                            }
                            else
                            {
                                Estado = EstadoEnum.Busqueda;
                            }
                        }
                        if (_targetPaquete != null && _targetPaquete.Visibility == Visibility.Hidden)
                        {
                            _targetPaquete = null;
                            Estado = EstadoEnum.Busqueda;
                        }
                    }
                    else
                    {
                        Estado = EstadoEnum.Busqueda;
                    }
                    if (Bateria < UmbralBateria && (_currentTarget == null || 
                        (_currentTarget.X != estacion.X && _currentTarget.Y != estacion.Y)))
                    {
                        Estado = EstadoEnum.IrBateria;
                    }
                    
                    if (Bateria == 0)
                    {
                        Estado = EstadoEnum.Muerto;
                    }
                    break;
                case EstadoEnum.Recargar:
                    RecargarBateria();
                    Thread.Sleep(500);
    
                    if (HayPaquetesPendientes())
                    {
                        Estado = EstadoEnum.Busqueda;
                    }
                    else
                    {
                        Estado = EstadoEnum.Completado;
                    }
                    break;
                case EstadoEnum.Muerto:
                    timer.Stop();
                    MessageBox.Show("Ha muerto...");
                    break;
                case EstadoEnum.Completado:
                    timer.Stop();
                    MessageBox.Show($"El robot obtuvo todos los {_paquetesRecolectados} paquetes.");
                    break;
                case EstadoEnum.Aleatorio:
                    if (Bateria < BateriaTotal * 0.9)
                    {
                        Estado = EstadoEnum.IrBateria;
                    }
                    else
                    {
                        Estado = EstadoEnum.Busqueda;
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
            _paquetesRecolectados = 0;
            _targetPaquete = null;
            Estado = EstadoEnum.Busqueda;
            timer.Start();
        }
    }
}
