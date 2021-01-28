using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace Model
{
    public class Competition
    {
        public List<IParticipant> Contestors { get; set; }
        public RaceData<ParticipantBreakDown> ContestorCountBroken { get; set; }
        public Queue<Track> Tracks { get; set; }
        public RaceData<ParticipantsPassing> ContestorOvertaken { get; set; }
        public RaceData<ParticipantTime> ContestorTime { get; set; }
        public RaceData<ParticipantSectionTime> ContestorSectionTime { get; set; }
        public RaceData<ParticipantPoints> ContestorPoints { get; set; }

        public Competition()
        {
            Contestors = new List<IParticipant>();
            Tracks = new Queue<Track>();
            ContestorPoints = new RaceData<ParticipantPoints>();
            ContestorOvertaken = new RaceData<ParticipantsPassing>();
            ContestorCountBroken = new RaceData<ParticipantBreakDown>();
            ContestorSectionTime = new RaceData<ParticipantSectionTime>();
            ContestorTime = new RaceData<ParticipantTime>();
        }

        public void AssignPoints(Dictionary<int, IParticipant> EndPositions)
        {
            int points = 30;

            // Assign points to contestors
            // 1: 30 points
            // 2: 24 points
            // 3: 18 points
            // 4: 12 points
            // 5: 6  points

            for (int i = 1; i <= EndPositions.Count; i++)
            {
                switch (i)
                {
                    case 1:
                        points -= 9;
                        break;
                    case 2:
                    case 3:
                        points -= 6;
                        break;
                    case 4:
                    case 5:
                        points -= 3;
                        break;
                }
                // Addds contestors to Contestor List
                IParticipant contestor = Contestors.First(p => p.Name == EndPositions[i].Name);
                contestor.Points += points;
                ContestorPoints.AssignToList(
                    new ParticipantPoints()
                    {
                        Name = EndPositions[i].Name,
                        points = points,
                        Contestor = contestor
                    });
                Debug.WriteLine($"{EndPositions[i].Name} heeft {points} punten toegewezen gekregen daarmee heeft deze deelnemer nu {contestor.Points} punten");
            }
        }


        // Creates new LapTime in list in RaceData participantdata 
        public void AddLapTime(IParticipant contestor, Track track, TimeSpan time)
        {
            ContestorTime.AssignToList(new ParticipantTime()
            {
                Name = contestor.Name,
                Track = track,
                Contestor = contestor,
                Time = time
            });
        }

        // Creates new SectionTime in list in RaceData participantdata 
        public void SetSectionTime(IParticipant contestor, TimeSpan time, Section section)
        {
            ContestorSectionTime.AssignToList(new ParticipantSectionTime()
            {
                Name = contestor.Name,
                Section = section,
                Time = time,
                Contestor = contestor
            });
        }

        // When contestor gets passed print message and add contestor to list of passers
        public void ContestorPassed(IParticipant passer, IParticipant passed)
        {
            Debug.Write($"{passed.Name} is ingehaald door {passer.Name}\n");
            ContestorOvertaken.AssignToList(new ParticipantsPassing(passer.Name, passed.Name));
        }

        public Track NextTrack()
        {
            Track returnTrack;
            Tracks.TryDequeue(out returnTrack);
            return returnTrack;
        }

        // Clean Data when a race ends
        public void CleanData()
        {
            ContestorTime?.ClearList();
            ContestorSectionTime?.ClearList();
            ContestorOvertaken?.ClearList();
        }

        // Add contestor to list when the contestor's car is broken
        public void ContestorBrokenCount(IParticipant contestor, int count)
        {
            ContestorCountBroken.AssignToList(new ParticipantBreakDown()
            {
                Name = contestor.Name,
                Count = count,
                Contestor = contestor
            });
        }
    }
}