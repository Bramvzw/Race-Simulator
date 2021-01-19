using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Model
{
    public class Track
    {
        public LinkedList<Section> Sections { get; set; }

        public string Name { get; set; }

        public Track(string name, SectionTypes[] sections)
        {
            Name = name;
            Sections = AssembleSectionTypesToSections(sections);
        }

        // See functionname ;-)
        private LinkedList<Section> AssembleSectionTypesToSections(SectionTypes[] sectionTypes)
        {
            LinkedList<Section> returnValue = new LinkedList<Section>();
            foreach (SectionTypes sectionType in sectionTypes)
            {
                Section section = new Section(sectionType);
                returnValue.AddLast(section);
            }
            return returnValue;
        }
    }
}
