using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DiscordStatus : MonoBehaviour {
    DiscordController discord;
    double lastScore;
    int steps = 0;
    int currentEpochTime = 0;
    public TextMesh rewardText;
    public TextMesh discordText;
    // Use this for initialization
    void Start () {
        DateTime epochStart = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
        currentEpochTime = (int)(DateTime.UtcNow - epochStart).TotalSeconds;
       discord = GetComponent<DiscordController>();
	}
	
	// Update is called once per frame
	void Update () {
        UpdatePresence();

    }

    public void OnDiscordConnect()
    {
      //  discordText.color = Color.green;
      //  discordText.text = "Discord Joined!";
      //  Debug.Log("Discord Connected");
    }

    public void SubmitStep(float score)
    {
        steps++;
        lastScore = score;
        rewardText.text = "Reward: " + score;
    }



    private void UpdatePresence()
    {

        
       discord.presence.state = "MoveNet Step: " + steps;
       discord.presence.details = "Last Reward: " + lastScore;
       discord.presence.largeImageKey = "4056";
       discord.presence.largeImageText = "Team 4056";
       discord.presence.startTimestamp = currentEpochTime;
       
       DiscordRpc.UpdatePresence(discord.presence);
    }
}
