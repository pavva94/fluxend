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
    private bool in_Collision = false;

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
        rigidbody = GetComponent<Rigidbody2D>();
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
        if (!in_Collision)
        {
            float distCovered = (Time.time - startTime) * speed;
            float fracJourney = distCovered / journeyLength;
            transform.position = Vector2.Lerp(transform.position, startPoint.transform.position, fracJourney);
        }
        //Vector2 direction = transform.position - startPoint.transform.position;
        //rigidbody.AddForceAtPosition(direction.normalized, transform.position);

        if (transform.position.y < bottomCorner.y - 10)
        {
            GameManager.gameOver();
        }
    }

    void OnCollisionEnter2D(Collision2D col)
    {
        in_Collision = true;
    }

    void OnCollisionExit2D(Collision2D col)
    {
        in_Collision = false;
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