using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AdaugaInCos : MonoBehaviour
{
    public Text numeprodus;
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }
    public void AddToCart()
    {
        foreach (var x in DatabaseHelper.restaurante[DatabaseHelper.SelectedShopId-1].Meniu)
        {
            if (x.Nume == numeprodus.text)
            {
                DatabaseHelper.CosDeCumparaturi.Add(x);
                DatabaseHelper.SubtotalComanda += x.Pret;
                break;
            }
        }
        

    }
}
