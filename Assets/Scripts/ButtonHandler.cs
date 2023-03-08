using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ButtonHandler : MonoBehaviour
{
    public GameObject TCEConsole;
    public GameObject TCECConsole;

    public Button TCEConsoleButton;
    public Button TCECConsoleButton;

    private Color buttonNotPressed = new Color(1, 1, 1);
    private Color buttonPressed = new Color(.7f, .7f, .7f);

    private void Awake()
    {
        if (TCEConsole.activeSelf) SetTCEconsole();
        else SetTCECconsole();
#if !UNITY_EDITOR
         if (Screen.width < 480 || Screen.height < 800)
            Screen.SetResolution(480, 800, false);
#endif
    }

    public void CloseApplication()
    {
        Application.Quit();
    }
    public void SetTCEconsole()
    {
        SetButtonColors(TCEConsoleButton, buttonNotPressed, buttonPressed);
        SetButtonColors(TCECConsoleButton, buttonPressed, buttonNotPressed);

        TCEConsole.SetActive(true);
        TCECConsole.SetActive(false);
    }
    public void SetTCECconsole()
    {
        SetButtonColors(TCECConsoleButton, buttonNotPressed, buttonPressed);
        SetButtonColors(TCEConsoleButton, buttonPressed, buttonNotPressed);

        TCEConsole.SetActive(false);
        TCECConsole.SetActive(true);
    }
    private void SetButtonColors(Button button, Color primaryColor, Color secondaryColor)
    { // primary is color of unpressed button and secondary is color of pressed button
        var buttonColors = button.colors;
        buttonColors.normalColor = primaryColor;
        buttonColors.highlightedColor = primaryColor;
        buttonColors.selectedColor = primaryColor;
        buttonColors.pressedColor = secondaryColor;
        buttonColors.disabledColor = secondaryColor;
        button.colors = buttonColors;
    } 
    public void SpawnTestMeteor()
    {
        string msg = CommandingClasses.Commands.Meteor.ToString() + "|" + JsonUtility.ToJson(new ShapeCommand());
        PipeConnection_Client.Instasnce.SendMessage(msg);
    }
    public void ResetPlanet()
    {
        string msg = CommandingClasses.Commands.PlanetReset.ToString();
        PipeConnection_Client.Instasnce.SendMessage(msg);
    }
    public void MoveCamera(int val)
    {
        CommandGeneratorMeteors.MoveCamera(val);
    }
    public void ClosePlanetApp()
    {
        CommandGeneratorMeteors.SendCommand(CommandingClasses.Commands.Quit.ToString());
    }
}
