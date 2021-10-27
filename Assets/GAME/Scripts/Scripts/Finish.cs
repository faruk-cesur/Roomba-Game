using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Finish : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        PlayerController player = other.GetComponentInParent<PlayerController>();
        if (player)
        {
            player.PlayerSpeedDown();
            PlayerPrefs.SetInt("TotalGold", UIManager.Instance.gold + PlayerPrefs.GetInt("TotalGold"));
            AnimationController.Instance.WinAnimation();
            UIManager.Instance.UpdateGoldInfo();
            GameManager.Instance.WinGame();
            GameManager.Instance.CurrentGameState = GameState.WinGame;
            SoundManager.Instance.PlaySound(SoundManager.Instance.winGameSound, 1);
        }
    }
}