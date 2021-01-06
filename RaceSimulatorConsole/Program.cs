using System;
using System.Threading;
using Controller;
using Model;

namespace RaceSimulator
{
    class Program
    {
        static void Main(string[] args)
        {
            Data.Initialise(new Competition());
            Data.NextRace();
            Display.Initialise();
            Display.DrawTrack(Data.CurrentRace.Track);

            for (; ; )
            {
                Thread.Sleep(100);
            }
        }
    }
}