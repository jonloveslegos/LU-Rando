using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using SQLiteWithCSharp.Models;

public delegate bool Del(World world);
public struct ItemType
{
    public string name;
    public ItemCategory category;
    public ItemType(ItemType itemType)
    {
        name = itemType.name;
        category = itemType.category;
    }
    public ItemType(string itemId, ItemCategory itemCategory)
    {
        name = itemId;
        category = itemCategory;
    }
}
public enum ItemCategory
{
    items = 0,
    emotes = 1,
    imagine = 2,
    inv = 3,
    currency = 4,
    exp = 5
}
public static class Rule
{
    public static bool PlacedItem(World world, ItemType itemId, int count)
    {
        var amount = 0;
        switch (itemId.category)
        {
            case ItemCategory.items:
                amount += world.placedLocations.Where(item => item.reward_item1.ToString() == itemId.name).Sum(item => item.reward_item1_count);
                amount += world.placedLocations.Where(item => item.reward_item2.ToString() == itemId.name).Sum(item => item.reward_item2_count);
                amount += world.placedLocations.Where(item => item.reward_item3.ToString() == itemId.name).Sum(item => item.reward_item3_count);
                amount += world.placedLocations.Where(item => item.reward_item4.ToString() == itemId.name).Sum(item => item.reward_item4_count);
                break;
            case ItemCategory.emotes:
                amount += world.placedLocations.Where(item => item.reward_emote.ToString() == itemId.name).Count();
                amount += world.placedLocations.Where(item => item.reward_emote2.ToString() == itemId.name).Count();
                amount += world.placedLocations.Where(item => item.reward_emote3.ToString() == itemId.name).Count();
                amount += world.placedLocations.Where(item => item.reward_emote4.ToString() == itemId.name).Count();
                break;
            case ItemCategory.imagine:
                amount += world.placedLocations.Sum(item => item.reward_maximagination);
                break;
            case ItemCategory.inv:
                amount += world.placedLocations.Sum(item => item.reward_maxinventory);
                break;
            case ItemCategory.currency:
                amount += (int)world.placedLocations.Sum(item => item.reward_currency);
                break;
            case ItemCategory.exp:
                amount += world.placedLocations.Sum(item => item.LegoScore);
                break;
            default:
                return false;
        }
        return amount >= count;
    }
    public static bool ReturnTrue(World world)
    {
        return true;
    }
}

public class World
{
    public Dictionary<int, Missions> locations = new Dictionary<int, Missions>();
    public List<Missions> placedLocations = new List<Missions>();
    public List<ItemType> items = new List<ItemType>();
    public List<ItemType> emotes = new List<ItemType>();
    public List<ItemType> imagine = new List<ItemType>();
    public List<ItemType> inv = new List<ItemType>();
    public List<ItemType> currency = new List<ItemType>();
    public List<ItemType> exp = new List<ItemType>();
    public Dictionary<int,Predicate<World>> rules = new Dictionary<int,Predicate<World>>();
    public Random rng = new Random();
    List<Missions> availableIdsSaved;
    public World(List<Missions> availableIds)
    {
        for (int i = 0; i <= 2077; i++)
        {
            if (availableIds.Count(x => x.id == i) > 0)
            {
                locations.Add(locations.Count, new Missions(availableIds.First(x => x.id == i)));
                locations[locations.Count - 1].reward_item1 = -999;
                locations[locations.Count - 1].id = i;
            }
        }
        for (var i = 0; i <= 2077; i++)
        {
            rules.Add(i, (world) => SpecialLogic.areaAccess[12].Invoke(world,0,0));
        }
        availableIdsSaved = availableIds;
    }
    public void PlaceItem(int locationLocId, int spot)
    {
        int locationId = locations.First(x => x.Value.id == locationLocId).Key;
        if (locations[locationId].reward_item1 == -999)
        {
            locations[locationId].reward_item1 = -1;
        }
        locations[locationId].reward_item1 = int.Parse(items[spot * 4].name.Split(':')[0]);
        locations[locationId].reward_item1_count = int.Parse(items[spot * 4].name.Split(':')[1]);
        locations[locationId].reward_item2 = int.Parse(items[spot * 4 + 1].name.Split(':')[0]);
        locations[locationId].reward_item2_count = int.Parse(items[spot * 4 + 1].name.Split(':')[1]);
        locations[locationId].reward_item3 = int.Parse(items[spot * 4 + 2].name.Split(':')[0]);
        locations[locationId].reward_item3_count = int.Parse(items[spot * 4 + 2].name.Split(':')[1]);
        locations[locationId].reward_item4 = int.Parse(items[spot * 4 + 3].name.Split(':')[0]);
        locations[locationId].reward_item4_count = int.Parse(items[spot * 4 + 3].name.Split(':')[1]);
        locations[locationId].reward_currency = long.Parse(currency[spot].name);
        locations[locationId].reward_maximagination = int.Parse(imagine[spot].name);
        locations[locationId].reward_maxinventory = int.Parse(inv[spot].name);
        locations[locationId].LegoScore = int.Parse(exp[spot].name);
        locations[locationId].reward_emote = int.Parse(emotes[spot * 4].name);
        locations[locationId].reward_emote2 = int.Parse(emotes[spot * 4 + 1].name);
        locations[locationId].reward_emote3 = int.Parse(emotes[spot * 4 + 2].name);
        locations[locationId].reward_emote4 = int.Parse(emotes[spot * 4 + 3].name);
    }
    private List<T> ShuffleList<T>(List<T> list, Random rng)
    {
        return list.Select(x => new { X = x, R = rng.Next() })
                   .OrderBy(x => x.R)
                   .Select(x => x.X)
                   .ToList();
    }

