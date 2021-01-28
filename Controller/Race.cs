using Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Timers;
using static System.IntPtr;
using static System.TimeSpan;
using static Controller.Data;

namespace Controller
{
    public class Race
    {
        // Needed for the function 'GetConsoleWindow'
        [System.Runtime.InteropServices.DllImport("kernel32.dll")]
        // https://stackoverflow.com/questions/1277563/how-do-i-get-the-handle-of-a-console-applications-window
        static extern IntPtr GetConsoleWindow();
        public Track Track { get; set; }
        public List<IParticipant> Contestors { get; set; }
        public DateTime StartTime { get; set; }
        private Timer timer;
        private Random random;
        private Dictionary<Section, SectionData> SectionData;
        private Dictionary<IParticipant, bool> Isfinished;
        private Dictionary<IParticipant, int> ContHascompletedLaps;
        private Dictionary<int, IParticipant> RankingContestor;
        private Dictionary<int, IParticipant> Ranking;
        private Dictionary<int, IParticipant> EndRank;
        private Dictionary<IParticipant, TimeSpan> sectionTimeContestor;
        private Dictionary<IParticipant, DateTime> sectionTime;
        private Dictionary<IParticipant, DateTime> lapTime;
        public event EventHandler DriversChanged;
        public event EventHandler RaceFinished;
        public static event EventHandler RaceStarted;
        private const int sectionlength = 239;
        private const int cornerlength = 75;

        public Race(Track track, List<IParticipant> participants)
        {
            random = new Random(DateTime.Now.Millisecond);
            SectionData = new Dictionary<Section, SectionData>();
            RankingContestor = new Dictionary<int, IParticipant>();
            Ranking = new Dictionary<int, IParticipant>();
            EndRank = new Dictionary<int, IParticipant>();
            Isfinished = new Dictionary<IParticipant, bool>();
            ContHascompletedLaps = new Dictionary<IParticipant, int>();
            sectionTimeContestor = new Dictionary<IParticipant, TimeSpan>();
            sectionTime = new Dictionary<IParticipant, DateTime>();
            lapTime = new Dictionary<IParticipant, DateTime>();
            timer = new Timer(500);
            Track = track;
            Contestors = participants;
            timer.Elapsed += OnTimedEvent;
            IniSectionData();
            ResetLaps();
            AddContestorsToTrack();
            RandomizeEquipment();
            Dictionaries();
            Start();
        }

        private void Start()
        {
            RaceStarted?.Invoke(this, new RaceStartedEventArgs() { Race = this });
            timer.Start();
        }

        // Dictionaries will be filled with the use of this function
        private void Dictionaries()
        {
            for (var i = 0; i < Contestors.Count; i++)
            {
                IParticipant contestor = Contestors[i];
                sectionTimeContestor.Add(contestor, TimeSpan.Zero);
                sectionTime.Add(contestor, DateTime.Now);
                lapTime.Add(contestor, DateTime.Now);
            }
        }

        // Reset eventhandlers to null
        public void CleanEventHandler()
        {
            DriversChanged = null;
            RaceFinished = null;
        }

        // Sets all the points of the contestors to 0
        private void ResetLaps()
        {
            Contestors.ForEach(_participant =>
            {
                if (_participant is null)
                {
                    throw new ArgumentNullException(nameof(_participant));
                }
                _participant.Points = 0;
            });
        }

        // Gives the equipment of the contestors random qualifications
        private void RandomizeEquipment()
        {
            Contestors.ForEach(contestor =>
            {
                contestor.Equipment.Speed = random.Next(5, 10);
                contestor.Equipment.Performance = random.Next(5, 10);
                contestor.Equipment.Quality = random.Next(75, 110) + 2;
            });
        }

        // Returns the Data from Section if not empty else creates new Section
        public SectionData GetSectionData(Section section)
        {
            if (SectionData.TryGetValue(section, out SectionData returnValue))
                return returnValue;
            returnValue = new SectionData();
            SectionData.Add(section, returnValue);
            return returnValue;

        }

