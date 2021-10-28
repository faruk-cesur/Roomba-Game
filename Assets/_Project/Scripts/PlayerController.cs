using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody), typeof(LineRenderer))]
public class PlayerController : MonoBehaviour
{
    [SerializeField] private List<GameObject> _wayPoints;
    [SerializeField] private GameObject _robotDeathParticle;
    [SerializeField] private float _timeForNextRay;
    [SerializeField] private float _robotSpeed;

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
                Debug.DrawLine(_cam.transform.position, direction, Color.white, 1f);
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
            _rigidbody.MovePosition(_wayPoints[_currentWayPoint].transform.position);

            if (transform.position == _wayPoints[_currentWayPoint].transform.position)
            {
                _currentWayPoint++;
            }

            if (_currentWayPoint == _wayPoints.Count)
            {
                if (UIManager.Instance.distanceSlider.value > 60)
                {
                    GameManager.Instance.WinGame();
                }
                else
                {
                    GameManager.Instance.LoseGame();
                }

                foreach (var wayPoint in _wayPoints)
                {
                    Destroy(wayPoint);
                    _wayPoints.Clear();
                    _wayIndex = 1;
                    _currentWayPoint = 0;
                }
            }
        }
    }


    private void OnTriggerEnter(Collider other)
    {
        Garbage garbage = other.GetComponentInParent<Garbage>();
        if (garbage)
        {
            Taptic.Light();
            UIManager.Instance.gold++;
            UIManager.Instance.distanceSlider.value++;
            //Instantiate(particleCollectable, playerModel.transform.position + new Vector3(0, 2, 0), Quaternion.identity);
            SoundManager.Instance.PlaySound(SoundManager.Instance.collectGarbageSound, 0.4f);
            Destroy(other.gameObject);
        }
        
        Obstacle obstacle = other.GetComponentInParent<Obstacle>();
        if (obstacle)
        {
            Taptic.Heavy();
            _robotDeathParticle.SetActive(true);
            GameManager.Instance.LoseGame();
            //Instantiate(particleCollectable, playerModel.transform.position + new Vector3(0, 2, 0), Quaternion.identity);
            SoundManager.Instance.PlaySound(SoundManager.Instance.loseGameSound, 0.4f);
        }
    }
}