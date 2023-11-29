using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIController : MonoBehaviour
{
    public GameObject[] ArrayButtonsMain;
    public GameObject[] ArrayMenus;
    public Transform characterPosition;
    public Transform CameraPosition;
    public Transform[] CheckPointArrayPosition;
    private void Start()
    {
    }
    #region Buttons Methods
    public void MapButtonOpen(){
        //Activation Map
        ArrayMenus[0].SetActive(false);
        ArrayMenus[1].SetActive(true);
        ArrayMenus[2].SetActive(false);
        ArrayMenus[3].SetActive(false);
        ArrayButtonsMain[5].SetActive(true);
        ArrayButtonsMain[4].SetActive(false);
    }
    public void ChatButtonOpen(){
        //Activation chat 
        ArrayMenus[0].SetActive(true);
        ArrayMenus[1].SetActive(false);
        ArrayMenus[2].SetActive(false);
        ArrayMenus[3].SetActive(false);
        ArrayButtonsMain[5].SetActive(true);
        ArrayButtonsMain[3].SetActive(false);
        ArrayButtonsMain[4].SetActive(false);
    }
    public void ListButtonOpen(){
        //Activation list with Chemical elements
        ArrayMenus[0].SetActive(false);
        ArrayMenus[1].SetActive(false);
        ArrayMenus[2].SetActive(true);
        ArrayMenus[3].SetActive(false);
        ArrayButtonsMain[5].SetActive(true);
        ArrayButtonsMain[4].SetActive(false);
    }
    public void BriefcaseButtonOpen(){
        //Activation our portable briefcase
        ArrayMenus[0].SetActive(false);
        ArrayMenus[1].SetActive(false);
        ArrayMenus[2].SetActive(false);
        ArrayMenus[3].SetActive(true);
        ArrayButtonsMain[5].SetActive(true);
        ArrayButtonsMain[0].SetActive(false);
        ArrayButtonsMain[1].SetActive(false);
        ArrayButtonsMain[2].SetActive(false);
        ArrayButtonsMain[3].SetActive(false);
        ArrayButtonsMain[4].SetActive(false);
    }
    public void NotificationButtonOpen(){
        //Activation chat
        ArrayMenus[0].SetActive(true);
        ArrayMenus[1].SetActive(false);
        ArrayMenus[2].SetActive(false);
        ArrayMenus[3].SetActive(false);
        ArrayButtonsMain[3].SetActive(false);
        ArrayButtonsMain[5].SetActive(true);
        ArrayButtonsMain[4].SetActive(false);
    }
    public void CloseButton(){
        //Activation chat
        ArrayMenus[0].SetActive(false);
        ArrayMenus[1].SetActive(false);
        ArrayMenus[2].SetActive(false);
        ArrayMenus[3].SetActive(false);
        ArrayButtonsMain[0].SetActive(true);
        ArrayButtonsMain[1].SetActive(true);
        ArrayButtonsMain[2].SetActive(true);
        ArrayButtonsMain[4].SetActive(true);
    }
    public void MarksMethod(GameObject ButtonObj){
        //Получение имени Кнопки
        string name = ButtonObj.name;
        //Получение его номера 
        char nameNumberChar = name[6];
        //Перевод номера в целочисленный тип
        int nameNumber = (nameNumberChar - '0') - 1;
        //Перенос персонажа и камеры в нужное место
        Vector3 newPositionCharacter = new Vector3(CheckPointArrayPosition[nameNumber].position.x, CheckPointArrayPosition[nameNumber].position.y, CheckPointArrayPosition[nameNumber].position.z + 5);
        characterPosition.position = newPositionCharacter;
        Vector3 newCamPosition = new Vector3(characterPosition.position.x, characterPosition.position.y, characterPosition.position.z);
        CameraPosition.position = newCamPosition;
        ArrayMenus[1].SetActive(false);
    }
    #endregion
}
