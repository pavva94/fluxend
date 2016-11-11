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

public class GameManager : MonoBehaviour {

	// static reference to game manager so can be called from other scripts directly (not just through gameobject component)
	public static GameManager gm;

	// levels to move to on victory and lose
	public string levelAfterVictory;
	public string levelAfterGameOver;

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

    // UI elements to control
    public Text UIScore;
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
    public int moveOk;
    //disabilita flusso che si sovrappone
    public int moveNotOk = 0;
    //memorizza l'ultimo movimento fatto dal flusso
    public int lastmoveOk = 0;

    // main camera
    public GameObject mainCamera;

    //flusso
    public GameObject flusso;

    // di quanto la camera di sposta
    public Vector3 offset = new Vector3(0.1f,0);
    public Vector3 offset2 = new Vector3(0.0f,0.1f);
    
	// pubblicita si/no
    [SerializeField]public bool pubblicita = true;

	// pannello di pausa
	public GameObject pausePanel;

	// pannello di morte
	public GameObject deathPanel;

	// Button di pausa
	public GameObject MenuPauseDefaultButton;
	public GameObject MenuDeathDefaultButton;

	// game in pausa
	bool paused = false;

	// setting button
	private bool vibrate = true;
	private bool sound = true;

	// la musica di sottofondo (AudioListener)
	public AudioSource musica;

    //public static Vector2 bottomCorner;
    //public static Vector2 topCorner;
    //public float moveVertical;
    // lista ostacoli da generare
    //public GameObject[] prefabs;
    //private string[] prefabs_loaded;

    // game performance
    //public int highscore = 0;

    // private variables
    //GameObject _player;
    //Vector3 _startLocation;

    // Gameobject padre dei cubi
    //public GameObject padreCubi;

    // Gameobject to load
    //private string prefab_to_load;

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

        // Avvia funzione per inizializzare movimento flusso
        InvokeRepeating("moveOn", 1, 1.0f);

       
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

        // spawn dei blocchi

