using UnityEngine;
using System.Collections;

public class Bullet : MonoBehaviour
{
    [SerializeField]
    float lifetime = 3f;

    void OnEnable()
    {
        // 총알이 활성화될 때마다 코루틴 시작
        StartCoroutine(DeactivateAfterTime());
    }

    IEnumerator DeactivateAfterTime()
    {
        yield return new WaitForSeconds(lifetime);
        BulletPoolManager.Instance.ReturnBullet(gameObject);
        
    }

    void Update()
    {
        transform.Translate(Vector3.forward * 10 * Time.deltaTime);
    }

    private void OnDisable()
    {
        StopAllCoroutines();
    }
}