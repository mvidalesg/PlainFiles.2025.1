
using System;
using System.IO;

namespace CVSWithLibrary; 

public static class Logger
{
    private static readonly string _logFilePath = "log.txt"; // Ruta del archivo de registro

    // Método para escribir un mensaje en el archivo de registro.
    // userContext: El nombre del usuario que realizó la acción o "Sistema" si es una acción interna.
    public static void Log(string message, string userContext = "Sistema")
    {
        try
        {
            // Obtiene la fecha y hora actual con formato
            string timestamp = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            // Formato del mensaje de log: [FECHA HORA] [USUARIO] MENSAJE
            string logEntry = $"[{timestamp}] [{userContext}] {message}";

            // Abre el archivo en modo "append" (añadir al final) y escribe la entrada.
            // true en StreamWriter indica que el archivo se abrirá para añadir texto al final.
            using (StreamWriter sw = new StreamWriter(_logFilePath, true))
            {
                sw.WriteLine(logEntry);
            }
        }
        catch (Exception ex)
        {
            // Si hay un error al escribir en el log, lo muestra en consola,
            // pero no detiene la aplicación principal.
            Console.WriteLine($"Error al escribir en el archivo de registro: {ex.Message}");
            // En una aplicación de producción, también podrías tener un mecanismo de log de errores para el log.
        }
    }
}