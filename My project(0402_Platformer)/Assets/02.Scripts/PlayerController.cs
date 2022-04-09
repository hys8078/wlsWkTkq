using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//유한상태머신 
//상태가 정해져있음


public class PlayerController : MonoBehaviour
{
    private Rigidbody2D rb;
    private Animator animator;
    private GroundDetector groundDetector;
    public float jumpForce;
    Vector2 move;
    public float moveSpeed; //스피드를 쉽게 조절하기위해public
    private float moveInputOffset = 0.1f;


    int _direction; //+1 :right , -1:left 가 되게 할거임

    public int direction
    {
        set
        {
            if (value < 0)
            {
                _direction = -1;
                transform.eulerAngles = new Vector3(0, 180f, 0);
            }
            else if (value > 0)
            {
                _direction = 1;
                transform.eulerAngles = Vector3.zero;
            }
        }
        get { return _direction; }
    }

    public PlayerState state;
    public JumpState jumpState;
    public FallState fallState;

    private float jumpTime = 0.1f;
    private float jumpTimer;

    private void Awake()
    {

        rb = GetComponent<Rigidbody2D>();
        animator = GetComponentInChildren<Animator>();
        groundDetector = GetComponent<GroundDetector>();
    }

    // Update is called once per frame
    private void Update()
    {
        float h = Input.GetAxis("Horizontal"); //수평입력받음

        //방향전환
        if (h < 0) direction = -1;
        else if (h > 0) direction = 1;

        if (Mathf.Abs(h) > moveInputOffset)
        {
            move.x = h;
            if (state == PlayerState.idle)
                ChangePlayerState(PlayerState.Run);
        } //Mathf.Abs 는 절댓값의미

        else
        {
            move.x = 0;
            if (state == PlayerState.Run)
                ChangePlayerState(PlayerState.idle);
        }
        //점프키
        if (Input.GetKeyDown(KeyCode.LeftAlt))
        {
            if (groundDetector.isDestected &&
                state != PlayerState.Jump &&
                state != PlayerState.Fall)
            {
                ChangePlayerState(PlayerState.Jump);
            }
        }

        UpdatePlayerState();                                        //이게 이해가안되면 애니메이터를보셈//그걸 스크립트로 구현한것.
                                                                                                                         //if문이 다끝나면 workFlow실행
    }
    private void FixedUpdate()
    {

        rb.position += new Vector2(move.x * moveSpeed, move.y) * Time.fixedDeltaTime; //이동
    }

    public void ChangePlayerState(PlayerState newState)
    {
        if (state == newState) return;
        //이전 상태 하위 머신 초기화
        switch (state)
        {
            case PlayerState.idle:
                break;
            case PlayerState.Run:
                break;
            case PlayerState.Jump:
                jumpState = JumpState.idle;
                break;
            case PlayerState.Fall:
                fallState = FallState.idle;
                break;
            default:
                break;
            
        }

        //현재 상태  바꿈
        state= newState;

        //현재 상태 하위 머신 준비
        switch (state)
        {
            case PlayerState.idle:
                break;
            case PlayerState.Run:
                break;
            case PlayerState.Jump:
                jumpState = JumpState.Prepare;
                break;
            case PlayerState.Fall:
                fallState = FallState.Prepare;
                break;
            default:
                break;

        }

    }


    private void UpdatePlayerState()
    {
        switch (state)
        {
            case PlayerState.idle:
                animator.Play("idle");  //UpdateidleState();
                break;
            case PlayerState.Run:
                animator.Play("Run");
                break;
            case PlayerState.Jump:
                UpdateJumpState();
                break;
            case PlayerState.Fall:
                UpdateFallState();
                break;
            default:
                break;
        }

    }
    private void UpdateIdleState()
    {
        

    }

    private void UpdateJumpState()
    {
        switch (jumpState)
        {
            case JumpState.idle:
                break;
            case JumpState.Prepare:
                animator.Play("Jump");
                rb.velocity = new Vector2(rb.velocity.x, 0);
                rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
                jumpTimer = jumpTime;
                fallState++;
                break;
            case JumpState.Casting:  //발이 땅에서 떨어졌는지 확인
                if(!groundDetector.isDestected)
                    jumpState++;
                else if(jumpTimer<0)
                    ChangePlayerState(PlayerState.idle);
                break;
            case JumpState.OnAction:
                if (rb.velocity.y < 0)
                {
                    jumpState++;
                }
                break;
            case JumpState.Finish:
                ChangePlayerState(PlayerState.idle);
                break;
            default:
                break;

        }
    }


    private void UpdateFallState()
    {
        switch (fallState)
        {
            case FallState.idle:
                break;
            case FallState.Prepare:
                animator.Play("Fall");
                fallState++;
                break;
            case FallState.Casting:
                fallState++;
                break;
            case FallState.OnAction:
                if (groundDetector.isDestected)
                    fallState++;
                break;
            case FallState.Finish:
                ChangePlayerState(PlayerState.idle);
                break;
            default:
                break;
        }

    }

    

    

}


//enum : 열거형
public enum PlayerState
{
    idle,
    Run,
    Jump,
    Fall

}

public enum JumpState
{
    idle,
    Prepare,
    Casting,
    OnAction,
    Finish,
}

public enum FallState
{
    idle,
    Prepare,
    Casting,
    OnAction,
    Finish,

}
