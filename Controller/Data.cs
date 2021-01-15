using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Model;

namespace Controller
{
    public static class Data
    {
        public static Competition Competition { get; set; }
        public static Race CurrentRace { get; set; }

        public static void Initialise(Competition competition)
        {
            Competition = competition;
            AddContestors();
            AddTracks();
        }


        // Creates new Contestors with Data and adds cars to the contestors
        static void AddContestors()
        {
            Competition.Contestors.Add(new Driver("Byron", new Car()) { TeamColour = TeamColours.Red });
            Competition.Contestors.Add(new Driver("Harvick", new Car()) { TeamColour = TeamColours.Green });
            Competition.Contestors.Add(new Driver("Dillon", new Car()) { TeamColour = TeamColours.Blue });
            Competition.Contestors.Add(new Driver("Custer", new Car()) { TeamColour = TeamColours.Yellow });
            Competition.Contestors.Add(new Driver("Larson", new Car()) { TeamColour = TeamColours.Orange });
        }

        // Creates Tracks for the contestorts to drive on
        static void AddTracks()
        {
            Track Phoenix_Raceway = new Track("Phoenix Raceway", new[]  {SectionTypes.StartGrid, SectionTypes.StartGrid, SectionTypes.StartGrid, SectionTypes.Straight, SectionTypes.RightCorner, SectionTypes.Straight, SectionTypes.Straight, SectionTypes.LeftCorner, SectionTypes.Straight, SectionTypes.Straight, SectionTypes.Straight, SectionTypes.LeftCorner, SectionTypes.Straight,
                SectionTypes.Straight, SectionTypes.RightCorner, SectionTypes.Straight, SectionTypes.Straight, SectionTypes.Straight, SectionTypes.RightCorner, SectionTypes.Straight, SectionTypes.Straight,
                SectionTypes.Straight, SectionTypes.Straight, SectionTypes.Straight, SectionTypes.Straight, SectionTypes.RightCorner, SectionTypes.Straight, SectionTypes.Straight, SectionTypes.Straight,
                SectionTypes.RightCorner, SectionTypes.Straight, SectionTypes.Straight, SectionTypes.LeftCorner, SectionTypes.Straight, SectionTypes.Straight, SectionTypes.Straight, SectionTypes.LeftCorner, SectionTypes.Straight, SectionTypes.Straight, SectionTypes.RightCorner, SectionTypes.Straight, SectionTypes.Straight,
                SectionTypes.Straight, SectionTypes.Straight, SectionTypes.RightCorner, SectionTypes.Straight, SectionTypes.RightCorner, SectionTypes.Straight, SectionTypes.Straight,SectionTypes.LeftCorner, SectionTypes.Straight, SectionTypes.Straight, SectionTypes.LeftCorner, SectionTypes.Straight, SectionTypes.Straight, SectionTypes.RightCorner, SectionTypes.Finish, SectionTypes.RightCorner, });


            Track Road_America = new Track("Road_America", new[] { SectionTypes.StartGrid, SectionTypes.StartGrid, SectionTypes.StartGrid, SectionTypes.Straight, SectionTypes.Straight, SectionTypes.RightCorner, SectionTypes.Straight, SectionTypes.Straight, SectionTypes.RightCorner, SectionTypes.Straight, SectionTypes.Straight, SectionTypes.Straight, SectionTypes.Straight, SectionTypes.Straight, SectionTypes.Straight,
                SectionTypes.RightCorner, SectionTypes.Straight, SectionTypes.Straight, SectionTypes.RightCorner, SectionTypes.Finish});

            Competition.Tracks.Enqueue(Phoenix_Raceway);

        }

        // When one race has ended a next track will be dequeud from Tracks
        public static void NextRace()
        {
            CurrentRace?.DisposeEventHandler();
            Track nextTrack = Competition.NextTrack();
            if (nextTrack != null)
            {
                PutParticipantsInOrderOfFinish();
                Competition.CleanData();
                CurrentRace = new Race(nextTrack, Competition.Contestors);
                CurrentRace.RaceFinished += OnRaceFinished;
                // When there are no more tracks, it knows that the race has ended
            }
            else
            {
                CurrentRace = null;
                if (!Console.IsOutputRedirected) Console.Clear();
                Console.WriteLine("Race is afgelopen");
                
            }
        }

        //public static void NextRace()
        //{
        //    CurrentRace?.DisposeEventHandler();
        //    Track nexttrack = Competition.NextTrack();
        //    if (nexttrack == null)
        //    {
        //        CurrentRace = null;
        //        if (!Console.IsOutputRedirected)
        //        {
        //            Console.WriteLine($"{Competition.Tracks} is afgelopen");
        //        }
        //    }
        //    else
        //    {
        //        PutParticipantsInOrderOfFinish();
        //        Competition.CleanData();
        //        CurrentRace = new Race(nexttrack, Competition.Contestors);
        //        CurrentRace.RaceFinished += OnRaceFinished;
        //    }
        //}

        // Sets the contestors in order when they have finished in th finalranking dictionary
        public static void PutParticipantsInOrderOfFinish()
        {
            if (CurrentRace == null) return;
            Competition.Contestors.Clear();
            Dictionary<int, IParticipant> finalRanking = CurrentRace.GetFinalRanking();
            for (int i = 1; i <= finalRanking.Count; i++)
            {
                Competition.Contestors.Add(finalRanking[i]);
            }
        }


        // Assign points to the contestor and initiates nextrace
        public static void OnRaceFinished(object sender, EventArgs e)
        {
            Competition.AssignPoints(CurrentRace.GetFinalRanking());
            NextRace();
        }
    }
}