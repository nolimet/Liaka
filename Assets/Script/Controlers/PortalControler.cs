using UnityEngine;
using util;
using System.Collections;

public class PortalControler : MonoBehaviour
{
    public delegate void VoidDelegate();
    public event VoidDelegate onInScreen;

    public Transform p;
    public Vector3 moveSpeed;
    public BackgroundControler b;
    public float backgroundSpeedMultStart;

    Vector3 endpos;
    
    // Use this for initialization
    void Start()
    {
        p.position = new Vector3((p.getChildBounds().size.x * 3) + MoveBoxScaler.screenSize.x, p.position.y);
        Debug.Log(new Vector3((p.getChildBounds().size.x * 3) + MoveBoxScaler.screenSize.x, p.position.y));

        endpos = new Vector3(p.getChildBounds().size.x + MoveBoxScaler.screenSize.x, p.position.y);
        b = FindObjectOfType<BackgroundControler>();
        //enabled = false;

    }

    float d;
    // Update is called once per frame
    void Update()
    {
        d = Vector3.Distance(p.position, endpos);
        if (d< 6)
        {
            if (d > 0.05f)
            {
                p.Translate(moveSpeed * Time.deltaTime);
                if (onInScreen != null)
                    onInScreen();
                if (b.SpeedMult > backgroundSpeedMultStart)
                    backgroundSpeedMultStart = b.SpeedMult;
                b.SpeedMult = (Vector3.Distance(p.position, endpos) / 6f) * backgroundSpeedMultStart;
            }
            else
            {
                b.SpeedMult = 0f;
            }
        }
        else
            p.Translate(moveSpeed * Time.deltaTime);

        util.Debugger.Log("test value", 5 / Time.deltaTime);
    }

    public void StartMove()
    {
        enabled = true;
    }
}
