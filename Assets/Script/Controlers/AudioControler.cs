using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class AudioControler : MonoBehaviour {

    public AudioClip playerJump, PlayerHitGround, PlayerShoots;
    public AudioClip enemyGetsHit, CoinGetsPicked, EnergyGetsPicked;
    public AudioClip bossGetsHitByBullet, bossGetsHitByEnemy;

    AudioSource s;

    Vector3 soundPos = new Vector3(0, 0, -10);

    float MusicVol, SFXVol;

    void Start()
    {
        PlayerControler.onPlayerCreated += PlayerCreated;
        PlayerControler.onPlayerDestoryed += PlayerDestoryed;
        GameManager.instance.onPauseGame += Instance_onPauseGame;
        BaseObject.onImpact += BaseObject_onImpact;
        PickObject.onPickup += PickObject_onPickup;


        s = gameObject.AddComponent<AudioSource>();
    }

   void Update()
    {
        MusicVol = GameManager.instance.saveDat.musicVolume;
        SFXVol = GameManager.instance.saveDat.soundVolume;
    }

    void GamePaused()
    {

    }

    void PlayerDestoryed(PlayerControler p)
    {
        p.onJump -= PlayerControler_onJump;
        p.onShoot -= PlayerControler_onShoot;
        p.onHitGround -= PlayerControler_onHitGround;
    }

    void PlayerCreated(PlayerControler p)
    {
        p.onJump += PlayerControler_onJump;
        p.onShoot += PlayerControler_onShoot;
        p.onHitGround += PlayerControler_onHitGround;
        
    }

    private void Instance_onPauseGame(bool b)
    {
        if (b)
            s.Pause();
        else
            s.UnPause();
    }

    private void PlayerControler_onHitGround()
    {
        AudioSource.PlayClipAtPoint(PlayerHitGround, soundPos, SFXVol);
    }

    private void PlayerControler_onJump()
    {
        AudioSource.PlayClipAtPoint(playerJump, soundPos, SFXVol);
    }

    private void PlayerControler_onShoot()
    {
        AudioSource.PlayClipAtPoint(PlayerShoots, soundPos, SFXVol);
    }

    private void BaseObject_onImpact(BaseObject.objectType o)
    {
        switch (o)
        {
            
        }
    }

    private void PickObject_onPickup(PickObject.pickupType p)
    {
        switch (p)
        {
            case PickObject.pickupType.Static_Coin:
            case PickObject.pickupType.Dynamic_Coin:
                AudioSource.PlayClipAtPoint(CoinGetsPicked, soundPos, SFXVol);
                break;

            case PickObject.pickupType.Dynamic_Energy:
            case PickObject.pickupType.Static_Energy:
                AudioSource.PlayClipAtPoint(EnergyGetsPicked, soundPos, SFXVol);
                break;
        }
    }
}
