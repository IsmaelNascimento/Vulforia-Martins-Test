using UnityEngine;
using Vuforia;

/// <summary>
/// This MonoBehaviour implements the Cloud Reco Event handling for this sample.
/// It registers itself at the CloudRecoBehaviour and is notified of new search results.
/// </summary>
public class SimpleCloudHandler : MonoBehaviour, ICloudRecoEventHandler
{
    #region Variables

    // ImageTracker reference to avoid lookups
    private ObjectTracker mImageTracker;

    /// <summary>
    /// can be set in the Unity inspector to reference a ImageTargetBehaviour that is used for augmentations of new cloud reco results.
    /// </summary>
    public ImageTargetBehaviour ImageTargetTemplate;

    #endregion

    #region Methods MonoBehaviour

    private void Start()
    {
        // Register for events at the CloudRecoBehaviour
        GetComponent<CloudRecoBehaviour>().RegisterEventHandler(this);
    }

    #endregion

    #region ICloudRecoEventHandler

    /// <summary>
    /// called when an error is reported during initialization
    /// </summary>
    public void OnInitError(TargetFinder.InitState initError)
    {
    }

    /// <summary>
    /// called when the CloudRecoBehaviour has finished initializing
    /// </summary>
    public void OnInitialized()
    {
        // get a reference to the Image Tracker, remember it
        mImageTracker = TrackerManager.Instance.GetTracker<ObjectTracker>();
    }

    /// <summary>
    /// called when a new search result is found
    /// </summary>
    /// <param name="targetSearchResult"></param>
    public void OnNewSearchResult(TargetFinder.TargetSearchResult targetSearchResult)
    {
        // duplicate the referenced image target
        GameObject newImageTarget = Instantiate(ImageTargetTemplate.gameObject) as GameObject;

        // enable the new result with the same ImageTargetBehaviour:
        ImageTargetBehaviour imageTargetBehaviour = mImageTracker.TargetFinder.EnableTracking(targetSearchResult, newImageTarget);

        print("Metadata value is " + targetSearchResult.MetaData);
    }

    /// <summary>
    /// called when the CloudRecoBehaviour starts or stops scanning
    /// </summary>
    public void OnStateChanged(bool scanning)
    {
    }

    /// <summary>
    /// called when an error is reported while updating
    /// </summary>
    public void OnUpdateError(TargetFinder.UpdateState updateError)
    {
    }

    #endregion
}