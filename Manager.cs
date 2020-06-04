using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UnityEngine.UI;
using System.Threading.Tasks;
using UnityEngine.SceneManagement;
using Photon.Realtime;
using UnityEngine.Networking;

public class Manager : MonoBehaviourPunCallbacks
{
    GameObject NickNameField;                                       
    GameObject ChatText;
    GameObject RoomNameField;
    GameObject RoomName;
    GameObject Create;
    GameObject AutoConnectToggle;
    GameObject[] pausebuts;
    bool isPaused = false;


    static public string MyNickName;

    public void Awake()                                                 //скрываем пауз меню
    {
        AutoConnectToggle = GameObject.Find("AutoConnectToggle");
        pausebuts = GameObject.FindGameObjectsWithTag("hideobjects");
        foreach (GameObject gameobj in pausebuts)
        {
            gameobj.SetActive(false);
        }
    }

    private void Start()                                                //инизиализация
    {
        ChatText = GameObject.Find("ChatText");
        RoomNameField = GameObject.Find("RoomNameField123");
        RoomName = GameObject.Find("RoomName");
        Create = GameObject.Find("Create");
        NickNameField = GameObject.Find("NickNameField");
        GameObject.Find("NickName").GetComponent<InputField>().text = PlayerPrefs.GetString("NickName");    //выставляем пользовательские настройки

        if (PlayerPrefs.GetInt("AutoConnect") == 1)
        {
            AutoConnectToggle.GetComponent<Toggle>().isOn = true;
            ConnectToMaster();
        }
        RoomName.GetComponent<InputField>().text = PlayerPrefs.GetString("RoomName");

    }
    public void ConnectToMaster()                                                                               //подключение к серверам фотона
    {
        if (PlayerPrefs.HasKey("NickName"))
            NickNameField.GetComponent<Text>().text = PlayerPrefs.GetString("NickName");


        if (NickNameField.GetComponent<Text>().text != "")
        {
            PhotonNetwork.NickName = NickNameField.GetComponent<Text>().text;
            PhotonNetwork.ConnectUsingSettings();
        }
        else
        {
            log("Enter your name");
        }
    }

    public override void OnConnectedToMaster()
    {

        GameObject.Find("NickName").GetComponent<InputField>().readOnly = true;
        NickNameField.GetComponent<Text>().color = Color.green;
        
        MyNickName = PhotonNetwork.NickName;
        log("Logged in!");
        PlayerPrefs.SetString("NickName", MyNickName);
    }

    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        if (message == "No match found")
            log("No available rooms found");
        else
            log(message + returnCode.ToString());
    }

    public override void OnJoinedRoom()
    {
        ChatText.GetComponent<Text>().text = "";
        if (PhotonNetwork.IsMasterClient)
        {
            GameObject.Find("RoomName").GetComponent<InputField>().text = PhotonNetwork.CurrentRoom.Name;
            log($"Room {PhotonNetwork.CurrentRoom.Name} created, players can join");
            GameObject.Find("Create").GetComponent<Button>().interactable = false;
        }
        else
        {
            log("Connected to " + PhotonNetwork.CurrentRoom.Name);
            PlayerPrefs.SetString("RoomName", PhotonNetwork.CurrentRoom.Name);
        }
        RoomName.GetComponent<InputField>().readOnly = true;
        RoomNameField.GetComponent<Text>().color = Color.yellow;
        PlayerPrefs.SetString("RoomName", PhotonNetwork.CurrentRoom.Name);

    }
    public override void OnLeftRoom()
    {
        RoomName.GetComponent<InputField>().readOnly = false;
        RoomNameField.GetComponent<Text>().color = Color.black;
        ChatText.GetComponent<Text>().text = "";
        Create.GetComponent<Button>().interactable = true;
        RoomName.GetComponent<InputField>().text = "";
    }

    public void CreateRoom()
    {
        if (RoomNameField.GetComponent<Text>().text == "")
            PhotonNetwork.CreateRoom(PhotonNetwork.NickName, new RoomOptions { MaxPlayers = 5 });
        else
            PhotonNetwork.CreateRoom(RoomNameField.GetComponent<Text>().text, new RoomOptions { MaxPlayers = 5 });
    }


    public void JoinRoom()
    {
        if (RoomNameField.GetComponent<Text>().text != "")
            PhotonNetwork.JoinRoom(RoomNameField.GetComponent<Text>().text);
        else
            PhotonNetwork.JoinRandomRoom();
    }

    public override void OnJoinRoomFailed(short returnCode, string message)
    {
        if (message == "Game does not exist")
            log("Room does not exist!");
        else
            log($"{ message} {returnCode.ToString()}");

    }
    public void LeaveRoom()
    {
        PlayerPrefs.SetString("RoomName", PhotonNetwork.CurrentRoom.Name);
        PhotonNetwork.LeaveRoom();
        
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        log(newPlayer.NickName + " Joined");
    }
    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        log(otherPlayer.NickName + " Left");
    }
    public override void OnCreateRoomFailed(short returnCode, string message)
    {
        log(message);
    }

    public void log(string mes)                                                     
    {
        ChatText.GetComponent<Text>().text += mes;
        ChatText.GetComponent<Text>().text += "\n";
    }

    public void Pause()
    {
        isPaused = !isPaused;
        if (isPaused)
        {
            foreach (GameObject gameobj in pausebuts)
            {
                gameobj.SetActive(true);
            }
        }
        else
        {
            foreach (GameObject gameobj in pausebuts)
            {
                gameobj.SetActive(false);
            }
        }
    }
    public void AppQuit()
    {
        SavePrefs();
        if (PhotonNetwork.InRoom)
        LeaveRoom();
        Application.Quit(0);
    }

    public void SavePrefs()
    {
        PlayerPrefs.Save();
    }

    public void AutoConnect()                                       
    {
        if (AutoConnectToggle.GetComponent<Toggle>().isOn)
            PlayerPrefs.SetInt("AutoConnect", 1);
        else
            PlayerPrefs.SetInt("AutoConnect", 0);
    }
}
