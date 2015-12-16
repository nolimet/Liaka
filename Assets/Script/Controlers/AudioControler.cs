using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;
using System.Collections.Generic;

public class AudioControler : MonoBehaviour
{
    //Player
    [Header("Player")]
    public AudioClip playerJump;
    public AudioClip PlayerHitGround, PlayerShoots;
    //BaseObject
    [Header("pickups")]
    public AudioClip enemyGetsHit;
    public AudioClip CoinGetsPicked, EnergyGetsPicked, coinHitGround;
    //Boss
    [Header("boss")]
    public AudioClip bossGetsHitByBullet;
    public AudioClip bossGetsHitByEnemy;
    //Music
    [Header("music")]
    public AudioClip[] Music;
    //UI
    [Header("UI")]
    public AudioClip MouseOver;
    public AudioClip MouseClick;


    [System.Serializable]
    public struct sources
    {
        //Player
        [Header("Player")]
        public AudioSource playerJump;
        public AudioSource PlayerHitGround, PlayerShoots;
        //BaseObject
        [Header("pickups")]
        public AudioSource enemyGetsHit;
        public AudioSource CoinGetsPicked, EnergyGetsPicked, coinHitGround;
        //Boss
        [Header("boss")]
        public AudioSource bossGetsHitByBullet;
        public AudioSource bossGetsHitByEnemy;
        //Music
        [Header("music")]
        public AudioSource Music;
        //UI
        [Header("UI")]
        public AudioSource MouseOver;
        public AudioSource MouseClick;

    }

    public sources AudioSources;

    float MusicVol, SFXVol, UIVol;

    void Start()
    {
        PlayerControler.onPlayerCreated += PlayerCreated;
        PlayerControler.onPlayerDestoryed += PlayerDestoryed;
        GameManager.instance.onPauseGame += Instance_onPauseGame;
        BaseObject.onHitBose += BaseObject_onHitBose;
        BaseObject.onImpact += BaseObject_onImpact;
        PickupBase.onPickup += PickupBase_onPickup;
        PickupBase.onGroundHit += PickupBase_onGroundHit;
        SetClicks();
    }

    

    void OnDestroy()
    {
        PlayerControler.onPlayerCreated -= PlayerCreated;
        PlayerControler.onPlayerDestoryed -= PlayerDestoryed;
        BaseObject.onHitBose -= BaseObject_onHitBose;
        BaseObject.onImpact -= BaseObject_onImpact;
        PickupBase.onPickup -= PickupBase_onPickup;
        PickupBase.onGroundHit -= PickupBase_onGroundHit;
    }

    public void OnLevelWasLoaded(int level)
    {
        if (level != 1 && level != 0 && level != 2 && level!=3 && level!=4)
            PlaySong();
        else
            AudioSources.Music.Stop();

        SetClicks();
    }

    /// <summary>
    /// create a new audio soruce
    /// </summary>
    /// <returns></returns>
    AudioSource CreateSource()
    {
        GameObject g = new GameObject();
        g.transform.SetParent(transform, false);
        g.transform.localPosition = Vector3.zero;
        return g.AddComponent<AudioSource>();
    }

    void Update()
    {
        if (MusicVol != GameManager.instance.saveDat.options.musicVolume)
        {
            AudioSources.Music.volume = GameManager.instance.saveDat.options.musicVolume;
            MusicVol = GameManager.instance.saveDat.options.musicVolume;
        }
        SFXVol = GameManager.instance.saveDat.options.soundVolume;
        UIVol = GameManager.instance.saveDat.options.interfaceVolume;
    }

    void SetClicks()
    {
        Button[] buttons = FindObjectsOfType<Button>();
        foreach (Button button in buttons)
        {
            EventTrigger eventTrigger = null;
            if (button.gameObject.GetComponent<EventTrigger>() == null)
            {
                button.gameObject.AddComponent<EventTrigger>();
            }
            eventTrigger = button.gameObject.GetComponent<EventTrigger>();

            EventTrigger.Entry over = new EventTrigger.Entry();
            over.eventID = EventTriggerType.PointerEnter;
            over.callback.AddListener(delegate { PlayMouseOver(); });
            if (eventTrigger.triggers == null)
            {
                eventTrigger.triggers = new System.Collections.Generic.List<EventTrigger.Entry>();
            }
            eventTrigger.triggers.Add(over);

            button.onClick.AddListener(delegate { PlayMouseClick(); });
        }
    }

    //Collection of Event listerns and such
    #region soundPlayers
    void PlaySong()
    {
        AudioSources.Music.clip = Music[Random.Range(0, Music.Length)];
        AudioSources.Music.volume = MusicVol;
        AudioSources.Music.Play();
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
        AudioSource[] AS = FindObjectsOfType<AudioSource>();
        foreach (AudioSource s in AS)
        {
            if (b)
                s.Pause();
            else
                s.UnPause();
        }
    }

    private void PlayerControler_onHitGround()
    {
        AudioSources.PlayerHitGround.PlayOneShot(PlayerHitGround, SFXVol);

        //AudioSource.PlayClipAtPoint(PlayerHitGround, soundPos, SFXVol);
    }

    private void PlayerControler_onJump()
    {
        AudioSources.playerJump.PlayOneShot(playerJump, SFXVol);
        //AudioSource.PlayClipAtPoint(playerJump, soundPos, SFXVol);
    }

    private void PlayerControler_onShoot()
    {
        AudioSources.PlayerShoots.PlayOneShot(PlayerShoots, SFXVol);
        //AudioSource.PlayClipAtPoint(PlayerShoots, soundPos, SFXVol);
    }

    private void BaseObject_onImpact(BaseObject.objectType o)
    {
        /*switch (o)
        {

        }*/
    }

    public void PlayMouseOver()
    {
        AudioSources.MouseOver.PlayOneShot(MouseOver, UIVol);
    }

    public void PlayMouseClick()
    {
        AudioSources.MouseOver.PlayOneShot(MouseClick, UIVol);
    }


    private void BaseObject_onHitBose(BaseObject.objectType o)
    {
        switch (o)
        {
            case BaseObject.objectType.Bullet:
                AudioSources.bossGetsHitByBullet.PlayOneShot(bossGetsHitByBullet, SFXVol);
                //AudioSource.PlayClipAtPoint(bossGetsHitByBullet, soundPos, SFXVol);
                break;

            case BaseObject.objectType.Enemy:
                AudioSources.bossGetsHitByEnemy.PlayOneShot(bossGetsHitByEnemy, SFXVol);
                //AudioSource.PlayClipAtPoint(bossGetsHitByEnemy, soundPos, SFXVol);
                break;
        }
    }

    private void PickupBase_onGroundHit(PickupBase.PickupType p)
    {
        switch (p)
        {
            case PickupBase.PickupType.Coin:
                AudioSources.coinHitGround.PlayOneShot(coinHitGround, SFXVol);
                break;
        }
    }

    private void PickupBase_onPickup(PickupBase.PickupType p)
    {
        switch (p)
        {
            case PickupBase.PickupType.Coin:
                AudioSources.CoinGetsPicked.PlayOneShot(CoinGetsPicked, SFXVol);
                //AudioSource.PlayClipAtPoint(CoinGetsPicked, soundPos, SFXVol);
                break;

            case PickupBase.PickupType.Energy:
                AudioSources.EnergyGetsPicked.PlayOneShot(EnergyGetsPicked, SFXVol);
                //AudioSource.PlayClipAtPoint(EnergyGetsPicked, soundPos, SFXVol);
                break;
        }
    }

    #endregion
}
