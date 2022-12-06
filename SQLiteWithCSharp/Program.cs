using System;
using System.Collections.Generic;
using System.Linq;
using SQLiteWithCSharp.Common;
using SQLiteWithCSharp.Models;
using SQLiteWithCSharp.Services;
using SQLiteWithCSharp.Utility;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;

namespace SQLiteWithCSharp
{
    static class Program
    {
        static MainApp mainApp = new MainApp();
        static Random rand = new Random();
        public static int chosenFaction = -1;
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            mainApp.BuildConnectionString();
            var listMissions = new MissionService().GetAll().ToList();
            listMissions.Remove(listMissions.First(x => x.id == 875));
            listMissions.Remove(listMissions.First(x => x.id == 870));
            var tempMission = new Missions(listMissions.First(x => x.id == 474));
            listMissions.Remove(listMissions.First(x => x.id == 474));
            var tempFile = File.OpenText("Options.txt");
            chosenFaction = int.Parse(tempFile.ReadLine().Split('=')[1]);
            if (chosenFaction == 0)
            {
                listMissions.Remove(listMissions.First(x => x.id == 589));
                listMissions.Remove(listMissions.First(x => x.id == 590));
                listMissions.Remove(listMissions.First(x => x.id == 591));
                listMissions.Remove(listMissions.First(x => x.id == 812));
                listMissions.Remove(listMissions.First(x => x.id == 813));
                listMissions.Remove(listMissions.First(x => x.id == 814));
                tempMission.isChoiceReward = 0;
                tempMission.reward_item1 = 6979;
                tempMission.reward_item2 = -1;
                tempMission.reward_item3 = -1;
                tempMission.reward_item4 = -1;
                tempMission.reward_item1_count = 1;
                tempMission.reward_item2_count = 0;
                tempMission.reward_item3_count = 0;
                tempMission.reward_item4_count = 0;
                new MissionService().Update(tempMission);
            }
            if (chosenFaction == 1)
            {
                listMissions.Remove(listMissions.First(x => x.id == 588));
                listMissions.Remove(listMissions.First(x => x.id == 590));
                listMissions.Remove(listMissions.First(x => x.id == 591));
                listMissions.Remove(listMissions.First(x => x.id == 813));
                listMissions.Remove(listMissions.First(x => x.id == 814));
                listMissions.Remove(listMissions.First(x => x.id == 815));
                tempMission.isChoiceReward = 0;
                tempMission.reward_item1 = 6980;
                tempMission.reward_item2 = -1;
                tempMission.reward_item3 = -1;
                tempMission.reward_item4 = -1;
                tempMission.reward_item1_count = 1;
                tempMission.reward_item2_count = 0;
                tempMission.reward_item3_count = 0;
                tempMission.reward_item4_count = 0;
                new MissionService().Update(tempMission);
            }
            if (chosenFaction == 2)
            {
                listMissions.Remove(listMissions.First(x => x.id == 588));
                listMissions.Remove(listMissions.First(x => x.id == 589));
                listMissions.Remove(listMissions.First(x => x.id == 591));
                listMissions.Remove(listMissions.First(x => x.id == 812));
                listMissions.Remove(listMissions.First(x => x.id == 814));
                listMissions.Remove(listMissions.First(x => x.id == 815));
                tempMission.isChoiceReward = 0;
                tempMission.reward_item1 = 6981;
                tempMission.reward_item2 = -1;
                tempMission.reward_item3 = -1;
                tempMission.reward_item4 = -1;
                tempMission.reward_item1_count = 1;
                tempMission.reward_item2_count = 0;
                tempMission.reward_item3_count = 0;
                tempMission.reward_item4_count = 0;
                new MissionService().Update(tempMission);
            }
            if (chosenFaction == 3)
            {
                listMissions.Remove(listMissions.First(x => x.id == 588));
                listMissions.Remove(listMissions.First(x => x.id == 589));
                listMissions.Remove(listMissions.First(x => x.id == 590));
                listMissions.Remove(listMissions.First(x => x.id == 812));
                listMissions.Remove(listMissions.First(x => x.id == 813));
                listMissions.Remove(listMissions.First(x => x.id == 815));
                tempMission.isChoiceReward = 0;
                tempMission.reward_item1 = 6978;
                tempMission.reward_item2 = -1;
                tempMission.reward_item3 = -1;
                tempMission.reward_item4 = -1;
                tempMission.reward_item1_count = 1;
                tempMission.reward_item2_count = 0;
                tempMission.reward_item3_count = 0;
                tempMission.reward_item4_count = 0;
                new MissionService().Update(tempMission);
            }
            Missions foundMission = new Missions();
                var editingFile = File.CreateText(Directory.GetCurrentDirectory() + "/emotes.txt");
                for (int i = 0; i < listMissions.Count; i++)
                {
                    foundMission = listMissions[i];
                    if (foundMission.id > 0)
                    {
                        editingFile.Write(foundMission.reward_emote + "\n");
                        editingFile.Write(foundMission.reward_emote2 + "\n");
                        editingFile.Write(foundMission.reward_emote3 + "\n");
                        editingFile.Write(foundMission.reward_emote4);
                    }
                    if (i != listMissions.Count - 1)
                    {
                        editingFile.Write("\n");
                    }
                }
                editingFile.Close();
                editingFile = File.CreateText(Directory.GetCurrentDirectory() + "/items.txt");
                for (int i = 0; i < listMissions.Count; i++)
                {
                    foundMission = listMissions[i];
                    if (foundMission.id > 0)
                    {
                        editingFile.Write(foundMission.reward_item1 + "\n");
                        editingFile.Write(foundMission.reward_item1_count + "\n");
                        editingFile.Write(foundMission.reward_item2 + "\n");
                        editingFile.Write(foundMission.reward_item2_count + "\n");
                        editingFile.Write(foundMission.reward_item3 + "\n");
                        editingFile.Write(foundMission.reward_item3_count + "\n");
                        editingFile.Write(foundMission.reward_item4 + "\n");
                        editingFile.Write(foundMission.reward_item4_count);
                    }
                    if (i != listMissions.Count - 1)
                    {
                        editingFile.Write("\n");
                    }
                }
                editingFile.Close();
                editingFile = File.CreateText(Directory.GetCurrentDirectory() + "/currency.txt");
                for (int i = 0; i < listMissions.Count; i++)
                {
                    foundMission = listMissions[i];
                    if (foundMission.id > 0)
                    {
                        editingFile.Write(foundMission.reward_currency);
                    }
                    if (i != listMissions.Count - 1)
                    {
                        editingFile.Write("\n");
                    }
                }
                editingFile.Close();
                editingFile = File.CreateText(Directory.GetCurrentDirectory() + "/exp.txt");
                for (int i = 0; i < listMissions.Count; i++)
                {
                    foundMission = listMissions[i];
                    if (foundMission.id > 0)
                    {
                        editingFile.Write(foundMission.LegoScore);
                    }
                    if (i != listMissions.Count - 1)
                    {
                        editingFile.Write("\n");
                    }
                }
                editingFile.Close();
                editingFile = File.CreateText(Directory.GetCurrentDirectory() + "/imagination.txt");
                for (int i = 0; i < listMissions.Count; i++)
                {
                    foundMission = listMissions[i];
                    if (foundMission.id > 0)
                    {
                        editingFile.Write(foundMission.reward_maximagination);
                    }
                    if (i != listMissions.Count - 1)
                    {
                        editingFile.Write("\n");
                    }
                }
                editingFile.Close();
                editingFile = File.CreateText(Directory.GetCurrentDirectory() + "/inventory.txt");
                for (int i = 0; i < listMissions.Count; i++)
                {
                    foundMission = listMissions[i];
                    if (foundMission.id > 0)
                    {
                        editingFile.Write(foundMission.reward_maxinventory);
                    }
                    if (i != listMissions.Count - 1)
                    {
                        editingFile.Write("\n");
                    }
                }
                editingFile.Close();
            for (int i = 0; i < listMissions.Count; i++)
            {
                foundMission = listMissions[i];
                foundMission.isChoiceReward = 0;
                    foundMission.reward_item1 = -1;
                    foundMission.reward_item2 = -1;
                    foundMission.reward_item3 = -1;
                    foundMission.reward_item4 = -1;
                    foundMission.reward_emote = -1;
                    foundMission.reward_emote2 = -1;
                    foundMission.reward_emote3 = -1;
                    foundMission.reward_emote4 = -1;
                    foundMission.reward_item1_count = 0;
                    foundMission.reward_item2_count = 0;
                    foundMission.reward_item3_count = 0;
                    foundMission.reward_item4_count = 0;
                    foundMission.reward_item1_repeatable = -1;
                    foundMission.reward_item2_repeatable = -1;
                    foundMission.reward_item3_repeatable = -1;
                    foundMission.reward_item4_repeatable = -1;
                    foundMission.reward_item1_repeat_count = 0;
                    foundMission.reward_item2_repeat_count = 0;
                    foundMission.reward_item3_repeat_count = 0;
                    foundMission.reward_item4_repeat_count = 0;
                    foundMission.reward_currency = 0;
                    foundMission.reward_currency_repeatable = 0;
                    foundMission.reward_maximagination = 0;
                    foundMission.reward_maxinventory = 0;
                    foundMission.LegoScore = 0;
            }
            World world = new World(listMissions);
            Logic logic = new Logic();
            logic.SetLogic(world);
            world.AddItems();
            world.Randomize();
            for (int i = 0; i < listMissions.Count; i++)
            {
                Console.WriteLine(String.Format("Writing {0}/{1}", i, listMissions.Count));
                foundMission = world.locations.First(x => x.id == listMissions[i].id);
                new MissionService().Update(foundMission);
            }
            Console.WriteLine(String.Format("Writing {0}/{1}", listMissions.Count, listMissions.Count));
            //System.Diagnostics.Process.Start(".\\Application_Files\\sqlite-to-fdb.exe", ".\\Application_Files\\template.fdb .\\Application_Files\\DataBase\\CDServer.sqlite .\\Application_Files\\cdclient.fdb");
            Console.WriteLine("Done!");
        }
        public static void Shuffle<T>(this IList<T> list)
        {
            int n = list.Count;
            while (n > 1)
            {
                n--;
                int k = rand.Next(n + 1);
                T value = list[k];
                list[k] = list[n];
                list[n] = value;
            }
        }
    }
    class MainApp
    {
        public object MissionService { get; private set; }
        public void BuildConnectionString()
        {
            if (String.IsNullOrEmpty(Storage.ConnectionString))
            {
                Storage.ConnectionString = string.Format("Data Source={0};Version=3;", System.IO.Directory.GetCurrentDirectory()+System.Configuration.ConfigurationManager.AppSettings["DatabaseFile"]+ "\\DataBase\\CDServer.sqlite");
            }
        }

        public long InsertMissions(Missions newMissions)
        {
            return new MissionService().Add(newMissions);
        }
        public void UpdateMissions(Missions existingMissions)
        {
            new MissionService().Update(existingMissions);
        }

        public Missions GetMissions(Int32 id)
        {
            return new MissionService().GetById(id);
        }
    }
}
