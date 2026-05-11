using UnityEngine;

// PlayerController는 플레이어 캐릭터로서 Player 게임 오브젝트를 제어한다.
public class PlayerController : MonoBehaviour {
   public AudioClip deathClip; // 사망시 재생할 오디오 클립
   public float jumpForce = 700f; // 점프 힘

   private int jumpCount = 0; // 누적 점프 횟수, 바닥에 닿을 때마다 초기화
   private bool isGrounded = false; // 바닥에 닿았는지 나타냄
   private bool isDead = false; // 사망 상태

   private Rigidbody2D playerRigidbody; // 사용할 리지드바디 컴포넌트
   private Animator animator; // 사용할 애니메이터 컴포넌트
   private AudioSource playerAudio; // 사용할 오디오 소스 컴포넌트

   private void Start() {
       //게임 오브젝트로부터 사용할 컴포넌트들을 가져와 할당
       playerRigidbody = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        playerAudio = GetComponent<AudioSource>();
   }

   private void Update() {
       // 사용자 입력을 감지하고 점프하는 처리
       if(isDead)
        {
            //사망 시 종료
            return;
        }

       //마우스 왼쪽 버튼을 눌렀으며 최대 점프 횟수(2)에 도달하지 않았다면
       if(Input.GetMouseButtonDown(0)&&jumpCount<2)
        {
            //점프 횟수 추가
            jumpCount++;
            //점프 직전에 속도를 순간적으로 0으로 변경
            playerRigidbody.linearVelocity = Vector2.zero;
            //리지드바디에 위쪽으로 힘을 추가
            playerRigidbody.AddForce(new Vector2(0, jumpForce));
            //오디오 소스 재생
            playerAudio.Play();
        }
       else if(Input.GetMouseButtonUp(0)&&playerRigidbody.linearVelocity.y > 0)
        {
            //마우스 왼쪽 버튼에서 손을 떼는 순간 && 속도의 y값이 0보다 크다면(위로 상승중이라면)
            //현재 속도를 절반으로 변경
            playerRigidbody.linearVelocity = playerRigidbody.linearVelocity * 0.5f;
        }
        //애니메이터의 Grounded 파라미터를 isGrounded 값으로 갱신
        animator.SetBool("Grounded", isGrounded);
   }

   private void Die() {
        // 사망 처리
        //애니메이터 트리거 파라미터 셋
        animator.SetTrigger("Die");

        //할당된 오디오 클립을 변경
        playerAudio.clip = deathClip;
        playerAudio.Play();

        //속도를 0로 변경
        playerRigidbody.linearVelocity = Vector2.zero;
        //사망 상태 = true
        isDead = true;
   }

   private void OnTriggerEnter2D(Collider2D other) {
       // 트리거 콜라이더를 가진 장애물과의 충돌을 감지
       if(other.tag == "Dead" && !isDead)
        {
            Die();
        }
   }

   private void OnCollisionEnter2D(Collision2D collision) {
        // 바닥에 닿았음을 감지하는 처리
        //collision.contacts[0] = 두 물체 사이의 여러 충돌 지점 중에서 첫번째 충돌지점의 정보
        //normal = 충돌 지점에서 충돌 표면의 방향(노멀벡터)
        //노멀벡터의 값이 1.0인 경우 해당 표면의 방향은 위쪽이고, -1일 경우 아래쪽이므로 
        //0.7인 경우 대략 45도 정도의 경사면임. 즉, 아래 조건문은 충돌 지점 표면 방향을 검사해서
        //절벽이나 천장을 바닥으로 이식하지 못하게끔 막는 장치임
        if (collision.contacts[0].normal.y > 0.7f)
        {
            //isGrounded를 true로 변경하고, 누적 점프횟수 초기화
            isGrounded = true;
            jumpCount = 0;
        }
   }

   private void OnCollisionExit2D(Collision2D collision) {
       // 바닥에서 벗어났음을 감지하는 처리
   }
}