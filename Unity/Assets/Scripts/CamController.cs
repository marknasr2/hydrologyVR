using UnityEngine;

public class CamController : MonoBehaviour 
{
	public Transform mainObject;                                           // object's reference.
    public Vector3 pivotOffset = new Vector3(0.0f, 1.0f,  0.0f);       // Offset to repoint the camera.
	public Vector3 camOffset = new Vector3(0.0f, 0.7f, -3.0f);       // Offset to relocate the camera related to the player position.
	public float smooth = 10f;                                         // Speed of camera responsiveness.
	public float horizontalAimingSpeed = 200f;                         // Horizontal turn speed.
	public float verticalAimingSpeed = 200f;                           // Vertical turn speed.
	public float maxVerticalAngle = 30f;                               // Camera max clamp angle. 
	public float minVerticalAngle = -60f;                              // Camera min clamp angle.

	private float angleH = 0;                                          // Float to store camera horizontal angle related to mouse movement.
	private float angleV = 0;                                          // Float to store camera vertical angle related to mouse movement.
	private Transform cam;                                             // This transform.
	private Vector3 relCameraPos;                                      // Current camera position relative to the player.
	private float relCameraPosMag;                                     // Current camera distance to the player.
	private Vector3 smoothPivotOffset;                                 // Camera current pivot offset on interpolation.
	private Vector3 smoothCamOffset;                                   // Camera current offset on interpolation.
	private Vector3 targetPivotOffset;                                 // Camera pivot offset target to iterpolate.
	private Vector3 targetCamOffset;                                   // Camera offset target to interpolate.
	private float targetMaxVerticalAngle;                              // Custom camera max vertical clamp angle. 

    private Quaternion aimRotation;

    private float mouseX = 0, mouseY = 0;

    private float mouseSensitivity = 1;

    private float targetFOV = 60;

    public GameObject uiObject;

    void Awake()
	{
        //Cursor.visible = true;
        //Cursor.lockState = CursorLockMode.Confined;

        // Reference to the camera transform.
        cam = transform;

		// Set camera default position.
		cam.position = mainObject.position + Quaternion.identity * pivotOffset + Quaternion.identity * camOffset;

        //will force rotation for first time going into fps
		cam.rotation = Quaternion.identity;

        // Get camera position relative to the mainObject, used for collision test.
        relCameraPos = transform.position - mainObject.position;
		relCameraPosMag = relCameraPos.magnitude - 0.5f;

		// Set up references and default values.
		smoothPivotOffset = pivotOffset;
		smoothCamOffset = camOffset;

        angleH = mainObject.eulerAngles.y;

        ResetTargetOffsets ();
		ResetMaxVerticalAngle();

        if(camOffset == Vector3.zero)
        {
            gameObject.SetActive(false);
        }
    }

    void Update()
    {
        if (Input.GetAxis("Mouse ScrollWheel") < 0 && targetFOV < 70)
        {
            targetFOV += 5;
        }
        else if (Input.GetAxis("Mouse ScrollWheel") > 0 && targetFOV > 15)
        {
            targetFOV -= 5;
        }

        //Cursor.visible = uiObject.activeSelf;

        //Cursor.lockState = CursorLockMode.Confined;
    }

    void LateUpdate()
	{
        mouseX = 0;
        mouseY = 0;

        mouseX = Input.GetAxis("Mouse X") * mouseSensitivity;
        mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity;

        // Get mouse movement to orbit the camera.
        angleH += mouseX * horizontalAimingSpeed * Time.deltaTime;
        angleV += mouseY * verticalAimingSpeed * Time.deltaTime;

        // Set vertical movement limit.
        angleV = Mathf.Clamp(angleV, minVerticalAngle, targetMaxVerticalAngle);

        // Set camera orientation..
        Quaternion camYRotation = Quaternion.Euler(0, angleH, 0);
        aimRotation = Quaternion.Euler(-angleV, angleH, 0);
        cam.rotation = aimRotation;

        // Set FOV.
        //cam.GetComponent<Camera>().fieldOfView = Mathf.Lerp(cam.GetComponent<Camera>().fieldOfView, targetFOV, Time.deltaTime);

        // Test for collision with the environment based on current camera position.
        Vector3 baseTempPosition = mainObject.position + camYRotation * targetPivotOffset;
        Vector3 noCollisionOffset = targetCamOffset;
        for (float zOffset = targetCamOffset.z; zOffset <= 0; zOffset += 0.5f)
        {
            noCollisionOffset.z = zOffset;
            if (DoubleViewingPosCheck(baseTempPosition + aimRotation * noCollisionOffset, Mathf.Abs(zOffset)) || zOffset == 0)
            {
                break;
            }
        }

        // Repostition the camera.
        smoothPivotOffset = Vector3.Lerp(smoothPivotOffset, targetPivotOffset, smooth * Time.deltaTime);


        smoothCamOffset = Vector3.Lerp(smoothCamOffset, noCollisionOffset, smooth * Time.deltaTime);
        cam.position = mainObject.position + camYRotation * smoothPivotOffset + aimRotation * smoothCamOffset;

        //zoom in
        Camera.main.fieldOfView = Mathf.Lerp(Camera.main.fieldOfView, targetFOV, Time.deltaTime * 3);
    }

