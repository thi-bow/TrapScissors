using UnityEngine;
using System.Collections;
//using 

public class Enemy : MonoBehaviour
{
    public float m_Speed = 1.0f;                    // 移動速度
    public Transform m_GroundPoint;                 // 接地ポイント
    public WallChackPoint m_WChackPoint;            // 壁捜索ポイント

    //protected Vector2 m_Position;                   // 位置ベクトル
    protected int m_Size = 0;                       // 動物の大きさ(内部数値)
    protected Vector2 m_Direction = Vector2.right;  // 方向
    protected Vector2 m_Velocity = Vector2.right;   // 移動量
    protected Rigidbody2D m_Rigidbody;

    // モーション番号
    protected int m_MotionNumber = (int)AnimationNumber.ANIME_IDEL_NUMBER;

    private State m_State = State.Idel;             // 状態
    private float m_StateTimer = 0.0f;              // 状態の時間
    //protected // アニメーション用のテクスチャリスト

    protected enum State
    {
        Idel,       // 待機状態
        Chase,      // 追跡状態
        Discover,   // 発見状態
        TrapHit,    // トラバサミに挟まれている状態
        Runaway,    // 逃亡状態
    }

    protected enum AnimationNumber
    {
        ANIME_IDEL_NUMBER = 0,
        ANIME_CHASE_NUMBER = 1,
        ANIME_DISCOVER_NUMBER = 2,
        ANIME_TRAP_NUMBER = 3,
        ANIME_RUNAWAY_NUMBER = 4,
        ANIME_DEAD_NUMBER = 5
    };

    // Use this for initialization
    void Start()
    {
        // アニメーションリストにリソースを追加
        m_Rigidbody = GetComponent<Rigidbody2D>();
        //Collider2D collider = GetComponent<Collider2D>();
        CircleCollider2D collider = GetComponent<CircleCollider2D>();

        if (m_WChackPoint != null)
        {
            m_WChackPoint.transform.position =
                this.transform.position + Vector3.right * collider.radius;
        }
    }

    // Update is called once per frame
    void Update()
    {
        // 状態の更新
        UpdateState(Time.deltaTime);
    }

    // 状態の更新
    private void UpdateState(float deltaTime)
    {
        // 状態の変更
        switch (m_State)
        {
            case State.Idel: Idel(deltaTime); break;
            case State.Discover: Discover(deltaTime); break;
            case State.Chase: Chase(deltaTime); break;
            case State.Runaway: Runaway(deltaTime); break;
        };

        if (isGround()) m_Rigidbody.gravityScale = 0.0f;

        // 状態の時間加算
        m_StateTimer += deltaTime;

        // 位置ベクトルを代入
        m_Rigidbody.velocity += m_Velocity;
        //m_Rigidbody.velocity.
        m_Velocity = Vector2.zero;
    }

    // 状態の変更
    protected void ChangeState(State state, AnimationNumber motion)
    {
        if (m_State == state) return;
        m_State = state;
        m_MotionNumber = (int)motion;
        m_StateTimer = 0.0f;
    }

    // 待機状態
    protected void Idel(float deltaTime)
    {
        if (InPlayer())
        {
            // 発見状態に遷移
            ChangeState(State.Discover, AnimationNumber.ANIME_IDEL_NUMBER);
            return;
        };
        // 移動
        Move(deltaTime);
    }

    // 発見状態
    protected void Discover(float deltaTime)
    {

    }

    // 追跡状態
    protected void Chase(float deltaTime)
    {

    }

    // トラバサミに挟まれている状態
    protected void TrapHit(float deltaTime)
    {

    }

    // 逃げ状態
    protected void Runaway(float deltaTime)
    {
        //find
    }

    protected void Move(float deltaTime)
    {
        // 壁に当たった、崖があった場合は折り返す
        if (m_WChackPoint != null)
        {
            if (m_WChackPoint.IsWallHit())
            {
                m_Direction.x *= -1;
                m_WChackPoint.ChangeDirection();
            }
        }
        // 移動
        m_Velocity = m_Speed * m_Direction * deltaTime;
    }

    // プレイヤーが見えているか
    protected bool InPlayer()
    {
        return false;
    }

    // 接地しているか
    protected bool isGround()
    {
        int layerMask = LayerMask.GetMask(new string[] { "Ground" });
        Collider2D hit =
            Physics2D.OverlapPoint(m_GroundPoint.position, layerMask);
        return hit != null;
    }

    // 衝突判定
    public void OnCollisionEnter2D(Collision2D collision)
    {
        var tag = collision.gameObject.tag;
        if (tag == "Player")
            Destroy(gameObject);
        else if(tag == "hasami_tekitou")
        {
            var name = collision.gameObject.name;
            if (name == "Big") // 大きいはさみに挟まった場合
            {
                if (m_Size < 5)    // サイズが小さい場合
                {
                    ChangeState(
                    State.Runaway,
                    AnimationNumber.ANIME_RUNAWAY_NUMBER
                    );
                    return;
                }
                else
                {
                    ChangeState(
                    State.TrapHit,
                    AnimationNumber.ANIME_TRAP_NUMBER
                    );
                    return;
                }
            }
            else if(name == "small") // 小さいはさみに挟まった場合
            {
            }
        }
    }

}
