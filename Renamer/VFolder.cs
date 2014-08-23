using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Renamer
{
    public class VFolder
    {
        public String Name, NewName;
        public List<VFolder> SubFolders = new List<VFolder>();
    }
}
