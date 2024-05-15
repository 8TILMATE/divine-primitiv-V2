using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Firebase.Database;

public class GetRestaurante : MonoBehaviour
{
    public static  List<MenuModel> meniuri = new List<MenuModel>();
    DatabaseReference reference;
    // Start is called before the first frame update
    void Start()
    {
        Firebase.AppOptions options = new Firebase.AppOptions
        {
            DatabaseUrl = new System.Uri(DatabaseHelper.connectionString)
        };
        var app = Firebase.FirebaseApp.Create(options);
        var firebaseDatabase = FirebaseDatabase.GetInstance(app, DatabaseHelper.connectionString);
        reference = firebaseDatabase.RootReference;
        StartCoroutine(GetRestaurante1(reference));
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public  IEnumerator GetRestaurante1(DatabaseReference reference)
    {
        DatabaseHelper.restaurante.Clear();
        var RestauranteLog = reference.Child("Restaurante").GetValueAsync();
        yield return new WaitUntil(predicate: () => RestauranteLog.IsCompleted);
        if (RestauranteLog != null)
        {
            foreach (var y in RestauranteLog.Result.Children)
            {
                Debug.Log(y.Key);
                RestaurantModel model = new RestaurantModel
                {
                    Id = int.Parse(y.Key),
                    lat = float.Parse(y.Child("lat").GetValue(true).ToString()),
                    lon = float.Parse(y.Child("lon").GetValue(true).ToString()),
                    Nume = y.Child("Nume").GetValue(true).ToString(),
                    OraDeschidere = int.Parse(y.Child("OraDeschidere").GetValue(true).ToString()),
                    OraInchidere = int.Parse(y.Child("OraInchidere").GetValue(true).ToString())
                };
                StartCoroutine(GetMeniu(model.Id.ToString(), reference));
                model.Meniu = meniuri;
                DatabaseHelper.restaurante.Add(model);
            }
        }

    }
    public  IEnumerator GetMeniu(string key, DatabaseReference reference)
    {
        meniuri.Clear();
        var MeniuLog = reference.Child("Restaurante").Child(key).Child("Meniu").GetValueAsync();
        yield return new WaitUntil(predicate: () => MeniuLog.IsCompleted);
        if (MeniuLog != null)
        {
            foreach (var y in MeniuLog.Result.Children)
            {
                meniuri.Add(new MenuModel
                {
                    Nume = y.Key,
                    Pret = int.Parse(y.GetValue(true).ToString())
                });
                Debug.Log(y.Key);
            }
           
        }

    }
}
