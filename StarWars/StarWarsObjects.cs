using Newtonsoft.Json;
using System;

namespace StarWars
{
    class Film
    {
        [JsonProperty(PropertyName = "title")]
        public string Title { get; set; }
        [JsonProperty(PropertyName = "episode_id")]
        public string EpisodeId { get; set; }
        [JsonProperty(PropertyName = "opening_crawl")]
        public string OpeningCrawl { get; set; }
        [JsonProperty(PropertyName = "director")]
        public string Director { get; set; }
        [JsonProperty(PropertyName = "producer")]
        public string Producer { get; set; }
        [JsonProperty(PropertyName = "release_date")]
        public string ReleaseDate { get; set; }
        [JsonProperty(PropertyName = "characters")]
        public string Characters { get; set; }
        [JsonProperty(PropertyName = "planets")]
        public string Planets { get; set; }
        [JsonProperty(PropertyName = "starships")]
        public string Starships { get; set; }
        [JsonProperty(PropertyName = "vehicles")]
        public string Vehicles { get; set; }
        [JsonProperty(PropertyName = "species")]
        public string Species { get; set; }
        [JsonProperty(PropertyName = "created")]
        public string Created { get; set; }
        [JsonProperty(PropertyName = "edited")]
        public string Edited { get; set; }
        [JsonProperty(PropertyName = "url")]
        public string URL { get; set; }
    }

    class Character
    {
        [JsonProperty(PropertyName = "name")]
        public string Name { get; set; }
        [JsonProperty(PropertyName = "height")]
        public UInt16 Height { get; set; }
        [JsonProperty(PropertyName = "mass")]
        public UInt16 Mass { get; set; }
        [JsonProperty(PropertyName = "hair_color")]
        public string HairColor { get; set; }
        [JsonProperty(PropertyName = "skin_color")]
        public string SkinColor { get; set; }
        [JsonProperty(PropertyName = "eye_color")]
        public string EyeColor { get; set; }
        [JsonProperty(PropertyName = "birth_year")]
        public string BirthYear { get; set; }
        [JsonProperty(PropertyName = "gender")]
        public string Gender { get; set; }
        [JsonProperty(PropertyName = "homeworld")]
        public string Homeworld { get; set; }
        [JsonProperty(PropertyName = "films")]
        public string[] Films { get; set; }
        [JsonProperty(PropertyName = "species")]
        public string[] Species { get; set; }
        [JsonProperty(PropertyName = "vehicles")]
        public string[] Vehicles { get; set; }
        [JsonProperty(PropertyName = "starships")]
        public string[] Starships { get; set; }
        [JsonProperty(PropertyName = "created")]
        public DateTime Created { get; set; }
        [JsonProperty(PropertyName = "edited")]
        public DateTime Edited { get; set; }
        [JsonProperty(PropertyName = "url")]
        public string URL { get; set; }
    }

    class Planet
    {
        [JsonProperty(PropertyName = "name")]
        public string Name { get; set; }
        [JsonProperty(PropertyName = "rotation_period")]
        public UInt16 RotationPeriod { get; set; }
        [JsonProperty(PropertyName = "orbital_period")]
        public UInt16 OrbitalPeriod { get; set; }
        [JsonProperty(PropertyName = "diameter")]
        public UInt32 Diameter { get; set; }
        [JsonProperty(PropertyName = "climate")]
        public string Climate { get; set; }
        [JsonProperty(PropertyName = "gravity")]
        public string Gravity { get; set; }
        [JsonProperty(PropertyName = "terrain")]
        public string Terrain { get; set; }
        [JsonProperty(PropertyName = "surface_water")]
        public UInt16 SurfaceWater { get; set; }
        [JsonProperty(PropertyName = "population")]
        public UInt32 Population { get; set; }
        [JsonProperty(PropertyName = "residents")]
        public string Residents { get; set; }
        [JsonProperty(PropertyName = "films")]
        public string[] Films { get; set; }
        [JsonProperty(PropertyName = "created")]
        public DateTime Created { get; set; }
        [JsonProperty(PropertyName = "edited")]
        public DateTime Edited { get; set; }
        [JsonProperty(PropertyName = "url")]
        public string URL { get; set; }
    }

