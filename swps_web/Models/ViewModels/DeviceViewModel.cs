namespace swps_web.Models.ViewModels
{
	public class DeviceViewModel
    {
        public List<ClientDevice>? Clients { get; set; }
        
        public string? ServerSN { get; set; }

        public bool ServerStatus { get; set;}
    }
}
