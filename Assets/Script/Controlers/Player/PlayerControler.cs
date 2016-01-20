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
    float l = 0.05f;
    int jumpCooldown = 0;

    [SerializeField, Tooltip("STarting point of ground checkRay")]
    Vector3 _distOff = Vector3.zero;
    public Vector3 distOff { get { return _distOff; } }
    [SerializeField]
    Transform SpeedUpEndPosition;
    [SerializeField]
    PlayerAnimationControler AnimationControler;

    Rigidbody2D _rigi2d;
    public Rigidbody2D rigi2d { get { return _rigi2d; } }
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

    [Header("Weapon heat Settings")]
    public float weaponHeat;
    public float MaxHeat = 100f;

    [Header("Energy Setting")]
    public float Energy;
    public float MaxEnergy = 50f;
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
        _rigi2d = GetComponent<Rigidbody2D>();

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

            if (onDeath != null)
                onDeath();
        }
    }

    void Update()
    {
        if (transform.position.y < -9)
            transform.position = new Vector3(transform.position.x, 0, transform.position.z);

        if (gamePaused || hitTrap)
            return;

        Update_Energy();
        
        Update_HeatLevel();
        Update_Boost();
        //Update_JumpSpeed();

        Update_Debug();
    }

    void FixedUpdate()
    {
        Update_GroundCheck();
    }
    #endregion

    #region eventListeners

    private void BaseObject_onHitPlayer(BaseObject.objectType o)
    {
        switch (o)
        {
            case BaseObject.objectType.Enemy:
                if (onCoinsLost != null)
                    onCoinsLost(Random.Range(2, 6));
                break;
        }
    }

    private void Instance_onPauseGame(bool b)
    {
        gamePaused = b;
        if (b)
        {
            speed = _rigi2d.velocity;
            _rigi2d.velocity = Vector2.zero;
            _rigi2d.isKinematic = true;
        }
        else
        {
            _rigi2d.isKinematic = false;
            _rigi2d.velocity = speed;
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
    void Update_JumpSpeed()
    {
        if (_rigi2d.velocity.y > 0 && _rigi2d.velocity.y < 0.3f)
            _rigi2d.AddForce(new Vector2(0, -10f*_rigi2d.mass), ForceMode2D.Impulse);
    }
    void Update_Energy()
    {
        if (Energy > 0)
            Energy -= Time.deltaTime;

        if (Energy <= 0)
        {
            if (!GameManager.instance.GodMode)
                if (onEnergyZero != null)
                    onEnergyZero();
        }

        
    }
    void Update_GroundCheck()
    {
        if(jumpCooldown>0)
        {
            jumpCooldown--;
            return;
        }
        int mask = 1 << LayerMask.NameToLayer("Ground");

        RaycastHit2D hit = Physics2D.Raycast(transform.position + _distOff, new Vector2(0, -1), l, mask);
        Debug.DrawLine(transform.position + _distOff, transform.position + _distOff + new Vector3(0, -l), Color.red);

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

    void Update_Debug()
    {
        util.Debugger.Log("GroundStatus", "G : " + g + " DoubleJump : " + doubleJump);
        util.Debugger.Log("HeatLevel", "current : " + weaponHeat + " max: " + MaxHeat);
        util.Debugger.Log("EnergyLevel", "current : " + Energy + " max: " + MaxEnergy);
        util.Debugger.Log("BoostLevel", "current : " + boostTimeLeft + " max: " + maxBoost);
    }
    #endregion

    void Jump()
    {
        if (hitTrap || gamePaused)
            return;

        _rigi2d.AddForce(new Vector3(0, 9 * _rigi2d.mass * _rigi2d.gravityScale, 0), ForceMode2D.Impulse);
        if (onJump != null)
            onJump();

        jumpCooldown = 3;
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

            gun.Shoot(p);

            return;
        }

        if (onBossBattleAttack != null)
            onBossBattleAttack();
    }

    public void ChangePlayerPos(float newXpos)
    {
        Debug.DrawRay(new Vector3(newXpos, -10, 0), Vector3.up, Color.cyan, 100f);
        transform.position = new Vector3(newXpos, transform.position.y);

       //playerScreenX = Camera.main.WorldToScreenPoint(transform.position).x;
    }
}
