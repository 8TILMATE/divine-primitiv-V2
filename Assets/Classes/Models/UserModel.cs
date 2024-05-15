using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UserModel 
{
    // Start is called before the first frame update
    public int Id { get; set; }
    public string Nume { get; set; }
    public string Email { get; set; }
    public string Parola { get; set; }
   
    public void User(int id,string nume,string email,string parola)
    {
        Id = id;
        Nume = nume;
        Email = email;
        Parola = parola;
    }
}
