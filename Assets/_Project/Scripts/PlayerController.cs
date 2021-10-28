using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Rigidbody), typeof(LineRenderer))]
public class PlayerController : MonoBehaviour
{
    #region Fields

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

    #endregion

    private void Awake()
    {
        _rigidbody = GetComponent<Rigidbody>();
        _lineRenderer = GetComponent<LineRenderer>();
        _cam = Camera.main;
        _lineRenderer.material = new Material(Shader.Find("Sprites/Default"));
        _lineRenderer.SetColors(new Color(1, 1, 1, 0.5f), new Color(1, 1, 1, 0.5f)); // Changing Line Renderer Color To Transparent White From Pink
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

    #region Methods

    public void OnMouseDown() // Line Renderer is enabled when click on player (robot)
    {
        _isMouseClickedOnRobot = true;
        _lineRenderer.enabled = true;
        _lineRenderer.positionCount = 1;
        _lineRenderer.SetPosition(0, transform.position);
        GameManager.Instance.StartGame();
    }

    public void Movement()
    {
        _timer += Time.deltaTime; // Keeping Real Time To Use Later

        if (Input.GetMouseButton(0) && _isMouseClickedOnRobot && _timer > _timeForNextRay && !_isRobotMove) // Holding LMB
        {
            Vector3 mousePos = _cam.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, 100f));
            Vector3 direction = mousePos - _cam.transform.position; // Keeping Mouse Position from Camera's ScreenToWorldPoint Then Find The Direction Between Them.

            RaycastHit hit;

            if (Physics.Raycast(_cam.transform.position, direction, out hit, 100f))
            {
                GameObject newWayPoint = new GameObject("WayPoint"); // Creating a new empty Game Object for every timeForNextRay
                newWayPoint.transform.position = new Vector3(hit.point.x, transform.position.y, hit.point.z); // Change their positions to where raycast hits
                _wayPoints.Add(newWayPoint); // Addings new empty game objects to waypoint list
                _lineRenderer.positionCount = _wayIndex + 1;
                _lineRenderer.SetPosition(_wayIndex, newWayPoint.transform.position); // Setting every line renderer position to new created way points
                _timer = 0;
                _wayIndex++;
            }
        }

        if (Input.GetMouseButtonUp(0)) // Releasing LMB
        {
            _isRobotMove = true;
            _isMouseClickedOnRobot = false;
        }

        if (_isRobotMove)
        {
            transform.LookAt(_wayPoints[_currentWayPoint].transform); // Robot looking at the waypoints while moving
            Vector3 dir = _wayPoints[_currentWayPoint].transform.position - transform.position;
            _rigidbody.MovePosition(transform.position + dir.normalized * _robotMoveSpeed * Time.deltaTime); //Robot Moves The Position of WayPoints One by one

            if (dir.magnitude < .75f)
            {
                _currentWayPoint++; // Goes to the next waypoint
            }

            if (_currentWayPoint == _wayPoints.Count) // If all waypoints completed
            {
                if (UIManager.Instance.garbageSlider.value > 60) // If player got at least one star, wins the game
                {
                    GameManager.Instance.WinGame();
                }
                else // If player doesn't have any star, loses the game
                {
                    GameManager.Instance.LoseGame();
                    UIManager.Instance.collectMoreGarbageText.enabled = true;
                }
            }
        }
    }

    #endregion
}