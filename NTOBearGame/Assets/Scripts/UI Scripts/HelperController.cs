using System.Collections;
using TMPro;
using UnityEngine;

public class HelperController : MonoBehaviour
{
    private bool IsQuestMessageExist;
    [SerializeField] private GameObject HelperEmptyForActivation;
    [SerializeField] private GameObject SmallPanel;
    [SerializeField] private GameObject BearOSObject;
    [SerializeField] private GameObject CaseMenuObject;
    [SerializeField] private GameObject EmptyTextsMeshPro;
    [SerializeField] private GameObject GLPanel;
    [SerializeField] private TextMeshPro TMProMessage;
    [SerializeField] private Transform MainCharacterTransform;
    [SerializeField] private Transform HeadHelperTransform;
    [SerializeField] private TextMeshProUGUI TextLabel;
    [SerializeField] private TextMeshProUGUI GLText;
    public static bool IsMessageCoroutineBreak;
    private Outline HelperOutline;

    private void Start()
    {
        IsQuestMessageExist = true;
        HelperOutline = gameObject.transform.GetChild(2).GetComponent<Outline>();
        HelperOutline.enabled = false;
        StartCoroutine(CoroutineRandomMessage());
    }

    private void Update()
    {
        //Стубуретка смотрит на игрока
        
        HeadHelperTransform.LookAt(MainCharacterTransform);
    }
    
    //Включение / отключение обводки при навведении / убирании курсора мыши
    private void OnMouseEnter()
    {
        HelperOutline.enabled = true;
    }
    
    private void OnMouseExit()
    {
        HelperOutline.enabled = false;
    }
    
    //Нажимаем на Стубуретку
    private void OnMouseDown()
    {
        StartWorkWithST();
    }
    
    //Выходим из меню Стубуретки
    public void ReturnButton()
    {
        IsMessageCoroutineBreak = false;
        IsQuestMessageExist = false;
        HelperEmptyForActivation.SetActive(false);
        SmallPanel.SetActive(true);
        EmptyTextsMeshPro.SetActive(false);
        StartCoroutine(CoroutineRandomMessage());
    }

    //Включение панели Стубуретки
    public void StartWorkWithST()
    {
        HelperEmptyForActivation.SetActive(true);
        SmallPanel.SetActive(false);
        BearOSObject.SetActive(false);
        CaseMenuObject.SetActive(false);
        EmptyTextsMeshPro.SetActive(false);
    }

    //Корутина мигания сообщений 
    private IEnumerator CoroutineFadeMessageNewQuest()
    {
        for (int i = 0; i < 6; i++)
        {
            if (i % 2 == 0)
            {
                EmptyTextsMeshPro.SetActive(false);
            }

            if (i % 2 != 0)
            {
                EmptyTextsMeshPro.SetActive(true);
            }
            yield return new WaitForSeconds(0.5f);
        }
    }

    //Метод для запуска корутины мигающий сообщений
    public void StartCoroutineFadeMessageNewQuest()
    {
        EmptyTextsMeshPro.SetActive(true);
        IsMessageCoroutineBreak = true;
        IsQuestMessageExist = true;
        StartCoroutine(CoroutineFadeMessageNewQuest());
    }
    
    //Корутина для рандомных сообщений
    private IEnumerator CoroutineRandomMessage()
    {
        while (!IsMessageCoroutineBreak && !IsQuestMessageExist)
        {
            string MessageHelperQuery = $"SELECT Message FROM RandomHelperMessage WHERE id = '{Random.Range(1, 10)}'";
            string MessageHelper = DBManager.ExecuteQuery(MessageHelperQuery);
            
            if (IsMessageCoroutineBreak)
            { 
                break;
            }
            TMProMessage.text = MessageHelper;

            yield return new WaitForSeconds(Random.Range(1f, 3f));
        }
    }
    
    //Метод кнопки перехода в глоссарий
    public void MoveToGlButton()
    {
        GLPanel.SetActive(true);
        HelperEmptyForActivation.SetActive(false);
    }
    
    //Кнопка перехода обратно в меню Стубура
    public void ButtonGLBack()
    {
        GLPanel.SetActive(false);
        HelperEmptyForActivation.SetActive(true);
    }
    
    //Метод для заполнения глоссария
    public void DropdownMethod()
    {
        switch (TextLabel.text)
        {
            case "Лаборатория медведя":
                string QueryLab = $"SELECT Text FROM GLTable WHERE id = 1";
                GLText.text = DBManager.ExecuteQuery(QueryLab);
                break;
            case "CТУБУР":
                string QuerySTUBUR = $"SELECT Text FROM GLTable WHERE id = 2";
                GLText.text = DBManager.ExecuteQuery(QuerySTUBUR);
                break;
            case "BearOS":
                string QueryOS = $"SELECT Text FROM GLTable WHERE id = 3";
                GLText.text = DBManager.ExecuteQuery(QueryOS);
                break;
        }
    }
    
    
}
