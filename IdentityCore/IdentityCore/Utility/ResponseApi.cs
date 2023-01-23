namespace IdentityCore.Utility
{
    public class ResponseApi<T>
    {
        public bool Status { get; set; }
        public string Message { get; set; }
        public T? Data { get; set; }
        public string? Token { get; set; }
        public IList<string>? Role { get; set; }
        public IEnumerable<string>? Errors { get; set; }

       
    }
}
