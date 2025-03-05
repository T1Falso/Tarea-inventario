using System;
using System.Collections.Generic;
using System.IO;

class Program
{
    static void Main()
    {
        Console.WriteLine("Sistema de Gestión de Inventario - Consola");
        Inventario gestionDeInventario = new Inventario();
        gestionDeInventario.InicializarArchivos();
        gestionDeInventario.MostrarMenuPrincipal();
    }
}

class Inventario
{
    private string rutaArchivoProductos = "productos.csv";
    private string rutaArchivoClientes = "clientes.csv";
    private string rutaArchivoVentas = "ventas.csv";
    private string[] categorias = { "Bebidas", "Abarrotes", "Tecnología", "Hogar", "Limpieza" };

    
    private string[,] inventarioMatrix = new string[5, 10];

    public void InicializarArchivos()
    {
        if (!File.Exists(rutaArchivoProductos))
        {
            File.WriteAllText(rutaArchivoProductos, "ID,Nombre,Categoría,Precio,Cantidad\n");
        }

        if (!File.Exists(rutaArchivoClientes))
        {
            File.WriteAllText(rutaArchivoClientes, "ID,Nombre,Direccion,Telefono\n");
        }

        if (!File.Exists(rutaArchivoVentas))
        {
            File.WriteAllText(rutaArchivoVentas, "IDCliente,NombreCliente,Productos,Cantidad,Total\n");
        }
    }

    public void MostrarMenuPrincipal()
    {
        while (true)
        {
            Console.WriteLine("1. Registrar Producto");
            Console.WriteLine("2. Registrar Cliente");
            Console.WriteLine("3. Mostrar Inventario");
            Console.WriteLine("4. Ver Lista de Clientes");
            Console.WriteLine("5. Registrar Nueva Venta");
            Console.WriteLine("6. Salir");

            Console.Write("Seleccione una opción: ");
            if (int.TryParse(Console.ReadLine(), out int opcionSeleccionada))
            {
                switch (opcionSeleccionada)
                {
                    case 1:
                        AgregarProducto();
                        break;
                    case 2:
                        AgregarCliente();
                        break;
                    case 3:
                        MostrarInventarioListado();
                        break;
                    case 4:
                        MostrarListaClientes();
                        break;
                    case 5:
                        AgregarVenta();
                        break;
                    case 6:
                        BorrarDatosAlSalir();
                        Console.WriteLine("Saliendo del programa...");
                        return;
                    default:
                        Console.WriteLine("Opción no válida");
                        break;
                }
            }
            else
            {
                Console.WriteLine("Entrada no válida. Por favor, ingrese un número.");
            }
        }
    }

    private void AgregarProducto()
    {
        Console.Write("Ingrese ID del producto: ");
        if (!int.TryParse(Console.ReadLine(), out int nuevoProductoID))
        {
            Console.WriteLine("ID no válido.");
            return;
        }

        // Verificar si el producto ya existe
        if (ProductoExistente(nuevoProductoID))
        {
            Console.WriteLine("El producto con este ID ya existe.");
            return;
        }

        Console.Write("Ingrese nombre del producto: ");
        string? nuevoProductoNombre = Console.ReadLine();

        Console.WriteLine("Seleccione la categoría del producto ingresando el número correspondiente:");
        for (int i = 0; i < categorias.Length; i++)
        {
            Console.WriteLine($"{i + 1}- {categorias[i]}");
        }

        Console.Write("Ingrese el número de la categoría: ");
        if (!int.TryParse(Console.ReadLine(), out int opcionCategoria) || opcionCategoria < 1 || opcionCategoria > categorias.Length)
        {
            Console.WriteLine("Categoría no válida.");
            return;
        }

        string nuevaCategoria = categorias[opcionCategoria - 1];

        Console.Write("Ingrese precio: ");
        if (!decimal.TryParse(Console.ReadLine(), out decimal nuevoPrecio))
        {
            Console.WriteLine("Precio no válido.");
            return;
        }

        Console.Write("Ingrese cantidad en stock: ");
        if (!int.TryParse(Console.ReadLine(), out int nuevaCantidad))
        {
            Console.WriteLine("Cantidad no válida.");
            return;
        }

        string productoGuardado = $"{nuevoProductoID},{nuevoProductoNombre},{nuevaCategoria},{nuevoPrecio},{nuevaCantidad}\n";
        File.AppendAllText(rutaArchivoProductos, productoGuardado);
        Console.WriteLine("Producto registrado con éxito.\n");
    }

