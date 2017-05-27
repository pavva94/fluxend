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
using UnityEngine.SceneManagement;
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
    int timeBallActive = 0;
    public bool debug = false;

    // game performance
    public int score = 0;
    public int startEnergy = 50;
    public int energy;
    public int amount = 10;
    private int highscore;
    private int initialHighscore;
    public float zoomSize = 0.1f; 
    int zoomOk = 0;
    public Canvas canvas;
    // UI elements to control
    public Text UIScore;
    public Text UIHighscore;
	public Text UIAddScore;
    public Text UIRemoveScore;
    public Text UILevel;
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
    //serve per il cambio di colore delle fluxball
    int sorteOn = 0;
    //serve per il cambio di colore delle fluxball
    private int contaCiclo = 0;
    //posizioni iniziali fluxball
    Vector3 fluxballsys = new Vector3(0,0,0);
	Vector3 fluxballsys2 = new Vector3(0,0,0);
    // main camera
    public GameObject mainCamera;

    //flusso
    public GameObject flusso;
    // particle system del flusso
    private ParticleSystem particleSystemflusso;
    // bagliore del flusso
    private SpriteRenderer baglioreflusso;
    //variabile per velocità flusso
    public float velflux = 1.0f; 
    //variabile per velocità timeball
    public float veltime = 0.6f;
    //sfere da catturare
    public GameObject fluxballBonus;
    public GameObject fluxballMalus;
    public GameObject timeBall;

    private GameObject fluxballAssoluta;
    //esplosione
    public GameObject esplosione;
    //esplosione 2
    public GameObject esplosione2;
	// esplosione timeball
	public GameObject timeballExplosion;

    //regolo luminosità gioco
