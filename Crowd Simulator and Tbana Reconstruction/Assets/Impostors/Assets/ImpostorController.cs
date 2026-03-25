using UnityEngine;

public class ImpostorController : MonoBehaviour
{
    public Transform cam;

    public int frames = 16;
    public int columns = 4;
    public int rows = 4;

    Renderer rend;

    void Start()
    {
        rend = GetComponent<Renderer>();
    }

    void Update()
    {
        Vector3 dir = cam.position - transform.position;

        float angle = Mathf.Atan2(dir.x, dir.z) * Mathf.Rad2Deg;

        if (angle < 0)
            angle += 360;

        int frame = 0;

        int column = frame % columns;
        int row = frame / columns;

        Vector2 scale = new Vector2(1f / columns, 1f / rows);

        Vector2 offset = new Vector2(
            column * scale.x,
            row * scale.y
        );

        rend.material.mainTextureScale = scale;
        rend.material.mainTextureOffset = offset;

        transform.Rotate(0,180,0);
    }
}