    private bool ProductoExistente(int idProducto)
    {
        string[] productos = File.ReadAllLines(rutaArchivoProductos);
        foreach (var linea in productos)
        {
            if (linea.Contains("ID")) continue;  // Ignorar encabezados
            var columnas = linea.Split(',');
            if (int.TryParse(columnas[0], out int id) && id == idProducto)
            {
                return true; // El producto ya existe
            }
        }
        return false;
    }

    private void MostrarInventarioListado()
    {
        Console.WriteLine("Inventario por categorías:\n");

        string[] productos = File.ReadAllLines(rutaArchivoProductos);

        // Llenar la matriz de inventario con los productos leídos
        int[] cantidadProductosPorCategoria = new int[categorias.Length];  // Mantener el conteo de productos por categoría
        foreach (var linea in productos)
        {
            if (linea.Contains("ID")) continue;
            var columnas = linea.Split(',');
            if (columnas.Length >= 5)
            {
                string categoria = columnas[2];
                int cantidad = int.Parse(columnas[4]);

                // Buscar la categoría en la matriz y agregar el producto
                for (int i = 0; i < categorias.Length; i++)
                {
                    if (categorias[i] == categoria)
                    {
                        if (cantidadProductosPorCategoria[i] < 10)
                        {
                            inventarioMatrix[i, cantidadProductosPorCategoria[i]] = $"{columnas[1]} (Cantidad: {columnas[4]})";
                            cantidadProductosPorCategoria[i]++;
                        }
                        break;
                    }
                }
            }
        }

        // Mostrar el inventario de la matriz
        for (int i = 0; i < categorias.Length; i++)
        {
            Console.WriteLine($"{i + 1} {categorias[i]}\n");
            for (int j = 0; j < 10; j++)
            {
                string producto = inventarioMatrix[i, j] ?? "Vacio";
                Console.WriteLine($"{j + 1}- {producto}");
            }
            Console.WriteLine();
        }
    }

    private void AgregarCliente()
    {
        Console.Write("Ingrese ID del cliente: ");
        if (!int.TryParse(Console.ReadLine(), out int nuevoClienteID))
        {
            Console.WriteLine("ID no válido.");
            return;
        }

        // Verificar si el cliente ya existe
        if (ClienteExistente(nuevoClienteID))
        {
            Console.WriteLine("El cliente con este ID ya existe.");
            return;
        }

        Console.Write("Ingrese nombre del cliente: ");
        string? nuevoClienteNombre = Console.ReadLine();

        Console.Write("Ingrese dirección del cliente: ");
        string? nuevaDireccion = Console.ReadLine();

        Console.Write("Ingrese número de teléfono del cliente: ");
        string? nuevoTelefono = Console.ReadLine();

        string clienteGuardado = $"{nuevoClienteID},{nuevoClienteNombre},{nuevaDireccion},{nuevoTelefono}\n";
        File.AppendAllText(rutaArchivoClientes, clienteGuardado);
        Console.WriteLine("Cliente registrado con éxito.\n");
    }

    private bool ClienteExistente(int idCliente)
    {
        string[] clientes = File.ReadAllLines(rutaArchivoClientes);
        foreach (var linea in clientes)
        {
            if (linea.Contains("ID")) continue;  // Omite el encabezado
            var columnas = linea.Split(',');
            if (int.TryParse(columnas[0], out int id) && id == idCliente)
            {
                return true; // El cliente ya existe
            }
        }
        return false;
    }

    private void MostrarListaClientes()
    {
        Console.WriteLine("Lista de clientes:\n");

        string[] clientes = File.ReadAllLines(rutaArchivoClientes);

        foreach (var linea in clientes)
        {
            if (linea.Contains("ID")) continue;  // Omite encabezado

            var columnas = linea.Split(',');

            // Formatear la salida de cada cliente
            Console.WriteLine($"ID: {columnas[0]}");
            Console.WriteLine($"Nombre: {columnas[1]}");
            Console.WriteLine($"Dirección: {columnas[2]}");
            Console.WriteLine($"Teléfono: {columnas[3]}\n");
        }
    }

    private void BorrarDatosAlSalir()
    {
        // Elimina todos los datos de productos y clientes al salir
        File.Delete(rutaArchivoProductos);
        File.Delete(rutaArchivoClientes);
    }

