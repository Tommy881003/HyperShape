using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Linq;

[CustomEditor(typeof(Buff))]
public class BuffEditor : Editor
{
    List<int> indexs = new List<int>();
    List<string> choices = new List<string>();
    string[] options;
    Buff Inspecting;
    public void OnEnable()
    {
        Inspecting = (Buff)target;
        WeapodMod.LoadString();
        indexs.Clear();
        choices.Clear();
        for (int i = 0; i < WeapodMod.intList.Count; i++)
            choices.Add(WeapodMod.intList[i]);
        for (int i = 0; i < WeapodMod.floatList.Count; i++)
            choices.Add(WeapodMod.floatList[i]);
        for (int i = 0; i < WeapodMod.boolList.Count; i++)
            choices.Add(WeapodMod.boolList[i]);
        for (int i = 0; i < Inspecting.modifiers.Count; i++)
            indexs.Add(choices.FindIndex(x => x == Inspecting.modifiers[i].param));
        options = choices.ToArray();
    }
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        while (Inspecting.modifiers.Count != indexs.Count)
        {
            if (Inspecting.modifiers.Count > indexs.Count)
                indexs.Add(0);
            else
                indexs.RemoveAt(indexs.Count - 1);       
        }    
        for (int i = 0; i < indexs.Count; i++)
        {
            indexs[i] = EditorGUILayout.Popup(indexs[i], options);
            Inspecting.modifiers[i].param = options[indexs[i]];
        }
    }
}
