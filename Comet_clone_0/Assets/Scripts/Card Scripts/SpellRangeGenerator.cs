using System.Collections.Generic;
using UnityEngine;
using System.Linq;


public class SpellRangeGenerator : MonoBehaviour
{
    Vector2Int[] cardinalDirections = { Vector2Int.right, Vector2Int.left, Vector2Int.up, Vector2Int.down };

    Dictionary<string, SpellCard> cardLibrary = new Dictionary<string, SpellCard>();
    public Dictionary<string, SpellCard> CardLibrary { get { return cardLibrary; } }


    public SpellCard emptyCard, move, blazingCross, thunderSpear, laserCannon, magicMirror, orbOfConfusion,
        spacialSlice, flashBang, ghostHand, cosmicStar, magicDisk, heal, fullHeal, teleport;


    public static SpellRangeGenerator instance;

    private void Awake() { instance = this; }


    void Start()
    {
        cardLibrary.Add("EmptyCard", emptyCard);
        cardLibrary.Add("Move", move);
        cardLibrary.Add("BlazingCross", blazingCross);
        cardLibrary.Add("ThunderSpear", thunderSpear);
        cardLibrary.Add("LaserCannon", laserCannon);
        cardLibrary.Add("MagicMirror", magicMirror);
        cardLibrary.Add("OrbOfConfusion", orbOfConfusion);
        cardLibrary.Add("SpacialSlice", spacialSlice);
        cardLibrary.Add("FlashBang", flashBang);
        cardLibrary.Add("GhostHand", ghostHand);
        cardLibrary.Add("CosmicStar", cosmicStar);
        cardLibrary.Add("MagicDisk", magicDisk);
        cardLibrary.Add("Heal", heal);
        cardLibrary.Add("FullHeal", fullHeal);
        cardLibrary.Add("Teleport", teleport);
    }

    public List<Tile> GenerateEffectRange(SpellCard.rangeType rangeType, Vector2Int playerCords)
    {
        switch (rangeType)
        {
            case SpellCard.rangeType.none:
                return new List<Tile>();
            case SpellCard.rangeType.cross:
                return GenerateCrossPattern(playerCords);
            case SpellCard.rangeType.flashbang:
                return GenerateTravelRange(playerCords, 5);
            case SpellCard.rangeType.orb:
                return GenerateTravelRange(playerCords, 4, true);
            case SpellCard.rangeType.star:
                return GenerateXPattern(playerCords);
            case SpellCard.rangeType.wholeMap:
                return WholeMapRange();

        }

        return new List<Tile>();
    }

    public List<Tile> GenerateEffectRange(SpellCard.rangeType rangeType, Vector2Int playerCords, Vector2Int direction)
    {
        switch (rangeType)
        {
            case SpellCard.rangeType.directionalLine:
                return GenerateDirectionalLinePattern(playerCords, direction);
            case SpellCard.rangeType.laser:
                return GenerateLaserPattern(playerCords, direction);
            case SpellCard.rangeType.hand:
                return GenerateHandPattern(playerCords, direction);
            case SpellCard.rangeType.slice:
                return GenerateSlicePattern(playerCords, direction);
            case SpellCard.rangeType.disk:
                return GenerateCirclepattern(playerCords, direction);
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

                if (GridManager.instance.Grid.ContainsKey(lineCords))
                    result.Add(GridManager.instance.Grid[lineCords]);
                else if (!GridManager.instance.Grid.ContainsKey(lineCords))
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

            if (GridManager.instance.Grid.ContainsKey(lineCords))
                result.Add(GridManager.instance.Grid[lineCords]);
            else if (!GridManager.instance.Grid.ContainsKey(lineCords))
                break;
        }
        return result;
    }

    List<Tile> GenerateLaserPattern(Vector2Int playerCords, Vector2Int direction)
    {
        List<Tile> result = new List<Tile>();

        int maxGridLength = Mathf.Max(GridManager.instance.GridSize.x, GridManager.instance.GridSize.y);


        for (int i = 1; i < maxGridLength; ++i)
        {
            Vector2Int lineCords = playerCords + (direction * i);

            if (GridManager.instance.Grid.ContainsKey(lineCords))
                result.Add(GridManager.instance.Grid[lineCords]);
        }
        return result;
    }

