using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;
using Vuforia;

public class ContentVideoManager : MonoBehaviour, ITrackableEventHandler
{
    private static ContentVideoManager instance;
    public static ContentVideoManager Instance
    {
        get
        {
            if (instance == null)
                instance = FindObjectOfType(typeof(ContentVideoManager)) as ContentVideoManager;

            return instance;
        }
    }

    [SerializeField] private TrackableBehaviour imageTargetCurrent;
    [SerializeField] private UnityEngine.UI.Image buttonPlayPause;
    [SerializeField] private Sprite iconPlay;
    [SerializeField] private Sprite iconPause;
    [SerializeField] private VideoPlayer videoPlayer;
    [SerializeField] private GameObject planeRendererVideo;
    [SerializeField] private RenderTexture rendererVideo;
    [SerializeField] private UnityEngine.UI.Image buttonFullscreen;
    private bool isFullScreen = false;

    private void Start()
    {
        imageTargetCurrent.RegisterTrackableEventHandler(this);
        buttonFullscreen.color = Color.green;
        Screen.autorotateToPortrait = false;
        Screen.orientation = ScreenOrientation.LandscapeLeft;
    }

    public void OnButtonPlayPauseClicked()
    {
        if(videoPlayer.isPlaying)
        {
            videoPlayer.Pause();
            buttonPlayPause.sprite = iconPlay;
        }
        else
        {
            videoPlayer.Play();
            buttonPlayPause.sprite = iconPause;
        }
    }

    public void OnButtonVideoFullScreenClicked()
    {
        isFullScreen = !isFullScreen;

        if (!isFullScreen)
        {
            buttonFullscreen.color = Color.green;
            videoPlayer.renderMode = VideoRenderMode.MaterialOverride;
            videoPlayer.targetMaterialRenderer = planeRendererVideo.GetComponent<MeshRenderer>();
            planeRendererVideo.SetActive(true);
        }
        else
        {
            buttonFullscreen.color = Color.red;
            videoPlayer.renderMode = VideoRenderMode.CameraFarPlane;
            videoPlayer.targetCamera = Camera.main;
            planeRendererVideo.SetActive(false);
        }
    }

    public void TrackingFound()
    {
        Camera.main.targetTexture = rendererVideo;
        planeRendererVideo.GetComponent<MeshCollider>().enabled = true;
        planeRendererVideo.GetComponent<MeshRenderer>().enabled = true;
    }

    public void TrackingLost()
    {
        Camera.main.targetTexture = null;

        if (videoPlayer.isPlaying)
        {
            videoPlayer.Pause();
            buttonPlayPause.sprite = iconPlay;
        }
    }

    public void OnTrackableStateChanged(TrackableBehaviour.Status previousStatus, TrackableBehaviour.Status newStatus)
    {
        if (newStatus == TrackableBehaviour.Status.DETECTED ||
            newStatus == TrackableBehaviour.Status.TRACKED ||
            newStatus == TrackableBehaviour.Status.EXTENDED_TRACKED)
        {
            TrackingFound();
        }
        else if (previousStatus == TrackableBehaviour.Status.TRACKED &&
                 newStatus == TrackableBehaviour.Status.NOT_FOUND)
        {
            TrackingLost();
        }
        else
            TrackingLost();
    }

    public void OnButtonMenuClicked(string nameScene)
    {
        Utils.LoadSceneStatic(nameScene);
        Screen.autorotateToPortrait = true;
        Screen.orientation = ScreenOrientation.Portrait;
    }
}