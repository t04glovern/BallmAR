using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class MarklessAR : MonoBehaviour {

	// Gyro
	private Gyroscope gyro;
	private GameObject cameraContainer;
	private Quaternion rotation;

	// Camera
	public WebCamTexture cam;
	public RawImage background;
	public AspectRatioFitter fit;

	private bool arReady = false;

    void Start()
    {
        /*
        // Check permissions
        yield return Application.RequestUserAuthorization
            (UserAuthorization.WebCam | UserAuthorization.Microphone);

        if (Application.HasUserAuthorization
            (UserAuthorization.WebCam | UserAuthorization.Microphone)) {
        } else {
            Debug.Log ("No Authentication provided");
        }
        */

        // Check if we support all services
        // Gyroscope
        if (!SystemInfo.supportsGyroscope) {
            Debug.Log ("This device does not have a Gyroscope");
            return;
        }

        // Back Camera
        for (int i = 0; i < WebCamTexture.devices.Length; i++) {
            if (!WebCamTexture.devices [i].isFrontFacing) {
                cam = new WebCamTexture(
                    WebCamTexture.devices[i].name,
                    Screen.width, 
                    Screen.height
                );
                break;
            }
        }

        // If no Back Camera is found
        if (cam == null) {
            Debug.Log ("This device does not have a Back Camera");
            return;
        }

        // All services are supported, let's enable them!

        // Create a camera container gameobject to house our main camera
        cameraContainer = new GameObject("Camera Container");
        cameraContainer.transform.position = transform.position;
        transform.SetParent (cameraContainer.transform);

        // Enable the gyro service
        gyro = Input.gyro;
        gyro.enabled = true;
        cameraContainer.transform.rotation = Quaternion.Euler (90f, 0, 0);
        rotation = new Quaternion (0, 0, 1, 0);

        // Play the camera feed onto the background texture
        cam.Play ();
        background.texture = cam;

        // Pre-requisite services for AR are met
        arReady = true;
	}

    void Update()
	{
        // If AR services were successful in initializing
        if (arReady) {
            // Update camera
            float ratio = (float)cam.width / (float)cam.height;
            fit.aspectRatio = ratio;

            float scaleY = cam.videoVerticallyMirrored ? -1.0f : 1.0f;
            background.rectTransform.localScale = new Vector3 (1f, scaleY, 1f);

            int orient = -cam.videoRotationAngle;
            background.rectTransform.localEulerAngles = new Vector3 (0, 0, orient);

            // Update gyro
            transform.localRotation = gyro.attitude * rotation;
        }

	}
}
