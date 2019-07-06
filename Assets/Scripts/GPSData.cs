using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class GPSData : MonoBehaviour {

	public LocationInfo currentGPSInfo;
	public Text longitudeText;
	public Text latitudeText;
	public Text altitudeText;
	public Text horizontalAccuracyText;
	public Text verticalAccuracyText;
	public Text timestampText;

    public GameObject googleDisplay;
    private GoogleMap googleMap;

	private bool gpsInit = false;

	IEnumerator Init() {
        Debug.Log("GPS IEnumeration begun");

		if (!Input.location.isEnabledByUser)
			yield break;

		Input.location.Start (1, 1);

		// 20 Second wait time to init
		int maxWait = 20;

		while (Input.location.status == LocationServiceStatus.Initializing && maxWait > 0) {
			yield return new WaitForSeconds (1);
            Debug.Log("GPS Polling (" + maxWait.ToString() + ")");
			maxWait--;
		}

		// If didn't init within 20 seconds, timeout
		if (maxWait < 1) {
			Debug.Log ("GPS Timed out");
			yield break;
		}

		// Connection for GPS has failed
		if (Input.location.status == LocationServiceStatus.Failed) {
			Debug.Log ("Unable to determine location");
			yield break;
		} else {
			// Successful connection
			gpsInit = true;

            latitudeText.text = "lat: N/A";
			longitudeText.text = "lng: N/A";
			altitudeText.text = "alt: N/A";
            horizontalAccuracyText.text = "h_acc: N/A";
            verticalAccuracyText.text = "v_acc: N/A";
            timestampText.text = "ts: N/A";
		}
	}

	void Start () {
        googleMap = googleDisplay.GetComponent<GoogleMap> ();
		StartCoroutine (Init ());
	}

	void LateUpdate() {

		if (gpsInit) {
			currentGPSInfo = Input.location.lastData;

            latitudeText.text = "lat: " + currentGPSInfo.latitude.ToString ();
            longitudeText.text = "lng: " + currentGPSInfo.longitude.ToString ();
            altitudeText.text = "alt: " + currentGPSInfo.altitude.ToString ();
			horizontalAccuracyText.text = "h_acc: " + currentGPSInfo.horizontalAccuracy.ToString ();
			verticalAccuracyText.text = "v_acc: " + currentGPSInfo.verticalAccuracy.ToString ();
			timestampText.text = "ts: " + currentGPSInfo.timestamp.ToString ();

            googleMap.centerLocation.latitude = currentGPSInfo.latitude;
            googleMap.centerLocation.longitude = currentGPSInfo.longitude;
            googleMap.markers[0].locations[0].longitude = currentGPSInfo.longitude;
            googleMap.markers[0].locations[0].latitude = currentGPSInfo.latitude;
            googleMap.Refresh ();

			// Temp fix, disable services
			Input.location.Stop ();
			gpsInit = false;
		}
	}

	public void UpdateGPSData() {
		StartCoroutine (Init ());
	}
}
