using UnityEngine;
using UnityEditor;
using System.IO;

public class ImpostorManager : MonoBehaviour {

	private Renderer renderer;
	private Camera captureCamera;

	private GameObject impostorQuad;
    private GameObject LODGroup;
	
	private Transform cameraTransform;
	private Transform targetTransform;

	private Material impostorMaterial;

    private float distance;

	[Header("Settings")]
	public int resolution = 1024;
	public int frames = 144;
	private int columns, rows;

	[Header("References")]
	public Material impostorNormalMaterial;
	public Material impostorMaterialPrefab;
    public GameObject impostorQuadPrefab;
	public GameObject captureCameraPrefab;
    public GameObject LODGroupPrefab;

    // Setup impostor stuff for this model and bake the atlases
    void Start() {
        // Add capture camera to the scene
		GameObject cameraObject = Instantiate(this.captureCameraPrefab);
        this.captureCamera = cameraObject.GetComponent<Camera>();
		this.cameraTransform = cameraObject.GetComponent<Transform>();
		if (cameraObject == null || this.captureCamera == null || this.cameraTransform == null) {
			Debug.Log("ImpostorManager is not setup correctly!?");
		}

        // Add LOD group to the scene
        this.LODGroup = Instantiate(this.LODGroupPrefab);
        if(LODGroup == null){
            Debug.Log("LODGroup didn't instantiate");
        }
        // Set model as child of LOD group (required for LOD group to manage it correctly)
        transform.SetParent(this.LODGroup.transform);

        // Duplicate the impostor material so that we can add the later baked textures to it
		this.impostorMaterial = Instantiate(this.impostorMaterialPrefab);
        if(LODGroup == null){
            Debug.Log("impostorMaterial didn't instantiate");
        }

        renderer = GetComponentInChildren<Renderer>();
        if (renderer == null){
            Debug.Log("renderer cannot be found");
        }

        // Create the 360° color and normal map atlases for the model
		this.BakeModel();
        // Remove the capture camera from the scene
		DestroyImmediate(cameraObject);

        // Add the impostor renderer to the scene
		this.impostorQuad = Instantiate(this.impostorQuadPrefab, this.LODGroup.transform);
        if(impostorQuad == null){
            Debug.Log("impostorQuad didn't instantiate");
        }
        // Make sure it has the correct height (same as the model)
        this.MatchQuadHeight();

        // Setup variables for the impostor renderer (Update) to work
		this.renderer = this.impostorQuad.GetComponent<Renderer>();
		this.renderer.material = this.impostorMaterial;
		this.targetTransform = this.impostorQuad.GetComponent<Transform>();
		this.cameraTransform = Camera.main.GetComponent<Transform>();

		this.columns = this.rows = Mathf.FloorToInt(Mathf.Sqrt(frames));

        // Setup the LOD group with the correct renderers
        UnityEngine.LOD[] lods = this.LODGroup.GetComponent<LODGroup>().GetLODs();
        lods[0].renderers = GetComponentsInChildren<Renderer>();
        lods[1].renderers = new Renderer[] { this.renderer };
        this.LODGroup.GetComponent<LODGroup>().SetLODs(lods);
	}

    // Handle impostor rendering using angle of player camera and baked atlas
    void Update() {
        // Unable to perform impostor rendering if we don't know the targets transform
		if (targetTransform == null) {
			return;
		}

        // Calculate the angle of attack
        Vector3 dir = cameraTransform.position - targetTransform.position;
        float angle = Mathf.Atan2(dir.x, dir.z) * Mathf.Rad2Deg;

        if (angle < 0) {
            angle += 360;
		}

        // Get the correct frame from the atlas depending on our angle from the impostor
        int frame = Mathf.FloorToInt(angle / (360f / frames)) % frames;

        int column = frame % columns;
        int row = rows + 5 - frame / columns;

        // Calculate the new texture offset and scale
        Vector2 scale = new Vector2(1f / columns, 1f / rows);
        Vector2 offset = new Vector2(column * scale.x, row * scale.y);

        // Set the color texture parameters
        this.renderer.material.SetTextureScale("_MainTex", scale);
        this.renderer.material.SetTextureOffset("_MainTex", offset);
        // Set the normal texture parameters
        this.renderer.material.SetTextureScale("_NormalTex", scale);
        this.renderer.material.SetTextureOffset("_NormalTex", offset);

        // Rotate the impostor so that it is always rotated towards the player camera
        Vector3 lookAwayDir = this.targetTransform.position - this.cameraTransform.position;
        lookAwayDir.y = 0;

        if (lookAwayDir.sqrMagnitude > 0.001f) {
            this.targetTransform.rotation = Quaternion.LookRotation(lookAwayDir);
        }
	}

