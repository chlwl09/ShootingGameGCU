using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerScript : MonoBehaviour
{
    private Rigidbody rigid;
    private float moveForce = 7.0f;
    private float x_Axis;
    private float z_Axis;
    [SerializeField]
    private float power;

    [SerializeField]
    private GameObject bullet;
    private GameObject[] bulletPool;
    public Transform FirePos;

    [SerializeField]
    private GameObject shieldPrefab;
    [SerializeField]
    private GameObject currentShield;
    private bool isShieldActive = false;
    private float shieldDuration = 3f;
    private float shieldCooldown = 5f;
    private bool isShieldOnCooldown = false;

    // Start is called before the first frame update
    void Start()
    {
        rigid = GetComponent<Rigidbody>();
        InitializeBulletPool();
    }
    private void InitializeBulletPool()
    {
        bulletPool = new GameObject[10]; // ���÷� 10���� �Ѿ��� Ǯ���մϴ�.
        for (int i = 0; i < bulletPool.Length; i++)
        {
            bulletPool[i] = Instantiate(bullet);
            bulletPool[i].SetActive(false);
        }
    }
    // Update is called once per frame
    void Update()
    {
        PlayerMovement();
        Shooting();
        Shield();
        CamerainPlayer();
    }

    private void PlayerMovement()
    {
        x_Axis = Input.GetAxis("Horizontal");
        z_Axis = Input.GetAxis("Vertical");

        Vector3 velocity = new Vector3(x_Axis, 0, z_Axis);
        velocity *= moveForce;
        rigid.velocity = velocity;

        foreach (Transform child in transform)
        {
            child.localPosition = Vector3.zero;
        }
    }

    private void Shooting()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            GameObject bullet = GetNextBulletFromPool();
            if (bullet != null)
            {
                bullet.transform.position = FirePos.position;
                bullet.transform.rotation = FirePos.rotation;
                bullet.SetActive(true);
            }
        }
    }

    private GameObject GetNextBulletFromPool()
    {
        for (int i = 0; i < bulletPool.Length; i++)
        {
            if (bulletPool[i] != null && !bulletPool[i].activeInHierarchy)
            {
                return bulletPool[i];
            }
        }
        return null;
    }

    void Shield()
    {
        if (Input.GetKeyDown(KeyCode.LeftShift) && !isShieldActive && !isShieldOnCooldown)
        {
            ActivateShield();
        }
    }

    void ActivateShield()
    {
        Vector3 offset = transform.forward * 0.8f;
        // ���带 �÷��̾��� ��ġ�� offset�� ���� ������ �̵����� �����մϴ�.
        currentShield = Instantiate(shieldPrefab, transform.position + offset, transform.rotation);
        // ���� Ȱ��ȭ ���·� �����մϴ�.
        isShieldActive = true;

        // ���� �ð��� ���� �Ŀ� ���带 ��Ȱ��ȭ�ϰ� ��Ÿ���� �����մϴ�.
        StartCoroutine(DisableShieldAfterDelay(shieldDuration));
    }

    IEnumerator DisableShieldAfterDelay(float delay)
    {
        // delay �� �Ŀ� ���带 ��Ȱ��ȭ�մϴ�.
        yield return new WaitForSeconds(delay);

        // ���带 ��Ȱ��ȭ�ϰ� �޸𸮿��� �����մϴ�.
        Destroy(currentShield);
        isShieldActive = false;

        // ��Ÿ���� �����մϴ�.
        StartCoroutine(StartShieldCooldown());
    }

    IEnumerator StartShieldCooldown()
    {
        // ��Ÿ�� ���� ����մϴ�.
        isShieldOnCooldown = true;
        yield return new WaitForSeconds(shieldCooldown);
        isShieldOnCooldown = false;
    }

    void CamerainPlayer()
    {
        Vector3 worldpos = Camera.main.WorldToViewportPoint(this.transform.position);
        worldpos.x = Mathf.Clamp01(worldpos.x);
        worldpos.y = Mathf.Clamp01(worldpos.y);
        this.transform.position = Camera.main.ViewportToWorldPoint(worldpos);
    }
}