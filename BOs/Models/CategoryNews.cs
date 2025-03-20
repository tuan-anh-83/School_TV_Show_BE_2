using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BOs.Models
{
    public class CategoryNews
    {
        public int CategoryNewsID { get; set; }
        public string CategoryName { get; set; }
        public string? Description { get; set; } // Kiểu string, cho phép null

        public ICollection<News> News { get; set; } = new List<News>();
    }
}
