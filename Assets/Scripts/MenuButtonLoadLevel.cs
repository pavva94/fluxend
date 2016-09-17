using UnityEngine;
using System.Collections;

public class MenuButtonLoadLevel : MonoBehaviour {

	public void loadLevel(string leveltoLoad)
	{
        Time.timeScale = 0f;
        Application.LoadLevel (leveltoLoad);
	}
}
