﻿using CVSWithoutLibraries;
using System.ComponentModel.DataAnnotations;
    
var people = new List<string[]>
{
    new[] { "Id", "Name", "Age" },
    new[] { "1", "Alice", "30" },
    new[] { "2", "Bob", "25" },
    new[] { "3", "Pedro", "45" },
    new[] { "4", "Maria", "35" },
    new[] { "5", "Edison", "18" }
};

var manualCvs = new ManualCvsHelper();
manualCvs.WriteCVS("people.csv", people);
var readPeople = manualCvs.ReadCVS("people.csv");
foreach (var person in readPeople)
{
    Console.WriteLine(string.Join(", ", person));
}