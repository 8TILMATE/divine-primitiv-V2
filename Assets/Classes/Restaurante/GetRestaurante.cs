using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Firebase.Database;
using UnityEngine.UI;
using UnityEngine.Networking;
using Firebase.Storage;
using Firebase;
using Firebase.Extensions;
using Firebase.Extensions;
public class GetRestaurante : MonoBehaviour
{
    public static List<List<MenuModel>> meniuri = new List<List<MenuModel>>();
    public static List<List<MenuModel>> meniuri1 = new List<List<MenuModel>>();

    DatabaseReference reference;
    public  GameObject taticul;
    private  GameObject gam;
    public  GameObject toSpawn;
    public GameObject Canvas1;
    public GameObject Canvas2;
    public GameObject Container;
    public RawImage textura;
    public Text SubTotalComanda;
    public List<KeyValuePair<Texture2D, string>> texturi = new List<KeyValuePair<Texture2D, string>>();
    // Start is called before the first frame update
    RawImage rawImage ;
    FirebaseStorage storage;
    StorageReference storageReference;

    IEnumerator LoadImage(string MediaUrl,string id)
    {
        UnityWebRequest request = UnityWebRequestTexture.GetTexture(MediaUrl); //Create a request
        yield return request.SendWebRequest(); //Wait for the request to complete
        if (request.isNetworkError || request.isHttpError)
        {
            Debug.Log(request.error);
        }
        else
        {
            texturi.Add(new KeyValuePair<Texture2D, string>(((DownloadHandlerTexture)request.downloadHandler).texture, id));
            //rawImage.texture = ((DownloadHandlerTexture)request.downloadHandler).texture;
            // setting the loaded image to our object
        }
    }
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
        StartCoroutine(GetComenzi(reference));

    }

    // Update is called once per frame
    void Update()
    {
        //SubTotalComanda.text = "Total = " + DatabaseHelper.SubtotalComanda.ToString() + " lei";
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
                yield return StartCoroutine(GetMeniu(model.Id.ToString(), reference));
                //model.Meniu = meniuri;
                DatabaseHelper.restaurante.Add(new RestaurantModel 
                {
                    Id=model.Id,
                    lat=model.lat,
                    lon=model.lon,
                    Nume=model.Nume,
                    OraDeschidere=model.OraDeschidere,
                    OraInchidere=model.OraInchidere,
                });                
            }
        }
        for(int i = 0; i < DatabaseHelper.restaurante.Count; i++)
        {
            DatabaseHelper.restaurante[i].Meniu = meniuri[i];
        }
        
        foreach (var x in DatabaseHelper.restaurante)
        {
            gam=Instantiate(toSpawn, taticul.transform.position, Quaternion.identity) as GameObject;
            gam.name = x.Id.ToString()+"_"+x.Nume;
            gam.transform.parent = taticul.transform;
            gam.gameObject.transform.localScale = new Vector3(1, 1, 1);
            foreach (Transform t in gam.transform)
            {

                Text text = t.GetComponent<Text>();
                if (text)
                {
                    text.text = x.Nume;
                }
                RawImage rawraw= t.GetComponent<RawImage>();
                Button button = t.GetComponent<Button>();
                if (button != null)
                {
                    OpenMenu om = button.gameObject.GetComponent<OpenMenu>();
                    if (om != null)
                    {
                        om.Canvas1 = Canvas1;
                        om.Canvas2 = Canvas2;
                        om.Container = Container;
                        om.RestaurantImagine = textura;
                    }
                }
                storage = FirebaseStorage.DefaultInstance;
                storageReference = storage.GetReferenceFromUrl("gs://divine-f6fa7.appspot.com");
                StorageReference image = storageReference.Child(x.Id.ToString()+".png");
             
                if (rawraw != null)
                {
                    rawraw.texture = new Texture2D(1, 1);
                    rawImage = rawraw;
                    image.GetDownloadUrlAsync().ContinueWithOnMainThread(task =>
                    {
                        if (!task.IsFaulted && !task.IsCanceled)
                        {
                             StartCoroutine(LoadImage((task.Result).ToString(), x.Id.ToString())); //Fetch file from the link
                        }
                        else
                        {
                            Debug.Log(task.Exception);
                        }
                    });
                  
                    /*
                    Firebase.Storage.StorageReference storageReference =
   Firebase.Storage.FirebaseStorage.DefaultInstance.GetReferenceFromUrl("gs://divine-f6fa7.appspot.com");

                    storageReference.Child(x.Id.ToString()+".png").GetBytesAsync(1024 * 1024).
                        ContinueWith((System.Threading.Tasks.Task<byte[]> task) =>
                        {
                            if (task.IsFaulted || task.IsCanceled)
                            {
                                Debug.Log(task.Exception.ToString());
                            }
                            else
                            {
                                byte[] fileContents = task.Result;
                                Texture2D texture = new Texture2D(1, 1);
                                texture.LoadImage(fileContents);
                                rawImage.texture = texture;
            //if you need sprite for SpriteRenderer or Image
            /*
            Sprite sprite = Sprite.Create(texture, new Rect(0.0f, 0.0f, texture.width,
                                texture.height), new Vector2(0.5f, 0.5f), 100.0f);
            
                                Debug.Log("Finished downloading!");
            
                            }

                        });
            */
            

                }
            }
            yield return new WaitForSeconds(3);
            foreach(Transform transform in taticul.transform)
            {
                var idname = transform.gameObject.name.Split("_");
                foreach(var y in texturi)
                {
                    if (y.Value == idname[0])
                    {
                        foreach(Transform t in transform.gameObject.transform)
                        {
                            RawImage img;
                            img = t.GetComponent<RawImage>();
                            if (img != null)
                            {
                                img.texture = y.Key;
                            }
                        }
                    }
                }
            }
        }

    }
        /*
    IEnumerator LoadImage(string MediaUrl)
    {
        UnityWebRequest request = UnityWebRequestTexture.GetTexture(MediaUrl); //Create a request
        yield return request.SendWebRequest(); //Wait for the request to complete
        if (request.isNetworkError || request.isHttpError)
        {
            Debug.Log(request.error);
        }
        else
        {
            rawImage.texture = ((DownloadHandlerTexture)request.downloadHandler).texture;
            // setting the loaded image to our object
        }
    }
        */
    public  IEnumerator GetMeniu(string key, DatabaseReference reference)
    {
        List<MenuModel> meniu = new List<MenuModel>();
        var MeniuLog = reference.Child("Restaurante").Child(key).Child("Meniu").GetValueAsync();
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
        meniuri.Add(meniu);


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
}

