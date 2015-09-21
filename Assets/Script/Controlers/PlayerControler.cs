using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerControler : MonoBehaviour
{
    public delegate void PlayerControlerEvent(PlayerControler p);
    public static event PlayerControlerEvent onPlayerCreated;
    public static event PlayerControlerEvent onPlayerDestoryed;

    public delegate void VoidDelegate();
    public event VoidDelegate onShoot;
    public event VoidDelegate onJump;
    public event VoidDelegate onHitGround;
    public event VoidDelegate onEnergyZero;

    public PlayerGun gun;

    [SerializeField]
    bool g, doubleJump;

    [SerializeField, Tooltip("Distance the ground check ray travels")]
    float l = 1f;

    [SerializeField,Tooltip("STarting point of ground checkRay")]
    Vector3 distOff = Vector2.zero;
    Rigidbody2D rigi2d;
    Vector2 speed = Vector2.zero;

    float coolDownDelay;
    public float weaponHeat, MaxHeat = 100f;
    public float Energy, MaxEnergy = 100f;
    public bool weaponForcedCooldown;
    bool gamePaused = false;

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
        
    }

    public void OnDestroy()
    {
        GameManager.inputManager.onSwipeUp -= InputManager_onSwipeUp;
        GameManager.inputManager.onTap -= InputManager_onTap;
        GameManager.instance.onPauseGame -= Instance_onPauseGame;
        BaseObject.onHitPlayer -= BaseObject_onHitPlayer;

        if (onPlayerDestoryed != null)
            onPlayerDestoryed(this);
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

    void Update()
    {
        if (gamePaused)
            return;
        int mask = 1 << LayerMask.NameToLayer("Ground");
        RaycastHit2D hit = Physics2D.Raycast(transform.position + distOff, new Vector2(0, -1), l, mask);
       // Debug.DrawLine(transform.position + distOff, transform.position + distOff + new Vector3(0, -l),Color.red);
        if (hit && hit.transform.tag == TagManager.Ground)
        {
           
           
            if (!g && onHitGround != null)
                onHitGround();

            g = true;
            doubleJump = true;
        }
        else
            g = false;

        if (weaponHeat > 0)
            weaponHeat -= 20f * Time.deltaTime; // remove 20 heat per second;

        if (weaponHeat <= 0)
        {
            weaponHeat = 0;
            weaponForcedCooldown = false;
        }

        if (Energy > 0)
            Energy -= 5f * Time.deltaTime;

        if(Energy<=0)
        {
           // Debug.Log("YOU DIED");
        }

        
    }

    void Jump()
    {
        rigi2d.AddForce(new Vector2(0, 400));
        if (onJump != null)
            onJump();
    }

    void shoot(Vector2 p)
    {
        if (weaponForcedCooldown)
            return;

       
        weaponHeat += Random.Range(10, 15);

        if (weaponHeat >= MaxHeat)
            weaponForcedCooldown = true;

        if (onShoot != null)
            onShoot();

        gun.Shoot(p);

        //Debug.Log("Shooting - Heat Level " + (weaponHeat / 100f).ToString("p"));
    }

    private void BaseObject_onHitPlayer(BaseObject.objectType o)
    {
        switch(o)
        {
            case BaseObject.objectType.Enemy:
                Energy -= 10;
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

    public void hitPickup(GameObject g)
    {
        if (g.GetComponent<BaseObject>().type == BaseObject.objectType.Pickup)
        {
            PickObject p = g.GetComponent<PickObject>();

            switch (p.TypePickup)
            {
                case PickObject.pickupType.Static_Coin:
                case PickObject.pickupType.Dynamic_Coin:
                    break;

                case PickObject.pickupType.Static_Energy:
                case PickObject.pickupType.Dynamic_Energy:
                    Energy += 10;
                    if (Energy > MaxEnergy)
                        Energy = MaxEnergy;
                    break;
            }
        }
    }

}
