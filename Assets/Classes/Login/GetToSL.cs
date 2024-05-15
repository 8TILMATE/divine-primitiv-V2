using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GetToSL : MonoBehaviour
{
    // Start is called before the first frame update
    public void LoginButton()
    {
        SceneManager.LoadScene("Login");
    }
    public void SignupButton()
    {
        SceneManager.LoadScene("SignUp");
    }
}
