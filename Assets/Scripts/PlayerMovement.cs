using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using Cinemachine;


public class PlayerMovement : MonoBehaviour
{
    Rigidbody rb;
    private float boostForce;
    int maximumBoostForce = 1;
    [SerializeField]
    Slider boostForceIndicator;
    float inputDirection;
    private Vector3 currentAngle;
    CinemachineVirtualCamera camBehaviour;
    [SerializeField]
    List<ParticleSystem> boostPS = new List<ParticleSystem>();
    ParticleSystem directionalPS;

    [SerializeField]
    float cameraZoomAmount;
    [SerializeField]
    float cameraTargetFov;

    Material outlineMat;

    private int timesJumped = 0;
    bool isGrounded;

    void OnValidate()
    {
        rb = this.gameObject.GetComponent<Rigidbody>();
        camBehaviour = GameObject.FindObjectOfType<CinemachineVirtualCamera>();
        directionalPS = GameObject.Find("DirectionalPS").GetComponent<ParticleSystem>();
        boostPS = GameObject.Find("BoostChargeParent").GetComponentsInChildren<ParticleSystem>().ToList();
        outlineMat = this.gameObject.GetComponentInChildren<MeshRenderer>().sharedMaterial;
    }

    private void Start()
    {
        currentAngle = this.transform.position;
        MultiParticleFX(boostPS, false);
        directionalPS.Stop();
        SetTimesJumped(timesJumped = 0);
        GroundedBehaviour();
    }

    void Update()
    {
        BoostBehaviour();
        RolloverBehaviour();
        GroundedBehaviour();
    }

    private void BoostBehaviour()
    {
        if (timesJumped >= 2)
            return;

        if (Input.GetButtonDown("Horizontal") && inputDirection == 0)
        {
            MultiParticleFX(boostPS, true);
            directionalPS.Play();
            Quaternion rotationTarget = Quaternion.Euler(0, Input.GetAxisRaw("Horizontal") * 90, 0);
            directionalPS.transform.localRotation = rotationTarget;

            inputDirection = Input.GetAxisRaw("Horizontal");
        }
        if (Input.GetButton("Horizontal") && inputDirection == Input.GetAxisRaw("Horizontal"))
        {
            camBehaviour.m_Lens.FieldOfView = cameraTargetFov - Mathf.SmoothStep(0, cameraZoomAmount, boostForce);
            Time.timeScale = 1 - boostForce / 2;

            if (boostForce < maximumBoostForce)
            {
                boostForce += 1.5f * Time.deltaTime;
                rb.velocity = rb.velocity - (rb.velocity * boostForce);
            }  
        }
        if (Input.GetButtonUp("Horizontal"))
        {
            Time.timeScale = 1;
            camBehaviour.m_Lens.FieldOfView = cameraTargetFov;

            MultiParticleFX(boostPS, false);

            directionalPS.Stop();
            directionalPS.Clear();

            if (timesJumped < 2)
            {
                rb.AddForce((transform.right * inputDirection * 20) * boostForce, ForceMode.Impulse);
            }
            timesJumped++;
            SetTimesJumped(timesJumped);

            inputDirection = 0;
            boostForce = 0;
        }

        boostForceIndicator.value = boostForce;
    }

    private void RolloverBehaviour()
    {
        if (rb.velocity.magnitude < 1 && boostForce < 0.1f)
        {
            Collider[] hitColliders = Physics.OverlapSphere(this.transform.position + (transform.up * 0.7f), 0.4f);
            if (hitColliders.Length > 1)
            {
                rb.AddForce(-transform.up);
                rb.AddTorque(0, 0f, -360, ForceMode.Impulse);
            }
        }
    }

    private void GroundedBehaviour()
    {
        Collider[] hitColliders = Physics.OverlapSphere(this.transform.position, 0.2f);
        if (hitColliders.Length > 1)
        {
            SetTimesJumped(timesJumped = 0);
            isGrounded = true;
        }
        else
        {
            if (isGrounded == true)
            {
                SetTimesJumped(timesJumped = 1);
                isGrounded = false;
            }
        }
    }

    private void SetTimesJumped(int timesJumped)
    {
        switch (timesJumped)
        {
            case 0:
                outlineMat.SetColor("_OutlineColor", Color.yellow);
                break;
            case 1:
                outlineMat.SetColor("_OutlineColor", new Color(1, 0.80f, 0.016f));
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
        Gizmos.DrawWireSphere(this.transform.position + (transform.up * 0.7f), 0.4f);
        Gizmos.DrawWireSphere(this.transform.position, 0.2f);
    }

    public void MultiParticleFX(List<ParticleSystem> psList, bool shouldPlay)
    {
        foreach (ParticleSystem ps in psList)
        {
            if (shouldPlay)
                ps.Play();
            else
                ps.Stop();
        }
    }
}
