using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class url_open : MonoBehaviour
{

public string Url;

public void Open()
{
	Application.OpenURL(Url);
}	

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
