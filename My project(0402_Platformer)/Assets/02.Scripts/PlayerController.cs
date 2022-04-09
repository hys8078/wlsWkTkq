using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//���ѻ��¸ӽ� 
//���°� ����������


public class PlayerController : MonoBehaviour
{
    private Rigidbody2D rb;
    private Animator animator;
    private GroundDetector groundDetector;
    public float jumpForce;
    Vector2 move;
    public float moveSpeed; //���ǵ带 ���� �����ϱ�����public
    private float moveInputOffset = 0.1f;


    int _direction; //+1 :right , -1:left �� �ǰ� �Ұ���

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
        float h = Input.GetAxis("Horizontal"); //�����Է¹���

        //������ȯ
        if (h < 0) direction = -1;
        else if (h > 0) direction = 1;

        if (Mathf.Abs(h) > moveInputOffset)
        {
            move.x = h;
            if (state == PlayerState.idle)
                ChangePlayerState(PlayerState.Run);
        } //Mathf.Abs �� �����ǹ�

        else
        {
            move.x = 0;
            if (state == PlayerState.Run)
                ChangePlayerState(PlayerState.idle);
        }
        //����Ű
        if (Input.GetKeyDown(KeyCode.LeftAlt))
        {
            if (groundDetector.isDestected &&
                state != PlayerState.Jump &&
                state != PlayerState.Fall)
            {
                ChangePlayerState(PlayerState.Jump);
            }
        }

        UpdatePlayerState();                                        //�̰� ���ذ��ȵǸ� �ִϸ����͸�����//�װ� ��ũ��Ʈ�� �����Ѱ�.
                                                                                                                         //if���� �ٳ����� workFlow����
    }
    private void FixedUpdate()
    {

        rb.position += new Vector2(move.x * moveSpeed, move.y) * Time.fixedDeltaTime; //�̵�
    }

    public void ChangePlayerState(PlayerState newState)
    {
        if (state == newState) return;
        //���� ���� ���� �ӽ� �ʱ�ȭ
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

        //���� ����  �ٲ�
        state= newState;

        //���� ���� ���� �ӽ� �غ�
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
            case JumpState.Casting:  //���� ������ ���������� Ȯ��
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


//enum : ������
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