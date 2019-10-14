using System;
using Kraken.Net;
using CryptoExchange.Net.Logging;
using Kraken.Net.Objects.Socket;
using Kraken.Net.Objects;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace Kraken_Demo
{
    class Program
    {
        // run multiple times. Until red font color at console
        static void Main()
        {
            var options = new KrakenSocketClientOptions()
            {
                LogVerbosity = LogVerbosity.Debug
            };
            var dict = new Dictionary<string, string>();
            dict.Add("ETH/USD", "ETH/USD");
            dict.Add("BTC/USD", "XBT/USD");
            dict.Add("ETC/ETH", "ETC/ETH");

            using var socket = new KrakenSocketClient(options);

            void handler(string name, KrakenSocketEvent<KrakenStreamOrderBook> depth)
            {
                var ask = depth.Data.Asks.Length == 0 ? 0 : depth.Data.Asks.Min(x => x.Price);
                var bid = depth.Data.Bids.Length == 0 ? 0 : depth.Data.Bids.Max(x => x.Price);
                
                if (dict[name] != depth.Market) // this condition must always return false. But it's not
                    Console.ForegroundColor = ConsoleColor.Red;
                
                Console.WriteLine($"{depth.ChannelId}| {depth.Market} [{name}]: {ask} - {bid}");

                Console.ForegroundColor = ConsoleColor.White;
            }
            foreach (var symbol in dict.Keys)
                socket.SubscribeToDepthUpdates(symbol, 25, x => handler(symbol, x));
            // messages may be sent in the wrong handler (in etc/eth handler i received depth for xbt/usd)
            Console.ReadKey();
        }
    }
}
