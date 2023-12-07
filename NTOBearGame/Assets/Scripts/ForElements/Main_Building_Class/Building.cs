using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Mono.Data.Sqlite;
using System.Data;
using System;


// КЛАСС ДЛЯ РАБОТЫ С ВЕЩЕСТВАМИ И РЕАКЦИЯМИ
// - Создал: @Artefok
// - Использует все using сверху, а также DBManager
// - Статичный класс для вызова элементов в других скриптах игры
public static class Building
{   
    // СТАТИЧНЫЕ ПЕРЕМЕННЫЕ В КЛАССЕ
    public static bool is_agregat_canvas_activated;

    // ОБЩИЕ МЕТОДЫ
    // - Пояснения к комментам:
    //     INPUT: параметры функция (*param - необязательный параметр)
    //     OUTPUT: то, что возвращает функция
    // - Методы указаны в том порядке, в котором они (в основном) будут использоваться в коде


    // Информация о элементе
    // INPUT: *id элемента ИЛИ *его название
    // OUTPUT: информация об элементе в виде словаря
    public static Dictionary<string, string> ElementInfo(int element_id = -1, string element_name = null){
        string element_info_query = "";
        if(element_id == -1){
            element_info_query = $"SELECT * FROM elements_info WHERE name = '{element_name}'";
        } else if(element_name == null){
            element_info_query = $"SELECT * FROM elements_info WHERE element_id = {element_id}";
        }

        DataTable element_info = DBManager.GetTable(element_info_query);
        Dictionary<string, string> res_element = new Dictionary<string, string>();

        for(int i = 0; i < element_info.Columns.Count; i++){
            string value = element_info.Rows[0][i].ToString();
            res_element[$"{element_info.Columns[i]}"] = value;
        }
        return res_element;
    }

    // Информация о действиях используемого агрегата
    // INPUT: -
    // OUTPUT: все возможные действия в рабочем агрегате
    public static List<string> ActionsChoiceInfo(string building){
        string building_id = DBManager.ExecuteQuery($"SELECT id_building FROM buildings WHERE building_name = '{building}'");
        DataTable actions = DBManager.GetTable($"SELECT action_name FROM actions WHERE building = {building_id}");
        List<string> res_actions = new List<string>();
        res_actions.Add(""); // добавляем пустой выбор для проверки в UI агрегата на полноту алгоритма
        for(int i = 0; i < actions.Rows.Count; i++){
            string value = actions.Rows[i][0].ToString();
            res_actions.Add(value);
        }
        return res_actions;
    }

    // Информация о всех элементах (если)
    // INPUT: все объекты или только те, что изучены?
    // OUTPUT: все возможные вещества (позже те, которые находятся в инвентаре)
    public static List<string> ElementsChoiceInfo(bool is_in_inventory = false){
        string query = "";
        List<string> res_elements = new List<string>();
        if(is_in_inventory){
            DataTable inventory_element_ids = DBManager.GetTable("SELECT element_id FROM inventory");
            for(int i = 0; i < inventory_element_ids.Rows.Count; i++){
                if(inventory_element_ids.Rows[i][0].ToString() != "0"){
                    res_elements.Add(ElementInfo(element_id: Convert.ToInt32(inventory_element_ids.Rows[i][0].ToString()))["name"]);
                } else {
                    res_elements.Add("-");
                }
            }
            return res_elements;

        } else {
            query = "SELECT name FROM elements_info WHERE element_id > 0";
            DataTable elements = DBManager.GetTable(query);
            res_elements.Add(""); // добавляем пустой выбор для проверки в UI агрегата на полноту алгоритма
            for(int i = 0; i < elements.Rows.Count; i++){
                string value = elements.Rows[i][0].ToString();
                res_elements.Add(value);
            }
            return res_elements;
        }
    }

