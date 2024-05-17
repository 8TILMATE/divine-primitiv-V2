using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Firebase.Database;

public class CheckPEK : MonoBehaviour
{
    // Start is called before the first frame update
    public InputField input;
    private DatabaseReference reference;
    public List<string> peks = new List<string>();
    public static List<List<MenuModel>> meniuri1 = new List<List<MenuModel>>();
    private GameObject gam;
    public GameObject toSpawn;
    public GameObject taticul;
    public GameObject Canvas1;
    public GameObject Canvas2;
    void Start()
    {
  
        Firebase.AppOptions options = new Firebase.AppOptions
        {
            DatabaseUrl = new System.Uri(DatabaseHelper.connectionString)
        };
        var app = Firebase.FirebaseApp.Create(options);
        var firebaseDatabase = FirebaseDatabase.GetInstance(app, DatabaseHelper.connectionString);
        reference = firebaseDatabase.RootReference;
        StartCoroutine(GetPEK());
        StartCoroutine(LoadRestaurante());
        StartCoroutine(GetComenzi(reference));
    }
    public IEnumerator GetPEK()
    {
        peks.Clear();
        var RestauranteLog = reference.Child("PEK").GetValueAsync();
        yield return new WaitUntil(predicate: () => RestauranteLog.IsCompleted);
        if (RestauranteLog != null)
        {
            foreach (var y in RestauranteLog.Result.Children)
            {
                peks.Add(y.Value.ToString());

                //model.Meniu = meniuri;

            }
        }
    }
    public void LogIntoPEK()
    {
        for(int i=0;i<peks.Count;i++)
        {
            if (input.text == peks[i])
            {
                Debug.Log("Logged");
                DatabaseHelper.pekRestaurant = i + 1;
                Canvas1.SetActive(false);
                Canvas2.SetActive(true);
                GenerateComenzi();
                break;
            }
        }
    }
    public void ComandaTerminata()
    {
        var nume = this.gameObject.transform.parent.name;
        var line = nume.Split('_');
        reference.Child("Comenzi").Child(line[0]).Child("StatusComanda").SetValueAsync(2);
    }
    public void ComandaPReluata(){
        var nume = this.gameObject.transform.parent.name;
        var line = nume.Split('_');
        reference.Child("Comenzi").Child(line[0]).Child("StatusComanda").SetValueAsync(1);
    }
    public void GenerateComenzi()
    {
        foreach (var x in DatabaseHelper.comenzi)
        {
            if (x.IdRestaurant == DatabaseHelper.pekRestaurant)
            {
                gam = Instantiate(toSpawn, taticul.transform.position, Quaternion.identity) as GameObject;
                gam.name = x.Id.ToString() + "_" + x.IdRestaurant.ToString();
                gam.transform.parent = taticul.transform;
                gam.gameObject.transform.localScale = new Vector3(1, 1, 1);
                foreach (Transform t in gam.transform)
                {

                    Text text = t.GetComponent<Text>();
                    if (text)
                    {
                        text.text = x.Id.ToString() + "\n" + x.StatusComanda+"\n";
                        foreach(var z in x.Meniu)
                        {
                            text.text += z.Nume + " ";
                        }
                    }
                    Button button = t.GetComponent<Button>();
                    if (button != null)
                    {
                    }
                }
            }
        }
    }
    public IEnumerator GetComenzi(DatabaseReference reference)
    {
        DatabaseHelper.comenzi.Clear();
        var RestauranteLog = reference.Child("Comenzi").GetValueAsync();
        yield return new WaitUntil(predicate: () => RestauranteLog.IsCompleted);
        if (RestauranteLog != null)
        {
            foreach (var y in RestauranteLog.Result.Children)
            {
                Debug.Log(y.Key);
                ComandaModel model = new ComandaModel
                {
                    Id = int.Parse(y.Key),
                    IdLivrator = int.Parse(y.Child("IdLivrator").GetValue(true).ToString()),
                    IdUser = int.Parse(y.Child("IdUser").GetValue(true).ToString()),
                    IdRestaurant = int.Parse(y.Child("IdRestaurant").GetValue(true).ToString()),
                    LivratorLat = float.Parse(y.Child("LivratorLat").GetValue(true).ToString()),
                    LivratorLon = float.Parse(y.Child("LivratorLon").GetValue(true).ToString()),
                    AdresaLat = float.Parse(y.Child("AdresaLat").GetValue(true).ToString()),
                    AdresaLon = float.Parse(y.Child("AdresaLon").GetValue(true).ToString()),
                    StatusComanda = int.Parse(y.Child("StatusComanda").GetValue(true).ToString()),
                };
                yield return StartCoroutine(GetMeniuComanda(model.Id.ToString(), reference));
                //model.Meniu = meniuri;
                DatabaseHelper.comenzi.Add(model);
            }
            for (int i = 0; i < DatabaseHelper.comenzi.Count; i++)
            {
                DatabaseHelper.comenzi[i].Meniu = meniuri1[i];
            }
        }
    }
    public IEnumerator GetMeniuComanda(string key, DatabaseReference reference)
    {
        List<MenuModel> meniu = new List<MenuModel>();
        var MeniuLog = reference.Child("Comenzi").Child(key).Child("Meniu").GetValueAsync();
        yield return new WaitUntil(predicate: () => MeniuLog.IsCompleted);
        if (MeniuLog != null)
        {
            foreach (var y in MeniuLog.Result.Children)
            {
                meniu.Add(new MenuModel
                {
                    Nume = y.Key,
                    Pret = int.Parse(y.GetValue(true).ToString())
                });
                Debug.Log(y.Key);
            }

        }
        meniuri1.Add(meniu);

    }
    private IEnumerator LoadRestaurante()
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
               // yield return StartCoroutine(GetMeniu(model.Id.ToString(), reference));
                //model.Meniu = meniuri;
                DatabaseHelper.restaurante.Add(new RestaurantModel
                {
                    Id = model.Id,
                    lat = model.lat,
                    lon = model.lon,
                    Nume = model.Nume,
                    OraDeschidere = model.OraDeschidere,
                    OraInchidere = model.OraInchidere,
                });
            }
        }
        /*
        for (int i = 0; i < DatabaseHelper.restaurante.Count; i++)
        {
            DatabaseHelper.restaurante[i].Meniu = meniuri[i];
        }
        */
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
