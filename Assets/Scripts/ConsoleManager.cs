using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ConsoleManager : MonoBehaviour
{
    public Text TCEConsole;
    public Text TCECConsole;
    private void Awake()
    {
        TCEConsole.text = "";
        TCECConsole.text = "";
    }
    public void PrintTceConsole(string message)
    {
        TCEConsole.text += message + '\n';
    }
    public void PrintTcecConsole(string message)
    {
        TCECConsole.text += message + '\n';
    }
}
