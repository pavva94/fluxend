using UnityEngine;
using System.Collections;

public class Tutorial : MonoBehaviour {
public GameObject flusso;
public GameObject flusso2;
public GameObject flusso3;
public GameObject ok;
public GameObject play;
public GameObject trascina;
public GameObject mano;
public GameObject frecdx;
public GameObject mano2;
public GameObject frecsx;
public GameObject mainCamera;
// di quanto la camera di sposta
public Vector3 offset = new Vector3(0.1f,0);
public Vector3 offset2 = new Vector3(0.0f,0.1f);
public float velflux = 1.0f; 
public int tutorialStep = 0;
 //rielva se il touch è premuto o no
 int touchPressed = 0;
 float startPosx; 
 float startPosy; 


	// Use this for initialization
	void Start () {
				flusso3.GetComponent<ParticleSystem> ().enableEmission = true;
				flusso3.GetComponent<ParticleSystem> ().Play ();
				flusso3.GetComponent<Animation> ().Play();
				trascina.SetActive(true);
				tutorial();
				Invoke("tutorialOn", 5f);
				Invoke("tutorial", 5.1f);
	}
	
	void tutorialOn () {
					 tutorialStep = 1;
				}

	void tutorial () {
				
				if (tutorialStep == 1) {
				 	flusso3.GetComponent<ParticleSystem> ().enableEmission = false;
					flusso3.GetComponent<ParticleSystem> ().Stop ();
					flusso3.GetComponent<Animation> ().Stop();
				 	flusso.GetComponent<ParticleSystem> ().enableEmission = true;
					flusso.GetComponent<ParticleSystem> ().Play ();
					flusso.GetComponent<Animation> ().Play();
					frecdx.SetActive(true);
					mano.SetActive(true);
				}
				if (tutorialStep == 2) {
				 	flusso.GetComponent<ParticleSystem> ().enableEmission = false;
					flusso.GetComponent<ParticleSystem> ().Stop ();
				 	flusso2.GetComponent<ParticleSystem> ().enableEmission = true;
					flusso2.GetComponent<ParticleSystem> ().Play ();
					frecsx.SetActive(true);
					mano2.SetActive(true);
				}
	}
	// Update is called once per frame
	void Update () {
				//frecdx.transform.position = new Vector3 (mano.transform.position.x + 4f, -2.07f, 0f);
				if (mainCamera.transform.position.x > flusso.transform.position.x & tutorialStep == 1) {
					flusso2.GetComponent<Animation> ().Play();
					ok.GetComponent<Animation> ().Play();
					ok.SetActive(true);
					tutorialStep = 2;
					tutorial();
				}
				if (mainCamera.transform.position.x <= 0 & tutorialStep == 2) {
					
					play.SetActive(true);
					play.GetComponent<Animation> ().Play();
					tutorialStep = 0;
				}
				if (mainCamera.transform.position.x < 0 & tutorialStep == 0) {
					flusso2.GetComponent<ParticleSystem> ().enableEmission = false;
					flusso2.GetComponent<ParticleSystem> ().Stop ();
					flusso3.GetComponent<ParticleSystem> ().enableEmission = true;
					flusso3.GetComponent<ParticleSystem> ().Play ();
					flusso3.GetComponent<Animation> ().Play();
					ok.SetActive(true);
					
				}				
				//Inizio funzione touch
				int tocchi = Input.touchCount;
				if (tocchi > 0) {
				// get the first one
				Touch touch = Input.GetTouch (0);
				
				if (touchPressed == 1) {
						
						if (touch.position.x < startPosx) {
						mainCamera.transform.position += offset * -1 * velflux;
						frecsx.SetActive(false);
						mano2.SetActive(false);
						ok.GetComponent<Animation> ().Stop();
						ok.SetActive(false);
						}
						if (touch.position.x > startPosx) {
						mainCamera.transform.position += offset * 1 * velflux;
						frecdx.SetActive(false);
						mano.SetActive(false);
						}
						if (touch.position.y < startPosy) {
						mainCamera.transform.position += offset2 * -1 * velflux;
						}
						if (touch.position.y > startPosy) {
						mainCamera.transform.position += offset2 * 1 * velflux;
						}
					}
					
				switch (touch.phase) {
				// Record initial touch position.
				case TouchPhase.Began:
					if (touchPressed == 0) {
					startPosx = touch.position.x;
					startPosy = touch.position.y;
					}

					break;
				
				// Determine direction by comparing the current touch position with the initial one.
				case TouchPhase.Moved:
					
					touchPressed = 1;
			
					break;

				// Report that a direction has been chosen when the finger is lifted.
				case TouchPhase.Ended:
					touchPressed = 0;
					break;
			}
		}
// CODICE UTILE PER TEST CON PC
	        
			float moveHorizontal = Input.GetAxis ("Horizontal");
			
			float moveVertical = Input.GetAxis ("Vertical");
			// movimento verticale costante blocchi
			//padreCubi.transform.Translate(Vector3.down * moveVertical);
			if (moveHorizontal > 0) {
				frecdx.SetActive(false);
				mano.SetActive(false);
				
				mainCamera.transform.position += offset * 1;
			}else if (moveHorizontal < 0) {
				frecsx.SetActive(false);
				mano2.SetActive(false);
				mainCamera.transform.position += offset * -1;
				ok.GetComponent<Animation> ().Stop();
				ok.SetActive(false);
			}
			if (moveVertical > 0)
				mainCamera.transform.position += offset2 * 1;
			else if (moveVertical < 0)
				mainCamera.transform.position += offset2 * -1;
			//transform.Translate(Vector3.down * moveVertical);*/
	
	}
}
