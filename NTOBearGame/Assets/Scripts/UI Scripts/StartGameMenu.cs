using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class StartGameMenu : MonoBehaviour
{
    //Объект содержащий панель для запроса прохождения туториала + Холст туториала
    public GameObject PreTutorialPanel, CanvasTutorial;
    
    //Получение объектов холстов
    public GameObject CanvasMain, CanvasMenus, CanvasStartGame, Buildings;

    //Ccылка на скрипт CameraController
    public CameraController CameraControllerScriptReference;

    //Ccылка на скрипт QuestClass
    private QuestClass QuestClassInstance;

    void Start()
    {
        QuestClassInstance = new QuestClass();
        CanvasStartGame.SetActive(true);
        CanvasMain.SetActive(false);
        CanvasMenus.SetActive(false);
        Buildings.SetActive(false);
        StaticStorage.IsInStartMenu = true;
        MusicController.StartMusicInStartGame();
    }

    //Нажатие на кнопку старта
    public void ClickStartButton(){

        //Запуск стартовой анимации

        CameraControllerScriptReference.StartCameraMovement();


        //Отключение стартового  UI

        gameObject.SetActive(false);
        CanvasStartGame.SetActive(false);
    }

    //Выполняется после стартовой анимации
    public void AfterStartAnimation(){

        //Перенос камеры в лабораторию(метод из скрипта CameraController)

        CameraControllerScriptReference.MovingCameraToLab();


        //Включение основого интерфейса

        CanvasMain.SetActive(true);
        CanvasMenus.SetActive(true);
        Buildings.SetActive(true);


        StaticStorage.IsInStartMenu = false;


        //Запуск музыкальной темы в лаборатории

        MusicController.StartMusicInLab();

        if (PlayerPrefs.GetInt("FirstIntrance") == 0)
        {
            //Активирование панели, содержащей запрос прохождения туториала
            
            BuildingObject.usingBuildings = false;
            CanvasTutorial.SetActive(true);
            PreTutorialPanel.SetActive(true);
        }
        else
        {
            //Запуск сообщений от профессора
            
            QuestClassInstance.TextChanger();
        }
    }

    //Выполняется при нажатии кнопки "Выход"
    public void ClickQuitButton(){

        //Выход из приложения

        Application.Quit();
    }
}
