using System.Collections; 
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Dummy2DTopDownMovement : MonoBehaviour
{
    [SerializeField] private GameObject bulletPrefab;
    [SerializeField] private Transform bulletSpawnPoint;
    [SerializeField] private int maxAmmo = 22;
    // [Range(0.1f,1f)]
    [SerializeField] private float fireRate = 0.5f;
    [SerializeField] private TMP_Text ammoText;

    // Start is called before the first frame update
    public float moveSpeed;
    public Rigidbody2D rb2d;
    private Vector2 moveInput;
    private Transform m_transform;
    private int currentAmmo;
    private bool isFiring = false; // Flag to check if firing is in progress
    private bool isReloading = false; // Flag to check if reloading is in progress
    void Start()
    {
        rb2d = GetComponent<Rigidbody2D>();
        m_transform = this.transform;
        currentAmmo = maxAmmo;
        UpdateAmmoText();
    }

    // Update is called once per frame
    void Update()
    {
        moveInput.x = Input.GetAxisRaw("Horizontal");
        moveInput.y = Input.GetAxisRaw("Vertical");
        moveInput.Normalize();
        rb2d.velocity = moveInput * moveSpeed;

        RotateTowardsMouse();

        if (Input.GetMouseButton(0) && currentAmmo > 0 && !isFiring)
        {
            StartCoroutine(FireContinuously());
        }

        if (Input.GetKeyDown(KeyCode.R) && !isReloading)
        {
            StartCoroutine(ReloadWithDelay());
        }
    }
    
    private IEnumerator FireContinuously()
    {
        isFiring = true;
        while (Input.GetMouseButton(0) && currentAmmo > 0)
        {
            Fire();
            yield return new WaitForSeconds(fireRate);
        }
        isFiring = false;
    }

    private void Fire()
    {
        Instantiate(bulletPrefab, bulletSpawnPoint.position, bulletSpawnPoint.rotation);
        currentAmmo--; // Decrease ammo count
        UpdateAmmoText();
    }
    private void RotateTowardsMouse()
    {
        Vector2 direction = Camera.main.ScreenToWorldPoint(Input.mousePosition) - transform.position;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        Quaternion rotation = Quaternion.AngleAxis(angle - 90, Vector3.forward);
        m_transform.rotation = rotation;
    }
    private IEnumerator ReloadWithDelay()
    {
        isReloading = true;
        yield return new WaitForSeconds(1); // Wait for 1 second
        currentAmmo = maxAmmo; // Reset ammo count
        isReloading = false;
        UpdateAmmoText();
    }
    private void UpdateAmmoText()
    {
        ammoText.SetText("Ammo: " + currentAmmo); // Update the text with the current ammo count
    }
}
