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
        if (itemId.category == ItemCategory.items) 
        {
            var amount = 0;
            foreach (var item in world.placedItems.Where(x => x.Value.reward_item1.ToString() == itemId.name))
            {
                amount += item.Value.reward_item1_count;
            }
            foreach (var item in world.placedItems.Where(x => x.Value.reward_item2.ToString() == itemId.name))
            {
                amount += item.Value.reward_item2_count;
            }
            foreach (var item in world.placedItems.Where(x => x.Value.reward_item3.ToString() == itemId.name))
            {
                amount += item.Value.reward_item3_count;
            }
            foreach (var item in world.placedItems.Where(x => x.Value.reward_item4.ToString() == itemId.name))
            {
                amount += item.Value.reward_item4_count;
            }
            if (amount >= count)
            {
                return true;
            }
        }
        if (itemId.category == ItemCategory.emotes)
        {
            var amount = 0;
            foreach (var item in world.placedLocations.Where(x => x.Value.reward_emote.ToString() == itemId.name))
            {
                amount += 1;
            }
            foreach (var item in world.placedLocations.Where(x => x.Value.reward_emote2.ToString() == itemId.name))
            {
                amount += 1;
            }
            foreach (var item in world.placedLocations.Where(x => x.Value.reward_emote3.ToString() == itemId.name))
            {
                amount += 1;
            }
            foreach (var item in world.placedLocations.Where(x => x.Value.reward_emote4.ToString() == itemId.name))
            {
                amount += 1;
            }
            if (amount >= count)
            {
                return true;
            }
        }
        if (itemId.category == ItemCategory.currency)
        {
            var amount = 0;
            foreach (var item in world.placedLocations.Where(x => x.Value.reward_currency > 0))
            {
                amount += (int)item.Value.reward_currency;
            }
            if (amount >= count)
            {
                return true;
            }
        }
        if (itemId.category == ItemCategory.imagine)
        {
            var amount = 0;
            foreach (var item in world.placedLocations.Where(x => x.Value.reward_maximagination > 0))
            {
                amount += item.Value.reward_maximagination;
            }
            if (amount >= count)
            {
                return true;
            }
        }
        if (itemId.category == ItemCategory.inv)
        {
            var amount = 0;
            foreach (var item in world.placedLocations.Where(x => x.Value.reward_maxinventory > 0))
            {
                amount += item.Value.reward_maxinventory;
            }
            if (amount >= count)
            {
                return true;
            }
        }
        if (itemId.category == ItemCategory.exp)
        {
            var amount = 0;
            foreach (var item in world.placedLocations.Where(x => x.Value.LegoScore > 0))
            {
                amount += item.Value.LegoScore;
            }
            if (amount >= count)
            {
                return true;
            }
        }
        return false;
    }
    public static bool ReturnTrue(World world)
    {
        return true;
    }
}

