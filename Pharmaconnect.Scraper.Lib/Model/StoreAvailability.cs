using System;

namespace Pharmaconnect.Scraper.Lib
{
    public class Store : IEquatable<Store>
    {
        public Guid ID { get; set; }
        public string Name { get; set; }
        public string Address { get; set; }
        public string Phone { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public string Timezone { get; set; }

        public override bool Equals(object obj)
            => Equals(obj as Store);
            
        public bool Equals(Store other)
            => other is not null
                && ID.Equals(other.ID)
                && Name.Equals(other.Name, StringComparison.InvariantCulture)
                && Address.Equals(other.Address, StringComparison.InvariantCulture)
                && Phone.Equals(other.Phone, StringComparison.InvariantCulture);

        public override int GetHashCode() 
            => HashCode.Combine(ID, Name, Address, Phone);
    }

    public class StoreAvailability : Store
    {
        public DayAvailability[] Availabilities { get; set; }
    }

    public class DayAvailability
    {
        public DateTime Day { get; set; }
        public string[] Times { get; set; }
    }
}
