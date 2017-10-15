using Gitea.API.v1;
using System;
using System.Threading.Tasks;

namespace Gitea.API.Sandbox {
    class Program {
        static void Main(string[] args) {
            try {
                RunAsync().Wait();   
            }
            catch (Exception ex) {
                Console.WriteLine(
                    (ex.GetBaseException() ?? ex)?.Message
                );
            }
        }

        static async Task RunAsync() {
            var auth = new BasicAuth()
                {
                    Password = Credentials.Password,
                    Username = Credentials.User,
                };

            using (var client = new Client(auth,
                                           Credentials.Host, Credentials.Port, Credentials.IsSecure)) {
                var users = await client.Users.SearchAsync("kloub");
                if (null != users) {
                    
                }
            }
        }
    }
}
