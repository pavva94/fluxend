using UnityEngine;
using System.Collections;


public class MovFlusso : MonoBehaviour
{
    
    public int moveOk;
    public int moveNotOk = 0;
    public int lastmoveOk = 0;
    void Start()
    {
     
        InvokeRepeating("moveOn", 1, 1.0f);

    }

    void moveOn()
    {

    moveOk = Random.Range(1,9);
    
    moveNotOk = 0;
    }   

    void moveOff()
    {

    moveOk = 0;

    }   
    void Update()
    {
        
        if (moveNotOk == 1)
        {
        moveOn();
        }
        
        if (moveOk == 1 & lastmoveOk != 4) 
        {
            transform.Translate(Vector3.right * Time.deltaTime * Random.Range(3,10) , Space.World);
            transform.Translate(Vector3.up * Time.deltaTime * Random.Range(3,10) , Space.World);
            gameObject.GetComponent<ParticleSystem>().enableEmission = true;
            gameObject.GetComponent<ParticleSystem>().Play();
            Debug.Log(moveNotOk);
            lastmoveOk = moveOk; 
        }
        else if (moveOk == 1 & lastmoveOk == 4)
        {
        moveNotOk = 1;
        }
        if (moveOk == 2 & lastmoveOk !=3) 
        {
            transform.Translate(Vector3.left * Time.deltaTime * Random.Range(3,10) , Space.World);
            transform.Translate(Vector3.up * Time.deltaTime * Random.Range(3,10) , Space.World);
            gameObject.GetComponent<ParticleSystem>().enableEmission = true;
            gameObject.GetComponent<ParticleSystem>().Play();
            Debug.Log(moveNotOk);
            lastmoveOk = moveOk; 
        }  
        else if (moveOk == 2 & lastmoveOk ==3) 
        {
        moveNotOk = 1;
        }
        
        if (moveOk == 3 & lastmoveOk !=2) 
        {
            transform.Translate(Vector3.right * Time.deltaTime * Random.Range(3,10) , Space.World);
            transform.Translate(Vector3.down * Time.deltaTime * Random.Range(3,10) , Space.World);
            gameObject.GetComponent<ParticleSystem>().enableEmission = true;
            gameObject.GetComponent<ParticleSystem>().Play();
            Debug.Log(moveNotOk);
            lastmoveOk = moveOk; 
        }
        else if (moveOk == 3 & lastmoveOk ==2)
        {
        moveNotOk = 1;
        }

        if (moveOk == 4 & lastmoveOk !=1) 
        {
            transform.Translate(Vector3.left * Time.deltaTime * Random.Range(3,10) , Space.World);
            transform.Translate(Vector3.down * Time.deltaTime * Random.Range(3,10) , Space.World);
            gameObject.GetComponent<ParticleSystem>().enableEmission = true;
            gameObject.GetComponent<ParticleSystem>().Play();
            Debug.Log(moveNotOk);
            lastmoveOk = moveOk; 
        }
        else if (moveOk == 4 & lastmoveOk ==1)
        {
        moveNotOk = 1;
        }
        if (moveOk == 5 & lastmoveOk !=6) 
        {
            transform.Translate(Vector3.right * Time.deltaTime * Random.Range(3,10) , Space.World);
            gameObject.GetComponent<ParticleSystem>().enableEmission = true;
            gameObject.GetComponent<ParticleSystem>().Play();
            Debug.Log(moveNotOk);
            lastmoveOk = moveOk; 
        }
        else if (moveOk == 5 & lastmoveOk ==6)
        {    
        moveNotOk = 1;
        }
        if (moveOk == 6 & lastmoveOk !=5) 
        {
            transform.Translate(Vector3.left * Time.deltaTime * Random.Range(3,10) , Space.World);
            gameObject.GetComponent<ParticleSystem>().enableEmission = true;
            gameObject.GetComponent<ParticleSystem>().Play();
            Debug.Log(moveNotOk);
            lastmoveOk = moveOk; 
        }
        else if (moveOk == 6 & lastmoveOk ==5) 
        {
        moveNotOk = 1;
        }
        if (moveOk == 7 & lastmoveOk !=8) 
        {
            transform.Translate(Vector3.up * Time.deltaTime * Random.Range(3,10) , Space.World);
            gameObject.GetComponent<ParticleSystem>().enableEmission = true;
            gameObject.GetComponent<ParticleSystem>().Play();
            Debug.Log(moveNotOk);
            lastmoveOk = moveOk; 
        }
        else if (moveOk == 7 & lastmoveOk ==8)
        {
        moveNotOk = 1;
        }
        if (moveOk == 8 & lastmoveOk !=7) 
        {
            transform.Translate(Vector3.down * Time.deltaTime * Random.Range(3,10) , Space.World);
            gameObject.GetComponent<ParticleSystem>().enableEmission = true;
            gameObject.GetComponent<ParticleSystem>().Play();
            Debug.Log(moveNotOk);
            lastmoveOk = moveOk;       
        }    
        else if (moveOk == 8 & lastmoveOk ==7) 
        {
        moveNotOk = 1;
        }
        if (moveOk == 0)  
        {

            gameObject.GetComponent<ParticleSystem>().enableEmission = false;
            gameObject.GetComponent<ParticleSystem>().Stop();

        }

        
    }
}