    // Информация о всех агрегатах, куда можно вывести вещество
    // INPUT: -
    // OUTPUT: все возможные строения из БД
    public static List<string> ExitsChoiceInfo(){
        DataTable buildings = DBManager.GetTable($"SELECT building_name FROM buildings");
        List<string> res_buildings = new List<string>();
        res_buildings.Add(""); // добавляем пустой выбор для проверки в UI агрегата на полноту алгоритма
        for(int i = 0; i < buildings.Rows.Count; i++){
            string value = buildings.Rows[i][0].ToString();
            res_buildings.Add(value);
        }
        return res_buildings;
    }

    // Проведение реакции с веществом(-ами)
    // INPUT: название агрегата, действия, параметр, основное вещество с его хар-ками, *доп.вещество
    // OUTPUT: информация о получившемся элементе
    public static List<Dictionary<string, string>> Reaction(string building, string action, List<int> element_ids, int parameter = 0){  
        string building_reaction_id = DBManager.ExecuteQuery($"SELECT id_building FROM buildings WHERE building_name = '{building}'"); // получение id рабочего агрегата
        DataTable action_info = DBManager.GetTable($"SELECT id_action, perm_output FROM actions WHERE action_name = '{action}' AND building = {Convert.ToInt32(building_reaction_id)}"); // получение id действия через id рабочего агрегата и action
        int action_id = Convert.ToInt32(action_info.Rows[0][0]); // id действия
        bool perm_output = Convert.ToBoolean(action_info.Rows[0][1]); // наличие вывода(true = есть; false = без выходного вещества)

        string action_name = DBManager.ExecuteQuery($"SELECT action_name FROM actions WHERE id_action = {action_id}");
        string res_element_query = ""; // запрос в БД

        if(action_id > 1 && !action_name.Contains("оставить основной элемент")){
            if(parameter == 1){
                res_element_query = $"SELECT result1 FROM elements_reactions WHERE id_element1 = {element_ids[0]} AND id_element2 = {element_ids[1]} AND action = '{action_id}' AND parameter_for_action = 0";
            } else if (parameter == 2){
                res_element_query = $"SELECT result2 FROM elements_reactions WHERE id_element1 = {element_ids[0]} AND id_element2 = {element_ids[1]} AND action = '{action_id}' AND parameter_for_action = 0";
            } else if (parameter == 0){
                res_element_query = $"SELECT result1,result2 FROM elements_reactions WHERE id_element1 = {element_ids[0]} AND id_element2 = {element_ids[1]} AND action = '{action_id}' AND parameter_for_action = 0";
            }
        } else if (action_id > 1 && action_name.Contains("оставить основной элемент")){
            res_element_query = $"SELECT result1 FROM elements_reactions WHERE id_element1 = {element_ids[0]} AND id_element2 = {element_ids[1]} AND action = '{action_id-1}' AND parameter_for_action = {parameter}";
        } else if (action_id > 1 && action_name.Contains("оставить основной элемент") && action_name.Contains("Аннигилировать")){
            res_element_query = "";
        } else {
            res_element_query = $"SELECT result1,result2 FROM elements_reactions WHERE id_element1 = {element_ids[0]} AND id_element2 = {element_ids[1]} AND action = '{action_id}' AND parameter_for_action = {parameter}";
        }

        Debug.Log(res_element_query);
        DataTable res_element_ids = null;
        if(res_element_query != ""){
            res_element_ids = DBManager.GetTable(res_element_query); // проведение нужного запроса в БД
        } else { // если пришла реакция исключение - Аннигилировать, оставить основной элемент
            return ReactionResultFormat(elementsList: new List<int>(){0,element_ids[0]});
        }
        
        Debug.Log(res_element_ids.Rows.Count);
        if(res_element_ids.Rows.Count == 0){ // если такой реакции нет
            res_element_query = $"SELECT result1, result2 FROM elements_reactions WHERE id_element1 = {element_ids[1]} AND id_element2 = {element_ids[0]} AND action = '{action_id}' AND parameter_for_action = {parameter}";
            res_element_ids = DBManager.GetTable(res_element_query); // проведение второго запроса в БД (если user решил провести реакцию соединения Cl и Na, а не Na и Cl, как положено в таблице)
            if(res_element_ids.Rows.Count == 0){ // если реакция отсутствует
                return ReactionResultFormat(elementsList: element_ids); // возвращает изначальные элементы без изменений
            }
        } else if(!perm_output){
            return null; // если реакция завязана на уничтожении элемента (исключение: при нагревании элемент расплавляется)
        }

        return ReactionResultFormat(elementsDT: res_element_ids); // возвращение информации об итоговом элементе
    }

