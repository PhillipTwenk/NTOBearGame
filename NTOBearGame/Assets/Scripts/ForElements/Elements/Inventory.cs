using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public static class Inventory 
{
    public static bool is_changed = false;
    public static Dropdown UpdateInventory(Dropdown SpawnElementChoice){
        SpawnElementChoice.AddOptions(Building.ElementsChoiceInfo(true));
        return SpawnElementChoice;
    }

}
