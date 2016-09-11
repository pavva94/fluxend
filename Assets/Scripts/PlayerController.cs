using UnityEngine;
using System.Collections;

[System.Serializable]
public class Boundary
{
	public float xMin, xMax, yMin, yMax;
}

public class PlayerController : MonoBehaviour
{
	public float speed = 0.5f;
	//public Boundary boundary;
	public GameObject startPoint;

	new Rigidbody2D rigidbody;

    private float minX, maxX, minY, maxY;
    private Vector2 startPosition;

    private float startTime;
    private float journeyLength;
    private Vector2 bottomCorner;

    void Start()
	{
        // Reset ();

        /* inutili ma potenzialmente utili
        float camDistance = Vector3.Distance(transform.position, Camera.main.transform.position);
        Vector2 bottomCorner = Camera.main.ViewportToWorldPoint(new Vector3(0, 0, camDistance));
        Vector2 topCorner = Camera.main.ViewportToWorldPoint(new Vector3(1, 1, camDistance));
        minX = bottomCorner.x;
        maxX = topCorner.x;
        minY = bottomCorner.y;
        maxY = topCorner.y;
        */
        startPosition = new Vector3(startPoint.transform.position.x, startPoint.transform.position.y, 0.0f);
        transform.position = startPosition;
        startTime = Time.time;
        journeyLength = Vector3.Distance(transform.position, startPoint.transform.position);

        bottomCorner = GameManager.bottomCorner;
    }

    void Update()
    {
        //Debug.Log(transform.position.x);
        //Debug.Log(transform.position.y);
        float distCovered = (Time.time - startTime) * speed;
        float fracJourney = distCovered / journeyLength;
        transform.position = Vector2.Lerp(transform.position, startPoint.transform.position, fracJourney);

        if (transform.position.y < bottomCorner.y - 10)
        {
            GameManager.gameOver();
        }
    }

    /*void OnBecameInvisible() {
		Stop ();
	}

	public void Reset() {
		rigidbody.position = startPoint.transform.position;
		Debug.Log ("RESET");
		Stop ();

	}

	void Stop() {
		rigidbody.velocity = Vector2.zero;
		rigidbody.angularVelocity = 0.0f;
        //GameManager.gameOver();
	}*/
}