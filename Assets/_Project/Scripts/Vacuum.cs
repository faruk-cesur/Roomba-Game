using System;
using DG.Tweening;
using UnityEngine;


public class Vacuum : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        Garbage garbage = other.GetComponentInParent<Garbage>();
        if (garbage)
        {
            UIManager.Instance.gold++;
            UIManager.Instance.garbageSlider.value++;
            other.gameObject.GetComponent<Collider>().enabled = false;
            other.gameObject.transform.SetParent(transform);
            other.gameObject.transform.DOLocalMove(new Vector3(0, 0, 0), UIManager.Instance.playerController.robotSweepPower);
        }
    }
}