public class World
{
    public Dictionary<int,Missions> locations = new Dictionary<int, Missions>();
    public Dictionary<int, Missions> placedLocations = new Dictionary<int, Missions>();
    public Dictionary<int, Missions> placedItems = new Dictionary<int, Missions>();
    public List<ItemType> items = new List<ItemType>();
    public List<ItemType> emotes = new List<ItemType>();
    public List<ItemType> imagine = new List<ItemType>();
    public List<ItemType> inv = new List<ItemType>();
    public List<ItemType> currency = new List<ItemType>();
    public List<ItemType> exp = new List<ItemType>();
    public List<Predicate<World>> rules = new List<Predicate<World>>();
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
            rules.Add((world) => SpecialLogic.AreaAccess(world, 12));
        }
        availableIdsSaved = availableIds;
    }
    public void PlaceItem(int locationLocId, ItemType itemListId)
    {
        int locationId = locations.First(x => x.Value.id == locationLocId).Key;
        if (locations[locationId].reward_item1 == -999)
        {
            locations[locationId].reward_item1 = -1;
        }
        if (itemListId.category == ItemCategory.items)
        {
            if (locations[locationId].reward_item1 == -1)
            {
                locations[locationId].reward_item1 = int.Parse(itemListId.name.Split(':')[0]);
                locations[locationId].reward_item1_count = int.Parse(itemListId.name.Split(':')[1]);
            }
            else if (locations[locationId].reward_item2 == -1)
            {
                locations[locationId].reward_item2 = int.Parse(itemListId.name.Split(':')[0]);
                locations[locationId].reward_item2_count = int.Parse(itemListId.name.Split(':')[1]);
            }
            else if (locations[locationId].reward_item3 == -1)
            {
                locations[locationId].reward_item3 = int.Parse(itemListId.name.Split(':')[0]);
                locations[locationId].reward_item3_count = int.Parse(itemListId.name.Split(':')[1]);
            }
            else if (locations[locationId].reward_item4 == -1)
            {
                locations[locationId].reward_item4 = int.Parse(itemListId.name.Split(':')[0]);
                locations[locationId].reward_item4_count = int.Parse(itemListId.name.Split(':')[1]);
            }
        }
        if (itemListId.category == ItemCategory.currency)
        {
            locations[locationId].reward_currency = long.Parse(itemListId.name);
        }
        if (itemListId.category == ItemCategory.exp)
        {
            locations[locationId].LegoScore = int.Parse(itemListId.name);
        }
        if (itemListId.category == ItemCategory.imagine)
        {
            locations[locationId].reward_maximagination = int.Parse(itemListId.name);
        }
        if (itemListId.category == ItemCategory.inv)
        {
            locations[locationId].reward_maxinventory = int.Parse(itemListId.name);
        }
        if (itemListId.category == ItemCategory.emotes)
        {
            if (locations[locationId].reward_emote == -1)
            {
                locations[locationId].reward_emote = int.Parse(itemListId.name);
            }
            else if (locations[locationId].reward_emote2 == -1)
            {
                locations[locationId].reward_emote2 = int.Parse(itemListId.name);
            }
            else if (locations[locationId].reward_emote3 == -1)
            {
                locations[locationId].reward_emote3 = int.Parse(itemListId.name);
            }
            else if (locations[locationId].reward_emote4 == -1)
            {
                locations[locationId].reward_emote4 = int.Parse(itemListId.name);
            }
        }
    }

    public void Randomize()
    {
        var backupLocations = new Dictionary<int, Missions>(locations);
        var removedItems = new List<Missions>();
        List<int> emptyLocs = new List<int>();
        for (int i = 0; i < locations.Count; i++)
        {
            if (locations[i].reward_item1 == -999)
            {
                emptyLocs.Add(locations[i].id);
            }
        }
        List<int> toChooseFrom = new List<int>();
        for (int i = 0; i < emptyLocs.Count; i++)
        {
            if (rules[emptyLocs[i]].Invoke(this))
            {
                toChooseFrom.Add(emptyLocs[i]);
            }
        }
        int lastSuccess = 999999;
        int timeSinceBack = 0;
        int backCount = 0;
        for (var iii = 0; iii < locations.Where(x => x.Value.reward_item1 == -999).Count(); iii = 0)
        {
            toChooseFrom = new List<int>();
            for (int i = 0; i < emptyLocs.Count; i++)
            {
                if (rules[emptyLocs[i]].Invoke(this))
                {
                    toChooseFrom.Add(emptyLocs[i]);
                }
            }
            if ((currency.Count + emotes.Count + imagine.Count + inv.Count + exp.Count + items.Count) <= 0)
            {
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

            }
            else if (toChooseFrom.Count <= 0)
            {
                Missions tempLoc;
                int tempId;
                lastSuccess--;
                tempLoc = placedLocations[placedLocations.Count - 1];
                removedItems.Add(new Missions(tempLoc));
                tempId = locations.First(x => x.Value.id == placedLocations[placedLocations.Count - 1].id).Key;
                locations[tempId] = new Missions(availableIdsSaved.First(x => x.id == placedLocations[placedLocations.Count - 1].id));
                locations[tempId].id = placedLocations[placedLocations.Count - 1].id;
                locations[tempId].reward_item1 = -999;
                emptyLocs.Add(placedLocations[placedLocations.Count - 1].id);
                if (placedLocations[placedLocations.Count - 1].reward_item1 != -1 || placedLocations[placedLocations.Count - 1].reward_item2 != -1 || placedLocations[placedLocations.Count - 1].reward_item3 != -1 || placedLocations[placedLocations.Count - 1].reward_item4 != -1)
                {
                    placedItems.Remove(placedItems.Count - 1);
                }
                placedLocations.Remove(placedLocations.Count - 1);
                timeSinceBack = 0;
                backCount++;
            }
            else
            {
                int chosen = toChooseFrom[rng.Next(toChooseFrom.Count)];
                var chosenItem = 0;
                chosenItem = rng.Next(currency.Count);
                PlaceItem(chosen, currency[chosenItem]);
                currency.RemoveAt(chosenItem);
                chosenItem = rng.Next(imagine.Count);
                PlaceItem(chosen, imagine[chosenItem]);
                imagine.RemoveAt(chosenItem);
                chosenItem = rng.Next(inv.Count);
                PlaceItem(chosen, inv[chosenItem]);
                inv.RemoveAt(chosenItem);
                chosenItem = rng.Next(exp.Count);
                PlaceItem(chosen, exp[chosenItem]);
                exp.RemoveAt(chosenItem);
                chosenItem = rng.Next(emotes.Count);
                PlaceItem(chosen, emotes[chosenItem]);
                emotes.RemoveAt(chosenItem);
                chosenItem = rng.Next(emotes.Count);
                PlaceItem(chosen, emotes[chosenItem]);
                emotes.RemoveAt(chosenItem);
                chosenItem = rng.Next(emotes.Count);
                PlaceItem(chosen, emotes[chosenItem]);
                emotes.RemoveAt(chosenItem);
                chosenItem = rng.Next(emotes.Count);
                PlaceItem(chosen, emotes[chosenItem]);
                emotes.RemoveAt(chosenItem);
                chosenItem = rng.Next(items.Count);
                PlaceItem(chosen, items[chosenItem]);
                items.RemoveAt(chosenItem);
                chosenItem = rng.Next(items.Count);
                PlaceItem(chosen, items[chosenItem]);
                items.RemoveAt(chosenItem);
                chosenItem = rng.Next(items.Count);
                PlaceItem(chosen, items[chosenItem]);
                items.RemoveAt(chosenItem);
                chosenItem = rng.Next(items.Count);
                PlaceItem(chosen, items[chosenItem]);
                items.RemoveAt(chosenItem);
                emptyLocs.Remove(chosen);
                placedLocations.Add(placedLocations.Count,new Missions(locations[locations.First(x => x.Value.id == chosen).Key]));
                if (placedLocations[placedLocations.Count-1].reward_item1 != -1 || placedLocations[placedLocations.Count - 1].reward_item2 != -1 || placedLocations[placedLocations.Count - 1].reward_item3 != -1 || placedLocations[placedLocations.Count - 1].reward_item4 != -1)
                {
                    placedItems.Add(placedItems.Count, new Missions(placedLocations[placedLocations.Count - 1]));
                }
                if (timeSinceBack > 0)
                {
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
                }
                timeSinceBack++;
                Console.Write("Placed item.\n" + (toChooseFrom.Count) + " locations accessible.\n" + (emptyLocs.Count) + " locations left.\n" + ((currency.Count + emotes.Count + imagine.Count + inv.Count + exp.Count + items.Count) / 12f).ToString() + " items left.");
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
    public static bool AreaAccess(World world, int areaId, int spent = 0, int greenSpent = 0)
    {
        if (areaId == 1)
        {
            return SpecialLogic.CanCompleteTutorial(world) && Rule.PlacedItem(world, new ItemType("6086", ItemCategory.items), 1);
        }
        if (areaId == 2)
        {
            return SpecialLogic.AreaAccess(world, areaId-1, spent, greenSpent) && (Rule.PlacedItem(world, new ItemType("4880", ItemCategory.items), 1) || Rule.PlacedItem(world, new ItemType("4881", ItemCategory.items), 1) || Rule.PlacedItem(world, new ItemType("4883", ItemCategory.items), 1));
        }
        if (areaId == 3)
        {
            return SpecialLogic.AreaAccess(world, areaId - 1, spent, greenSpent) && Rule.PlacedItem(world, new ItemType("356", ItemCategory.emotes), 1);
        }
        if (areaId == 4)
        {
            return SpecialLogic.AreaAccess(world, areaId - 1, spent+1000, greenSpent) && Rule.PlacedItem(world, new ItemType("-1", ItemCategory.currency), spent + 1000);
        }
        if (areaId == 5)
        {
            return SpecialLogic.AreaAccess(world, areaId - 1, spent+1000, greenSpent) && Rule.PlacedItem(world, new ItemType("-1", ItemCategory.currency), spent + 1000);
        }
        if (areaId == 6)
        {
            return SpecialLogic.AreaAccess(world, areaId - 1, spent, greenSpent + 1) && Rule.PlacedItem(world, new ItemType("3039", ItemCategory.items), greenSpent + 1);
        }
        if (areaId == 7)
        {
            return SpecialLogic.AreaAccess(world, areaId - 1, spent, greenSpent) && Rule.PlacedItem(world, new ItemType("14592", ItemCategory.items), 1);
        }
        if (areaId == 8)
        {
            return SpecialLogic.AreaAccess(world, areaId - 1, spent, greenSpent) && Rule.PlacedItem(world, new ItemType("14553", ItemCategory.items), 1);
        }
        if (areaId == 9)
        {
            return SpecialLogic.AreaAccess(world, areaId - 1, spent, greenSpent) && (Rule.PlacedItem(world, new ItemType("14359", ItemCategory.items), 1) || Rule.PlacedItem(world, new ItemType("14321", ItemCategory.items), 1) || Rule.PlacedItem(world, new ItemType("14353", ItemCategory.items), 1) || Rule.PlacedItem(world, new ItemType("14315", ItemCategory.items), 1));
        }
        if (areaId == 10)
        {
            return SpecialLogic.AreaAccess(world, areaId - 1, spent, greenSpent) && (Rule.PlacedItem(world, new ItemType("14359", ItemCategory.items), 2) || Rule.PlacedItem(world, new ItemType("14321", ItemCategory.items), 2) || Rule.PlacedItem(world, new ItemType("14353", ItemCategory.items), 2) || Rule.PlacedItem(world, new ItemType("14315", ItemCategory.items), 2));
        }
        if (areaId == 11)
        {
            return SpecialLogic.AreaAccess(world, areaId - 1, spent, greenSpent) && (Rule.PlacedItem(world, new ItemType("9516", ItemCategory.items), 1) || Rule.PlacedItem(world, new ItemType("9517", ItemCategory.items), 1) || Rule.PlacedItem(world, new ItemType("9518",ItemCategory.items), 1));
        }
        if (areaId == 12)
        {
            return SpecialLogic.AreaAccess(world, areaId - 1, spent, greenSpent) && 
                ((SQLiteWithCSharp.Program.chosenFaction == 0 && (Rule.PlacedItem(world, new ItemType("8033", ItemCategory.items), 1) || Rule.PlacedItem(world, new ItemType("8031", ItemCategory.items), 1) || Rule.PlacedItem(world, new ItemType("7591", ItemCategory.items), 1))) ||
                (SQLiteWithCSharp.Program.chosenFaction == 1 && (Rule.PlacedItem(world, new ItemType("7586", ItemCategory.items), 1) || Rule.PlacedItem(world, new ItemType("8032", ItemCategory.items), 1) || Rule.PlacedItem(world, new ItemType("8034", ItemCategory.items), 1))) ||
                (SQLiteWithCSharp.Program.chosenFaction == 2 && (Rule.PlacedItem(world, new ItemType("7589", ItemCategory.items), 1) || Rule.PlacedItem(world, new ItemType("8029", ItemCategory.items), 1) || Rule.PlacedItem(world, new ItemType("8030", ItemCategory.items), 1))) ||
                (SQLiteWithCSharp.Program.chosenFaction == 3 && (Rule.PlacedItem(world, new ItemType("7590", ItemCategory.items), 1) || Rule.PlacedItem(world, new ItemType("8028", ItemCategory.items), 1) || Rule.PlacedItem(world, new ItemType("8027", ItemCategory.items), 1))));
        }
        return true;
    }
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
        foreach (var item in world.placedItems.Where(x => x.Value.reward_item1 == int.Parse("10012")))
        {
            foundCount += item.Value.reward_item1_count * 2;
        }
        foreach (var item in world.placedItems.Where(x => x.Value.reward_item1 == int.Parse("10095")))
        {
            foundCount += item.Value.reward_item1_count * 5;
        }
        foreach (var item in world.placedItems.Where(x => x.Value.reward_item1 == int.Parse("10096")))
        {
            foundCount += item.Value.reward_item1_count * 10;
        }
        foreach (var item in world.placedItems.Where(x => x.Value.reward_item1 == int.Parse("14125")))
        {
            foundCount += item.Value.reward_item1_count * 50;
        }
        foreach (var item in world.placedItems.Where(x => x.Value.reward_item1 == int.Parse("14126")))
        {
            foundCount += item.Value.reward_item1_count * 100;
        }
        foreach (var item in world.placedItems.Where(x => x.Value.reward_item1 == int.Parse("12424")))
        {
            foundCount += item.Value.reward_item1_count * 15;
        }
        foreach (var item in world.placedItems.Where(x => x.Value.reward_item1 == int.Parse("12425")))
        {
            foundCount += item.Value.reward_item1_count * 20;
        }
        foreach (var item in world.placedItems.Where(x => x.Value.reward_item1 == int.Parse("14127")))
        {
            foundCount += item.Value.reward_item1_count * 200;
        }


        foreach (var item in world.placedItems.Where(x => x.Value.reward_item2 == int.Parse("10012")))
        {
            foundCount += item.Value.reward_item2_count * 2;
        }
        foreach (var item in world.placedItems.Where(x => x.Value.reward_item2 == int.Parse("10095")))
        {
            foundCount += item.Value.reward_item2_count * 5;
        }
        foreach (var item in world.placedItems.Where(x => x.Value.reward_item2 == int.Parse("10096")))
        {
            foundCount += item.Value.reward_item2_count * 10;
        }
        foreach (var item in world.placedItems.Where(x => x.Value.reward_item2 == int.Parse("14125")))
        {
            foundCount += item.Value.reward_item2_count * 50;
        }
        foreach (var item in world.placedItems.Where(x => x.Value.reward_item2 == int.Parse("14126")))
        {
            foundCount += item.Value.reward_item2_count * 100;
        }
        foreach (var item in world.placedItems.Where(x => x.Value.reward_item2 == int.Parse("12424")))
        {
            foundCount += item.Value.reward_item2_count * 15;
        }
        foreach (var item in world.placedItems.Where(x => x.Value.reward_item2 == int.Parse("12425")))
        {
            foundCount += item.Value.reward_item2_count * 20;
        }
        foreach (var item in world.placedItems.Where(x => x.Value.reward_item2 == int.Parse("14127")))
        {
            foundCount += item.Value.reward_item2_count * 200;
        }


        foreach (var item in world.placedItems.Where(x => x.Value.reward_item3 == int.Parse("10012")))
        {
            foundCount += item.Value.reward_item3_count * 2;
        }
        foreach (var item in world.placedItems.Where(x => x.Value.reward_item3 == int.Parse("10095")))
        {
            foundCount += item.Value.reward_item3_count * 5;
        }
        foreach (var item in world.placedItems.Where(x => x.Value.reward_item3 == int.Parse("10096")))
        {
            foundCount += item.Value.reward_item3_count * 10;
        }
        foreach (var item in world.placedItems.Where(x => x.Value.reward_item3 == int.Parse("14125")))
        {
            foundCount += item.Value.reward_item3_count * 50;
        }
        foreach (var item in world.placedItems.Where(x => x.Value.reward_item3 == int.Parse("14126")))
        {
            foundCount += item.Value.reward_item3_count * 100;
        }
        foreach (var item in world.placedItems.Where(x => x.Value.reward_item3 == int.Parse("12424")))
        {
            foundCount += item.Value.reward_item3_count * 15;
        }
        foreach (var item in world.placedItems.Where(x => x.Value.reward_item3 == int.Parse("12425")))
        {
            foundCount += item.Value.reward_item3_count * 20;
        }
        foreach (var item in world.placedItems.Where(x => x.Value.reward_item3 == int.Parse("14127")))
        {
            foundCount += item.Value.reward_item3_count * 200;
        }


        foreach (var item in world.placedItems.Where(x => x.Value.reward_item4 == int.Parse("10012")))
        {
            foundCount += item.Value.reward_item4_count * 2;
        }
        foreach (var item in world.placedItems.Where(x => x.Value.reward_item4 == int.Parse("10095")))
        {
            foundCount += item.Value.reward_item4_count * 5;
        }
        foreach (var item in world.placedItems.Where(x => x.Value.reward_item4 == int.Parse("10096")))
        {
            foundCount += item.Value.reward_item4_count * 10;
        }
        foreach (var item in world.placedItems.Where(x => x.Value.reward_item4 == int.Parse("14125")))
        {
            foundCount += item.Value.reward_item4_count * 50;
        }
        foreach (var item in world.placedItems.Where(x => x.Value.reward_item4 == int.Parse("14126")))
        {
            foundCount += item.Value.reward_item4_count * 100;
        }
        foreach (var item in world.placedItems.Where(x => x.Value.reward_item4 == int.Parse("12424")))
        {
            foundCount += item.Value.reward_item4_count * 15;
        }
        foreach (var item in world.placedItems.Where(x => x.Value.reward_item4 == int.Parse("12425")))
        {
            foundCount += item.Value.reward_item4_count * 20;
        }
        foreach (var item in world.placedItems.Where(x => x.Value.reward_item4 == int.Parse("14127")))
        {
            foundCount += item.Value.reward_item4_count * 200;
        }
        return foundCount >= requiredAmount;
    }
    public static bool UnlockedAreaCount(World world, int amountToUnlock)
    {
        int unlocked = 0;
        if (SpecialLogic.AreaAccess(world, 1))
        {
            unlocked++;
        }
        if (SpecialLogic.AreaAccess(world, 2))
        {
            unlocked++;
        }
        if (SpecialLogic.AreaAccess(world, 3))
        {
            unlocked++;
        }
        if (SpecialLogic.AreaAccess(world, 4))
        {
            unlocked++;
        }
        return (unlocked >= amountToUnlock);
    }
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
        inWorld.rules[311] = (world) => SpecialLogic.AreaAccess(world, 1);
        inWorld.rules[1728] = (world) => SpecialLogic.AreaAccess(world, 1);
        inWorld.rules[353] = (world) => SpecialLogic.AreaAccess(world, 1);
        inWorld.rules[355] = (world) => SpecialLogic.AreaAccess(world, 1);
        inWorld.rules[356] = (world) => SpecialLogic.AreaAccess(world, 1);
        inWorld.rules[251] = (world) => SpecialLogic.AreaAccess(world, 1);
        inWorld.rules[252] = (world) => SpecialLogic.AreaAccess(world, 1);
        inWorld.rules[706] = (world) => SpecialLogic.AreaAccess(world, 1);
        inWorld.rules[707] = (world) => SpecialLogic.AreaAccess(world, 1);
        inWorld.rules[287] = (world) => SpecialLogic.AreaAccess(world, 1);
        inWorld.rules[755] = (world) => SpecialLogic.AreaAccess(world, 2);
        inWorld.rules[312] = (world) => SpecialLogic.AreaAccess(world, 2);
        inWorld.rules[314] = (world) => SpecialLogic.AreaAccess(world, 2);
        inWorld.rules[259] = (world) => SpecialLogic.AreaAccess(world, 3);
        inWorld.rules[315] = (world) => SpecialLogic.AreaAccess(world, 3);
        inWorld.rules[733] = (world) => SpecialLogic.AreaAccess(world, 4);
        inWorld.rules[316] = (world) => SpecialLogic.AreaAccess(world, 4);
        inWorld.rules[939] = (world) => SpecialLogic.AreaAccess(world, 5);
        inWorld.rules[940] = (world) => SpecialLogic.AreaAccess(world, 5);
        inWorld.rules[479] = (world) => SpecialLogic.AreaAccess(world, 6);
        inWorld.rules[1847] = (world) => SpecialLogic.AreaAccess(world, 6);
        inWorld.rules[1848] = (world) => SpecialLogic.AreaAccess(world, 6);
        inWorld.rules[477] = (world) => SpecialLogic.AreaAccess(world, 6);
        inWorld.rules[260] = (world) => SpecialLogic.AreaAccess(world, 7);
        inWorld.rules[1151] = (world) => SpecialLogic.AreaAccess(world, 7);
        inWorld.rules[1849] = (world) => SpecialLogic.AreaAccess(world, 7);
        inWorld.rules[1850] = (world) => SpecialLogic.AreaAccess(world, 8);
        inWorld.rules[1851] = (world) => SpecialLogic.AreaAccess(world, 8);
        inWorld.rules[1852] = (world) => SpecialLogic.AreaAccess(world, 8);
        inWorld.rules[1935] = (world) => SpecialLogic.AreaAccess(world, 9);
        inWorld.rules[313] = (world) => SpecialLogic.AreaAccess(world, 9);
        inWorld.rules[1853] = (world) => SpecialLogic.AreaAccess(world, 9);
        inWorld.rules[1936] = (world) => SpecialLogic.AreaAccess(world, 9);
        inWorld.rules[317] = (world) => SpecialLogic.AreaAccess(world, 9);
        inWorld.rules[1854] = (world) => SpecialLogic.AreaAccess(world, 9);
        inWorld.rules[1855] = (world) => SpecialLogic.AreaAccess(world, 9);
        inWorld.rules[1856] = (world) => SpecialLogic.AreaAccess(world, 9);
        inWorld.rules[318] = (world) => SpecialLogic.AreaAccess(world, 9);
        inWorld.rules[633] = (world) => SpecialLogic.AreaAccess(world, 9);
        inWorld.rules[244] = (world) => SpecialLogic.AreaAccess(world, 9);
        inWorld.rules[377] = (world) => SpecialLogic.AreaAccess(world, 9);
        inWorld.rules[1894] = (world) => SpecialLogic.AreaAccess(world, 9);
        inWorld.rules[208] = (world) => SpecialLogic.AreaAccess(world, 9);
        inWorld.rules[261] = (world) => SpecialLogic.AreaAccess(world, 9);
        inWorld.rules[708] = (world) => SpecialLogic.AreaAccess(world, 9);
        inWorld.rules[783] = (world) => SpecialLogic.AreaAccess(world, 9);
        inWorld.rules[346] = (world) => SpecialLogic.AreaAccess(world, 9);
        inWorld.rules[347] = (world) => SpecialLogic.AreaAccess(world, 9);
        inWorld.rules[348] = (world) => SpecialLogic.AreaAccess(world, 9);
        inWorld.rules[349] = (world) => SpecialLogic.AreaAccess(world, 9);
        inWorld.rules[709] = (world) => SpecialLogic.AreaAccess(world, 9);
        inWorld.rules[250] = (world) => SpecialLogic.AreaAccess(world, 9);
        inWorld.rules[282] = (world) => SpecialLogic.AreaAccess(world, 9);
        inWorld.rules[1950] = (world) => SpecialLogic.AreaAccess(world, 10);
        inWorld.rules[768] = (world) => SpecialLogic.AreaAccess(world, 10);
        inWorld.rules[871] = (world) => SpecialLogic.AreaAccess(world, 10);
        inWorld.rules[891] = (world) => SpecialLogic.AreaAccess(world, 10);
        inWorld.rules[320] = (world) => SpecialLogic.AreaAccess(world, 10);
        inWorld.rules[1877] = (world) => SpecialLogic.AreaAccess(world, 10);
        inWorld.rules[319] = (world) => SpecialLogic.AreaAccess(world, 10);
        inWorld.rules[470] = (world) => SpecialLogic.AreaAccess(world, 10);
        inWorld.rules[325] = (world) => SpecialLogic.AreaAccess(world, 10);
        inWorld.rules[286] = (world) => SpecialLogic.AreaAccess(world, 10);
        inWorld.rules[1293] = (world) => SpecialLogic.AreaAccess(world, 10);
        inWorld.rules[957] = (world) => SpecialLogic.AreaAccess(world, 10);
        inWorld.rules[1153] = (world) => SpecialLogic.AreaAccess(world, 10);
        inWorld.rules[322] = (world) => SpecialLogic.AreaAccess(world, 10);
        inWorld.rules[1152] = (world) => SpecialLogic.AreaAccess(world, 10);
        inWorld.rules[324] = (world) => SpecialLogic.AreaAccess(world, 10);
        inWorld.rules[680] = (world) => SpecialLogic.AreaAccess(world, 10);
        inWorld.rules[938] = (world) => SpecialLogic.AreaAccess(world, 10);
        inWorld.rules[483] = (world) => SpecialLogic.AreaAccess(world, 10);
        inWorld.rules[476] = (world) => SpecialLogic.AreaAccess(world, 10);
        inWorld.rules[809] = (world) => SpecialLogic.AreaAccess(world, 11);
        inWorld.rules[475] = (world) => SpecialLogic.AreaAccess(world, 11);
        inWorld.rules[478] = (world) => SpecialLogic.AreaAccess(world, 11);
        inWorld.rules[482] = (world) => SpecialLogic.AreaAccess(world, 11);
        inWorld.rules[588] = (world) => SpecialLogic.AreaAccess(world, 11);
        inWorld.rules[589] = (world) => SpecialLogic.AreaAccess(world, 11);
        inWorld.rules[590] = (world) => SpecialLogic.AreaAccess(world, 11);
        inWorld.rules[591] = (world) => SpecialLogic.AreaAccess(world, 11);
        inWorld.rules[684] = (world) => SpecialLogic.AreaAccess(world, 11);
        inWorld.rules[685] = (world) => SpecialLogic.AreaAccess(world, 11);
        inWorld.rules[686] = (world) => SpecialLogic.AreaAccess(world, 11);
        inWorld.rules[687] = (world) => SpecialLogic.AreaAccess(world, 11);
        inWorld.rules[593] = (world) => SpecialLogic.AreaAccess(world, 11);
        inWorld.rules[256] = (world) => SpecialLogic.AreaAccess(world, 11);
        inWorld.rules[283] = (world) => SpecialLogic.AreaAccess(world, 11);
        inWorld.rules[537] = (world) => SpecialLogic.AreaAccess(world, 11);
        inWorld.rules[302] = (world) => SpecialLogic.AreaAccess(world, 11);
        inWorld.rules[595] = (world) => SpecialLogic.AreaAccess(world, 11);
        inWorld.rules[812] = (world) => SpecialLogic.AreaAccess(world, 12);
        inWorld.rules[813] = (world) => SpecialLogic.AreaAccess(world, 12);
        inWorld.rules[814] = (world) => SpecialLogic.AreaAccess(world, 12);
        inWorld.rules[815] = (world) => SpecialLogic.AreaAccess(world, 12);
        inWorld.rules[1889] = (world) => SpecialLogic.AreaAccess(world, 12);
        inWorld.rules[1890] = (world) => SpecialLogic.AreaAccess(world, 12);
        inWorld.rules[1891] = (world) => SpecialLogic.AreaAccess(world, 12);
        inWorld.rules[1892] = (world) => SpecialLogic.AreaAccess(world, 12);
        inWorld.rules[867] = (world) => SpecialLogic.AreaAccess(world, 12) && SpecialLogic.TokenCount(world, 25);
        inWorld.rules[866] = (world) => SpecialLogic.AreaAccess(world, 12) && SpecialLogic.TokenCount(world, 25);
        inWorld.rules[865] = (world) => SpecialLogic.AreaAccess(world, 12) && SpecialLogic.TokenCount(world, 25);
        inWorld.rules[767] = (world) => SpecialLogic.AreaAccess(world, 12) && SpecialLogic.TokenCount(world, 25);
        inWorld.rules[779] = (world) => SpecialLogic.AreaAccess(world, 12);
        inWorld.rules[833] = (world) => SpecialLogic.AreaAccess(world, 12);
        inWorld.rules[1381] = (world) => SpecialLogic.AreaAccess(world, 12);
        inWorld.rules[876] = (world) => SpecialLogic.AreaAccess(world, 12);
        inWorld.rules[175] = (world) => SpecialLogic.AreaAccess(world, 12);
        inWorld.rules[236] = (world) => SpecialLogic.AreaAccess(world, 12);
        inWorld.rules[176] = (world) => SpecialLogic.AreaAccess(world, 12);
        inWorld.rules[943] = (world) => SpecialLogic.AreaAccess(world, 12);
        inWorld.rules[944] = (world) => SpecialLogic.AreaAccess(world, 12);
        inWorld.rules[942] = (world) => SpecialLogic.AreaAccess(world, 12);
        inWorld.rules[857] = (world) => SpecialLogic.AreaAccess(world, 12);
        inWorld.rules[379] = (world) => SpecialLogic.AreaAccess(world, 12) && Rule.PlacedItem(world, new ItemType("3206", ItemCategory.items), 1);
        inWorld.rules[500] = (world) => SpecialLogic.AreaAccess(world, 12) && Rule.PlacedItem(world, new ItemType("3206", ItemCategory.items), 1);
    }
}