    public void Randomize()
    {
        List<int> emptyLocs = locations.Where(x => x.Value.reward_item1 == -999).Select(x => x.Value.id).ToList();
        emptyLocs = ShuffleList(emptyLocs, rng);
        List<int> toChooseFrom = emptyLocs.Where(x => rules[x].Invoke(this)).ToList();
        currency = ShuffleList(currency, rng);
        imagine = ShuffleList(imagine, rng);
        inv = ShuffleList(inv, rng);
        exp = ShuffleList(exp, rng);
        items = ShuffleList(items, rng);
        emotes = ShuffleList(emotes, rng);
        int chosen = toChooseFrom[0];
        var canPlace = false;
        var placedIt = true;
        var ind = 0;
        var removedItems = new List<Missions>();
        while (emptyLocs.Count() > 0)
        {
            chosen = toChooseFrom[0];
            canPlace = false;
            placedIt = true;
            ind = 0;
            while (canPlace == false)
            {
                PlaceItem(chosen, ind);
                placedLocations.Add(new Missions(locations[locations.First(x => x.Value.id == chosen).Key]));
                emptyLocs.Remove(chosen);
                toChooseFrom = emptyLocs.Where(x => rules[x].Invoke(this)).ToList();
                if (toChooseFrom.Count > 0)
                {
                    canPlace = true;
                    currency.RemoveAt(ind);
                    imagine.RemoveAt(ind);
                    inv.RemoveAt(ind);
                    exp.RemoveAt(ind);
                    emotes.RemoveAt(ind);
                    emotes.RemoveAt(ind);
                    emotes.RemoveAt(ind);
                    emotes.RemoveAt(ind);
                    items.RemoveAt(ind);
                    items.RemoveAt(ind);
                    items.RemoveAt(ind);
                    items.RemoveAt(ind);
                }
                else
                {
                    ind++;
                    placedLocations.RemoveAt(placedLocations.Count - 1);
                    locations[locations.First(x => x.Value.id == chosen).Key] = new Missions(availableIdsSaved.First(x => x.id == chosen));
                    if (ind >= currency.Count)
                    {
                        placedIt = false;
                        emptyLocs.Add(chosen);
                        break;
                    }

                }
            }
            if (placedIt)
            {
                Console.Write("\nPlaced item.\n" + (toChooseFrom.Count) + " locations accessible.\n" + (emptyLocs.Count) + " locations left.\n" + ((currency.Count + emotes.Count + imagine.Count + inv.Count + exp.Count + items.Count) / 12f).ToString() + " items left.");
            }
            else
            {
                removedItems.Clear();
                removedItems.Add(placedLocations[placedLocations.Count - 1]);
                emptyLocs.Add(placedLocations[placedLocations.Count - 1].id);
                locations[locations.First(x => x.Value.id == placedLocations[placedLocations.Count - 1].id).Key] = new Missions(availableIdsSaved[locations.First(x => x.Value.id == placedLocations[placedLocations.Count - 1].id).Key]);
                placedLocations.RemoveAt(placedLocations.Count - 1);
                removedItems.Add(placedLocations[placedLocations.Count - 1]);
                emptyLocs.Add(placedLocations[placedLocations.Count - 1].id);
                locations[locations.First(x => x.Value.id == placedLocations[placedLocations.Count - 1].id).Key] = new Missions(availableIdsSaved[locations.First(x => x.Value.id == placedLocations[placedLocations.Count - 1].id).Key]);
                placedLocations.RemoveAt(placedLocations.Count - 1);
                foreach (var item in removedItems)
                {

                    items.Add(new ItemType(item.reward_item1 + ":" + item.reward_item1_count, ItemCategory.items));
                    items.Add(new ItemType(item.reward_item2 + ":" + item.reward_item2_count, ItemCategory.items));
                    items.Add(new ItemType(item.reward_item3 + ":" + item.reward_item3_count, ItemCategory.items));
                    items.Add(new ItemType(item.reward_item4 + ":" + item.reward_item4_count, ItemCategory.items));
                    emotes.Add(new ItemType(item.reward_emote.ToString(), ItemCategory.emotes));
                    emotes.Add(new ItemType(item.reward_emote2.ToString(), ItemCategory.emotes));
                    emotes.Add(new ItemType(item.reward_emote3.ToString(), ItemCategory.emotes));
                    emotes.Add(new ItemType(item.reward_emote4.ToString(), ItemCategory.emotes));
                    currency.Add(new ItemType(item.reward_currency.ToString(), ItemCategory.currency));
                    exp.Add(new ItemType(item.LegoScore.ToString(), ItemCategory.exp));
                    inv.Add(new ItemType(item.reward_maxinventory.ToString(), ItemCategory.inv));
                    imagine.Add(new ItemType(item.reward_maximagination.ToString(), ItemCategory.imagine));
                }
                removedItems.Clear();
                toChooseFrom = emptyLocs.Where(x => rules[x].Invoke(this)).ToList();
                Console.Write("\nUnplaced item.\n" + (toChooseFrom.Count) + " locations accessible.\n" + (emptyLocs.Count) + " locations left.\n" + ((currency.Count + emotes.Count + imagine.Count + inv.Count + exp.Count + items.Count) / 12f).ToString() + " items left.");
            }
        }
    }

