using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json; //nuget package to initialize parse for json file


//the classes keys and value pairs to show the JSON structure
public class WeatherInfo
{
	public mainData main { get; set; }
	public windData wind { get; set; }
	public Weatherdescription[] Weather { get; set; }
	public SysData Sys { get; set; }
	public string Name { get; set; }
}

public class mainData
{
	public float temp { get; set; }
	public int humidity { get; set; }
}

public class windData
{
	public float speed { get; set; }
}

public class Weatherdescription
{
	public string description { get; set; }
}

public class SysData
{
	public string Country { get; set; }
}

class Program
{
	private const string ApiKey = "4a10b7f5f601e202246857687ab8ed87";
	private const string ApiUrlFormat = "https://api.openweathermap.org/data/2.5/weather?q={0}&appid={1}";

	private const string FavoritesFilePath = "favorite_Cities_Selected.json"; //variable to store the selected user favorite cities
	private const int MaxFavorites = 3; //initializing variable for how many cities they can favorite per runtime

	static async Task Main(string[] args)
	{
		List<string> cities = new List<string>(); //"cities" is the list of strings the user will input that will be stored in the favorites
		List<string> favorites = new List<string>(); //"favorites" Similar to cities is a new list representing a list of their favorited cities.

		for (int i = 0; i < 5; i++) //loop thru all 5 entered cities
		{
			Console.Write($"Please enter name of your city #{i + 1}: "); //should loop thru from 1-5
			string city = Console.ReadLine();
			cities.Add(city);

			// Fetch weather details and display
			await DisplayWeatherDetails(city);
		}

		Console.WriteLine("\nPlease enter your favorite cities from the list of 5 cities:"); //this will let user type favorite city into the console

		for (int i = 0; i < MaxFavorites; i++)
		{
			Console.Write($"Enter the name of your favorite city #{i + 1}: ");
			string favorite_City = Console.ReadLine();

			if (cities.Contains(favorite_City) && !favorites.Contains(favorite_City))
			{
				favorites.Add(favorite_City);
			}
			else
			{
				Console.WriteLine($"Invalid. Please try again.");
				i--; // this works for if the ctiy already exists and has the same index. it handles decrement and allows user to type in city again
			}
		}

		Console.WriteLine("\nYour favorite cities:");
		DisplayFavoriteCities(favorites);

		Console.WriteLine("\nNow, let's update your favorite cities:");

		while (favorites.Count > 0)
		{
			Console.WriteLine("\nYour current favorite cities:");
			DisplayFavoriteCities(favorites);

			Console.Write("\nEnter the name of the city you would like to remove from favorites (or type 'close' to finish): ");
			string remove_City = Console.ReadLine();

			if (remove_City.ToLower() == "close") //closes program as enabled in Tools > debugging > auto close after runtime
			{
				break;
			}

			if (favorites.Contains(remove_City))
			{
				favorites.Remove(remove_City);
				Console.WriteLine($"{remove_City} has been removed from your favorites.");
			}
			else
			{
				Console.WriteLine($"{remove_City} has been removed from your favorites.");
			}
		}

		SaveFavorites(favorites);
		Console.WriteLine("Your favorites are saved for next visit. Closing the application!");
	}

	static void DisplayFavoriteCities(List<string> favorites)
	{
		if (favorites.Count == 0)
		{
			Console.WriteLine("No favorite cities yet."); //if no favorites are pcicked display this and then go back to ask for city again
		}
		else
		{
			foreach (var city in favorites)
			{
				Console.WriteLine(city);
			}
		}
	}

	static async Task DisplayWeatherDetails(string city)
	{
		string apiUrl = string.Format(ApiUrlFormat, city, ApiKey);

		using (HttpClient client = new HttpClient())
		{
			try
			{
				HttpResponseMessage response = await client.GetAsync(apiUrl);


				//Loop through each city and retrieve the weather information
				if (response.IsSuccessStatusCode)
				{
					//display results for weather details in json format with key and value
					string json = await response.Content.ReadAsStringAsync();
					WeatherInfo WeatherInfo = JsonConvert.DeserializeObject<WeatherInfo>(json);

					Console.WriteLine($"\nWeather in {WeatherInfo.Name}, {WeatherInfo.Sys.Country}:"); //adds the country for countries with similar cities 
					Console.WriteLine($"temperature: {WeatherInfo.main.temp}°C");
					Console.WriteLine($"description: {WeatherInfo.Weather[0].description}");
					Console.WriteLine($"humidity: {WeatherInfo.main.humidity}%");
					Console.WriteLine($"wind speed: {WeatherInfo.wind.speed} m/s");
				}
				else
				{
					Console.WriteLine($"Error: Unable to get weather data for {city}"); //handle situations where there's an error during an attempt to fetch weather data (basic exception handler)
				}
			}
			catch (Exception ex)
			{
				Console.WriteLine($"An error occurred: {ex.Message}");
			}
		}
	}

	static void SaveFavorites(List<string> favorites) //this method takes the list of favorite cities (List<string> favorites) and converts it to a json type output, and then saves to "FavoritesFilePath"
	{
		string json = JsonConvert.SerializeObject(favorites, Formatting.Indented);
		File.WriteAllText(FavoritesFilePath, json);
		Console.WriteLine("Favorites saved.");
	}
}

