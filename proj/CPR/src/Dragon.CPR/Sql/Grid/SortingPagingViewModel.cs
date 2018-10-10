namespace Dragon.CPR.Sql.Grid
{
    public class SortingPagingViewModel 
    {
        public SortingPagingViewModel()
        {
            SortAscending = true;
        }

        private int? m_page;
        private int? m_itemsPerPage;
        public static int DefaultItemsPerPage = 10;
        public string SortProperty { get; set; }
        public bool SortAscending { get; set; }
        public string SearchString { get; set; }

        public int MaxPage { get; set; }

        public int? Page
        {
            get
            {
                return m_page ?? 1;
            }
            set
            {
                m_page = value;
            }
        }

        public int? ItemsPerPage
        {
            get
            {
                return m_itemsPerPage ?? DefaultItemsPerPage;
            }
            set
            {
                m_itemsPerPage = value;
            }
        }

        public void SetDefaultItemsPerPage(int i)
        {
            DefaultItemsPerPage = i;
        }
    }
}
