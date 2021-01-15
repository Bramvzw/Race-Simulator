using Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Timers;

namespace Controller
{
    public class Race
    {
        [System.Runtime.InteropServices.DllImport("kernel32.dll")]
        static extern IntPtr GetConsoleWindow();
        public Track Track { get; set; }
        public List<IParticipant> Contestors { get; set; }
        public DateTime StartTime { get; set; }
        private Timer _timer;
        private Random Random;
        private Dictionary<Section, SectionData> SectionData;
        private Dictionary<IParticipant, bool> _finished;
        private Dictionary<IParticipant, int> _ContHascompletedLaps;
        private Dictionary<int, IParticipant> _ranking;
        private Dictionary<int, IParticipant> _rankingCache;
        private Dictionary<int, IParticipant> _finalRanking;
        private Dictionary<IParticipant, TimeSpan> _currentTimeOnSection;
        private Dictionary<IParticipant, DateTime> _sectionTimeCache;
        private Dictionary<IParticipant, DateTime> _lapTimeCache;
        public event EventHandler DriversChanged;
        public event EventHandler RaceFinished;
        public static event EventHandler RaceStarted;
        private const int SECTION_LENGTH = 239;
        private const int INNER_CORNER_LENGTH = 75;

        public Race(Track track, List<IParticipant> participants)
        {
            Track = track;
            Contestors = participants;
            Random = new Random(DateTime.Now.Millisecond);
            SectionData = new Dictionary<Section, SectionData>();
            _ranking = new Dictionary<int, IParticipant>();
            _rankingCache = new Dictionary<int, IParticipant>();
            _finalRanking = new Dictionary<int, IParticipant>();
            _finished = new Dictionary<IParticipant, bool>();
            _ContHascompletedLaps = new Dictionary<IParticipant, int>();
            _currentTimeOnSection = new Dictionary<IParticipant, TimeSpan>();
            _sectionTimeCache = new Dictionary<IParticipant, DateTime>();
            _lapTimeCache = new Dictionary<IParticipant, DateTime>();
            _timer = new Timer(500);
            _timer.Elapsed += OnTimedEvent;
            InitialiseSectionData();
            ResetLaps();
            AddContestorsToTrack();
            RandomizeEquipment();
            Dictionaries();
            Start();
        }

        private void OnTimedEvent(object sender, ElapsedEventArgs e)
        {
            ContestorData(e.SignalTime);
            CheckEquipment();
            RepairEquipment();
        }


        // Dictionaries will be filled with the use of this function
        private void Dictionaries()
        {
            foreach (IParticipant contestor in Contestors)
            {
                _sectionTimeCache.Add(contestor, DateTime.Now);
                _currentTimeOnSection.Add(contestor, TimeSpan.Zero);
                _lapTimeCache.Add(contestor, DateTime.Now);
            }
        }

        private void Start()
        {
            RaceStarted?.Invoke(this, new RaceStartedEventArgs() { Race = this });
            _timer.Start();
        }

        public void DisposeEventHandler()
        {
            DriversChanged = null;
            RaceFinished = null;
        }

        // Sets all the points of the contestors to 0
        private void ResetLaps()
        {
            Contestors.ForEach(_participant => _participant.Points = 0);
        }

        // Gives the equipment of the contestors random qualifications
        private void RandomizeEquipment()
        {
            Contestors.ForEach(_participant =>
            {
                _participant.Equipment.Speed = Random.Next(7, 13);
                _participant.Equipment.Performance = Random.Next(6, 13);
                _participant.Equipment.Quality = Random.Next(74, 100) + 1;
            });
        }

        // Returns the Data from Section if not empty else creates new Section
        public SectionData GetSectionData(Section section)
        {
            if (SectionData.TryGetValue(section, out SectionData returnValue))
            {
                return returnValue;
            }
            else
            {
                returnValue = new SectionData();
                SectionData.Add(section, returnValue);
                return returnValue;
            }
        }

        // Checks when equipment has broken down and add it to the counter also gives the equipment some damage which can not be lower than 0
        //private void CheckEquipment()
        //{
        //    int number = 10;
        //    foreach (IParticipant contestors in Contestors)
        //    {
        //        if (contestors.Equipment.IsBroken) continue;

        //        if (Random.Next(0, 1000) < number)
        //        {
        //            contestors.Equipment.IsBroken = true;
        //            Data.Competition.ContestorBrokenCount(contestors, 1);

        //            contestors.Equipment.Quality -= 5;

        //            if (contestors.Equipment.Quality <= 0)
        //            {
        //                contestors.Equipment.Quality = 0;
        //            }
        //        }
        //    }
        //}

