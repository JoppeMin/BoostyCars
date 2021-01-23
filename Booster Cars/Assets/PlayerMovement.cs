using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Cinemachine;


public class PlayerMovement : MonoBehaviour
{
    Rigidbody rb;
    [SerializeField]
    private float boostForce;
    int maximumBoostForce = 1;
    [SerializeField]
    Slider boostForceIndicator;
    float inputDirection;
    private Vector3 currentAngle;
    CinemachineVirtualCamera camBehaviour;
    ParticleSystem boostPS;
    ParticleSystem directionalPS;

    [SerializeField]
    float cameraZoomAmount;
    [SerializeField]
    float cameraTargetFov;

    Material outlineMat;

    private int timesJumped = 0;

    void OnValidate()
    {
        rb = this.gameObject.GetComponent<Rigidbody>();
        camBehaviour = GameObject.FindObjectOfType<CinemachineVirtualCamera>();
        boostPS = this.gameObject.GetComponentInChildren<ParticleSystem>();
        directionalPS = GameObject.Find("DirectionalPS").GetComponent<ParticleSystem>();
        outlineMat = this.gameObject.GetComponentInChildren<MeshRenderer>().sharedMaterial;
    }

    private void Start()
    {
        currentAngle = this.transform.position;
        boostPS.Stop();
        directionalPS.Stop();
    }

    void Update()
    {
        BoostBehaviour();
        RolloverBehaviour();
    }

    private void BoostBehaviour()
    {
        if (timesJumped > 1)
            return;

        if (Input.GetButtonDown("Horizontal"))
        {
            boostPS.Play();
            directionalPS.Play();
            Quaternion rotationTarget = Quaternion.Euler(0, Input.GetAxisRaw("Horizontal") * 90, 0);
            directionalPS.transform.localRotation = rotationTarget;
        }
        if (Input.GetButton("Horizontal"))
        {
            camBehaviour.m_Lens.FieldOfView = cameraTargetFov - Mathf.SmoothStep(0, cameraZoomAmount, boostForce);
            Time.timeScale = 1 - boostForce / 2;

            if (boostForce < maximumBoostForce)
            {
                boostForce += 0.01f;
                rb.velocity = rb.velocity - (rb.velocity * boostForce);
            }
                
            inputDirection = Input.GetAxisRaw("Horizontal");
        }
        if (Input.GetButtonUp("Horizontal"))
        {
            Time.timeScale = 1;
            camBehaviour.m_Lens.FieldOfView = cameraTargetFov;
            
            boostPS.Stop();
            directionalPS.Stop();
            directionalPS.Clear();

            if (timesJumped < 2)
            {
                
                rb.AddForce((transform.right * inputDirection * 20) * boostForce, ForceMode.Impulse);
            }
            timesJumped++;
            OutlineColor();

            inputDirection = 0;
            boostForce = 0;
        }

        boostForceIndicator.value = boostForce;
    }

    private void RolloverBehaviour()
    {
        
        if (rb.velocity.magnitude < 1 && boostForce < 0.1f)
        {
            Collider[] hitColliders = Physics.OverlapBox(this.transform.position + (-transform.up * 0.7f), new Vector3(1f, 0.6f, 1), transform.rotation);
            if (hitColliders.Length < 1)
            {
                rb.AddForce(-transform.up);
                rb.AddTorque(0, 0f, -360, ForceMode.Impulse);
            }
            else
            {
                timesJumped = 0;
                OutlineColor();
            }
        }
    }

    private void OutlineColor()
    {
        
        switch (timesJumped)
        {
            case 0:
                outlineMat.SetColor("_OutlineColor", Color.white);
                break;
            case 1:
                outlineMat.SetColor("_OutlineColor", Color.yellow);
                break;
            case 2:
                outlineMat.SetColor("_OutlineColor", Color.grey);
                break;
            default:
                break;
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireCube(this.transform.position + (transform.up * 0.7f), new Vector3(1.2f, 0.6f, 1));
    }
}
