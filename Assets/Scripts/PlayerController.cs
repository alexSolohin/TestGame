using System;
using System.Collections;
using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class PlayerController : MonoBehaviour
{
    [Header("Move settings")]
    [Range(0, 100)]
    [SerializeField] private float speed;
    
    [Header("Mouse settings")]
    [Range(0, 100)]
    [SerializeField] private float sensitivityH;
    [Range(0, 100)]
    [SerializeField] private float sensitivityV;
    
    [Range(-90, 0)]
    [SerializeField] private float minV;
    [Range(0, 90)]
    [SerializeField] private float maxV;

    [Header("Gun")] 
    [SerializeField] private GameObject bulletPref;
    [SerializeField] private Transform spawnTransform;
    [Range(0, 100)] 
    [SerializeField] private float speedBullet;

    [SerializeField] private Transform _cameraTransform;
    private Camera _camera;
    
    private float _rotX = 0;
    private CharacterController _charController;
    private float g = Physics.gravity.y;

    private MeshRenderer currentSelected;
    private Color defaultEnemyColor;

    private void Start()
    {
        _charController = GetComponent<CharacterController>();
        _camera = _cameraTransform.GetComponent<Camera>();
    }

    private void OnCollisionEnter(Collision other)
    {
        if (other.transform.CompareTag("BulletEnemy"))
        {
            _charController.Move(Vector3.back);
        }
    }

    void Update()
    {
        Move();
        Rotate();
        Raycast();
        
        if (Input.GetMouseButtonDown(0))
            ShotBullet();
        if (transform.position.y <= -1f)
            GameManager.GameOver?.Invoke("Проиграл");
        else if (transform.position.z >= 40f)
            GameManager.GameOver?.Invoke("Выйграл");
    }

    private void Raycast()
    {
        var point = new Vector3(_camera.pixelWidth / 2, _camera.pixelHeight / 2, 0);
        Ray ray = _camera.ScreenPointToRay(point);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, Single.MaxValue, 1 << 7))
        {
            var selected = hit.transform.GetComponent<MeshRenderer>();
            if (currentSelected == null)
            {
                currentSelected = selected;
                defaultEnemyColor = currentSelected.material.color;
                currentSelected.material.color = Color.yellow;
            }
            else if (currentSelected && currentSelected.GetInstanceID() != selected.GetInstanceID())
            {
                currentSelected.material.color = defaultEnemyColor;
                currentSelected = selected;
                currentSelected.material.color = Color.yellow;
            }
        }
        else
        {
            if (!currentSelected) return;
            currentSelected.material.color = defaultEnemyColor;
            currentSelected = null;
        }
    }

    private void ShotBullet()
    {
        var bullet = Instantiate(bulletPref, spawnTransform.position, Quaternion.identity);
        bullet.transform.rotation = _cameraTransform.rotation;
        bullet.GetComponent<Rigidbody>().velocity = _cameraTransform.forward * speedBullet;
        Destroy(bullet, 3f);
    }

    private void Move()
    {
        var deltaX = Input.GetAxis("Horizontal") * speed;
        var deltaZ = Input.GetAxis("Vertical") * speed;
        var movement = new Vector3(deltaX, 0, deltaZ);
        movement = Vector3.ClampMagnitude(movement, speed);

        movement.y = g;
        movement *= Time.deltaTime;
        movement = transform.TransformDirection(movement);
        _charController.Move(movement);
    }

    private void Rotate()
    {
        _rotX -= Input.GetAxis("Mouse Y") * sensitivityV * Time.deltaTime;
        _rotX = Mathf.Clamp(_rotX, minV, maxV);

        var delta = Input.GetAxis("Mouse X") * sensitivityH * Time.deltaTime;
        var rotY = _cameraTransform.localEulerAngles.y + delta;

        _cameraTransform.localEulerAngles = new Vector3(_rotX, rotY, 0);
    }
    
}
