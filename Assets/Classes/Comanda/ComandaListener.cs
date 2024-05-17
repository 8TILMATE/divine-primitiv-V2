using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Firebase.Database;
using UnityEngine.UI;

public class ComandaListener : MonoBehaviour
{
    DatabaseReference reference;
    public Text Status;
    public Text ETA;
    public Text Livrator;

    // Start is called before the first frame update
    void Start()
    {
        Livrator.text = DatabaseHelper.livratorComanda.Nume;
        Firebase.AppOptions options = new Firebase.AppOptions
        {
            DatabaseUrl = new System.Uri(DatabaseHelper.connectionString)
        };
        var app = Firebase.FirebaseApp.Create(options);
        var firebaseDatabase = FirebaseDatabase.GetInstance(app, DatabaseHelper.connectionString);
        reference = firebaseDatabase.RootReference;
       FirebaseDatabase.DefaultInstance
       .GetReference("Comenzi").Child(DatabaseHelper.comanda.Id.ToString()).Child("StatusComanda")
       .ValueChanged += StatusComanda;
    }
    void StatusComanda(object sender, ValueChangedEventArgs args)
    {
        Debug.Log("Helllooooo!");
        DatabaseHelper.comanda.StatusComanda = int.Parse(args.Snapshot.Value.ToString());
        if (DatabaseHelper.comanda.StatusComanda == 0)
        {
            Status.text = "Status:   Mancarea ta este acum in pregatire...";
        }
        if(DatabaseHelper.comanda.StatusComanda == 1)
        {
            Status.text = "Status:   Comanda ta a fost preluata de un Rider diVIne";

        }
        if (DatabaseHelper.comanda.StatusComanda == 2)
        {
            Status.text = "Status:  Comanda ta a fost livrata! Pofta Buna! :)";
        }
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
