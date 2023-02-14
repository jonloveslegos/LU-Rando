using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using LURando.Models;

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
                foreach (var item in world.placedLocations.Where(item => item.reward_item1.ToString() == itemId.name))
                {
                    amount += Math.Max(1, item.reward_item1_count);
                }
                foreach (var item in world.placedLocations.Where(item => item.reward_item2.ToString() == itemId.name))
                {
                    amount += Math.Max(1, item.reward_item2_count);
                }
                foreach (var item in world.placedLocations.Where(item => item.reward_item3.ToString() == itemId.name))
                {
                    amount += Math.Max(1, item.reward_item3_count);
                }
                foreach (var item in world.placedLocations.Where(item => item.reward_item4.ToString() == itemId.name))
                {
                    amount += Math.Max(1, item.reward_item4_count);
                }
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
    public List<Missions> locations = new List<Missions>();
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
                locations.Add(new Missions(availableIds.Find(x => x.id == i)));
                locations[locations.Count - 1].reward_item1 = -999;
            }
        }
        for (int i = 0; i < 99; i++)
        {
            SpecialLogic.savedAreas.Add(false);
        }
        for (var i = 0; i <= 2077; i++)
        {
            rules.Add(i, (world) => SpecialLogic.savedAreas[12]);
        }
        availableIdsSaved = new List<Missions>(locations);
    }
    public void PlaceItem(int locationLocId, int spot)
    {
        int locationId = locations.FindIndex(x => x.id == locationLocId);
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

    public bool Randomize()
    {
        List<int> emptyLocs = locations.Where(x => x.reward_item1 == -999).Select(x => x.id).ToList();
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
        int backs = 0;
        var removedItems = new List<Missions>();
        while (emptyLocs.Count() > 0)
        {
            chosen = toChooseFrom[rng.Next(toChooseFrom.Count)];
            canPlace = false;
            placedIt = true;
            ind = 0;
            while (canPlace == false)
            {
                PlaceItem(chosen, ind);
                placedLocations.Add(new Missions(locations.Find(x => x.id == chosen)));
                emptyLocs.Remove(chosen);
                for (int i = 1; i < SpecialLogic.areaAccess.Count + 1; i++)
                {
                    if (SpecialLogic.savedAreas[i] == false)
                    {
                        SpecialLogic.savedAreas[i] = SpecialLogic.areaAccess[i].Invoke(this, 0, 0);
                    }
                }
                toChooseFrom = emptyLocs.Where(x => rules[x].Invoke(this)).ToList();
                if (toChooseFrom.Count > 0 || emptyLocs.Count() <= 0)
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
                    locations[locations.FindIndex(x => x.id == chosen)] = new Missions(availableIdsSaved.Find(x => x.id == chosen));
                    locations[locations.FindIndex(x => x.id == chosen)].reward_item1 = -999;
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
                Console.Write("Placed item.\n" + (toChooseFrom.Count) + " locations accessible.\n" + (emptyLocs.Count) + " locations left.\n" + ((currency.Count + emotes.Count + imagine.Count + inv.Count + exp.Count + items.Count) / 12f).ToString() + " items left.\n");
            }
            else
            {
                backs++;
                if (backs >= 50)
                {
                    return false;
                }
                removedItems.Clear();
                removedItems.Add(placedLocations[placedLocations.Count - 1]);
                emptyLocs.Add(placedLocations[placedLocations.Count - 1].id);
                var tempind = locations.FindIndex(x => x.id == placedLocations[placedLocations.Count - 1].id);
                locations[tempind] = new Missions(availableIdsSaved[tempind]);
                locations[tempind].reward_item1 = -999;
                placedLocations.RemoveAt(placedLocations.Count - 1);
                removedItems.Add(placedLocations[placedLocations.Count - 1]);
                emptyLocs.Add(placedLocations[placedLocations.Count - 1].id);
                tempind = locations.FindIndex(x => x.id == placedLocations[placedLocations.Count - 1].id);
                locations[tempind] = new Missions(availableIdsSaved[tempind]);
                locations[tempind].reward_item1 = -999;
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
                for (int i = 1; i < SpecialLogic.areaAccess.Count + 1; i++)
                {
                    SpecialLogic.savedAreas[i] = SpecialLogic.areaAccess[i].Invoke(this, 0, 0);
                }
                toChooseFrom = emptyLocs.Where(x => rules[x].Invoke(this)).ToList();
            }
        }
        return true;
    }

    public void AddItems()
    {
        items = new List<ItemType>();
        currency = new List<ItemType>();
        emotes = new List<ItemType>();
        imagine = new List<ItemType>();
        inv = new List<ItemType>();
        exp = new List<ItemType>();
        var itemsTxt = File.OpenText(Directory.GetCurrentDirectory() + "/temp/items.txt");
        while (!itemsTxt.EndOfStream)
        {
            items.Add(new ItemType(itemsTxt.ReadLine() + ":" + itemsTxt.ReadLine(), ItemCategory.items));
        }
        itemsTxt.Close();
        var emoteTxt = File.OpenText(Directory.GetCurrentDirectory() + "/temp/emotes.txt");
        while (!emoteTxt.EndOfStream)
        {
            emotes.Add(new ItemType(emoteTxt.ReadLine(), ItemCategory.emotes));
        }
        emoteTxt.Close();
        var currencyTxt = File.OpenText(Directory.GetCurrentDirectory() + "/temp/currency.txt");
        while (!currencyTxt.EndOfStream)
        {
            currency.Add(new ItemType(currencyTxt.ReadLine(), ItemCategory.currency));
        }
        currencyTxt.Close();
        var expTxt = File.OpenText(Directory.GetCurrentDirectory() + "/temp/exp.txt");
        while (!expTxt.EndOfStream)
        {
            exp.Add(new ItemType(expTxt.ReadLine(), ItemCategory.exp));
        }
        expTxt.Close();
        var invTxt = File.OpenText(Directory.GetCurrentDirectory() + "/temp/inventory.txt");
        while (!invTxt.EndOfStream)
        {
            inv.Add(new ItemType(invTxt.ReadLine(), ItemCategory.inv));
        }
        invTxt.Close();
        var imaginationTxt = File.OpenText(Directory.GetCurrentDirectory() + "/temp/imagination.txt");
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
        foreach (var item in world.placedLocations.Where(x => x.reward_item1 == int.Parse("10012")))
        {
            foundCount += item.reward_item1_count * 2;
        }
        foreach (var item in world.placedLocations.Where(x => x.reward_item1 == int.Parse("10095")))
        {
            foundCount += item.reward_item1_count * 5;
        }
        foreach (var item in world.placedLocations.Where(x => x.reward_item1 == int.Parse("10096")))
        {
            foundCount += item.reward_item1_count * 10;
        }
        foreach (var item in world.placedLocations.Where(x => x.reward_item1 == int.Parse("14125")))
        {
            foundCount += item.reward_item1_count * 50;
        }
        foreach (var item in world.placedLocations.Where(x => x.reward_item1 == int.Parse("14126")))
        {
            foundCount += item.reward_item1_count * 100;
        }
        foreach (var item in world.placedLocations.Where(x => x.reward_item1 == int.Parse("12424")))
        {
            foundCount += item.reward_item1_count * 15;
        }
        foreach (var item in world.placedLocations.Where(x => x.reward_item1 == int.Parse("12425")))
        {
            foundCount += item.reward_item1_count * 20;
        }
        foreach (var item in world.placedLocations.Where(x => x.reward_item1 == int.Parse("14127")))
        {
            foundCount += item.reward_item1_count * 200;
        }


        foreach (var item in world.placedLocations.Where(x => x.reward_item2 == int.Parse("10012")))
        {
            foundCount += item.reward_item2_count * 2;
        }
        foreach (var item in world.placedLocations.Where(x => x.reward_item2 == int.Parse("10095")))
        {
            foundCount += item.reward_item2_count * 5;
        }
        foreach (var item in world.placedLocations.Where(x => x.reward_item2 == int.Parse("10096")))
        {
            foundCount += item.reward_item2_count * 10;
        }
        foreach (var item in world.placedLocations.Where(x => x.reward_item2 == int.Parse("14125")))
        {
            foundCount += item.reward_item2_count * 50;
        }
        foreach (var item in world.placedLocations.Where(x => x.reward_item2 == int.Parse("14126")))
        {
            foundCount += item.reward_item2_count * 100;
        }
        foreach (var item in world.placedLocations.Where(x => x.reward_item2 == int.Parse("12424")))
        {
            foundCount += item.reward_item2_count * 15;
        }
        foreach (var item in world.placedLocations.Where(x => x.reward_item2 == int.Parse("12425")))
        {
            foundCount += item.reward_item2_count * 20;
        }
        foreach (var item in world.placedLocations.Where(x => x.reward_item2 == int.Parse("14127")))
        {
            foundCount += item.reward_item2_count * 200;
        }


        foreach (var item in world.placedLocations.Where(x => x.reward_item3 == int.Parse("10012")))
        {
            foundCount += item.reward_item3_count * 2;
        }
        foreach (var item in world.placedLocations.Where(x => x.reward_item3 == int.Parse("10095")))
        {
            foundCount += item.reward_item3_count * 5;
        }
        foreach (var item in world.placedLocations.Where(x => x.reward_item3 == int.Parse("10096")))
        {
            foundCount += item.reward_item3_count * 10;
        }
        foreach (var item in world.placedLocations.Where(x => x.reward_item3 == int.Parse("14125")))
        {
            foundCount += item.reward_item3_count * 50;
        }
        foreach (var item in world.placedLocations.Where(x => x.reward_item3 == int.Parse("14126")))
        {
            foundCount += item.reward_item3_count * 100;
        }
        foreach (var item in world.placedLocations.Where(x => x.reward_item3 == int.Parse("12424")))
        {
            foundCount += item.reward_item3_count * 15;
        }
        foreach (var item in world.placedLocations.Where(x => x.reward_item3 == int.Parse("12425")))
        {
            foundCount += item.reward_item3_count * 20;
        }
        foreach (var item in world.placedLocations.Where(x => x.reward_item3 == int.Parse("14127")))
        {
            foundCount += item.reward_item3_count * 200;
        }


        foreach (var item in world.placedLocations.Where(x => x.reward_item4 == int.Parse("10012")))
        {
            foundCount += item.reward_item4_count * 2;
        }
        foreach (var item in world.placedLocations.Where(x => x.reward_item4 == int.Parse("10095")))
        {
            foundCount += item.reward_item4_count * 5;
        }
        foreach (var item in world.placedLocations.Where(x => x.reward_item4 == int.Parse("10096")))
        {
            foundCount += item.reward_item4_count * 10;
        }
        foreach (var item in world.placedLocations.Where(x => x.reward_item4 == int.Parse("14125")))
        {
            foundCount += item.reward_item4_count * 50;
        }
        foreach (var item in world.placedLocations.Where(x => x.reward_item4 == int.Parse("14126")))
        {
            foundCount += item.reward_item4_count * 100;
        }
        foreach (var item in world.placedLocations.Where(x => x.reward_item4 == int.Parse("12424")))
        {
            foundCount += item.reward_item4_count * 15;
        }
        foreach (var item in world.placedLocations.Where(x => x.reward_item4 == int.Parse("12425")))
        {
            foundCount += item.reward_item4_count * 20;
        }
        foreach (var item in world.placedLocations.Where(x => x.reward_item4 == int.Parse("14127")))
        {
            foundCount += item.reward_item4_count * 200;
        }
        return foundCount >= requiredAmount;
    }
    public static List<bool> savedAreas = new List<bool>();
    public static Dictionary<int, Func<World, int, int, bool>> areaAccess = new Dictionary<int, Func<World, int, int, bool>>
    {
        {1, (w, s, g) => SpecialLogic.CanCompleteTutorial(w) && Rule.PlacedItem(w, new ItemType("6086", ItemCategory.items), 1)},
        {2, (w, s, g) => (Rule.PlacedItem(w, new ItemType("4880", ItemCategory.items), 1) || Rule.PlacedItem(w, new ItemType("4881", ItemCategory.items), 1) || Rule.PlacedItem(w, new ItemType("4883", ItemCategory.items), 1)) && SpecialLogic.savedAreas[1]},
        {3, (w, s, g) => Rule.PlacedItem(w, new ItemType("356", ItemCategory.emotes), 1) && SpecialLogic.savedAreas[2]},
        {4, (w, s, g) => Rule.PlacedItem(w, new ItemType("-1", ItemCategory.currency), s + 100) && SpecialLogic.savedAreas[3]},
        {5, (w, s, g) =>  Rule.PlacedItem(w, new ItemType("-1", ItemCategory.currency), s + 100) && SpecialLogic.savedAreas[4]},
        {6, (w, s, g) => Rule.PlacedItem(w, new ItemType("3039", ItemCategory.items), g + 1) && SpecialLogic.savedAreas[5]},
        {7, (w, s, g) =>  Rule.PlacedItem(w, new ItemType("14592", ItemCategory.items), 1) && SpecialLogic.savedAreas[6]},
        {8, (w, s, g) =>  Rule.PlacedItem(w, new ItemType("14553", ItemCategory.items), 1) && SpecialLogic.savedAreas[7]},
        {9, (w, s, g) =>  (Rule.PlacedItem(w, new ItemType("14359", ItemCategory.items), 1) || Rule.PlacedItem(w, new ItemType("14321", ItemCategory.items), 1) || Rule.PlacedItem(w, new ItemType("14353", ItemCategory.items), 1) || Rule.PlacedItem(w, new ItemType("14315", ItemCategory.items), 1)) && SpecialLogic.savedAreas[8]},
        {10, (w, s, g) =>  (Rule.PlacedItem(w, new ItemType("14359", ItemCategory.items), 2) || Rule.PlacedItem(w, new ItemType("14321", ItemCategory.items), 2) || Rule.PlacedItem(w, new ItemType("14353", ItemCategory.items), 2) || Rule.PlacedItem(w, new ItemType("14315", ItemCategory.items), 2)) && SpecialLogic.savedAreas[9]},
        {11, (w, s, g) => (Rule.PlacedItem(w, new ItemType("9516", ItemCategory.items), 1) || Rule.PlacedItem(w, new ItemType("9517", ItemCategory.items), 1) || Rule.PlacedItem(w, new ItemType("9518", ItemCategory.items), 1)) && SpecialLogic.savedAreas[10]},
        {12, (w, s, g) =>
        ((LURando.Program.chosenFaction == 0 && (Rule.PlacedItem(w, new ItemType("8033", ItemCategory.items), 1) || Rule.PlacedItem(w, new ItemType("8031", ItemCategory.items), 1) || Rule.PlacedItem(w, new ItemType("7591", ItemCategory.items), 1))) ||
        (LURando.Program.chosenFaction == 1 && (Rule.PlacedItem(w, new ItemType("7586", ItemCategory.items), 1) || Rule.PlacedItem(w, new ItemType("8032", ItemCategory.items), 1) || Rule.PlacedItem(w, new ItemType("8034", ItemCategory.items), 1))) ||
        (LURando.Program.chosenFaction == 2 && (Rule.PlacedItem(w, new ItemType("7589", ItemCategory.items), 1) || Rule.PlacedItem(w, new ItemType("8029", ItemCategory.items), 1) || Rule.PlacedItem(w, new ItemType("8030", ItemCategory.items), 1))) ||
        (LURando.Program.chosenFaction == 3 && (Rule.PlacedItem(w, new ItemType("7590", ItemCategory.items), 1) || Rule.PlacedItem(w, new ItemType("8028", ItemCategory.items), 1) || Rule.PlacedItem(w, new ItemType("8027", ItemCategory.items), 1)))
        ) && SpecialLogic.savedAreas[11]},
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
        inWorld.rules[311] = (world) => SpecialLogic.savedAreas[1];
        inWorld.rules[1728] = (world) => SpecialLogic.savedAreas[1];
        inWorld.rules[353] = (world) => SpecialLogic.savedAreas[1];
        inWorld.rules[355] = (world) => SpecialLogic.savedAreas[1];
        inWorld.rules[356] = (world) => SpecialLogic.savedAreas[1];
        inWorld.rules[251] = (world) => SpecialLogic.savedAreas[1];
        inWorld.rules[252] = (world) => SpecialLogic.savedAreas[1];
        inWorld.rules[706] = (world) => SpecialLogic.savedAreas[1];
        inWorld.rules[14] = (world) => SpecialLogic.savedAreas[1];
        inWorld.rules[707] = (world) => SpecialLogic.savedAreas[1];
        inWorld.rules[287] = (world) => SpecialLogic.savedAreas[1];
        inWorld.rules[755] = (world) => SpecialLogic.savedAreas[2];
        inWorld.rules[312] = (world) => SpecialLogic.savedAreas[2];
        inWorld.rules[314] = (world) => SpecialLogic.savedAreas[2];
        inWorld.rules[259] = (world) => SpecialLogic.savedAreas[3];
        inWorld.rules[315] = (world) => SpecialLogic.savedAreas[3];
        inWorld.rules[16] = (world) => SpecialLogic.savedAreas[4];
        inWorld.rules[733] = (world) => SpecialLogic.savedAreas[4];
        inWorld.rules[316] = (world) => SpecialLogic.savedAreas[4];
        inWorld.rules[939] = (world) => SpecialLogic.savedAreas[5];
        inWorld.rules[940] = (world) => SpecialLogic.savedAreas[5];
        inWorld.rules[479] = (world) => SpecialLogic.savedAreas[6];
        inWorld.rules[1847] = (world) => SpecialLogic.savedAreas[6];
        inWorld.rules[1848] = (world) => SpecialLogic.savedAreas[6];
        inWorld.rules[477] = (world) => SpecialLogic.savedAreas[6];
        inWorld.rules[260] = (world) => SpecialLogic.savedAreas[7];
        inWorld.rules[1151] = (world) => SpecialLogic.savedAreas[7];
        inWorld.rules[1849] = (world) => SpecialLogic.savedAreas[7];
        inWorld.rules[1850] = (world) => SpecialLogic.savedAreas[8];
        inWorld.rules[1851] = (world) => SpecialLogic.savedAreas[8];
        inWorld.rules[1852] = (world) => SpecialLogic.savedAreas[8];
        inWorld.rules[1935] = (world) => SpecialLogic.savedAreas[9];
        inWorld.rules[313] = (world) => SpecialLogic.savedAreas[9];
        inWorld.rules[1853] = (world) => SpecialLogic.savedAreas[9];
        inWorld.rules[1936] = (world) => SpecialLogic.savedAreas[9];
        inWorld.rules[317] = (world) => SpecialLogic.savedAreas[9];
        inWorld.rules[1854] = (world) => SpecialLogic.savedAreas[9];
        inWorld.rules[1855] = (world) => SpecialLogic.savedAreas[9];
        inWorld.rules[1856] = (world) => SpecialLogic.savedAreas[9];
        inWorld.rules[318] = (world) => SpecialLogic.savedAreas[9];
        inWorld.rules[633] = (world) => SpecialLogic.savedAreas[9];
        inWorld.rules[244] = (world) => SpecialLogic.savedAreas[9];
        inWorld.rules[377] = (world) => SpecialLogic.savedAreas[9];
        inWorld.rules[1894] = (world) => SpecialLogic.savedAreas[9];
        inWorld.rules[208] = (world) => SpecialLogic.savedAreas[9];
        inWorld.rules[261] = (world) => SpecialLogic.savedAreas[9];
        inWorld.rules[708] = (world) => SpecialLogic.savedAreas[9];
        inWorld.rules[783] = (world) => SpecialLogic.savedAreas[9];
        inWorld.rules[346] = (world) => SpecialLogic.savedAreas[9];
        inWorld.rules[347] = (world) => SpecialLogic.savedAreas[9];
        inWorld.rules[348] = (world) => SpecialLogic.savedAreas[9];
        inWorld.rules[349] = (world) => SpecialLogic.savedAreas[9];
        inWorld.rules[709] = (world) => SpecialLogic.savedAreas[9];
        inWorld.rules[250] = (world) => SpecialLogic.savedAreas[9];
        inWorld.rules[282] = (world) => SpecialLogic.savedAreas[9];
        inWorld.rules[1950] = (world) => SpecialLogic.savedAreas[10];
        inWorld.rules[768] = (world) => SpecialLogic.savedAreas[10];
        inWorld.rules[871] = (world) => SpecialLogic.savedAreas[10];
        inWorld.rules[891] = (world) => SpecialLogic.savedAreas[10];
        inWorld.rules[320] = (world) => SpecialLogic.savedAreas[10];
        inWorld.rules[1877] = (world) => SpecialLogic.savedAreas[10];
        inWorld.rules[319] = (world) => SpecialLogic.savedAreas[10];
        inWorld.rules[470] = (world) => SpecialLogic.savedAreas[10];
        inWorld.rules[325] = (world) => SpecialLogic.savedAreas[10];
        inWorld.rules[286] = (world) => SpecialLogic.savedAreas[10];
        inWorld.rules[1293] = (world) => SpecialLogic.savedAreas[10];
        inWorld.rules[957] = (world) => SpecialLogic.savedAreas[10];
        inWorld.rules[1153] = (world) => SpecialLogic.savedAreas[10];
        inWorld.rules[322] = (world) => SpecialLogic.savedAreas[10];
        inWorld.rules[1152] = (world) => SpecialLogic.savedAreas[10];
        inWorld.rules[445] = (world) => SpecialLogic.savedAreas[10];
        inWorld.rules[324] = (world) => SpecialLogic.savedAreas[10];
        inWorld.rules[680] = (world) => SpecialLogic.savedAreas[10];
        inWorld.rules[938] = (world) => SpecialLogic.savedAreas[10];
        inWorld.rules[483] = (world) => SpecialLogic.savedAreas[10];
        inWorld.rules[476] = (world) => SpecialLogic.savedAreas[10];
        inWorld.rules[809] = (world) => SpecialLogic.savedAreas[11];
        inWorld.rules[475] = (world) => SpecialLogic.savedAreas[11];
        inWorld.rules[478] = (world) => SpecialLogic.savedAreas[11];
        inWorld.rules[482] = (world) => SpecialLogic.savedAreas[11];
        inWorld.rules[588] = (world) => SpecialLogic.savedAreas[11];
        inWorld.rules[589] = (world) => SpecialLogic.savedAreas[11];
        inWorld.rules[590] = (world) => SpecialLogic.savedAreas[11];
        inWorld.rules[591] = (world) => SpecialLogic.savedAreas[11];
        inWorld.rules[684] = (world) => SpecialLogic.savedAreas[11];
        inWorld.rules[685] = (world) => SpecialLogic.savedAreas[11];
        inWorld.rules[686] = (world) => SpecialLogic.savedAreas[11];
        inWorld.rules[687] = (world) => SpecialLogic.savedAreas[11];
        inWorld.rules[593] = (world) => SpecialLogic.savedAreas[11];
        inWorld.rules[256] = (world) => SpecialLogic.savedAreas[11];
        inWorld.rules[283] = (world) => SpecialLogic.savedAreas[11];
        inWorld.rules[537] = (world) => SpecialLogic.savedAreas[11];
        inWorld.rules[302] = (world) => SpecialLogic.savedAreas[11];
        inWorld.rules[595] = (world) => SpecialLogic.savedAreas[11];
        inWorld.rules[778] = (world) => SpecialLogic.savedAreas[11];
        inWorld.rules[446] = (world) => SpecialLogic.savedAreas[11];
        inWorld.rules[546] = (world) => SpecialLogic.savedAreas[11] && Rule.PlacedItem(world, new ItemType("7591",ItemCategory.items), 1);
        inWorld.rules[552] = (world) => SpecialLogic.savedAreas[11] && Rule.PlacedItem(world, new ItemType("8030",ItemCategory.items), 1);
        inWorld.rules[955] = (world) => SpecialLogic.savedAreas[11] && Rule.PlacedItem(world, new ItemType("8031", ItemCategory.items), 1);
        inWorld.rules[557] = (world) => SpecialLogic.savedAreas[11] && Rule.PlacedItem(world, new ItemType("7586", ItemCategory.items), 1);
        inWorld.rules[563] = (world) => SpecialLogic.savedAreas[11] && Rule.PlacedItem(world, new ItemType("8032", ItemCategory.items), 1);
        inWorld.rules[952] = (world) => SpecialLogic.savedAreas[11] && Rule.PlacedItem(world, new ItemType("8034", ItemCategory.items), 1);
        inWorld.rules[579] = (world) => SpecialLogic.savedAreas[11] && Rule.PlacedItem(world, new ItemType("7589", ItemCategory.items), 1);
        inWorld.rules[582] = (world) => SpecialLogic.savedAreas[11] && Rule.PlacedItem(world, new ItemType("8029", ItemCategory.items), 1);
        inWorld.rules[953] = (world) => SpecialLogic.savedAreas[11] && Rule.PlacedItem(world, new ItemType("8030", ItemCategory.items), 1);
        inWorld.rules[568] = (world) => SpecialLogic.savedAreas[11] && Rule.PlacedItem(world, new ItemType("7590", ItemCategory.items), 1);
        inWorld.rules[574] = (world) => SpecialLogic.savedAreas[11] && Rule.PlacedItem(world, new ItemType("8028", ItemCategory.items), 1);
        inWorld.rules[954] = (world) => SpecialLogic.savedAreas[11] && Rule.PlacedItem(world, new ItemType("8027", ItemCategory.items), 1);
        inWorld.rules[1472] = (world) => SpecialLogic.savedAreas[11] && Rule.PlacedItem(world, new ItemType("7591", ItemCategory.items), 1);
        inWorld.rules[1475] = (world) => SpecialLogic.savedAreas[11] && Rule.PlacedItem(world, new ItemType("8030", ItemCategory.items), 1);
        inWorld.rules[1478] = (world) => SpecialLogic.savedAreas[11] && Rule.PlacedItem(world, new ItemType("8031", ItemCategory.items), 1);
        inWorld.rules[1490] = (world) => SpecialLogic.savedAreas[11] && Rule.PlacedItem(world, new ItemType("7586", ItemCategory.items), 1);
        inWorld.rules[1493] = (world) => SpecialLogic.savedAreas[11] && Rule.PlacedItem(world, new ItemType("8032", ItemCategory.items), 1);
        inWorld.rules[1496] = (world) => SpecialLogic.savedAreas[11] && Rule.PlacedItem(world, new ItemType("8034", ItemCategory.items), 1);
        inWorld.rules[1481] = (world) => SpecialLogic.savedAreas[11] && Rule.PlacedItem(world, new ItemType("7589", ItemCategory.items), 1);
        inWorld.rules[1484] = (world) => SpecialLogic.savedAreas[11] && Rule.PlacedItem(world, new ItemType("8029", ItemCategory.items), 1);
        inWorld.rules[1487] = (world) => SpecialLogic.savedAreas[11] && Rule.PlacedItem(world, new ItemType("8030", ItemCategory.items), 1);
        inWorld.rules[1372] = (world) => SpecialLogic.savedAreas[11] && Rule.PlacedItem(world, new ItemType("7590", ItemCategory.items), 1);
        inWorld.rules[1466] = (world) => SpecialLogic.savedAreas[11] && Rule.PlacedItem(world, new ItemType("8028", ItemCategory.items), 1);
        inWorld.rules[1469] = (world) => SpecialLogic.savedAreas[11] && Rule.PlacedItem(world, new ItemType("8027", ItemCategory.items), 1);
        inWorld.rules[812] = (world) => SpecialLogic.savedAreas[12];
        inWorld.rules[813] = (world) => SpecialLogic.savedAreas[12];
        inWorld.rules[814] = (world) => SpecialLogic.savedAreas[12];
        inWorld.rules[815] = (world) => SpecialLogic.savedAreas[12];
        inWorld.rules[1889] = (world) => SpecialLogic.savedAreas[12];
        inWorld.rules[1890] = (world) => SpecialLogic.savedAreas[12];
        inWorld.rules[1891] = (world) => SpecialLogic.savedAreas[12];
        inWorld.rules[1892] = (world) => SpecialLogic.savedAreas[12];
        inWorld.rules[867] = (world) => SpecialLogic.savedAreas[12] && SpecialLogic.TokenCount(world, 25);
        inWorld.rules[866] = (world) => SpecialLogic.savedAreas[12] && SpecialLogic.TokenCount(world, 25);
        inWorld.rules[865] = (world) => SpecialLogic.savedAreas[12] && SpecialLogic.TokenCount(world, 25);
        inWorld.rules[767] = (world) => SpecialLogic.savedAreas[12] && SpecialLogic.TokenCount(world, 25);
        inWorld.rules[779] = (world) => SpecialLogic.savedAreas[12];
        inWorld.rules[833] = (world) => SpecialLogic.savedAreas[12];
        inWorld.rules[1381] = (world) => SpecialLogic.savedAreas[12];
        inWorld.rules[876] = (world) => SpecialLogic.savedAreas[12];
        inWorld.rules[175] = (world) => SpecialLogic.savedAreas[12];
        inWorld.rules[236] = (world) => SpecialLogic.savedAreas[12];
        inWorld.rules[176] = (world) => SpecialLogic.savedAreas[12];
        inWorld.rules[943] = (world) => SpecialLogic.savedAreas[12];
        inWorld.rules[944] = (world) => SpecialLogic.savedAreas[12];
        inWorld.rules[942] = (world) => SpecialLogic.savedAreas[12];
        inWorld.rules[857] = (world) => SpecialLogic.savedAreas[12];
        inWorld.rules[379] = (world) => SpecialLogic.savedAreas[12] && Rule.PlacedItem(world, new ItemType("3206", ItemCategory.items), 1);
        inWorld.rules[500] = (world) => SpecialLogic.savedAreas[12] && Rule.PlacedItem(world, new ItemType("3206", ItemCategory.items), 1);
    }
}