        // Combines function
        private void OnTimedEvent(object sender, ElapsedEventArgs e)
        {
            ContestorData(e.SignalTime);
            CheckEquipment();
            RepairEquipment();
        }

        // Can cause a car to crash
        private void CheckEquipment()
        {
            int number = 10;
            foreach (IParticipant contestor in Contestors)
            {
                if (contestor.Equipment.Broken) continue;
                if (random.Next(0, 1200) < number)
                {
                    contestor.Equipment.Broken = true;
                    Data.Competition.ContestorBrokenCount(contestor, 1);
                    contestor.Equipment.Quality -= 5;
                    switch (contestor.Equipment.Quality >= 0)
                    {
                        case true:
                            continue;
                        default:
                            contestor.Equipment.Quality = 0;
                            break;
                    }
                }
            }
        }

        // Repairs a car when it is broken 
        private void RepairEquipment()
        {
            int number = 20;
            foreach (var contestor in Contestors.Where(contestor => contestor.Equipment.Broken))
            {
                contestor.Equipment.Broken = !(random.Next(0, 120) < number);
            }
        }

        // If the Contestor has finished one lap it's name will be added to the completed laps dictionary else it adds to the counter
        private void CompleteLap(IParticipant participant, DateTime counter)
        {
            if (!ContHascompletedLaps.ContainsKey(participant))
            {
                ContHascompletedLaps.Add(participant, 0);
                AssertLapTimeToContestor(participant, counter);
                UpdateLapTime(participant, counter);
            }
            else
            {
                AssertLapTimeToContestor(participant, counter);
                UpdateLapTime(participant, counter);
            }
            ContHascompletedLaps[participant]++;

        }

        public int GetRankingOfParticipant(IParticipant participant)
        {
            KeyValuePair<int, IParticipant> first = new KeyValuePair<int, IParticipant>();
            foreach (var p in RankingContestor)
            {
                if (p.Value.Name == participant.Name)
                {
                    first = p;
                    break;
                }
            }

            return first.Key;
        }
        

        // returns the endranking dict
        public Dictionary<int, IParticipant> GetFinalRanking()
        {
            return EndRank;
        }

        // saves the finalranking to the endranking dict
        public void SetFinalRanking(Dictionary<int, IParticipant> finalRanking)
        {
            EndRank = finalRanking;
        }

        public bool CheckCorner(Section section)
        {
            bool corner = false;
            if (section.SectionType == SectionTypes.RightCorner)
                corner = true;
            if (section.SectionType == SectionTypes.LeftCorner)
                corner = true;
            return corner;
        }


        public bool MoveParticipants(IParticipant contestor, SectionData data, float speed, bool left, bool InsCorner, Section section, DateTime time)
        {
            // Checks if the sectiontype is a corner
            bool corner = CheckCorner(section);
            int interval;
            // Calculates the distance
            if (left) interval = data.IntervalLeft;
            else interval = data.IntervalRight;

            interval += (int)Math.Ceiling(speed);

            if (left) data.IntervalLeft = interval;
            else data.IntervalRight = interval;

            int outCorner;
            if (corner && !InsCorner) outCorner = 70;
            else outCorner = 0;

            // when te distance is higher than the length of the section minus the corner 
            if (interval >= (sectionlength - outCorner) || (InsCorner && interval >= cornerlength))
            {
                if (section.SectionType == SectionTypes.Finish && !Isfinished[contestor])
                {
                    // When the contestor has finished a lap
                    CompleteLap(contestor, time);
                    Isfinished[contestor] = true;

                    // When the contestor has driven enough laps
                    if (ContHascompletedLaps[contestor] == 1)
                    {
                        EndRank.Add(EndRank.Count + 1, contestor);
                        if (left)
                        {
                            data.Left = null;
                        }
                        else
                        {
                            data.Right = null;
                        }
                        return true;
                    }
                }
                // Move a contestor to the next section if it is empty
                Section nextSection = GetNextSection(section);
                if (SectionData[nextSection].Left == null)
                {
                    SectionData[nextSection].Left = contestor;
                    SectionData[nextSection].IntervalLeft = 0;
                }
                else if (SectionData[nextSection].Right == null)
                {
                    SectionData[nextSection].Right = contestor;
                    SectionData[nextSection].IntervalRight = 0;
                }
                else // Both not empty stops
                {
                    AssertTimeToContestor(contestor, time);
                    return false;
                }
                if (left) data.Left = null;
                else data.Right = null;
                // Section time is saved and cleaned
                SaveSectionTime(contestor, section);
                CleanTime(contestor, time);
                Isfinished[contestor] = false;
                return true;
            }
            else
            {
                AssertTimeToContestor(contestor, time);
                return false;
            }
        }

