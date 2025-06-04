using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CVSWithLibrary;


// User.cs
public class User
{
    public string Username { get; set; } = null!;
    public string Password { get; set; } = null!;
    public bool IsActive { get; set; }
    public int LoginAttempts { get; set; } 
}