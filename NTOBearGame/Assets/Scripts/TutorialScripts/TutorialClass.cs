using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using System;

public class TutorialClass: MonoBehaviour
{
    //Обычные переменные
    private int MaxValueTutorial;

    //Статические переменные
    public static bool IsInTutorial;
    public static int TutorialCounter;
    public static bool IsTextingMessage;
    public static bool IsTextingMessageBreak;
    public static bool IsNotEnterContinue;
    
    //Игровые объекты
    public GameObject PreTutorialPanel, CanvasTutorial, ProfTutorialPanel, TMProHint, ForObjects, FirstParentObject, NewAgregatObject, ShadowPanel;

    //Экземпляры классов
    private QuestClass QuestClassInstance;

    //Массивы / Динамические массивы
    public List<RectTransform> PositionsProfPanelList;
    public List<GameObject> ObjectsDuringTutorial;
    public List<Outline> BuildingsOutine;




    private void Start()
    {
        QuestClassInstance = new QuestClass();
        MaxValueTutorial = 50;
        IsNotEnterContinue = false;
    }

    //Метод для обновления состояния туториала ( при нажатии Enter появляется новый текст )
    public void UpdateTutorialStage()
    {
        if (IsInTutorial)
        {
            TutorialCounter += 1;
            InputNewTextInProfTutorial();
        }
    }
    
    //Метод для запроса данных из БД для получения текста и запуск корутины печатания текста
    private void InputNewTextInProfTutorial()
    {
        if (IsInTutorial)
        {
            string TutorialTextQuery;
            string TutorialTextOnThisStage;
        
        
            //Если сделать запрос больше количество фраз в БД, выдаст ошибку
            if (TutorialCounter <= MaxValueTutorial)
            {
                TutorialTextQuery = $"SELECT Phrase FROM Tutorial_Phrases WHERE id = '{TutorialCounter}'";
                TutorialTextOnThisStage = DBManager.ExecuteQuery(TutorialTextQuery);
            }
            else
            {
                TutorialTextOnThisStage = "";
            }

            //Перемещение панели
            MoveProfPanel();

            //Активация нужных UI элементов
            ThisUI();

            //Активация подсказки, если это нужно
            HintActivate();

            //Включение подсветки агрегатов
            OutlineTutorial();
            
            //Включает / выключает затемнение
            ShadowControl();
        
            //Обнуление перед следующим вводом текста
            StaticStorage.TextMProTutorialStatic.text = "";
        
        
            //Запуск корутины ввода текста
            StartCoroutine(TextInputCoroutine(TutorialTextOnThisStage));
        }
        
    }
    
    //Корутина для "Анимации" печатания текста на панели туториала
    private IEnumerator TextInputCoroutine(string text)
    {
        IsTextingMessage = true;
        foreach (char letter in text)
        {
            StaticStorage.TextMProTutorialStatic.text += letter;
            
            //При повторном нажатии Enter текст заполняется полностью
            if (IsTextingMessageBreak && IsTextingMessage)
            {
                StaticStorage.TextMProTutorialStatic.text = text;
                IsTextingMessageBreak = false;
                break;
            }
            yield return new WaitForFixedUpdate();
        }
        IsTextingMessage = false;
    }

    //Метод для запуска туториала
    public void StartTutorial()
    {
        IsInTutorial = true;
        PreTutorialPanel.SetActive(false);
        ProfTutorialPanel.SetActive(true);
        TutorialCounter = 1;
        BuildingObject.usingBuildings = false;
        AddElementsToItem();
        InputNewTextInProfTutorial();
    }

    //Метод для окончания туториала
    public void EndTutorial(bool IsOnPreTutorialPanel)
    {
        IsInTutorial = false;
        TutorialCounter = 1;
        PreTutorialPanel.SetActive(false);
        ProfTutorialPanel.SetActive(false);
        CanvasTutorial.SetActive(false);
        Destroy(NewAgregatObject);
        BuildingObject.usingBuildings = false;
        
        //Если мы находимся на панели, где нас спрашивают о прохождении туториала, и нажимаем нет
        if (IsOnPreTutorialPanel)
        {
            QuestClassInstance.TextChanger();
        }
    }
    
    //Метод, перемещающий панель в нужные координаты в зависимости от квеста
    private void MoveProfPanel()
    {
        //Запрос БД о том в каком месте должна находиться панель на данном этапе туториала
        string PositionOnCurrentStageTutorialQuery 
            = $"SELECT PositionInList FROM Tutorial_Phrases WHERE id = '{TutorialCounter}'";
        int PositionOnCurrentStageTutorial = int.Parse(DBManager.ExecuteQuery(PositionOnCurrentStageTutorialQuery));
        
        //Перемещение панели
        ProfTutorialPanel.GetComponent<RectTransform>().position =
            PositionsProfPanelList[PositionOnCurrentStageTutorial].position;
    }
    
    //Методы для продолжения туториала при нажатии какой - либо кнопки
    public void ContinueTutorial(int WhichStage)
    {
        if (TutorialCounter == WhichStage && IsNotEnterContinue && IsInTutorial)
        {
            IsNotEnterContinue = false;
            UpdateTutorialStage();
            TMProHint.SetActive(false);
        }
    }
    
