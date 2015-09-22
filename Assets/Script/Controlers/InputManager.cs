using UnityEngine;
using System.Collections;

public class InputManager : MonoBehaviour
{

    public delegate void VoidDelegate();
    /// <summary>
    /// Thown When player swipes up
    /// </summary>
    public event VoidDelegate onSwipeUp;
    /// <summary>
    /// When player presses the back button on there phone
    /// </summary>
    public event VoidDelegate onEscapePressed;
    public delegate void screenTap(Vector2 pos);
    /// <summary>
    /// Thrown when player taps anywhere on the screen
    /// </summary>
    public event screenTap onTap;

    [SerializeField]
    private bool DebugTouches = false;
    bool eventActive;
    // Use this for initialization


    void Start()
    {
        GameManager.instance.onPauseGame += Instance_onPauseGame;
        Instance_onPauseGame(false);
    }

    private void Instance_onPauseGame(bool b)
    {
        setReconizers(!b);
    }

    public void OnLevelWasLoaded(int level)
    {
        if (level == 1)
            setReconizers(false);
    }   

    void setReconizers(bool b)
    {
        Debug.Log(b ? "Removing Reconziers" : "Adding Reconziers");
        if (!b && !eventActive)
        {
            eventActive = false;
            TouchKit.removeAllGestureRecognizers();
            return;
        }
        if (eventActive)
            return;

        eventActive = true;
        TKSwipeRecognizer r1 = new TKSwipeRecognizer(TKSwipeDirection.Up);
        r1.gestureRecognizedEvent += (r) =>
        {
            if (DebugTouches)
                Debug.Log("swipe recognizer fired: " + r);

            if (onSwipeUp != null)
                onSwipeUp();
        };
        TouchKit.addGestureRecognizer(r1);

        TKTapRecognizer r2 = new TKTapRecognizer();

        // we can limit recognition to a specific Rect, in this case the bottom-left corner of the screen

        r2.gestureRecognizedEvent += (r) =>
        {
            if (DebugTouches)
                Debug.Log("tap recognizer fired: " + r);

            if (onTap != null)
                onTap(r.touchLocation());
        };
        TouchKit.addGestureRecognizer(r2);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
            if (onEscapePressed != null)
                onEscapePressed();
    }
}
