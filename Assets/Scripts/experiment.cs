using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DeltaDNA; 

public class experiment : MonoBehaviour {

    public Text lblUnityVersion;
    public Text lblDdnaVersion;
    public Text lblDevice;
    public Text lblOperatingSystem;
    public Text lblUserLevel; 
    private int userLevel = 1; 

    // Use this for initialization
    void Start () {

        DDNA.Instance.SetLoggingLevel(DeltaDNA.Logger.Level.DEBUG);

        // Hook up callback to fire when DDNA SDK has received session config info, including Event Triggered campaigns.
        DDNA.Instance.NotifyOnSessionConfigured(true);
        DDNA.Instance.OnSessionConfigured += (bool cachedConfig) => GetGameConfig(cachedConfig);

        // Allow multiple game parameter actions callbacks from a single event trigger        
        DDNA.Instance.Settings.MultipleActionsForEventTriggerEnabled = true; 

        DDNA.Instance.StartSDK();
        UpdateHud();

    }
	
    private void UpdateHud()
    {
        lblUnityVersion.text = "Unity version : " + Application.unityVersion;
        lblDdnaVersion.text = "DDNA Version : " + Settings.SDK_VERSION;

        lblDevice.text = "Device : " + SystemInfo.deviceModel + " ( " + SystemInfo.deviceType + " )";
        lblOperatingSystem.text = "Operating System : " + SystemInfo.operatingSystem.ToString();
        lblUserLevel.text = "User Level : " + userLevel.ToString();
    }


    // The callback indicating that the deltaDNA has downloaded its session configuration, including 
    // Event Triggered Campaign actions and logic, is used to record a "sdkConfigured" event 
    // that can be used provision remotely configured parameters. 
    // i.e. deferring the game session config until it knows it has received any info it might need
    public void GetGameConfig(bool cachedConfig)
    {
        Debug.Log("Configuration Loaded, Cached =  "  +cachedConfig.ToString());
        Debug.Log("Recording a sdkConfigured event for Event Triggered Campaign to react to");

        // Create an sdkConfigured event object
        var gameEvent = new GameEvent("sdkConfigured")
            .AddParam("clientVersion", DDNA.Instance.ClientVersion)
            .AddParam("userLevel",userLevel);

        // Record sdkConfigured event and wire up handler callbacks
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

    public void newSessionButtonClick()
    {
        DDNA.Instance.NewSession();
    }

    public void levelUpButtonClick()
    {
        // Clicking Button will record a levelUp event
        // which may trigger a campaign
        Debug.Log("Level Up Button Clicked");
        userLevel++; 

        // Create a missionCompleted event object
        var gameEvent = new GameEvent("levelUp")
            .AddParam("levelUpName", "Level " + userLevel.ToString())
            .AddParam("userLevel", userLevel);


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

        UpdateHud();
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
