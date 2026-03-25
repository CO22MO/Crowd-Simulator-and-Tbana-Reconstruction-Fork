using UnityEngine;
using System.IO;

public class ImpostorCapture : MonoBehaviour
{
    public Camera captureCamera;
    public Transform target;

    public int frames = 16;
    public int resolution = 1024;
    public float distance = 5f;

    int columns;
    int rows;

    void Start()
    {
        columns = Mathf.CeilToInt(Mathf.Sqrt(frames));
        rows = Mathf.CeilToInt((float)frames / columns);

        RenderTexture rt = new RenderTexture(resolution, resolution, 24);
        captureCamera.targetTexture = rt;

        Texture2D atlas = new Texture2D(
            resolution * columns,
            resolution * rows,
            TextureFormat.RGBA32,
            false
        );

        for (int i = 0; i < frames; i++)
        {
            float angle = (360f / frames) * i;

            Vector3 dir = Quaternion.Euler(0, angle, 0) * Vector3.back;

            captureCamera.transform.position =
                target.position + dir * distance;

            captureCamera.transform.LookAt(target);

            captureCamera.Render();

            Texture2D frame = new Texture2D(
                resolution,
                resolution,
                TextureFormat.RGBA32,
                false
            );

            RenderTexture.active = rt;

            frame.ReadPixels(
                new Rect(0,0,resolution,resolution),
                0,
                0
            );

            frame.Apply();

            int column = i % columns;
            int row = i / columns;

            atlas.SetPixels(
                column * resolution,
                (rows - 1 - row) * resolution,
                resolution,
                resolution,
                frame.GetPixels()
            );
        }

        atlas.Apply();

        byte[] bytes = atlas.EncodeToPNG();

        File.WriteAllBytes(
            Application.dataPath + "/ImpostorAtlas.png",
            bytes
        );

        Debug.Log("Atlas created!");

        RenderTexture.active = null;
    }
}