    class Starship
    {
        [JsonProperty(PropertyName = "name")]
        public string Name { get; set; }
        [JsonProperty(PropertyName = "model")]
        public string Model { get; set; }
        [JsonProperty(PropertyName = "manufacturer")]
        public string Manufacturer { get; set; }
        [JsonProperty(PropertyName = "cost_in_credits")]
        public long CostInCredits { get; set; }
        [JsonProperty(PropertyName = "length")]
        public UInt32 Length { get; set; }
        [JsonProperty(PropertyName = "max_atmosphering_speed")]
        public string MaxAtmospheringSpeed { get; set; }
        [JsonProperty(PropertyName = "crew")]
        public UInt32 Crew { get; set; }
        [JsonProperty(PropertyName = "passengers")]
        public long Passengers { get; set; }
        [JsonProperty(PropertyName = "cargo_capacity")]
        public UInt64 CargoCapacity { get; set; }
        [JsonProperty(PropertyName = "consumables")]
        public string Consumables { get; set; }
        [JsonProperty(PropertyName = "hyperdrive_rating")]
        public Single HyperdriveRating { get; set; }
        [JsonProperty(PropertyName = "MGLT")]
        public byte MGLT { get; set; }
        [JsonProperty(PropertyName = "starship_class")]
        public string StarshipClass { get; set; }
        [JsonProperty(PropertyName = "pilots")]
        public object[] Pilots { get; set; }
        [JsonProperty(PropertyName = "films")]
        public string[] Films { get; set; }
        [JsonProperty(PropertyName = "created")]
        public DateTime Created { get; set; }
        [JsonProperty(PropertyName = "edited")]
        public DateTime Edited { get; set; }
        [JsonProperty(PropertyName = "url")]
        public string URL { get; set; }
    }

}

#region Film
/*
    {
    "title": "A New Hope",
    "episode_id": 4,
    "opening_crawl": "It is a period of civil war.\r\nRebel spaceships, striking\r\nfrom a hidden base, have won\r\ntheir first victory against\r\nthe evil Galactic Empire.\r\n\r\nDuring the battle, Rebel\r\nspies managed to steal secret\r\nplans to the Empire's\r\nultimate weapon, the DEATH\r\nSTAR, an armored space\r\nstation with enough power\r\nto destroy an entire planet.\r\n\r\nPursued by the Empire's\r\nsinister agents, Princess\r\nLeia races home aboard her\r\nstarship, custodian of the\r\nstolen plans that can save her\r\npeople and restore\r\nfreedom to the galaxy....",
    "director": "George Lucas",
    "producer": "Gary Kurtz, Rick McCallum",
    "release_date": "1977-05-25",
    "characters": [
	    "https://swapi.dev/api/people/1/",
	    "https://swapi.dev/api/people/2/",
	    "https://swapi.dev/api/people/3/",
	    "https://swapi.dev/api/people/4/",
	    "https://swapi.dev/api/people/5/",
	    "https://swapi.dev/api/people/6/",
	    "https://swapi.dev/api/people/7/",
	    "https://swapi.dev/api/people/8/",
	    "https://swapi.dev/api/people/9/",
	    "https://swapi.dev/api/people/10/",
	    "https://swapi.dev/api/people/12/",
	    "https://swapi.dev/api/people/13/",
	    "https://swapi.dev/api/people/14/",
	    "https://swapi.dev/api/people/15/",
	    "https://swapi.dev/api/people/16/",
	    "https://swapi.dev/api/people/18/",
	    "https://swapi.dev/api/people/19/",
	    "https://swapi.dev/api/people/81/"
    ],
    "planets": [
	    "https://swapi.dev/api/planets/2/",
	    "https://swapi.dev/api/planets/3/",
	    "https://swapi.dev/api/planets/1/"
    ],
    "starships": [
	    "https://swapi.dev/api/starships/2/",
	    "https://swapi.dev/api/starships/3/",
	    "https://swapi.dev/api/starships/5/",
	    "https://swapi.dev/api/starships/9/",
	    "https://swapi.dev/api/starships/10/",
	    "https://swapi.dev/api/starships/11/",
	    "https://swapi.dev/api/starships/12/",
	    "https://swapi.dev/api/starships/13/"
    ],
    "vehicles": [
	    "https://swapi.dev/api/vehicles/4/",
	    "https://swapi.dev/api/vehicles/6/",
	    "https://swapi.dev/api/vehicles/7/",
	    "https://swapi.dev/api/vehicles/8/"
    ],
    "species": [
	    "https://swapi.dev/api/species/5/",
	    "https://swapi.dev/api/species/3/",
	    "https://swapi.dev/api/species/2/",
	    "https://swapi.dev/api/species/1/",
	    "https://swapi.dev/api/species/4/"
    ],
    "created": "2014-12-10T14:23:31.880000Z",
    "edited": "2015-04-11T09:46:52.774897Z",
    "url": "https://swapi.dev/api/films/1/"
    },   
*/
#endregion