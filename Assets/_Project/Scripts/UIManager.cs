using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;


public class UIManager : MonoBehaviour
{
    #region Fields

    private static UIManager _instance;
    public static UIManager Instance => _instance;

    public PlayerController playerController;

    public Slider garbageSlider;

    public TextMeshProUGUI currentGoldText,
        earnedGoldText,
        earnedExtraGoldText,
        getExtraGoldText,
        prepareTotalGoldText,
        winTotalGoldText,
        sliderLevelText,
        getGoldText,
        collectMoreGarbageText,
        dontTouchHousewaresText;

    [HideInInspector] public int sliderLevel = 1, gold;

    [SerializeField] private GameObject _prepareGameUI,
        _mainGameUI,
        _loseGameUI,
        _winGameUI,
        _extraGoldArrow,
        _getButton,
        _getExtraButton,
        _extraGoldPanel;

    [SerializeField] private List<GameObject> _goldenCoins, _yellowStars;

    private float _anglerBonusArrowZ, _time = 1f;
    private int _multiplyStarNumber = 1;

    #endregion

    private void Awake() // Using Singleton Design Pattern
    {
        if (_instance != null && _instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            _instance = this;
        }
    }

    private void Start()
    {
        SetGoldZeroOnStart();
        SetPlayerPrefs();
    }

    private void Update()
    {
        switch (GameManager.Instance.CurrentGameState)
        {
            case GameState.PrepareGame:
                PrepareGameUI();
                UpdateGoldInfo();
                break;
            case GameState.MainGame:
                CalculateStars();
                UpdateGoldInfo();
                break;
            case GameState.LoseGame:
                break;
            case GameState.WinGame:
                CalculateStars();
                UpdateGoldInfo();
                break;
        }
    }

    #region Methods

    public void PrepareGameUI()
    {
        foreach (var yellowStar in _yellowStars)
        {
            yellowStar.SetActive(false);
        }

        dontTouchHousewaresText.enabled = false;
        collectMoreGarbageText.enabled = false;
        _prepareGameUI.SetActive(true);
        _mainGameUI.SetActive(false);
        _loseGameUI.SetActive(false);
        _winGameUI.SetActive(false);
    }

    public void MainGameUI()
    {
        _prepareGameUI.SetActive(false);
        _mainGameUI.SetActive(true);
        _loseGameUI.SetActive(false);
        _winGameUI.SetActive(false);
    }

    public void LoseGameUI()
    {
        playerController.gameObject.SetActive(false);
        _prepareGameUI.SetActive(false);
        _mainGameUI.SetActive(false);
        _loseGameUI.SetActive(true);
        _winGameUI.SetActive(false);
    }

    public void WinGameUI()
    {
        playerController.gameObject.SetActive(false);
        _prepareGameUI.SetActive(false);
        _mainGameUI.SetActive(false);
        _loseGameUI.SetActive(false);
        _winGameUI.SetActive(true);
    }


    public void UpdateGoldInfo()
    {
        CalculateBonusArrowRotation();
        currentGoldText.text = gold.ToString();
        earnedGoldText.text = (_multiplyStarNumber * gold).ToString();
        prepareTotalGoldText.text = PlayerPrefs.GetInt("TotalGold").ToString();
        winTotalGoldText.text = PlayerPrefs.GetInt("TotalGold").ToString();
    }


    public void CalculateStars() // Calculating The Reward On Win According To Stars Player Have
    {
        if (garbageSlider.value > 60)
        {
            _yellowStars[0].SetActive(true);
            getGoldText.text = "GET X1";
        }

        if (garbageSlider.value > 133)
        {
            _yellowStars[1].SetActive(true);
            getGoldText.text = "GET X2";
            _multiplyStarNumber = 2;
        }

        if (garbageSlider.value > 205)
        {
            _yellowStars[2].SetActive(true);
            getGoldText.text = "GET X3";
            _multiplyStarNumber = 3;
        }
    }

    public IEnumerator DurationWinGameUI()
    {
        yield return new WaitForSeconds(2f);
        WinGameUI();
    }

    public IEnumerator DurationLoseGameUI()
    {
        yield return new WaitForSeconds(2f);
        SoundManager.Instance.PlaySound(SoundManager.Instance.loseGameSound, 1f);
        LoseGameUI();
    }

    public void RetryButton()
    {
        Scene currentScene = SceneManager.GetActiveScene();
        SceneManager.LoadScene(currentScene.buildIndex);
    }

