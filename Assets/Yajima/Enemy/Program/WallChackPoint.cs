using UnityEngine;
using System.Collections;

public class WallChackPoint : MonoBehaviour
{
    private bool m_IsWallHit = false;   // 壁に衝突したか
    private Vector2 m_Direction = Vector2.right;

    // Use this for initialization
    // void Start() { }

    // Update is called once per frame
    void Update() { m_IsWallHit = false; }

    // 壁に衝突したか
    public bool IsWallHit() { return m_IsWallHit; }

    // 方向を変えます
    public void ChangeDirection() {
        m_Direction *= -1;
        var pos = gameObject.transform.localPosition;
        var newPos = new Vector3(
            pos.x * m_Direction.x,
            pos.y * m_Direction.y,
            0.0f
            );
        gameObject.transform.localPosition = newPos;
    }

    public void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag != "Wall") return;
        m_IsWallHit = true;
    }
}
