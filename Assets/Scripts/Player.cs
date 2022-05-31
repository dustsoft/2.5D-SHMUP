using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    [SerializeField] private float _speed = 3.5f;
    [SerializeField] private float _focusSpeed = 5.0f;
    private bool _focusMode = false;

    private float _speedX = 1.5f;
    [SerializeField] private GameObject _laserPrefab;
    [SerializeField] private GameObject _tripleShotPrefab;
    [SerializeField] private float _fireRate = 0.5f;
    private float _canFire = -1f;
    [SerializeField] private int _lives = 3;
    private SpawnManager _spawnManager;

    private bool _isTripleShotActive = false;
    private bool _isSpeedActive = false;
    private bool _isShieldActive = false;

    [SerializeField] private GameObject _shieldVisual;

    [SerializeField] private GameObject _rightEngine;
    [SerializeField] private GameObject _leftEngine;

    [SerializeField] private int _score;

    private UIManager _uiManager;

    [SerializeField] private AudioClip _laserSoundClip;
    private AudioSource _audioSource;

    void Start()
    {
        //Starting Player Position
        transform.position = new Vector3(0, 0, 0);

        _spawnManager = GameObject.Find("Spawn_Manager").GetComponent<SpawnManager>();
        _audioSource = GetComponent<AudioSource>();


        if (_spawnManager == null)
        {
            Debug.LogError("The Spawn Manager is NULL.");
        }

        _uiManager = GameObject.Find("Canvas").GetComponent<UIManager>();
        if (_uiManager == null)
        {
            Debug.LogError("The UI Manager is NULL.");
        }

        if (_audioSource == null)
        {
            Debug.LogError("AudioSource on the Player is null!");
        }
        else
        {
            _audioSource.clip = _laserSoundClip;
        }
    }

    void Update()
    {


        CalculateMovement();

        if (Input.GetKey(KeyCode.Space) && Time.time > _canFire)
        {
            FireLaser();
        }
    }

    void CalculateMovement()
    {
        //Player Movement

        if (_focusMode == false)
        {
            NormalMovement();
        }
        if (_focusMode == true)
        {
            FocusMovement();
        }

        if (Input.GetKey(KeyCode.LeftShift))
        {
            _focusMode = true;
        }
        else
        {
            _focusMode = false;
        }

        //clamp Y
        transform.position = new Vector3(transform.position.x, Mathf.Clamp(transform.position.y, -3.8f, 6.0f), 0);

        //clamp X
        transform.position = new Vector3(Mathf.Clamp(transform.position.x, -9.0f, 9.0f), transform.position.y, 0);

    }

    void NormalMovement()
    {
        float horizontalInput = Input.GetAxisRaw("Horizontal");
        float verticalInput = Input.GetAxisRaw("Vertical");

        transform.Translate(Vector3.right * horizontalInput * _speed * Time.deltaTime);
        transform.Translate(Vector3.up * verticalInput * _speed * Time.deltaTime);
    }

    void FocusMovement()
    {
        float horizontalInput = Input.GetAxisRaw("Horizontal");
        float verticalInput = Input.GetAxisRaw("Vertical");

        transform.Translate(Vector3.right * horizontalInput * _focusSpeed * Time.deltaTime);
        transform.Translate(Vector3.up * verticalInput * _focusSpeed * Time.deltaTime);
    }

    void FireLaser()
    {
        {
            _canFire = Time.time + _fireRate;
            
            if (_isTripleShotActive == true)
            {
                Instantiate(_tripleShotPrefab, transform.position, Quaternion.identity);
            }
            else
            {
                Instantiate(_laserPrefab, transform.position + new Vector3(0, 0.98f, 0), Quaternion.identity);
            }

            _audioSource.Play();
        }
    }

    public void Damage()
    {
        if (_isShieldActive == true)
        {
            _isShieldActive = false;
            _shieldVisual.SetActive(false);
            return;
        }
        _lives --;

        if (_lives == 2)
        {
            _leftEngine.SetActive(true);
        }
        else if (_lives == 1)
        {
            _rightEngine.SetActive(true);
        }

        _uiManager.UpdateLives(_lives);

        if (_lives < 1)
        {
            _spawnManager.OnPlayerDeath();
            Destroy(this.gameObject);
        }


    }

    public void TripleShotActive()
    {
        _isTripleShotActive = true;
        StartCoroutine(TripleShotPowerDownRoutine());
    }

    IEnumerator TripleShotPowerDownRoutine()
    {
        yield return new WaitForSeconds(9.5f);
        _isTripleShotActive = false;
    }

    public void SpeedBoostActive()
    {
        _isSpeedActive = true;
        _speed *= _speedX;
        StartCoroutine(SpeedBoostPowerDownRoutine());
    }

    IEnumerator SpeedBoostPowerDownRoutine()
    {
        yield return new WaitForSeconds(5.0f);
        _isSpeedActive = false;
        _speed /= _speedX;
    }

    public void ShieldActive()
    {
        _isShieldActive = true;
        _shieldVisual.SetActive(true);

    }

    public void AddScore(int points)
    {
        _score += points;
        _uiManager.UpdateScore(_score);
    }
}