    public void NextLevelButton()
    {
        PlayerPrefs.SetInt("SliderLevel", PlayerPrefs.GetInt("SliderLevel") + 1);
        sliderLevelText.text = PlayerPrefs.GetInt("SliderLevel").ToString();
        StartCoroutine(LevelManager.Instance.NextLevel());
    }


    public void GetExtraButton() // Getting Extra Gold Reward For Watching Ads
    {
        foreach (var goldenCoin in _goldenCoins) // Golden Coins Moving Up
        {
            goldenCoin.SetActive(true);
            goldenCoin.transform.DOLocalMove(new Vector3(38.7f, -50f, 0), _time);
            _time += 0.05f;
        }

        if (_anglerBonusArrowZ <= 360 && _anglerBonusArrowZ >= 306f)
        {
            PlayerPrefs.SetInt("TotalGold", gold * 2 * _multiplyStarNumber + PlayerPrefs.GetInt("TotalGold"));
        }

        if (_anglerBonusArrowZ < 306f && _anglerBonusArrowZ >= 250f)
        {
            PlayerPrefs.SetInt("TotalGold", gold * 3 * _multiplyStarNumber + PlayerPrefs.GetInt("TotalGold"));
        }

        if (_anglerBonusArrowZ < 250f && _anglerBonusArrowZ >= 202f)
        {
            PlayerPrefs.SetInt("TotalGold", gold * 4 * _multiplyStarNumber + PlayerPrefs.GetInt("TotalGold"));
        }

        if (_anglerBonusArrowZ < 202f && _anglerBonusArrowZ >= 180f)
        {
            PlayerPrefs.SetInt("TotalGold", gold * 5 * _multiplyStarNumber + PlayerPrefs.GetInt("TotalGold"));
        }

        _getButton.SetActive(false);
        _getExtraButton.SetActive(false);
        _extraGoldPanel.SetActive(false);
        GameManager.Instance.NextLevel();
    }

    public void GetButton() // Getting Gold Reward
    {
        foreach (var goldenCoin in _goldenCoins)
        {
            goldenCoin.SetActive(true);
            goldenCoin.transform.DOLocalMove(new Vector3(38.7f, -50f, 0), _time);
            _time += 0.05f;
        }

        PlayerPrefs.SetInt("TotalGold", (_multiplyStarNumber * gold) + PlayerPrefs.GetInt("TotalGold"));
        _getButton.SetActive(false);
        _getExtraButton.SetActive(false);
        _extraGoldPanel.SetActive(false);
        GameManager.Instance.NextLevel();
    }

    private void CalculateBonusArrowRotation() // Calculating Rotation Of Extra Gold Reward Arrow
    {
        var anglerZ = _extraGoldArrow.transform.localEulerAngles.z;
        _anglerBonusArrowZ = anglerZ;
        if (anglerZ <= 360 && anglerZ >= 306f)
        {
            earnedExtraGoldText.text = (gold * 2 * _multiplyStarNumber).ToString();
            getExtraGoldText.text = "GET EXTRA X2";
        }

        if (anglerZ < 306f && anglerZ >= 250f)
        {
            earnedExtraGoldText.text = (gold * 3 * _multiplyStarNumber).ToString();
            getExtraGoldText.text = "GET EXTRA X3";
        }

        if (anglerZ < 250f && anglerZ >= 202f)
        {
            earnedExtraGoldText.text = (gold * 4 * _multiplyStarNumber).ToString();
            getExtraGoldText.text = "GET EXTRA X4";
        }

        if (anglerZ < 202f && anglerZ >= 180f)
        {
            earnedExtraGoldText.text = (gold * 5 * _multiplyStarNumber).ToString();
            getExtraGoldText.text = "GET EXTRA X5";
        }
    }

    private void SetGoldZeroOnStart()
    {
        gold = 0;
    }

    private void SetPlayerPrefs() // Setting PlayerPrefs On Start
    {
        if (!PlayerPrefs.HasKey("TotalGold"))
        {
            PlayerPrefs.SetInt("TotalGold", gold);
        }

        if (!PlayerPrefs.HasKey("SliderLevel"))
        {
            PlayerPrefs.SetInt("SliderLevel", sliderLevel);
        }

        sliderLevelText.text = PlayerPrefs.GetInt("SliderLevel").ToString();
    }

    #endregion
}