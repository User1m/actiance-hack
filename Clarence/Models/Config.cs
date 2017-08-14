using Microsoft.Bot.Connector;
using System.Configuration;

public class Config
{
    public static string ServiceUrl { get; set; } = ConfigurationManager.AppSettings["ServiceUrl"];
    public static ChannelAccount Bot { get; } = new ChannelAccount(id: ConfigurationManager.AppSettings["BotId"], name: "Clarence");
    public static string ChannelId { get; } = Config.ChannelId;
}