    List<Tile> GenerateHandPattern(Vector2Int playerCords, Vector2Int direction)
    {
        List<Tile> result = new List<Tile>();

        if (GridManager.instance.Grid.ContainsKey(playerCords + direction))
            result.Add(GridManager.instance.Grid[playerCords + direction]);

        return result;
    }

    List<Tile> GenerateSlicePattern(Vector2Int playerCords, Vector2Int direction)
    {
        List<Tile> result = new List<Tile>();

        List<Vector2Int> pattern = new List<Vector2Int>();



        if (direction == Vector2Int.down || direction == Vector2Int.up)
        {
            pattern.Add(Vector2Int.left);
            pattern.Add(Vector2Int.zero);
            pattern.Add(Vector2Int.right);

            foreach (Vector2Int i in pattern)
            {
                if (GridManager.instance.Grid.ContainsKey(playerCords + i + (direction * 2)))
                    result.Add(GridManager.instance.Grid[playerCords + i + (direction * 2)]);
            }
        }
        else if (direction == Vector2Int.left || direction == Vector2Int.right)
        {
            pattern.Add(Vector2Int.up);
            pattern.Add(Vector2Int.zero);
            pattern.Add(Vector2Int.down);

            foreach (Vector2Int i in pattern)
            {
                if (GridManager.instance.Grid.ContainsKey(playerCords + i + (direction * 2)))
                    result.Add(GridManager.instance.Grid[playerCords + i + (direction * 2)]);
            }
        }
        return result;
    }

    List<Tile> GenerateCirclepattern(Vector2Int playerCords, Vector2Int direction)
    {
        List<Tile> result = new();

        Vector2Int center = playerCords + (direction * 3);

        foreach (Vector2Int dir in cardinalDirections)
        {
            List<Tile> side = GenerateSlicePattern(center, dir);

            foreach (Tile tile in side)
                result.Add(tile);
        }
        return result;
    }

    List<Tile> GenerateXPattern(Vector2Int playerCords)
    {
        List<Tile> result = new();

        for (int i = 0; i < cardinalDirections.Length; i++)
        {
            Vector2Int currentDirection = cardinalDirections[i];

            if (i == 0)
                currentDirection += Vector2Int.up;
            else if (i == 1)
                currentDirection += Vector2Int.down;
            else if (i == 2)
                currentDirection += Vector2Int.left;
            else if (i == 3)
                currentDirection += Vector2Int.right;

            for ( int j = 1; j < 7; ++j)
            {
                Vector2Int diagonalCenter = playerCords + currentDirection * j;

                if (GridManager.instance.Grid.ContainsKey(diagonalCenter))
                {
                    result.Add(GridManager.instance.Grid[diagonalCenter]);
                    result = ExploreNeighbors(GridManager.instance.Grid[diagonalCenter], result);
                }

            }
        }
        return result;
    }

    List<Tile> GenerateTravelRange(Vector2Int playerCords, int range, bool selfInflicting = false)
    {

        List<Tile> exploreRange = new List<Tile>();
        List<Tile> travelRange = new List<Tile>();

        travelRange.Add(GridManager.instance.Grid[playerCords]);
        exploreRange.Add(GridManager.instance.Grid[playerCords]);

        // Get every tile in player movement radius
        for (int i = 0; i < range; ++i)
        {

            foreach (Tile x in exploreRange)
            {
                travelRange = ExploreNeighbors(x, travelRange);
            }

            foreach (Tile y in travelRange)
            {
                if (exploreRange.Contains(y))
                    exploreRange.Remove(y);
                else
                    exploreRange.Add(y);
            }
        }

        if (!selfInflicting)
            travelRange.Remove(GridManager.instance.Grid[playerCords]);

        return travelRange;
    }

    List<Tile> ExploreNeighbors(Tile tile, List<Tile> travelRange)
    {
        foreach (Vector2Int direction in cardinalDirections)
        {
            Vector2Int neighborCords = tile.cords + direction;
            if (GridManager.instance.Grid.ContainsKey(neighborCords))
            {
                if (!travelRange.Contains(GridManager.instance.Grid[neighborCords]))
                {
                    travelRange.Add(GridManager.instance.Grid[neighborCords]);
                }
            }
        }

        return travelRange;
    }

    List<Tile> WholeMapRange()
    {
        return GridManager.instance.Grid.Values.ToList();
    }

    #endregion
}