	// Set camera offsets to custom values.
	public void SetTargetOffsets(Vector3 newPivotOffset, Vector3 newCamOffset)
	{
		targetPivotOffset = newPivotOffset;
		targetCamOffset = newCamOffset;
	}

	// Reset camera offsets to default values.
	public void ResetTargetOffsets()
	{
		targetPivotOffset = pivotOffset;
		targetCamOffset = camOffset;
	}

	// Set camera vertical offset.
	public void SetYCamOffset(float y)
	{
		targetCamOffset.y = y;
	}

	// Set max vertical camera rotation angle.
	public void SetMaxVerticalAngle(float angle)
	{
		this.targetMaxVerticalAngle = angle;
	}

	// Reset max vertical camera rotation angle to default value.
	public void ResetMaxVerticalAngle()
	{
		this.targetMaxVerticalAngle = maxVerticalAngle;
	}

	// Double check for collisions: concave objects doesn't detect hit from outside, so cast in both directions.
	bool DoubleViewingPosCheck(Vector3 checkPos, float offset)
	{
		float mainObjectFocusHeight = mainObject.GetComponent<CapsuleCollider>().height * 0.5f;
		return ViewingPosCheck (checkPos, mainObjectFocusHeight) && ReverseViewingPosCheck (checkPos, mainObjectFocusHeight, offset);
	}

    // Check for collision from camera to mainObject.
    bool ViewingPosCheck (Vector3 checkPos, float deltaMainObjectHeight)
	{
		RaycastHit hit;

        // If a raycast from the check position to the mainObject hits something...
        if (Physics.Raycast(checkPos, mainObject.position+(Vector3.up* deltaMainObjectHeight) - checkPos, out hit, relCameraPosMag))
		{
            // ... if it is not the mainObject
            if (hit.transform.root != mainObject && !hit.collider.isTrigger)
			{
				// This position isn't appropriate.
				return false;
			}
		}
        // If we haven't hit anything or we've hit the mainObject, this is an appropriate position.
        return true;
	}

    // Check for collision from mainObject to camera.
    bool ReverseViewingPosCheck(Vector3 checkPos, float deltaMainObjectHeight, float maxDistance)
	{
		RaycastHit hit;

		if(Physics.Raycast(mainObject.position+(Vector3.up* deltaMainObjectHeight), checkPos - mainObject.position, out hit, maxDistance))
		{
			if (hit.transform.root != mainObject && hit.transform != transform && !hit.collider.isTrigger)
            {
				return false;
			}
		}
		return true;
	}

	// Get camera magnitude.
	public float getCurrentPivotMagnitude(Vector3 finalPivotOffset)
	{
		return Mathf.Abs ((finalPivotOffset - smoothPivotOffset).magnitude);
	}

    public Vector3 SmoothCamOffset
    {
        get
        {
            return smoothCamOffset;
        }
        set
        {
            smoothCamOffset = value;
        }
    }

    public Vector3 SmoothPivotOffset
    {
        get
        {
            return smoothPivotOffset;
        }
        set
        {
            smoothPivotOffset = value;
        }
    }

    public Vector2 CameraRotation
    {
        get
        {
            return new Vector2(angleV, angleH);
        }
        set
        {
            angleV = value.x;
            angleH = value.y;     
        }
    }

    public float MouseSensitivity
    {
        get
        {
            return mouseSensitivity;
        }
        set
        {
            mouseSensitivity = value;
        }
    }
}
