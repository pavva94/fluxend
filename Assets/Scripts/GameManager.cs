using UnityEngine;
using System.Collections;
using UnityEngine.UI; // include UI namespace so can reference UI elements
using UnityEngine.Advertisements;
using UnityEngine.Analytics;
using System.Collections.Generic;
using UnityEngine.EventSystems; // include EventSystems namespace so can set initial input for controller support
using GooglePlayGames;
using GooglePlayGames.BasicApi;
using UnityEngine.SocialPlatforms;
using ChartboostSDK;
using System.Linq;

public class GameManager : MonoBehaviour {

	// static reference to game manager so can be called from other scripts directly (not just through gameobject component)
	public static GameManager gm;
	//movimento mouse mentre è cliccato
    private bool isHeld;
	// levels to move to on victory and lose
	public string levelAfterVictory;
	public string levelAfterGameOver;
	public float lunghezzaFlusso = 10.0f;
    // velocita movimento
    public float speed;
    // centro dello schermo
    private float screenCenterX;
    // centro dello schermo
    private float screenCenterY;

    public bool debug = false;

    // game performance
    public int score = 0;
    public int startEnergy = 50;
    public int energy;
    public int amount = 10;
    
	public Canvas canvas;
    // UI elements to control
    public Text UIScore;
	public Text UIAddScore;
    public Text UIRemoveScore;
	// public Text UIHighScore;
    public Text UIEnergy;
    //public GameObject[] UIExtraLives;
	public GameObject UIGameOver;
    // GameObject startpoint
    public GameObject startpoint;
    //timer per regolare velocità incrementale
    float Timer = 0.0f;
    //grado che verrà moltiplicato a moveVertical
    float gradovel = 0.0f;
    //abilita al movimento del flusso in auto
    private int moveOk;
    //disabilita flusso che si sovrappone
    private int moveNotOk = 0;
    //memorizza l'ultimo movimento fatto dal flusso
    private int lastmoveOk = 0;
    
    // main camera
    public GameObject mainCamera;

    //flusso
    public GameObject flusso;
    // particle system del flusso
    private ParticleSystem particleSystemflusso;
    //variabile per velocità flusso
    public float velflux = 1.0f; 
    //sfere da catturare
    public GameObject fluxballBonus;
    public GameObject fluxballMalus;

    //esplosione
    public GameObject esplosione;
    //esplosione 2
    public GameObject esplosione2;

    //regolo luminosità gioco
    public GameObject lumen;
    //bagliore della punta del flusso
    public GameObject bagliore;

    // di quanto la camera di sposta
    public Vector3 offset = new Vector3(0.1f,0);
    public Vector3 offset2 = new Vector3(0.0f,0.1f);
    
	// pubblicita si/no
    [SerializeField]
	public bool pubblicita = true;

	// pannello di pausa
	public GameObject pausePanel;

	// pannello di morte
	public GameObject deathPanel;

	// Button di pausa
	public GameObject MenuPauseDefaultButton;
	public GameObject MenuDeathDefaultButton;

	// Button di Pausa
	public GameObject pauseButton;

	// game in pausa
	bool paused = false;

	// setting button
	private bool vibrate = true;
	private bool sound = true;

	// la musica di sottofondo (AudioListener)
	public AudioSource musica;

    // lista colori già usati dal flusso
    private List<Color> listaColoriUsati = new List<Color>(100);

    // contatore fluxball prese
    private int contFluxball = 0;
    public int ChangeColorAfterXFluxball = 10;

    // set things up here
    void Awake () {
		

		// setup reference to game manager
		if (gm == null)
			gm = this.GetComponent<GameManager>();

		// Instantiate (flusso);

		// setup all the variables, the UI, and provide errors if things not setup properly.
		setupDefaults();

    }

