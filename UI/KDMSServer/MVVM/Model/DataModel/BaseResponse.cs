namespace KDMSServer.Model
{
    public class BaseResponse
    {
        public bool Result { get; set; }
        public Error? Error { get; set; }
    }

    public class Error
    {
        public string Code { get; set; } = string.Empty;
        public string Message { get; set; } = string.Empty;
    }
}
