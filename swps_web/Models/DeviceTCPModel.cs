namespace swps_web.Models
{
    public class DeviceTCPModel<T>
    {
        public string? Api { get; set; }
        public int Result { get; set; }
        public T? Data { get; set; }
    }
}