    // Use this for initialization
    void Start()
    {
    	//parte esplosione 
    	esplosione.GetComponent<ParticleSystem> ().enableEmission = true;
		esplosione.GetComponent<ParticleSystem> ().Play ();
		//parte esplosione 2 
    	esplosione2.GetComponent<ParticleSystem> ().enableEmission = true;
		esplosione2.GetComponent<ParticleSystem> ().Play ();
        // Avvia funzione per inizializzare movimento flusso
        InvokeRepeating("moveOn", 3, 1.0f);
        //Esegue funzione spawn sfere di flusso Bonus
        InvokeRepeating("spawnFluxballBonus", 5f, 3f);
        //Esegue funzione spawn sfere di flusso Malus
        InvokeRepeating("spawnFluxballMalus", 5f, 3f);

        //UIGameOver.SetActive(false); // disattiva il text gameOver

        // rigidbody = GetComponent<Rigidbody2D>();
        // Disable screen dimming
        Screen.sleepTimeout = SleepTimeout.NeverSleep;
        screenCenterX = Screen.width * 0.5f;
        screenCenterY = Screen.height * 0.5f;

        energy = startEnergy;
        refreshGUI();

        Time.timeScale = 1f;

		musica = Camera.main.gameObject.GetComponent<AudioSource>();

        // prendo il particle system del flusso, deve sempre esserci un figlio con il ParticleSystem
        particleSystemflusso = flusso.GetComponentsInChildren(typeof(ParticleSystem))[1] as ParticleSystem;

        // Carico gli Ads di ChartBoost 
        manageAds();
    }
        //abilita movimento flusso
    void moveOn()
    {
	    //Serve per dare la punta di bagliore blu al flusso
	    moveOk = Random.Range(1,9);
	    moveNotOk = 0;
	    bagliore.GetComponent<SpriteRenderer> ().sprite = Resources.Load("bagliorepunta", typeof(Sprite)) as Sprite;
    }   
    
    //disabilita movimento flusso
    void moveOff()
    {
        moveOk = 0;
    }
    //Funzione per spawn delle sfere di flusso random nello schermo
    void spawnFluxballBonus() 
    {
                  
        for(int i=0;i<2;i++) {   
		   
		GameObject cloneFluxball = (GameObject)Instantiate (fluxballBonus,genpos(),Quaternion.identity);
		//Distrugge dopo un valore impostato l'oggetto istanziato
		Destroy (cloneFluxball, 2.9f);                         
		}
	}

    //Funzione per spawn delle sfere di flusso random nello schermo
    void spawnFluxballMalus()
    {
       GameObject cloneFluxball = (GameObject)Instantiate(fluxballMalus, genpos(), Quaternion.identity);
       //Distrugge dopo un valore impostato l'oggetto istanziato
       Destroy(cloneFluxball, 2.9f);
    }

    Vector3 genpos() { 
		float x,y,z;
		x = Random.Range(flusso.transform.position.x - 5, flusso.transform.position.x + 5);
		y = Random.Range(flusso.transform.position.y - 5,flusso.transform.position.y + 5);
		z = 0;
		return new Vector3(x,y,z);
	}
    
