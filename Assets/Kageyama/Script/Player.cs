using UnityEngine;
using System.Collections;

public class Player : MonoBehaviour
{
    //プレイヤーの状態
    public enum State
    {
        Wait,
        Touch,
        Endure
    }
    public State _state;

    [SerializeField, TooltipAttribute("メインカメラ")]
    private GameObject _mainCamera;

    #region 鋏むときに必要な変数
    //鋏むんでいるオブジェクトを入れる
    public GameObject _target;
    //鋏む対象との距離
    private Vector3 offset = Vector3.zero;
    //鋏んだときの反動中かどうか
    public bool _recoil;
    //暴れるのを耐えるゲージ
    public float _endureGage;
    //離すときに数える数
    private int _separate;
    //ゲージが増える速さ
    public float _gageSpeed;
    //落とされるまでの時間を計測する値
    private float _fallTimeCheck;
    //落とされるまでの時間
    private float _fallTime;

    #endregion

    #region 移動や重力に必要な変数
    //移動する方向ベクトル
    private Vector3 _moveDirection = Vector3.zero;
    //プレイヤーの移動速度
    public float _speed;
    //落ちていく速度を加速させる
    private float _gadd;
    //地面に衝突しているかどうか
    private bool _groundOn;
    #endregion

    // Use this for initialization
    void Start ()
    {
        _recoil = false;
        _state = State.Wait;
        _groundOn = false;
        _endureGage = 0;
        _fallTime = 5;
        _fallTimeCheck = 0;
        _gageSpeed = 1.0f;
        _separate = 0;
    }
	
	// Update is called once per frame
	void Update ()
    {
        if (this.name == "Player(1)")
        {
            var cameraRight = Vector3.Scale(_mainCamera.transform.right, new Vector3(1, 1, 1)).normalized;
            _moveDirection = _mainCamera.transform.up * Input.GetAxis("Vertical") + cameraRight * Input.GetAxis("Horizontal");
            this.transform.position += _moveDirection * _speed;
        }
        else
        {
            Move(3.0f);
            Action();
            if (_state == State.Endure)
            {
                EndureAction();
            }
        }

        
	}

    void Move(float gravity)
    {
        Vector2 newPosition = transform.position;
        //何かを挟んでいたら、位置を固定する
        if (_state != State.Wait)
        {
            _gadd = 0;
            newPosition.x = _target.transform.position.x + offset.x;
            newPosition.y = _target.transform.position.y + offset.y;
        }
        //何も挟んでいないなら、重力で落ちる
        if (_state == State.Wait)
        {
            if(_groundOn == false)_gadd += 0.05f;
            newPosition.y -= gravity * Time.deltaTime * _gadd;
        }
        //ピッタリと追いかける
        transform.position = newPosition;

    }

    void Action()
    {
        //何も鋏んでいないときに挟もうとすると音を鳴らす
        if (_state == State.Wait)
        {
            if (Input.GetKeyDown(KeyCode.Z))
            {
                SoundManger.Instance.PlaySE(0);
            }
            if (Input.GetKeyDown(KeyCode.X))
            {
                SoundManger.Instance.PlaySE(1);
            }
        }

        //何かを挟んでいたら
        if(_state == State.Touch)
        {
            if(Input.GetKey(KeyCode.Z))
            {
                _separate++;
            }
            if (Input.GetKeyUp(KeyCode.Z)) _separate = 0;
            if (_separate > 60)
            {
                _target = null;
                _endureGage = 0;
                _separate = 0;
                _state = State.Wait;
            }
        }
        //ターゲットを外す
        //if ((Input.GetKeyDown(KeyCode.Z) || Input.GetKeyDown(KeyCode.X)) && _recoil == false)
        //{
        //    _target = null;
        //    _state = State.Wait;
        //}
    }
    void EndureAction()
    {
        _fallTimeCheck += Time.deltaTime;
        if(_fallTimeCheck > _fallTime)
        {
            _target = null;
            _state = State.Wait;
            _fallTimeCheck = 0;
            _endureGage = 0;
            return;
        }
        if (Input.GetKeyDown(KeyCode.Z))
        {
            _endureGage += 10 * _gageSpeed;
        }
        //時間内に100までGageを溜めれば確実に鋏むことができる
        if(_endureGage >= 100)
        {
            _state = State.Touch;
            _fallTimeCheck = 0;
        }
    }

    void OnTriggerStay2D(Collider2D col)
    {
        //鋏む相手を探しているとき以外は衝突しても無視するを無視する
        if (_state != State.Wait) return;
        //何かとぶつかっているときにZボタンを押すと大きいハサミで鋏む
        if (Input.GetKeyDown(KeyCode.Z) && _target == null &&
            col.tag != "Ground" && col.tag != "Gimmick")
        {
            _target = col.gameObject;
            Scissors();
        }

        //何かとぶつかっているときにXボタンを押すと小さいハサミで鋏む
        if (Input.GetKeyDown(KeyCode.X) && _target == null &&
            col.tag != "Ground")
        {
            _target = col.gameObject;
            Scissors();
        }

        //地面とぶつかっていたら重力による落下を止める
        if (col.tag == "Ground")
        {
            _gadd = 0;
            _groundOn = true;
        }
    }

    void Scissors()
    {
        _recoil = true;
        offset = transform.position - _target.transform.position;
        _state = State.Endure;
        StartCoroutine(Recoil());
    }


    void OnTriggerExit2D(Collider2D col)
    {
        //挟まらずに離れたらターゲットを外す
        if (_state == State.Wait)
        {
            _target = null;
        }
        //地面と離れたら重力が加速するようになる
        if (col.tag == "Ground")
        {
            _groundOn = false;
        }
    }

    //鋏んだら少しの間反動で抜けないようにする
    IEnumerator Recoil()
    {
        yield return new WaitForSeconds(0.2f);
        _recoil = false;
    }
}
