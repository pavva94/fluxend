using UnityEngine;
using System.Collections;

public class MenuButtonLoadLevel : MonoBehaviour {

	public void loadLevel(string leveltoLoad)
	{
		Application.LoadLevel (leveltoLoad);
	}
}
