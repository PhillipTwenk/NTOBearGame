using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckPointClass
{
    public void CheckPointStartValue(){
        PlayerPrefs.SetInt("CPCondition", 0);
    }

    // Определение номера ЧекПоинта
    
    public int DefiningCheckPoint(GameObject gameObjectCP){
        string name = gameObjectCP.name;
        char nameNumberChar = name[1];
        int nameNumber = (nameNumberChar - '0') - 1;
        return nameNumber;
    }

    // Активация чекпоинта при условии что он ещё не активен

    public void ActivationCheckPoint(GameObject Mark){
        if(!Mark.active){
            Mark.SetActive(true);
        }
    }

    // Удаление триггера, отслеживающего активацию чекпоинта

    public void DeleteCPTrigger(SphereCollider TriggerCP){
        if(TriggerCP.enabled == true){
            TriggerCP.enabled = false;
        }
    }

    // Сохраняет новое значение последнего чекпоинта

    public void NewSave(int NumberCheckPoint){
        PlayerPrefs.SetInt("CPCondition", NumberCheckPoint);
    }

    // Реализация телепортации после смерти игрока от падения

    public void DeadTeleportation(Transform[] CPposArray, Transform CharacterPos, Transform CameraPos) {
        int CPCondition = PlayerPrefs.GetInt("CPCondition");
        Vector3 newCharacterPosition = new Vector3(CPposArray[CPCondition].position.x, CPposArray[CPCondition].position.y, CPposArray[CPCondition].position.z);
        CharacterPos.position = newCharacterPosition;
        Vector3 newCamPosition = new Vector3(CharacterPos.position.x, CharacterPos.position.y, CharacterPos.position.z);
        CameraPos.position = newCamPosition;
    }
}
