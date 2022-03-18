using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using agora_gaming_rtc;
using agora_utilities;


public class AgoraChat : MonoBehaviour
{
    public string appID;
    public string token;
    public string channelName = "futura";

    [Header("Cameras")]
    [Space(10)]
    public GameObject myCamera;
    public GameObject remoteCamera;

    [Header("MyView")]
    [Space(10)]
    public GameObject videoSurface;


    [SerializeField] private AgoraUI uiControler;


    VideoSurface myView;
    VideoSurface remoteView;
    IRtcEngine mRtcEngine;

    
    void Start()
    {
        SetupAgora();
    }

    public void EnterRoom()
    {
        channelName = uiControler.inputRoom.text;
        uiControler.panelRoom.SetActive(false);
        uiControler.panelVideoChat.SetActive(true);
    }


    public void Join()
    {
        //Active View Canvas
        myCamera.SetActive(true);

        myView.SetEnable(true);
        mRtcEngine.JoinChannel(channelName, "", 0);
        mRtcEngine.EnableVideo();
        mRtcEngine.EnableVideoObserver();
        
        
    }

    public void ScreenSharing()
    {
        mRtcEngine.StartScreenCaptureForWeb();
    }

    public void Leave()
    {
        //Desactivate View Canvas
        myCamera.SetActive(false);

        mRtcEngine.LeaveChannel();
        mRtcEngine.DisableVideo();
        mRtcEngine.DisableVideoObserver();

        //Sale de la sala
        uiControler.panelRoom.SetActive(true);
        uiControler.panelVideoChat.SetActive(false);
    }

    void OnJoinChannelSuccessHandler(string channelName, uint uid, int elapsed)
    {
        // can add other logics here, for now just print to the log
        Debug.LogFormat("Joined channel {0} successful, my uid = {1}", channelName, uid);
    }

    void OnLeaveChannelHandler(RtcStats stats)
    {
        myView.SetEnable(false);
        if (remoteView != null)
        {
            remoteView.SetEnable(false);
        }
    }

    void OnUserJoined(uint uid, int elapsed)
    {
        GameObject go = GameObject.Find("RemoteView");

        if (remoteView == null)
        {
            remoteView = go.AddComponent<VideoSurface>();
        }

        remoteCamera.SetActive(true);

        remoteView.SetForUser(uid);
        remoteView.SetEnable(true);
        remoteView.SetVideoSurfaceType(AgoraVideoSurfaceType.RawImage);
        remoteView.SetGameFps(60);
    }

    void OnUserOffline(uint uid, USER_OFFLINE_REASON reason)
    {
        remoteCamera.SetActive(false);
        remoteView.SetEnable(false);
    }


    void SetupAgora()
    {
        //GameObject go = GameObject.Find("MyView");
        myView = videoSurface.AddComponent<VideoSurface>();
        print("Setup Agora...");
        mRtcEngine = InitEngine();
    }

    private IRtcEngine InitEngine()
    {
        //Creating Instace Engine
        agora_gaming_rtc.IRtcEngine mRtcEngine = IRtcEngine.GetEngine(appID);
        mRtcEngine.SetLogFile("log.txt");
        mRtcEngine.SetChannelProfile(CHANNEL_PROFILE.CHANNEL_PROFILE_LIVE_BROADCASTING);
        mRtcEngine.SetClientRole(CLIENT_ROLE_TYPE.CLIENT_ROLE_BROADCASTER);
        mRtcEngine.EnableAudio();
        mRtcEngine.EnableVideo();
        mRtcEngine.EnableVideoObserver();
        print("Agora Started, getting token...");
        mRtcEngine.RenewToken(token);
        print("Token Renew!!");
        
        //EVENTS!
        mRtcEngine.OnUserJoined += OnUserJoined;
        mRtcEngine.OnUserOffline += OnUserOffline;
        mRtcEngine.OnJoinChannelSuccess += OnJoinChannelSuccessHandler;
        mRtcEngine.OnLeaveChannel += OnLeaveChannelHandler;

        return mRtcEngine;
    }

    //Agora corre de forma nativa en C++. Con esta funcion nos aseguramos de limpar bien el recurso
    void OnApplicationQuit()
    {
        if (mRtcEngine != null)
        {
            IRtcEngine.Destroy();
            mRtcEngine = null;
        }
    }
}