    private void AgregarVenta()
    {
        Console.WriteLine("Registrar nueva venta");

        // Paso 1: Selección de cliente
        Console.Write("Ingrese el ID del cliente: ");
        if (!int.TryParse(Console.ReadLine(), out int idCliente))
        {
            Console.WriteLine("ID de cliente no válido.");
            return;
        }

        if (!ClienteExistente(idCliente))
        {
            Console.WriteLine("El cliente no existe.");
            return;
        }

        string clienteNombre = ObtenerNombreCliente(idCliente);
        Console.WriteLine($"Cliente seleccionado: {clienteNombre}");

        // Paso 2: Selección de productos
        List<(int idProducto, string nombre, decimal precio, int cantidadEnStock)> productosDisponibles = ObtenerProductosDisponibles();
        if (productosDisponibles.Count == 0)
        {
            Console.WriteLine("No hay productos disponibles para vender.");
            return;
        }

        List<(int idProducto, int cantidad)> productosComprados = new List<(int idProducto, int cantidad)>();
        decimal totalVenta = 0;

        Console.WriteLine("Seleccione los productos a comprar:");
        foreach (var producto in productosDisponibles)
        {
            Console.WriteLine($"{producto.idProducto}. {producto.nombre} - Precio: {producto.precio} - Stock: {producto.cantidadEnStock}");
        }

        while (true)
        {
            Console.Write("Ingrese el ID del producto (o 0 para finalizar): ");
            if (!int.TryParse(Console.ReadLine(), out int idProductoSeleccionado) || idProductoSeleccionado == 0)
            {
                break;
            }

            var productoSeleccionado = productosDisponibles.Find(p => p.idProducto == idProductoSeleccionado);
            if (productoSeleccionado.idProducto == 0)
            {
                Console.WriteLine("Producto no válido.");
                continue;
            }

            Console.Write($"Ingrese la cantidad para el producto {productoSeleccionado.nombre}: ");
            if (!int.TryParse(Console.ReadLine(), out int cantidad) || cantidad <= 0 || cantidad > productoSeleccionado.cantidadEnStock)
            {
                Console.WriteLine("Cantidad no válida.");
                continue;
            }

            productosComprados.Add((productoSeleccionado.idProducto, cantidad));
            totalVenta += cantidad * productoSeleccionado.precio;
        }

        // Paso 3: Mostrar el resumen de la venta
        Console.WriteLine("\nResumen de la venta:");
        Console.WriteLine($"Cliente: {clienteNombre}");
        foreach (var producto in productosComprados)
        {
            var productoInfo = productosDisponibles.Find(p => p.idProducto == producto.idProducto);
            Console.WriteLine($"Producto: {productoInfo.nombre}, Cantidad: {producto.cantidad}, Total: {producto.cantidad * productoInfo.precio}");
        }
        Console.WriteLine($"Total a pagar: {totalVenta:C}");

        // Registrar la venta en un archivo (puedes hacerlo en un archivo de ventas si lo deseas)
        string ventaRegistrada = $"{idCliente},{clienteNombre},{string.Join(";", productosComprados.Select(p => $"{p.idProducto}-{p.cantidad}"))},{totalVenta}\n";
        File.AppendAllText(rutaArchivoVentas, ventaRegistrada);
        Console.WriteLine("Venta registrada con éxito.");
    }

    private string ObtenerNombreCliente(int idCliente)
    {
        string[] clientes = File.ReadAllLines(rutaArchivoClientes);
        foreach (var linea in clientes)
        {
            if (linea.Contains("ID")) continue;  // Omite encabezado
            var columnas = linea.Split(',');
            if (int.TryParse(columnas[0], out int id) && id == idCliente)
            {
                return columnas[1]; // Retorna el nombre del cliente
            }
        }
        return string.Empty;
    }

    private List<(int idProducto, string nombre, decimal precio, int cantidadEnStock)> ObtenerProductosDisponibles()
    {
        string[] productos = File.ReadAllLines(rutaArchivoProductos);
        List<(int idProducto, string nombre, decimal precio, int cantidadEnStock)> productosDisponibles = new List<(int, string, decimal, int)>();

        foreach (var linea in productos)
        {
            if (linea.Contains("ID")) continue;  // Omite encabezado
            var columnas = linea.Split(',');
            if (columnas.Length >= 5 && int.TryParse(columnas[0], out int idProducto) &&
                decimal.TryParse(columnas[3], out decimal precio) && int.TryParse(columnas[4], out int cantidadEnStock) && cantidadEnStock > 0)
            {
                productosDisponibles.Add((idProducto, columnas[1], precio, cantidadEnStock));
            }
        }
        return productosDisponibles;
    }
}