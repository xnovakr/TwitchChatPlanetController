using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class CommandGeneratorMeteors
{
    public static void GenerateCommand(string message, bool admin = false)
    {
        if (message == null) return;
        string commandType = message.Split(MessageConverter.separator)[0];
        string command = MessageConverter.RemoveFistSplitFromString(message, MessageConverter.separator);

        switch (CommandingClasses.GetCommandEnum(commandType))
        {
            case CommandingClasses.Commands.NULL:
                break;
            case CommandingClasses.Commands.Meteor:
                SendCommand(commandType);
                break;
            case CommandingClasses.Commands.Noise:
                command = GetJsonShapeCommand(command);
                SendCommand(commandType + MessageConverter.separator + command);
                break;
            case CommandingClasses.Commands.Color:
                command = GetJsonColorCommand(command);
                SendCommand(commandType + MessageConverter.separator + command);
                break;
            case CommandingClasses.Commands.Resolution:
                SendCommand(commandType + MessageConverter.separator + GetResolutionCommand(command, admin));
                break;
            case CommandingClasses.Commands.Size:
                SendCommand(commandType + MessageConverter.separator + GetSizeCommand(command, admin));
                break;
            case CommandingClasses.Commands.PlanetReset:
                SendCommand(commandType);
                break;
            case CommandingClasses.Commands.CameraMovement:
                break;
        }
    }
    public static void SendCommand(string command)
    {
        if (PipeConnection_Client.Instasnce.isAlloved)
        {
            PipeConnection_Client.Instasnce.SendMessage(command);
        }
        else
        {
            Debug.Log(command);
        }
    }
    private static string GetJsonShapeCommand(string command)
    {
        ShapeCommand shapeCommand = new ShapeCommand();
        if (command.Contains("up")) shapeCommand.SetValue(.01f);
        if (command.Contains("down")) shapeCommand.SetValue(-.01f);
        return JsonUtility.ToJson(shapeCommand);
    }
    private static string GetJsonColorCommand(string command)
    {
        ColorCommand colorCommand = new ColorCommand();
        if (command.Contains("red")) colorCommand.AddRed();
        if (command.Contains("green")) colorCommand.AddGreen();
        if (command.Contains("blue")) colorCommand.AddBlue();

        return JsonUtility.ToJson(colorCommand);
    }
    public static void MoveCamera(int value)
    {
        CameraCommand cameraCommand = new CameraCommand();
        cameraCommand.z = value;
        SendCommand(CommandingClasses.Commands.CameraMovement.ToString() + MessageConverter.separator + JsonUtility.ToJson(cameraCommand));
    }
    public static string GetResolutionCommand(string value, bool admin)
    {
        if (value.Contains("up")) return "1";
        if (value.Contains("down")) return "-1";
        if (admin) return value;
        return null;
    }
    public static string GetSizeCommand(string value, bool admin)
    {
        if (value.Contains("up")) return "0.1";
        if (value.Contains("down")) return "-0.1";
        if (admin) return value;
        return null;
    }
}
