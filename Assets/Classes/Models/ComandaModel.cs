using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ComandaModel : MonoBehaviour
{
    public int Id { get; set; }
    public int IdUser { get; set; }
    public int IdRestaurant { get; set; }
    public int IdLivrator { get; set; }
    public float AdresaLat { get; set; }
    public float AdresaLon { get; set; }
    public float LivratorLat { get; set; }
    public float LivratorLon { get; set; }
    public int StatusComanda { get; set; }
    public List<MenuModel> Meniu { get; set; }

}
