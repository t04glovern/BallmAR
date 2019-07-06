using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Paths : MonoBehaviour {

    GoogleMapLocation[] locations;
    public GoogleMap map;
    int limitArray = 10;
    public TextAsset jsonData;
    // Use this for initialization
    void Start () {
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void ReadJSON()
    {
        string JSONToParse = "{\"values\":" + jsonData.text + "}";
        RootObject nodes = null;
        nodes = JsonUtility.FromJson<RootObject>(JSONToParse);
        locations = new GoogleMapLocation[limitArray];//nodes.values.Length];
        for (int i = 0; i < limitArray; i++) // locations.Length; i++)
        {
            locations[i] = new GoogleMapLocation();
            locations[i].address = "";
            if (nodes.values[i].coordinates.Count == 2)
            {
                locations[i].latitude = nodes.values[i].coordinates[0];
                locations[i].longitude = nodes.values[i].coordinates[1];
            }
        }
        GoogleMapMarker markers = new GoogleMapMarker();
        markers.locations = locations;
        map.markers = new GoogleMapMarker[1];
        map.markers[0] = markers;
        //markers.locations = locations;
        
    }
}

[System.Serializable]
public class JSONNode
{
    public int Node_ID, Node_name, severity;
    public object Name_1, Name_2;
    public List<float> coordinates;
}

[System.Serializable]
public class RootObject
{
    public JSONNode[] values;
}