    // game loop
    void Update() {
 
		// se sono in pausa non faccio nulla
		if (!paused) {
			
			//velocità incrementale flusso (in base al tempo)
			//Incrementa timer ogni secondo
			Timer += Time.deltaTime;

			
			if (Timer > 5) {
				gradovel += 0.01f;
				Timer = 0.0f;
				velflux = velflux + gradovel;
			}   
			
			
			
			//assegna color per alpha luminosità
        	Color color = lumen.GetComponent<Renderer>().material.color;
			
			//cambia luminosità in base a distanza flusso
			if (mainCamera.transform.position.x < flusso.transform.position.x +2.3 | mainCamera.transform.position.x > flusso.transform.position.x -2.3 | mainCamera.transform.position.y < flusso.transform.position.y +3 | mainCamera.transform.position.y > flusso.transform.position.y -3 ) {
				//lumen.GetComponent<SpriteRenderer> ().sprite = Resources.Load("lumen001", typeof(Sprite)) as Sprite;
	 			color.a = 0.0f;
	 			lumen.GetComponent<Renderer>().material.SetColor("_Color", color);
			}
			if (mainCamera.transform.position.x > flusso.transform.position.x +2.3 | mainCamera.transform.position.x < flusso.transform.position.x -2.3 | mainCamera.transform.position.y > flusso.transform.position.y +3 | mainCamera.transform.position.y < flusso.transform.position.y -3 ) {
				//lumen.GetComponent<SpriteRenderer> ().sprite = Resources.Load("lumen001", typeof(Sprite)) as Sprite;
	 			color.a = 0.3f;
	 			lumen.GetComponent<Renderer>().material.SetColor("_Color", color);
			}
			if (mainCamera.transform.position.x > flusso.transform.position.x +2.6 | mainCamera.transform.position.x < flusso.transform.position.x -2.6 | mainCamera.transform.position.y > flusso.transform.position.y +3.5 | mainCamera.transform.position.y < flusso.transform.position.y -3.5 ) {
				//lumen.GetComponent<SpriteRenderer> ().sprite = Resources.Load("lumen01", typeof(Sprite)) as Sprite;
	 			color.a = 0.5f;
	 			lumen.GetComponent<Renderer>().material.SetColor("_Color", color);
			}
			if (mainCamera.transform.position.x > flusso.transform.position.x +3 | mainCamera.transform.position.x < flusso.transform.position.x -3 | mainCamera.transform.position.y > flusso.transform.position.y +4 | mainCamera.transform.position.y < flusso.transform.position.y -4 ) {
				//lumen.GetComponent<SpriteRenderer> ().sprite = Resources.Load("lumen1", typeof(Sprite)) as Sprite;
	 			color.a = 0.7f;
	 			lumen.GetComponent<Renderer>().material.SetColor("_Color", color);
			}
			if (mainCamera.transform.position.x > flusso.transform.position.x +3.5 | mainCamera.transform.position.x < flusso.transform.position.x -3.5 | mainCamera.transform.position.y > flusso.transform.position.y +5 | mainCamera.transform.position.y < flusso.transform.position.y -5 ) {
				//lumen.GetComponent<SpriteRenderer> ().sprite = Resources.Load("lumen2", typeof(Sprite)) as Sprite;
	 			color.a = 0.9f;
	 			lumen.GetComponent<Renderer>().material.SetColor("_Color", color);
			}
			

			// prova di guadagno trofeo
			/*Social.ReportProgress("CgkI6Imc5NEGEAIQAg", 100.0f, (bool success) => {
				// handle success or failure
			});*/


			//----INIZIO codice per automizzare nelle diverse direzioni l'andamento del flusso
			if (moveNotOk == 1) {
				moveOn ();
			}
			
			if (moveOk == 1 & lastmoveOk != 4) {
				flusso.transform.Translate (Vector3.right * Time.deltaTime * velflux, Space.World);
				flusso.transform.Translate (Vector3.up * Time.deltaTime * velflux, Space.World);
				flusso.GetComponent<ParticleSystem> ().enableEmission = true;
				flusso.GetComponent<ParticleSystem> ().Play ();
				
				lastmoveOk = moveOk; 
			} else if (moveOk == 1 & lastmoveOk == 4) {
				moveNotOk = 1;
			}
			if (moveOk == 2 & lastmoveOk != 3) {
				flusso.transform.Translate (Vector3.left * Time.deltaTime * velflux, Space.World);
				flusso.transform.Translate (Vector3.up * Time.deltaTime * velflux, Space.World);
				flusso.GetComponent<ParticleSystem> ().enableEmission = true;
				flusso.GetComponent<ParticleSystem> ().Play ();
				
				lastmoveOk = moveOk; 
			} else if (moveOk == 2 & lastmoveOk == 3) {
				moveNotOk = 1;
			}
	        
			if (moveOk == 3 & lastmoveOk != 2) {
				flusso.transform.Translate (Vector3.right * Time.deltaTime * velflux, Space.World);
				flusso.transform.Translate (Vector3.down * Time.deltaTime * velflux, Space.World);
				flusso.GetComponent<ParticleSystem> ().enableEmission = true;
				flusso.GetComponent<ParticleSystem> ().Play ();
				
				lastmoveOk = moveOk; 
			} else if (moveOk == 3 & lastmoveOk == 2) {
				moveNotOk = 1;
			}

			if (moveOk == 4 & lastmoveOk != 1) {
				flusso.transform.Translate (Vector3.left * Time.deltaTime * velflux, Space.World);
				flusso.transform.Translate (Vector3.down * Time.deltaTime * velflux, Space.World);
				flusso.GetComponent<ParticleSystem> ().enableEmission = true;
				flusso.GetComponent<ParticleSystem> ().Play ();
				
				lastmoveOk = moveOk; 
			} else if (moveOk == 4 & lastmoveOk == 1) {
				moveNotOk = 1;
			}
			if (moveOk == 5 & lastmoveOk != 6) {
				flusso.transform.Translate (Vector3.right * Time.deltaTime * velflux, Space.World);
				flusso.GetComponent<ParticleSystem> ().enableEmission = true;
				flusso.GetComponent<ParticleSystem> ().Play ();
				
				lastmoveOk = moveOk; 
			} else if (moveOk == 5 & lastmoveOk == 6) {    
				moveNotOk = 1;
			}
			if (moveOk == 6 & lastmoveOk != 5) {
				flusso.transform.Translate (Vector3.left * Time.deltaTime * velflux, Space.World);
				flusso.GetComponent<ParticleSystem> ().enableEmission = true;
				flusso.GetComponent<ParticleSystem> ().Play ();
				
				lastmoveOk = moveOk; 
			} else if (moveOk == 6 & lastmoveOk == 5) {
				moveNotOk = 1;
			}
			if (moveOk == 7 & lastmoveOk != 8) {
				flusso.transform.Translate (Vector3.up * Time.deltaTime * velflux, Space.World);
				flusso.GetComponent<ParticleSystem> ().enableEmission = true;
				flusso.GetComponent<ParticleSystem> ().Play ();
				
				lastmoveOk = moveOk; 
			} else if (moveOk == 7 & lastmoveOk == 8) {
				moveNotOk = 1;
			}
			if (moveOk == 8 & lastmoveOk != 7) {
				flusso.transform.Translate (Vector3.down * Time.deltaTime * velflux, Space.World);
				flusso.GetComponent<ParticleSystem> ().enableEmission = true;
				flusso.GetComponent<ParticleSystem> ().Play ();
				
				lastmoveOk = moveOk;       
			} else if (moveOk == 8 & lastmoveOk == 7) {
				moveNotOk = 1;
			}
			if (moveOk == 0) {

				flusso.GetComponent<ParticleSystem> ().enableEmission = false;
				flusso.GetComponent<ParticleSystem> ().Stop ();

			}
			//----FINE CODICE AUTOMATIZZAZIONE MOVIMENTI FLUSSO 

			//Incrementa timer ogni secondo
			Timer += Time.deltaTime;

			if (Timer > 5) {
				gradovel += 0.01f;
				Timer = 0.0f;
			}   
	        
			// movimento verticale costante blocchi
			//mainCamera.transform.Translate(Vector3.up * (moveVertical + gradovel));

			int tocchi = Input.touchCount;

			if (tocchi > 0) {
				// get the first one
				Touch firstTouch = Input.GetTouch (0);
				// if it began this frame
				if (firstTouch.phase == TouchPhase.Moved || firstTouch.phase == TouchPhase.Stationary || firstTouch.phase == TouchPhase.Began || firstTouch.phase == TouchPhase.Ended) {
					RaycastHit2D hitObj = Physics2D.Raycast (Camera.main.ScreenToWorldPoint((Input.GetTouch (0).position)), Vector2.zero);
					if(hitObj.collider != null){
		                hit(hitObj.transform.gameObject);   
				    }

					if (firstTouch.position.x > screenCenterX) {
						// if the touch position is to the right of center
						// move right
						//padreCubi.transform.Translate(speed * 1, Vector3.down.y * moveVertical, 0);
						mainCamera.transform.position += offset * 1;
					} else if (firstTouch.position.x < screenCenterX) {
						// if the touch position is to the left of center
						// move left
						//padreCubi.transform.Translate(speed * -1, Vector3.down.y * moveVertical, 0);
						mainCamera.transform.position += offset * -1;
					}

					if (firstTouch.position.y > screenCenterY) {
						// if the touch position is to the right of center
						// move right
						//padreCubi.transform.Translate(speed * 1, Vector3.down.y * moveVertical, 0);
						mainCamera.transform.position += offset2 * 1;
					} else if (firstTouch.position.y < screenCenterY) {
						// if the touch position is to the left of center
						// move left
						//padreCubi.transform.Translate(speed * -1, Vector3.down.y * moveVertical, 0);
						mainCamera.transform.position += offset2 * -1;
					}
				}
			}
				
			// CODICE UTILE PER TEST CON PC
	        
			float moveHorizontal = Input.GetAxis ("Horizontal");
			
			float moveVertical = Input.GetAxis ("Vertical");
			// movimento verticale costante blocchi
			//padreCubi.transform.Translate(Vector3.down * moveVertical);
			if (moveHorizontal > 0)
				mainCamera.transform.position += offset * 1;
			else if (moveHorizontal < 0)
				mainCamera.transform.position += offset * -1;
			if (moveVertical > 0)
				mainCamera.transform.position += offset2 * 1;
			else if (moveVertical < 0)
				mainCamera.transform.position += offset2 * -1;
			//transform.Translate(Vector3.down * moveVertical);*/
		}
		
		//Rileva box collider toccato e permette di eseguire delle azioni una volta premuto(solo con mouse premuto)
		if (isHeld) {
	        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
	    	RaycastHit2D hitObj = Physics2D.Raycast (ray.origin, ray.direction, Mathf.Infinity);
	        
            if(hitObj.collider != null){
                hit(hitObj.transform.gameObject); 
		    }
		}	
		
		//Comandi mouse per test
    	if (Input.GetMouseButtonDown(0)) {
	        Debug.Log("Pressed left click.");
	     	OnMouseDown(); 
	    }
		if (Input.GetMouseButtonUp(0)) {
	        Debug.Log("Released left click.");
	     	OnMouseUp();  
	    }
		if (Input.GetMouseButtonDown(1)) {
            Debug.Log("Pressed right click.");
        }
        if (Input.GetMouseButtonDown(2)) {
            Debug.Log("Pressed middle click.");
        }
    
    }
  	//rileva se mouse è premuto o rilasciato
  	void OnMouseDown ()
    {      
        isHeld = true;
    }
   