    public void AddItems()
    {
        items = new List<ItemType>();
        currency = new List<ItemType>();
        emotes = new List<ItemType>();
        imagine = new List<ItemType>();
        inv = new List<ItemType>();
        exp = new List<ItemType>();
        var itemsTxt = File.OpenText(Directory.GetCurrentDirectory() + "/items.txt");
        while (!itemsTxt.EndOfStream)
        {
            items.Add(new ItemType(itemsTxt.ReadLine() + ":" + itemsTxt.ReadLine(), ItemCategory.items));
        }
        itemsTxt.Close();
        var emoteTxt = File.OpenText(Directory.GetCurrentDirectory() + "/emotes.txt");
        while (!emoteTxt.EndOfStream)
        {
            emotes.Add(new ItemType(emoteTxt.ReadLine(), ItemCategory.emotes));
        }
        emoteTxt.Close();
        var currencyTxt = File.OpenText(Directory.GetCurrentDirectory() + "/currency.txt");
        while (!currencyTxt.EndOfStream)
        {
            currency.Add(new ItemType(currencyTxt.ReadLine(), ItemCategory.currency));
        }
        currencyTxt.Close();
        var expTxt = File.OpenText(Directory.GetCurrentDirectory() + "/exp.txt");
        while (!expTxt.EndOfStream)
        {
            exp.Add(new ItemType(expTxt.ReadLine(), ItemCategory.exp));
        }
        expTxt.Close();
        var invTxt = File.OpenText(Directory.GetCurrentDirectory() + "/inventory.txt");
        while (!invTxt.EndOfStream)
        {
            inv.Add(new ItemType(invTxt.ReadLine(), ItemCategory.inv));
        }
        invTxt.Close();
        var imaginationTxt = File.OpenText(Directory.GetCurrentDirectory() + "/imagination.txt");
        while (!imaginationTxt.EndOfStream)
        {
            imagine.Add(new ItemType(imaginationTxt.ReadLine(), ItemCategory.imagine));
        }
        imaginationTxt.Close();
    }
}

