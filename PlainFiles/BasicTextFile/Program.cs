using BasicTextFile;
using System.ComponentModel.Design;
using System.Xml.Linq;

var textFile  = new SimpleTextFile("data.txt");
var lines = textFile.ReadLines();
var opc = "0";

do
{
    opc = Menu();
    Console.WriteLine("=======================================");
    switch(opc)
    {
        case "1":
            if (lines.Length == 0)
            {
                Console.WriteLine("Empty file.");
                break;
            }
             foreach (var line in lines)
                {
                    Console.WriteLine(line);
                }
            
            break;
        case "2":
            Console.Write("Enter the line to add: ");
            var newLine = Console.ReadLine();
            if (!string.IsNullOrEmpty(newLine))
            {
                lines = lines.Append(newLine).ToArray();

            }
            break;
        case "3":
            Console.Write("Enter the line to update: ");
            var lineToUpdate = Console.ReadLine();
            Console.Write("Enter the new value: ");
            var newValue = Console.ReadLine();
            for (int i = 0; i < lines.Length; i++)
            {
                if (lines[i] == lineToUpdate)
                {
                    lines[i] = newValue!;
                   
                    break;
                }

            }
            break;
        case "4":
            Console.Write("Enter the line to remove: ");
            var lineToRemove = Console.ReadLine();
            if (!string.IsNullOrEmpty(lineToRemove))
            {
                lines = lines.Where(line => line != lineToRemove).ToArray();
            }
            break;

        case "5":
            SaveChanges();
            break;
        case "0":
            Console.WriteLine("Exiting the program...");
            break;
        default:
            Console.WriteLine("Invalid option, please try again.");
            continue;
    }

} while (opc != "0");
SaveChanges();

void SaveChanges()
{
    Console.WriteLine("Saving changes...");
    textFile.WriteLines(lines);
    Console.WriteLine("Changes saved.");
}

string Menu()
{
    Console.WriteLine("=======================================");
    Console.WriteLine("1. Show content");
    Console.WriteLine("2. Add line");
    Console.WriteLine("3. Update line");
    Console.WriteLine("4. Remove line");
    Console.WriteLine("5. save Changes");
    Console.WriteLine("0. Exit");
    Console.Write("Enter your option: ");
    return Console.ReadLine() ?? "0";




}