//    public GameObject lumen;
    //bagliore della punta del flusso
    public GameObject bagliore;

    // di quanto la camera di sposta
    public Vector3 offset = new Vector3(0.1f,0);
    public Vector3 offset2 = new Vector3(0.0f,0.1f);
    
    //rielva se il touch è premuto o no
    int touchPressed = 0;
    
    float startPosx; 
    float startPosy; 
	// pubblicita si/no
    [SerializeField]
	public bool pubblicita = true;

	// pannello di pausa
	public GameObject pausePanel;

	// pannello di morte
	public GameObject deathPanel;

    // pannello di istruzioni
    public GameObject instructionPanel;
    public GameObject instructionLevelPanel;

    // pannello di livello successivo
    public GameObject nextLevelPanel;

    // Button di pausa
    public GameObject MenuPauseDefaultButton;
	public GameObject MenuDeathDefaultButton;
    public GameObject MenuNextLevelDefaultButton;

    // Button di Pausa
    public GameObject pauseButton;

	// game in pausa
	bool paused = false;

	// setting button
	private bool vibrate = true;
	private bool sound = true;

    // la musica di sottofondo (AudioSource)
    private AudioSource musica;

    // lista colori già usati dal flusso
    private List<Color> listaColoriUsati = new List<Color>(100);

    // contatore fluxball prese
    private int contFluxball = 0;
    public int ChangeColorAfterXFluxball = 10;

    private float timeFuoriSchermo;
    private float timerDeath = 0f;
    private bool isDeath = false;
    // instanza dell'immagine di morte
    private GameObject deathImage;
    //serve per zoom su flusso
    public float value = 0.1f; //1 by default in inspector
    // variabili di gioco
    private int doMusic;
    private int doVibrate;

    // timer di gioco
    private float playTime;

    private int firstTime;
    private int firstLevelTime;
    // button di play dopo le istruzioni
    public GameObject TouchForPlay;

    //gameobject della timeball
    GameObject timeBallin;

	// sound fluxball e timeball
	public AudioSource timeballSound;
	private AudioSource audioTimeball;
	public AudioSource fluxballSound;
	private AudioSource audioFluxball;

    // livello corrente
    int livello;

	// id leaderboard
	string id_leaderboard = "CgkI6Imc5NEGEAIQBw";

    // set things up here
    void Awake () {
		

		// setup reference to game manager
		if (gm == null)
			gm = this.GetComponent<GameManager>();

		// setup all the variables, the UI, and provide errors if things not setup properly.
		setupDefaults();

        if (Advertisement.isSupported)
        {
            Advertisement.Initialize ("1267892", false);
        }
        else
        {
            Debug.Log("Platform not supported");
        }

    }

    // Use this for initialization
    void Start()
    {
        // prendo il livello corrente
        livello = PlayerPrefs.GetInt("livello", 0);
        paused = false;
    	if(livello == 0) {
	        //parte esplosione 
	    	esplosione.GetComponent<ParticleSystem> ().enableEmission = true;
			esplosione.GetComponent<ParticleSystem> ().Play ();
			//parte esplosione 2 
	    	esplosione2.GetComponent<ParticleSystem> ().enableEmission = true;
			esplosione2.GetComponent<ParticleSystem> ().Play ();
        
        }
        // Avvia funzione per inizializzare movimento flusso
        InvokeRepeating("moveOn", 2, 1.0f);
        //Esegue funzione spawn sfere di flusso Bonus

        InvokeRepeating("spawnFluxballSorte", 3f, 2.5f);
        //Esegue funzione spawn sfere di flusso Bonus
        InvokeRepeating("spawnFluxballSorte2", 3f, 2.5f);
        //Esegue funzione spawn timeball
        InvokeRepeating("spawnTimeBall", 5f, 5f);
		//Esegue funzione conteggioTime
        InvokeRepeating("conteggioTime", 2f, 1f);

        Invoke("zoomActive", 3f);

        //Esegue funzione conteggioTime
        InvokeRepeating("sottraiLunghezza", 5f, 0.2f);

        //UIGameOver.SetActive(false); // disattiva il text gameOver

        // rigidbody = GetComponent<Rigidbody2D>();
        // Disable screen dimming
        Screen.sleepTimeout = SleepTimeout.NeverSleep;
        screenCenterX = Screen.width * 0.5f;
        screenCenterY = Screen.height * 0.5f;

        energy = startEnergy;

        Time.timeScale = 1;

        // timer di gioco
        playTime = Time.time;

        // prendo l'highscore
        highscore = PlayerPrefs.GetInt("highscore", 0);
        initialHighscore = highscore;

        refreshGUI();

        // prendo il particle system del flusso, deve sempre esserci un figlio con il ParticleSystem
        particleSystemflusso = flusso.GetComponentsInChildren(typeof(ParticleSystem))[1] as ParticleSystem;
        
		// prendo il bagliore del flusso, deve sempre esserci
		baglioreflusso = bagliore.GetComponent(typeof(SpriteRenderer)) as SpriteRenderer;

        // Carico gli Ads di ChartBoost 
        manageAds();

        // prendo il livello corrente
        livello = PlayerPrefs.GetInt("livello", 0);
        // imposto il gioco in base alla modalità di gioco
        // imposto l'header

        
        // setto il text di livello
        if (livello != 0)
        {
            UILevel.text = "Level " + livello;
            // se siamo nei livelli imposto la lunghezza del flusso in base a livello corrente
            lunghezzaFlusso = 10 * livello;
            UIHighscore.enabled = false;
            UIScore.enabled = false;
			//setta zoom iniziale
			Camera.main.orthographicSize = Camera.main.orthographicSize +10f;
            
        }
        else {
            UILevel.text = "Endless";
            lunghezzaFlusso = 10;
        }

        // se è la prima volta che gioca all'ENDLESSfaccio partire le istruzioni
        // dopo X secondi di gioco faccio partire le istruzioni
        firstTime = PlayerPrefs.GetInt("firstTime", 1);
		if (firstTime == 1 && livello == 0) {
			Debug.Log (PlayerPrefs.GetInt ("tutorial", 1));
			if (PlayerPrefs.GetInt ("tutorial", 1) == 1) {
				PlayerPrefs.SetInt ("tutorial", 0);
				Application.LoadLevel (3);
			} else {
				InstructionPause ();
			}
		}

        // se è la prima volta che gioca ai LIVELLI faccio partire le istruzioni
        // dopo X secondi di gioco faccio partire le istruzioni
        firstLevelTime = PlayerPrefs.GetInt("firstLevelTime", 1);
        if (firstLevelTime == 1 && livello != 0)
            InstructionLevelPause();

		// prendo l'highscore dai play service
		// se è minore di quello che ho salvato in locale allora aggiorno i play service
		if (Social.localUser.authenticated) {
			int highscoreGoogle = 0;
			PlayGamesPlatform.Instance.LoadScores (
				id_leaderboard,
				LeaderboardStart.PlayerCentered,
				1,
				LeaderboardCollection.Public,
				LeaderboardTimeSpan.AllTime,
				(LeaderboardScoreData data) => {
					Debug.Log (data.Valid);
					Debug.Log (data.Id);
					Debug.Log (data.PlayerScore);
					Debug.Log (data.PlayerScore.userID);
					highscoreGoogle = int.Parse(data.PlayerScore.formattedValue);
				});
			
			if (initialHighscore > highscoreGoogle) {
				Social.ReportScore (initialHighscore, id_leaderboard, (bool success) => {

				});
			}
		}

		if (PlayerPrefs.GetInt ("numGames", 0) == 5) {
			// guadagni il trofeo per aver giocato tot volte
			Social.ReportProgress("CgkI6Imc5NEGEAIQAw", 100.0f, (bool success) => {
				// handle success or failure
			});
		}

		if (PlayerPrefs.GetInt ("numGames", 0) == 15) {
			// guadagni il trofeo per aver giocato tante volte
			Social.ReportProgress("CgkI6Imc5NEGEAIQBA", 100.0f, (bool success) => {
				// handle success or failure
			});
		}

    }
    
 	void zoomActive(){

		if (Camera.main.orthographicSize > 5) {
			mainCamera.transform.position = new Vector3 (flusso.transform.position.x, flusso.transform.position.y, -1);
	       
			Camera.main.orthographicSize = Camera.main.orthographicSize - zoomSize;
			Invoke ("zoomActive", 0.1f);
		} else {
			CancelInvoke ("zoomActive");
		}
	} 

    //abilita movimento flusso
    void moveOn()
    {
	    //Serve per dare la punta di bagliore blu al flusso
	    moveOk = Random.Range(1,9);
	    moveNotOk = 0;
        // prendo il bagliore del flusso, deve sempre esserci
        baglioreflusso = bagliore.GetComponent(typeof(SpriteRenderer)) as SpriteRenderer;

        // assegno il bagliore
        bagliore.GetComponent<SpriteRenderer>().sprite = Resources.Load("bagliorepunta", typeof(Sprite)) as Sprite;
        zoomOk = 1;
    }   
    
    //disabilita movimento flusso
    void moveOff()
    {
        moveOk = 0;
    }
    
    

    //Funzione per spawn delle sfere di flusso random nello schermo
    void spawnFluxballSorte() 
    {
		if (!isDeath || !paused) {
			int spawnSorte = Random.Range (1, 3);
        
			if (contaCiclo <= 1) {
				fluxballAssoluta = fluxballBonus;
			} else if (contaCiclo > 1) {
				fluxballAssoluta = fluxballMalus;
			}               
        
           

			GameObject cloneFluxball = (GameObject)Instantiate (fluxballAssoluta, genpos (), Quaternion.identity);
			//Distrugge dopo un valore impostato l'oggetto istanziato
			Destroy (cloneFluxball, 2.49f);                         
			contaCiclo += 1;
		}
    }


    Vector3 genpos() { 
        if (sorteOn == 0 | contaCiclo == 1) {
            fluxballsys = new Vector3(Random.Range(flusso.transform.position.x - 5, flusso.transform.position.x + 5),Random.Range(flusso.transform.position.y - 5,flusso.transform.position.y + 5), 10);
            fluxballsys2 = fluxballsys;
            sorteOn = 1;
        }
        if (sorteOn == 1 & contaCiclo > 1) {
            sorteOn = 0;
            contaCiclo = 0;
        }
        return fluxballsys2;

    }
	
