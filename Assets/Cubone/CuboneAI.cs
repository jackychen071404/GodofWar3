using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CuboneAI : MonoBehaviour
{
    // Start is called before the first frame update
    public GameObject pointA;
    public GameObject pointB;
    private Rigidbody2D rb;
    private Animator anim;
    private Transform currentPoint;
    public float speed;

    [Header("Idle Behavior")]
    [SerializeField] private float idleDuration;
    [SerializeField] private GameObject self;
    private float idleTimer;

    [SerializeField] private float attackCD;
    [SerializeField] private float range;
    [SerializeField] private float colliderDistance;
    [SerializeField] private int dmg;
    [SerializeField] private BoxCollider2D boxCollider;
    [SerializeField] private LayerMask playerLayer;
    private float CDtimer = Mathf.Infinity;
    private bool canMove = true; 
    private float framesStopped = 0f;
    public float stopDuration = 1;

    public GameObject attackPoint;
    public float radius;
    public LayerMask player;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        currentPoint = pointB.transform;
        anim.SetBool("running",true);
    }

    // Update is called once per frame
    void Update()
    {
        //attack logic
        CDtimer += Time.deltaTime;

        if (PlayerInSight()) {
            if (CDtimer >= attackCD) {
                CDtimer = 0;
                anim.SetTrigger("attacking");
            }
        }
        if (canMove) // Check if movement is allowed
        {
            MoveLogic();
        }

        if (!canMove)
        {
            // Increment the stop timer
            framesStopped += Time.deltaTime;
            Debug.Log("Frames stopped: " + framesStopped);

            // Check if the stop duration has passed
            if (framesStopped >= stopDuration)
            {
                // Reset movement flag and timer
                canMove = true;
                framesStopped = 0f;
            }
        }
        
        if (this.GetComponent<cuboneHealth>().health == 0) {
            rb.velocity = new Vector2(0,0);
            Destroy(this);
            Destroy(self,2);
        }

    }

    private void MoveLogic() {
        //patrol logic
        Vector2 point = currentPoint.position - transform.position;
        if (currentPoint == pointB.transform) {
            rb.velocity = new Vector2(speed,0);
        }
        else {
            rb.velocity = new Vector2(-speed,0);
        }

        if (Vector2.Distance(transform.position,currentPoint.position) < 0.5f && currentPoint == pointB.transform) {
            flip();
            currentPoint = pointA.transform;
        }
        if (Vector2.Distance(transform.position,currentPoint.position) < 0.5f && currentPoint == pointA.transform) {
            flip();
            currentPoint = pointB.transform;
        }
    }
    private void flip() {
        Vector3 localScale = transform.localScale;
        localScale.x *= -1;
        transform.localScale = localScale;
    }

    private bool PlayerInSight() {
        RaycastHit2D hit = Physics2D.BoxCast(boxCollider.bounds.center + transform.right * range * -transform.localScale.x * colliderDistance,new Vector3(boxCollider.bounds.size.x * range, boxCollider.bounds.size.y,boxCollider.bounds.size.z) ,0,Vector2.left,0,playerLayer);
        return hit.collider != null;
    }

    private void DetectAttack() {
        Collider2D[] players = Physics2D.OverlapCircleAll(attackPoint.transform.position,radius,player);
        foreach (Collider2D playerGameobject in players) {
            Debug.Log("Hit player");
            playerGameobject.GetComponent<playerHealth>().TakeDamage(dmg);
        }
    }

    public void StopMoving()
    {
        canMove = false; // Stop movement
        rb.velocity = Vector2.zero; // Set velocity to zero
        framesStopped = 0f;
    }

    private void OnDrawGizmos() {
        Gizmos.DrawWireSphere(pointA.transform.position,0.5f);
        Gizmos.DrawWireSphere(pointB.transform.position,0.5f);
        Gizmos.DrawLine(pointA.transform.position, pointB.transform.position);
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(boxCollider.bounds.center + transform.right * range * -transform.localScale.x * colliderDistance,new Vector3(boxCollider.bounds.size.x * range, boxCollider.bounds.size.y,boxCollider.bounds.size.z));
        Gizmos.DrawWireSphere(attackPoint.transform.position, radius);
    }
}
