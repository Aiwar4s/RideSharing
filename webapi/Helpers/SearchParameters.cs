namespace webapi.Helpers
{
    public class SearchParameters
    {
        private int _pageSize = 5;
        private int _pageNumber = 1;
        private int? pageSize;
        private const int MaxPageSize = 50;
        public int? PageNumber 
        {
            get => _pageNumber;
            set => _pageNumber = value ?? _pageNumber;
        }
        public int? PageSize 
        {
            get => pageSize > MaxPageSize ? MaxPageSize : _pageSize;
            set => pageSize = value ?? _pageSize; 
        }
    }
}
