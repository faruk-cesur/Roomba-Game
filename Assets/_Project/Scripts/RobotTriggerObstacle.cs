using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RobotTriggerObstacle : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        Obstacle obstacle = other.GetComponentInParent<Obstacle>();  // If Robot Collides With An Obstacle
        if (obstacle)
        {
            Taptic.Heavy(); // Vibration
            UIManager.Instance.playerController.robotExplosionParticle.SetActive(true); // Explosion particle
            GameManager.Instance.LoseGame();
            SoundManager.Instance.PlaySound(SoundManager.Instance.explosionSound, 1);
            StartCoroutine(SoundManager.Instance.LoseGameSound());
            UIManager.Instance.dontTouchHousewaresText.enabled = true;
        }
    }
}