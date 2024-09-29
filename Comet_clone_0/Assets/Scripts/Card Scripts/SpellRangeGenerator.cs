using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class SpellRangeGenerator : MonoBehaviour
{
    Vector2Int[] cardinalDirections = { Vector2Int.right, Vector2Int.left, Vector2Int.up, Vector2Int.down };
    GridManager gridManager;

    Dictionary<string, SpellCard> cardLibrary = new Dictionary<string, SpellCard>();
    public Dictionary<string, SpellCard> CardLibrary { get { return cardLibrary; } }


    public SpellCard move, blazingCross, thunderSpear;



    void Start()
    {
        gridManager = FindObjectOfType<GridManager>();

        cardLibrary.Add("Move", move);
        cardLibrary.Add("BlazingCross", blazingCross);
        cardLibrary.Add("ThunderSpear", thunderSpear);
    }

    public List<Tile> GenerateEffectRange(SpellCard.rangeType rangeType, Vector2Int playerCords)
    {
        switch (rangeType)
        {
            case SpellCard.rangeType.none:
                return null;
            case SpellCard.rangeType.cross:
                return GenerateCrossPattern(playerCords);
        }

        return null;
    }

    public List<Tile> GenerateEffectRange(SpellCard.rangeType rangeType, Vector2Int playerCords, Vector2Int direction)
    {
        switch (rangeType)
        {
            case SpellCard.rangeType.directionalLine:
                return GenerateDirectionalLinePattern(playerCords, direction);
        }

        return null;
    }




    #region CardRangeAlgorithms
    List<Tile> GenerateCrossPattern(Vector2Int playerCords)
    {
        List<Tile> result = new List<Tile>();

        foreach (Vector2Int direction in cardinalDirections)
        {

            for (int i = 1; i < 7; ++i)
            {
                Vector2Int lineCords = playerCords + direction*i;

                if (gridManager.Grid.ContainsKey(lineCords))
                    result.Add(gridManager.Grid[lineCords]);
                else if (!gridManager.Grid.ContainsKey(lineCords))
                    break;

            }
        }
        return result;
    }

    List<Tile> GenerateDirectionalLinePattern(Vector2Int playerCords, Vector2Int direction)
    {
        List<Tile> result = new List<Tile>();
        for (int i = 1; i < 5; ++i)
        {
            Vector2Int lineCords = playerCords + (direction*i);

            if (gridManager.Grid.ContainsKey(lineCords))
                result.Add(gridManager.Grid[lineCords]);
            else if (!gridManager.Grid.ContainsKey(lineCords))
                break;
        }
        return result;
    }

    List<Tile> GenerateCirclepattern()
    {
        return new List<Tile>();
    }

    List<Tile> GenerateStarPattern()
    {
        return new List<Tile>();
    }

    #endregion
}
