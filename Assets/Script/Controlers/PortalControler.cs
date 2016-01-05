using UnityEngine;
using util;
using System.Collections;

public class PortalControler : MonoBehaviour
{
    public delegate void VoidDelegate();
    public event VoidDelegate onMoveDone;

    public Transform p;
    public Vector3 moveSpeed;
    public BackgroundControler b;
    public float backgroundSpeedMultStart;

    bool allowPlayerMove;
    bool moveComplete;
    bool playerOutOfScreen;

    Vector3 endpos;
    
    // Use this for initialization
    void Start()
    {
        p.position = new Vector3((p.getChildBounds().size.x * 3) + MoveBoxScaler.screenSize.x, p.position.y);
        Debug.Log(new Vector3((p.getChildBounds().size.x * 3) + MoveBoxScaler.screenSize.x, p.position.y));

        endpos = new Vector3(p.getChildBounds().size.x + MoveBoxScaler.screenSize.x, p.position.y);
        b = FindObjectOfType<BackgroundControler>();
        enabled = false;

    }

    float d;
    // Update is called once per frame

    
    void Update()
    {
        move_portal();
        move_player();
    }

    void move_portal()
    {
        if (moveComplete)
            return;

        d = Vector3.Distance(p.position, endpos);
        if (d < 6)
        {
            allowPlayerMove = true;
            if (d > 0.05f)
            {
                
                p.Translate(moveSpeed * Time.deltaTime);

                if (b.SpeedMult > backgroundSpeedMultStart)
                    backgroundSpeedMultStart = b.SpeedMult;
                b.SpeedMult = (d / 6f) * backgroundSpeedMultStart;
            }
            else
            {
                b.SpeedMult = 0f;
               

                d = 0;
                moveComplete = true;
            }
        }
        else
            p.Translate(moveSpeed * Time.deltaTime);
    }
    void move_player()
    {
        util.Debugger.Log("playerScreenMove ", (((12 * (1 - (d / 6f))) * Time.deltaTime)));
        util.Debugger.Log("playerScreenPos ", GameManager.playerControler.transform.position.x);
        if (allowPlayerMove)
        {
            GameManager.playerControler.ChangePlayerPos(GameManager.playerControler.transform.position.x + ((12 * (1 - (d / 6f))) * Time.deltaTime));
            GameManager.playerControler.enabled = false;

            if (GameManager.playerControler.transform.position.x >= 20)
                if (onMoveDone != null)
                    onMoveDone();
        }
    }
    public void StartMove()
    {
        enabled = true;
        moveComplete = false;
    }
}
