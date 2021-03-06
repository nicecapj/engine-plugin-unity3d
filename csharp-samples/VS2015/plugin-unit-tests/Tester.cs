﻿// vim: fileencoding=utf-8 expandtab tabstop=4 softtabstop=4 shiftwidth=4
//
// Copyright (C) 2013-2016 iFunFactory Inc. All Rights Reserved.
//
// This work is confidential and proprietary to iFunFactory Inc and
// must not be used, disclosed, copied, or distributed without the prior
// consent of iFunFactory Inc.

using Fun;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Threading;


namespace plugin_unit_tests
{
    [TestClass]
    public class Tester
    {
        const int kClientMax = 100;
        const string kServerIp = "127.0.0.1";

        SessionOption option = new SessionOption();
        List<Client> list_ = new List<Client>();


        [TestMethod]
        public void TestSession ()
        {
            FunapiEncryptor.public_key = "0b8504a9c1108584f4f0a631ead8dd548c0101287b91736566e13ead3f008f5d";

            new Tester().start();
        }


        void start ()
        {
            writeTitle("START");
            FunDebug.Log("Client count is {0}.", kClientMax);

            option.sessionReliability = false;
            option.sendSessionIdOnlyOnce = false;

            for (int i = 0; i < kClientMax; ++i)
            {
                Client client = new Client(i, kServerIp);
                list_.Add(client);
            }

            Thread t = new Thread(new ThreadStart(onUpdate));
            t.Start();

            foreach (Client c in list_)
            {
                c.Connect(option);
                Thread.Sleep(10);
            }

            while (waitForConnect())
            {
                Thread.Sleep(500);
            }
            writeTitle("All sessions are connected.");

            foreach (Client c in list_)
            {
                c.SendMessage();
                Thread.Sleep(10);
            }

            while (waitForStop())
            {
                Thread.Sleep(500);
            }

            t.Abort();
            writeTitle("FINISHED");
        }

        void onUpdate ()
        {
            while (true)
            {
                foreach (Client c in list_)
                {
                    c.Update();
                }

                Thread.Sleep(33);
            }
        }

        bool waitForConnect ()
        {
            foreach (Client c in list_)
            {
                if (!c.Connected)
                    return true;
            }

            return false;
        }

        bool waitForStop ()
        {
            foreach (Client c in list_)
            {
                if (c.Connected)
                    return true;
            }

            return false;
        }

        void writeTitle (string message)
        {
            Console.WriteLine("");
            Console.WriteLine("---------------------- "
                              + message
                              + " -----------------------");
            Console.WriteLine("");
        }
    }
}
