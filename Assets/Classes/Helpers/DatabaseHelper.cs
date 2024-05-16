using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Firebase.Database;
using UnityEditor;

public static class DatabaseHelper
{
    public static List<UserModel> utilizatori = new List<UserModel>();
    public static UserModel utilizatorLogat = new UserModel();
    public static List<RestaurantModel> restaurante = new List<RestaurantModel>();
    // Start is called before the first frame update
    public static int SelectedShopId = 0;
    public static int ShopperId=0;
    public static  string connectionString = "https://divine-f6fa7-default-rtdb.europe-west1.firebasedatabase.app/";
    public static List<List<MenuModel>> meniuri = new List<List<MenuModel>>();
    public static List<MenuModel> CosDeCumparaturi = new List<MenuModel>();
    public static int SubtotalComanda = 0;
    public static List<ComandaModel> comenzi = new List<ComandaModel>();
    public static List<LivratorModel> Livratori = new List<LivratorModel>();

   

    public static IEnumerator Login(string email,string parola, DatabaseReference reference)
    {
        utilizatori.Clear();
        var UserLog = reference.Child("Users").Child("Utilizatori").GetValueAsync();
        yield return new WaitUntil(predicate: () => UserLog.IsCompleted);
        if (UserLog != null)
        {
            foreach (var y in UserLog.Result.Children)
            {
                UserModel user = new UserModel
                {
                    Id = int.Parse(y.Key.ToString()),
                    Email = y.Child("Email").GetValue(true).ToString(),
                    Nume = y.Child("Nume").GetValue(true).ToString(),
                    Parola = y.Child("Parola").GetValue(true).ToString(),
                };
                utilizatori.Add(user);
                Debug.Log(user.Email);
            }
        }
        foreach ( var y in utilizatori)
        {
            Debug.Log(y.Email+y.Parola);
            if(y.Email==email && y.Parola == parola)
            {
                Debug.LogWarning("Logged!");
                EditorUtility.DisplayDialog("Logat", "Utilizator inregistrat cu succes!","Ok");
                utilizatorLogat = y;
                break;
            }
        }
    }
    
    public static IEnumerator Signup(DatabaseReference reference, UserModel model)
    {
        utilizatori.Clear();
        int LastId=0;
        var UserLog = reference.Child("Users").Child("Utilizatori").GetValueAsync();
        yield return new WaitUntil(predicate: () => UserLog.IsCompleted);
        if (UserLog != null)
        {
            foreach (var y in UserLog.Result.Children)
            {
                UserModel user = new UserModel
                {
                    Id = int.Parse(y.Key.ToString()),
                    Email = y.Child("Email").GetValue(true).ToString(),
                    Nume = y.Child("Nume").GetValue(true).ToString(),
                    Parola = y.Child("Parola").GetValue(true).ToString(),
                };
                if (user.Id > LastId)
                {
                    LastId = user.Id;
                }
                utilizatori.Add(user);
                
                Debug.Log(user.Email);
            }
        }
        model.Id = LastId + 1;
        reference.Child("Users").Child("Utilizatori").Child(model.Id.ToString()).Child("Nume").SetValueAsync(model.Nume);
        reference.Child("Users").Child("Utilizatori").Child(model.Id.ToString()).Child("Parola").SetValueAsync(model.Parola);
        reference.Child("Users").Child("Utilizatori").Child(model.Id.ToString()).Child("Email").SetValueAsync(model.Email);
        reference.Child("Users").Child("Utilizatori").Child(model.Id.ToString()).Child("Id").SetValueAsync
          (model.Id);
        EditorUtility.DisplayDialog("Inregistrat", "Utilizator inregistrat cu succes!", "Ok");



    }

}
