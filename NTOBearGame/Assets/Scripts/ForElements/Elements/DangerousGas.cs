using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Data;

public class DangerousGas : MonoBehaviour
{
    private Dictionary<string, string> element_info;
    void Start(){
        element_info = Building.ElementInfo(element_name: gameObject.name.Split('(')[0]);
        gameObject.name = gameObject.name.Split('(')[0];
    }
    private void AddEffect(){
        PlayerState.player_state = "";
        DataTable result_effect = DBManager.GetTable($"SELECT result, result_parameter FROM elements_effects WHERE entry_element = {element_info["element_id"]}");
        string result_effect_name = DBManager.ExecuteQuery($"SELECT result FROM elements_effects_result WHERE result_id = {Convert.ToInt32(result_effect.Rows[0][0].ToString())}");
        string element_name = Building.ElementInfo(element_id: Convert.ToInt32(result_effect.Rows[0][1].ToString()))["name"];
        result_effect_name += element_name;
        PlayerState.player_state = result_effect_name;
        PlayerState.is_changed = true;
    }

    private void OnTriggerEnter(Collider coll){
        if(coll.name == "fullcharacter"){
            AddEffect();
        }
    }
}
