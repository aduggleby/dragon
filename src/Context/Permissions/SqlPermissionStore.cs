using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using Dapper;
using Dragon.Common;
using Dragon.Context.Exceptions;
using Dragon.Interfaces;

namespace Dragon.Context.Permissions
{
    public class SqlPermissionStore : PermissionStoreBase
    {
        public SqlPermissionStore()
            : base()
        {

            RebuildTree();
        }

        #region Nodes

        protected override void AddNodeInternal(Guid parentID, Guid childID)
        {
            var parents = EnumerateParentNodesInternal(childID);

            if (!parents.Contains(parentID))
            {
                using (var conn = new SqlConnection(StandardSqlStore.ConnectionString))
                {
                    conn.Open();
                    var param = new SqlPermissionNode() {ParentID = parentID, ChildID = childID};
                    conn.Execute(SQL.SqlPermissionStore_InsertPermissionNode, param);
                }
            }

            RebuildTree();
        }

        protected override void RemoveNodeInternal(Guid parentID, Guid childID)
        {
            // no parenthood test here because delete will ignore anyway

            using (var conn = new SqlConnection(StandardSqlStore.ConnectionString))
            {
                conn.Open();
                var param = new { ParentID = parentID, ChildID = childID };
                conn.Execute(SQL.SqlPermissionStore_DeletePermissionNode, param);
            }

            RebuildTree();
        }

        protected override IEnumerable<Guid> EnumerateParentNodesInternal(Guid childID)
        {
            using (var conn = new SqlConnection(StandardSqlStore.ConnectionString))
            {
                conn.Open();
                var param = new { ChildID = childID };
                return
                    conn.Query<SqlPermissionNode>(SQL.SqlPermissionStore_GetPermissionParentNodes, param)
                        .Select(x => x.ParentID);
            }

        }

        protected override IEnumerable<Guid> EnumerateChildrenNodesInternal(Guid parentID)
        {
            using (var conn = new SqlConnection(StandardSqlStore.ConnectionString))
            {
                conn.Open();
                var param = new { ParentID = parentID };
                return conn.Query<SqlPermissionNode>(SQL.SqlPermissionStore_GetPermissionNodesByParentID, param)
                    .Select(x=>x.ParentID);
            }
        }

        protected override IEnumerable<IPermissionNode> EnumerateAllNodesInternal()
        {
            using (var conn = new SqlConnection(StandardSqlStore.ConnectionString))
            {
                conn.Open();
                var param = new {};
                return conn.Query<SqlPermissionNode>(SQL.SqlPermissionStore_GetAllPermissionNodes, param);
            }
        }

        public override bool IsChildNodeOf(Guid parentID, Guid childID)
        {
            var parent = EnumerateParentNodesInternal(childID);
            return parent.Contains(parentID);
        }

        #endregion

        #region Rights

        protected override void AddRightInternal(Guid nodeID, Guid subjectID, string spec, bool inherit)
        {
            if (EnumerateRightsInternal(nodeID).Any(x => x.Spec == spec))
            {
                RemoveRightInternal(nodeID, subjectID, spec);
            }
        
            using (var conn = new SqlConnection(StandardSqlStore.ConnectionString))
            {
                conn.Open();
                var param = new SqlPermissionRight()
                    {
                        LID = Guid.NewGuid(),
                        NodeID = nodeID,
                        SubjectID = subjectID,
                        Spec = spec,
                        Inherit = inherit
                    };
                conn.Execute(SQL.SqlPermissionStore_InsertPermissionRight, param);
            }

            RebuildTree();
        }

        protected override void RemoveRightInternal(Guid nodeID, Guid subjectID, string spec)
        {
            var candidate = EnumerateRightsInternal(nodeID).FirstOrDefault(x => x.Spec == spec);

            if (candidate == null) 
                throw new RightDoesNotExistException();

            using (var conn = new SqlConnection(StandardSqlStore.ConnectionString))
            {
                conn.Open();
                var param = new
                {
                    LID = candidate.LID
                };
                conn.Execute(SQL.SqlPermissionStore_DeletePermissionRight, param);
            }

            RebuildTree();
        }

        protected override IEnumerable<IPermissionRight> EnumerateRightsInternal(Guid nodeID)
        {
            using (var conn = new SqlConnection(StandardSqlStore.ConnectionString))
            {
                conn.Open();
                var param = new { NodeID = nodeID };
                return conn.Query<SqlPermissionRight>(SQL.SqlPermissionStore_GetPermissionRightsByNode, param);
            }
        }

        protected override IEnumerable<IPermissionRight> EnumerateAllRightsInternal()
        {
            using (var conn = new SqlConnection(StandardSqlStore.ConnectionString))
            {
                conn.Open();
                var param = new {  };
                return conn.Query<SqlPermissionRight>(SQL.SqlPermissionStore_GetAllPermissionRights, param);
            }
        }

        #endregion

    }
}
