using System;

namespace Pharmaconnect.Scraper.Lib
{
    public class Store
    {
        public Guid ID { get; set; }
        public string Name { get; set; }
        public string Address { get; set; }
        public string Phone { get; set; }

        public override bool Equals(object obj)
            => obj is Store storeB
                && ID.Equals(storeB.ID)
                && Name.Equals(storeB.Name)
                && Address.Equals(storeB.Address)
                && Phone.Equals(storeB.Phone);
    }
    public class DayAvailability
    {
        public DateTime Day { get; set; }
        public string[] Times { get; set; }
    }
}
