using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapClickManager : MonoBehaviour {

    private RectTransform rectTransform;
    bool isOpen = false;

    //time after object reach a target place 
    float timeOfTravel = 5f;
    // actual floting time 
    float currentTime = 0f;
    float normalizedValue;

    void Start() {
        rectTransform = GetComponent<RectTransform>();
    }

    public void MapClicked() {
        if (!isOpen) {
            LerpObject(new Vector3(200f, -322f, 0f));
            isOpen = true;
        } else {
            LerpObject(new Vector3(-200f, -322f, 0f));
            isOpen = false;
        }
    }

    void LerpObject(Vector3 newPosition){ 

        while (currentTime <= timeOfTravel) {
            currentTime += Time.deltaTime;
            normalizedValue = currentTime / timeOfTravel; // we normalize our time 
            rectTransform.anchoredPosition=Vector3.Lerp(rectTransform.rect.position, newPosition, normalizedValue); 
        }
        currentTime = 0f;
    }
}
