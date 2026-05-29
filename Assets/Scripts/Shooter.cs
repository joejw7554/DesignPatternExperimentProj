using UnityEngine;
using UnityEngine.InputSystem;  // 추가

public class Shooter : MonoBehaviour
{
    [SerializeField]
    Transform firePoint;

    void Update()
    {
        // 새로운 Input System 사용
        if(Mouse.current.leftButton.wasPressedThisFrame)
        {
            Shoot();
        }
    }

    void Shoot()
    {
        GameObject bullet = BulletPoolManager.Instance.GetBullet();
        
        if(firePoint != null)
        {
            bullet.transform.position = firePoint.position;
            bullet.transform.rotation = firePoint.rotation;
        }
        else
        {
            bullet.transform.position = transform.position;
            bullet.transform.rotation = transform.rotation;
        }
    }
}