using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Mono.Data.Sqlite;
using System.Data;
using System;
using TMPro;

// КЛАСС ДЛЯ РАБОТЫ С АГРЕГАТАМИ
// - Создал: @Artefok
// - Использует все using сверху, а также статичный класс Building
// - Публичный класс для создания, обработки и выполнения алгоритма внутри любого агрегата
// - Этот класс заточен под все агрегаты в игре (просто при добавлении скрипта на новый агрегат надо указать его имя в двух переменных из БД на рус. и англ.)


public class BuildingObject : MonoBehaviour
{
    // СТАТИЧНЫЕ ПЕРЕМЕННЫЕ В КЛАССЕ
    

    // ОБЩИЕ МЕТОДЫ
    // - Пояснения к комментам:
    //     INPUT: параметры функция (*param - необязательный параметр)
    //     OUTPUT: то, что возвращает функция
    // - Методы указаны в том порядке, в котором они (в основном) будут использоваться в коде

    // Переменные
    [SerializeField] string building_name; // Название агрегата в интерфейсе (рус., задаётся в каждом агрегате своё значение из БД)
    [SerializeField] string sys_building_name; // Название агрегата для операций внутри (англ., задаётся в каждом агрегате своё значение из БД)
    private List<string> actions = new List<string>(){}; // Действие алгоритма
    private List<string> exits = new List<string>(){}; // Конечная точка алгоритма, куда отправить итог (если указан текущий агрегат - вывод сразу рядом с ним)
    private string reaction_state; // Статус реакции
    private List<int> parameters = new List<int>(){}; // Параметр алгоритма
    private float timer;
    private bool is_canvas_activated = false; // Состояние канваса 
    private bool is_reacted = false;
    public List<List<int>> element_ids = new List<List<int>>(){}; // Используемые в алгоритме ID элементов из базы (если 0 -> вещества нет)
    public List<List<string>> element_names = new List<List<string>>(){}; // Используемые в алгоритме имена веществ из базы (если "" -> вещества нет)
    public List<string> temp_storage; // Хранилище имён элементов, попавших в агрегат 
    public List<int> temp_element_ids; // Хранилище ID элементов, попавших в агрегат 
    public List<string> elements_info; // Список всех элементов, подающихся на выбор в агрегате

    [SerializeField] GameObject Canvas; // Основной канвас агрегата
    [SerializeField] Dropdown ActionsChoice; // Выпадающий список действий для текущего агрегата
    [SerializeField] Dropdown ElementsChoice1; // Выпадающий список элементов для текущего агрегата (I)
    [SerializeField] Dropdown ElementsChoice2; // Выпадающий список элементов для текущего агрегата (II)
    [SerializeField] Dropdown ExitsChoice; // Выпадающий список вариантов вывода для текущего агрегата
    [SerializeField] InputField ParameterInput; // Строка для ввода параметра
    [SerializeField] ElementsPrefabs EP; // Список префабов всех элементов
    [SerializeField] GameObject OutputPlace; // Место у текущего агрегата, куда выводится итог реакции (если цепочка алгоритмов закончилась)
    [SerializeField] GameObject InputPlace; // Место у текущего агрегата, куда вводится итог реакции (если существует цепочка алгоритмов)
    [SerializeField] Text AlgorithmText; // Текст алгоритма в UI
    [SerializeField] GameObject PlayerMenu; // Основное меню игрока (отключаем при открытии канваса агрегата )
    [SerializeField] Text AgregatName; // Надпись (название алгоритма) в UI
    [SerializeField] TMP_Text ReactionStateText; // Надпись над объектом(состояние алгоритма)
    [SerializeField] TMP_Text InputElementsText; // Надпись над объектом(подающиеся элементы)

    // Запуск при появлении на сцене
    // INPUT: -
    // OUTPUT: - (сброс предыдущих алгоритмов и отключение канваса)
    void Start()
    {
        Transporter.AgregatInputPlaces[building_name] = InputPlace;
        // обнуляем сохраненный алгоритм до этого
        AgregatName.text = $"{building_name}";
        ExitAgregatUI();
    }
    public void ExitAgregatUI(){
        ParameterInput.gameObject.SetActive(false); // отключаем поле ввода для параметра
        Canvas.gameObject.SetActive(false); //отключаем канвас
        is_canvas_activated = false;
        Building.is_agregat_canvas_activated = false;
        PlayerMenu.SetActive(true); // отключаем интерфейс игрока, чтобы не было наслоения
    }
    // Функция вызывается каждый кадр и предназначена для проверки закрытия интерфейса алгоритма на Esc
    // INPUT: -
    // OUTPUT: - (при нажатии на кнопку Esc, пока активен интерфейс агрегата, происходит цеопчка действий для удачного закрытия его интерфейса)
    void Update()
    {
        if(is_canvas_activated && Input.GetKey(KeyCode.Escape)){ // нажимая Esc при открытом канвасе(UI агрегата) -> закрываем его с полем ввода параметра
            if(ActionsChoice.value != 0 && (ElementsChoice1.value != 0 && ElementsChoice2.value != 0) && ExitsChoice.options[ExitsChoice.value].text != ""){ // если алгоритм есть
                BuildAlgorithm(); // сохраняем алгоритм
            } else { // если же нет
                ExitAgregatUI();
            }
        } else if(is_reacted){ // если есть какой-то статус реакции
            ReactionStateText.text = reaction_state; // ставим этот статус
            timer += Time.deltaTime;
            if(timer > 3f){
                ReactionStateText.text = ""; // убираем статус через таймер 
                is_reacted = false;
            }
        }
    }

