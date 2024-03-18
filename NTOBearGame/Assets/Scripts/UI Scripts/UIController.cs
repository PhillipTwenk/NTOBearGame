using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIController : MonoBehaviour
{
    private TextMeshProUGUI AntidoteTextMPro;
    public TextMeshProUGUI TextTeleportButton;
    
    public GameObject AntidotePanelObj;
    public GameObject BriefcaseObj;
    public GameObject BearOSPanel;
    public GameObject DetailPanelObj;
    public GameObject ProgressPanel;
    public GameObject PauseMenu;
    
    public Transform characterPosition;
    public Transform CameraPosition;
    public Transform[] CheckPointArrayPosition;
    public Transform HomeCheckPoint;
    
    private QuestClass QuestClassInstance;
    private void Start()
    {
        AntidoteTextMPro = AntidotePanelObj.transform.GetChild(1).GetComponent<TextMeshProUGUI>();
        QuestClassInstance = new QuestClass();
    }
    void Update()
    {
        
        //Открытие инвентаря
        if(Input.GetKeyDown(KeyCode.E))
        {
            BriefcaseButtonOpen();
        }
        
        //Рабочая панель
        if(Input.GetKeyDown(KeyCode.Q))
        {
            BearOS();
            AntidotePanel();
        }
        
        //Пауза
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            PauseButton();
        }

        //ЧИТЫЫЫЫ
        if (Input.GetKeyDown(KeyCode.M))
        {
            Debug.Log("ИСПОЛЬЗОВАНЫ ЧИТ КОДЫ!!!!!!!!!!!");
            QuestClassInstance.StartNewQuest(PlayerPrefs.GetInt("ProgressInt"));
        }

        //Проверка на нажатие кнопки Enter во время туториала
        if (Input.GetKeyDown(KeyCode.Return) && TutorialClass.IsInTutorial  && !TutorialClass.IsNotEnterContinue)
        {
            if (TutorialClass.IsTextingMessage)
            {
                TutorialClass.IsTextingMessageBreak = true;
            }
            else if(!TutorialClass.IsNotEnterContinue){
                    StaticStorage.TutorialClassStatic.UpdateTutorialStage();
                }
        }
    }
    #region Buttons Methods
    
    //Выполняется при нажатии на паузу(кнопка esc)
    public void PauseButton(){
        if (StaticStorage.IsPause)
        {
            Time.timeScale = 1f;
        }
        else
        {
            Time.timeScale = 0f;
        }
        PauseMenu.SetActive(!PauseMenu.activeSelf);
        StaticStorage.IsPause = !StaticStorage.IsPause;
        if(BearOSPanel.activeSelf){
            BearOSPanel.SetActive(!BearOSPanel.activeSelf);
        }
        ProgressPanel.SetActive(true);
    }
    
    //Активация/Дезактивация панели BearOS
    public void BearOS(){
        BearOSPanel.SetActive(!BearOSPanel.activeSelf);
        if(DetailPanelObj.activeSelf){
            DetailPanelObj.SetActive(!ProgressPanel.activeSelf);
        }
        if(BriefcaseObj.activeSelf){
            BriefcaseObj.SetActive(!BriefcaseObj.activeSelf);
        }
        ProgressPanel.SetActive(!ProgressPanel.activeSelf);
        
        Building.is_agregat_canvas_activated = !Building.is_agregat_canvas_activated;
        
        QuestClassInstance.CheckQuest(1);
        QuestClassInstance.CheckQuest(7);
        
        StaticStorage.TutorialClassStatic.ContinueTutorial(4);
        StaticStorage.TutorialClassStatic.ContinueTutorial(14);
        StaticStorage.TutorialClassStatic.ContinueTutorial(31);
        StaticStorage.TutorialClassStatic.ContinueTutorial(35);
    }

    //Метод, отвечающий за Изменение антидота на соответствующей панели
    public void AntidotePanel()
    {
        if (PlayerPrefs.GetInt("ProgressInt") < 19)
        {
            AntidoteTextMPro.text = "NaOCl";
        }
        else
        {
            AntidoteTextMPro.text = "Na₂S₂O₃";
        }
    }

    //Активация / деактивация панели инвентаря
    public void BriefcaseButtonOpen(){
        BriefcaseObj.SetActive(!BriefcaseObj.activeSelf);
        if(BearOSPanel.activeSelf){
            BearOSPanel.SetActive(!BearOSPanel.activeSelf);
        }
        if (!TutorialClass.IsInTutorial)
        {
            Building.is_agregat_canvas_activated = !Building.is_agregat_canvas_activated;
        }
        
        StaticStorage.TutorialClassStatic.ContinueTutorial(22);
        StaticStorage.TutorialClassStatic.ContinueTutorial(26);
        StaticStorage.TutorialClassStatic.ContinueTutorial(29);
    }
    
    //Метод, отвечающий за телепортацию
    public void TeleportMethod(){
        if (!TutorialClass.IsInTutorial)
        {
            int CPNumber = PlayerPrefs.GetInt("CPNumber");
            if (StaticStorage.IsInLab){
                Vector3 newPositionCharacter = new Vector3(CheckPointArrayPosition[CPNumber].position.x, CheckPointArrayPosition[CPNumber].position.y, CheckPointArrayPosition[CPNumber].position.z + 5);
                characterPosition.position = newPositionCharacter;
                TextTeleportButton.text = "В лабораторию";
                switch (CPNumber)
                {
                    // Телепортировались в 1 безопасную зону [квест 2]
                    case 0:
                       QuestClassInstance.CheckQuest(2);
                       StaticStorage.IsInZone = true;
                       StaticStorage.IsInLab = false;
                       MusicController.StartMusicInZone(); 
                       break;

                    // Телепортировались обратно в комнату [квест 11, 20]
                    case 1:
                        QuestClassInstance.CheckQuest(11);
                        StaticStorage.IsInZone = true;
                        StaticStorage.IsInLab = false;
                        MusicController.StartMusicInZone();
                    break;
                    case 2:
                        StaticStorage.IsInZone = true;
                        StaticStorage.IsInLab = false;
                        MusicController.StartMusicInZone();
                    break;
                    case 3:
                        QuestClassInstance.CheckQuest(20);
                        StaticStorage.IsInZone = true;
                        StaticStorage.IsInLab = false;
                        MusicController.StartMusicInZone();
                    break;
                    case 4:
                        StaticStorage.IsInZone = true;
                        StaticStorage.IsInLab = false;
                        MusicController.StartMusicInZone();
                    break;
                    case 5:
                       StaticStorage.IsInZone = true;
                       StaticStorage.IsInLab = false; 
                       MusicController.StartMusicInZone();
                    break;
                }
            }
            else{
                Vector3 newPositionCharacter = new Vector3(HomeCheckPoint.position.x, HomeCheckPoint.position.y, HomeCheckPoint.position.z + 5);
                characterPosition.position = newPositionCharacter;
                TextTeleportButton.text = "В Контрольную точку";
                QuestClassInstance.CheckQuest(6);
                QuestClassInstance.CheckQuest(18);
                QuestClassInstance.CheckQuest(27);
                StaticStorage.IsInZone = false;
                StaticStorage.IsInLab = true;
                MusicController.StartMusicInLab();
            }
            Vector3 newCamPosition = new Vector3(characterPosition.position.x, characterPosition.position.y, characterPosition.position.z);
            CameraPosition.position = newCamPosition;
            Debug.Log(StaticStorage.IsInLab);

            BearOSPanel.SetActive(false);
            ProgressPanel.SetActive(true);
            DetailPanelObj.SetActive(false);

            Debug.Log(CPNumber);
        }
        
    }
    #endregion
}
