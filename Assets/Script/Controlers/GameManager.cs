using UnityEngine;
using System.Collections;

public class GameManager : MonoBehaviour
{

    public delegate void VoidDelegate();
    public delegate void BoolDelegate(bool b);

    //Void Events
    public event VoidDelegate onStageDone;
    public event VoidDelegate onPlayerDeath;
    public event VoidDelegate onBonusLevel;

    //Bool Events
    public event BoolDelegate onPauseGame;

    //Statics
    public static GameManager instance;
    //InputManager
    public static InputManager inputManager;
    public InputManager _inputManager;
    //Player Controler
    public static PlayerControler playerControler;
    public PlayerControler _playerControler;
    //UI contrler
    public static UIControler uiControler;
    public UIControler _UIControler;
    //publics
    public GameObject PauseMenu;

    public SaveData saveDat = null;

    bool GamePaused;

    void Awake()
    {
        if (instance)
            Destroy(gameObject);
        SetStatics();
        EventInit();
        SaveLoad(false);

        PauseMenu.SetActive(false);
        GamePaused = false;
    }

    public void OnDestroy()
    {
        SaveLoad(true);
    }

    void SetStatics()
    {
        if (_inputManager && !inputManager)
            inputManager = _inputManager;

        if (!instance)
            instance = this;

        if (_playerControler && !playerControler)
            playerControler = _playerControler;

        if (_UIControler && !uiControler)
            uiControler = _UIControler;

        
    }

    void onPlayerDestoryed(PlayerControler p)
    {
        if (_playerControler != p)
            return;

        _playerControler = null;
        playerControler = null;
    }

    void onPlayerCreated(PlayerControler p)
    {
        if (_playerControler == null)
            _playerControler = p;
    }

    void SaveLoad(bool save)
    {
        string dataName = "GameSave";
        if (!save)
        {
            Serialization.Load(dataName, Serialization.fileTypes.binary, ref saveDat);

            if (saveDat == null)
            {
                saveDat = new SaveData();
                Serialization.Save(dataName, Serialization.fileTypes.binary, saveDat);
            }
        }
        else
        {
            if (saveDat == null)
                saveDat = new SaveData();
            Serialization.Save(dataName, Serialization.fileTypes.binary, saveDat);
        }
    }

    void EventInit()
    {
        _inputManager.onEscapePressed += EscapePressed;
        PlayerControler.onPlayerDestoryed += onPlayerDestoryed;
        PlayerControler.onPlayerCreated += onPlayerCreated;

        DontDestroyOnLoad(inputManager);
        DontDestroyOnLoad(uiControler);
        DontDestroyOnLoad(instance);
    }

    void EscapePressed()
    {
        if (Application.loadedLevel == 1)
            return;

        GamePaused = !GamePaused;
        onPauseGame(GamePaused);
        PauseMenu.SetActive(GamePaused);
        Debug.Log(GamePaused ? "OPENED PAUSE MENU" : "CLOSED PAUSE MENU");
    }

    public void ContinueGame()
    {
        GamePaused = false;
        onPauseGame(false);
        PauseMenu.SetActive(false);
        uiControler.gameObject.SetActive(true);
    }

    void Update()
    {
        SetStatics();
    }

    public void OnLevelWasLoaded(int level)
    {
        if (level != 1)
        {
            ContinueGame();
            
        }

        if(level == 1)
        {
            PauseMenu.SetActive(false);
            
            uiControler.gameObject.SetActive(false);
        }
    }
}
