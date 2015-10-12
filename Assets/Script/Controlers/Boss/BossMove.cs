using UnityEngine;
using System.Collections;

public class BossMove : MonoBehaviour
{
    public delegate void moveDirDelegate(moveDir d);
    public moveDirDelegate onMoveChange, onMoveChangeEarly;
    public enum moveDir
    {
        left = -1,
        none = 0,
        right= 1
    }

    Vector3 startPos;
    public Transform endPos;
    public float SpeedForward = 0.5f,SpeedBackward = 4f;
    bool alive;
    public bool forceMove;
    bool paused;

    void Start()
    {
        startPos = transform.position;
        endPos.position = new Vector3(endPos.position.x, transform.position.y, transform.position.z);
        StartCoroutine(bossMove());

        GameManager.instance.onPauseGame += Instance_onPauseGame;
    }

    private void Instance_onPauseGame(bool b)
    {
        paused = b;
    }

    public void OnDestroy()
    {
        if(enabled && GameManager.instance)
            GameManager.instance.onPauseGame -= Instance_onPauseGame;
        alive = false;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.T))
            forceMove = true;
    }

    IEnumerator bossMove()
    {
        if (alive)
            yield break;

        alive = true;
        float t = 2f;
        int dir = 0;
        while (alive)
        {
            while (t > 0)
            {
                if (!paused)
                    t -= Time.deltaTime;
                yield return new WaitForEndOfFrame();
            }
            t = 2f;

            if (forceMove || util.RandomChange.getChance(100, 5))
            {

                

                if (Vector3.Distance(transform.position, endPos.position) > 0.3f)
                    dir = 1;
                else if (Vector3.Distance(transform.position, startPos) > 0.3f)
                    dir = -1;
                else
                    dir = 0;

                //send message that warns other systems that boss wil sprint forward
                if (onMoveChangeEarly != null)
                    onMoveChangeEarly(moveDir.right);

                t = 3f;

                //waiting while player picks up the powerup
                while (t > 0)
                {
                    if (!paused)
                        t -= Time.deltaTime;
                    yield return new WaitForEndOfFrame();
                }

                //warn other systems that boss is moving, this includes bossControler
                if (onMoveChange != null)
                    onMoveChange(moveDir.right);
                Debug.Log(dir);
                //Move forward
                while (dir==1 && Vector3.Distance(transform.position, endPos.position) > 0.3f)
                {
                    if (!paused)
                    {
                            transform.Translate(Vector3.right * Random.Range(0.6f, 2f) * SpeedForward * Time.deltaTime);                         
                    }
                    yield return new WaitForEndOfFrame();
                }
                Debug.Log(dir);
                //wait a lil bit
                t = Random.Range(4f, 6f);
                while (t > 0)
                {
                    if (!paused)
                        t -= Time.deltaTime;
                    yield return new WaitForEndOfFrame();
                }
                Debug.Log(dir);
                //send message that warns other systems that boss wil sprint forward
                if (onMoveChangeEarly != null)
                    onMoveChangeEarly(moveDir.left);

                if (onMoveChange != null)
                    onMoveChange(moveDir.left);

                dir = -1;

                Debug.Log(dir);

                //move backwards
                while (dir == -1 && Vector3.Distance(transform.position, startPos) > 0.3f)
                {
                    if (!paused)
                    {
                        transform.Translate(Vector3.left * Random.Range(0.6f, 2f) * SpeedBackward * Time.deltaTime);
                    }
                    yield return new WaitForEndOfFrame();
                }

                t = 2f;
                //Say that obss stoped moving
                if (onMoveChange != null)
                    onMoveChange(moveDir.none);

                forceMove = false;
            }
            
        }
    }
}
