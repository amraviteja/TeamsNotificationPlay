// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using Microsoft.Extensions.Configuration;
using System.Collections.Generic;
using System.IO;
using System.Globalization;
using System;

namespace TeamsNotification
{
    public  class ChannelStore
    {
        public List<ChannelInfo> Channels { get; set; }

        public void AddChannel(ChannelInfo channelInfo) {
            if (Channels== null)
            {
                Channels = new List<ChannelInfo>();
            }
            Channels.Add(channelInfo);
        }

        public static ChannelStore ReadFromJsonFile(string path)
        {
            ChannelStore channelStore = new ChannelStore();
            try
            {
                IConfigurationRoot Configuration;

                var builder = new ConfigurationBuilder()
                    .AddJsonFile(path, true, true);


                Configuration = builder.Build();

                return Configuration.Get<ChannelStore>();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

            return channelStore;
        }
    }

    public class ChannelInfo
    {
        public string Id{ get; set; }
        public string Name{ get; set; }       
    }
}