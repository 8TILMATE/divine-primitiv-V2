using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using OpenAI;
using System.Globalization;
using System.Threading;
using Firebase.Database;
using System.IO;
using System.Threading.Tasks;
using UnityEditor;

public class PlaseazaComanda : MonoBehaviour
{

    private OpenAIApi openAi = new OpenAIApi("sk-proj-GqexhIq0ST9udsYjK2vwT3BlbkFJYnYqaHsHvOFqATcbhZOX", "org-gcb4HKEJ9NBpp0SAu3CbMWc9");
    private List<ChatMessage> messages = new List<ChatMessage>();
    public string raspuns;
    private LivratorModel livrator;
    public static ComandaModel model = new ComandaModel();
    DatabaseReference reference;
    public async void AskGPT(string newText)
    {
       // using(StreamReader rdr = new StreamReader("Assets/Classes/Restaurante/Credidentials.txt"))
        //{
          //  var line = rdr.ReadLine().Split(' ');
            //openAi = new OpenAIApi(line[0].Trim(), line[1].Trim());
        //}
        ChatMessage chatMessage = new ChatMessage();
        chatMessage.Content = newText;
        chatMessage.Role = "user";
        messages.Add(chatMessage);
        CreateChatCompletionRequest request = new CreateChatCompletionRequest();
        request.Messages = messages;
        request.Model = "gpt-3.5-turbo";
        var response = await openAi.CreateChatCompletion(request);

        if (response.Choices!=null && response.Choices.Count > 0)
        {
            var chatResponse = response.Choices[0].Message;
            messages.Add(chatResponse);
            raspuns=chatResponse.Content;
            Debug.Log(raspuns);
        }





    }
    // Start is called before the first frame update
    void Start()
    {
        // StartChatGPTRequest("transform this into lat and lon coordinates 'Strada rizer nr55A, Galati, Romania''. Give me the results in this format: lat:;lon: without saying anything else");
        //AskGPT("transform this into lat and lon coordinates 'Strada rizer nr55A, Galati, Romania'. Give me the results in this format: ',' without saying anything else");
    
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void PlaceOrder()
    {
        StartCoroutine(PlaseazaComenzi());
    }
    public IEnumerator PlaseazaComenzi()
    {
        AskGPT("transform this into lat and lon coordinates 'Strada rizer nr55A, Galati, Romania'. Give me the results in this format: ',' without saying anything else");
        yield return new WaitForSeconds(3);
        var line = raspuns.Split(",");

        model = new ComandaModel
        {
            AdresaLat = float.Parse(line[0].Trim()),
            AdresaLon = float.Parse(line[1].Trim().Replace(" ","")),
            StatusComanda = 0,
            IdRestaurant = DatabaseHelper.SelectedShopId,
            Meniu = DatabaseHelper.CosDeCumparaturi,
            IdUser = DatabaseHelper.ShopperId,
            Id = DatabaseHelper.comenzi.Count + 1,
        };
        Firebase.AppOptions options = new Firebase.AppOptions
        {
            DatabaseUrl = new System.Uri(DatabaseHelper.connectionString)
        };
        var app = Firebase.FirebaseApp.Create(options);
        var firebaseDatabase = FirebaseDatabase.GetInstance(app, DatabaseHelper.connectionString);
        reference = firebaseDatabase.RootReference;
        var UserLog = reference.Child("Livratori").GetValueAsync();
        yield return new WaitUntil(predicate: () => UserLog.IsCompleted);
        DatabaseHelper.Livratori.Clear();
        if (UserLog != null)
        {
            foreach (var y in UserLog.Result.Children)
            {
                LivratorModel user = new LivratorModel();
                yield return new WaitForSeconds(0.1f);
                user.Id = int.Parse(y.Key.ToString());
                user.Email = y.Child("Email").GetValue(true).ToString();
                user.Nume = y.Child("Nume").GetValue(true).ToString();
               user.Telefon = y.Child("Telefon").GetValue(true).ToString();
                user.Status = int.Parse(y.Child("Status").GetValue(true).ToString());
                
                DatabaseHelper.Livratori.Add(user);
                //Debug.Log(user.Email);
                
            }
        }
        Task.Delay(100);
        //StartCoroutine(calculus());
        bool soferGasit = false;
        foreach (var sofer in DatabaseHelper.Livratori)
        {
            if (sofer.Status == 0)
            {
                model.IdLivrator = sofer.Id;
                model.LivratorLat = 28.02f;
                model.LivratorLon = 45.45f;
                DatabaseHelper.livratorComanda = sofer;
                DatabaseHelper.comanda = model;
                soferGasit = true;
            }
        }
        if (!soferGasit)
        {
            EditorUtility.DisplayDialog("Eroare", "Rideri Indisponibili", "Ok");
        }
        else
        {
            int LastId = 0;
            UserLog = reference.Child("Comenzi").GetValueAsync();
            yield return new WaitUntil(predicate: () => UserLog.IsCompleted);
            if (UserLog != null)
            {
                foreach (var y in UserLog.Result.Children)
                {

                    int Id = int.Parse(y.Key.ToString());
                        //Email = y.Child("Email").GetValue(true).ToString(),
                       // Nume = y.Child("Nume").GetValue(true).ToString(),
                        //Parola = y.Child("Parola").GetValue(true).ToString(),
                    
                    if (Id > LastId)
                    {
                        LastId = Id;
                    }
                }
            }
            model.Id = LastId + 1;
            reference.Child("Comenzi").Child(model.Id.ToString()).Child("AdresaLat").SetValueAsync(model.AdresaLat);
            reference.Child("Comenzi").Child(model.Id.ToString()).Child("AdresaLon").SetValueAsync(model.AdresaLon);
            reference.Child("Comenzi").Child(model.Id.ToString()).Child("IdLivrator").SetValueAsync(model.IdLivrator);
            reference.Child("Comenzi").Child(model.Id.ToString()).Child("IdUser").SetValueAsync(model.IdUser);
            reference.Child("Comenzi").Child(model.Id.ToString()).Child("IdRestaurant").SetValueAsync(model.IdRestaurant);
            reference.Child("Comenzi").Child(model.Id.ToString()).Child("LivratorLat").SetValueAsync(model.LivratorLat);
            reference.Child("Comenzi").Child(model.Id.ToString()).Child("LivratorLon").SetValueAsync(model.LivratorLon);
            reference.Child("Comenzi").Child(model.Id.ToString()).Child("IdLivrator").SetValueAsync(model.IdLivrator);
            foreach (MenuModel menu in model.Meniu)
            {
                reference.Child("Comenzi").Child(model.Id.ToString()).Child("Meniu").Child(menu.Nume).SetValueAsync(menu.Pret);
            }
            reference.Child("Comenzi").Child(model.Id.ToString()).Child("StatusComanda").SetValueAsync(model.StatusComanda);
            EditorUtility.DisplayDialog("Comanda Plasata cu Succes", "Comanda plasata cu Succes", "Ok");
        }


    }
    public IEnumerator AddComandaToModel(ComandaModel model)
    {
        int LastId = 0;
        var UserLog = reference.Child("Comenzi").GetValueAsync();
        yield return new WaitUntil(predicate: () => UserLog.IsCompleted);
        if (UserLog != null)
        {
            foreach (var y in UserLog.Result.Children)
            {
                UserModel user = new UserModel
                {
                    Id = int.Parse(y.Key.ToString()),
                    //Email = y.Child("Email").GetValue(true).ToString(),
                    //Nume = y.Child("Nume").GetValue(true).ToString(),
                    //Parola = y.Child("Parola").GetValue(true).ToString(),
                };
                if (user.Id > LastId)
                {
                    LastId = user.Id;
                }
            }
        }
        model.Id = LastId + 1;
        reference.Child("Comenzi").Child(model.Id.ToString()).Child("AdresaLat").SetValueAsync(model.AdresaLat);
        reference.Child("Comenzi").Child(model.Id.ToString()).Child("AdresaLon").SetValueAsync(model.AdresaLon);
        reference.Child("Comenzi").Child(model.Id.ToString()).Child("IdLivrator").SetValueAsync(model.IdLivrator);
        reference.Child("Comenzi").Child(model.Id.ToString()).Child("IdUser").SetValueAsync(model.IdUser);
        reference.Child("Comenzi").Child(model.Id.ToString()).Child("IdRestaurant").SetValueAsync(model.IdRestaurant);
        reference.Child("Comenzi").Child(model.Id.ToString()).Child("LivratorLat").SetValueAsync(model.LivratorLat);
        reference.Child("Comenzi").Child(model.Id.ToString()).Child("LivratorLon").SetValueAsync(model.LivratorLon);
        reference.Child("Comenzi").Child(model.Id.ToString()).Child("IdLivrator").SetValueAsync(model.IdLivrator);
        foreach(MenuModel menu in model.Meniu)
        {
            reference.Child("Livratori").Child(model.Id.ToString()).Child("Meniu").Child(menu.Nume).SetValueAsync(menu.Pret);
        }
        reference.Child("Livratori").Child(model.Id.ToString()).Child("StatusComanda").SetValueAsync(model.StatusComanda);
        EditorUtility.DisplayDialog("Comanda Plasata cu Succes", "Comanda plasata cu Succes", "Ok");
    }

    public IEnumerator calculus()
    {

        bool soferDisponibil = false;
        while (!soferDisponibil)
        {
            foreach(var sofer in DatabaseHelper.Livratori)
            {
                if (sofer.Status == 0)
                {
                    model.IdLivrator = sofer.Id;
                    model.LivratorLat = 28.02f;
                    model.LivratorLon = 45.45f;
                    soferDisponibil = true;
                }
            }
            DatabaseHelper.Livratori.Clear();
            yield return new WaitForSeconds(1);
            //StartCoroutine(getLivratori());
            var UserLog = reference.Child("Livratori").GetValueAsync();
            yield return new WaitUntil(predicate: () => UserLog.IsCompleted);
            if (UserLog != null)
            {
                foreach (var y in UserLog.Result.Children)
                {
                    LivratorModel user = new LivratorModel
                    {
                        Id = int.Parse(y.Key.ToString()),
                        Email = y.Child("Email").GetValue(true).ToString(),
                        Nume = y.Child("Nume").GetValue(true).ToString(),
                        Telefon = y.Child("Telefon").GetValue(true).ToString(),
                        Status = int.Parse(y.Child("Status").GetValue(true).ToString())
                    };
                    DatabaseHelper.Livratori.Add(user);
                    Debug.Log(user.Email);
                }
            }
            //yield return new WaitForSeconds(2);
            //ar task = Task.Run(getLivratori);
            //yield return new WaitUntil(() => task.IsCompleted);

        }
        Debug.Log("Done");
    }
    public IEnumerator getLivratori()
    {
        var UserLog = reference.Child("Livratori").GetValueAsync();
        yield return new WaitUntil(predicate: () => UserLog.IsCompleted);
        if (UserLog != null)
        {
            foreach (var y in UserLog.Result.Children)
            {
                LivratorModel user = new LivratorModel
                {
                    Id = int.Parse(y.Key.ToString()),
                    Email = y.Child("Email").GetValue(true).ToString(),
                    Nume = y.Child("Nume").GetValue(true).ToString(),
                    Telefon = y.Child("Telefon").GetValue(true).ToString(),
                    Status= int.Parse(y.Child("Status").GetValue(true).ToString())
                };
                DatabaseHelper.Livratori.Add(user);
                Debug.Log(user.Email);
            }
        }
    }
}
