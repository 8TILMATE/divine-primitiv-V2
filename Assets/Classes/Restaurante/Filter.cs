using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Filter : MonoBehaviour
{
    public InputField input;
    public GameObject container;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void OnTextChanged()
    {
        foreach(Transform z in container.transform)
        {
            if (!z.gameObject.name.Contains(input.text))
            {
                z.gameObject.SetActive(false);
            }
            else
            {
                z.gameObject.SetActive(true);
            }
        }
    }
}
