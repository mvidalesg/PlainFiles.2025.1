using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CVSWithLibrary;

    public class Person
    {
        public int Id { get; set; }
        public string FirstName { get; set; } = null!;
        public string LastName { get; set; } = null!;
        public string Phone { get; set; } = null!;
        public string City { get; set; } = null!;
        public decimal Balance { get; set; }

        public override string ToString()
    {
        return $"{Id}\t{FirstName}{LastName}\n\t" +
            $"phone: {Phone}\n\t" +
            $"City: {City}\n\t" +
            $"Balance: {Balance,20:C2}\n";
    }

}

