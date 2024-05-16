using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class OpenMenu : MonoBehaviour
{
    public GameObject Canvas1;
    public GameObject Canvas2;
    public GameObject MenuPrefab;
    public GameObject Container;
    private GameObject GeneratedMenuItem;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void OpenRestaurantMenu()
    {
        Canvas1.active = false;
        Canvas2.active = true;
        string numeparinte = this.transform.parent.gameObject.name;
        var idnume = numeparinte.Split("_");
        foreach(var restaurant in DatabaseHelper.restaurante)
        {
            if (restaurant.Id == int.Parse(idnume[0]))
            {
                for (int i = 0; i < restaurant.Meniu.Count; i++)
                {
                    GeneratedMenuItem = Instantiate(MenuPrefab, Container.transform.position, Quaternion.identity) as GameObject;
                    GeneratedMenuItem.name = i.ToString() + "_" + restaurant.Meniu[i].Nume;
                    GeneratedMenuItem.transform.parent = Container.transform;
                    foreach (Transform t in GeneratedMenuItem.transform)
                    {

                        Text text = t.GetComponent<Text>();
                        if (text)
                        {
                            text.text = restaurant.Meniu[i].Nume;
                        }
                    }
                }
                break;
            }
        }
    }
}