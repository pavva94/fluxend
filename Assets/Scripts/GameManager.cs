using UnityEngine;
using System.Collections;
using UnityEngine.UI; // include UI namespace so can reference UI elements
using UnityEngine.Analytics;
using System.Collections.Generic;

public class GameManager : MonoBehaviour {

	// static reference to game manager so can be called from other scripts directly (not just through gameobject component)
	public static GameManager gm;

	// levels to move to on victory and lose
	public string levelAfterVictory;
	public string levelAfterGameOver;

	// game performance
	public int highscore = 0;

    public float speed;
    private float screenCenterX;
    public float moveVertical;

    // game performance
    public int score = 0;
    public int startEnergy = 50;
    public int energy;
    public int amount = -10;

    // UI elements to control
    public Text UIScore;
	// public Text UIHighScore;
    public Text UIEnergy;
    //public GameObject[] UIExtraLives;
	public GameObject UIGameOver;

	// private variables
	GameObject _player;
	Vector3 _spawnLocation;

    // Gameobject ostacolo
    public GameObject cubo;

    // GameObject startpoint
    public GameObject startpoint;

    // set things up here
    void Awake () {
		// setup reference to game manager
		if (gm == null)
			gm = this.GetComponent<GameManager>();

		// setup all the variables, the UI, and provide errors if things not setup properly.
		setupDefaults();
	}

    // Use this for initialization
    void Start()
    {
        UIGameOver.SetActive(false); // disattiva il text gameOver

        // rigidbody = GetComponent<Rigidbody2D>();

        screenCenterX = Screen.width * 0.5f;

        energy = startEnergy;
        refreshGUI();

        // spawn dei blocchi
        InvokeRepeating("randomspawn", 1, 1);
    }

    // game loop
    void Update() {
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
        int tocchi = Input.touchCount;
        
        GameObject[] cubi = GameObject.FindGameObjectsWithTag("Blocks");
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
        }

        // CODICE UTILE PER TEST CON PC
        /*float input = Input.GetAxis("Horizontal");
        float moveHorizontal = Input.GetAxis("Horizontal");
        Debug.Log(input);
        Debug.Log(moveHorizontal);
        GameObject[] cubi = GameObject.FindGameObjectsWithTag("Blocks");
        foreach (GameObject cubo in cubi)
        {
            Transform transform = cubo.GetComponent<Transform>();
            Rigidbody2D rigidbody = cubo.GetComponent<Rigidbody2D>();
            float moveVertical = 0.09f;
            // movimento utente blocchi
            transform.Translate(Vector3.down * moveVertical);

            
           
                // movimento verticale costante blocchi

                Vector2 movement = new Vector2(moveHorizontal *0.04f, moveVertical);
                rigidbody.velocity = movement * speed;
                //Debug.Log(transform.position.x);
                if (moveHorizontal > 0) transform.Translate(Vector3.right * Time.deltaTime);
                else if (moveHorizontal < 0) transform.Translate(Vector3.left * Time.deltaTime);
                transform.Translate(Vector3.down * moveVertical);

            
        }*/

        _addPoints((int)Time.timeSinceLevelLoad);
    }

    void randomspawn()
    {
        for (int i = 0; i < 6; i++)
        {
            Instantiate(cubo, genpos(), Quaternion.identity);
        }
    }

    Vector3 genpos()
    {
        int x, y, z;
        x = Random.Range(-25, 25);
        y = 4;
        z = 0;
        return new Vector3(x, y, z);
    }

    // setup all the variables, the UI, and provide errors if things not setup properly.
    void setupDefaults() {
		// setup reference to player
		if (_player == null)
			_player = GameObject.FindGameObjectWithTag("Player");
		
		if (_player==null)
			Debug.LogError("Player not found in Game Manager");
		
        if (startpoint==null)
            Debug.LogError("Start point not defined");
        else
            // get initial _spawnLocation based on initial position of player
            _spawnLocation = _player.transform.position;

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
		
		// get stored player prefs
		// refreshPlayerState();

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
        UIScore.text = "Score: " + score.ToString();
        UIEnergy.text = "Energy: " + energy.ToString();
        //UIHighScore.text = "Highscore: " + highscore.ToString();
    }

    // funzione chiamabile dall'esterno che aggiunge punti allo score
    public static void removePoints(int amount)
    {
        gm._addPoints(amount);
    }

    // public function to add points and update the gui and highscore player prefs accordingly
    private void _addPoints(int amount)
	{
		// increase score
		score+=amount;

		// update UI
		UIScore.text = "Score: "+score.ToString();

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
        // decrese score
        energy -= amount;
        Debug.Log("energy");
        Debug.Log(energy);
        // update UI
        UIEnergy.text = "Energy: " + energy.ToString();

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
        _removeEnergy();
        if (energy <= 0)
        {
            Debug.Log("GAME OVER");
            UIGameOver.SetActive(true); // this brings up the gameOver UI
            Time.timeScale = 0f;
            // SceneManager.LoadScene("OneMoreChance");
        }
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
}
