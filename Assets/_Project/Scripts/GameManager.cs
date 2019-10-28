using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.Networking;
using System.IO;

public class GameManager : MonoBehaviour
{
    #region Variables
    private static GameManager m_Instance;
    public static GameManager Instance
    {
        get
        {
            if (m_Instance == null)
                m_Instance = FindObjectOfType(typeof(GameManager)) as GameManager;

            return m_Instance;
        }
    }

    [SerializeField] private GameObject m_ButtonPanelColors;
    [SerializeField] private GameObject m_PanelColors;
    public GameObject m_Models3D;
    [SerializeField] private List<GameObject> m_AllFurnitures;

    private int m_IndexAllFurnitures = 0;

    [Header("AssetBundles")]
    public string m_UriBundle = "https://s3.amazonaws.com/prime-team/AssetBundles/Windows/models3d";
    public string m_UriVersionBundles = "https://s3.amazonaws.com/prime-team/AssetBundles/version.txt";
    public List<string> m_AllFurnituresNames;
    AssetBundle m_Bundle;
    private GameObject m_LastModelActive;
    private static bool m_IsLoadBundle;

    #endregion

    private void Start()
    {
        StartCoroutine(LoadBundles_Coroutine());
    }

    private void OnDestroy()
    {
        m_Bundle.Unload(false);
    }

    #region Methods Others

    private void ActiveOrDisableChangeColorObject()
    {
        if (m_AllFurnitures[m_IndexAllFurnitures].GetComponent<MeshRenderer>())
            m_ButtonPanelColors.SetActive(true);
        else
            m_ButtonPanelColors.SetActive(false);
    }
    
    #endregion

    #region ButtonsUIs

    public void ButtonNext()
    {
        m_IndexAllFurnitures++;

        if (m_IndexAllFurnitures >= m_AllFurnituresNames.Count)
        {
            m_IndexAllFurnitures = 0;
            //m_AllFurnitures[m_AllFurnitures.Count - 1].SetActive(false);
        }
        //else
        //    m_AllFurnitures[m_IndexAllFurnitures - 1].SetActive(false);

        //m_AllFurnitures[m_IndexAllFurnitures].SetActive(true);
        LoadModel3D(m_AllFurnituresNames[m_IndexAllFurnitures]);
        ActiveOrDisableChangeColorObject();
    }

    public void ButtonPrevious()
    {
        m_IndexAllFurnitures--;

        if (m_IndexAllFurnitures < 0)
        {
            m_IndexAllFurnitures = m_AllFurnituresNames.Count - 1;
            //m_AllFurnitures[0].SetActive(false);
        }
        //else
        //    m_AllFurnitures[m_IndexAllFurnitures + 1].SetActive(false);

        //m_AllFurnitures[m_IndexAllFurnitures].SetActive(true);
        LoadModel3D(m_AllFurnituresNames[m_IndexAllFurnitures]);
        ActiveOrDisableChangeColorObject();
    }

    public void ChangeColor(UnityEngine.UI.Image newColor)
    {
        m_LastModelActive.GetComponent<MeshRenderer>().material.color = newColor.color;
    }

    public void ActivePanelColors()
    {
        m_PanelColors.GetComponent<Animator>().Play("PanelColorsInput");
    }

    public void DisablePanelColors()
    {
        m_PanelColors.GetComponent<Animator>().Play("PanelColorsExit");
        m_LastModelActive.GetComponent<MeshRenderer>().material.color = Color.white;
    }

    public void ActiveObject(GameObject obj)
    {
        obj.SetActive(true);
    }

    public void DisableObject(GameObject obj)
    {
        obj.SetActive(false);
    }

    public void TakeAndShareScreenshot(GameObject obj)
    {
        StartCoroutine(TakeAndShareScreenshot_Coroutine(obj));
    }

    public void LaunchApp(string bundleId)
    {
        bool fail = false;
        AndroidJavaClass up = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
        AndroidJavaObject ca = up.GetStatic<AndroidJavaObject>("currentActivity");
        AndroidJavaObject packageManager = ca.Call<AndroidJavaObject>("getPackageManager");
        AndroidJavaObject launchIntent = null;

        try
        {
            launchIntent = packageManager.Call<AndroidJavaObject>("getLaunchIntentForPackage", bundleId);
        }
        catch (Exception e)
        {
            print(e);
            fail = true;
        }

        if (fail)
            print("Fail open app");
        else //open the app
            ca.Call("startActivity", launchIntent);

        up.Dispose();
        ca.Dispose();
        packageManager.Dispose();
        launchIntent.Dispose();
    }

    private void LoadModel3D(string nameModel, Action<GameObject> modelCreated = null)
    {
        Utils.ActiveScreenLoad();
        //m_Bundle.Unload(true);

        if (m_LastModelActive != null)
            Destroy(m_LastModelActive);

        GameObject model = m_Bundle.LoadAsset(nameModel) as GameObject;
        model.SetActive(true);
        var goInstantiate = Instantiate(model, m_Models3D.transform);
        m_LastModelActive = goInstantiate;

        if (modelCreated != null)
            modelCreated(goInstantiate);

        print("Load Model " + nameModel + " of AssetBundles");
        Utils.DisableScreenLoad();
    }

    #endregion

    #region Coroutines

    private IEnumerator TakeAndShareScreenshot_Coroutine(GameObject obj)
    {
        var nameScreenshot = "Screenshot_" + DateTime.Now.ToString("dd-mm-yyyy-hh-mm-ss") + ".png";
        ScreenCapture.CaptureScreenshot(nameScreenshot);
        yield return new WaitForEndOfFrame();
        obj.SetActive(true);
    }

    private IEnumerator GetBundles_Coroutine(int versionBundles)
    {
        UnityWebRequest request = UnityWebRequest.GetAssetBundle(m_UriBundle, (uint)versionBundles, 0);
        Utils.ActiveScreenLoad();
        yield return request.SendWebRequest();

        if (request.isNetworkError || request.isHttpError)
        {
            print(request.error);
            StartCoroutine(GetBundles_Coroutine(versionBundles));
        }
        else
        {
            m_Bundle = DownloadHandlerAssetBundle.GetContent(request);
            m_AllFurnituresNames = new List<string>();

            foreach (var bundleName in m_Bundle.GetAllAssetNames())
            {
                var nameBudle = Path.GetFileNameWithoutExtension(bundleName);
                m_AllFurnituresNames.Add(nameBudle);
            }

            LoadModel3D(m_AllFurnituresNames[0]);
            print("Bundle Load");
            Utils.DisableScreenLoad();
        }
    }

    private IEnumerator LoadBundles_Coroutine()
    {
        using (UnityWebRequest request = UnityWebRequest.Get(m_UriVersionBundles))
        {
            yield return request.SendWebRequest();

            if (request.isNetworkError || request.isHttpError)
            {
                print(request.error);
            }
            else
            {
                var versionBundles = int.Parse(request.downloadHandler.text);
                print("Version Bundles:: " + versionBundles);
                StartCoroutine(GetBundles_Coroutine(versionBundles));
            }
        }
    }

    #endregion
}