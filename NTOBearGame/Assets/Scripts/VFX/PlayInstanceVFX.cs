using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class PlayInstanceVFX : MonoBehaviour
{
    private Material material;
    public float NothingVal;
    public float MaterializedVal;
    public float ChangeSpeed;
    public ParticleSystem InstanceVFX;
    void Start()
    {
        material = GetComponent<SkinnedMeshRenderer>().sharedMaterial;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.V))
        {
            InstanceEffectVFX();
        }
    }
    public void InstanceEffectVFX()
    {
        Debug.Log("PlayVXF");
        StartCoroutine(Instance());
        InstanceVFX.Play();
    }
    private IEnumerator Instance()
    {
        material.SetFloat("_CutoffHeight", NothingVal);
        float currentVal = NothingVal;
        while (currentVal > MaterializedVal)
        {
            currentVal -= ChangeSpeed;
            material.SetFloat("_CutoffHeight", NothingVal + currentVal);
            yield return 0;
        }
        Debug.Log("end");
    }
}
