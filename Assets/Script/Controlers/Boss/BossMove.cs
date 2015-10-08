using UnityEngine;
using System.Collections;

public class BossMove : MonoBehaviour
{
    public delegate void moveDirDelegate(moveDir d);
    public moveDirDelegate onMoveChange;

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
        if(enabled)
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

                int dir;
                if (Vector3.Distance(transform.position, endPos.position) > 0.3f)
                {
                    dir = 1;
                    if (onMoveChange != null)
                        onMoveChange(moveDir.right);
                }
                else if (Vector3.Distance(transform.position, startPos) > 0.3f)
                {
                    dir = -1;
                    if (onMoveChange != null)
                        onMoveChange(moveDir.left);
                }
                else
                    dir = 0;
                //Debug.Log("dir " + dir + " z " + z);
                
                while (dir==1 && Vector3.Distance(transform.position, endPos.position) > 0.3f || dir == -1 && Vector3.Distance(transform.position,startPos)>0.3f)
                {
                    if (!paused)
                    {
                        if (dir == 1)
                            transform.Translate(Vector3.right * Random.Range(0.6f, 2f) * SpeedForward * Time.deltaTime * dir);
                        else if (dir == -1)
                            transform.Translate(Vector3.right * Random.Range(0.6f, 2f) * SpeedBackward * Time.deltaTime * dir);
                    }
                    yield return new WaitForEndOfFrame();

                }

                if (onMoveChange != null)
                    onMoveChange(moveDir.none);

                forceMove = false;
            }
            
        }
    }
}
