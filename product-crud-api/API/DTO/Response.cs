namespace product_crud_api.API.DTO
{
    public class Response<T>
    {
        public bool Succeeded { get; set; }
        public string Message { get; set; }
        public T Data { get; set; }
        public Response(T data, bool succeeded = true, string message = "")
        {
            Succeeded = succeeded;
            Message = message;
            Data = data;
        }
    }
}