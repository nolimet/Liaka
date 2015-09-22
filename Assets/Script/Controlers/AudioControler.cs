using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;
using System.Collections.Generic;

public class AudioControler : MonoBehaviour
{
    //Player
    public AudioClip playerJump, PlayerHitGround, PlayerShoots;
    //BaseObject
    public AudioClip enemyGetsHit, CoinGetsPicked, EnergyGetsPicked;
    //Boss
    public AudioClip bossGetsHitByBullet, bossGetsHitByEnemy;
    //Music
    public AudioClip[] Music;
    //UI
    public AudioClip MouseOver, MouseClick;

    [System.Serializable]
    public struct sources
    {
        public AudioSource playerJump, PlayerHitGround, PlayerShoots;
        public AudioSource enemyGetsHit, CoinGetsPicked, EnergyGetsPicked;
        public AudioSource bossGetsHitByBullet, bossGetsHitByEnemy;
        public AudioSource Music;
    }

    public sources AudioSources;

    Vector3 soundPos = new Vector3(0, 0, -10);

    float MusicVol, SFXVol;

    void Start()
    {
        PlayerControler.onPlayerCreated += PlayerCreated;
        PlayerControler.onPlayerDestoryed += PlayerDestoryed;
        GameManager.instance.onPauseGame += Instance_onPauseGame;
        BaseObject.onHitBose += BaseObject_onHitBose;
        BaseObject.onImpact += BaseObject_onImpact;
        PickObject.onPickup += PickObject_onPickup;
    }

    public void OnLevelWasLoaded(int level)
    {
        if (level != 1 && level != 0)
            PlaySong();
        else
            AudioSources.Music.Stop();
    }

    AudioSource CreateSource()
    {
        GameObject g = new GameObject();
        g.transform.SetParent(transform, false);
        g.transform.localPosition = Vector3.zero;
        return g.AddComponent<AudioSource>();
    }

    void Update()
    {
        MusicVol = GameManager.instance.saveDat.musicVolume;
        SFXVol = GameManager.instance.saveDat.soundVolume;
    }

    void GamePaused()
    {

    }

    void PlaySong()
    {
        AudioSources.Music.clip = Music[Random.Range(0, Music.Length)];
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
        switch (o)
        {

        }
    }

    void OnEnable()
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

    public void PlayMouseOver()
    {
        AudioSource.PlayClipAtPoint(MouseOver, Camera.main.transform.position);
    }

    public void PlayMouseClick()
    {
        AudioSource.PlayClipAtPoint(MouseClick, Camera.main.transform.position);
    }

    private void PickObject_onPickup(PickObject.pickupType p)
    {
        switch (p)
        {
            case PickObject.pickupType.Static_Coin:
            case PickObject.pickupType.Dynamic_Coin:
                AudioSources.CoinGetsPicked.PlayOneShot(CoinGetsPicked, SFXVol);
                //AudioSource.PlayClipAtPoint(CoinGetsPicked, soundPos, SFXVol);
                break;

            case PickObject.pickupType.Dynamic_Energy:
            case PickObject.pickupType.Static_Energy:
                AudioSources.EnergyGetsPicked.PlayOneShot(EnergyGetsPicked, SFXVol);
                //AudioSource.PlayClipAtPoint(EnergyGetsPicked, soundPos, SFXVol);
                break;
        }
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
}
