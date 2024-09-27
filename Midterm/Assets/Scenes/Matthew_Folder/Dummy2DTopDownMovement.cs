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

    [Header("Movement Settings")]

    // Start is called before the first frame update
    [SerializeField] public float moveSpeed;
    [SerializeField] public float dashSpeed = 15f;
    [SerializeField] public float dashDuration = 0.25f;
    [SerializeField] public float dashCooldown = 1f;
    [SerializeField] public float decelerationRate = 10f; // Rate at which the dash decelerates
    public Rigidbody2D rb2d;
    private Vector2 moveInput;
    private Transform m_transform;
    private int currentAmmo;
    private bool isFiring = false; // Flag to check if firing is in progress
    private bool isReloading = false; // Flag to check if reloading is in progress
    private bool isDashing = false;
    private bool canDash = true;

    void Start()
    {
        canDash = true;
        rb2d = GetComponent<Rigidbody2D>();
        m_transform = this.transform;
        currentAmmo = maxAmmo;
        UpdateAmmoText();
    }

    // Update is called once per frame
    void Update()
    {
        RotateTowardsMouse();
        if (isDashing)
        {
            return;
        }
        moveInput.x = Input.GetAxisRaw("Horizontal");
        moveInput.y = Input.GetAxisRaw("Vertical");
        moveInput.Normalize();
        rb2d.velocity = moveInput * moveSpeed;

        

        if (Input.GetMouseButton(0) && currentAmmo > 0 && !isFiring)
        {
            StartCoroutine(FireContinuously());
        }

        if (Input.GetKeyDown(KeyCode.R) && !isReloading)
        {
            StartCoroutine(ReloadWithDelay());
        }

        if ((Input.GetKeyDown(KeyCode.Space) && !isDashing) && canDash) 
        {
            StartCoroutine(Dash());
        }
    }

    private IEnumerator Dash()
    {
        isDashing = true;
        canDash = false;
        Vector2 dashDirection = new Vector2(moveInput.x, moveInput.y).normalized;
        if (moveInput == Vector2.zero)
        {
            dashDirection = new Vector2(1, 0).normalized;
        }
        
        float dashTime = 0f;

        while (dashTime < dashDuration)
        {
            rb2d.velocity = dashDirection * Mathf.Lerp(dashSpeed, 0, dashTime / dashDuration);
            dashTime += Time.deltaTime;
            yield return null;
        }

        rb2d.velocity = Vector2.zero;
        isDashing = false;
        yield return new WaitForSeconds(dashCooldown);
        canDash = true;
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
        Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector2 direction = (mousePosition - transform.position).normalized;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(new Vector3(0, 0, angle - 90));
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