        private void CheckEquipment()
        {
            int number = 10;
            foreach (IParticipant contestor in Contestors)
            {
                if (contestor.Equipment.IsBroken) continue;
                if (Random.Next(0, 1000) < number)
                {
                    contestor.Equipment.IsBroken = true;
                    Data.Competition.ContestorBrokenCount(contestor, 1);
                    contestor.Equipment.Quality -= 5;
                    if (contestor.Equipment.Quality < 0)
                        contestor.Equipment.Quality = 0;
                }
            }
        }

        // Repairs a car when it is broken 
        private void RepairEquipment()
        {
            int number = 20;
            foreach (IParticipant participant in Contestors)
            {
                if (!participant.Equipment.IsBroken) continue;

                participant.Equipment.IsBroken = !(Random.Next(0, 100) < number);
            }

        }


        // If the Contestor has finished one lap it's name will be added to the completed laps dictionary else it adds to the counter
        private void CompleteLap(IParticipant participant, DateTime counter)
        {
            if (!_ContHascompletedLaps.ContainsKey(participant))
            {
                _ContHascompletedLaps.Add(participant, 0);
                SetLapTimetoContestor(participant, counter);
                UpdateLapTime(participant, counter);
            }
            else
            {
                SetLapTimetoContestor(participant, counter);
                UpdateLapTime(participant, counter);
            }
            _ContHascompletedLaps[participant]++;

        }

        public int GetRankingOfParticipant(IParticipant participant)
        {
            return _ranking.FirstOrDefault(p => p.Value.Name == participant.Name).Key;
        }

        public Dictionary<int, IParticipant> GetFinalRanking()
        {
            return _finalRanking;
        }


