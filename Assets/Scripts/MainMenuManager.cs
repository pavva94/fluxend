using UnityEngine;
using System.Collections;
using UnityEngine.UI; // include UI namespace since references UI Buttons directly
using UnityEngine.EventSystems; // include EventSystems namespace so can set initial input for controller support
using GooglePlayGames;
using GooglePlayGames.BasicApi;
using UnityEngine.SocialPlatforms;
using UnityEngine.SceneManagement;


public class MainMenuManager : MonoBehaviour {

	// public int startLives=3; // how many lives to start the game with on New Game

	// references to Submenus
	public GameObject _MainMenu;
	public GameObject _LevelsMenu;
	public GameObject _InstructionMenu;
	public GameObject _LeaderboardMenu;
	public GameObject _SettingMenu;
	public GameObject _CreditsMenu;

	// references to Button GameObjects
	public GameObject MenuDefaultButton;
	public GameObject LevelMainMenuButton;
	public GameObject LeaderboardMainMenuButton;
	public GameObject SettingMainMenuButton;
	public GameObject InstructionMainMenuButton;
	public GameObject CreditsMainMenuButton;

	// list the level names
	public int[] LevelNames;

	// reference to the LevelsPanel gameObject where the buttons should be childed
	public GameObject LevelsPanel;

	// reference to the default Level Button template
	public GameObject[] LevelButtonPrefab;
	
	// reference the titleText so we can change it dynamically
	// public Text titleText;

	// store the initial title so we can set it back
	private string _mainTitle;

    // setting
    private int musica;
    private AudioSource musicMenu;
    private int vibrate;

    private bool settingFirstTime = false;

    // init the menu
    void Awake()
	{
		// store the initial title so we can set it back
		_mainTitle = "";

		// disable/enable Level buttons based on player progress
		setLevelSelect();

		// determine if Quit button should be shown
		//displayQuitWhenAppropriate();

		// Show the proper menu
		ShowMenu("MainMenu");
	}

	void Start() {
		/// <summary>
		/// Inizializzo i google play service
		/// </summary>

		Time.timeScale = 1;
		if (!Social.localUser.authenticated) {
			PlayGamesClientConfiguration config = new PlayGamesClientConfiguration.Builder()
				// enables saving game progress.
				//.EnableSavedGames()
				/*// registers a callback to handle game invitations received while the game is not running.
				.WithInvitationDelegate(<callback method>)
				// registers a callback for turn based match notifications received while the
				// game is not running.
				.WithMatchDelegate(<callback method>)
				// require access to a player's Google+ social graph (usually not needed)
				.RequireGooglePlus()*/
				.Build();

			PlayGamesPlatform.InitializeInstance(config);
			// recommended for debugging:
			PlayGamesPlatform.DebugLogEnabled = true;
			// Activate the Google Play Games platform
			PlayGamesPlatform.Activate();

			Social.localUser.Authenticate((bool success) => {
				// handle success or failure
				if (!success)
					Debug.Log("Can't log in");
				else
					Debug.Log("Log in!!!");
				});
		}

        /// <summary>
        /// Fine inizializzazione google play service
        /// </summary>

        // prendo i valori di configurazione per vibrazione e musica
        vibrate = PlayerPrefs.GetInt("vibrate", 1);
        musica = PlayerPrefs.GetInt("musica", 1);
        
        Toggle[] toggles = _SettingMenu.GetComponentsInChildren<Toggle>();
        Toggle vibrateToggle = toggles[0];
        Toggle musicaToggle = toggles[1];

        musicMenu = Camera.main.gameObject.GetComponent<AudioSource>();      

        if (musicMenu != null)
        {
            if (musica == 1)
            {
                settingFirstTime = false;
                // questa asseganzione fa si che venga chiamata la funzione gameSetting()
                musicaToggle.isOn = true;
                musicMenu.Play();
            }
            else
            {
                
                // questa asseganzione fa si che venga chiamata la funzione gameSetting()
                musicaToggle.isOn = false;
                musicMenu.Stop();
            }

        }

    }

	// loop through all the LevelButtons and set them to interactable 
	// based on if PlayerPref key is set for the level.
	void setLevelSelect() {
		// turn on levels menu while setting it up so no null refs
		_LevelsMenu.SetActive(true);

		// loop through each levelName defined in the editor
		for(int i=0;i<LevelNames.Length;i++) {
			// get the level name
			int levelname = LevelNames[i];

			// dynamically create a button from the template
			GameObject levelButton = Instantiate(LevelButtonPrefab[i],Vector3.zero,Quaternion.identity) as GameObject;

			// name the game object
			// levelButton.name ="ButtonLevel"+levelname;

			// set the parent of the button as the LevelsPanel so it will be dynamically arrange based on the defined layout
			levelButton.transform.SetParent(LevelsPanel.transform,false);

			// get the Button script attached to the button
			Button levelButtonScript = levelButton.GetComponent<Button>();

			// setup the listener to loadlevel when clicked
			levelButtonScript.onClick.RemoveAllListeners();
			levelButtonScript.onClick.AddListener(() => loadLevel(levelname));

			// set the label of the button
			//Text levelButtonLabel = levelButton.GetComponentInChildren<Text>();
			// levelButtonLabel.text = levelname.ToString();
			levelButtonScript.interactable = true;

			// determine if the button should be interactable based on if the level is unlocked
			/*if (PlayerPrefManager.LevelIsUnlocked (levelname)) {
				levelButtonScript.interactable = true;
			} else {
				levelButtonScript.interactable = false;
			}*/
		}
	}

