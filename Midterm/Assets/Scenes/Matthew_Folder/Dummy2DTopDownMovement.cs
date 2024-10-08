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
        string[] tags = { "Bullet" };
        GameObject[] objectsToIgnore = FindGameObjectsWithTags(tags).ToArray();
        foreach (GameObject obj in objectsToIgnore)
        {
            Collider2D objCollider = obj.GetComponent<Collider2D>();
            if (objCollider != null)
            {
                Physics2D.IgnoreCollision(GetComponent<Collider2D>(), objCollider);
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (PauseMenu.GameIsPaused)
        {
            return; // Skip the rest of the update logic if the game is paused
        }
        if (isDashing)
        {
            return;
        }
        moveInput.x = Input.GetAxisRaw("Horizontal");
        moveInput.y = Input.GetAxisRaw("Vertical");
        moveInput.Normalize();
        

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

    void FixedUpdate(){
        rb2d.velocity = moveInput * moveSpeed;
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
        
        float dashTime = 1.5f;

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

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Lava"))
        {
            DamageIFrame damageIFrame = GetComponent<DamageIFrame>();
            if (damageIFrame != null)
            {
                damageIFrame.damageLogic();
            }
        }
    }

    private List<GameObject> FindGameObjectsWithTags(string[] tags)
    {
        List<GameObject> combinedList = new List<GameObject>();
        for (int i = 0; i < tags.Length; i++)
        {
            GameObject[] taggedObjects = GameObject.FindGameObjectsWithTag(tags[i]); 
            combinedList.AddRange(taggedObjects); 
        }
        return combinedList;
    }
}