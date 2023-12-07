using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Data;
using TMPro;

public class Element : MonoBehaviour
{
    private Dictionary<string, string> element_info;
    private bool is_mouse_on_object = false;
    [SerializeField] TMP_Text element_name_text;

    private void Start(){
        element_info = Building.ElementInfo(element_name: gameObject.name.Split('(')[0]);
        gameObject.name = gameObject.name.Split('(')[0];
        element_name_text.text = gameObject.name.Split('(')[0];
        
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
        DBManager.ExecuteQueryWithoutAnswer($"UPDATE elements_info SET studied_state = 1 WHERE name = '{element_name_text.text}' AND studied_state = 0");
        string empty_slot_id = DBManager.ExecuteQuery($"SELECT MIN(slot_id) FROM inventory WHERE element_id = 0");
        DBManager.ExecuteQueryWithoutAnswer($"UPDATE inventory SET element_id = {element_info["element_id"]} WHERE slot_id = {Convert.ToInt32(empty_slot_id)}"); 
        Inventory.is_changed = true;
        Destroy(gameObject);
    }
    private void AddEffect(){
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
