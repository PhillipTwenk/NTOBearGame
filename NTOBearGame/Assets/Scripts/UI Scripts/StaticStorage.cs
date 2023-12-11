using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class StaticStorage : MonoBehaviour
{
    public static Text ProgressPanelTextStatic;
    public static TextMeshProUGUI DetailPanelTextStatic;
    public Text ProgressPanelText;
    public TextMeshProUGUI DetailPanelText;
    public static bool isChatRead;
    public GameObject textObjectB, textObjectL;
    public GameObject TextingMessageAnimationObj;
    public static GameObject TextingMessageAnimationObjStatic;
    public static GameObject textObjectPrefabB, textObjectPrefabL;
    public static string[] AllMessagesArray;
    public ChatSystem ChatSystemRef;
    public static ChatSystem ChatSystemRefStatic;
    public static bool IsInStartMenu;
    public static bool IsInLab;
    public static bool IsInZone;

    public static AudioSource MusicInLabStatic;
    public static AudioSource MusicInZoneStatic;
    public static AudioSource MusicInStartGameMenuStatic;
    public AudioSource MusicInLab;
    public AudioSource MusicInZone;
    public AudioSource MusicInStartGameMenu;
    public AudioClip HitHammer;
    public AudioClip ThornHit;
    public AudioClip NewMessage;
    public AudioClip Walking;
    public AudioClip Running;
    public static AudioClip HitHammerStatic;
    public static AudioClip ThornHitStatic;
    public static AudioClip NewMessageStatic;
    public static AudioClip WalkingStatic;
    public static AudioClip RunningStatic;
    void Start()
    {
        Walking = WalkingStatic;
        Running = RunningStatic;
        HitHammer = HitHammerStatic;
        ThornHit = ThornHitStatic;
        NewMessage = NewMessageStatic;
        MusicInLabStatic = MusicInLab;
        MusicInZoneStatic = MusicInZone;
        MusicInStartGameMenuStatic = MusicInStartGameMenu;
        IsInStartMenu = true;
        IsInLab = false;
        IsInZone = false;
        ChatSystemRefStatic = ChatSystemRef;
        AllMessagesArray = new string[47];
        AssigningValues();
        TextingMessageAnimationObjStatic = TextingMessageAnimationObj;
        ProgressPanelTextStatic = ProgressPanelText;
        DetailPanelTextStatic = DetailPanelText;
        textObjectPrefabB = textObjectB;
        textObjectPrefabL = textObjectL;
    }
    private void AssigningValues()
    {
        AllMessagesArray[0] = "Здравствуй, Борислав! Ты как всегда в лаборатории? Трудишься не покладая рук. Но пришло время обсудить наше новое задание";
        AllMessagesArray[1] = "Как ты помнишь, мы медленно, но верно двигаемся к созданию эликсира, который изменит жизнь и историю всех медведей на планете";
        AllMessagesArray[2] = "Сегодня даю тебе задание на создание вещества для синтезирования эликсира. Для этого придётся немного прогуляться, а то ты не вылезаешь из лаборатории";
        AllMessagesArray[3] = "Итак, я выслал тебе на очки несколько полезных инструментов, один из которых карта";
        AllMessagesArray[4] = "На ней указано строение подземелья, где и предстоит добыть все необходимые вещества для выполнения задачи";
        AllMessagesArray[5] = "Синим цветом отмечены зоны с разнообразными ловушками, через которые нужно пройти";
        AllMessagesArray[6] = "А вот красным обозначены локации, где нет ловушек, но опасность представляют ядовитые испарения, против которых придётся сначала создать антидот";
        AllMessagesArray[7] = "Пора начинать! Высланная мной карта соединена с телепортатором, для открытия точек телепортации  достаточно один раз войти в зону их действия";
        AllMessagesArray[8] = "Помимо этого, они могут спасти твою шкуру. Когда система чувствует крайнюю угрозу она телепортирует тебя на последнюю открытую тобой точку";
        AllMessagesArray[9] = "Для начала открой карту (Кнопка с обозначением метки) и используй уже открытую тобой первую метку";
        AllMessagesArray[10] = "Последовательность локаций идёт сверху вниз. Напишу тебе, как достигнешь конца первого уровня. Удачи";

        AllMessagesArray[11] = "Смотрю, ты справился, прекрасно! Далее тебе необходимо войти в портал, они соединяют уровни подземелья между собой";

        AllMessagesArray[12] = "Здесь и располагаются точки телепортации и хранилища с нужными нам веществами. Нажми на них и выпадут нужные вещества, после возьми их с собой";
        AllMessagesArray[13] = "Пора возвращаться. Открой карту и нажми на точку телепортации лаборатории,  вернёшься обратно";

        AllMessagesArray[14] = "Ах да, совсем забыл рассказать, как создавать вещества ";
        AllMessagesArray[15] = "Для начала зайди в карту, там будет подписано, какое противоядие создавать против ядовитой зоны, это мы уже исследовали";

        AllMessagesArray[16] = "Вторым твоим новым инструментом является справочник. Тут расписаны все реакции, которые понадобятся в создании всех противоядий и нужного нам вещества";
        AllMessagesArray[17] = "Знаю, знаю, даже для тебя там сложно разобраться, но я немного помогу тебе с этим";
        AllMessagesArray[18] = "Для начала посмотри, как создаётся противоядие, выбери в выпадающем списке нужный тебе элемент, нам будут написаны реакции, в которых он создаётся";
        AllMessagesArray[19] = "Дальше посмотри, какие элементы ты добыл, и какие реакции ты можешь составить для получения нужного тебе элемента";
        AllMessagesArray[20] = "Думаю,  я подскажу тебе в первый раз. Ты должен был получить Na, H₂O и Cl";
        AllMessagesArray[21] = "По справочнику нужное тебе противоядие(NaOCl, кажется) можно создать по такой реакции: NaOH + Cl = NaOCl + H₂O";
        AllMessagesArray[22] = "Для создания нам не хватает NaOH, но в нашем справочнике есть нужная нам реакция(раздел Na): Na + H₂O = NaOH";
        AllMessagesArray[23] = "Как видишь всё просто. Для обучения работы с новым оборудованием я выслал тебе небольшую презентацию. Удачи в работе";

        AllMessagesArray[24] = "Вау, ты смог получить элемент, мои поздравления!";
        AllMessagesArray[25] = "Теперь пора использовать наше противоядие на практике. Переместись в то же место, откуда взял элементы, там находится точка телепортации";
        AllMessagesArray[25] = "Подойди к порталу поближе, но сразу не входи, сначала достань противоядие из инвентаря и используй его на себе";
        AllMessagesArray[26] = "А теперь беги в портал! Действие противоядия ограничено, поэтому поторопись прямо сейчас, сразу беги в портал, напишу тебе после твоего прохождения";

        AllMessagesArray[27] = "Смотрю ты живой, какое счастье, ха-ха! Хотя в любом случае тебя бы спасла система";
        AllMessagesArray[28] = "Следующий уровень ждёт тебя. Напишу как вернёшься в лабораторию с новыми элементами";

        AllMessagesArray[29] = "Ты снова в лаборатории, отлично. У меня совсем немного времени тебе писать, сейчас пойду на конференцию как раз по вопросу эликсира";
        AllMessagesArray[30] = "Подскажу лишь, что формулы нужно искать в разделах противоядия(Для второй ядовитой зоны) и в разделе Na";
        AllMessagesArray[31] = "Прости, сейчас я буду на конференции, долго не смогу отвечать , надеюсь ты справишься";
        AllMessagesArray[32] = "И совсем забыл сказать, нам нужно создать Li₂CO₃ , с его созданием я уже не смогу помочь, но ты смышлёный, справишься";

        AllMessagesArray[33] = "Снова здравствуй! Я получил информацию о том что ты закончил создание последнего элемента, ты проделал огромную работу";
        AllMessagesArray[34] = "Я пришлю курьера за этим веществом, а пока можешь уделить это время себе";
        AllMessagesArray[35] = "До связи!";
    }

}
