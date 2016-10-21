using UnityEngine;
using System.Collections;


public class MovFlusso : MonoBehaviour
{
	
    public int moveOk;
    void Start()
	{
     
        InvokeRepeating("moveOn", 1, 1.0f);

    }

    void moveOn()
    {

    moveOk = Random.Range(1,3);

    }   

    void moveOff()
    {

    moveOk = 0;

    }   
    void Update()
    {
       
        if (moveOk == 1) 
        {


            transform.Translate(Vector3.right * Time.deltaTime * Random.Range(1,4) , Camera.main.transform);
            gameObject.GetComponent<ParticleSystem>().enableEmission = true;
            gameObject.GetComponent<ParticleSystem>().Play();
            Debug.Log("Move 1");
        } 
        if (moveOk == 2) 
        {
            transform.Translate(Vector3.left * Time.deltaTime * Random.Range(1,4) , Camera.main.transform);
            gameObject.GetComponent<ParticleSystem>().enableEmission = true;
            gameObject.GetComponent<ParticleSystem>().Play();
            Debug.Log("Move 2");
        }
        if (moveOk == 0)  
        {

            gameObject.GetComponent<ParticleSystem>().enableEmission = false;
            gameObject.GetComponent<ParticleSystem>().Stop();

        }

        
    }
}
