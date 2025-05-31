using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public enum CharacterActionEffectType
{
    SOURCE_ATTACK_TARGET,
    MOVE_TO_CELL,
    SOURCE_DEAL_CONSTANT_DAMAGE_TARGET,
    SOURCE_ADD_AP,
    SOURCE_HEAL,

    TARGET_HEAL,
    TARGET_DEAL_DAMAGE
}

[CreateAssetMenu(fileName = "CharacterActionEffect", menuName = "CharacterAction/CharacterActionEffect")]
public class CharacterActionEffect : ScriptableObject
{
    public CharacterActionEffectType effectType;

    public float floatValue = 0.0f;
    public int intValue = 0;

    public void Execute(Dictionary<string, object> parameters)
    {
        switch (effectType)
        {
            case CharacterActionEffectType.SOURCE_ATTACK_TARGET:
                Character target = parameters["target"] as Character;
                Character source = parameters["source"] as Character;

                if (target == null || source == null) break;
                source.Attack(target);
                break;
            case CharacterActionEffectType.MOVE_TO_CELL:
                MapCell cell = parameters["cell"] as MapCell;
                source = parameters["source"] as Character;

                source.Move(cell.mapPosition);

                break;
            case CharacterActionEffectType.SOURCE_DEAL_CONSTANT_DAMAGE_TARGET:
                target = parameters["target"] as Character;

                if (target == null) break;
                target.RecieveDamage(floatValue, false);
                break;
            case CharacterActionEffectType.SOURCE_ADD_AP:
                source = parameters["source"] as Character;

                if (source == null) break;
                source.AddAP(intValue);
                break;
            case CharacterActionEffectType.SOURCE_HEAL:
                source = parameters["source"] as Character;

                if (source == null) break;
                source.Heal(floatValue);
                break;
            case CharacterActionEffectType.TARGET_HEAL:
                target = parameters["target"] as Character;

                if (target == null) break;
                target.Heal(floatValue);
                break;
            case CharacterActionEffectType.TARGET_DEAL_DAMAGE:
                target = parameters["target"] as Character;

                if (target == null) break;
                target.RecieveDamage(floatValue, false);
                break;
        }
    }
}

[CustomEditor(typeof(CharacterActionEffect))]
public class CharacterActionEffectEditor : Editor
{
    CharacterActionEffectType effectType;

    private void OnEnable()
    {
        var script = target as CharacterActionEffect;
        effectType = script.effectType;
    }

    public override void OnInspectorGUI()
    {
        var script = target as CharacterActionEffect;

        script.effectType = (CharacterActionEffectType)EditorGUILayout.EnumPopup("Action Type:", script.effectType);
        EditorUtility.SetDirty(script);

        switch(script.effectType)
        {
            case CharacterActionEffectType.SOURCE_DEAL_CONSTANT_DAMAGE_TARGET:
            case CharacterActionEffectType.TARGET_DEAL_DAMAGE:
                script.floatValue = EditorGUILayout.FloatField("Damage:", script.floatValue);
                break;
            case CharacterActionEffectType.SOURCE_ADD_AP:
                script.intValue = EditorGUILayout.IntField("How much:", script.intValue);
                break;
            case CharacterActionEffectType.SOURCE_HEAL:
            case CharacterActionEffectType.TARGET_HEAL:
                script.floatValue = EditorGUILayout.FloatField("Added Health:", script.floatValue);
                break;
            default:
                break;
        }
    }
}
