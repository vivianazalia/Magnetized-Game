using UnityEngine;

public class Tower : MonoBehaviour
{
    private bool isEnabled;

    void Update()
    {
        isEnabled = this.gameObject.GetComponent<BoxCollider2D>().enabled;
    }

    void OnMouseDown()
    {
        this.gameObject.GetComponent<BoxCollider2D>().enabled = true;
    }

    void OnMouseUp()
    {
        this.gameObject.GetComponent<BoxCollider2D>().enabled = false;
    }
}
