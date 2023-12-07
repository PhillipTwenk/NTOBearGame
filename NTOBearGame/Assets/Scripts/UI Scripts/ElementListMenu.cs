using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Mono.Data.Sqlite;
using System.Data;
using System;

public class ElementListMenu : MonoBehaviour
{
    private List<string> ElementsNames; 
    private List<string> ElementsReactions;
    public Dictionary<string, string> temp_element_info;
    private List<string> ElemColumns = new List<string>(){"name", "contain_temp", "state", "dangerous", "antidote"};
    private List<string> ElemColumnsTranslation = new List<string>(){"Название","Температура хранения °C","Состояние", "Опасен", "Антидот"};
    private Dictionary<string, string> ElemStateTranslations = new Dictionary<string, string>(){
        {"liquid","Жидкость"}, 
        {"solid","Твердое"}, 
        {"gas","Газ"}
    };
    [SerializeField] Dropdown ListOfElements;
    [SerializeField] Text ElementReactionsText;
    [SerializeField] Text ElementInfoText;
    void Start()
    {
        ListOfElements.ClearOptions();
        ElementsNames = Building.ElementsChoiceInfo();
        ListOfElements.AddOptions(ElementsNames);
    }

    public void ElementsAndReactions(){
        ElementInfoText.text = "";
        ElementReactionsText.text = "";

        temp_element_info = Building.ElementInfo(element_id: ListOfElements.value);
        for(int i = 0; i < ElemColumnsTranslation.Count; i++){
            if(ElemColumnsTranslation[i] == "Состояние"){
                ElementInfoText.text += $"{ElemColumnsTranslation[i]}: {ElemStateTranslations[temp_element_info[ElemColumns[i]]]}\n";
            } else if(ElemColumnsTranslation[i] == "Опасен") {
                if(temp_element_info["dangerous"] == "1") ElementInfoText.text += $"{ElemColumnsTranslation[i]}: да\n";
                else ElementInfoText.text += $"{ElemColumnsTranslation[i]}: нет\n";
            } else if (ElemColumnsTranslation[i] == "Антидот"){
                if(temp_element_info["antidote"] == "0") ElementInfoText.text += $"{ElemColumnsTranslation[i]}: -\n";
                else ElementInfoText.text += $"{ElemColumnsTranslation[i]}: {Building.ElementInfo(element_id: Convert.ToInt32(temp_element_info["antidote"]))["name"]}\n";
            } else {
                ElementInfoText.text += $"{ElemColumnsTranslation[i]}: {temp_element_info[ElemColumns[i]]}\n";
            }
        }

        ElementsReactions = Building.ReactionsWithElement(ListOfElements.value);
        for(int i = 0; i < ElementsReactions.Count; i++){
            ElementReactionsText.text += $"{ElementsReactions[i]}\n";
        }
    }
}
