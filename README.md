# MEFROBOT

MEFROBOT es una simulación en WPF de un robot autónomo que recolecta paquetes en un entorno gráfico, gestionando su energía y recargándose cuando es necesario. El robot utiliza el algoritmo A* para una navegación eficiente.

## Características

- Simulación visual de un robot que recolecta paquetes distribuidos aleatoriamente.
- Navegación inteligente del robot utilizando el algoritmo A* para planificar rutas óptimas hacia los paquetes y la estación de recarga.
- Gestión automática de la batería: el robot decide cuándo debe ir a recargar para evitar quedarse sin energía, considerando la distancia a la estación.
- Parámetros configurables por el usuario al inicio:
  - Cantidad de paquetes a recolectar.
  - Energía total de la batería del robot.
  - Umbral mínimo de batería para iniciar la recarga.

## Uso

1.  Al iniciar la aplicación, se solicitarán los parámetros de simulación.
2.  El robot, los paquetes y la estación de recarga aparecerán en posiciones aleatorias dentro del área de trabajo.
3.  El robot comenzará a recolectar paquetes automáticamente, utilizando A* para encontrar el camino más corto.
4.  Cuando la batería desciende por debajo del umbral configurado, o si no puede alcanzar un paquete y luego la estación, el robot usará A* para dirigirse a la estación de recarga.
5.  El estado y la batería del robot se muestran en la interfaz.

## Dependencias

- .NET Framework 4.7.2
- WPF

## Algoritmo de Búsqueda A*

El algoritmo A* (A estrella) es un algoritmo de búsqueda de caminos que encuentra la ruta de menor costo entre un nodo inicial y un nodo objetivo en un grafo ponderado. Es ampliamente utilizado en inteligencia artificial, robótica y videojuegos debido a su eficiencia y optimalidad (si la heurística utilizada es admisible).

### Conceptos Clave:

-   **Nodos**: Representan puntos o ubicaciones en el mapa o espacio de búsqueda. Cada nodo almacena:
    -   **Coordenadas (X, Y)**: Su posición.
    -   **Costo G**: El costo real del camino desde el nodo inicial hasta el nodo actual.
    -   **Costo H (Heurística)**: Una estimación del costo desde el nodo actual hasta el nodo objetivo. Una heurística común y admisible para mapas de cuadrícula es la **distancia Manhattan**: `|actual.X - objetivo.X| + |actual.Y - objetivo.Y|`. Esta heurística nunca sobreestima el costo real si los movimientos solo pueden ser horizontales o verticales, o si el costo de los movimientos diagonales es igual al de los movimientos cardinales.
    -   **Costo F**: La suma de G y H (`F = G + H`). Es una estimación del costo total del camino si pasa por este nodo. A* prioriza la expansión de nodos con el menor costo F.
    -   **Nodo Padre (Parent)**: Una referencia al nodo desde el cual se llegó al nodo actual en el camino más corto encontrado hasta ahora. Se utiliza para reconstruir la ruta una vez que se alcanza el objetivo.

### Funcionamiento General:

1.  **Inicialización**:
    -   Se crean dos listas:
        -   `openList`: Contiene los nodos que han sido descubiertos pero aún no evaluados (expandidos). Inicialmente, solo contiene el nodo de inicio.
        -   `closedList`: Contiene los nodos que ya han sido evaluados. Inicialmente está vacía.
    -   Para el nodo de inicio: su costo G es 0, y su costo H se calcula usando la función heurística hacia el nodo objetivo.

2.  **Bucle Principal**: El algoritmo se ejecuta mientras la `openList` no esté vacía:
    a.  **Selección del Nodo Actual**: Se elige el nodo de la `openList` que tenga el menor costo F. Este es el nodo más prometedor para continuar la búsqueda. Se le denomina `currentNode`.
    b.  **Verificación del Objetivo**: Si `currentNode` es el nodo objetivo, se ha encontrado el camino más corto. El algoritmo termina y se reconstruye la ruta utilizando las referencias a los nodos `Parent`.
    c.  **Movimiento a `closedList`**: `currentNode` se elimina de la `openList` y se añade a la `closedList` para evitar reevaluarlo.
    d.  **Exploración de Vecinos**: Para cada nodo `neighbor` adyacente a `currentNode`:
        i.  **Ignorar si está en `closedList`**: Si `neighbor` ya está en la `closedList`, significa que ya se ha encontrado un camino óptimo hacia él, por lo que se ignora.
        ii. **Calcular Costo G Tentativo**: Se calcula el costo para llegar a `neighbor` a través de `currentNode`. Este es `currentNode.G` más el costo de moverse de `currentNode` a `neighbor` (generalmente 1 para movimientos en cuadrícula).
        iii. **Procesar `neighbor`**:
            -   **Si `neighbor` no está en `openList`**: Es un nodo nuevo. Se calcula su costo H, se establece su costo G como el tentativo, se asigna `currentNode` como su `Parent`, y se añade a la `openList`.
            -   **Si `neighbor` ya está en `openList`**: Se compara el costo G tentativo con el costo G que ya tiene `neighbor`. Si el costo G tentativo es menor, significa que se ha encontrado un camino más corto hacia `neighbor` a través de `currentNode`. En este caso, se actualiza el costo G de `neighbor` al valor tentativo y se cambia su `Parent` a `currentNode`.

3.  **Sin Camino**: Si la `openList` se vacía y no se ha alcanzado el nodo objetivo, significa que no existe un camino transitable entre el inicio y el objetivo.

### Reconstrucción del Camino:

Una vez que el nodo objetivo es seleccionado como `currentNode`, el camino se reconstruye retrocediendo desde el nodo objetivo hasta el nodo inicial, siguiendo las referencias `Parent` de cada nodo. Esta secuencia de nodos, invertida, forma el camino óptimo.
