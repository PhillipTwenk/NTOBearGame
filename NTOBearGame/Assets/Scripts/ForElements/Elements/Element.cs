using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Data;
using TMPro;

public class Element : MonoBehaviour
{
    [SerializeField] private Dictionary<string, string> element_info;
    private bool is_mouse_on_object = false;
    [SerializeField] TMP_Text element_name_text;
    private QuestClass QuestClassInstance;
    private int Counter;

    private void Start(){
        Counter = 1;
        element_info = Building.ElementInfo(element_name: gameObject.name.Split('(')[0]);
        gameObject.name = gameObject.name.Split('(')[0];
        element_name_text.text = gameObject.name.Split('(')[0];
        QuestClassInstance = new QuestClass();
    }
    private void Update(){
        if(Input.GetMouseButtonDown(0) && is_mouse_on_object){
            AddItemToInventory();
        } else if (Input.GetMouseButtonDown(1) && is_mouse_on_object){
            AddEffect();
        }
    }

    private void OnMouseEnter(){
        is_mouse_on_object = true; // если мы навелись на объект
    }
    private void OnMouseExit(){
        is_mouse_on_object = false; // если мы отводим мышку от объекта
    }   

    private void AddItemToInventory(){
        if(!TutorialClass.GetElementBool() && TutorialClass.IsInTutorial){
            return;
        }
        Debug.Log(element_name_text.text);
        if (gameObject.name.Split('(')[0] == "H")
        {
            StaticStorage.TutorialClassStatic.ContinueTutorial(27);
        }
        if(gameObject.name.Split('(')[0] == "NaOCl"){
            QuestClassInstance.CheckQuest(10);
        }
        if(gameObject.name.Split('(')[0] == "Na₂S₂O₃"){
            QuestClassInstance.CheckQuest(19);
        }
        if(gameObject.name.Split('(')[0] == "Li₂CO₃"){
            QuestClassInstance.CheckQuest(28);
        }
        if(gameObject.name.Split('(')[0] == "Na"){
            QuestClassInstance.CheckQuest(5);
            if(Counter != 3)
            {
                Counter += 1;
            }
            else
            {
                Counter = 1;
            }
        }
        if(gameObject.name.Split('(')[0] == "Сl"){
            QuestClassInstance.CheckQuest(5);
            if(Counter != 3)
            {
                Counter += 1;
            }
            else
            {
                Counter = 1;
            }
        }
        if(gameObject.name.Split('(')[0] == "H₂O"){
            QuestClassInstance.CheckQuest(5);
            if(Counter != 3)
            {
                Counter += 1;
            }
            else
            {
                Counter = 1;
            }
        }
        if(gameObject.name.Split('(')[0] == "S"){
            QuestClassInstance.CheckQuest(17);
            if(Counter != 3)
            {
                Counter += 1;
            }
            else
            {
                Counter = 1;
            }
        }
        if(gameObject.name.Split('(')[0] == "Na₂O"){
            QuestClassInstance.CheckQuest(17);
            if(Counter != 3)
            {
                Counter += 1;
            }
            else
            {
                Counter = 1;
            }
        }
        if(gameObject.name.Split('(')[0] == "S"){
            QuestClassInstance.CheckQuest(17);
            if(Counter != 3)
            {
                Counter += 1;
            }
            else
            {
                Counter = 1;
            }
        }
        if(gameObject.name.Split('(')[0] == "CO₂"){
            QuestClassInstance.CheckQuest(26);
            if(Counter != 3)
            {
                Counter += 1;
            }
            else
            {
                Counter = 1;
            }
        }
        if(gameObject.name.Split('(')[0] == "NaOH"){
            QuestClassInstance.CheckQuest(26);
            if(Counter != 3)
            {
                Counter += 1;
            }
            else
            {
                Counter = 1;
            }
        }
        DBManager.ExecuteQueryWithoutAnswer($"UPDATE elements_info SET studied_state = 1 WHERE name = '{element_name_text.text}' AND studied_state = 0");
        string empty_slot_id = DBManager.ExecuteQuery($"SELECT MIN(slot_id) FROM inventory WHERE element_id = 0");
        DBManager.ExecuteQueryWithoutAnswer($"UPDATE inventory SET element_id = {element_info["element_id"]} WHERE slot_id = {Convert.ToInt32(empty_slot_id)}"); 
        Inventory.is_changed = true;
        if(gameObject.tag != "Case"){
            Destroy(gameObject);
        }
    }
    private void AddEffect(){
        Debug.Log(1);
        if(gameObject.name.Split('(')[0] == "NaOCl"){
            QuestClassInstance.CheckQuest(12);
        }
        if(gameObject.name.Split('(')[0] == "Na₂S₂O₃"){
            QuestClassInstance.CheckQuest(21);
        }
        PlayerState.player_state = "";
        DataTable result_effect = DBManager.GetTable($"SELECT result, result_parameter FROM elements_effects WHERE entry_element = {element_info["element_id"]}");
        string result_effect_name = DBManager.ExecuteQuery($"SELECT result FROM elements_effects_result WHERE result_id = {Convert.ToInt32(result_effect.Rows[0][0].ToString())}");
        string element_name = Building.ElementInfo(element_id: Convert.ToInt32(result_effect.Rows[0][1].ToString()))["name"];
        result_effect_name += element_name;
        PlayerState.player_state = result_effect_name;
        PlayerState.is_changed = true;
        Destroy(gameObject);
    }
    

}
