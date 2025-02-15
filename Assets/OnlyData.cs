using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using TMPro;

public class OnlyData : MonoBehaviour
{
    public GameType gametype;
    public GameObject LoadingPanel;

    public static OnlyData Data;
    public bool AlreadyPlayedGames;
    public string Playername;

    void Awake()
    {
        if (Data == null)
        {
            Data = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void PPMode()
    {
        gametype = GameType.pass;
    }

    public void MultiMode()
    {
        gametype = GameType.Multi;
    }

    public void boolFN()
    {
        AlreadyPlayedGames = true;
    }

}
public enum GameType
{
    Multi,
    pass,
    None
}