//Funzione per spawn delle sfere di flusso random nello schermo
    void spawnFluxballSorte2() 
    {
		if (!isDeath || !paused) {
			int spawnSorte = Random.Range (1, 3);
        
			if (contaCiclo <= 1) {
				fluxballAssoluta = fluxballBonus;
			} else if (contaCiclo > 1) {
				fluxballAssoluta = fluxballMalus;
			}  

			GameObject cloneFluxball = (GameObject)Instantiate (fluxballAssoluta, genpos2 (), Quaternion.identity);
			//Distrugge dopo un valore impostato l'oggetto istanziato
			Destroy (cloneFluxball, 2.49f);                         
			contaCiclo += 1;
		}
    }


    Vector3 genpos2() { 

        if (sorteOn == 0 | contaCiclo == 1) {
			fluxballsys = new Vector3(Random.Range(flusso.transform.position.x - 5, flusso.transform.position.x + 5),Random.Range(flusso.transform.position.y - 5,flusso.transform.position.y + 5), 10);
            fluxballsys2 = fluxballsys;
            sorteOn = 1;
        }
        if (sorteOn == 1 & contaCiclo > 1) {
            sorteOn = 0;
            contaCiclo = 0;
        }
        return fluxballsys2;

    }

	void spawnTimeBall() 
	    {
	        timeBallin = (GameObject)Instantiate (timeBall,genposTimeball(),Quaternion.Euler(new Vector3(-90, -90, 0)));
			timeBallin.GetComponent<ParticleSystem> ().enableEmission = true;
			timeBallin.GetComponent<ParticleSystem> ().Play ();  
			timeBallActive = 1;
	        Destroy (timeBallin, 4.99f);
	    }


 	Vector3 genposTimeball() {
            float x,y,z;
            x = Random.Range(flusso.transform.position.x - 3, flusso.transform.position.x + 3);
            y = flusso.transform.position.y + 5;
            z = 0;
            return new Vector3(x,y,z);

    }

    void sottraiLunghezza() {
		if (!isDeath || !paused) {
			if (livello != 0)
				lunghezzaFlusso += 0.1f;
			else 
            //sottrae lunghezza flusso in base al tempo impostato nell'Invoke
		    lunghezzaFlusso -= 0.2f;  
		}
    }
    void conteggioTime() {
		if (!isDeath || !paused) {
			//velocità incrementale flusso (in base al tempo)
			//Incrementa timer ogni secondo
			if (velflux < 2.5f)
				velflux += 0.005f;
			else if (velflux < 3)
				velflux += 0.0005f;
			else if (velflux < 5)
				velflux += 0.000005f;
			// livello != 0
			if (false)
				gradovel *= livello;
			//incrementa movimento camera ogni secondo
			offset += new Vector3 (0.00015f, 0.000015f);
			offset2 += new Vector3 (0.00015f, 0.0015f);
			Timer = 0.0f;
		}

	}
    // game loop
    void Update() {

         playTime += Time.deltaTime;
        if (playTime > 3.0f & firstTime == 1)
        {
            TouchForPlay.SetActive(true);
            PlayerPrefs.SetInt("firstTime", 0);
            firstTime = 0;
        }

        refreshGUI();
        // se sono in pausa non faccio nulla
        if (!paused) {
			
			//velocità incrementale flusso (in base al tempo)
			//Incrementa timer ogni secondo
			Timer += Time.deltaTime;

            // quando il flusso va fuori dallo schermo mi salvo il tempo
            // mi servirà per capire quanto il flusso sta fuori e per l'eventuale morte
            if (!baglioreflusso.GetComponent<Renderer>().isVisible && timeFuoriSchermo == 0)
            { 
                timeFuoriSchermo = Time.time;
            }
            else if (baglioreflusso.GetComponent<Renderer>().isVisible && timeFuoriSchermo != 0)
            { 
                timeFuoriSchermo = 0f;
            }
			
			//assegna color per alpha luminosità
//        	Color color = lumen.GetComponent<Renderer>().material.color;
//			
//			//cambia luminosità in base a distanza flusso
//			if (mainCamera.transform.position.x < flusso.transform.position.x +2.3 | mainCamera.transform.position.x > flusso.transform.position.x -2.3 | mainCamera.transform.position.y < flusso.transform.position.y +3 | mainCamera.transform.position.y > flusso.transform.position.y -3 ) {
//				//lumen.GetComponent<SpriteRenderer> ().sprite = Resources.Load("lumen001", typeof(Sprite)) as Sprite;
//	 			color.a = 0.0f;
//	 			lumen.GetComponent<Renderer>().material.SetColor("_Color", color);
//			}
//			if (mainCamera.transform.position.x > flusso.transform.position.x +2.3 | mainCamera.transform.position.x < flusso.transform.position.x -2.3 | mainCamera.transform.position.y > flusso.transform.position.y +3 | mainCamera.transform.position.y < flusso.transform.position.y -3 ) {
//				//lumen.GetComponent<SpriteRenderer> ().sprite = Resources.Load("lumen001", typeof(Sprite)) as Sprite;
//	 			color.a = 0.3f;
//	 			lumen.GetComponent<Renderer>().material.SetColor("_Color", color);
//			}
//			if (mainCamera.transform.position.x > flusso.transform.position.x +2.6 | mainCamera.transform.position.x < flusso.transform.position.x -2.6 | mainCamera.transform.position.y > flusso.transform.position.y +3.5 | mainCamera.transform.position.y < flusso.transform.position.y -3.5 ) {
//				//lumen.GetComponent<SpriteRenderer> ().sprite = Resources.Load("lumen01", typeof(Sprite)) as Sprite;
//	 			color.a = 0.5f;
//	 			lumen.GetComponent<Renderer>().material.SetColor("_Color", color);
//			}
//			if (mainCamera.transform.position.x > flusso.transform.position.x +3 | mainCamera.transform.position.x < flusso.transform.position.x -3 | mainCamera.transform.position.y > flusso.transform.position.y +4 | mainCamera.transform.position.y < flusso.transform.position.y -4 ) {
//				//lumen.GetComponent<SpriteRenderer> ().sprite = Resources.Load("lumen1", typeof(Sprite)) as Sprite;
//	 			color.a = 0.7f;
//	 			lumen.GetComponent<Renderer>().material.SetColor("_Color", color);
//			}
//			if (mainCamera.transform.position.x > flusso.transform.position.x +3.5 | mainCamera.transform.position.x < flusso.transform.position.x -3.5 | mainCamera.transform.position.y > flusso.transform.position.y +5 | mainCamera.transform.position.y < flusso.transform.position.y -5 ) {
//				//lumen.GetComponent<SpriteRenderer> ().sprite = Resources.Load("lumen2", typeof(Sprite)) as Sprite;
//	 			color.a = 0.9f;
//	 			lumen.GetComponent<Renderer>().material.SetColor("_Color", color);
//			}
			

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

            if (moveOk != 0)
            {

            }
			//----FINE CODICE AUTOMATIZZAZIONE MOVIMENTI FLUSSO 
            //AUTOMATIZZA DISCESA TIMEBALL
			if (timeBallActive == 1 && timeBallin != null) {
	            timeBallin.transform.Translate (Vector3.down * Time.deltaTime * veltime, Space.World);
            } 
			//Incrementa timer ogni secondo
			Timer += Time.deltaTime;

			int tocchi = Input.touchCount;
            float moltiplicatoreVelTouch = 0.06f;

            if (tocchi > 0) {
				// get the first one
				Touch touch = Input.GetTouch (0);
				
				if (touchPressed == 1) {
                    
                    if (touch.position.x < startPosx) {
					    mainCamera.transform.Translate(Vector3.left * moltiplicatoreVelTouch * velflux, Space.World);
					}
					if (touch.position.x > startPosx) {
					    mainCamera.transform.Translate(Vector3.right * moltiplicatoreVelTouch * velflux, Space.World);
					}
					if (touch.position.y < startPosy) {
					    mainCamera.transform.Translate(Vector3.down * moltiplicatoreVelTouch * velflux, Space.World);
					}
					if (touch.position.y > startPosy) {
					    mainCamera.transform.Translate(Vector3.up * moltiplicatoreVelTouch * velflux, Space.World);
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
				// if it began this frame
				if (touch.phase == TouchPhase.Moved || touch.phase == TouchPhase.Stationary || touch.phase == TouchPhase.Began || touch.phase == TouchPhase.Ended) {
					RaycastHit2D hitObj = Physics2D.Raycast (Camera.main.ScreenToWorldPoint((Input.GetTouch (0).position)), Vector2.zero);
					if(hitObj.collider != null){
		                hit(hitObj.transform.gameObject);   
				    }

					
				}

			}

            // se una condizione di morte è vera e non sono già morto allora visualizzo il teschio 
            if (checkDeath() & !isDeath & livello == 0)
            {
                // animazione teschio
                deathImage = Instantiate(UIGameOver);
                deathImage.transform.SetParent(canvas.transform);
                timerDeath = Time.time;
                isDeath = true;
            }
                

            // se sono morto e sono passati tot secondi da quando lo sono allora visualizzo la schermata di morte
            if (isDeath && Time.time - timerDeath > 1f)
                Death();
           
            // se sono in un livello controllo se è finito il livello o no
            if (livello != 0)
                checkEndLevel();
            

			// CODICE UTILE PER TEST CON PC
	        
			float moveHorizontal = Input.GetAxis ("Horizontal");
			
			float moveVertical = Input.GetAxis ("Vertical");
			// movimento verticale costante blocchi
			//padreCubi.transform.Translate(Vector3.down * moveVertical);
			if (moveHorizontal > 0)
				mainCamera.transform.Translate(offset * 1 * velflux);
			else if (moveHorizontal < 0)
				mainCamera.transform.Translate(offset * -1 * velflux);
			if (moveVertical > 0)
				mainCamera.transform.Translate(offset2 * 1 * velflux);
			else if (moveVertical < 0)
				mainCamera.transform.Translate( offset2 * -1 * velflux);
			//transform.Translate(Vector3.down * moveVertical);*/


            //Rileva box collider toccato e permette di eseguire delle azioni una volta premuto(solo con mouse premuto)
		    if (isHeld) {
	            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
	    	    RaycastHit2D hitObj = Physics2D.Raycast (ray.origin, ray.direction, Mathf.Infinity);
	        
                if(hitObj.collider != null){
                    hit(hitObj.transform.gameObject); 
		        }
		    }	
		}
		
		
		
		//Comandi mouse per test
    	if (Input.GetMouseButtonDown(0)) {
	        
	     	OnMouseDown(); 
	    }
		if (Input.GetMouseButtonUp(0)) {
	       
	     	OnMouseUp();  
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
           
        }  
    }
 
    void OnMouseExit ()
    {
        if (isHeld) {
            isHeld = false;
           
        }
    }

  	//Rileva box collider toccato e permette di eseguire delle azioni una volta premuto
  	public void hit(GameObject hitObj){
        // vibrazione ogni volta che prendo qualcosa
        if (vibrate)
        {
            Handheld.Vibrate();
        }
        // controllo che fluxball ho toccato
        if (hitObj.tag == "fluxballBonus")
        {
            // in base alla modalità di gioco decido quanto aggiungere
            if (livello != 0)
                lunghezzaFlusso += 2.5f * livello;
            else
                lunghezzaFlusso += 5f;
            Destroy(hitObj);
            //rileva oggetto pressato per limitare spawn di altre fluxball nella stessa posizione
            contaCiclo = 0;
            sorteOn = 0;
            // aumento il punteggio
            _addPoints(10);
			// suono fluxball
			audioFluxball.Play();

        } else if (hitObj.tag == "fluxballMalus")
        {
            // in base alla modalità di gioco decido quanto togliere
            if (livello != 0)
                lunghezzaFlusso -= 5.0f * livello;
            else
                lunghezzaFlusso -= 5f;
            
            Destroy(hitObj);
            //rileva oggetto pressato per limitare spawn di altre fluxball nella stessa posizione
            contaCiclo = 0;
            sorteOn = 0;
            // diminuisco il punteggio
            _addPoints(-20);
			// suono fluxball
			audioFluxball.Play();

        } else if (hitObj.tag == "timeBall")
        {
            // dice che la timeball non è più attiva
            timeBallActive = 0;
            //rallenta velflux
			rallentaTempo();
			// faccio partire esplosione timeball
			GameObject timeballExp = (GameObject)Instantiate (timeballExplosion, new Vector3(hitObj.transform.position.x, hitObj.transform.position.y, hitObj.transform.position.z),Quaternion.Euler(new Vector3(-90, -90, 0)));
			timeballExp.GetComponent<ParticleSystem> ().enableEmission = true;
			timeBallin.GetComponent<ParticleSystem> ().Play ();
			// suono timeball
			audioTimeball.Play();

            Destroy(hitObj);
            
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

        // friendly error messages
        if (UIRemoveScore == null)
            Debug.LogError("Need to set UIRemoveScore on Game Manager.");

        // friendly error messages
        if (UIAddScore == null)
            Debug.LogError("Need to set UIAddScore on Game Manager.");

        if (UIGameOver==null)
			Debug.LogError ("Need to set UIGameOver on Game Manager.");
		
        if (mainCamera==null)
            Debug.LogError("Need to set MainCamera on Game Manager.");

        if (UILevel == null)
            Debug.LogError("Need to set UILevel on Game Manager.");

        // get stored player prefs
        // refreshPlayerState();
        doMusic = PlayerPrefs.GetInt("musica", 1);
        doVibrate = PlayerPrefs.GetInt("vibrate", 1);

        musica = gm.GetComponent<AudioSource>();

        if (musica != null)
        {
            if (doMusic == 1)
            {     
                musica.Play();
            }
            else
            {
                musica.Stop();
            }

        }

        if (doVibrate == 1)
        {
            vibrate = true;
        } else
        {
            vibrate = false;
        }

		audioTimeball = (AudioSource) Instantiate(timeballSound);
		audioFluxball = (AudioSource) Instantiate(fluxballSound);

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
        UIHighscore.text = highscore.ToString();
    }

    // funzione chiamabile dall'esterno che aggiunge punti allo score
    public static void addPoints(int am)
    {
        gm._addPoints(am);
    }

    // public function to add points and update the gui
    private void _addPoints(int am)
	{
        // do animation se non siamo nei livelli
        if (am > 0)
        {
            if (livello == 0)
            {
                Text addScore = Instantiate(UIAddScore);
                addScore.transform.SetParent(canvas.transform);
            }

            // ogni volta che prendo una fluxball verde la conto
            contFluxball += 1;
        }
        else
        {
            if (livello == 0)
            {
                // do animation se non siamo nei livelli
                Text removeScore = Instantiate(UIRemoveScore);
                removeScore.transform.SetParent(canvas.transform);
            }
        }

        // increase score
        score += (int)am;

        // increase highscore
        if (score > highscore)
            highscore = score;

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
        baglioreflusso.color = coloreNuovo;
    }

	private void rallentaTempo () {
		// rallento il tempo
		Time.timeScale = 0.7f;
		InvokeRepeating ("aumentoTimeScale", 0.2f, 0.4f);
	}

	// public function to remove player life and reset game accordingly
	public void ResetGame() {
        // remove life and update GUI

        score = 0;
		refreshGUI();
        

		// esempio di chiamata ad un metodo dentro il player
	    //_player.GetComponent<CharacterController2D>().Respawn(_spawnLocation);
		
	}

    IEnumerator LoadLevel(string _name, float _delay)
    {
        yield return new WaitForSeconds(_delay);
        Application.LoadLevel(3);
    }

	public void LoadLevel(int number)
	{
		//yield return new WaitForSeconds(_delay);
		Application.LoadLevel(number);
	}

	private void aumentoTimeScale() {
		Debug.Log (Time.timeScale);
		float tempTimeScale = Time.timeScale;
		if (tempTimeScale == 1f) {
			CancelInvoke ("aumentoTimeScale");
		} else {
			Time.timeScale += 0.05f;
		}
	}


	private void reloadGame () {
		Debug.Log ("reloadGame");
		// porto la camera sul flusso
		isDeath = false;
		timeFuoriSchermo = 0;
		mainCamera.transform.position = new Vector3(flusso.transform.position.x, flusso.transform.position.y, -10f);
		deathPanel.SetActive (false);
		pauseButton.SetActive (true);
		paused = false;
		Debug.Log ("paused");
		Debug.Log (paused);
		// se la lunghezza del flusso è vicina a zero allungo il flusso
		if (lunghezzaFlusso <= 2f & !debug) {
			Debug.Log ("start lifetime one more chance");
			Debug.Log (particleSystemflusso.startLifetime);
			//particleSystemflusso.startLifetime = 1f;
			lunghezzaFlusso = 10;
			Debug.Log (particleSystemflusso.startLifetime);
		}
		rallentaTempo ();
	}

    // CI POSSONO SERVIRE
    // public function for level complete
    /* public void LevelCompete() {
		
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
        musica.Stop();
		EventSystem.current.SetSelectedGameObject (MenuPauseDefaultButton);

        // salvo il punteggio
        //PlayerPrefs.SetInt("highscore", highscore);

        if (pubblicita)
        {
            //mostro la pubblicita ogni 3 morti
            int pausePubblicita = PlayerPrefs.GetInt("pausePubblicita", 0);
            if (pausePubblicita == 3)
            {
                manageAds(1);
                PlayerPrefs.SetInt("pausePubblicita", 0);
            }
            else
                PlayerPrefs.SetInt("pausePubblicita", pausePubblicita + 1);
        }
	}

	private void _PauseGame() 
	{
		paused = true;
		Time.timeScale = 0;
		pauseButton.SetActive (false);
		pausePanel.SetActive (true);
		EventSystem.current.SetSelectedGameObject (MenuPauseDefaultButton);

        // salvo il punteggio
        //PlayerPrefs.SetInt("highscore", highscore);

        if (pubblicita)
			manageAds (1);
	}

	public void ContinueGame() 
	{
        pauseButton.SetActive(true);
        pausePanel.SetActive(false);
        instructionPanel.SetActive(false);
        instructionLevelPanel.SetActive(false);
        deathPanel.SetActive(false);
        nextLevelPanel.SetActive(false);
        paused = false;
        Time.timeScale = 1;
        musica.Play();
    }

    public void InstructionPause()
    {
        paused = true;
        Time.timeScale = 0;
        pauseButton.SetActive(false);
        instructionPanel.SetActive(true);
        PlayerPrefs.SetInt("firstTime", 0);
        // musica.Stop();

        // guadagni il trofeo per aver inizato
        Social.ReportProgress("CgkI6Imc5NEGEAIQAg", 100.0f, (bool success) => {
            // handle success or failure
        });
    }

    public void InstructionLevelPause()
    {
        paused = true;
        Time.timeScale = 0;
        pauseButton.SetActive(false);
        instructionLevelPanel.SetActive(true);
        PlayerPrefs.SetInt("firstLevelTime", 0);
        // musica.Stop();
        //PlayerPrefs.SetInt("firstTime", 0);

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
        // metto in pausa il gioco
        paused = true;

        // invio l'highscore solo se è stato modificato
        if (highscore > initialHighscore)
        {
			initialHighscore = highscore;
			// salvo il punteggio
			PlayerPrefs.SetInt("highscore", highscore);
			PlayerPrefs.Save();
			Social.ReportScore(highscore, id_leaderboard, (bool success) => {
                
            });
        }
			
        // disabilito i pannelli per sicurezza
        pausePanel.SetActive (false);
		deathPanel.SetActive (false);
        deathImage.SetActive (false);
        pauseButton.SetActive(false);
        nextLevelPanel.SetActive(false);
        //Time.timeScale = 0;

        deathPanel.SetActive (true);
		EventSystem.current.SetSelectedGameObject (MenuDeathDefaultButton);

        //mostro la pubblicita ogni 2 morti
        int deathPubblicita = PlayerPrefs.GetInt("deathPubblicita", 0);
        if (deathPubblicita == 2)
        {
            ShowSimpleAd();
            PlayerPrefs.SetInt("deathPubblicita", 0);
        }
        else
            PlayerPrefs.SetInt("deathPubblicita", deathPubblicita + 1);

    }

    private void NextLevel()
    {
        paused = true;
        Time.timeScale = 0;
        pauseButton.SetActive(false);
        nextLevelPanel.SetActive(true);
        EventSystem.current.SetSelectedGameObject(MenuNextLevelDefaultButton);

        // salvo il livello successivo
        PlayerPrefs.SetInt("livello", livello + 1);

    }

    public void OneMoreChance()
	{
		// nella one more chance gli faccio vedere un video non skippabile
		Debug.Log ("ONEMORECHANCE");
		ShowRewardedAd ();
		Debug.Log ("ISREADY");
		// devo riportare la camera sul flusso
		// devo aumentare la lunghezza del flusso, solo se è quasi a zero
		// devo rallentare il tempo Time.timeScale = 0.07 e con un invoke chiamare una funzione ogni x secondi per aumentarla fino a 1 e poi stopparsi
		//WaitSecond (10.0f);
		//paused = false;
		//Time.timeScale = 1;
		//deathPanel.SetActive (false);
	}

	IEnumerator WaitSecond(float _delay)
	{
		yield return new WaitForSeconds(_delay);
	}

    private bool checkDeath()
    {
        // per morire posso avere due condizioni:
        // 1) ho perso il flusso:quando il flusso va fuori e sta fuori dallo schermo per più di tot secondi
        // 2) il flusso non ha più coda e quindi muoio
		if (((timeFuoriSchermo != 0 && Time.time - timeFuoriSchermo > 3) |( lunghezzaFlusso <= 2f)) & !debug)

        {
            return true;
        } 

        return false;
    }

    private void checkEndLevel()
    {
		if (lunghezzaFlusso <= 2f)
        {
            NextLevel();
        }
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
		if (Advertisement.IsReady ("video")) {
			Advertisement.Show ("video");
		}
	}

    public void ShowRewardedAd()
    {
		if (Advertisement.isInitialized && Advertisement.IsReady("rewardedVideo"))
        {
            var options = new ShowOptions { resultCallback = HandleShowResult };
            Advertisement.Show("rewardedVideo", options);
        } else
        {
			Debug.Log ("TEST AD");
            HandleShowResult(ShowResult.Finished);
        }
    }

    private void HandleShowResult(ShowResult result)
    {
		Debug.Log (result);
        switch (result)
        {
		case ShowResult.Finished:
			Debug.Log ("The ad was successfully shown.");
                //
                // YOUR CODE TO REWARD THE GAMER
                // Give coins etc.
			reloadGame ();
            break;
		case ShowResult.Skipped:
			Debug.Log ("The ad was skipped before reaching the end.");
			LoadLevel ("MainMenu", 2.0f);
			reloadGame ();
            break;
		case ShowResult.Failed:
			reloadGame ();
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
