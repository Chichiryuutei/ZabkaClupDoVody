using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Click_Animations : MonoBehaviour
{


	Animator click;
    // Start is called before the first frame update
    void Start()
    {
        click = GetComponent<Animator>();
		
		
    }

    // Update is called once per frame
    void Update()
    {
     
		if (Input.GetMouseButtonDown(0))
		{
		click.SetTrigger("LeftClick");
		}
	 
		if (Input.GetMouseButtonDown(1))
		{
		click.SetTrigger("RightClick");
		}
	    
    }
}