        // set cache to time and currenttime to zero
        private void CleanTime(IParticipant contestor, DateTime time)
        {
            sectionTimeContestor[contestor] = TimeSpan.Zero;
            sectionTime[contestor] = time;
        }

        public void ContestorData(DateTime time)
        {
            bool driversChanged = false;
            for (int i = 1; i <= RankingContestor.Count; i++)
            {
                if (RankingContestor[i].Equipment.Broken) continue;
                // Retrieves data from section using order of partipants
                var data = GetContestorSectionData(RankingContestor[i]);
                if (data != null)
                {
                    // calculates the speed of a contestor by equipment_speed * equipment_performace * equipment_quality
                    float speed = (RankingContestor[i].Equipment.Speed * RankingContestor[i].Equipment.Performance) *
                        (RankingContestor[i].Equipment.Quality *
                            (float) Math.Sqrt(RankingContestor[i].Equipment.Quality) / 1000f) + 10f;
                    var section = GetSectionBySectionData(data);

                    //
                    bool isLeft = data.Left == RankingContestor[i];
                    bool driversChangedTemp = MoveParticipants(RankingContestor[i], data, speed, isLeft,
                        isLeft && section.SectionType == SectionTypes.LeftCorner, section, time);
                    driversChanged = driversChangedTemp || driversChanged;
                }

            }


            if (AllContestorsFinished())
            {
                RaceFinished?.Invoke(this, new EventArgs());
            }

            if (driversChanged)
            {
                Ranking = new Dictionary<int, IParticipant>(RankingContestor);
                GetRankingContestors();
                if (GetConsoleWindow() != IntPtr.Zero)
                    if (DriversChanged != null)
                    {
                        DriversChanged?.Invoke(this, new DriversChangedEventArgs() {Track = Track});
                    }
            }

            if (GetConsoleWindow() != IntPtr.Zero) return;
            DriversChanged?.Invoke(this, new DriversChangedEventArgs() { Track = Track });
        }

        public bool GetFinishedContestors(IParticipant participant)
        {
            return Isfinished[participant];
        }

        private void IniSectionData()
        {
            foreach (Section section in Track.Sections)
            {
                if (SectionData != null)
                {
                    SectionData.Add(section, new SectionData());
                }
            }
        }

        // Sets Time of contestor it took to finish the section
        private void SaveSectionTime(IParticipant participant, Section section)
        {
            Data.Competition.SetSectionTime(participant, sectionTimeContestor[participant], section);
        }



        // lap time is updated
        private void UpdateLapTime(IParticipant participant, DateTime time)
        {
            if (lapTime.ContainsKey(participant)) lapTime[participant] = time;
        }

        // Returns the ranking
        public bool AllContestorsFinished()
        {
            foreach (var rank in RankingContestor)
            {
                if (rank.Value.Points != 3) return false;
            }

            return true;
        }

        // Assigns time to contestor for sections
        private void AssertTimeToContestor(IParticipant participant, DateTime time)
        {
            DateTime cache;
            cache = sectionTime[participant];
            if (sectionTimeContestor == null) return;
            sectionTimeContestor[participant] = time - cache;
        }

