using UnityEngine;

public class GroundScrolling : MonoBehaviour
{

    [SerializeField] private float speed = 5f;
    private float width; // ne kadar mesafede yenilesin kendini
    void Start()
    {
        width = GetComponent<BoxCollider2D>().size.x; // x in uzunlugunu aldik
    }

    // Update is called once per frame
    void Update()
    {
        transform.Translate(Vector2.left * speed * Time.deltaTime);
        if (transform.position.x < -width)
        {
            //ayni sprite i saga al
            transform.position += new Vector3(width * 2f, 0, 0);
        }
    }
}
