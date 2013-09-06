using System;
using Dragon.Interfaces.PermissionInfo;

namespace Dragon.Context.Permissions
{
    public class DefaultNameResolver : INameResolver
    {
        private readonly string m_subjectPrefix;
        private readonly string m_nodePrefix;

        public DefaultNameResolver(string subjectPrefix, string nodePrefix)
        {
            m_subjectPrefix = subjectPrefix;
            m_nodePrefix = nodePrefix;
        }

        public DefaultNameResolver(string generalPrefix)
        {
            m_subjectPrefix = generalPrefix;
            m_nodePrefix = generalPrefix;
        }

        public DefaultNameResolver():this(string.Empty)
        {
        }
        
        public string ResolveSubjectID(Guid subjectID)
        {
            return m_subjectPrefix + subjectID;

        }

        public string ResolveNodeID(Guid nodeID)
        {
            return m_nodePrefix + nodeID;

        }
    }
}