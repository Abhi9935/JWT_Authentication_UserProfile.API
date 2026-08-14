namespace ResumeBuilder.API.DTOs
{
    public class PagedResultDTO<T>
    {
        public int PageNumber { get; set; }

        public int PageSize { get; set; }

        public int TotalRecords { get; set; }

        public int TotalPages { get; set; }        

        public IEnumerable<T> Items { get; set; }
            = Enumerable.Empty<T>();
               
    }
}