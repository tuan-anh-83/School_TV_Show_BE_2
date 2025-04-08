using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BOs.Models
{
    public class Membership
    {
        public int MembershipID { get; set; }
        public int AccountID { get; set; }
        public Account Account { get; set; }

        public int PackageID { get; set; }
        public Package Package { get; set; }

        public DateTime StartDate { get; set; }
        public DateTime ExpirationDate { get; set; }

        public int RemainingDuration { get; set; }

        public bool IsActive { get; set; } = true;
    }
}
