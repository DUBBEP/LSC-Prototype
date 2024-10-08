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


    public SpellCard move, blazingCross, thunderSpear, laserCannon;


    public static SpellRangeGenerator instance;

    private void Awake() { instance = this; }


    void Start()
    {
        gridManager = FindObjectOfType<GridManager>();

        cardLibrary.Add("Move", move);
        cardLibrary.Add("BlazingCross", blazingCross);
        cardLibrary.Add("ThunderSpear", thunderSpear);
        cardLibrary.Add("LaserCannon", laserCannon);
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
            case SpellCard.rangeType.laser:
                return GenerateLaserPattern(playerCords, direction);
        }

        return null;
    }

    public List<Tile> GenerateEffectRange(Vector2Int playerCords, int range)
    {
        return GenerateTravelRange(playerCords, range);
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

    List<Tile> GenerateLaserPattern(Vector2Int playerCords, Vector2Int direction)
    {
        List<Tile> result = new List<Tile>();

        int maxGridLength = Mathf.Max(GridManager.instance.GridSize.x, gridManager.GridSize.y);


        for (int i = 1; i < maxGridLength; ++i)
        {
            Vector2Int lineCords = playerCords + (direction * i);

            if (gridManager.Grid.ContainsKey(lineCords))
                result.Add(gridManager.Grid[lineCords]);
            else if (!gridManager.Grid.ContainsKey(lineCords))
                continue;
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


    List<Tile> GenerateTravelRange(Vector2Int playerCords, int range)
    {
        // take the passed range value to build the characters movement range
        // start with the players current position and check for available spots
        // in the cardinal directions. For any available spaces found, add them to
        // the list of the spaces in the players current movement range.
        // then check the cardinal directions of the newly added spaces and add tiles
        // next to those ones. Repeat this process for however many times the passed range
        // value indicates.

        List<Tile> exploreRange = new List<Tile>();
        List<Tile> travelRange = new List<Tile>();

        travelRange.Add(gridManager.Grid[playerCords]);
        exploreRange.Add(gridManager.Grid[playerCords]);

        // Get every tile in player movement radius
        for (int i = 0; i < range; ++i)
        {

            foreach (Tile x in exploreRange)
            {
                travelRange = ExploreNeighbors(x, travelRange);
            }


            // This loop does not lead to the intended effect and must be changed
            foreach (Tile y in travelRange)
            {
                if (exploreRange.Contains(y))
                    exploreRange.Remove(y);
                else
                    exploreRange.Add(y);
            }
        }

        return travelRange;
    }

    List<Tile> ExploreNeighbors(Tile tile, List<Tile> travelRange)
    {
        foreach (Vector2Int direction in cardinalDirections)
        {
            Vector2Int neighborCords = tile.cords + direction;
            if (gridManager.Grid.ContainsKey(neighborCords))
            {
                if (!travelRange.Contains(gridManager.Grid[neighborCords]))
                {
                    travelRange.Add(gridManager.Grid[neighborCords]);
                }
            }
        }

        return travelRange;
    }

    #endregion
}
