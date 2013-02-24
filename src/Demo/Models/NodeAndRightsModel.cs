using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Dragon.Common.Objects.Tree;
using Dragon.Interfaces;

namespace Demo.Models
{
    public class NodeAndRightsModel
    {
        public IEnumerable<TreeNode<Guid, IEnumerable<IPermissionRight>>> Nodes { get; set; }
        public IEnumerable<IPermissionRight> Rights {get;set;}
    }
}