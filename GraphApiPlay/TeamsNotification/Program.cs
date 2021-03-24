using Microsoft.Graph;
using Microsoft.Graph.Auth;
using Microsoft.Identity.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TeamsNotification
{
    class Program
    {
        static bool loop = true;
        static string GroupId
        {
            get
            {
                AuthenticationConfig config = AuthenticationConfig.ReadFromJsonFile("appsettings.json");

                return config.GroupId;
            }
        }
        static string graphV1Endpoint = "https://graph.microsoft.com/v1.0";
        static GraphServiceClient GraphClient;
        static GraphServiceClient PublicGraphClient;
        static string LogonName;
        static System.Security.SecureString Passcode = new System.Security.SecureString();
        static List<ChannelInfo> Channels = new List<ChannelInfo>();
        static string[] ApplicaionScopes
        {
            get
            {
                AuthenticationConfig config = AuthenticationConfig.ReadFromJsonFile("appsettings.json");
                List<string> appScopes = config.ApplicationScopes.ToList();

                List<string> scopes = new List<string>();
                appScopes.ForEach(x =>
                {
                    scopes.Add($"{config.ApiUrl}{x}");
                });
                return scopes.ToArray();
            }
        }


        public static async Task Main(string[] args)
        {
            try
            {
                await ReadChannelData();
            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine(ex.Message);
                Console.ResetColor();
            }

            try
            {
                AuthenticationConfig config = AuthenticationConfig.ReadFromJsonFile("appsettings.json");
                LogonName = config.UserName;
                var passCode = config.Passcode;

                var x = passCode.ToCharArray();

                x.ToList().ForEach(x => Passcode.AppendChar(x));


                await GetUserNameByCredentialProvider();

                //await SetUp();
                Console.ForegroundColor = ConsoleColor.Yellow;
                while (loop)
                {
                    Loop();
                }

            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine(ex.Message);
                Console.ResetColor();
            }
        }



        private async static Task ReadChannelData()
        {
            AuthenticationConfig config = AuthenticationConfig.ReadFromJsonFile("appsettings.json");
            var channelInfoFile = config.ChannelsConfig;


            ChannelStore channelInfo = ChannelStore.ReadFromJsonFile(channelInfoFile);

            if (channelInfo != null && channelInfo.Channels.Any())
            {
                Channels.AddRange(channelInfo.Channels);
            }
            else
            {
                Console.WriteLine("No Channels to List");
            }
        }

        private static async Task<string> GetUserNameByCredentialProvider()
        {
            AuthenticationConfig config = AuthenticationConfig.ReadFromJsonFile("appsettings.json");

            PublicClientApplicationOptions options = new PublicClientApplicationOptions
            {
                ClientId = config.PublicClientId
            };


            var application = PublicClientApplicationBuilder.CreateWithApplicationOptions(options)
                    .WithRedirectUri(config.RedirectUri)
                    .WithAuthority(AzureCloudInstance.AzurePublic, config.Tenant)
                    .Build();

            UsernamePasswordProvider authProvider = new UsernamePasswordProvider(application, ApplicaionScopes);
            await application.AcquireTokenByUsernamePassword(ApplicaionScopes, LogonName, Passcode).
                                ExecuteAsync();
            await authProvider.ClientApplication.AcquireTokenByUsernamePassword(ApplicaionScopes, LogonName, Passcode).
                                ExecuteAsync();
            PublicGraphClient = new GraphServiceClient(authProvider);


            User me = await PublicGraphClient.Me.Request().GetAsync();
            Console.WriteLine($"Logged in as [{me.DisplayName}] @ {DateTime.Now.ToString()}.");
            Console.WriteLine();
            return me.Id;            
        }

        private static void Loop()
        {

            Console.WriteLine("1. List Channels");
            Console.WriteLine("2. List Members");
            Console.WriteLine("3. SendCahnnelNotification");

            var option = Console.ReadKey();

            Console.Clear();

            switch (option.Key)
            {

                case ConsoleKey.D1:
                    ListChannels().GetAwaiter().GetResult();
                    break;


                case ConsoleKey.D2:
                    ListChannelMembers().GetAwaiter().GetResult();
                    break;

                case ConsoleKey.D3:
                    SendCahnnelNotification().GetAwaiter().GetResult();
                    break;

            }

            Console.WriteLine();
            Console.WriteLine("Press 'X' to exit");
            Console.WriteLine("Press 'ENTER' to continue");

            var key = Console.ReadKey();
            switch (key.Key)
            {
                case ConsoleKey.X:
                    loop = false;
                    break;
                case ConsoleKey.Enter:
                    Console.Clear();
                    break;
            }
        }

        private static async Task<List<ChannelInfo>> ListChannels()
        {
            if (!Channels.Any())
            {
                Console.WriteLine("No Channels Found.");
            }
            var i = 1;
            Console.WriteLine("Channels List =>");
            foreach (var ch in Channels)
            {
                Console.WriteLine($"{i}. {ch.Name}");
                i++;
            }

            return Channels;
        }

        private static async Task SendCahnnelNotification()
        {

            var channel = await GetChannelSelection();


            if (channel != null)
            {
                Console.WriteLine($"Sending message in {channel.Name}");
                var chatMessage = new ChatMessage
                {
                    Body = new ItemBody
                    {
                        Content = "This is auto generated notification from graph api play."
                    }
                };

                try
                {
                    await PublicGraphClient.Teams[GroupId].Channels[channel.Id].Messages
                            .Request()
                            .AddAsync(chatMessage);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                    throw new Exception("Test", ex);
                }
            }
            else
            {
                Console.WriteLine("Invalid Channel Selection");
            }
        }

        private static async Task ListChannelMembers()
        {

            await PrintChannelmembers();
        }

        private static async Task PrintChannelmembers()
        {
            var channel = await GetChannelSelection();

            Console.WriteLine();
            if (channel != null)
            {
                var ch = await PublicGraphClient.Teams[GroupId].Channels[channel.Id]
                .Request()
                .GetAsync();

                Console.WriteLine($"Channel[{ch.DisplayName}] members.. ");

                var members = await PublicGraphClient.Teams[GroupId].Channels[ch.Id].Members
                          .Request()
                          .GetAsync();
                foreach (var item in members)
                {
                    Console.WriteLine($"{item.DisplayName}[{string.Join(", ", item.Roles)}]");
                }
            }
            else
            {
                Console.WriteLine("Invalid Channel Selection");
            }
        }

        private static async Task<ChannelInfo> GetChannelSelection()
        {
            var channels = await ListChannels();

            if (channels.Count == 0)
            {
                Console.WriteLine("Channels not Found. Create a new Channel.");
                return null;
            }

            Console.WriteLine("Select channel");
            int selection;

            ConsoleKeyInfo UserInput = Console.ReadKey();

            if (char.IsDigit(UserInput.KeyChar))
            {
                selection = int.Parse(UserInput.KeyChar.ToString());
            }
            else
            {
                selection = -1;
            }



            if (selection <= channels.Count && selection > 0)
            {
                var selectedChannel = channels.ElementAt(selection - 1);
                return selectedChannel;
            }

            return null;
        }
    }
}
