using System;
using System.Collections;
using UnityEngine;


public class PlayerController : MonoBehaviour
{
    #region Fields

    [SerializeField] private float _robotSpeed;

    #endregion

    private void Update()
    {
        switch (GameManager.Instance.CurrentGameState)
        {
            case GameState.PrepareGame:
                break;
            case GameState.MainGame:
                break;
            case GameState.LoseGame:
                break;
            case GameState.WinGame:
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    #region PlayerMovement

    #endregion


    #region Methods

    private void OnTriggerEnter(Collider other)
    {
        Garbage garbage = other.GetComponentInParent<Garbage>();
        if (garbage)
        {
            UIManager.Instance.gold++;
            //Instantiate(particleCollectable, playerModel.transform.position + new Vector3(0, 2, 0), Quaternion.identity);
            SoundManager.Instance.PlaySound(SoundManager.Instance.collectGarbageSound, 0.4f);
            Destroy(other.gameObject);
        }
    }

    #endregion
}