using UnityEngine;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Zenject;
using System.Linq;
using System.Net.NetworkInformation;
using static UnityEngine.GraphicsBuffer;

public enum AICurrentCharacterStrategy
{
    ATTACK_NEARBY,
    HEAL_SOMEONE,
    JUST_SKIP
}

public class EnemyAI
{
    GameState _gameState;
    Map _map;

    [Inject]
    EnemyAI(GameState gameState, Map map)
    {
        _gameState = gameState;
        _map = map;

        _gameState.OnPlayerTurnEnd += OnPlayerTurnEnd;
    }

    public void OnPlayerTurnEnd()
    {
        Think();
    }

    async public void Think()
    {
        Debug.Log("[AI] Okay, let me see...");

        foreach(Character character in _gameState.enemyArmy)
        {
            AICurrentCharacterStrategy strategy = AICurrentCharacterStrategy.ATTACK_NEARBY;
            Debug.Log("[AI] What " + character.name + " will do...?");
            while (character.RemainingMovement > 0)
            {
                await UniTask.Delay(500);

                if (strategy == AICurrentCharacterStrategy.ATTACK_NEARBY)
                {
                    Debug.Log("[AI] Let me see if I can hit someone...");
                    var enemies = GetAccessableEnemies(character);
                    if (enemies.Count > 0)
                    {
                        Debug.Log("[AI] I can! Let me see what can I do...");
                        Character closestEnemy = enemies.OrderBy(x => Vector2.Distance(x.MapPosition, character.MapPosition)).FirstOrDefault();
                        CharacterAction attackAction = GetAttackAction(character);
                        
                        if(attackAction == null)
                        {
                            Debug.Log("[AI] Can't find any available attack abilities, I guess I will do something else...");
                            strategy = AICurrentCharacterStrategy.HEAL_SOMEONE;
                            continue;
                        }

                        attackAction.PerformTarget(character, closestEnemy);
                        character.SpendManaAndAPOnAction(attackAction);
                        character.DoALittleMovementInDirectionToCharacter(closestEnemy);
                    }
                    else
                    {
                        Debug.Log("[AI] No one nearby, huh? Then I will approach someone.");
                        Character closestEnemy = _gameState.playerArmy.OrderBy(x => Vector2.Distance(x.MapPosition, character.MapPosition)).FirstOrDefault();
                        if (closestEnemy == null)
                        {
                            Debug.Log("[AI] No enemies on the map?? I guess I will do something else...");
                            strategy = AICurrentCharacterStrategy.HEAL_SOMEONE;
                            continue;
                        }

                        CharacterAction moveAction = GetMoveAction(character);
                        if(moveAction == null)
                        {
                            Debug.Log("[AI] I have no ability to move, then I will do something else, I guess...");
                            strategy = AICurrentCharacterStrategy.HEAL_SOMEONE;
                            continue;
                        }

                        Vector2Int approachPoint = GetValidPositionToApproachEnemy(character, closestEnemy);

                        bool movedSuccessfully = false;
                        float dist = Mathf.Min(character.RemainingMovement, Vector2Int.Distance(character.MapPosition, approachPoint));
                        while (!movedSuccessfully)
                        {
                            if (dist <= 0) break;

                            Vector2 point = Vector2.MoveTowards(character.MapPosition, approachPoint, dist);
                            Vector2Int toMap = new Vector2Int(Mathf.RoundToInt(point.x), Mathf.RoundToInt(point.y));
                            if (Vector2Int.Distance(character.MapPosition, toMap) > character.RemainingMovement) { dist -= 0.5f; continue; }

                            MapCell cell = _map.map[toMap];
                            if (cell == null) { dist -= 0.5f; continue; }
                            if (cell.entity != null) { dist -= 0.5f; continue; }

                            moveAction.PerformCell(cell, character);
                            movedSuccessfully = true;
                        }

                        if (!movedSuccessfully)
                        {
                            Debug.Log("[AI] Can't approach closest enemy, I guess I will do something else...");
                            strategy = AICurrentCharacterStrategy.HEAL_SOMEONE;
                            continue;
                        }
                    }
                }
                else if(strategy == AICurrentCharacterStrategy.HEAL_SOMEONE)
                {
                    Debug.Log("[AI] Can't heal anybody because the god of this world haven't released this functionality yet.");
                    strategy = AICurrentCharacterStrategy.JUST_SKIP;
                } 
                else if (strategy == AICurrentCharacterStrategy.JUST_SKIP)
                {
                    Debug.Log("[AI] I guess this character will do absolutely nothing.");
                    break;
                }
            }
        }

        Debug.Log("[AI] I think that's it.");
        _gameState.EndTurn();
    }

