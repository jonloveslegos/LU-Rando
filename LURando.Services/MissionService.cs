using LURando.Common;
using LURando.Models;
using LURando.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LURando.Services
{
    public class MissionService : BaseService<Missions>
    {
        public MissionService()
            :base(Storage.ConnectionString)
        {

        }


        // you can write extended methods here
    }
}
