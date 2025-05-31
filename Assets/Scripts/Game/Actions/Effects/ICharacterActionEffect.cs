using System;
using System.Collections.Generic;
using UnityEngine;

public interface ICharacterActionEffect
{
    public void Execute(Dictionary<string, object> parameters);
}
