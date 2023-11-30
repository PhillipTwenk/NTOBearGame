using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PortalSystem : MonoBehaviour
{
    public Transform[] PointPortalTeleport;
    private Transform CharacterPosition;
    public Transform CameraPosition;
    void Start()
    {
        CharacterPosition = GetComponent<Transform>();
    }
    void OnCollisionEnter(Collision other)
    {
        if(other.gameObject.tag == "PortalTriggerForward"){
            string name = other.gameObject.name;
            int NumberTrigger = int.Parse(name);
            CharacterPosition.position = PointPortalTeleport[NumberTrigger + 1].position;
            Vector3 newCamPosition = new Vector3(CharacterPosition.position.x, CharacterPosition.position.y, CharacterPosition.position.z);
            CameraPosition.position = newCamPosition;
        }
        if(other.gameObject.tag == "PortalTriggerBackward"){
            string name = other.gameObject.name;
            int NumberTrigger = int.Parse(name);
            CharacterPosition.position = PointPortalTeleport[NumberTrigger - 1].position;
            Vector3 newCamPosition = new Vector3(CharacterPosition.position.x, CharacterPosition.position.y, CharacterPosition.position.z);
            CameraPosition.position = newCamPosition;
        }
    }
}