	// determine if the QUIT button should be present based on what platform the game is running on
	/*void displayQuitWhenAppropriate() 
	{
		switch (Application.platform) {
			// platforms that should have quit button
			case RuntimePlatform.WindowsPlayer:
			case RuntimePlatform.OSXPlayer:
			case RuntimePlatform.LinuxPlayer:
				QuitButton.SetActive(true);
				break;

			// platforms that should not have quit button
			// note: included just for demonstration purposed since
			// default will cover all of these. 
			case RuntimePlatform.WindowsEditor:
			case RuntimePlatform.OSXEditor:
			case RuntimePlatform.IPhonePlayer:
			case RuntimePlatform.OSXWebPlayer:
			case RuntimePlatform.WindowsWebPlayer:
			case RuntimePlatform.WebGLPlayer: 
				QuitButton.SetActive(false);
				break;

			// all other platforms default to no quit button
			default:
				QuitButton.SetActive(false);
				break;
		}
	}*/

	// Public functions below that are available via the UI Event Triggers, such as on Buttons.

	// Show the proper menu
	public void ShowMenu(string nameButton)
	{
		// turn all menus off
		_MainMenu.SetActive (false);
		_InstructionMenu.SetActive(false);
		_LevelsMenu.SetActive(false);
		_LeaderboardMenu.SetActive(false);
		_SettingMenu.SetActive(false);
		_CreditsMenu.SetActive(false);

		// turn on desired menu and set default selected button for controller input
		switch(nameButton) {
		case "MainMenu":
			    _MainMenu.SetActive (true);
			    EventSystem.current.SetSelectedGameObject (MenuDefaultButton);
			    //titleText.text = _mainTitle;
			    break;
		case "LevelSelect":
			    _LevelsMenu.SetActive(true);
			    EventSystem.current.SetSelectedGameObject (LevelMainMenuButton);
			    //titleText.text = "Level Select";
			    break;
		case "Instruction":
			    _InstructionMenu.SetActive(true);
			    EventSystem.current.SetSelectedGameObject (InstructionMainMenuButton);
			    //titleText.text = "About";
			    break;
		case "Leaderboard":
			    //_LeaderboardMenu.SetActive (true);
			    _OnShowLeaderBoard ();
                _MainMenu.SetActive(true);
                //EventSystem.current.SetSelectedGameObject (LeaderboardMainMenuButton);
                //titleText.text = "About";
                break;
		case "Setting":
			    _SettingMenu.SetActive(true);
                EventSystem.current.SetSelectedGameObject (SettingMainMenuButton);
			    //titleText.text = "About";
			    break;
		case "Credits":
			    _CreditsMenu.SetActive (true);
			    EventSystem.current.SetSelectedGameObject (CreditsMainMenuButton);
			    //titleText.text = "About";
			    break;
		}
	}

	private void _OnShowLeaderBoard ()
	{
		//        Social.ShowLeaderboardUI (); // Show all leaderboard
		PlayGamesPlatform.Instance.ShowLeaderboardUI ("CgkI6Imc5NEGEAIQAA");
		//((PlayGamesPlatform)Social.Active).ShowLeaderboardUI ("CgkI6Imc5NEGEAIQAA"); // Show current (Active) leaderboard
	}

	// Setting panel
	public void gameSetting(string nameToggle)
	{
        // accrocchio per non far cambiare i setting la prima volta
        if (!settingFirstTime)
        {
            switch (nameToggle)
            {
                case "Vibration":
                    //GameManager.changeVibrate();
					Debug.Log(vibrate);
                    if (vibrate == 1)
                    {
                        PlayerPrefs.SetInt("vibrate", 0);
                        vibrate = 0;

                    }
                    else {
                        PlayerPrefs.SetInt("vibrate", 1);
                        vibrate = 1;
                    }
                    break;
                case "Sound":
                    //GameManager.changeSound();
				Debug.Log(musica);
                    if (musica == 1)
                    {
                       
                        PlayerPrefs.SetInt("musica", 0);
                        musica = 0;
                        musicMenu.Stop();
                    }
                    else {
                        
                        PlayerPrefs.SetInt("musica", 1);
                        musica = 1;
                        musicMenu.Play();
                    }
                    break;
            }
        }
        else
            settingFirstTime = false;
	}

	public void loadScene(int numScenetoLoad) {
		Application.LoadLevel(numScenetoLoad);
	}

	// load the specified Unity level
	public void loadLevel(int leveltoLoad)
	{
        // start new game so initialize player state
        //PlayerPrefManager.ResetPlayerState(startLives,false);

        // se carico Endless setto la costante "livello" a null
        // ENDLESS = 0
        // setto la variabile se sono livello o no
        if (leveltoLoad == 0)
        {
            PlayerPrefs.SetInt("livello", 0);
            
        } else
        {
            PlayerPrefs.SetInt("livello", leveltoLoad);
            
        }
        // livello da caricare il 2
        leveltoLoad = 2;
        Debug.Log(PlayerPrefs.GetInt("livello", 1214342));
        // load the specified level
        SceneManager.LoadScene(leveltoLoad);
    }

	// quit the game
	public void QuitGame()
	{
		Application.Quit ();
	}
}
