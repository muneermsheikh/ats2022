namespace core.Params
{
    public class ParamPages
    {
        private const int MaxPageSize = 50;
        public int PageIndex { get; set; } = 1;
        public string Sort { get; set; }
        
        public int PageSize
        {
            get => _pageSize;
            set => _pageSize = (value > MaxPageSize) ? MaxPageSize : value;
        }
        public string Search
        {
            get => _search;
            set => _search = value.ToLower();
        }
        
        private string _search;
        private int _pageSize = 6;
    }
}