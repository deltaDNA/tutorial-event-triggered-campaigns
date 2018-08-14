using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DeltaDNA; 

public class experiment : MonoBehaviour {

    public Text lblUnityVersion;
    public Text lbldDdnaVersion;
    public Text lblDevice;
    public Text lblOperatingSystem;

    // Use this for initialization
    void Start () {

        DDNA.Instance.SetLoggingLevel(DeltaDNA.Logger.Level.DEBUG);
        DDNA.Instance.StartSDK();

        UpdateHud();
    }
	
    private void UpdateHud()
    {
        lblUnityVersion.text = "Unity version : " + Application.unityVersion;
        lbldDdnaVersion.text = "DDNA Version : " + Settings.SDK_VERSION;

        lblDevice.text = SystemInfo.deviceModel + " ( " + SystemInfo.deviceType + " )";
        lblOperatingSystem.text = SystemInfo.operatingSystem.ToString(); 
    }


    public void missionStartedButtonClick()
    {
        // Clicking Button will record a missionStarted event
        // which may trigger a campaign
        Debug.Log("Mission Started Button Clicked");

        // Create a missionStarted event object
        var gameEvent = new GameEvent("missionStarted")
            .AddParam("missionName", "First Time User Forest")
            .AddParam("isTutorial", true)
            .AddParam("targetScore", 100)
            .AddParam("maximumMoves", 50)
            .AddParam("livesBalance", 3)
            .AddParam("missionDifficulty", "EASY")
            .AddParam("goldBalance", 999);

        // Record missionCompleted event and wire up handler callbacks
        DDNA.Instance.RecordEvent(gameEvent)
            .Add(new GameParametersHandler(gameParameters => {
                // do something with the game parameters
                myGameParameterHandler(gameParameters);
            }))
            .Add(new ImageMessageHandler(DDNA.Instance, imageMessage => {
                // do something with the image message
                myImageMessageHandler(imageMessage);
            }))
            .Run();

    }

    public void missionCompletedButtonClick()
    {
        // Clicking Button will record a missionCompleted event
        // which may trigger a campaign
        Debug.Log("Mission Completed Button Clicked");

        // Create a missionCompleted event object
        var gameEvent = new GameEvent("missionCompleted")
            .AddParam("missionName", "First Time User Forest")
            .AddParam("missionDifficulty", "EASY")
            .AddParam("isTutorial", true)
            .AddParam("targetScore", 100)
            .AddParam("maximumMoves", 50)
            .AddParam("movesRemaining", 8)
            .AddParam("livesBalance", 3)
            .AddParam("goldBalance", 999);


        // Record missionCompleted event and wire up handler callbacks
        DDNA.Instance.RecordEvent(gameEvent)
            .Add(new GameParametersHandler(gameParameters => {
                // do something with the game parameters
                myGameParameterHandler(gameParameters);
            }))
            .Add(new ImageMessageHandler(DDNA.Instance, imageMessage => {
                // do something with the image message
                myImageMessageHandler(imageMessage);
            }))
            .Run();



    }



    private void myGameParameterHandler(Dictionary<string, object> gameParameters)
    {
        // Parameters Received      
        Debug.Log("Received game parameters from event trigger: " + DeltaDNA.MiniJSON.Json.Serialize(gameParameters));
    }



    private void myImageMessageHandler(ImageMessage imageMessage)
    {
        // Add a handler for the 'dismiss' action.
        imageMessage.OnDismiss += (ImageMessage.EventArgs obj) => {
            Debug.Log("Image Message dismissed by " + obj.ID);

            // NB : parameters not processed if player dismisses action
        };

        // Add a handler for the 'action' action.
        imageMessage.OnAction += (ImageMessage.EventArgs obj) => {
            Debug.Log("Image Message actioned by " + obj.ID + " with command " + obj.ActionValue);

            // Process parameters on image message if player triggers image message action
            if (imageMessage.Parameters != null) myGameParameterHandler(imageMessage.Parameters);
        };

        // the image message is already cached and prepared so it will show instantly
        imageMessage.Show();
    }


}
