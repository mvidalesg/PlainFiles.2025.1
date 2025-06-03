using CVSWithLibrary;

var helper = new CvsHelperExample();
var readPeople = helper.read("people.csv"). ToList();
var opc = "0";
do
{
    opc = Menu();
    Console.WriteLine("=======================================");
    switch (opc)
    {
        case "1":
            foreach (var person in readPeople)
            {
                Console.WriteLine(person);
            }
            break;
        case "2":
            Console.Write("Enter the Id: ");
            var id = Console.ReadLine();
            Console.Write("Enter the First name: ");
            var firstName = Console.ReadLine();
            Console.Write("Enter the Last name: ");
            var lastName = Console.ReadLine();
            Console.Write("Enter the phone: ");
            var phone = Console.ReadLine();
            Console.Write("Enter the city: ");
            var city = Console.ReadLine();
            Console.Write("Enter the balance: ");
            var balance = Console.ReadLine();

        var newPerson = new Person
        { 
            Id = int.Parse(id!),
            FirsName = firstName ?? string.Empty,
            LastName = lastName ?? string.Empty,
            Phone = phone ?? string.Empty,
            City = city ?? string.Empty,
            Balance = decimal.Parse(balance!)

        };
            readPeople.Add(newPerson);

            break;
        case "3":
            SaveChanges();
            break;

    }

    } while (opc != "0") ;
SaveChanges();
 void SaveChanges()
{
    helper.write("people.csv", readPeople);
}

    string Menu()
    {
      Console.WriteLine("=======================================");
      Console.WriteLine("1. Show content.");
      Console.WriteLine("2. Add person.");
      Console.WriteLine("3. save changes.");
      Console.WriteLine("0. Exit.");
      Console.Write("Choose an option: ");
      return Console.ReadLine() ?? "0";
    }
