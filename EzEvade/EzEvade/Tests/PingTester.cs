﻿using System;

using EloBuddy;
using EloBuddy.SDK;
using EloBuddy.SDK.Menu;

namespace ezEvade
{
    class PingTester
    {
        public static Menu testMenu;

        private static AIHeroClient myHero { get { return ObjectManager.Player; } }

        private static float lastTimerCheck = 0;
        private static bool lastRandomMoveCoeff = false;

        private static float sumPingTime = 0;
        private static float averagePingTime = ObjectCache.gamePing;
        private static int testCount = 0;
        private static int autoTestCount = 0;
        private static float maxPingTime = ObjectCache.gamePing;

        private static bool autoTestPing = false;

        private static EvadeCommand lastTestMoveToCommand;

        public PingTester()
        {
            Game.OnUpdate += Game_OnGameUpdate;
            
            //testMenu = new Menu("Ping Tester", "PingTest", true);
            //testMenu.AddItem(new MenuItem("AutoSetPing", "Auto Set Ping").SetValue(false));
            //testMenu.AddItem(new MenuItem("TestMoveTime", "Test Ping").SetValue(false));
            //testMenu.AddItem(new MenuItem("SetMaxPing", "Set Max Ping").SetValue(false));
            //testMenu.AddItem(new MenuItem("SetAvgPing", "Set Avg Ping").SetValue(false));
            //testMenu.AddItem(new MenuItem("Test20MoveTime", "Test Ping x20").SetValue(false));
            //testMenu.AddItem(new MenuItem("PrintResults", "Print Results").SetValue(false));
            //testMenu.AddToMainMenu();
        }

        private void IssueTestMove(int recursionCount)
        {

            var movePos = ObjectCache.myHeroCache.serverPos2D;

            Random rand = new Random();

            lastRandomMoveCoeff = !lastRandomMoveCoeff;
            if (lastRandomMoveCoeff)
            {
                movePos.X += 65 + rand.Next(0, 20);
            }
            else
            {
                movePos.X -= 65 + rand.Next(0, 20);
            }

            lastTestMoveToCommand = new EvadeCommand
            {
                order = EvadeOrderCommand.MoveTo,
                targetPosition = movePos,
                timestamp = EvadeUtils.TickCount,
                isProcessed = false
            };
            //myHero.IssueOrder(GameObjectOrder.MoveTo, movePos.To3D(), true);

            if (recursionCount > 1)
            {
                DelayAction.Add(500, () => IssueTestMove(recursionCount - 1));
            }

        }

        private void SetPing(int ping)
        {
            //Evade.menu.Item("ExtraPingBuffer").SetValue(new Slider(ping, 0, 200));
        }

        private void Game_OnGameUpdate(EventArgs args)
        {
            //if (testMenu.Item("AutoSetPing").Cast<CheckBox>().CurrentValue)
            //{
            //    Console.WriteLine("Testing Ping...Please wait 10 seconds");

            //    int testAmount = 20;

            //    testMenu.Item("AutoSetPing").SetValue(false);
            //    IssueTestMove(testAmount);
            //    autoTestCount = testCount + testAmount;
            //    autoTestPing = true;
                
            //}

            //if (testMenu.Item("PrintResults").Cast<CheckBox>().CurrentValue)
            //{
            //    testMenu.Item("PrintResults").SetValue(false);

            //    Console.WriteLine("Average Extra Delay: " + averagePingTime);
            //    Console.WriteLine("Max Extra Delay: " + maxPingTime);
            //}

            if (autoTestPing == true && testCount >= autoTestCount)
            {
                Console.WriteLine("Auto Set Ping Complete");

                Console.WriteLine("Average Extra Delay: " + averagePingTime);
                Console.WriteLine("Max Extra Delay: " + maxPingTime);

                SetPing((int)(averagePingTime+10));
                Console.WriteLine("Set Average extra ping + 10: " + (averagePingTime+10));

                autoTestPing = false;
            }

            //if (testMenu.Item("TestMoveTime").Cast<CheckBox>().CurrentValue)
            //{
            //    testMenu.Item("TestMoveTime").SetValue(false);
            //    IssueTestMove(1);
            //}


            //if (testMenu.Item("Test20MoveTime").Cast<CheckBox>().CurrentValue)
            //{
            //    testMenu.Item("Test20MoveTime").SetValue(false);
            //    IssueTestMove(20);
            //}

            //if (testMenu.Item("SetMaxPing").Cast<CheckBox>().CurrentValue)
            //{
            //    testMenu.Item("SetMaxPing").SetValue(false);

            //    if (testCount < 10)
            //    {
            //        Console.WriteLine("Please test 10 times before setting ping");
            //    }
            //    else
            //    {
            //        Console.WriteLine("Set Max extra ping: " + maxPingTime);
            //        SetPing((int)maxPingTime);
            //    }                
            //}

            //if (testMenu.Item("SetAvgPing").Cast<CheckBox>().CurrentValue)
            //{
            //    testMenu.Item("SetAvgPing").SetValue(false);

            //    if (testCount < 10)
            //    {
            //        Console.WriteLine("Please test 10 times before setting ping");
            //    }
            //    else
            //    {
            //        Console.WriteLine("Set Average extra ping: " + averagePingTime);
            //        SetPing((int)averagePingTime);
            //    }                         
            //}

            if (myHero.IsMoving)
            {
                if (lastTestMoveToCommand != null && lastTestMoveToCommand.isProcessed == false && lastTestMoveToCommand.order == EvadeOrderCommand.MoveTo)
                {
                    var path = myHero.Path;

                    if (path.Length > 0)
                    {
                        var movePos = path[path.Length - 1].To2D();

                        if (movePos.Distance(lastTestMoveToCommand.targetPosition) < 10)
                        {
                            float moveTime = EvadeUtils.TickCount - lastTestMoveToCommand.timestamp - ObjectCache.gamePing;
                            Console.WriteLine("Extra Delay: " + moveTime);
                            lastTestMoveToCommand.isProcessed = true;

                            sumPingTime += moveTime;
                            testCount += 1;
                            averagePingTime = sumPingTime / testCount;
                            maxPingTime = Math.Max(maxPingTime, moveTime);
                        }
                    }

                }
            }
        }
    }
}
