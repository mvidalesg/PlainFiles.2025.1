using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BasicTextFile;

    public class SimpleTextFile
    {
       private string _path;

    public SimpleTextFile(string path)
    {
        _path = path;
    }
    public void WriteLines(string [] lines)
    {
        File.WriteAllLines(_path, lines);
    }
    public string[] ReadLines()
    { 
        if (!File.Exists(_path))
        {
            return [];
        }
        return File.ReadAllLines(_path);

    }
}
   

