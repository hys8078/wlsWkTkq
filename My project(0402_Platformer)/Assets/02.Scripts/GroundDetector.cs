using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//땅만 따로감지하기위함
public class GroundDetector : MonoBehaviour
{
    public bool isDestected;
    public LayerMask groundLayer;

    private Rigidbody2D rb;
    private CapsuleCollider2D col;
    private Vector2 size; //감지할 범위
    private Vector2 center;


    private void Awake()
    {
        rb=GetComponent<Rigidbody2D>();
        col=GetComponent<CapsuleCollider2D>();
        size.x = col.size.x/2;  //기존 콜라이더의 절반만큼 감지할거임
        size.y = 0.005f; //y는 이정도로

    }
    private void Update()
    {
        center.x=rb.position.x+col.offset.x;
        center.y = rb.position.y + col.offset.y-col.size.y/2-size.y;
        isDestected = Physics2D.OverlapBox(center, size, 0, groundLayer);

    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireCube(new Vector3(center.x, center.y,0),
                            new Vector3(size.x, size.y,0));

    }




}