    // Create and setup impostor for model (color and normal map)
    public void BakeModel() {
        if (renderer == null){
            Debug.Log("renderer is null");
        }

        if (renderer.materials.Length == 0) {
            Debug.Log("[ImposterCapture] Unable to find any materials on mesh");
        }

        // Calculate correct camera parameters and set it up
        Vector3 centerPos = renderer.bounds.center;
        float radius = renderer.bounds.extents.magnitude;

        this.distance = radius + this.captureCamera.nearClipPlane + 0.01f;

        this.captureCamera.orthographic = true;
        this.captureCamera.orthographicSize = radius * 1.05f; // Add just a little bit

        // Get the 360° color atlas of the model
        Texture2D colorAtlas = BakeAtlas(centerPos, false);

        // Swap out all materials to the normal map material
        Material[] normalMaterials = new Material[renderer.materials.Length];
        for (int i = 0; i < normalMaterials.Length; ++i) normalMaterials[i] = impostorNormalMaterial;

        Material[] realMaterials = renderer.materials;
        renderer.materials = normalMaterials;

        // Get the 360° normal map atlas of the model
        Texture2D normalAtlas = BakeAtlas(centerPos, true);
        // Swap back the model materials
        renderer.materials = realMaterials;

        // Set the textures accordingly
		this.impostorMaterial.SetTexture("_MainTex", colorAtlas);
		this.impostorMaterial.SetTexture("_NormalTex", normalAtlas);

        Debug.Log("Atli created!");
        RenderTexture.active = null;
    }

	// This doesn't need to be recreated every time, we can also store them on disk and let unity handle it (but remember to set the normal map texture to texturetype NormalMap)
    private Texture2D BakeAtlas(Vector3 centerPos, bool isNormalMap) {
        // Calculate columns and rows depending on frames
        int columns = Mathf.CeilToInt(Mathf.Sqrt(frames));
        int rows = Mathf.CeilToInt((float)frames / columns);

        // Create render texture to be used during image capture
        RenderTexture rt = new RenderTexture(resolution, resolution, 24);
        captureCamera.targetTexture = rt;

        // Create the atlas texture
        Texture2D atlas = new Texture2D(resolution * columns, resolution * rows, TextureFormat.RGBA32, false, isNormalMap);
        // Create the frame texture
        Texture2D frame = new Texture2D(resolution, resolution, TextureFormat.RGBA32, false, isNormalMap);

        // Loop over all frames and capture them
        for (int i = 0; i < frames; i++) {
            float angle = (360f / frames) * i;

            // Calculate the correct placement of the camera for this frame
            Vector3 dir = Quaternion.Euler(0, angle, 0) * Vector3.back;

            captureCamera.transform.position = centerPos + dir * distance;
            captureCamera.transform.LookAt(centerPos);
            // Capture the frame and write it to the render texture
            captureCamera.Render();

            // Create frame texture

            RenderTexture.active = rt;

            // Get image from render texture
            frame.ReadPixels(new Rect(0, 0, resolution, resolution), 0, 0);
            frame.Apply();

            // Calculate placement on atlas
            int column = i % columns;
            int row = i / columns;

            // Draw frame on atlas
            atlas.SetPixels(column * resolution, (rows - 1 - row) * resolution, resolution, resolution, frame.GetPixels());
        }

        // Apply pixel data to texture
        atlas.Apply();

        // Remove stuff to free up space
        DestroyImmediate(frame);
		RenderTexture.active = null;
		captureCamera.targetTexture = null;
		DestroyImmediate(rt);

		return atlas;
    }

    // Adjusts size of quad to the height of the object.
    // Also centers the center of the quad to the center of the object.
    private void MatchQuadHeight(){
        Renderer impostorQuadRenderer = impostorQuad.GetComponentInChildren<Renderer>();
        if(impostorQuad == null){
            Debug.Log("impostorQuad is null");
        }

        Bounds sourceBounds = renderer.bounds;
        Vector3 sourceObjectCenter = renderer.bounds.center;
        Vector3 impostorQuadCenter = impostorQuadRenderer.bounds.center;
        float rescale = sourceBounds.size.y * distance;

        //Since the quad is a square, if the y scale is adjusted, the x scale must be as well in order to retain the x to y ratio.
        impostorQuad.transform.localScale = new Vector3(rescale,rescale, 1f);
        impostorQuad.transform.position += sourceObjectCenter - impostorQuadCenter;
    }
}
