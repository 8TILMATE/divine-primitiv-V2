using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RestaurantModel
{
    public int Id;
    public float lon { get; set; }
    public float lat { get; set; }
    public int OraDeschidere { get; set; }
    public int OraInchidere { get; set; }
    public string Nume { get; set; }
    public List<MenuModel> Meniu { get; set; }
}
