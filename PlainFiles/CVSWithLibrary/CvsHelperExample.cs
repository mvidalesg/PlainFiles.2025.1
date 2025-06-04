
using CsvHelper; 
using CsvHelper.Configuration;
using System.Collections.Generic;
using System.Globalization;
using System.IO; 
using System.Linq; 
using System; 

namespace CVSWithLibrary; 

internal class CvsHelperExample 
{
    
    private const string _peopleFilePath = "people.csv";

    // Método para escribir una lista de objetos Person en el archivo CSV.
    
    public void write(IEnumerable<Person> people)
    {
        try
        {
           
            var config = new CsvConfiguration(CultureInfo.InvariantCulture)
            {
                HasHeaderRecord = false, 
            };

            using (var writer = new StreamWriter(_peopleFilePath)) 
            using (var csv = new CsvWriter(writer, config))
            {
                csv.WriteRecords(people);
            }
        }
        catch (IOException ex)
        {
            Console.WriteLine($"Error al escribir en '{_peopleFilePath}': {ex.Message}");
            
        }
        catch (CsvHelperException ex)
        {
            Console.WriteLine($"Error de CsvHelper al escribir en '{_peopleFilePath}': {ex.Message}");
            Console.WriteLine("Detalles: " + ex.InnerException?.Message);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Ocurrió un error inesperado al escribir en '{_peopleFilePath}': {ex.Message}");
        }
    }

    
    
    public List<Person> read() // Ahora retorna List<Person> para facilidad de uso
    {
        // Si el archivo no existe, lo crea automáticamente (vacío)
        if (!File.Exists(_peopleFilePath))
        {
            Console.WriteLine($"El archivo de datos de personas '{_peopleFilePath}' no fue encontrado. Creando uno nuevo (vacío)...");
            File.Create(_peopleFilePath).Dispose(); // Crea el archivo y lo cierra inmediatamente
            return new List<Person>(); // Retorna una lista vacía
        }

        try
        {
            var config = new CsvConfiguration(CultureInfo.InvariantCulture)
            {
                HasHeaderRecord = false, // Ajusta si tu CSV de personas tiene encabezado
            };

            using (var reader = new StreamReader(_peopleFilePath)) // Usa la ruta interna
            using (var csv = new CsvReader(reader, config))
            {
                return csv.GetRecords<Person>().ToList();
            }
        }
        catch (CsvHelperException ex)
        {
            Console.WriteLine($"Error al leer el archivo de datos de personas. El formato podría ser incorrecto: {ex.Message}");
            Console.WriteLine("Detalles: " + ex.InnerException?.Message);
            return new List<Person>();
        }
        catch (IOException ex)
        {
            Console.WriteLine($"Error de E/S al leer el archivo de datos de personas: {ex.Message}");
            return new List<Person>();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Ocurrió un error inesperado al leer en '{_peopleFilePath}': {ex.Message}");
            return new List<Person>();
        }
    }
}
