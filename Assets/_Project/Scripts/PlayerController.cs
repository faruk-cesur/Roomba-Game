﻿using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Rigidbody), typeof(LineRenderer))]
public class PlayerController : MonoBehaviour
{
    public GameObject robotExplosionParticle;
    public float robotSweepPower;

    [SerializeField] private List<GameObject> _wayPoints;
    [SerializeField] private float _timeForNextRay;
    [SerializeField] private float _robotMoveSpeed;

    private Rigidbody _rigidbody;
    private LineRenderer _lineRenderer;
    private Camera _cam;
    private float _timer = 0;
    private int _wayIndex;
    private int _currentWayPoint = 0;
    private bool _isRobotMove;
    private bool _isMouseClickedOnRobot;

    private void Awake()
    {
        _rigidbody = GetComponent<Rigidbody>();
        _lineRenderer = GetComponent<LineRenderer>();
        _cam = Camera.main;
        _lineRenderer.material = new Material(Shader.Find("Sprites/Default"));
        _lineRenderer.SetColors(new Color(1, 1, 1, 0.5f), new Color(1, 1, 1, 0.5f));
    }

    private void Start()
    {
        _lineRenderer.enabled = false;
        _isRobotMove = false;
        _isMouseClickedOnRobot = false;
        _wayIndex = 1;
    }

    private void Update()
    {
        switch (GameManager.Instance.CurrentGameState)
        {
            case GameState.PrepareGame:
                Movement();
                break;
            case GameState.MainGame:
                Movement();
                break;
            case GameState.LoseGame:
                break;
            case GameState.WinGame:
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    public void OnMouseDown()
    {
        _isMouseClickedOnRobot = true;
        _lineRenderer.enabled = true;
        _lineRenderer.positionCount = 1;
        _lineRenderer.SetPosition(0, transform.position);
        GameManager.Instance.StartGame();
    }

    public void Movement()
    {
        _timer += Time.deltaTime;

        if (Input.GetMouseButton(0) && _isMouseClickedOnRobot && _timer > _timeForNextRay && !_isRobotMove)
        {
            Vector3 mousePos = _cam.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, 100f));
            Vector3 direction = mousePos - _cam.transform.position;

            RaycastHit hit;


            if (Physics.Raycast(_cam.transform.position, direction, out hit, 100f))
            {
                GameObject newWayPoint = new GameObject("WayPoint");
                newWayPoint.transform.position = new Vector3(hit.point.x, transform.position.y, hit.point.z);
                _wayPoints.Add(newWayPoint);
                _lineRenderer.positionCount = _wayIndex + 1;
                _lineRenderer.SetPosition(_wayIndex, newWayPoint.transform.position);
                _timer = 0;
                _wayIndex++;
            }
        }

        if (Input.GetMouseButtonUp(0))
        {
            _isRobotMove = true;
            _isMouseClickedOnRobot = false;
        }

        if (_isRobotMove)
        {
            transform.LookAt(_wayPoints[_currentWayPoint].transform);
            Vector3 dir = _wayPoints[_currentWayPoint].transform.position - transform.position;
            _rigidbody.MovePosition(transform.position + dir.normalized * _robotMoveSpeed * Time.deltaTime);

            if (dir.magnitude < .75f)
            {
                _currentWayPoint++;
            }

            if (_currentWayPoint == _wayPoints.Count)
            {
                if (UIManager.Instance.garbageSlider.value > 60)
                {
                    GameManager.Instance.WinGame();
                    SoundManager.Instance.PlaySound(SoundManager.Instance.winGameSound, 0.4f);
                }
                else
                {
                    GameManager.Instance.LoseGame();
                    UIManager.Instance.collectMoreGarbageText.enabled = true;
                }
            }
        }
    }
}