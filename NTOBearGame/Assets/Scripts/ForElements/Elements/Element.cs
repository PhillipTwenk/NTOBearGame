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
        Debug.Log(element_name_text.text);
        if(gameObject.name.Split('(')[0] == "NaOCl" && PlayerPrefs.GetInt("ProgressInt") == 10){
            QuestClassInstance.StartNewQuest(PlayerPrefs.GetInt("ProgressInt"));
        }
        if(gameObject.name.Split('(')[0] == "Na₂S₂O₃" && PlayerPrefs.GetInt("ProgressInt") == 19){
            QuestClassInstance.StartNewQuest(PlayerPrefs.GetInt("ProgressInt"));
        }
        if(gameObject.name.Split('(')[0] == "Li₂CO₃" && PlayerPrefs.GetInt("ProgressInt") == 28){
            QuestClassInstance.StartNewQuest(PlayerPrefs.GetInt("ProgressInt"));
        }
        if(gameObject.name.Split('(')[0] == "Na" && PlayerPrefs.GetInt("ProgressInt") == 5){
            QuestClassInstance.StartNewQuest(PlayerPrefs.GetInt("ProgressInt"));
            if(Counter != 3)
            {
                Counter += 1;
            }
            else
            {
                Counter = 1;
            }
        }
        if(gameObject.name.Split('(')[0] == "Сl" && PlayerPrefs.GetInt("ProgressInt") == 5){
            QuestClassInstance.StartNewQuest(PlayerPrefs.GetInt("ProgressInt"));
            if(Counter != 3)
            {
                Counter += 1;
            }
            else
            {
                Counter = 1;
            }
        }
        if(gameObject.name.Split('(')[0] == "H₂O" && PlayerPrefs.GetInt("ProgressInt") == 5){
            QuestClassInstance.StartNewQuest(PlayerPrefs.GetInt("ProgressInt"));
            if(Counter != 3)
            {
                Counter += 1;
            }
            else
            {
                Counter = 1;
            }
        }
        if(gameObject.name.Split('(')[0] == "S" && PlayerPrefs.GetInt("ProgressInt") == 17){
            QuestClassInstance.StartNewQuest(PlayerPrefs.GetInt("ProgressInt"));
            if(Counter != 3)
            {
                Counter += 1;
            }
            else
            {
                Counter = 1;
            }
        }
        if(gameObject.name.Split('(')[0] == "Na₂O" && PlayerPrefs.GetInt("ProgressInt") == 17){
            QuestClassInstance.StartNewQuest(PlayerPrefs.GetInt("ProgressInt"));
            if(Counter != 3)
            {
                Counter += 1;
            }
            else
            {
                Counter = 1;
            }
        }
        if(gameObject.name.Split('(')[0] == "S" && PlayerPrefs.GetInt("ProgressInt") == 17){
            QuestClassInstance.StartNewQuest(PlayerPrefs.GetInt("ProgressInt"));
            if(Counter != 3)
            {
                Counter += 1;
            }
            else
            {
                Counter = 1;
            }
        }
        if(gameObject.name.Split('(')[0] == "CO₂" && PlayerPrefs.GetInt("ProgressInt") == 26){
            QuestClassInstance.StartNewQuest(PlayerPrefs.GetInt("ProgressInt"));
            if(Counter != 3)
            {
                Counter += 1;
            }
            else
            {
                Counter = 1;
            }
        }
        if(gameObject.name.Split('(')[0] == "NaOH" && PlayerPrefs.GetInt("ProgressInt") == 26){
            QuestClassInstance.StartNewQuest(PlayerPrefs.GetInt("ProgressInt"));
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
        if(gameObject.name.Split('(')[0] == "NaOCl" && PlayerPrefs.GetInt("ProgressInt") == 12){
            QuestClassInstance.StartNewQuest(PlayerPrefs.GetInt("ProgressInt"));
        }
        if(gameObject.name.Split('(')[0] == "Na₂S₂O₃" && PlayerPrefs.GetInt("ProgressInt") == 21){
            QuestClassInstance.StartNewQuest(PlayerPrefs.GetInt("ProgressInt"));
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
