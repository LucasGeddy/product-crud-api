namespace product_crud_api.API.DTO
{
    public class Response<T>
    {
        public string Message { get; set; }
        public T? Data { get; set; }
        public Response(T data, string message = "")
        {
            Message = message;
            Data = data;
        }
    }
}