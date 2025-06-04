
using CsvHelper;
using CVSWithLibrary;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;

namespace CVSWithLibrary;

internal class UserFileHandler
{
    // Método para escribir una lista de usuarios en el archivo CSV.
    // Sobrescribe el archivo existente con el estado actual de la lista.
    public void WriteUsers(string path, IEnumerable<User> users)
    {
        try
        {
            using (var writer = new StreamWriter(path))
            using (var csv = new CsvWriter(writer, CultureInfo.InvariantCulture))
            {
                csv.WriteRecords(users);
            }
        }
        catch (IOException ex)
        {
            Console.WriteLine($"Error al escribir en el archivo de usuarios: {ex.Message}");
            // En un caso real, esto se loggearía.
        }
    }

    // Método para leer usuarios del archivo CSV.
    // Si el archivo no existe, lo crea automáticamente con un conjunto de usuarios por defecto.
    public List<User> ReadUsers(string path)
    {
        var defaultUsers = new List<User>
        {
            new User { Username = "jzuluaga", Password = "P@ssw0rd123!", IsActive = true },
            new User { Username = "mbedoya", Password = "S0yS3gur02025*", IsActive = false } // mbedoya inicia bloqueado según tu ejemplo
        };

        if (!File.Exists(path))
        {
            Console.WriteLine($"El archivo de usuarios '{path}' no fue encontrado. Creando uno nuevo con usuarios por defecto...");
            WriteUsers(path, defaultUsers);
            return defaultUsers;
        }

        try
        {
            using (var reader = new StreamReader(path))
            using (var csv = new CsvReader(reader, CultureInfo.InvariantCulture))
            {
                return csv.GetRecords<User>().ToList();
            }
        }
        catch (CsvHelperException ex)
        {
            Console.WriteLine($"Error al leer el archivo de usuarios. El formato podría ser incorrecto: {ex.Message}");
            Console.WriteLine("Se intentará recrear el archivo con usuarios por defecto.");
            WriteUsers(path, defaultUsers); // Intenta recrear el archivo si hay un error de lectura
            return defaultUsers;
        }
        catch (IOException ex)
        {
            Console.WriteLine($"Error de E/S al leer el archivo de usuarios: {ex.Message}");
            return new List<User>(); // Retorna lista vacía si no se puede leer
        }
    }
}