using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerControl : MonoBehaviour
{

    // Speed of player
    public float moveSpeed = 5f;
    public Transform movePoint;

    public LayerMask CollisionLayer;

    // Start is called before the first frame update
    private void Start()
    {
        //dest = transform.position;
        movePoint.parent = null;
    }

    private void Update()
    {
        

        transform.position = Vector2.MoveTowards(transform.position, movePoint.position, moveSpeed * Time.deltaTime);

        GetComponent<Animator>().SetBool("Idle", true);
        GetComponent<Animator>().SetFloat("DirX", 0f);
        GetComponent<Animator>().SetFloat("DirY", 0f);

        if (Vector2.Distance(transform.position, movePoint.position) <= 0.05f)
        {
            if (Mathf.Abs(Input.GetAxisRaw("Horizontal")) == 1f)
            {
                if (!Physics2D.OverlapCircle(movePoint.position + new Vector3(Input.GetAxisRaw("Horizontal"), 0f, 0f), 0.2f, CollisionLayer))
                {
                    // Update player direction and load relevant animation
                    GetComponent<Animator>().SetFloat("DirX", Input.GetAxis("Horizontal"));
                    GetComponent<Animator>().SetFloat("DirY", 0f);
                    GetComponent<Animator>().SetBool("Idle", false);
                    movePoint.position += new Vector3(Input.GetAxisRaw("Horizontal"), 0f, 0f);
                }
            }
            else if (Mathf.Abs(Input.GetAxisRaw("Vertical")) == 1f)
            {
                if (!Physics2D.OverlapCircle(movePoint.position + new Vector3(0f, Input.GetAxisRaw("Vertical"), 0f), 0.2f, CollisionLayer))
                {
                    // Update player direction and load relevant animation
                    GetComponent<Animator>().SetFloat("DirX", 0f);
                    GetComponent<Animator>().SetFloat("DirY", Input.GetAxis("Vertical"));
                    GetComponent<Animator>().SetBool("Idle", false);
                    movePoint.position += new Vector3(0f, Input.GetAxisRaw("Vertical"), 0f);
                }
                
            }
        }
    }
}