    // Нажатие на агрегат -> активация интерфейса агрегата (отключение основного интерфейса игрока) выставление в выпадающие списки нужные значения
    // INPUT: - (клик по мыши на агрегат)
    // OUTPUT: - (все нужные значения в выпадающих списках из БД)
    void OnMouseDown(){
        if(!Building.is_agregat_canvas_activated && !is_canvas_activated){
            PlayerMenu.SetActive(false);
            Canvas.gameObject.SetActive(true);
            is_canvas_activated = true;
            Building.is_agregat_canvas_activated = true;
            // Заполнение опций для выбора алгоритма          
            // - доступные элементы для 1 позиции
            ElementsChoice1.ClearOptions(); // очищаем опции выбора
            elements_info = Building.ElementsChoiceInfo(); // заносим в список через функцию все элементы
            ElementsChoice1.AddOptions(elements_info); 

            // - доступные элементы для 2 позиции
            ElementsChoice2.ClearOptions(); // очищаем опции выбора
            elements_info = Building.ElementsChoiceInfo(); // заносим в список через функцию все элементы
            ElementsChoice2.AddOptions(elements_info); 

            // - действия(зависят от строения)
            ActionsChoice.ClearOptions(); // очищаем опции выбора
            List<string> actionsInfo = Building.ActionsChoiceInfo(building_name); // заносим в список через функцию все действия агрегата
            ActionsChoice.AddOptions(actionsInfo);
            ParameterInput.gameObject.SetActive(true);

            // - доступные выходы
            ExitsChoice.ClearOptions(); // очищаем опции выбора
            List<string> exitsInfo = Building.ExitsChoiceInfo(); // заносим в список через функцию все действия выхода
            ExitsChoice.AddOptions(exitsInfo);
        }
    }

    // Создание алгоритма на основе ведённых данных
    // INPUT: - (введённые данные из выпадающих списков)
    // OUTPUT: - (сохранение во все нужные переменные, отключение интерфейса агрегата и включение меню игрока)
    public void BuildAlgorithm(){
        // нахождение нужных переменных для последующего сохранения алгоритма
        if(element_ids.Count < 2){
            Debug.Log(1);
            parameters.Add(Convert.ToInt32(ParameterInput.text));
            Debug.Log(2);
            exits.Add(ExitsChoice.options[ExitsChoice.value].text);
            Debug.Log(3);
            actions.Add(ActionsChoice.options[ActionsChoice.value].text);
            Debug.Log("--");
            List<int> temp_elem_ids = new List<int>(){ElementsChoice1.value, ElementsChoice2.value};
            Debug.Log(ElementsChoice1.value);
            Debug.Log(ElementsChoice2.value);
            element_ids.Add(temp_elem_ids);
            Debug.Log("--");
            List<string> temp_elem_names = new List<string>(){ElementsChoice1.options[ElementsChoice1.value].text, ElementsChoice2.options[ElementsChoice2.value].text};
            Debug.Log(7);
            Debug.Log(element_ids.Count);
            element_names.Add(temp_elem_names);
            Debug.Log(8);
        } else {
            Debug.Log("Максимум алгоритмов достигнут");
        }
        
        // сохраняем все части алгоритма в PlayerPrefs
        // for(int i = 1; i < 3; i++){
        //     PlayerPrefs.SetInt($"{sys_building_name}ElementID{i}", element_ids[i-1]);
        //     if(element_names[i-1] != ""){
        //         PlayerPrefs.SetString($"{sys_building_name}ElementName{i}", element_names[i-1]);
        //     } else {
        //         PlayerPrefs.SetString($"{sys_building_name}ElementName{i}", "-");
        //     }
        // }
        // PlayerPrefs.SetString($"{sys_building_name}Action", action);
        // PlayerPrefs.SetString($"{sys_building_name}Exit", exit);
        // PlayerPrefs.SetInt($"{sys_building_name}Parameter", parameter);

        // составление строки алгоритма 
        if(element_ids[element_ids.Count-1][element_ids[element_ids.Count-1].Count-1] == 0){
            Debug.Log(10);
            AlgorithmText.text += $"\n{actions[actions.Count-1]} {element_names[element_names.Count-1][0]}, Параметр = {parameters[parameters.Count-1]}, Вывести в {exits[exits.Count-1]}";
            ParameterInput.text = ""; // сброс параметра в строке ввода
            ExitAgregatUI();
        } else {
            Debug.Log(11);
            AlgorithmText.text += $"\n{actions[actions.Count-1]} {element_names[element_names.Count-1][0]} и {element_names[element_names.Count-1][1]}, Параметр = {parameters[parameters.Count-1]}, Вывести в {exits[exits.Count-1]}";
            ParameterInput.text = ""; // сброс параметра в строке ввода
            ExitAgregatUI();
        }
    }

