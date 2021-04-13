using UnityEngine;
using System.Collections;

public class PlayerDeath : MonoBehaviour
{
    public delegate void VoidDelegate();

    public event VoidDelegate onAnimationDone;

    private bool eventSend = false;

    public PlayerAnimationControler playerAni;
    public BackgroundControler back;
    public PlayerControler playerControler;
    public GameObject Skeleton;
    public Vector3 OffsetSkeletonCauseDeathMove;
    private Rigidbody2D ri;

    private int rayMask = 0;

    private float maxHeight = 0;

    private void Start()
    {
        ri = playerAni.GetComponent<Rigidbody2D>();
        rayMask = 1 << LayerMask.NameToLayer("Ground");

        if (!back)
            back = FindObjectOfType<BackgroundControler>();
        if (!playerControler)
            playerControler = FindObjectOfType<PlayerControler>();
        enabled = false;
    }

    public void StartAni()
    {
        enabled = true;
    }

    public void OnEnable()
    {
        if (!ri)
            return;
        ri.AddForce(new Vector3(0, 9 * ri.mass * ri.gravityScale, 0), ForceMode2D.Impulse);

        playerAni.GetComponent<PlayerControler>().enabled = false;
        playerAni.enabled = false;

        playerAni.Player_Death();

        eventSend = false;
        back.SpeedMult = 0;

        Skeleton.transform.localPosition = OffsetSkeletonCauseDeathMove;

        Debug.Log("Started ANI");
    }

    private void Update()
    {
        RaycastHit2D hit = Physics2D.Raycast(ri.transform.position + playerControler.distOff, Vector2.down, Mathf.Infinity, rayMask);
        Debug.Log(hit.distance);
        util.Debugger.Log("Player ground dist ", hit.distance);
        if (hit.distance > maxHeight)
            maxHeight = hit.distance;

        if (hit.distance < maxHeight && hit.distance < 0.1f)
        {
            if (eventSend)
                return;

            eventSend = true;
            if (onAnimationDone != null)
                onAnimationDone();
            Debug.Log("send event");
        }
    }
}