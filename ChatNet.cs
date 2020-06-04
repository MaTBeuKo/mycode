using Photon.Pun;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine.Networking;
using Photon.Realtime;
using System.IO;

public class ChatNet : MonoBehaviour
{
    GameObject chat;                                        
    static public PhotonView photonView;
    GameObject MessageInput;
    Player[] playerlist;

    private void Awake()
    {

    }
    public void Start()
    {
        MessageInput = GameObject.Find("ChatInput");
        photonView = GetComponent<PhotonView>();
        chat = GameObject.Find("ChatText");
    }

    public void Send()                                                                                              //отправить сообщение
    {

        if (MessageInput.GetComponent<Text>().text != "")
        {
            if (MessageInput.GetComponent<Text>().text[0] != '.')
            {
                photonView.RPC("SendMessage", RpcTarget.AllViaServer, Manager.MyNickName, MessageInput.GetComponent<Text>().text);

            }
            else
            {                                                                                                       //или выполнить команду
                
                playerlist = PhotonNetwork.PlayerList;
                if (MessageInput.GetComponent<Text>().text == ".list")                                              //вывести список участников чата
                {
                    chat.GetComponent<Text>().text += "Players online:\n";
                    for (int i = 0; i < playerlist.Length; i++)
                    {
                        chat.GetComponent<Text>().text += playerlist[i].NickName;
                        if (playerlist[i].IsMasterClient)
                            chat.GetComponent<Text>().text += " (Owner)";
                        chat.GetComponent<Text>().text += ", ";
                    }
                    chat.GetComponent<Text>().text += "\n";
                }

                else if (MessageInput.GetComponent<Text>().text.Split()[0] == ".kick")                              //выгнать участника
                {
                    if (PhotonNetwork.IsMasterClient)
                    {
                        for (int i = 0; i < playerlist.Length; i++)
                        {
                            if (playerlist[i].NickName == MessageInput.GetComponent<Text>().text.Split()[1])
                            {
                                PhotonNetwork.CloseConnection(playerlist[i]);
                                break;
                            }
                        }
                    }
                    else
                    {
                        chat.GetComponent<Text>().text += "Only creator able to kick \n";
                    }
                }
                else if (MessageInput.GetComponent<Text>().text == ".clear")                                        //очистить чат
                {
                    chat.GetComponent<Text>().text = "";
                }
                else if (MessageInput.GetComponent<Text>().text == ".help")                                         //помощь
                {
                    chat.GetComponent<Text>().text += "Commands list: \n.list - list of the players in your room \n.kick (playername) - for room creator only \n.clear - clear the chat \n";
                }

            }
        }
        GameObject.Find("ChatInputField").GetComponent<InputField>().text = "";
    }



    int i = 1;
    [PunRPC]
    void SendMessage(string name, string mes)                                                           //выводит полученное сообщение
    {
        chat.GetComponent<Text>().text += $"{DateTime.Now.Hour}:{ DateTime.Now.Minute} {name}: {mes}";
        chat.GetComponent<Text>().text += "\n";
    }


}
