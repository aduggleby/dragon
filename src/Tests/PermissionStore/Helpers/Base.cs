using System;

namespace Dragon.Tests.PermissionStore.Helpers
{
    public class Base
    {
        protected Guid n1 = Guid.NewGuid();
        protected Guid n1_1 = Guid.NewGuid();
        protected Guid n1_2 = Guid.NewGuid();
        protected Guid n1_1_1 = Guid.NewGuid();
        protected Guid n1_2_1 = Guid.NewGuid();
        protected Guid n1_2_2 = Guid.NewGuid();
        protected Guid n1_2_3 = Guid.NewGuid();
        protected Guid special = Guid.NewGuid();
        
        protected Guid s1 = Guid.NewGuid();
        protected Guid s2 = Guid.NewGuid();
        protected Guid s3 = Guid.NewGuid();
        
        protected InMemoryPermissionStore store = new InMemoryPermissionStore();

        protected const string READ = "read";
        protected const string WRITE= "write";
        protected const string MANAGE = "manage";

        public void Setup()
        {
            // n1
            // |--> n1_1
            // |    '--> n1_1_1
            // |    '--> special
            // '--> n1_2
            //      |--> n1_2_1
            //      |--> n1_2_2
            //      |    '--> special
            //      '--> n1_2_3
           
            store.AddNode(n1, n1_1);
            store.AddNode(n1, n1_2);
            store.AddNode(n1_1, n1_1_1);
            store.AddNode(n1_2, n1_2_1);
            store.AddNode(n1_2, n1_2_2);
            store.AddNode(n1_2, n1_2_3);
            store.AddNode(n1_1, special);
            store.AddNode(n1_2_2, special);

            store.AddRight(n1, s1, READ, true);
            store.AddRight(n1, s2, READ, false);
            store.AddRight(n1_2, s2, WRITE, false);
            store.AddRight(n1_2_2, s3, MANAGE, true);
        }
    }
}
