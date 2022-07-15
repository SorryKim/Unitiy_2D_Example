using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMove : MonoBehaviour
{
    public float maxSpeed;
    public float jumpPower;
    Rigidbody2D rigid;
    SpriteRenderer spriteRenderer;
    Animator anim;

    private void Awake()
    {
        rigid = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        anim = GetComponent<Animator>();
    }

    // 단발적인 업데이트
    private void Update()
    {
        // Jump
        if (Input.GetButtonDown("Jump") && !anim.GetBool("isJumping"))
        {
            rigid.AddForce(Vector2.up * jumpPower, ForceMode2D.Impulse);
            anim.SetBool("isJumping", true);
        }

        // Stop Speed
        if (Input.GetButtonUp("Horizontal"))
        {
            // 단위벡터로 만드는 normalized
            rigid.velocity = new Vector2(rigid.velocity.normalized.x * 0.5f, rigid.velocity.y); // y축의 속도는 유지
        }

        // Direction Sprite
        if (Input.GetButton("Horizontal"))
        {
            spriteRenderer.flipX = Input.GetAxisRaw("Horizontal") == -1;
        }

        // Animation
        if (Mathf.Abs(rigid.velocity.x) <= 0.4)
            anim.SetBool("isWalking", false);
        else
            anim.SetBool("isWalking", true);
    }

    // 지속적인 업데이트 ex) 키 이동
    private void FixedUpdate()
    {
        // Move by Control
        float h = Input.GetAxisRaw("Horizontal");
        rigid.AddForce(Vector2.right * h, ForceMode2D.Impulse);

        // right max speed
        if (rigid.velocity.x > maxSpeed)
            rigid.velocity = new Vector2(maxSpeed, rigid.velocity.y); // y축의 속도는 유지
        // left max speed
        else if (rigid.velocity.x < maxSpeed*(-1))
            rigid.velocity = new Vector2(maxSpeed*(-1), rigid.velocity.y);

        // Landing Platform
        if (rigid.velocity.y < 0)
        {
            // Debug.DrawRay(rigid.position, Vector3.down, new Color(0, 1, 0));
            RaycastHit2D rayHit = Physics2D.Raycast(rigid.position, Vector3.down, 1, LayerMask.GetMask("Platform"));
            if (rayHit.collider != null)
            {
                if (rayHit.distance < 0.55f)
                {
                    anim.SetBool("isJumping", false);
                }

            }
        }


    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if(collision.gameObject.tag == "Enemy")
        {
            // 몬스터보다 위에있음 + 아래로 낙하 중이면 밟아서 enemy 제거
            if(rigid.velocity.y < 0 && transform.position.y > collision.transform.position.y)
            {

            }
            else
                OnDamaged(collision.transform.position);
        }
    }

    // 플레이어 피격 시
    void OnDamaged(Vector2 targetPos)
    {
        // 피격시 무적상태로 변경
        gameObject.layer = 9;

        // 투명하게 바꿈
        spriteRenderer.color = new Color(1, 1, 1, 0.4f);

        // Reaction Force
        int dirc = transform.position.x - targetPos.x > 0 ? 1 : -1;
        rigid.AddForce(new Vector2(dirc, 1) * 5, ForceMode2D.Impulse);

        // Animation
        anim.SetTrigger("doDamaged");

        // 무적 해제
        Invoke("OffDamaged", 2);
    }

    void OffDamaged()
    {
        gameObject.layer = 8;
        spriteRenderer.color = new Color(1, 1, 1, 1);
    }
}
