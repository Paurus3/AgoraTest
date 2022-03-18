using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AgoraUI : MonoBehaviour
{
    [SerializeField] private AgoraChat chat;
    [Header ("Panels")]
    public GameObject panelRoom;
    public GameObject panelVideoChat;
    
    [Header ("Buttons")]
    [SerializeField] private Button enterButton;
    [SerializeField] private Button leaveButton;
    [SerializeField] private Button joinButton;

    [Header ("Buttons")]
    public InputField inputRoom;

    // Start is called before the first frame update
    void Start()
    {
        joinButton.onClick.AddListener(chat.Join);
        leaveButton.onClick.AddListener(chat.Leave);
        enterButton.onClick.AddListener(chat.EnterRoom);

        //INit
        inputRoom.text = chat.channelName;

        panelRoom.SetActive(true);
        panelVideoChat.SetActive(false);
    }
}
