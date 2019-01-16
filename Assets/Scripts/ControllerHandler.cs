using UnityEngine;
using Vuforia;

/// <summary>
///     A custom handler that implements the ITrackableEventHandler interface.
/// </summary>
public class ControllerHandler : MonoBehaviour, ITrackableEventHandler
{

    public GameObject level1;
    public GameObject level2;
    public GameObject level3;
    public GameObject level4;

    private GameObject lineReset;

    #region PROTECTED_MEMBER_VARIABLES

    protected TrackableBehaviour mTrackableBehaviour;

    #endregion // PROTECTED_MEMBER_VARIABLES

    #region UNITY_MONOBEHAVIOUR_METHODS

    protected virtual void Start() {
        mTrackableBehaviour = GetComponent<TrackableBehaviour>();
        if (mTrackableBehaviour) {
            mTrackableBehaviour.RegisterTrackableEventHandler(this);

        }

        lineReset = GameObject.FindGameObjectWithTag("GameController");
    }

    protected virtual void OnDestroy() {
        if (mTrackableBehaviour)
            mTrackableBehaviour.UnregisterTrackableEventHandler(this);
    }

    #endregion // UNITY_MONOBEHAVIOUR_METHODS

    #region PUBLIC_METHODS

    /// <summary>
    ///     Implementation of the ITrackableEventHandler function called when the
    ///     tracking state changes.
    /// </summary>
    public void OnTrackableStateChanged(
        TrackableBehaviour.Status previousStatus,
        TrackableBehaviour.Status newStatus) {
        if (newStatus == TrackableBehaviour.Status.DETECTED ||
            newStatus == TrackableBehaviour.Status.TRACKED ||
            newStatus == TrackableBehaviour.Status.EXTENDED_TRACKED) {
 
            OnTrackingFound();
        }
        else if (previousStatus == TrackableBehaviour.Status.TRACKED &&
                 newStatus == TrackableBehaviour.Status.NOT_FOUND) {

            OnTrackingLost();
        }
        else {
            // For combo of previousStatus=UNKNOWN + newStatus=UNKNOWN|NOT_FOUND
            // Vuforia is starting, but tracking has not been lost or found yet
            // Call OnTrackingLost() to hide the augmentations
            OnTrackingLost();
        }
    }

    #endregion // PUBLIC_METHODS

    #region PROTECTED_METHODS

    protected virtual void OnTrackingFound() {

        lineReset.GetComponent<DrawLineWithCollider>().points.Clear();
        GameObject ball = GameObject.FindGameObjectWithTag("Player");
        Rigidbody2D rb = ball.GetComponent<Rigidbody2D>();
        Vector3 position = ball.GetComponent<BallReset>().startPosition;

        ball.transform.localPosition = position;
        rb.velocity = Vector2.zero;
        rb.angularVelocity = 0;

        if (level1.activeSelf) {

            level1.SetActive(false);
            level2.SetActive(true);
        }
        else if (level2.activeSelf) {

            level2.SetActive(false);
            level3.SetActive(true);
        }
        else if (level3.activeSelf) {

            level3.SetActive(false);
            level4.SetActive(true);
        }
        else if (level4.activeSelf) {

            level4.SetActive(false);
            level1.SetActive(true);
        }

    }

    protected virtual void OnTrackingLost() {

    }

    #endregion // PROTECTED_METHODS
}
