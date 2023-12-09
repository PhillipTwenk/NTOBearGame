using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckPointSystem : MonoBehaviour
{
    public GameObject Mark;
    private CheckPointClass CPclass;
    private int NumberCheckPoint;
    private SphereCollider TriggerCP;
    public ParticleSystem checkpoint;
    void Start()
    {
        CPclass = new CheckPointClass();
        TriggerCP = GetComponent<SphereCollider>();
        CPclass.CheckPointStartValue();
        NumberCheckPoint = CPclass.DefiningCheckPoint(gameObject);
    }
    void OnTriggerEnter(Collider other)
    {
        // Игрок открывает новый чекпоинт

        if (other.gameObject.tag == "Player")
        {
            checkpoint.Play();
            CPclass.ActivationCheckPoint(Mark);
            CPclass.DeleteCPTrigger(TriggerCP);
            CPclass.NewSave(NumberCheckPoint);
        }
    }
}