        // Assigns time to contestor for a lap
        private void AssertLapTimeToContestor(IParticipant participant, DateTime time)
        {
            DateTime cache;
            cache = lapTime[participant];
            Data.Competition.AddLapTime(participant, Track, time - cache);
        }

        // 
        public void GetRankingContestors()
        {
            RankingContestor.Clear();
            int pos = 1;
            for (LinkedListNode<Section> sectionNode = Track.Sections.Last; sectionNode != null; sectionNode = sectionNode.Previous)
            {
                Section section;
                section = sectionNode.Value;
                SectionData data;
                data = SectionData[section];
                if (data.Left == null && data.Right == null)
                    continue;
                if (data.Left != null)
                {
                    RankingContestor.Add(pos, data.Left);
                    pos++;
                }

                if (data.Right != null)
                {
                    RankingContestor.Add(pos, data.Right);
                    pos++;
                }
            }
            GetPassers();
        }


        // When section is  in use of a contestor return data else returns null
        public SectionData GetContestorSectionData(IParticipant contestor)
        {
            foreach (Section section in Track.Sections)
            {
                SectionData data = SectionData[section];
                if (data.Left == null && data.Right == null)
                {
                    continue;
                }

                if (data.Left != contestor && data.Right != contestor)
                {
                    continue;
                }

                return data;
            }

            return null;
        }

        public Section GetSectionBySectionData(SectionData data)
        {
            Section returnValue = Track.Sections.FirstOrDefault(_section => GetSectionData(_section) == data);
            return returnValue;
        }

        public Section GetNextSection(Section section)
        {
            Section returnValue = Track.Sections.Find(section)?.Next?.Value ?? Track.Sections.First.Value;
            return returnValue;
        }

        private void GetPassers()
        {
            for (int i = 1; i <= RankingContestor.Count; i++)
            {
                if (!Ranking.ContainsKey(i))
                {
                    continue;
                }

                if (RankingContestor[i] == Ranking[i])
                {
                    continue;
                }

                KeyValuePair<int, IParticipant> first = new KeyValuePair<int, IParticipant>();
                foreach (var x in Ranking)
                {
                    if (x.Value == RankingContestor[i])
                    {
                        first = x;
                        break;
                    }
                }

                int prevPosition = first.Key;
                if (prevPosition > i)
                {
                    continue;
                }

                Data.Competition.ContestorPassed(RankingContestor[i], Ranking[i]);
            }
        }

        public Dictionary<int, IParticipant> GetRanking()
        {
            return RankingContestor;
        }

        // Put contestors on the track
        public void AddContestorsToTrack()
        {
            SortedDictionary<int, Section> secDict = new SortedDictionary<int, Section>();
            int counter = 0;
            var startNode = Track.Sections.First;
            // Check if first section is a startgrid
            if (startNode.Value.SectionType != SectionTypes.StartGrid)
                throw new Exception("Een track moet beginnen met een Startsection");
            for (var node = startNode; node != null; node = node.Next)
            {
                if (node.Value.SectionType != SectionTypes.StartGrid)
                {
                    continue;
                }

                bool isStart = node.Value.SectionType == SectionTypes.StartGrid;
                if (isStart)
                {
                    secDict.Add(counter, node.Value);
                    counter--;
                }
                else
                {
                    counter++;
                }
            }

            List<Section> startSections = new List<Section>(secDict.Values);

            for (int i = 0; i < startSections.Count; i++)
            {
                SectionData sectionData;
                sectionData = GetSectionData(startSections[i]);
                for (int j = 2 * i; j <= 2 * i + 1; j++)
                {
                    if (j >= Contestors.Count)
                    {
                        break;
                    }

                    RankingContestor.Add(j + 1, Contestors[j]);
                    Isfinished.Add(Contestors[j], false);
                    if (j % 2 == 0)
                    {
                        sectionData.Left = Contestors[j];
                    }
                    else
                    {
                        sectionData.Right = Contestors[j];
                    }
                }
            }
        }
    }
}