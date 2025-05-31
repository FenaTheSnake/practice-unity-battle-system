using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public enum CharacterActionEffectType
{
    MOVE_TO_CELL,
    SOURCE_ATTACK_TARGET,
    SOURCE_ADD_AP,
    SOURCE_HEAL,

    TARGET_HEAL,
    TARGET_DEAL_DAMAGE,
    TARGET_DEAL_CONSTANT_DAMAGE,
    TARGET_ADD_DEFENSE,
    TARGET_ADD_ATTACK,
    TARGET_ADD_MOVEMENT_RANGE,
    TARGET_HEAL_MANA
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
            case CharacterActionEffectType.TARGET_DEAL_CONSTANT_DAMAGE:
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
            case CharacterActionEffectType.TARGET_HEAL_MANA:
                target = parameters["target"] as Character;

                if (target == null) break;
                target.HealMana(floatValue);
                break;
            case CharacterActionEffectType.TARGET_DEAL_DAMAGE:
                target = parameters["target"] as Character;

                if (target == null) break;
                target.RecieveDamage(floatValue, true);
                break;
            case CharacterActionEffectType.TARGET_ADD_DEFENSE:
                target = parameters["target"] as Character;

                if (target == null) break;
                target.AddDefense(floatValue);
                break;
            case CharacterActionEffectType.TARGET_ADD_MOVEMENT_RANGE:
                target = parameters["target"] as Character;

                if (target == null) break;
                target.AddMovementRange(floatValue);
                break;
            case CharacterActionEffectType.TARGET_ADD_ATTACK:
                target = parameters["target"] as Character;

                if (target == null) break;
                target.AddAttack(floatValue);
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
            case CharacterActionEffectType.TARGET_DEAL_CONSTANT_DAMAGE:
            case CharacterActionEffectType.TARGET_DEAL_DAMAGE:
                script.floatValue = EditorGUILayout.FloatField("Damage:", script.floatValue);
                break;
            case CharacterActionEffectType.SOURCE_ADD_AP:
                script.intValue = EditorGUILayout.IntField("How much:", script.intValue);
                break;
            case CharacterActionEffectType.SOURCE_HEAL:
            case CharacterActionEffectType.TARGET_HEAL:
            case CharacterActionEffectType.TARGET_HEAL_MANA:
            case CharacterActionEffectType.TARGET_ADD_DEFENSE:
            case CharacterActionEffectType.TARGET_ADD_ATTACK:
            case CharacterActionEffectType.TARGET_ADD_MOVEMENT_RANGE:
                script.floatValue = EditorGUILayout.FloatField("How much:", script.floatValue);
                break;
            default:
                break;
        }
    }
}
