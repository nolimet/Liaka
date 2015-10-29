using UnityEngine;
using System.Collections;

public class GameManager : MonoBehaviour
{
    #region Events
    public delegate void VoidDelegate();
    public delegate void BoolDelegate(bool b);

    //Void Events
    public event VoidDelegate onStageDone;
    public event VoidDelegate onPlayerDeath;
    public event VoidDelegate onBonusLevel;
    public event VoidDelegate onGameOver;

    //Bool Events
    /// <summary>
    /// called when game is paused.
    /// </summary>
    public event BoolDelegate onPauseGame;
    #endregion

    #region varibles
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
    
    //AudioControler
    public static AudioControler audioControler;
    public AudioControler _AudioControler;

    //DropTable
    public static DropTableControler dropTable;
    public DropTableControler _DropTable;

    public static StageControler stageControler;
    public StageControler _stageControler;


    //publics
    public GameObject PauseMenu;

    public SaveData saveDat = null;

    public bool GodMode = false;

    bool GamePaused;
    #endregion

    #region UnityFunctions
    void Awake()
    {
        if (instance)
            Destroy(gameObject);
        SetStatics();
        EventInit();
        SaveLoad(false);

        PauseMenu.SetActive(false);
        GamePaused = false;

        if (!Application.isEditor)
            GodMode = false;
    }

    public void OnDestroy()
    {
        SaveLoad(true);
        dropTable = null;
        audioControler = null;
        uiControler = null;
        playerControler = null;
        inputManager = null;
        instance = null;
        stageControler = null;
    }

    public void OnLevelWasLoaded(int level)
    {
        if (level != 1 && level != 2)
        {
            ContinueGame();

        }

        if (level == 1 || level ==2)
        {
            PauseMenu.SetActive(false);
            onPauseGame(true);
            uiControler.gameObject.SetActive(false);
        }

        
    }

    #endregion

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

        if (_AudioControler && !audioControler)
            audioControler = _AudioControler;

        if (_DropTable && !dropTable)
            dropTable = _DropTable;

        if (_stageControler && !stageControler)
            stageControler = _stageControler;
    }

    void SaveLoad(bool save)
    {
        string dataName = "GameSave";
        if (!save)
        {
            try
            {
                Serialization.Load(dataName, Serialization.fileTypes.binary, ref saveDat);

                if (saveDat == null)
                {
                    saveDat = new SaveData();
                    Serialization.Save(dataName, Serialization.fileTypes.binary, saveDat);
                }
            }
            catch (System.Exception e)
            {
                AnalyticsGameManager.ThrowSystemError(e);

                Debug.Log(saveDat);
            }
            
        }
        else
        {
            if (saveDat == null)
                saveDat = new SaveData();
            Serialization.Save(dataName, Serialization.fileTypes.binary, saveDat);
        }
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
        if(onPauseGame!=null)
        onPauseGame(false);
        PauseMenu.SetActive(false);
        uiControler.gameObject.SetActive(true);
    }

    void Update()
    {
        SetStatics();
    } 

    #region EventListeners
    void EventInit()
    {
        _inputManager.onEscapePressed += EscapePressed;
        PlayerControler.onPlayerDestoryed += onPlayerDestoryed;
        PlayerControler.onPlayerCreated += onPlayerCreated;
        
        StageControler.onStageCreated += onStageControlerLoaded;
        StageControler.onStageDestroyed += onStageControlerDestroyed;
        StageControler.onStageTimerEnded += onStageEnded;

        DontDestroyOnLoad(inputManager);
        DontDestroyOnLoad(uiControler);
        DontDestroyOnLoad(audioControler);
        DontDestroyOnLoad(instance);
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

    void onStageControlerLoaded(StageControler s)
    {
        if (_stageControler == null)
            _stageControler = s;
    }

    void onStageControlerDestroyed(StageControler s)
    {
        if (_stageControler != s)
            return;

        stageControler = null;
        _stageControler = null;
    }

    void onStageEnded()
    {
        
    }

    void onPlayerHitTrap()
    {
        
    }
    #endregion
}
