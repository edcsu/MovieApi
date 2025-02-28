// See https://aka.ms/new-console-template for more information

using System.Text.Json;
using Movies.Api.Sdk;
using Refit;

var moviesApi = RestService.For<IMoviesApi>("https://localhost:7078");

var movie = await moviesApi.GetMovieAsync("nick-the-grrek-2022");

Console.WriteLine(JsonSerializer.Serialize(movie));