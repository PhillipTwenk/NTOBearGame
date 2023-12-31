using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EffectController : MonoBehaviour
{
    public Transform CharacterPosition;
    public Transform CameraPosition;
    public Transform[] CPpositionsArray;
    [SerializeField] Image EffectImage;
    [SerializeField] Text TimerText;
    [SerializeField] Text ElementName;
    public Sprite[] ArrayEffectSprites;
    private float timer;
    private float timer_limit;
    private bool isEffect = false;
    private string immune_to = "";
    private string toxic = "";
    private CheckPointClass CPclass;
    void Start()
    {
        CPclass = new CheckPointClass();
    }
    private void Update(){
        if(PlayerState.is_changed == true){
            NewEffect();
        }

        if(timer_limit > 0){
            TimerText.text = Convert.ToInt32(timer).ToString();
            timer -= Time.deltaTime;
            if(timer <= 0f){
                timer = 0;

                EffectImage.sprite = null;
                EffectImage.color = new Color32(255,255,225,0);
                EffectImage.fillAmount = 1f;
                TimerText.text = "";
                
                PlayerState.player_state = null;
                if(immune_to != ""){
                    immune_to = "";
                } else if (toxic != ""){
                    toxic = "";
                    CPclass.DeadTeleportation(CPpositionsArray, CharacterPosition, CameraPosition);
                }

                isEffect = false;
                ElementName.text = "";
                timer_limit = 0;
            }
        }
    }

    private void NewEffect(){
        EffectImage.color = new Color32(255,255,225,255);
        if(PlayerState.player_state.Split(' ')[0] == "Смерть" && immune_to != PlayerState.player_state.Split(' ')[2] && toxic == ""){
            timer = 5;
            timer_limit = 5;
            EffectImage.sprite = ArrayEffectSprites[1];
            toxic = PlayerState.player_state.Split(' ')[2];
            ElementName.text = PlayerState.player_state.Split(' ')[2];
        } else if (PlayerState.player_state.Split(' ')[0] == "Противоядие" && (toxic == PlayerState.player_state.Split(' ')[2] || toxic == "")){
            timer = 120;
            timer_limit = 120;
            immune_to = PlayerState.player_state.Split(' ')[2];
            string entry_element = DBManager.ExecuteQuery($"SELECT entry_element FROM elements_effects WHERE result_parameter = '{Convert.ToInt32(Building.ElementInfo(element_name: immune_to)["element_id"])}' AND result = 2");
            ElementName.text = Building.ElementInfo(element_id: Convert.ToInt32(entry_element))["name"];
            EffectImage.sprite = ArrayEffectSprites[2];
            toxic = "";
        } else if (toxic == "" && immune_to == "" && !isEffect){
            timer = 1;
            timer_limit = 1;
            EffectImage.sprite = ArrayEffectSprites[0];
            ElementName.text = PlayerState.player_state;
            isEffect = true;
        }
        PlayerState.is_changed = false;
    }

}
