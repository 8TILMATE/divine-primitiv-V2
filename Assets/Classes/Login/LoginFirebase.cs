using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Firebase.Database;
using Firebase.Extensions;

public class LoginFirebase : MonoBehaviour
{
    // Start is called before the first frame update
    public Button AddDataButton;
    DatabaseReference reference;

  
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
    }
    public  void LoginButton()
    {
        Firebase.AppOptions options = new Firebase.AppOptions
        {
            DatabaseUrl = new System.Uri("https://imposter-killer-955fd-default-rtdb.firebaseio.com/")
        };
        // Create a new FirebaseApp instance with options
        var app = Firebase.FirebaseApp.Create(options);
        // Get the FirebaseDatabase instance using the FirebaseApp
        var firebaseDatabase = FirebaseDatabase.GetInstance(app, "https://divine-f6fa7-default-rtdb.europe-west1.firebasedatabase.app/");
        reference =firebaseDatabase.RootReference;
        UserModel model = new UserModel();
        model.User(1, "Sandu Robert Cristian", "rafxgam@gmail.com", "Robert2301");
        Login(reference, model);
    }
    private void Login(DatabaseReference reference,UserModel model)
    {
       
        reference.Child("Users").Child("Utilizatori").Child(model.Id.ToString()).Child("Nume").SetValueAsync(model.Nume);
        reference.Child("Users").Child("Utilizatori").Child(model.Id.ToString()).Child("Parola").SetValueAsync(model.Parola);
        reference.Child("Users").Child("Utilizatori").Child(model.Id.ToString()).Child("Email").SetValueAsync(model.Email);
        reference.Child("Users").Child("Utilizatori").Child(model.Id.ToString()).Child("Id").SetValueAsync
          (model.Id);


    }
}
