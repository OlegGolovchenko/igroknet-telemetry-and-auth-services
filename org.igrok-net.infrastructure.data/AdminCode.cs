using System.IO;

namespace org.igrok_net.infrastructure.data
{
    public class AdminAccessCode
    {

        public string AdminCode { get; set; }

        public AdminAccessCode(string adminAccessCode)
        {
            if (string.IsNullOrWhiteSpace(adminAccessCode))
            {
                var data = File.ReadAllText("admin.code");
                AdminCode = data;
            }
            else
            {
                AdminCode = adminAccessCode;
            }
        }
    }
}
