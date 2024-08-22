using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;

namespace GameServer.Models.Config
{
    public class ModMileConfig
    {
        private static ModMileConfig instance = null;

        public static ModMileConfig Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = GetFromFile();
                }

                return instance;
            }
        }

        public bool RequireUniquePlayersToCheckIn = true;
        public Dictionary<int, ModExplorerCity> Cities = new()
        {
            { 1, new ModExplorerCity {
                Name = "Paris",
                Country = "France",
                Latitude = 48.856667f,
                Longitude = 2.350833f
            } },
            { 2, new ModExplorerCity {
                Name = "London",
                Country = "United Kingdom",
                Latitude = 51.507222f,
                Longitude = -0.1275f
            } },
            { 3, new ModExplorerCity {
                Name = "Singapore",
                Country = "Singapore",
                Latitude = 1.283333f,
                Longitude = 103.833333f
            } },
            { 4, new ModExplorerCity {
                Name = "Hong Kong",
                Country = "China",
                Latitude = 22.278333f,
                Longitude = 114.158889f
            } },
            { 5, new ModExplorerCity {
                Name = "New York",
                Country = "USA",
                Latitude = 40.716667f,
                Longitude = -74f
            } },
            { 6, new ModExplorerCity {
                Name = "Dubai",
                Country = "United Arab Emirates",
                Latitude = 25.25f,
                Longitude = 55.3f
            } },
            { 7, new ModExplorerCity {
                Name = "Rome",
                Country = "Italy",
                Latitude = 41.9f,
                Longitude = 12.5f
            } },
            { 8, new ModExplorerCity {
                Name = "Sydney",
                Country = "Australia",
                Latitude = -33.859972f,
                Longitude = 151.211111f
            } },
            { 9, new ModExplorerCity {
                Name = "Madrid",
                Country = "Spain",
                Latitude = 40.4f,
                Longitude = -3.683333f
            } },
            { 10, new ModExplorerCity {
                Name = "Athens",
                Country = "Greece",
                Latitude = 37.966667f,
                Longitude = 23.716667f
            } },
            { 11, new ModExplorerCity {
                Name = "Beijing",
                Country = "China",
                Latitude = 39.913889f,
                Longitude = 116.391667f
            } },
            { 12, new ModExplorerCity {
                Name = "Vienna",
                Country = "Austria",
                Latitude = 48.208333f,
                Longitude = 16.373056f
            } },
            { 13, new ModExplorerCity {
                Name = "Mexico City",
                Country = "Mexico",
                Latitude = 19.433333f,
                Longitude = -99.133333f
            } },
            { 14, new ModExplorerCity {
                Name = "Berlin",
                Country = "Germany",
                Latitude = 52.500556f,
                Longitude = 13.398889f
            } },
            { 15, new ModExplorerCity {
                Name = "Toronto",
                Country = "Canada",
                Latitude = 43.716589f,
                Longitude = -79.340686f
            } },
            { 16, new ModExplorerCity {
                Name = "Moscow",
                Country = "Russia",
                Latitude = 55.751667f,
                Longitude = 37.617778f
            } },
            { 17, new ModExplorerCity {
                Name = "Tokyo",
                Country = "Japan",
                Latitude = 35.700556f,
                Longitude = 139.715f
            } },
            { 18, new ModExplorerCity {
                Name = "Washington, D.C.",
                Country = "USA",
                Latitude = 38.895111f,
                Longitude = -77.036667f
            } },
            { 19, new ModExplorerCity {
                Name = "Los Angeles",
                Country = "USA",
                Latitude = 34.05f,
                Longitude = -118.25f
            } },
            { 20, new ModExplorerCity {
                Name = "Las Vegas",
                Country = "USA",
                Latitude = 36.176f,
                Longitude = -115.137f
            } },
            { 21, new ModExplorerCity {
                Name = "Honolulu",
                Country = "USA",
                Latitude = 21.308889f,
                Longitude = -157.826111f
            } },
            { 22, new ModExplorerCity {
                Name = "Seoul",
                Country = "South Korea",
                Latitude = 37.568889f,
                Longitude = 126.976667f
            } },
            { 23, new ModExplorerCity {
                Name = "Kyoto",
                Country = "Japan",
                Latitude = 35.011581f,
                Longitude = 135.768186f
            } },
            { 24, new ModExplorerCity {
                Name = "Vancouver",
                Country = "Canada",
                Latitude = 49.25f,
                Longitude = -123.1f
            } },
            { 25, new ModExplorerCity {
                Name = "San Francisco",
                Country = "USA",
                Latitude = 37.7793f,
                Longitude = -122.4192f
            } },
            { 26, new ModExplorerCity {
                Name = "Johannesburg",
                Country = "South Africa",
                Latitude = -26.204444f,
                Longitude = 28.045556f
            } },
            { 27, new ModExplorerCity {
                Name = "Sao Paulo",
                Country = "Brazil",
                Latitude = -23.55f,
                Longitude = -46.633333f
            } },
            { 28, new ModExplorerCity {
                Name = "Buenos Aires",
                Country = "Argentina",
                Latitude = -34.603333f,
                Longitude = -58.381667f
            } },
            { 29, new ModExplorerCity {
                Name = "Rio de Janeiro",
                Country = "Brazil",
                Latitude = -22.908333f,
                Longitude = -43.196389f
            } },
            { 30, new ModExplorerCity {
                Name = "Cape Town",
                Country = "South Africa",
                Latitude = -33.925278f,
                Longitude = 18.423889f
            } },
            { 31, new ModExplorerCity {
                Name = "Brussels",
                Country = "Belgium",
                Latitude = 50.85f,
                Longitude = 4.35f
            } },
            { 32, new ModExplorerCity {
                Name = "Copenhagen",
                Country = "Denmark",
                Latitude = 55.676111f,
                Longitude = 12.568333f
            } },
            { 33, new ModExplorerCity {
                Name = "Helsinki",
                Country = "Finland",
                Latitude = 60.170833f,
                Longitude = 24.9375f
            } },
            { 34, new ModExplorerCity {
                Name = "Dublin",
                Country = "Ireland",
                Latitude = 53.347778f,
                Longitude = -6.259722f
            } },
            { 35, new ModExplorerCity {
                Name = "Monaco",
                Country = "Monaco",
                Latitude = 43.732778f,
                Longitude = 7.419722f
            } },
            { 36, new ModExplorerCity {
                Name = "Oslo",
                Country = "Norway",
                Latitude = 59.949444f,
                Longitude = 10.756389f
            } },
            { 37, new ModExplorerCity {
                Name = "Warsaw",
                Country = "Poland",
                Latitude = 52.23f,
                Longitude = 21.010833f
            } },
            { 38, new ModExplorerCity {
                Name = "Lisbon",
                Country = "Portugal",
                Latitude = 38.706944f,
                Longitude = -9.135278f
            } },
            { 39, new ModExplorerCity {
                Name = "Stockholm",
                Country = "Sweden",
                Latitude = 59.35f,
                Longitude = 18.066667f
            } },
            { 40, new ModExplorerCity {
                Name = "Istanbul",
                Country = "Turkey",
                Latitude = 41.01224f,
                Longitude = 28.976018f
            } },
            { 41, new ModExplorerCity {
                Name = "Saint Petersburg",
                Country = "Russia",
                Latitude = 59.95f,
                Longitude = 30.316667f
            } },
            { 42, new ModExplorerCity {
                Name = "Melbourne",
                Country = "Australia",
                Latitude = -37.813611f,
                Longitude = 144.963056f
            } },
            { 43, new ModExplorerCity {
                Name = "Chicago",
                Country = "USA",
                Latitude = 41.881944f,
                Longitude = -87.627778f
            } },
            { 44, new ModExplorerCity {
                Name = "Houston",
                Country = "USA",
                Latitude = 29.762778f,
                Longitude = -95.383056f
            } },
            { 45, new ModExplorerCity {
                Name = "Dallas",
                Country = "USA",
                Latitude = 32.782778f,
                Longitude = -96.803889f
            } },
        };
        public Dictionary<int, PointOfInterest> PointsOfInterest = new()
        {
            { 1, new PointOfInterest {
                Name = "Eiffel Tower",
                CityId = 1,
                Latitude = 48.8583f,
                Longitude = 2.2945f,
                Radius = 0.35f,
                Awards =
                [
                    new PointOfInterestAward
                    {
                        Name = "Part_Kart_Body_Swan",
                        Type = "Kart_Asset",
                        CheckIns = 10
                    },
                    new PointOfInterestAward
                    {
                        Name = "Part_Kart_Wheel_Rose",
                        Type = "Kart_Asset",
                        CheckIns = 50
                    },
                    new PointOfInterestAward
                    {
                        Name = "Part_Kart_Steering_Heart",
                        Type = "Kart_Asset",
                        CheckIns = 100
                    },
                    new PointOfInterestAward
                    {
                        Name = "Part_Kart_Seat_Ring",
                        Type = "Kart_Asset",
                        CheckIns = 200
                    },
                    new PointOfInterestAward
                    {
                        Name = "Part_Kart_Engine_Valentine",
                        Type = "Kart_Asset",
                        CheckIns = 300
                    },
                    new PointOfInterestAward
                    {
                        Name = "00000098_VALENTINEKART",
                        Type = "Kart",
                        CheckIns = 400
                    },
                ]
            } },
            { 2, new PointOfInterest {
                Name = "Arc de Triomphe",
                CityId = 1,
                Latitude = 48.8738f,
                Longitude = 2.295f,
                Radius = 0.3f,
                Awards =
                [
                    new PointOfInterestAward
                    {
                        Name = "Mask_Wasp",
                        Type = "Character_Asset",
                        CheckIns = 10
                    },
                    new PointOfInterestAward
                    {
                        Name = "Part_Char_Feeler",
                        Type = "Character_Asset",
                        CheckIns = 100
                    },
                    new PointOfInterestAward
                    {
                        Name = "000004F0_DLCCHAR_HOLIDAYVALENTINE",
                        Type = "Character",
                        CheckIns = 250
                    },
                    new PointOfInterestAward
                    {
                        Name = "Part_Char_Pattern_Insect",
                        Type = "Character_Asset",
                        CheckIns = 400
                    },
                ]
            } },
            { 3, new PointOfInterest {
                Name = "Musee du Louvre",
                CityId = 1,
                Latitude = 48.860395f,
                Longitude = 2.337599f,
                Radius = 0.3f,
                Awards =
                [
                    new PointOfInterestAward
                    {
                        Name = "Medieval_ARC_FarmhouseWSty",
                        Type = "Track_Asset",
                        CheckIns = 10
                    },
                    new PointOfInterestAward
                    {
                        Name = "Medieval_ARC_Stables01",
                        Type = "Track_Asset",
                        CheckIns = 100
                    },
                    new PointOfInterestAward
                    {
                        Name = "Medieval_ARC_VillageHut02",
                        Type = "Track_Asset",
                        CheckIns = 250
                    },
                    new PointOfInterestAward
                    {
                        Name = "Medieval_PROP_KnightsHorse",
                        Type = "Track_Asset",
                        CheckIns = 400
                    },
                ]
            } },
            { 4, new PointOfInterest {
                Name = "Tower of London",
                CityId = 2,
                Latitude = 51.508056f,
                Longitude = -0.076111f,
                Radius = 0.3f,
                Awards =
                [
                    new PointOfInterestAward
                    {
                        Name = "000004C9_DLCCHAR14",
                        Type = "Character",
                        CheckIns = 10
                    },
                    new PointOfInterestAward
                    {
                        Name = "Part_Char_Accessories_90",
                        Type = "Character_Asset",
                        CheckIns = 100
                    },
                    new PointOfInterestAward
                    {
                        Name = "Stunt_SMK_GlassPlate",
                        Type = "Track_Asset",
                        CheckIns = 250
                    },
                    new PointOfInterestAward
                    {
                        Name = "Part_Char_Shirt_pilotjacket",
                        Type = "Character_Asset",
                        CheckIns = 400
                    },
                ]
            } },
            { 5, new PointOfInterest {
                Name = "London Eye",
                CityId = 2,
                Latitude = 51.5033f,
                Longitude = -0.1197f,
                Radius = 0.3f,
                Awards =
                [
                    new PointOfInterestAward
                    {
                        Name = "Part_Kart_Ornament_Kart_Ornament4",
                        Type = "Kart_Asset",
                        CheckIns = 10
                    },
                    new PointOfInterestAward
                    {
                        Name = "Part_Kart_Steering_X",
                        Type = "Kart_Asset",
                        CheckIns = 100
                    },
                    new PointOfInterestAward
                    {
                        Name = "00000074_DLCKART14",
                        Type = "Kart",
                        CheckIns = 250
                    },
                    new PointOfInterestAward
                    {
                        Name = "Part_Kart_Body_Biplane",
                        Type = "Kart_Asset",
                        CheckIns = 400
                    },
                ]
            } },
            { 6, new PointOfInterest {
                Name = "Buckingham Palace",
                CityId = 2,
                Latitude = 51.501f,
                Longitude = -0.142f,
                Radius = 0.3f,
                Awards =
                [
                    new PointOfInterestAward
                    {
                        Name = "Medieval_PROP_JoustingFence",
                        Type = "Track_Asset",
                        CheckIns = 10
                    },
                    new PointOfInterestAward
                    {
                        Name = "Medieval_PROP_JoustingFenceEnd",
                        Type = "Track_Asset",
                        CheckIns = 100
                    },
                    new PointOfInterestAward
                    {
                        Name = "Medieval_PROP_JoustingTent02",
                        Type = "Track_Asset",
                        CheckIns = 250
                    },
                    new PointOfInterestAward
                    {
                        Name = "Medieval_PROP_JoustingTent01",
                        Type = "Track_Asset",
                        CheckIns = 400
                    },
                ]
            } },
            { 7, new PointOfInterest {
                Name = "Orchard Road",
                CityId = 3,
                Latitude = 1.305083f,
                Longitude = 103.831908f,
                Radius = 0.45f,
                Awards =
                [
                    new PointOfInterestAward
                    {
                        Name = "Part_Kart_Body_Frontloader",
                        Type = "Kart_Asset",
                        CheckIns = 10
                    },
                    new PointOfInterestAward
                    {
                        Name = "Construction_PROP_PortaCabin",
                        Type = "Track_Asset",
                        CheckIns = 100
                    },
                    new PointOfInterestAward
                    {
                        Name = "Construction_PROP_Scaffold_Run",
                        Type = "Track_Asset",
                        CheckIns = 200
                    },
                    new PointOfInterestAward
                    {
                        Name = "Construction_PROP_Scaffold_Ramp",
                        Type = "Track_Asset",
                        CheckIns = 300
                    },
                ]
            } },
            { 8, new PointOfInterest {
                Name = "Tiger Sky Tower",
                CityId = 3,
                Latitude = 1.255f,
                Longitude = 103.817778f,
                Radius = 0.14f,
                Awards =
                [
                    new PointOfInterestAward
                    {
                        Name = "Part_Char_Shirt_Dress01",
                        Type = "Character_Asset",
                        CheckIns = 10
                    },
                    new PointOfInterestAward
                    {
                        Name = "Pants_Fishnet",
                        Type = "Character_Asset",
                        CheckIns = 100
                    },
                    new PointOfInterestAward
                    {
                        Name = "000004F7_MODIVACHAR",
                        Type = "Character",
                        CheckIns = 200
                    },
                    new PointOfInterestAward
                    {
                        Name = "Shoe_Highheels",
                        Type = "Character_Asset",
                        CheckIns = 300
                    },
                ]
            } },
            { 9, new PointOfInterest {
                Name = "Singapore Zoo",
                CityId = 3,
                Latitude = 1.404417f,
                Longitude = 103.791139f,
                Radius = 0.4f,
                Awards =
                [
                    new PointOfInterestAward
                    {
                        Name = "Construction_PROP_DumpsterRamp",
                        Type = "Track_Asset",
                        CheckIns = 10
                    },
                    new PointOfInterestAward
                    {
                        Name = "Construction_SMK_GiantTire",
                        Type = "Track_Asset",
                        CheckIns = 100
                    },
                    new PointOfInterestAward
                    {
                        Name = "Construction_PROP_RubbleRamp",
                        Type = "Track_Asset",
                        CheckIns = 200
                    },
                    new PointOfInterestAward
                    {
                        Name = "Construction_PROP_PipeRamp",
                        Type = "Track_Asset",
                        CheckIns = 300
                    },
                ]
            } },
            { 10, new PointOfInterest {
                Name = "Hong Kong Museum of Art",
                CityId = 4,
                Latitude = 22.293547f,
                Longitude = 114.172025f,
                Radius = 0.1f,
                Awards =
                [
                    new PointOfInterestAward
                    {
                        Name = "Part_Char_Accessories_Helmet_Takeout",
                        Type = "Character_Asset",
                        CheckIns = 10
                    },
                    new PointOfInterestAward
                    {
                        Name = "Part_Kart_Ornament_Kart_Ornament67",
                        Type = "Kart_Asset",
                        CheckIns = 150
                    },
                    new PointOfInterestAward
                    {
                        Name = "Part_Kart_Ornament_Kart_Ornament69",
                        Type = "Kart_Asset",
                        CheckIns = 300
                    },
                    new PointOfInterestAward
                    {
                        Name = "Part_Kart_Wheel_YingYang",
                        Type = "Kart_Asset",
                        CheckIns = 400
                    },
                ]
            } },
            { 11, new PointOfInterest {
                Name = "Clock Tower",
                CityId = 4,
                Latitude = 22.293678f,
                Longitude = 114.169364f,
                Radius = 0.15f,
                Awards =
                [
                    new PointOfInterestAward
                    {
                        Name = "000004CA_DLCCHAR15",
                        Type = "Character",
                        CheckIns = 10
                    },
                    new PointOfInterestAward
                    {
                        Name = "Part_Char_Shoe_Fireman",
                        Type = "Character_Asset",
                        CheckIns = 100
                    },
                    new PointOfInterestAward
                    {
                        Name = "Part_Char_Shirt_Fireman",
                        Type = "Character_Asset",
                        CheckIns = 200
                    },
                    new PointOfInterestAward
                    {
                        Name = "Part_Char_Pants_Fireman",
                        Type = "Character_Asset",
                        CheckIns = 300
                    },
                    new PointOfInterestAward
                    {
                        Name = "Part_Char_Helmet_Fireman",
                        Type = "Character_Asset",
                        CheckIns = 400
                    },
                ]
            } },
            { 12, new PointOfInterest {
                Name = "International Commerce Centre",
                CityId = 4,
                Latitude = 22.303392f,
                Longitude = 114.160169f,
                Radius = 0.15f,
                Awards =
                [
                    new PointOfInterestAward
                    {
                        Name = "Stunt_PROP_FireRing",
                        Type = "Track_Asset",
                        CheckIns = 10
                    },
                    new PointOfInterestAward
                    {
                        Name = "Part_Kart_Ornament_Kart_Ornament32",
                        Type = "Kart_Asset",
                        CheckIns = 150
                    },
                    new PointOfInterestAward
                    {
                        Name = "00000075_DLCKART15",
                        Type = "Kart",
                        CheckIns = 300
                    },
                    new PointOfInterestAward
                    {
                        Name = "Part_Kart_Body_Firetruck",
                        Type = "Kart_Asset",
                        CheckIns = 400
                    },
                ]
            } },
            { 13, new PointOfInterest {
                Name = "Statue of Liberty",
                CityId = 5,
                Latitude = 40.689167f,
                Longitude = -74.044444f,
                Radius = 1.95f,
                Awards =
                [
                    new PointOfInterestAward
                    {
                        Name = "00000096_SANTAKART",
                        Type = "Kart",
                        CheckIns = 10
                    },
                    new PointOfInterestAward
                    {
                        Name = "Part_Kart_Engine_Presents",
                        Type = "Kart_Asset",
                        CheckIns = 100
                    },
                    new PointOfInterestAward
                    {
                        Name = "Part_voice_SANTA",
                        Type = "Character_Asset",
                        CheckIns = 200
                    },
                    new PointOfInterestAward
                    {
                        Name = "SantaBelt",
                        Type = "Character_Asset",
                        CheckIns = 350
                    },
                    new PointOfInterestAward
                    {
                        Name = "Part_Kart_Body_Sleigh",
                        Type = "Kart_Asset",
                        CheckIns = 500
                    },
                ]
            } },
            { 14, new PointOfInterest {
                Name = "Empire State Building",
                CityId = 5,
                Latitude = 40.748433f,
                Longitude = -73.985656f,
                Radius = 0.1f,
                Awards =
                [
                    new PointOfInterestAward
                    {
                        Name = "Mask_Santa",
                        Type = "Character_Asset",
                        CheckIns = 10
                    },
                    new PointOfInterestAward
                    {
                        Name = "Part_Char_Shoe_Santa",
                        Type = "Character_Asset",
                        CheckIns = 100
                    },
                    new PointOfInterestAward
                    {
                        Name = "Part_Char_Shirt_Santa",
                        Type = "Character_Asset",
                        CheckIns = 200
                    },
                    new PointOfInterestAward
                    {
                        Name = "Pants_Santa",
                        Type = "Character_Asset",
                        CheckIns = 300
                    },
                    new PointOfInterestAward
                    {
                        Name = "000004ED_DLCCHAR_HOLIDAYCHRISTMAS",
                        Type = "Character",
                        CheckIns = 400
                    },
                    new PointOfInterestAward
                    {
                        Name = "Hat_Santa",
                        Type = "Character_Asset",
                        CheckIns = 500
                    },
                ]
            } },
            { 15, new PointOfInterest {
                Name = "Times Square",
                CityId = 5,
                Latitude = 40.75773f,
                Longitude = -73.985708f,
                Radius = 0.16f,
                Awards =
                [
                    new PointOfInterestAward
                    {
                        Name = "Part_Char_Shirt_Messy",
                        Type = "Character_Asset",
                        CheckIns = 10
                    },
                    new PointOfInterestAward
                    {
                        Name = "000004EE_DLCCHAR_HOLIDAYNEWYEAR",
                        Type = "Character",
                        CheckIns = 100
                    },
                    new PointOfInterestAward
                    {
                        Name = "Part_Char_Noisemaker",
                        Type = "Character_Asset",
                        CheckIns = 250
                    },
                    new PointOfInterestAward
                    {
                        Name = "Hat_Lamp",
                        Type = "Character_Asset",
                        CheckIns = 500
                    },
                ]
            } },
            { 16, new PointOfInterest {
                Name = "Dubai Museum",
                CityId = 6,
                Latitude = 25.263056f,
                Longitude = 55.297222f,
                Radius = 0.2f,
                Awards =
                [
                    new PointOfInterestAward
                    {
                        Name = "Construction_SMK_CableSpool",
                        Type = "Track_Asset",
                        CheckIns = 10
                    },
                    new PointOfInterestAward
                    {
                        Name = "Construction_PROP_ConcreteBlock",
                        Type = "Track_Asset",
                        CheckIns = 50
                    },
                    new PointOfInterestAward
                    {
                        Name = "Construction_PROP_MetalTruss",
                        Type = "Track_Asset",
                        CheckIns = 100
                    },
                    new PointOfInterestAward
                    {
                        Name = "Construction_AI_PileDriver",
                        Type = "Track_Asset",
                        CheckIns = 150
                    },
                ]
            } },
            { 17, new PointOfInterest {
                Name = "Burj Al Arab",
                CityId = 6,
                Latitude = 25.141975f,
                Longitude = 55.186147f,
                Radius = 0.5f,
                Awards =
                [
                    new PointOfInterestAward
                    {
                        Name = "Part_Char_hand_bracelets",
                        Type = "Character_Asset",
                        CheckIns = 10
                    },
                    new PointOfInterestAward
                    {
                        Name = "Pants_India",
                        Type = "Character_Asset",
                        CheckIns = 50
                    },
                    new PointOfInterestAward
                    {
                        Name = "Part_Char_Shirt_India",
                        Type = "Character_Asset",
                        CheckIns = 100
                    },
                    new PointOfInterestAward
                    {
                        Name = "Hat_Turban",
                        Type = "Character_Asset",
                        CheckIns = 150
                    },
                ]
            } },
            { 18, new PointOfInterest {
                Name = "Burj Khalifa",
                CityId = 6,
                Latitude = 25.197139f,
                Longitude = 55.274111f,
                Radius = 0.5f,
                Awards =
                [
                    new PointOfInterestAward
                    {
                        Name = "Part_voice_Babu",
                        Type = "Character_Asset",
                        CheckIns = 10
                    },
                    new PointOfInterestAward
                    {
                        Name = "Ear_Elephant",
                        Type = "Character_Asset",
                        CheckIns = 50
                    },
                    new PointOfInterestAward
                    {
                        Name = "Part_Char_Pattern_Elephant",
                        Type = "Character_Asset",
                        CheckIns = 100
                    },
                    new PointOfInterestAward
                    {
                        Name = "Mask_Elephant",
                        Type = "Character_Asset",
                        CheckIns = 150
                    },
                ]
            } },
            { 19, new PointOfInterest {
                Name = "Colosseum",
                CityId = 7,
                Latitude = 41.890169f,
                Longitude = 12.492269f,
                Radius = 0.35f,
                Awards =
                [
                    new PointOfInterestAward
                    {
                        Name = "Gladiator_PROP_StatueFinger",
                        Type = "Track_Asset",
                        CheckIns = 10
                    },
                    new PointOfInterestAward
                    {
                        Name = "Gladiator_SMK_TrainingDummy",
                        Type = "Track_Asset",
                        CheckIns = 50
                    },
                    new PointOfInterestAward
                    {
                        Name = "Gladiator_PROP_Statue03",
                        Type = "Track_Asset",
                        CheckIns = 100
                    },
                    new PointOfInterestAward
                    {
                        Name = "Gladiator_PROP_GiantFlame",
                        Type = "Track_Asset",
                        CheckIns = 150
                    },
                ]
            } },
            { 20, new PointOfInterest {
                Name = "St. Peter's Basilica",
                CityId = 7,
                Latitude = 41.902222f,
                Longitude = 12.453333f,
                Radius = 0.15f,
                Awards =
                [
                    new PointOfInterestAward
                    {
                        Name = "Progressive_PROP_Balloon01",
                        Type = "Track_Asset",
                        CheckIns = 10
                    },
                    new PointOfInterestAward
                    {
                        Name = "Part_Kart_Engine_EspressoMachine",
                        Type = "Kart_Asset",
                        CheckIns = 50
                    },
                    new PointOfInterestAward
                    {
                        Name = "Progressive_PROP_HotairBalloon01",
                        Type = "Track_Asset",
                        CheckIns = 100
                    },
                    new PointOfInterestAward
                    {
                        Name = "Progressive_PROP_TPP11_Blimp",
                        Type = "Track_Asset",
                        CheckIns = 150
                    },
                ]
            } },
            { 21, new PointOfInterest {
                Name = "The Pantheon",
                CityId = 7,
                Latitude = 41.8986f,
                Longitude = 12.4768f,
                Radius = 0.15f,
                Awards =
                [
                    new PointOfInterestAward
                    {
                        Name = "Gladiator_PROP_Columns01",
                        Type = "Track_Asset",
                        CheckIns = 10
                    },
                    new PointOfInterestAward
                    {
                        Name = "Gladiator_PROP_Columns02",
                        Type = "Track_Asset",
                        CheckIns = 50
                    },
                    new PointOfInterestAward
                    {
                        Name = "Gladiator_PROP_Columns03",
                        Type = "Track_Asset",
                        CheckIns = 100
                    },
                    new PointOfInterestAward
                    {
                        Name = "Gladiator_PROP_AquaductArch",
                        Type = "Track_Asset",
                        CheckIns = 150
                    },
                ]
            } },
            { 22, new PointOfInterest {
                Name = "Sydney Opera House",
                CityId = 8,
                Latitude = -33.856944f,
                Longitude = 151.215278f,
                Radius = 0.13f,
                Awards =
                [
                    new PointOfInterestAward
                    {
                        Name = "000004C6_DLCCHAR11",
                        Type = "Character",
                        CheckIns = 10
                    },
                    new PointOfInterestAward
                    {
                        Name = "Part_Char_Shirt_Dirtyundershirt",
                        Type = "Character_Asset",
                        CheckIns = 50
                    },
                    new PointOfInterestAward
                    {
                        Name = "Part_Char_HillBilly",
                        Type = "Character_Asset",
                        CheckIns = 100
                    },
                    new PointOfInterestAward
                    {
                        Name = "Part_Char_Accessories_11",
                        Type = "Character_Asset",
                        CheckIns = 150
                    },
                    new PointOfInterestAward
                    {
                        Name = "Part_Char_Pants_Dirtyoveralls",
                        Type = "Character_Asset",
                        CheckIns = 200
                    },
                ]
            } },
            { 23, new PointOfInterest {
                Name = "Sydney Tower",
                CityId = 8,
                Latitude = -33.870456f,
                Longitude = 151.208889f,
                Radius = 0.1f,
                Awards =
                [
                    new PointOfInterestAward
                    {
                        Name = "WaterWorld_PROP_InflatableWave",
                        Type = "Track_Asset",
                        CheckIns = 10
                    },
                    new PointOfInterestAward
                    {
                        Name = "WaterWorld_PROP_TPP7_driveThruJump",
                        Type = "Track_Asset",
                        CheckIns = 50
                    },
                    new PointOfInterestAward
                    {
                        Name = "WaterWorld_PROP_InflatableDome",
                        Type = "Track_Asset",
                        CheckIns = 100
                    },
                    new PointOfInterestAward
                    {
                        Name = "WaterWorld_PROP_InflatableHoop",
                        Type = "Track_Asset",
                        CheckIns = 200
                    },
                ]
            } },
            { 24, new PointOfInterest {
                Name = "Australian Museum",
                CityId = 8,
                Latitude = -33.874321f,
                Longitude = 151.212732f,
                Radius = 0.1f,
                Awards =
                [
                    new PointOfInterestAward
                    {
                        Name = "Stunt_AI_TiltingPlatform_01",
                        Type = "Track_Asset",
                        CheckIns = 10
                    },
                    new PointOfInterestAward
                    {
                        Name = "Part_Kart_Seat_Mattress",
                        Type = "Kart_Asset",
                        CheckIns = 50
                    },
                    new PointOfInterestAward
                    {
                        Name = "00000071_DLCKART11",
                        Type = "Kart",
                        CheckIns = 100
                    },
                    new PointOfInterestAward
                    {
                        Name = "Part_Kart_Body_Vintage2",
                        Type = "Kart_Asset",
                        CheckIns = 200
                    },
                ]
            } },
            { 25, new PointOfInterest {
                Name = "Royal Palace of Madrid",
                CityId = 9,
                Latitude = 40.418056f,
                Longitude = -3.713889f,
                Radius = 0.2f,
                Awards =
                [
                    new PointOfInterestAward
                    {
                        Name = "Pirate_ARC_Dwelling01",
                        Type = "Track_Asset",
                        CheckIns = 10
                    },
                    new PointOfInterestAward
                    {
                        Name = "Medieval_PROP_Ox",
                        Type = "Track_Asset",
                        CheckIns = 100
                    },
                    new PointOfInterestAward
                    {
                        Name = "Medieval_SMK_BanquetTable",
                        Type = "Track_Asset",
                        CheckIns = 200
                    },
                    new PointOfInterestAward
                    {
                        Name = "Medieval_AI_Cannon01",
                        Type = "Track_Asset",
                        CheckIns = 300
                    },
                ]
            } },
            { 26, new PointOfInterest {
                Name = "Prado Museum",
                CityId = 9,
                Latitude = 40.413889f,
                Longitude = -3.6925f,
                Radius = 0.15f,
                Awards =
                [
                    new PointOfInterestAward
                    {
                        Name = "Pirate_PROP_WoodenCrane",
                        Type = "Track_Asset",
                        CheckIns = 10
                    },
                    new PointOfInterestAward
                    {
                        Name = "Pirate_ARC_Dwelling02",
                        Type = "Track_Asset",
                        CheckIns = 100
                    },
                    new PointOfInterestAward
                    {
                        Name = "Pirate_ARC_Pub",
                        Type = "Track_Asset",
                        CheckIns = 200
                    },
                    new PointOfInterestAward
                    {
                        Name = "Pirate_ARC_Shop01",
                        Type = "Track_Asset",
                        CheckIns = 300
                    },
                ]
            } },
            { 27, new PointOfInterest {
                Name = "Temple of Debod",
                CityId = 9,
                Latitude = 40.424053f,
                Longitude = -3.717778f,
                Radius = 0.2f,
                Awards =
                [
                    new PointOfInterestAward
                    {
                        Name = "Pirate_ARC_Blacksmith_Pirate",
                        Type = "Track_Asset",
                        CheckIns = 10
                    },
                    new PointOfInterestAward
                    {
                        Name = "Pirate_PROP_ShipDryDock",
                        Type = "Track_Asset",
                        CheckIns = 100
                    },
                    new PointOfInterestAward
                    {
                        Name = "Pirate_PROP_Jetty",
                        Type = "Track_Asset",
                        CheckIns = 200
                    },
                    new PointOfInterestAward
                    {
                        Name = "Pirate_ARC_Warehouse",
                        Type = "Track_Asset",
                        CheckIns = 300
                    },
                ]
            } },
            { 28, new PointOfInterest {
                Name = "Acropolis of Athens",
                CityId = 10,
                Latitude = 37.971421f,
                Longitude = 23.726166f,
                Radius = 0.2f,
                Awards =
                [
                    new PointOfInterestAward
                    {
                        Name = "Gladiator_PROP_StatueHead",
                        Type = "Track_Asset",
                        CheckIns = 10
                    },
                    new PointOfInterestAward
                    {
                        Name = "Gladiator_SMK_Urn",
                        Type = "Track_Asset",
                        CheckIns = 50
                    },
                    new PointOfInterestAward
                    {
                        Name = "Gladiator_PROP_Minotaur",
                        Type = "Track_Asset",
                        CheckIns = 100
                    },
                    new PointOfInterestAward
                    {
                        Name = "Gladiator_PROP_Ampitheatre",
                        Type = "Track_Asset",
                        CheckIns = 150
                    },
                ]
            } },
            { 29, new PointOfInterest {
                Name = "Temple of Olympian Zeus",
                CityId = 10,
                Latitude = 37.969372f,
                Longitude = 23.733078f,
                Radius = 0.16f,
                Awards =
                [
                    new PointOfInterestAward
                    {
                        Name = "Gladiator_PROP_Statue01",
                        Type = "Track_Asset",
                        CheckIns = 10
                    },
                    new PointOfInterestAward
                    {
                        Name = "Gladiator_PROP_TempleBase",
                        Type = "Track_Asset",
                        CheckIns = 50
                    },
                    new PointOfInterestAward
                    {
                        Name = "Gladiator_PROP_TempleTop",
                        Type = "Track_Asset",
                        CheckIns = 100
                    },
                    new PointOfInterestAward
                    {
                        Name = "Gladiator_AI_TrojanHorse",
                        Type = "Track_Asset",
                        CheckIns = 150
                    },
                ]
            } },
            { 30, new PointOfInterest {
                Name = "National Archaeological Museum",
                CityId = 10,
                Latitude = 37.989167f,
                Longitude = 23.7325f,
                Radius = 0.14f,
                Awards =
                [
                    new PointOfInterestAward
                    {
                        Name = "MoonColony_PROP_TPP5_MiningVehicle",
                        Type = "Track_Asset",
                        CheckIns = 10
                    },
                    new PointOfInterestAward
                    {
                        Name = "MoonColony_PROP_ConnectingTube",
                        Type = "Track_Asset",
                        CheckIns = 50
                    },
                    new PointOfInterestAward
                    {
                        Name = "MoonColony_PROP_ConnectingTubenoSupports",
                        Type = "Track_Asset",
                        CheckIns = 100
                    },
                    new PointOfInterestAward
                    {
                        Name = "MoonColony_PROP_MiningVent",
                        Type = "Track_Asset",
                        CheckIns = 150
                    },
                ]
            } },
            { 31, new PointOfInterest {
                Name = "Forbidden City",
                CityId = 11,
                Latitude = 39.914722f,
                Longitude = 116.390556f,
                Radius = 1.0f,
                Awards =
                [
                    new PointOfInterestAward
                    {
                        Name = "JunkYard_AI_OilDrum",
                        Type = "Track_Asset",
                        CheckIns = 10
                    },
                    new PointOfInterestAward
                    {
                        Name = "JunkYard_SMK_OxyacetyleneTanks",
                        Type = "Track_Asset",
                        CheckIns = 100
                    },
                    new PointOfInterestAward
                    {
                        Name = "JunkYard_PROP_TPP8_Doberman",
                        Type = "Track_Asset",
                        CheckIns = 250
                    },
                    new PointOfInterestAward
                    {
                        Name = "Construction_AI_Digger",
                        Type = "Track_Asset",
                        CheckIns = 400
                    },
                ]
            } },
            { 32, new PointOfInterest {
                Name = "National Museum of China",
                CityId = 11,
                Latitude = 39.903333f,
                Longitude = 116.39f,
                Radius = 0.7f,
                Awards =
                [
                    new PointOfInterestAward
                    {
                        Name = "Stunt_PROP_InteractiveTrailer",
                        Type = "Track_Asset",
                        CheckIns = 10
                    },
                    new PointOfInterestAward
                    {
                        Name = "Stunt_AI_StuntRamp_04",
                        Type = "Track_Asset",
                        CheckIns = 100
                    },
                    new PointOfInterestAward
                    {
                        Name = "Haunted_SMK_Mummy",
                        Type = "Track_Asset",
                        CheckIns = 250
                    },
                    new PointOfInterestAward
                    {
                        Name = "Stunt_PROP_ChopperStunt01",
                        Type = "Track_Asset",
                        CheckIns = 400
                    },
                ]
            } },
            { 33, new PointOfInterest {
                Name = "Beihai Park",
                CityId = 11,
                Latitude = 39.924444f,
                Longitude = 116.383056f,
                Radius = 0.7f,
                Awards =
                [
                    new PointOfInterestAward
                    {
                        Name = "Glasses_Jazzy",
                        Type = "Character_Asset",
                        CheckIns = 10
                    },
                    new PointOfInterestAward
                    {
                        Name = "Hat_Poker",
                        Type = "Character_Asset",
                        CheckIns = 100
                    },
                    new PointOfInterestAward
                    {
                        Name = "Hat_Harvest",
                        Type = "Character_Asset",
                        CheckIns = 250
                    },
                    new PointOfInterestAward
                    {
                        Name = "Part_Kart_Body_Plane",
                        Type = "Kart_Asset",
                        CheckIns = 400
                    },
                ]
            } },
            { 34, new PointOfInterest {
                Name = "St. Stephen's Cathedral",
                CityId = 12,
                Latitude = 48.20833f,
                Longitude = 16.37278f,
                Radius = 0.1f,
                Awards =
                [
                    new PointOfInterestAward
                    {
                        Name = "Medieval_ARC_Millhouse",
                        Type = "Track_Asset",
                        CheckIns = 10
                    },
                    new PointOfInterestAward
                    {
                        Name = "Medieval_ARC_VillageHut01",
                        Type = "Track_Asset",
                        CheckIns = 50
                    },
                    new PointOfInterestAward
                    {
                        Name = "Medieval_PROP_WaterWell",
                        Type = "Track_Asset",
                        CheckIns = 100
                    },
                    new PointOfInterestAward
                    {
                        Name = "Medieval_ARC_Friary",
                        Type = "Track_Asset",
                        CheckIns = 150
                    },
                ]
            } },
            { 35, new PointOfInterest {
                Name = "Hofburg Palace",
                CityId = 12,
                Latitude = 48.206507f,
                Longitude = 16.365262f,
                Radius = 0.16f,
                Awards =
                [
                    new PointOfInterestAward
                    {
                        Name = "Part_Char_Vampire_Cape",
                        Type = "Character_Asset",
                        CheckIns = 10
                    },
                    new PointOfInterestAward
                    {
                        Name = "Part_Char_Hair_26",
                        Type = "Character_Asset",
                        CheckIns = 50
                    },
                    new PointOfInterestAward
                    {
                        Name = "Part_Char_Shirt_jacket_vampire",
                        Type = "Character_Asset",
                        CheckIns = 75
                    },
                    new PointOfInterestAward
                    {
                        Name = "000004C7_DLCCHAR12",
                        Type = "Character",
                        CheckIns = 100
                    },
                    new PointOfInterestAward
                    {
                        Name = "Part_Char_Ears_17",
                        Type = "Character_Asset",
                        CheckIns = 150
                    },
                ]
            } },
            { 36, new PointOfInterest {
                Name = "Albertina",
                CityId = 12,
                Latitude = 48.204444f,
                Longitude = 16.367778f,
                Radius = 0.14f,
                Awards =
                [
                    new PointOfInterestAward
                    {
                        Name = "00000072_DLCKART12",
                        Type = "Kart",
                        CheckIns = 10
                    },
                    new PointOfInterestAward
                    {
                        Name = "Part_Kart_Seat_Pillow",
                        Type = "Kart_Asset",
                        CheckIns = 50
                    },
                    new PointOfInterestAward
                    {
                        Name = "Part_Kart_Ornament_Kart_Ornament65",
                        Type = "Kart_Asset",
                        CheckIns = 100
                    },
                    new PointOfInterestAward
                    {
                        Name = "Part_Kart_Body_Casket",
                        Type = "Kart_Asset",
                        CheckIns = 150
                    },
                ]
            } },
            { 37, new PointOfInterest {
                Name = "National Museum of Anthropology and History",
                CityId = 13,
                Latitude = 19.426f,
                Longitude = -99.186f,
                Radius = 0.2f,
                Awards =
                [
                    new PointOfInterestAward
                    {
                        Name = "Stunt_AI_StuntRamp_01",
                        Type = "Track_Asset",
                        CheckIns = 10
                    },
                    new PointOfInterestAward
                    {
                        Name = "Part_Kart_Seat_FoldingChair",
                        Type = "Kart_Asset",
                        CheckIns = 100
                    },
                    new PointOfInterestAward
                    {
                        Name = "0000004A_DLCKART4",
                        Type = "Kart",
                        CheckIns = 250
                    },
                    new PointOfInterestAward
                    {
                        Name = "Part_Kart_Body_ClassicMuscle5",
                        Type = "Kart_Asset",
                        CheckIns = 400
                    },
                ]
            } },
            { 38, new PointOfInterest {
                Name = "Mexican National Palace",
                CityId = 13,
                Latitude = 19.4325f,
                Longitude = -99.131111f,
                Radius = 0.23f,
                Awards =
                [
                    new PointOfInterestAward
                    {
                        Name = "000004BF_DLCCHAR4",
                        Type = "Character",
                        CheckIns = 10
                    },
                    new PointOfInterestAward
                    {
                        Name = "Part_Char_Snake",
                        Type = "Character_Asset",
                        CheckIns = 100
                    },
                    new PointOfInterestAward
                    {
                        Name = "Part_Char_ELSERPENTO",
                        Type = "Character_Asset",
                        CheckIns = 200
                    },
                    new PointOfInterestAward
                    {
                        Name = "Part_Char_Pants_wrestler3",
                        Type = "Character_Asset",
                        CheckIns = 300
                    },
                    new PointOfInterestAward
                    {
                        Name = "Part_Char_Shirt_wrestler3",
                        Type = "Character_Asset",
                        CheckIns = 400
                    },
                ]
            } },
            { 39, new PointOfInterest {
                Name = "Angel of Independence",
                CityId = 13,
                Latitude = 19.427f,
                Longitude = -99.16771f,
                Radius = 0.1f,
                Awards =
                [
                    new PointOfInterestAward
                    {
                        Name = "Part_Char_Accessories_22",
                        Type = "Character_Asset",
                        CheckIns = 10
                    },
                    new PointOfInterestAward
                    {
                        Name = "Part_Char_Accessories_26",
                        Type = "Character_Asset",
                        CheckIns = 100
                    },
                    new PointOfInterestAward
                    {
                        Name = "Part_voice_SNAKEO",
                        Type = "Character_Asset",
                        CheckIns = 250
                    },
                    new PointOfInterestAward
                    {
                        Name = "Part_Char_Pattern_Serpent",
                        Type = "Character_Asset",
                        CheckIns = 400
                    },
                ]
            } },
            { 40, new PointOfInterest {
                Name = "Berlin Zoological Garden",
                CityId = 14,
                Latitude = 52.508333f,
                Longitude = 13.3375f,
                Radius = 0.7f,
                Awards =
                [
                    new PointOfInterestAward
                    {
                        Name = "00000099_DRAGONKART",
                        Type = "Kart",
                        CheckIns = 10
                    },
                    new PointOfInterestAward
                    {
                        Name = "Part_Kart_Steering_Shield",
                        Type = "Kart_Asset",
                        CheckIns = 75
                    },
                    new PointOfInterestAward
                    {
                        Name = "Part_Kart_Ornament_Kart_Ornament77",
                        Type = "Kart_Asset",
                        CheckIns = 150
                    },
                    new PointOfInterestAward
                    {
                        Name = "Part_Kart_Body_Dragon",
                        Type = "Kart_Asset",
                        CheckIns = 250
                    },
                ]
            } },
            { 41, new PointOfInterest {
                Name = "Reichstag Building",
                CityId = 14,
                Latitude = 52.5186f,
                Longitude = 13.376f,
                Radius = 0.07f,
                Awards =
                [
                    new PointOfInterestAward
                    {
                        Name = "Medieval_SMK_SuitOfArmour",
                        Type = "Track_Asset",
                        CheckIns = 10
                    },
                    new PointOfInterestAward
                    {
                        Name = "Part_voice_KNIGHT",
                        Type = "Character_Asset",
                        CheckIns = 75
                    },
                    new PointOfInterestAward
                    {
                        Name = "Medieval_SMK_WeaponRack",
                        Type = "Track_Asset",
                        CheckIns = 150
                    },
                    new PointOfInterestAward
                    {
                        Name = "ChainMail",
                        Type = "Character_Asset",
                        CheckIns = 250
                    },
                ]
            } },
            { 42, new PointOfInterest {
                Name = "Brandenburg Gate",
                CityId = 14,
                Latitude = 52.516272f,
                Longitude = 13.377722f,
                Radius = 0.04f,
                Awards =
                [
                    new PointOfInterestAward
                    {
                        Name = "Part_Char_Shirt_Knight",
                        Type = "Character_Asset",
                        CheckIns = 10
                    },
                    new PointOfInterestAward
                    {
                        Name = "Part_Char_Pants_Knight",
                        Type = "Character_Asset",
                        CheckIns = 50
                    },
                    new PointOfInterestAward
                    {
                        Name = "Part_Char_hand_knight",
                        Type = "Character_Asset",
                        CheckIns = 100
                    },
                    new PointOfInterestAward
                    {
                        Name = "Part_Char_Shoe_Knight",
                        Type = "Character_Asset",
                        CheckIns = 150
                    },
                    new PointOfInterestAward
                    {
                        Name = "000004F3_DLCCHAR19",
                        Type = "Character",
                        CheckIns = 200
                    },
                    new PointOfInterestAward
                    {
                        Name = "Hat_Knight",
                        Type = "Character_Asset",
                        CheckIns = 250
                    },
                ]
            } },
            { 43, new PointOfInterest {
                Name = "CN Tower",
                CityId = 15,
                Latitude = 43.6426f,
                Longitude = -79.3871f,
                Radius = 0.27f,
                Awards =
                [
                    new PointOfInterestAward
                    {
                        Name = "000004C2_DLCCHAR7",
                        Type = "Character",
                        CheckIns = 10
                    },
                    new PointOfInterestAward
                    {
                        Name = "Part_Char_Pattern_biffpow",
                        Type = "Character_Asset",
                        CheckIns = 75
                    },
                    new PointOfInterestAward
                    {
                        Name = "Part_Char_Accessories_15",
                        Type = "Character_Asset",
                        CheckIns = 150
                    },
                    new PointOfInterestAward
                    {
                        Name = "Part_Char_Glasses_6",
                        Type = "Character_Asset",
                        CheckIns = 250
                    },
                ]
            } },
            { 44, new PointOfInterest {
                Name = "Royal Ontario Museum",
                CityId = 15,
                Latitude = 43.667476f,
                Longitude = -79.39417f,
                Radius = 0.1f,
                Awards =
                [
                    new PointOfInterestAward
                    {
                        Name = "Part_Char_Hat_Flaps",
                        Type = "Character_Asset",
                        CheckIns = 10
                    },
                    new PointOfInterestAward
                    {
                        Name = "Part_Char_Hockey_Helmet",
                        Type = "Character_Asset",
                        CheckIns = 75
                    },
                    new PointOfInterestAward
                    {
                        Name = "Hat_Beaver",
                        Type = "Character_Asset",
                        CheckIns = 150
                    },
                    new PointOfInterestAward
                    {
                        Name = "Part_Kart_Ornament_Kart_Ornament72",
                        Type = "Kart_Asset",
                        CheckIns = 250
                    },
                ]
            } },
            { 45, new PointOfInterest {
                Name = "Ontario Science Centre",
                CityId = 15,
                Latitude = 43.716667f,
                Longitude = -79.338333f,
                Radius = 0.1f,
                Awards =
                [
                    new PointOfInterestAward
                    {
                        Name = "Part_Kart_Ornament_Kart_Ornament58",
                        Type = "Kart_Asset",
                        CheckIns = 10
                    },
                    new PointOfInterestAward
                    {
                        Name = "Part_Kart_Ornament_Kart_Ornament59",
                        Type = "Kart_Asset",
                        CheckIns = 50
                    },
                    new PointOfInterestAward
                    {
                        Name = "Part_Kart_Wheel_Recycle",
                        Type = "Kart_Asset",
                        CheckIns = 100
                    },
                    new PointOfInterestAward
                    {
                        Name = "0000006D_DLCKART7",
                        Type = "Kart",
                        CheckIns = 175
                    },
                    new PointOfInterestAward
                    {
                        Name = "Part_Kart_Body_Paper",
                        Type = "Kart_Asset",
                        CheckIns = 250
                    },
                ]
            } },
            { 46, new PointOfInterest {
                Name = "Kremlin",
                CityId = 16,
                Latitude = 55.751667f,
                Longitude = 37.617778f,
                Radius = 0.4f,
                Awards =
                [
                    new PointOfInterestAward
                    {
                        Name = "0000009A_SUBMARINEKART",
                        Type = "Kart",
                        CheckIns = 10
                    },
                    new PointOfInterestAward
                    {
                        Name = "Part_Kart_Steering_Hatch",
                        Type = "Kart_Asset",
                        CheckIns = 100
                    },
                    new PointOfInterestAward
                    {
                        Name = "Part_Kart_Seat_Subway",
                        Type = "Kart_Asset",
                        CheckIns = 250
                    },
                    new PointOfInterestAward
                    {
                        Name = "Part_Kart_Body_MiniSub",
                        Type = "Kart_Asset",
                        CheckIns = 400
                    },
                ]
            } },
            { 47, new PointOfInterest {
                Name = "Gorky Park",
                CityId = 16,
                Latitude = 55.731389f,
                Longitude = 37.603889f,
                Radius = 0.6f,
                Awards =
                [
                    new PointOfInterestAward
                    {
                        Name = "Part_Kart_Wheel_Subway",
                        Type = "Kart_Asset",
                        CheckIns = 10
                    },
                    new PointOfInterestAward
                    {
                        Name = "Pants_Wetsuit",
                        Type = "Character_Asset",
                        CheckIns = 100
                    },
                    new PointOfInterestAward
                    {
                        Name = "000004F4_DLCCHAR20",
                        Type = "Character",
                        CheckIns = 250
                    },
                    new PointOfInterestAward
                    {
                        Name = "Mask_Octopus",
                        Type = "Character_Asset",
                        CheckIns = 400
                    },
                ]
            } },
            { 48, new PointOfInterest {
                Name = "Monument to the Conquerors of Space",
                CityId = 16,
                Latitude = 55.822778f,
                Longitude = 37.64f,
                Radius = 0.25f,
                Awards =
                [
                    new PointOfInterestAward
                    {
                        Name = "Helmet_Cosmo",
                        Type = "Character_Asset",
                        CheckIns = 10
                    },
                    new PointOfInterestAward
                    {
                        Name = "Alien_PROP_FenceWire01",
                        Type = "Track_Asset",
                        CheckIns = 100
                    },
                    new PointOfInterestAward
                    {
                        Name = "Alien_PROP_FenceWire01End",
                        Type = "Track_Asset",
                        CheckIns = 250
                    },
                    new PointOfInterestAward
                    {
                        Name = "Alien_PROP_UFO",
                        Type = "Track_Asset",
                        CheckIns = 400
                    },
                ]
            } },
            { 49, new PointOfInterest {
                Name = "Tokyo Tower",
                CityId = 17,
                Latitude = 35.658611f,
                Longitude = 139.745556f,
                Radius = 0.07f,
                Awards =
                [
                    new PointOfInterestAward
                    {
                        Name = "000004F6_CHELSEACHAR",
                        Type = "Character",
                        CheckIns = 5
                    },
                    new PointOfInterestAward
                    {
                        Name = "Hair_Chelsea",
                        Type = "Character_Asset",
                        CheckIns = 10
                    },
                    new PointOfInterestAward
                    {
                        Name = "Part_Char_Helmet_Baseball",
                        Type = "Character_Asset",
                        CheckIns = 25
                    },
                    new PointOfInterestAward
                    {
                        Name = "Part_Kart_Body_Subway",
                        Type = "Kart_Asset",
                        CheckIns = 50
                    },
                ]
            } },
            { 50, new PointOfInterest {
                Name = "Imperial Palace",
                CityId = 17,
                Latitude = 35.684753f,
                Longitude = 139.752458f,
                Radius = 0.4f,
                Awards =
                [
                    new PointOfInterestAward
                    {
                        Name = "Part_Kart_Ornament_Kart_Ornament66",
                        Type = "Kart_Asset",
                        CheckIns = 5
                    },
                    new PointOfInterestAward
                    {
                        Name = "Glasses_Superstar",
                        Type = "Character_Asset",
                        CheckIns = 10
                    },
                    new PointOfInterestAward
                    {
                        Name = "Pants_Jeansacid",
                        Type = "Character_Asset",
                        CheckIns = 25
                    },
                    new PointOfInterestAward
                    {
                        Name = "Part_Kart_Body_Tuner5",
                        Type = "Kart_Asset",
                        CheckIns = 50
                    },
                ]
            } },
            { 51, new PointOfInterest {
                Name = "Meiji Shrine",
                CityId = 17,
                Latitude = 35.676111f,
                Longitude = 139.699167f,
                Radius = 0.4f,
                Awards =
                [
                    new PointOfInterestAward
                    {
                        Name = "000004F2_DLCCHAR18",
                        Type = "Character",
                        CheckIns = 5
                    },
                    new PointOfInterestAward
                    {
                        Name = "Hair_Fairy",
                        Type = "Character_Asset",
                        CheckIns = 10
                    },
                    new PointOfInterestAward
                    {
                        Name = "Part_Char_Shirt_Dressfairy",
                        Type = "Character_Asset",
                        CheckIns = 25
                    },
                    new PointOfInterestAward
                    {
                        Name = "FairyWings",
                        Type = "Character_Asset",
                        CheckIns = 50
                    },
                ]
            } },
            { 52, new PointOfInterest {
                Name = "Washington Monument",
                CityId = 18,
                Latitude = 38.889468f,
                Longitude = -77.03524f,
                Radius = 0.24f,
                Awards =
                [
                    new PointOfInterestAward
                    {
                        Name = "000004F1_DLCCHAR_HOLIDAYEASTER",
                        Type = "Character",
                        CheckIns = 10
                    },
                    new PointOfInterestAward
                    {
                        Name = "EasterBunny",
                        Type = "Character_Asset",
                        CheckIns = 75
                    },
                    new PointOfInterestAward
                    {
                        Name = "Pants_Foil",
                        Type = "Character_Asset",
                        CheckIns = 150
                    },
                    new PointOfInterestAward
                    {
                        Name = "Hat_Bunny",
                        Type = "Character_Asset",
                        CheckIns = 250
                    },
                ]
            } },
            { 53, new PointOfInterest {
                Name = "National Air and Space Museum",
                CityId = 18,
                Latitude = 38.888333f,
                Longitude = -77.02f,
                Radius = 0.15f,
                Awards =
                [
                    new PointOfInterestAward
                    {
                        Name = "Alien_ARCTSP03_AircraftHangar",
                        Type = "Track_Asset",
                        CheckIns = 10
                    },
                    new PointOfInterestAward
                    {
                        Name = "Alien_ARCTSP03_MilitaryDome",
                        Type = "Track_Asset",
                        CheckIns = 75
                    },
                    new PointOfInterestAward
                    {
                        Name = "Alien_ARC_BigDishComsTower",
                        Type = "Track_Asset",
                        CheckIns = 150
                    },
                    new PointOfInterestAward
                    {
                        Name = "Alien_PROP_UFODriveThruSaucer",
                        Type = "Track_Asset",
                        CheckIns = 250
                    },
                ]
            } },
            { 54, new PointOfInterest {
                Name = "White House",
                CityId = 18,
                Latitude = 38.89767f,
                Longitude = -77.03655f,
                Radius = 0.2f,
                Awards =
                [
                    new PointOfInterestAward
                    {
                        Name = "Part_Kart_Engine_Easter",
                        Type = "Kart_Asset",
                        CheckIns = 10
                    },
                    new PointOfInterestAward
                    {
                        Name = "Part_Kart_Wheel_Carrot",
                        Type = "Kart_Asset",
                        CheckIns = 50
                    },
                    new PointOfInterestAward
                    {
                        Name = "Part_Kart_Steering_Egg",
                        Type = "Kart_Asset",
                        CheckIns = 100
                    },
                    new PointOfInterestAward
                    {
                        Name = "00000097_EASTERKART",
                        Type = "Kart",
                        CheckIns = 150
                    },
                    new PointOfInterestAward
                    {
                        Name = "Part_Kart_Body_Carrot",
                        Type = "Kart_Asset",
                        CheckIns = 250
                    },
                ]
            } },
            { 55, new PointOfInterest {
                Name = "Grauman's Chinese Theatre",
                CityId = 19,
                Latitude = 34.101944f,
                Longitude = -118.340972f,
                Radius = 0.1f,
                Awards =
                [
                    new PointOfInterestAward
                    {
                        Name = "Western_PROP_MovieTrailer",
                        Type = "Track_Asset",
                        CheckIns = 10
                    },
                    new PointOfInterestAward
                    {
                        Name = "Western_PROP_Saloon",
                        Type = "Track_Asset",
                        CheckIns = 100
                    },
                    new PointOfInterestAward
                    {
                        Name = "Western_PROP_TrainStation",
                        Type = "Track_Asset",
                        CheckIns = 250
                    },
                    new PointOfInterestAward
                    {
                        Name = "Western_PROP_ClockTower",
                        Type = "Track_Asset",
                        CheckIns = 500
                    },
                ]
            } },
            { 56, new PointOfInterest {
                Name = "Sunset Strip",
                CityId = 19,
                Latitude = 34.0907f,
                Longitude = -118.386f,
                Radius = 0.5f,
                Awards =
                [
                    new PointOfInterestAward
                    {
                        Name = "Western_PROP_Bank",
                        Type = "Track_Asset",
                        CheckIns = 10
                    },
                    new PointOfInterestAward
                    {
                        Name = "Western_PROP_Blacksmiths",
                        Type = "Track_Asset",
                        CheckIns = 100
                    },
                    new PointOfInterestAward
                    {
                        Name = "Western_PROP_Jail",
                        Type = "Track_Asset",
                        CheckIns = 250
                    },
                    new PointOfInterestAward
                    {
                        Name = "Western_PROP_GeneralStore",
                        Type = "Track_Asset",
                        CheckIns = 500
                    },
                ]
            } },
            { 57, new PointOfInterest {
                Name = "Santa Monica Pier",
                CityId = 19,
                Latitude = 34.008611f,
                Longitude = -118.498611f,
                Radius = 0.3f,
                Awards =
                [
                    new PointOfInterestAward
                    {
                        Name = "Western_PROP_BackDrop01B",
                        Type = "Track_Asset",
                        CheckIns = 10
                    },
                    new PointOfInterestAward
                    {
                        Name = "Western_PROP_BackDrop02",
                        Type = "Track_Asset",
                        CheckIns = 100
                    },
                    new PointOfInterestAward
                    {
                        Name = "Western_PROP_BackDrop03",
                        Type = "Track_Asset",
                        CheckIns = 250
                    },
                    new PointOfInterestAward
                    {
                        Name = "Western_PROP_BackDrop01A",
                        Type = "Track_Asset",
                        CheckIns = 500
                    },
                ]
            } },
            { 58, new PointOfInterest {
                Name = "Atomic Testing Museum",
                CityId = 20,
                Latitude = 36.11416f,
                Longitude = -115.1486f,
                Radius = 0.12f,
                Awards =
                [
                    new PointOfInterestAward
                    {
                        Name = "Part_Kart_Steering_Suicide_Knob",
                        Type = "Kart_Asset",
                        CheckIns = 10
                    },
                    new PointOfInterestAward
                    {
                        Name = "Part_Char_curly",
                        Type = "Character_Asset",
                        CheckIns = 25
                    },
                    new PointOfInterestAward
                    {
                        Name = "000004BD_DLCCHAR2",
                        Type = "Character",
                        CheckIns = 50
                    },
                    new PointOfInterestAward
                    {
                        Name = "Part_Char_Accessories_19",
                        Type = "Character_Asset",
                        CheckIns = 100
                    },
                ]
            } },
            { 59, new PointOfInterest {
                Name = "Welcome to Las Vegas Sign",
                CityId = 20,
                Latitude = 36.082056f,
                Longitude = -115.172778f,
                Radius = 0.17f,
                Awards =
                [
                    new PointOfInterestAward
                    {
                        Name = "Alien_SMK_AlienSign_01",
                        Type = "Track_Asset",
                        CheckIns = 10
                    },
                    new PointOfInterestAward
                    {
                        Name = "Western_SMK_Tumbleweed",
                        Type = "Track_Asset",
                        CheckIns = 25
                    },
                    new PointOfInterestAward
                    {
                        Name = "Alien_SMK_AlienSign_02",
                        Type = "Track_Asset",
                        CheckIns = 50
                    },
                    new PointOfInterestAward
                    {
                        Name = "Western_PROP_TownSignArch",
                        Type = "Track_Asset",
                        CheckIns = 100
                    },
                ]
            } },
            { 60, new PointOfInterest {
                Name = "Fremont Street Experience",
                CityId = 20,
                Latitude = 36.170833f,
                Longitude = -115.144167f,
                Radius = 0.3f,
                Awards =
                [
                    new PointOfInterestAward
                    {
                        Name = "00000047_DLCKART2",
                        Type = "Kart",
                        CheckIns = 10
                    },
                    new PointOfInterestAward
                    {
                        Name = "Part_Kart_Steering_Valve",
                        Type = "Kart_Asset",
                        CheckIns = 25
                    },
                    new PointOfInterestAward
                    {
                        Name = "Part_Kart_Seat_OilDrum",
                        Type = "Kart_Asset",
                        CheckIns = 50
                    },
                    new PointOfInterestAward
                    {
                        Name = "Part_Kart_Body_OffRoad6",
                        Type = "Kart_Asset",
                        CheckIns = 100
                    },
                ]
            } },
            { 61, new PointOfInterest {
                Name = "'Iolani Palace",
                CityId = 21,
                Latitude = 21.306622f,
                Longitude = -157.858958f,
                Radius = 0.1f,
                Awards =
                [
                    new PointOfInterestAward
                    {
                        Name = "Pirate_PROP_TreasureCave",
                        Type = "Track_Asset",
                        CheckIns = 5
                    },
                    new PointOfInterestAward
                    {
                        Name = "Pirate_PROP_ManCage",
                        Type = "Track_Asset",
                        CheckIns = 25
                    },
                    new PointOfInterestAward
                    {
                        Name = "Part_Kart_Seat_ElectricChair",
                        Type = "Kart_Asset",
                        CheckIns = 50
                    },
                    new PointOfInterestAward
                    {
                        Name = "Pirate_SMK_TreasureChest",
                        Type = "Track_Asset",
                        CheckIns = 75
                    },
                ]
            } },
            { 62, new PointOfInterest {
                Name = "Aloha Tower",
                CityId = 21,
                Latitude = 21.306944f,
                Longitude = -157.865833f,
                Radius = 0.1f,
                Awards =
                [
                    new PointOfInterestAward
                    {
                        Name = "Progressive_PROP_Balloon02",
                        Type = "Track_Asset",
                        CheckIns = 5
                    },
                    new PointOfInterestAward
                    {
                        Name = "Progressive_PROP_HotairBalloon02",
                        Type = "Track_Asset",
                        CheckIns = 25
                    },
                    new PointOfInterestAward
                    {
                        Name = "Progressive_PROP_TPP11_Blimp2",
                        Type = "Track_Asset",
                        CheckIns = 50
                    },
                    new PointOfInterestAward
                    {
                        Name = "Progressive_PROP_InflatOverpass",
                        Type = "Track_Asset",
                        CheckIns = 75
                    },
                ]
            } },
            { 63, new PointOfInterest {
                Name = "Waikiki Aquarium",
                CityId = 21,
                Latitude = 21.2659f,
                Longitude = -157.822f,
                Radius = 0.1f,
                Awards =
                [
                    new PointOfInterestAward
                    {
                        Name = "Pirate_PROP_Dodo",
                        Type = "Track_Asset",
                        CheckIns = 5
                    },
                    new PointOfInterestAward
                    {
                        Name = "Pirate_PROP_Plant01_A",
                        Type = "Track_Asset",
                        CheckIns = 25
                    },
                    new PointOfInterestAward
                    {
                        Name = "Pirate_PROP_Plant02_B",
                        Type = "Track_Asset",
                        CheckIns = 50
                    },
                    new PointOfInterestAward
                    {
                        Name = "Part_Kart_CowSeat",
                        Type = "Kart_Asset",
                        CheckIns = 75
                    },
                ]
            } },
            { 64, new PointOfInterest {
                Name = "Seoul Museum of Art",
                CityId = 22,
                Latitude = 37.564122f,
                Longitude = 126.973808f,
                Radius = 0.1f,
                Awards =
                [
                    new PointOfInterestAward
                    {
                        Name = "Part_Kart_Steering_Futuristic",
                        Type = "Kart_Asset",
                        CheckIns = 10
                    },
                    new PointOfInterestAward
                    {
                        Name = "Part_Kart_Seat_Sandbags",
                        Type = "Kart_Asset",
                        CheckIns = 100
                    },
                    new PointOfInterestAward
                    {
                        Name = "00000070_DLCKART10",
                        Type = "Kart",
                        CheckIns = 250
                    },
                    new PointOfInterestAward
                    {
                        Name = "Part_Kart_Body_Concept5",
                        Type = "Kart_Asset",
                        CheckIns = 400
                    },
                ]
            } },
            { 65, new PointOfInterest {
                Name = "Wongudan",
                CityId = 22,
                Latitude = 37.565053f,
                Longitude = 126.97985f,
                Radius = 0.1f,
                Awards =
                [
                    new PointOfInterestAward
                    {
                        Name = "Part_Char_Belt_Superhero",
                        Type = "Character_Asset",
                        CheckIns = 10
                    },
                    new PointOfInterestAward
                    {
                        Name = "Part_voice_SUPERMOD",
                        Type = "Character_Asset",
                        CheckIns = 100
                    },
                    new PointOfInterestAward
                    {
                        Name = "Part_Char_Shoe_SuperMod",
                        Type = "Character_Asset",
                        CheckIns = 250
                    },
                    new PointOfInterestAward
                    {
                        Name = "Part_Char_hand_supermod",
                        Type = "Character_Asset",
                        CheckIns = 400
                    },
                ]
            } },
            { 66, new PointOfInterest {
                Name = "N Seoul Tower",
                CityId = 22,
                Latitude = 37.551425f,
                Longitude = 126.988f,
                Radius = 0.3f,
                Awards =
                [
                    new PointOfInterestAward
                    {
                        Name = "000004C5_DLCCHAR10",
                        Type = "Character",
                        CheckIns = 10
                    },
                    new PointOfInterestAward
                    {
                        Name = "Shirt_SuperMod",
                        Type = "Character_Asset",
                        CheckIns = 100
                    },
                    new PointOfInterestAward
                    {
                        Name = "Pants_SuperMod",
                        Type = "Character_Asset",
                        CheckIns = 250
                    },
                    new PointOfInterestAward
                    {
                        Name = "Part_Char_Mask_Justice",
                        Type = "Character_Asset",
                        CheckIns = 400
                    },
                ]
            } },
            { 67, new PointOfInterest {
                Name = "Iwatayama Monkey Park",
                CityId = 23,
                Latitude = 35.008938f,
                Longitude = 135.674681f,
                Radius = 0.45f,
                Awards =
                [
                    new PointOfInterestAward
                    {
                        Name = "Alien_SMK_AlienPod",
                        Type = "Track_Asset",
                        CheckIns = 5
                    },
                    new PointOfInterestAward
                    {
                        Name = "Alien_SMK_AlienSlimeBarrel",
                        Type = "Track_Asset",
                        CheckIns = 10
                    },
                    new PointOfInterestAward
                    {
                        Name = "Alien_PROP_SlimeCrate",
                        Type = "Track_Asset",
                        CheckIns = 25
                    },
                    new PointOfInterestAward
                    {
                        Name = "Alien_PROP_AlienNest",
                        Type = "Track_Asset",
                        CheckIns = 50
                    },
                ]
            } },
            { 68, new PointOfInterest {
                Name = "Kamigamo Shrine",
                CityId = 23,
                Latitude = 35.060278f,
                Longitude = 135.752778f,
                Radius = 0.2f,
                Awards =
                [
                    new PointOfInterestAward
                    {
                        Name = "Hair_Katsura",
                        Type = "Character_Asset",
                        CheckIns = 5
                    },
                    new PointOfInterestAward
                    {
                        Name = "Shoe_Kimono",
                        Type = "Character_Asset",
                        CheckIns = 10
                    },
                    new PointOfInterestAward
                    {
                        Name = "Pants_Kimono",
                        Type = "Character_Asset",
                        CheckIns = 25
                    },
                    new PointOfInterestAward
                    {
                        Name = "Shirt_Kimono",
                        Type = "Character_Asset",
                        CheckIns = 50
                    },
                ]
            } },
            { 69, new PointOfInterest {
                Name = "To-ji",
                CityId = 23,
                Latitude = 34.980556f,
                Longitude = 135.747778f,
                Radius = 0.1f,
                Awards =
                [
                    new PointOfInterestAward
                    {
                        Name = "Helmet_Samurai",
                        Type = "Character_Asset",
                        CheckIns = 5
                    },
                    new PointOfInterestAward
                    {
                        Name = "Hair_Warrior",
                        Type = "Character_Asset",
                        CheckIns = 10
                    },
                    new PointOfInterestAward
                    {
                        Name = "Part_Kart_Ornament_Kart_Ornament68",
                        Type = "Kart_Asset",
                        CheckIns = 25
                    },
                    new PointOfInterestAward
                    {
                        Name = "Mask_Samurai",
                        Type = "Character_Asset",
                        CheckIns = 50
                    },
                ]
            } },
            { 70, new PointOfInterest {
                Name = "Vancouver Aquarium",
                CityId = 24,
                Latitude = 49.300586f,
                Longitude = -123.131053f,
                Radius = 0.13f,
                Awards =
                [
                    new PointOfInterestAward
                    {
                        Name = "Pant_Thermal",
                        Type = "Character_Asset",
                        CheckIns = 10
                    },
                    new PointOfInterestAward
                    {
                        Name = "Part_Kart_Wheel_Studded",
                        Type = "Kart_Asset",
                        CheckIns = 50
                    },
                    new PointOfInterestAward
                    {
                        Name = "Part_Kart_Body_Zamboni",
                        Type = "Kart_Asset",
                        CheckIns = 100
                    },
                    new PointOfInterestAward
                    {
                        Name = "Part_Kart_Engine_VendingMachine",
                        Type = "Kart_Asset",
                        CheckIns = 150
                    },
                ]
            } },
            { 71, new PointOfInterest {
                Name = "H. R. MacMillan Space Centre",
                CityId = 24,
                Latitude = 49.276205f,
                Longitude = -123.1444f,
                Radius = 0.15f,
                Awards =
                [
                    new PointOfInterestAward
                    {
                        Name = "Glasses_Slopes",
                        Type = "Character_Asset",
                        CheckIns = 10
                    },
                    new PointOfInterestAward
                    {
                        Name = "Part_Kart_Seat_Cooler",
                        Type = "Kart_Asset",
                        CheckIns = 50
                    },
                    new PointOfInterestAward
                    {
                        Name = "Pants_Baggy",
                        Type = "Character_Asset",
                        CheckIns = 100
                    },
                    new PointOfInterestAward
                    {
                        Name = "Part_Kart_Spoiler_21",
                        Type = "Kart_Asset",
                        CheckIns = 150
                    },
                ]
            } },
            { 72, new PointOfInterest {
                Name = "Science World",
                CityId = 24,
                Latitude = 49.273251f,
                Longitude = -123.103767f,
                Radius = 0.1f,
                Awards =
                [
                    new PointOfInterestAward
                    {
                        Name = "Part_Kart_Spoiler_24",
                        Type = "Kart_Asset",
                        CheckIns = 10
                    },
                    new PointOfInterestAward
                    {
                        Name = "Part_Char_Shirt_Longjohn",
                        Type = "Character_Asset",
                        CheckIns = 50
                    },
                    new PointOfInterestAward
                    {
                        Name = "Pants_Longjohn",
                        Type = "Character_Asset",
                        CheckIns = 100
                    },
                    new PointOfInterestAward
                    {
                        Name = "Glasses_Snow",
                        Type = "Character_Asset",
                        CheckIns = 150
                    },
                ]
            } },
            { 73, new PointOfInterest {
                Name = "Golden Gate Bridge",
                CityId = 25,
                Latitude = 37.819722f,
                Longitude = -122.478611f,
                Radius = 1.0f,
                Awards =
                [
                    new PointOfInterestAward
                    {
                        Name = "00000049_DLCKART3",
                        Type = "Kart",
                        CheckIns = 10
                    },
                    new PointOfInterestAward
                    {
                        Name = "Part_Kart_Seat_Beanbag",
                        Type = "Kart_Asset",
                        CheckIns = 50
                    },
                    new PointOfInterestAward
                    {
                        Name = "Decal_Graphic_Graffiti10",
                        Type = "Kart_Asset",
                        CheckIns = 100
                    },
                    new PointOfInterestAward
                    {
                        Name = "Part_Kart_Body_Van",
                        Type = "Kart_Asset",
                        CheckIns = 200
                    },
                ]
            } },
            { 74, new PointOfInterest {
                Name = "Chinatown",
                CityId = 25,
                Latitude = 37.794722f,
                Longitude = -122.407222f,
                Radius = 0.8f,
                Awards =
                [
                    new PointOfInterestAward
                    {
                        Name = "Alien_SMK_AlienRobot01",
                        Type = "Track_Asset",
                        CheckIns = 10
                    },
                    new PointOfInterestAward
                    {
                        Name = "Alien_ARCTSP03_GasStation",
                        Type = "Track_Asset",
                        CheckIns = 50
                    },
                    new PointOfInterestAward
                    {
                        Name = "Alien_ARCTSP03_Diner",
                        Type = "Track_Asset",
                        CheckIns = 100
                    },
                    new PointOfInterestAward
                    {
                        Name = "Alien_PROP_UFOwithLaser",
                        Type = "Track_Asset",
                        CheckIns = 200
                    },
                ]
            } },
            { 75, new PointOfInterest {
                Name = "Fisherman's Wharf",
                CityId = 25,
                Latitude = 37.808333f,
                Longitude = -122.415556f,
                Radius = 0.4f,
                Awards =
                [
                    new PointOfInterestAward
                    {
                        Name = "Part_Char_Shirt_Leisuresuit",
                        Type = "Character_Asset",
                        CheckIns = 10
                    },
                    new PointOfInterestAward
                    {
                        Name = "Part_Char_Pants_Leisure",
                        Type = "Character_Asset",
                        CheckIns = 50
                    },
                    new PointOfInterestAward
                    {
                        Name = "Part_Char_AfroPick",
                        Type = "Character_Asset",
                        CheckIns = 100
                    },
                    new PointOfInterestAward
                    {
                        Name = "000004BE_DLCCHAR3",
                        Type = "Character",
                        CheckIns = 150
                    },
                    new PointOfInterestAward
                    {
                        Name = "Part_Char_Hair_Vanfro",
                        Type = "Character_Asset",
                        CheckIns = 200
                    },
                ]
            } },
            { 76, new PointOfInterest {
                Name = "Apartheid Museum",
                CityId = 26,
                Latitude = -26.2376f,
                Longitude = 28.009f,
                Radius = 0.2f,
                Awards =
                [
                    new PointOfInterestAward
                    {
                        Name = "Prehistoric_PROP_StandingStone_1",
                        Type = "Track_Asset",
                        CheckIns = 10
                    },
                    new PointOfInterestAward
                    {
                        Name = "Prehistoric_PROP_LeatherHut01",
                        Type = "Track_Asset",
                        CheckIns = 75
                    },
                    new PointOfInterestAward
                    {
                        Name = "Prehistoric_PROP_StandingStone_2",
                        Type = "Track_Asset",
                        CheckIns = 150
                    },
                    new PointOfInterestAward
                    {
                        Name = "Prehistoric_PROP_StoneArch",
                        Type = "Track_Asset",
                        CheckIns = 250
                    },
                ]
            } },
            { 77, new PointOfInterest {
                Name = "Nelson Mandela National Museum",
                CityId = 26,
                Latitude = -26.238536f,
                Longitude = 27.908772f,
                Radius = 0.1f,
                Awards =
                [
                    new PointOfInterestAward
                    {
                        Name = "Progressive_PROP_TPP11_AnimatedBillboard",
                        Type = "Track_Asset",
                        CheckIns = 10
                    },
                    new PointOfInterestAward
                    {
                        Name = "Progressive_SMK_TPP11_Box2",
                        Type = "Track_Asset",
                        CheckIns = 75
                    },
                    new PointOfInterestAward
                    {
                        Name = "Progressive_PROP_SemiTrailer",
                        Type = "Track_Asset",
                        CheckIns = 150
                    },
                    new PointOfInterestAward
                    {
                        Name = "Progressive_AI_ProgressiveRamp02",
                        Type = "Track_Asset",
                        CheckIns = 250
                    },
                ]
            } },
            { 78, new PointOfInterest {
                Name = "Johannesburg Zoo",
                CityId = 26,
                Latitude = -26.166375f,
                Longitude = 28.038186f,
                Radius = 0.5f,
                Awards =
                [
                    new PointOfInterestAward
                    {
                        Name = "Prehistoric_PROP_Pterodactyl",
                        Type = "Track_Asset",
                        CheckIns = 10
                    },
                    new PointOfInterestAward
                    {
                        Name = "Prehistoric_SMK_DinoEgg",
                        Type = "Track_Asset",
                        CheckIns = 75
                    },
                    new PointOfInterestAward
                    {
                        Name = "Prehistoric_PROP_StoneHut",
                        Type = "Track_Asset",
                        CheckIns = 150
                    },
                    new PointOfInterestAward
                    {
                        Name = "Prehistoric_PROP_Dino",
                        Type = "Track_Asset",
                        CheckIns = 250
                    },
                ]
            } },
            { 79, new PointOfInterest {
                Name = "Sao Paulo Museum of Art",
                CityId = 27,
                Latitude = -23.561111f,
                Longitude = -46.655833f,
                Radius = 0.1f,
                Awards =
                [
                    new PointOfInterestAward
                    {
                        Name = "Pants_Baggytrack",
                        Type = "Character_Asset",
                        CheckIns = 10
                    },
                    new PointOfInterestAward
                    {
                        Name = "Part_voice_MCMOD",
                        Type = "Character_Asset",
                        CheckIns = 100
                    },
                    new PointOfInterestAward
                    {
                        Name = "Haunted_SMK_Zombie",
                        Type = "Track_Asset",
                        CheckIns = 250
                    },
                    new PointOfInterestAward
                    {
                        Name = "Haunted_PROP_StoneTomb",
                        Type = "Track_Asset",
                        CheckIns = 500
                    },
                ]
            } },
            { 80, new PointOfInterest {
                Name = "Banespa Building",
                CityId = 27,
                Latitude = -23.545833f,
                Longitude = -46.633889f,
                Radius = 0.1f,
                Awards =
                [
                    new PointOfInterestAward
                    {
                        Name = "Part_Kart_Wheel_BlingBling",
                        Type = "Kart_Asset",
                        CheckIns = 10
                    },
                    new PointOfInterestAward
                    {
                        Name = "Part_Kart_Ornament_Kart_Ornament78",
                        Type = "Kart_Asset",
                        CheckIns = 100
                    },
                    new PointOfInterestAward
                    {
                        Name = "00000076_DLCKART16",
                        Type = "Kart",
                        CheckIns = 250
                    },
                    new PointOfInterestAward
                    {
                        Name = "Part_Kart_Body_Suv",
                        Type = "Kart_Asset",
                        CheckIns = 500
                    },
                ]
            } },
            { 81, new PointOfInterest {
                Name = "Museu Paulista",
                CityId = 27,
                Latitude = -23.561389f,
                Longitude = -46.656111f,
                Radius = 0.1f,
                Awards =
                [
                    new PointOfInterestAward
                    {
                        Name = "000004CB_DLCCHAR16",
                        Type = "Character",
                        CheckIns = 10
                    },
                    new PointOfInterestAward
                    {
                        Name = "Part_Char_Hat_Fitted",
                        Type = "Character_Asset",
                        CheckIns = 100
                    },
                    new PointOfInterestAward
                    {
                        Name = "BigWatch",
                        Type = "Character_Asset",
                        CheckIns = 250
                    },
                    new PointOfInterestAward
                    {
                        Name = "Part_Char_Shirt_Football",
                        Type = "Character_Asset",
                        CheckIns = 500
                    },
                ]
            } },
            { 82, new PointOfInterest {
                Name = "Recoleta Cemetary",
                CityId = 28,
                Latitude = -34.588056f,
                Longitude = -58.393056f,
                Radius = 0.15f,
                Awards =
                [
                    new PointOfInterestAward
                    {
                        Name = "Haunted_SMK_Tombstone01",
                        Type = "Track_Asset",
                        CheckIns = 10
                    },
                    new PointOfInterestAward
                    {
                        Name = "Haunted_SMK_Tombstone02",
                        Type = "Track_Asset",
                        CheckIns = 100
                    },
                    new PointOfInterestAward
                    {
                        Name = "Haunted_SMK_Tombstone03",
                        Type = "Track_Asset",
                        CheckIns = 250
                    },
                    new PointOfInterestAward
                    {
                        Name = "Haunted_PROP_FloatingGhost",
                        Type = "Track_Asset",
                        CheckIns = 400
                    },
                ]
            } },
            { 83, new PointOfInterest {
                Name = "Buenos Aires Metropolitan Cathedral",
                CityId = 28,
                Latitude = -34.607408f,
                Longitude = -58.373277f,
                Radius = 0.1f,
                Awards =
                [
                    new PointOfInterestAward
                    {
                        Name = "Haunted_PROP_FlamingTorches",
                        Type = "Track_Asset",
                        CheckIns = 10
                    },
                    new PointOfInterestAward
                    {
                        Name = "Haunted_PROP_CemeteryFence",
                        Type = "Track_Asset",
                        CheckIns = 100
                    },
                    new PointOfInterestAward
                    {
                        Name = "Haunted_PROP_CemeteryFenceEnd",
                        Type = "Track_Asset",
                        CheckIns = 250
                    },
                    new PointOfInterestAward
                    {
                        Name = "Haunted_PROP_CemeteryGates",
                        Type = "Track_Asset",
                        CheckIns = 400
                    },
                ]
            } },
            { 84, new PointOfInterest {
                Name = "Obelisk of Buenos Aires",
                CityId = 28,
                Latitude = -34.603611f,
                Longitude = -58.381667f,
                Radius = 0.1f,
                Awards =
                [
                    new PointOfInterestAward
                    {
                        Name = "Haunted_PROP_SpookyTree",
                        Type = "Track_Asset",
                        CheckIns = 10
                    },
                    new PointOfInterestAward
                    {
                        Name = "Haunted_PROP_HauntedHouse",
                        Type = "Track_Asset",
                        CheckIns = 100
                    },
                    new PointOfInterestAward
                    {
                        Name = "Haunted_SMK_Skeleton",
                        Type = "Track_Asset",
                        CheckIns = 250
                    },
                    new PointOfInterestAward
                    {
                        Name = "Haunted_PROP_SpiderWebTrampoline",
                        Type = "Track_Asset",
                        CheckIns = 400
                    },
                ]
            } },
            { 85, new PointOfInterest {
                Name = "Christ the Redeemer",
                CityId = 29,
                Latitude = -22.951667f,
                Longitude = -43.210833f,
                Radius = 0.1f,
                Awards =
                [
                    new PointOfInterestAward
                    {
                        Name = "Carnival_AI_Roller",
                        Type = "Track_Asset",
                        CheckIns = 10
                    },
                    new PointOfInterestAward
                    {
                        Name = "Carnival_PROP_GenericTent_01",
                        Type = "Track_Asset",
                        CheckIns = 100
                    },
                    new PointOfInterestAward
                    {
                        Name = "Carnival_PROP_BigTopTent",
                        Type = "Track_Asset",
                        CheckIns = 250
                    },
                    new PointOfInterestAward
                    {
                        Name = "Carnival_PROP_SaltNPepper",
                        Type = "Track_Asset",
                        CheckIns = 400
                    },
                ]
            } },
            { 86, new PointOfInterest {
                Name = "Fort Copacabana",
                CityId = 29,
                Latitude = -22.986763f,
                Longitude = -43.187674f,
                Radius = 0.13f,
                Awards =
                [
                    new PointOfInterestAward
                    {
                        Name = "Carnival_SMK_MilkBottle",
                        Type = "Track_Asset",
                        CheckIns = 10
                    },
                    new PointOfInterestAward
                    {
                        Name = "Carnival_PROP_GenericTent_02",
                        Type = "Track_Asset",
                        CheckIns = 100
                    },
                    new PointOfInterestAward
                    {
                        Name = "Carnival_PROP_GenericTent_03",
                        Type = "Track_Asset",
                        CheckIns = 250
                    },
                    new PointOfInterestAward
                    {
                        Name = "Carnival_AI_Carousel",
                        Type = "Track_Asset",
                        CheckIns = 400
                    },
                ]
            } },
            { 87, new PointOfInterest {
                Name = "National Library of Brazil",
                CityId = 29,
                Latitude = -22.912f,
                Longitude = -43.175f,
                Radius = 0.1f,
                Awards =
                [
                    new PointOfInterestAward
                    {
                        Name = "Carnival_PROP_GenericTent_04",
                        Type = "Track_Asset",
                        CheckIns = 10
                    },
                    new PointOfInterestAward
                    {
                        Name = "Carnival_SMK_TicketBooth_01",
                        Type = "Track_Asset",
                        CheckIns = 100
                    },
                    new PointOfInterestAward
                    {
                        Name = "Carnival_PROP_FunHouse",
                        Type = "Track_Asset",
                        CheckIns = 250
                    },
                    new PointOfInterestAward
                    {
                        Name = "Carnival_AI_Trampoline",
                        Type = "Track_Asset",
                        CheckIns = 400
                    },
                ]
            } },
            { 88, new PointOfInterest {
                Name = "Two Oceans Aquarium",
                CityId = 30,
                Latitude = -33.908056f,
                Longitude = 18.4175f,
                Radius = 0.13f,
                Awards =
                [
                    new PointOfInterestAward
                    {
                        Name = "Prehistoric_PROP_PlantPrehistoric01",
                        Type = "Track_Asset",
                        CheckIns = 10
                    },
                    new PointOfInterestAward
                    {
                        Name = "Prehistoric_PROP_StandingStone_3",
                        Type = "Track_Asset",
                        CheckIns = 50
                    },
                    new PointOfInterestAward
                    {
                        Name = "Prehistoric_PROP_PlantPrehistoric02",
                        Type = "Track_Asset",
                        CheckIns = 100
                    },
                    new PointOfInterestAward
                    {
                        Name = "Prehistoric_PROP_DinoRamp",
                        Type = "Track_Asset",
                        CheckIns = 150
                    },
                ]
            } },
            { 89, new PointOfInterest {
                Name = "Table Mountain National Park",
                CityId = 30,
                Latitude = -33.966667f,
                Longitude = 18.425f,
                Radius = 2.3f,
                Awards =
                [
                    new PointOfInterestAward
                    {
                        Name = "Prehistoric_PROP_FenceBone01",
                        Type = "Track_Asset",
                        CheckIns = 10
                    },
                    new PointOfInterestAward
                    {
                        Name = "Prehistoric_PROP_StandingStone_4",
                        Type = "Track_Asset",
                        CheckIns = 50
                    },
                    new PointOfInterestAward
                    {
                        Name = "Prehistoric_AI_TunnelRamp",
                        Type = "Track_Asset",
                        CheckIns = 100
                    },
                    new PointOfInterestAward
                    {
                        Name = "Prehistoric_AI_RampSkull",
                        Type = "Track_Asset",
                        CheckIns = 150
                    },
                ]
            } },
            { 90, new PointOfInterest {
                Name = "Victoria and Alfred Waterfront",
                CityId = 30,
                Latitude = -33.903056f,
                Longitude = 18.422778f,
                Radius = 0.1f,
                Awards =
                [
                    new PointOfInterestAward
                    {
                        Name = "Prehistoric_SMK_StoneTires_01",
                        Type = "Track_Asset",
                        CheckIns = 10
                    },
                    new PointOfInterestAward
                    {
                        Name = "Carnival_PROP_JumboDonut",
                        Type = "Track_Asset",
                        CheckIns = 50
                    },
                    new PointOfInterestAward
                    {
                        Name = "Carnival_AI_PopupClown",
                        Type = "Track_Asset",
                        CheckIns = 100
                    },
                    new PointOfInterestAward
                    {
                        Name = "Carnival_PROP_FerrisWheel",
                        Type = "Track_Asset",
                        CheckIns = 150
                    },
                ]
            } },
            { 91, new PointOfInterest {
                Name = "Grand Place",
                CityId = 31,
                Latitude = 50.8467f,
                Longitude = 4.3525f,
                Radius = 0.1f,
                Awards =
                [
                    new PointOfInterestAward
                    {
                        Name = "000004F5_STREETRATCHAR",
                        Type = "Character",
                        CheckIns = 10
                    },
                    new PointOfInterestAward
                    {
                        Name = "Part_Kart_Steering_Anti_Theft_Lock",
                        Type = "Kart_Asset",
                        CheckIns = 25
                    },
                    new PointOfInterestAward
                    {
                        Name = "PigeonStamp",
                        Type = "Character_Asset",
                        CheckIns = 50
                    },
                    new PointOfInterestAward
                    {
                        Name = "Hat_Homeless",
                        Type = "Character_Asset",
                        CheckIns = 100
                    },
                ]
            } },
            { 92, new PointOfInterest {
                Name = "The Atomium",
                CityId = 31,
                Latitude = 50.894722f,
                Longitude = 4.341111f,
                Radius = 0.1f,
                Awards =
                [
                    new PointOfInterestAward
                    {
                        Name = "Medieval_ARC_Tavern",
                        Type = "Track_Asset",
                        CheckIns = 10
                    },
                    new PointOfInterestAward
                    {
                        Name = "Medieval_PROP_Flags1",
                        Type = "Track_Asset",
                        CheckIns = 25
                    },
                    new PointOfInterestAward
                    {
                        Name = "Medieval_PROP_Banners1",
                        Type = "Track_Asset",
                        CheckIns = 50
                    },
                    new PointOfInterestAward
                    {
                        Name = "Medieval_SMK_ArcheryTarget01",
                        Type = "Track_Asset",
                        CheckIns = 100
                    },
                ]
            } },
            { 93, new PointOfInterest {
                Name = "Royal Museums of Fine Arts of Belgium",
                CityId = 31,
                Latitude = 50.841944f,
                Longitude = 4.357778f,
                Radius = 0.1f,
                Awards =
                [
                    new PointOfInterestAward
                    {
                        Name = "Medieval_PROP_Haymain",
                        Type = "Track_Asset",
                        CheckIns = 10
                    },
                    new PointOfInterestAward
                    {
                        Name = "Medieval_PROP_Banners2",
                        Type = "Track_Asset",
                        CheckIns = 25
                    },
                    new PointOfInterestAward
                    {
                        Name = "Medieval_PROP_Flags2",
                        Type = "Track_Asset",
                        CheckIns = 50
                    },
                    new PointOfInterestAward
                    {
                        Name = "Medieval_ARC_Drawbridge",
                        Type = "Track_Asset",
                        CheckIns = 100
                    },
                ]
            } },
            { 94, new PointOfInterest {
                Name = "Frederick's Church",
                CityId = 32,
                Latitude = 55.685f,
                Longitude = 12.589444f,
                Radius = 0.1f,
                Awards =
                [
                    new PointOfInterestAward
                    {
                        Name = "Christmas_PROP_XmasWreath",
                        Type = "Track_Asset",
                        CheckIns = 10
                    },
                    new PointOfInterestAward
                    {
                        Name = "Christmas_ARC_ChristmasHouse",
                        Type = "Track_Asset",
                        CheckIns = 25
                    },
                    new PointOfInterestAward
                    {
                        Name = "Christmas_SMK_GiftBoxes",
                        Type = "Track_Asset",
                        CheckIns = 50
                    },
                    new PointOfInterestAward
                    {
                        Name = "Christmas_PROP_TreeChristmas",
                        Type = "Track_Asset",
                        CheckIns = 100
                    },
                ]
            } },
            { 95, new PointOfInterest {
                Name = "Rosenborg Castle",
                CityId = 32,
                Latitude = 55.685556f,
                Longitude = 12.577778f,
                Radius = 0.14f,
                Awards =
                [
                    new PointOfInterestAward
                    {
                        Name = "Christmas_Prop_ToySoldier",
                        Type = "Track_Asset",
                        CheckIns = 10
                    },
                    new PointOfInterestAward
                    {
                        Name = "Medieval_PROP_Banners3",
                        Type = "Track_Asset",
                        CheckIns = 25
                    },
                    new PointOfInterestAward
                    {
                        Name = "Medieval_PROP_Flags3",
                        Type = "Track_Asset",
                        CheckIns = 50
                    },
                    new PointOfInterestAward
                    {
                        Name = "Medieval_ARC_Blacksmith_Medieval",
                        Type = "Track_Asset",
                        CheckIns = 100
                    },
                ]
            } },
            { 96, new PointOfInterest {
                Name = "Gefion Fountain",
                CityId = 32,
                Latitude = 55.689444f,
                Longitude = 12.5975f,
                Radius = 0.1f,
                Awards =
                [
                    new PointOfInterestAward
                    {
                        Name = "Christmas_SMK_Sleigh",
                        Type = "Track_Asset",
                        CheckIns = 10
                    },
                    new PointOfInterestAward
                    {
                        Name = "Christmas_PROP_TeddyBear",
                        Type = "Track_Asset",
                        CheckIns = 25
                    },
                    new PointOfInterestAward
                    {
                        Name = "Christmas_PROP_GiantCandycane",
                        Type = "Track_Asset",
                        CheckIns = 50
                    },
                    new PointOfInterestAward
                    {
                        Name = "Christmas_PROP_Reindeer",
                        Type = "Track_Asset",
                        CheckIns = 100
                    },
                ]
            } },
            { 97, new PointOfInterest {
                Name = "Helsinki Cathedral",
                CityId = 33,
                Latitude = 60.170278f,
                Longitude = 24.952222f,
                Radius = 0.12f,
                Awards =
                [
                    new PointOfInterestAward
                    {
                        Name = "000004CC_ARCTICCHICKENCHAR",
                        Type = "Character",
                        CheckIns = 10
                    },
                    new PointOfInterestAward
                    {
                        Name = "Part_Char_Shirt_Lumberjack",
                        Type = "Character_Asset",
                        CheckIns = 25
                    },
                    new PointOfInterestAward
                    {
                        Name = "Part_Kart_Seat_Construction",
                        Type = "Kart_Asset",
                        CheckIns = 50
                    },
                    new PointOfInterestAward
                    {
                        Name = "Mask_PolarBear",
                        Type = "Character_Asset",
                        CheckIns = 100
                    },
                ]
            } },
            { 98, new PointOfInterest {
                Name = "National Museum of Finland",
                CityId = 33,
                Latitude = 60.175f,
                Longitude = 24.931944f,
                Radius = 0.1f,
                Awards =
                [
                    new PointOfInterestAward
                    {
                        Name = "Christmas_PROP_SnowFort",
                        Type = "Track_Asset",
                        CheckIns = 10
                    },
                    new PointOfInterestAward
                    {
                        Name = "Christmas_SMK_Snowman",
                        Type = "Track_Asset",
                        CheckIns = 25
                    },
                    new PointOfInterestAward
                    {
                        Name = "Part_Kart_Ornament_Kart_Ornament74",
                        Type = "Kart_Asset",
                        CheckIns = 50
                    },
                    new PointOfInterestAward
                    {
                        Name = "Christmas_PROP_SnowballLauncher",
                        Type = "Track_Asset",
                        CheckIns = 100
                    },
                ]
            } },
            { 99, new PointOfInterest {
                Name = "Suomenlinna",
                CityId = 33,
                Latitude = 60.143611f,
                Longitude = 24.984444f,
                Radius = 1.5f,
                Awards =
                [
                    new PointOfInterestAward
                    {
                        Name = "Hat_Snowbank",
                        Type = "Character_Asset",
                        CheckIns = 10
                    },
                    new PointOfInterestAward
                    {
                        Name = "Shoulders_Snowbank",
                        Type = "Character_Asset",
                        CheckIns = 25
                    },
                    new PointOfInterestAward
                    {
                        Name = "Part_Kart_Ornament_Kart_Ornament75",
                        Type = "Kart_Asset",
                        CheckIns = 50
                    },
                    new PointOfInterestAward
                    {
                        Name = "Part_Kart_Engine_SnowThrower",
                        Type = "Kart_Asset",
                        CheckIns = 100
                    },
                ]
            } },
            { 100, new PointOfInterest {
                Name = "Spire of Dublin",
                CityId = 34,
                Latitude = 53.349722f,
                Longitude = -6.260278f,
                Radius = 0.1f,
                Awards =
                [
                    new PointOfInterestAward
                    {
                        Name = "Part_Char_Accessories_68",
                        Type = "Character_Asset",
                        CheckIns = 10
                    },
                    new PointOfInterestAward
                    {
                        Name = "Part_Char_Nose_Gopher",
                        Type = "Character_Asset",
                        CheckIns = 25
                    },
                    new PointOfInterestAward
                    {
                        Name = "000004C4_DLCCHAR9",
                        Type = "Character",
                        CheckIns = 50
                    },
                    new PointOfInterestAward
                    {
                        Name = "Part_Char_Shirt_Sweatervest",
                        Type = "Character_Asset",
                        CheckIns = 100
                    },
                ]
            } },
            { 101, new PointOfInterest {
                Name = "Dublin Castle",
                CityId = 34,
                Latitude = 53.343109f,
                Longitude = -6.267394f,
                Radius = 0.15f,
                Awards =
                [
                    new PointOfInterestAward
                    {
                        Name = "JunkYard_AI_GiantMagnet",
                        Type = "Track_Asset",
                        CheckIns = 10
                    },
                    new PointOfInterestAward
                    {
                        Name = "0000006F_DLCKART9",
                        Type = "Kart",
                        CheckIns = 25
                    },
                    new PointOfInterestAward
                    {
                        Name = "Part_Kart_Seat_GolfSeat",
                        Type = "Kart_Asset",
                        CheckIns = 50
                    },
                    new PointOfInterestAward
                    {
                        Name = "Part_Kart_Body_GolfKart",
                        Type = "Kart_Asset",
                        CheckIns = 100
                    },
                ]
            } },
            { 102, new PointOfInterest {
                Name = "St. Patrick's Cathedral",
                CityId = 34,
                Latitude = 53.339444f,
                Longitude = -6.271389f,
                Radius = 0.1f,
                Awards =
                [
                    new PointOfInterestAward
                    {
                        Name = "JunkYard_AI_Incinerator",
                        Type = "Track_Asset",
                        CheckIns = 10
                    },
                    new PointOfInterestAward
                    {
                        Name = "Progressive_SMK_AdvertizingHoardings",
                        Type = "Track_Asset",
                        CheckIns = 25
                    },
                    new PointOfInterestAward
                    {
                        Name = "Stunt_AI_StuntRamp_03",
                        Type = "Track_Asset",
                        CheckIns = 50
                    },
                    new PointOfInterestAward
                    {
                        Name = "Stunt_SMK_FireWall",
                        Type = "Track_Asset",
                        CheckIns = 100
                    },
                ]
            } },
            { 103, new PointOfInterest {
                Name = "Oceanographic Museum",
                CityId = 35,
                Latitude = 43.730833f,
                Longitude = 7.425278f,
                Radius = 0.1f,
                Awards =
                [
                    new PointOfInterestAward
                    {
                        Name = "000004C3_DLCCHAR8",
                        Type = "Character",
                        CheckIns = 5
                    },
                    new PointOfInterestAward
                    {
                        Name = "Part_Char_Glasses_9",
                        Type = "Character_Asset",
                        CheckIns = 10
                    },
                    new PointOfInterestAward
                    {
                        Name = "Part_Kart_Wheel_Bicycle",
                        Type = "Kart_Asset",
                        CheckIns = 25
                    },
                    new PointOfInterestAward
                    {
                        Name = "Part_Char_Accessories_65",
                        Type = "Character_Asset",
                        CheckIns = 50
                    },
                ]
            } },
            { 104, new PointOfInterest {
                Name = "Prince's Palace of Monaco",
                CityId = 35,
                Latitude = 43.731417f,
                Longitude = 7.420275f,
                Radius = 0.1f,
                Awards =
                [
                    new PointOfInterestAward
                    {
                        Name = "Part_Kart_Ornament_Kart_Ornament10",
                        Type = "Kart_Asset",
                        CheckIns = 5
                    },
                    new PointOfInterestAward
                    {
                        Name = "Part_Kart_Steering_Trash",
                        Type = "Kart_Asset",
                        CheckIns = 10
                    },
                    new PointOfInterestAward
                    {
                        Name = "0000006E_DLCKART8",
                        Type = "Kart",
                        CheckIns = 25
                    },
                    new PointOfInterestAward
                    {
                        Name = "Part_Kart_Body_Soapbox",
                        Type = "Kart_Asset",
                        CheckIns = 50
                    },
                ]
            } },
            { 105, new PointOfInterest {
                Name = "Saint Nicholas Cathedral",
                CityId = 35,
                Latitude = 43.73f,
                Longitude = 7.422222f,
                Radius = 0.1f,
                Awards =
                [
                    new PointOfInterestAward
                    {
                        Name = "Hat_Fastfood",
                        Type = "Character_Asset",
                        CheckIns = 5
                    },
                    new PointOfInterestAward
                    {
                        Name = "Medieval_PROP_Flags4",
                        Type = "Track_Asset",
                        CheckIns = 10
                    },
                    new PointOfInterestAward
                    {
                        Name = "Progressive_AI_ProgressiveRamp01",
                        Type = "Track_Asset",
                        CheckIns = 25
                    },
                    new PointOfInterestAward
                    {
                        Name = "Progressive_PROP_JumpThruHoop",
                        Type = "Track_Asset",
                        CheckIns = 50
                    },
                ]
            } },
            { 106, new PointOfInterest {
                Name = "Nobel Peace Center",
                CityId = 36,
                Latitude = 59.911483f,
                Longitude = 10.730481f,
                Radius = 0.1f,
                Awards =
                [
                    new PointOfInterestAward
                    {
                        Name = "Part_Kart_Wheel_Chain",
                        Type = "Kart_Asset",
                        CheckIns = 10
                    },
                    new PointOfInterestAward
                    {
                        Name = "Part_Char_Shoe_Bigboot",
                        Type = "Character_Asset",
                        CheckIns = 25
                    },
                    new PointOfInterestAward
                    {
                        Name = "Part_Kart_Steering_Airbag",
                        Type = "Kart_Asset",
                        CheckIns = 50
                    },
                    new PointOfInterestAward
                    {
                        Name = "Part_Kart_Body_Snowplow",
                        Type = "Kart_Asset",
                        CheckIns = 100
                    },
                ]
            } },
            { 107, new PointOfInterest {
                Name = "Norwegian Museum of Cultural History",
                CityId = 36,
                Latitude = 59.906911f,
                Longitude = 10.685958f,
                Radius = 0.1f,
                Awards =
                [
                    new PointOfInterestAward
                    {
                        Name = "000005E5_SKIBUNNYCHAR",
                        Type = "Character",
                        CheckIns = 10
                    },
                    new PointOfInterestAward
                    {
                        Name = "Pants_Ski",
                        Type = "Character_Asset",
                        CheckIns = 25
                    },
                    new PointOfInterestAward
                    {
                        Name = "Part_Char_hand_ski",
                        Type = "Character_Asset",
                        CheckIns = 50
                    },
                    new PointOfInterestAward
                    {
                        Name = "Part_Char_Shoe_Ski",
                        Type = "Character_Asset",
                        CheckIns = 75
                    },
                    new PointOfInterestAward
                    {
                        Name = "Part_Char_Jacket_Ski",
                        Type = "Character_Asset",
                        CheckIns = 100
                    },
                ]
            } },
            { 108, new PointOfInterest {
                Name = "Oslo Opera House",
                CityId = 36,
                Latitude = 59.906944f,
                Longitude = 10.753611f,
                Radius = 0.14f,
                Awards =
                [
                    new PointOfInterestAward
                    {
                        Name = "Hair_Frozen",
                        Type = "Character_Asset",
                        CheckIns = 10
                    },
                    new PointOfInterestAward
                    {
                        Name = "Part_Char_Jacket_Parka",
                        Type = "Character_Asset",
                        CheckIns = 25
                    },
                    new PointOfInterestAward
                    {
                        Name = "Part_Char_Hat_Parka",
                        Type = "Character_Asset",
                        CheckIns = 50
                    },
                    new PointOfInterestAward
                    {
                        Name = "Part_Char_IceBeard",
                        Type = "Character_Asset",
                        CheckIns = 100
                    },
                ]
            } },
            { 109, new PointOfInterest {
                Name = "Royal Castle in Warsaw",
                CityId = 37,
                Latitude = 52.247778f,
                Longitude = 21.014167f,
                Radius = 0.06f,
                Awards =
                [
                    new PointOfInterestAward
                    {
                        Name = "Part_Char_Accessories_78",
                        Type = "Character_Asset",
                        CheckIns = 10
                    },
                    new PointOfInterestAward
                    {
                        Name = "Part_Kart_Seat_BabyCarSeat",
                        Type = "Kart_Asset",
                        CheckIns = 25
                    },
                    new PointOfInterestAward
                    {
                        Name = "000004C8_DLCCHAR13",
                        Type = "Character",
                        CheckIns = 50
                    },
                    new PointOfInterestAward
                    {
                        Name = "Part_Char_Hand_6",
                        Type = "Character_Asset",
                        CheckIns = 100
                    },
                ]
            } },
            { 110, new PointOfInterest {
                Name = "St. John's Cathedral",
                CityId = 37,
                Latitude = 52.248889f,
                Longitude = 21.013611f,
                Radius = 0.05f,
                Awards =
                [
                    new PointOfInterestAward
                    {
                        Name = "00000073_DLCKART13",
                        Type = "Kart",
                        CheckIns = 10
                    },
                    new PointOfInterestAward
                    {
                        Name = "Part_Kart_Steering_Mirror",
                        Type = "Kart_Asset",
                        CheckIns = 25
                    },
                    new PointOfInterestAward
                    {
                        Name = "Part_Kart_Wheel_Carriage",
                        Type = "Kart_Asset",
                        CheckIns = 50
                    },
                    new PointOfInterestAward
                    {
                        Name = "Part_Kart_Body_Carriage",
                        Type = "Kart_Asset",
                        CheckIns = 100
                    },
                ]
            } },
            { 111, new PointOfInterest {
                Name = "Palace of Culture and Science",
                CityId = 37,
                Latitude = 52.231667f,
                Longitude = 21.006389f,
                Radius = 0.13f,
                Awards =
                [
                    new PointOfInterestAward
                    {
                        Name = "Part_Kart_Ornament_Kart_Ornament63",
                        Type = "Kart_Asset",
                        CheckIns = 5
                    },
                    new PointOfInterestAward
                    {
                        Name = "Part_Kart_Ornament_Kart_Ornament62",
                        Type = "Kart_Asset",
                        CheckIns = 10
                    },
                    new PointOfInterestAward
                    {
                        Name = "Part_Kart_Ornament_Kart_Ornament61",
                        Type = "Kart_Asset",
                        CheckIns = 25
                    },
                    new PointOfInterestAward
                    {
                        Name = "Part_Char_Gem_Oval",
                        Type = "Character_Asset",
                        CheckIns = 50
                    },
                    new PointOfInterestAward
                    {
                        Name = "Part_Char_Gem_Bar",
                        Type = "Character_Asset",
                        CheckIns = 75
                    },
                    new PointOfInterestAward
                    {
                        Name = "Part_Char_Gem_Diamond",
                        Type = "Character_Asset",
                        CheckIns = 100
                    },
                ]
            } },
            { 112, new PointOfInterest {
                Name = "Belem Tower",
                CityId = 38,
                Latitude = 38.691389f,
                Longitude = -9.215833f,
                Radius = 0.14f,
                Awards =
                [
                    new PointOfInterestAward
                    {
                        Name = "Pirate_ARC_GuardTower",
                        Type = "Track_Asset",
                        CheckIns = 10
                    },
                    new PointOfInterestAward
                    {
                        Name = "Pirate_SMK_Capstan01",
                        Type = "Track_Asset",
                        CheckIns = 25
                    },
                    new PointOfInterestAward
                    {
                        Name = "Pirate_SMK_Cannon",
                        Type = "Track_Asset",
                        CheckIns = 50
                    },
                    new PointOfInterestAward
                    {
                        Name = "Pirate_PROP_SubmergedGalleon",
                        Type = "Track_Asset",
                        CheckIns = 100
                    },
                ]
            } },
            { 113, new PointOfInterest {
                Name = "Rua Augusta Arch",
                CityId = 38,
                Latitude = 38.7084f,
                Longitude = -9.1368f,
                Radius = 0.1f,
                Awards =
                [
                    new PointOfInterestAward
                    {
                        Name = "Pirate_PROP_Galleon",
                        Type = "Track_Asset",
                        CheckIns = 10
                    },
                    new PointOfInterestAward
                    {
                        Name = "Pirate_SMK_RumBarrel01",
                        Type = "Track_Asset",
                        CheckIns = 25
                    },
                    new PointOfInterestAward
                    {
                        Name = "Pirate_PROP_HorseAndCart",
                        Type = "Track_Asset",
                        CheckIns = 50
                    },
                    new PointOfInterestAward
                    {
                        Name = "Pirate_PROP_CannonPlatform",
                        Type = "Track_Asset",
                        CheckIns = 100
                    },
                ]
            } },
            { 114, new PointOfInterest {
                Name = "Castle of Sao Jorge",
                CityId = 38,
                Latitude = 38.713889f,
                Longitude = -9.133611f,
                Radius = 0.15f,
                Awards =
                [
                    new PointOfInterestAward
                    {
                        Name = "Pirate_SMK_Crate01",
                        Type = "Track_Asset",
                        CheckIns = 10
                    },
                    new PointOfInterestAward
                    {
                        Name = "Pirate_PROP_PileOfShipsSupplies",
                        Type = "Track_Asset",
                        CheckIns = 25
                    },
                    new PointOfInterestAward
                    {
                        Name = "Pirate_PROP_ShireHorse",
                        Type = "Track_Asset",
                        CheckIns = 50
                    },
                    new PointOfInterestAward
                    {
                        Name = "Pirate_PROP_BritFrigate",
                        Type = "Track_Asset",
                        CheckIns = 100
                    },
                ]
            } },
            { 115, new PointOfInterest {
                Name = "Ericsson Globe",
                CityId = 39,
                Latitude = 59.293556f,
                Longitude = 18.083236f,
                Radius = 0.2f,
                Awards =
                [
                    new PointOfInterestAward
                    {
                        Name = "Alien_PROP_CrashedComet",
                        Type = "Track_Asset",
                        CheckIns = 10
                    },
                    new PointOfInterestAward
                    {
                        Name = "Alien_ARCTSP03_Observatory",
                        Type = "Track_Asset",
                        CheckIns = 25
                    },
                    new PointOfInterestAward
                    {
                        Name = "Part_Kart_Ornament_Kart_Ornament71",
                        Type = "Kart_Asset",
                        CheckIns = 50
                    },
                    new PointOfInterestAward
                    {
                        Name = "Alien_PROP_Jupiter",
                        Type = "Track_Asset",
                        CheckIns = 100
                    },
                ]
            } },
            { 116, new PointOfInterest {
                Name = "Earth",
                CityId = 39,
                Latitude = 59.369022f,
                Longitude = 18.053428f,
                Radius = 0.1f,
                Awards =
                [
                    new PointOfInterestAward
                    {
                        Name = "MoonColony_PROP_MoonRock",
                        Type = "Track_Asset",
                        CheckIns = 10
                    },
                    new PointOfInterestAward
                    {
                        Name = "Alien_Prop_CrashedComet02",
                        Type = "Track_Asset",
                        CheckIns = 25
                    },
                    new PointOfInterestAward
                    {
                        Name = "Part_Kart_Ornament_Kart_Ornament73",
                        Type = "Kart_Asset",
                        CheckIns = 50
                    },
                    new PointOfInterestAward
                    {
                        Name = "Alien_PROP_Saturn",
                        Type = "Track_Asset",
                        CheckIns = 100
                    },
                ]
            } },
            { 117, new PointOfInterest {
                Name = "Mercury",
                CityId = 39,
                Latitude = 59.319722f,
                Longitude = 18.070556f,
                Radius = 0.05f,
                Awards =
                [
                    new PointOfInterestAward
                    {
                        Name = "Alien_Prop_CrashedComet03",
                        Type = "Track_Asset",
                        CheckIns = 10
                    },
                    new PointOfInterestAward
                    {
                        Name = "Part_Kart_Seat_SnowBlock",
                        Type = "Kart_Asset",
                        CheckIns = 25
                    },
                    new PointOfInterestAward
                    {
                        Name = "Part_Kart_Steering_Ice",
                        Type = "Kart_Asset",
                        CheckIns = 50
                    },
                    new PointOfInterestAward
                    {
                        Name = "Alien_PROP_Mars",
                        Type = "Track_Asset",
                        CheckIns = 100
                    },
                ]
            } },
            { 118, new PointOfInterest {
                Name = "Galata Tower",
                CityId = 40,
                Latitude = 41.025556f,
                Longitude = 28.974167f,
                Radius = 0.1f,
                Awards =
                [
                    new PointOfInterestAward
                    {
                        Name = "Stunt_PROP_DoubleDecker",
                        Type = "Track_Asset",
                        CheckIns = 10
                    },
                    new PointOfInterestAward
                    {
                        Name = "JunkYard_PROP_TirePile",
                        Type = "Track_Asset",
                        CheckIns = 100
                    },
                    new PointOfInterestAward
                    {
                        Name = "JunkYard_PROP_TPP8_OfficeShack",
                        Type = "Track_Asset",
                        CheckIns = 200
                    },
                    new PointOfInterestAward
                    {
                        Name = "JunkYard_AI_ConveyorBeltRamp",
                        Type = "Track_Asset",
                        CheckIns = 300
                    },
                ]
            } },
            { 119, new PointOfInterest {
                Name = "Topkapi Palace",
                CityId = 40,
                Latitude = 41.013f,
                Longitude = 28.984f,
                Radius = 0.7f,
                Awards =
                [
                    new PointOfInterestAward
                    {
                        Name = "Stunt_SMK_StuntTrailer_01",
                        Type = "Track_Asset",
                        CheckIns = 10
                    },
                    new PointOfInterestAward
                    {
                        Name = "Stunt_AI_StuntRamp_02",
                        Type = "Track_Asset",
                        CheckIns = 100
                    },
                    new PointOfInterestAward
                    {
                        Name = "Stunt_SMK_Tires_01",
                        Type = "Track_Asset",
                        CheckIns = 200
                    },
                    new PointOfInterestAward
                    {
                        Name = "Stunt_PROP_Robosaurus",
                        Type = "Track_Asset",
                        CheckIns = 300
                    },
                ]
            } },
            { 120, new PointOfInterest {
                Name = "Hagia Sophia",
                CityId = 40,
                Latitude = 41.008611f,
                Longitude = 28.98f,
                Radius = 0.1f,
                Awards =
                [
                    new PointOfInterestAward
                    {
                        Name = "JunkYard_AI_Compactor",
                        Type = "Track_Asset",
                        CheckIns = 10
                    },
                    new PointOfInterestAward
                    {
                        Name = "JunkYard_PROP_CrushedCube",
                        Type = "Track_Asset",
                        CheckIns = 100
                    },
                    new PointOfInterestAward
                    {
                        Name = "JunkYard_SMK_TPP8_WreckedCarPile",
                        Type = "Track_Asset",
                        CheckIns = 200
                    },
                    new PointOfInterestAward
                    {
                        Name = "JunkYard_AI_CarCrusher",
                        Type = "Track_Asset",
                        CheckIns = 300
                    },
                ]
            } },
            { 121, new PointOfInterest {
                Name = "Alexander Column",
                CityId = 41,
                Latitude = 59.939167f,
                Longitude = 30.315833f,
                Radius = 0.1f,
                Awards =
                [
                    new PointOfInterestAward
                    {
                        Name = "Medieval_PROP_AssaultTower",
                        Type = "Track_Asset",
                        CheckIns = 10
                    },
                    new PointOfInterestAward
                    {
                        Name = "Medieval_PROP_Banners4",
                        Type = "Track_Asset",
                        CheckIns = 50
                    },
                    new PointOfInterestAward
                    {
                        Name = "Medieval_PROP_StockPlatform",
                        Type = "Track_Asset",
                        CheckIns = 100
                    },
                    new PointOfInterestAward
                    {
                        Name = "Medieval_SMK_Trebuchet",
                        Type = "Track_Asset",
                        CheckIns = 200
                    },
                ]
            } },
            { 122, new PointOfInterest {
                Name = "Bronze Horseman",
                CityId = 41,
                Latitude = 59.9364f,
                Longitude = 30.3022f,
                Radius = 0.1f,
                Awards =
                [
                    new PointOfInterestAward
                    {
                        Name = "Medieval_PROP_BatteringRam",
                        Type = "Track_Asset",
                        CheckIns = 10
                    },
                    new PointOfInterestAward
                    {
                        Name = "Alien_PROP_MilitaryTank01",
                        Type = "Track_Asset",
                        CheckIns = 50
                    },
                    new PointOfInterestAward
                    {
                        Name = "Alien_PROP_MilitaryTruck01",
                        Type = "Track_Asset",
                        CheckIns = 100
                    },
                    new PointOfInterestAward
                    {
                        Name = "Alien_PROP_AlienBarrelLauncher",
                        Type = "Track_Asset",
                        CheckIns = 200
                    },
                ]
            } },
            { 123, new PointOfInterest {
                Name = "Winter Palace",
                CityId = 41,
                Latitude = 59.9404f,
                Longitude = 30.3139f,
                Radius = 0.14f,
                Awards =
                [
                    new PointOfInterestAward
                    {
                        Name = "Part_Kart_Steering_Plastic_Wrapped",
                        Type = "Kart_Asset",
                        CheckIns = 10
                    },
                    new PointOfInterestAward
                    {
                        Name = "000004E9_ICEQUEENCHAR",
                        Type = "Character",
                        CheckIns = 50
                    },
                    new PointOfInterestAward
                    {
                        Name = "Hair_Queen",
                        Type = "Character_Asset",
                        CheckIns = 100
                    },
                    new PointOfInterestAward
                    {
                        Name = "IceSkin",
                        Type = "Character_Asset",
                        CheckIns = 200
                    },
                ]
            } },
            { 124, new PointOfInterest {
                Name = "Shrine of Remembrance",
                CityId = 42,
                Latitude = -37.830434f,
                Longitude = 144.973258f,
                Radius = 0.38f,
                Awards =
                [
                    new PointOfInterestAward
                    {
                        Name = "Western_PROP_FenceCoral01",
                        Type = "Track_Asset",
                        CheckIns = 10
                    },
                    new PointOfInterestAward
                    {
                        Name = "Western_PROP_FenceCoral01End",
                        Type = "Track_Asset",
                        CheckIns = 50
                    },
                    new PointOfInterestAward
                    {
                        Name = "Progressive_SMK_TPP11_Box3",
                        Type = "Track_Asset",
                        CheckIns = 100
                    },
                    new PointOfInterestAward
                    {
                        Name = "Western_PROP_Horse",
                        Type = "Track_Asset",
                        CheckIns = 150
                    },
                ]
            } },
            { 125, new PointOfInterest {
                Name = "Melbourne Museum",
                CityId = 42,
                Latitude = -37.803337f,
                Longitude = 144.971445f,
                Radius = 0.15f,
                Awards =
                [
                    new PointOfInterestAward
                    {
                        Name = "Western_SMK_TNTCrate",
                        Type = "Track_Asset",
                        CheckIns = 10
                    },
                    new PointOfInterestAward
                    {
                        Name = "WaterWorld_PROP_PlasticBuoy",
                        Type = "Track_Asset",
                        CheckIns = 50
                    },
                    new PointOfInterestAward
                    {
                        Name = "WaterWorld_PROP_TPP7_SupportPlatform",
                        Type = "Track_Asset",
                        CheckIns = 100
                    },
                    new PointOfInterestAward
                    {
                        Name = "Western_SMK_TNTBarrel",
                        Type = "Track_Asset",
                        CheckIns = 150
                    },
                ]
            } },
            { 126, new PointOfInterest {
                Name = "Eureka Tower",
                CityId = 42,
                Latitude = -37.821667f,
                Longitude = 144.964444f,
                Radius = 0.1f,
                Awards =
                [
                    new PointOfInterestAward
                    {
                        Name = "WaterWorld_PROP_Trampoline",
                        Type = "Track_Asset",
                        CheckIns = 10
                    },
                    new PointOfInterestAward
                    {
                        Name = "WaterWorld_PROP_InflatableToy",
                        Type = "Track_Asset",
                        CheckIns = 50
                    },
                    new PointOfInterestAward
                    {
                        Name = "WaterWorld_PROP_RockingPlatform",
                        Type = "Track_Asset",
                        CheckIns = 100
                    },
                    new PointOfInterestAward
                    {
                        Name = "WaterWorld_PROP_Logroll01",
                        Type = "Track_Asset",
                        CheckIns = 150
                    },
                ]
            } },
            { 127, new PointOfInterest {
                Name = "Museum Campus Chicago",
                CityId = 43,
                Latitude = 41.8665f,
                Longitude = -87.6141f,
                Radius = 0.43f,
                Awards =
                [
                    new PointOfInterestAward
                    {
                        Name = "Part_Kart_Antenna_GiantHand",
                        Type = "Kart_Asset",
                        CheckIns = 10
                    },
                    new PointOfInterestAward
                    {
                        Name = "Part_Kart_Seat_LawnChair",
                        Type = "Kart_Asset",
                        CheckIns = 100
                    },
                    new PointOfInterestAward
                    {
                        Name = "0000006C_DLCKART6",
                        Type = "Kart",
                        CheckIns = 200
                    },
                    new PointOfInterestAward
                    {
                        Name = "Part_Kart_Body_RV",
                        Type = "Kart_Asset",
                        CheckIns = 350
                    },
                ]
            } },
            { 128, new PointOfInterest {
                Name = "Buckingham Fountain",
                CityId = 43,
                Latitude = 41.875792f,
                Longitude = -87.618944f,
                Radius = 0.1f,
                Awards =
                [
                    new PointOfInterestAward
                    {
                        Name = "MoonColony_PROP_SolarRamp",
                        Type = "Track_Asset",
                        CheckIns = 10
                    },
                    new PointOfInterestAward
                    {
                        Name = "Western_PROP_WaterTower",
                        Type = "Track_Asset",
                        CheckIns = 100
                    },
                    new PointOfInterestAward
                    {
                        Name = "Western_SMK_Piano",
                        Type = "Track_Asset",
                        CheckIns = 200
                    },
                    new PointOfInterestAward
                    {
                        Name = "MoonColony_AI_SolarPanel",
                        Type = "Track_Asset",
                        CheckIns = 350
                    },
                ]
            } },
            { 129, new PointOfInterest {
                Name = "Navy Pier",
                CityId = 43,
                Latitude = 41.891389f,
                Longitude = -87.599722f,
                Radius = 0.5f,
                Awards =
                [
                    new PointOfInterestAward
                    {
                        Name = "000004C1_DLCCHAR6",
                        Type = "Character",
                        CheckIns = 10
                    },
                    new PointOfInterestAward
                    {
                        Name = "Football_Pads",
                        Type = "Character_Asset",
                        CheckIns = 100
                    },
                    new PointOfInterestAward
                    {
                        Name = "Part_Char_Meltonleather",
                        Type = "Character_Asset",
                        CheckIns = 200
                    },
                    new PointOfInterestAward
                    {
                        Name = "Part_Char_Accessories_9",
                        Type = "Character_Asset",
                        CheckIns = 350
                    },
                ]
            } },
            { 130, new PointOfInterest {
                Name = "Space Center Houston",
                CityId = 44,
                Latitude = 29.551881f,
                Longitude = -95.098343f,
                Radius = 0.2f,
                Awards =
                [
                    new PointOfInterestAward
                    {
                        Name = "MoonColony_PROP_TPP5_MoonRover",
                        Type = "Track_Asset",
                        CheckIns = 10
                    },
                    new PointOfInterestAward
                    {
                        Name = "MoonColony_PROP_LaunchPad",
                        Type = "Track_Asset",
                        CheckIns = 75
                    },
                    new PointOfInterestAward
                    {
                        Name = "MoonColony_PROP_LandingCraft",
                        Type = "Track_Asset",
                        CheckIns = 150
                    },
                    new PointOfInterestAward
                    {
                        Name = "Alien_PROP_RocketLaunchPad",
                        Type = "Track_Asset",
                        CheckIns = 250
                    },
                ]
            } },
            { 131, new PointOfInterest {
                Name = "Hermann Park",
                CityId = 44,
                Latitude = 29.721111f,
                Longitude = -95.391389f,
                Radius = 1.15f,
                Awards =
                [
                    new PointOfInterestAward
                    {
                        Name = "Western_AI_CartRamp01",
                        Type = "Track_Asset",
                        CheckIns = 10
                    },
                    new PointOfInterestAward
                    {
                        Name = "Western_PROP_Goldmine",
                        Type = "Track_Asset",
                        CheckIns = 75
                    },
                    new PointOfInterestAward
                    {
                        Name = "Western_SMK_BarrelWater",
                        Type = "Track_Asset",
                        CheckIns = 150
                    },
                    new PointOfInterestAward
                    {
                        Name = "Western_PROP_WindMill",
                        Type = "Track_Asset",
                        CheckIns = 250
                    },
                ]
            } },
            { 132, new PointOfInterest {
                Name = "Houston Arboretum and Nature Center",
                CityId = 44,
                Latitude = 29.7652f,
                Longitude = -95.452f,
                Radius = 0.2f,
                Awards =
                [
                    new PointOfInterestAward
                    {
                        Name = "Western_PROP_CactusWest",
                        Type = "Track_Asset",
                        CheckIns = 10
                    },
                    new PointOfInterestAward
                    {
                        Name = "Western_PROP_RockFormationA",
                        Type = "Track_Asset",
                        CheckIns = 75
                    },
                    new PointOfInterestAward
                    {
                        Name = "Western_PROP_RockFormationB",
                        Type = "Track_Asset",
                        CheckIns = 150
                    },
                    new PointOfInterestAward
                    {
                        Name = "Western_PROP_TreeJoshua",
                        Type = "Track_Asset",
                        CheckIns = 250
                    },
                ]
            } },
            { 133, new PointOfInterest {
                Name = "The Sixth Floor Museum",
                CityId = 45,
                Latitude = 32.779722f,
                Longitude = -96.808333f,
                Radius = 0.1f,
                Awards =
                [
                    new PointOfInterestAward
                    {
                        Name = "Western_PROP_FortGate",
                        Type = "Track_Asset",
                        CheckIns = 10
                    },
                    new PointOfInterestAward
                    {
                        Name = "Western_PROP_FortTower",
                        Type = "Track_Asset",
                        CheckIns = 100
                    },
                    new PointOfInterestAward
                    {
                        Name = "Western_PROP_FortWall",
                        Type = "Track_Asset",
                        CheckIns = 200
                    },
                    new PointOfInterestAward
                    {
                        Name = "Western_PROP_FortRamp",
                        Type = "Track_Asset",
                        CheckIns = 300
                    },
                ]
            } },
            { 134, new PointOfInterest {
                Name = "Reunion Tower",
                CityId = 45,
                Latitude = 32.7753f,
                Longitude = -96.8089f,
                Radius = 0.14f,
                Awards =
                [
                    new PointOfInterestAward
                    {
                        Name = "00000004_DLCKART1",
                        Type = "Kart",
                        CheckIns = 10
                    },
                    new PointOfInterestAward
                    {
                        Name = "Part_Kart_Seat_Saddle",
                        Type = "Kart_Asset",
                        CheckIns = 100
                    },
                    new PointOfInterestAward
                    {
                        Name = "Part_Kart_Steering_Rope",
                        Type = "Kart_Asset",
                        CheckIns = 200
                    },
                    new PointOfInterestAward
                    {
                        Name = "Part_Kart_Body_OffRoad5",
                        Type = "Kart_Asset",
                        CheckIns = 300
                    },
                ]
            } },
            { 135, new PointOfInterest {
                Name = "Bank of America Plaza",
                CityId = 45,
                Latitude = 32.7799f,
                Longitude = -96.8038f,
                Radius = 0.1f,
                Awards =
                [
                    new PointOfInterestAward
                    {
                        Name = "Part_Char_Pants_chaps",
                        Type = "Character_Asset",
                        CheckIns = 10
                    },
                    new PointOfInterestAward
                    {
                        Name = "Part_Char_Straw",
                        Type = "Character_Asset",
                        CheckIns = 50
                    },
                    new PointOfInterestAward
                    {
                        Name = "Part_Char_Shoe_Cowboy",
                        Type = "Character_Asset",
                        CheckIns = 100
                    },
                    new PointOfInterestAward
                    {
                        Name = "000004BC_DLCCHAR1",
                        Type = "Character",
                        CheckIns = 200
                    },
                    new PointOfInterestAward
                    {
                        Name = "Part_Char_Accessories_63",
                        Type = "Character_Asset",
                        CheckIns = 300
                    },
                ]
            } },
        };
        public List<ModExplorerTravelAward> TravelAwards =
        [
            new ModExplorerTravelAward
            {
                Name = "Explorer_PROP_EasterIslandMoai",
                Type = "Track_Asset",
                Points = 4,
                IsGlobalPoints = false
            },
            new ModExplorerTravelAward
            {
                Name = "Explorer_ARC_Alamo",
                Type = "Track_Asset",
                Points = 50,
                IsGlobalPoints = false
            },
            new ModExplorerTravelAward
            {
                Name = "Explorer_PROP_Sphinx",
                Type = "Track_Asset",
                Points = 100,
                IsGlobalPoints = false
            },
            new ModExplorerTravelAward
            {
                Name = "Explorer_ARC_BigBen",
                Type = "Track_Asset",
                Points = 500,
                IsGlobalPoints = false
            },
            new ModExplorerTravelAward
            {
                Name = "Explorer_ARC_SpaceNeedle",
                Type = "Track_Asset",
                Points = 1000,
                IsGlobalPoints = false
            },
            new ModExplorerTravelAward
            {
                Name = "Explorer_ARC_TowerPisa",
                Type = "Track_Asset",
                Points = 1500,
                IsGlobalPoints = false
            },
            new ModExplorerTravelAward
            {
                Name = "Explorer_PROP_StatueLiberty",
                Type = "Track_Asset",
                Points = 2000,
                IsGlobalPoints = false
            },
            new ModExplorerTravelAward
            {
                Name = "Explorer_ARC_ElCastilloChichenItza",
                Type = "Track_Asset",
                Points = 100000000,
                IsGlobalPoints = true
            },
            new ModExplorerTravelAward
            {
                Name = "Explorer_ARC_WhiteHouse",
                Type = "Track_Asset",
                Points = 150000000,
                IsGlobalPoints = true
            },
            new ModExplorerTravelAward
            {
                Name = "Explorer_ARC_TajMahal",
                Type = "Track_Asset",
                Points = 200000000,
                IsGlobalPoints = true
            },
            new ModExplorerTravelAward
            {
                Name = "Explorer_ARC_HimejiCastle",
                Type = "Track_Asset",
                Points = 250000000,
                IsGlobalPoints = true
            },
            new ModExplorerTravelAward
            {
                Name = "Explorer_ARC_BasilsCathedral",
                Type = "Track_Asset",
                Points = 300000000,
                IsGlobalPoints = true
            },
            new ModExplorerTravelAward
            {
                Name = "Explorer_ARC_PyramidGiza",
                Type = "Track_Asset",
                Points = 350000000,
                IsGlobalPoints = true
            },
            new ModExplorerTravelAward
            {
                Name = "Explorer_ARC_SydneyOperaHouse",
                Type = "Track_Asset",
                Points = 400000000,
                IsGlobalPoints = true
            },
            new ModExplorerTravelAward
            {
                Name = "Explorer_ARC_Colosseum",
                Type = "Track_Asset",
                Points = 500000000,
                IsGlobalPoints = true
            }
        ];

        public static ModMileConfig GetFromFile()
        {
            ModMileConfig config = new();
            if (File.Exists("./mod_explorer.json"))
            {
                string file = File.ReadAllText("./mod_explorer.json");
                config = JsonConvert.DeserializeObject<ModMileConfig>(file);
            }
            else
                File.WriteAllText("./mod_explorer.json", JsonConvert.SerializeObject(config, Formatting.Indented));
            
            return config;
        }
    }
}