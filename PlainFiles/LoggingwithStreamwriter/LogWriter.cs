using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LoggingWithStreamWriter;


public class LogWriter : IDisposable  
{
    private readonly StreamWriter _Writer;
    
    public LogWriter(string path)
    {
      
        _Writer = new StreamWriter(path, append: true)
        {
            AutoFlush = true
        };
    }
    public void WriteLog(string level, string message)
    {
        var tiemstamp = DateTime.Now.ToString("s"); // ISO 8601 format
        _Writer.WriteLine($"{tiemstamp} [{level}]{message}");
    }
    
    public void Dispose()
    {
        _Writer?.Dispose();
    }
}
