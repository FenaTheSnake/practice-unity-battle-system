using System.Collections.Generic;
using TMPro.EditorUtilities;
using UnityEngine;
using static Unity.VisualScripting.Member;
using static UnityEngine.GraphicsBuffer;

public enum CharacterActionType
{
    ENEMY_TARGET,
    ALLY_TARGET,
    ANY_TARGET,
    CELL,
    OPEN_ACTIONS,

    ALL_ALLIES,
    ALL_ENEMIES,
    ALL_UNITS,
    RANDOM_ALLY,
    RANDOM_ENEMY,
    RANDOM_UNIT
}

public enum CharacterActionAIHint
{
    MOVEMENT,
    ATTACK_SINGLE,
}

[CreateAssetMenu(fileName = "CharacterAction", menuName = "CharacterAction/CharacterAction")]
public class CharacterAction : ScriptableObject
{
    public string actionName;
    public string actionDescription;
    public CharacterActionType actionType;
    public int requiredMP = 0;
    public int requiredAP = 0;
    public int requiredMovement = 1;

    public List<CharacterActionEffect> effects;

    [Space]
    // Подсказка для ИИ насчет этого действия
    public CharacterActionAIHint actionAIHint;

    public void PerformTarget(Character source, Character target)
    {
        Dictionary<string, object> parameters = new Dictionary<string, object>();
        parameters["source"] = source;
        parameters["target"] = target;
        foreach (CharacterActionEffect effect in effects)
        {
            effect.Execute(parameters);
        }
    }

    public void PerformCell(MapCell cell, Character source)
    {
        Dictionary<string, object> parameters = new Dictionary<string, object>();
        parameters["cell"] = cell;
        parameters["source"] = source;
        foreach (CharacterActionEffect effect in effects)
        {
            effect.Execute(parameters);
        }
    }
}