        /*InvokeRepeating("randomspawn", 1, 1);

        // bottom and top corner of screen
        float camDistance = Vector3.Distance(transform.position, Camera.main.transform.position);
        Vector2 bottomCorner = Camera.main.ViewportToWorldPoint(new Vector3(0, 0, camDistance));
        Vector2 topCorner = Camera.main.ViewportToWorldPoint(new Vector3(1, 1, camDistance));*/
        // InvokeRepeating("randomspawn", 1, 0.3f);
    }
        //abilita movimento flusso
    void moveOn()
        {
            moveOk = Random.Range(1,9);
            moveNotOk = 0;
        }   
        //disabilita movimento flusso
    void moveOff()
        {
            moveOk = 0;
        }
    // game loop
    void Update() {

		// se sono in pausa non faccio nulla
		if (!paused) {

			Social.ReportProgress("CgkI6Imc5NEGEAIQAg", 100.0f, (bool success) => {
				// handle success or failure
			});

			//----INIZIO codice per automizzare nelle diverse direzioni l'andamento del flusso
			if (moveNotOk == 1) {
				moveOn ();
			}
			Debug.Log (moveOk);
			if (moveOk == 1 & lastmoveOk != 4) {
				flusso.transform.Translate (Vector3.right * Time.deltaTime * Random.Range (3, 10), Space.World);
				flusso.transform.Translate (Vector3.up * Time.deltaTime * Random.Range (3, 10), Space.World);
				flusso.GetComponent<ParticleSystem> ().enableEmission = true;
				flusso.GetComponent<ParticleSystem> ().Play ();
				Debug.Log (moveNotOk);
				lastmoveOk = moveOk; 
			} else if (moveOk == 1 & lastmoveOk == 4) {
				moveNotOk = 1;
			}
			if (moveOk == 2 & lastmoveOk != 3) {
				flusso.transform.Translate (Vector3.left * Time.deltaTime * Random.Range (3, 10), Space.World);
				flusso.transform.Translate (Vector3.up * Time.deltaTime * Random.Range (3, 10), Space.World);
				flusso.GetComponent<ParticleSystem> ().enableEmission = true;
				flusso.GetComponent<ParticleSystem> ().Play ();
				Debug.Log (moveNotOk);
				lastmoveOk = moveOk; 
			} else if (moveOk == 2 & lastmoveOk == 3) {
				moveNotOk = 1;
			}
	        
			if (moveOk == 3 & lastmoveOk != 2) {
				flusso.transform.Translate (Vector3.right * Time.deltaTime * Random.Range (3, 10), Space.World);
				flusso.transform.Translate (Vector3.down * Time.deltaTime * Random.Range (3, 10), Space.World);
				flusso.GetComponent<ParticleSystem> ().enableEmission = true;
				flusso.GetComponent<ParticleSystem> ().Play ();
				Debug.Log (moveNotOk);
				lastmoveOk = moveOk; 
			} else if (moveOk == 3 & lastmoveOk == 2) {
				moveNotOk = 1;
			}

			if (moveOk == 4 & lastmoveOk != 1) {
				flusso.transform.Translate (Vector3.left * Time.deltaTime * Random.Range (3, 10), Space.World);
				flusso.transform.Translate (Vector3.down * Time.deltaTime * Random.Range (3, 10), Space.World);
				flusso.GetComponent<ParticleSystem> ().enableEmission = true;
				flusso.GetComponent<ParticleSystem> ().Play ();
				Debug.Log (moveNotOk);
				lastmoveOk = moveOk; 
			} else if (moveOk == 4 & lastmoveOk == 1) {
				moveNotOk = 1;
			}
			if (moveOk == 5 & lastmoveOk != 6) {
				flusso.transform.Translate (Vector3.right * Time.deltaTime * Random.Range (3, 10), Space.World);
				flusso.GetComponent<ParticleSystem> ().enableEmission = true;
				flusso.GetComponent<ParticleSystem> ().Play ();
				Debug.Log (moveNotOk);
				lastmoveOk = moveOk; 
			} else if (moveOk == 5 & lastmoveOk == 6) {    
				moveNotOk = 1;
			}
			if (moveOk == 6 & lastmoveOk != 5) {
				flusso.transform.Translate (Vector3.left * Time.deltaTime * Random.Range (3, 10), Space.World);
				flusso.GetComponent<ParticleSystem> ().enableEmission = true;
				flusso.GetComponent<ParticleSystem> ().Play ();
				Debug.Log (moveNotOk);
				lastmoveOk = moveOk; 
			} else if (moveOk == 6 & lastmoveOk == 5) {
				moveNotOk = 1;
			}
			if (moveOk == 7 & lastmoveOk != 8) {
				flusso.transform.Translate (Vector3.up * Time.deltaTime * Random.Range (3, 10), Space.World);
				flusso.GetComponent<ParticleSystem> ().enableEmission = true;
				flusso.GetComponent<ParticleSystem> ().Play ();
				Debug.Log (moveNotOk);
				lastmoveOk = moveOk; 
			} else if (moveOk == 7 & lastmoveOk == 8) {
				moveNotOk = 1;
			}
			if (moveOk == 8 & lastmoveOk != 7) {
				flusso.transform.Translate (Vector3.down * Time.deltaTime * Random.Range (3, 10), Space.World);
				flusso.GetComponent<ParticleSystem> ().enableEmission = true;
				flusso.GetComponent<ParticleSystem> ().Play ();
				Debug.Log (moveNotOk);
				lastmoveOk = moveOk;       
			} else if (moveOk == 8 & lastmoveOk == 7) {
				moveNotOk = 1;
			}
			if (moveOk == 0) {

				flusso.GetComponent<ParticleSystem> ().enableEmission = false;
				flusso.GetComponent<ParticleSystem> ().Stop ();

			}
			//----FINE CODICE AUTOMATIZZAZIONE MOVIMENTI FLUSSO 

			// if ESC pressed then pause the game
			/*if (Input.GetKeyDown(KeyCode.Escape)) {
				if (Time.timeScale > 0f) {
					UIGameOver.SetActive(true); // this brings up the pause UI
					Time.timeScale = 0f; // this pauses the game action
				} else {
					Time.timeScale = 1f; // this unpauses the game action (ie. back to normal)
					UIGameOver.SetActive(false); // remove the pause UI
				}
			}*/
			//Incrementa timer ogni secondo
			Timer += Time.deltaTime;

			// un po di pubblicita iniziale non fa male
			// ShowRewardedAd();
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

			/* GameObject[] cubi = GameObject.FindGameObjectsWithTag("Blocks");
	         foreach (GameObject cubo in cubi)
	         {
	             Transform transform = cubo.transform;
	             Rigidbody2D rigidboby_cubo = cubo.GetComponent<Rigidbody2D>();
	             // movimento utente blocchi
	             //transform.Translate(0, Vector3.down.y * moveVertical, 0);
	             Debug.Log(transform);
	             transform.Translate(Vector3.down * moveVertical);
	             Vector2 movement = new Vector2(0.0f, moveVertical);
	             rigidboby_cubo.velocity = movement * speed;
	             // if there are any touches currently
	             if (tocchi > 0)
	             {
	                 // get the first one
	                 Touch firstTouch = Input.GetTouch(0);
	                 // if it began this frame
	                 if (firstTouch.phase == TouchPhase.Moved || firstTouch.phase == TouchPhase.Stationary)
	                 {
	                     if (firstTouch.position.x > screenCenterX)
	                     {
	                         // if the touch position is to the right of center
	                         // move right
	                         transform.Translate(speed, Vector3.down.y * moveVertical, 0);
	                     }
	                     else if (firstTouch.position.x < screenCenterX)
	                     {
	                         // if the touch position is to the left of center
	                         // move left
	                         transform.Translate(-speed, Vector3.down.y * moveVertical, 0);
	                     }
	                 }
	             }
	             transform.Translate(Vector3.down * moveVertical);
	         }*/

			// CODICE UTILE PER TEST CON PC
	        
			float moveHorizontal = Input.GetAxis ("Horizontal");
			Debug.Log (moveHorizontal);
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


			_addPoints ((int)Time.timeSinceLevelLoad);
		}

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
        //UIScore.text = "Score: " + score.ToString();
        //UIEnergy.text = "Energy: " + energy.ToString();
        //UIHighScore.text = "Highscore: " + highscore.ToString();
    }

    // funzione chiamabile dall'esterno che aggiunge punti allo score
    public static void addPoints(float am)
    {
        gm._addPoints(am);
    }

    // public function to add points and update the gui and highscore player prefs accordingly
    private void _addPoints(float am)
	{
		// increase score
		score+= (int)am;

		// update UI
		//UIScore.text = "Score: "+score.ToString();

		// if score>highscore then update the highscore UI too
		/*if (score>highscore) {
			highscore = score;
			UIHighScore.text = "Highscore: "+score.ToString();
		}*/
	}

	// public function to remove player life and reset game accordingly
	public void ResetGame() {
        // remove life and update GUI

        score = 0;
        energy = startEnergy;
		refreshGUI();
        

		// esempio di chiamata ad un metodo dentro il player
	    //_player.GetComponent<CharacterController2D>().Respawn(_spawnLocation);
		
	}

    // funzione chiamabile dall'esterno che rimuove energia
    public static void removeEnergy()
    {
        gm._removeEnergy();
    }
    
    private void _removeEnergy()
    {
        if (!debug)
        {
            // decrese score
            energy -= amount;
            Debug.Log("energy");
            Debug.Log(energy);
            // update UI
            UIEnergy.text = "Energy: " + energy.ToString();
        }
        // if score>highscore then update the highscore UI too
        /*if (score > highscore)
        {
            highscore = score;
            UIHighScore.text = "Highscore: " + score.ToString();
        }*/
    }

    public static void playerCollision()
    {
        gm._playerCollision();
    }

    private void _playerCollision()
    {
        Debug.Log("ENERGY: " + energy.ToString());
        Debug.Log("AMOUNT: " + amount.ToString());
        _removeEnergy();
        if (energy <= 0)
        {
            _gameOver();
        }
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
		pausePanel.SetActive (true);
		EventSystem.current.SetSelectedGameObject (MenuPauseDefaultButton);

		Social.ReportScore(12345, "CgkI6Imc5NEGEAIQAA", (bool success) => {
			// handle success or failure
		});

		ShowSimpleAd ();
	}

	private void _PauseGame() 
	{
		paused = true;
		Time.timeScale = 0;
		pausePanel.SetActive (true);
		EventSystem.current.SetSelectedGameObject (MenuPauseDefaultButton);

		Social.ReportScore(12345, "CgkI6Imc5NEGEAIQAA", (bool success) => {
			// handle success or failure
		});

		ShowSimpleAd ();
	}

	public void ContinueGame() 
	{
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
		if (sound) {
			sound = false;

		}
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
}
