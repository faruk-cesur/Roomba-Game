﻿using System;
using System.Collections;
using System.Collections.Generic;
using NaughtyAttributes;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelManager : MonoBehaviour
{
    private static LevelManager _instance;
    public static LevelManager Instance => _instance;

    public List<GameObject> levels;

    [HideInInspector] public int currentLevel;

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
        SetLevelPlayerPrefs();
        CallLevel();
    }

    public void SetLevelPlayerPrefs() // If it is first start, create playerprefs and keep the current level
    {
        if (!PlayerPrefs.HasKey("CurrentLevel"))
        {
            currentLevel = 1;
            PlayerPrefs.SetInt("CurrentLevel", currentLevel);
        }
        else
        {
            currentLevel = PlayerPrefs.GetInt("CurrentLevel");
        }
    }

    public void CallLevel() // Calling The Current Level From PlayerPrefs
    {
        if (currentLevel > levels.Count)
        {
            currentLevel = 1;
            PlayerPrefs.SetInt("CurrentLevel", currentLevel);
            levels[PlayerPrefs.GetInt("CurrentLevel") - 1].SetActive(true);
        }
        else
        {
            levels[PlayerPrefs.GetInt("CurrentLevel") - 1].SetActive(true);
        }
    }

    public IEnumerator NextLevel()
    {
        yield return new WaitForSeconds(3f);
        currentLevel++;
        PlayerPrefs.SetInt("CurrentLevel", currentLevel);
        Scene currentScene = SceneManager.GetActiveScene();
        SceneManager.LoadScene(currentScene.buildIndex);
    }
}