    public List<Character> GetAccessableEnemies(Character character)
    {
        Vector2Int center = character.MapPosition;
        int radius = character.attackType == AttackType.MELEE ? (int)character.RemainingMovement : (int)character.movementRange;

        List<Character> enemies = new List<Character>();

        for (int i = center.x - radius; i <= center.x + radius; i++)
        {
            for (int j = center.y - radius; j <= center.y + radius; j++)
            {
                if (i < 0 || j < 0 || i >= _map.Width || j >= _map.Height) continue;
                float d = Vector2Int.Distance(center, new Vector2Int(i, j));
                if (d > radius) continue;

                MapCell cell = _map.map[new Vector2Int(i, j)];
                if (cell.entity != null)
                {
                    if (cell.entity is Character c && c.isPlayerUnit)
                    {
                        if (character.attackType == AttackType.RANGED || (d <= 1)) enemies.Add(c);
                    }
                    continue;
                }
            }
        }

        return enemies;
    }
    public CharacterAction GetMoveAction(Character character)
    {
        return character.actionSet.Where(x => x.actionAIHint == CharacterActionAIHint.MOVEMENT).FirstOrDefault();
    }
    public CharacterAction GetAttackAction(Character character)
    {
        List<CharacterAction> attackActions = character.actionSet.Where(x => x.actionAIHint ==  CharacterActionAIHint.ATTACK_SINGLE && 
                                                                                                x.requiredMP <= character.Mana &&
                                                                                                x.requiredMovement <= character.RemainingMovement &&
                                                                                                x.requiredAP <= character.AP).ToList();
        if (attackActions.Count == 0) return null;

        return attackActions[Random.Range(0, attackActions.Count)];
    }

    // Returns the closest cell to the side of enemy for character to approach so it can attack the enemy
    // if character is ranged, returns the middle point from where character can attack enemy
    public Vector2Int GetValidPositionToApproachEnemy(Character character, Character enemy)
    {
        if(character.attackType == AttackType.RANGED)
        {
            Vector2 enemyPos = new Vector2(enemy.MapPosition.x, enemy.MapPosition.y);
            Vector2 characterPos = new Vector2(character.MapPosition.x, character.MapPosition.y);
            Vector2 point = Vector2.MoveTowards(enemyPos, characterPos, character.movementRange / 2);

            return new Vector2Int(Mathf.RoundToInt(point.x), Mathf.RoundToInt(point.y));
        }

        Vector2Int result = enemy.MapPosition;
        float minDist = 99999.0f;

        Vector2 charPos = new Vector2(character.MapPosition.x, character.MapPosition.y);

        Vector2 leftSide = new Vector2(enemy.MapPosition.x - 1, enemy.MapPosition.y);
        if(!(leftSide.x < 0 || leftSide.y < 0 || leftSide.x >= _map.Height || leftSide.y >= _map.Height))
        {
            float d = Vector2.Distance(leftSide, character.MapPosition);
            if (d < minDist)
            {
                result = new Vector2Int(Mathf.RoundToInt(leftSide.x), Mathf.RoundToInt(leftSide.y));
                minDist = d;
            }
        }
        Vector2 rightSide = new Vector2(enemy.MapPosition.x + 1, enemy.MapPosition.y);
        if (!(rightSide.x < 0 || rightSide.y < 0 || rightSide.x >= _map.Height || rightSide.y >= _map.Height))
        {
            float d = Vector2.Distance(rightSide, character.MapPosition);
            if (d < minDist)
            {
                result = new Vector2Int(Mathf.RoundToInt(rightSide.x), Mathf.RoundToInt(rightSide.y));
                minDist = d;
            }
        }
        Vector2 topSide = new Vector2(enemy.MapPosition.x, enemy.MapPosition.y - 1);
        if (!(topSide.x < 0 || topSide.y < 0 || topSide.x >= _map.Height || topSide.y >= _map.Height))
        {
            float d = Vector2.Distance(rightSide, character.MapPosition);
            if (d < minDist)
            {
                result = new Vector2Int(Mathf.RoundToInt(topSide.x), Mathf.RoundToInt(topSide.y));
                minDist = d;
            }
        }
        Vector2 bottomSide = new Vector2(enemy.MapPosition.x, enemy.MapPosition.y + 1);
        if (!(bottomSide.x < 0 || bottomSide.y < 0 || bottomSide.x >= _map.Height || bottomSide.y >= _map.Height))
        {
            float d = Vector2.Distance(bottomSide, character.MapPosition);
            if (d < minDist)
            {
                result = new Vector2Int(Mathf.RoundToInt(bottomSide.x), Mathf.RoundToInt(bottomSide.y));
                minDist = d;
            }
        }

        return result;
    }
}
