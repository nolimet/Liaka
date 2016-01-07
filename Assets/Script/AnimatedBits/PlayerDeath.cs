using UnityEngine;
using System.Collections;

public class PlayerDeath : MonoBehaviour
{

    public delegate void VoidDelegate();
    public event VoidDelegate onAnimationDone;

    bool eventSend = false;

    public PlayerAnimationControler playerAni;
    public BackgroundControler back;
    Rigidbody2D ri;

    int rayMask = 0;

    float maxHeight = 0;

    void Start()
    {
        ri = playerAni.GetComponent<Rigidbody2D>();
        rayMask = 1 << LayerMask.NameToLayer("Ground");

        if (!back)
            back = FindObjectOfType<BackgroundControler>();
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

        playerAni.Player_Death();

        eventSend = false;
        back.SpeedMult = 0;

        Debug.Log("Started ANI");
    }

    RaycastHit2D rc2D;

    void Update()
    {
        rc2D = Physics2D.Raycast(ri.transform.position, Vector2.down, Mathf.Infinity, rayMask);

        if (rc2D.distance > maxHeight)
            maxHeight = rc2D.distance;

        if (rc2D.distance < maxHeight)
            if (rc2D.distance < 0.1f)
            {
                eventSend = true;
                if (onAnimationDone != null)
                    onAnimationDone();
            }

        rc2D = new RaycastHit2D();
    }
}
