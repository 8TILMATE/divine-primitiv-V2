using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public static class GoToMap
{
    public static void GoToHarta()
    {
        SceneManager.LoadScene("Location-basedGame");

    }
}
public class NavigationAid : MonoBehaviour
{
    // Start is called before the first frame update
    public GameObject canvas1;
    public GameObject canvas2;
    public GameObject canvas3;
    public GameObject canvas4;
    public GameObject MenuPrefab;
    public GameObject Container;
    private GameObject GeneratedMenuItem;
    public InputField InputAdresa;
    public GameObject ContainerIstoric;
    public GameObject IstoricPrefab;
    public Text Total;
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void BackToMain()
    {
        canvas2.SetActive(false);
        canvas1.SetActive(true);
        canvas3.SetActive(false);
        canvas4.SetActive(false);
    }
    
    public void ComenziTrecute()
    {
        canvas2.SetActive(false);
        canvas1.SetActive(false);
        canvas3.SetActive(false);
        canvas4.SetActive(true) ;
        ContainerIstoric.transform.DetachChildren();
        for (int i = 0; i < DatabaseHelper.comenzi.Count; i++)
        {
            if (DatabaseHelper.comenzi[i].IdUser == DatabaseHelper.ShopperId)
            {
                GeneratedMenuItem = Instantiate(MenuPrefab, Container.transform.position, Quaternion.identity) as GameObject;
                GeneratedMenuItem.name = i.ToString();
                GeneratedMenuItem.transform.parent = ContainerIstoric.transform;
                GeneratedMenuItem.transform.localScale = new Vector3(2, 2, 2);
                foreach (Transform t in GeneratedMenuItem.transform)
                {

                    Text text = t.GetComponent<Text>();
                    if (text)
                    {
                        text.text = "Restaurant:" + DatabaseHelper.restaurante[DatabaseHelper.comenzi[i].IdRestaurant].Nume + "," + "Status" + DatabaseHelper.comenzi[i].StatusComanda.ToString();
                        
                    }
                }
            }
        }
    }
    public void GoToCos()
    {
        canvas2.SetActive(false);
        canvas1.SetActive(false);
        canvas3.SetActive(true);
        canvas4.SetActive(false);

        DatabaseHelper.AdresaCasa = InputAdresa.text;
        Container.transform.DetachChildren();
        for (int i = 0; i <DatabaseHelper.CosDeCumparaturi.Count; i++)
        {
            GeneratedMenuItem = Instantiate(MenuPrefab, Container.transform.position, Quaternion.identity) as GameObject;
            GeneratedMenuItem.name = i.ToString() + "_" + DatabaseHelper.CosDeCumparaturi[i].Pret+ToString();
            GeneratedMenuItem.transform.parent = Container.transform;
            GeneratedMenuItem.transform.localScale = new Vector3(1, 1, 1);
            foreach (Transform t in GeneratedMenuItem.transform)
            {

                Text text = t.GetComponent<Text>();
                if (text)
                {
                    text.text = DatabaseHelper.CosDeCumparaturi[i].Nume+"          "+DatabaseHelper.CosDeCumparaturi[i].Pret.ToString();
                }
            }
        }
        Total.text = "Total: " + DatabaseHelper.SubtotalComanda.ToString() + " Lei";
    }
}
