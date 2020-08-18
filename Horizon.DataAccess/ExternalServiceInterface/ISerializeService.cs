﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Horizon.DataAccess
{
    public interface ISerializeService
    {
        string SerializeObject(object value);
        string SugarSerializeObject(object value);
        T DeserializeObject<T>(string value);
    }
}
