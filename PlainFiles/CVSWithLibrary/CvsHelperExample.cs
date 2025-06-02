using CsvHelper;
using System.Globalization;
using System.Windows.Markup;

namespace CVSWithLibrary;

    internal class CvsHelperExample
{
    public void write(string path, IEnumerable<Person> people)
    {
        using var sw = new StreamWriter(path);
        using var cw = new CsvWriter(sw, CultureInfo.InvariantCulture);
        cw.WriteRecords(people);
    }
    public IEnumerable<Person> read(string path)
    {
        if (!File.Exists(path))
        {
            return Enumerable.Empty<Person>();
        }
        using var sr = new StreamReader(path);
        using var cr = new CsvReader(sr, CultureInfo.InvariantCulture);
        return cr.GetRecords<Person>().ToList();
    }
}

