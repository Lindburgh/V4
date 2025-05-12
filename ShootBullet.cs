using UnityEngine;

public class ShootBullet : MonoBehaviour
{
    public GameObject bulletPrefab; // 弾のプレハブ
    public float bulletSpeed = 10f; // 弾の速度

    void Update()
    {
        // PC用：スペースキーが押されたとき
        if (Input.GetKeyDown(KeyCode.Space))
        {
            Shoot();
        }

        // スマホ/タブレット用：画面タッチが検出されたとき
        if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began)
        {
            Shoot();
        }

        // ※ エディタでのマウスクリックでも試したい場合は以下も追加可
        // if (Input.GetMouseButtonDown(0)) { Shoot(); }
    }

    void Shoot()
    {
        // プレイヤーの位置に弾を生成
        GameObject bullet = Instantiate(bulletPrefab, transform.position, Quaternion.identity);

        // Rigidbody2Dを使って真上に速度を与える
        Rigidbody2D rb = bullet.GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            rb.velocity = Vector2.up * bulletSpeed;
        }
    }
}