    //Метод для активирования панели подсказки, когда это неоюходимо
    private void HintActivate()
    {
        //Запрос в БД, чтобы узнать нужно ли и какую фразу нужно вывести в подсказки
        string IsThereActivateRequirementQuery = $"SELECT ActivateRequirement FROM Tutorial_Phrases WHERE id = '{TutorialCounter}'";
        int IsThereActivateRequirement = int.Parse(DBManager.ExecuteQuery(IsThereActivateRequirementQuery));
        
        //Если больше нуля, значит фраза есть
        if (IsThereActivateRequirement > 0)
        {
            //Делаем запрос для получения фразы 
            string WhichPhraseQuery = $"SELECT Hint FROM Hints_Table WHERE id = '{IsThereActivateRequirement}'";
            string WhichPhrase = DBManager.ExecuteQuery(WhichPhraseQuery);
            TMProHint.SetActive(true);
            TMProHint.GetComponent<TextMeshProUGUI>().text = WhichPhrase;
            IsNotEnterContinue = true;
            Debug.Log(TutorialClass.IsNotEnterContinue);
            if (TutorialCounter == 36)
            {
                Building.is_agregat_canvas_activated = false;
            }
        }
    }
    
    //Метод для добавления тестовых элементов в инвентрарь
    public static void AddElementsToItem()
    {
        string empty_slot_idH = DBManager.ExecuteQuery($"SELECT MIN(slot_id) FROM inventory WHERE element_id = 0");
        DBManager.ExecuteQueryWithoutAnswer($"UPDATE inventory SET element_id = 4 WHERE slot_id = {Convert.ToInt32(empty_slot_idH)}"); 
        Inventory.is_changed = true;
        
        string empty_slot_idO = DBManager.ExecuteQuery($"SELECT MIN(slot_id) FROM inventory WHERE element_id = 0");
        DBManager.ExecuteQueryWithoutAnswer($"UPDATE inventory SET element_id = 5 WHERE slot_id = {Convert.ToInt32(empty_slot_idO)}"); 
        Inventory.is_changed = true;
    }
    
    //Метод для подсвечивания нужных UI объектов в определённые моменты туториала
    public void ThisUI()
    {
        //Какой UI деактивировать
        string WhichUIDeActivateQuery;
        int WhichUIDeActivate;
        if (TutorialCounter > 1)
        {
            WhichUIDeActivateQuery = $"SELECT WhichUIActivate FROM Tutorial_Phrases WHERE id = '{TutorialCounter - 1}'";
            WhichUIDeActivate = int.Parse(DBManager.ExecuteQuery(WhichUIDeActivateQuery));
        }
        else
        {
            WhichUIDeActivate = 0;
        }

        //Деактивация
        if (WhichUIDeActivate > 0)
        {
            Debug.Log(WhichUIDeActivate);
            ObjectsDuringTutorial[WhichUIDeActivate - 1].transform.SetParent(FirstParentObject.transform);
        }
        
        //Запрос в БД, чтобы узнать нужно ли и какой UI элемент нужно подсветить
        string WhichUIActivateQuery = $"SELECT WhichUIActivate FROM Tutorial_Phrases WHERE id = '{TutorialCounter}'";
        int WhichUIActivate = int.Parse(DBManager.ExecuteQuery(WhichUIActivateQuery));

        //Какой UI деактивировать
        if (WhichUIActivate > 0)
        {
            FirstParentObject = ObjectsDuringTutorial[WhichUIActivate - 1].transform.parent.gameObject;
            Debug.Log(FirstParentObject.name);
            ObjectsDuringTutorial[WhichUIActivate - 1].transform.SetParent(ForObjects.transform);
        }
    }

    //Подсвечивает агрегаты когда это нужно
    private void OutlineTutorial()
    {
        int minValue = 16;
        int maxValue = 20;
        if (TutorialCounter >= minValue && TutorialCounter <= maxValue)
        {
            string OutlineListQuery = $"SELECT WhichItem FROM OutlineTutorialTable WHERE StageNumber = '{TutorialCounter}'";
            int OutlineListItem = int.Parse(DBManager.ExecuteQuery(OutlineListQuery));

            BuildingsOutine[OutlineListItem].enabled = true;
            for (int i = 0; i < BuildingsOutine.Count; i++)
            {
                if (i == OutlineListItem)
                {
                    continue;
                }
                BuildingsOutine[i].enabled = false;
            }
        }
        if (TutorialCounter == 21)
        {
            BuildingsOutine[4].enabled = false;
        }
    }
    
    //Включает / выключает затемнение
    private void ShadowControl()
    {
        string IsThereShadowQuery = $"SELECT IsThereShadow FROM Tutorial_Phrases WHERE id = '{TutorialCounter}'";
        bool IsThereShadow = bool.Parse(DBManager.ExecuteQuery(IsThereShadowQuery));

        ShadowPanel.SetActive(IsThereShadow);
    }
    
}
