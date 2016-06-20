using UnityEngine;
using System.Collections;

public class spawn: MonoBehaviour {

public GameObject cubo;
 

 void Start() {
      InvokeRepeating("randomspawn", 5, 5);
 }          
                     
void randomspawn() {
                  
              
            
for(int i=0;i<5;i++) {   
   
Instantiate(cubo,genpos(),Quaternion.identity);
                         
    }
} 
     

Vector3 genpos()
{ 
int x,y,z;
 x = UnityEngine.Random.Range(-10,10);
 y = 4;
 z = 0;
return new Vector3(x,y,z);
}



}


