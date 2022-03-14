namespace product_crud_api.API.DTO
{
    public class PagedResponse<T> : Response<T>
    {
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public int TotalPages { get; set; }
        public PagedResponse(T data, int pageNumber, int pageSize, string message = "") : base(data, message)
        {
            PageNumber = pageNumber;
            PageSize = pageSize;
        }
    }
}
