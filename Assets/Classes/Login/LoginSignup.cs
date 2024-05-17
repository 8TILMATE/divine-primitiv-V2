using Firebase.Database;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LoginSignup : MonoBehaviour
{
    DatabaseReference reference;
    public TMP_InputField Parola;
    public TMP_InputField Email;
    public TMP_InputField Nume;
    public TMP_InputField Parolax2;
    public void LoginButton()
    {
        Firebase.AppOptions options = new Firebase.AppOptions
        {
            DatabaseUrl = new System.Uri(DatabaseHelper.connectionString)
        };
        var app = Firebase.FirebaseApp.Create(options);
        var firebaseDatabase = FirebaseDatabase.GetInstance(app, DatabaseHelper.connectionString);
        reference = firebaseDatabase.RootReference;

        StartCoroutine(DatabaseHelper.Login(Email.text, Parola.text, reference));
    }
    public void SignUpButton()
    {
        Firebase.AppOptions options = new Firebase.AppOptions
        {
            DatabaseUrl = new System.Uri(DatabaseHelper.connectionString)
        };
        var app = Firebase.FirebaseApp.Create(options);
        var firebaseDatabase = FirebaseDatabase.GetInstance(app, DatabaseHelper.connectionString);
        reference = firebaseDatabase.RootReference;
        if (Parolax2.text == Parola.text)
        {
            StartCoroutine(DatabaseHelper.Signup(reference, new UserModel
            {
                Email = Email.text,
                Parola = Parola.text,
                Nume = Nume.text
            }));
        }
    }
    public void InapoiButton()
    {
        SceneManager.LoadScene("LogIn");
    }



}
