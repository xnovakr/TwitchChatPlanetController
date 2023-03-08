using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class MessageConverter
{
    public static char nameSeparator = ':';
    public static char commandIndicator = '!';
    public static char separator = '|';
    public static string ConverMessageToCommand(string message)
    {
        string command = null;

        //if message dont contain name separator its invalid and return null
        if (!message.Contains(nameSeparator.ToString())) return null;
        command = RemoveNameFromMessage(message);
        
        //if command dont start with command indicator its invalid and return null
        if (!command.StartsWith(commandIndicator.ToString()))
        {
            if (!command.StartsWith(" " + commandIndicator.ToString())) return null;
        }
        command = RemoveCommandIndicator(command);

        command = ReconstructCommand(command);

        return command;
    }
    private static string RemoveNameFromMessage(string message)
    {
        string newMessage = RemoveFistSplitFromString(message, nameSeparator);
        return newMessage;
    }
    private static string RemoveCommandIndicator(string command)
    {
        string newCommand = RemoveFistSplitFromString(command, commandIndicator);
        return newCommand;
    }
    public static string RemoveFistSplitFromString(string message, char splittingChar)
    {
        string newMessage = "";
        string[] messageSplit;
        messageSplit = message.Split(splittingChar);
        for (int i = 1; i < messageSplit.Length; i++)
        {
            newMessage += messageSplit[i];
            if (i != messageSplit.Length - 1) newMessage += splittingChar;
        }
        return newMessage;
    }
    private static string ReconstructCommand(string command)
    {//replaces spaces with separator sign
        string[] commandSplit;
        commandSplit = command.Split(' ');
        string newCommand = FindCommand(commandSplit[0]) + separator;
        for (int i = 1; i < commandSplit.Length; i++)
        {
            newCommand += FindAtrubite(commandSplit[i]);
            if (i != commandSplit.Length - 1) newCommand += separator;
        }
        return newCommand;
    }
    private static string FindCommand(string command)
    {
        if (command.Contains("eteo")) return CommandingClasses.Commands.Meteor.ToString();
        if (command.Contains("ountin")) return CommandingClasses.Commands.Noise.ToString();
        if (command.Contains("colo")) return CommandingClasses.Commands.Color.ToString();
        if (command.Contains("solutio")) return CommandingClasses.Commands.Resolution.ToString();
        if (command.Contains("ize")) return CommandingClasses.Commands.Size.ToString();
        return command;
    }
    private static string FindAtrubite(string command)
    {
        if (command.Contains("up")) return "up";
        if (command.Contains("own")) return "down";
        if (command.Contains("red")) return "red";
        if (command.Contains("reen")) return "green";
        if (command.Contains("lue")) return "blue";
        return command;
    }
}