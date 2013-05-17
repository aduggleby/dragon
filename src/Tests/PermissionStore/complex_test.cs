using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Dragon.Tests.PermissionStore.Helpers;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using FluentAssertions;

namespace Dragon.Tests.PermissionStore
{
    [TestClass]
    public class complex_test : Base
    {
        Dictionary<string,Guid> m_users = new Dictionary<string,Guid>();
        Dictionary<string, Guid> m_rights = new Dictionary<string, Guid>();
        Dictionary<string, Guid> m_projects = new Dictionary<string, Guid>();
        Dictionary<string, Guid> m_groups = new Dictionary<string, Guid>();


        private Guid U(string key)
        {
            if (!m_users.ContainsKey(key)){
                m_users.Add(key, Guid.NewGuid());
            }
            return m_users[key];
        }

        private string R(string key)
        {
            if (!m_rights.ContainsKey(key))
            {
                m_rights.Add(key, Guid.NewGuid());
            }
            return m_rights[key].ToString();
        }

        private Guid P(string key)
        {
            if (!m_projects.ContainsKey(key))
            {
                m_projects.Add(key, Guid.NewGuid());
            }
            return m_projects[key];
        }

        private Guid G(string key)
        {
            if (!m_groups.ContainsKey(key))
            {
                m_groups.Add(key, Guid.NewGuid());
            }
            return m_groups[key];
        }

        [TestInitialize]
        public void LocalSetup()
        {
           
        }

        [TestMethod]
        public void reenactment_of_setup_controller()
        {
            store.AddRight(Guid.Empty, U("admin"), R("sysadmin"), false);
            store.AddRight(Guid.Empty, U("admin"), R("project-read"), true);

            /////////////////////////////////////////////////////////////////////////////
            //
            // USER 1
            //
            store.AddRight(P("project1"), U("user1"), R("project-write"), false);
            store.AddRight(P("project1"), U("user1"), R("project-read"), false);

            // 3 public groups
            store.AddRight(G("default-public-memberreaders"), U("user1"), R("group-member"), false);
            store.AddRight(G("default-public-memberreaders"), U("user1"), R("project-read"), true);

            store.AddRight(G("public"), U("user1"), R("group-member"), false);

            store.AddRight(G("public-memberreaders"), U("user1"), R("group-member"), false);
            store.AddRight(G("public-memberreaders"), U("user1"), R("project-read"), true);

            /////////////////////////////////////////////////////////////////////////////
            //
            // USER 2
            //
            store.AddRight(P("project2"), U("user2"), R("project-write"), false);
            store.AddRight(P("project2"), U("user2"), R("project-read"), false);

            // 3 public groups
            store.AddRight(G("default-public-memberreaders"), U("user2"), R("group-member"), false);
            store.AddRight(G("default-public-memberreaders"), U("user2"), R("project-read"), true);

            store.AddRight(G("public"), U("user2"), R("group-member"), false);

            store.AddRight(G("public-memberreaders"), U("user2"), R("group-member"), false);
            store.AddRight(G("public-memberreaders"), U("user2"), R("project-read"), true);

            // gives user1 r/w access to his project
            store.AddRight(P("project2"), U("user1"), R("project-read"), false);
            store.AddRight(P("project2"), U("user1"), R("project-write"), false);

            /////////////////////////////////////////////////////////////////////////////
            //
            // USER 3
            //
            store.AddRight(P("project3"), U("user3"), R("project-write"), false);
            store.AddRight(P("project3"), U("user3"), R("project-read"), false);

            // gives user1 r access to his project
            store.AddRight(P("project3"), U("user1"), R("project-read"), false);

            // gives user2 r/w access to his project
            store.AddRight(P("project3"), U("user2"), R("project-read"), false);
            store.AddRight(P("project3"), U("user2"), R("project-write"), false);

            // give all groups rights to project
            store.HasRight(P("project3"), U("user3"), R("project-write"));
            store.AddRight(P("project3"), G("default-public-memberreaders"), R("project-read"), false);
            store.HasRight(P("project3"), U("user3"), R("project-write"));
            store.AddRight(P("project3"), G("public-memberreaders"), R("project-read"), true);
            store.HasRight(P("project3"), U("user3"), R("project-write"));
            store.AddRight(P("project3"), G("public"), R("project-read"), true);

            var s = store.DebugOutputTree().ToString();
        }
    }
}
