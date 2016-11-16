using UnityEngine;
using System.Collections;
using UnityEngine.UI; // include UI namespace since references UI Buttons directly
using UnityEngine.EventSystems; // include EventSystems namespace so can set initial input for controller support
using GooglePlayGames;
using GooglePlayGames.BasicApi;
using UnityEngine.SocialPlatforms;
using ChartboostSDK;


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
	public string[] LevelNames;

	// reference to the LevelsPanel gameObject where the buttons should be childed
	public GameObject LevelsPanel;

	// reference to the default Level Button template
	public GameObject LevelButtonPrefab;
	
	// reference the titleText so we can change it dynamically
	// public Text titleText;

	// store the initial title so we can set it back
	private string _mainTitle;

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

		// Carico gli Ads di ChartBoost 
		manageAds();
	}

	// loop through all the LevelButtons and set them to interactable 
	// based on if PlayerPref key is set for the level.
	void setLevelSelect() {
		// turn on levels menu while setting it up so no null refs
		_LevelsMenu.SetActive(true);

		// loop through each levelName defined in the editor
		for(int i=0;i<LevelNames.Length;i++) {
			// get the level name
			string levelname = LevelNames[i];

			// dynamically create a button from the template
			GameObject levelButton = Instantiate(LevelButtonPrefab,Vector3.zero,Quaternion.identity) as GameObject;

			// name the game object
			levelButton.name = levelname+" Button";

			// set the parent of the button as the LevelsPanel so it will be dynamically arrange based on the defined layout
			levelButton.transform.SetParent(LevelsPanel.transform,false);

			// get the Button script attached to the button
			Button levelButtonScript = levelButton.GetComponent<Button>();

			// setup the listener to loadlevel when clicked
			levelButtonScript.onClick.RemoveAllListeners();
			levelButtonScript.onClick.AddListener(() => loadLevel(levelname));

			// set the label of the button
			Text levelButtonLabel = levelButton.GetComponentInChildren<Text>();
			levelButtonLabel.text = levelname;
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
			manageAds (1);
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
		switch(nameToggle) {
		case "Vibration":
			//GameManager.changeVibrate();
			break;
		case "Sound":
			//GameManager.changeSound();
			break;
		}
	}


	// load the specified Unity level
	public void loadLevel(string leveltoLoad)
	{
		// start new game so initialize player state
		//PlayerPrefManager.ResetPlayerState(startLives,false);

		// load the specified level
		Application.LoadLevel (leveltoLoad);
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

	// quit the game
	public void QuitGame()
	{
		Application.Quit ();
	}
}
