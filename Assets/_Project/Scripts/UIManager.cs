using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;


public class UIManager : MonoBehaviour
{
    private static UIManager _instance;
    public static UIManager Instance => _instance;

    public PlayerController player;

    public Slider distanceSlider;

    public TextMeshProUGUI currentGoldText,
        earnedGoldText,
        earnedExtraGoldText,
        getExtraGoldText,
        prepareTotalGoldText,
        winTotalGoldText,
        sliderLevelText;

    [HideInInspector] public int sliderLevel = 1, gold;

    [SerializeField] private GameObject _prepareGameUI,
        _mainGameUI,
        _loseGameUI,
        _winGameUI,
        _extraGoldArrow,
        _getButton,
        _getExtraButton,
        _extraGoldPanel;

    [SerializeField] private List<GameObject> _goldenCoins;
    [SerializeField] private List<GameObject> _yellowStars;

    private float _anglerBonusArrowZ, _time = 1f;

    private void Awake()
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
                UpdateGoldInfo();
                break;
        }
    }

    public void PrepareGameUI()
    {
        foreach (var yellowStar in _yellowStars)
        {
            yellowStar.SetActive(false);
        }
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
        _prepareGameUI.SetActive(false);
        _mainGameUI.SetActive(false);
        _loseGameUI.SetActive(true);
        _winGameUI.SetActive(false);
    }

    public void WinGameUI()
    {
        _prepareGameUI.SetActive(false);
        _mainGameUI.SetActive(false);
        _loseGameUI.SetActive(false);
        _winGameUI.SetActive(true);
    }


    public void UpdateGoldInfo()
    {
        CalculateBonusArrowRotation();
        currentGoldText.text = gold.ToString();
        earnedGoldText.text = currentGoldText.text;
        prepareTotalGoldText.text = PlayerPrefs.GetInt("TotalGold").ToString();
        winTotalGoldText.text = PlayerPrefs.GetInt("TotalGold").ToString();
    }


    public void CalculateStars()
    {
        if (distanceSlider.value > 60)
        {
            _yellowStars[0].SetActive(true);
        }

        if (distanceSlider.value > 133)
        {
            _yellowStars[1].SetActive(true);
        }

        if (distanceSlider.value > 205)
        {
            _yellowStars[2].SetActive(true);
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


    public void GetExtraButton()
    {
        foreach (var goldenCoin in _goldenCoins)
        {
            goldenCoin.SetActive(true);
            goldenCoin.transform.DOLocalMove(new Vector3(38.7f, -50f, 0), _time);
            _time += 0.05f;
        }

        if (_anglerBonusArrowZ <= 360 && _anglerBonusArrowZ >= 306f)
        {
            PlayerPrefs.SetInt("TotalGold", gold * 2 + PlayerPrefs.GetInt("TotalGold"));
        }

        if (_anglerBonusArrowZ < 306f && _anglerBonusArrowZ >= 250f)
        {
            PlayerPrefs.SetInt("TotalGold", gold * 3 + PlayerPrefs.GetInt("TotalGold"));
        }

        if (_anglerBonusArrowZ < 250f && _anglerBonusArrowZ >= 202f)
        {
            PlayerPrefs.SetInt("TotalGold", gold * 4 + PlayerPrefs.GetInt("TotalGold"));
        }

        if (_anglerBonusArrowZ < 202f && _anglerBonusArrowZ >= 180f)
        {
            PlayerPrefs.SetInt("TotalGold", gold * 5 + PlayerPrefs.GetInt("TotalGold"));
        }

        _getButton.SetActive(false);
        _getExtraButton.SetActive(false);
        _extraGoldPanel.SetActive(false);
        GameManager.Instance.NextLevel();
    }

    public void GetButton()
    {
        foreach (var goldenCoin in _goldenCoins)
        {
            goldenCoin.SetActive(true);
            goldenCoin.transform.DOLocalMove(new Vector3(38.7f, -50f, 0), _time);
            _time += 0.05f;
        }

        PlayerPrefs.SetInt("TotalGold", gold + PlayerPrefs.GetInt("TotalGold"));
        _getButton.SetActive(false);
        _getExtraButton.SetActive(false);
        _extraGoldPanel.SetActive(false);
        GameManager.Instance.NextLevel();
    }

    private void CalculateBonusArrowRotation()
    {
        var anglerZ = _extraGoldArrow.transform.localEulerAngles.z;
        _anglerBonusArrowZ = anglerZ;
        if (anglerZ <= 360 && anglerZ >= 306f)
        {
            earnedExtraGoldText.text = (gold * 2).ToString();
            getExtraGoldText.text = "GET EXTRA X2";
        }

        if (anglerZ < 306f && anglerZ >= 250f)
        {
            earnedExtraGoldText.text = (gold * 3).ToString();
            getExtraGoldText.text = "GET EXTRA X3";
        }

        if (anglerZ < 250f && anglerZ >= 202f)
        {
            earnedExtraGoldText.text = (gold * 4).ToString();
            getExtraGoldText.text = "GET EXTRA X4";
        }

        if (anglerZ < 202f && anglerZ >= 180f)
        {
            earnedExtraGoldText.text = (gold * 5).ToString();
            getExtraGoldText.text = "GET EXTRA X5";
        }
    }

    private void SetGoldZeroOnStart()
    {
        gold = 0;
    }

    private void SetPlayerPrefs()
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
}