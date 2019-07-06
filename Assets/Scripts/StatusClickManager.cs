using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StatusClickManager : MonoBehaviour {

    public RandomGateway randomGateway;

    private RectTransform rectTransform;
    bool isOpen = false;

    //time after object reach a target place 
    float timeOfTravel = 5f;
    // actual floting time 
    float currentTime = 0f;
    float normalizedValue;

    void Start() {
        rectTransform = GetComponent<RectTransform>();
        InvokeRepeating("DoVideoAnalysisAsyncOperation", 2.0f, 15.0f);
    }

    private VideoBreakdownAsyncOperation breakdownAsyncOperation;
    private VideoBreakdownAsyncOperation.TimedData[] topics;
    private VideoBreakdownAsyncOperation.TimedData[] annotations;
    public Text field1TopicText;
    public Text field1AnnotationsText;
    private bool toggle = true;

    void DoVideoAnalysisAsyncOperation()
    {
        if (randomGateway.latestVideoAnalysisAsyncOperation != null && !string.IsNullOrEmpty(randomGateway.latestVideoAnalysisAsyncOperation.BreakdownId)) {
            if (breakdownAsyncOperation == null) {
                breakdownAsyncOperation = randomGateway.GetVideoBreakdown(randomGateway.latestVideoAnalysisAsyncOperation.BreakdownId);
            }

            if(breakdownAsyncOperation.IsDone) {
                if (breakdownAsyncOperation.Data.summarizedInsights.topics != null) {
                    topics = breakdownAsyncOperation.Data.summarizedInsights.topics;
                    string topic_str = "";
                    foreach (VideoBreakdownAsyncOperation.TimedData i in topics) {
                        topic_str += i.name + ",";
                    }
                    if (toggle) {
                        field1TopicText.text = topic_str;
                    } else {
                        field1TopicText.text = topic_str;
                    }
                }
                if (breakdownAsyncOperation.Data.summarizedInsights.annotations != null) {
                    annotations = breakdownAsyncOperation.Data.summarizedInsights.annotations;
                    string annotations_str = "";
                    foreach (VideoBreakdownAsyncOperation.TimedData i in annotations) {
                        annotations_str += i.name + ",";
                    }
                    if (toggle) {
                        field1AnnotationsText.text = annotations_str;
                    } else {
                        field1AnnotationsText.text = annotations_str;
                    }
                }
            }
        }
    }

    public void StatusClicked() {
        if (!isOpen) {
            LerpObject(new Vector3(-100f, -422f, 0f));
            isOpen = true;
        } else {
            LerpObject(new Vector3(100f, -422f, 0f));
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
