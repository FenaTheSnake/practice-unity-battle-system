using System.Collections.Generic;
using UnityEngine;

public class CAEAttackTarget : ICharacterActionEffect
{
    public void Execute(Dictionary<string, object> parameters)
    {
        Character target = parameters["target"] as Character;
        Character source = parameters["source"] as Character;

        if (target == null || source == null) return;
        source.Attack(target);
    }
}
