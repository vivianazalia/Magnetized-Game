using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private UIControllerScript uiControl;
    private Rigidbody2D playerRb;
    private AudioSource crashAudio;

    public float moveSpeed;
    public float pullForce;
    public float rotateSpeed;
    private GameObject closestTower;
    private GameObject hookedTower;
    private Vector3 startPosition;
    private bool isPulled = false;
    private bool isCrashed = false;

    void Start()
    {
        playerRb = this.gameObject.GetComponent<Rigidbody2D>();
        uiControl = GameObject.Find("Canvas").GetComponent<UIControllerScript>();
        crashAudio = this.gameObject.GetComponent<AudioSource>();

        startPosition = this.transform.position;
        Time.timeScale = 1;
    }

    void Update()
    {
        if (closestTower != null && !isPulled)
        {
            if(hookedTower == null)
            {
                hookedTower = closestTower;
            }

            if (hookedTower)
            {
                float distance = Vector2.Distance(transform.position, hookedTower.transform.position);
                
                //Gravitasi sekitar tower
                Vector3 pullDirection = (hookedTower.transform.position - transform.position).normalized;
                float newPullForce = Mathf.Clamp(pullForce / distance, 30, 100);

                playerRb.AddForce(pullDirection * newPullForce);

                //Kecepatan sudut
                playerRb.angularVelocity = -rotateSpeed / distance;

                isPulled = true;
            }
        }

        if (closestTower == null)
        {
            isPulled = false;
            hookedTower = null;
        }

        if (isCrashed)
        {
            if (!crashAudio.isPlaying)
            {
                RestartPosition();
            }
        }
        else
        {
            playerRb.velocity = -transform.up * moveSpeed;
        }
    }

    public void RestartPosition()
    {
        this.transform.position = startPosition;

        this.transform.rotation = Quaternion.Euler(0, 0, 90);

        isCrashed = false;

        if (closestTower)
        {
            closestTower.GetComponent<SpriteRenderer>().color = Color.white;
            closestTower = null;
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if(collision.gameObject.tag == "Wall" || collision.gameObject.tag == "Tower")
        {
            if (!isCrashed)
            {
                crashAudio.Play();
                playerRb.velocity = new Vector3(0, 0, 0);
                playerRb.angularVelocity = 0;
                isCrashed = true;
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Goal")
        {
            uiControl.EndGame();
        }
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if(collision.gameObject.tag == "Tower")
        {
            closestTower = collision.gameObject;

            collision.gameObject.GetComponent<SpriteRenderer>().color = Color.green;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (isPulled)
        {
            return;
        }

        if (collision.gameObject.tag == "Tower")
        {
            closestTower = null;
            hookedTower = null;

            collision.gameObject.GetComponent<SpriteRenderer>().color = Color.white;
        }
    }
}
