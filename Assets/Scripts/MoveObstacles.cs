using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections.Generic;


public class MoveObstacles : MonoBehaviour {

    public float speed;
    private float screenCenterX;
    public float moveVertical;

    // game performance
    public int score = 0;
    public int startEnergy = 50;
    public int energy;

    // UI elements to control
    public Text UIScore;
    public Text UIEnergy;
    public GameObject UIGameOver;

    new Rigidbody2D rigidbody;

    // Use this for initialization
    void Start() {
        UIGameOver.SetActive(false); // disattiva il text gameOver

        rigidbody = GetComponent<Rigidbody2D>();

        screenCenterX = Screen.width * 0.5f;

        energy = startEnergy;
        refreshGUI();
    }

    // Update is called once per frame
    void FixedUpdate() {
        // movimento utente blocchi
        transform.Translate(Vector3.down * moveVertical);
        // if there are any touches currently
        if (Input.touchCount > 0)
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
        AddPoints((int) Time.timeSinceLevelLoad);
    }

    void OnCollisionEnter2D(Collision2D coll)
    {
        Debug.Log(coll.gameObject.tag);
        if (coll.gameObject.tag == "Player")
        {
            // rigidbody.velocity = Vector3.zero;
            // Time.timeScale = 0;
            // carico la prossima scena
            // SceneManager.LoadScene("OneMoreChance");
            RemoveEnergy(10);
            if (energy <= 0)
            {
                Debug.Log("GAME OVER");
                UIGameOver.SetActive(true); // this brings up the gameOver UI
                Time.timeScale = 0f;
                // SceneManager.LoadScene("OneMoreChance");
            }
        }
    }

    // refresh all the GUI elements
    void refreshGUI()
    {
        // set the text elements of the UI
        UIScore.text = "Score: " + score.ToString();
        UIEnergy.text = "Energy: " + energy.ToString();
    }

    // public function to add points and update the gui and highscore player prefs accordingly
    public void AddPoints(int amount)
    {
        // increase score
        score += amount;

        // update UI
        UIScore.text = "Score: " + score.ToString();

        // if score>highscore then update the highscore UI too
        /*if (score > highscore)
        {
            highscore = score;
            UIHighScore.text = "Highscore: " + score.ToString();
        }*/
    }

    // public function to add points and update the gui and highscore player prefs accordingly
    public void RemoveEnergy(int amount)
    {
        // increase score
        energy -= amount;

        // update UI
        UIEnergy.text = "Energy: " + score.ToString();

        // if score>highscore then update the highscore UI too
        /*if (score > highscore)
        {
            highscore = score;
            UIHighScore.text = "Highscore: " + score.ToString();
        }*/
    }
}
