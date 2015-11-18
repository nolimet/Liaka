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
    public delegate void IntDelegate(int i);
    /// <summary>
    /// called when player shoots
    /// </summary>
    public event VoidDelegate onShoot;
    /// <summary>
    /// called when player jumps
    /// </summary>
    public event VoidDelegate onJump;
    /// <summary>
    /// Called when player hits the ground
    /// </summary>
    public event VoidDelegate onHitGround;
    /// <summary>
    /// Called when your energy ran out
    /// </summary>
    public event VoidDelegate onEnergyZero;
    /// <summary>
    /// Called when a playerkilling event occured
    /// </summary>
    public event VoidDelegate onDeath;
    /// <summary>
    /// Callen when the player does a attack on the boss
    /// </summary>
    public event VoidDelegate onBossBattleAttack;
    /// <summary>
    /// Called when the player picks up a coin
    /// </summary>
    public event VoidDelegate onCoinPickup;
    /// <summary>
    /// Called when the player losses coins
    /// </summary>
    public event IntDelegate onCoinsLost;
    #endregion

    #region Varibles
    public PlayerGun gun;

    [SerializeField,Header("Movement")]
    bool g, doubleJump;

    [SerializeField, Tooltip("Distance the ground check ray travels")]
    float l = 1f;

    [SerializeField, Tooltip("STarting point of ground checkRay")]
    Vector3 distOff = Vector3.zero;
    [SerializeField]
    Transform SpeedUpEndPosition;
    [SerializeField]
    PlayerAnimationControler AnimationControler;

    Rigidbody2D rigi2d;
    Vector2 speed = Vector2.zero, startPos;

    float coolDownDelay;

    //boost publics
    [Header("Boost Settings"), SerializeField]
    public float boostMoveSpeed = 10f;
    public float maxBoost = 3.5f;
    public float boostDegrationSpeed = 1f;
    public float boostTimeLeft = 0f;
    public float BoostTimeStay = 2f;
    public float BoostForwardTimePerUnit = 1f;
    public float BoostGainPerPickup = 0.75f;

    //boost Privates
    float LastBoostTimeStep;
    float BoostNormalPos = 0, BoostNormalTarget = 0;
    int boostDirection;
    bool atNewBoostPos;
    float BoostStayTimeLeft;

    [HideInInspector]
    public float weaponHeat, MaxHeat = 100f;
    [HideInInspector]
    public float Energy, MaxEnergy = 100f;
    [HideInInspector]
    public bool weaponForcedCooldown;
    [HideInInspector]
    public float playerScreenX = 0f;
    bool gamePaused = false, hitTrap = false;
    #endregion

    #region Unity functions
    void Start()
    {
        Energy = MaxEnergy;
        rigi2d = GetComponent<Rigidbody2D>();

        GameManager.inputManager.onSwipeUp += InputManager_onSwipeUp;
        GameManager.inputManager.onTap += InputManager_onTap;
        GameManager.instance.onPauseGame += Instance_onPauseGame;
        EnemyBase.onHitPlayer += BaseObject_onHitPlayer;

        if (onPlayerCreated != null)
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

        if (GameManager.instance)
            GameManager.instance.onPauseGame -= Instance_onPauseGame;

        if (onPlayerDestoryed != null)
            onPlayerDestoryed(this);

        EnemyBase.onHitPlayer -= BaseObject_onHitPlayer;
    }

    public void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.transform.tag == TagManager.Boss && !GameManager.instance.GodMode)
        {
            if (onDeath != null)
                onDeath();
        }
    }

    public void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.transform.tag == TagManager.Trap && (GameManager.instance && !GameManager.instance.GodMode)) 
        { 
            hitTrap = true;

            rigi2d.constraints = RigidbodyConstraints2D.FreezeRotation;
        }
    }

    void Update()
    {
        if (transform.position.y < -9)
            transform.position = new Vector3(transform.position.x, 0, transform.position.z);

        if (gamePaused || hitTrap)
            return;

        Update_Energy();
        Update_GroundCheck();
        Update_HeatLevel();
        Update_Boost();
    }
    #endregion

    #region eventListeners

    private void BaseObject_onHitPlayer(BaseObject.objectType o)
    {
        switch (o)
        {
            case BaseObject.objectType.Enemy:
                if (onCoinsLost != null)
                    onCoinsLost(Random.Range(5, 8));

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
                    if (onCoinPickup != null)
                        onCoinPickup();
                    break;

                case PickupBase.PickupType.Energy:
                    Energy += 15;
                    if (Energy > MaxEnergy)
                        Energy = MaxEnergy;
                    break;

                case PickupBase.PickupType.SpeedUp:
                    boostTimeLeft += BoostGainPerPickup;
                    BoostStayTimeLeft = BoostTimeStay;
                    atNewBoostPos = false;
                    break;

                case PickupBase.PickupType.SpeedDown:
                    boostTimeLeft -= BoostGainPerPickup;
                    BoostStayTimeLeft = BoostTimeStay;
                    atNewBoostPos = false;
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
            if (!GameManager.instance.GodMode)
                if (onEnergyZero != null)
                    onEnergyZero();
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
    /// <summary>
    /// calls the Functions for the MoveBoost
    /// They need to be executed in a certain order to work well
    /// </summary>
    void Update_Boost()
    {
        //normal boost
        if (boostTimeLeft > maxBoost)
            Debug.Log("overBoost");

        moveBoostCalc();
        moveBoostStep();
        moveBoostDecay();
    }
    /// <summary>
    /// Does the calculation for the moveBoost
    /// </summary>
    void moveBoostCalc()
    {
        if(LastBoostTimeStep!=boostTimeLeft)
        {
            
            if (LastBoostTimeStep > boostTimeLeft)
                boostDirection = -1;
            else if (LastBoostTimeStep < boostTimeLeft)
                boostDirection = 1;

            LastBoostTimeStep = boostTimeLeft;
            BoostNormalTarget = (boostTimeLeft / maxBoost) ;
        }
        
    }
    /// <summary>
    /// Does the steps the player makes on screen
    /// So when he needs to move this function handels that
    /// </summary>
    void moveBoostStep()
    {
        if (boostDirection > 0)
        {
            if (BoostNormalPos < BoostNormalTarget)
            {
                BoostNormalPos += Time.deltaTime * BoostForwardTimePerUnit;
            }
            else
                atNewBoostPos = true;

        }
        else if (boostDirection < 0)
        {
            if (BoostNormalPos > BoostNormalTarget)
            {
                BoostNormalPos -= Time.deltaTime * BoostForwardTimePerUnit;
            }
            else
                atNewBoostPos = true;
        }

        if (BoostNormalPos >= 0)
            ChangePlayerPos(Mathf.Lerp(startPos.x, SpeedUpEndPosition.position.x, BoostNormalPos));
        else if (BoostNormalPos <= 0)
            ChangePlayerPos(Mathf.Lerp(startPos.x, startPos.x - 7, -BoostNormalPos));
    }
    /// <summary>
    /// When the boost idle timer runs out the boost starts to decay.
    /// The Boost time slowly starts to run to zero. When it has hit zero it stops decaying. 
    /// When the boost is below zero nothing will happen. 
    /// </summary>
    void moveBoostDecay()
    {
        if (atNewBoostPos)
        {
            if(BoostStayTimeLeft > 0)
            {
                BoostStayTimeLeft -= Time.deltaTime;
                return;
            }

            if (boostTimeLeft > 0)
                boostTimeLeft -= Time.deltaTime * boostDegrationSpeed;
        }
    }



    #endregion

    void Jump()
    {
        if (hitTrap || gamePaused)
            return;

        rigi2d.AddForce(new Vector3(0, 9 * rigi2d.mass, 0), ForceMode2D.Impulse);
        if (onJump != null)
            onJump();
    }

    void shoot(Vector2 p)
    {
        if (weaponForcedCooldown || hitTrap || gamePaused)
            return;

        if (p.x <= playerScreenX)
            return;

        if (!GameManager.stageControler.bossFighting)
        {
            weaponHeat += Random.Range(10, 15);

            if (weaponHeat >= MaxHeat)
                weaponForcedCooldown = true;

            if (onShoot != null)
                onShoot();

            gun.Shoot(Vector2.right);

            return;
        }

        if (onBossBattleAttack != null)
            onBossBattleAttack();
    }

    public void ChangePlayerPos(float newXpos)
    {
        Debug.DrawRay(new Vector3(newXpos, -10, 0), Vector3.up, Color.cyan, 100f);
        transform.position = new Vector3(newXpos, transform.position.y);

       // playerScreenX = Camera.main.WorldToScreenPoint(transform.position).x;
    }
}
