using System;
using System.Collections.Generic;
using System.Text;

namespace StratML.Core
{
    public class Corporation : Organization
    {
        public Corporation()
        {
            Organizations = new HashSet<Organization>();
            People = new HashSet<Person>();
            Strategies = new HashSet<PerformancePlanOrReport>();
        }
        public ICollection<Organization> Organizations { get; set; }
        public ICollection<Person> People { get; set; }
        public PerformancePlanOrReport OverallStrategy { get; set; }
        public ICollection<PerformancePlanOrReport> Strategies { get; set; }
        
    }
    public class Person
    {
        public NameDescriptionType NameDescription { get; set; }
        public ContactInformationType PointOfContact { get; set; }
    }
}