    void OnMouseUp ()
    {
        if (isHeld) {
            isHeld = false;
            Debug.Log("You released the object!");
        }  
    }
 
    void OnMouseExit ()
    {
        if (isHeld) {
            isHeld = false;
            Debug.Log("You released the object!");
        }
    }

  	//Rileva box collider toccato e permette di eseguire delle azioni una volta premuto
  	public void hit(GameObject hitObj){
		// ci sarà da fare un if che controlla l'oggetto che ho toccato
		// di modo che si personalizza l'azione per ogni oggetto toccato
        if (hitObj.tag == "fluxballBonus")
        {
            //lunghezza iniziale del flusso
            lunghezzaFlusso += 0.3f;
            Destroy(hitObj);
            // aumento il punteggio
            _addPoints(10);
        } else if (hitObj.tag == "fluxballMalus")
        {
            //lunghezza iniziale del flusso
            lunghezzaFlusso -= 0.3f;
            Destroy(hitObj);
            // aumento il punteggio
            _addPoints(-10);
        }
	    Debug.Log("Distrutto: "+ hitObj.name);
	    
		refreshGUI();
	}		
		
    // setup all the variables, the UI, and provide errors if things not setup properly.
    void setupDefaults() {
		// if levels not specified, default to current level
		if (levelAfterVictory=="") {
			Debug.LogWarning("levelAfterVictory not specified, defaulted to current level");
			levelAfterVictory = Application.loadedLevelName;
		}
		
		if (levelAfterGameOver=="") {
			Debug.LogWarning("levelAfterGameOver not specified, defaulted to current level");
			levelAfterGameOver = Application.loadedLevelName;
		}

		// friendly error messages
		if (UIScore==null)
			Debug.LogError ("Need to set UIScore on Game Manager.");

        // friendly error messages
        if (UIRemoveScore == null)
            Debug.LogError("Need to set UIRemoveScore on Game Manager.");

        // friendly error messages
        if (UIAddScore == null)
            Debug.LogError("Need to set UIAddScore on Game Manager.");

        //if (UIHighScore==null)
        //	Debug.LogError ("Need to set UIHighScore on Game Manager.");

        if (UIGameOver==null)
			Debug.LogError ("Need to set UIGameOver on Game Manager.");
		
        if (mainCamera==null)
            Debug.LogError("Need to set MainCamera on Game Manager.");
            
        // get stored player prefs
        // refreshPlayerState();

		if (musica) 
		{
			
		}

        // get the UI ready for the game
        refreshGUI();
	}