public static class SpecialLogic
{
    public static bool ReturnTrue(World world)
    {
        return true;
    }
    public static bool CanCompleteTutorial(World world)
    {
        return Rule.PlacedItem(world, new ItemType("-1", ItemCategory.imagine), 6);
    }
    public static bool TokenCount(World world, int requiredAmount)
    {
        int foundCount = 0;
        int[] itemIds = new int[] { 10012, 10095, 10096, 14125, 14126, 12424, 12425, 14127 };
        string[] itemCountFields = new string[] { "reward_item1", "reward_item2" };

        foreach (int itemId in itemIds)
        {
            foundCount += world.placedLocations
                               .Where(x => (int)x.reward_item1 == itemId)
                               .Sum(x => (int)x.reward_item1_count *
                                         (itemId == 10012 ? 2 : (itemId == 10095 ? 5 : (itemId == 10096 ? 10 :
                                         (itemId == 14125 ? 50 : (itemId == 14126 ? 100 : (itemId == 12424 ? 15 :
                                         (itemId == 12425 ? 20 : 200))))))));
            foundCount += world.placedLocations
                               .Where(x => (int)x.reward_item2 == itemId)
                               .Sum(x => (int)x.reward_item2_count *
                                         (itemId == 10012 ? 2 : (itemId == 10095 ? 5 : (itemId == 10096 ? 10 :
                                         (itemId == 14125 ? 50 : (itemId == 14126 ? 100 : (itemId == 12424 ? 15 :
                                         (itemId == 12425 ? 20 : 200))))))));
            foundCount += world.placedLocations
                               .Where(x => (int)x.reward_item3 == itemId)
                               .Sum(x => (int)x.reward_item3_count *
                                         (itemId == 10012 ? 2 : (itemId == 10095 ? 5 : (itemId == 10096 ? 10 :
                                         (itemId == 14125 ? 50 : (itemId == 14126 ? 100 : (itemId == 12424 ? 15 :
                                         (itemId == 12425 ? 20 : 200))))))));
            foundCount += world.placedLocations
                               .Where(x => (int)x.reward_item4 == itemId)
                               .Sum(x => (int)x.reward_item4_count *
                                         (itemId == 10012 ? 2 : (itemId == 10095 ? 5 : (itemId == 10096 ? 10 :
                                         (itemId == 14125 ? 50 : (itemId == 14126 ? 100 : (itemId == 12424 ? 15 :
                                         (itemId == 12425 ? 20 : 200))))))));
        }
        

        return foundCount >= requiredAmount;
    }
    public static Dictionary<int, Func<World, int, int, bool>> areaAccess = new Dictionary<int, Func<World, int, int, bool>>
    {
        {1, (w, s, g) => SpecialLogic.CanCompleteTutorial(w) && Rule.PlacedItem(w, new ItemType("6086", ItemCategory.items), 1)},
        {2, (w, s, g) => (Rule.PlacedItem(w, new ItemType("4880", ItemCategory.items), 1) || Rule.PlacedItem(w, new ItemType("4881", ItemCategory.items), 1) || Rule.PlacedItem(w, new ItemType("4883", ItemCategory.items), 1)) && SpecialLogic.areaAccess[1].Invoke(w, s, g)},
        {3, (w, s, g) => Rule.PlacedItem(w, new ItemType("356", ItemCategory.emotes), 1) && SpecialLogic.areaAccess[2].Invoke(w, s, g)},
        {4, (w, s, g) => Rule.PlacedItem(w, new ItemType("-1", ItemCategory.currency), s + 100) && SpecialLogic.areaAccess[3].Invoke(w, s, g)},
        {5, (w, s, g) =>  Rule.PlacedItem(w, new ItemType("-1", ItemCategory.currency), s + 100) && SpecialLogic.areaAccess[4].Invoke(w, s, g)},
        {6, (w, s, g) => Rule.PlacedItem(w, new ItemType("3039", ItemCategory.items), g + 1) && SpecialLogic.areaAccess[5].Invoke(w, s, g)},
        {7, (w, s, g) =>  Rule.PlacedItem(w, new ItemType("14592", ItemCategory.items), 1) && SpecialLogic.areaAccess[6].Invoke(w, s, g)},
        {8, (w, s, g) =>  Rule.PlacedItem(w, new ItemType("14553", ItemCategory.items), 1) && SpecialLogic.areaAccess[7].Invoke(w, s, g)},
        {9, (w, s, g) =>  (Rule.PlacedItem(w, new ItemType("14359", ItemCategory.items), 1) || Rule.PlacedItem(w, new ItemType("14321", ItemCategory.items), 1) || Rule.PlacedItem(w, new ItemType("14353", ItemCategory.items), 1) || Rule.PlacedItem(w, new ItemType("14315", ItemCategory.items), 1)) && SpecialLogic.areaAccess[8].Invoke(w, s, g)},
        {10, (w, s, g) =>  (Rule.PlacedItem(w, new ItemType("14359", ItemCategory.items), 2) || Rule.PlacedItem(w, new ItemType("14321", ItemCategory.items), 2) || Rule.PlacedItem(w, new ItemType("14353", ItemCategory.items), 2) || Rule.PlacedItem(w, new ItemType("14315", ItemCategory.items), 2)) && SpecialLogic.areaAccess[9].Invoke(w, s, g)},
        {11, (w, s, g) => (Rule.PlacedItem(w, new ItemType("9516", ItemCategory.items), 1) || Rule.PlacedItem(w, new ItemType("9517", ItemCategory.items), 1) || Rule.PlacedItem(w, new ItemType("9518", ItemCategory.items), 1)) && SpecialLogic.areaAccess[10].Invoke(w, s, g)},
        {12, (w, s, g) =>
        ((SQLiteWithCSharp.Program.chosenFaction == 0 && (Rule.PlacedItem(w, new ItemType("8033", ItemCategory.items), 1) || Rule.PlacedItem(w, new ItemType("8031", ItemCategory.items), 1) || Rule.PlacedItem(w, new ItemType("7591", ItemCategory.items), 1))) ||
        (SQLiteWithCSharp.Program.chosenFaction == 1 && (Rule.PlacedItem(w, new ItemType("7586", ItemCategory.items), 1) || Rule.PlacedItem(w, new ItemType("8032", ItemCategory.items), 1) || Rule.PlacedItem(w, new ItemType("8034", ItemCategory.items), 1))) ||
        (SQLiteWithCSharp.Program.chosenFaction == 2 && (Rule.PlacedItem(w, new ItemType("7589", ItemCategory.items), 1) || Rule.PlacedItem(w, new ItemType("8029", ItemCategory.items), 1) || Rule.PlacedItem(w, new ItemType("8030", ItemCategory.items), 1))) ||
        (SQLiteWithCSharp.Program.chosenFaction == 3 && (Rule.PlacedItem(w, new ItemType("7590", ItemCategory.items), 1) || Rule.PlacedItem(w, new ItemType("8028", ItemCategory.items), 1) || Rule.PlacedItem(w, new ItemType("8027", ItemCategory.items), 1)))
        ) && SpecialLogic.areaAccess[11].Invoke(w, s, g)},
    };
}