    // Преобразование таблицы элементов/списка id элементов в список словарей информации об элементах для последующего вывода в функции Reaction
    // INPUT: *таблица веществ ИЛИ *список id элементов
    // OUTPUT: список словарей информации об элементах
    private static List<Dictionary<string, string>> ReactionResultFormat(DataTable elementsDT = null, List<int> elementsList = null){
        List<Dictionary<string, string>> results = new List<Dictionary<string, string>>(); // итоговый список информации о получившихся элементах
        if(elementsList != null){
            for(int i = 0; i < elementsList.Count; i++){
                string value = elementsList[i].ToString(); 
                Dictionary<string, string> info = ElementInfo(element_id: Convert.ToInt32(value)); // заполнение информации об элементе в словарь
                results.Add(info); // пополнение итогового списка информацией об элементе
            }
        } else if(elementsDT != null){
            for(int i = 0; i < elementsDT.Columns.Count; i++){
                string value = elementsDT.Rows[0][i].ToString(); // получение id элемента из i столбца
                Dictionary<string, string> info = ElementInfo(element_id: Convert.ToInt32(value)); // заполнение информации об элементе в словарь
                results.Add(info); // пополнение итогового списка информацией об элементе
            }
        }

        return results; // вывод списка словарей информации об элементах
    }

    // Нахождение всех известных реакций с данным веществом из БД
    // INPUT: id элемента
    // OUTPUT: список из строк самих реакций (e.g. Na + O = Na2O без учета коэффициентов)
    public static List<string> ReactionsWithElement(int element_id){
        List<string> output = new List<string>();
        Debug.Log(element_id);
        DataTable all_reactions = DBManager.GetTable($"SELECT * FROM elements_reactions WHERE id_element1 = {element_id} OR id_element2 = {element_id} OR result1 = {element_id} OR result2 = {element_id}");
        for(int i = 0; i < all_reactions.Rows.Count; i++){
            string reaction = "";
            string element1 = ElementInfo(element_id: Convert.ToInt32(all_reactions.Rows[i]["id_element1"].ToString()))["name"];
            string element2 = ElementInfo(element_id: Convert.ToInt32(all_reactions.Rows[i]["id_element2"].ToString()))["name"];

            if(element2 == "0"){
                element2 = "";
                reaction += $"{element1} = ";
            } else {
                reaction += $"{element1} + {element2} = ";
            }

            string result_element1 = ElementInfo(element_id: Convert.ToInt32(all_reactions.Rows[i]["result1"].ToString()))["name"];
            string result_element2 = ElementInfo(element_id: Convert.ToInt32(all_reactions.Rows[i]["result2"].ToString()))["name"];
            if(result_element1 == "0" && result_element2 == "0"){
                reaction += "(уничтожение)";
            } else if (result_element1 == "0" && result_element2 != "0"){
                reaction += result_element2;
            } else if (result_element1 != "0" && result_element2 == "0"){
                reaction += result_element1;
            } else if (result_element1 != "0" && result_element2 != "0"){
                reaction += $"{result_element1} + {result_element2}";
            }

            int temp_action_id = Convert.ToInt32(all_reactions.Rows[i]["action"].ToString());
            string temp_action = DBManager.ExecuteQuery($"SELECT action_name FROM actions WHERE id_action = {temp_action_id}");
            string parameter = all_reactions.Rows[i]["parameter_for_action"].ToString();
            reaction += $" ({temp_action} при параметре {parameter})";
            
            output.Add(reaction);
            reaction = "";
        }
        return output;
    }


    // Вывод результата реакции в другой агрегат
    // INPUT: элемент, имя агрегата, в который нужно направить
    // OUTPUT: - (помещает элемент в место принятия веществ агрегата)

}