	// get stored Player Prefs if they exist, otherwise go with defaults set on gameObject
	/*void refreshPlayerState() {
		lives = PlayerPrefManager.GetLives();

		// special case if lives <= 0 then must be testing in editor, so reset the player prefs
		if (lives <= 0) {
			PlayerPrefManager.ResetPlayerState(startLives,false);
			lives = PlayerPrefManager.GetLives();
		}
		score = PlayerPrefManager.GetScore();
		highscore = PlayerPrefManager.GetHighscore();

		// save that this level has been accessed so the MainMenu can enable it
		PlayerPrefManager.UnlockLevel();
	}*/

    // refresh all the GUI elements
    void refreshGUI()
    {
        // set the text elements of the UI
        UIScore.text = score.ToString();
        //UIEnergy.text = "Energy: " + energy.ToString();
        //UIHighScore.text = "Highscore: " + highscore.ToString();
    }

    // funzione chiamabile dall'esterno che aggiunge punti allo score
    public static void addPoints(int am)
    {
        gm._addPoints(am);
    }

    // public function to add points and update the gui
    private void _addPoints(int am)
	{
        if (am > 0)
        {
            // do animation
            Text addScore = Instantiate(UIAddScore);
            addScore.transform.SetParent(canvas.transform);

            // ogni volta che prendo una fluxball la conto
            contFluxball += 1;
        }
        else
        {
            // do animation
            Text removeScore = Instantiate(UIRemoveScore);
            removeScore.transform.SetParent(canvas.transform);
        }

        // increase score
        score += (int)am;

        // devo cambiare colore ogni X fluxball prese
        if (contFluxball == ChangeColorAfterXFluxball)
        {
            changeColor();
            contFluxball = 0;
        }
    }

