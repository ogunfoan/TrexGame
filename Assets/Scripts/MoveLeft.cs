using UnityEngine;
using Vector2 = System.Numerics.Vector2;

public class MoveLeft : MonoBehaviour
{
    public float speed= 5f;

    void Update()
    {
        transform.Translate(Vector3.left * speed * Time.deltaTime);
        if (transform.position.x < -15f)
        {
            Destroy(gameObject);
        }

    }

}
