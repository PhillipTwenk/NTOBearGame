using System;
using UnityEngine;


public class QuestClass
    {
        public void TextChanger(){
            
            // Получение значения, отображающего наше продвижение в квесте и продвижение в сообщениях

            int Progress = PlayerPrefs.GetInt("ProgressInt");
            
            //Запрос в БД для получения названия квеста и его описания
            string SmallPanelQuery = $"SELECT SmallPanel FROM Panel_Table WHERE id = '{Progress}'";
            string DetailPanelQuery = $"SELECT DetailedPanel FROM Panel_Table WHERE id = '{Progress}'";

            
            //Получение текста из БД
            
            string SmallPanelCurrentText = DBManager.ExecuteQuery(SmallPanelQuery);
            string DetailedPanelCurrentText = DBManager.ExecuteQuery(DetailPanelQuery);
            
            
            //Изменение текста в нужных панелях
            
            StaticStorage.ProgressPanelTextStatic.text = SmallPanelCurrentText;
            StaticStorage.DetailPanelTextStatic.text = DetailPanelQuery;
            StaticStorage.TextHelperQuestStatic.text = SmallPanelCurrentText;
            StaticStorage.TextHelperSTstatic.text = SmallPanelCurrentText;
            StaticStorage.TextDetialPanelSTstatic.text = DetailedPanelCurrentText;

            StaticStorage.HCReferenceStatic.StartCoroutineFadeMessageNewQuest();
            HelperController.IsMessageCoroutineBreak = true;
            //Отправление сообщений от профессора
            SendQuestMessage(Progress);
        }

        //Отправка сообщений, если на текущем квесте это необходимо
        public void SendQuestMessage(int progress){

            //Получение информации о том, нужно ли отправлять сообщение на текущем квесте

            string QueryBoolMessage = $"SELECT AreThereMessages FROM Panel_Table WHERE id = '{progress}'";
            bool AreThereMessagesOnTheQuest = bool.Parse(DBManager.ExecuteQuery(QueryBoolMessage));

            //Проверка

            if(AreThereMessagesOnTheQuest){

                //Получение информации о том, какой номер прогресса сообщений в текущем квесте

                string MessageQueryNumber = $"SELECT MessageNumber FROM QuestsWithMessages WHERE QuestNumber = '{progress}'";

                //Запуск корутины с отправкой сообщений
                StaticStorage.ChatSystemRefStatic.StartCoroutineMethod(int.Parse(DBManager.ExecuteQuery(MessageQueryNumber)));
            }
        }
        
        //Cnарт нового квеста
        public void StartNewQuest(int ActiveQuest){
            
            //Обновлене состояния квеста на следующий
            PlayerPrefs.SetInt("ProgressInt", ActiveQuest + 1);
            
            //Изменение текста в панелях
            TextChanger();
            
            Debug.Log($"{ActiveQuest} квест выполнен! ");
        }
        
        //Проверка на текущий квест
        public void CheckQuest(int CurrentQuestStage)
        {
            //Если нынешний квест равен указаному в параметре метода, запускается новый квест
            if (PlayerPrefs.GetInt("ProgressInt") == CurrentQuestStage && !TutorialClass.IsInTutorial)
            {
                StartNewQuest(CurrentQuestStage);
            }
        }
    }