	// funzione di cambiamento di colore del flusso
	private void changeColor()
	{
        // creo un nuovo colore random
        Color coloreNuovo = Random.ColorHSV(0f, 1f, 1f, 1f, 0.5f, 1f);
        // cambio colore finche non ne trovo uno nuovo
        while (listaColoriUsati.Contains(coloreNuovo))
        {
            coloreNuovo = Random.ColorHSV(0f, 1f, 1f, 1f, 0.5f, 1f);
        }
        // mi salvo i colori già utilizzati
        listaColoriUsati.Add(coloreNuovo);
        // cambio colore al flusso
        particleSystemflusso.startColor = coloreNuovo;
    }

	// public function to remove player life and reset game accordingly
	public void ResetGame() {
        // remove life and update GUI

        score = 0;
		refreshGUI();
        

		// esempio di chiamata ad un metodo dentro il player
	    //_player.GetComponent<CharacterController2D>().Respawn(_spawnLocation);
		
	}

    public static void gameOver()
    {
        gm._gameOver();
    }

    private void _gameOver()
    {
        Debug.Log("GAME OVER");
        UIGameOver.SetActive(true); // this brings up the gameOver 
        Application.LoadLevel(3);
    }

    IEnumerator LoadLevel(string _name, float _delay)
    {
        Debug.Log("Load Level");
        yield return new WaitForSeconds(_delay);
        Application.LoadLevel(3);
    }

	public void LoadLevel(int number)
	{
		Debug.Log("Load Level");
		//yield return new WaitForSeconds(_delay);
		Application.LoadLevel(number);
	}

    // CI POSSONO SERVIRE
    // public function for level complete
    /*public void LevelCompete() {
		
        // use a coroutine to allow the player to get fanfare before moving to next level
        StartCoroutine(LoadNextLevel());
	}

	// load the nextLevel after delay
	IEnumerator LoadNextLevel() {
		yield return new WaitForSeconds(3.5f); 
		Application.LoadLevel (levelAfterVictory);
	}*/

	public void PauseGame() 
	{
		paused = true;
		Time.timeScale = 0;
		pauseButton.SetActive (false);
		pausePanel.SetActive (true);
		EventSystem.current.SetSelectedGameObject (MenuPauseDefaultButton);

		Social.ReportScore(score, "CgkI6Imc5NEGEAIQAA", (bool success) => {
			Debug.Log("REPORT SCORE");
			Debug.Log(success);
		});

		if (pubblicita)
			manageAds (1);
	}

