using System;
using System.ComponentModel;
using System.Net.Sockets;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Text;

public class ChatReader : MonoBehaviour
{
    public GameObject pipeObject;
    public GameObject chatObject;
    public GameObject channelNameObject;

    private ConsoleManager consoleManager;
    private TcpClient twitchClient;
    private StreamReader reader;
    private StreamWriter writer;
    //private bool dsa = true;
    public string username, password, channelName; //Get the password from https://twitchapps.com/tmi
    public string pingRequest = "PING :tmi.twitch.tv";
    public string pingAnswer = "PONG :tmi.twitch.tv";

    private string[] admins;

    private void Start()
    {
        admins = new string[] { channelName, "Pecokis" };


        consoleManager = GameObject.Find("GameManager").GetComponent<ConsoleManager>();

        if (pipeObject == null)
        {
            pipeObject = GameObject.Find("UnityPipeClient");
        }
        Connect();
        chatObject.GetComponent<Text>().text = "\n";

    }
    private void Update()
    {
        if (!twitchClient.Connected) Connect();
        ReadChat();
    }

    private void Connect()
    {
        twitchClient = new TcpClient("irc.chat.twitch.tv", 6667);
        reader = new StreamReader(twitchClient.GetStream());
        writer = new StreamWriter(twitchClient.GetStream());

        writer.WriteLine("PASS " + password);
        writer.WriteLine("NICK " + username);
        writer.WriteLine("USER " + username + " 8 * :" + username);
        writer.WriteLine("JOIN #" + channelName);
        writer.Flush();

        channelNameObject.GetComponent<Text>().text = channelName;
        if (twitchClient.Connected) Debug.Log("Connected to Twitch client " + username);
        else
        {
            Debug.Log("Failed to connect. Retrying connection");
            Connect();
        }
    }

    private void ReadChat()
    {
        if (twitchClient.Available > 0)
        {
            var message = reader.ReadLine();

            //print(message);
            if (message.Contains("PRIVMSG"))
            {
                //get username by splitting it from the string
                var splitPoint = message.IndexOf("!", 1);
                var chatName = message.Substring(0, splitPoint);
                chatName = chatName.Substring(1);

                //get the user message bz splitting it from the string
                splitPoint = message.IndexOf(":", 1);
                message = message.Substring(splitPoint + 1);
                message = String.Format("{0}: {1}", chatName, message);
                print(message);
                //consoleManager.PrintTcecConsole(message);
                CommandGeneratorMeteors.GenerateCommand(MessageConverter.ConverMessageToCommand(message), AdminCheck(message.Split(':')[0]));
                chatObject.GetComponent<Text>().text += message + "\n";
                //if (PipeConnection_Client.Instasnce.isAlloved)
                //{
                //    PipeConnection_Client.Instasnce.SendMessage(message);
                //}
            }
            else
            {
                Debug.LogWarning(message);
                if (message.Contains(pingRequest))
                {
                    writer.WriteLine(pingAnswer);
                    writer.Flush();
                    Debug.Log("pinging");
                }
            }
        }
    }
    private void OnApplicationQuit()
    {
        twitchClient.Close();
    }
    private bool AdminCheck(string nickName)
    {
        foreach (string admin in admins)
        {
            if (admin == nickName) return true;
        }
        return false;
    }
}
