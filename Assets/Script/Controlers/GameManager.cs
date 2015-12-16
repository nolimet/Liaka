using UnityEngine;
using util;
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

    #region Variables-Static
    //Keept spawing new Instances when quiting the game. This fixed that.
    private static bool _Destroyed = false;

    //Statics
    private static GameManager _instance;
    public static GameManager instance
    {
        get
        {
            if (!_instance)
            {
                _instance = FindObjectOfType<GameManager>();

                if (!_instance && !_Destroyed)
                {
                    GameObject g = Instantiate(Resources.Load("GameManager")) as GameObject;
                    g.name = "Game Manager";
                    Debug.Log("created new object");
                    return g.GetComponent<GameManager>();
                }
            }
            return _instance;
        }
    }

    //InputManager
    public static InputManager inputManager;
    public InputManager _inputManager;

    //OptionsMenu
    public static OptionsMenuControler optionsMenu;
    public OptionsMenuControler _optionsMenu;

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

   public StageControler _stageControler;
    public static StageControler stageControler
    {
        get
        {
            if (!instance._stageControler)
                instance._stageControler = FindObjectOfType<StageControler>();

            if (!instance._stageControler)
                return null;

            return instance._stageControler;
        }
    }

    public Camera _UICamera;
    public static Camera UICamera;
    //publics
    public GameObject PauseMenu;

    public SaveData saveDat = null;

    public bool GodMode = false;

    private static bool _gamePaused;
    public static bool gamePaused
    {
        get
        {
            return _gamePaused;
        }
    }
    #endregion

    int _coinsCollectedRun = 0;
    public int coinsCollectedRun
    {
        get
        {
            return _coinsCollectedRun;
        }
    }

    int _lastScene, _lastScene_1;
    public int LastScene
    {
        get
        {
            return _lastScene;
        }
    }

    public bool _AllowDebug;
    public static bool AllowDebug { get { return instance._AllowDebug; } }

    #region UnityFunctions
    void Awake()
    {

        if (_instance)
            Destroy(gameObject);
        SetStatics();
        EventInit();
        SaveLoad(false);

        PauseMenu.SetActive(false);
        _gamePaused = false;

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
        _instance = null;
        _stageControler = null;
        _Destroyed = true;
    }
    public void OnLevelWasLoaded(int level)
    {
        if (level != 4)
        {
            _lastScene = _lastScene_1;
            _lastScene_1 = level;
        }

        if (level == 1 || level == 2 || level == 3 || level == 4)
        {
            PauseMenu.SendMessage("SetState", true, SendMessageOptions.DontRequireReceiver);
            onPauseGame(true);
            uiControler.gameObject.SetActive(false);
        }
        else
        {
            ContinueGame();
        }

        if (level > 3 && _lastScene <= 4)
        {
            saveDat.game.addCoins(_coinsCollectedRun);
            _coinsCollectedRun = 0;
        }
    }

    #endregion

    void SetStatics()
    {
        if (_inputManager && !inputManager)
            inputManager = _inputManager;

        if (!_instance)
            _instance = this;

        if (_playerControler && !playerControler)
            playerControler = _playerControler;

        if (_UIControler && !uiControler)
            uiControler = _UIControler;

        if (_AudioControler && !audioControler)
            audioControler = _AudioControler;

        if (_DropTable && !dropTable)
            dropTable = _DropTable;

        if (_optionsMenu && !optionsMenu)
            optionsMenu = _optionsMenu;

        if (_UICamera && !UICamera)
            UICamera = _UICamera;
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
                Debug.LogException(e);
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

    public void ResetSave()
    {
        saveDat = new SaveData();
        SaveLoad(true);
    }

    public void GameEnd(int coins)
    {
        _coinsCollectedRun += coins;
    }

    public void ContinueGame()
    {
        _gamePaused = false;
        if (onPauseGame != null)
            onPauseGame(false);

        PauseMenu.SendMessage("SetState", false , SendMessageOptions.DontRequireReceiver);

        uiControler.gameObject.SetActive(true);
    }

    public void PauseGame()
    {
        _gamePaused = true;
        if (onPauseGame != null)
            onPauseGame(true);

        PauseMenu.SetActive(true);
        PauseMenu.SendMessage("SetState", true , SendMessageOptions.DontRequireReceiver);

        uiControler.gameObject.SetActive(false);
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
        DontDestroyOnLoad(_instance);
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

        _stageControler = null;
    }

    void onStageEnded()
    {

    }

    void onPlayerHitTrap()
    {

    }

    void EscapePressed()
    {
        if (Application.loadedLevel == 1)          
            return;

        _gamePaused = !_gamePaused;
        //onPauseGame(_gamePaused);
        if (_gamePaused)
            PauseGame();
        else
            ContinueGame();

        Debug.Log(_gamePaused ? "OPENED PAUSE MENU" : "CLOSED PAUSE MENU");
    }
    #endregion
}
