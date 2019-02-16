using System;
using System.Net;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using LongJohnSilver.Statics;
using Newtonsoft.Json.Linq;

namespace LongJohnSilver.Commands.General
{
    public class Weather : ModuleBase<SocketCommandContext>
    {
        [Command("weather")]
        public async Task WeatherReportAsync([Remainder] string input = "")
        {
            var lat = string.Empty;
            var lon = string.Empty;
            var location = string.Empty;

            using (var w = new WebClient())
            {
                var url = $"https://api.opencagedata.com/geocode/v1/json?key={BotSetupHandler.GeoKey}&q={input}";
                var jsonString = w.DownloadString(url);

                dynamic array = JObject.Parse(jsonString);

                location = array.results[0].formatted;
                lat = array.results[0].geometry.lat;
                lon = array.results[0].geometry.lng;
            }

            using (var w = new WebClient())
            {
                var url = $"http://api.openweathermap.org/data/2.5/weather?lat={lat}&lon={lon}&APPID={BotSetupHandler.WeatherKey}";
                var jsonString = w.DownloadString(url);

                dynamic array = JObject.Parse(jsonString);

                string townname = location;
                string country = array.sys.country;
                string description = array.weather[0].description;
                string icon = array.weather[0].icon;
                string weatherSymbol = string.Empty;

                switch (icon)
                {
                    case "01d":
                        weatherSymbol = ":sunny:";
                        break;
                    case "01n":
                        weatherSymbol = ":first_quarter_moon_with_face:";
                        break;
                    case "02d":
                        weatherSymbol = ":white_sun_cloud:";
                        break;
                    case "02n":
                        weatherSymbol = ":cloud:";
                        break;
                    case "03d":
                        weatherSymbol = ":cloud:";
                        break;
                    case "03n":
                        weatherSymbol = ":cloud:";
                        break;
                    case "04d":
                        weatherSymbol = ":cloud:";
                        break;
                    case "04n":
                        weatherSymbol = ":cloud:";
                        break;
                    case "09d":
                        weatherSymbol = ":cloud_rain:";
                        break;
                    case "09n":
                        weatherSymbol = ":cloud_rain:";
                        break;
                    case "10d":
                        weatherSymbol = ":cloud_rain:";
                        break;
                    case "10n":
                        weatherSymbol = ":cloud_rain:";
                        break;
                    case "11d":
                        weatherSymbol = ":thunder_cloud_rain:";
                        break;
                    case "11n":
                        weatherSymbol = ":thunder_cloud_rain:";
                        break;
                    case "13d":
                    case "13n":
                        weatherSymbol = ":cloud_snow:";
                        break;
                    case "50d":
                    case "50n":
                        weatherSymbol = ":foggy:";
                        break;
                    default:
                        weatherSymbol = ":question:";
                        break;
                }


                float temp = array.main.temp;
                var tempC = Math.Round(temp - 273.15, 1);
                var tempF = Math.Round(temp * 9 / 5 - 459.67, 1);

                var embed = new EmbedBuilder();
                embed.WithAuthor($"{townname}, {country}");
                embed.WithColor(40, 200, 150);
                embed.WithDescription($"{weatherSymbol}  *{description}*\n:thermometer:{tempC}c   :thermometer:{tempF}f");

                await Context.Channel.SendMessageAsync("", false, embed.Build());
            }
        }
    }
}
