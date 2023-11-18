using System;
using System.Collections.Generic;
using UnityEngine;

public class Building_Heater : MonoBehaviour
{   
    // Переменные нагревателя
    private Dictionary<string, string> Element;  // подаваемое вещество
    private string Action; // нужное действие 
    private string Parameter; // параметр к действию
    private string Exit; // куда должен выйти элемент
    private List<Dictionary<string, string>> temp_storage; // хранилище для последующих и текущего элемента
    private List<Dictionary<string, string>> storage; // хранилище для предыдущих элементов

    // Инициализация нужного алгоритма, который позже выполнится
    public void Algorithm(Dictionary<string, string> _element, string _action, string _param, string _exit){
        Element = _element;
        temp_storage.Add(Element);
        Action = _action;
        Parameter = _param;
        if(_exit != null) Exit = _exit; 
    }

    // Вызов соответствующей функции(действия) под алгоритм
    // P.S. плиз упростите эти три условия как нибудь
    public void MainCheck(string _action){
        if (Action == "heat"){
            Heat();
        } else if (Action == "combine_with"){
            CombineWithPrevious();
        } else if (Action == "remove_remain"){
            RemoveRemains();
        }
    }
    // Нагреть элемент до определенной температуры
    private void Heat(){
        Element["temperature"] += Convert.ToInt32(Parameter);
        if(Exit != null) storage.Add(Element);
        else Output();
    }
    private void CombineWithPrevious(){

    }
    private void RemoveRemains(){
        
    }
    private void Output(){

    }
}
