namespace product_crud_api.API.DTO
{
    public class PagedResponse<T> : Response<T>
    {
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public int TotalPages { get; set; }
        public T Data { get; set; }
        public PagedResponse(T data, int pageNumber, int pageSize, bool succeeded = true, string message = "") : base(data, succeeded, message)
        {
            PageNumber = pageNumber;
            PageSize = pageSize;
        }
    }
}
