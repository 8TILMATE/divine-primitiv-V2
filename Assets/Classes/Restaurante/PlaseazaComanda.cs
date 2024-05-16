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


public class PlaseazaComanda : MonoBehaviour
{

    private OpenAIApi openAi;//= new OpenAIApi("sk-proj-TbgHgQoQNLcH2C5limiQT3BlbkFJNed7C817K2uf9FWgNFsh", "org-gcb4HKEJ9NBpp0SAu3CbMWc9");
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
        AskGPT("transform this into lat and lon coordinates 'Strada rizer nr55A, Galati, Romania'. Give me the results in this format: ',' without saying anything else");
    
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public async void PlaseazaComenzi()
    {
        AskGPT("transform this into lat and lon coordinates 'Strada rizer nr55A, Galati, Romania'. Give me the results in this format: ',' without saying anything else");
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
        StartCoroutine(calculus());
        
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
