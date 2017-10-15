using Gitea.API.v1;
using Gitea.API.v1.Users;
using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Collections.ObjectModel;

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
                var user = await client.Users.GetCurrentAsync();
                var following = await user.GetFollowingAsync<Collection<User>>();
            }
        }
    }
}
