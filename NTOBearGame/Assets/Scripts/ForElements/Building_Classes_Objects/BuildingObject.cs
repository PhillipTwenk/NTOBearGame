using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
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
    public static bool is_canvas_activated = false; // Состояние канваса
    public static bool usingBuildings = true; // Можно ли использовать агрегаты

    // ОБЩИЕ МЕТОДЫ
    // - Пояснения к комментам:
    //     INPUT: параметры функция (*param - необязательный параметр)
    //     OUTPUT: то, что возвращает функция
    // - Методы указаны в том порядке, в котором они (в основном) будут использоваться в коде

    // Переменные
    [SerializeField] string building_name; // Название агрегата в интерфейсе (рус., задаётся в каждом агрегате своё значение из БД)
    [SerializeField] string sys_building_name; // Название агрегата для операций внутри (англ., задаётся в каждом агрегате своё значение из БД)
    private string action; // Действие алгоритма
    private string exit; // Конечная точка алгоритма, куда отправить итог (если указан текущий агрегат - вывод сразу рядом с ним)
    private string reaction_state; // Статус реакции
    private int parameter = 0; // Параметр алгоритма
    private float timer; 
    private bool is_reacted = false;
    public List<int> element_ids; // Используемые в алгоритме ID элементов из базы (если 0 -> вещества нет)
    public List<string> element_names; // Используемые в алгоритме имена веществ из базы (если "" -> вещества нет)
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
    [SerializeField] TMP_Text AlgorithmText; // Текст алгоритма в UI
    [SerializeField] GameObject PlayerMenu; // Основное меню игрока (отключаем при открытии канваса агрегата )
    [SerializeField] TMP_Text AgregatName; // Надпись (название алгоритма) в UI
    [SerializeField] TMP_Text ReactionStateText; // Надпись над объектом(состояние алгоритма)
    [SerializeField] TMP_Text InputElementsText; // Надпись над объектом(подающиеся элементы)
    [SerializeField] Outline outline;
    
    // Запуск при появлении на сцене
    // INPUT: -
    // OUTPUT: - (сброс предыдущих алгоритмов и отключение канваса)
    void Start()
    {
        Transporter.AgregatInputPlaces[building_name] = InputPlace;
        // обнуляем сохраненный алгоритм до этого
        DeleteAlgorithm();
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
            CheckChosenAction(); // сохраняем в action текущее действие
            if(action != "" && (ElementsChoice1.value != 0 && ElementsChoice2.value != 0) && ExitsChoice.options[ExitsChoice.value].text != ""){ // если алгоритм есть
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

        if(!Building.is_agregat_canvas_activated){
            ExitAgregatUI();
        }
    }
    private void OnMouseEnter(){
        if (usingBuildings)
        {
            outline.enabled = true; // если мы навелись на объект
        }
    }

    private void OnMouseExit(){
        if (usingBuildings)
        {
            outline.enabled = false; // если мы навелись на объект
        }
    }
    // Нажатие на агрегат -> активация интерфейса агрегата (отключение основного интерфейса игрока) выставление в выпадающие списки нужные значения
    // INPUT: - (клик по мыши на агрегат)
    // OUTPUT: - (все нужные значения в выпадающих списках из БД)
    void OnMouseDown(){
        if(!Building.is_agregat_canvas_activated && !is_canvas_activated && usingBuildings){
            
            if(TutorialClass.IsInTutorial){
                StaticStorage.TutorialClassStatic.ContinueTutorial(36);
                StaticStorage.TutorialClassStatic.ContinueTutorial(47);
            }

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
        element_ids.Clear();
        element_names.Clear();

        parameter = Convert.ToInt32(ParameterInput.text);
        exit = ExitsChoice.options[ExitsChoice.value].text;
        element_ids.Add(ElementsChoice1.value);
        element_ids.Add(ElementsChoice2.value);
        element_names.Add(ElementsChoice1.options[ElementsChoice1.value].text);
        element_names.Add(ElementsChoice2.options[ElementsChoice2.value].text);

        // сохраняем все части алгоритма в PlayerPrefs
        for(int i = 1; i < 3; i++){
            PlayerPrefs.SetInt($"{sys_building_name}ElementID{i}", element_ids[i-1]);
            if(element_names[i-1] != ""){
                PlayerPrefs.SetString($"{sys_building_name}ElementName{i}", element_names[i-1]);
            } else {
                PlayerPrefs.SetString($"{sys_building_name}ElementName{i}", "-");
            }
        }
        PlayerPrefs.SetString($"{sys_building_name}Action", action);
        PlayerPrefs.SetString($"{sys_building_name}Exit", exit);
        PlayerPrefs.SetInt($"{sys_building_name}Parameter", parameter);
        ParameterInput.text = ""; // сброс параметра в строке ввода

        // составление строки алгоритма 
        if(element_ids[1] == 0){
            AlgorithmText.text = $"{action} {element_names[0]}, Параметр = {parameter}, Вывести в {exit}";
        } else {
            AlgorithmText.text = $"{action} {element_names[0]} и {element_names[1]}, Параметр = {parameter}, Вывести в {exit}";
        }

        ExitAgregatUI();
    }

    // Сохранение действия при выборе
    // INPUT: - (выход из выпадающего списка после выбора)
    // OUTPUT: - (сохранение действия в action и в зависимости от него включение параметра)
    public void CheckChosenAction(){ // проверка при выборе действия (всегда в одном порядке: 1 - с параметром; 2 - без параметра; 3 - удалить остатки)
        action = ActionsChoice.options[ActionsChoice.value].text;
    }

    // Сохранение результата параметра
    // INPUT: - (выход из строки ввода после её заполнения)
    // OUTPUT: - (сохранение в параметр значение строки ввода)
    public void ParameterInputResult(){
        parameter = Convert.ToInt32(ParameterInput.text);
    }

    public void DeleteAlgorithm(){
        for(int i = 1; i < 3; i++){
            PlayerPrefs.DeleteKey($"{sys_building_name}ElementName{i}");
            PlayerPrefs.DeleteKey($"{sys_building_name}ElementID{i}");
        }

        PlayerPrefs.DeleteKey($"{sys_building_name}Parameter");
        PlayerPrefs.DeleteKey($"{sys_building_name}Action");
        PlayerPrefs.DeleteKey($"{sys_building_name}Exit");
        AlgorithmText.text = "";

        parameter = 0;
        action = "";
    }

    private void OnTriggerEnter(Collider coll){
        string element_name = coll.name.ToString().Split('(')[0]; // просмотр имени объекта (split сделан в случае такого названия - Na(Clone), которое наш алгоритм не засчитает за элемент впринципе)
        if(!elements_info.Contains(coll.name) || !element_names.Contains(element_name)){ // если прикоснувшийся объект вообще не вещество или он не указан в списке элементов алгоритма
            return;
        }

        temp_storage.Add(element_name); // добавление элемента в хранилище агрегата
        Destroy(coll.gameObject); // моментальное уничтожение объекта (в данном контексте положили в агрегат)
        InputElementsText.text += $"{element_name}\n";

        // Условие: если в temp_storage находятся вещества, которые являются условием для начала алгоритма, то начинается подготовка к проведению реакции
        if((temp_storage.Contains(PlayerPrefs.GetString($"{sys_building_name}ElementName1")) || PlayerPrefs.GetString($"{sys_building_name}ElementName1") == "-") && (temp_storage.Contains(PlayerPrefs.GetString($"{sys_building_name}ElementName2")) || PlayerPrefs.GetString($"{sys_building_name}ElementName2") == "-") ){
            // добавление ID соответствующих элементов в temp_element_ids
            temp_element_ids.Add(PlayerPrefs.GetInt($"{sys_building_name}ElementID1")); 
            temp_element_ids.Add(PlayerPrefs.GetInt($"{sys_building_name}ElementID2"));

            // пример вызова функции для получения вещества по алгоритму (строение, действие, ID элементов, параметр)
            List<Dictionary<string, string>> result_element = Building.Reaction( 
                building_name, // строение
                PlayerPrefs.GetString($"{sys_building_name}Action"), // действие
                temp_element_ids, // ID элементов в БД
                parameter=PlayerPrefs.GetInt($"{sys_building_name}Parameter") // параметр действия
            );


            // если результата нет
            if(Convert.ToInt32(result_element[0]["element_id"]) == temp_element_ids[0] && PlayerPrefs.GetString($"{sys_building_name}Action") != "Аннигилировать, оставить основной элемент"){
                reaction_state = "Нет такой реакции!"; // выставление соответствующего состояния агрегата
                if(exit == building_name){
                    Instantiate(EP.elements_prefabs[Convert.ToInt32(result_element[0]["element_id"])-1], OutputPlace.transform.position, Quaternion.identity);
                } else {
                    Instantiate(EP.elements_prefabs[Convert.ToInt32(result_element[0]["element_id"])-1], Transporter.AgregatInputPlaces[PlayerPrefs.GetString($"{sys_building_name}Exit")].transform.position, Quaternion.identity);
                }
                is_reacted = true;
            } else{ // если же он есть
                reaction_state = "Реакция прошла успешно!";
                foreach(Dictionary<string, string> element in result_element){
                    if(Convert.ToInt32(element["element_id"]) == 0){
                        continue;
                    }
                    
                    if(exit == building_name){
                        Instantiate(EP.elements_prefabs[Convert.ToInt32(element["element_id"])-1], OutputPlace.transform.position, Quaternion.identity);
                    } else {
                        Instantiate(EP.elements_prefabs[Convert.ToInt32(element["element_id"])-1], Transporter.AgregatInputPlaces[PlayerPrefs.GetString($"{sys_building_name}Exit")].transform.position, Quaternion.identity);
                    }
                }
                is_reacted = true;
            }
            InputElementsText.text = "";
            DeleteAlgorithm();        
            element_ids.Clear();
            element_names.Clear();
            temp_storage.Clear();
            temp_element_ids.Clear();
        } 
    }

}
