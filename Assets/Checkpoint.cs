using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Checkpoint : MonoBehaviour
{
    bool active = false;
    [SerializeField] List<ParticleSystem> psList;

    private void OnValidate()
    {
        psList = gameObject.GetComponentsInChildren<ParticleSystem>().ToList();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && active == false)
        {
            this.gameObject.GetComponent<Animation>().Play();
            MultiParticleFX(psList, true);
            active = true;
        }
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
