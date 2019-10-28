using UnityEngine;
using UnityEngine.SceneManagement;

public class Utils : MonoBehaviour
{
    private const string nameScreenLoad = "CanvasLoad";
    private static GameObject canvasLoadAux;

    private void Start()
    {
        //Screen.orientation = ScreenOrientation.Portrait;
    }

    public static void LoadSceneStatic(string scene)
    {
        ActiveScreenLoad();
        SceneManager.LoadScene(scene);
    }

    public void LoadScene(string scene)
    {
        ActiveScreenLoad();
        SceneManager.LoadScene(scene);
    }

    public static void ActiveScreenLoad()
    {
        if(canvasLoadAux != null)
            DisableScreenLoad();

        var canvasLoad = Resources.Load(nameScreenLoad) as GameObject;
        var canvasLoadInScene = Instantiate(canvasLoad);
        canvasLoadAux = canvasLoadInScene;
    }

    public static void DisableScreenLoad()
    {
        Destroy(canvasLoadAux);
    }
}