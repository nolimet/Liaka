using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerControler : MonoBehaviour
{
    #region Delegates
    public delegate void PlayerControlerEvent(PlayerControler p);
    public static event PlayerControlerEvent onPlayerCreated;
    public static event PlayerControlerEvent onPlayerDestoryed;

    public delegate void VoidDelegate();
    public event VoidDelegate onShoot;
    public event VoidDelegate onJump;
    public event VoidDelegate onHitGround;
    public event VoidDelegate onEnergyZero;
    #endregion

    #region Varibles
    public PlayerGun gun;

    [SerializeField]
    bool g, doubleJump;

    [SerializeField, Tooltip("Distance the ground check ray travels")]
    float l = 1f;

    [SerializeField,Tooltip("STarting point of ground checkRay")]
    Vector3 distOff = Vector3.zero;
    [SerializeField]
    Transform SpeedUpEndPosition;
    [SerializeField]
    PlayerAnimationControler AnimationControler;

    Rigidbody2D rigi2d;
    Vector2 speed = Vector2.zero, startPos;

    float coolDownDelay;
    public float boostMoveSpeed = 10f, maxBoost = 10f, boostDegrationSpeed = 2.5f, boostTimeLeft = 0f, BoostTimeStay = 2f,BoostForwardTimePerUnit =1f;
    [Header("boostStuff"),SerializeField]
    float boostTimeStayLeft, BoostForwardCalculatedTimeTarget, BoostForwardTime,LastCalcBoostTime, lastPickedupBoostTime = 0;
    float maxForwardBoostTime = 0f;

    [HideInInspector]
    public float weaponHeat, MaxHeat = 100f;
    [HideInInspector]
    public float Energy, MaxEnergy = 100f;
    [HideInInspector]
    public bool weaponForcedCooldown;
    [HideInInspector]
    public float playerScreenX = 0f;
    bool gamePaused = false; 
    #endregion

    #region Unity functions
    void Start()
    {
        Energy = MaxEnergy;
        rigi2d = GetComponent<Rigidbody2D>();
        
        GameManager.inputManager.onSwipeUp += InputManager_onSwipeUp;
        GameManager.inputManager.onTap += InputManager_onTap;
        GameManager.instance.onPauseGame += Instance_onPauseGame;
        BaseObject.onHitPlayer += BaseObject_onHitPlayer;

        if (onPlayerCreated!=null)
            onPlayerCreated(this);

        playerScreenX = Camera.main.WorldToScreenPoint(transform.position).x;
        SpeedUpEndPosition.position = new Vector3(SpeedUpEndPosition.position.x, transform.position.x, transform.position.z);
        startPos = transform.position;
    }

    public void OnDestroy()
    {
        if (GameManager.inputManager)
        {
            GameManager.inputManager.onSwipeUp -= InputManager_onSwipeUp;
            GameManager.inputManager.onTap -= InputManager_onTap;
        }

        if(GameManager.instance)
            GameManager.instance.onPauseGame -= Instance_onPauseGame;
        BaseObject.onHitPlayer -= BaseObject_onHitPlayer;

        if (onPlayerDestoryed != null)
            onPlayerDestoryed(this);
    }

    void Update()
    {
        if (gamePaused)
            return;

        Update_Energy();
        Update_HeatLevel();
        Update_GroundCheck();
        Update_Boost();
    }
    #endregion

    #region eventListeners

    private void BaseObject_onHitPlayer(BaseObject.objectType o)
    {
        switch (o)
        {
            case BaseObject.objectType.Enemy:
                Energy -= 10;
                if (Energy < 0)
                    Energy = 0;
                
                break;
        }
    }

    private void Instance_onPauseGame(bool b)
    {
        gamePaused = b;
        if (b)
        {
            speed = rigi2d.velocity;
            rigi2d.velocity = Vector2.zero;
            rigi2d.isKinematic = true;
        }
        else
        {
            rigi2d.isKinematic = false;
            rigi2d.velocity = speed;
        }
    }

    private void InputManager_onTap(Vector2 pos)
    {
        shoot(pos);
    }

    private void InputManager_onSwipeUp()
    {
        if (g)
        {
            g = false;
            Jump();
        }
        else if (doubleJump)
        {
            doubleJump = false;
            Jump();
        }
    }

    //called by string method. Not exactly save...
    public void hitPickup(GameObject g)
    {
        if (g.GetComponent<BaseObject>().type == BaseObject.objectType.Pickup)
        {
            PickupBase p = g.GetComponent<PickupBase>();

            switch (p.pType)
            {
                case PickupBase.PickupType.Coin:
                    break;

                case PickupBase.PickupType.Energy:
                    Energy += 15;
                    if (Energy > MaxEnergy)
                        Energy = MaxEnergy;
                    break;

                case PickupBase.PickupType.SpeedUp:
                    lastPickedupBoostTime = boostTimeLeft;
                    boostTimeLeft +=0.5f;  
                    break;
            }
        }
    }
    #endregion

    #region UpdateFunctions
    void Update_Energy()
    {
        if (Energy > 0)
            Energy -= 2f * Time.deltaTime;

        if (Energy <= 0)
        {
            // Debug.Log("YOU DIED");
        }
    }
    void Update_Boost()
    {
        if (boostTimeLeft <= 0 || gamePaused)
            return;
        
        if (boostTimeLeft > maxBoost)
            Debug.Log("overBoost");

        if (BoostForwardTime < BoostForwardCalculatedTimeTarget || LastCalcBoostTime != lastPickedupBoostTime)
        {
            if(LastCalcBoostTime!=lastPickedupBoostTime)
            {
                boostTimeStayLeft = BoostTimeStay;

                maxForwardBoostTime = Vector3.Distance(startPos, SpeedUpEndPosition.position) * BoostForwardTimePerUnit;
                float normalBoost = boostTimeLeft / maxBoost;

                BoostForwardCalculatedTimeTarget = normalBoost * maxForwardBoostTime;

                LastCalcBoostTime = lastPickedupBoostTime;
                
            }
            ChangePlayerPos(Mathf.Lerp(startPos.x, SpeedUpEndPosition.position.x, BoostForwardTime / maxForwardBoostTime));
            BoostForwardTime += Time.deltaTime;
        }
        else if (boostTimeStayLeft > 0)
            boostTimeStayLeft -= Time.deltaTime;
        else if (boostTimeLeft > 0)
        {
            float t = boostTimeLeft / maxBoost;
            ChangePlayerPos(Mathf.Lerp(startPos.x, SpeedUpEndPosition.position.x, t));
            boostTimeLeft -= Time.deltaTime / boostDegrationSpeed;

            BoostForwardCalculatedTimeTarget = BoostForwardTime = (boostTimeLeft / maxBoost) * maxForwardBoostTime;

            if (boostTimeLeft <= 0)
            {
                LastCalcBoostTime = 0;
                
                BoostForwardCalculatedTimeTarget = 0f;
                boostTimeStayLeft = BoostTimeStay;
            }
                
        }
    }
    void Update_GroundCheck()
    {
        int mask = 1 << LayerMask.NameToLayer("Ground");

        RaycastHit2D hit = Physics2D.Raycast(transform.position + distOff, new Vector2(0, -1), l, mask);
        Debug.DrawLine(transform.position + distOff, transform.position + distOff + new Vector3(0, -l), Color.red);
        if (hit && hit.transform.tag == TagManager.Ground)
        {
            if (!g && onHitGround != null)
                onHitGround();

            g = true;
            doubleJump = true;
        }
        else
            g = false;
    }
    void Update_HeatLevel()
    {
        if (weaponHeat > 0)
            weaponHeat -= 20f * Time.deltaTime; // remove 20 heat per second;

        if (weaponHeat <= 0)
        {
            weaponHeat = 0;
            weaponForcedCooldown = false;
        }
    }
    #endregion

    void Jump()
    {

        /* float g = Physics.gravity.magnitude; // get the gravity value
         float vertSpeed = Mathf.Sqrt(2 * g * 5); // calculate the vertical speed
         float totalTime = 2 * vertSpeed / g; // calculate the total time
         //var hSpeed = maxDistance / totalTime; // calculate the horizontal speed
         rigi2d.velocity = new Vector2(rigi2d.velocity.x, vertSpeed); // launch the projectile!
         //rigi2d.AddForce(new Vector2(0, 500));*/
        rigi2d.AddForce(new Vector3(0, 9 * rigi2d.mass, 0), ForceMode2D.Impulse);
        if (onJump != null)
            onJump();
    }

    void shoot(Vector2 p)
    {
        if (weaponForcedCooldown)
            return;

        if (p.x <= playerScreenX)
            return;
       
        weaponHeat += Random.Range(10, 15);

        if (weaponHeat >= MaxHeat)
            weaponForcedCooldown = true;

        if (onShoot != null)
            onShoot();

        gun.Shoot(p);

        //Debug.Log("Shooting - Heat Level " + (weaponHeat / 100f).ToString("p"));
    }

    public void ChangePlayerPos(float newXpos)
    {
        Debug.DrawRay(new Vector3(newXpos, -10, 0), Vector3.up, Color.cyan, 100f);
        playerScreenX = newXpos;
        transform.position = new Vector3(newXpos, transform.position.y);

        playerScreenX = Camera.main.WorldToScreenPoint(transform.position).x;
    }

    /* IEnumerator playerBoost(float duration)
     {
         boostTimeLeft += duration;
         if(boostActive)
         yield break;

         Vector3 startPos = transform.position;
         float startX;
         float t;
         float m;
         float maxBoost = 0;

         boostTimeLeft = duration;
         boostActive = true;


         while(boostTimeLeft>0)
         {
             if (boostTimeLeft > maxBoost)
             {
                 maxBoost = boostTimeLeft;
                 if (maxBoost > 10)
                     boostTimeLeft = maxBoost = 10f;
                 startX = transform.position.x;
                  t = 1f * (Vector3.Distance(startPos, transform.position) / Vector3.Distance(startPos, SpeedUpEndPosition.position));
                  m = 1f * (maxBoost / 10f);



                 if (m > 1)
                     m = 1;
                 while (t < m)
                 {
                     if (!gamePaused)
                     {
                         ChangePlayerPos(Mathf.Lerp(startPos.x, SpeedUpEndPosition.position.x, t));
                         t += Time.deltaTime / boostMoveSpeed;
                     }
                     yield return new WaitForEndOfFrame();
                 }
             }
             if (!gamePaused)
                 boostTimeLeft -= Time.deltaTime/5f;

             yield return new WaitForEndOfFrame();
         }

         boostActive = false;

         startX = transform.position.x;
         t = 1f * (Vector3.Distance(startPos, transform.position) / Vector3.Distance(startPos, SpeedUpEndPosition.position));
         m = 1f * (maxBoost / 10f);

         if (m > 1)
             m = 1;
         while (t > 0)
         {
             if (!gamePaused)
             {
                 ChangePlayerPos(Mathf.Lerp(startPos.x, SpeedUpEndPosition.position.x, t));
                 t -= Time.deltaTime / boostMoveSpeed;
             }
             yield return new WaitForEndOfFrame();
         }
     }
     */
}
