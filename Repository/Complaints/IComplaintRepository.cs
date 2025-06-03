﻿using Repository.BaseRepository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository.Complaints
{
    public interface IComplaintRepository:IBaseRepository<Models.Complaint>
    {
    }
}