    public void DeleteAlgorithm(){
        element_ids.RemoveAt(element_ids.Count-1);
        element_names.RemoveAt(element_names.Count-1);
        parameters.RemoveAt(parameters.Count-1);
        actions.RemoveAt(actions.Count-1);
        exits.RemoveAt(exits.Count-1);
        AlgorithmText.text = "";
    }

    private void OnTriggerEnter(Collider coll){
        string element_name = coll.name.ToString().Split('(')[0]; // просмотр имени объекта (split сделан в случае такого названия - Na(Clone), которое наш алгоритм не засчитает за элемент впринципе)
        if(!elements_info.Contains(coll.name) && (!element_names[0].Contains(element_name) || !element_names[1].Contains(element_name))){ // если прикоснувшийся объект вообще не вещество или он не указан в списке элементов алгоритма
            return;
        }

        temp_storage.Add(element_name); // добавление элемента в хранилище агрегата
        Destroy(coll.gameObject); // моментальное уничтожение объекта (в данном контексте положили в агрегат)
        InputElementsText.text += $"{element_name}\n";
        string temp_action = "";
        string temp_exit = "";
        int temp_algorithm_id = -1;
        List<Dictionary<string, string>> result_element = null;
        // Условие: если в temp_storage находятся вещества, которые являются условием для начала алгоритма #1, то начинается подготовка к проведению реакции
        if(temp_storage.Equals(element_names[0])){
            temp_algorithm_id = 0;
            // добавление ID соответствующих элементов в temp_element_ids
            temp_element_ids.Add(element_ids[0][0]); 
            temp_element_ids.Add(element_ids[0][1]);
            temp_action = actions[0];
            temp_exit = exits[0];
            // пример вызова функции для получения вещества по алгоритму (строение, действие, ID элементов, параметр)
            result_element = Building.Reaction( 
                building_name, // строение
                actions[0], // действие
                temp_element_ids, // ID элементов в БД
                parameters[0] // параметр действия
            );
        // Условие: если в temp_storage находятся вещества, которые являются условием для начала алгоритма #2, то начинается подготовка к проведению реакции
        } else if (temp_storage.Equals(element_names[element_names.Count-1])){
            temp_algorithm_id = 1;
            // добавление ID соответствующих элементов в temp_element_ids
            temp_element_ids.Add(element_ids[element_ids.Count-1][0]); 
            temp_element_ids.Add(element_ids[element_ids.Count-1][1]);
            temp_action = actions[actions.Count-1];
            temp_exit = exits[exits.Count-1];
            // пример вызова функции для получения вещества по алгоритму (строение, действие, ID элементов, параметр)
            result_element = Building.Reaction( 
                building_name, // строение
                actions[actions.Count-1], // действие
                temp_element_ids, // ID элементов в БД
                parameters[parameters.Count-1] // параметр действия
            );
        }

        // если результата нет
        if(Convert.ToInt32(result_element[0]["element_id"]) == temp_element_ids[0] && temp_action != "Аннигилировать, оставить основной элемент"){
            reaction_state = "Нет такой реакции!"; // выставление соответствующего состояния агрегата
            if(temp_exit == building_name){
                Instantiate(EP.elements_prefabs[Convert.ToInt32(result_element[0]["element_id"])-1], OutputPlace.transform.position, Quaternion.identity);
            } else {
                Instantiate(EP.elements_prefabs[Convert.ToInt32(result_element[0]["element_id"])-1], Transporter.AgregatInputPlaces[temp_exit].transform.position, Quaternion.identity);
            }
            is_reacted = true;
        } else{ // если же он есть
            reaction_state = "Реакция прошла успешно!";
            foreach(Dictionary<string, string> element in result_element){
                if(Convert.ToInt32(element["element_id"]) == 0){
                    continue;
                }
                if(temp_exit == building_name){
                    Instantiate(EP.elements_prefabs[Convert.ToInt32(element["element_id"])-1], OutputPlace.transform.position, Quaternion.identity);
                } else {
                    Instantiate(EP.elements_prefabs[Convert.ToInt32(element["element_id"])-1], Transporter.AgregatInputPlaces[temp_exit].transform.position, Quaternion.identity);
                }
            }
            is_reacted = true;
        }
        InputElementsText.text = "";
        DeleteAlgorithm();        
        element_ids.RemoveAt(temp_algorithm_id);
        element_names.RemoveAt(temp_algorithm_id);
        temp_storage.Clear();
        temp_element_ids.Clear();
        temp_action = null;
        exits.RemoveAt(temp_algorithm_id);
        temp_exit = null;
        parameters.RemoveAt(temp_algorithm_id);
    } 
}

