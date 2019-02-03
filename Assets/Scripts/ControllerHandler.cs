using UnityEngine;
using Vuforia;

/// <summary>
///     A custom handler that implements the ITrackableEventHandler interface.
/// </summary>
public class ControllerHandler : MonoBehaviour, ITrackableEventHandler
{

    private TrailDrawer lineReset;
    private LevelChanger levelChanger;

    #region PROTECTED_MEMBER_VARIABLES

    protected TrackableBehaviour mTrackableBehaviour;

    #endregion // PROTECTED_MEMBER_VARIABLES

    #region UNITY_MONOBEHAVIOUR_METHODS

    protected virtual void Start() {
        mTrackableBehaviour = GetComponent<TrackableBehaviour>();
        if (mTrackableBehaviour) {
            mTrackableBehaviour.RegisterTrackableEventHandler(this);

        }

        lineReset = GameObject.FindGameObjectWithTag("GameController").GetComponent<TrailDrawer>();
        levelChanger = GetComponent<LevelChanger>();
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
    }

    #endregion // PUBLIC_METHODS

    #region PROTECTED_METHODS

    /// <summary>
    /// Reset the ball position and current drawn line then change the level
    /// </summary>
    protected virtual void OnTrackingFound() {

        BallReset reset = GameObject.FindGameObjectWithTag("Player").GetComponent<BallReset>();

        lineReset.points.Clear();
        reset.ResetBall();
        levelChanger.ChangeLevel();
    }

    #endregion // PROTECTED_METHODS
}
