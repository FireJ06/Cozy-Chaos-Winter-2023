using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    private float MoveX;
    private float MoveY;
    [Header("Movement")]
    [SerializeField] private float jumpForce = 5f;
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float accelSlowdown = 3.5f;
    public List<GameObject> PickUpList; // Used by burn script for ball count
    [Header("References")]
    [SerializeReference] private GameObject PickUp;
    [SerializeReference] private GameObject BluePickUp;
    [SerializeReference] private GameObject PickedUp;
    [SerializeReference] private GameObject BluePickedUp;

    [SerializeReference] private GameObject CameraLookAt;
    [SerializeReference] private ParticleSystem GroundParticles;
    [SerializeReference] private GameObject scarf;
    private bool isGrounded;
    private Rigidbody2D rb;
    private SpriteRenderer sr;
    private CapsuleCollider2D cc;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        sr = GetComponent<SpriteRenderer>();
        cc = GetComponent<CapsuleCollider2D>();
    }

    private void Update()
    {
        // Gets momentum and moves it
        MoveX = Input.GetAxis("Horizontal") * moveSpeed;
        MoveY = Input.GetAxis("Vertical");

        if (MoveX > 0) sr.flipX = false;
        else if (MoveX < 0) sr.flipX = true;

        if (Input.GetKeyDown(KeyCode.W) && (isGrounded || PickUpList.Count >= 1)) //check if you can jump
        {
            rb.AddForce(new Vector2(0f, jumpForce), ForceMode2D.Impulse);
            if (!isGrounded) RemoveBall(false);
        }

        if (Input.GetKey(KeyCode.S) && isGrounded) rb.AddForce(new Vector2(0f, MoveY), ForceMode2D.Impulse); //go down
    }

    private void FixedUpdate()
    {
        RaycastHit2D hit = Physics2D.Raycast(new Vector2(transform.position.x, transform.position.y-(1*PickUpList.Count)), Vector2.down, 1.00025f-0.40f); //raycast to check if grounded
        if (hit.collider != null)
        {
            if (hit.collider.CompareTag("Ground"))
            {
                if(!isGrounded) Instantiate(GroundParticles, new Vector2(transform.position.x, transform.position.y - PickUpList.Count), Quaternion.identity);
                isGrounded = true;
            }
            else
            {
                isGrounded = false;
            }
        }
        else
        {
            isGrounded = false;
        }

        if(-moveSpeed < rb.velocity.x && rb.velocity.x < moveSpeed)
        {
            rb.AddForce(new Vector2(MoveX/accelSlowdown, 0));
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("PickUp")) 
        {
            if (collision.gameObject.GetComponent<PickUp>().cooldownDone)
            {
                Destroy(collision.gameObject); //remove loose ball
                PickUpList.Add(Instantiate(PickedUp, new Vector2(gameObject.transform.position.x, gameObject.transform.position.y - gameObject.transform.localScale.y * (PickUpList.Count + 1)), Quaternion.identity, gameObject.transform)); //instantiate snowball beneath player
                scarf.SetActive(true);

                // Fix collider and move player up                                                                                                                                                                                                                         // PickUpList[PickUpList.Count-1].tag = "PickedUp"; //add tag (just to be sure)
                gameObject.transform.position = new Vector2(gameObject.transform.position.x, gameObject.transform.position.y + gameObject.transform.localScale.y); //moves player up
                cc.offset = new Vector2(cc.offset.x, cc.offset.y - .5f); //changes offset so we don't bug into the ground
                cc.size = new Vector2(cc.size.x, cc.size.y + 1); //changes size so we don't bug into the ground

                CameraLookAt.transform.position = new Vector2(CameraLookAt.transform.position.x, CameraLookAt.transform.position.y - gameObject.transform.localScale.y / 2); //moves camera look at
            }
        }

        else if (collision.gameObject.CompareTag("BluePickUp")) {

            if (collision.gameObject.GetComponent<PickUp>().cooldownDone)
            {
                Destroy(collision.gameObject); //remove loose ball
                PickUpList.Add(Instantiate(BluePickedUp, new Vector2(gameObject.transform.position.x, gameObject.transform.position.y - gameObject.transform.localScale.y * (PickUpList.Count + 1)), Quaternion.identity, gameObject.transform)); //instantiate blue snowball beneath player
                scarf.SetActive(true);

                // Fix collider and move player up                                                                                                                                                                                                                         // PickUpList[PickUpList.Count-1].tag = "PickedUp"; //add tag (just to be sure)
                gameObject.transform.position = new Vector2(gameObject.transform.position.x, gameObject.transform.position.y + gameObject.transform.localScale.y); //moves player up
                cc.offset = new Vector2(cc.offset.x, cc.offset.y - .5f); //changes offset so we don't bug into the ground
                cc.size = new Vector2(cc.size.x, cc.size.y + 1); //changes size so we don't bug into the ground

                CameraLookAt.transform.position = new Vector2(CameraLookAt.transform.position.x, CameraLookAt.transform.position.y - gameObject.transform.localScale.y / 2); //moves camera look at
            }
       
        }

        
    }

    public bool RemoveBall(bool cooldown)
    {
        if (PickUpList.Count != 0)
        {
            Destroy(PickUpList[PickUpList.Count - 1]); //destroys gameobject
            PickUpList.RemoveAt(PickUpList.Count - 1); //also remove it from list (for consistency sake

            cc.offset = new Vector2(cc.offset.x, cc.offset.y + .5f); //undo offset so we don't float
            cc.size = new Vector2(cc.size.x, cc.size.y - 1); //undo size so we don't float

            GameObject NewPickUp = Instantiate(PickUp, new Vector2(gameObject.transform.position.x, gameObject.transform.position.y - gameObject.transform.localScale.y * (PickUpList.Count + 1) - 0.1f), Quaternion.identity);
            NewPickUp.GetComponent<PickUp>().Spawned(cooldown, rb.velocity.x); //spawn new ball

            CameraLookAt.transform.position = new Vector2(CameraLookAt.transform.position.x, CameraLookAt.transform.position.y + gameObject.transform.localScale.y / 2);//moves camera look at
            if (PickUpList.Count == 0) scarf.SetActive(false); //if no balls left, hide scarf
            return true;
        }
        return false;
    }

    public bool RemoveBall(bool cooldown)
    {
        if(PickUpList.Count != 0)
        {
            Destroy(PickUpList[PickUpList.Count - 1]); //destroys gameobject
            PickUpList.RemoveAt(PickUpList.Count - 1); //also remove it from list (for consistency sake)
            
            cc.offset = new Vector2(cc.offset.x, cc.offset.y + .5f); //undo offset so we don't float
            cc.size = new Vector2(cc.size.x, cc.size.y - 1); //undo size so we don't float

            if (PickUpList[PickUpList.Count - 1].CompareTag("BluePickedUp"))
            {
                Instantiate(BluePickUp, new Vector2(gameObject.transform.position.x + Mathf.Clamp(MoveX, -1, 1), gameObject.transform.position.y - gameObject.transform.localScale.y * (PickUpList.Count + 1) - 0.1f), Quaternion.identity).GetComponent<PickUp>().Spawned(cooldown); //spawn loose ball back and give it random physics
                
            }

            else if (PickUpList[PickUpList.Count - 1].CompareTag("PickedUp"))
            {
                Instantiate(PickUp, new Vector2(gameObject.transform.position.x + Mathf.Clamp(MoveX, -1, 1), gameObject.transform.position.y - gameObject.transform.localScale.y * (PickUpList.Count + 1) - 0.1f), Quaternion.identity).GetComponent<PickUp>().Spawned(cooldown); //spawn loose ball back and give it random physics
                
            }

            if (PickUpList.Count == 0) scarf.SetActive(false); //if no balls left, hide scarf
            CameraLookAt.transform.position = new Vector2(CameraLookAt.transform.position.x, CameraLookAt.transform.position.y + gameObject.transform.localScale.y / 2);//moves camera look at

            return true;
        }
        return false;
    }
}