	private void _PauseGame() 
	{
		paused = true;
		Time.timeScale = 0;
		pauseButton.SetActive (false);
		pausePanel.SetActive (true);
		EventSystem.current.SetSelectedGameObject (MenuPauseDefaultButton);

		Social.ReportScore(12345, "CgkI6Imc5NEGEAIQAA", (bool success) => {
			Debug.Log("REPORT SCORE");
			Debug.Log(success);
		});

		if (pubblicita)
			manageAds (1);
	}

	public void ContinueGame() 
	{
		pauseButton.SetActive (true);
		paused = false;
		Time.timeScale = 1;
		pausePanel.SetActive (false);
	}

	/// <summary>
	/// Funzione da chiamare quando si muore
	/// Ferma il gioco e attiva la schermata di GameOver
	/// </summary>

	public void GameOver() 
	{
		gm.Death ();
	}

	private void Death() 
	{
		// disabilito i pannelli per sicurezza
		pausePanel.SetActive (false);
		deathPanel.SetActive (false);

		// metto in pausa il gioco
		paused = true;
		Time.timeScale = 0;
		deathPanel.SetActive (true);
		EventSystem.current.SetSelectedGameObject (MenuDeathDefaultButton);
	}

	public void OneMoreChance()
	{
		// nella one more chance gli faccio vedere un video non skippabile
		ShowRewardedAd ();
		WaitSecond (10.0f);
		paused = false;
		Time.timeScale = 1;
		deathPanel.SetActive (false);
	}

	IEnumerator WaitSecond(float _delay)
	{
		Debug.Log("Wait..");
		yield return new WaitForSeconds(_delay);
	}

	// metodi per cambaire le impostazioni del gioco
	public void changeVibrate()
	{
		// in pratica quando implementeremo la vibrazione se vibrate è a true allora vibra senno no
		if (vibrate)
			vibrate = false;
		else
			vibrate = true;
	}

	public void changeSound()
	{
		if (sound)
			sound = false;
		else
			sound = true;
	}


    /// <summary>
    ///  questi due metodi implementano la pubblicita, chiamare ShowRewardedAd() per visualizzare la pubblicita
    ///  alla fine della pubblicità si vede se l'utente ha guardato tutto oppure se l'ha skippato
    ///  implementare ricompense
    /// </summary>

	public void ShowSimpleAd()
	{
		if (Advertisement.IsReady ()) {
			Advertisement.Show ("video");
		}
	}

    public void ShowRewardedAd()
    {
        
        if (Advertisement.IsReady("rewardedVideo"))
        {
            var options = new ShowOptions { resultCallback = HandleShowResult };
            Advertisement.Show("rewardedVideo", options);
        }
    }

    private void HandleShowResult(ShowResult result)
    {
        switch (result)
        {
		case ShowResult.Finished:
			Debug.Log ("The ad was successfully shown.");
                //
                // YOUR CODE TO REWARD THE GAMER
                // Give coins etc.
			deathPanel.SetActive (false);
			paused = false;
            break;
		case ShowResult.Skipped:
			Debug.Log ("The ad was skipped before reaching the end.");
			LoadLevel ("MainMenu", 2.0f);
            break;
        case ShowResult.Failed:
            Debug.LogError("The ad failed to be shown.");
			LoadLevel ("MainMenu", 2.0f);
            break;
        }
    }

	private void manageAds(int numAds=0) {
		///numAds = 0: Carica i video
		///numAds = 1: Interstitial
		///numAds = 2: rewarded
		if (Chartboost.hasInterstitial (CBLocation.Default) == true && numAds == 1) {
			Chartboost.showInterstitial (CBLocation.Default);
		} else {
			Debug.LogError ("NO INTERSTITIAL");
			Chartboost.cacheInterstitial(CBLocation.Default);
		}
		if (Chartboost.hasRewardedVideo (CBLocation.Default) == true && numAds == 2) {
			Chartboost.showRewardedVideo (CBLocation.Default);
		} else {
			Debug.LogError ("NO REWARDED VIDEO");
			Chartboost.cacheRewardedVideo(CBLocation.Default);
		}
	}
}
