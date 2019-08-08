using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    //player and camera
    CharacterController Controller;
    private GameObject AttachedCamera;

    public QuantumState QuantumStateHandler;
    public int PlayerID;
    public bool IsActive = true; //used to prevent moving multiple instances of the player at once
    public GameObject Holding; //the item the player is holding
    
    public float MovementSpeed = 5, LookSpeed = 180, JumpSpeed = 5, slidespeed = 6;
    private float Gravity = 9.8f, CurrentVerticalVelocity = 0;
    public Vector3 Velocity, SlopeAngle; //movement velocity and angle of slope
    private bool IsOnSlope; //player on slope

    //ensure interact is only used once when pressing button
    private bool InteractAxisInUse = false;

    // Start is called before the first frame update
    void Start()
    {
        AttachedCamera = transform.GetChild(0).gameObject;
        Controller = GetComponent<CharacterController>();
        Cursor.lockState = CursorLockMode.Locked;
    }

    // Update is called once per frame
    void Update()
    {
        if (Holding != null)
        {
            //position item relative to player
            Holding.GetComponent<Rigidbody>().MovePosition(transform.GetChild(1).gameObject.transform.position);
            Holding.transform.rotation = transform.rotation;
        }
        if (Controller.isGrounded && !IsOnSlope)
        {
            //set downwards velocity to 0
            CurrentVerticalVelocity = 0;
        }
        if (IsActive)
        {
            CameraMovement();
            Velocity = transform.forward * Input.GetAxis("Vertical") * MovementSpeed;
            Velocity += transform.right * Input.GetAxis("Horizontal") * MovementSpeed;
            if (Controller.isGrounded && !IsOnSlope && Input.GetAxis("Jump") != 0)
            {
                CurrentVerticalVelocity = JumpSpeed;
            }
            if (Input.GetAxis("Interact") != 0)
            {
                if (!InteractAxisInUse)
                {
                    InteractionBehaviour();
                    InteractAxisInUse = true;
                }
            }
            else if (Input.GetAxis("Interact") == 0)
            {
                InteractAxisInUse = false;
            }
        }  
        else 
        {
            Velocity = Vector3.zero;
        }
        CurrentVerticalVelocity -= Gravity * Time.deltaTime;
        Velocity.y = CurrentVerticalVelocity;
        IsOnSlope = (Vector3.Angle (Vector3.up, SlopeAngle) > Controller.slopeLimit);
        if (IsOnSlope) {
            Velocity.x += ((1f - SlopeAngle.y) * SlopeAngle.x) * slidespeed;
            Velocity.z += ((1f - SlopeAngle.y) * SlopeAngle.z) * slidespeed;
        }
        Controller.Move(Velocity * Time.deltaTime);
        
    }

    void LateUpdate()
    {
        if (Holding != null)
        {
            //position item relative to player
            Holding.transform.position = (transform.GetChild(1).gameObject.transform.position);
            Holding.GetComponent<Rigidbody>().MovePosition(transform.GetChild(1).gameObject.transform.position);
            Holding.transform.rotation = transform.rotation;
        }
    }

    private void CameraMovement()
    {
        //rotates player in Y, rotates camera in X
        transform.Rotate(0, Input.GetAxis("HorizontalL") * LookSpeed * Time.deltaTime, 0);
        AttachedCamera.transform.Rotate(-Input.GetAxis("VerticalL") * LookSpeed * Time.deltaTime, 0, 0);
        CameraLimit();
    }

    private void CameraLimit()
    {
        //locks the camera to 180 degrees of rotation in X
        float CameraAngle = AttachedCamera.transform.eulerAngles.x;
        if (CameraAngle > 90 && CameraAngle <= 180)
        {
            CameraAngle = 90;
        }
        else if (CameraAngle > 180 && CameraAngle < 270)
        {
            CameraAngle = 270;
        }
        AttachedCamera.transform.localEulerAngles = new Vector3(CameraAngle, 0, 0);
    }

    public void Collapse()
    {
        QuantumStateHandler.Collapse(PlayerID);
    }

    public void InteractionBehaviour()
    {
        if (Holding != null)
        {
            Holding.GetComponent<ItemBehaviour>().ChangeOwner(null);
        }
        else
        {
            RaycastHit hit;
            Debug.DrawRay(AttachedCamera.transform.position, AttachedCamera.transform.forward, Color.green);
            if (Physics.Raycast(AttachedCamera.transform.position, AttachedCamera.transform.forward, out hit, maxDistance: 3))
            {
                if (hit.collider.gameObject.tag == "Item")
                {
                    Holding = hit.transform.gameObject;
                    Holding.GetComponent<ItemBehaviour>().ChangeOwner(gameObject);
                }
                else if (hit.collider.gameObject.tag == "Button")
                {
                    hit.transform.GetComponent<Button>().ButtonPress();
                }
            }
        }
    }

    public void ForceGrab(GameObject Item)
    {
        Holding = Item;
        Holding.GetComponent<ItemBehaviour>().ChangeOwner(gameObject);
    }

    public void ForceDrop()
    {
        Holding.GetComponent<ItemBehaviour>().ChangeOwner(null);
    }
    
    void OnControllerColliderHit (ControllerColliderHit hit)
    {
        SlopeAngle = hit.normal;
    }
}