public class Logic
{
    public void SetLogic(World inWorld)
    {
        inWorld.rules[1727] = (world) => true;
        inWorld.rules[173] = (world) => true;
        inWorld.rules[354] = (world) => true;
        inWorld.rules[1750] = (world) => true;
        inWorld.rules[1751] = (world) => true;
        inWorld.rules[1752] = (world) => true;
        inWorld.rules[664] = (world) => true;
        inWorld.rules[1748] = (world) => SpecialLogic.CanCompleteTutorial(world);
        inWorld.rules[1732] = (world) => SpecialLogic.CanCompleteTutorial(world);
        inWorld.rules[308] = (world) => SpecialLogic.CanCompleteTutorial(world);
        inWorld.rules[488] = (world) => SpecialLogic.CanCompleteTutorial(world);
        inWorld.rules[660] = (world) => SpecialLogic.CanCompleteTutorial(world);
        inWorld.rules[757] = (world) => SpecialLogic.CanCompleteTutorial(world);
        inWorld.rules[1896] = (world) => SpecialLogic.CanCompleteTutorial(world);
        inWorld.rules[311] = (world) => SpecialLogic.areaAccess[1].Invoke(world,0,0);
        inWorld.rules[1728] = (world) => SpecialLogic.areaAccess[1].Invoke(world,0,0);
        inWorld.rules[353] = (world) => SpecialLogic.areaAccess[1].Invoke(world,0,0);
        inWorld.rules[355] = (world) => SpecialLogic.areaAccess[1].Invoke(world,0,0);
        inWorld.rules[356] = (world) => SpecialLogic.areaAccess[1].Invoke(world,0,0);
        inWorld.rules[251] = (world) => SpecialLogic.areaAccess[1].Invoke(world,0,0);
        inWorld.rules[252] = (world) => SpecialLogic.areaAccess[1].Invoke(world,0,0);
        inWorld.rules[706] = (world) => SpecialLogic.areaAccess[1].Invoke(world,0,0);
        inWorld.rules[707] = (world) => SpecialLogic.areaAccess[1].Invoke(world,0,0);
        inWorld.rules[287] = (world) => SpecialLogic.areaAccess[1].Invoke(world,0,0);
        inWorld.rules[755] = (world) => SpecialLogic.areaAccess[2].Invoke(world,0,0);
        inWorld.rules[312] = (world) => SpecialLogic.areaAccess[2].Invoke(world,0,0);
        inWorld.rules[314] = (world) => SpecialLogic.areaAccess[2].Invoke(world,0,0);
        inWorld.rules[259] = (world) => SpecialLogic.areaAccess[3].Invoke(world,0,0);
        inWorld.rules[315] = (world) => SpecialLogic.areaAccess[3].Invoke(world,0,0);
        inWorld.rules[733] = (world) => SpecialLogic.areaAccess[4].Invoke(world,0,0);
        inWorld.rules[316] = (world) => SpecialLogic.areaAccess[4].Invoke(world,0,0);
        inWorld.rules[939] = (world) => SpecialLogic.areaAccess[5].Invoke(world,0,0);
        inWorld.rules[940] = (world) => SpecialLogic.areaAccess[5].Invoke(world,0,0);
        inWorld.rules[479] = (world) => SpecialLogic.areaAccess[6].Invoke(world,0,0);
        inWorld.rules[1847] = (world) => SpecialLogic.areaAccess[6].Invoke(world,0,0);
        inWorld.rules[1848] = (world) => SpecialLogic.areaAccess[6].Invoke(world,0,0);
        inWorld.rules[477] = (world) => SpecialLogic.areaAccess[6].Invoke(world,0,0);
        inWorld.rules[260] = (world) => SpecialLogic.areaAccess[7].Invoke(world,0,0);
        inWorld.rules[1151] = (world) => SpecialLogic.areaAccess[7].Invoke(world,0,0);
        inWorld.rules[1849] = (world) => SpecialLogic.areaAccess[7].Invoke(world,0,0);
        inWorld.rules[1850] = (world) => SpecialLogic.areaAccess[8].Invoke(world,0,0);
        inWorld.rules[1851] = (world) => SpecialLogic.areaAccess[8].Invoke(world,0,0);
        inWorld.rules[1852] = (world) => SpecialLogic.areaAccess[8].Invoke(world,0,0);
        inWorld.rules[1935] = (world) => SpecialLogic.areaAccess[9].Invoke(world,0,0);
        inWorld.rules[313] = (world) => SpecialLogic.areaAccess[9].Invoke(world,0,0);
        inWorld.rules[1853] = (world) => SpecialLogic.areaAccess[9].Invoke(world,0,0);
        inWorld.rules[1936] = (world) => SpecialLogic.areaAccess[9].Invoke(world,0,0);
        inWorld.rules[317] = (world) => SpecialLogic.areaAccess[9].Invoke(world,0,0);
        inWorld.rules[1854] = (world) => SpecialLogic.areaAccess[9].Invoke(world,0,0);
        inWorld.rules[1855] = (world) => SpecialLogic.areaAccess[9].Invoke(world,0,0);
        inWorld.rules[1856] = (world) => SpecialLogic.areaAccess[9].Invoke(world,0,0);
        inWorld.rules[318] = (world) => SpecialLogic.areaAccess[9].Invoke(world,0,0);
        inWorld.rules[633] = (world) => SpecialLogic.areaAccess[9].Invoke(world,0,0);
        inWorld.rules[244] = (world) => SpecialLogic.areaAccess[9].Invoke(world,0,0);
        inWorld.rules[377] = (world) => SpecialLogic.areaAccess[9].Invoke(world,0,0);
        inWorld.rules[1894] = (world) => SpecialLogic.areaAccess[9].Invoke(world,0,0);
        inWorld.rules[208] = (world) => SpecialLogic.areaAccess[9].Invoke(world,0,0);
        inWorld.rules[261] = (world) => SpecialLogic.areaAccess[9].Invoke(world,0,0);
        inWorld.rules[708] = (world) => SpecialLogic.areaAccess[9].Invoke(world,0,0);
        inWorld.rules[783] = (world) => SpecialLogic.areaAccess[9].Invoke(world,0,0);
        inWorld.rules[346] = (world) => SpecialLogic.areaAccess[9].Invoke(world,0,0);
        inWorld.rules[347] = (world) => SpecialLogic.areaAccess[9].Invoke(world,0,0);
        inWorld.rules[348] = (world) => SpecialLogic.areaAccess[9].Invoke(world,0,0);
        inWorld.rules[349] = (world) => SpecialLogic.areaAccess[9].Invoke(world,0,0);
        inWorld.rules[709] = (world) => SpecialLogic.areaAccess[9].Invoke(world,0,0);
        inWorld.rules[250] = (world) => SpecialLogic.areaAccess[9].Invoke(world,0,0);
        inWorld.rules[282] = (world) => SpecialLogic.areaAccess[9].Invoke(world,0,0);
        inWorld.rules[1950] = (world) => SpecialLogic.areaAccess[10].Invoke(world,0,0);
        inWorld.rules[768] = (world) => SpecialLogic.areaAccess[10].Invoke(world,0,0);
        inWorld.rules[871] = (world) => SpecialLogic.areaAccess[10].Invoke(world,0,0);
        inWorld.rules[891] = (world) => SpecialLogic.areaAccess[10].Invoke(world,0,0);
        inWorld.rules[320] = (world) => SpecialLogic.areaAccess[10].Invoke(world,0,0);
        inWorld.rules[1877] = (world) => SpecialLogic.areaAccess[10].Invoke(world,0,0);
        inWorld.rules[319] = (world) => SpecialLogic.areaAccess[10].Invoke(world,0,0);
        inWorld.rules[470] = (world) => SpecialLogic.areaAccess[10].Invoke(world,0,0);
        inWorld.rules[325] = (world) => SpecialLogic.areaAccess[10].Invoke(world,0,0);
        inWorld.rules[286] = (world) => SpecialLogic.areaAccess[10].Invoke(world,0,0);
        inWorld.rules[1293] = (world) => SpecialLogic.areaAccess[10].Invoke(world,0,0);
        inWorld.rules[957] = (world) => SpecialLogic.areaAccess[10].Invoke(world,0,0);
        inWorld.rules[1153] = (world) => SpecialLogic.areaAccess[10].Invoke(world,0,0);
        inWorld.rules[322] = (world) => SpecialLogic.areaAccess[10].Invoke(world,0,0);
        inWorld.rules[1152] = (world) => SpecialLogic.areaAccess[10].Invoke(world,0,0);
        inWorld.rules[324] = (world) => SpecialLogic.areaAccess[10].Invoke(world,0,0);
        inWorld.rules[680] = (world) => SpecialLogic.areaAccess[10].Invoke(world,0,0);
        inWorld.rules[938] = (world) => SpecialLogic.areaAccess[10].Invoke(world,0,0);
        inWorld.rules[483] = (world) => SpecialLogic.areaAccess[10].Invoke(world,0,0);
        inWorld.rules[476] = (world) => SpecialLogic.areaAccess[10].Invoke(world,0,0);
        inWorld.rules[809] = (world) => SpecialLogic.areaAccess[11].Invoke(world,0,0);
        inWorld.rules[475] = (world) => SpecialLogic.areaAccess[11].Invoke(world,0,0);
        inWorld.rules[478] = (world) => SpecialLogic.areaAccess[11].Invoke(world,0,0);
        inWorld.rules[482] = (world) => SpecialLogic.areaAccess[11].Invoke(world,0,0);
        inWorld.rules[588] = (world) => SpecialLogic.areaAccess[11].Invoke(world,0,0);
        inWorld.rules[589] = (world) => SpecialLogic.areaAccess[11].Invoke(world,0,0);
        inWorld.rules[590] = (world) => SpecialLogic.areaAccess[11].Invoke(world,0,0);
        inWorld.rules[591] = (world) => SpecialLogic.areaAccess[11].Invoke(world,0,0);
        inWorld.rules[684] = (world) => SpecialLogic.areaAccess[11].Invoke(world,0,0);
        inWorld.rules[685] = (world) => SpecialLogic.areaAccess[11].Invoke(world,0,0);
        inWorld.rules[686] = (world) => SpecialLogic.areaAccess[11].Invoke(world,0,0);
        inWorld.rules[687] = (world) => SpecialLogic.areaAccess[11].Invoke(world,0,0);
        inWorld.rules[593] = (world) => SpecialLogic.areaAccess[11].Invoke(world,0,0);
        inWorld.rules[256] = (world) => SpecialLogic.areaAccess[11].Invoke(world,0,0);
        inWorld.rules[283] = (world) => SpecialLogic.areaAccess[11].Invoke(world,0,0);
        inWorld.rules[537] = (world) => SpecialLogic.areaAccess[11].Invoke(world,0,0);
        inWorld.rules[302] = (world) => SpecialLogic.areaAccess[11].Invoke(world,0,0);
        inWorld.rules[595] = (world) => SpecialLogic.areaAccess[11].Invoke(world,0,0);
        inWorld.rules[812] = (world) => SpecialLogic.areaAccess[12].Invoke(world,0,0);
        inWorld.rules[813] = (world) => SpecialLogic.areaAccess[12].Invoke(world,0,0);
        inWorld.rules[814] = (world) => SpecialLogic.areaAccess[12].Invoke(world,0,0);
        inWorld.rules[815] = (world) => SpecialLogic.areaAccess[12].Invoke(world,0,0);
        inWorld.rules[1889] = (world) => SpecialLogic.areaAccess[12].Invoke(world,0,0);
        inWorld.rules[1890] = (world) => SpecialLogic.areaAccess[12].Invoke(world,0,0);
        inWorld.rules[1891] = (world) => SpecialLogic.areaAccess[12].Invoke(world,0,0);
        inWorld.rules[1892] = (world) => SpecialLogic.areaAccess[12].Invoke(world,0,0);
        inWorld.rules[867] = (world) => SpecialLogic.areaAccess[12].Invoke(world,0,0) && SpecialLogic.TokenCount(world, 25);
        inWorld.rules[866] = (world) => SpecialLogic.areaAccess[12].Invoke(world,0,0) && SpecialLogic.TokenCount(world, 25);
        inWorld.rules[865] = (world) => SpecialLogic.areaAccess[12].Invoke(world,0,0) && SpecialLogic.TokenCount(world, 25);
        inWorld.rules[767] = (world) => SpecialLogic.areaAccess[12].Invoke(world,0,0) && SpecialLogic.TokenCount(world, 25);
        inWorld.rules[779] = (world) => SpecialLogic.areaAccess[12].Invoke(world,0,0);
        inWorld.rules[833] = (world) => SpecialLogic.areaAccess[12].Invoke(world,0,0);
        inWorld.rules[1381] = (world) => SpecialLogic.areaAccess[12].Invoke(world,0,0);
        inWorld.rules[876] = (world) => SpecialLogic.areaAccess[12].Invoke(world,0,0);
        inWorld.rules[175] = (world) => SpecialLogic.areaAccess[12].Invoke(world,0,0);
        inWorld.rules[236] = (world) => SpecialLogic.areaAccess[12].Invoke(world,0,0);
        inWorld.rules[176] = (world) => SpecialLogic.areaAccess[12].Invoke(world,0,0);
        inWorld.rules[943] = (world) => SpecialLogic.areaAccess[12].Invoke(world,0,0);
        inWorld.rules[944] = (world) => SpecialLogic.areaAccess[12].Invoke(world,0,0);
        inWorld.rules[942] = (world) => SpecialLogic.areaAccess[12].Invoke(world,0,0);
        inWorld.rules[857] = (world) => SpecialLogic.areaAccess[12].Invoke(world,0,0);
        inWorld.rules[379] = (world) => SpecialLogic.areaAccess[12].Invoke(world,0,0) && Rule.PlacedItem(world, new ItemType("3206", ItemCategory.items), 1);
        inWorld.rules[500] = (world) => SpecialLogic.areaAccess[12].Invoke(world,0,0) && Rule.PlacedItem(world, new ItemType("3206", ItemCategory.items), 1);
    }
}