using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assist.Objects.Helpers
{
    internal class MapNames
    {
        public static Dictionary<string, string> MapsById = new Dictionary<string, string>
        {
            {"7eaecc1b-4337-bbf6-6ab9-04b8f06b3319","Ascent"},
            {"d960549e-485c-e861-8d71-aa9d1aed12a2","Split"},
            {"b529448b-4d60-346e-e89e-00a4c527a405","Fracture"},
            {"2c9d57ec-4431-9c5e-2939-8f9ef6dd5cba","Bind"},
            {"2fb9a4fd-47b8-4e7d-a969-74b4046ebd53","Breeze"},
            {"fd267378-4d1d-484f-ff52-77821ed10dc2","Pearl"},
            {"e2ad5c54-4114-a870-9641-8ea21279579a","Icebox"},
            {"9c91a445-4f78-1baa-a3ea-8f8aadf4914d","Lotus"},
            {"ee613ee9-28b7-4beb-9666-08db13bb2244","The Range"},
            {"2bee0dc9-4ffe-519b-1cbd-7fbe763a6047","Haven"},
        };

        public static Dictionary<string, string> MapsByPath = new Dictionary<string, string>
        {
            {"/game/maps/ascent/ascent","Ascent"},
            {"/game/maps/bonsai/bonsai","Split"},
            {"/game/maps/canyon/canyon","Fracture"},
            {"/game/maps/duality/duality","Bind"},
            {"/game/maps/foxtrot/foxtrot","Breeze"},
            {"/game/maps/foxtrot_Copy/foxtrot_Copy","Breeze"},
            {"/game/maps/pitt/pitt","Pearl"},
            {"/game/maps/port/port","Icebox"},
            {"/game/maps/poveglia/range","The Range"},
            {"/game/maps/triad/triad","Haven"},
            {"/game/maps/jam/jam","Lotus"},
        };
    }
}

