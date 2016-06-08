using UnityEngine;
using System.Collections;

public class spawn: MonoBehaviour {

public GameObject cubo;
 float x;
 float y;
 float z;


                
                     
                  void randomspawn() {
                  
              
                 x = Random.Range(-5, 5);
                 y = Random.Range(-2, 2);
                 z = 0;

            Vector3 currentPosition = cubo.transform.position;
                       for(int i=0;i<2;i++)
{   
                         GameObject tmpObj = GameObject.Instantiate(cubo,currentPosition,Quaternion.identity) as GameObject;
                         currentPosition = new Vector3(x,y,z);
                     }
                 }
     
void Update() {
        Invoke("randomspawn", 10);
    }
}


