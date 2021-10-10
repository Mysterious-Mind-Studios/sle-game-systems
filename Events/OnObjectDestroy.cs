using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SLE.Events
{
    public delegate void OnObjectDestroy<in T>(T obj);
}
