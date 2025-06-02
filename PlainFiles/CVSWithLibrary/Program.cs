using CVSWithLibrary;

var list = new List<Person>
{
    new Person { Id = 1, Name = "Maria", Age = 28 },
    new Person { Id = 2, Name = "tomas", Age = 24 },
    
};  
var helper = new CvsHelperExample();
helper.write("people.csv", list);

var readPeople = helper.read("people.csv");
foreach (var person in readPeople)
{
    Console.WriteLine($"Id: {person.Id}, Name: {person.Name}, Age: {person.Age}");
}
