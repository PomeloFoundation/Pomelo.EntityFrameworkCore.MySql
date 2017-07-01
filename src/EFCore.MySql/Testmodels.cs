using System;
using System.Collections.Generic;

namespace EFCore.MySql
{
    public partial class Testmodels
    {
        public int Id { get; set; }
        public string Description { get; set; }
        public DateTime Time { get; set; }
        public string Title { get; set; }
    }
}
