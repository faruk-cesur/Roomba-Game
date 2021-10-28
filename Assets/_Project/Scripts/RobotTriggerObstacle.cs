using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RobotTriggerObstacle : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        Obstacle obstacle = other.GetComponentInParent<Obstacle>();
        if (obstacle)
        {
            Taptic.Heavy();
            UIManager.Instance.playerController.robotExplosionParticle.SetActive(true);
            GameManager.Instance.LoseGame();
            SoundManager.Instance.PlaySound(SoundManager.Instance.explosionSound, 0.4f);
            StartCoroutine(SoundManager.Instance.LoseGameSound());
            UIManager.Instance.dontTouchHousewaresText.enabled = true;
        }
    }
}