        public void SetFinalRanking(Dictionary<int, IParticipant> finalRanking)
        {
            _finalRanking = finalRanking;
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


        public bool MoveParticipants(IParticipant contestor, SectionData data, float speed, bool left, bool isInInnerCorner, Section section, DateTime time)
        {
            // Checks if the sectiontype is a corner
            bool corner = CheckCorner(section);
            // Calculates the distance
            int distance;
            if (left)
                distance = data.DistanceLeft;
            else
                distance = data.DistanceRight;
            distance += (int)Math.Ceiling(speed);
            if (left)
                data.DistanceLeft = distance;
            else
                data.DistanceRight = distance;

            data.DistanceRight = distance;
            int outerCornerLength = corner && !isInInnerCorner ? 80 : 0;
            // when te distance is higher than the length of the section minus the corner 
            if (distance >= (SECTION_LENGTH - outerCornerLength) || (isInInnerCorner && distance >= INNER_CORNER_LENGTH))
            {
                if (section.SectionType == SectionTypes.Finish && !_finished[contestor])
                {
                    // When the contestor has finished a lap
                    CompleteLap(contestor, time);
                    _finished[contestor] = true;

                    // When the contestor has driven enough laps
                    if (_ContHascompletedLaps[contestor] == 1)
                    {
                        _finalRanking.Add(_finalRanking.Count + 1, contestor);
                        if (left)
                            data.Left = null;
                        else
                            data.Right = null;
                        return true;
                    }
                }
                // Move a contestor to the next section if it is empty
                var nextSection = GetNextSection(section);
                if (SectionData[nextSection].Left == null)
                {
                    SectionData[nextSection].Left = contestor;
                    SectionData[nextSection].DistanceLeft = 0;
                }
                else if (SectionData[nextSection].Right == null)
                {
                    SectionData[nextSection].Right = contestor;
                    SectionData[nextSection].DistanceRight = 0;
                }
                else // Both not empty stops
                {
                    AddTimeToParticipant(contestor, time);
                    return false;
                }
                if (left)
                {
                    data.Left = null;
                }
                else
                {
                    data.Right = null;
                }
                // Section time is saved and cleaned
                SaveSectionTime(contestor, section);
                ResetTime(contestor, time);
                _finished[contestor] = false;
                return true;
            }
            else
            {
                AddTimeToParticipant(contestor, time);
                return false;
            }
        }

        public void ContestorData(DateTime time)
        {
            bool driversChanged = false;
            for (int i = 1; i <= _ranking.Count; i++)
            {
                if (_ranking[i].Equipment.IsBroken) continue;
                // Retrieves data from section using order of partipants
                var data = GetSectionDataByParticipant(_ranking[i]);
                if (data == null)
                    continue;
                // calculates the speed of a contestor by equipment_speed * equipment_performace * equipment_quality
                float speed = (_ranking[i].Equipment.Speed * _ranking[i].Equipment.Performance) * (_ranking[i].Equipment.Quality * (float)Math.Sqrt(_ranking[i].Equipment.Quality) / 1000f) + 10f;
                var section = GetSectionBySectionData(data);

                //
                bool isLeft = data.Left == _ranking[i];
                bool driversChangedTemp = MoveParticipants(_ranking[i], data, speed, isLeft,
                    isLeft && section.SectionType == SectionTypes.LeftCorner, section, time);
                driversChanged = driversChangedTemp || driversChanged;
            }


            if (AllContestorsFinished())
            {
                RaceFinished?.Invoke(this, new EventArgs());
            }

            if (driversChanged)
            {
                _rankingCache = new Dictionary<int, IParticipant>(_ranking);
                DetermineRanking();
                if (GetConsoleWindow() != IntPtr.Zero)
                    DriversChanged?.Invoke(this, new DriversChangedEventArgs() { Track = Track });
            }
            if (GetConsoleWindow() == IntPtr.Zero)
            {
                DriversChanged?.Invoke(this, new DriversChangedEventArgs() { Track = Track });
            }
        }

        public bool GetIsFinished(IParticipant participant)
        {
            return _finished[participant];
        }

        // 
        public bool AllContestorsFinished()
        {
            return _ranking.All(rank => rank.Value.Points > 2);
        }

        // Sets Time of contestor it took to finish the section
        private void SaveSectionTime(IParticipant participant, Section section)
        {
            Data.Competition.SetSectionTime(participant, _currentTimeOnSection[participant], section);
        }

        // set cache to time and currenttime to zero
        private void ResetTime(IParticipant participant, DateTime time)
        {
            _sectionTimeCache[participant] = time;
            _currentTimeOnSection[participant] = TimeSpan.Zero;
        }

        // lap time cache is updated
        private void UpdateLapTime(IParticipant participant, DateTime time)
        {
            _lapTimeCache[participant] = time;
        }

        // Assigns time to contestor for sections
        private void AddTimeToParticipant(IParticipant participant, DateTime time)
        {
            DateTime cache = _sectionTimeCache[participant];
            _currentTimeOnSection[participant] = time - cache;
        }

        // Assigns time to contestor for a lap
        private void SetLapTimetoContestor(IParticipant participant, DateTime time)
        {
            DateTime cache = _lapTimeCache[participant];
            Data.Competition.AddLapTime(participant, Track, time - cache);
        }

        private void InitialiseSectionData()
        {
            foreach (Section section in Track.Sections)
            {
                if (section != null)
                {
                    SectionData.Add(section, new SectionData());
                }
                else
                {
                    DetermineRanking(); 
                }
            }
        }

        // When section is  in use of a contestor return data else returns null
        public SectionData GetSectionDataByParticipant(IParticipant contestor)
        {
            foreach (Section section in Track.Sections)
            {
                SectionData data = SectionData[section];
                if (data.Left == null && data.Right == null) continue;
                if (data.Left != contestor && data.Right != contestor) continue;
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


        public void DetermineRanking()
        {
            _ranking.Clear();
            int pos = 1;
            for (var sectionNode = Track.Sections.Last; sectionNode != null; sectionNode = sectionNode.Previous)
            {
                Section section = sectionNode.Value;
                SectionData data = SectionData[section];
                if (data.Left == null && data.Right == null)
                    continue;
                if (data.Left != null)
                {
                    _ranking.Add(pos, data.Left);
                    pos++;
                }

                if (data.Right != null)
                {
                    _ranking.Add(pos, data.Right);
                    pos++;
                }
            }
            DetermineOvertakers();
        }

        private void DetermineOvertakers()
        {
            for (int i = 1; i <= _ranking.Count; i++)
            {
                if (!_rankingCache.ContainsKey(i)) continue;
                if (_ranking[i] == _rankingCache[i]) continue;
                int prevPosition = _rankingCache.FirstOrDefault(x => x.Value == _ranking[i]).Key;
                if (prevPosition > i) continue;
                Data.Competition.ContestorPassed(_ranking[i], _rankingCache[i]);
            }
        }

        public Dictionary<int, IParticipant> GetRanking()
        {
            return _ranking;
        }

        public void AddContestorsToTrack()
        {
            SortedDictionary<int, Section> helpDict = new SortedDictionary<int, Section>();
            int counter = 0;
            var startNode = Track.Sections.First;
            if (startNode.Value.SectionType != SectionTypes.StartGrid)
                throw new Exception("First section should be a start!");
            for (var node = startNode; node != null; node = node.Next)
            {
                bool isStart = node.Value.SectionType == SectionTypes.StartGrid;
                if (isStart)
                {
                    helpDict.Add(counter, node.Value);
                    counter--;
                }
                else
                {
                    counter++;
                }
            }

            List<Section> startSections = new List<Section>(helpDict.Values);

            for (int i = 0; i < startSections.Count; i++)
            {
                SectionData sectionData = GetSectionData(startSections[i]);
                for (int j = 2 * i; j <= 2 * i + 1; j++)
                {
                    if (j >= Contestors.Count) break;
                    _ranking.Add(j + 1, Contestors[j]);
                    _finished.Add(Contestors[j], false);
                    if (j % 2 == 0)
                        sectionData.Left = Contestors[j];
                    else
                        sectionData.Right = Contestors[j];
                }
            }
        }
    }
}