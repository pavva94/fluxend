using UnityEngine;


public class Obstacles : MonoBehaviour {

    void OnCollisionEnter2D(Collision2D coll)
    {
        Debug.Log(coll.gameObject.tag);
        if (coll.gameObject.tag == "Player")
        {
            // rigidbody.velocity = Vector3.zero;
            // Time.timeScale = 0;
            // carico la prossima scena
            // SceneManager.LoadScene("OneMoreChance");
            GameManager.playerCollision();
        }
    }
}
