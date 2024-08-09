namespace KDMSServer.Model
{
    public class BaseResponse
    {
        public bool Result { get; set; }
        public Error? Error { get; set; }
    }

    public class Error
    {
        public string Code { get; set; } 
        public string Message { get; set; }
    }
}
